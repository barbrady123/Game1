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
		private bool _mouseInWorld;

		public List<NPC> NPCs { get; set; }
		public Character Character { get; set; }
		public Map CurrentMap { get; set; }
		// Testing this out as objects that sit directly on the map....
		public List<WorldItem> Items { get; set; }

		// Testing this out as interactives that sit directly on the map...
		public List<WorldInteractive> Interactives { get; set; }

		public List<Character> AllCharacters => new List<Character>((this.NPCs?.Count ?? 0) + 1) { this.Character }.Concat(this.NPCs).ToList();

		public event EventHandler<ComponentEventArgs> OnItemsChange;
		// This should probably be a more generalized event for character events...
		public event EventHandler<ComponentEventArgs> OnCharacterDied;

		public World()
		{
			_physics = new PhysicsManager(this);
			_mouseInWorld = false;
		}

		public void Initialize()
		{
			// TODO: All this crap should be coming from load game files, map files, etc...
			this.CurrentMap = IOManager.ObjectFromFile<Map>(Game1.MapFile);
			this.CurrentMap.GenerateTiles();
			this.Character = IOManager.ObjectFromFile<Character>(Game1.PlayerFile);
			this.Character.OnDied += Character_OnDied;
			this.Character.Strength = GameRandom.Next(10, 20);
			this.Character.Dexterity = GameRandom.Next(10, 20);
			this.Character.Intelligence = GameRandom.Next(10, 20);
			this.Character.Wisdom = GameRandom.Next(10, 20);
			this.Character.Charisma = GameRandom.Next(10, 20);
			this.Character.Constitution = GameRandom.Next(10, 20);
			this.Character.MaxHP = GameRandom.Next(30, 50);
			this.Character.CurrentHP = 1;	//this.Character.MaxHP / 2;
			this.Character.MaxMana = GameRandom.Next(30, 50);
			this.Character.CurrentMana = this.Character.MaxMana;

			CharacterSex oppositeSex = (this.Character.Sex == CharacterSex.Male) ? CharacterSex.Female : CharacterSex.Male;
			this.NPCs = new List<NPC> {
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (128.0f, 128.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (256.0f, 256.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (1024.0f, 1024.0f) }
			};
		}

		private void Interactive_OnDestroyed(object sender, EventArgs e)
		{
			// Another thing that shouldn't be here...testing...
			var interactive = (WorldInteractive)sender;
			this.Interactives.Remove(interactive);
			// Need something to generate loot from loot table...just doing this temp...
			foreach (var l in interactive.Interactive.LootTable)
			{
				if (GameRandom.Next(0, 99) < l.Odds)
				{
					// Makes no sense....
					var newItem = ItemManager.GetItem(l.ItemPool.First());
					newItem.Quantity = GameRandom.Next(l.MinQuantity, l.MaxQuantity);
					AddItem(newItem, interactive.Position, true);
				}
			}
		}

		// Eventually make this more generalized event
		private void Character_OnDied(object sender, ComponentEventArgs e)
		{
			OnCharacterDied?.Invoke(this, e);
		}

		public override void LoadContent()
		{
			this.Character.LoadContent();
			foreach (var npc in this.NPCs)
				npc.LoadContent();

			// Obviously none of this crap should be here...just for testing purposes...
			this.Character.HotBar.AddItem(ItemManager.GetItem(10));
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem());
			this.Character.HotBar.AddItem(ItemManager.GetItem(6));
			for (int i = 0; i < 15; i++)
				this.Character.Backpack.AddItem(ItemManager.GetItem((i < 11) ? i : (int?)null));

			this.Items = new List<WorldItem>();
			
			for (int i = 0; i < 30; i++)
				this.Items.Add(
					new WorldItem { Pickup = true, Item = ItemManager.GetItem(), Position = new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100)) }
				);

			this.Interactives = new List<WorldInteractive> { 
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100))),
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100))),
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100))),
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100))),
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100))),
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100))),
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100))),
				MetaManager.GetInteractve(new Vector2(GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100), GameRandom.Next(100, (this.CurrentMap.Width * Game1.TileSize) - 100)))
			};

			foreach (var i in this.Interactives)
				i.OnDestroyed += Interactive_OnDestroyed;

			// This will need to be redone if the map changes....
			_physics.CalculateParameters();
		}

		public void Update(GameTime gameTime, bool mouseInWorld)
		{
			_mouseInWorld = mouseInWorld;
			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			this.CurrentMap.Update(gameTime);
			this.Character.Update(gameTime, _mouseInWorld);
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
					if (this.Character.AddItem(item.Item, true))
						removedItems.Add(item);
				}
			}

			if (removedItems.Any())
			{
				this.Items.RemoveAll(x => removedItems.Contains(x));
				OnItemsChange?.Invoke(this, null);
			}
		}

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
