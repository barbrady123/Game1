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
using Game1.Menus;

namespace Game1.Interface
{
	public class HotbarView : ItemContainerView
	{
		public HotbarView(ItemContainer container, Rectangle bounds, bool highlightActiveItem) : base(container, bounds, highlightActiveItem) { }

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			if (InputManager.KeyPressed(Keys.Right) || (InputManager.MouseScrollAmount < 0))
			{
				this.ActiveItemIndex = (this.ActiveItemIndex < this.Size - 1) ? this.ActiveItemIndex + 1 : 0;
			}
			if (InputManager.KeyPressed(Keys.Left)  || (InputManager.MouseScrollAmount > 0))
			{
				this.ActiveItemIndex = (this.ActiveItemIndex > 0 ) ? this.ActiveItemIndex - 1 : this.Size - 1;
			}
			foreach (var key in InputManager.GetPressedKeys())
			{
				char newChar = InputManager.KeyToChar(key, false);
				if (Int32.TryParse(newChar.ToString(), out int val))
					this.ActiveItemIndex = (val == 0 ? 10 : val) - 1;
			}
		}
	}
}
