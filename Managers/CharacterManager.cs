using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Dapper;

namespace XRpg
{
	//==========================================================================
	// CharacterManager
	// • Manages all data-related RPG tasks, e.g. loading stat info
	//==========================================================================
	public class CharacterManager
	{
		// These are here to be able to cache results...
		private Dictionary<int, CharacterBase> _dictCharactersAll;

		private Dictionary<int, Stat> _dictAttribsAll;
		private Dictionary<int, Stat> _dictSkillsAll;
		private Dictionary<int, Stat> _dictSpecialStatsAll;

		const float c_fAttrLvlBase = 50;
		const float c_fAttrLvlCap = 100;

		static SQLiteConnection _sqlConn;

		static SQLiteConnection SQLConn
		{
			get { return _sqlConn; }
		}


		public CharacterManager()
		{
		}


		//TODO: MOVE OUT TO SEPARATE FILE
		//----------------------------------------------------------------------
		public bool InitDB()
		{
			//TODO: Load DB filename from config...?
			const string strDataPath = @".\Data";
			const string strDbFile = "xrpg.sqlite";

			if ( !Directory.Exists( strDataPath ) )
			{
				Directory.CreateDirectory( strDataPath );
			}

			string strDbFilePath = strDataPath + '\\' + strDbFile;

			// Does the DB exist...?
			if ( !File.Exists( strDbFilePath ) )
			{
				// ...Nope; create it.
				SQLiteConnection.CreateFile( strDbFilePath );
				bool bSuccess = CreateCharacterTables();

				if ( !bSuccess ) return false;
			}

			//NOTE: If connection pooling required, add "Pooling=True;Max Pool Size=100;"...
			string strDBSource = $@"Data Source={strDbFilePath};version=3;";

			try
			{
				_sqlConn = new SQLiteConnection( strDBSource );
				_sqlConn.Open();
			}
			catch ( Exception e )
			{
				// Most likely issue here; sqlite.interop.dll is not where it's supposed to be.

				throw new ApplicationException( e.Message, e);
			}

			return true;
		}


		//TODO: MOVE OUT
		//----------------------------------------------------------------------
		bool CreateCharacterTables()
		{
			//TODO: Put SQL to create tables in here
			return true;
		}


		//----------------------------------------------------------------------
		public CharacterBase LoadCharacterById( uint Id )
		{
			Debug.Assert( _sqlConn != null );

			const string sql =
				@"SELECT * FROM Characters
				WHERE Characters.Id = @charId";

			CharacterBase character = _sqlConn.Query<CharacterBase>(
				sql,
				new { @charId = Id } ).FirstOrDefault();

			return character;
		}


		//----------------------------------------------------------------------
		public List<CharacterBase> LoadAllCharacters( CharacterType charType = CharacterType.Player )
		{
			Debug.Assert( _sqlConn != null );

			const string sql =
				@"SELECT * FROM Characters
				WHERE Characters.Type = @type";

			List<CharacterBase> listChars = _sqlConn.Query<CharacterBase>(
				sql,
				new { @type = (int)charType } ).ToList();


			return listChars;
		}


		//----------------------------------------------------------------------
		public  Dictionary<int, StatBase> LoadAllStats()
		{
			const string sqlStats =
				@"SELECT
				*
				FROM StatsBase";
				//WHERE StatsBase.Type IN @types";

			var dictStats = _sqlConn.Query<StatBase>( sqlStats )
				.ToDictionary( stat => stat.Id, stat => stat );

			//TODO: REMOVE? Attribute modifiers would probably be on a per-character basis
			// ----- Now fetch modifiers for each attribute...
			const string sqlMods =
				@"SELECT
					StatsBase.Id
				FROM StatsBase
				JOIN StatModifiers_map AS mods ON mods.StatId = StatsBase.Id
				WHERE mods.TargetStatId = @statId";

			//TODO: Fetch modifiers individually for now...
			foreach( var kvp in dictStats )
			{
				//kvp.Value.listModIds = new List<int>();

				kvp.Value.listModIds =
					_sqlConn.Query<int>(
					sqlMods,
					new
					{
						@statId = kvp.Key
					} ).ToList();
			}

			return dictStats;
		}


