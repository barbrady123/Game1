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
		private const int SwitchInterval = 5;
		private int _currentInterval;

		public SpriteSheetEffect(bool isActive) : base(isActive)
		{
			Initialize();
		}

		protected override void Initialize()
		{
			_currentInterval = 0;
		}

		public override void Update(GameTime gameTime)
		{
			if (!this.IsActive)
				return;

			// It's only valid to assign this effect to an ImageSpriteSheet...should probably check this another way lol
			if (!(_image is ImageSpriteSheet spriteSheet))
				return;

			if (_currentInterval < SpriteSheetEffect.SwitchInterval)
			{
				_currentInterval++;
				return;
			}

			spriteSheet.IncrementFrame();
			_currentInterval = 0;
			/*
			var sourceRect = _image.SourceRect;
			int currentFrame = sourceRect.X / Game1.TileSize;
			int newFrame = currentFrame + 1;
			if (newFrame >= Game1.SpriteSheetWalkFrameCount)
				newFrame = 0;

			_image.SourceRect = new Rectangle(newFrame * Game1.TileSize, sourceRect.Y, Game1.TileSize, Game1.TileSize);
			_currentInterval = 0;
			*/
		}
	}
}
