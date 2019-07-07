using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1
{
	// Eventually we'll probably need a buff "group" concept...for cases where buffs overwrite other buffs
	// i.e. A "minor defense potion" buff might get overwritten by a "major defense potion", not stack...	
	public class DebuffEffect : StatusEffect
	{
		[JsonProperty("effect")]
		public CharacterDebuffEffect Effect { get; set; }
	}
}
