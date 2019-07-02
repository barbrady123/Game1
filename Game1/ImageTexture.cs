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
		protected Texture2D _texture;
		protected Texture2D _highlightTexture;

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

		public virtual void LoadContent()
		{
			UpdatePosition();
		}

		/// <summary>
		/// Only necessary if this texture is not tracked and disposed of elsewhere...
		/// </summary>
		public virtual void UnloadContent()
		{
			_texture.Dispose();
		}

		// This needs to be easily callable on position/alignment update, so we need a 
		// better flag for whether or not it's ok to mess with the SourceRect!
		protected virtual void UpdatePosition()
		{
			if (this.SourceRect == Rectangle.Empty)
				this.SourceRect = _texture.Bounds;

			SetOrigin();
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
	}
}
