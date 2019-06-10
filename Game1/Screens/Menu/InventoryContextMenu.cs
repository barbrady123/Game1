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
using Game1.Interface;
using Game1.Items;
using Game1.Screens.Menu;

namespace Game1.Screens.Menu
{
	public class InventoryContextMenu : MenuScreen
	{
		public InventoryItemView Owner { get; private set; }

		private bool _isEquipped;

		public InventoryContextMenu() : base(Rectangle.Empty, background: "black")
		{
			// We allow empty instanciation so the object can be registered with a ComponentManager if necessary...
		}

		public void Initialize(InventoryItemView owner, Point position, bool isEquipped)
		{
			this.Owner = owner;
			this.Bounds = InventoryContextMenu.CalculateItemMenuBounds(position, owner?.Item, isEquipped);
			_isEquipped = isEquipped;

			UnloadContent();

			// Setup Component stuff (background/border)
			SetupBackground();
			SetupBorder();
			RepositionObjects();

			// Setup MenuScreen stuff (items)
			_items = GetItemData();

			LoadContent();
		}

		public void Clear()
		{
			this.Owner = null;
			this.Bounds = Rectangle.Empty;
			UnloadContent();
			_background = null;
			_border = null;
			_items.ForEach(x => x.Image.UnloadContent());
			_items.Clear();
		}

		protected override List<MenuItem> GetItemData()
		{
			return InventoryContextMenu.GetItemMenuOptions(this.Owner?.Item, _isEquipped).Select(o => new MenuItem { Id = o.ToLower(), Text = o }).ToList();
		}

		private static Rectangle CalculateItemMenuBounds(Point position, InventoryItem item, bool isEquipped)
		{
			if ((position == Util.PointInvalid) || (item == null))
				return Rectangle.Empty;

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
			InputManager.BlockAllInput();
		}

		private static List<string> GetItemMenuOptions(InventoryItem item, bool isEquipped)
		{
			var items = new List<string>();
			if (item == null)
				return items;

			switch (item.Item)
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
	}
}
