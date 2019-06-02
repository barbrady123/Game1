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
using Game1.Screens.Menu;

namespace Game1
{
	public static class PointExtensions
	{
		public static Point Offset(this Point point, int xOffset, int yOffset) => new Point(point.X + xOffset, point.Y + yOffset);
	}
}
