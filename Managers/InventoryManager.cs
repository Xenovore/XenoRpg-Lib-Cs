#pragma warning disable RECS0091 // Use 'var' keyword when possible

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Dapper;
using XRpg.Utility;

namespace XRpg
{
	//==========================================================================
	// InventoryManager
	// • Manages all inventory item stuff
	//==========================================================================
	public class InventoryManager
	{
		public InventoryManager()
		{
		}


		//----------------------------------------------------------------------
		public List<ItemTypeDef> LoadItemTypes()
		{
			Debug.Assert( XRpgDB.Conn != null );

			const string sql =
				@"SELECT
					Name,
					HexValue,
					IconIdx
				FROM ItemTypes";

			List<ItemTypeDef> listItemTypes = XRpgDB.Conn.Query<ItemTypeDef>( sql ).AsList();

			return listItemTypes;
		}


		//----------------------------------------------------------------------
		public List<Item> LoadAllItems()
		{
			Debug.Assert( XRpgDB.Conn != null );

			List<Item> listItems;

			const string sql =
				@"SELECT
				*
				FROM Items";

			listItems = XRpgDB.Conn.Query<Item>( sql ).AsList();

			//TODO: Container

			//TODO: Effects

			return listItems;
		}

		public Item LoadContainer( uint containerId )
		{
			// This query returns *everything* in the container...
			const string sql =
				@"WITH RECURSIVE charItems (level, parentContId, itemId) AS
				(
					-- First param values (0) are just placeholders...
					-- Params: nested level, item ID, root container ID
					VALUES( 0, 0, @rootContainerId )
					UNION ALL 
					SELECT
						charItems.level + 1 as level,
						contmap.id,
						contmap.itemId
					FROM ContainerItems_map AS contmap, charItems WHERE charItems.itemId = contmap.id
					-- LIMIT is on total results, not nested level
				)
				SELECT
					charItems.level AS NestLevel,
					charItems.parentContId,
					Items.*,
					ItemContainers_ext.* FROM charItems
				JOIN items ON items.id = charItems.itemId
				JOIN ItemContainers_ext ON ItemContainers_ext.ItemId = charItems.itemId
				ORDER BY ItemId";

			List<Item> listItems =
				XRpgDB.Conn.Query<Item, ItemContainer, Item>(
					sql,
					( item, cont) => { item.Container = cont; return item; },
					new { @rootContainerId = containerId },
					splitOn: "ItemId"
				).ToList();

			var newContainer = buildInventoryTree_r( listItems[0], listItems );

			return newContainer;
		}


		private Item buildInventoryTree_r( Item parentItem, List<Item> listItems )
		{
			if ( listItems == null || !listItems.Any()) return null;

			parentItem.Container._listItems = new List<Item>();
			parentItem.Container._listItems = listItems;

			foreach ( var item in listItems )
			{
				// Is it a container...?
				if ( ( item.Type & ItemType.Container ) > 0 )
				{
					uint parentId = item.Id;

					var newCont = buildInventoryTree_r(
						item,
						listItems.FindAll( ( itm ) => itm.ParentContId == parentId ) );

					//if ( newCont != null )
					//{
					//	if ( item.Container._listItems == null )
					//	{
					//		item.Container._listItems = new List<Item>();
					//	}

					//	item.Container._listItems.Add( newCont );
					//}
				}

			}

			return listItems[0]; //FIrst in list is container?
		}


		//TODO: MOVE BACK INTO ItemContainer CLASS?
		// ----------------------------------------------------------------------
		//	LoadItemsInContainer( <item container> )
		//	• Loads items in container from DB; recurses down into all child containers
		// ----------------------------------------------------------------------
		public List<Item> LoadItemsInContainer( uint containerId, bool bRecurse = true )
		{
			List<Item> listItems = LoadItemsInContainer_r( containerId, bRecurse );

			return listItems;
		}


		// ----------------------------------------------------------------------------
		private List<Item> LoadItemsInContainer_r( uint containerId, bool bRecurse )
		{

			const string sql = @"SELECT * FROM Items WHERE ";

			List<Item> listItems = XRpgDB.Conn.Query<Item>(sql).AsList();

			if ( !bRecurse ) return listItems;

			//TODO: For each CONTAINER, recurse and get contained items
			foreach ( Item item in listItems )
			{
				listItems = LoadItemsInContainer_r( containerId, true );
			}

			return listItems;
		}
	}
}
