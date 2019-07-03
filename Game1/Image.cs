using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
	public abstract class Image
	{		
		protected List<ImageEffect> _effects;
		protected Vector2 _origin;

		public bool IsActive { get; set; }

		public Vector2 Scale { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 PositionOffset { get; set; }
		public float Alpha { get; set; }
		public Color Color { get; set; }
		public int Index { get; set; }
		public float Rotation { get; set; }
		public bool Highlight { get; set; }

		public Vector2 OriginOffset { get; set; }
		public Rectangle Bounds { get; protected set; }
		public Rectangle? SourceRect { get; set; }

		public T AddEffect<T>(bool isActive) where T : ImageEffect
		{
			var currentEffect = _effects.FirstOrDefault(e => e.GetType() == typeof(T));
			if ((currentEffect != null) && isActive)
			{
				currentEffect.Start();
				return (T)currentEffect;
			}

			var effect = (T)Activator.CreateInstance(typeof(T), args:new object[] { this, isActive });
			_effects.Add(effect);
			return effect;
		}

		public void StartEffect(Type effectType)
		{
			foreach (var effect in _effects)
				if (effect.GetType() == effectType)
					effect.Start();
		}

		public void StopEffect(Type effectType)
		{
			foreach (var effect in _effects)
				if (effect.GetType() == effectType)
					effect.IsActive = false;
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
			this.PositionOffset = Vector2.Zero;
			this.Alpha = 1.0f;
			this.SourceRect = null;
			_effects = new List<ImageEffect>();
			this.Alignment = ImageAlignment.Centered;
			this.Color = Color.White;
			_origin = Vector2.Zero;
			this.OriginOffset = Vector2.Zero;
		}

		public virtual void Update(GameTime gameTime)
		{
			if (this.IsActive)
			{
				foreach (var effect in _effects)
					effect.Update(gameTime);
			}
		}

		public virtual void Draw(SpriteBatch spriteBatch, float? alphaBlend = null, Vector2? position = null, Vector2? positionOffset = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None)
		{
			if (this.IsActive)
				DrawActive(spriteBatch, alphaBlend, position, positionOffset, scale, spriteEffects);
		}

		public abstract void DrawActive(SpriteBatch spriteBatch, float? alphaBlend = null, Vector2? position = null, Vector2? positionOffset = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None);

		/*
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
		*/

		protected virtual void SetOrigin()
		{
			_origin = Vector2.Zero;

			var originalScale = this.Bounds.SizeVector();

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
