using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRpg
{
	//==========================================================================
	// Stat
	// • Primary data needed to load character stats
	//==========================================================================
	public class Stat
	{
		// StatBase table...
		public int          Id           { get; set; }
		public string       Name         { get; set; }
		public string       Abbr         { get; set; } // Abbreviation, e.g. ST = Strength, IT = Intuition, IG = Intelligence

		// CharacterStats table...
		public float        Level        { get; set; } // Unmodified level
		public float        XpToNextLvl  { get; set; } // Total XP towards next level

		// StatBase table... (down here so they come last in gridview)
		public StatTypeEnum TypeId       { get; set; }
		public string       TypeName     { get; set; }
		public string       Description  { get; set; } // Last here so it's last in gridview

		// Calculated...
		protected float _fLevelTotal;   // Total modified level
		protected float _fScoreTotal;   // Score multiplied by level + modifiers
		//public float m_fScoreActual;

		//TODO: Remove; probably not needed here; see Stat.listModIds
		//public List<Stat> m_listAddlMods; // e.g. what attributes apply to a skill

	};


	//==========================================================================
	// StatBase
	// • Global data applicable to all stats for all characters;
	//   e.g. stat names, descriptions, types, etc.
	// • All attribute and skill info is created in separate lists and
	//   referenced by the character Stat lists
	//==========================================================================
	public class StatBase: Stat
	{
		// StatBase table...
		public float     LevelMin     { get; set; } // Level range min.; can't go below this*
		public float     LevelBaseLo  { get; set; } // Base level low end*
		public float     LevelBaseHi  { get; set; } // Base level high end*
		public float     LevelMax     { get; set; } // Level range max; can't go above this*
		public float     ScoreBase    { get; set; } // Per level score
		public float     XPCost       { get; set; } // Per level XP cost to increase level
		public ApplyType ApplyType    { get; set; } // 1 = score, 2 = level
		public ApplyHow  ApplyHow     { get; set; } // 1 = add,  2= multiply
		public float     ApplyPercent { get; set; } // Addl. scalar; how much to apply

		public List<int> listModIds  { get; set; } // What stats modifies this one?

		// * Level range min/max: This is before modifiers
		// * Level ranges:
		//   LevelMin - LevelBaseLo = penalty
		//   LevelBaseLo - LevelBaseHi = no penalty/bonus
		//   LevelBaseHi - LevelMax = bonus

		//TODO: MIGHT NEED THESE IN StatCharacter
		public float ScoreTotal
		{
			get { return GetScore(); }
		}


		private float GetScore()
		{
			// Score determination: ( Level - LevelBase ) * ScoreBase
			_fLevelTotal = Level + ( 0 ); //TODO: m_listAddlMods
			_fScoreTotal =
				( _fLevelTotal - LevelBaseLo ) *
				ScoreBase;

			_fScoreTotal = (float)Math.Floor( _fScoreTotal );

			// Apply modifiers...
			float fScoreModTotal = ( 0 ); //TODO: m_listAddlMods
			_fScoreTotal += fScoreModTotal;
			return _fScoreTotal;

			/*
			Attribute Example:
				Level Max = 100
				Level Base = 50
				ScorePerLevel = 0.2
				If Level = 73, Score = +4 ( 73 - 50 =  23;  23 * 0.2 =  4.6, round down/truncate )
				If Level = 21, Score = -5 ( 21 - 50 = -29; -29 * 0.2 = -5.8, round down )

			Skill Example:
				Level Max = 20
				Level Base = 0
				ScorePerLevel = 5
				If Level = 15, Score = 75 ( 15 - 0 = 15; 15 * 5 = 75 )
				If Level =  4, Score = 20 (  4 - 0 =  4;  4 * 5 = 20 )

			*/
		}

		//TODO: Add mod to list

		//TODO: Remove mod from list


		//----------------------------------------------------------------------
		public bool CanLevelUp()
		{
			// Is the current level below the max?
			// Is there enough XP to spend?
			return
				( ( Level < LevelMax ) &&
			      ( XpToNextLvl >= XPCost * ( Level + 1 ) ) );
		}


		//----------------------------------------------------------------------
		public bool LevelUp()
		{
			if ( !CanLevelUp() ) return false;

			// New level requires spending ( XP Cost per Level * Next Level )
			XpToNextLvl -= XPCost * ( Level + 1 ); // Remove XP
			Level++; // Increase level

			return true;
		}


		//----------------------------------------------------------------------
		public bool AddXP( float fNewXP )
		{
			// If already at max level, no more XP...
			if ( Level >= LevelMax ) return false;

			XpToNextLvl += fNewXP;

			return true;
		}
	}
}
