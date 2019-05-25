using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Screen.Menu.Character;

namespace Game1.Screen
{
	public class CharacterCreateScreen : GameScreen
	{
		private Character _newChar;
		public ImageText _titleText;
		public ImageTexture _characterViewBack;
		public ImageTexture _characterView;
		public CharacterNewMenu _menu;

		public CharacterCreateScreen(Rectangle bounds): base(bounds, "brick")
		{
			_newChar = new Character();

			// TODO: Figure out a better way to configure this layout...
			// Title...
			_titleText = new ImageText("Create New Character", true);
			_titleText.Scale = new Vector2(1.5f, 1.5f);
			_titleText.Position = new Vector2(this.Bounds.Width / 2, 50.0f);
			// Character View...
			_characterViewBack = new ImageTexture("Background/black3", true);
			_characterViewBack.Alignment = ImageAlignment.Centered;
			_characterViewBack.Alpha = 0.4f;
			_characterViewBack.Scale = new Vector2(5.0f, 5.0f);
			_characterViewBack.Position = new Vector2(300.0f, 400.0f);
			_characterView = new ImageTexture($"Character/Preview/{_newChar.Sex.ToString("g")}", true);	// This will have to move elsewhere to be dynamic on menu change
			_characterView.Alignment = ImageAlignment.Centered;
			_characterView.Scale = new Vector2(5.0f, 5.0f);
			_characterView.Position = new Vector2(300.0f, 400.0f);
			// Menu
			_menu = new CharacterNewMenu(new Rectangle(600, 200, 400, 400));
		}
		
		public override void LoadContent()
		{
			base.LoadContent();
			_titleText.LoadContent();
			_characterViewBack.LoadContent();
			_characterView.LoadContent();
			_menu.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_titleText.UnloadContent();
			_characterViewBack.UnloadContent();
			_characterView.UnloadContent();
			_menu.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			if (processInput)
			{
				if (InputManager.Instance.KeyPressed(Keys.Enter))
				{

				}
			}

			base.Update(gameTime, processInput);
			_characterView.Update(gameTime);
			_menu.Update(gameTime, true);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			_titleText.Draw(spriteBatch);
			_characterViewBack.Draw(spriteBatch);
			_characterView.Draw(spriteBatch);
			_menu.Draw(spriteBatch);
		}
	}
}
