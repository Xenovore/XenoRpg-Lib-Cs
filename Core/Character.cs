using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XRpg
{
	public class CharacterBase
	{
		public uint   Id           { get; set; } // DB ID
		public string Type         { get; set; } // 1 = PC, 2 = NPC
		public string Name         { get; set; }
		public string NameLast     { get; set; }
		public string Title        { get; set; } // Lookup from table of titles?
		public uint   Age          { get; set; }
		public Gender Gender       { get; set; }
		public float  Level        { get; set; } // Calculated from all attribute/skill levels; save in DB for convenience?
		public uint   XPPool       { get; set; } // General XP; can be applied to any attribute/skill
		public uint   RaceId       { get; set; } // ID into Race table
		public uint   ProfessionId { get; set; } // or Class; ID into Profession table
		public int    Alignment    { get; set; } // negative = evil, positive = good (use stat?)
		public uint   ContBodyId   { get; set; } // ID of character's inventory container "Body"

		//TODO: Health: special attribute?
		//TODO: Stamina/Endurance (Short term, e.g. sprinting. Regenerates quickly)
		//TODO: Exhaustion/Vigor (Long term, only recovers with sleep )
		//TODO: power, karma, reputation, etc.

		// Karma (attribute?): Is this different from alignment???
		// Reputation/Alliance List
		// Guild List
		//

		public List<StatCharacter> ListStats { get; set; }

		// Root container for all character inventory; this container is never
		// displayed, only acting as the root for all other inventory containers...
		public Item _contBody;


		public CharacterBase()
		{
			//_contBody.InitCapacity();
		}


		private float GetAverageLevel( IEnumerable<StatCharacter> listStats )
		{
			Debug.Assert( listStats != null );

			if ( listStats.Count() == 0 ) return 0;

			//TODO: Use level, base, max, min; e.g. for attribute, level is actually 70-50=20
			//TODO: MOVE CALC TO Stat class, GetLevel or something...
			float averageStatLevel = listStats.Sum( stat => stat.Level );
			averageStatLevel /= listStats.Count();

			return averageStatLevel;
		}


		public float CalculateCharacterLevel()
		{
	
			float level = 0;
			level += GetAverageLevel(
				ListStats.Where( stat => stat.BaseInfo.TypeId == StatTypeEnum.AttribPhysical ) );

			level += GetAverageLevel(
				ListStats.Where( stat => stat.BaseInfo.TypeId == StatTypeEnum.AttribMental ) );

			level += GetAverageLevel(
				ListStats.Where( stat => stat.BaseInfo.TypeId == StatTypeEnum.AttribSpecial ) );

			level += GetAverageLevel(
				ListStats.Where( stat => stat.BaseInfo.TypeId == StatTypeEnum.Skill ) );

			level += GetAverageLevel(
				ListStats.Where( stat => stat.BaseInfo.TypeId == StatTypeEnum.Resistance ) );

			level += GetAverageLevel(
				ListStats.Where( stat => stat.BaseInfo.TypeId == StatTypeEnum.Special ) );

			Level = level / 6.0f;

			return Level;
		}

	}


	public class CharacterPlayer: CharacterBase
	{
		//TODO: Add player character specifics here
	}


	public class CharacterAI: CharacterBase
	{
		//TODO: Add non-player character specifics here
	}

}
