using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Game1.Effect;
using Game1.Enum;

namespace Game1.Screens.Menu
{
	public class MenuScreen : Screen, IActivatable
	{
		private const int MENU_PADDING = 20;
		private const float ENABLED_MENU_ALPHA = 1.0f;
		private const float DISABLED_MENU_ALPHA = 0.4f;
		private const float DEFAULT_ITEM_ALPHA = 1.0f;

		private static readonly Color ActiveItemColor = Color.White;
		private static readonly Color SelectedItemColor = new Color(240, 240, 240);
		private static readonly Color UnselectedItemColor = new Color(100, 100, 100);

		protected int _currentIndex;
		protected List<MenuItem> _items;
		protected MenuLayout _layout; 

		private bool _isActive;
		private bool _escapeToDisable;
		private bool _beyondBoundaryDisable;
		private bool _fireAltAxisEvents;

		private Keys ForwardKey => (_layout == MenuLayout.Vertical) ? Keys.Down : Keys.Right;
		private Keys BackwardKey => (_layout == MenuLayout.Vertical) ? Keys.Up : Keys.Left;
		private Keys AltAxisFowardKey  => (_layout == MenuLayout.Vertical) ? Keys.Right : Keys.Down;
		private Keys AltAxisBackwardKey  => (_layout == MenuLayout.Vertical) ? Keys.Left : Keys.Up;

		public event EventHandler OnReadyDisable;
		public event EventHandler OnCurrentItemChange;
		public event EventHandler OnItemSelect;

		private int _delayInputCycles;

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		public int ItemCount => _items?.Count ?? 0;

		public bool IsActive
		{
			get { return _isActive; }
			set {
				if (value != _isActive)
				{
					_isActive = value;
					if (_isActive)
					{
						if (_currentIndex < 0)
							this.CurrentIndex = 0;

						SetCurrentItemEffects(true);
						DelayInput(1);
					}
					else
					{
						SetCurrentItemEffects(false);
					}
				}
			}
		}

		public int CurrentIndex
		{
			get { return _currentIndex; }
			set {
				int index = Util.Clamp(value, 0, _items.Count - 1);
				if (index == _currentIndex)
					return;
				
				SetCurrentItemEffects(false);
				_currentIndex = index;
				SetCurrentItemEffects(true);
			}
		}

		public void ClearSelection()
		{
			SetCurrentItemEffects(false);
			_currentIndex = -1;
		}

		public MenuScreen(Rectangle bounds,
						  MenuLayout layout = MenuLayout.Vertical,
						  bool hasBackground = true,
						  bool escapeToDisable = false,
						  bool beyondBoundaryDisable = false,
						  bool fireAltAxisEvents = false): base(bounds, hasBackground ? "brick" : null)
		{
			_currentIndex = -1;
			_items = new List<MenuItem>();
			_layout = layout;
			_escapeToDisable = escapeToDisable;
			_beyondBoundaryDisable = beyondBoundaryDisable;
			_fireAltAxisEvents = fireAltAxisEvents;
			_delayInputCycles = 0;
			this.IsActive = false;
		}

		public override void LoadContent()
		{			
			base.LoadContent();
			LoadItemData();

			int menuSize = 0;

			foreach (var item in _items)
			{
				item.Image = new ImageText(item.Text, true);
				item.Image.Alignment = (_layout == MenuLayout.Vertical) ? ImageAlignment.Centered : ImageAlignment.LeftCentered;
				item.Image.LoadContent();
				menuSize += (menuSize > 0 ? MENU_PADDING : 0) + (_layout == MenuLayout.Vertical ? (int)item.Image.Size.Y : (int)item.Image.Size.X);
			}

			int locX = this.Bounds.X;
			int locY = this.Bounds.Y;

			if (_layout == MenuLayout.Vertical)
			{
				locX += (int)(this.Bounds.Width / 2);
				locY += (int)(this.Bounds.Height - menuSize) / 2;
			}
			else
			{
				locX += (int)(this.Bounds.Width - menuSize) / 2;
				locY += (int)(this.Bounds.Height / 2);
			}

			foreach (var item in _items)
			{
				item.Image.Position = new Vector2(locX, locY);
				if (_layout == MenuLayout.Vertical)
					locY += (int)item.Image.Size.Y + MENU_PADDING;
				else
					locX += (int)item.Image.Size.X + MENU_PADDING;
			}

			this.CurrentIndex = 0;
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			foreach (var item in _items)
				item.Image.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			if (!this.IsActive)
			{
				for (int x = 0; x < _items.Count; x++)
					_items[x].Image.Color = (x == _currentIndex) ? MenuScreen.SelectedItemColor : MenuScreen.UnselectedItemColor;
				return;
			}

			UpdateActive(gameTime, processInput);			
		}

