using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
	public class World
	{
		private Rectangle _currentMapSourceRect;
		private Rectangle _drawArea;

		public Character Player { get; set; }
		public Map CurrentMap { get; set; }

		public Rectangle DrawArea
		{ 
			get { return _drawArea; }
			set {
				_drawArea = value;
				if (this.CurrentMap != null)
					this.CurrentMap.DrawArea = _drawArea;
			}
		}

		public void LoadContent()
		{
			this.CurrentMap.LoadContent();
			this.Player.LoadContent();
		}

		public void UnloadContent()
		{
			this.CurrentMap.UnloadContent();
			this.Player.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			this.CurrentMap.Update(gameTime);
			this.Player.Update(gameTime);

			var drawAreaSize = this.DrawArea.SizeVector();
			var drawAreaPadding = this.DrawArea.SizeVector() / 2;
			var mapSize = this.CurrentMap.GroundSize;

			int sourceX = 0;
			int sourceY = 0;
			
			if (this.Player.Position.X > drawAreaPadding.X)
			{
				if (this.Player.Position.X > this.CurrentMap.GroundSize.X - (drawAreaPadding.X))
					sourceX = this.DrawArea.Width;
				else
					sourceX = (int)(this.Player.Position.X - drawAreaPadding.X);
			}

			if (this.Player.Position.Y > drawAreaPadding.Y)
			{
				if (this.Player.Position.Y > this.CurrentMap.GroundSize.Y - (drawAreaPadding.Y))
					sourceY = this.DrawArea.Height;
				else
					sourceY = (int)(this.Player.Position.Y - drawAreaPadding.Y);
			}

			_currentMapSourceRect = new Rectangle(sourceX, sourceY, this.DrawArea.Width, this.DrawArea.Height);
			this.CurrentMap.SourceRect = _currentMapSourceRect;
		}

		public void Draw(SpriteBatch spriteBatch)
		{						
			this.CurrentMap.Draw(spriteBatch);
			this.Player.Draw(spriteBatch);
		}
	}
}
