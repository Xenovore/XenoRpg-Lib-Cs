using System;


namespace XRpg
{
	public enum CharacterType: ushort
	{
		Invalid   = 0,
		Player    = 1,
		NonPlayer = 2,
	}

	// ItemType allows for custom handling by item type;
	// primarily used to determine what item types containers can accept.
	// Also might determine the 2D icon, move/drop sound, etc....
	//TODO: READ FROM DB! THIS IS LIKELY NOT UP-TO-DATE WITH ITEM TYPE TABLE!
	[Flags]
	public enum ItemType: ulong // 64 bits
	{
		None          = 0x00000000, // On container, no restrictions
		// Containers...
		ContEndo      = 0x00000001, // Defaults to Endo (maybe Slot?)
		ContSlot      = 0x00000002, // Single item container (mainly used for body slots)
		ContExo       = 0x00000004, // Can attach stuff on this, e.g. armor sockets. Also displays 3D model in world
		Container     = 0x00000007,
			// ContEndo |
			// ContSlot |
			// ContExo  |
		Liquid        = 0x00000008, // Type determined by item effect. Has stackable "doses"; 1 dose = 1 gulp = 1 fl. oz = 30 ml)
		ContLiquid    = 0x00000009,
			//Container | Liquid, // Can only hold liquid

		// Equipable Body Locations...
		BodyHead      = 0x00000010,
		BodyEar       = 0x00000020, //? Only used for earring
		BodyFace      = 0x00000040,
		BodyNeck      = 0x00000080,
		BodyShldr     = 0x00000100,
		BodyTorso     = 0x00000200,
		BodyArm       = 0x00000400,
		BodyWrist     = 0x00000800, // REMOVE? Only used for bracelet; bracelet can use arm
		BodyHand      = 0x00001000,
		BodyWaist     = 0x00002000,
		BodyLeg       = 0x00004000,
		BodyFoot      = 0x00008000,
		Body          = 0x0000FFFF,
			//BodyHead  |
			//BodyEar   |
			//BodyFace  |
			//BodyNeck  |
			//BodyShldr |
			//BodyTorso |
			//BodyArm   |
			//BodyWrist |
			//BodyHand  |
			//BodyWaist |
			//BodyLeg   |
			//BodyFoot,

		// Clothing Group
		Clothing      = 0x00010000, // Cloth apparel; separate from armor since can wear armor over cloth
		ClothingHead  = 0x00010010, // Hat, hood
		ClothingFace  = 0x00010040, // Mask
		ClothingNeck  = 0x00010080, // Scarf
		ClothingShldr = 0x00010100, // Cloak
		ClothingTorso = 0x00010200, // Shirt
		ClothingArm   = 0x00010400, // Shirt
		ClothingHand  = 0x00011000, // Glove
		ClothingWaist = 0x00012000, // Belt
		ClothingLeg   = 0x00014000, // Pants
		ClothingFoot  = 0x00018000, // Sock

		// Armor Group
		Armor         = 0x00020000, // Non-cloth apparel: leather, chain, plate, etc.
		ArmorHead     = 0x00020010,
		ArmorFace     = 0x00020040,
		ArmorNeck     = 0x00020080,
		ArmorShldr    = 0x00020100,
		ArmorTorso    = 0x00020200,
		ArmorArm      = 0x00020400,
		ArmorHand     = 0x00021000,
		ArmorWaist    = 0x00022000,
		ArmorLeg      = 0x00024000,
		ArmorFoot     = 0x00028000,

		Jewelry       = 0x00040000,
		Earring       = 0x00040020,
		Necklace      = 0x00040080,
		Bracelet      = 0x00040800, // Wrist, but should be arm?
		Ring          = 0x00041000,
		Anklet        = 0x00048000, // Foot, but should be leg?

		Socketable    = 0x00080000, // Can be attached to armor/weapon socket

		SideLeft      = 0x00100000,
		SideRight     = 0x00200000,
		Side3         = 0x00400000,
		Side4         = 0x00800000,

