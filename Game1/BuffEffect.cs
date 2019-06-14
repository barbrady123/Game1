using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1
{
	// Break this into two (everything can be in the base other than CharacterBuffEffect/CharacterDebuffEffect)...
	// So we can use either in the StatusViewer component...
	public class BuffEffect
	{
		public CharacterBuffEffect Effect { get; set; }

		public CharacterAttribute AffectedAttribute { get; set; }

		public ImageTexture Icon { get; set; }

		public string Text { get; set; }

		public string Description { get; set; }

		public int EffectValue { get; set; }

		public int? Duration { get; set; }

		public int? Period { get; set; }

		public int MaxEffectStacks { get; set; }

		public int DurationStack { get; set; }

		public int? MaxDuration { get; set; }

		public BuffEffect(CharacterBuffEffect effect, CharacterAttribute affectedAttribute, ImageTexture icon, string text, string description, int effectValue, int? duration, int? period, int maxEffectStacks, int durationStack, int? maxDuration)
		{
			this.Effect = effect;
			this.AffectedAttribute = affectedAttribute;
			this.Icon = icon;
			this.Text = text;
			this.EffectValue = effectValue;
			this.Description = description;
			this.Duration = duration;
			this.Period = period;
			this.MaxEffectStacks = maxEffectStacks;
			this.DurationStack = durationStack;
			this.MaxDuration = maxDuration;
		}
	}
}
