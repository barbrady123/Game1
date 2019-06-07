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
	/// <summary>
	/// TODO: This should just take a Position and optional Size...and calculate based on items if size isn't specified...
	/// This would greatly simplify other parts of the code where the items are dynamic, etc....and we don't have arbitrary
	/// menu sizes, such as "the entire screen", etc...
	/// </summary>
	public class MenuScreen : Screen, IActivatable
	{
		public const int MENU_PADDING = 20;
		private const float ENABLED_MENU_ALPHA = 1.0f;
		private const float DISABLED_MENU_ALPHA = 0.4f;
		private const float DEFAULT_ITEM_ALPHA = 1.0f;

		private static readonly Color ActiveItemColor = Color.White;
		private static readonly Color SelectedItemColor = new Color(240, 240, 240);
		private static readonly Color UnselectedItemColor = new Color(100, 100, 100);

		protected string EventSource { get; set; }
		protected int? EventSourceIndex { get; set; } = null;
		protected int _currentIndex;
		protected List<MenuItem> _items;
		protected MenuLayout _layout; 

		private bool _isActive;
		private bool _escapeToDisable;

		public event EventHandler OnReadyDisable;
		public event EventHandler OnCurrentItemChange;
		public event EventHandler OnItemSelect;
		public event EventHandler OnMouseIn;
		public event EventHandler OnMouseOut;

		private bool _mouseOverMenu;

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
				if (_currentIndex == value)
					return;
				
				SetCurrentItemEffects(false);
				_currentIndex = value;
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
						  string background = "brick",
						  bool escapeToDisable = false): base(bounds, background)
		{
			this.EventSource = this.GetType().Name;
			_currentIndex = -1;
			_items = new List<MenuItem>();
			_layout = layout;
			_escapeToDisable = escapeToDisable;
			_delayInputCycles = 0;
			_mouseOverMenu = false;
			this.IsActive = false;
		}

		public override void LoadContent()
		{			
			base.LoadContent();
			_items = LoadItemData();

			int menuSize = 0;

			foreach (var item in _items)
			{
				item.Image = new ImageText(item.Text, true);
				item.Image.Alignment = ImageAlignment.LeftTop;
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
				var size = item.Image.Size;
				item.Image.Position = new Vector2(
					locX - ((_layout == MenuLayout.Vertical) ? (int)(size.X / 2) : 0),
					locY - ((_layout == MenuLayout.Horizontal) ? (int)(size.Y / 2) : 0));
				item.Bounds = item.Image.Position.ExpandToRectangeTopLeft(size.X, size.Y);
				if (_layout == MenuLayout.Vertical)
					locY += (int)item.Image.Size.Y + MENU_PADDING;
				else
					locX += (int)item.Image.Size.X + MENU_PADDING;
			}
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			foreach (var item in _items)
				item.Image.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);

			// Need to check this even if menu is inactive, in case this triggers the container to activate it...
			bool mouseOverMenu = InputManager.MouseOver(this.Bounds);
			if (mouseOverMenu != _mouseOverMenu)
			{
				if (mouseOverMenu)
					OnMouseIn?.Invoke(this, new MouseEventArgs());
				else
					OnMouseOut?.Invoke(this, new MouseEventArgs());
			}

			_mouseOverMenu = mouseOverMenu;

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

			foreach (var item in _items)
				item.Image.Update(gameTime);
		}

		public virtual void UpdateInput(GameTime gameTime)
		{
			bool mouseOverItem = false;

			for (int i = 0; i < _items.Count; i++)
			{
				if (!InputManager.MouseOver(_items[i].Bounds))
					continue;

				mouseOverItem = true;

				if (this.CurrentIndex != i)
				{
					this.CurrentIndex = i;
					OnCurrentItemChange?.Invoke(this, new MenuEventArgs("currentChange", this.EventSource, this.EventSourceIndex, _items[_currentIndex].Id));
				}

				if (InputManager.LeftMouseClick())
					OnItemSelect?.Invoke(this, new MenuEventArgs("select", this.EventSource, this.EventSourceIndex, _items[_currentIndex].Id));				}

			if (!mouseOverItem)
				this.CurrentIndex = -1;

			if (_escapeToDisable && InputManager.KeyPressed(Keys.Escape, true))
			{
				OnReadyDisable?.Invoke(this, new MenuEventArgs("escape", this.EventSource, this.EventSourceIndex, null));
			}

			if (_mouseOverMenu)
				InputManager.BlockButtonClicks();
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

		protected virtual List<MenuItem> LoadItemData()
		{
			using (var reader = new StreamReader($"Load\\Menu\\{this.GetType().Name}.json"))
			{
				return JsonConvert.DeserializeObject<List<MenuItem>>(reader.ReadToEnd());
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

		public static Size CalculateMenuSize(int padding, int spacing, List<string> options, MenuLayout layout)
		{
			float width = 0;
			float height = 0;

			foreach (var option in options)
			{
				var size = FontManager.MeasureString(option);
				width = (layout == MenuLayout.Vertical) ? Math.Max(width, size.X) : width + size.X;
				height = (layout == MenuLayout.Vertical) ? height + size.Y : Math.Max(height, size.Y);
			}

			int paddingWidth = padding * 2;
			if (layout == MenuLayout.Horizontal)
				paddingWidth += padding * (options.Count - 1);

			int paddingHeight = padding * 2;
			if (layout == MenuLayout.Vertical)
				paddingHeight += padding * (options.Count - 1);

			return new Size((int)width + paddingWidth, (int)height + paddingHeight);
		}

		protected virtual void ReadyDisable(object sender, MenuEventArgs e)
		{
			OnReadyDisable?.Invoke(sender, e);
		}
	}
}
