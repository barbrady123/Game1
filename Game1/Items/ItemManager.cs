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
		public const string MetaItemRoot = Game1.MetaRoot + "\\Item";

		// TODO: This will eventually be much more organized/structured by item "type", etc...
		private static List<ItemGeneral> _generals;
		private static List<ItemConsumable> _consumables;
		private static List<ItemArmor> _armors;
		private static List<ItemWeapon> _weapons;
		private static List<ItemTool> _tools;

		static ItemManager()
		{
			_generals = IOManager.ObjectFromFile<List<ItemGeneral>>(Path.Combine(MetaItemRoot, "items_general"));
			_consumables = IOManager.ObjectFromFile<List<ItemConsumable>>(Path.Combine(MetaItemRoot, "items_consumable"));
			_armors = IOManager.ObjectFromFile<List<ItemArmor>>(Path.Combine(MetaItemRoot, "items_armor"));
			_weapons = IOManager.ObjectFromFile<List<ItemWeapon>>(Path.Combine(MetaItemRoot, "items_weapon"));
			_tools = IOManager.ObjectFromFile<List<ItemTool>>(Path.Combine(MetaItemRoot, "items_tool"));
		}

		// TEMP...this is dumb lol...
		private static List<Item> _allItems =>	_generals.Cast<Item>()
												.Concat(_consumables.Cast<Item>())
												.Concat(_armors.Cast<Item>())
												.Concat(_weapons.Cast<Item>())
												.Concat(_tools.Cast<Item>()).ToList();

		// TODO: This is just a temp crap method so I don't pollute the main code with temp code....
		public static InventoryItem GetItem(int? index = null)
		{
			var items = _allItems;

			int val = index ?? GameRandom.Next(0, items.Count - 1);
			return new InventoryItem(
				items[val],
				AssetManager.GetIconItem(items[val].IconName),
				GameRandom.Next(1, items[val].MaxStackSize));
		}

		public static InventoryItem GetItem(string id, int quantity)
		{
			var items = _allItems;

			var item = items.SingleOrDefault(i => i.Id == id);
			if (item == null)
				throw new ArgumentException($"No item found with id '{id}'");
			if (quantity > item.MaxStackSize)
				throw new ArgumentException($"Invalid quantity '{quantity}' requested for item '{id}' (max: {item.MaxStackSize})");

			return new InventoryItem(item, AssetManager.GetIconItem(item.IconName), quantity);
		}

		public static InventoryItem FromItem(InventoryItem item, int? quantity, bool removeItemQuantity = true)
		{
			int trueQuantity = Math.Min(quantity ?? Int32.MaxValue, item.Quantity);
			if (removeItemQuantity)
				item.Quantity -= trueQuantity;
			return new InventoryItem(
				item.Item,
				AssetManager.GetIconItem(item.Item.IconName),
				trueQuantity);
		}

		public static InventoryItem CopyItem(InventoryItem item, int? newQuantity = null)
		{
			if (item == null)
				return null;

			return new InventoryItem(item.Item, AssetManager.GetIconItem(item.Item.IconName), newQuantity ?? item.Quantity, item.Id);
		}
	}
}
