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
		private Character _character;
		private ItemContainerView _containerViewBackpack;
		private ItemContainerView _containerViewHotbar;
		private ItemContainer _containerBackpack;
		private ItemContainer _containerHotbar;
		private InventoryContextMenu _contextMenu;
		private Tooltip _tooltip;
		private SplitWindow _splitWindow;

		public InventoryWindow(Rectangle bounds, Character character, string text) : base(bounds, true, "brick")
		{
			_components = new ComponentManager();

			_character = character;
			_containerBackpack = character.Backpack;
			var viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);

			_containerName = new ImageText(text, true) {
				Alignment = ImageAlignment.Centered,
				Position = bounds.TopCenterVector(yOffset:this.ContentMargin.Height + (FontManager.FontHeight / 2))
			};

			// Eventually, we want to make ItemContainerView a component (and InventoryItemView)....
			_components.Register(_containerViewBackpack = ItemContainerView.New<ItemContainerView>(_containerBackpack, viewPosition, false));
			_containerViewBackpack.OnMouseClick += _containerView_OnMouseClick;
			_containerViewBackpack.OnMouseOver += _containerView_OnMouseOver;
			_containerViewBackpack.OnMouseOut += _containerView_OnMouseOut;
			_components.SetState(_containerViewBackpack, ComponentState.All, null);

			// Eventually, we want to make ItemContainerView a component (and InventoryItemView)....
			_containerHotbar = character.HotBar;
			_components.Register(
				_containerViewHotbar = ItemContainerView.New<ItemContainerView>(
					_containerHotbar,
					new Point(viewPosition.X, viewPosition.Y + _containerViewBackpack.Bounds.Size.Y + this.ContentMargin.Height),
					false
				));
			_containerViewHotbar.OnMouseClick += _containerView_OnMouseClick;
			_components.SetState(_containerViewHotbar, ComponentState.All, null);

			_components.Register(_tooltip = new Tooltip());
			_components.SetState(_tooltip, ComponentState.Active, null);

			_components.Register(_contextMenu = new InventoryContextMenu(null, Util.PointInvalid, false));
			// These need to be instanciatable early, their parameters need to be run-time adjustable
			// so we can easily register them here....
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
			_contextMenu.UnloadContent();
			DisableContextMenu();
			DisableSplitWindow();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			// We removed a bunch of conditional input checks here...need to add these elsewhere (with the ComponentManager)....
			_splitWindow?.Update(gameTime);
			_contextMenu.Update(gameTime);
			_containerViewBackpack.Update(gameTime);
			_containerViewHotbar.Update(gameTime);
			_tooltip.Update(gameTime);			
			base.UpdateActive(gameTime);
			InputManager.BlockAllInput();
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_containerViewBackpack.Draw(spriteBatch);
			_containerViewHotbar.Draw(spriteBatch);
			// This whole mechanism should be cleaned up also....
			SpriteBatchData batchData = null;
			if (_tooltip.State.HasFlag(ComponentState.Visible))
			{
				batchData = SpriteBatchManager.Get("tooltip");
				batchData.ScissorWindow = _tooltip.Bounds;	// one reason why it was good to have the components self-aware of the batch andwrap the call...
				_tooltip.Draw(batchData.SpriteBatch);
			}
			batchData = SpriteBatchManager.Get("context");
			batchData.ScissorWindow = _contextMenu.Bounds;
			_contextMenu.Draw(batchData.SpriteBatch);
			if (_splitWindow != null)
			{
				batchData.ScissorWindow = _splitWindow.Bounds;
				_splitWindow.Draw(batchData.SpriteBatch);
			}
		}

		protected override void ReadyDisable(ComponentEventArgs args)
		{
			if (_character.HeldItem != null)
			{
				_character.PutItem(_containerViewBackpack.Container);
				_character.HeldItem = null;
			}

			base.ReadyDisable(args);
		}

		private void _containerView_OnMouseClick(object sender, ComponentEventArgs e)
		{
			var clickedContainer = ((ItemContainerView)sender).Container;
			var clickedItemView = (InventoryItemView)e.InnerEventArgs.Sender;

			if (e.Button == MouseButton.Left)
			{
				_character.PutItem(clickedContainer, clickedItemView.Index);
			}
			else if ((e.Button == MouseButton.Right) && (clickedItemView.Item != null))
			{
				_contextMenu.Initialize(clickedItemView, InputManager.MousePosition.Offset(-10, -10), false);
				_contextMenu.OnMouseOut += _contextMenu_OnMouseOut;
				_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;
				_components.SetState(_contextMenu, ComponentState.All, null);
			}
		}

		private void _contextMenu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			var itemView = (InventoryItemView)((InventoryContextMenu)sender).Owner;

			switch (e.Item)
			{
				case "equip"	:	
					_character.EquipArmor(itemView.ContainingView.Container, itemView.Index);
					_character.PutItem(_character.Backpack);
					break;
				case "split"	:
					var startPosition = InputManager.MousePosition.Offset(-10, -10);
					_splitWindow = new SplitWindow(new Rectangle(startPosition.X, startPosition.Y, 200, 200), itemView.Item) { State = ComponentState.All };
					_splitWindow.LoadContent();
					_splitWindow.OnButtonClick += _splitWindow_OnButtonClick;
					_splitWindow.OnReadyDisable += _splitWindow_OnReadyDisable;
					break;
			}
			DisableContextMenu();
		}

		private void _splitWindow_OnReadyDisable(object sender, EventArgs e)
		{
			DisableSplitWindow();
		}

		private void _splitWindow_OnButtonClick(object sender, EventArgs e)
		{
			var args = (ComponentEventArgs)e;
			switch (args.Item)
			{
				case "ok" : 
					break;					
				case "cancel" :
					break;
			}

			DisableSplitWindow();
		}

		private void _contextMenu_OnMouseOut(object sender, ComponentEventArgs e)
		{
			DisableContextMenu();
		}

		private void _containerView_OnMouseOver(object sender, ComponentEventArgs e)
		{
			var overItemView = (InventoryItemView)e.InnerEventArgs.Sender;

			if ((_contextMenu?.Owner != overItemView) || (overItemView.Item == null))
				_tooltip.Show(overItemView.Item.Item.DisplayName, InputManager.MousePosition.Offset(10, 10), 15, overItemView);
			else
				_tooltip.Reset(sender);
		}

		private void _containerView_OnMouseOut(object sender, ComponentEventArgs e)
		{
			_tooltip.Reset(sender);
		}

		private void DisableContextMenu()
		{
			_components.SetState(_contextMenu, ComponentState.None, null);
			_contextMenu.Clear();
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
