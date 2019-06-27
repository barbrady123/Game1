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
	public class MapMeta
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("position")]
		public Point Position { get; set; }
	}
}
