using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1
{
	// Eventually we'll probably need a buff "group" concept...for cases where buffs overwrite other buffs
	// i.e. A "minor defense potion" buff might get overwritten by a "major defense potion", not stack...	
	public class DebuffEffect : StatusEffect
	{
		public CharacterDebuffEffect Effect { get; set; }

		public DebuffEffect(CharacterDebuffEffect effect,
							CharacterAttribute affectedAttribute,
							string iconName,
							string text,
							string description,
							int effectValue,
							int? duration,
							int? period,
							int maxEffectStacks,
							int durationStack,
							int? maxDuration) : base(	affectedAttribute,
														iconName,
														text,
														description,
														effectValue,
														duration,
														period,
														maxEffectStacks,
														durationStack,
														maxDuration)
		{
			this.Effect = effect;
		}
	}
}
