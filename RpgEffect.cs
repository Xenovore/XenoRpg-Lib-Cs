using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XenoRPGLib
{
	public class RpgEffect
	{
		public string m_strName;
		public string m_strDesc;
		//TODO: Effect that can be attached to item (or cast)

		// ----- Effect types/components
		// Damage target: min, range
		// Armor, reduces damage: value
		// Effect type (spell sphere): physical, fire, water, spirit, earth, etc.
		// Duration: seconds, permanent
		// Power source: none, pp/use, charge/use
		// Charge: recharge rate ( daily item could have an recharge rate )
		// Can be recharged?
		// Destroys item? (Cheaper materials slowly burn out as power passes though.)
		// Uses: total, per day
		// Trigger: use(manual), character hit(auto), attack/parry(auto), strike(auto), equip(auto)
		// Range: Self, Touch, Distance in meters, seen/visible
		// Area: sphere diameter or square meters
		// Form type: slash, blunt, pierce, missile (blunt, pierce),
		//		area surface, area volume, path
	}
}

/* Examples...
Sword:
* Damage: range 5-10
* Type: slash, pierce
* Trigger: attack
* Range: Touch and/or 1 m

SL Armor:
* Reduce Damage: 5 (AR)
* Type: leather
* Trigger: character hit
* Range: self
*/
/*

itemRightHand.



*/