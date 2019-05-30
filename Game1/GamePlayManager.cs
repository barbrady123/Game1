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
	public class GamePlayManager
	{
		private readonly GamePlayCamera _camera;
		private readonly PhysicsManager _physics;
		private readonly World _world;

		public GamePlayManager(Rectangle gameViewArea, World world)
		{
			_world = world;
			_world.Initialize();
			_camera = new GamePlayCamera(_world, gameViewArea);
			_physics = new PhysicsManager(_world);
		}

		public void LoadContent()
		{
			_world.LoadContent();
			_camera.TerrainTileSheetName = _world.CurrentMap.TileSheet;
			_camera.TerrainLayerData  = _world.CurrentMap.Layers;
			_camera.LoadContent();
			_physics.CalculateParameters();
		}

		public void UnloadContent()
		{
			_world.UnloadContent();
			_camera.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			_world.Update(gameTime);
			_physics.Update(gameTime);
			_camera.Update(gameTime);
		}

		public void Draw()
		{
			_camera.Draw();
		}
	}
}
