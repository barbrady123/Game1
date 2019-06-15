using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class UseItemEastEffect : ImageEffect
	{
		private bool _movingForward;
		private float _maxRotation;

		public UseItemEastEffect(bool isActive = false) : base(isActive)
		{			
			_maxRotation = Convert.ToSingle(Math.PI / 4);	// 45 degrees
			this.Speed = 10.0f;
			Initialize();
		}

		protected override void Initialize()
		{
			_movingForward = true;
		}

		public override void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				if (_movingForward)
				{
					_image.Rotation += this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation >= _maxRotation)
					{
						_image.Rotation = _maxRotation;
						_movingForward = false;
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
	}
}
