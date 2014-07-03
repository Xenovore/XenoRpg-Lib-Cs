using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//TODO: Remove, only needed for temp message box...
using System.Windows.Forms;

namespace KevInvTest.Source
{
	// Slot can only hold one item at a time; only used for character equipped items.
	// Works like container without overhead of tracking multiple items, weight totals, etc.

	public class RpgContainerSlot: RpgContainer
	{
		public RpgItem m_item;

		public RpgContainerSlot()
		{
			m_itemType = RpgItemType.ContSlot;
		}


		public ItemAddResult CanAddX( RpgItem item )
		{
			ItemAddResult addRes;

			// Check if item exists...
			if ( item == null ) addRes = ItemAddResult.NullItem;

			// Check item type...
			else if ( ( item.m_itemType & m_itemTypeAllowed ) == 0 )
			{
				addRes = ItemAddResult.NotAllowed;
			}

			// Make sure slot is empty...
			else if ( m_item != null ) addRes = ItemAddResult.NotEmpty;
			else addRes = ItemAddResult.Allowed;

			return addRes;
		}


		//----------------------------------------------------------------------
		// AddItem()
		// • Member of KItemContainer -- can only add items to a container...
		//TODO: Except if items are stackable, it should add to stack
		//----------------------------------------------------------------------
		public ItemAddResult AddItemX( RpgItem item )
		{
			ItemAddResult addRes = CanAdd( item );
			if ( addRes != ItemAddResult.Allowed )
			{
				//TODO: remove temp message box...
				MessageBox.Show( GetAddResultText( addRes, item.m_strName ) );

				return addRes;
			}

			//TODO: Check for existing stacks; add to stacked item when possible
			m_item = item;

			return ItemAddResult.Allowed;
		}


		//----------------------------------------------------------------------
		public bool RemoveItemByNameX( string strName, bool bRecurse = false )
		{
			if ( strName == m_item.m_strName )
			{
				//m_iVolumeTotal -= m_item.m_bySizeW * m_item.m_bySizeH;
				//m_fWeightTotal -= m_item.m_fWeight;
				m_item = null;
				return true;
			}
			else if ( ( bRecurse ) && ( m_item is RpgContainer ) )
			{
				bool bRemoved = 
					( (RpgContainer)m_item ).RemoveItemByName(
						strName,
						bRecurse );
				if ( bRemoved ) return bRemoved;
			}
			else if ( ( bRecurse ) && ( m_item is RpgContainerSlot ) )
			{
				bool bRemoved = 
					( (RpgContainerSlot)m_item ).RemoveItemByName(
						strName,
						bRecurse );
				if ( bRemoved ) return bRemoved;
			}

			return false; // not found
		}


		//----------------------------------------------------------------------
		// SwapItem()
		// • Swaps out current item with new item
		//----------------------------------------------------------------------
		public ItemAddResult SwapItem( RpgItem item )
		{
			ItemAddResult addRes = CanAdd( item );
			if ( addRes != ItemAddResult.Allowed )
			{
				return addRes;
			}

			RpgItem itemPrev = m_item;
			m_item = item;
			item = itemPrev;

			return ItemAddResult.Allowed;
		}
	}
}
