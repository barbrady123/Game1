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
	public abstract class Image : IActivatable
	{		
		protected List<ImageEffect> _effects;
		protected ContentManager _content;
		protected Vector2 _origin;
		protected Rectangle _sourceRect;

		public bool IsActive { get; set; }

		public Vector2 Scale { get; set; }
		public Vector2 Position{ get; set; }
		public float Alpha { get; set; }
		public Color Color { get; set; }
		public int Index { get; set; }

		public Rectangle SourceRect
		{ 
			get { return _sourceRect; }
			set	{
				_sourceRect = value;
				SetOrigin();
			}
		}

		public T AddEffect<T>(T effect) where T: ImageEffect
		{
			effect.Image = this;
			_effects.Add(effect);
			return effect;
		}

		public void ClearEffects()
		{
			_effects.Clear();
		}

		/// <summary>
		/// Point in the texture that the Position points to...
		/// </summary>
		public ImageAlignment Alignment { get; set; }

		public Image(bool isActive = false)
		{
			this.IsActive = isActive;
			this.Scale = Vector2.One;
			this.Position = Vector2.Zero;
			this.Alpha = 1.0f;
			this.SourceRect = Rectangle.Empty;
			_effects = new List<ImageEffect>();
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
				foreach (var effect in _effects)
					effect.Update(gameTime);
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch, float? alphaBlend = null, Vector2? position = null)
		{
			if (this.IsActive)
				DrawActive(spriteBatch, alphaBlend, position);
		}

		public abstract void DrawActive(SpriteBatch spriteBatch, float? alphaBlend = null, Vector2? position = null);

		protected virtual void SetOrigin()
		{
			_origin = Vector2.Zero;
			var originalScale = this.SourceRect.SizeVector();

			switch (this.Alignment.Horizatal)
			{
				case (HorizontalAlignment.Center) : _origin.X += originalScale.X / 2;	break;
				case (HorizontalAlignment.Right):	_origin.X += originalScale.X;		break;
			}

			switch (this.Alignment.Vertical)
			{
				case (VerticalAlignment.Center) :	_origin.Y += originalScale.Y / 2;	break;
				case (VerticalAlignment.Bottom):	_origin.Y += originalScale.Y;		break;
			}
		}
	}
}
