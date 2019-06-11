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
		private static Dictionary<string, ImageTexture> _icons;
		private static ContentManager _content;

		// Temp...
		private static List<Item> _items;

		static ItemManager()
		{
			_icons = new Dictionary<string, ImageTexture>();	
		}

		public static void LoadContent()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(Game1.ContentRoot, Game1.IconRoot)))
			{				
				string fileName = Path.GetFileNameWithoutExtension(file);
				_icons[fileName] = new ImageTexture(Path.Combine(Game1.IconRoot, fileName), true) { Alignment = ImageAlignment.Centered };
				_icons[fileName].LoadContent();
			}

			_items = new List<Item> {
				new ItemGeneral {	DisplayName = "Heart",			Icon = _icons["heart"],		Id = 0,	MaxStackSize = 99,	Weight = 0.4f },
				new ItemGeneral {	DisplayName = "Ruby",			Icon = _icons["gemRed"],	Id = 1,	MaxStackSize = 99,	Weight = 0.2f },
				new ItemGeneral {	DisplayName = "Health Potion",	Icon = _icons["potionRed"],	Id = 2,	MaxStackSize = 99,	Weight = 0.1f },
				// Things like metal type, etc. should be modifiers to a base type...
				new ItemArmor {		DisplayName = "Chain Helm",		Icon = _icons["helmet"],	Id = 3,	MaxStackSize = 1,	Weight = 3.2f,	Slot = ArmorSlot.Head,	Defense = 2	},
				new ItemArmor {		DisplayName = "Iron Armor",		Icon = _icons["armor"],		Id = 4,	MaxStackSize = 1,	Weight = 7.4f,	Slot = ArmorSlot.Chest, Defense = 5	},
				new ItemArmor {		DisplayName = "Gold Armor",		Icon = _icons["upg_armor"],	Id = 5,	MaxStackSize = 1,	Weight = 5.5f,	Slot = ArmorSlot.Chest, Defense = 8	}
			};
		}

		public static void UnloadContent()
		{
			foreach (var icon in _icons)
				icon.Value.UnloadContent();
		}

		// TODO: This is just a temp crap method so I don't pollute the main code with temp code....
		public static InventoryItem GetItem()
		{
			int val = GameRandom.Next(0, 5);
			return new InventoryItem(_items[val], GameRandom.Next(1, _items[val].MaxStackSize));
		}

		public static InventoryItem FromItem(InventoryItem item, int? quantity)
		{
			return new InventoryItem(item, quantity);
		}
	}
}
