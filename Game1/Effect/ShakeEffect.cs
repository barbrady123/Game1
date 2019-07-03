using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class ShakeEffect : ImageEffect
	{
		private const int FrameSwitchCount = 2;
		private const int MaxShakes = 10;

		private int _currentFrameCount;
		private int _shakeCount;

		public ShakeEffect(Image image, bool isActive = false) : base(image, isActive)
		{	
			Initialize();
		}

		protected override void Initialize()
		{
			_currentFrameCount = 0;
			_shakeCount = 0;
		}

		protected override void ResetImage()
		{
			_image.PositionOffset = Vector2.Zero;
		}
			
		protected override void ActiveChange(bool isActive)
		{
			base.ActiveChange(isActive);
		}

		public override void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				if (_currentFrameCount < FrameSwitchCount)
				{
					_currentFrameCount++;
					return;
				}

				if (_shakeCount >= MaxShakes)
				{
					this.IsActive = false;
					return;
				}

				_image.PositionOffset = new Vector2(GameRandom.Next(-1, 1), GameRandom.Next(-1, 1));
				_currentFrameCount = 0;
				_shakeCount++;
			}
		}
	}
}
