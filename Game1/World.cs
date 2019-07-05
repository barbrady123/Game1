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
using Game1.Items;
using Game1.Maps;

namespace Game1
{
	public class World : Component
	{
		private readonly PhysicsManager _physics;
		private readonly string _playerId;
		private bool _mouseInWorld;

		public List<NPC> NPCs { get; set; }
		public Character Character { get; set; }
		public Map CurrentMap { get; set; }
		// Testing this out as objects that sit directly on the map....
		public List<WorldItem> Items { get; set; }

		// Testing this out as interactives that sit directly on the map...
		public List<WorldInteractive> Interactives { get; set; }

		// Testing this out as transitions that sit directly on the map...
		public List<WorldTransition> Transitions { get; set; }

		public List<Character> AllCharacters => new List<Character>((this.NPCs?.Count ?? 0) + 1) { this.Character }.Concat(this.NPCs).ToList();

		// This should probably be a more generalized event for character events...
		public event EventHandler<ComponentEventArgs> OnCharacterDied;

		public World(string playerId)
		{
			_physics = new PhysicsManager(this);
			_mouseInWorld = false;
			_playerId = playerId;
		}

		public void Initialize()
		{
			this.Character = IOManager.ObjectFromFile<Character>(Game1.PlayerFile);
			AssetManager.LoadPlayerAssets(this.Character);
			this.Character.OnDied += Character_OnDied;
			ChangeMap(this.Character.Location, this.Character.Position.ToPoint());
		}

		public void ChangeMap(string mapName, Point playerPosition)
		{
			if (this.CurrentMap != null)
			{
				// TODO: Save map with current data, timestamp it, etc...
			}

			this.CurrentMap = IOManager.ObjectFromFile<Map>(Path.Combine(Game1.MapRoot, mapName));
			this.CurrentMap.Initialize();

			this.Character.Location = mapName;
			this.Character.PreviousPosition = Vector2.Zero;
			this.Character.Position = playerPosition.ToVector2();

			LoadDataFromCurrentMap();
		}

		public void Update(GameTime gameTime, bool mouseInWorld)
		{
			_mouseInWorld = mouseInWorld;
			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			this.Character.Update(gameTime, _mouseInWorld);
			foreach (var npc in this.NPCs)
				npc.Update(gameTime);
			_physics.Update(gameTime);

			// TODO: Here is a good spot for this.CurrentMap.Update() ... which would update the moving entities in the entity list, and perform
			// various other operations for character/npc/item/etc updates (all entity updates)...
			// TODO: But, until then, TEMP: we're going to update entities that move in the entity list...
			if (this.Character.Moved)
				this.MapObjects.Move(this.Character);
			foreach (var npc in this.NPCs.Where(n => n.Moved))
				this.MapObjects.Move(npc);

			// TODO: these collections could/should be mainined in the Entity list object (which should be moved inside the Map class)....
			foreach (var item in this.Items)
				item.Update(gameTime);
			foreach (var interactive in this.Interactives)
				interactive.Update(gameTime);
			foreach (var transition in this.Transitions)
				transition.Update(gameTime);

			// I think it makes sense to put things like "Item pickup" from proximity after the physics update?
			// I'm not 100% sure where i even want this to live yet or what entity's responsibility this should be
			foreach (var item in this.MapObjects.GetEntities<WorldItem>(this.Character.Bounds))
			{
				item.InRange = Vector2.Distance(item.Position, this.Character.Position) <= Game1.DefaultPickupRadius;

				if (item.Pickup && item.InRange)
				{
					if (this.Character.AddItem(item.Item, true))
						this.MapObjects.Remove(this.Items.RemoveItem(item));
				}
			}
		}

		public void AddItem(InventoryItem item, Vector2? position = null, bool pickup = true)
		{
			if (item == null)
				return;

			position = position ?? this.Character.Position;
			this.MapObjects.Add(this.Items.AddItem(new WorldItem(item, (Vector2)position, pickup)));
		}

		private void Npc_OnDied(object sender, ComponentEventArgs e)
		{
			// Just testing...should be a loot check
			this.NPCs.Remove((NPC)sender);
			this.MapObjects.Remove((NPC)sender);
		}

		private void Interactive_OnDestroyed(object sender, EventArgs e)
		{
			// Another thing that shouldn't be here...testing...
			var interactive = (WorldInteractive)sender;
			this.MapObjects.Remove(this.Interactives.RemoveItem(interactive));
			// Need something to generate loot from loot table...just doing this temp...
			foreach (var l in interactive.Interactive.LootTable)
			{
				if (GameRandom.Percent(l.Odds))
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

		public WorldEntityList MapObjects { get; set; }

		private void LoadDataFromCurrentMap()
		{
			AssetManager.LoadMapAssets(this.CurrentMap);
			this.MapObjects = new WorldEntityList(this.CurrentMap.Width, this.CurrentMap.Height, Game1.TileSize);
			this.MapObjects.Add(this.Character);

			this.Items = new List<WorldItem>();
			foreach (var item in this.CurrentMap.Items)
				this.MapObjects.Add(this.Items.AddItem(new WorldItem(ItemManager.GetItem(item.Id, item.Quantity), item.Position.ToVector2(), true)));

			this.Interactives = new List<WorldInteractive>();
			foreach (var interactive in this.CurrentMap.Interactives)
			{
				var worldInteractive = this.MapObjects.Add(this.Interactives.AddItem(MetaManager.GetInteractve(interactive.Id, interactive.Position.ToVector2())));
				worldInteractive.OnDestroyed += Interactive_OnDestroyed;
			}

			this.Transitions = new List<WorldTransition>();
			foreach (var transition in this.CurrentMap.Transitions)
				this.MapObjects.Add(this.Transitions.AddItem(MetaManager.GetTransition(transition)));

			// Solid layer blocks
			foreach (var layer in this.CurrentMap.Layers.Where(l => l.Type == LayerType.Solid))
			{
				for (int y = 0; y < layer.TileData.GetLength(1); y++)
				for (int x = 0; x < layer.TileData.GetLength(0); x++)
				{
					// Again, coords here are reversed so file data can "visually" match the screen...
					if (layer.TileData[y,x] < 0)
						continue;

					this.MapObjects.Add(new WorldSolid(new Vector2(x * Game1.TileSize, y * Game1.TileSize)));
				}
			}

			this.NPCs = new List<NPC>();

			// TODO: Need Metadata for NPCs...
			foreach (var npc in this.CurrentMap.NPCs)
			{
				var worldNPC = new NPC(npc.Id, CharacterSex.Male, npc.Position.ToVector2(), 10, 10);
				worldNPC.SpriteSheet = AssetManager.GetSpriteSheet(worldNPC.SpriteSheetName);
				worldNPC.OnDied += Npc_OnDied;
				this.MapObjects.Add(this.NPCs.AddItem(worldNPC));
			}

			_physics.CalculateParameters();
		}
	}
}
