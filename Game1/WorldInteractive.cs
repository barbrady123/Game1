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
	public class WorldInteractive
	{
		public ImageTexture Icon { get; set; }

		public Interactive Interactive { get; set; }

		public Vector2 Position { get; set; }

		public Rectangle Bounds { get; set; }

		public int? Health { get; set; }

		public WorldInteractive(Interactive interactive, ImageTexture icon, Vector2 position)
		{
			this.Interactive = interactive ?? throw new ArgumentNullException(nameof(interactive));
			this.Icon = icon ?? throw new ArgumentNullException(nameof(icon));
			this.Icon.LoadContent();
			this.Health = interactive.Health;
			this.Position = position;
			this.Bounds = position.ExpandToRectangleCentered(interactive.Size.Width / 2, interactive.Size.Height / 2);
		}

		public void Update(GameTime gameTime)
		{
			this.Icon.Update(gameTime);
		}
	}
}
