using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1
{
	public class InstantEffect
	{
		public CharacterInstantEffect Effect { get; set; }

		public CharacterAttribute AffectedAttribute { get; set; }

		public string Text { get; set; }

		public string Description { get; set; }

		public int EffectRangeMin { get; set; }

		public int EffectRangeMax { get; set; }

		public InstantEffect(CharacterInstantEffect effect, CharacterAttribute affectedAttribute, string text, string description, int effectRangeMin, int effectRangeMax)
		{
			this.Effect = effect;
			this.AffectedAttribute = affectedAttribute;
			this.Text = text;
			this.Description = description;
			this.EffectRangeMin = effectRangeMin;
			this.EffectRangeMax = effectRangeMax;
		}
	}
}
