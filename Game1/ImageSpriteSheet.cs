using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game1.Effect;
using Game1.Enum;

namespace Game1
{
	public class ImageSpriteSheet : ImageTexture
	{
		public ImageSpriteSheet(Texture2D texture, bool isActive = false) : base(texture, isActive)
		{
			Alignment = ImageAlignment.Centered;
		}

		public ImageSpriteSheet(Texture2D texture, Texture2D highlight, bool isActive = false) : base(texture, highlight, isActive)
		{
			Alignment = ImageAlignment.Centered;
		}

		public override void LoadContent()
		{
			base.LoadContent();
		}

		protected override void UpdatePosition() { }

		public void UpdateDirection(Cardinal direction)
		{
			this.SourceRect = new Rectangle(this.SourceRect.X, (int)direction * Game1.TileSize, Game1.TileSize, Game1.TileSize);
		}
	}
}
