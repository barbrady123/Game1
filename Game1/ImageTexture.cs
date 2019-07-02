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
	public class ImageTexture : Image
	{
		protected Texture2D _highlightTexture;
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
			UpdatePosition();
		}

		public ImageTexture(Texture2D texture, Texture2D highlight, bool isActive = false) : base(isActive)
		{	
			_texture = texture;
			_highlightTexture = highlight;
			this.Alignment = ImageAlignment.LeftTop;
			UpdatePosition();
		}

		public Texture2D Texture => _texture;

		public override void LoadContent()
		{
			base.LoadContent();
			if (_texture == null)
				_texture = _content.Load<Texture2D>(_name);

			UpdatePosition();
		}

		protected virtual void UpdatePosition()
		{
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

		public override void DrawActive(SpriteBatch spriteBatch, float? alphaBlend = null, Vector2? position = null, Vector2? positionOffset = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None)
		{	
			// This is a lot of computation for each draw...resolve this....
			var pos = (position ?? this.Position) + this.PositionOffset;
			if (positionOffset != null)
				pos += (Vector2)positionOffset;

			if (this.Highlight && (_highlightTexture != null))
			{
				// Besides caching _highlightTexture texture, writing these to a render target to save 3 draws after the inital one might also be more performant??
				spriteBatch.Draw(_highlightTexture, pos.Offset(-1, -1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects, 0.0f);
				spriteBatch.Draw(_highlightTexture, pos.Offset(1, -1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects, 0.0f);
				spriteBatch.Draw(_highlightTexture, pos.Offset(-1, 1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects, 0.0f);
				spriteBatch.Draw(_highlightTexture, pos.Offset(1, 1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects, 0.0f);
				
			}

			spriteBatch.Draw(_texture, pos, this.SourceRect, this.Color * this.Alpha * (alphaBlend ?? 1.0f), this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects, 0.0f);
		}

		public void SwapTexture(string name, bool resetSourceRect = true)
		{
			_name = name;
			_texture = null;
			if (resetSourceRect)
				this.SourceRect = Rectangle.Empty;
			LoadContent();
		}
	}
}
