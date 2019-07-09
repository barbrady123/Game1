using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1
{
	// This could be StatusEffect<TCharacterEffect> where TCharacterEffect: System.Enum
	public abstract class StatusEffect
	{
		// Seems like we actually don't need this for anything...
		[JsonProperty("id")]
		public string Id {  get; set; }

		[JsonProperty("attribute")]
		public CharacterAttribute AffectedAttribute { get; set; }

		[JsonProperty("iconname")]
		public string IconName { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("effectvalue")]
		public int EffectValue { get; set; }

		[JsonProperty("duration")]
		public int? Duration { get; set; }

		[JsonProperty("period")]
		public int? Period { get; set; }

		[JsonProperty("maxeffectstacks")]
		public int MaxEffectStacks { get; set; }

		[JsonProperty("durationstack")]
		public int DurationStack { get; set; }

		[JsonProperty("maxduration")]
		public int? MaxDuration { get; set; }
	}
}
