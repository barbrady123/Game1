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
	public class ImageSpriteSheet : ImageTexture
	{
		private int _frameSize;
		private int _currentFrame;
		private int _frameCount;
		private Cardinal _direction;

		public ImageSpriteSheet(Texture2D texture, int frameSize, bool isActive = false) : this(texture, null, frameSize, isActive) { }

		public ImageSpriteSheet(Texture2D texture, Texture2D highlight, int frameSize, bool isActive = false) : base(texture, highlight, ImageAlignment.Centered, isActive)
		{
			_frameSize = frameSize;
			_currentFrame = 0;
			_frameCount = _texture.Bounds.Width / _frameSize;
			_direction = Cardinal.South;
			_origin = new Vector2(frameSize / 2, frameSize / 2);
			this.AddEffect<SpriteSheetEffect>(false);
		}

		public void UpdateDirection(Cardinal direction)
		{
			_direction = direction;
			UpdateSourceRect();
		}

		public void IncrementFrame()
		{
			_currentFrame = (_currentFrame < _frameCount - 1) ? _currentFrame + 1 : 0;
			UpdateSourceRect();
		}

		public void SetFrame(int frameIndex)
		{
			if (frameIndex != Util.Clamp(frameIndex, 0, _frameCount - 1))
				throw new ArgumentOutOfRangeException(nameof(frameIndex));

			_currentFrame = frameIndex;
			UpdateSourceRect();
		}

		private void UpdateSourceRect()
		{
			this.SourceRect = new Rectangle(_currentFrame * _frameSize, (int)_direction * _frameSize, _frameSize, _frameSize);
		}
	}
}
