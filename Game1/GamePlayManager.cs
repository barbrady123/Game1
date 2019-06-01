using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;

namespace Game1
{
	public class GamePlayManager : IActivatable
	{
		private const int BorderWidth = 3;
		private const int ViewWindowOffset = 25;
		private static readonly Rectangle _gameViewArea = new Rectangle(
			GamePlayManager.ViewWindowOffset + GamePlayManager.BorderWidth,
			GamePlayManager.ViewWindowOffset + GamePlayManager.BorderWidth,
			Game1.TileSize * Game1.GameViewAreaWidth,
			Game1.TileSize * Game1.GameViewAreaHeight);

		private readonly Rectangle _bounds;
		private readonly ActivationManager _activation;
		private readonly GamePlayCamera _camera;
		private readonly PhysicsManager _physics;
		private readonly World _world;
		private readonly InventoryWindow _inventoryWindow;

		private ImageTexture _gameViewBorder;

		public bool IsActive { get; set; }

		public GamePlayManager(Rectangle bounds)
		{
			_bounds = bounds;

			_world = new World();
			_world.Initialize();

			_gameViewBorder = GenerateGameViewBorder();
			_gameViewBorder.LoadContent();

			_camera = new GamePlayCamera(_world, _gameViewArea);
			_physics = new PhysicsManager(_world);
			_activation = new ActivationManager();
			_activation.Add(this);
			_activation.Activate(this);

			_activation.Add(_inventoryWindow = new InventoryWindow("Backpack", DialogButton.None, _bounds.CenteredRegion(1000, 600), null, _world.Character.Backpack, _world.Character.HotBar));
			_inventoryWindow.OnReadyDisable += _inventoryView_OnReadyDisable;
		}

		public void LoadContent()
		{
			ItemManager.LoadContent();
			_world.LoadContent();
			_camera.TerrainTileSheetName = _world.CurrentMap.TileSheet;
			_camera.TerrainLayerData  = _world.CurrentMap.Layers;
			_camera.LoadContent();
			_physics.CalculateParameters();
			_inventoryWindow.LoadContent();
		}

		public void UnloadContent()
		{
			ItemManager.UnloadContent();
			_gameViewBorder.UnloadContent();
			_world.UnloadContent();
			_camera.UnloadContent();
			_inventoryWindow.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			_inventoryWindow.Update(gameTime, true);

			// In this case "IsActive" effectively means the game is running, pausing the GamePlayManager pauses the game,
			// So anything like a modal that would cause the game to pause must be Updated above here...
			if (!this.IsActive)
				return;

			_gameViewBorder.Update(gameTime);
			_world.Update(gameTime);
			_physics.Update(gameTime);
			_camera.Update(gameTime);

			// TODO: Move this to a better location...
			if (InputManager.KeyPressed(Keys.I))
				_activation.Activate(_inventoryWindow);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_gameViewBorder.Draw(spriteBatch);
			_camera.Draw();
			_inventoryWindow.Draw(spriteBatch);
		}

		private ImageTexture GenerateGameViewBorder()
		{
			var texture = Util.GenerateBorderTexture(
				_gameViewArea.Width + (GamePlayManager.BorderWidth * 2),
				_gameViewArea.Height + (GamePlayManager.BorderWidth * 2),
				GamePlayManager.BorderWidth,
				Color.DarkSlateBlue);
			texture.Alignment = ImageAlignment.Centered;
			texture.Position = _gameViewArea.CenterVector();
			return texture;
		}

		private void _inventoryView_OnReadyDisable(object sender, EventArgs e)
		{
			_activation.Activate(this);
		}
	}
}
