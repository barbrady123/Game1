using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Effect;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	public class WorldCharacter : WorldEntity
	{
		private Size _boundsExpansionSize;

		protected Vector2 _position;

		public Character Character { get; set; }
		public ImageSpriteSheet SpriteSheet { get; set; }
		public Vector2 PreviousPosition { get; set; }
		public Vector2 Motion { get; set; }
		public Cardinal Direction { get; set; }

		public override bool IsSolid => true;
		public override Vector2 Position
		{
			get { return _position; }
			set { throw new NotSupportedException("Use Move() to change position"); }
		}

		public virtual float MovementSpeed => this.Character.MovementSpeed;

		public void Move(Vector2 movement)
		{
			if (movement == Vector2.Zero)
				return;

			RecalculatePosition(movement);
		}

		private void RecalculatePosition(Vector2 movement)
		{
			_position += movement;
			this.Bounds = _position.ExpandToRectangleCentered(_boundsExpansionSize.Width, _boundsExpansionSize.Height);
			this.Direction = Util.DirectionFromVector(Motion, this.Direction);
		}

		public Func<Vector2, Vector2> UpdateMotion { get; private set; }

		public WorldCharacter(Character character, ImageSpriteSheet spriteSheet, Vector2 position)
		{
			this.Character = character;
			this.SpriteSheet = spriteSheet;
			_boundsExpansionSize = MetaManager.GetCreatureSize(character.CreatureType) / 2;
			this.Direction = Cardinal.South;
			this.Position = position;
			this.Motion = Vector2.Zero;
			RecalculatePosition(Vector2.Zero);
			this.UpdateMotion = AIManager.GetMotionMethod(character.Motion);
		}

		public virtual void Update(GameTime gameTime)
		{
			this.PreviousPosition = _position;
			var motion = UpdateMotion.Invoke(this.PreviousPosition);

			if (motion != Vector2.Zero)
			{
				motion.Normalize();
				motion *= (this.MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
				this.SpriteSheet.StartEffect(typeof(SpriteSheetEffect));
			}
			else
			{
				this.SpriteSheet.StopEffect(typeof(SpriteSheetEffect));
			}

			this.Motion = motion;
			this.SpriteSheet.UpdateDirection(this.Direction);
			this.SpriteSheet.Update(gameTime);			
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			this.SpriteSheet.Draw(spriteBatch, position: this.Position + cameraOffset, highlight: this.IsHighlighted);
		}

		public void SetImageEffect<T>() where T: ImageEffect
		{
			this.SpriteSheet.AddEffect<T>(true);
		}
	}
}
