using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game1.Effect;
using Game1.Enum;

namespace Game1
{
	public abstract class ImageBase
	{
		protected ContentManager _content;
		protected Vector2 _origin;

		public bool IsActive { get; set; }

		public Vector2 Scale { get; set; }
		public Vector2 Position{ get; set; }
		public float Alpha { get; set; }
		public Rectangle SourceRect { get; set; }
		public Rectangle DrawArea { get; set; }
		public Color Color { get; set; }

		public List<ImageEffect> Effects { get; set; }

		/// <summary>
		/// Point in the texture that the Position points to...
		/// </summary>
		public ImageAlignment Alignment { get; set; }

		public ImageBase(bool isActive = false)
		{
			this.IsActive = isActive;
			this.Scale = Vector2.One;
			this.Position = Vector2.Zero;
			this.Alpha = 1.0f;
			this.SourceRect = Rectangle.Empty;
			this.Effects = new List<ImageEffect>();
			this.Alignment = ImageAlignment.Centered;
			this.Color = Color.White;
			_origin = Vector2.Zero;
		}

		public virtual void LoadContent()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
		}

		public virtual void UnloadContent()
		{
			if (_content != null)
				_content.Unload();
		}

		public virtual void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				foreach (var effect in this.Effects)
					effect.Update(gameTime);
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (this.IsActive)
				DrawActive(spriteBatch);
		}

		public abstract void DrawActive(SpriteBatch spriteBatch);

		protected virtual void SetOrigin()
		{
			_origin = Vector2.Zero;

			switch (this.Alignment.Horizatal)
			{
				case (HorizontalAlignment.Center) : _origin.X += this.SourceRect.Width / 2;	break;
				case (HorizontalAlignment.Right):	_origin.X += this.SourceRect.Width;		break;
			}

			switch (this.Alignment.Vertical)
			{
				case (VerticalAlignment.Center) :	_origin.Y += this.SourceRect.Height / 2;	break;
				case (VerticalAlignment.Bottom):	_origin.Y += this.SourceRect.Height;		break;
			}
		}
	}
}
