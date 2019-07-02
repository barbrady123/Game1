using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	public class Interactive
	{
		public string Id { get; set; }

		public string DisplayText { get; set; }

		public string IconName { get; set; }	// Overworld icon

		public int? Health { get; set; }	// Null == unbreakable...?

		public Dictionary<ToolType, float> Effectiveness { get; set; }

		public List<Loot> LootTable { get; set; }

		public bool IsSolid { get; set; }

		public Size Size { get; set; }
	}
}
