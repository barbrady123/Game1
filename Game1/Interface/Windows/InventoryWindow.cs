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

		public InventoryWindow(	string text,
								DialogButton buttons,
								Rectangle bounds, 
								int? duration, 
								Character character) : base(bounds, "brick", text, duration, true)
		{
			_character = character;
			_containerBackpack = character.Backpack;
			var viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);

			_containerViewBackpack = ItemContainerView.New<ItemContainerView>(_containerBackpack, viewPosition, false);
			_containerViewBackpack.OnMouseClick += _containerView_OnMouseClick;

			_containerHotbar = character.HotBar;
			_containerViewHotbar = ItemContainerView.New<ItemContainerView>(_containerHotbar, new Point(viewPosition.X, viewPosition.Y + _containerViewBackpack.Bounds.Size.Y + this.ContentMargin.Height), false);
			_containerViewHotbar.OnMouseClick += _containerView_OnMouseClick;
		}

		private void _containerView_OnMouseClick(object sender, EventArgs e)
		{
			var args = (MouseEventArgs)e;
			var clickedContainer = (sender as ItemContainerView).Container;
			int clickedIndex = args.SourceIndex;
			var clickedItem = clickedContainer[clickedIndex];

			_character.PutItem(clickedContainer, clickedIndex);

			if (_character.HeldItem?.Item != null)
				InputManager.SetMouseCursor(_character.HeldItem.Item.Icon.Texture);
			else
				InputManager.ResetMouseCursor();
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_containerViewBackpack.LoadContent();
			_containerViewHotbar.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_containerViewBackpack.UnloadContent();
			_containerViewHotbar.UnloadContent();
		}

		public override void UpdateReady(GameTime gameTime, bool processInput)
		{
			base.UpdateReady(gameTime, processInput);
			_containerViewBackpack.Update(gameTime);
			_containerViewHotbar.Update(gameTime);
		}

		public override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_containerViewBackpack.Draw(spriteBatch);
			_containerViewHotbar.Draw(spriteBatch);
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
	}
}
