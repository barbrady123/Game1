using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;

namespace Game1.Screen.Menu
{
	public class OptionsMenu : MenuScreen
	{
		public OptionsMenu(Rectangle bounds): base(bounds) { }

		private void Back()
		{
			ReadyScreenUnload(this, new ScreenEventArgs("change", "MainMenu"));
		}

		private void Sex()
		{
			ReadyScreenUnload(this, new ScreenEventArgs("change", "SexMenu"));
		}

		private void Setting()
		{

		}
	}
}
