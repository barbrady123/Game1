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

		public InventoryContextMenu(SpriteBatchData spriteBatchData = null) : base(Rectangle.Empty, background: "black", spriteBatchData: spriteBatchData, drawIfDisabled: false)
		{
			// We allow empty instanciation so the object can be registered with a ComponentManager if necessary...
		}

		public void Initialize(InventoryItemView owner, Point position, bool isEquipped)
		{
			UnloadContent();

			this.Owner = owner;
			this.Bounds = InventoryContextMenu.CalculateItemMenuBounds(position, owner?.Item, isEquipped);
			_isEquipped = isEquipped;

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
			_mouseover = false;
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

		protected override void ItemSelect(ComponentEventArgs e)
		{
			e.Meta = this.Owner;
			base.ItemSelect(e);
		}

		private static List<string> GetItemMenuOptions(InventoryItem item, bool isEquipped)
		{
			var items = new List<string>();
			if (item == null)
				return items;

			switch (item.Item)
			{
				case ItemArmor armor:
					items.Add(isEquipped ? "Unequip" : "Equip");			
					break;
				case ItemConsumable consumable:
					switch (consumable.Type)
					{
						case ConsumableType.Food : items.Add("Eat");		break;
						case ConsumableType.Potion : items.Add("Drink");	break;
					}
					break;
			}

			if (item.Quantity > 1)
				items.Add("Split");

			items.Add("Drop");
			items.Add("Cancel");

			return items;
		}
	}
}
