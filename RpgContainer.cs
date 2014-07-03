using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;//TODO: Remove, only needed for temp message box...
using XenoRPGEditor;

namespace XenoRPGLib
{
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


	// CONTAINER
	// Containers hold items internally; items are only visible when
	// interacting with a container via UI

	public class RpgItemContainer: RpgItem
	{
		public List<RpgItem> m_listItems;
		//TODO: Use slot item type list? Would cut down on extra slots that are only used to check valid item type
		//public List<RpgItemType> m_listTypeAllowed; // type allowed per slot

		public int   m_iItemTotal;
		public int   m_iItemCap;     // Some containers can only hold a limited # of items, regardless of wt/vol
		public int   m_iVolumeTotal;
		public int   m_iVolumeCap;
		public float m_fWeightTotal;
		public float m_fWeightCap;
		public RpgItemType m_itemTypeAllowed;


		//----------------------------------------------------------------------
		public RpgItemContainer()
		{
			m_listItems = new List<RpgItem>();

			m_itemType     = RpgItemType.ContEndo;
			m_iItemTotal   = 0;
			m_iItemCap     = 0;    
			m_iVolumeTotal = 0;
			m_iVolumeCap   = 0;    
			m_fWeightTotal = 0.0f;
			m_fWeightCap   = 0.0f;
			m_bDisabled      = false;
			m_bySizeH      = 1;
			m_bySizeW      = 1;

		}


		//----------------------------------------------------------------------
		~RpgItemContainer()
		{
			//TODO C++: Free listItems and all contained items
		}


		//----------------------------------------------------------------------
		public void InitCapacity()
		{
			// Should be faster to allocate space all at once vs. each time a new item is added
			if ( m_iItemCap > 0 )
				m_listItems.Capacity = (int)m_iItemCap;
		}


		//----------------------------------------------------------------------
		public ItemAddResult CanAdd( RpgItem item )
		{
			ItemAddResult addRes;

			// Exists...?
			if ( item == null ) addRes = ItemAddResult.NullItem;

			// Locked...?
			if ( m_bDisabled ) addRes = ItemAddResult.Locked;
			
			// Allowed item type...?
			else if ( ( item.m_itemType & m_itemTypeAllowed ) == 0 )
			{
				addRes = ItemAddResult.NotAllowed;
			}

			// Over capacity...?
			else if ( ( m_iItemCap != 0 ) &&
				( m_iItemTotal >= m_iItemCap ) )
			{
				addRes = ItemAddResult.Full;
			}

			//TODO: Weight is special case... 
			// Need to check back through parent containers and make
			// sure it's not exceeding weight cap. of parent containers

			// Over weight...
			else if ( ( m_fWeightCap != 0 ) &&
				( item.m_fWeight + m_fWeightTotal > m_fWeightCap ) )
			{
				addRes = ItemAddResult.OverWeight;
			}

			// Over volume...?
			else if ( ( m_iVolumeCap != 0 ) &&
				( ( item.m_bySizeH * item.m_bySizeW ) + m_iVolumeTotal > m_iVolumeCap ) )
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

				default: //Full
					strMsg = "Cannot add '{0}'. Container '{1}' is full.";
					break;
			}
			return string.Format( strMsg, strNameAddItem, this.m_strName );
		}


		//----------------------------------------------------------------------
		// AddItem()
		//----------------------------------------------------------------------
		public ItemAddResult AddItem( RpgItem item, bool bStack = false )
		{
			//TODO: Check for matching existing item; if stackable,
			// then try to add to that item's stack (quantity)
			
			ItemAddResult addRes = CanAdd( item );
			if ( addRes != ItemAddResult.Allowed )
			{
				//TODO: Option to check and add to sub-containers, 
				// if can't add to this container; e.g. drop earring on
				// head slot and it finds ear slot

				//TODO: remove temp message box...
				MessageBox.Show( GetAddResultText( addRes, item.m_strName ) );

				return addRes;
			}

			m_fWeightTotal += item.m_fWeight;
			m_iVolumeTotal += item.m_bySizeW * item.m_bySizeH;

			m_listItems.Add( item );
			m_iItemTotal++;

			return ItemAddResult.Allowed;
		}


