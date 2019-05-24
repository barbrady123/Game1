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
	public class MainMenu : MenuScreen
	{
		public MainMenu(GraphicsDevice graphics, Vector2 size): base(graphics, size)
		{
			// TODO: these should be loaded from file/config...
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Start New Game", true), LinkAction=ActionFromMethodName("StartNewGame")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Options", true), LinkAction=ActionFromMethodName("Options")});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Exit Game", true), LinkAction=ActionFromMethodName("ExitGame")});
		}

		private void StartNewGame()
		{
			
		}

		private void Options()
		{
			ReadyScreenUnload(this, new ScreenEventArgs("change", "OptionsMenu"));
		}

		private void ExitGame()
		{
			Game1.Instance.Exit();			
		}
	}
}
