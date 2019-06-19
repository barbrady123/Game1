using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Menus;

namespace Game1.Screens
{
	public class MenuScreen<T> : Component where T: Menu
	{
		private T _menu;

		public MenuScreen(Rectangle bounds): base(bounds, true, "brick")
		{
			_menu = (T)Activator.CreateInstance(typeof(T), new object[] { Point.Zero });
			_menu.OnItemSelect += _menu_OnItemSelect;
			var size = _menu.CalculateMenuSize(null);
			_menu.UpdatePosition(new Point(bounds.Center.X - (size.Width / 2), bounds.Center.Y - (size.Height / 2)));
			_menu.IsActive = true;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_menu.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_menu.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			_menu.Update(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_menu.Draw(spriteBatch);
		}

		private void _menu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			ReadyDisable(e);
		}
	}
}
