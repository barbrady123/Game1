﻿using System;
using System.Collections.Generic;
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

namespace Game1
{
	// Either we need a new base class, or this should be the base and some of this crap needs to be moved into a child class....
	public class Character
	{
		private Vector2 _position;
		private ImageSpriteSheet _spriteSheet;
		private ItemContainer _hotbar;
		private ItemContainer _backpack;
		protected float _movementSpeed;
		protected float _toolSpeed;
		private int _currentHP;
		private int _currentMana;
		private InventoryItem _heldItem;
		private InventoryItem _activeItem;
		//private int _itemDefense;
		//private int _baseDefense;

		public string SpriteSheetName => this.Sex.ToString("g").ToLower();
		public Vector2 Motion { get; set; }
		public float MovementSpeed => _movementSpeed * MovementSpeedModifier();
		public float ToolSpeed => _toolSpeed * ToolSpeedModifier();
		public Cardinal Direction { get; set; }

		public Vector2 PreviousPosition { get; set; }
		public bool Moved => this.PreviousPosition != this.Position;

		public string Name { get; set; }
		public CharacterSex Sex { get; set; }
		public int Level { get; set; }
		public int Experience { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Intelligence { get; set; }
		public int Wisdom { get; set; }
		public int Charisma { get; set; }
		public int Constitution { get; set; }

		public List<CharacterStatus<BuffEffect>> Buffs { get; set; }
		public List<CharacterStatus<DebuffEffect>> Debuffs { get; set; }

		// Again...if these were indexed array slots, this would be way easier!
		public int Defense =>
			(((ItemArmor)this.EquippedArmorHead?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorChest?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorLegs?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorFeet?.Item)?.Defense ?? 0) +
			DefenseModifier();

		// Find a better place for these types of methods...
		private int DefenseModifier()
		{
			return this.Buffs.Where(b => b.Effect.AffectedAttribute == CharacterAttribute.Defense).Sum(b => b.Effect.EffectValue * b.Stacks);
		}

		private float MovementSpeedModifier()
		{
			return 1.0f + (float)this.Buffs.Where(b => b.Effect.AffectedAttribute == CharacterAttribute.MovementSpeed).Sum(b => b.Effect.EffectValue * b.Stacks) / 100.0f;
		}

		private float ToolSpeedModifier() => 1.0f;

		public int MaxHP { get; set; }
		public int CurrentHP 
		{ 
			get { return _currentHP; }
			set
			{ 
				_currentHP = Util.Clamp(value, 0, this.MaxHP);
				if (_currentHP == 0)
					OnDied?.Invoke(this, null);
			}
		}

		public int MaxMana { get; set; }
		public int CurrentMana
		{ 
			get { return _currentMana; }
			set { _currentMana = Util.Clamp(value, 0, this.MaxMana); }
		}

		public int Gold { get; set; }

		public ItemContainer HotBar => _hotbar;
		public ItemContainer Backpack => _backpack;

		// World entities probably shouldn't use ComponentEventArgs...
		public event EventHandler<ComponentEventArgs> OnHeldItemChanged;
		public event EventHandler<ComponentEventArgs> OnActiveItemChanged;
		public event EventHandler<ComponentEventArgs> OnDied;
		public event EventHandler<ComponentEventArgs> OnGotExternalItem;
		public event EventHandler<CharacterEventArgs> OnItemUse;
		
		public InventoryItem HeldItem
		{ 
			get { return _heldItem; }
			set
			{
				if (_heldItem != value)
				{
					_heldItem = value;
					OnHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = _heldItem });
				}
			}
		}

		public InventoryItem ActiveItem
		{ 
			get { return _activeItem; }
			set
			{
				if (_activeItem?.Id != value?.Id)
				{
					// We're making a copy here so effects can be applied to the in-game image without affecting the inventory image...
					_activeItem = ItemManager.CopyItem(value);					
					OnActiveItemChanged?.Invoke(this, new ComponentEventArgs { Meta = _activeItem });
				}
			}
		}

		public bool ActiveItemHoldable => this.ActiveItem?.Item is ItemHoldable;

		public InventoryItem EquippedArmorHead	{ get; set; }
		public InventoryItem EquippedArmorChest { get; set; }
		public InventoryItem EquippedArmorLegs	{ get; set; }
		public InventoryItem EquippedArmorFeet	{ get; set; }

		public Vector2 Position
		{
			get { return _position; }
			set {
				if (_position != value)
				{
					_position = value;
				}
			}
		}

		public Character()
		{
			this.Direction = Cardinal.South;
			_movementSpeed = 150.0f;
			_toolSpeed = 3.0f;
			_hotbar = new ItemContainer(10);
			_backpack = new ItemContainer(40);
			this.Buffs = new List<CharacterStatus<BuffEffect>>();
			this.Debuffs = new List<CharacterStatus<DebuffEffect>>();
			this.PreviousPosition = -Vector2.One;
		}

		public void LoadContent()
		{
			_spriteSheet = MetaManager.GetSpriteSheet(this.SpriteSheetName);
			_spriteSheet.LoadContent();
		}

		public virtual Vector2 UpdateMotion()
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

