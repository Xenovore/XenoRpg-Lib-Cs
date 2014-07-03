using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XenoRPGLib
{
	public class RpgCharacterBase
	{
		public string m_strName;

		//TODO: Attributes
		public List<RpgStat> m_listAttributes;

		//TODO: Health, power, stamina,  exhaustion, karma, reputation, etc.

		//TODO: Skills
		public List<RpgStat> m_listSkills;
		//TODO: Resistances

		//TODO: Special abilities: racial, etc.

		// Root container for all character inventory; 
		// this container is never displayed, only acting as
		// the root for all other inventory containers....
		public RpgItemContainer m_contBody;


		public RpgCharacterBase()
		{
			m_contBody = new RpgItemContainer()
			{
				m_strName         = "Body",
				m_iItemCap        = 11,
				m_itemType        = RpgItemType.ContExo,
				m_itemTypeAllowed = RpgItemType.Container,
			};

			m_contBody.InitCapacity();
		}


		public bool LoadAttribs( Dictionary<string, RpgStatInfo> dictInfo )
		{
			//TODO: THESE HAVE TEST VALUES; 
			//ACTUAL VALUES SHOULD BE LOADED FROM DB

			m_listAttributes = new List<RpgStat>();

			// ----- PHYSICAL Attributes...
			RpgStat rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "ST" ],
				m_fLevel       = 60,
				m_fLevelTotal  = 60,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "CN" ],
				m_fLevel       = 57,
				m_fLevelTotal  = 57,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "DX" ],
				m_fLevel       = 78,
				m_fLevelTotal  = 78,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "RF" ],
				m_fLevel       = 92,
				m_fLevelTotal  = 92,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "AP" ],
				m_fLevel       = 66,
				m_fLevelTotal  = 66,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			// ----- MENTAL ATTRIBUTES
			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "IG" ],
				m_fLevel       = 50,
				m_fLevelTotal  = 50,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "IT" ],
				m_fLevel       = 40,
				m_fLevelTotal  = 40,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "DS" ],
				m_fLevel       = 50,
				m_fLevelTotal  = 50,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "PR" ],
				m_fLevel       = 50,
				m_fLevelTotal  = 50,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "TC" ],
				m_fLevel       = 68,
				m_fLevelTotal  = 68,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );

			return true;
		}


		public bool LoadSkills( Dictionary<string, RpgStatInfo> dictInfo )
		{
			//TODO: LOAD SKILLS FROM DB

			m_listSkills = new List<RpgStat>();
			// ----- TEST skills...
			RpgStat rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "1HSW" ],
				m_fLevel       = 1,
				m_fLevelTotal  = 1,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listSkills.Add( rpgStatNew );
			//TODO: Add skill modifiers

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "DGGR" ],
				m_fLevel       = 5,
				m_fLevelTotal  = 5,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listSkills.Add( rpgStatNew );

			rpgStatNew = new RpgStat()
			{
				m_statInfo     = dictInfo[ "SBOW" ],
				m_fLevel       = 9,
				m_fLevelTotal  = 9,
				m_fXPTotal     = 0.0f,
				m_fScoreTotal  = 0.0f,
			};
			m_listSkills.Add( rpgStatNew );

			return true;
		}

	}


	public class RpgCharacterPlayer: RpgCharacterBase
	{
		//TODO: Add player character specifics here
	}


	public class RpgCharacterAI: RpgCharacterBase
	{
		//TODO: Add non-player character specifics here
	}

}
