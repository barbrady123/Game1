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
	public abstract class ContextMenu : MenuScreen
	{
		private Component _owner;

		public Component Owner
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

		public ContextMenu(SpriteBatchData spriteBatchData = null) : base(Rectangle.Empty, background: "black", spriteBatchData: spriteBatchData, killFurtherInput: true, drawIfDisabled: false)
		{
			// We allow empty instanciation so the object can be registered with a ComponentManager if necessary...
		}

		public void Show()
		{
			UnloadContent();
			var position = InputManager.MousePosition.Offset(-10, -10);
			this.Bounds = CalculateItemMenuBounds(position);
			_items = GetItemData();
			LoadContent();
		}

		public void Hide()
		{
			this.IsActive = false;
		}

		protected abstract Rectangle CalculateItemMenuBounds(Point position);

		protected override void ItemSelect(ComponentEventArgs e)
		{
			e.Meta = this.Owner;
			base.ItemSelect(e);
		}

		protected abstract List<string> GetItemMenuOptions();

		protected override void MouseOut(ComponentEventArgs e)
		{
			base.MouseOut(e);
			this.Owner = null;
		}

		private void _host_OnMouseOver(object sender, ComponentEventArgs e)
		{
			this.Owner = e.Meta as Component;
		}
	}
}
