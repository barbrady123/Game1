﻿using System;
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
		private float _maxRotation;
		private Vector2 _initialOffset;
		private Vector2 _animationOffset = new Vector2(-0.5f, -0.8f);

		public UseItemWestEffect(bool isActive = false) : base(isActive)
		{			
			_maxRotation = Convert.ToSingle(-Math.PI / 3);	// 60 degrees
			this.Speed = 3.0f;
			Initialize();
		}

		public event EventHandler OnFullyExtended;

		protected override void Initialize()
		{
			_movingForward = true;
		}

		protected override void ResetImage()
		{
			_image.PositionOffset = _initialOffset;
			_image.Rotation = 0.0f;
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
				if (_movingForward)
				{
					_image.PositionOffset += _animationOffset;
					_image.Rotation -= this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation <= _maxRotation)
					{
						_image.Rotation = _maxRotation;
						_movingForward = false;
						OnFullyExtended?.Invoke(this, null);
					}
				}
				else
				{
					_image.PositionOffset -= _animationOffset;
					_image.Rotation += this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Rotation >= 0.0f)
						this.IsActive = false;
				}
			}
		}
	}
}
