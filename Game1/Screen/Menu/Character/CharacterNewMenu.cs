﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Screen.Menu.Character
{
	public class CharacterNewMenu : MenuScreen
	{
		public CharacterNewMenu(Rectangle bounds): base(bounds,
														hasBackground: false,
														escapeToDisable: true,
														beyondBoundaryDisable: true) { }

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);
		}
	}
}
