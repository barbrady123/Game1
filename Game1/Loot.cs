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
	// This may need to be completely redone once we start using it for mobs...
	// Need a way to hook quest items into here eventually...
	public class Loot
	{
		public int Odds { get; set; }

		public List<int> ItemPool { get; set; }

		public int MinQuantity { get; set; }

		public int MaxQuantity { get; set; }

		public List<Loot> SubLoot { get; set; }
	}
}
