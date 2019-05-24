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
			// TODO: these should be loaded from file/config...
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Back", true), LinkID = "back"});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting A", true), LinkID = "settinga"});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting B", true), LinkID = "settingb"});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting C", true), LinkID = "settingc"});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting D", true), LinkID = "settingd"});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Setting E", true), LinkID = "settinge"});
		}

		protected override void PerformAction(string id)
		{
			switch (id)
			{
				case "back" :		Back();		break;
				case "settinga" :
				case "settingb" :
				case "settingc" :
				case "settingd" :
				case "settinge" :	Setting();	break;
			}
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
