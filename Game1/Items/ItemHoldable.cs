using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game1.Items
{
	public class ItemHoldable : Item
	{
		// Should probably also have another icon specific to being held, etc...
		[JsonProperty("speed")]
		public int Speed { get; set; }

		[JsonProperty("range")]
		public int Range { get; set; }
	}
}
