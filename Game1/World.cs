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
		public Player Player { get; set; }
		public Map CurrentMap { get; set; }

		// This should probably be a more generalized event for character events...
		public event EventHandler<ComponentEventArgs> OnCharacterDied;

		public World(string playerId)
		{
			_physics = new PhysicsManager(this);
			_playerId = playerId;
		}

		public void Initialize()
		{
			this.Player = IOManager.ObjectFromFile<PlayerSerializer>(Game1.PlayerFile).ToPlayer();
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
			foreach (var npc in this.MapObjects.Characters)
				npc.Update(gameTime);
			_physics.Update(gameTime);
			foreach (var character in this.MapObjects.Characters.Where(n => n.Moved))
				this.MapObjects.Move(character);
			foreach (var item in this.MapObjects.Items)
				item.Update(gameTime);
			foreach (var interactive in this.MapObjects.Interactives)
				interactive.Update(gameTime);
			foreach (var transition in this.MapObjects.Transitions)
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
						this.MapObjects.Remove(item);
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
			this.MapObjects.Add(new WorldItem(item, (Vector2)position, pickup));
		}

		private void Interactive_OnDestroyed(object sender, EventArgs e)
		{
			// Another thing that shouldn't be here...testing...
			var interactive = (WorldInteractive)sender;
			this.MapObjects.Remove(interactive);
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
			this.MapObjects = new WorldEntityList(this.CurrentMap, this.Player, Game1.TileSize);
			foreach (var interactive in this.MapObjects.Interactives)
				interactive.OnDestroyed += Interactive_OnDestroyed;

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
