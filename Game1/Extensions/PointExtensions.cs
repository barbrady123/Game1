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
using Game1.Screens;
using Game1.Menus;

namespace Game1
{
	public static class PointExtensions
	{
		public static Point Offset(this Point point, int xOffset, int yOffset) => new Point(point.X + xOffset, point.Y + yOffset);

		public static Rectangle ExpandToRectangleTopLeft(this Point point, int expansionX, int expansionY)
		{
			return new Rectangle((int)point.X, (int)point.Y, expansionX, expansionY);
		}

		public static Point DivideBy(this Point point, int scale)
		{
			return new Point(point.X / scale, point.Y / scale);
		}
	}
}
