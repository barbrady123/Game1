using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1.Screen.Menu.Character
{
	public class SexMenu : MenuScreen
	{
		public SexMenu(Rectangle bounds): base(	bounds: bounds,
												layout: MenuLayout.Horizontal,
												hasBackground: false,
												escapeToDisable: true) { }
	}
}
