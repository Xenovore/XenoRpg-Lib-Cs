using System;
using System.Collections.Generic;
//TODO: MOVE TO MANAGER>>>>> using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace XRpg
{
	// CONTAINER
	// Containers hold items internally; items are only visible when
	// interacting with a container via UI

	// Database Table: ItemContainers_ext
	public class ItemContainer
	{
		public uint     ItemId      { get; set; } // Container is an item; reference to item in Items table
		public uint     ItemTotal   { get; set; }
		public uint     ItemCap     { get; set; } // Some containers can only hold a limited # of items, regardless of wt/vol
		public uint     VolumeTotal { get; set; }
		public uint     VolumeCap   { get; set; }
		public float    WeightTotal { get; set; }
		public float    WeightCap   { get; set; }
		public ItemTypeEnum TypeAllowed { get; set; } // Item types allowed in container
		public bool     Locked      { get; set; }

		public string   Name { get; set; } // Item.Name

		public List<Item> _listItems;

		//TODO: Use slot item type list? Would cut down on extra slots that are only used to check valid item type
		//public List<RpgItemType> m_listTypeAllowed; // type allowed per slot

		//----------------------------------------------------------------------
		public ItemContainer()
		{
		}


		//----------------------------------------------------------------------
		~ItemContainer()
		{
			//TODO C++: Free listItems and all contained items
		}


		//----------------------------------------------------------------------
		public void InitCapacity()
		{
			// Should be faster to allocate space all at once vs. each time a new item is added
			if ( ItemCap > 0 ) _listItems.Capacity = (int)ItemCap;
		}


		//----------------------------------------------------------------------
		public ItemAddResult CanAdd( Item item )
		{
			ItemAddResult addRes;

			// Exists...?
			if ( item == null ) addRes = ItemAddResult.NullItem;

			// Locked...?
			else if ( Locked ) addRes = ItemAddResult.Locked;

			// Allowed item type...?
			else if ( ( item.Type & TypeAllowed ) != item.Type )
			{
				addRes = ItemAddResult.NotAllowed;
			}

			// Over capacity...?
			else if ( ( ItemCap != 0 ) &&
				( ItemTotal >= ItemCap ) )
			{
				addRes = ItemAddResult.Full;
			}

			//TODO: Weight is special case...
			// Need to check back through parent containers and make
			// sure it's not exceeding weight cap. of parent containers

			// Over weight...
			else if ( ( WeightCap > 0 ) &&
				( item.Weight + WeightTotal > WeightCap ) )
			{
				addRes = ItemAddResult.OverWeight;
			}

			// Over volume...?
			else if ( ( VolumeCap > 0 ) &&
				( ( item.SizeH * item.SizeW ) + VolumeTotal > VolumeCap ) )
			{
				addRes = ItemAddResult.OverVolume;
			}
			else
				addRes = ItemAddResult.Allowed;

			return addRes;
		}


		//----------------------------------------------------------------------
		public string GetAddResultText( ItemAddResult addRes, string strNameAddItem )
		{
			string strMsg;
			switch ( addRes )
			{

				case ItemAddResult.NotAllowed:
					strMsg = "Cannot add '{0}'. Type not allowed in container '{1}'.";
					break;

				case ItemAddResult.NullItem:
					strMsg = "Cannot add null item to '{1}'.";
					break;

				case ItemAddResult.NotEmpty:
					strMsg = "Cannot add '{0}'. Slot '{1}' is not empty.";
					break;

				case ItemAddResult.OverVolume:
					strMsg = "Cannot add '{0}'. Item is too big to fit in container '{1}'.";
					break;

				case ItemAddResult.OverWeight:
					strMsg = "Cannot add '{0}'. Item would make container '{1}' too heavy.";
					break;

				case ItemAddResult.Locked:
					strMsg = "Cannot add '{0}'. Container '{1}' is locked.";
					break;

				case ItemAddResult.Full:
					strMsg = "Cannot add '{0}'. Container '{1}' is full.";
					break;

				default: //Allowed
					strMsg = "";
					break;
			}

			return string.Format( strMsg, strNameAddItem, this.Name );
		}


		//----------------------------------------------------------------------
		// AddItem()
		//----------------------------------------------------------------------
		public ItemAddResult AddItem( Item item, bool bStack = false )
		{
			//TODO: Check for matching existing item; if stackable,
			// then try to add to that item's stack (quantity)

			ItemAddResult addRes = CanAdd( item );
			if ( addRes != ItemAddResult.Allowed )
			{
				//TODO: Option to check and add to sub-containers,
				// if can't add to this container; e.g. drop earring on
				// head slot and it finds ear slot

				return addRes;
			}

			WeightTotal += item.Weight;
			VolumeTotal += ( item.SizeW * item.SizeH );

			_listItems.Add( item );
			ItemTotal++;

			return ItemAddResult.Allowed;
		}


		//TODO: NewContainerFromTemplate

		//TODO: AddItemByID()

/* BROKEN STUFF DOWN BELOW
		//----------------------------------------------------------------------
		public bool AddItemToContainerByName( string strName, bool bRecurse = false )
		{
			//TODO: FINISH THIS UP

			//TODO: Check weight/volume capacity
			//TODO: Check item type; can it be added to this container type?
			//TODO: Check for existing stacks; add to stacked item when possible

			for ( int i = 0; i < _listItems.Count(); ++i )
			{
				string strItemName = _listItems[ i ].Name;
				if ( strName == strItemName )
				{
					//TODO: ADD not remove
					_listItems.RemoveAt( i );
					return true;
				}
				else if ( bRecurse && _listItems[ i ] is ItemContainer )
				{
					bool bAdded =
						( (ItemContainer)_listItems[ i ] ).AddItemToContainerByName(
							strName,
							bRecurse );
					if ( bAdded ) return bAdded;
				}
			}

			return false; // not found
		}


		//----------------------------------------------------------------------
		public bool Clear()
		{
			_listItems.Clear(); //TODO C++: Need to recurse and clear all child containers
			return true;
		}


		//----------------------------------------------------------------------
		public bool RemoveItem( int i )
		{
			Debug.Assert(
				( i >= 0 ) && ( i < _listItems.Count ),
				string.Format( "Item index {0} out of bounds.", i ) );

			// Save weight/vol and subtract later only if remove succeeds...
			float fItemWt = _listItems[ i ].Weight;
			uint iItemSize = _listItems[ i ].SizeW * _listItems[ i ].SizeH;

			try
			{
				_listItems.RemoveAt( i );
			}
			catch
			{
				// Usually throws exception if index is invalid

				return false;
			}

			WeightTotal -= fItemWt;
			VolumeTotal -= iItemSize;

			return true;
		}


		//----------------------------------------------------------------------
		public bool RemoveItemByName( string strName )
		{
			Item itemTmp = _listItems.Find( n => strName == Name );
			if ( itemTmp == null ) return false; // Not found

			VolumeTotal -= itemTmp.SizeW * itemTmp.SizeH;
			WeightTotal -= itemTmp.Weight;
			_listItems.Remove( itemTmp );
			return true;
		}


		//----------------------------------------------------------------------
		public bool RemoveItemByName_r( string strName, bool bRecurse = false )
		{
			for ( int i = 0; i < _listItems.Count(); ++i )
			{
				string strItemName = _listItems[ i ].Name;
				if ( strName == strItemName )
				{
					return RemoveItem( i );
				}
				else if ( ( bRecurse ) && ( _listItems[ i ] is ItemContainer ) )
				{
					bool bRemoved =
						( (ItemContainer)_listItems[ i ] ).RemoveItemByName_r( strName, bRecurse );
					if ( bRemoved ) return bRemoved;
				}

			}

			return false; // not found
		}


		//----------------------------------------------------------------------
		// FindItemByName( <name>, <recurse?> )
		// • Find item <name>; returns the item object if found.
		//   <recurse?> specifies whether to search contained containers.
		//----------------------------------------------------------------------
		public Item FindItemByName( string strName, bool bRecurse = false )
		{
			for ( int i = 0; i < _listItems.Count(); ++i )
			{
				string strItemName = _listItems[ i ].Name;
				if ( strName == strItemName ) return _listItems[ i ];
				else if ( ( bRecurse ) && ( _listItems[ i ] is ItemContainer ) )
				{
					Item item =
						( (ItemContainer)_listItems[ i ] ).FindItemByName(
							strName,
							bRecurse );
					if ( item != null ) return item;
				}
			}

			return null; // not found
		}


		//----------------------------------------------------------------------
		// Returns the newly added item object if it's a container,
		// otherwise we don't care -- return null...
		//----------------------------------------------------------------------
		public ItemAddResult AddNewItem(
			string strName,
			ItemType type = ItemType.Any,
			float fWeight    = 0.01f,
			byte byVolW      = 1,
			byte byVolH      = 1 )
		{
			Item itemNew = new Item()
			{
				Name      = strName,
				Type     = type,
				Weight      = fWeight,
				SizeW      = byVolW,
				SizeH      = byVolH,
				Quantity    = 1,
				QuantityCap = 1,
			};

			return AddItem( itemNew );
		}


		//----------------------------------------------------------------------
		//	AddNewContainer()
		//	• Creates a new container and puts it in the current container
		//----------------------------------------------------------------------
		public ItemContainer AddNewContainer(
			string strName,
			ItemType type        = ItemType.ContSlot,
			ItemType typeAllowed = ItemType.Any,
			uint iItemCap = 1,
			float fWtCap = 0.0f,
			uint iVolCap  = 0 )
		{
			ItemContainer contNew = new ItemContainer()
			{
				Name        = strName,
				TypeAllowed = typeAllowed,
				ItemCap     = iItemCap,
				VolumeCap   = iVolCap,
				WeightCap   = fWtCap,
			};

			contNew.Type = type;
			contNew.InitCapacity();

			if ( AddItem( contNew ) == ItemAddResult.Allowed )
			{
				return contNew;
			}
			else
				return null;
		}


		//----------------------------------------------------------------------------
		private Item CreateNewItem( ItemType itemType )
		{
			Item itemNew;

			if ( ( itemType & ItemType.Container ) > 0 )
			{
				itemNew = new ItemContainer();
			}
			else
			{
				//if ( ( itemType & RpgItemType.Stackable ) > 0 )
				//{
				itemNew = new Item();
				//}
			}

			itemNew.Type = itemType;

			return itemNew;
		}


		//TODO: Move to InventoryManager?
		//----------------------------------------------------------------------
		//	Load( <xmlfile>, <recurse> )
		//	• Load inventory from <xmlfile> into this container
		//----------------------------------------------------------------------
		public bool LoadFromXML( string strFileName, bool bRecurse = true )
		{
			XmlReaderSettings xmlSettings = new XmlReaderSettings();
			xmlSettings.IgnoreComments = true;
			xmlSettings.IgnoreWhitespace = true;

			XmlReader xmlRead = XmlReader.Create( strFileName, xmlSettings );
			bool bSuccess = true;
			try
			{
				xmlRead.ReadStartElement( "InventoryRoot" );
				bSuccess = LoadItemFromXML_r( xmlRead, this, bRecurse );
			}
			catch ( Exception e )
			{
				MessageBox.Show(
					"Error loading inventory \"" + Name + "\":\n" +
					e.Message );
				return false;
			}
			finally
			{
				xmlRead.Close();
			}

			return bSuccess;
		}


		//----------------------------------------------------------------------------
		private bool LoadItemFromXML_r( XmlReader xmlRead, Item item, bool bRecurse = true )
		{
			if ( xmlRead == null ) return false;

			if ( item == null )
			{
				MessageBox.Show( "Error loading inventory; no item object." );
				return false;
			}

			// End of XML file, or not a start element, or an empty element? Bail...
			if ( xmlRead.EOF ||
				!xmlRead.IsStartElement() ||
				  xmlRead.IsEmptyElement )
			{
				return true; // Do nothing
			}

			//----- Get item type, type-specific properties, and create item object...
			//----- Read general properties...
			item.Name            = xmlRead.GetAttribute( "Name" );
			item.Id              = Convert.ToUInt32( xmlRead.GetAttribute( "ID" ) );
			//item.m_iTemplateID = Convert.ToUInt32( xmlRead.GetAttribute( "TemplateID" ) );
			item.Type            = (ItemType)Convert.ToUInt32( xmlRead.GetAttribute( "Type" ), 16 );
			item.Weight          = Convert.ToSingle( xmlRead.GetAttribute( "Weight" ) );
			item.SizeW           = Convert.ToByte( xmlRead.GetAttribute( "SizeW" ) );
			item.SizeH           = Convert.ToByte( xmlRead.GetAttribute( "SizeH" ) );
			item.IsEnabled       = ( xmlRead.GetAttribute( "Locked" ) == "true" );

			// ----- Container? Read container properties...
			if ( ( item.Type & ItemType.Container ) > 0  )
			{
				ItemContainer container = (ItemContainer)item;
				container._iItemTotal   = Convert.ToInt32( xmlRead.GetAttribute( "ItemTotal" ) );
				container.ItemCap     = Convert.ToInt32( xmlRead.GetAttribute( "ItemCap" ) );
				container._fWeightTotal = Convert.ToSingle( xmlRead.GetAttribute( "WeightTotal" ) );
				container._fWeightCap   = Convert.ToSingle( xmlRead.GetAttribute( "WeightCap" ) );
				container._iVolumeTotal = Convert.ToInt32( xmlRead.GetAttribute( "VolumeTotal" ) );
				container._iVolumeCap   = Convert.ToInt32( xmlRead.GetAttribute( "VolumeCap" ) );
				container._itemTypeAllowed =
					(ItemType)Convert.ToUInt32( xmlRead.GetAttribute( "TypeAllowed" ), 16 );

				container.InitCapacity();

				if ( bRecurse )
				{
					int iChildIdx = 0;
					while ( iChildIdx < container._iItemTotal )
					{
						xmlRead.Read(); // Advance to child
						if ( !xmlRead.IsEmptyElement && xmlRead.IsStartElement() )
						{
							ItemType type = (ItemType)Convert.ToUInt32( xmlRead.GetAttribute( "Type" ), 16 );
							container._listItems.Add( CreateNewItem( type ) );
							LoadItemFromXML_r( xmlRead, container._listItems[ iChildIdx ], bRecurse );
							iChildIdx++;
						}
					}
				}
			}

			// ----- Normal item...
			else // if ( ( itemParent.m_itemType & RpgItemType.Stackable ) > 0 ) //Don't really need the Stackable type
			{
				( (Item)item ).Quantity =
					Convert.ToUInt16( xmlRead.GetAttribute( "QuantityTotal" ) );
				( (Item)item ).QuantityCap   =
					Convert.ToUInt16( xmlRead.GetAttribute( "QuantityCap" ) );
			}

			return true;
		} //LoadItem()


		//Data Manager will handle loading from SQLite


		//----------------------------------------------------------------------
		public bool SaveToXML( string strFileName, bool bRecurse = true )
		{
			XmlWriterSettings xmlSettings = new XmlWriterSettings();
			xmlSettings.Indent = true;
			xmlSettings.IndentChars = "    ";
			xmlSettings.NewLineOnAttributes = true;

			XmlWriter xmlWrite = XmlWriter.Create( strFileName, xmlSettings );
			try
			{
				xmlWrite.WriteStartDocument();
				xmlWrite.WriteStartElement( "InventoryRoot" );

				SaveItemToXML_r( xmlWrite, this, bRecurse );

				xmlWrite.WriteEndElement();
				xmlWrite.WriteEndDocument();
			}
			catch ( Exception e )
			{
				MessageBox.Show(
					"Error saving inventory \"" + Name + "\":\n" +
					e.Message );
				return false;
			}
			finally
			{
				xmlWrite.Close();
			}

			return true;
		}


		//----------------------------------------------------------------------
		private void SaveItemToXML_r( XmlWriter xmlWrite, Item itemInv, bool bRecurse = true )
		{
			if ( xmlWrite == null ) return;

			//TODO: Just return? Only write empty slot for debugging...?
			// Container slot is empty...
			if ( itemInv  == null )
			{
				xmlWrite.WriteStartElement( "Empty" );
				xmlWrite.WriteEndElement();
				return;
			}

			//TODO: Don't really need named elements, but it clarifies things...
			if ( itemInv is ItemContainer )
			{
				xmlWrite.WriteStartElement( "Container" );
			}
			else
			{
				xmlWrite.WriteStartElement( "Item" );
			}

			// ----- Save item general properties...
			xmlWrite.WriteAttributeString( "Name",       itemInv.Name );
			xmlWrite.WriteAttributeString( "ID",         itemInv.Id.ToString() );
			//xmlWrite.WriteAttributeString( "TemplateID", itemInv.m_iTemplateID.ToString() );
			xmlWrite.WriteAttributeString( "Type",       itemInv.Type.ToString( "X" ) );
			xmlWrite.WriteAttributeString( "Weight",     itemInv.Weight.ToString() );
			xmlWrite.WriteAttributeString( "SizeW",      itemInv.SizeW.ToString() );
			xmlWrite.WriteAttributeString( "SizeH",      itemInv.SizeH.ToString() );
			xmlWrite.WriteAttributeString( "Locked",     itemInv.IsEnabled.ToString() );

			// ----- Container? Save container properties...
			if ( itemInv is ItemContainer )
			{
				ItemContainer cont = (ItemContainer)itemInv;
				xmlWrite.WriteAttributeString( "ItemTotal",   cont._iItemTotal.ToString() );
				xmlWrite.WriteAttributeString( "ItemCap",     cont.ItemCap.ToString() );
				xmlWrite.WriteAttributeString( "WeightTotal", cont._fWeightTotal.ToString() );
				xmlWrite.WriteAttributeString( "WeightCap",   cont._fWeightCap.ToString() );
				xmlWrite.WriteAttributeString( "VolTotal",    cont._iVolumeTotal.ToString() );
				xmlWrite.WriteAttributeString( "VolCap",      cont._iVolumeCap.ToString() );
				xmlWrite.WriteAttributeString( "TypeAllowed", cont._itemTypeAllowed.ToString( "X" ) );

				//int iChildIdx = 0;
				if ( bRecurse )
				{
					for ( int iChildIdx = 0; iChildIdx < cont._listItems.Capacity; ++iChildIdx )
					//while ( iChildIdx < cont.m_listItems.Capacity )
					{
						if ( iChildIdx < cont._listItems.Count )
						{
							SaveItemToXML_r(
								xmlWrite,
								cont._listItems[ iChildIdx ],
								bRecurse );
						}
						else
						{
							//TODO: Don't really need this; debugging only, puts all empty slots in XML
							SaveItemToXML_r( xmlWrite, null, false );
						}
						//iChildIdx++;
					}
				}
			}
			// ----- Save normal item properties...
			else
			{
				Item item = (Item)itemInv;
				xmlWrite.WriteAttributeString( "QuantityTotal", item.Quantity.ToString() );
				xmlWrite.WriteAttributeString( "QuantityCap",   item.QuantityCap.ToString() );
			}

			xmlWrite.WriteFullEndElement();
		} //SaveItem()

		/* TODO: MOVE TO MANAGER>>>>>
		public bool LoadFromDB()
		{
			//TODO: Load from SQLite
			//var sqlConn = new SQLiteConnection( @"Data Source=InvTest.sqlite" );
			//sqlConn.Open();

			//----- First nuke old inventory table...
			string strSQL = "SELECT * FROM items_containers";
			var sqlCmd = new SQLiteCommand( strSQL, m_sqlConn );

			var sqlReader = sqlCmd.ExecuteReader();
			while ( sqlReader.Read() )
			{
				//sqlReader[ i ]; // Column index
				//sqlReader[ "" ]; // Column name
			}

			return true;
		}
		//<<<<< */

		/*TODO: MOVE TO MANAGER>>>>>
		public bool SaveToDB()
		{
			//var sqlConn = new SQLiteConnection( @"Data Source=InvTest.sqlite" );
			//sqlConn.Open();

			//TODO: This is test code; prolly don't want to nuke the entire table
			// if it contains all inventories for all characters
			//----- First nuke old table...
			string strSQL =
				"DELETE FROM items_containers;" +
				"DELETE FROM sqlite_sequence WHERE name='items_containers';";
			var sqlCmd = new SQLiteCommand( strSQL, m_sqlConn );
			sqlCmd.ExecuteNonQuery();

			//----- Now save out inventory...
			SaveItemToDB_r( this, m_sqlConn );

			//sqlConn.Close();
			return true; //TODO: Build some better error handling
		}
		//<<<<<*/


		/*TODO: MOVE TO MANAGER>>>>>
		//----------------------------------------------------------------------
		private void SaveItemToDB_r(
			RpgItem itemInv,
			SQLiteConnection sqlConn,
			uint iIDContainer = 0,
			bool bRecurse = true )
		{
			uint iIDItem;
			iIDItem = ( itemInv != null ) ? itemInv.m_iID : 0;
			string strSQL =
				"INSERT INTO items_containers (id_item, id_container)" +
				"VALUES (@item, @cont)";
			var sqlCmd = new SQLiteCommand( strSQL, sqlConn );
			sqlCmd.Parameters.AddWithValue( "@item", iIDItem );
			sqlCmd.Parameters.AddWithValue( "@cont", iIDContainer );
			sqlCmd.ExecuteNonQuery();

			//itemInv.m_iID.ToString()
			//TODO: write iIDOwner
			if ( itemInv is RpgItemContainer )
			{
				RpgItemContainer cont = (RpgItemContainer)itemInv;
				if ( bRecurse )
				{
					for ( int iChildIdx = 0; iChildIdx < cont.m_listItems.Capacity; ++iChildIdx )
					{
						if ( iChildIdx < cont.m_listItems.Count )
							SaveItemToDB_r(
							cont.m_listItems[ iChildIdx ], sqlConn, itemInv.m_iID );
						else
							SaveItemToDB_r( null, sqlConn, itemInv.m_iID, false );
					}
				}
			}
		}
		//<<<<< */
	}//RpgContainer


	//==========================================================================
	// TODO: Create exo-container class? Probably no reason for it...
	// Exo-container holds items externally.
	// All held items are visible in 3D world; i.e. if held items don't need
	// world visibility, use a standard container instead.
	// Examples: Character body, belt w/pouches,weapon rack, coat hanger...
	// * Exo-containers generally should not go inside other containers.
	//==========================================================================
}
