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
using Game1.Interface;
using Game1.Screens.Menu;

namespace Game1.Screens.Menu.Character
{
	public class CharacterNewCompositeMenu : IActivatable
	{
		private readonly ActivationManager _activation = new ActivationManager();
		private readonly Rectangle _bounds;
		private readonly CharacterNewMenu _menuCharacter;
		private readonly TextInput _nameEdit;
		private readonly SexMenu _menuSex;
		private readonly StartCancelMenu _menuStart;

		public string CharacterName { get; set; }
		public CharacterSex CharacterSex { get; set; }

		public event EventHandler OnReadyDisable;
		public event EventHandler OnSexItemChange;
		public event EventHandler OnUserNotify;

		public bool IsActive { get; set; }

		public CharacterNewCompositeMenu(Rectangle bounds)
		{
			_bounds = bounds;
			
			// Main menu...
			_activation.Add(_menuCharacter = new CharacterNewMenu(new Rectangle(_bounds.Left, _bounds.Top, 200, 200)));
			_menuCharacter.OnItemSelect += _menuCharacter_OnItemSelect;
			_menuCharacter.OnMouseIn += _menuCharacter_OnMouseIn;

			// Name edit box...
			_activation.Add(_nameEdit = new TextInput(275, this.CharacterName, 12, false) {
				Position = new Vector2(_menuCharacter.Bounds.Left + 170,  _menuCharacter.Bounds.Top + 62)
			});
			_nameEdit.OnReadyDisable += _nameEdit_OnReadyDisable;

			// Sex menu...
			_activation.Add(_menuSex = new SexMenu(new Rectangle(_menuCharacter.Bounds.Left + 90, _menuCharacter.Bounds.Top + 60, 300, 120)) { IsActive = false });
			_menuSex.OnCurrentItemChange += _menuSex_OnCurrentItemChange;
			_menuSex.OnItemSelect += _menuSex_OnItemSelect;
			_menuSex.OnReadyDisable += _menuSex_OnReadyDisable;

			// Start/Cancel menu...
			_activation.Add(_menuStart = new StartCancelMenu(new Rectangle(_menuCharacter.Bounds.Left + 50, _menuCharacter.Bounds.Top + 300, 300, 50)) { IsActive = false });
			_menuStart.OnItemSelect += _menuStart_OnItemSelect;
			_menuStart.OnMouseIn += _menuStart_OnMouseIn;

			_activation.Activate(_menuCharacter);
		}

		public void LoadContent()
		{
			_menuCharacter.LoadContent();
			_nameEdit.LoadContent();
			_menuSex.LoadContent();
			_menuSex.SetById(this.CharacterSex.ToString("g").ToLower());
			_menuStart.LoadContent();
			_menuStart.ClearSelection();
		}

		public void UnloadContent()
		{
			_menuCharacter.UnloadContent();
			_nameEdit.UnloadContent();
			_menuSex.UnloadContent();
			_menuStart.UnloadContent();
		}

		public void Update(GameTime gameTime, bool processInput)
		{
			if (!this.IsActive)
				return;

			_menuCharacter.Update(gameTime, processInput);
			_nameEdit.Update(gameTime, processInput);
			_menuSex.Update(gameTime, processInput);
			_menuStart.Update(gameTime, processInput);

			if (InputManager.KeyPressed(Keys.Escape))
				OnReadyDisable?.Invoke(this, new MenuEventArgs("back", this.GetType().Name, null, null));
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_menuCharacter.Draw(spriteBatch);
			_nameEdit.Draw(spriteBatch);
			_menuSex.Draw(spriteBatch);
			_menuStart.Draw(spriteBatch);
		}

		private void _menuCharacter_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Item)
			{
				case "name":	_activation.Activate(_nameEdit);	break;
				case "sex":		_activation.Activate(_menuSex);		break;
			}
		}

		private void _nameEdit_OnReadyDisable(object sender, EventArgs e)
		{
			var key = ((TextInputEventArgs)e).Key;
			if (key == Keys.Enter)
			{
				this.CharacterName = _nameEdit.Text;
			}
			else if (key == Keys.Escape)
			{
				_nameEdit.Text = this.CharacterName;
			}

			_activation.Activate(_menuCharacter);
		}

		private void _menuSex_OnCurrentItemChange(object sender, EventArgs e)
		{
			OnSexItemChange?.Invoke(this, e);
		}

		private void _menuSex_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Item)
			{
				case "female" : this.CharacterSex = CharacterSex.Female;	break;
				case "male" :	this.CharacterSex = CharacterSex.Male;		break;
			}

			OnSexItemChange?.Invoke(this, e);
			_activation.Activate(_menuCharacter);
		}

		private void _menuSex_OnReadyDisable(object sender, EventArgs e)
		{
			int index = _menuSex.CurrentIndex;
			_menuSex.SetById(this.CharacterSex.ToString("g").ToLower());
			if (index != _menuSex.CurrentIndex)
				OnSexItemChange?.Invoke(this, new MenuEventArgs("change", this.GetType().Name, null, this.CharacterSex.ToString("g").ToLower()));
			_activation.Activate(_menuCharacter);
		}

		private void _menuStart_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Item)
			{
				case "startgame" :
					if (!ValidateInput())
						break;
					OnReadyDisable?.Invoke(this, new MenuEventArgs("continue", this.GetType().Name, null, null));
					break;
				case "cancel" :
					OnReadyDisable?.Invoke(this, new MenuEventArgs("back", this.GetType().Name, null, null));
					break;
			}
		}

		private bool ValidateInput()
		{
			if (String.IsNullOrWhiteSpace(this.CharacterName))
			{
				OnUserNotify?.Invoke(this, new UserNotifyArgs { Text = "Enter a name for the character" });
				return false;
			}

			return true;
		}

		private void _menuCharacter_OnMouseIn(object sender, EventArgs e)
		{
			if (_activation.IsActive(_menuStart))
				_activation.Activate(_menuCharacter);
		}

		private void _menuStart_OnMouseIn(object sender, EventArgs e)
		{
			if (_activation.IsActive(_menuCharacter))
				_activation.Activate(_menuStart);
		}
	}
}
