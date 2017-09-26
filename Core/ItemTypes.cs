using System;
using System.Collections.Generic;
using System.Diagnostics;

using Dapper;
using XRpg.Utility;

namespace XRpg
{
	public static class ItemTypes
	{
		// Used to cache results...
		static KeyedCollectionEx<string, ItemTypeClass> _collectionItemTypes;


		static ItemTypes()
		{
			LoadItemTypes();
		}


		static public void Reload()
		{
			LoadItemTypes( true );
		}


		static private KeyedCollectionEx<string, ItemTypeClass> LoadItemTypes( bool forceReload = false )
		{
			Debug.Assert( XRpgDB.Conn != null );

			if ( forceReload ||
				 _collectionItemTypes == null ||
				 _collectionItemTypes?.Count == 0 )
			{
				const string sql =
					@"SELECT
						Name,
						Value,
						IconIdx
					FROM ItemTypes";

				//TODO: Need custom mapper for KeyedCollectionEx; for now use List and convert...
				List<ItemTypeClass> listItemTypes = XRpgDB.Conn.Query<ItemTypeClass>( sql ).AsList();

				if ( _collectionItemTypes == null )
				{
					_collectionItemTypes = new KeyedCollectionEx<string, ItemTypeClass>( itemType => itemType.Name );
				}
				else
				{
					_collectionItemTypes.Clear();
				}

				foreach ( var itemType in listItemTypes )
				{
					_collectionItemTypes.Add( itemType );
				}
			}

			return _collectionItemTypes;
		}


		static public ulong GetValue( string key )
		{
			if ( key == null ) return 0;

			return _collectionItemTypes[key].Value;
		}
	}
}
