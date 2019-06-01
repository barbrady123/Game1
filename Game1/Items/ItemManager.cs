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
			Item newItem = null;

			switch (val)
			{
				case 0 : newItem = new ItemGeneral {	DisplayName = "Heart",			Icon = _icons["heart"],		Id = val,	MaxStackSize = 99,	Weight = 0.4f };	break;
				case 1 : newItem = new ItemGeneral {	DisplayName = "Ruby",			Icon = _icons["gemRed"],	Id = val,	MaxStackSize = 99,	Weight = 0.2f };	break;
				case 2 : newItem = new ItemGeneral {	DisplayName = "Health Potion",	Icon = _icons["potionRed"],	Id = val,	MaxStackSize = 99,	Weight = 0.1f };	break;
				// Things like metal type, etc. should be modifiers to a base type...
				case 3 : newItem = new ItemArmor {		DisplayName = "Chain Helm",		Icon = _icons["helmet"],	Id = val,	MaxStackSize = 1,	Weight = 3.2f };	break;
				case 4 : newItem = new ItemArmor {		DisplayName = "Iron Armor",		Icon = _icons["armor"],		Id = val,	MaxStackSize = 1,	Weight = 7.4f };	break;
				case 5 : newItem = new ItemArmor {		DisplayName = "Gold Armor",		Icon = _icons["upg_armor"],	Id = val,	MaxStackSize = 1,	Weight = 5.5f };	break;
			}

			return new InventoryItem(newItem, GameRandom.Next(1, newItem.MaxStackSize));
		}
	}
}
