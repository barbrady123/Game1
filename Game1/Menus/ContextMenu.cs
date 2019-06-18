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

namespace Game1.Menus
{
	public class ContextMenu : Menu
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

		public ContextMenu(Component host, SpriteBatchData spriteBatchData = null) : base(Util.PointInvalid, background: "black", spriteBatchData: spriteBatchData, drawIfDisabled: false)
		{
			_host = host;
			_host.OnMouseRightClick += _host_OnMouseRightClick;
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			InputManager.BlockAllInput();
		}

		private void Show()
		{
			var position = InputManager.MousePosition.Offset(-10, -10);
			this.Bounds = SetupMenuItems(position);
			if (this.Bounds != Rectangle.Empty)
				this.IsActive = true;
		}

		private void Hide()
		{
			this.IsActive = false;
		}

		protected override List<string> GetItemData()
		{
			return this.Owner?.GetContextMenuOptions() ?? new List<string>();
		}

		protected override void ItemSelect(ComponentEventArgs e)
		{
			e.Meta = this.Owner;
			base.ItemSelect(e);
			this.Owner = null;
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
