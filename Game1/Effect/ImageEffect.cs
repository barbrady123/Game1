using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.Effect
{
	public abstract class ImageEffect
	{
		protected Image _image;
		private bool _isActive;

		public bool IsIncreasing { get; set; }
		public float Speed { get; set; }
		
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnActiveChange?.Invoke(this, new EffectEventArgs { IsActive = _isActive });
				}
			}
		}

		public event EventHandler OnActiveChange;

		public ImageEffect(Image image, bool isActive = false)
		{
			_image = image;			
			this.IsActive = isActive;
			this.IsIncreasing = false;
			this.Speed = 1.0f;
		}

		public virtual void LoadContent(ref Image Image)
		{

		}

		public virtual void UnloadContent()
		{

		}

		public virtual void Update(GameTime gameTime)
		{

		}
	}
}
