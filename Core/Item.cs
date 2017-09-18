using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRpg
{
	// Database Table: Item
	public class Item
	{
		public UInt32      NestLevel    { get; set; }
		public UInt32      ParentContId { get; set; }
		public UInt32      Id           { get; set; }

		public string      Name         { get; set; }
		public string      Description  { get; set; }

		public ItemTypeEnum    Type         { get; set; }

		public float       Weight       { get; set; } // Kg; min = 0.01 (1 g)
		public bool        IsEnabled    { get; set; } // Can player manipulate this item
		public UInt16      Quantity     { get; set; } // Stack total
		public UInt16      QuantityCap  { get; set; } // Stack max capacity
		public ItemLocation  Location   { get; set; } // In world, or local (on character: in backpack, pouch, etc.)

		//UI display stuff
		public UInt16      IconIndex    { get; set; }
		public uint        SizeW        { get; set; } // 2D tile size, e.g. 1x1 for smallest items
		public uint        SizeH        { get; set; }

		//TODO: put back in Vector
		public float       PosX         { get; set; } // position (if local, z = layer depth)
		public float       PosY         { get; set; }
		public float       PosZ         { get; set; }

		public ItemContainer Container { get; set; }

		// Sockets? Like Diablo 3, Path of Exiles, etc.

		public List<UInt32> m_listOwnerIds; // Who owns this item? Could have multiple owners, e.g. guild members all own it.

		public List<Effect> m_listEffects;

		//----------------------------------------------------------------------
		public Item()
		{
		}


		public void SetItemType( ItemTypeEnum type )
		{
			Type = type;
		}


		public ItemAddResult AddQuantity( UInt16 iQty )
		{
			if ( Quantity + iQty <= QuantityCap )
			{
				Quantity += iQty;
				return ItemAddResult.Allowed;
			}
			else
			{
				return ItemAddResult.Full;
			}
		}
	};
}
