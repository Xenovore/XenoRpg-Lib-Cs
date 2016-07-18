#pragma warning disable RECS0091 // Use 'var' keyword when possible

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Data.SQLite;
using Dapper;


namespace XRpg
{
	//==========================================================================
	// InventoryManager
	// • Manages all inventory item stuff
	//==========================================================================
	public class InventoryManager
	{

		static SQLiteConnection _sqlConn;

		//TODO: probably don't want a public SQL connection... remove
		static SQLiteConnection SQLConn
		{
			get { return _sqlConn; }
		}


		public InventoryManager()
		{
		}


		//TODO: MOVE OUT
		//----------------------------------------------------------------------
		public bool InitDB()
		{
			//TO DO: Load DB filename from config...?
			const string strDataPath = @"Data";
			const string strDbFile = "xrpg.sqlite";

			if ( !Directory.Exists( strDataPath ) )
			{
				Directory.CreateDirectory( strDataPath );
			}

			string strDbFilePath = strDataPath + '\\' + strDbFile;

			// Does the DB exist...?
			if ( !File.Exists( strDbFilePath ) )
			{
				// ...Nope; create it.
				SQLiteConnection.CreateFile( strDbFilePath );
				bool bSuccess = CreateAllItemTables();
				if ( !bSuccess ) return false;
			}

			//NOTE: If connection pooling required, add "Pooling=True;Max Pool Size=100;"...
			string strDBSource =
				string.Format( @"Data Source={0};version=3;", strDbFilePath );

			try
			{
				_sqlConn = new SQLiteConnection( strDBSource );
				_sqlConn.Open();
			}
			catch ( Exception e )
			{
				// Most likely issue here; sqlite.interop.dll is not where it's supposed to be.

				throw new ApplicationException( e.Message, e);
				return false;
			}

			return true;
		}


		//TODO: MOVE OUT
		//----------------------------------------------------------------------
		bool CreateAllItemTables()
		{
			//TODO: Put SQL to create tables in here
			return true;
		}


		//----------------------------------------------------------------------
		public List<ItemTypeDef> LoadItemTypes()
		{
			Debug.Assert( _sqlConn != null );

			const string sql =
				@"SELECT
					Name,
					HexValue,
					IconIdx
				FROM ItemTypes";

			List<ItemTypeDef> listItemTypes = _sqlConn.Query<ItemTypeDef>( sql ).ToList();

			return listItemTypes;
		}


		//----------------------------------------------------------------------
		public List<Item> LoadAllItems()
		{
			Debug.Assert( _sqlConn != null );

			List<Item> listItems;

			const string sql =
				@"SELECT
				*
				FROM Items";

			listItems = _sqlConn.Query<Item>( sql ).ToList();

			//TODO: Container

			//TODO: Effects

			return listItems;
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

			List<Item> listItems = _sqlConn.Query<Item>(sql).ToList();

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
