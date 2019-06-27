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

		private static List<ItemGeneral> _generals;
		private static List<ItemConsumable> _consumables;
		private static List<ItemArmor> _armors;
		private static List<ItemWeapon> _weapons;
		private static List<ItemTool> _tools;

		static ItemManager()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			_textures = new Dictionary<string, Texture2D>();

			_generals = IOManager.ObjectFromFile<List<ItemGeneral>>(Path.Combine(Game1.MetaRoot, "items_general"));
			_consumables = IOManager.ObjectFromFile<List<ItemConsumable>>(Path.Combine(Game1.MetaRoot, "items_consumable"));
			_armors = IOManager.ObjectFromFile<List<ItemArmor>>(Path.Combine(Game1.MetaRoot, "items_armor"));
			_weapons = IOManager.ObjectFromFile<List<ItemWeapon>>(Path.Combine(Game1.MetaRoot, "items_weapon"));
			_tools = IOManager.ObjectFromFile<List<ItemTool>>(Path.Combine(Game1.MetaRoot, "items_tool"));
		}

		// TEMP...this is dumb lol...
		private static List<Item> _allItems =>	_generals.Cast<Item>()
												.Concat(_consumables.Cast<Item>())
												.Concat(_armors.Cast<Item>())
												.Concat(_weapons.Cast<Item>())
												.Concat(_tools.Cast<Item>()).ToList();

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
			var items = _allItems;

			int val = index ?? GameRandom.Next(0, items.Count - 1);
			return new InventoryItem(
				items[val],
				new ImageTexture(_textures[items[val].IconName], true) { Alignment = ImageAlignment.Centered },
				GameRandom.Next(1, items[val].MaxStackSize));
		}

		public static InventoryItem GetItem(string id, int quantity)
		{
			var items = _allItems;

			// TODO: For now, we're just using the IconName as the id lookup, but we have an actual id field (maybe should change to string)...
			var item = items.SingleOrDefault(i => i.IconName == id);
			if (item == null)
				throw new ArgumentException($"No item found with id '{id}'");
			if (quantity > item.MaxStackSize)
				throw new ArgumentException($"Invalid quantity '{quantity}' requested for item '{id}' (max: {item.MaxStackSize})");

			return new InventoryItem(item, new ImageTexture(_textures[item.IconName], true) { Alignment = ImageAlignment.Centered }, quantity);
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

		public static InventoryItem CopyItem(InventoryItem item, int? newQuantity = null)
		{
			if (item == null)
				return null;

			return new InventoryItem(item.Item, new ImageTexture(_textures[item.Item.IconName], true) { Alignment = ImageAlignment.Centered }, newQuantity ?? item.Quantity, item.Id);
		}
	}
}
