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
		private int _itemDefense;
		private int _baseDefense;

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

		// Or we store the effective def (and other stats) to reduce realtime computations during combat...
		// Also will have other modifiers (buffs, debuffs, etc)
		public int Defense => _baseDefense + _itemDefense;

		public ItemContainer HotBar => _hotbar;
		public ItemContainer Backpack => _backpack;
		public InventoryItem HeldItem { get; set; }

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

		public void PutItem(ItemContainer container, int? index = null)
		{
			if (index == null)
			{
				index = container.AddItem(this.HeldItem);
				if (index == null)
				{
					// Here we need to do something with the held item that we don't have room for...
					// Minecraft just "throws" (drops) it...that might when when we are able to have 
					// items in the environment??
					return;
				}
			}

			this.HeldItem = container.AddItem(this.HeldItem, (int)index);
		}
	}
}
