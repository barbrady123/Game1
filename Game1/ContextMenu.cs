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
using Game1.Interface;
using Game1.Items;
using Game1.Screens.Menu;

namespace Game1.Screens.Menu
{
	public class ContextMenu : MenuScreen
	{
		private ISupportsContextMenu _owner;
		private Component _host;

		public ISupportsContextMenu Owner
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

		public ContextMenu(Component host, SpriteBatchData spriteBatchData = null) : base(Rectangle.Empty, background: "black", spriteBatchData: spriteBatchData, drawIfDisabled: false)
		{
			_host = host;
			_host.OnMouseRightClick += _host_OnMouseRightClick;
		}

		protected override List<MenuItem> GetItemData() { return new List<MenuItem>(); }

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			InputManager.BlockAllInput();
		}

		private void Show()
		{
			UnloadContent();
			var position = InputManager.MousePosition.Offset(-10, -10);
			this.Bounds = CalculateItemMenuBounds(position);
			LoadContent();
			this.IsActive = true;
		}

		private void Hide()
		{
			this.IsActive = false;
		}

		protected virtual Rectangle CalculateItemMenuBounds(Point position)
		{
			if ((position == Util.PointInvalid) || (this.Owner == null))
				return Rectangle.Empty;
			
			var menuOptions = this.Owner.GetContextMenuOptions();
			var size = MenuScreen.CalculateMenuSize(MenuScreen.MENU_PADDING, MenuScreen.MENU_PADDING, menuOptions, MenuLayout.Vertical);
			_items = menuOptions.Select(o => new MenuItem { Id = o.ToLower(), Text = o }).ToList();
			return new Rectangle(position.X, position.Y, size.Width, size.Height);
		}

		protected override void ItemSelect(ComponentEventArgs e)
		{
			e.Meta = this.Owner;
			base.ItemSelect(e);
			this.Owner = null;	// Is this the right spot to kill it after select??
		}

		protected override void MouseOut(ComponentEventArgs e)
		{
			base.MouseOut(e);
			this.Owner = null;
		}

		private void _host_OnMouseRightClick(object sender, ComponentEventArgs e)
		{
			this.Owner = (ISupportsContextMenu)e.Meta;
		}
	}
}
