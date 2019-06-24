﻿using System;
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


		public override void DrawActive(SpriteBatch spriteBatch, float? alphaBlend = null, Vector2? position = null, Vector2? scale = null, SpriteEffects? spriteEffects = null)
		{	
			// Testing a highlight concept....there's gotta be a more performant way LOL...
			/*if ((_texture.Name?.Contains("rock") ?? false) || (_texture.Name?.Contains("sword") ?? false))
			{
				// Testing...
				var testPos = position ?? this.Position;
				var data = new Color[_texture.Width * _texture.Height];
				_texture.GetData(data);

				for (int i = 0; i < data.Length; i++)
				{
					if (data[i].A > 0)
						data[i] = Color.White;
				}

				var reversedTexture = new Texture2D(Game1.Graphics, _texture.Width, _texture.Height);
				reversedTexture.SetData(data);

				spriteBatch.Draw(reversedTexture, testPos.Offset(-1, -1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects ?? SpriteEffects.None, 0.0f);
				spriteBatch.Draw(reversedTexture, testPos.Offset(1, -1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects ?? SpriteEffects.None, 0.0f);
				spriteBatch.Draw(reversedTexture, testPos.Offset(-1, 1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects ?? SpriteEffects.None, 0.0f);
				spriteBatch.Draw(reversedTexture, testPos.Offset(1, 1), this.SourceRect, Color.White, this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects ?? SpriteEffects.None, 0.0f);
			}*/

			spriteBatch.Draw(_texture, position ?? this.Position, this.SourceRect, this.Color * this.Alpha * (alphaBlend ?? 1.0f), this.Rotation, _origin + this.OriginOffset, scale ?? this.Scale, spriteEffects ?? SpriteEffects.None, 0.0f);
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
