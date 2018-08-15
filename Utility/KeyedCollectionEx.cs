//==============================================================================
// KeyedCollectionEx
// • Works like dictionary but allows specifying a class property as the key
// • Use lambda expression to set class property, e.g.:
//   var  _collectionItemTypes = new KeyedCollectionEx<string, ItemTypeClass>( itemType => itemType.Name );
//==============================================================================
using System;
using System.Collections.ObjectModel;

namespace XRpg.Utility
{
	public class KeyedCollectionEx<TKey, TItem> : KeyedCollection<TKey, TItem>
	{
		Func<TItem, TKey> _getKeyForItemDelegate;

		protected override TKey GetKeyForItem( TItem item ) => _getKeyForItemDelegate( item );


		public KeyedCollectionEx( Func<TItem, TKey> getKeyForItemDelegate ) : base()
		{
			if ( getKeyForItemDelegate == null )
				throw new ArgumentNullException( nameof( getKeyForItemDelegate ), "Null delegate!" );

			_getKeyForItemDelegate = getKeyForItemDelegate;
		}
	}
}
