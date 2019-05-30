using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
	public class ItemContainer
	{
		private readonly InventoryItem[] _items;
		
		public int Size { get; private set; }

		public ItemContainer(int size)
		{
			this.Size = size;
			_items = new InventoryItem[size];
		}

		public InventoryItem[] Items => _items;

		public InventoryItem this[int key] => _items[key];

		/// <summary>
		/// Returns new item position index (or null if it didn't fit)
		/// </summary>
		public int? AddItem(InventoryItem item)
		{
			int? position = NextEmptyPosition(0);
			if (position != null)
				_items[(int)position] = item;

			return position;
		}

		/// <summary>
		/// Will return the item previously in the specified position...
		/// </summary>
		public InventoryItem AddItem(InventoryItem item, int position)
		{
			if (!Util.InRange(position, 0, _items.Length - 1))
				throw new IndexOutOfRangeException($"Position {position} invalid for item container");

			var removedItem = _items[(int)position];
			_items[(int)position] = item;
			return removedItem;
		}

		public InventoryItem RemoveItem(int position)
		{
			var removedItem = _items[position];
			_items[position] = null;
			return removedItem;
		}

		private int? NextEmptyPosition(int start)
		{
			for (int i = start; i < _items.Length; i++)
				if (_items[i] == null)
					return i;

			return null;
		}
	}
}
