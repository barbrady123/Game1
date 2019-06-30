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
		private Size _humanoidBoxSize = new Size(28, 54);

		private World _world;
		//private List<Rectangle> _solidBlocks;
		private Rectangle _mapBounds;

		public PhysicsManager(World world)
		{
			_world = world;
		}

		// This is slooooooooow......do something about it eventually (also we can just test x and y movement seperately, instead of possibly testing 3 possibilities)
		public bool MovementOk(Character character, List<Character> allChars)
		{
			if (character.Motion == Vector2.Zero)
				return true;

			var proposedBox = (character.Position + character.Motion).ExpandToRectangleCentered(_humanoidBoxSize.Width / 2, _humanoidBoxSize.Height / 2);

			// Map bounds
			if (!_mapBounds.Contains(proposedBox))
				return false;

			// Solid blocks
			// These need to be stored in the mapObjects structure for much better performance here...this is currently checking the entire map!
			/*
			foreach (var solidBlock in _solidBlocks)
			{
				if (solidBlock.Intersects(proposedBox))
					return false;
			}
			*/
			// TODO: Just testing with the solid blocks, this should just handle all collisions...
			foreach (var block in _world.MapObjects.GetEntities(proposedBox).OfType<WorldSolid>())
			{
				if (block.Bounds.Intersects(proposedBox))
					return false;
			}

			// Other mobs
			foreach (var otherChar in allChars.Where(c => c != character))
			{
				// Eventually need to check mob "size" or "type" for bounding box settings...
				var otherCharBox = otherChar.Position.ExpandToRectangleCentered(_humanoidBoxSize.Width / 2, _humanoidBoxSize.Height / 2);
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
			var allChars = _world.AllCharacters;
		
			foreach (var character in allChars)
			{
				var originalMotion = character.Motion;
				if (MovementOk(character, allChars))
				{
					character.Position += originalMotion;
					continue;
				}

				// Try X movement only...
				character.Motion = originalMotion.XVector();
				if (MovementOk(character, allChars))
				{
					character.Position += character.Motion;
					continue;
				}

				// Try Y movement only...
				character.Motion = originalMotion.YVector();
				if (MovementOk(character, allChars))
				{
					character.Position += character.Motion;
					continue;
				}
			}

			var activeBounds = _world.Character.ActiveItemBounds;
			if (activeBounds != Rectangle.Empty)
			{
				if (_world.Character.ActiveItem.Item is ItemWeapon weapon)
				{
					for (int i = _world.NPCs.Count - 1; i >= 0; i--)
					{
						var npcBox = _world.NPCs[i].Position.ExpandToRectangleCentered(_humanoidBoxSize.Width / 2, _humanoidBoxSize.Height / 2);
						if (npcBox.Intersects(activeBounds))
						{
							_world.NPCs[i].SetImageEffect<ShakeEffect>();
							_world.NPCs[i].CurrentHP -= GameRandom.Next(weapon.MinDamage, weapon.MaxDamage);	// Obviously greatly simplified...
						}
					}
				}
				if (_world.Character.ActiveItem.Item is ItemTool tool)
				{					
					for (int i = _world.Interactives.Count - 1; i >= 0; i--)
					{
						if (_world.Interactives[i].Health == null)
							continue;

						if (_world.Interactives[i].Bounds.Intersects(activeBounds))
						{
							_world.Interactives[i].Icon.AddEffect<ShakeEffect>(true);
							_world.Interactives[i].Health -= (int)(tool.Damage * _world.Interactives[i].Interactive.Effectiveness[tool.Type]);
						}
					}
				}
			}
		}

		public void CalculateParameters()	
		{
			/*
			var solidBlocks = new Dictionary<Point, Rectangle>();

			foreach (var layer in _world.CurrentMap.Layers.Where(l => l.Type == LayerType.Solid))
			{
				for (int y = 0; y < layer.TileData.GetLength(1); y++)
				for (int x = 0; x < layer.TileData.GetLength(0); x++)
				{
					// Again, coords here are reversed so file data can "visually" match the screen...
					if (layer.TileData[y,x] < 0)
						continue;

					var point = new Point(x, y);
					if (!solidBlocks.ContainsKey(point))
						solidBlocks[point] = new Rectangle(x * Game1.TileSize, y * Game1.TileSize, Game1.TileSize, Game1.TileSize);
				}
			}

			_solidBlocks = solidBlocks.Values.ToList();
			*/

			_mapBounds = new Rectangle(0, 0, _world.CurrentMap.Width * Game1.TileSize, _world.CurrentMap.Height * Game1.TileSize);
		}
	}
}
