using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class SpriteSheetEffect : ImageEffect
	{
		private int _switchInterval;
		private int _currentInterval;

		public SpriteSheetEffect(bool isActive) : base(isActive)
		{
			_switchInterval = 100;
			_currentInterval = 0;
		}

		public override void Update(GameTime gameTime)
		{
			if (!this.IsActive)
				return;

			if (_currentInterval < _switchInterval)
			{
				_currentInterval++;
				return;
			}

			var sourceRect = _image.SourceRect;
			int currentFrame = sourceRect.X / Game1.TileSize;
			int newFrame = currentFrame++;
			if (newFrame >= Game1.SpriteSheetWalkFrameCount)
				newFrame = 0;

			_image.SourceRect = new Rectangle(newFrame * Game1.TileSize, sourceRect.Y, sourceRect.Width, sourceRect.Height);
			_currentInterval = 0;
		}
	}
}
