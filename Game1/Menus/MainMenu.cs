using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Enum;

namespace Game1.Menus
{
	public class MainMenu : Menu
	{
		public MainMenu(Point position): base(position,background: null, escapeToDisable: true) { }

		protected override List<string> GetItemData() => new List<string> { "Start New Game", "Load Game", "Options", "Exit Game" };
	}
}
