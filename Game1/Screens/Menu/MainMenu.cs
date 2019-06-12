using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Enum;

namespace Game1.Screens.Menu
{
	public class MainMenu : MenuScreen
	{
		public MainMenu(Rectangle bounds): base(bounds, escapeToDisable: true) { }

		protected override void ItemSelect(ComponentEventArgs e)
		{
			ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.MenuItemSelect, Value = e.Value });
		}
	}
}
