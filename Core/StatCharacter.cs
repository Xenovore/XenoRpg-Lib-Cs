using System;

namespace XRpg
{
	//==========================================================================
	// StatCharacter
	// • Primary data needed to load character stats
	//! • CharacterStats table
	//==========================================================================
	public class StatCharacter
	{
		public int   CharacterId    { get; set; }
		public int   StatBaseInfoId { get; set; }
		public float Level          { get; set; } // Unmodified level
		public float LevelTotal     { get; set; } // TODO: Calculated; do we need to save it?
		public float XpToNextLvl    { get; set; } // Total XP towards next level

		public StatBaseInfo BaseInfo { get; set; } // Use StatBaseId to find it, save it here

		// Calculated...
		protected float _fLevelTotal;   // Total modified level
		protected float _fScoreTotal;   // Score multiplied by level + modifiers
		//public float m_fScoreActual;

		//TODO: Special ability modifiers that are specific to each character (would need new DB table)
		//public List<StatBase> ListSpecialAbilityStats { get; set; }


		public float ScoreTotal
		{
			get { return GetScore(); }
		}

		//TODO: Fetch Base Stat first
		//private float GetModifiersTotal() 
		//{
		//	float modtotal = 0.0f;

		//	foreach( var modstat in ListModifierStats )
		//	{
		//		switch( modstat.ApplyType )
		//		{
		//			case ApplyType.Score:
		//				float score = GetScore();
		//				switch ( modstat.ApplyHow )
		//				{
		//					case ApplyHow.Add:	    modtotal += modstat.GetScore(); break;
		//					case ApplyHow.Multiply: modtotal += score + ( score * ( modstat.GetScore() - 1.0f ) ); break; //TODO: Double-check this math
		//					default: break;
		//				}
		//			break;

		//			case ApplyType.Level:
		//				float level = Level;
		//			break;

		//			case ApplyType.None:
		//			default:
		//			break;
		//		}
		//	}

		//	return modtotal;
		//}

		private float GetScore()
		{
			// Score determination: ( Level - LevelBase ) * ScoreBase
			_fLevelTotal = Level + ( 0 ); //TODO: m_listAddlMods
			_fScoreTotal =
				( _fLevelTotal - BaseInfo.LevelBaseLo ) *
				BaseInfo.ScoreBase;

			_fScoreTotal = (float)Math.Floor( _fScoreTotal );

			// Apply modifiers...
			float fScoreModTotal = 0;//TODO: GetModifiersTotal();
			_fScoreTotal += fScoreModTotal;
			return _fScoreTotal;

			/*
			Attribute Example:
				Level Max = 100
				Level Base = 50
				ScorePerLevel = 1
				If Level = 73, Score = +23 ( 73 - 50 =  23;  23 * 1 =  23, round down/truncate )
				If Level = 21, Score = -29 ( 21 - 50 = -29; -29 * 1 = -29, round down )

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
				( ( Level < BaseInfo.LevelMax ) &&
			      ( XpToNextLvl >= BaseInfo.XPCost * ( Level + 1 ) ) );
		}


		//----------------------------------------------------------------------
		public bool LevelUp()
		{
			if ( !CanLevelUp() ) return false;

			// New level requires spending ( XP Cost per Level * Next Level )
			XpToNextLvl -= BaseInfo.XPCost * ( Level + 1 ); // Remove XP
			Level++; // Increase level

			return true;
		}


		//----------------------------------------------------------------------
		public bool AddXP( float fNewXP )
		{
			// If already at max level, no more XP...
			if ( Level >= BaseInfo.LevelMax ) return false;

			XpToNextLvl += fNewXP;

			return true;
		}
	}
}
