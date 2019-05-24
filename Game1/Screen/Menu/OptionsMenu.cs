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

namespace Game1.Screen.Menu
{
	public class OptionsMenu : MenuScreen
	{
		public OptionsMenu(GraphicsDevice graphics, Vector2 size): base(graphics, size)
		{
			// TODO: Eventually we'll probably load all this metadata in (including the LinkActions) from config/file...
			// here I'm testing how to get from private method name to action delegate for later reference...

			// TODO: these should be loaded from file/config...
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Back", true), LinkAction=ActionFromMethodName("Back")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting A", true), LinkAction=ActionFromMethodName("Setting")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting B", true), LinkAction=ActionFromMethodName("Setting")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting C", true), LinkAction=ActionFromMethodName("Setting")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting D", true), LinkAction=ActionFromMethodName("Setting")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting E", true), LinkAction=ActionFromMethodName("Setting")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting F", true), LinkAction=ActionFromMethodName("Setting")});
		}

		private void Back()
		{
			ReadyScreenUnload(this, new ScreenEventArgs("change", "MainMenu"));
		}

		private void Setting()
		{

		}
	}
}
