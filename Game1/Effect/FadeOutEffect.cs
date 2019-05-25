using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class FadeOutEffect : ImageEffect
	{
		public FadeOutEffect(ImageBase img, bool isActive = false) : base(img, isActive)
		{

		}

		public override void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				_image.Alpha -= this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (_image.Alpha <= 0.0f)
				{
					_image.Alpha = 0.0f;
					this.IsActive = false;
				}
			}
		}
	}
}
