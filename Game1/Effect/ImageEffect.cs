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
		protected readonly object _lock = new object();
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
					ActiveChange(_isActive);
					if (!_isActive)
						ResetImage();
				}
			}
		}

		protected virtual void ResetImage() { }

		public Image Image
		{
			get { return _image; }
			set { _image = value; }
		}


		public ImageEffect(bool isActive = false)
		{
			_isActive = isActive;
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

		public void Start()
		{
			if (this.IsActive)
				return;

			this.IsActive = true;
			Initialize();
		}

		protected virtual void Initialize() { }

		#region Events
		protected virtual void ActiveChange(bool isActive)
		{
			_onActiveChange?.Invoke(this, new EffectEventArgs { IsActive = isActive });
		}

		private event EventHandler _onActiveChange;
		public event EventHandler OnActiveChange
		{
			add		{ lock(_lock) { _onActiveChange -= value; _onActiveChange += value; } }
			remove	{ lock(_lock) { _onActiveChange -= value; } }
		}
		#endregion
	}
}
