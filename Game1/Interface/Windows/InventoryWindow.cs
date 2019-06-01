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
		private ItemContainerView _containerViewBackpack;
		private ItemContainerView _containerViewHotbar;
		private ItemContainer _containerBackpack;
		private ItemContainer _containerHotbar;

		public InventoryWindow(	string text,
								DialogButton buttons,
								Rectangle bounds, 
								int? duration, 
								ItemContainer containerBackpack, 
								ItemContainer containerHotbar) : base(bounds, "brick", text, duration, true)
		{
			_containerBackpack = containerBackpack;
			var viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);
			_containerViewBackpack = ItemContainerView.New<ItemContainerView>(containerBackpack, viewPosition, false);

			_containerHotbar = containerHotbar;
			_containerViewHotbar = ItemContainerView.New<ItemContainerView>(containerHotbar, new Point(viewPosition.X, viewPosition.Y + _containerViewBackpack.Bounds.Size.Y + this.ContentMargin.Height), false);
			viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);
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
	}
}
