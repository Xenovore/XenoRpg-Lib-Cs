using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XenoRPGLib
{
	// ItemType allows for custom handling by item type; 
	// primarily used to determine what item types containers can accept.
	// Also might determine the 2D icon, move/drop sound, etc....
	public enum RpgItemType: uint // expand to ulong if necessary
	{
		None          = 0x00000000, 
		// Containers...
		ContSlot      = 0x00000001, // Single item container (mainly used for body slots)
		ContEndo      = 0x00000002, // Can put stuff in this
		ContExo       = 0x00000004, // Can hang stuff on this
		ContLiquid    = 0x00000008, // Can only hold liquid; liquid is stacked item (stack = 0.01 liter)?
		Container     = 
			ContEndo  | 
			ContExo   | 
			ContSlot  |
			ContLiquid ,

		// Body (Worn) locations...
		BodyHead      = 0x00000010,
		BodyEar       = 0x00000020,
		BodyFace      = 0x00000040,
		BodyNeck      = 0x00000080,
		BodyShldr     = 0x00000100,
		BodyTorso     = 0x00000200,
		BodyArm       = 0x00000400,
		BodyWrist     = 0x00000800,
		BodyHand      = 0x00001000,
		BodyWaist     = 0x00002000,
		BodyLeg       = 0x00004000,
		BodyFoot      = 0x00008000,
		Body          =
			BodyHead  |
			BodyEar   |
			BodyFace  |
			BodyNeck  |
			BodyShldr |
			BodyTorso |
			BodyArm   |
			BodyWrist |
			BodyHand  |
			BodyWaist |
			BodyLeg   |
			BodyFoot,

		// Jewelry...
		Jewelry       = 0x00010000,
		Ring          = Jewelry | BodyHand,
		Bracelet      = Jewelry | BodyWrist,
		Necklace      = Jewelry | BodyNeck,
		Earring       = Jewelry | BodyEar,

		// Armor...
		Armor         = 0x00020000, // Non-cloth apparel: leather, chain, plate, etc.
		ArmorHead     = Armor | BodyHead,
		ArmorFace     = Armor | BodyFace,
		ArmorNeck     = Armor | BodyNeck,
		ArmorShldr    = Armor | BodyShldr,
		ArmorTorso    = Armor | BodyTorso,
		ArmorArm      = Armor | BodyArm,
		ArmorHand     = Armor | BodyHand,
		ArmorWaist    = Armor | BodyWaist,
		ArmorLeg      = Armor | BodyLeg,
		ArmorFoot     = Armor | BodyFoot,

		// Clothing...
		Clothing      = 0x00040000, // Cloth apparel; separate from armor since can wear armor together over cloth
		ClothingHead  = Clothing | BodyHead,
		ClothingFace  = Clothing | BodyFace,
		ClothingNeck  = Clothing | BodyNeck,
		ClothingShldr = Clothing | BodyShldr,
		ClothingTorso = Clothing | BodyTorso,
		ClothingArm   = Clothing | BodyArm,
		ClothingHand  = Clothing | BodyHand,
		ClothingWaist = Clothing | BodyWaist,
		ClothingLeg   = Clothing | BodyLeg,
		ClothingFoot  = Clothing | BodyFoot,

		Unused1       = 0x00080000,

		// Weapons... ( "Sheath" slots should only take certain weapon types )
		WeaponBlade   = 0x00100000,
		WeaponBlunt   = 0x00200000,
		WeaponThrown  = 0x00400000,
		WeaponMissile = 0x00800000,
		Weapon        = 
		WeaponBlade   | 
		WeaponBlunt   |
		WeaponThrown  |
		WeaponMissile,

		Sheath        = ContSlot | WeaponBlade,

		// Ammo... ( Ammo containers, e.g. quiver, only take certain ammo types )
		AmmoArrow     = 0x01000000, 
		AmmoBolt      = 0x02000000,
		AmmoOther     = 0x04000000,
		Unused5       = 0x08000000,
		Ammo          = AmmoArrow | AmmoBolt | AmmoOther,

		Quiver        = ContEndo | AmmoArrow | AmmoBolt,

		Coin          = 0x10000000, // gold, silver, copper pieces
		Gem           = 0x20000000, // 
		Herb          = 0x40000000, // or "reagent"
		Liquid        = 0x80000000, // Type determined by effect. Has stackable "doses"

		
		/* NO MORE ROOM IN UINT FOR THESE>>>>>
		// Food (consumable)... TODO: Any reason for food-only containers?
		Meat         = 0x10000000, // TODO: Prolly not needed
		Bread        = 0x20000000, // TODO: Prolly not needed
		Powder       = 0x40000000, // TODO: Prolly not needed
		Fruit        = 0x00100000, // TEMP CHANGE
		Food         = Meat | Bread,
		Unused6      = 0x00200000,
		Unused7      = 0x00400000,
		Unused8      = 0x00800000,
		// Paper, books...
		Paper        = 0x01000000, // Can be written on or used as craft item: note, etc.
		Scroll       = 0x02000000, // Magic: spell can be inscribed, power included
		Book         = 0x04000000, // Normal book: history, story, etc.
		SpellBook    = 0x08000000, // Magic: spells can be inscribed, no power included
		// Craft items...
		Wood         = 0x10000000, // Craft item
		Metal        = 0x20000000, // Craft item
		Leather      = 0x40000000, // Craft item
		Stone        = 0x80000000, // Craft item
		<<<<<*/
		Any          = 0xFFFFFFFF
	};

	/*UNUSED>>>>>
	struct RpgItemTypeName
	{
		public string Name { get; set; }
		public RpgItemType Type { get; set; }
	}
	<<<<<*/

	public enum RpgItemLoc { World, Local };

	public class RpgItem
	{
		// Match order in DB table...
		public UInt32      m_iID;         
		public UInt16      m_iIconIdx;
		public string      m_strName;
		public RpgItemType m_itemType;    
		public float       m_fWeight;      // Kg; min = 0.01 (1 g)
		public byte        m_bySizeW;
		public byte        m_bySizeH;      // 2D tile size, e.g. 1x1 for smallest items
		public bool        m_bDisabled;    // Can player manipulate this item
		public UInt16      m_iQuantity;    // Stack total
		public UInt16      m_iQuantityCap; // Stack max capacity
		public RpgItemLoc  m_itemLoc;	   // In world, or local (in backpack, pouch, etc.)
		public Vector      m_vectLocation; // location (if local, z = layer depth)
		public UInt32      m_iTemplateID;  // ID of item base template (Do we care?)
		public string      m_strDesc;

		//TODO: item ownership; list of character IDs?
		//TODO: item special effects when used or equipped
		public RpgEffect   m_effect; //TODO: SHould be a list of effects

		public RpgItem()
		{
			m_iID          = 0x00000000;
			m_iIconIdx     = 0;
			m_strName      = "";
			m_itemType     = RpgItemType.Any;
			m_fWeight      = 0.01f;
			m_bySizeW      = 1;
			m_bySizeH      = 1;
			m_bDisabled    = false;
			m_iQuantity    = 0;
			m_iQuantityCap = 1;
			m_itemLoc      = RpgItemLoc.Local;
			m_vectLocation = new Vector( 0.0f, 0.0f, 0.0f );
			m_iTemplateID  = 0x00000000;
			m_strDesc      = "";
		}

		public void SetItemType( RpgItemType type )
		{
			m_itemType = type;
		}
		
		public ItemAddResult AddQuantity( UInt16 iQty )
		{
			if ( m_iQuantity + iQty <= m_iQuantityCap )
			{
				m_iQuantity += iQty;
				return ItemAddResult.Allowed;
			}
			else
			{
				return ItemAddResult.Full;
			}
		}
	};

	
	// Hmm, do I really want another class?
	// Maybe weapon effects should be under base item effects...
	// (as well as armor effects, etc.)
	/*
	public class RpgItemWeapon: RpgItem
	{
		//TODO: weapons stats: damage, speed, etc.

		public RpgItemWeapon()
		{
		}

	}
	 */

}
