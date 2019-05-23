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
		private Image _titleText;
		private Vector2 _size;

		public SplashScreen(GraphicsDevice graphics, Vector2 size): base(graphics)
		{
			_size = size;
			_backgroundImage = new Image(graphics, "Background/maze", null, true);
			_titleText = new Image(graphics, null, "The Title", true);
			_titleText.Position = _size / 2;
			_titleText.Effects.Add(new ZoomCycleEffect(_titleText, true));			
		}

		public override void LoadContent(IServiceProvider services)
		{
			base.LoadContent(services);
			_titleText.LoadContent(services);
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
					ReadyScreenUnload(this);
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
