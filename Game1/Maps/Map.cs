using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1.Maps
{
	public class Map
	{
		[JsonProperty("lastactive")]
		public int LastActive { get; set; }

		[JsonProperty("tilesize")]
		public int TileSize { get; set; }

		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("layers")]
		public Layer[] Layers { get; set; }
		 
		[JsonProperty("items")]
		public MapItem[] Items { get; set; }

		[JsonProperty("interactives")]
		public MapMeta[] Interactives { get; set; }

		[JsonProperty("npcs")]
		public MapMeta[] NPCs { get; set; }

		[JsonProperty("mobs")]
		public MapMob[] Mobs { get; set; }

		[JsonProperty("transitions")]
		public MapTransition[] Transitions { get; set; }

		[JsonIgnore]
		public Size Size => new Size(this.Width * this.TileSize, this.Height * this.TileSize);

		[JsonIgnore]
		public IEnumerable<Layer> StaticLayers
		{
			get
			{
				foreach (var layer in this.Layers.Where(l => l.Type != LayerType.Breakable))
					yield return layer;
			}
		}

		public void Initialize()
		{
			foreach (var layer in this.Layers)
				layer.GenerateTiles(this.Width, this.Height);
		}
	}
}
