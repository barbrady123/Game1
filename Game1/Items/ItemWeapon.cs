﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Items
{
	public class ItemWeapon : ItemHoldable
	{
		public int MinDamage { get; set; }

		public int MaxDamage { get; set; }
	}
}
