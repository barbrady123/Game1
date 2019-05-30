using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1.Items
{
	public class Item
	{
		// This is another place where we may load images seperate and change this to a pointer so we can share images...
		private ImageTexture Icon { get; set; }

		public int Id { get; set; }

		public string DisplayName { get; set; }

		// Do we need a "does not stack" indicator and/or amount?  Or is "1" MaxStackSize sufficient for "doesn't stack"...??
		public int MaxStackSize { get; set; }

		public float Weight { get; set; }
	}
}
