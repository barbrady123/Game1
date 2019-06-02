using Microsoft.Xna.Framework;

namespace Game1
{
	public static class Vector2Extensions
	{
		public static Rectangle ExpandToRectangleCentered(this Vector2 vector, int expansionX, int expansionY)
		{
			return new Rectangle((int)vector.X - expansionX, (int)vector.Y - expansionY, expansionX * 2, expansionY * 2);
		}

		public static Rectangle ExpandToRectangeTopLeft(this Vector2 vector, int expansionX, int expansionY)
		{
			return new Rectangle((int)vector.X, (int)vector.Y, expansionX, expansionY);
		}

		public static Vector2 Offset(this Vector2 vector, int offsetX, int offsetY) => new Vector2(vector.X + offsetX, vector.Y + offsetY);
	}
}
