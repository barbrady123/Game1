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
		public static Vector2 CenterVector(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.Center.X + (xOffset ?? 0), rect.Center.Y + (yOffset ?? 0));

		public static Vector2 TopLeftVector(this Rectangle rect, int? xOffset = null, int? yOffset = null) => new Vector2(rect.X + (xOffset ?? 0), rect.Y + (yOffset ?? 0));

		public static Vector2 SizeVector(this Rectangle rect) => new Vector2(rect.Width, rect.Height);
	}
}
