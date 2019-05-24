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

namespace Game1
{
	public class Image
	{
		// TODO: Maybe move this to base abstract and break up texture/text images....do we ever need this shit together like this?
		private ContentManager _content;
		private GraphicsDevice _graphics;

		private string _text;
		private string _fontName;
		private string _name;

		private Texture2D _texture;
		private SpriteFont _font;

		public bool IsActive { get; set; }

		public Vector2 Scale { get; set; }
		public Vector2 Position { get; set; }
		public float Alpha { get; set; }
		public Rectangle SourceRect { get; set; }

		public List<ImageEffect> Effects { get; set; }

		public Image(GraphicsDevice graphics, string name, string text = null, bool isActive = false)
		{
			_graphics = graphics;
			_name = name;
			_text = text;
			_fontName = "Fonts/Orbitron";
			this.SourceRect = Rectangle.Empty;
			this.Alpha = 1.0f;
			this.Effects = new List<ImageEffect>();
			this.Scale = Vector2.One;
			this.IsActive = isActive;
		}

		public void LoadContent(IServiceProvider services)
		{
			_content = new ContentManager(services, "Content");
			if (!String.IsNullOrWhiteSpace(_name))
				_texture = _content.Load<Texture2D>(_name);

			if (!String.IsNullOrWhiteSpace(_fontName))
				_font = _content.Load<SpriteFont>(_fontName);

			Vector2 dimensions = Vector2.Zero;

			// Calculate the size of the source rectangle
			if (_texture != null)
			{
				dimensions.X = _texture.Width;
				dimensions.Y = _texture.Height;
			}

			if (!String.IsNullOrWhiteSpace(_text))
			{				
				var textSize = _font.MeasureString(_text);
				dimensions.X += textSize.X;
				dimensions.Y = Math.Max(_texture?.Height ?? 0, textSize.Y);
			}

			this.SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

			// Generate the final texture (image and/or text string + effects)...
			var renderTarget = new RenderTarget2D(_graphics, (int)dimensions.X, (int)dimensions.Y);
			_graphics.SetRenderTarget(renderTarget);
			_graphics.Clear(Color.Transparent);
			var spriteBatch = new SpriteBatch(_graphics);
			spriteBatch.Begin();
			if (_texture != null)
				spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
			if (!String.IsNullOrWhiteSpace(_text))
				spriteBatch.DrawString(_font, _text, Vector2.Zero, Color.White);
			spriteBatch.End();
			_texture = renderTarget;
			_graphics.SetRenderTarget(null);
		}

		public void UnloadContent()
		{
			if (_content != null)
				_content.Unload();
		}

		public void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				foreach (var effect in this.Effects)
					effect.Update(gameTime);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (this.IsActive)
			{
				var origin = new Vector2(this.SourceRect.Width / 2, this.SourceRect.Height / 2);
				spriteBatch.Draw(_texture, this.Position + origin, this.SourceRect, Color.White * this.Alpha, 0.0f, origin, this.Scale, SpriteEffects.None, 0.0f);
			}
		}
	}
}
