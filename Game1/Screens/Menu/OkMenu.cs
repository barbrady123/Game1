﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Screens.Menu;

namespace Game1.Screens.Menu
{
	public class OkMenu : MenuScreen
	{
		public OkMenu(Rectangle bounds) : base(bounds, background: null) { }

		protected override void LoadItemData()
		{
			_items = new List<MenuItem> { new MenuItem() { Id = "ok", Text = "OK" } };
		}
	}
}
