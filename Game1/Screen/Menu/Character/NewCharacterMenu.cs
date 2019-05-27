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
using Game1.Screen.Menu;

namespace Game1.Screen.Menu.Character
{
	/// <summary>
	/// Testing new concept of a "composite" menu of sorts...to make building/reusing these complex menus easier...
	/// </summary>
	public class NewCharacterMenu	// : CompositeMenu (or something)...
	{
		private readonly Rectangle _bounds;
		private readonly CharacterNewMenu _menuCharacter;
		private readonly TextInput _nameEdit;
		private readonly SexMenu _menuSex;

		public string CharacterName { get; set; }
		public CharacterSex CharacterSex { get; set; }

		public event EventHandler OnReadyDisable;
		public event EventHandler OnSexItemChange;
		public event EventHandler OnSexItemSelect;

		public NewCharacterMenu(Rectangle bounds)
		{
			_bounds = bounds;	// for now let's base everything on 600, 200
			
			// Main menu...
			_menuCharacter = new CharacterNewMenu(new Rectangle(_bounds.Left, _bounds.Top, 200, 200));
			_menuCharacter.OnItemSelect += _menuCharacter_OnItemSelect;

			// Name edit box...
			_nameEdit = new TextInput(275, this.CharacterName, 12, false) {
				Position = new Vector2(_menuCharacter.Bounds.Left + 170,  _menuCharacter.Bounds.Top + 57)
			};
			_nameEdit.OnReadyDisable += _nameEdit_OnReadyDisable;

			// Sex menu...
			_menuSex = new SexMenu(new Rectangle(_menuCharacter.Bounds.Left + 90, _menuCharacter.Bounds.Top + 50, 300, 120)) { IsActive = false };
			_menuSex.OnCurrentItemChange += _menuSex_OnCurrentItemChange;
			_menuSex.OnItemSelect += _menuSex_OnItemSelect;
			_menuSex.OnReadyDisable += _menuSex_OnReadyDisable;
		}

		public void LoadContent()
		{
			_menuCharacter.LoadContent();
			_nameEdit.LoadContent();
			_menuSex.LoadContent();
			_menuSex.SetById(this.CharacterSex.ToString("g").ToLower());
		}

		public void UnloadContent()
		{
			_menuCharacter.UnloadContent();
			_nameEdit.UnloadContent();
			_menuSex.UnloadContent();
		}

		public void Update(GameTime gameTime, bool processInput)
		{
			_menuCharacter.Update(gameTime, processInput);
			_nameEdit.Update(gameTime, processInput);
			_menuSex.Update(gameTime, processInput);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_menuCharacter.Draw(spriteBatch);
			_nameEdit.Draw(spriteBatch);
			_menuSex.Draw(spriteBatch);
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

			_nameEdit.IsActive = false;
			_menuCharacter.IsActive = true;
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

			OnSexItemSelect?.Invoke(this, e);
			_menuSex.IsActive = false;
			_menuCharacter.IsActive = true;
		}

		private void _menuSex_OnReadyDisable(object sender, EventArgs e)
		{
			_menuSex.SetById(this.CharacterSex.ToString("g").ToLower());
			_menuSex.IsActive = false;
			_menuCharacter.IsActive = true;
		}
	}
}
