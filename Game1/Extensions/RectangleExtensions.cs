using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
	public static class RectangleExtensions
	{
		public static Vector2 TopCenterVector (this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.Center.X + (xOffset ?? 0), rect.Y + (yOffset ?? 0));

		public static Vector2 CenterVector(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.Center.X + (xOffset ?? 0), rect.Center.Y + (yOffset ?? 0));

		public static Vector2 TopLeftVector(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.X + (xOffset ?? 0), rect.Y + (yOffset ?? 0));

		public static Vector2 TopRightVector(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.X + rect.Width -1 + (xOffset ?? 0), rect.Y + (yOffset ?? 0));

		public static Vector2 BottomRightVector(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.X + rect.Width -1 + (xOffset ?? 0), rect.Y + rect.Height - 1 + (yOffset ?? 0));

		public static Vector2 SizeVector(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.Width + (xOffset ?? 0), rect.Height + (yOffset ?? 0));

		public static Rectangle CenteredRegion(this Rectangle rect, int width, int height) => new Rectangle((rect.Width - width) / 2, (rect.Height - height) / 2, width, height);

		public static Rectangle Move(this Rectangle rect, int xMove, int yMove) => new Rectangle(rect.X + xMove, rect.Y + yMove, rect.Width, rect.Height);

		public static Point TopLeftPoint(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Point(rect.X + (xOffset ?? 0), rect.Y + (yOffset ?? 0));
	}
}
