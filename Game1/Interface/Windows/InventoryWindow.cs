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
	public class InventoryWindow : Window
	{
		private Character _character;
		private ItemContainerView _containerViewBackpack;
		private ItemContainerView _containerViewHotbar;
		private ItemContainer _containerBackpack;
		private ItemContainer _containerHotbar;
		private InventoryContextMenu _contextMenu;
		private Tooltip _tooltip;
		private SplitWindow _splitWindow;

		public InventoryWindow(	string text,
								Rectangle bounds, 
								Character character) : base(bounds, "brick", text, null, true, true)
		{
			_character = character;
			_containerBackpack = character.Backpack;
			var viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);

			_containerViewBackpack = ItemContainerView.New<ItemContainerView>(_containerBackpack, viewPosition, false);
			_containerViewBackpack.OnMouseClick += _containerView_OnMouseClick;
			_containerViewBackpack.OnMouseOver += _containerView_OnMouseOver;
			_containerViewBackpack.OnMouseOut += _containerView_OnMouseOut;

			_containerHotbar = character.HotBar;
			_containerViewHotbar = ItemContainerView.New<ItemContainerView>(_containerHotbar, new Point(viewPosition.X, viewPosition.Y + _containerViewBackpack.Bounds.Size.Y + this.ContentMargin.Height), false);
			_containerViewHotbar.OnMouseClick += _containerView_OnMouseClick;

			_tooltip = new Tooltip();
			_contextMenu = null;
			_splitWindow = null;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_containerViewBackpack.LoadContent();
			_containerViewHotbar.LoadContent();
			_tooltip.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_containerViewBackpack.UnloadContent();
			_containerViewHotbar.UnloadContent();
			_tooltip.UnloadContent();
			DisableContextMenu();
			DisableSplitWindow();
		}

		public override void UpdateReady(GameTime gameTime, bool processInput)
		{
			base.UpdateReady(gameTime, processInput);
			_splitWindow?.Update(gameTime, processInput);
			_contextMenu?.Update(gameTime, processInput);	// Check first since it's "on top" and might need to kill a click-through if it's over another clickable item...
			_containerViewBackpack.Update(gameTime);
			_containerViewHotbar.Update(gameTime);
			_tooltip.Update(gameTime, processInput);
		}

		public override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_containerViewBackpack.Draw(spriteBatch);
			_containerViewHotbar.Draw(spriteBatch);
			_tooltip.Draw(spriteBatch);
			var batchData = SpriteBatchManager.Get("context");
			if (_contextMenu != null)
			{
				batchData.ScissorWindow = _contextMenu.Bounds;
				_contextMenu.Draw(batchData.SpriteBatch);
			}
			if (_splitWindow != null)
			{
				// Why the fuck is this changing the scissorwindow for the modal batch????
				batchData.ScissorWindow = _splitWindow.Bounds;
				_splitWindow.Draw(batchData.SpriteBatch);
			}
		}

		protected override void BeforeReadyDisable(ScreenEventArgs args)
		{
			if (_character.HeldItem != null)
			{
				_character.PutItem(_containerViewBackpack.Container);
				_character.HeldItem = null;
			}

			base.BeforeReadyDisable(args);
		}

		private void _containerView_OnMouseClick(object sender, EventArgs e)
		{
			var args = (MouseEventArgs)e;
			var clickedContainer = (sender as ItemContainerView).Container;
			int clickedIndex = args.SourceIndex;
			var clickedItem = clickedContainer[clickedIndex];

			if (args.Button == MouseButton.Left)
			{
				_character.PutItem(clickedContainer, clickedIndex);

				if (_character.HeldItem?.Item != null)
					InputManager.SetMouseCursor(_character.HeldItem.Item.Icon.Texture);
				else
					InputManager.ResetMouseCursor();
			}
			else if ((args.Button == MouseButton.Right) && (clickedItem != null))
			{
				// We need to know which container....
				_contextMenu = new InventoryContextMenu(sender, (clickedContainer == _containerBackpack) ? "backpack" : "hotbar", args.SourceIndex,  InputManager.MousePosition.Offset(-10, -10), clickedItem, false) { IsActive = true };
				_contextMenu.LoadContent();
				_contextMenu.OnMouseOut += _contextMenu_OnMouseOut;
				_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;
			}
		}

		private void _contextMenu_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;
			var container = (args.Source == "backpack") ? _containerBackpack : _containerHotbar;

			switch (args.Item)
			{
				case "equip"	:	
					_character.EquipArmor(container, (int)args.SourceIndex);
					_character.PutItem(_character.Backpack);
					break;
				case "split"	:
					var startPosition = InputManager.MousePosition.Offset(-10, -10);
					_splitWindow = new SplitWindow(new Rectangle(startPosition.X, startPosition.Y, 200, 200), container[(int)args.SourceIndex]) { IsActive = true };
					_splitWindow.LoadContent();
					break;
			}
			DisableContextMenu();
		}

		private void _contextMenu_OnMouseOut(object sender, EventArgs e)
		{
			DisableContextMenu();
		}

		private void _containerView_OnMouseOver(object sender, EventArgs e)
		{
			var args = (MouseEventArgs)e;
			var overContainer = (sender as ItemContainerView).Container;
			int overIndex = args.SourceIndex;
			var overItem = overContainer[overIndex];

			if ((overItem != null) && (_contextMenu?.Owner != sender))
				_tooltip.Show(overItem.Item.DisplayName, InputManager.MousePosition.Offset(10, 10), 15, sender);
			else
				_tooltip.Reset(sender);
		}

		private void _containerView_OnMouseOut(object sender, EventArgs e)
		{
			_tooltip.Reset(sender);
		}

		private void DisableContextMenu()
		{
			if (_contextMenu != null)
			{
				_contextMenu.UnloadContent();
				_contextMenu = null;
			}
		}

		private void DisableSplitWindow()
		{
			if (_splitWindow != null)
			{
				_splitWindow.UnloadContent();
				_splitWindow = null;
			}
		}
	}
}
