using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1.Items
{
	public class ItemTool: ItemHoldable
	{
		public ToolType Type { get; set; }

		public int Damage { get; set; }
	}
}
