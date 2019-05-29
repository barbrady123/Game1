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
		private ContentManager _content;
		private ImageTexture _spriteSheet;

		public string Name { get; set; }
		public CharacterSex Sex { get; set; }
		public Vector2 Position { get; set; }
		public Cardinal Direction { get; set; }
		public float Speed { get; set; }

		public Character()
		{
			this.Direction = Cardinal.South;
			this.Speed = 1.0f;
		}

		public void LoadContent()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			_spriteSheet = new ImageTexture(_content.Load<Texture2D>($"{Game1.SpriteSheetRoot}\\{this.Sex.ToString("g")}")) { IsActive = true };
			_spriteSheet.AddEffect(new SpriteSheetEffect(true));
			_spriteSheet.LoadContent();
		}

		public void UnloadContent()
		{

		}

		public void Update(GameTime gameTime)
		{
			Vector2 motion = Vector2.Zero;

			if (InputManager.KeyPressed(Keys.W))
			{
				motion.Y = -1;
				this.Direction = Cardinal.North;
			}
			if (InputManager.KeyPressed(Keys.S))
			{
				motion.Y = 1;
				this.Direction = Cardinal.South;
			}
			if (InputManager.KeyPressed(Keys.A))
			{
				motion.X = -1;
				this.Direction = Cardinal.West;
			}
			if (InputManager.KeyPressed(Keys.D))
			{
				motion.X = 1;
				this.Direction = Cardinal.East;
			}

			if (motion != Vector2.Zero)
			{
				motion.Normalize();
				motion *= (this.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
				this.Position += motion;
			}

			_spriteSheet.SourceRect = new Rectangle(_spriteSheet.SourceRect.X, (int)this.Direction * Game1.TileSize, Game1.TileSize, Game1.TileSize);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			//this.Position = 
			_spriteSheet.Draw(spriteBatch);
		}
	}
}
