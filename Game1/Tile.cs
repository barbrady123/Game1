using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game1
{
	public class Tile
	{
		[JsonProperty("tile")]
		public int TileIndex { get; set; }

		[JsonProperty("solid")]
		public bool IsSolid { get; set; }
	}
}
