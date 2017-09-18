
using XRpg.Utility;

namespace XRpg
{
	// Effect = What does the item or spell do?
	public class Effect
	{
		public uint _Id;
		public string _strName;
		public string _strDesc;
		public uint TargetId;
		public uint Type;           // Physical, Fire, Poison, etc.
		public float DamageMin;		// Damage target: min, range. Negative value = heal
		public float DamageMax;		// Armor, reduces damage: value
		public float DamageScale;	// Addl. scalar on damage
		public uint Duration;		// Duration: seconds, 0 = permanent
		public uint Frequency;		// How often does it process?
		public uint Source;			// Power source: none, pp/use, charge/use
		public bool CanRecharge;	// Can be recharged?
		public float ChargeRate;	// Charge: recharge rate ( daily item could have an recharge rate )
		public float PowerLevel;
		public float PowerCap;
		public uint DailyUses;		// Uses: total, per day
		public uint TriggerTypeId;	// Trigger: use(manual), character hit(auto), attack/parry(auto), strike(auto), equip(auto)
		public uint Range;			// Range: Self, Touch, Distance in meters, seen/visible
		public uint Form;			// Form type: slash, blunt, pierce, missile (blunt, pierce),
		public Vector Area;			// Area volume: sphere diameter or square meters; surface, area volume, path

		// Destroys item? (Cheaper materials slowly burn out as power passes though.)
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