		public void Update(GameTime gameTime, bool mouseInWorld = false)
		{
			this.PreviousPosition = _position;
			Vector2 motion = UpdateMotion();

			if (motion != Vector2.Zero)
			{
				motion.Normalize();
				motion *= (this.MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
				_spriteSheet.AddEffect<SpriteSheetEffect>(true);
			}
			else
			{
				_spriteSheet.StopEffect(typeof(SpriteSheetEffect));
			}

			this.Motion = motion;
			this.Direction = Util.DirectionFromVector(motion, this.Direction);
			_spriteSheet.UpdateDirection(this.Direction);
			_spriteSheet.Update(gameTime);

			if (mouseInWorld)
				UpdateMouse();

			this.Backpack.Update(gameTime);
			this.HotBar.Update(gameTime);
			this.EquippedArmorHead?.Update(gameTime);
			this.EquippedArmorChest?.Update(gameTime);
			this.EquippedArmorLegs?.Update(gameTime);
			this.EquippedArmorFeet?.Update(gameTime);
			this.HeldItem?.Update(gameTime);
			this.ActiveItem?.Update(gameTime);
			for (int i = this.Buffs.Count - 1; i >= 0; i--)
				this.Buffs[i].Update(gameTime);

			for (int i = this.Debuffs.Count - 1; i >= 0; i--)
				this.Debuffs[i].Update(gameTime);
		}

		private void UpdateMouse()
		{
			if (!InputManager.LeftMouseClick())
				return;

			if (!(this.ActiveItem?.Item is ItemHoldable held))
				return;

			if ((this.Direction == Cardinal.North) || (this.Direction == Cardinal.West))
			{
				var effect = this.ActiveItem.Icon.AddEffect<UseItemWestEffect>(true);
				effect.OnFullyExtended -= Effect_OnFullyExtended;
				effect.OnFullyExtended += Effect_OnFullyExtended;
			}
			else
			{
				var effect = this.ActiveItem.Icon.AddEffect<UseItemEastEffect>(true);
				effect.OnFullyExtended -= Effect_OnFullyExtended;
				effect.OnFullyExtended += Effect_OnFullyExtended;
			}
		}

		private void Effect_OnFullyExtended(object sender, EventArgs e)
		{
			// I'm sure we'll need more values than just the held item...
			OnItemUse?.Invoke(this, new CharacterEventArgs { Item = this.ActiveItem, Direction = this.Direction });	
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 offset)
		{
			DrawBehind(spriteBatch, offset);
			_spriteSheet.Draw(spriteBatch, position: this.Position + offset);
			DrawInfront(spriteBatch, offset);
		}

		private void DrawBehind(SpriteBatch spriteBatch, Vector2 offset)
		{
			if (!this.ActiveItemHoldable)
				return;

			if ((this.Direction == Cardinal.North) || (this.Direction == Cardinal.West))
			{
				this.ActiveItem.Icon.OriginOffset = GamePlayCamera.ActiveItemOriginOffsets[this.Direction];
				this.ActiveItem.Icon.Draw(spriteBatch, null, this.Position + offset + GamePlayCamera.ActiveItemOffsets[this.Direction], GamePlayCamera.ActiveItemScale);
			}
		}

		private void DrawInfront(SpriteBatch spriteBatch, Vector2 offset)
		{
			if (!this.ActiveItemHoldable)
				return;

			if ((this.Direction == Cardinal.South) || (this.Direction == Cardinal.East))
			{
				this.ActiveItem.Icon.OriginOffset = GamePlayCamera.ActiveItemOriginOffsets[this.Direction];
				this.ActiveItem.Icon.Draw(spriteBatch, null, this.Position + offset + GamePlayCamera.ActiveItemOffsets[this.Direction], GamePlayCamera.ActiveItemScale, SpriteEffects.FlipHorizontally);
			}
		}

		public bool IsItemHeld => this.HeldItem != null;

		public bool AddItem(InventoryItem item, bool fromExternal = false)
		{
			if (item == null)
				return true;

			// In case the stack is split and the original item quantity
			// has changed, we want to notify the world of the original
			// quantity that was receieved...
			int originalQuantity = item.Quantity;

			int? index = _hotbar.AddItem(item);
			if (index == null)
				index = _backpack.AddItem(item);

			if ((index != null) && fromExternal)
				OnGotExternalItem?.Invoke(this, new ComponentEventArgs { Meta = ItemManager.CopyItem(item, originalQuantity) });

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
				OnHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = this.HeldItem });
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
					OnHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = this.HeldItem });
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
				OnHeldItemChanged?.Invoke(this, new ComponentEventArgs { Meta = this.HeldItem });
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
					case ArmorSlot.Legs:	this.EquippedArmorLegs= item;	break;
					case ArmorSlot.Feet:	this.EquippedArmorFeet= item;	break;
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

		public void AddBuff(CharacterStatus<BuffEffect> buff)
		{
			buff.OnExpired += Buff_OnExpired;
			buff.OnPeriodicTick += Status_OnPeriodicTick;
			this.Buffs.Add(buff);
		}

		public void AddDebuff(CharacterStatus<DebuffEffect> debuff)
		{
			debuff.OnExpired += Debuff_OnExpired;
			debuff.OnPeriodicTick += Status_OnPeriodicTick;
			this.Debuffs.Add(debuff);
		}

		private void Status_OnPeriodicTick(object sender, CharacterStatusEventArgs e)
		{
			switch (e.AffectedAttribute)
			{
				case CharacterAttribute.CurrentHP : this.CurrentHP += e.EffectValue;	break;
			}
		}

		private void Buff_OnExpired(object sender, CharacterStatusEventArgs e)
		{
			this.Buffs.Remove((CharacterStatus<BuffEffect>)sender);
		}

		private void Debuff_OnExpired(object sender, CharacterStatusEventArgs e)
		{
			this.Debuffs.Remove((CharacterStatus<DebuffEffect>)sender);
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
	}
}
