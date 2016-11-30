using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRpg
{
	public class CharacterBase
	{
		public uint   Id            { get; set; } // DB ID
		public string Type          { get; set; } // 1 = PC, 2 = NPC
		public string Name          { get; set; }
		public string NameLast      { get; set; }
		public string Title         { get; set; } // Lookup from table of titles?
		public uint   Age           { get; set; }
		public uint   Level         { get; set; } // Calculated from all attribute/skill levels; save in DB for convenience
		public uint   XPPool        { get; set; } // General XP; can be applied to any attribute/skill
		public uint   RaceId        { get; set; } // ID into Race table
		public uint   ProfessionId  { get; set; } // or Class; ID into Profession table
		public int    Alignment     { get; set; } // negative = evil, positive = good (use stat?)
		public uint   ContBodyId    { get; set; } // ID of character's inventory container "Body"

		// Root container for all character inventory;
		// this container is never displayed, only acting as
		// the root for all other inventory containers....
		public Item _contBody;

		// Karma (attribute?)
		// Reputation/Alliance List
		// Guild List
		//

		public List<Stat> _listAttributes;
		public List<Stat> _listResists;
		public List<Stat> _listSkills;
		public List<Stat> _listSpecials; // background/racial

		//TODO: Health
		//TODO: Stamina/Endurance (Short term, e.g. sprinting. Regenerates quickly)
		//TODO: Exhaustion/Vigor (Long term, only recovers with sleep )
		//TODO: power, karma, reputation, etc.

		public CharacterBase()
		{
			//_contBody.InitCapacity();
		}


		public bool LoadCharacterAttribs( Dictionary<string, Stat> dictInfo )
		{
			//NOTE: Base Attributes should be in Dictionary so they can be referenced here

			_listAttributes = new List<Stat>();

			/* EXAMPLE
			rpgStatNew = new Stat()
			{
				_statInfo     = dictInfo[ "AP" ],
				m_fLevel       = 66,
				_fScoreTotal  = 0.0f,
			};
			m_listAttributes.Add( rpgStatNew );
			*/

			return true;
		}


		public bool LoadCharacterSkills( Dictionary<string, Stat> dictInfo )
		{
			//TODO: LOAD SKILLS FROM DB

			return true;
		}

	}


	public class CharacterPlayer: CharacterBase
	{
		//TODO: Add player character specifics here
	}


	public class CharacterAI: CharacterBase
	{
		//TODO: Add non-player character specifics here
	}

}
