using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Screens.Menu.Character
{
	public class CharacterNewMenu : MenuScreen
	{
		// shouldn't need escape to disable here either
		public CharacterNewMenu(Rectangle bounds): base(bounds, background: null, inactiveMouseEvents: true, drawIfDisabled: true) { }

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);
		}
	}
}
