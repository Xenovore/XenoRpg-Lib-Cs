using System.Collections.Generic;

namespace XRpg
{
	//==========================================================================
	// StatBase
	// • Global data applicable to all stats for all characters;
	//   e.g. stat names, descriptions, types, etc.
	// • All attribute and skill info is created in separate lists and referenced by the character Stat lists
	//! • StatsBase table
	//==========================================================================
	public class StatBaseInfo
	{
		public int          Id           { get; set; }
		public string       Name         { get; set; }
		public string       Abbr         { get; set; } // Abbreviation, e.g. ST = Strength, IT = Intuition, IG = Intelligence

		public StatTypeEnum Type         { get; set; }
		public string       TypeName     { get; set; }
		public string       Description  { get; set; } // Last here so it's last in gridview
		public float        LevelMin     { get; set; } // Level range min.; can't go below this*
		public float        LevelBaseLo  { get; set; } // Base level low end*
		public float        LevelBaseHi  { get; set; } // Base level high end*
		public float        LevelMax     { get; set; } // Level range max; can't go above this*
		public float        ScoreBase    { get; set; } // Per level score
		public float        XPCost       { get; set; } // Per level XP cost to increase level
		public ApplyType    ApplyType    { get; set; } // 1 = score, 2 = level
		public ApplyHow     ApplyHow     { get; set; } // 1 = add,  2= multiply
		public float        ApplyPercent { get; set; } // Addl. scalar; how much to apply

		public List<StatBaseInfo> ListModifierStats { get; set; } // What stats modifies this one?

		// * Level range min/max: This is before modifiers
		// * Level ranges:
		//   LevelMin - LevelBaseLo = penalty
		//   LevelBaseLo - LevelBaseHi = no penalty/bonus
		//   LevelBaseHi - LevelMax = bonus
	}
}
