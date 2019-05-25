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

namespace Game1.Screen.Menu
{
	public class MenuScreen : GameScreen
	{
		private const int MENU_PADDING = 20;
		private static readonly Color ActiveItemColor = Color.White;
		private static readonly Color SelectedItemColor = new Color(240, 240, 240);
		private static readonly Color UnselectedItemColor = new Color(100, 100, 100);

		protected int _currentIndex;
		protected List<MenuItem> _items;
		protected MenuLayout _layout; 

		private FadeCycleEffect _selectedEffect;
		private bool _isActive;
		private bool _escapeToDisable;

		private Keys ForwardKey => (_layout == MenuLayout.Vertical) ? Keys.Down : Keys.Right;
		private Keys BackwardKey => (_layout == MenuLayout.Vertical) ? Keys.Up : Keys.Left;

		public event EventHandler OnReadyMenuDisable;

		public bool IsActive
		{
			get { return _isActive; }
			set {
				_isActive = value;
				SetItemsAlpha(_isActive);
			}
		}

		public MenuScreen(Rectangle bounds,
						  MenuLayout layout = MenuLayout.Vertical,
						  bool hasBackground = true,
						  bool escapeToDisable = false): base(bounds, hasBackground ? "brick" : null)
		{
			_currentIndex = -1;
			_items = new List<MenuItem>();
			_selectedEffect = null;
			_layout = layout;
			_escapeToDisable = escapeToDisable;
			this.IsActive = true;
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

				item.LinkAction = ActionFromMethodName(item.Link ?? item.Text.Replace(" ", ""));
				menuSize +=  (menuSize > 0 ? MENU_PADDING : 0) + (_layout == MenuLayout.Vertical ? item.Image.SourceRect.Height : item.Image.SourceRect.Width);
			}

			SetItemsAlpha(_isActive);

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
					locY += (int)item.Image.SourceRect.Height + MENU_PADDING;
				else
					locX += (int)item.Image.SourceRect.Width + MENU_PADDING;
			}

			SetCurrentIndex(0);
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

			if (processInput)
			{
				UpdateInput(gameTime);
			}

			base.Update(gameTime, processInput);
			foreach (var item in _items)
				item.Image.Update(gameTime);
		}

		public virtual void UpdateInput(GameTime gameTime)
		{
			if (InputManager.Instance.KeyPressed(this.ForwardKey))
			{
				int newIndex = _currentIndex + 1;
				if (newIndex < _items.Count)
					SetCurrentIndex(newIndex);
			}
			else if (InputManager.Instance.KeyPressed(this.BackwardKey))
			{
				int newIndex = _currentIndex - 1;
				if (newIndex >= 0)
					SetCurrentIndex(newIndex);
			}
			else if (InputManager.Instance.KeyPressed(Keys.Enter))
			{
				if (_items[_currentIndex].LinkAction != null)
					_items[_currentIndex].LinkAction.Invoke();
			}
			else if (_escapeToDisable && InputManager.Instance.KeyPressed(Keys.Escape))
			{
				OnReadyMenuDisable?.Invoke(this, null);
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			// I *think* we wanna draw menus even if they are not active
			base.Draw(spriteBatch);

			foreach (var item in _items)
				item.Image.Draw(spriteBatch);
		}

		private void SetCurrentIndex(int index)
		{
			if (_items.Count < index)
				return;
						
			if (_currentIndex >= 0)
			{
				_items[_currentIndex].Image.Effects.Clear();
				_items[_currentIndex].Image.Alpha = 1.0f;
			}

			_currentIndex = index;
			_items[_currentIndex].Image.Effects.Add(_selectedEffect = new FadeCycleEffect(_items[_currentIndex].Image, true));
		}

		protected Action ActionFromMethodName(string name)
		{
			Type thisType = this.GetType();
			MethodInfo theMethod = thisType.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
			return (Action) Delegate.CreateDelegate(typeof(Action), this, theMethod);
		}

		protected virtual void LoadItemData()
		{
			using (var reader = new StreamReader($"Load\\Menu\\{this.GetType().Name}.json"))
			{
				_items = JsonConvert.DeserializeObject<List<MenuItem>>(reader.ReadToEnd());
			}
		}

		private void SetItemsAlpha(bool isActive)
		{
			foreach (var item in _items)
				item.Image.Alpha = IsActive ? 1.0f : 0.3f;
		}

		protected void ReadyMenuDisable(object sender, MenuEventArgs args = null)
		{
			OnReadyMenuDisable?.Invoke(sender, args);
		}
	}
}
