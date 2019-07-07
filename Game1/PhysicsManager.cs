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
		public static readonly Size HumanoidBoxSize = new Size(28, 54);

		private World _world;
		//private List<Rectangle> _solidBlocks;
		private Rectangle _mapBounds;

		public PhysicsManager(World world)
		{
			_world = world;
		}

		public bool MovementOk(Character character, List<Character> allChars)
		{
			var proposedBox = (character.Position + character.Motion).ExpandToRectangleCentered(HumanoidBoxSize.Width / 2, HumanoidBoxSize.Height / 2);

			if (!_mapBounds.Contains(proposedBox))
				return false;

			// I *think* we can optimize this GetEntities by getting a slightly bigger box initially and caching it...
			// or at the very least, make GetEntities return IEnumerable so we possibly don't have to find all the entities
			// if there's an early collision...
			foreach (var obj in _world.MapObjects.GetEntities(proposedBox).Where(o => o.IsSolid && (o != character)))
			{
				if (obj.Bounds.Intersects(proposedBox))
					return false;
			}

			return true;
		}

		public void Update(GameTime gameTime)
		{
			var allChars = _world.AllCharacters;
		
			foreach (var character in allChars.Where(c => c.Motion != Vector2.Zero))
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

			// Active item bounds...
			var activeBounds = _world.Character.ActiveItemBounds;
			if (activeBounds != Rectangle.Empty)
			{
				// Eventually we might want to allow other tools to do damage...
				if (_world.Character.ActiveItem.Item is ItemWeapon weapon)
				{
					foreach (var npc in _world.MapObjects.GetEntities<NPC>(activeBounds))
					{
						if (npc.Bounds.Intersects(activeBounds))
						{
							npc.SetImageEffect<ShakeEffect>();
							npc.CurrentHP -= GameRandom.Next(weapon.MinDamage, weapon.MaxDamage);	// Obviously greatly simplified...
						}
					}
				}
			}
		}

		public void CalculateParameters()	
		{
			_mapBounds = new Rectangle(0, 0, _world.CurrentMap.Width * Game1.TileSize, _world.CurrentMap.Height * Game1.TileSize);
		}
	}
}
