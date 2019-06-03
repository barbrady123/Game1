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

namespace Game1
{
	public class World
	{
		public List<NPC> NPCs { get; set; }
		public Character Character { get; set; }
		public Map CurrentMap { get; set; }

		public List<Character> AllCharacters => new List<Character>((this.NPCs?.Count ?? 0) + 1) { this.Character }.Concat(this.NPCs).ToList();

		public void Initialize()
		{
			// TODO: All this crap should be coming from load game files, map files, etc...
			this.CurrentMap = IOManager.ObjectFromFile<Map>(Game1.MapFile);
			this.CurrentMap.GenerateTiles();
			this.Character = IOManager.ObjectFromFile<Character>(Game1.PlayerFile);
			this.Character.Strength = GameRandom.Next(10, 20);
			this.Character.Dexterity = GameRandom.Next(10, 20);
			this.Character.Intelligence = GameRandom.Next(10, 20);
			this.Character.Wisdom = GameRandom.Next(10, 20);
			this.Character.Charisma = GameRandom.Next(10, 20);
			this.Character.Constitution = GameRandom.Next(10, 20);

			CharacterSex oppositeSex = (this.Character.Sex == CharacterSex.Male) ? CharacterSex.Female : CharacterSex.Male;
			this.NPCs = new List<NPC> {
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (128.0f, 128.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (256.0f, 256.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (1024.0f, 1024.0f) }
			};
		}

		public void LoadContent()
		{
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.Backpack.AddItem(ItemManager.GetItem());
			this.Character.Backpack.AddItem(ItemManager.GetItem());
			this.Character.Backpack.AddItem(ItemManager.GetItem());
			this.Character.Backpack.AddItem(ItemManager.GetItem());
			this.Character.Backpack.AddItem(ItemManager.GetItem());
			this.Character.EquippedArmorHead = ItemManager.GetItem();
			this.Character.EquippedArmorChest = ItemManager.GetItem();
			//this.Character.EquippedArmorLegs = ItemManager.GetItem();
			this.Character.EquippedArmorFeet = ItemManager.GetItem();
		}

		public void UnloadContent()
		{

		}

		public void Update(GameTime gameTime)
		{
			this.CurrentMap.Update(gameTime);
			this.Character.Update(gameTime);
			foreach (var npc in this.NPCs)
				npc.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{

		}
	}
}
