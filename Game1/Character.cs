using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;
using Game1.Enum;

namespace Game1
{
	public class Character
	{
		private Vector2 _position;
		private Vector2 _previousPosition;
		protected Vector2 _latestMotion;

		public string Name { get; set; }
		public CharacterSex Sex { get; set; }
		public Cardinal Direction { get; set; }
		public float Speed { get; set; }
		public Vector2 Motion { get; set; }
		public string SpriteSheetName => this.Sex.ToString("g");

		public Vector2 Position
		{
			get { return _position; }
			set {
				if (_position != value)
				{
					_previousPosition = _position;
					_position = value;
				}
			}
		}

		public Character()
		{
			this.Direction = Cardinal.South;
			this.Speed = 150.0f;
		}

		public void RevertPosition()
		{
			if (_previousPosition != null)
				this.Position = _previousPosition;
		}

		public void LoadContent()
		{
		}

		public void UnloadContent()
		{

		}

		public virtual Vector2 UpdateMotion()
		{
			Vector2 motion = Vector2.Zero;

			if (InputManager.KeyDown(Keys.W))
			{
				motion.Y = -1;
				this.Direction = Cardinal.North;
			}
			if (InputManager.KeyDown(Keys.S))
			{
				motion.Y = 1;
				this.Direction = Cardinal.South;
			}
			if (InputManager.KeyDown(Keys.A))
			{
				motion.X = -1;
				this.Direction = Cardinal.West;
			}
			if (InputManager.KeyDown(Keys.D))
			{
				motion.X = 1;
				this.Direction = Cardinal.East;
			}

			return motion;
		}

		public void Update(GameTime gameTime)
		{
			Vector2 motion = UpdateMotion();

			if (motion != Vector2.Zero)
			{
				motion.Normalize();
				motion *= (this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
				this.Position += motion;
				this.Motion = motion;
			}

			_latestMotion = motion;
		}

		public void Draw(SpriteBatch spriteBatch)
		{

		}
	}
}
