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
		// TODO: Temp...
		private Interface.TextInput _text;

		public SplashScreen(Rectangle bounds): base(bounds, "maze")
		{
			_titleText = new ImageText("Labyrinth", true);
			_titleText.Position = this.Bounds.CenterVector();
			_titleText.Effects.Add(new ZoomCycleEffect(_titleText, true));
			// Temp...
			_text = new Interface.TextInput(100, "", 100, true) { Position = new Vector2(100, 100) };
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_titleText.LoadContent();
			// temp...
			_text.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_titleText.UnloadContent();
			// temp
			_text.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			if (processInput)
			{
				if (InputManager.Instance.KeyPressed(Keys.Enter))
				{
					_titleText.IsActive = false;
					ReadyScreenUnload(this, new ScreenEventArgs("change", "MainMenu"));
				}
				else if (InputManager.Instance.KeyPressed(Keys.Escape))
				{
					_titleText.IsActive = false;
					ReadyScreenUnload(this, new ScreenEventArgs("exit", null));
				}
			}

			base.Update(gameTime, processInput);
			_titleText.Update(gameTime);
			// temp
			_text.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			_titleText.Draw(spriteBatch);
			// temp
			_text.Draw(spriteBatch);
		}
	}
}
