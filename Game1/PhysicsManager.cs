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

namespace Game1
{
	public class PhysicsManager
	{
		// Eventually these should be a config file (either load here or pass a PhysicsConfig object from outside)....
		private Vector2 _humanoidBoxSize = new Vector2(28.0f, 54.0f);

		private World _world;
		private List<Rectangle> _solidBlocks;

		public PhysicsManager(World world)
		{
			_world = world;
		}

		public void Update(GameTime gameTime)
		{
			var mapBounds = new Rectangle(0, 0, _world.CurrentMap.Width * Game1.TileSize, _world.CurrentMap.Height * Game1.TileSize);
			var allChars = _world.AllCharacters;
		
			foreach (var character in allChars)
			{
				if (character.Motion == Vector2.Zero)
					continue;

				// Eventually need to check mob "size" or "type" for bounding box settings...
				var charBox = character.Position.ExpandToRectangle((int)_humanoidBoxSize.X / 2, (int)_humanoidBoxSize.Y / 2);
				bool moved = true;

				// Map bounds
				if (!mapBounds.Contains(charBox))
				{				
					character.RevertPosition();
					moved = AttemptPartialMotion(character, mapBounds, IntersectionType.Contains, true);
				}

				if (moved)
				{
					// Solid blocks
					foreach (var solidBlock in _solidBlocks)
					{
						if (solidBlock.Intersects(charBox))
						{						
							character.RevertPosition();
							if (!AttemptPartialMotion(character, solidBlock, IntersectionType.Intersects, false))
								break;
						}
					}
				}

				if (moved)
				{
					// Other mobs
					foreach (var otherChar in allChars.Where(c => c != character))
					{
						// Eventually need to check mob "size" or "type" for bounding box settings...
						var otherCharBox = otherChar.Position.ExpandToRectangle((int)_humanoidBoxSize.X / 2, (int)_humanoidBoxSize.Y / 2);
						if (otherCharBox.Intersects(charBox))
						{
							character.RevertPosition();
							//if (!AttemptPartialMotion(character, otherCharBox, IntersectionType.Intersects, false))
							//	break;
						}
					}
				}
			}
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

		private bool AttemptPartialMotion(Character character, Rectangle boundary, IntersectionType type, bool condition)
		{
			bool acceptHorizontal = false;
			bool acceptVeritical = false;

			var motion = character.Motion;
			if ((motion.X == 0.0f) || (motion.Y == 0.0f))
				return false;

			var proposedHorizontalPosition = (character.Position + new Vector2(motion.X, 0.0f)).ExpandToRectangle((int)_humanoidBoxSize.X / 2, (int)_humanoidBoxSize.Y / 2);
			var proposedVerticalPosition = (character.Position + new Vector2(0.0f, motion.Y)).ExpandToRectangle((int)_humanoidBoxSize.X / 2, (int)_humanoidBoxSize.Y / 2);

			if (type == IntersectionType.Contains)
			{
				if (boundary.Contains(proposedHorizontalPosition) == condition)
					acceptHorizontal = true;
				else if (boundary.Contains(proposedVerticalPosition) == condition)
					acceptVeritical = true;
			}
			else if (type == IntersectionType.Intersects)
			{
				if (boundary.Intersects(proposedHorizontalPosition) == condition)
					acceptHorizontal = true;
				else if (boundary.Intersects(proposedVerticalPosition) == condition)
					acceptVeritical = true;
			}

			if (acceptHorizontal)
			{
				character.Motion = new Vector2(motion.X, 0);
				character.Position += character.Motion;
			}
			else if (acceptVeritical)
			{
				character.Motion = new Vector2(0, motion.Y);
				character.Position += character.Motion;
			}

			return (acceptHorizontal || acceptVeritical);
		}
	}
}
