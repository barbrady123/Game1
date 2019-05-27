using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;

namespace Game1.Screen
{
	public class SplashScreen : GameScreen
	{
		private ImageBase _titleText;

		public SplashScreen(Rectangle bounds): base(bounds, "maze")
		{
			_titleText = new ImageText("Labyrinth", true);
			_titleText.Position = this.Bounds.CenterVector();
			_titleText.Effects.Add(new ZoomCycleEffect(_titleText, true));
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_titleText.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_titleText.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			if (processInput)
			{
				if (InputManager.Instance.KeyPressed(Keys.Enter))
				{
					_titleText.IsActive = false;
					ReadyScreenUnload(this, new ScreenEventArgs("continue", this.GetType().Name, null));
				}
				else if (InputManager.Instance.KeyPressed(Keys.Escape))
				{
					_titleText.IsActive = false;
					ReadyScreenUnload(this, new ScreenEventArgs("exit", this.GetType().Name, null));
				}
			}

			base.Update(gameTime, processInput);
			_titleText.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			_titleText.Draw(spriteBatch);
		}
	}
}
