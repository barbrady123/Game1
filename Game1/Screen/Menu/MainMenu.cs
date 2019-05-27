using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Screen.Menu
{
	public class MainMenu : MenuScreen
	{
		public MainMenu(Rectangle bounds): base(bounds) { }

		public event EventHandler OnStartNewGameSelect;
		public event EventHandler OnOptionsSelect;
		public event EventHandler OnExitGameSelect;

		private void StartNewGame()
		{
			OnStartNewGameSelect?.Invoke(this, null);
			ReadyScreenUnload(this, new ScreenEventArgs("change", "CharacterCreate"));
		}

		private void Options()
		{
			OnOptionsSelect?.Invoke(this, null);
			ReadyScreenUnload(this, new ScreenEventArgs("change", "OptionsMenu"));
		}

		private void ExitGame()
		{
			OnExitGameSelect?.Invoke(this, null);
			ReadyScreenUnload(this, new ScreenEventArgs("exit", null));
		}
	}
}
