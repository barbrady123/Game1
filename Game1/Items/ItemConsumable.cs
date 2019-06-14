using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1.Items
{
	public class ItemConsumable : Item
	{
		public ConsumableType Type { get; set; }

		public CharacterInstantEffect? InstantEffect { get; set; }

		public CharacterBuffEffect? BuffEffect { get; set; }

		public int EffectRangeMin { get; set; }

		public int EffectRangeMax { get; set; }

		public int? Duration { get; set; }
	}
}
