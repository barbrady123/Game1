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
	public class WorldSolid : IWorldEntity
	{
		public Vector2 Position { get; set; }

		public Rectangle Bounds { get; set; }

		public bool IsSolid => true;

		public WorldSolid(Vector2 position)
		{
			this.Position = position;
			this.Bounds = position.ExpandToRectangleTopLeft(Game1.TileSize, Game1.TileSize);
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			// Currently, static layers are pre-rendered and handled in GamePlayCamera...
			// However we may eventually make these actual entities so we can animate them, etc...
		}
	}
}
