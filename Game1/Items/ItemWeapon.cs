using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
	public class ItemWeapon : Item
	{
		public int MinDamage { get; set; }

		public int MaxDamage { get; set; }

		public int Speed { get; set; }
	}
}
