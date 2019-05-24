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

namespace Game1.Screen.Menu
{
	public class MenuScreen : GameScreen
	{
		private const int MENU_PADDING = 20;

		protected Vector2 _size;
		protected Vector2 _menuSize;
		protected int _currentIndex;

		protected List<MenuItem> _items;

		public MenuScreen(GraphicsDevice graphics, Vector2 size): base(graphics)
		{
			_size = size;
			_menuSize = Vector2.Zero;
			_backgroundImage = new Image(graphics, "Background/brick", null, true);
			_currentIndex = -1;
			_items = new List<MenuItem>();
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
				/*
				if (InputManager.Instance.KeyPressed(Keys.Enter))
				{
					_titleText.IsActive = false;
					ReadyScreenUnload(this);
				}
				*/
			}

			base.Update(gameTime, processInput);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			foreach (var item in _items)
				item.Image.Draw(spriteBatch);
		}
	}
}
