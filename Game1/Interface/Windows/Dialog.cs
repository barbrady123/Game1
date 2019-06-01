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
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1.Interface.Windows
{
	// TODO: Support text wrap/multiline...
	public class Dialog : Window
	{
		private MenuScreen _buttonMenu;

		public DialogButton Buttons { get; set; }
		
		public Dialog(string text, DialogButton buttons, Rectangle bounds, int? duration) : base(bounds, "black", text, duration, true)
		{
			this.Buttons = buttons;
			this.IsActive = false;
			// TODO : Move this to a buttons component...
			switch (this.Buttons)
			{
				case DialogButton.Ok :	
					_buttonMenu = new OkMenu(new Rectangle(this.Bounds.X, this.Bounds.Y + (int)(this.Bounds.Height * 0.75f),  this.Bounds.Width, (int)(this.Bounds.Height * 0.25f))) { IsActive = true };
					_buttonMenu.OnItemSelect += _buttonMenu_OnItemSelect;
					// Window should handle the escape key now...
					break;
			}
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_buttonMenu?.LoadContent();
		}

		public override void UnloadContent()
		{
			_buttonMenu?.UnloadContent();
		}


		public override void UpdateReady(GameTime gameTime, bool processInput)
		{
			base.UpdateReady(gameTime, processInput);
			_buttonMenu?.Update(gameTime, this.IsActive);
		}

		public override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_buttonMenu?.Draw(spriteBatch);
		}

		private void _buttonMenu_OnItemSelect(object sender, EventArgs e)
		{
			ButtonClick(e as MenuEventArgs);
		}
	}
}
