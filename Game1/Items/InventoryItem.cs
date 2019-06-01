namespace Game1.Items
{
	public class InventoryItem
	{
		public Item Item { get; set; }

		public int Quantity { get; set; }

		public bool InTransition { get; set; }

		public InventoryItem(Item item, int quantity)
		{
			this.Item = item;
			this.Quantity = quantity;
			this.InTransition = false;
		}
	}
}
