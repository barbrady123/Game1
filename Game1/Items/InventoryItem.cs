using System;
using Microsoft.Xna.Framework;

namespace Game1.Items
{
	public class InventoryItem
	{
		public Guid Id { get; set; }

		public Item Item { get; set; }

		public ImageTexture Icon { get; set; }

		public int Quantity { get; set; }

		public InventoryItem(Item item, ImageTexture icon, int quantity, Guid? id = null)
		{
			this.Id = id ?? Guid.NewGuid();
			this.Item = item ?? throw new ArgumentNullException(nameof(item));
			this.Icon = icon;
			this.Icon.LoadContent();
			this.Quantity = quantity;
		}

		public void Update(GameTime gameTime)
		{
			this.Icon.Update(gameTime);
		}
	}
}
