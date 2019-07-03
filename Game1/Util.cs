using System;
using System.Linq;
using System.Linq.Expressions;
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

			var t = new Texture2D(Game1.Graphics, width, height) { Name = AssetManager.UntrackedAssetName };
			t.SetData(data);
			return new ImageTexture(t, ImageAlignment.Centered, isActive);
		}

		// Hmm, wouldn't it be faster to create a render target, clear it to a color, then save it/cast to texture??
		public static ImageTexture GenerateSolidBackground(int width, int height, Color color)
		{
			if (width > 0)
			{
				int size = width * height;
				var data  = new Color[size];
				for (int x = 0; x < size; x++)
					data[x] = color;

				var t = new Texture2D(Game1.Graphics, width, height) { Name = AssetManager.UntrackedAssetName };
				t.SetData(data);
				return new ImageTexture(t, null, true);
			}
			else
			{
				return new ImageTexture(new Texture2D(Game1.Graphics, 1, 1), null, true);
			}
		}

		public static SpriteBatch WrappedDraw(Action<SpriteBatch> func, string batchName, Rectangle bounds)
		{
			var batchData = SpriteBatchManager.Get(batchName);
			batchData.ScissorWindow = bounds;
			func.Invoke(batchData.SpriteBatch);
			return batchData.SpriteBatch;
		}

		public static void WrappedDraw(Action<SpriteBatch> func, SpriteBatchData spriteBatchData, Rectangle bounds)
		{
			spriteBatchData.ScissorWindow = bounds;
			func.Invoke(spriteBatchData.SpriteBatch);
		}

		public static Cardinal DirectionFromVector(Vector2 vector, Cardinal defaultDir)
		{
			// Prioritize Left/Right images (they just look better)...
			if (vector.X > 0.0f)
				return Cardinal.East;
			if (vector.X < 0.0f)
				return Cardinal.West;
			if (vector.Y > 0.0f)
				return Cardinal.South;
			if (vector.Y < 0.0f)
				return Cardinal.North;
			return defaultDir;
		}

		/// <summary>
		/// Returns vector scale to increase start rectangle to end rectangle.  Will
		/// return 1.0f minimum for dimensions where start >= end.
		/// </summary>
		public static Vector2 ScaleUpVector(Rectangle start, Rectangle end) => new Vector2(Math.Max((float)end.Width / (float)start.Width, 1.0f), Math.Max((float)end.Height / (float)start.Height, 1.0f));
	}
}
