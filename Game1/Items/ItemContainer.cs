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
		private int _activeItemIndex;
		
		public int Size { get; private set; }

		
		public InventoryItem ActiveItem => _items[this.ActiveItemIndex];

		public int ActiveItemIndex
		{
			get { return _activeItemIndex; }
			set { _activeItemIndex = Util.Clamp(value, 0, _items.Length - 1); }
		}

		public ItemContainer(int size)
		{
			this.Size = size;
			_items = new InventoryItem[size];
			this.ActiveItemIndex = 0;
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
			
			var currentItem = _items[(int)position];

			if ((item != null) && (currentItem != null) && (item.Item == currentItem.Item) && (currentItem.Quantity < currentItem.Item.MaxStackSize))
			{
				// Try to combine...
				int spaceLeft = currentItem.Item.MaxStackSize - currentItem.Quantity;
				int transferAmount = Math.Min(spaceLeft, item.Quantity);
				currentItem.Quantity += transferAmount;
				item.Quantity -= transferAmount;
				return (item.Quantity > 0) ? item : null;
			}

			_items[(int)position] = item;
			return currentItem;
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