		public virtual void UpdateActive(GameTime gameTime, bool processInput)
		{
			for (int x = 0; x < _items.Count; x++)
				_items[x].Image.Color = MenuScreen.ActiveItemColor;

			if (processInput && (_delayInputCycles == 0))
				UpdateInput(gameTime);
			_delayInputCycles = Math.Max(0, _delayInputCycles - 1);

			base.Update(gameTime, processInput);
			foreach (var item in _items)
				item.Image.Update(gameTime);
		}

		public virtual void UpdateInput(GameTime gameTime)
		{
			bool beyondBoundary = false;

			if (InputManager.KeyPressed(this.ForwardKey))
			{
				beyondBoundary = (_currentIndex >= _items.Count - 1);

				int newIndex = Math.Min(_items.Count - 1, _currentIndex + 1);
				if (newIndex != _currentIndex)
				{
					this.CurrentIndex = newIndex;
					OnCurrentItemChange?.Invoke(this, new MenuEventArgs("currentChange", this.GetType().Name, _items[_currentIndex].Id));
				}

				if (beyondBoundary && _beyondBoundaryDisable)
					OnReadyDisable?.Invoke(this, new MenuEventArgs("beyondend", this.GetType().Name, null));
			}
			else if (InputManager.KeyPressed(this.BackwardKey))
			{
				beyondBoundary = (_currentIndex <= 0);

				int newIndex = Math.Max(0, _currentIndex - 1);
				if (newIndex != _currentIndex)
				{
					this.CurrentIndex = newIndex;
					OnCurrentItemChange?.Invoke(this, new MenuEventArgs("currentChange", this.GetType().Name, _items[_currentIndex].Id));
				}

				if (beyondBoundary && _beyondBoundaryDisable)
					OnReadyDisable?.Invoke(this, new MenuEventArgs("beyondbeginning", this.GetType().Name, null));
			}
			else if (InputManager.KeyPressed(Keys.Enter))
			{
				OnItemSelect?.Invoke(this, new MenuEventArgs("select", this.GetType().Name, _items[_currentIndex].Id));
			}
			else if (_escapeToDisable && InputManager.KeyPressed(Keys.Escape))
			{
				OnReadyDisable?.Invoke(this, new MenuEventArgs("escape", this.GetType().Name, null));
			}
			else if (_fireAltAxisEvents && InputManager.KeyPressed(this.AltAxisFowardKey))
			{
				OnReadyDisable?.Invoke(this, new MenuEventArgs("altfoward", this.GetType().Name, _items[_currentIndex].Id));
			}
			else if (_fireAltAxisEvents && InputManager.KeyPressed(this.AltAxisBackwardKey))
			{
				OnReadyDisable?.Invoke(this, new MenuEventArgs("altbackward", this.GetType().Name, _items[_currentIndex].Id));
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			foreach (var item in _items)
				item.Image.Draw(spriteBatch, this.IsActive ? MenuScreen.ENABLED_MENU_ALPHA : MenuScreen.DISABLED_MENU_ALPHA);
		}

		public int SetById(string id)
		{
			for (int i = 0; i < _items.Count; i++)
				if (_items[i].Id == id)
				{
					this.CurrentIndex = i;
					return i;
				}

			return -1;
		}

		protected virtual void LoadItemData()
		{
			using (var reader = new StreamReader($"Load\\Menu\\{this.GetType().Name}.json"))
			{
				_items = JsonConvert.DeserializeObject<List<MenuItem>>(reader.ReadToEnd());
			}
		}

		private void SetCurrentItemEffects(bool active)
		{
			if (_currentIndex < 0)
				return;

			if (active)
			{
				_items[_currentIndex].Image.AddEffect(new FadeCycleEffect(true));
			}
			else
			{
				_items[_currentIndex].Image.ClearEffects();
				_items[_currentIndex].Image.Alpha = MenuScreen.DEFAULT_ITEM_ALPHA;
			}
		}
	}
}
