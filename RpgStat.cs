using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XenoRPGLib
{
	public enum StatType
	{
		AttribPhysical = 1,	// Strength, Dexterity, Constitution...
		AttribMental   = 2, // Intelligence, Intuition, Discipline...
		AttribSpecial  = 3, // Essence, Faith
		Skill          = 4,
		Racial         = 5, // Racial and background can prolly be merged
		Background     = 6,
	}

	public enum ApplyType
	{
		Score = 1, // Mod applies to score
		Level = 2, // Mod applies to level
	}


	//==========================================================================
	// RpgStatInfo
	// • Global data applicable to all stats for all characters;
	//   e.g. stat names, descriptions, types, etc.
	// • All attribute and skill info is created in separate lists and 
	//   referenced by the character RpgStat lists
	//==========================================================================
	public struct RpgStatInfo
	{
		public UInt32    m_iID;
		public string    m_strName;
		public string    m_strAbbr;
		public string    m_strDesc;
		public StatType  m_type;
		public ApplyType m_applyType;     // What does this apply to?
		//TODO: Define low and high base levels? (Above high base = bonus, below low base = penalty)
		public float     m_fLevelBase;    // Base level; above = bonus, below = penalty
		public float     m_fLevelCap;     // Level range cap
		public float     m_fScorePerLvl; 
		public float     m_fXPCostPerLvl; 
	}


	//==========================================================================
	// RpgStat
	// • Attributes, skills, etc.
	//==========================================================================
	public class RpgStat
	{
		public RpgStatInfo m_statInfo;
		public float m_fLevel;        // Unmodified level
		public float m_fLevelTotal;   // Modified level
		public float m_fScoreTotal;   // Score multiplied by level + modifiers
		//public float m_fScoreActual;
		public List<RpgStat> m_listAddlMods; // e.g. what attributes apply to a skill
		public float m_fXPTotal;

		// ----- Properties are necessary for C# DataGridView to work
		public string Name
		{
			get { return m_statInfo.m_strName; }
			set { m_statInfo.m_strName = value; }
		}

		public string Abbr
		{
			get { return m_statInfo.m_strAbbr; }
			set { m_statInfo.m_strAbbr = value; }
		}

		public string Desc
		{
			get { return m_statInfo.m_strDesc; }
			set { m_statInfo.m_strDesc = value; }
		}

		public float Level
		{
			get { return m_fLevel; }
			set { m_fLevel = value; }
		}

		public float LevelTotal
		{
			get { return m_fLevelTotal; }
			set { m_fLevelTotal = value; }
		}

		public float ScoreTotal
		{
			get { return GetScore(); }
		}

		public float XPTotal
		{
			get { return m_fXPTotal; }
			set { m_fXPTotal = value; }
		}



		public float GetScore()
		{
			// Score determination: ( Level - LevelBase ) * ScorePerLevel
			m_fLevelTotal = m_fLevel + ( 0 ); //TODO: m_listAddlMods
			m_fScoreTotal = 
				( m_fLevelTotal - m_statInfo.m_fLevelBase ) * 
				m_statInfo.m_fScorePerLvl;

			m_fScoreTotal = (float)Math.Floor( m_fScoreTotal );
			
			// Apply modifiers...
			float fScoreModTotal = ( 0 ); //TODO: m_listAddlMods
			m_fScoreTotal += fScoreModTotal;
			return m_fScoreTotal;

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
				( ( m_fLevel < m_statInfo.m_fLevelCap ) &&
			      ( m_fXPTotal >= m_statInfo.m_fXPCostPerLvl * ( m_fLevel + 1 ) ) );
		}

	
		//----------------------------------------------------------------------
		public bool LevelUp()
		{
			if ( !CanLevelUp() ) return false;

			// New level requires spending ( XP Cost per Level * Next Level )
			m_fXPTotal -= m_statInfo.m_fXPCostPerLvl * ( m_fLevel + 1 ); // Remove XP
			m_fLevel++; // Increase level

			return true;
		}


		//----------------------------------------------------------------------
		public bool AddXP( float fNewXP )
		{
			// If already at max level, no more XP...
			if ( m_fLevel == m_statInfo.m_fLevelCap ) return false;
			
			m_fXPTotal += fNewXP;

			return true;
		}
	}
}
