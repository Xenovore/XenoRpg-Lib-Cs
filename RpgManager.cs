using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XenoRPGLib
{
	// Body hit locations
	// Prolly can scale back, but this covers all the major locations
	// TODO: Move? Should be in a combat handling class
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


	//==========================================================================
	// RpgManager
	// • Manages all global/general RPG tasks, e.g. loading stat info
	//==========================================================================
	public class RpgManager
	{
		public Dictionary<string, RpgStatInfo> m_dictAttribsAll;
		public Dictionary<string, RpgStatInfo> m_dictSkillsAll;

		public Dictionary<RpgItemType, string> dictTypeNames;
		
		//----------------------------------------------------------------------
		public void InitItemTypeNames()
		{
			// Init item type names... NOTE: Could use enum names; don't need addl. strings
			dictTypeNames = new Dictionary<RpgItemType, string>();

			dictTypeNames.Add( RpgItemType.None         , "None"          );
			dictTypeNames.Add( RpgItemType.ContSlot     , "ContSlot"      );
			dictTypeNames.Add( RpgItemType.ContEndo     , "ContEndo"      );
			dictTypeNames.Add( RpgItemType.ContExo      , "ContExo"       );
			dictTypeNames.Add( RpgItemType.Container    , "Container"     );
			dictTypeNames.Add( RpgItemType.ContLiquid   , "ContLiquid"    );
			dictTypeNames.Add( RpgItemType.BodyHead     , "BodyHead"      );
			dictTypeNames.Add( RpgItemType.BodyEar      , "BodyEar"       );
			dictTypeNames.Add( RpgItemType.BodyFace     , "BodyFace"      );
			dictTypeNames.Add( RpgItemType.BodyNeck     , "BodyNeck"      );
			dictTypeNames.Add( RpgItemType.BodyShldr    , "BodyShldr"     );
			dictTypeNames.Add( RpgItemType.BodyTorso    , "BodyTorso"     );
			dictTypeNames.Add( RpgItemType.BodyArm      , "BodyArm"       );
			dictTypeNames.Add( RpgItemType.BodyWrist    , "BodyWrist"     );
			dictTypeNames.Add( RpgItemType.BodyHand     , "BodyHand"      );
			dictTypeNames.Add( RpgItemType.BodyWaist    , "BodyWaist"     );
			dictTypeNames.Add( RpgItemType.BodyLeg      , "BodyLeg"       );
			dictTypeNames.Add( RpgItemType.BodyFoot     , "BodyFoot"      );
			dictTypeNames.Add( RpgItemType.Body			, "Body"          );
			dictTypeNames.Add( RpgItemType.Jewelry      , "Jewelry"       );
			dictTypeNames.Add( RpgItemType.Ring         , "Ring"          );
			dictTypeNames.Add( RpgItemType.Bracelet     , "Bracelet"      );
			dictTypeNames.Add( RpgItemType.Necklace     , "Necklace"      );
			dictTypeNames.Add( RpgItemType.Earring      , "Earring"       );
			dictTypeNames.Add( RpgItemType.Armor        , "Armor"         );
			dictTypeNames.Add( RpgItemType.ArmorHead    , "ArmorHead"     );
			dictTypeNames.Add( RpgItemType.ArmorFace    , "ArmorFace"     );
			dictTypeNames.Add( RpgItemType.ArmorNeck    , "ArmorNeck"     );
			dictTypeNames.Add( RpgItemType.ArmorShldr   , "ArmorShldr"    );
			dictTypeNames.Add( RpgItemType.ArmorTorso   , "ArmorTorso"    );
			dictTypeNames.Add( RpgItemType.ArmorArm     , "ArmorArm"      );
			dictTypeNames.Add( RpgItemType.ArmorHand    , "ArmorHand"     );
			dictTypeNames.Add( RpgItemType.ArmorWaist   , "ArmorWaist"    );
			dictTypeNames.Add( RpgItemType.ArmorLeg     , "ArmorLeg"      );
			dictTypeNames.Add( RpgItemType.ArmorFoot    , "ArmorFoot"     );
			dictTypeNames.Add( RpgItemType.Clothing     , "Clothing"      );
			dictTypeNames.Add( RpgItemType.ClothingHead , "ClothingHead"  );
			dictTypeNames.Add( RpgItemType.ClothingFace , "ClothingFace"  );
			dictTypeNames.Add( RpgItemType.ClothingNeck , "ClothingNeck"  );
			dictTypeNames.Add( RpgItemType.ClothingShldr, "ClothingShldr" );
			dictTypeNames.Add( RpgItemType.ClothingTorso, "ClothingTorso" );
			dictTypeNames.Add( RpgItemType.ClothingArm  , "ClothingArm"   );
			dictTypeNames.Add( RpgItemType.ClothingHand , "ClothingHand"  );
			dictTypeNames.Add( RpgItemType.ClothingWaist, "ClothingWaist" );
			dictTypeNames.Add( RpgItemType.ClothingLeg  , "ClothingLeg"   );
			dictTypeNames.Add( RpgItemType.ClothingFoot , "ClothingFoot"  );
			dictTypeNames.Add( RpgItemType.Unused1      , "Unused1"       );
			dictTypeNames.Add( RpgItemType.WeaponBlade  , "WeaponBlade"   );
			dictTypeNames.Add( RpgItemType.WeaponBlunt  , "WeaponBlunt"   );
			dictTypeNames.Add( RpgItemType.WeaponThrown , "WeaponThrown"  );
			dictTypeNames.Add( RpgItemType.WeaponMissile, "WeaponMissile" );
			dictTypeNames.Add( RpgItemType.Weapon       , "Weapon"        );
			dictTypeNames.Add( RpgItemType.AmmoArrow    , "AmmoArrow"     );
			dictTypeNames.Add( RpgItemType.AmmoBolt     , "AmmoBolt"      );
			dictTypeNames.Add( RpgItemType.AmmoOther    , "AmmoOther"     );
			dictTypeNames.Add( RpgItemType.Unused5      , "Unused5"       );
			dictTypeNames.Add( RpgItemType.Ammo         , "Ammo"          );
			dictTypeNames.Add( RpgItemType.Coin         , "Coin"          );
			dictTypeNames.Add( RpgItemType.Gem          , "Gem"           );
			dictTypeNames.Add( RpgItemType.Herb         , "Herb"          );
			dictTypeNames.Add( RpgItemType.Liquid       , "Liquid"        );
		}


		public const float c_fAttrLvlBase = 50;
		public const float c_fAttrLvlCap = 100;

		
		public bool LoadMasterAttribList()
		{
			// Attrib list already populated? Bail...
			if ( ( m_dictAttribsAll != null ) &&
				 ( m_dictAttribsAll.Count != 0 ) )
			{
				return false;
			}

			//TODO: Load all attributes from DB

			m_dictAttribsAll = new Dictionary<string, RpgStatInfo>();

			var rpgStatInfoNew = new RpgStatInfo()
			{
				m_type          = StatType.AttribPhysical,
				m_strName       = "Strength",
				m_strAbbr       = "ST",
				m_strDesc       = "Affects melee weapon damage.",
				m_fLevelBase    = c_fAttrLvlBase,
				m_fLevelCap     = c_fAttrLvlCap,
				m_applyType     = ApplyType.Score,
				m_fScorePerLvl  = 1.0f,
				m_fXPCostPerLvl = 1000.0f,
			};
			m_dictAttribsAll.Add( 
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribPhysical,
				m_strName      = "Constitution",
				m_strAbbr      = "CN",
				m_strDesc      = "Affects hit point total.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribPhysical,
				m_strName      = "Dexterity",
				m_strAbbr      = "DX",
				m_strDesc      = "Affects chance to hit.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribPhysical,
				m_strName      = "Reflexes",
				m_strAbbr      = "RF",
				m_strDesc      = "Affects chance to dodge, and initiative.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribPhysical,
				m_strName      = "Appearance",
				m_strAbbr      = "AP",
				m_strDesc      = "Affects encounters.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			// ----- MENTAL ATTRIBUTES
			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribMental,
				m_strName      = "Intelligence",
				m_strAbbr      = "IG",
				m_strDesc      = "Affects casting ability.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribMental,
				m_strName      = "Intuition",
				m_strAbbr      = "IT",
				m_strDesc      = "Affects perception and riding skills.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribMental,
				m_strName      = "Discipline",
				m_strAbbr      = "DS",
				m_strDesc      = "Affects cool and resistance to mental attack.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribMental,
				m_strName      = "Presence",
				m_strAbbr      = "PR",
				m_strDesc      = "Affects encounters.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.AttribMental,
				m_strName      = "Technical",
				m_strAbbr      = "TC",
				m_strDesc      = "Affects ability to repair items and pick locks.",
				m_fLevelBase   = c_fAttrLvlBase,
				m_fLevelCap    = c_fAttrLvlCap,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictAttribsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			return true;
		}

		public void ClearAttribs()
		{
			m_dictAttribsAll.Clear();
		}


		//----------------------------------------------------------------------
		public bool LoadMasterSkillList()
		{
			// Skill list already populated? Bail...
			if ( ( m_dictSkillsAll != null ) && 
				 ( m_dictSkillsAll.Count != 0 ) )
			{
				return false;
			}

			//TODO: Load all skills from DB

			m_dictSkillsAll = new Dictionary<string, RpgStatInfo>();

			var rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.Skill,
				m_strName      = "1-H Sword",
				m_strAbbr      = "1HSW",
				m_strDesc      = "Skill with 1-handed swords.",
				m_fLevelBase   = 0,
				m_fLevelCap    = 100,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictSkillsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );
			//TODO: Add skill modifiers

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.Skill,
				m_strName      = "Dagger",
				m_strAbbr      = "DGGR",
				m_strDesc      = "Skill with daggers and small knives.",
				m_fLevelBase   = 0,
				m_fLevelCap    = 100,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictSkillsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			rpgStatInfoNew = new RpgStatInfo()
			{
				m_type         = StatType.Skill,
				m_strName      = "Shortbow",
				m_strAbbr      = "SBOW",
				m_strDesc      = "Skill with shortbows.",
				m_fLevelBase   = 0,
				m_fLevelCap    = 100,
				m_applyType    = ApplyType.Score,
				m_fScorePerLvl = 1.0f,
			};
			m_dictSkillsAll.Add(
				rpgStatInfoNew.m_strAbbr, rpgStatInfoNew );

			return true;
		}


		public void ClearSkills()
		{
			m_dictSkillsAll.Clear();
		}
	}
}
