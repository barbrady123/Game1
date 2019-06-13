using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class RotateEffect : ImageEffect
	{
		private bool _isIncreasing;
		private float _maxRotation;

		public RotateEffect(float maxRotation, bool isActive = false) : base(isActive)
		{
			_isIncreasing = maxRotation >= 0;
			_maxRotation = maxRotation;
			this.Speed = 8.0f;
		}

		public override void Update(GameTime gameTime)
		{
			if (_maxRotation >= 0)
				UpdateClockwise(gameTime);
			else
				UpdateCounterClockwise(gameTime);
		}

		public void UpdateClockwise(GameTime gameTime)
		{
			if (this.IsActive)
			{
				if (_isIncreasing)
				{
					_image.Rotation += this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation >= _maxRotation)
					{
						_image.Rotation = _maxRotation;
						_isIncreasing = false;
					}
				}
				else
				{
					_image.Rotation -= this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation <= 0.0f)
					{
						_image.Rotation = 0.0f;
						this.IsActive = false;
					}
				}
			}
		}

		private void UpdateCounterClockwise(GameTime gameTime)
		{
			if (this.IsActive)
			{
				if (!_isIncreasing)
				{
					_image.Rotation -= this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation <= _maxRotation)
					{
						_image.Rotation = _maxRotation;
						_isIncreasing = true;
					}
				}
				else
				{
					_image.Rotation += this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation >= 0.0f)
					{
						_image.Rotation = 0.0f;
						this.IsActive = false;
					}
				}
			}
		}
	}
}
