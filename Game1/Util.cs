﻿using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Enum;

namespace Game1
{
	public static class Util
	{
		public static readonly Point PointInvalid = new Point(-1, -1);

		public static BindingFlags GetPropertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

		public static int Clamp(int input, int min, int max) => Math.Min(Math.Max(input, min), max);

		public static bool InRange(int input, int min, int max, bool inclusiveMin = true, bool inclusiveMax = true) => (input >= (inclusiveMin ? min : min + 1)) && (input <= (inclusiveMax ? max : max - 1));

		public static ImageTexture GenerateBorderTexture(int width, int height, int thickness, Color color, bool isActive = true)
		{
			var data = new Color[width * height];
			for (int w = 0; w < width; w++)
			for (int h = 0; h < height; h++)
				if ((h < thickness) || (h >= height - thickness) || (w < thickness) || (w >= width - thickness))
					data[w + (h * width)] = color;

			var t = new Texture2D(Game1.Graphics, width, height);
			t.SetData(data);
			return new ImageTexture(t, isActive);
		}

		public static ImageTexture GenerateSolidBackground(int width, int height, Color color)
		{
			if (width > 0)
			{
				int size = width * height;
				var data  = new Color[size];
				for (int x = 0; x < size; x++)
					data[x] = color;

				var t = new Texture2D(Game1.Graphics, width, height);
				t.SetData(data);
				return new ImageTexture(t, true);
			}
			else
			{
				return new ImageTexture(new Texture2D(Game1.Graphics, 1, 1));
			}
		}

		public static SpriteBatch WrappedDraw(Action<SpriteBatch> func, string batchName, Rectangle bounds)
		{
			var batchData = SpriteBatchManager.Get(batchName);
			batchData.ScissorWindow = bounds;
			func.Invoke(batchData.SpriteBatch);
			return batchData.SpriteBatch;
		}
	}
}
