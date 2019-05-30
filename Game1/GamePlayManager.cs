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
using Game1.Items;

namespace Game1
{
	public class GamePlayManager : IActivatable
	{
		private readonly ActivationManager _activation;
		private readonly GamePlayCamera _camera;
		private readonly PhysicsManager _physics;
		private readonly World _world;
		private readonly DialogBox _dialogBox;
		private readonly ItemContainerView _inventoryView;

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

			// Inventory View
			_activation.Add(_inventoryView = new ItemContainerView(_world.Character.Backpack, "Backpack", new Rectangle(200, 200, 1200, 800)));
			_inventoryView.OnReadyDisable += _inventoryView_OnReadyDisable;
		}

		public void LoadContent()
		{
			_world.LoadContent();
			_camera.TerrainTileSheetName = _world.CurrentMap.TileSheet;
			_camera.TerrainLayerData  = _world.CurrentMap.Layers;
			_camera.LoadContent();
			_physics.CalculateParameters();
			_dialogBox.LoadContent();
			_inventoryView.LoadContent();
		}

		public void UnloadContent()
		{
			_world.UnloadContent();
			_camera.UnloadContent();
			_dialogBox.UnloadContent();
			_inventoryView.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			// This is up here in case a modal is active...this is the problem with self-registration in the ActivationManager...do we want to keep it this way or 
			// move modals outside the GamePlayManager?
			_dialogBox.Update(gameTime);
			_inventoryView.Update(gameTime);

			if (!this.IsActive)
				return;

			_world.Update(gameTime);
			_physics.Update(gameTime);
			_camera.Update(gameTime);


			// TODO: Move this to a better location...
			if (InputManager.KeyPressed(Keys.I))
				_activation.Activate(_inventoryView);
			else if (InputManager.KeyPressed(Keys.Escape))
				_activation.Activate(_dialogBox);
		}

		public void Draw()
		{
			_camera.Draw();
			_dialogBox.Draw();
			_inventoryView.Draw();
		}

		private void _dialogBox_OnButtonClick(object sender, EventArgs e)
		{
			_activation.Activate(this);
		}

		// Can we just have a general "modal closed" event hanlder?  
		private void _inventoryView_OnReadyDisable(object sender, EventArgs e)
		{
			_activation.Activate(this);
		}
	}
}
