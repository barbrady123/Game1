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

		public event EventHandler<ComponentEventArgs> OnItemsChange;
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
			LoadDataFromCurrentMap();

			this.Character.Location = mapName;
			this.Character.PreviousPosition = Vector2.Zero;
			this.Character.Position = playerPosition.ToVector2();
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
			foreach (var item in this.Items)
				item.Update(gameTime);
			foreach (var interactive in this.Interactives)
				interactive.Update(gameTime);
			foreach (var transition in this.Transitions)
				transition.Update(gameTime);

			// I think it makes sense to put things like "Item pickup" from proximity after the physics update?
			// I'm not 100% sure where i even want this to live yet or what entity's responsibility this should be
			List<WorldItem> removedItems = new List<WorldItem>();

			// TODO: Need to optimize this to use the new mapObjects....

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
				removedItems.ForEach(x => this.MapObjects.Remove(x));
				OnItemsChange?.Invoke(this, null);
			}
		}

		public void AddItem(InventoryItem item, Vector2? position = null, bool pickup = true)
		{
			if (item == null)
				return;

			position = position ?? this.Character.Position;
			this.MapObjects.Add(this.Items.AddItem(new WorldItem(item, (Vector2)position, pickup)));
			OnItemsChange?.Invoke(this, null);
		}

		private void Npc_OnDied(object sender, ComponentEventArgs e)
		{
			// Just testing...should be a loot check
			this.NPCs.Remove((NPC)sender);
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

		public WorldEntityList MapObjects { get; set; }

		private void LoadDataFromCurrentMap()
		{			
			this.MapObjects = new WorldEntityList(this.CurrentMap.Width, this.CurrentMap.Height);

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

			this.NPCs = new List<NPC>();
			// TODO: Need Metadata for NPCs...
			foreach (var npc in this.CurrentMap.NPCs)
			{
				var worldNPC = new NPC(npc.Id, CharacterSex.Male, npc.Position.ToVector2(), 10, 10);
				worldNPC.OnDied += Npc_OnDied;
				this.NPCs.Add(worldNPC);
			}

			_physics.CalculateParameters();
		}
	}
}