		// Weapons... (These are more for item management than effect; Effects will handle damage type.)
		WeaponBlade   = 0x01000000,
		WeaponBlunt   = 0x02000000,
		WeaponThrown  = 0x04000000,
		WeaponMissile = 0x08000000,
		Weapon        = 0x0F000000,
			//WeaponBlade  |
			//WeaponBlunt  |
			//WeaponThrown |
			//WeaponMissile,


		Sheath        = ContSlot | WeaponBlade, // "Sheath" container should only take certain weapon types

		// Ammo... ( Ammo containers, e.g. quiver, only take certain ammo types )
		AmmoArrow     = 0x10000000,
		AmmoBolt      = 0x20000000,
		AmmoBullet    = 0x40000000, // Sling, musket, rifle
		AmmoUnused    = 0x80000000,

		Ammo          = 0xF0000000,
			//AmmoArrow  |
			//AmmoBolt   |
			//AmmoBullet |
			//AmmoUnused,

		Quiver        = 0x30000001,
			//ContEndo  |
			//AmmoArrow |
			//AmmoBolt,

		// No more room in uint for these...
		Coin          = 0x0000000100000000, // gold, silver, copper pieces
		Gem           = 0x0000000200000000, // Normal or magic; effect determines property
		Herb          = 0x0000000400000000, // or "reagent"
		Craft         = 0x0000000800000000, // Can be used in craft recipes; "junk"

		Tool          = 0x0000001000000000, // Can be used as tool, e.g. can you chop down a tree with this item

		/*
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
		Any          = 0xFFFFFFFFFFFFFFFF
	};


	public enum ItemLocation
	{
		Local, // on character
		World,
		// Space
		// DimensionX
	};


	// AddItem() result types...
	public enum ItemAddResult
	{
		NullItem,   // Attempt to add null item
		NotAllowed, // Item type not allowed in this container
		NotEmpty,   // Attempt to add item to full slot
		Full,       // Container slots are all taken
		OverWeight, // Item too heavy for container
		OverVolume, // Item too big for container
		Locked,     // container cannot be manipulated
		Allowed,
	}


	// MATERIALS
	//==============================================================================

	// Material types
	// • Affects item quality and durability
	public enum MaterialType
	{
		Wood,
		Bone,
		Stone,
		Glass,
		Copper,
		Tin,
		Bronze,
		Iron,
		Steel,
		Silver,
		Gold,
		Ceramic,
	}


	// STATS (ATTRIBUTES, SKILLS...)
	//==============================================================================

	//TODO: If loaded from DB, needs to be a class with
	//NOTE: "StatTypeEnum" to differentiate from "StatType" class
	public enum StatTypeEnum
	{
		Invalid        = 0,
		AttribPhysical = 1, // Strength, Dexterity, Constitution...
		AttribMental   = 2, // Intelligence, Intuition, Discipline...
		AttribSpecial  = 3, // Essence, Faith
		Resistance     = 4,
		Skill          = 5,
		Special        = 6, // Special ability, background option, one-time bonus, etc.
	}

	public enum ApplyType
	{
		None  = 0, // Stat does not modify another stat (skills)
		Score = 1, // Mod applies to score
		Level = 2, // Mod applies to level
	}

	public enum ApplyHow
	{
		Add = 1,
		Multiply = 2,
	}

	// ACTIONS
	//==============================================================================

	public enum ActionType
	{
		Passive, // Action is checked automatically
		Active,  // Action must be activated
	}

	// Body hit locations
	// Prolly can scale back, but this covers all the major locations


	// COMBAT: HIT LOCATIONS, ETC.
	//==============================================================================

	public enum BodyHitLocation
	{
		Head,
		EarL,
		EarR,
		EyeL,
		EyeR,
		Nose, // ?
		Mouth, // ?
		Neck,
		Chest,
		Back,
		Abdomen,
		ShoulderL,
		ShoulderR,
		ArmL,
		ArmR,
		WristL, // ?
		WristR, // ?
		HandL,
		HandR,
		Waist, // ?
		LegL,
		LegR,
		AnkleL, // ?
		AnkleR, // ?
		FootL,
		FootR,
	};


}