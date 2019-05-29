using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
	public static class Util
	{
		public static int Clamp(int input, int min, int max) => Math.Min(Math.Max(input, min), max);

		public static bool InRange(int input, int min, int max, bool inclusiveMin = true, bool inclusiveMax = true) => (input >= (inclusiveMin ? min : min + 1)) && (input <= (inclusiveMax ? max : max - 1));

		public static ImageTexture GenerateBorderTexture(int width, int height, int thickness, Color color)
		{
			var data = new Color[width * height];
			for (int w = 0; w < width; w++)
			for (int h = 0; h < height; h++)
				if ((h < thickness) || (h >= height - thickness) || (w < thickness) || (w >= width - thickness))
					data[w + (h * width)] = color;

			var t = new Texture2D(Game1.Graphics, width, height);
			t.SetData(data);
			return new ImageTexture(t, true);
		}
	}
}
