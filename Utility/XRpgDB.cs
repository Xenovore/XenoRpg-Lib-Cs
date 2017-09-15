using System;
using System.IO;
using System.Data.SQLite;

namespace XRpg.Utility
{
	public static class XRpgDB
	{
		//TODO: Load DB filename from config...?
		const string _strDataPath = @"Data\";
		const string _strDbFile = "xrpg.sqlite";

		static SQLiteConnection _sqlConn = null;

		public static SQLiteConnection Conn	=> _sqlConn;


		static XRpgDB()
		{
			bool bCreateTables = OpenConnection();
			if ( bCreateTables )
			{
				bool bSuccess = CreateDBTables();
				if ( !bSuccess )
				{
					throw new ApplicationException( "Failed to create DB tables!" );
				}
			}
		}


		static bool OpenConnection()
		{
			bool bCreateTables = false;
			try
			{
				if ( !Directory.Exists( _strDataPath ) )
				{
					Directory.CreateDirectory( _strDataPath );
				}

				string strDbFilePath = _strDataPath + _strDbFile;

				// Does the DB exist...?
				if ( !File.Exists( strDbFilePath ) )
				{
					// ...Nope; create it.
					SQLiteConnection.CreateFile( strDbFilePath );
					bCreateTables = true;
				}
				//NOTE: If connection pooling required, add "Pooling=True;Max Pool Size=100;"...
				string strDBSource = $"Data Source={strDbFilePath};version=3;";
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
