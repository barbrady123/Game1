using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1.Menus.Character
{
	public class StartCancelMenu : Menu
	{
		public StartCancelMenu (Point position): base(  position: position,
														layout: MenuLayout.Horizontal,
														background: null,
														inactiveMouseEvents: true,
														drawIfDisabled: true) { }

		protected override List<string> GetItemData() => new List<string> { "Start Game", "Cancel" };
	}
}
