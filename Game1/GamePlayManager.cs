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
using Game1.Screens;

namespace Game1
{
	public class GamePlayManager : IActivatable
	{
		private readonly ActivationManager _activation;
		private readonly GamePlayCamera _camera;
		private readonly PhysicsManager _physics;
		private readonly World _world;
		private readonly DialogBox _dialogBox;


		public bool IsActive { get; set; }

		public GamePlayManager(Rectangle gameViewArea, World world)
		{
			_world = world;
			_world.Initialize();
			_camera = new GamePlayCamera(_world, gameViewArea);
			_physics = new PhysicsManager(_world);
			_activation = new ActivationManager();
			_activation.Add(this);
			_activation.Activate(this);

			// Dialog
			_activation.Add(_dialogBox = new DialogBox("Paused", DialogButton.Ok, new Rectangle(600, 500, 400, 200), null));
			_dialogBox.OnButtonClick += _dialogBox_OnButtonClick;
			_dialogBox.OnReadyDisable += _dialogBox_OnButtonClick;
		}

		public void LoadContent()
		{
			_world.LoadContent();
			_camera.TerrainTileSheetName = _world.CurrentMap.TileSheet;
			_camera.TerrainLayerData  = _world.CurrentMap.Layers;
			_camera.LoadContent();
			_physics.CalculateParameters();
			_dialogBox.LoadContent();
		}

		public void UnloadContent()
		{
			_world.UnloadContent();
			_camera.UnloadContent();
			_dialogBox.UnloadContent();
		}

		private int count = 0;

		public void Update(GameTime gameTime)
		{
			count++;
			_dialogBox.Update(gameTime);

			if (!this.IsActive)
				return;

			_world.Update(gameTime);
			_physics.Update(gameTime);
			_camera.Update(gameTime);


			if (InputManager.KeyPressed(Keys.Escape))
				_activation.Activate(_dialogBox);
		}

		public void Draw()
		{
			_camera.Draw();
			_dialogBox.Draw();
		}

		private void _dialogBox_OnButtonClick(object sender, EventArgs e)
		{
			_activation.Activate(this);
		}
	}
}
