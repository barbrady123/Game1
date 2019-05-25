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

		protected int _currentIndex;
		protected List<MenuItem> _items;
		protected MenuLayout _layout; 

		private FadeCycleEffect _selectedEffect;

		private Keys ForwardKey => (_layout == MenuLayout.Vertical) ? Keys.Down : Keys.Right;
		private Keys BackwardKey => (_layout == MenuLayout.Vertical) ? Keys.Up : Keys.Left;

		public bool IsActive { get; set; }

		public MenuScreen(Rectangle bounds, MenuLayout layout = MenuLayout.Vertical, bool hasBackground = true): base(bounds, hasBackground ? "brick" : null)
		{
			_currentIndex = -1;
			_items = new List<MenuItem>();
			_selectedEffect = null;
			_layout = layout;
			this.IsActive = true;
		}

		public override void LoadContent()
		{			
			base.LoadContent();
			LoadItemData();

			int menuSize = 0;

			foreach (var item in _items)
			{
				item.Image.LoadContent();
				menuSize +=  (menuSize > 0 ? MENU_PADDING : 0) + (_layout == MenuLayout.Vertical ? item.Image.SourceRect.Height : item.Image.SourceRect.Width);
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
					locY += (int)item.Image.SourceRect.Height + MENU_PADDING;
				else
					locX += (int)item.Image.SourceRect.Width + MENU_PADDING;

				if (item.SubMenu != null)
				{
					int subLocX = locX;
					int subLocY = locY;

					if (_layout == MenuLayout.Vertical)
					{
						subLocX += item.Image.SourceRect.Width + MENU_PADDING;
						item.SubMenu.Bounds = new Rectangle(subLocX, subLocY, 200, item.Image.SourceRect.Height);	// No idea how to determine width
					}
					else
					{
						subLocY += item.Image.SourceRect.Height + MENU_PADDING;
						item.SubMenu.Bounds = new Rectangle(subLocX, subLocY, item.Image.SourceRect.Width, 200);	// No idea how to determine height
					}
					
					item.SubMenu.LoadContent();
				}
			}

			SetCurrentIndex(0);
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			foreach (var item in _items)
			{
				item.Image.UnloadContent();
				item.SubMenu?.UnloadContent();
			}
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			if (!this.IsActive)
				return;

			if (processInput)
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
			}

			base.Update(gameTime, processInput);
			foreach (var item in _items)
			{
				item.Image.Update(gameTime);
				item.SubMenu?.Update(gameTime, processInput);	// Not sure how to get this parameter
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			// I *think* we wanna draw menus even if they are not active
			base.Draw(spriteBatch);

			foreach (var item in _items)
			{
				item.Image.Draw(spriteBatch);
				item.SubMenu?.Draw(spriteBatch);
			}
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

			foreach (var item in _items)
			{
				item.Image = new ImageText(item.Text, true);
				item.Image.Alignment = (_layout == MenuLayout.Vertical) ? ImageAlignment.Centered : ImageAlignment.LeftCentered;
				if (item.HorizontalAlignment == HorizontalAlignment.Right)
					item.Image.Alignment = ImageAlignment.RightCentered;

				item.LinkAction = ActionFromMethodName(item.Link ?? item.Text.Replace(" ", ""));
				if (item.Type == "text")
				{
					// Input text...
				}
				else if (item.Type == "sub")
				{
					item.SubMenu = (MenuScreen)Activator.CreateInstance(Type.GetType($"{this.GetType().Namespace}.{item.Target}"), Rectangle.Empty);
					item.SubMenu.IsActive = false;
				}
				else
				{
					item.LinkAction = ActionFromMethodName(item.Link ?? item.Text.Replace(" ", ""));
				}
			}
		}
	}
}
