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

namespace Game1.Interface.Windows
{
	public class SplitWindow : Window
	{
		private InventoryItem _item;
		private OkCancelMenu _menu;
		private TextInput _input;
		private Button _halfButton;

		public SplitWindow(Rectangle bounds, InventoryItem item) : base(bounds, "black", null, null, true)
		{
			_item = item;

			this.IsActive = false;
			var bottomCenter = bounds.BottomCenterVector();
			// This arbitrary sizing sucks...TODO: Read the comment on the MenuScreen class...menus should be able to auto-size themselves given a Top-Left position...
			_menu = new OkCancelMenu(new Rectangle((int)bottomCenter.X, (int)bottomCenter.Y - 50, bounds.Width, 30)) { IsActive = true };
			_menu.OnItemSelect += _menu_OnItemSelect;
			
			_input = new TextInput(50, "1", 2, true) { Position = new Vector2((bounds.Width - 50) / 2, bounds.Y + 10) };
			_input.OnReadyDisable += _input_OnReadyDisable;

			_halfButton = new Button(bounds.CenteredRegion(80, 40), "Half");
			_halfButton.OnClick += _halfButton_OnClick;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_menu.LoadContent();
			_input.LoadContent();
			_halfButton.LoadContent();
		}

		public override void UnloadContent()
		{
			_menu.UnloadContent();
			_input.UnloadContent();
			_halfButton.UnloadContent();
		}


		public override void UpdateReady(GameTime gameTime, bool processInput)
		{
			base.UpdateReady(gameTime, processInput);
			_menu.Update(gameTime, this.IsActive);
			_input.Update(gameTime, this.IsActive);
			_halfButton.Update(gameTime);
		}

		public override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_menu.Draw(spriteBatch);
			_input.Draw(spriteBatch);
			_halfButton.Draw(spriteBatch);
		}

		private void _halfButton_OnClick(object sender, EventArgs e)
		{
			_input.Text = (_item.Quantity / 2).ToString();
		}

		private void _menu_OnItemSelect(object sender, EventArgs e)
		{

		}

		private void _input_OnReadyDisable(object sender, EventArgs e)
		{

		}
	}
}
