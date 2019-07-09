using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Items
{
	public class ItemContainer : IEnumerable<InventoryItem>
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

		public IEnumerator<InventoryItem> GetEnumerator() => (IEnumerator<InventoryItem>)_items.Select(x => x).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

		public event EventHandler<ComponentEventArgs> OnItemChanged;

		public void Update(GameTime gameTime)
		{
			foreach (var item in _items)
				item?.Update(gameTime);
		}

		/// <summary>
		/// Will return the item previously in the specified position...
		/// </summary>
		public InventoryItem SwapItem(int position, InventoryItem item)
		{
			if (!Util.InRange(position, 0, _items.Length - 1))
				throw new IndexOutOfRangeException($"Position {position} invalid for item container");
			
			var currentItem = _items[position];

			if ((item != null) && (currentItem != null) && (item.Item == currentItem.Item) && (currentItem.Quantity < currentItem.Item.MaxStackSize))
			{
				// Try to combine...
				int spaceLeft = currentItem.Item.MaxStackSize - currentItem.Quantity;
				if (spaceLeft > 0)
				{
					int transferAmount = Math.Min(spaceLeft, item.Quantity);
					currentItem.Quantity += transferAmount;
					ItemChanged(position);
					item.Quantity -= transferAmount;
					return (item.Quantity > 0) ? item : null;
				}
			}

			_items[position] = item;
			ItemChanged(position);
			return currentItem;
		}

		/// <summary>
		/// Returns new item position index (or null if it didn't fit)
		/// </summary>
		public int? AddItem(InventoryItem item)
		{
			if (item == null)
				return null;

			int? openPosition = null;

			// First see if we can stack
			if (item.Item.MaxStackSize > 1)
			{
				for (int i = 0; i < _items.Length; i++)
				{
					if (_items[i] == null)
					{
						openPosition = openPosition ?? i;
						continue;
					}

					if	(_items[i].Item != item.Item)
						continue;

					int spaceLeft = _items[i].Item.MaxStackSize - _items[i].Quantity;
					int transferAmount = Math.Min(spaceLeft, item.Quantity);
					_items[i].Quantity += transferAmount;
					ItemChanged(i);
					item.Quantity -= transferAmount;
					if (item.Quantity == 0)
						return i;
				}
			}

			int? position = openPosition ?? NextEmptyPosition(0);
			if (position != null)
			{
				_items[(int)position] = item;
				ItemChanged((int)position);
			}

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
				ItemChanged((int)position);
				item.Quantity -= transferAmount;
				return (item.Quantity > 0) ? item : null;
			}

			_items[(int)position] = item;
			ItemChanged((int)position);
			return currentItem;
		}

		public InventoryItem RemoveItem(int position)
		{
			var removedItem = _items[position];
			_items[position] = null;
			ItemChanged(position);
			return removedItem;
		}

		private int? NextEmptyPosition(int start)
		{
			for (int i = start; i < _items.Length; i++)
				if (_items[i] == null)
					return i;

			return null;
		}

		private void ItemChanged(int index)
		{
			OnItemChanged?.Invoke(this, new ComponentEventArgs { Index = index, Meta = _items[index] });
		}
}
}
