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
	public class MapItem : MapMeta
	{
		[JsonProperty("quantity")]
		public int Quantity { get; set; }
	}
}
