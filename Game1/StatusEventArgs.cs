using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1
{
	public class CharacterStatusEventArgs : EventArgs
	{
		public CharacterAttribute AffectedAttribute { get; set; }

		public int EffectValue { get; set; }
	}
}
