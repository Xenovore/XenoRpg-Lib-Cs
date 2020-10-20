//==============================================================================
// XRpgDB
// • Static class provides a SQLite DB connection specifically for "xrpg.sqlite" DB.
// • Use it like this; e.g. "XRpgDb.Conn.Query<Item>( sql )"
//==============================================================================

using System;
using System.Data.SQLite;
using System.IO;

namespace XRpg.Utility
{
	public static class XRpgDB
	{
		const string _xRpgDbLocation = "Data/xrpg.sqlite";

		static SQLiteConnection _sqlConn = null;

		//TODO: Make this more robust; if _sqlConn is null, call OpenConnection()
		public static SQLiteConnection Conn	=> _sqlConn;


		static XRpgDB()
		{
		}


		public static bool Init()
		{
			// Don't open another connection if we already have one...
			if ( _sqlConn != null ) return true;

			bool bCreateTables = OpenConnection();
			if ( bCreateTables )
			{
				bool bSuccess = CreateDBTables();
				if ( !bSuccess )
				{
					throw new ApplicationException( "Failed to create DB tables!" );
				}
			}

			// For now, just return true; any errors will result in an exception
			return true;
		}


		static bool OpenConnection()
		{
			bool bCreateTables = false;
			try
			{
				//TODO: Someday, if it seems worth it, get DB path from config

				//NOTE: SQLite will auto-create the file if it doesn't exist.
				// But, need to know if it's necessary to create the tables...
				if ( !File.Exists( _xRpgDbLocation ) )
				{
					// ...and might as well explicitly create it.
					SQLiteConnection.CreateFile( _xRpgDbLocation );
					bCreateTables = true;
				}

				//NOTE: If connection pooling required, add "Pooling=True;Max Pool Size=100;"...
				string strDBSource = $"Data Source={_xRpgDbLocation};version=3;";
				_sqlConn = new SQLiteConnection( strDBSource );
				_sqlConn.Open();
			}
			catch ( Exception e )
			{
				// Most likely issue here; sqlite.interop.dll is not where it's supposed to be.

				throw new ApplicationException( e.Message, e);
			}

			return bCreateTables;
		}


		//----------------------------------------------------------------------
		static bool CreateDBTables()
		{
			//TODO: Put SQL to create tables in here

			// For now, we'll assume tables already exist and are populated
			return true;
		}



	}
}
