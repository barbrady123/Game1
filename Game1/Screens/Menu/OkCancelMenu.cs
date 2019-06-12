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
using Game1.Screens.Menu;

namespace Game1.Screens.Menu
{
	public class OkCancelMenu : MenuScreen
	{
		public OkCancelMenu(Rectangle bounds) : base(bounds, MenuLayout.Horizontal, null, null, true) { }

		protected override List<MenuItem> GetItemData()
		{
			return new List<MenuItem> {
				new MenuItem() { Id = "ok", Text = "OK" },
				new MenuItem() { Id = "cancel", Text = "Cancel" }
			};
		}
	}
}
