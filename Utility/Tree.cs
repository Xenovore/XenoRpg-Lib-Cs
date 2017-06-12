using System.Collections.Generic;

namespace Xrpg.Utility
{
	//TODO: Not used yet; use this instead of Item list in ItemContainer?
	public class TreeNode<T>
	{
		readonly List<TreeNode<T>> _listChildren = new List<TreeNode<T>>();

		T Item { get; set; }


		public TreeNode (T item)
		{
			Item = item;
		}


		public TreeNode<T> Add( T item )
		{
			TreeNode<T> nodeItem = new TreeNode<T>( item );
			_listChildren.Add(nodeItem);
			return nodeItem;
		}
	}
}
