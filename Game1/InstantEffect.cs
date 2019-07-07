using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1
{
	public class InstantEffect
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("effect")]
		public CharacterInstantEffect Effect { get; set; }

		[JsonProperty("attribute")]
		public CharacterAttribute AffectedAttribute { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("effectmin")]
		public int EffectRangeMin { get; set; }

		[JsonProperty("effectmax")]
		public int EffectRangeMax { get; set; }
	}
}
