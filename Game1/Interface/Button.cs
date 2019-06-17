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
	public class Button : Component
	{
		private static readonly Vector2 MouseOverScale = new Vector2(1.1f, 1.1f);

		private ImageText _text;

		public Button(Rectangle bounds, string text, bool killFurtherInput = true) : base(bounds, hasBorder: true)
		{
			_text = new ImageText(text, true) { Position = bounds.CenterVector() };
		}

		protected override void BoundsChanged(bool resized)
		{
			base.BoundsChanged(resized);
			if (_text != null)
				_text.Position = Bounds.CenterVector();
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_text.LoadContent();
		}

		public override void UnloadContent()
		{
			base.LoadContent();
			_text.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_background.Scale = _mouseover ? Button.MouseOverScale : Vector2.One;
			_border.Scale = _mouseover ? Button.MouseOverScale : Vector2.One;
			_text.Scale = _mouseover ? Button.MouseOverScale : Vector2.One;
			_text.Update(gameTime);
			base.UpdateActive(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_text.Draw(spriteBatch);
		}
	}
}
