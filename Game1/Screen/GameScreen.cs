using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Screen
{
	public abstract class GameScreen
	{
		private ContentManager _content;

		protected Image _backgroundImage;
		protected GraphicsDevice _graphics;

		public event EventHandler OnReadyScreenUnload;

		protected void ReadyScreenUnload(object sender, ScreenEventArgs args = null)
		{
			OnReadyScreenUnload?.Invoke(sender, args);
		}

		public GameScreen(GraphicsDevice graphics)
		{
			_graphics = graphics;
		}

		public virtual void LoadContent(IServiceProvider services)
		{
			_content = new ContentManager(services);
			_backgroundImage.LoadContent(services);
		}

		public virtual void UnloadContent()
		{
			if (_content != null)
				_content.Unload();
		}

		public virtual void Update(GameTime gameTime, bool processInput)
		{
			_backgroundImage.Update(gameTime);	
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			_backgroundImage.Draw(spriteBatch);
		}

	}
}
