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
using Game1.Effect;
using Game1.Enum;
using Game1.Items;
using Game1.Maps;

namespace Game1
{
	public class World : Component
	{
		private readonly PhysicsManager _physics;
		private readonly string _playerId;

		public WorldEntityList MapObjects { get; set; }
		public List<WorldCharacter> NPCs { get; set; }
		public Player Player { get; set; }
		public Map CurrentMap { get; set; }

		// TODO: Move these to WorldEntityList (I think).....
		// Testing this out as objects that sit directly on the map....
		public List<WorldItem> Items { get; set; }
		// Testing this out as interactives that sit directly on the map...
		public List<WorldInteractive> Interactives { get; set; }
		// Testing this out as transitions that sit directly on the map...
		public List<WorldTransition> Transitions { get; set; }

		public List<WorldCharacter> AllCharacters => new List<WorldCharacter>((this.NPCs?.Count ?? 0) + 1) { this.Player }.Concat(this.NPCs).ToList();

		// This should probably be a more generalized event for character events...
		public event EventHandler<ComponentEventArgs> OnCharacterDied;

		public World(string playerId)
		{
			_physics = new PhysicsManager(this);
			_playerId = playerId;
		}

		public void Initialize()
		{
			var serializer = IOManager.ObjectFromFile<PlayerSerializer>(Game1.PlayerFile);
			this.Player = serializer.ToPlayer();
			AssetManager.LoadPlayerAssets(this.Player);
			this.Player.OnDied += Character_OnDied;
			ChangeMap(this.Player.Location, this.Player.Position.ToPoint());
		}

		public void ChangeMap(string mapName, Point playerPosition)
		{
			if (this.CurrentMap != null)
			{
				// TODO: Save map with current data, timestamp it, etc...
			}

			this.CurrentMap = IOManager.ObjectFromFile<Map>(Path.Combine(Game1.MapRoot, mapName));
			this.CurrentMap.Initialize();

			this.Player.Location = mapName;
			this.Player.PreviousPosition = Vector2.Zero;
			this.Player.Position = playerPosition.ToVector2();

			LoadDataFromCurrentMap();
			_onMapChanged?.Invoke(this, null);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			this.Player.Update(gameTime);
			foreach (var npc in this.NPCs)
				npc.Update(gameTime);
			_physics.Update(gameTime);

			if (this.Player.Moved)
				this.MapObjects.Move(this.Player);
			foreach (var npc in this.NPCs.Where(n => n.Moved))
				this.MapObjects.Move(npc);

			// TODO: these collections could/should be mainined in the Entity list object (which should be moved inside the Map class)....
			foreach (var item in this.Items)
				item.Update(gameTime);
			foreach (var interactive in this.Interactives)
				interactive.Update(gameTime);
			foreach (var transition in this.Transitions)
				transition.Update(gameTime);
		}

		public IWorldEntity ProcessInteractivity(Point mousePosition, bool mouseLeftClick, bool interactiveKeyPressed)
		{
			// Transition check...
			if (interactiveKeyPressed)
			{
				foreach (var transition in this.MapObjects.GetEntities<WorldTransition>(this.Player.Bounds))
					if (Vector2.Distance(transition.Position, this.Player.Position) < 30.0f)	// Make this distance configurable...
					{
						ChangeMap(transition.DestinationMap, transition.DestinationPosition);
						break;
					}
			}

			// Item proximity pickup...
			foreach (var item in this.MapObjects.GetEntities<WorldItem>(this.Player.Bounds))
			{
				item.InRange = Vector2.Distance(item.Position, this.Player.Position) <= Game1.DefaultPickupRadius;

				if (item.Pickup && item.InRange)
				{
					if (this.Player.AddItem(item.Item, true))
						this.MapObjects.Remove(this.Items.RemoveItem(item));
				}
			}

			if (mousePosition == Point.Zero)
				return null;

			IWorldEntity targetEntity = null;

			// Targetting (mouseover) entity...
			foreach (var entity in this.MapObjects.GetEntities(mousePosition).Where(e => e != this.Player))
			{
				if (!entity.Bounds.Contains(mousePosition))
					continue;

				// How can we use a key to cycle through these??
				entity.IsHighlighted = true;
				targetEntity = entity;
				if (entity is WorldEntity e)
					e.MouseOver();

				// Interactive/Tool check...
				if (this.Player.ActiveItemSolid && (this.Player.ActiveItem?.Item is ItemTool tool))
				{
					if ((entity is WorldInteractive interactive) && (Vector2.Distance(interactive.Position, this.Player.Position) <= tool.Range))
					{
						interactive.Icon.AddEffect<ShakeEffect>(true);
						if (interactive.Health != null)
							interactive.Health -= (int)(tool.Damage * interactive.Interactive.Effectiveness[tool.Type]);
					}
				}

				break;
			}
			
			if (mouseLeftClick)
			{
				// Clicking on an empty space will drop item if held...
				if (this.Player.IsItemHeld && (targetEntity == null))
					this.AddItem(this.Player.DropHeld(), pickup: false);
				else 
					this.Player.UseActiveItem();
			}

			return targetEntity;
		}

		public void AddItem(InventoryItem item, Vector2? position = null, bool pickup = true)
		{
			if (item == null)
				return;

			position = position ?? this.Player.Position;
			this.MapObjects.Add(this.Items.AddItem(new WorldItem(item, (Vector2)position, pickup)));
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

		private void LoadDataFromCurrentMap()
		{
			AssetManager.LoadMapAssets(this.CurrentMap);
			this.MapObjects = new WorldEntityList(this.CurrentMap.Width, this.CurrentMap.Height, Game1.TileSize);
			this.MapObjects.Add(this.Player);

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

			this.NPCs = new List<WorldCharacter>();

			foreach (var npc in this.CurrentMap.NPCs)
			{
				//var worldNPC = MetaManager.GetCharacter(npc.Id, npc.Position.ToVector2());
				//var worldNPC = new NPC(npc.Id, CharacterSex.Male, npc.Position.ToVector2(), 10, 10);
				//worldNPC.SpriteSheet = AssetManager.GetSpriteSheet(worldNPC.SpriteSheetName);
				//worldNPC.OnDied += Npc_OnDied;
				this.MapObjects.Add(this.NPCs.AddItem(MetaManager.GetCharacter(npc.Id, npc.Position.ToVector2())));
			}

			_physics.CalculateParameters();
		}

		#region Events
		private event EventHandler<ComponentEventArgs> _onMapChanged;
		public event EventHandler<ComponentEventArgs> OnMapChanged
		{
			add		{ lock(_lock) { _onMapChanged -= value; _onMapChanged += value; } }
			remove	{ lock(_lock) { _onMapChanged -= value; } }
		}
		#endregion
	}
}
