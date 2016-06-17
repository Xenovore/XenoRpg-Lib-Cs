using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace KevInvTest.Source
{
	public abstract class RpgContainerBase: RpgItem
	{
		public RpgItemType m_itemTypeAllowed;


		//----------------------------------------------------------------------
		public RpgContainerBase()
		{
			m_itemType        = RpgItemType.Container;
			m_itemTypeAllowed = RpgItemType.Any;
		}

		
		public abstract ItemAddResult CanAdd( RpgItem item );
		public abstract ItemAddResult AddItem( RpgItem item );
		public abstract bool RemoveItemByName( string strName, bool bRecurse = false );

		//----------------------------------------------------------------------
		// Returns the newly added item object if it's a container, 
		// otherwise we don't care -- return null...
		//----------------------------------------------------------------------
		public ItemAddResult AddNewItem(
			string strName,
			RpgItemType type    = RpgItemType.Any,
			float    fWeight = 0.01f,
			byte     byVolW  = 1,
			byte     byVolH  = 1 )
		{
			RpgItem itemNew = new RpgItem()
			{
				m_strName  = strName,
				m_itemType = type,
				m_fWeight      = fWeight,
				m_bySizeW   = byVolW,
				m_bySizeH   = byVolH,
			};

			return AddItem( itemNew );
		}


		//----------------------------------------------------------------------
		//	AddNewContainer() 
		//	• Creates a new container and puts it in the current container
		//----------------------------------------------------------------------
		public RpgContainer AddNewContainer(
			string strName,
			RpgItemType typeAllowed = RpgItemType.Any,
			RpgItemType type        = RpgItemType.ContSlot,
			int iItemCap = 1,
			float fWtCap = 0.0f,
			int iVolCap  = 0 )
		{
			RpgContainer contNew = new RpgContainer()
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

		
		//----------------------------------------------------------------------
		//	Load( <xmlfile>, <recurse> )
		//	• Load inventory from <xmlfile> into this container
		//----------------------------------------------------------------------
		public bool Load( string strFileName, bool bRecurse = true )
		{
			XmlReaderSettings xmlSettings = new XmlReaderSettings();
			xmlSettings.IgnoreComments = true;
			xmlSettings.IgnoreWhitespace = true;

			XmlReader xmlRead = XmlReader.Create( strFileName, xmlSettings );
			bool bSuccess = true;
			try
			{
				xmlRead.ReadStartElement( "InventoryRoot" );
				bSuccess = LoadItem( xmlRead, this, bRecurse );
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
		private RpgItem CreateNewItem( RpgItemType itemType )
		{
			if ( ( itemType & RpgItemType.ContEndo ) > 0 ||
			     ( itemType & RpgItemType.ContExo ) > 0 )
			{
				return new RpgContainer();
			}
			
			if ( ( itemType & RpgItemType.ContSlot ) > 0 )
				return new RpgContainerSlot();

			if ( ( itemType & RpgItemType.Stackable ) > 0 )
				return new RpgItemStack();

			return new RpgItem();
		}

		
		//----------------------------------------------------------------------------
		public bool LoadItem( XmlReader xmlRead, RpgItem itemParent, bool bRecurse = true )
		{
			if ( xmlRead == null ) return false;

			if ( itemParent == null )
			{
				MessageBox.Show( "Error loading inventory; no parent inventory object." );
				return false;
			}

			if ( xmlRead.EOF || 
				!xmlRead.IsStartElement() || 
				  xmlRead.IsEmptyElement )
			{
				return true; // Do nothing
			}

			//----- Get item type, type-specific properties, and create item object...
			//----- Read general properties...
			itemParent.m_strName     = xmlRead.GetAttribute( "Name" );
			itemParent.m_iID         = Convert.ToUInt32( xmlRead.GetAttribute( "ID" ) );
			itemParent.m_iTemplateID = Convert.ToUInt32( xmlRead.GetAttribute( "TemplateID" ) );
			itemParent.m_itemType    = (RpgItemType)Convert.ToUInt32( xmlRead.GetAttribute( "Type" ), 16 );
			itemParent.m_fWeight     = Convert.ToSingle( xmlRead.GetAttribute( "Weight" ) );
			itemParent.m_bySizeW     = Convert.ToByte( xmlRead.GetAttribute( "SizeW" ) );
			itemParent.m_bySizeH     = Convert.ToByte( xmlRead.GetAttribute( "SizeH" ) );
			itemParent.m_bLocked     = ( xmlRead.GetAttribute( "Locked" ) == "true" );

			// Container...?
			if ( ( itemParent.m_itemType & RpgItemType.ContEndo ) > 0 || 
					( itemParent.m_itemType & RpgItemType.ContExo ) > 0 )
			{
				RpgContainer container = (RpgContainer)itemParent;
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
							LoadItem( xmlRead, container.m_listItems[ iChildIdx ], bRecurse );
							iChildIdx++;
						}
					}
				}
			}

			// Slot...?
			else if ( ( itemParent.m_itemType & RpgItemType.ContSlot ) > 0 )
			{
				RpgContainerSlot slot = (RpgContainerSlot)itemParent;
				slot.m_itemTypeAllowed = 
					(RpgItemType)Convert.ToUInt32( xmlRead.GetAttribute( "TypeAllowed" ), 16 );

				xmlRead.Read(); // Advance to child
				if ( !xmlRead.IsEmptyElement &&
						xmlRead.IsStartElement() )
				{
					RpgItemType type = 
						(RpgItemType)Convert.ToUInt32( xmlRead.GetAttribute( "Type" ), 16 );
					
					if ( bRecurse )
					{
						slot.m_item = CreateNewItem( type );
						LoadItem( xmlRead, slot.m_item, bRecurse );
					}
				}
			}

			// Stackable item...?
			else if ( ( itemParent.m_itemType & RpgItemType.Stackable ) > 0 )
			{
				( (RpgItemStack)itemParent ).m_iStackTotal = 
					Convert.ToUInt16( xmlRead.GetAttribute( "StackTotal" ) );
				( (RpgItemStack)itemParent ).m_iStackCap   = 
					Convert.ToUInt16( xmlRead.GetAttribute( "StackCap" ) );
			}

			return true;
		} //LoadItem()


		//----------------------------------------------------------------------
		public bool Save( string strFileName, bool bRecurse = true )
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

				SaveInvItem( xmlWrite, this, bRecurse );

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
		private void SaveInvItem( XmlWriter xmlWrite, RpgItem itemInv, bool bRecurse = true )
		{
			if ( xmlWrite == null ) return;

			//TODO: Just return; only show empty slot for debugging...
			// Container slot is empty...
			if ( itemInv  == null )
			{
				xmlWrite.WriteStartElement( "Empty" );
				xmlWrite.WriteEndElement();
				return;
			}

			//TODO: Don't really need named elements, but it clarifies things...
			if ( itemInv is RpgContainerSlot )
			{
				xmlWrite.WriteStartElement( "Slot" );
			}
			else if ( itemInv is RpgContainer )
			{
				xmlWrite.WriteStartElement( "Container" );
			}
			else if ( itemInv is RpgItemStack )
			{
				xmlWrite.WriteStartElement( "ItemStack" );
			}
			else
			{
				xmlWrite.WriteStartElement( "Item" );
			}

			xmlWrite.WriteAttributeString( "Name",       itemInv.m_strName );
			xmlWrite.WriteAttributeString( "ID",         itemInv.m_iID.ToString() );
			xmlWrite.WriteAttributeString( "TemplateID", itemInv.m_iTemplateID.ToString() );
			xmlWrite.WriteAttributeString( "Type",       itemInv.m_itemType.ToString( "X" ) );
			xmlWrite.WriteAttributeString( "Weight",     itemInv.m_fWeight.ToString() );
			xmlWrite.WriteAttributeString( "SizeW",      itemInv.m_bySizeW.ToString() );
			xmlWrite.WriteAttributeString( "SizeH",      itemInv.m_bySizeH.ToString() );
			xmlWrite.WriteAttributeString( "Locked",     itemInv.m_bLocked.ToString() );

			if ( itemInv is RpgContainerSlot )
			{
				RpgContainerSlot cont = ( (RpgContainerSlot)itemInv );
				xmlWrite.WriteAttributeString( "TypeAllowed", cont.m_itemTypeAllowed.ToString( "X" ) );

				if ( bRecurse )
				{
					SaveInvItem(
						xmlWrite,
						cont.m_item,
						bRecurse );
				}
			}
			else if ( itemInv is RpgContainer )
			{
				RpgContainer cont = ( (RpgContainer)itemInv );
				xmlWrite.WriteAttributeString( "ItemTotal",   cont.m_iItemTotal.ToString() );
				xmlWrite.WriteAttributeString( "ItemCap",     cont.m_iItemCap.ToString() );
				xmlWrite.WriteAttributeString( "WeightTotal", cont.m_fWeightTotal.ToString() );
				xmlWrite.WriteAttributeString( "WeightCap",   cont.m_fWeightCap.ToString() );
				xmlWrite.WriteAttributeString( "VolTotal",    cont.m_iVolumeTotal.ToString() );
				xmlWrite.WriteAttributeString( "VolCap",      cont.m_iVolumeCap.ToString() );
				xmlWrite.WriteAttributeString( "TypeAllowed", cont.m_itemTypeAllowed.ToString( "X" ) );

				int iChildIdx = 0;
				if ( bRecurse )
				{
					while ( iChildIdx < cont.m_listItems.Capacity )
					{
						if ( iChildIdx < cont.m_listItems.Count )
						{
							SaveInvItem(
								xmlWrite,
								cont.m_listItems[ iChildIdx ],
								bRecurse );
						}
						else
						{
							//TODO: Don't need this; debugging only
							SaveInvItem( xmlWrite, null );
						}
						iChildIdx++;
					}
				}
			}
			else if ( itemInv is RpgItemStack )
			{
				xmlWrite.WriteAttributeString( "StackTotal", ( (RpgItemStack)itemInv ).m_iStackTotal.ToString() );
				xmlWrite.WriteAttributeString( "StackCap",   ( (RpgItemStack)itemInv ).m_iStackCap.ToString() );
			}

			xmlWrite.WriteFullEndElement();
		} //SaveItem()


	}
}
