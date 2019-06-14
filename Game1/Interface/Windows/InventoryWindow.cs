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
	public class InventoryWindow : Component
	{
		private readonly ComponentManager _components;
		private ImageText _containerName;
		private World _world;
		private ItemContainerView _containerViewBackpack;
		private ItemContainerView _containerViewHotbar;
		private ItemContainer _containerBackpack;
		private ItemContainer _containerHotbar;
		private InventoryContextMenu _contextMenu;
		private Tooltip _tooltip;
		private SplitWindow _splitWindow;

		public InventoryWindow(Rectangle bounds, World world, string text,  SpriteBatchData spriteBatchData = null) : base(bounds, true, "brick", spriteBatchData)
		{
			_components = new ComponentManager();

			_world = world;
			_containerBackpack = _world.Character.Backpack;
			var viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);

			_containerName = new ImageText(text, true) {
				Alignment = ImageAlignment.Centered,
				Position = bounds.TopCenterVector(yOffset:this.ContentMargin.Height + (FontManager.FontHeight / 2))
			};

			// Eventually, we want to make ItemContainerView a component (and InventoryItemView)....
			_components.Register(_containerViewBackpack = ItemContainerView.New<ItemContainerView>(_containerBackpack, viewPosition.Offset(0, this.ContentMargin.Height * 3), false));
			_containerViewBackpack.OnMouseClick += _containerView_OnMouseClick;
			_containerViewBackpack.OnMouseOver += _containerView_OnMouseOver;
			_containerViewBackpack.OnMouseOut += _containerView_OnMouseOut;
			_components.SetState(_containerViewBackpack, ComponentState.All, null);

			// Eventually, we want to make ItemContainerView a component (and InventoryItemView)....
			_containerHotbar = _world.Character.HotBar;
			_components.Register(
				_containerViewHotbar = ItemContainerView.New<ItemContainerView>(
					_containerHotbar,
					new Point(viewPosition.X, _containerViewBackpack.Bounds.Bottom + (this.ContentMargin.Height * 2)),
					false
				));
			_containerViewHotbar.OnMouseClick += _containerView_OnMouseClick;
			_components.SetState(_containerViewHotbar, ComponentState.All, null);

			_components.Register(_tooltip = new Tooltip(SpriteBatchManager.Get("tooltip")));
			_components.SetState(_tooltip, ComponentState.Active, null);

			var contextBatchData = SpriteBatchManager.Get("context");
			_components.Register(_contextMenu = new InventoryContextMenu(contextBatchData));
			_contextMenu.OnMouseOut += _contextMenu_OnMouseOut;
			_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;

			_components.Register(_splitWindow = new SplitWindow(contextBatchData));
			_splitWindow.OnButtonClick += _splitWindow_OnButtonClick;
			_splitWindow.OnReadyDisable += _splitWindow_OnReadyDisable;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_containerName.LoadContent();
			_containerViewBackpack.LoadContent();
			_containerViewHotbar.LoadContent();
			_tooltip.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_containerName.UnloadContent();
			_containerViewBackpack.UnloadContent();
			_containerViewHotbar.UnloadContent();
			_tooltip.UnloadContent();
			_contextMenu.UnloadContent();
			_splitWindow.UnloadContent();
			DisableContextMenu();
			DisableSplitWindow();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_contextMenu.Update(gameTime);
			_splitWindow.Update(gameTime);
			_tooltip.Update(gameTime);
			_containerViewBackpack.Update(gameTime);
			_containerViewHotbar.Update(gameTime);
			base.UpdateActive(gameTime);
			InputManager.BlockAllInput();
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_containerName.Draw(spriteBatch);
			_containerViewBackpack.Draw(spriteBatch);
			_containerViewHotbar.Draw(spriteBatch);
			_tooltip.Draw(spriteBatch);
			_contextMenu.Draw(spriteBatch);
			_splitWindow.Draw(spriteBatch);
		}

		private void _containerView_OnMouseClick(object sender, ComponentEventArgs e)
		{
			var clickedItemView = (InventoryItemView)e.Meta;
			var clickedContainer = ((ItemContainerView)sender).Container;

			if (e.Button == MouseButton.Left)
			{
				_world.Character.SwapHeld(clickedContainer, clickedItemView.Index);
			}
			else if ((e.Button == MouseButton.Right) && (clickedItemView.Item != null))
			{
				EnableContextMenu(clickedItemView);
			}

			_tooltip.Reset(clickedItemView);
		}

		private void _contextMenu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			var itemView = (InventoryItemView)e.Meta;
			DisableContextMenu();

			switch (e.Value)
			{
				case "equip"	:	
					_world.Character.EquipArmor(itemView.ContainingView.Container, itemView.Index);
					break;
				case "eat"		:
				case "drink"	:
					_world.Character.Consume(itemView.ContainingView.Container, itemView.Index);
					break;
				case "drop"		:
					var item = itemView.ContainingView.Container.RemoveItem(itemView.Index);
					_world.AddItem(item, pickup: false);
					break;
				case "split"	:
					EnableSplitWindow(itemView);
					break;
			}
		}

		private void _splitWindow_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			DisableSplitWindow();
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
				case "cancel" :
					break;
			}

			DisableSplitWindow();
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

		private void _contextMenu_OnMouseOut(object sender, ComponentEventArgs e)
		{
			DisableContextMenu();
		}

		private void _containerView_OnMouseOver(object sender, ComponentEventArgs e)
		{
			var overItemView = (InventoryItemView)e.Meta;

			if ((_contextMenu?.Owner != overItemView) && (overItemView.Item != null))
				_tooltip.Show(overItemView.Item.Item.DisplayName, InputManager.MousePosition.Offset(10, 10), 15, overItemView);
			else
				_tooltip.Reset(sender);
		}

		private void _containerView_OnMouseOut(object sender, ComponentEventArgs e)
		{
			_tooltip.Reset(e.Meta);
		}

		private void EnableContextMenu(InventoryItemView clickedItemView)
		{
			_contextMenu.Initialize(clickedItemView, InputManager.MousePosition.Offset(-10, -10), false);
			_components.SetState(_contextMenu, ComponentState.All, null);
			_components.ClearState(new[] {_containerViewBackpack, _containerViewHotbar}, ComponentState.TakingInput);
			_tooltip.Reset();
		}

		private void DisableContextMenu()
		{
			_components.AddState(new[] {_containerViewBackpack, _containerViewHotbar}, ComponentState.TakingInput);
			_components.SetState(_contextMenu, ComponentState.None, null);
			_contextMenu.Clear();
		}

		private void EnableSplitWindow(InventoryItemView itemView)
		{
			var startPosition = InputManager.MousePosition.Offset(-10, -10);
			_splitWindow.Initialize(itemView, new Rectangle(startPosition.X, startPosition.Y, 200, 200));
			_components.SetState(_splitWindow, ComponentState.All, null);
			_components.ClearState(new[] {_containerViewBackpack, _containerViewHotbar}, ComponentState.TakingInput);
			_tooltip.Reset();
		}

		private void DisableSplitWindow()
		{
			_components.AddState(new[] {_containerViewBackpack, _containerViewHotbar}, ComponentState.TakingInput);
			_components.SetState(_splitWindow, ComponentState.None, null);
			_splitWindow.Clear();
		}
	}
}
