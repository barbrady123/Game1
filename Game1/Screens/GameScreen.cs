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
		private const int BorderWidth = 3;
		private const int ViewWindowOffset = 25;
		private static readonly Rectangle _gameViewArea = new Rectangle(
			GameScreen.ViewWindowOffset + GameScreen.BorderWidth,
			GameScreen.ViewWindowOffset + GameScreen.BorderWidth,
			Game1.TileSize * Game1.GameViewAreaWidth,
			Game1.TileSize * Game1.GameViewAreaHeight);
		private GamePlayManager _gameplay;
		private ImageTexture _gameViewBorder;
		private World _world;

		//private ActivationManager _activation = new ActivationManager();

		public GameScreen(Rectangle bounds): base(bounds, "rock")
		{
			_world = new World();
			_gameplay = new GamePlayManager(_gameViewArea, _world) { IsActive = true };
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_gameplay.LoadContent();
			_gameViewBorder = Util.GenerateBorderTexture(
				_gameViewArea.Width + (GameScreen.BorderWidth * 2),
				_gameViewArea.Height + (GameScreen.BorderWidth * 2),
				GameScreen.BorderWidth,
				Color.DarkSlateBlue);
			_gameViewBorder.Alignment = ImageAlignment.Centered;
			_gameViewBorder.Position = _gameViewArea.CenterVector();
			_gameViewBorder.LoadContent();
			_gameplay.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_gameViewBorder.UnloadContent();
			_gameplay.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_gameViewBorder.Update(gameTime);
			_gameplay.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{	
			base.Draw(spriteBatch);
			_gameViewBorder.Draw(spriteBatch);
			_gameplay.Draw();
		}
	}
}
