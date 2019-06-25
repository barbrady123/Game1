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
using Game1.Enum;
using Game1.Items;

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

		public bool MovementOk(Rectangle mapBounds, Character character, List<Character> allChars)
		{
			if (character.Motion == Vector2.Zero)
				return true;

			var proposedBox = (character.Position + character.Motion).ExpandToRectangleCentered((int)_humanoidBoxSize.X / 2, (int)_humanoidBoxSize.Y / 2);

			// Map bounds
			if (!mapBounds.Contains(proposedBox))
				return false;

			// Solid blocks
			foreach (var solidBlock in _solidBlocks)
			{
				if (solidBlock.Intersects(proposedBox))
					return false;
			}

			// Other mobs
			foreach (var otherChar in allChars.Where(c => c != character))
			{
				// Eventually need to check mob "size" or "type" for bounding box settings...
				var otherCharBox = otherChar.Position.ExpandToRectangleCentered((int)_humanoidBoxSize.X / 2, (int)_humanoidBoxSize.Y / 2);
				if (otherCharBox.Intersects(proposedBox))
					return false;
			}

			// Solid interactives
			foreach (var solid in _world.Interactives.Where(i => i.Interactive.IsSolid))
			{
				if (solid.Bounds.Intersects(proposedBox))
					return false;
			}

			return true;
		}

		public void Update(GameTime gameTime)
		{
			var mapBounds = new Rectangle(0, 0, _world.CurrentMap.Width * Game1.TileSize, _world.CurrentMap.Height * Game1.TileSize);
			var allChars = _world.AllCharacters;
		
			foreach (var character in allChars)
			{
				var originalMotion = character.Motion;
				if (MovementOk(mapBounds, character, allChars))
				{
					character.Position += originalMotion;
					continue;
				}

				// Try X movement only...
				character.Motion = originalMotion.XVector();
				if (MovementOk(mapBounds, character, allChars))
				{
					character.Position += character.Motion;
					continue;
				}

				// Try Y movement only...
				character.Motion = originalMotion.YVector();
				if (MovementOk(mapBounds, character, allChars))
				{
					character.Position += character.Motion;
					continue;
				}
			}

			// Interactives...
			var activeBounds = _world.Character.ActiveItemBounds;
			if (activeBounds != Rectangle.Empty)
			{
				// Need to test weapons for combat also...
				if (_world.Character.ActiveItem.Item is ItemTool tool)
				{					
					for (int i = _world.Interactives.Count - 1; i >= 0; i--)
					{
						if (_world.Interactives[i].Health == null)
							continue;

						if (_world.Interactives[i].Bounds.Intersects(activeBounds))
						{
							_world.Interactives[i].Icon.AddEffect<JiggleEffect>(true);
							_world.Interactives[i].Health -= (int)(tool.Damage * _world.Interactives[i].Interactive.Effectiveness[tool.Type]);
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
	}
}
