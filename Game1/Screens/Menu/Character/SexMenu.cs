using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1.Screens.Menu.Character
{
	public class SexMenu : MenuScreen
	{
		// this is probably the only place the "escapeToDisable" is valid on this page...(and the text input)
		public SexMenu(Rectangle bounds): base(	bounds: bounds,
												layout: MenuLayout.Horizontal,
												background: null,
												escapeToDisable: true) { }
	}
}