		//----------------------------------------------------------------------
		public Dictionary<int, StatType> LoadStatTypes()
		{
			const string sqlStats =
				@"SELECT
				*
				FROM StatTypes";
				//WHERE StatsBase.Type IN @types";

			var dictStatTypes = _sqlConn.Query<StatType>( sqlStats )
				.ToDictionary( statType => statType.Id, statType => statType );

			return dictStatTypes;
		}


		//----------------------------------------------------------------------
		// LoadCharacterStats()
		// • id = character ID
		// • type = physical, mental, etc.
		//----------------------------------------------------------------------
		public List<Stat> LoadCharacterStats( uint characterId, uint typeId = 0 )
		{
			Debug.Assert( characterId > 0 );

			string sql =
				@"SELECT
					StatsBase.Id,
					StatsBase.Name,
					StatsBase.Abbr,
					StatsBase.Description,
					StatsBase.TypeId,
					StatTypes.Name AS TypeName,
					CharacterStats.Level,
					CharacterStats.XpToNextLvl
				FROM CharacterStats
				JOIN StatsBase ON StatsBase.Id = CharacterStats.StatId
				JOIN StatTypes ON StatTypes.Id = StatsBase.TypeId
				WHERE CharacterStats.CharacterId = @charId";

			if ( typeId != 0 )
				sql += @" AND StatsBase.TypeId = @typeId";

			var listStats = _sqlConn.Query<Stat>(
				sql,
				new
				{
					@charId = characterId,
					@typeId = typeId,
				}  )
				.ToList();

			return listStats;
		}


		//TODO: UPDATE TO SAVE ALL STATS
		//----------------------------------------------------------------------
		public int SaveMasterAttribs( List<Stat> listNewAttribs )
		{
			Debug.Assert( listNewAttribs != null );

			int result;

			using ( var transact = _sqlConn.BeginTransaction() )
			{
				try
				{
					//Note: Id is auto-increment; don't set it here...
					const string sql =
						@"-- Try to update a current record first...
						UPDATE StatsBase
						SET
							Name         = @Name,
							Abbr         = @Abbr,
							Description  = @Description,
							TypeId       = @Type,
							LevelMin     = @LevelMin,
							LevelBase    = @LevelBase,
							LevelCap     = @LevelCap,
							Modifier     = @Modifier,
							XPCost       = @XPCost,
							ApplyType    = @ApplyType,
							ApplyHow     = @ApplyHow,
							ApplyPercent = @ApplyPercent
						WHERE Id = @Id;

						-- If already in table, ignore; otherwise, insert the new record...
						INSERT OR IGNORE INTO StatsBase
						VALUES
						(
							@Name,
							@Abbr,
							@Description,
							@Type,
							@LevelMin,
							@LevelBase,
							@LevelCap,
							@Modifier,
							@XPCost,
							@ApplyType,
							@ApplyHow,
							@ApplyPercent
						);";

					//TODO: Need to save modifiers too

					result =  _sqlConn.Execute( sql, listNewAttribs );

					transact.Commit();
				}
				catch ( Exception ex )
				{
					transact.Rollback();
					throw new Exception( ex.Message );
				}
			}

			return result;
		}


		//----------------------------------------------------------------------
		public void ClearAttribs()
		{
			_dictAttribsAll.Clear();
		}


		//----------------------------------------------------------------------
		public void ClearSkills()
		{
			_dictSkillsAll.Clear();
		}


		//----------------------------------------------------------------------
		public List<Species> GetSpeciesList()
		{
			const string sql =
				@"SELECT
				*
				FROM Species";

			var list = _sqlConn.Query<Species>( sql ).ToList();
			return list;
		}


		//----------------------------------------------------------------------
		public List<Profession> GetProfessionList()
		{
			const string sql =
				@"SELECT
				*
				FROM Professions";

			var list = _sqlConn.Query<Profession>( sql ).ToList();
			return list;
		}


		//----------------------------------------------------------------------
		public List<StatType> GetStatTypeList()
		{
			const string sql =
				@"SELECT
				*
				FROM StatTypes";

			var list = _sqlConn.Query<StatType>( sql ).ToList();
			return list;
		}
	}
}
