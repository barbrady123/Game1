using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1.Items
{
	/// <summary>
	/// This will handle the main game "inventory" tables, loading from data files, could handle random loot tables/generation, etc...
	/// Should also be the factory for all objects...should be NO new of items outside this class...
	/// </summary>
	public static class ItemManager
	{
		// TODO: This will eventually be much more organized/structured by item "type", etc...
		// For now, we just have a bunch of random item icons in the root folder...
		private static Dictionary<string, Texture2D> _textures;
		private static ContentManager _content;

		// Temp...
		private static List<Item> _items;

		static ItemManager()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			_textures = new Dictionary<string, Texture2D>();

			// Just load some temp data for testing...
			_items = new List<Item> {
				new ItemGeneral {	DisplayName = "Heart",			IconName = "heart",		Id = 0,	MaxStackSize = 99,	Weight = 0.4f },
				new ItemGeneral {	DisplayName = "Ruby",			IconName = "gemRed",	Id = 1,	MaxStackSize = 99,	Weight = 0.2f },
				new ItemGeneral {	DisplayName = "Health Potion",	IconName = "potionRed",	Id = 2,	MaxStackSize = 99,	Weight = 0.1f },
				// Things like metal type, etc. should be modifiers to a base type...
				new ItemArmor {		DisplayName = "Chain Helm",		IconName = "helmet",	Id = 3,	MaxStackSize = 1,	Weight = 3.2f,	Slot = ArmorSlot.Head,	Defense = 2	},
				new ItemArmor {		DisplayName = "Iron Armor",		IconName = "armor",		Id = 4,	MaxStackSize = 1,	Weight = 6.4f,	Slot = ArmorSlot.Chest, Defense = 5	},
				new ItemArmor {		DisplayName = "Gold Armor",		IconName = "upg_armor",	Id = 5,	MaxStackSize = 1,	Weight = 8.5f,	Slot = ArmorSlot.Chest, Defense = 8	},
				new ItemWeapon {	DisplayName = "Iron Sword",		IconName = "sword",		Id = 6, MaxStackSize = 1,	Weight = 2.9f,	MaxDamage = 10 }
			};
		}

		public static void LoadContent()
		{
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(Game1.ContentRoot, Game1.IconRoot)))
			{				
				string fileName = Path.GetFileNameWithoutExtension(file);
				_textures[fileName] = _content.Load<Texture2D>(Path.Combine(Game1.IconRoot, fileName));
			}
		}

		public static void UnloadContent()
		{
			_content.Unload();
		}

		// TODO: This is just a temp crap method so I don't pollute the main code with temp code....
		public static InventoryItem GetItem(int? index = null)
		{
			int val = index ?? GameRandom.Next(0, 6);
			return new InventoryItem(
				_items[val],
				new ImageTexture(_textures[_items[val].IconName], true) { Alignment = ImageAlignment.Centered },
				GameRandom.Next(1, _items[val].MaxStackSize));
		}

		public static InventoryItem FromItem(InventoryItem item, int? quantity, bool removeItemQuantity = true)
		{
			int trueQuantity = Math.Min(quantity ?? Int32.MaxValue, item.Quantity);
			if (removeItemQuantity)
				item.Quantity -= trueQuantity;
			return new InventoryItem(
				item.Item,
				new ImageTexture(_textures[item.Item.IconName], true) { Alignment = ImageAlignment.Centered },
				trueQuantity);
		}

		public static InventoryItem CopyItem(InventoryItem item)
		{
			if (item == null)
				return null;

			return new InventoryItem(item.Item, new ImageTexture(_textures[item.Item.IconName], true) { Alignment = ImageAlignment.Centered }, item.Quantity, item.Id);
		}
	}
}
