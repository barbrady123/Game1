using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Screen.Menu
{
	public class MainMenu : MenuScreen
	{
		public MainMenu(Rectangle bounds): base(bounds, escapeToDisable: true) { }
	}
}
