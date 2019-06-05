using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1.Items
{
	public class ItemArmor : Item
	{
		public ArmorSlot Slot { get; set; }	

		public int Defense { get; set; }
	}
}
