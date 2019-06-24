using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	public class CharacterEventArgs : EventArgs
	{
		public InventoryItem Item { get; set; }

		public Cardinal Direction { get; set; }
	}
}
