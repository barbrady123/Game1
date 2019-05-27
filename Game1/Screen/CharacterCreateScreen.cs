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
		public CharacterNewMenu _menuCharacter;
		public TextInput _nameEdit;
		public SexMenu _menuSex;

		private string CharacterPreviewImage(CharacterSex sex) => $"Character/Preview/{sex.ToString("g")}";

		public CharacterCreateScreen(Rectangle bounds): base(bounds, "brick")
		{
			_newChar = new Character() { Sex = CharacterSex.Male };

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
				Position = new Vector2(300.0f, 270.0f)
			};
			_characterView = new ImageTexture(this.CharacterPreviewImage(_newChar.Sex), true)	{
				Alignment = ImageAlignment.Centered,
				Scale = new Vector2(5.0f, 5.0f),
				Position = new Vector2(300.0f, 270.0f)
			};
			// Menu
			_menuCharacter = new CharacterNewMenu(new Rectangle(600, 200, 200, 200));
			_menuCharacter.OnItemSelect += _menuCharacter_OnItemSelect;
			_nameEdit = new TextInput(275, _newChar.Name, 12, false) { Position = new Vector2(770, 257) };
			_nameEdit.OnReadyDisable += _nameEdit_OnReadyDisable;
			_menuSex = new SexMenu(new Rectangle(690, 250, 300, 120)) { IsActive = false };
			_menuSex.OnCurrentItemChange += _menuSex_OnCurrentItemChange;
			_menuSex.OnItemSelect += _menuSex_OnItemSelect;
			_menuSex.OnReadyDisable += _menuSex_OnReadyDisable;
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
			_menuSex.SetById(_newChar.Sex.ToString("g").ToLower());
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

		private void _nameEdit_OnReadyDisable(object sender, EventArgs e)
		{
			var key = ((TextInputEventArgs)e).Key;
			if (key == Keys.Enter)
			{
				_newChar.Name = _nameEdit.Text;
			}
			else if (key == Keys.Escape)
			{
				_nameEdit.Text = _newChar.Name;
			}

			_nameEdit.IsActive = false;
			_menuCharacter.IsActive = true;
		}

		private void _menuCharacter_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Item)
			{
				case "name":	_menuCharacter.IsActive = false;
								_nameEdit.IsActive = true;
								_nameEdit.DelayInput(1);
								break;
				case "sex":		_menuCharacter.IsActive = false;
								_menuSex.IsActive = true;
								_menuSex.DelayInput(1);
								break;
			}
		}

		private void _menuSex_OnCurrentItemChange(object sender, EventArgs e)
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

		private void _menuSex_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Item)
			{
				case "female" : _newChar.Sex = CharacterSex.Female;	break;
				case "male" :	_newChar.Sex = CharacterSex.Male;	break;
			}

			_characterView.SwapTexture(this.CharacterPreviewImage(_newChar.Sex));
			_menuSex.IsActive = false;
			_menuCharacter.IsActive = true;
		}

		private void _menuSex_OnReadyDisable(object sender, EventArgs e)
		{
			_characterView.SwapTexture(this.CharacterPreviewImage(_newChar.Sex));
			_menuSex.SetById(_newChar.Sex.ToString("g").ToLower());
			_menuSex.IsActive = false;
			_menuCharacter.IsActive = true;
		}
	}
}
