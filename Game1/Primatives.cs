using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Game1
{
	/// <summary>
	/// Primitive that better represets size...
	/// </summary>
	public class Size
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public Size() { }

		public Size(int squareSideLength)
		{
			this.Width = squareSideLength;
			this.Height = squareSideLength;
		}

		public Size(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}
	}
}
