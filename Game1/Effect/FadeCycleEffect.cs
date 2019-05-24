using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class FadeCycleEffect : ImageEffect
	{
		public FadeCycleEffect(Image img, bool isActive = false) : base(img, isActive)
		{

		}

		public override void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				if (this.IsIncreasing)
				{
					_image.Alpha += this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Alpha >= 1.0f)
					{
						_image.Alpha = 1.0f;
						this.IsIncreasing = false;
					}
				}
				else
				{
					_image.Alpha -= this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (_image.Alpha <= 0.0f)
					{
						_image.Alpha = 0.0f;
						this.IsIncreasing = true;
					}
				}
			}
		}
	}
}
