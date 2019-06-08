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
using TextInputEventArgs = Game1.Interface.TextInputEventArgs;

namespace Game1.Interface.Windows
{
	public class SplitWindow : Component
	{
		private InventoryItem _item;
		private OkCancelMenu _menu;
		private TextInput _input;
		private Button _halfButton;

		public event EventHandler<MenuEventArgs> OnButtonClick;

		public SplitWindow(Rectangle bounds, InventoryItem item) : base(bounds, true, "black")
		{
			_item = item;

			var bottomCenter = bounds.BottomCenterVector();
			// This arbitrary sizing sucks...TODO: Read the comment on the MenuScreen class...menus should be able to auto-size themselves given a Top-Left position...
			// Is it ssafe for all these subitems to get State.All ?
			_menu = new OkCancelMenu(new Rectangle((int)bottomCenter.X - 90, (int)bottomCenter.Y - 50, bounds.Width, 30)) { State = ComponentState.All };
			_menu.OnItemSelect += _menu_OnItemSelect;
			
			_input = new TextInput(50, new Vector2(bounds.X + (bounds.Width - 50) / 2, bounds.Y + 20), "", 2) { 
				AllowedCharacters = "0123456789"
			};
			_input.OnReadyDisable += _input_OnReadyDisable;
			_input.OnBeforeTextUpdate += _input_OnBeforeTextUpdate;

			_halfButton = new Button(bounds.CenteredRegion(80, 40), "Half") { IsActive = true };
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


		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_menu.Update(gameTime);
			_input.Update(gameTime);
			_halfButton.Update(gameTime);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
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
			OnButtonClick?.Invoke(this, (MenuEventArgs)e);
		}

		private void _input_OnReadyDisable(object sender, EventArgs e)
		{
			var args = (TextInputEventArgs)e;

			if (args.Key == Keys.Enter)
			{
				int value = Int32.Parse(_input.Text ?? "0");
				_input.Text = Util.Clamp(value, 1, _item.Quantity).ToString();
			}
		}

		private void _input_OnBeforeTextUpdate(object sender, EventArgs e)
		{
			var args = (TextInputEventArgs)e;
			int newValue = Int32.Parse(args.ResultText);
			if (newValue > _item.Quantity)
				args.Cancel = true;
		}
	}
}
