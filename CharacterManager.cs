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
using Xrpg.Core;


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

		private Dictionary<int, StatBase> _dictAttribsAll;
		private Dictionary<int, StatBase> _dictSkillsAll;
		private Dictionary<int, StatBase> _dictSpecialStatsAll;

		const float c_fAttrLvlBase = 50;
		const float c_fAttrLvlCap = 100;

		static SQLiteConnection _sqlConn;

		//TODO: probably don't want a public SQL connection... remove
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
			//TO DO: Load DB filename from config...?
			const string strDataPath = @"Data";
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
			string strDBSource =
				string.Format( @"Data Source={0};version=3;", strDbFilePath );

			try
			{
				_sqlConn = new SQLiteConnection( strDBSource );
				_sqlConn.Open();
			}
			catch ( Exception e )
			{
				// Most likely issue here; sqlite.interop.dll is not where it's supposed to be.

				throw new ApplicationException( e.Message, e);
				return false;
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
		public List<CharacterBase> LoadCharacters( CharacterType charType = CharacterType.Player )
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
				JOIN StatModifiers AS mods ON mods.StatId = StatsBase.Id
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


		/*OLD>>>>>
		//----------------------------------------------------------------------
		public Dictionary<int, StatBase> LoadMasterAttribs( bool reload = false )
		{
			if ( !reload && _dictAttribsAll != null && _dictAttribsAll.Any() )
			{
				return _dictAttribsAll;
			}

			_dictAttribsAll = LoadStatsAll(
				new List<int>
				{
					(int)StatType.AttribPhysical,
					(int)StatType.AttribMental,
					(int)StatType.AttribSpecial
				} );

			return _dictAttribsAll;
		}
		//<<<<< */

		//TODO: SAVE ALL STATS
		//----------------------------------------------------------------------
		public int SaveMasterAttribs( List<StatBase> listNewAttribs )
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
							Type         = @Type,
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


		/*OLD>>>>>
		//----------------------------------------------------------------------
		public Dictionary<int, StatBase> LoadMasterSkills( bool reload = false )
		{
			if ( !reload && _dictSkillsAll != null && _dictSkillsAll.Any() )
			{
				return _dictSkillsAll;
			}

			_dictSkillsAll = LoadStatsAll(
				new List<int>
				{
					(int)StatType.Skill,
					(int)StatType.Resistance,
				} );

			return _dictSkillsAll;
		}


		//----------------------------------------------------------------------
		public Dictionary<int, StatBase> LoadMasterMiscStats( bool reload = false )
		{
			if ( !reload && _dictSpecialStatsAll != null && _dictSpecialStatsAll.Any() )
			{
				return _dictSpecialStatsAll;
			}

			_dictSpecialStatsAll = LoadStatsAll(
				new List<int>
				{
					(int)StatType.Skill,
				} );

			return _dictSpecialStatsAll;
		}
		//<<<<< */

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
	}
}
