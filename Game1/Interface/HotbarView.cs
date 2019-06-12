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
using Game1.Items;
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1.Interface
{
	public class HotbarView : ItemContainerView
	{
		public HotbarView(ItemContainer container, Rectangle bounds, bool highlightActiveItem) : base(container, bounds, highlightActiveItem) { }

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			var hotbar = this.Container;
			int hotbarIndex = hotbar.ActiveItemIndex;
			if (InputManager.KeyPressed(Keys.Right) || (InputManager.MouseScrollAmount < 0))
			{
				hotbar.ActiveItemIndex = (hotbarIndex < hotbar.Size - 1) ? hotbarIndex + 1 : 0;
			}
			if (InputManager.KeyPressed(Keys.Left)  || (InputManager.MouseScrollAmount > 0))
			{
				hotbar.ActiveItemIndex = (hotbarIndex > 0 ) ? hotbarIndex - 1 : hotbar.Size - 1;
			}
			foreach (var key in InputManager.GetPressedKeys())
			{
				char newChar = InputManager.KeyToChar(key, false);
				if (Int32.TryParse(newChar.ToString(), out int val))
					hotbar.ActiveItemIndex = (val == 0 ? 10 : val) - 1;
			}

		}
	}
}
