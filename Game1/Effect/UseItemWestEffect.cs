using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class UseItemWestEffect : ImageEffect
	{
		private bool _movingForward;
		private readonly float _maxRotation;

		public UseItemWestEffect(Image image, bool isActive = false) : base(image, isActive)
		{			
			_maxRotation = Convert.ToSingle(-Math.PI / 3);	// 60 degrees
			this.Speed = 3.0f;
			Initialize();
		}

		private event EventHandler _onFullyExtended;
		public event EventHandler OnFullyExtended
		{
			add		{ lock(_lock) { _onFullyExtended -= value; _onFullyExtended += value; } }
			remove	{ lock(_lock) { _onFullyExtended -= value; } }
		}

		protected override void Initialize()
		{
			_movingForward = true;
		}

		protected override void ResetImage()
		{
			_image.Rotation = 0.0f;
		}

		protected override void ActiveChange(bool isActive)
		{
			base.ActiveChange(isActive);
		}

		public override void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				if (_movingForward)
				{
					_image.Rotation -= this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation <= _maxRotation)
					{
						_image.Rotation = _maxRotation;
						_movingForward = false;
						_onFullyExtended?.Invoke(this, null);
					}
				}
				else
				{
					_image.Rotation += this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation >= 0.0f)
						this.IsActive = false;
				}
			}
		}
	}
}
