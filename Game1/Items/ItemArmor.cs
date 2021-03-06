﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1.Items
{
	public class ItemArmor : Item
	{
		[JsonProperty("slot")]
		public ArmorSlot Slot { get; set; }	

		[JsonProperty("defense")]
		public int Defense { get; set; }
	}
}
