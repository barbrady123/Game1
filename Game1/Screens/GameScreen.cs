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

namespace Game1.Screens
{
	public class GameScreen : Screen
	{
		private World _world;
		private const int BorderWidth = 3;
		private Rectangle _mapAreaBounds = new Rectangle(20 + GameScreen.BorderWidth, 20 + GameScreen.BorderWidth, Game1.TileSize * 7, Game1.TileSize * 7);

		private ActivationManager _activation = new ActivationManager();
		private ImageTexture _mapAreaBorder;

		public GameScreen(Rectangle bounds, World world): base(bounds, "stone")
		{
			_world = world;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_mapAreaBorder = Util.GenerateBorderTexture(
				_mapAreaBounds.Width + (GameScreen.BorderWidth * 2),
				_mapAreaBounds.Height + (GameScreen.BorderWidth * 2),
				GameScreen.BorderWidth,
				Color.DarkSlateBlue);
			_mapAreaBorder.Alignment = ImageAlignment.Centered;
			_mapAreaBorder.Position = _mapAreaBounds.CenterVector();
			_mapAreaBorder.LoadContent();
			_world.DrawArea = _mapAreaBounds;
			_world.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_mapAreaBorder.UnloadContent();
			_world.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_mapAreaBorder.Update(gameTime);
			_world.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{	
			base.Draw(spriteBatch);
			_mapAreaBorder.Draw(spriteBatch);
			_world.Draw(spriteBatch);
		}
	}
}
