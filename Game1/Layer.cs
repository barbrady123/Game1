using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1
{
	public class Layer
	{
		[JsonProperty("type")]
		public LayerType Type { get; set; }

		[JsonProperty("tilesheet")]
		public string TileSheet { get; set; }

		[JsonProperty("tiledata")]
		public string[] RawTileData { get; set; }

		[JsonIgnore]
		public int[,] TileData { get; set; }

		public void GenerateTiles(int width, int height)
		{
			if (this.RawTileData == null)
				this.RawTileData = new string[0];

			this.TileData = new int[height, width];			

			for (int y = 0; y < height; y++)
			{
				string[] tiles = ((y < this.RawTileData.Length) && !String.IsNullOrWhiteSpace(this.RawTileData[y])) ? this.RawTileData[y].Split(';') : new string[0];
				for (int x = 0; x < width; x++)
					this.TileData[y, x] = (x < tiles.Length) ? Int32.Parse(tiles[x]) : -1;
			}
		}
	}
}
