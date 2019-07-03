using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.Effect
{
	public class ZoomCycleEffect : ImageEffect
	{
		private readonly Vector2 _minScale;
		private readonly Vector2 _maxScale;
		private readonly Vector2 _deltaScale;

		public ZoomCycleEffect(Image image, bool isActive = false) : base(image, isActive)
		{
			// TODO: Make these configurable in the future...
			_minScale = new Vector2(4.0f, 4.0f);
			_maxScale = new Vector2(6.0f, 6.0f);
			_deltaScale = _maxScale - _minScale;
		}

		public override void Update(GameTime gameTime)
		{
			if (this.IsActive)
				_image.Scale = _minScale + _deltaScale * (float)Math.Abs(Math.Sin(gameTime.TotalGameTime.TotalSeconds));
		}
	}
}
