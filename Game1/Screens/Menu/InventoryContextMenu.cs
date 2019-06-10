using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Items;
using Game1.Screens.Menu;

namespace Game1.Screens.Menu
{
	public class InventoryContextMenu : MenuScreen
	{
		public object Owner { get; private set; }

		private readonly InventoryItem _item;
		private readonly bool _isEquipped;

		public InventoryContextMenu(object owner, 
									Point position,
									InventoryItem item,
									bool isEquipped) : base(InventoryContextMenu.CalculateItemMenuBounds(position, item, isEquipped), background: "black")
		{
			this.Owner = owner;
			_item = item;
			_isEquipped = isEquipped;
		}

		protected override List<MenuItem> GetItemData()
		{
			return InventoryContextMenu.GetItemMenuOptions(_item, _isEquipped).Select(o => new MenuItem { Id = o.ToLower(), Text = o }).ToList();
		}

		private static List<string> GetItemMenuOptions(InventoryItem item, bool isEquipped)
		{
			var items = new List<string>();

			switch (item?.Item)
			{
				case ItemWeapon weapon:
				case ItemArmor armor:
					items.Add(isEquipped ? "Unequip" : "Equip");												
					break;
				case ItemConsumable consumable:
					items.Add("Eat");	// eventually we should have a ConsumableType so this can be eat or drink...
					break;
			}

			if (item.Quantity > 1)
				items.Add("Split");

			items.Add("Cancel");

			return items;
		}

		private static Rectangle CalculateItemMenuBounds(Point position, InventoryItem item, bool isEquipped)
		{
			var size = MenuScreen.CalculateMenuSize(MenuScreen.MENU_PADDING, MenuScreen.MENU_PADDING, InventoryContextMenu.GetItemMenuOptions(item, isEquipped), MenuLayout.Vertical);
			return new Rectangle(position.X, position.Y, size.Width, size.Height);
		}

		protected override void CurrentItemChange(ComponentEventArgs e)
		{
			e.Sender = this.Owner;
			base.CurrentItemChange(e);
		}

		protected override void ItemSelect(ComponentEventArgs e)
		{
			e.Sender = this.Owner;
			base.ItemSelect(e);
		}
	}
}
