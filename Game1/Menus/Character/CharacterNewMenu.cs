using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Menus.Character
{
	public class CharacterNewMenu : Menu
	{
		public CharacterNewMenu(Point position): base(position, background: null, inactiveMouseEvents: true, drawIfDisabled: true) { }

		protected override List<string> GetItemData() => new List<string> { "Name", "Sex" };
	}
}
