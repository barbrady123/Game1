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

namespace Game1.Screen
{
	public class CharacterCreateScreen : GameScreen
	{
		private Character _newChar;
		public ImageText _titleText;
		public ImageTexture _characterViewBack;
		public ImageTexture _characterView;
		public CharacterNewMenu _menuCharacter;
		public TextInput _nameEdit;
		public SexMenu _menuSex;

		private string CharacterPreviewImage => $"Character/Preview/{_newChar.Sex.ToString("g")}";

		public CharacterCreateScreen(Rectangle bounds): base(bounds, "brick")
		{
			_newChar = new Character();

			// TODO: Figure out a better way to configure this layout...
			// Title...
			_titleText = new ImageText("Create New Character", true)	{
				Scale = new Vector2(1.5f, 1.5f),
				Position = new Vector2(this.Bounds.Width / 2, 50.0f)
			};
			// Character View...
			_characterViewBack = new ImageTexture("Background/black3", true) {
				Alignment = ImageAlignment.Centered,
				Alpha = 0.4f,
				Scale = new Vector2(5.0f, 5.0f),
				Position = new Vector2(300.0f, 400.0f)
			};
			_characterView = new ImageTexture(this.CharacterPreviewImage, true)	{
				Alignment = ImageAlignment.Centered,
				Scale = new Vector2(5.0f, 5.0f),
				Position = new Vector2(300.0f, 400.0f)
			};
			// Menu
			_menuCharacter = new CharacterNewMenu(new Rectangle(600, 200, 200, 200));
			_menuCharacter.OnNameSelect += _menuCharacter_OnNameSelect;
			_menuCharacter.OnSexSelect += _menuCharacter_OnSexSelect;
			_nameEdit = new TextInput(200, _newChar.Name, 12, false) { Position = new Vector2(770, 257) };
			_nameEdit.OnEnterPressed += _nameEdit_OnEnterPressed;
			_nameEdit.OnEscapePressed += _nameEdit_OnEscapePressed;
			_menuSex = new SexMenu(new Rectangle(690, 250, 300, 120)) { IsActive = false };
			_menuSex.OnFemaleSelect += _menuSex_OnFemaleSelect;
			_menuSex.OnMaleSelect += _menuSex_OnMaleSelect;
			_menuSex.OnReadyMenuDisable += _menuSex_OnReadyMenuDisable;
		}

		private void _menuSex_OnReadyMenuDisable(object sender, EventArgs e)
		{
			_menuSex.IsActive = false;
			_menuCharacter.IsActive = true;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_titleText.LoadContent();
			_characterViewBack.LoadContent();
			_characterView.LoadContent();
			_menuCharacter.LoadContent();
			_nameEdit.LoadContent();
			_menuSex.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_titleText.UnloadContent();
			_characterViewBack.UnloadContent();
			_characterView.UnloadContent();
			_menuCharacter.UnloadContent();
			_nameEdit.UnloadContent();
			_menuSex.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_characterView.Update(gameTime);
			_menuCharacter.Update(gameTime, processInput);
			_nameEdit.Update(gameTime, processInput);
			_menuSex.Update(gameTime, processInput);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			_titleText.Draw(spriteBatch);
			_characterViewBack.Draw(spriteBatch);
			_characterView.Draw(spriteBatch);
			_menuCharacter.Draw(spriteBatch);
			_nameEdit.Draw(spriteBatch);
			_menuSex.Draw(spriteBatch);
		}

		private void _menuCharacter_OnNameSelect(object sender, EventArgs e)
		{
			_menuCharacter.IsActive = false;
			_nameEdit.IsActive = true;
			_nameEdit.DelayInput(1);
		}

		private void _menuCharacter_OnSexSelect(object sender, EventArgs e)
		{
			_menuCharacter.IsActive = false;
			_menuSex.IsActive = true;
			_menuSex.DelayInput(1);
		}

		private void _menuSex_OnFemaleSelect(object sender, EventArgs e)
		{
			if (_newChar.Sex != CharacterSex.Female)
			{
				 _newChar.Sex = CharacterSex.Female;
				 _characterView.SwapTexture(this.CharacterPreviewImage);
			}
		}

		private void _menuSex_OnMaleSelect(object sender, EventArgs e)
		{
			if (_newChar.Sex != CharacterSex.Male)
			{
				_newChar.Sex = CharacterSex.Male;
				_characterView.SwapTexture(this.CharacterPreviewImage);
			}
		}

		private void _nameEdit_OnEnterPressed(object sender, EventArgs e)
		{
			_newChar.Name = _nameEdit.Text;
			_nameEdit.IsActive = false;
			_menuCharacter.IsActive = true;
		}

		private void _nameEdit_OnEscapePressed(object sender, EventArgs e)
		{
			_nameEdit.Text = _newChar.Name;
			_nameEdit.IsActive = false;
			_menuCharacter.IsActive = true;
		}
	}
}
