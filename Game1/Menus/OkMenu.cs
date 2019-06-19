using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.Menus
{
	public class OkMenu : Menu
	{
		public OkMenu(Point position) : base(position, background: null) { }

		protected override List<string> GetItemData() =>new List<string> { "OK" };
	}
}
