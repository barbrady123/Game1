using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Screens
{
	public abstract class Screen
	{
		protected ImageTexture _backgroundImage;

		public Rectangle Bounds { get; set; }

		public event EventHandler OnReadyScreenUnload;

		public Screen(Rectangle bounds, string backgroundName)
		{
			this.Bounds = bounds;

			if (!String.IsNullOrWhiteSpace(backgroundName))
				_backgroundImage = new ImageTexture($"{Game1.BackgroundRoot}/{backgroundName}", true) { Position = bounds.TopLeftVector() };
		}

		public virtual void LoadContent()
		{
			if (_backgroundImage != null)
			{
				_backgroundImage.LoadContent();
				// This will let a smaller background tile...
				_backgroundImage.SourceRect = this.Bounds;
			}
		}

		public virtual void UnloadContent()
		{
			_backgroundImage?.UnloadContent();
		}

		public virtual void Update(GameTime gameTime, bool processInput)
		{
			_backgroundImage?.Update(gameTime);	
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			_backgroundImage?.Draw(spriteBatch);
		}

		protected void ReadyScreenUnload(object sender, ScreenEventArgs args = null)
		{
			OnReadyScreenUnload?.Invoke(sender, args);
		}
	}
}
