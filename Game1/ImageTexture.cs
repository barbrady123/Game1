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
	public class ImageTexture : ImageBase
	{
		protected Texture2D _texture;
		private string _name;

		public ImageTexture(string name, bool isActive = false) : base(isActive)
		{
			_name = name;
			this.Alignment = ImageAlignment.LeftTop;
		}

		public ImageTexture(Texture2D texture, bool isActive = false) : base(isActive)
		{
			_texture = texture;
			this.Alignment = ImageAlignment.LeftTop;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			if (_texture == null)
				_texture = _content.Load<Texture2D>(_name);

			if (this.SourceRect == Rectangle.Empty)
				this.SourceRect = _texture.Bounds;

			SetOrigin();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			// In case this texture was generated elsewhere...
			_texture?.Dispose();
		}

		public override void DrawActive(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(_texture, this.Position + this.DrawArea.TopLeftVector(), this.SourceRect, this.Color * this.Alpha, 0.0f, _origin, this.Scale, SpriteEffects.None, 0.0f);
		}

		public void SwapTexture(string name, bool resetSourceRect = true)
		{
			_name = name;
			if (resetSourceRect)
				this.SourceRect = Rectangle.Empty;
			LoadContent();
		}
	}
}
