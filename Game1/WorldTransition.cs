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
using Game1.Items;

namespace Game1
{
	public class WorldTransition
	{
		public Transition Transition { get; set; }

		public Vector2 Position { get; set; }

		public ImageTexture Icon { get; set; }

		public string DestinationMap { get; set; }

		public Point DestinationPosition { get; set; }

		public WorldTransition(Transition transition, Vector2 position, ImageTexture icon, string destinationMap, Point destinationPosition)
		{
			this.Transition = transition;
			this.Position = position;
			this.Icon = icon;
			this.Icon.LoadContent();
			this.DestinationMap = destinationMap;
			this.DestinationPosition = destinationPosition;
		}

		public void Update(GameTime gameTime)
		{
			this.Icon.Update(gameTime);
		}
	}
}
