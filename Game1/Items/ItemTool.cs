using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1.Items
{
	public class ItemTool: ItemHoldable
	{
		[JsonProperty("type")]
		public ToolType Type { get; set; }

		[JsonProperty("damage")]
		public int Damage { get; set; }
	}
}
