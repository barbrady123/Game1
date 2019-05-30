using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
	/// <summary>
	/// This will handle the main game "inventory" tables, loading from data files, could handle random loot tables/generation, etc...
	/// Should also be the factory for all objects...should be NO new of items outside this class...
	/// </summary>
	public static class ItemManager
	{
		// TODO: This is just a temp crap method so I don't pollute the main code with temp code....
		public static InventoryItem GetItem()
		{
			int val = GameRandom.Next(0, 5);
			Item newItem = null;

			switch (val)
			{
				case 0 : newItem = new ItemGeneral { DisplayName = "Rock", Id = val, MaxStackSize = 99, Weight = 0.4f };			break;
				case 1 : newItem = new ItemGeneral { DisplayName = "Stick", Id = val, MaxStackSize = 99, Weight = 0.2f };			break;
				case 2 : newItem = new ItemGeneral { DisplayName = "Marble", Id = val, MaxStackSize = 99, Weight = 0.1f };			break;
				// Things like metal type, etc. should be modifiers to a base type...
				case 3 : newItem = new ItemArmor { DisplayName = "Chain Helm", Id = val, MaxStackSize = 1, Weight = 3.2f };		break;
				case 4 : newItem = new ItemArmor { DisplayName = "Iron Chestplate", Id = val, MaxStackSize = 1, Weight = 7.4f };	break;
				case 5 : newItem = new ItemArmor { DisplayName = "Gold Sabatons", Id = val, MaxStackSize = 1, Weight = 5.5f };		break;
			}

			return new InventoryItem { Item = newItem, Quantity = GameRandom.Next(1, newItem.MaxStackSize) };
		}
	}
}
