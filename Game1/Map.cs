using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Game1
{
	public class Map
	{
		[JsonProperty("mapsizex")]
		public int Width { get; set; }
		[JsonProperty("mapsizey")]
		public int Height { get; set; }
		[JsonProperty("tilesheet")]
		public string TileSheet { get; set; }	// Probably each layer should have it's own tilesheet....
		[JsonProperty("layers")]
		public Layer[] Layers { get; set; }

		public Layer LayerById(string id) => this.Layers?.FirstOrDefault(l => l.Id == id);

		public void GenerateTiles()
		{
			foreach (var layer in this.Layers)
				layer.GenerateTiles();
		}

		public void Update(GameTime gameTime)
		{
		}
	}
}
