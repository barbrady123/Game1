using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game1.Effect;
using Game1.Enum;

namespace Game1
{
	public class ImageText : ImageBase
	{
		public const string DefaultFont = "Orbitron";

		private string _text;
		private string _fontName;
		private SpriteFont _font;

		public string FontName { get; set; }

		public ImageText(string text = null, bool isActive = false) : this(text, null, isActive) { }

		public ImageText(string text = null, string fontName = null, bool isActive = false) : base(isActive)
		{
			_text = text;
			_fontName = fontName ?? ImageText.DefaultFont;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			if (!String.IsNullOrWhiteSpace(_fontName))
				_font = _content.Load<SpriteFont>($"{Game1.FontsRoot}/{_fontName}");

			_texture = GenerateTextureFromText(_font.MeasureString(_text));
			if (this.SourceRect == Rectangle.Empty)
				this.SourceRect = _texture.Bounds;

			SetOrigin();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}

		/// <summary>
		/// Generate texture from text (to make texture/string rendering sharable...
		/// </summary>
		private Texture2D GenerateTextureFromText(Vector2 textSize)
		{
			var renderTarget = new RenderTarget2D(Game1.Graphics, (int)textSize.X, (int)textSize.Y);
			Game1.Graphics.SetRenderTarget(renderTarget);
			Game1.Graphics.Clear(Color.Transparent);
			var spriteBatch = new SpriteBatch(Game1.Graphics);
			spriteBatch.Begin();
			spriteBatch.DrawString(_font, _text, Vector2.Zero, Color.White);
			spriteBatch.End();
			Game1.Graphics.SetRenderTarget(null);
			return renderTarget;
		}
	}
}
