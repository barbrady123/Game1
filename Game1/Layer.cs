using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game1
{
	/// <summary>
	///  "Solid" concept should probably be per layer...makes calcs in the physics engine easier also...
	/// </summary>
	public class Layer
	{
		[JsonProperty("active")]
		public bool IsActive { get; set; }

		[JsonProperty("solid")]
		public bool IsSolid { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("index")]
		public int Index { get; set; }

		[JsonIgnore]
		public Tile[,] TileData { get; set; }

		[JsonProperty("tiledata")]
		public string[] RawTileData { get; set; }

		public void GenerateTiles()
		{
			if (!(this.RawTileData?.Any() ?? false))
				return;

			int rowSize = this.RawTileData.First().Split(';').Count();

			this.TileData = new Tile[this.RawTileData.GetLength(0), rowSize];

			for (int y = 0; y < this.RawTileData.Length; y++)
			{
				string[] tiles = this.RawTileData[y].Split(';');
				for (int x = 0; x < tiles.Length; x++)
					this.TileData[y,x] = new Tile { TileIndex = Int32.Parse(tiles[x]) };	// Again coords swapped...
			}
		}
	}
}
