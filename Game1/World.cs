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
	public class World : Component
	{
		private readonly PhysicsManager _physics;

		public List<NPC> NPCs { get; set; }
		public Character Character { get; set; }
		public Map CurrentMap { get; set; }
		// Testing this out as objects that sit directly on the map....
		public List<WorldItem> Items{ get; set; }

		public List<Character> AllCharacters => new List<Character>((this.NPCs?.Count ?? 0) + 1) { this.Character }.Concat(this.NPCs).ToList();

		public event EventHandler<ComponentEventArgs> OnItemsChange;

		public World()
		{
			_physics = new PhysicsManager(this);
		}

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
			this.Character.MaxHP = GameRandom.Next(30, 50);
			this.Character.CurrentHP = this.Character.MaxHP / 2;
			this.Character.MaxMana = GameRandom.Next(30, 50);
			this.Character.CurrentMana = this.Character.MaxMana;

			CharacterSex oppositeSex = (this.Character.Sex == CharacterSex.Male) ? CharacterSex.Female : CharacterSex.Male;
			this.NPCs = new List<NPC> {
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (128.0f, 128.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (256.0f, 256.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (1024.0f, 1024.0f) }
			};
		}

		public override void LoadContent()
		{
			// Obviously none of this crap should be here...just for testing purposes...
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem(6));
			for (int i = 0; i < 15; i++)
				this.Character.Backpack.AddItem(ItemManager.GetItem());
			this.Items = new List<WorldItem> {
				new WorldItem { Pickup = true, Item = ItemManager.GetItem(), Position = new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100)) },
				new WorldItem { Pickup = true, Item = ItemManager.GetItem(), Position = new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100)) },
				new WorldItem { Pickup = true, Item = ItemManager.GetItem(), Position = new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100)) },
				new WorldItem { Pickup = true, Item = ItemManager.GetItem(), Position = new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100)) },
				new WorldItem { Pickup = true, Item = ItemManager.GetItem(), Position = new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100)) }
			};

			// This will need to be redone if the map changes....
			_physics.CalculateParameters();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			this.CurrentMap.Update(gameTime);
			this.Character.Update(gameTime);
			foreach (var npc in this.NPCs)
				npc.Update(gameTime);
			_physics.Update(gameTime);
			foreach (var item in this.Items)
				item.Update(gameTime);

			// I think it makes sense to put things like "Item pickup" from proximity after the physics update?
			// I'm not 100% sure where i even want this to live yet or what entity's responsibility this should be
			List<WorldItem> removedItems = new List<WorldItem>();

			foreach (var item in this.Items)
			{
				item.InRange = Vector2.Distance(item.Position, this.Character.Position) <= Game1.DefaultPickupRadius;

				if (item.Pickup && item.InRange)
				{
					if (this.Character.AddItem(item.Item))
						removedItems.Add(item);
				}
			}

			if (removedItems.Any())
			{
				this.Items.RemoveAll(x => removedItems.Contains(x));
				OnItemsChange?.Invoke(this, null);
			}
		}

		public override void Draw(SpriteBatch spriteBatch) { }

		public void AddItem(InventoryItem item, Vector2? position = null, bool pickup = true)
		{
			if (item == null)
				return;

			position = position ?? this.Character.Position;
			this.Items.Add(new WorldItem { Item = item, Position = (Vector2)position, Pickup = pickup });
			OnItemsChange?.Invoke(this, null);
		}
	}
}
