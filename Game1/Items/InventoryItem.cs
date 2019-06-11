using System;

namespace Game1.Items
{
	public class InventoryItem
	{
		public Item Item { get; set; }

		public int Quantity { get; set; }

		public InventoryItem(InventoryItem item, int? quantity)
		{
			this.Item = item.Item;
			this.Quantity = quantity ?? item.Quantity;
		}

		public InventoryItem(Item item, int quantity)
		{
			this.Item = item ?? throw new ArgumentNullException(nameof(item));
			this.Quantity = quantity;
		}
	}
}
