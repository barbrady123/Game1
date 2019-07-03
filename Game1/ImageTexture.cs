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

		public ImageTexture(Texture2D texture, ImageAlignment? alignment, bool isActive = false) : this(texture, null, alignment, isActive) { }

		public ImageTexture(Texture2D texture, Texture2D highlight, ImageAlignment? alignment, bool isActive = false) : base(isActive)
		{	
			_texture = texture;
			_highlightTexture = highlight;
			this.Alignment = alignment ?? ImageAlignment.LeftTop;	// We probably want to default this to center but i need to run through the code and reverse some assumptions first
			this.Bounds = _texture.Bounds;
			SetOrigin();
		}

		public Texture2D Texture => _texture;

		/// <summary>
		/// Only necessary if this texture is not tracked and disposed of elsewhere...
		/// </summary>
		public virtual void UnloadContent()
		{
			// TODO: WE need to remove this and put the responsibiliy for these else...thsi is dangerous!!
			_texture.Dispose();
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
