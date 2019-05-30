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
using Game1.Screens.Menu.Character;
using Game1.Interface;
using Game1.Screens.Menu;

namespace Game1.Screens
{
	public class CharacterCreateScreen : Screen
	{
		private ActivationManager _activation = new ActivationManager();
		private Character _newChar;
		private Vector2 _characterViewPosition = new Vector2(450.0f, 400.0f);
		private readonly DialogBox _dialogBox;

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
			_characterViewBack = new ImageTexture("Background/black", true) {
				Alignment = ImageAlignment.Centered,
				Alpha = 0.6f,
				Scale = new Vector2(320.0f, 320.0f),
				Position = _characterViewPosition
			};
			_characterView = new ImageTexture(this.CharacterPreviewImage(_newChar.Sex), true)	{
				Alignment = ImageAlignment.Centered,
				Scale = new Vector2(5.0f, 5.0f),
				Position = _characterViewPosition
			};

			// Menu
			_activation.Add(_menuCharacter = new CharacterNewCompositeMenu(new Rectangle(650, 200, 200, 200)));
			_menuCharacter.OnSexItemChange += _menuCharacter_OnSexItemChange;
			_menuCharacter.OnReadyDisable += _menuCharacter_OnReadyDisable;
			_menuCharacter.OnUserNotify += _menuCharacter_OnUserNotify;

			// Dialog
			_activation.Add(_dialogBox = new DialogBox(null, DialogButton.Ok, new Rectangle(600, 500, 400, 200), null));
			_dialogBox.OnButtonClick += _dialogBox_OnButtonClick;
			_dialogBox.OnReadyDisable += _dialogBox_OnButtonClick;

			_activation.Activate(_menuCharacter);
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_titleText.LoadContent();
			_characterViewBack.LoadContent();
			_characterView.LoadContent();
			_menuCharacter.LoadContent();
			_dialogBox.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_titleText.UnloadContent();
			_characterViewBack.UnloadContent();
			_characterView.UnloadContent();
			_menuCharacter.UnloadContent();
			_dialogBox.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_characterView.Update(gameTime);
			_menuCharacter.Update(gameTime, processInput);
			_dialogBox.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			_titleText.Draw(spriteBatch);
			_characterViewBack.Draw(spriteBatch);
			_characterView.Draw(spriteBatch);
			_menuCharacter.Draw(spriteBatch);
			_dialogBox.Draw(spriteBatch);
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
					_newChar.Position = new Vector2(Game1.TileSize / 2, Game1.TileSize / 2);
					IOManager.ObjectToFile(Game1.PlayerFile, _newChar);
					// TODO: Eventually we need to handle some kind of identifier of this new player to the parent, when we have multiple player/world files...
					ReadyScreenUnload(this, new ScreenEventArgs("game", this.GetType().Name, null));
					break;
				case "back" :
					ReadyScreenUnload(this, new ScreenEventArgs("back", this.GetType().Name, null));
					break;
			}
		}

		private void _menuCharacter_OnUserNotify(object sender, EventArgs e)
		{
			var args = (UserNotifyArgs)e;
			_dialogBox.Text = args.Text;
			_dialogBox.Duration = 300;
			_activation.Activate(_dialogBox);
		}

		private void _dialogBox_OnButtonClick(object sender, EventArgs e)
		{
			_activation.Deactivate(_dialogBox);
		}
	}
}
