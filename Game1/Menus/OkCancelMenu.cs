using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1.Menus
{
	public class OkCancelMenu : Menu
	{
		public OkCancelMenu(Point position) : base(position, MenuLayout.Horizontal, null) { }

		protected override List<string> GetItemData() => new List<string> { "OK", "Cancel" };
	}
}
