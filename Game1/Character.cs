using System;
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
	public class Character
	{
		private Vector2 _position;
		private Vector2 _previousPosition;
		private ItemContainer _hotbar;
		private ItemContainer _backpack;
		private int _currentHP;
		private int _currentMana;
		private InventoryItem _heldItem;
		//private int _itemDefense;
		//private int _baseDefense;

		public string SpriteSheetName => this.Sex.ToString("g");
		public Vector2 Motion { get; set; }
		public float Speed { get; set; }
		public Cardinal Direction { get; set; }

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

		// Again...if these were indexed array slots, this would be way easier!
		public int Defense =>
			(((ItemArmor)this.EquippedArmorHead?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorChest?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorLegs?.Item)?.Defense ?? 0) +
			(((ItemArmor)this.EquippedArmorFeet?.Item)?.Defense ?? 0);
			// TODO: Need other modifiers here eventually...(buffs/debuffs/etc)

		public int MaxHP { get; set; }
		public int CurrentHP 
		{ 
			get { return _currentHP; }
			set { _currentHP = Util.Clamp(value, 0, this.MaxHP); }
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
		
		public InventoryItem HeldItem
		{ 
			get { return _heldItem; }
			set
			{
				if (_heldItem != value)
				{
					_heldItem = value;
					UpdateMouseCursor();
				}
			}
		}

		// Ok this makes a weird dependency in the Character class to the InputManager...needs to be fixed...
		public void UpdateMouseCursor()
		{
			if (_heldItem == null)
			{
				InputManager.SetMouseCursor(null);
				return;
			}

			int? quantity = _heldItem.Quantity > 1 ? _heldItem.Quantity : (int?)null;
			InputManager.SetMouseCursor(_heldItem?.Item.Icon.Texture, quantity);
		}

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
					_previousPosition = _position;
					_position = value;
				}
			}
		}

		public Character()
		{
			this.Direction = Cardinal.South;
			this.Speed = 150.0f;
			_hotbar = new ItemContainer(10);
			_backpack = new ItemContainer(40);
		}

		public void RevertPosition()
		{
			if (_previousPosition != null)
				this.Position = _previousPosition;
		}

		public virtual Vector2 UpdateMotion()
		{
			Vector2 motion = Vector2.Zero;

			if (InputManager.KeyDown(Keys.W))
			{
				motion.Y = -1;
				this.Direction = Cardinal.North;
			}
			if (InputManager.KeyDown(Keys.S))
			{	
				motion.Y = 1;
				this.Direction = Cardinal.South;
			}
			if (InputManager.KeyDown(Keys.A))
			{
				motion.X = -1;
				this.Direction = Cardinal.West;
			}
			if (InputManager.KeyDown(Keys.D))
			{
				motion.X = 1;
				this.Direction = Cardinal.East;
			}

			return motion;
		}

		public void Update(GameTime gameTime)
		{
			Vector2 motion = UpdateMotion();

			if (motion != Vector2.Zero)
			{
				motion.Normalize();
				motion *= (this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
				this.Position += motion;
			}

			this.Motion = motion;
		}

		public void GetItem(ItemContainer container, int index, int? quantity = null)
		{
			var item = container[index];
			if (item == null)
				return;

			quantity = Math.Min(quantity ?? Int32.MaxValue, item.Quantity);
			
			if (quantity == item.Quantity) 
			{
				PutItem(container, index);
			}
			else
			{
				PutItem(container);
				// This means there's no room in the backpack to drop the held item, exit for now...
				if (this.HeldItem != null)
					return;

				this.HeldItem = ItemManager.FromItem(item, quantity);
				item.Quantity -= (int)quantity;
			}
		}

		public void PutItem(ItemContainer container, int? index = null)
		{			
			if (index == null)
			{
				index = container.AddItem(this.HeldItem);
				if (index == null)
				{
					// Here we need to do something with the held item that we don't have room for...
					// Minecraft just "throws" (drops) it...that might work when we are able to have 
					// items in the environment??
				}
				// This should be safe since the above version of AddItem won't swap with an existing item...
				this.HeldItem = null;
				return;
			}

			var prevHeldItem = this.HeldItem;
			int prevQuantity = prevHeldItem?.Quantity ?? 0;
			this.HeldItem = container.AddItem(this.HeldItem, (int)index);

			if ((this.HeldItem == prevHeldItem) && (prevQuantity != (this.HeldItem?.Quantity ?? 0)))
				// Weird scenario we're covering here, probably should be refactored:
				// If, due to AddItem(), only the non-null HeldItem quantity changes, the mouse cursor update won't trigger
				// automatically (because technically this.HeldItem didn't "change")...so we call it manually
				// here if it's the same item as before but the quantity has changed (due to an item merge with leftover)...
				UpdateMouseCursor();
		}

		// Maybe we should move armor slots to indexed (by enum value) array, to make these types of methods easier 
		public void EquipArmor(ItemContainer container, int index)
		{
			var item = container[index];
			if (item?.Item is ItemArmor armor)
			{
				UnequipArmor(armor.Slot);
				switch (armor.Slot)
				{
					case ArmorSlot.Head:	this.EquippedArmorHead = item;	break;
					case ArmorSlot.Chest:	this.EquippedArmorChest = item;	break;
					case ArmorSlot.Legs:	this.EquippedArmorLegs= item;	break;
					case ArmorSlot.Feet:	this.EquippedArmorFeet= item;	break;
				}
				container.Items[index] = null;
				PutItem(container, index);
			}
		}

		public void UnequipArmor(ArmorSlot slot, bool holdItem = true)
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

			// Do we need to check if there's something there arleady?   
			// We should probably have this property auto-store anything in there in a container if avialable
			if (unequipped != null)
				this.HeldItem = unequipped;
		}
	}
}
