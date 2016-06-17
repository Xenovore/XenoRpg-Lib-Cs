using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRpg
{
	public class Action
	{
		//TODO: Action takes input modifiers, returns result
		/* Input modifiers:
			•	Skill +
				Addl. char. modifiers (low health/stamina) +
				Character effect modifiers (stun, sleep, DOT, etc.) +
				random roll

			•	Base difficulty +
				Target diff. modifier (dodge, parry, etc.) +
				Environmental diff. modifiers (water, light, etc.) +
		*/
		public float m_fDiffBase;
		public float m_fDiffTotal;
		public ActionType m_actionType;
		public Stat m_statSkill; // Associated skill
		public float m_fRollMax;    // RNG range
		public int m_iXPForFail;    // Fail XP Amount
		public int m_iXPForSuccess; // Success XP Amount

		/*
		To check, total skill modifiers + random roll,
		subtract total difficulty, result is negative(failure) or
		positive(success)

		Result value could be just success/failure or
		could be applied to result tables, like in MERP.

		Spell/item effects usually directly affect attributes or skills,
		so are already factored in

		*/

		public float GetDiff()
		{
			//TODO: Calculate difficulty total here
			m_fDiffTotal = m_fDiffBase; // + etc.
			return m_fDiffTotal;
		}


		public int Check()
		{
			int iResult = 0;

			return iResult;
		}
	}


	public class RpgActionManager //???
	{

	}
}
