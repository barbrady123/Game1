using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1
{
	// This could be StatusEffect<TCharacterEffect> where TCharacterEffect: System.Enum
	public abstract class StatusEffect
	{
		public CharacterAttribute AffectedAttribute { get; set; }

		public string IconName { get; set; }

		public string Text { get; set; }

		public string Description { get; set; }

		public int EffectValue { get; set; }

		public int? Duration { get; set; }

		public int? Period { get; set; }

		public int MaxEffectStacks { get; set; }

		public int DurationStack { get; set; }

		public int? MaxDuration { get; set; }

		public StatusEffect(CharacterAttribute affectedAttribute,
							string iconName,
							string text,
							string description,
							int effectValue,
							int? duration,
							int? period,
							int maxEffectStacks,
							int durationStack,
							int? maxDuration)
		{
			this.AffectedAttribute = affectedAttribute;
			this.IconName = iconName;
			this.Text = text;
			this.Description = description;
			this.EffectValue = effectValue;
			this.Duration = duration;
			this.Period = period;
			this.MaxEffectStacks = maxEffectStacks;
			this.DurationStack = durationStack;
			this.MaxDuration = maxDuration;
		}
	}
}