		//TODO: NewContainerFromTemplate

		//TODO: AddItemByID()


		//----------------------------------------------------------------------
		public bool AddItemToContainerByName( string strName, bool bRecurse = false )
		{
			//TODO: FINISH THIS UP

			//TODO: Check weight/volume capacity
			//TODO: Check item type; can it be added to this container type?
			//TODO: Check for existing stacks; add to stacked item when possible

			for ( int i = 0; i < m_listItems.Count(); ++i )
			{
				string strItemName = m_listItems[ i ].m_strName;
				if ( strName == strItemName )
				{
					//TODO: ADD not remove
					m_listItems.RemoveAt( i );
					return true;
				}
				else if ( bRecurse && m_listItems[ i ] is RpgItemContainer )
				{
					bool bAdded = 
						( (RpgItemContainer)m_listItems[ i ] ).AddItemToContainerByName(
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
			m_listItems.Clear(); //TODO C++: Need to recurse and clear all child containers
			return true;
		}


		//----------------------------------------------------------------------
		public bool RemoveItem( int i )
		{
			Debug.Assert(
				( i >= 0 ) && ( i < m_listItems.Count ),
				string.Format( "Item index {0} out of bounds.", i ) );

			// Save weight/vol and subtract later only if remove succeeds...
			float fItemWt = m_listItems[ i ].m_fWeight;
			int iItemSize = m_listItems[ i ].m_bySizeW * m_listItems[ i ].m_bySizeH;
			
			try
			{
				m_listItems.RemoveAt( i );
			}
			catch
			{
				// Usually throws exception if index is invalid

				return false;
			}

			m_fWeightTotal -= fItemWt;
			m_iVolumeTotal -= iItemSize;
			
			return true;
		}


		//----------------------------------------------------------------------
		public bool RemoveItemByName( string strName )
		{
			RpgItem itemTmp = m_listItems.Find( n => strName == m_strName );
			if ( itemTmp == null ) return false; // Not found

			m_iVolumeTotal -= itemTmp.m_bySizeW * itemTmp.m_bySizeH;
			m_fWeightTotal -= itemTmp.m_fWeight;
			m_listItems.Remove( itemTmp );
			return true;
		}


		//----------------------------------------------------------------------
		public bool RemoveItemByName( string strName, bool bRecurse = false )
		{
			for ( int i = 0; i < m_listItems.Count(); ++i )
			{
				string strItemName = m_listItems[ i ].m_strName;
				if ( strName == strItemName )
				{
					return RemoveItem( i );
				}
				else if ( ( bRecurse ) && ( m_listItems[ i ] is RpgItemContainer ) )
				{
					bool bRemoved = 
						( (RpgItemContainer)m_listItems[ i ] ).RemoveItemByName(
							strName,
							bRecurse );
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
		public RpgItem FindItemByName( string strName, bool bRecurse = false )
		{
			for ( int i = 0; i < m_listItems.Count(); ++i )
			{
				string strItemName = m_listItems[ i ].m_strName;
				if ( strName == strItemName ) return m_listItems[ i ];
				else if ( ( bRecurse ) && ( m_listItems[ i ] is RpgItemContainer ) )
				{
					RpgItem item = 
						( (RpgItemContainer)m_listItems[ i ] ).FindItemByName(
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
			RpgItemType type = RpgItemType.Any,
			float fWeight    = 0.01f,
			byte byVolW      = 1,
			byte byVolH      = 1 )
		{
			RpgItem itemNew = new RpgItem()
			{
				m_strName      = strName,
				m_itemType     = type,
				m_fWeight      = fWeight,
				m_bySizeW      = byVolW,
				m_bySizeH      = byVolH,
				m_iQuantity    = 1,
				m_iQuantityCap = 1,
			};

			return AddItem( itemNew );
		}


		//----------------------------------------------------------------------
		//	AddNewContainer() 
		//	• Creates a new container and puts it in the current container
		//----------------------------------------------------------------------
		public RpgItemContainer AddNewContainer(
			string strName,
			RpgItemType typeAllowed = RpgItemType.Any,
			RpgItemType type        = RpgItemType.ContSlot,
			int iItemCap = 1,
			float fWtCap = 0.0f,
			int iVolCap  = 0 )
		{
			RpgItemContainer contNew = new RpgItemContainer()
			{
				m_strName         = strName,
				m_itemTypeAllowed = typeAllowed,
				m_iItemCap        = iItemCap,
				m_iVolumeCap      = iVolCap,
				m_fWeightCap      = fWtCap,
			};

			contNew.m_itemType = type;
			contNew.InitCapacity();

			if ( AddItem( contNew ) == ItemAddResult.Allowed )
			{
				return contNew;
			}
			else
				return null;
		}


		//----------------------------------------------------------------------------
		private RpgItem CreateNewItem( RpgItemType itemType )
		{
			RpgItem itemNew;

			if ( ( itemType & RpgItemType.Container ) > 0 )
			{
				itemNew = new RpgItemContainer();
			}
			else
			{
				//if ( ( itemType & RpgItemType.Stackable ) > 0 )
				//{
				itemNew = new RpgItem();
				//}
			}

			itemNew.m_itemType = itemType;

			return itemNew;
		}


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
					"Error loading inventory \"" + m_strName + "\":\n" +
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
		private bool LoadItemFromXML_r( XmlReader xmlRead, RpgItem item, bool bRecurse = true )
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
			item.m_strName     = xmlRead.GetAttribute( "Name" );
			item.m_iID         = Convert.ToUInt32( xmlRead.GetAttribute( "ID" ) );
			item.m_iTemplateID = Convert.ToUInt32( xmlRead.GetAttribute( "TemplateID" ) );
			item.m_itemType    = (RpgItemType)Convert.ToUInt32( xmlRead.GetAttribute( "Type" ), 16 );
			item.m_fWeight     = Convert.ToSingle( xmlRead.GetAttribute( "Weight" ) );
			item.m_bySizeW     = Convert.ToByte( xmlRead.GetAttribute( "SizeW" ) );
			item.m_bySizeH     = Convert.ToByte( xmlRead.GetAttribute( "SizeH" ) );
			item.m_bDisabled     = ( xmlRead.GetAttribute( "Locked" ) == "true" );

			// ----- Container? Read container properties...
			if ( ( item.m_itemType & RpgItemType.Container ) > 0  )
			{
				RpgItemContainer container = (RpgItemContainer)item;
				container.m_iItemTotal   = Convert.ToInt32( xmlRead.GetAttribute( "ItemTotal" ) );
				container.m_iItemCap     = Convert.ToInt32( xmlRead.GetAttribute( "ItemCap" ) );
				container.m_fWeightTotal = Convert.ToSingle( xmlRead.GetAttribute( "WeightTotal" ) );
				container.m_fWeightCap   = Convert.ToSingle( xmlRead.GetAttribute( "WeightCap" ) );
				container.m_iVolumeTotal = Convert.ToInt32( xmlRead.GetAttribute( "VolumeTotal" ) );
				container.m_iVolumeCap   = Convert.ToInt32( xmlRead.GetAttribute( "VolumeCap" ) );
				container.m_itemTypeAllowed = 
					(RpgItemType)Convert.ToUInt32( xmlRead.GetAttribute( "TypeAllowed" ), 16 );

				container.InitCapacity();

				if ( bRecurse )
				{
					int iChildIdx = 0;
					while ( iChildIdx < container.m_iItemTotal )
					{
						xmlRead.Read(); // Advance to child
						if ( !xmlRead.IsEmptyElement && xmlRead.IsStartElement() )
						{
							RpgItemType type = (RpgItemType)Convert.ToUInt32( xmlRead.GetAttribute( "Type" ), 16 );
							container.m_listItems.Add( CreateNewItem( type ) );
							LoadItemFromXML_r( xmlRead, container.m_listItems[ iChildIdx ], bRecurse );
							iChildIdx++;
						}
					}
				}
			}

			// ----- Normal item...
			else // if ( ( itemParent.m_itemType & RpgItemType.Stackable ) > 0 ) //Don't really need the Stackable type
			{
				( (RpgItem)item ).m_iQuantity = 
					Convert.ToUInt16( xmlRead.GetAttribute( "QuantityTotal" ) );
				( (RpgItem)item ).m_iQuantityCap   = 
					Convert.ToUInt16( xmlRead.GetAttribute( "QuantityCap" ) );
			}

			return true;
		} //LoadItem()

		
		//TODO: Load from SQLite

		
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
					"Error saving inventory \"" + m_strName + "\":\n" +
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
		private void SaveItemToXML_r( XmlWriter xmlWrite, RpgItem itemInv, bool bRecurse = true )
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
			if ( itemInv is RpgItemContainer )
			{
				xmlWrite.WriteStartElement( "Container" );
			}
			else
			{
				xmlWrite.WriteStartElement( "Item" );
			}

			// ----- Save item general properties...
			xmlWrite.WriteAttributeString( "Name",       itemInv.m_strName );
			xmlWrite.WriteAttributeString( "ID",         itemInv.m_iID.ToString() );
			xmlWrite.WriteAttributeString( "TemplateID", itemInv.m_iTemplateID.ToString() );
			xmlWrite.WriteAttributeString( "Type",       itemInv.m_itemType.ToString( "X" ) );
			xmlWrite.WriteAttributeString( "Weight",     itemInv.m_fWeight.ToString() );
			xmlWrite.WriteAttributeString( "SizeW",      itemInv.m_bySizeW.ToString() );
			xmlWrite.WriteAttributeString( "SizeH",      itemInv.m_bySizeH.ToString() );
			xmlWrite.WriteAttributeString( "Locked",     itemInv.m_bDisabled.ToString() );

			// ----- Container? Save container properties...
			if ( itemInv is RpgItemContainer )
			{
				RpgItemContainer cont = (RpgItemContainer)itemInv;
				xmlWrite.WriteAttributeString( "ItemTotal",   cont.m_iItemTotal.ToString() );
				xmlWrite.WriteAttributeString( "ItemCap",     cont.m_iItemCap.ToString() );
				xmlWrite.WriteAttributeString( "WeightTotal", cont.m_fWeightTotal.ToString() );
				xmlWrite.WriteAttributeString( "WeightCap",   cont.m_fWeightCap.ToString() );
				xmlWrite.WriteAttributeString( "VolTotal",    cont.m_iVolumeTotal.ToString() );
				xmlWrite.WriteAttributeString( "VolCap",      cont.m_iVolumeCap.ToString() );
				xmlWrite.WriteAttributeString( "TypeAllowed", cont.m_itemTypeAllowed.ToString( "X" ) );

				//int iChildIdx = 0;
				if ( bRecurse )
				{
					for ( int iChildIdx = 0; iChildIdx < cont.m_listItems.Capacity; ++iChildIdx )
					//while ( iChildIdx < cont.m_listItems.Capacity )
					{
						if ( iChildIdx < cont.m_listItems.Count )
						{
							SaveItemToXML_r(
								xmlWrite,
								cont.m_listItems[ iChildIdx ],
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
				RpgItem item = (RpgItem)itemInv;
				xmlWrite.WriteAttributeString( "QuantityTotal", item.m_iQuantity.ToString() );
				xmlWrite.WriteAttributeString( "QuantityCap",   item.m_iQuantityCap.ToString() );
			}

			xmlWrite.WriteFullEndElement();
		} //SaveItem()

		
		public bool LoadFromDB()
		{
			//TODO: Load from SQLite
			//var sqlConn = new SQLiteConnection( @"Data Source=InvTest.sqlite" );
			//sqlConn.Open();

			//----- First nuke old inventory table...
			string strSQL = "SELECT * FROM items_containers";
			var sqlCmd = new SQLiteCommand( strSQL, FormMain.SQLConn );

			var sqlReader = sqlCmd.ExecuteReader();
			while ( sqlReader.Read() )
			{
				//sqlReader[ i ]; // Column index
				//sqlReader[ "" ]; // Column name
			}

			return true;
		}


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
			var sqlCmd = new SQLiteCommand( strSQL, FormMain.SQLConn );
			sqlCmd.ExecuteNonQuery();

			//----- Now save out inventory...
			SaveItemToDB_r( this, FormMain.SQLConn );

			//sqlConn.Close();
			return true; //TODO: Build some better error handling
		}

		
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
