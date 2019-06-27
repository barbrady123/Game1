using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1.Items
{
	public class ItemConsumable : Item
	{
		[JsonProperty("type")]
		public ConsumableType Type { get; set; }

		[JsonProperty("instanteffect")]
		public CharacterInstantEffect? InstantEffect { get; set; }

		[JsonProperty("buffeffect")]
		public CharacterBuffEffect? BuffEffect { get; set; }

		[JsonProperty("debuffeffect")]
		public CharacterDebuffEffect? DebuffEffect { get; set; }

		[JsonProperty("effectrangemin")]
		public int EffectRangeMin { get; set; }

		[JsonProperty("effectrangemax")]
		public int EffectRangeMax { get; set; }
	}
}
