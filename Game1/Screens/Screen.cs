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
		private Rectangle _bounds;

		public Rectangle Bounds
		{ 
			get { return _bounds; }
			set
			{
				if (_bounds != value)
				{
					_bounds = value;
					RepositionScreenObjects();
				}
			}
		}

		public virtual void RepositionScreenObjects()
		{
			if (_backgroundImage != null)
			{
				_backgroundImage.Position = this.Bounds.TopLeftVector();
				// We're basically tiling here instead of scaling....might be smarter to have an
				// option to scale instead of tile (not sure about performance???) and/or use
				// a larger black image as our base (does that even matter??)
				_backgroundImage.SourceRect = this.Bounds;
			}
		}

		public event EventHandler OnReadyScreenUnload;

		public Screen(Rectangle bounds, string backgroundName)
		{
			if (!String.IsNullOrWhiteSpace(backgroundName))
				_backgroundImage = new ImageTexture($"{Game1.BackgroundRoot}/{backgroundName}", true);
			this.Bounds = bounds;
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

		protected virtual void ReadyScreenUnload(object sender, ScreenEventArgs args = null)
		{
			OnReadyScreenUnload?.Invoke(sender, args);
		}
	}
}
