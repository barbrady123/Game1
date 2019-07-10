using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Game1.Effect;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	public class WorldCharacter : WorldEntity
	{
		protected Vector2 _position;

		public Size BoundsExpansionSize { get; protected set; }
		public Func<Vector2, Vector2> UpdateMotion { get; protected set; }

		public Character Character { get; set; }
		public ImageSpriteSheet SpriteSheet { get; set; }
		public Vector2 PreviousPosition { get; set; }
		public Vector2 Motion { get; set; }
		public Cardinal Direction { get; set; }

		public override bool IsSolid => true;
		public bool Moved => this.PreviousPosition != this.Position;
		public override string TooltipText => this.Character.Name;

		public override Vector2 Position
		{
			get { return _position; }
			set
			{
				if (_position == value)
					return;

				_position = value;
				RecalculatePosition(Vector2.Zero);
			}
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
			this.Bounds = _position.ExpandToRectangleCentered(this.BoundsExpansionSize.Width, this.BoundsExpansionSize.Height);
			this.Direction = Util.DirectionFromVector(Motion, this.Direction);
		}

		public WorldCharacter(Character character, ImageSpriteSheet spriteSheet, Vector2 position)
		{
			this.Character = character;
			this.SpriteSheet = spriteSheet;
			this.BoundsExpansionSize = MetaManager.GetCreatureSize(character.CreatureType) / 2;
			this.Direction = Cardinal.South;
			this.Position = position;
			this.Motion = Vector2.Zero;
			RecalculatePosition(Vector2.Zero);
			this.UpdateMotion = AIManager.GetMotionMethod(character.Motion);
		}

		public virtual void Update(GameTime gameTime)
		{
			this.PreviousPosition = _position;
			var motion = UpdateMotion.Invoke(this.Motion);

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
