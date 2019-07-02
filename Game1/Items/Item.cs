using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1.Items
{
	public class Item
	{
		[JsonProperty("iconname")]
		public string IconName { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("displayname")]
		public string DisplayName { get; set; }

		[JsonProperty("maxstack")]
		public int MaxStackSize { get; set; }

		[JsonProperty("weight")]
		public float Weight { get; set; }
	}
}
