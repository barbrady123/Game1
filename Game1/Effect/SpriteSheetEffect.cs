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
		private ImageSpriteSheet _spriteSheet;
		private const int SwitchInterval = 5;
		private int _currentInterval;

		public SpriteSheetEffect(Image image, bool isActive) : base(image, isActive)
		{
			if (!(image is ImageSpriteSheet spriteSheet))
				throw new ArgumentException("Parameter must be an instance of ImageSpriteSheet", nameof(image));

			_spriteSheet = spriteSheet;
			Initialize();
		}

		protected override void Initialize()
		{
			_currentInterval = 0;
			_spriteSheet.SetFrame(0);
		}

		public override void Update(GameTime gameTime)
		{
			if (!this.IsActive)
				return;

			if (_currentInterval < SwitchInterval)
			{
				_currentInterval++;
				return;
			}

			_spriteSheet.IncrementFrame();
			_currentInterval = 0;
		}
	}
}
