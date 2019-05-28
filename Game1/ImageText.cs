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

		public Vector2 Size { get; set; }

		public Vector2 SubstringSize(int start, int length)
		{
			return StringSize(_text.Substring(start, length));
		}

		public Vector2 StringSize(string text) => _font.MeasureString(text) * this.Scale;

		public int StringLength(string text) => (int)this.StringSize(text).X;

		public ImageText(string text = null, bool isActive = false) : this(text, null, isActive) { }

		public ImageText(string text = null, string fontName = null, bool isActive = false) : base(isActive)
		{
			_text = text;
			_fontName = fontName ?? ImageText.DefaultFont;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_font = _content.Load<SpriteFont>($"{Game1.FontsRoot}/{_fontName}");
			CalculateTextSize();
		}

		public override void DrawActive(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawString(_font, _text, this.Position + this.DrawArea.TopLeftVector(), this.Color * this.Alpha, 0.0f, _origin, this.Scale, SpriteEffects.None, 0.0f);
		}

		public void UpdateText(string text)
		{
			if (text != _text)
			{
				_text = text;
				CalculateTextSize();
			}
		}

		private void CalculateTextSize()
		{
			var baseSize = _font.MeasureString(_text);
			this.Size = baseSize * this.Scale;
			if (this.SourceRect == Rectangle.Empty)
				this.SourceRect = new Rectangle(0, 0, (int)baseSize.X, (int)baseSize.Y);
			SetOrigin();
		}
	}
}
