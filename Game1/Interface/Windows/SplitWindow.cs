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
		private const int TextInputWidth = 50;

		private OkCancelMenu _menu;
		private TextInput _input;
		private Button _halfButton;

		public int Quantity => Int32.TryParse(_input?.Text, out int quantity) ? quantity : 0;

		public event EventHandler<ComponentEventArgs> OnButtonClick;

		public InventoryItemView Owner { get; private set; }

		public SplitWindow() : base(Rectangle.Empty, background: "black")
		{
			// We allow empty instanciation so the object can be registered with a ComponentManager if necessary...
		}

		public void Initialize(InventoryItemView owner, Rectangle bounds)
		{
			UnloadContent();
			this.Owner = owner;
			this.Bounds = bounds;

			var bottomCenter = bounds.BottomCenterVector();
			// This arbitrary sizing sucks...TODO: Read the comment on the MenuScreen class...menus should be able to auto-size themselves given a Top-Left position...
			// Is it ssafe for all these subitems to get State.All ?
			_menu = new OkCancelMenu(new Rectangle((int)bottomCenter.X - 90, (int)bottomCenter.Y - 50, bounds.Width, 30)) { State = ComponentState.All };
			_menu.OnItemSelect += _menu_OnItemSelect;
			
			_input = new TextInput(SplitWindow.TextInputWidth, new Vector2(bounds.Center.X, bounds.Y + this.ContentMargin.Height + (TextInput.Height / 2)), "", 2) {
				State = ComponentState.All,
				AllowedCharacters = "0123456789"
			};
			_input.OnReadyDisable += _input_OnReadyDisable;
			_input.OnBeforeTextUpdate += _input_OnBeforeTextUpdate;

			_halfButton = new Button(bounds.CenteredRegion(80, 40), "Half") { State = ComponentState.All };
			_halfButton.OnClick += _halfButton_OnClick;

			LoadContent();
		}

		public void Clear()
		{
			this.Owner = null;
			this.Bounds = Rectangle.Empty;
			UnloadContent();
			_background = null;
			_border = null;
			_menu?.UnloadContent();
			_input?.UnloadContent();
			_halfButton?.UnloadContent();
			_mouseover = false;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_menu?.LoadContent();
			_input?.LoadContent();
			_halfButton?.LoadContent();
		}

		public override void UnloadContent()
		{
			_menu?.UnloadContent();
			_input?.UnloadContent();
			_halfButton?.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_menu.Update(gameTime);
			_input.Update(gameTime);
			_halfButton.Update(gameTime);
			InputManager.BlockAllInput();
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_menu?.Draw(spriteBatch);
			_input?.Draw(spriteBatch);
			_halfButton?.Draw(spriteBatch);
		}

		private void _halfButton_OnClick(object sender, EventArgs e)
		{
			if (this.Owner?.Item == null)
				return;

			_input.Text = (this.Owner.Item.Quantity / 2).ToString();
		}

		private void _menu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			OnButtonClick?.Invoke(this, e);
		}

		private void _input_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			if (this.Owner?.Item == null)
				return;

			if (e.Key == Keys.Enter)
			{
				int value = Int32.Parse(_input.Text ?? "0");
				_input.Text = Util.Clamp(value, 1, this.Owner.Item.Quantity).ToString();
				_menu.SelectItem(0);
			}
		}

		private void _input_OnBeforeTextUpdate(object sender, ComponentEventArgs e)
		{
			if (this.Owner?.Item == null)
				return;

			int newValue = Int32.Parse(e.ResultText);
			if (newValue > this.Owner.Item.Quantity)
				e.Cancel = true;
		}
	}
}
