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

namespace Game1.Interface.Windows
{
	public class InventoryWindow : Component
	{
		private ImageText _containerName;
		private World _world;
		private ItemContainerView _containerViewBackpack;
		private ItemContainerView _containerViewHotbar;
		private ItemContainer _containerBackpack;
		private ItemContainer _containerHotbar;
		private SplitWindow _splitWindow;

		public InventoryWindow(	Rectangle bounds,
								World world,
								string text,
								SpriteBatchData spriteBatchData = null) : base(	bounds,
																				true,
																				"brick",
																				spriteBatchData,
																				fireMouseEvents: false,
																				drawIfDisabled: false,
																				enabledTooltip: true,
																				enabledContextMenu: true)
		{
			_world = world;
			_containerBackpack = _world.Character.Backpack;
			var viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);

			_containerName = new ImageText(text, true) {
				Alignment = ImageAlignment.Centered,
				Position = bounds.TopCenterVector(yOffset:this.ContentMargin.Height + (FontManager.FontHeight / 2))
			};

			// Eventually, we want to make ItemContainerView a component (and InventoryItemView)....
			_activator.Register(_containerViewBackpack = ItemContainerView.New<ItemContainerView>(_containerBackpack, viewPosition.Offset(0, this.ContentMargin.Height * 3), false), true, "backpack");
			_containerViewBackpack.OnMouseLeftClick += _containerView_OnMouseLeftClick;
			_containerViewBackpack.OnMouseRightClick += _containerView_OnMouseRightClick;
			_containerViewBackpack.OnMouseOver += _containerView_OnMouseOver;

			_containerHotbar = _world.Character.HotBar;
			_activator.Register(
				_containerViewHotbar = ItemContainerView.New<ItemContainerView>(
					_containerHotbar,
					new Point(viewPosition.X, _containerViewBackpack.Bounds.Bottom + (this.ContentMargin.Height * 2)),
					false
				), true, "hotbar");
			_containerViewHotbar.OnMouseLeftClick += _containerView_OnMouseLeftClick;
			_containerViewHotbar.OnMouseRightClick += _containerView_OnMouseRightClick;
			_containerViewHotbar.OnMouseOver += _containerView_OnMouseOver;

			_activator.Register(_splitWindow = new SplitWindow(SpriteBatchManager.Get("context")), false, new[] { "popup", "backpack", "hotbar" });
			_splitWindow.OnButtonClick += _splitWindow_OnButtonClick;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_containerName.LoadContent();
			_containerViewBackpack.LoadContent();
			_containerViewHotbar.LoadContent();
			_splitWindow.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_containerName.UnloadContent();
			_containerViewBackpack.UnloadContent();
			_containerViewHotbar.UnloadContent();
			_splitWindow.UnloadContent();
			DisableSplitWindow();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_splitWindow.Update(gameTime);
			base.UpdateActive(gameTime);
			_containerViewBackpack.Update(gameTime);
			_containerViewHotbar.Update(gameTime);
			InputManager.BlockAllInput();
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			_splitWindow.Draw(spriteBatch);
			base.DrawInternal(spriteBatch);
			_containerName.Draw(spriteBatch);
			_containerViewBackpack.Draw(spriteBatch);
			_containerViewHotbar.Draw(spriteBatch);
		}

		private void _containerView_OnMouseLeftClick(object sender, ComponentEventArgs e)
		{
			var clickedItemView = (InventoryItemView)e.Meta;
			var clickedContainer = ((ItemContainerView)sender).Container;
			_world.Character.SwapHeld(clickedContainer, clickedItemView.Index);
		}

		private void _containerView_OnMouseRightClick(object sender, ComponentEventArgs e) => MouseRightClick(e);
		private void _containerView_OnMouseOver(object sender, ComponentEventArgs e) => MouseOver(e);

		protected override void ContextMenuSelect(ComponentEventArgs e)
		{
			// e.Meta type check is needed since a component supporting ContextMenu could have different objects that we need menus for...
			if (!(e.Meta is InventoryItemView itemView))
				return;

			switch (e.Value)
			{
				case "equip"	:	
					_world.Character.EquipArmor(itemView.ContainingView.Container, itemView.Index);
					break;
				case "eat"		:
				case "drink"	:
				case "read"		:
					_world.Character.Consume(itemView.ContainingView.Container, itemView.Index);
					break;
				case "drop"		:
					var item = itemView.ContainingView.Container.RemoveItem(itemView.Index);
					_world.AddItem(item, pickup: false);
					break;
				case "split"	:
					EnableSplitWindow(itemView);
					break;
				case "cancel" :
					break;
			}
		}

		protected override void ReadyDisable(ComponentEventArgs e)
		{
			_splitWindow.Owner = null;
			base.ReadyDisable(e);
		}

		// This functionality needs to be sharable somehow....move elsewhere...
		// This could be SplitWindow functionality, but then we'd have to give the
		// window access to the _world object...seems odd...
		private void _splitWindow_OnButtonClick(object sender, ComponentEventArgs e)
		{
			switch (e.Value)
			{
				case "ok" :
					var window = (SplitWindow)sender;
					SplitItem(window.Owner, window.Quantity);
					break;
			}
		}

		// This functionality needs to be sharable somehow....move elsewhere...
		private void SplitItem(InventoryItemView itemView, int quantity)
		{
			if (!_world.Character.HoldItemQuantity(itemView.ContainingView.Container, itemView.Index, quantity))
			{
				// Need to tell user this failed...if we get here it means that we couldn't
				// combine the quantity specified with the currently held item, and we also
				// didn't have room to store the held item anywhere...
			}
		}

		private void EnableSplitWindow(InventoryItemView itemView)
		{
			_splitWindow.Owner = itemView;
		}

		private void DisableSplitWindow()
		{
			_activator.SetState(_splitWindow, false);
		}
	}
}
