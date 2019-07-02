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
using Game1.Menus.Character;
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Menus;

namespace Game1.Screens
{
	/// <summary>
	/// TODO: Remove _dialog from here and use the Component stuff!!
	/// </summary>
	public class CharacterCreateScreen : Component
	{
		public const CharacterSex DefaultSex = CharacterSex.Female;
		private Vector2 _characterViewPosition = new Vector2(450.0f, 400.0f);
		private readonly Dialog _dialog;

		public ImageText _titleText;
		public ImageTexture _characterViewBack;
		public ImageTexture _characterView;
		public CharacterNewCompositeMenu _menuCharacter;

		private Dictionary<CharacterSex, ImageTexture> _preview;

		public CharacterCreateScreen(Rectangle bounds): base(bounds, background: "brick")
		{
			// Title...
			_titleText = new ImageText("Create New Character", true)	{
				Scale = new Vector2(1.5f, 1.5f),
				Position = new Vector2(this.Bounds.Width / 2, 50.0f)
			};

			// Character View...
			_characterViewBack = AssetManager.GetBackground("black");
			_characterViewBack.Alpha = 0.6f;
			_characterViewBack.Scale = new Vector2(320.0f, 320.0f);
			_characterViewBack.Position = _characterViewPosition;

			_preview = new Dictionary<CharacterSex, ImageTexture>();
			foreach (CharacterSex sex in (CharacterSex[])System.Enum.GetValues(typeof(CharacterSex)))
			{
				_preview[sex] = new ImageTexture(AssetManager.GetScreenTexture($"Character/Preview/{sex.ToString("g")}"), true) {
					Alignment = ImageAlignment.Centered,
					Scale = new Vector2(5.0f, 5.0f),
					Position = _characterViewPosition
				};
				_preview[sex].LoadContent();
			}
			_characterView = _preview[DefaultSex];
				
			// Menu
			_activator.Register(_menuCharacter = new CharacterNewCompositeMenu(new Rectangle(650, 200, 200, 200)) { CharacterName = "", CharacterSex = DefaultSex }, true, "active");
			_menuCharacter.OnSexItemChange += _menuCharacter_OnSexItemChange;
			_menuCharacter.OnReadyDisable += _menuCharacter_OnReadyDisable;
			_menuCharacter.OnUserNotify += _menuCharacter_OnUserNotify;

			// Dialog
			_activator.Register(_dialog = new Dialog(null, DialogButton.Ok, new Rectangle(600, 500, 400, 200), null), false, "active");
			_dialog.OnItemSelect += _dialog_OnItemSelect;
			_dialog.OnReadyDisable += _dialogBox_OnButtonClick;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_characterViewBack.LoadContent();
			_menuCharacter.LoadContent();
			_dialog.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_characterViewBack.UnloadContent();
			_menuCharacter.UnloadContent();
			_dialog.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			_characterView.Update(gameTime);
			_menuCharacter.Update(gameTime);
			_dialog.Update(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_titleText.Draw(spriteBatch);
			_characterViewBack.Draw(spriteBatch);
			_characterView.Draw(spriteBatch);
			_menuCharacter.Draw(spriteBatch);
			_dialog.Draw(spriteBatch);
		}

		private void _menuCharacter_OnSexItemChange(object sender, ComponentEventArgs e)
		{
			switch (e.Value)
			{
				case "female" :	_characterView = _preview[CharacterSex.Female];	break;
				case "male" :	_characterView = _preview[CharacterSex.Male];	break;
			}
		}

		private void _menuCharacter_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			switch (e.Value)
			{
				case "startgame" :
					var newChar = Character.New(_menuCharacter.CharacterName, _menuCharacter.CharacterSex);
					// TODO: Needs to store in it's own directory...based on a Guid/some other ID...
					IOManager.ObjectToFile(Game1.PlayerFile, newChar);
					e.Meta = Guid.NewGuid().ToString();	// meaningless for now...
					// TODO: Eventually we need to handle some kind of identifier of this new player to the parent, when we have multiple player/world files...
					ReadyDisable(e);
					break;
				case "cancel" :
				case "escape" :
					ReadyDisable(e);
					break;
			}
		}

		private void _menuCharacter_OnUserNotify(object sender, ComponentEventArgs e)
		{
			// Dialog should have a dynamiccomponent setup method...
			_dialog.Text = e.Text;
			_dialog.Duration = 300;
			_activator.SetState(_dialog, true);
		}

		private void _dialogBox_OnButtonClick(object sender, ComponentEventArgs e)
		{
			_activator.SetState(_menuCharacter, true);
		}

		private void _dialog_OnItemSelect(object sender, ComponentEventArgs e)
		{
			// Eventually we need to figure out what they clicked here...
			_activator.SetState(_menuCharacter, true);
		}
	}
}
