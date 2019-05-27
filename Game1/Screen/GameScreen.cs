﻿using System;
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

		protected ImageTexture _backgroundImage;

		public Rectangle Bounds { get; set; }

		public event EventHandler OnReadyScreenUnload;

		public GameScreen(Rectangle bounds, string backgroundName)
		{
			this.Bounds = bounds;

			if (!String.IsNullOrWhiteSpace(backgroundName))
			{
				_backgroundImage = new ImageTexture($"{Game1.BackgroundRoot}/{backgroundName}", true);
				_backgroundImage.DrawArea = bounds;
				_backgroundImage.SourceRect = new Rectangle(0, 0, bounds.Width, bounds.Height);
			}
		}

		public virtual void LoadContent()
		{
			_content = new ContentManager(Game1.ServiceProvider);
			_backgroundImage?.LoadContent();
		}

		public virtual void UnloadContent()
		{
			_content?.Unload();
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
