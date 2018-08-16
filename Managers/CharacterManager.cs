#pragma warning disable RECS0091 // Use 'var' keyword when possible

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dapper;
using XRpg.Utility;

namespace XRpg
{
	//==========================================================================
	// CharacterManager
	// • Manages all data-related RPG tasks, e.g. loading stat info
	//==========================================================================
	static public class CharacterManager
	{
		//! Character Manager should own these, not the UI
		static public List<CharacterBase> ListCharactersAll { get; set; }
		static public List<StatBaseInfo>  ListStatsInfoAll  { get; set; }

		static CharacterManager()
		{
		}


		// • Explicitly call Init(), so we know stuff should be initialized already
		//----------------------------------------------------------------------
		static public void Init()
		{
			XRpgDB.Init();

			ListCharactersAll = LoadAllCharacters();
			ListStatsInfoAll  = LoadAllStatsBaseInfo();
		}


		//----------------------------------------------------------------------
		static public List<CharacterBase> LoadAllCharacters( CharacterType charType = CharacterType.All )
		{
			Debug.Assert( XRpgDB.Conn != null );

			string sql = @"SELECT * FROM Characters";
			if ( charType != CharacterType.All ) sql += @"WHERE Characters.Type = @type";

			List<CharacterBase> listChars = XRpgDB.Conn.Query<CharacterBase>(
				sql,
				new { @type = (int)charType } ).ToList();


			return listChars;
		}


		//----------------------------------------------------------------------
		static public List<StatBaseInfo> LoadAllStatsBaseInfo()
		{
			const string sqlStats =
				@"SELECT
				*
				FROM StatsBase";
				//WHERE StatsBase.Type IN @types";

			var list = XRpgDB.Conn.Query<StatBaseInfo>( sqlStats )
				.ToList();

			return list;
		}


		static public List<int> LoadStatModifierIds( int statId )
		{
			const string sqlMods =
				@"SELECT
					StatsBase.Id
				FROM StatsBase
				JOIN StatModifiers_map AS mods ON mods.StatId = StatsBase.Id
				WHERE mods.TargetStatId = @statId";

				var list =
					XRpgDB.Conn.Query<int>(
					sqlMods,
					new
					{
						@statId = statId
					} ).ToList();

				return list;
		}


		//----------------------------------------------------------------------
		static public Dictionary<int, StatType> LoadStatTypes()
		{
			const string sqlStats =
				@"SELECT
				*
				FROM StatTypes";
				//WHERE StatsBase.Type IN @types";

			var dictStatTypes = XRpgDB.Conn.Query<StatType>( sqlStats )
				.ToDictionary( statType => statType.Id, statType => statType );

			return dictStatTypes;
		}


		//TODO: SAVE CHARACTER EDITOR CHANGES
		static public void SaveCharacter() {}


		//----------------------------------------------------------------------
		static public CharacterBase LoadCharacterById( uint Id )
		{
			Debug.Assert( XRpgDB.Conn != null );

			const string sql =
				@"SELECT * FROM Characters
				WHERE Characters.Id = @charId";

			CharacterBase character = XRpgDB.Conn.Query<CharacterBase>(
				sql,
				new { @charId = Id } ).FirstOrDefault();

			return character;
		}


		//----------------------------------------------------------------------
		static public CharacterBase GetCharacterById( uint id )
		{
			Debug.Assert( ListCharactersAll?.Count > 0 );

			return ListCharactersAll.First( character => character.Id == id );
		}


		//----------------------------------------------------------------------
		// LoadCharacterStats()
		// • id = character ID
		// • type = physical, mental, etc. 0 =  all
		//----------------------------------------------------------------------
		static public List<StatCharacter> LoadCharacterStats( uint characterId, uint typeId = 0 )
		{
			Debug.Assert( characterId > 0 );

			string sql =
				@"SELECT
					CharacterStats.CharacterId,
					CharacterStats.StatId AS StatBaseInfoId,
					CharacterStats.Level,
					CharacterStats.LevelTotal,
					CharacterStats.XpToNextLvl
				FROM CharacterStats
				WHERE CharacterStats.CharacterId = @charId";

			// Load by stat type; 0 = all...
			if ( typeId != 0 )
				sql += @" AND StatsBase.TypeId = @typeId";

			var listStats = XRpgDB.Conn.Query<StatCharacter>(
				sql,
				new
				{
					@charId = characterId,
					@typeId = typeId,
				}  )
				.ToList();

			
			// Get all base stat info...
			if ( ListStatsInfoAll?.Count == 0 ) { LoadAllStatsBaseInfo(); }
			foreach ( var stat in listStats )
			{
				stat.BaseInfo = ListStatsInfoAll.First( baseInfo => baseInfo.Id == stat.StatBaseInfoId );
			}
			
			//TODO: Load stat modifiers...
			foreach ( var stat in listStats )
			{}

			return listStats;
		}


		//TODO: SAVE STAT EDITOR CHANGES


		//TODO: UPDATE TO SAVE ALL STATS
		//----------------------------------------------------------------------
		static public int SaveMasterAttribs( List<StatBaseInfo> listNewAttribs )
		{
			Debug.Assert( listNewAttribs != null );

			int result;

			using ( var transact = XRpgDB.Conn.BeginTransaction() )
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

					result =  XRpgDB.Conn.Execute( sql, listNewAttribs );

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
		static public void ClearAttribs()
		{
			//_dictAttribsAll.Clear();
		}


		//----------------------------------------------------------------------
		static public void ClearSkills()
		{
			//_dictSkillsAll.Clear();
		}


		//----------------------------------------------------------------------
		static public List<Species> GetSpeciesList()
		{
			const string sql =
				@"SELECT
				*
				FROM Species";

			var list = XRpgDB.Conn.Query<Species>( sql ).ToList();
			return list;
		}


		//----------------------------------------------------------------------
		static public List<Profession> GetProfessionList()
		{
			const string sql =
				@"SELECT
				*
				FROM Professions";

			var list = XRpgDB.Conn.Query<Profession>( sql ).ToList();
			return list;
		}


		//----------------------------------------------------------------------
		static public List<StatType> GetStatTypeList()
		{
			const string sql =
				@"SELECT
				*
				FROM StatTypes";

			var list = XRpgDB.Conn.Query<StatType>( sql ).ToList();
			return list;
		}
	}
}
