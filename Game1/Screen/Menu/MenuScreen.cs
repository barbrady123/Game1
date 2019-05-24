using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;

namespace Game1.Screen.Menu
{
	public class MenuScreen : GameScreen
	{
		private const int MENU_PADDING = 20;

		protected Vector2 _size;
		protected Vector2 _menuSize;
		protected int _currentIndex;
		protected List<MenuItem> _items;

		private FadeCycleEffect _selectedEffect;

		public MenuScreen(GraphicsDevice graphics, Vector2 size): base(graphics)
		{
			_size = size;
			_menuSize = Vector2.Zero;
			_backgroundImage = new Image(graphics, "Background/brick", null, true);
			_currentIndex = -1;
			_items = new List<MenuItem>();
			_selectedEffect = null;
		}

		public override void LoadContent(IServiceProvider services)
		{			
			base.LoadContent(services);
			foreach (var item in _items)
			{
				item.Image.LoadContent(services);
				_menuSize.X = Math.Max(_menuSize.X, item.Image.SourceRect.Width);
				_menuSize.Y += item.Image.SourceRect.Height + (_menuSize.Y > 0 ? MENU_PADDING : 0);
			}

			int locY = (int)(_size.Y - _menuSize.Y) / 2;

			foreach (var item in _items)
			{
				int locX = (int)(_size.X - (int)item.Image.SourceRect.Width) / 2;
				item.Image.Position = new Vector2(locX, locY);
				locY += (int)item.Image.SourceRect.Height + MENU_PADDING;
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
			if (processInput)
			{
				if (InputManager.Instance.KeyPressed(Keys.Down))
				{
					int newIndex = _currentIndex + 1;
					if (newIndex < _items.Count)
						SetCurrentIndex(newIndex);
				}
				else if (InputManager.Instance.KeyPressed(Keys.Up))
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
				item.Image.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
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
	}
}
