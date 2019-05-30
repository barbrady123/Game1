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
	public class PhysicsManager
	{
		// Eventually these should be a config file (either load here or pass a PhysicsConfig object from outside)....
		private Vector2 _playerBoxSize = new Vector2(28.0f, 54.0f);

		private World _world;
		private List<Rectangle> _solidBlocks;

		public PhysicsManager(World world)
		{
			_world = world;
		}

		public void Update(GameTime gameTime)
		{
			var mapBounds = new Rectangle(0, 0, _world.CurrentMap.Width * Game1.TileSize, _world.CurrentMap.Height * Game1.TileSize);
			var playerBox = _world.Character.Position.ExpandToRectange((int)_playerBoxSize.X / 2, (int)_playerBoxSize.Y / 2);

			// TODO: This needs to support continuing is a single direction if 2 keys are pushed but only one direction is blocked
			// Player - Map bounds
			if (!mapBounds.Contains(playerBox))
			{
				_world.Character.RevertPosition();
			}
			else
			{
				// Player - Solid blocks
				foreach (var solidBlock in _solidBlocks)
				{
					if (solidBlock.Intersects(playerBox))
					{
						_world.Character.RevertPosition();
						break;
					}
				}
			}

			// NPC code is reusable technically, but might evolve differently over time...

			// NPC - Map bounds
			foreach (var npc in _world.NPCs)
			{
				var npcBox = npc.Position.ExpandToRectange((int)_playerBoxSize.X / 2, (int)_playerBoxSize.Y / 2);
				if (!mapBounds.Contains(npcBox))
				{
					npc.RevertPosition();
				}
				else
				{
					// NPC - Solid blocks
					foreach (var solidBlock in _solidBlocks)
					{
						if (solidBlock.Intersects(npcBox))
						{
							npc.RevertPosition();
							break;
						}
					}
				}
			}

			// Eventually...mobs, etc...things that actually move will make this more complex...
		}

		public void CalculateParameters()	
		{
			var solidBlocks = new Dictionary<Point, Rectangle>();

			foreach (var layer in _world.CurrentMap.Layers.Where(l => l.IsSolid))
			{
				for (int y = 0; y < layer.TileData.GetLength(1); y++)
				for (int x = 0; x < layer.TileData.GetLength(0); x++)
				{
					// Again, coords here are reversed so file data can "visually" match the screen...
					if (layer.TileData[y,x].TileIndex < 0)
						continue;

					var point = new Point(x, y);
					if (!solidBlocks.ContainsKey(point))
						solidBlocks[point] = new Rectangle(x * Game1.TileSize, y * Game1.TileSize, Game1.TileSize, Game1.TileSize);
				}
			}

			_solidBlocks = solidBlocks.Values.ToList();
		}
	}
}
