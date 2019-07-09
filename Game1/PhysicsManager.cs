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

		public bool MovementOk(WorldCharacter character, List<WorldCharacter> allChars)
		{
			var proposedBox = (character.Position + character.Motion).ExpandToRectangleCentered(character.BoundsExpansionSize.Width, character.BoundsExpansionSize.Height);

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
					character.Move(originalMotion);
					continue;
				}

				// Try X movement only...
				character.Motion = originalMotion.XVector();
				if (MovementOk(character, allChars))
				{
					character.Move(character.Motion);
					continue;
				}

				// Try Y movement only...
				character.Motion = originalMotion.YVector();
				if (MovementOk(character, allChars))
				{
					character.Move(character.Motion);
					continue;
				}
			}

			// Active item bounds...
			// TODO: We support all CombatCharacters with ActiveItems...may need to add this (or just keep mob combat simpler?)...
			var activeBounds = _world.Player.ActiveItemBounds;
			if (activeBounds != Rectangle.Empty)
			{
				// Eventually we might want to allow other tools to do damage...
				if (_world.Player.ActiveItem.Item is ItemWeapon weapon)
				{
					// TODO: Re-implement this when mobs are available...right now this is invalid as NPCs (base WorldCharacters) are no longer attackable...
					/*
					foreach (var npc in _world.MapObjects.GetEntities<WorldCharacter>(activeBounds).Where(e => e != _world.Player))
					{
						if (npc.Bounds.Intersects(activeBounds))
						{
							npc.SetImageEffect<ShakeEffect>();
							npc.CurrentHP -= GameRandom.Next(weapon.MinDamage, weapon.MaxDamage);	// Obviously greatly simplified...
						}
					}
					*/
				}
			}
		}

		public void CalculateParameters()	
		{
			_mapBounds = new Rectangle(0, 0, _world.CurrentMap.Width * Game1.TileSize, _world.CurrentMap.Height * Game1.TileSize);
		}
	}
}
