using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Enum;

namespace Game1.Items
{
	public class Item
	{
		public string IconName { get; set; }

		public int Id { get; set; }

		public string DisplayName { get; set; }

		public int MaxStackSize { get; set; }

		public float Weight { get; set; }
	}
}
