using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class JiggleEffect : ImageEffect
	{
		private const int FrameSwitchCount = 2;
		private const int MaxJiggles = 10;

		private Vector2 _initialOffset;
		private int _currentFrameCount;
		private int _jiggleCount;

		public JiggleEffect(bool isActive = false) : base(isActive)
		{	
			Initialize();
		}

		protected override void Initialize()
		{
			_currentFrameCount = 0;
			_jiggleCount = 0;
		}

		protected override void ResetImage()
		{
			_image.PositionOffset = _initialOffset;
		}
			
		protected override void ActiveChange(bool isActive)
		{
			base.ActiveChange(isActive);
			if (isActive)
				_initialOffset = _image.PositionOffset;
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

				if (_jiggleCount >= MaxJiggles)
				{
					this.IsActive = false;
					return;
				}

				_image.PositionOffset = _initialOffset + new Vector2(GameRandom.Next(-1, 1), GameRandom.Next(-1, 1));
				_currentFrameCount = 0;
				_jiggleCount++;
			}
		}
	}
}
