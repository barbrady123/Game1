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
using Game1.Enum;
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1.Interface
{
	public class Button
	{
		private static readonly Vector2 MouseOverScale = new Vector2(1.1f, 1.1f);

		private Rectangle _bounds;
		private ImageTexture _background;
		private ImageTexture _border;
		private ImageText _text;

		public bool IsActive { get; set; }

		public event EventHandler OnClick;

		public Button(Rectangle bounds, string text)
		{
			_bounds = bounds;
			_background = Util.GenerateSolidBackground(bounds.Width, bounds.Height, Color.Black);
			_background.Position = bounds.CenterVector();
			_background.Alignment = ImageAlignment.Centered;
			_border = Util.GenerateBorderTexture(bounds.Width, bounds.Height, 2, Color.White);
			_border.Position = bounds.CenterVector();
			_border.Alignment = ImageAlignment.Centered;
			_text = new ImageText(text, true) { Position = bounds.CenterVector() };
		}

		public void LoadContent()
		{
			_background.LoadContent();
			_border.LoadContent();
			_text.LoadContent();
		}

		public void UnloadContent()
		{
			_background.UnloadContent();
			_border.UnloadContent();
			_text.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				bool mouseOver = InputManager.MouseOver(_bounds);

				_background.Scale = mouseOver ? Button.MouseOverScale : Vector2.One;
				_background.Update(gameTime);
				_border.Scale = mouseOver ? Button.MouseOverScale : Vector2.One;
				_border.Update(gameTime);
				_text.Scale = mouseOver ? Button.MouseOverScale : Vector2.One;
				_text.Update(gameTime);

				if (mouseOver)
				{
					if (InputManager.LeftMouseClick())
						OnClick?.Invoke(this, new MouseEventArgs(MouseButton.Left));
					else if (InputManager.RightMouseClick())
						OnClick?.Invoke(this, new MouseEventArgs(MouseButton.Right));
					InputManager.BlockButtonClicks();
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_background.Draw(spriteBatch);
			_border.Draw(spriteBatch);
			_text.Draw(spriteBatch);
		}
	}
}
