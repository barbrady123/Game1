using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game1.Items
{
	public class ItemWeapon : ItemHoldable
	{
		[JsonProperty("mindamage")]
		public int MinDamage { get; set; }

		[JsonProperty("maxdamage")]
		public int MaxDamage { get; set; }
	}
}
