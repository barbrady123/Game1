using Microsoft.Xna.Framework;

namespace Game1
{
	public static class Vector2Extensions
	{
		public static Rectangle ExpandToRectangle(this Vector2 vector, int expansionX, int expansionY)
		{
			return new Rectangle((int)vector.X - expansionX, (int)vector.Y - expansionY, expansionX * 2, expansionY * 2);
		}
	}
}
