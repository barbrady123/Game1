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
		public static Vector2 CenterVector(this Rectangle rect) => new Vector2(rect.Center.X, rect.Center.Y);

		public static Vector2 TopLeftVector(this Rectangle rect) => new Vector2(rect.X, rect.Y);
	}
}
