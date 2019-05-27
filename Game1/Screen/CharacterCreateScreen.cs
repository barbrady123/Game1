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
using Game1.Interface;
using Game1.Screen.Menu;

namespace Game1.Screen
{
	public class CharacterCreateScreen : GameScreen
	{
		private Character _newChar;
		public ImageText _titleText;
		public ImageTexture _characterViewBack;
		public ImageTexture _characterView;
		public CharacterNewCompositeMenu _menuCharacter;

		private string CharacterPreviewImage(CharacterSex sex) => $"Character/Preview/{sex.ToString("g")}";

		public CharacterCreateScreen(Rectangle bounds): base(bounds, "brick")
		{
			_newChar = new Character();

			// Title...
			_titleText = new ImageText("Create New Character", true)	{
				Scale = new Vector2(1.5f, 1.5f),
				Position = new Vector2(this.Bounds.Width / 2, 50.0f)
			};
			// Character View...
			_characterViewBack = new ImageTexture("Background/black3", true) {
				Alignment = ImageAlignment.Centered,
				Alpha = 0.6f,
				Scale = new Vector2(5.0f, 5.0f),
				Position = new Vector2(300.0f, 270.0f)
			};
			_characterView = new ImageTexture(this.CharacterPreviewImage(_newChar.Sex), true)	{
				Alignment = ImageAlignment.Centered,
				Scale = new Vector2(5.0f, 5.0f),
				Position = new Vector2(300.0f, 270.0f)
			};

			// Menu
			_menuCharacter = new CharacterNewCompositeMenu(new Rectangle(650, 200, 200, 200));
			_menuCharacter.OnSexItemChange += _menuCharacter_OnSexItemChange;
			_menuCharacter.OnReadyDisable += _menuCharacter_OnReadyDisable;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_titleText.LoadContent();
			_characterViewBack.LoadContent();
			_characterView.LoadContent();
			_menuCharacter.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_titleText.UnloadContent();
			_characterViewBack.UnloadContent();
			_characterView.UnloadContent();
			_menuCharacter.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_characterView.Update(gameTime);
			_menuCharacter.Update(gameTime, processInput);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			_titleText.Draw(spriteBatch);
			_characterViewBack.Draw(spriteBatch);
			_characterView.Draw(spriteBatch);
			_menuCharacter.Draw(spriteBatch);
		}

		private void _menuCharacter_OnSexItemChange(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Item)
			{
				case "female" :
					_characterView.SwapTexture(this.CharacterPreviewImage(CharacterSex.Female));
					break;
				case "male" :
					_characterView.SwapTexture(this.CharacterPreviewImage(CharacterSex.Male));
					break;
			}
		}

		private void _menuCharacter_OnReadyDisable(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Type)
			{
				case "continue" :
					_newChar.Name = _menuCharacter.CharacterName;
					_newChar.Sex = _menuCharacter.CharacterSex;
					// start the game...
					break;
				case "back" :
					ReadyScreenUnload(this, new ScreenEventArgs("back", this.GetType().Name, null));
					break;
			}
		}
	}
}
