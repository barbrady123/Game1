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
using Game1.Effect;

namespace Game1.Screens
{
	public class SplashScreen : Component
	{
		private ImageText _title;

		public SplashScreen(Rectangle bounds): base(bounds, true, "maze")
		{
			_title = new ImageText("Labyrinth", true) { Position = this.Bounds.CenterVector() };
			_title.AddEffect<ZoomCycleEffect>(true);
		}

		public override void LoadContent()
		{
			base.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_title.Update(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);

			if (InputManager.KeyPressed(Keys.Enter))
			{
				_title.IsActive = false;
				ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.KeyPressed });
			}
		}

		protected override void MouseLeftClick(ComponentEventArgs e)
		{
			_title.IsActive = false;
			ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.ButtonClick });
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{	
			base.DrawInternal(spriteBatch);
			_title.Draw(spriteBatch);
		}
	}
}
