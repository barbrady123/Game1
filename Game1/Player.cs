using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	public class Player : CombatCharacter
	{
		private (bool, InventoryItem) _queuedActiveItem;
		private InventoryItem _heldItem;

		public ItemContainer HotBar { get; private set; }
		public ItemContainer Backpack { get; private set; }
		public bool IsItemHeld => this.HeldItem != null;

		public string Location { get; set; }
		public InventoryItem EquippedArmorHead	{ get; set; }
		public InventoryItem EquippedArmorChest { get; set; }
		public InventoryItem EquippedArmorLegs	{ get; set; }
		public InventoryItem EquippedArmorFeet	{ get; set; }

		public InventoryItem HeldItem
		{ 
			get { return _heldItem; }
			set
			{
				if (_heldItem != value)
				{
					_heldItem = value;
					_onHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = _heldItem });
				}
			}
		}

		// Again...if these were indexed array slots, this would be way easier!
		public override int Defense =>
			(((ItemArmor)this.EquippedArmorHead?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorChest?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorLegs?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorFeet?.Item)?.Defense ?? 0) +
			DefenseModifier();

		public override bool InAction
		{ 
			get { return _inAction; }
			protected set
			{
				_inAction = value;
				if ((!_inAction) && (_queuedActiveItem.Item1))
				{
					SetActiveItem(_queuedActiveItem.Item2);
					_queuedActiveItem = (false, null);
				}
			}
		}

		public Player(Character character, ImageSpriteSheet spriteSheet, Vector2 position) : base(character, spriteSheet, position)
		{
			this.HotBar = new ItemContainer(10);
			this.Backpack = new ItemContainer(40);
			_queuedActiveItem = (false, null);
			this.UpdateMotion = PlayerUpdateMotion;
		}

		public virtual Vector2 PlayerUpdateMotion(Vector2 previousMotion)
		{
			Vector2 motion = Vector2.Zero;

			if (InputManager.KeyDown(Keys.W))
				motion.Y = -1;
			if (InputManager.KeyDown(Keys.S))
				motion.Y = 1;
			if (InputManager.KeyDown(Keys.A))
				motion.X = -1;
			if (InputManager.KeyDown(Keys.D))
				motion.X = 1;

			return motion;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			this.Backpack.Update(gameTime);
			this.HotBar.Update(gameTime);
			this.EquippedArmorHead?.Update(gameTime);
			this.EquippedArmorChest?.Update(gameTime);
			this.EquippedArmorLegs?.Update(gameTime);
			this.EquippedArmorFeet?.Update(gameTime);
			this.HeldItem?.Update(gameTime);
		}

		public override void SetActiveItem(InventoryItem item)
		{ 
			if (this.ActiveItem?.Id != item?.Id)
			{
				if (this.InAction)
				{
					_queuedActiveItem = (true, item);
				}
				else
				{
					// We're making a copy here so effects can be applied to the in-game image without affecting the inventory image...
					this.ActiveItem = ItemManager.CopyItem(item);
					ActiveItemChanged(new ComponentEventArgs { Meta = this.ActiveItem });
				}
			}
		}

		public static Player New(string name, CharacterSex sex)
		{
			return new Player(
				new Character {
					Id = Guid.NewGuid().ToString(),
					CreatureType = CreatureType.Humanoid,
					Name = name,
					Sex = sex,					
					SpriteSheetName = sex.ToString("g").ToLower(),	// TODO: This should be passed in from a larger selection...
					MovementSpeed = Game1.DefaultPlayerMovementSpeed
				},
				null,	// No reason to generate this now...
				new Vector2(Game1.PlayerStartLocation.X, Game1.PlayerStartLocation.Y)
			)
			{
				Location = "map",
				Position = new Vector2(Game1.PlayerStartLocation.X, Game1.PlayerStartLocation.Y),
				Strength = GameRandom.Next(10, 20),
				Dexterity = GameRandom.Next(10, 20),
				Intelligence = GameRandom.Next(10, 20),
				Wisdom = GameRandom.Next(10, 20),
				Charisma = GameRandom.Next(10, 20),
				Constitution = GameRandom.Next(10, 20),
				MaxHP = GameRandom.Next(30, 50),
				CurrentHP = GameRandom.Next(20, 30),
				MaxMana = GameRandom.Next(30, 50),
				CurrentMana = GameRandom.Next(20, 30)
			};
		}

		#region Equipment
		public bool AddItem(InventoryItem item, bool fromExternal = false)
		{
			if (item == null)
				return true;

			// In case the stack is split and the original item quantity
			// has changed, we want to notify the world of the original
			// quantity that was receieved...
			int originalQuantity = item.Quantity;

			int? index = this.HotBar.AddItem(item);
			if (index == null)
				index = this.Backpack.AddItem(item);

			if ((index != null) && fromExternal)
				_onGotExternalItem?.Invoke(this, new ComponentEventArgs { Meta = ItemManager.CopyItem(item, originalQuantity) });

			return index != null;
		}

		public void SwapHeld(ItemContainer container, int index)
		{
			var previousHeld = this.HeldItem;
			int previousQuantity = this.HeldItem?.Quantity ?? 0;
			this.HeldItem = container.SwapItem(index, this.HeldItem);
			if ((this.HeldItem == previousHeld) && (previousQuantity != (this.HeldItem?.Quantity ?? 0)))
				// Weird scenario we're covering here, probably should be refactored:
				// If, due to SwapItem(), only the non-null HeldItem quantity changes, the mouse cursor update won't trigger
				// automatically (because technically this.HeldItem didn't "change")...so we call it manually
				// here if it's the same item as before but the quantity has changed (due to an item merge with leftover)...
				_onHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = this.HeldItem });
		}

		public bool StoreHeld()
		{
			if (!AddItem(this.HeldItem))
				return false;

			this.HeldItem = null;
			return true;
		}

		public InventoryItem DropHeld()
		{
			var heldItem = this.HeldItem;
			this.HeldItem = null;
			return heldItem;
		}

		public bool HoldItemQuantity(ItemContainer container, int index, int? quantity = null)
		{			
			var item = container[index];
			if (item == null)
				return true;

			int trueQuantity = Math.Min(quantity ?? Int32.MaxValue, item.Quantity);
			
			// If the quantity is the entire stack, just pick it up...
			if (trueQuantity == item.Quantity) 
			{
				if (this.IsItemHeld && (this.HeldItem.Item == item.Item) && (this.HeldItem.Quantity + trueQuantity <= this.HeldItem.Item.MaxStackSize))
				{
					// If there's a held item that is the same as what you're picing up, and there's enough room to hold the requested quantity, combine them...
					this.HeldItem.Quantity += trueQuantity;
					// See comment above regarding this event firing...
					_onHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = this.HeldItem });
					container.RemoveItem(index);
					return true;
				}

				SwapHeld(container, index);
				return true;
			}
			else if (trueQuantity == 0)
			{
				// If the quantity was 0, don't do anything...
				return true;
			}
			else if (this.IsItemHeld && (this.HeldItem.Item == item.Item) && (this.HeldItem.Quantity + trueQuantity <= this.HeldItem.Item.MaxStackSize))
			{
				// If there's a held item that is the same as what you're picing up, and there's enough room to hold the requested quantity, combine them...
				this.HeldItem.Quantity += trueQuantity;
				// See comment above regarding this event firing...
				_onHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = this.HeldItem });
				item.Quantity -= trueQuantity;
				return true;
			}

			// If nothing above was applicable, we have to store whatever we have held before we can pickup the new item/quantity...
			if (!StoreHeld())
				return false;

			this.HeldItem = ItemManager.FromItem(item, trueQuantity);
			if (this.HeldItem.Quantity == 0)
				this.HeldItem = null;

			return true;
		}

		// Maybe we should move armor slots to indexed (by enum value) array, to make these types of methods easier 
		public void EquipArmor(ItemContainer container, int index)
		{
			var item = container[index];
			if (item?.Item is ItemArmor armor)
			{
				var previousArmor = UnequipArmor(armor.Slot);

				switch (armor.Slot)
				{
					case ArmorSlot.Head:	this.EquippedArmorHead = item;	break;
					case ArmorSlot.Chest:	this.EquippedArmorChest = item;	break;
					case ArmorSlot.Legs:	this.EquippedArmorLegs = item;	break;
					case ArmorSlot.Feet:	this.EquippedArmorFeet = item;	break;
				}

				container.Items[index] = previousArmor;
			}
		}

		public InventoryItem UnequipArmor(ArmorSlot slot)
		{
			InventoryItem unequipped = null;

			switch (slot)
			{
				case ArmorSlot.Head:
					unequipped = this.EquippedArmorHead;	
					this.EquippedArmorHead = null;
					break;
				case ArmorSlot.Chest:
					unequipped = this.EquippedArmorChest;
					this.EquippedArmorChest = null;
					break;
				case ArmorSlot.Legs:
					unequipped = this.EquippedArmorLegs;
					this.EquippedArmorLegs = null;
					break;
				case ArmorSlot.Feet:
					unequipped = this.EquippedArmorFeet;
					this.EquippedArmorFeet = null;
					break;
			}

			return unequipped;
		}

		public void Consume(ItemContainer container, int index)
		{
			if (!(container[index]?.Item is ItemConsumable item))
				return;

			if (item.InstantEffect != null)
				MetaManager.ApplyCharacterInstantEffect((CharacterInstantEffect)item.InstantEffect, this);

			if (item.BuffEffect != null)
				MetaManager.ApplyCharacterBuffEffect((CharacterBuffEffect)item.BuffEffect, this);

			if (item.DebuffEffect != null)
				MetaManager.ApplyCharacterDebuffEffect((CharacterDebuffEffect)item.DebuffEffect, this);

			container[index].Quantity--;
			if (container[index].Quantity <= 0)
				container.Items[index] = null;
		}
		#endregion

		#region Events
		private event EventHandler<ComponentEventArgs> _onHeldItemChanged;
		public event EventHandler<ComponentEventArgs> OnHeldItemChanged
		{
			add		{ lock(_lock) { _onHeldItemChanged -= value; _onHeldItemChanged += value; } }
			remove	{ lock(_lock) { _onHeldItemChanged -= value; } }
		}

		private event EventHandler<ComponentEventArgs> _onGotExternalItem;
		public event EventHandler<ComponentEventArgs> OnGotExternalItem
		{
			add		{ lock(_lock) { _onGotExternalItem -= value; _onGotExternalItem += value; } }
			remove	{ lock(_lock) { _onGotExternalItem -= value; } }
		}
		#endregion
	}
}
