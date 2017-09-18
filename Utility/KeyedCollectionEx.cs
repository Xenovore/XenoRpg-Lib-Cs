//==============================================================================
// KeyedCollectionEx
// • Works like dictionary but allows specifying a class property as the key
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
