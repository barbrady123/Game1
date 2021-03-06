﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	public class WorldTransition : WorldEntity
	{
		public Transition Transition { get; set; }

		public override bool IsSolid => false;

		public string DestinationMap { get; set; }

		public Point DestinationPosition { get; set; }

		public override string TooltipText => this.Transition.DisplayName;

		public WorldTransition(Transition transition, Vector2 position, ImageTexture icon, string destinationMap, Point destinationPosition)
		{
			this.Transition = transition;
			this.Position = position;
			this.Bounds = position.ExpandToRectangleCentered(Game1.TileHalfSize, Game1.TileHalfSize);
			this.Icon = icon;
			this.DestinationMap = destinationMap;
			this.DestinationPosition = destinationPosition;
			this.IsHighlighted = false;
		}

		public void Update(GameTime gameTime)
		{
			this.Icon.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			this.Icon.Draw(spriteBatch, position: this.Position + cameraOffset, highlight: this.IsHighlighted);
		}
	}
}
