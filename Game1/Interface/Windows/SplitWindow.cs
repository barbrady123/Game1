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
	public class SplitWindow : Component
	{
		private const int TextInputWidth = 50;

		private InventoryItemView _owner;
		private OkCancelMenu _menu;
		private TextInput _input;
		private Button _halfButton;

		public InventoryItemView Owner
		{
			get { return _owner; }
			set
			{
				if (_owner != value)
				{
					_owner = value;

					if (_owner != null)
						Show();
					else
						Hide();
				}
			}
		}

		public int Quantity => Int32.TryParse(_input?.Text, out int quantity) ? quantity : 0;

		public event EventHandler<ComponentEventArgs> OnButtonClick;

		public SplitWindow(SpriteBatchData spriteBatchData) : base(Rectangle.Empty, true, background: "black", spriteBatchData: spriteBatchData, drawIfDisabled: false)
		{
			// We allow empty instanciation so the object can be registered with a ComponentManager if necessary...
		}

		// This will WAY simplier after Positiongeddon....
		private void Show()
		{
			UnloadContent();
			var position = InputManager.MousePosition.Offset(-10, -10);
			this.Bounds = new Rectangle(position.X, position.Y, 160, 200);

			var bottomCenter = this.Bounds.BottomCenterVector();

			_menu = new OkCancelMenu(new Rectangle((int)bottomCenter.X - 80, (int)bottomCenter.Y - 50, this.Bounds.Width, 30)) { IsActive = true };
			_menu.OnItemSelect += _menu_OnItemSelect;
			
			_input = new TextInput(SplitWindow.TextInputWidth, new Vector2(this.Bounds.Center.X, this.Bounds.Y + this.ContentMargin.Height + (TextInput.Height / 2)), "", 2) {
				IsActive = true,
				AllowedCharacters = "0123456789"
			};
			_input.OnReadyDisable += _input_OnReadyDisable;
			_input.OnBeforeTextUpdate += _input_OnBeforeTextUpdate;

			_halfButton = new Button(this.Bounds.CenteredRegion(80, 40), "Half") { IsActive = true };
			_halfButton.OnMouseLeftClick += _halfButton_OnMouseLeftClick;

			LoadContent();
			this.IsActive = true;
		}

		private void Hide()
		{
			this.IsActive = false;
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

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_menu?.Draw(spriteBatch);
			_input?.Draw(spriteBatch);
			_halfButton?.Draw(spriteBatch);
		}

		private void _halfButton_OnMouseLeftClick(object sender, EventArgs e)
		{
			if (this.Owner?.Item == null)
				return;

			_input.Text = (this.Owner.Item.Quantity / 2).ToString();
		}

		private void _menu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			OnButtonClick?.Invoke(this, e);
			ReadyDisable(e);
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

			int newValue = Int32.Parse(e.Text);
			if (newValue > this.Owner.Item.Quantity)
				e.Cancel = true;
		}

		protected override void ReadyDisable(ComponentEventArgs e)
		{
			this.Owner = null;
			base.ReadyDisable(e);
		}
	}
}
