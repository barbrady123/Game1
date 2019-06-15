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
	public class CharacterNewCompositeMenu : Component
	{
		private readonly CharacterNewMenu _menuCharacter;
		private readonly TextInput _nameEdit;
		private readonly SexMenu _menuSex;
		private readonly StartCancelMenu _menuStart;

		public string CharacterName { get; set; }
		public CharacterSex CharacterSex { get; set; }

		public event EventHandler<ComponentEventArgs> OnSexItemChange;
		public event EventHandler<ComponentEventArgs> OnUserNotify;

		public CharacterNewCompositeMenu(Rectangle bounds) : base(bounds, true, "brick")
		{
			// Main menu...
			_activator.Register(_menuCharacter = new CharacterNewMenu(new Rectangle(this.Bounds.Left, this.Bounds.Top, 200, 200)), true, "active");
			_menuCharacter.OnItemSelect += _menuCharacter_OnItemSelect;
			_menuCharacter.OnMouseIn += _menuCharacter_OnMouseIn;

			var basePosition = _menuCharacter.Bounds.TopLeftPoint();

			// Name edit box...
			_activator.Register(_nameEdit = new TextInput(275, _menuCharacter.Bounds.TopLeftVector(300, 80), this.CharacterName, 12), false, "active");
			_nameEdit.OnReadyDisable += _nameEdit_OnReadyDisable;

			// Sex menu...
			_activator.Register(_menuSex = new SexMenu(new Rectangle(basePosition.X + 90, basePosition.Y + 60, 300, 120)), false, "active");
			_menuSex.OnCurrentItemChange += _menuSex_OnCurrentItemChange;
			_menuSex.OnItemSelect += _menuSex_OnItemSelect;
			_menuSex.OnReadyDisable += _menuSex_OnReadyDisable;

			// Start/Cancel menu...
			_activator.Register(_menuStart = new StartCancelMenu(new Rectangle(basePosition.X + 50, basePosition.Y + 300, 300, 50)), false, "active");
			_menuStart.OnItemSelect += _menuStart_OnItemSelect;
			_menuStart.OnMouseIn += _menuStart_OnMouseIn;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_menuCharacter.LoadContent();
			_nameEdit.LoadContent();
			_menuSex.LoadContent();
			_menuSex.SetById(this.CharacterSex.ToString("g").ToLower());
			_menuStart.LoadContent();
			_menuStart.ClearSelection();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_menuCharacter.UnloadContent();
			_nameEdit.UnloadContent();
			_menuSex.UnloadContent();
			_menuStart.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_menuCharacter.Update(gameTime);
			_nameEdit.Update(gameTime);
			_menuSex.Update(gameTime);
			_menuStart.Update(gameTime);
			base.UpdateActive(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_menuCharacter.Draw(spriteBatch);
			_nameEdit.Draw(spriteBatch);
			_menuSex.Draw(spriteBatch);
			_menuStart.Draw(spriteBatch);
		}

		private void _menuCharacter_OnItemSelect(object sender, ComponentEventArgs e)
		{
			switch (e.Value)
			{
				case "name":	_activator.SetState(_nameEdit, true);	break;
				case "sex":		_activator.SetState(_menuSex, true);	break;
			}
		}

		private void _nameEdit_OnReadyDisable(object sender, EventArgs e)
		{
			var args = (ComponentEventArgs)e;

			if (args.Key == Keys.Enter)
			{
				this.CharacterName = _nameEdit.Text;
			}
			else if (args.Key == Keys.Escape)
			{
				_nameEdit.Text = this.CharacterName;
			}

			_activator.SetState(_menuCharacter, true);
		}

		private void _menuSex_OnCurrentItemChange(object sender, ComponentEventArgs e)
		{
			OnSexItemChange?.Invoke(this, e);
		}

		private void _menuSex_OnItemSelect(object sender, ComponentEventArgs e)
		{
			switch (e.Value)
			{
				case "female" : this.CharacterSex = CharacterSex.Female;	break;
				case "male" :	this.CharacterSex = CharacterSex.Male;		break;
			}

			OnSexItemChange?.Invoke(this, e);
			_activator.SetState(_menuCharacter, true);
		}

		private void _menuSex_OnReadyDisable(object sender, EventArgs e)
		{
			_menuSex.SetById(this.CharacterSex.ToString("g").ToLower());
			OnSexItemChange?.Invoke(this, new ComponentEventArgs { Value = this.CharacterSex.ToString("g").ToLower() });
			_activator.SetState(_menuCharacter, true);
		}

		private void _menuStart_OnItemSelect(object sender, ComponentEventArgs e)
		{
			switch (e.Value)
			{
				case "startgame" :
					if (!ValidateInput())
						break;
					ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.MenuItemSelect, Value = e.Value });
					break;
				case "cancel" :
					ReadyDisable(new ComponentEventArgs { Trigger = EventTrigger.MenuItemSelect, Value = e.Value });
					break;
			}
		}

		private bool ValidateInput()
		{
			if (String.IsNullOrWhiteSpace(this.CharacterName))
			{
				OnUserNotify?.Invoke(this, new ComponentEventArgs { Text = "Enter a name for the character" });
				return false;
			}

			return true;
		}

		private void _menuCharacter_OnMouseIn(object sender, ComponentEventArgs e)
		{			
			if (_menuStart.IsActive)
				_activator.SetState(_menuCharacter, true);
		}

		private void _menuStart_OnMouseIn(object sender, EventArgs e)
		{
			if (_menuCharacter.IsActive)
				_activator.SetState(_menuStart, true);
		}
	}
}
