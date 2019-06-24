using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Items;

namespace Game1
{
	public class ItemHoldable : Item
	{
		// Should probably also have another icon specific to being held, etc...

		public int Speed { get; set; }

		public int Range { get; set; }
	}
}
