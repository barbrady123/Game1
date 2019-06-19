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

namespace Game1.Menus
{
	public class OptionsMenu : Menu
	{
		public OptionsMenu(Point position): base(position, background: null, escapeToDisable: true) { }

		protected override List<string> GetItemData() => new List<string> { "Back", "Setting A", "Setting B", "Setting C", "Setting D", "Setting E", "Setting F", "Setting G", "Setting H" };
	}
}
