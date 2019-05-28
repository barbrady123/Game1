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
using Game1.Screen.Menu;

namespace Game1.Screen
{
	public class DialogBox
	{
		private readonly Rectangle _bounds;

		private ImageTexture _backgroundImage;
		private ImageText _textImage;
		private MenuScreen _buttonMenu;
		private string _text;

		public string Text
		{ 
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					_textImage.UpdateText(_text);
					// TODO: We'll need to reposition this stuff (once we actually have positioning here)...
				}
			}
		}

		public DialogButton Buttons { get; set; }
		public bool IsActive { get; set; }

		public event EventHandler OnButtonClick;
		public event EventHandler OnReadyDisable;

		public int? Duration { get; set; }

		public DialogBox(string text, DialogButton buttons, Rectangle bounds, int? duration, bool isActive = false)
		{
			_text = text;
			this.Buttons = buttons;
			this.IsActive = isActive;
			this.Duration = duration;
			_bounds = bounds;
			_backgroundImage = new ImageTexture($"{Game1.BackgroundRoot}/black", true) { 
				Alignment = ImageAlignment.Centered,
				Scale = _bounds.SizeVector(),
				DrawArea = _bounds,
				Position = _bounds.SizeVector() / 2
			};
			_textImage = new ImageText(_text, true) { Position = _bounds.CenterVector(yOffset: -100) };
			switch (this.Buttons)
			{
				case DialogButton.Ok :	
					_buttonMenu = new OkMenu(new Rectangle(_bounds.X, _bounds.Y + (int)(_bounds.Height * 0.75f),  _bounds.Width, (int)(_bounds.Height * 0.25f))) { IsActive = true };
					_buttonMenu.OnItemSelect += _buttonMenu_OnItemSelect;
					_buttonMenu.OnReadyDisable += _buttonMenu_OnReadyDisable;
					break;
			}
		}

		public void LoadContent()
		{
			_backgroundImage.LoadContent();
			_buttonMenu?.LoadContent();
			_textImage.LoadContent();
		}

		public void UnloadContent()
		{
			_backgroundImage.UnloadContent();
			_buttonMenu?.UnloadContent();
			_textImage.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			if (!this.IsActive)
				return;

			_backgroundImage.Update(gameTime);
			if (_buttonMenu != null)
				_buttonMenu.Update(gameTime, this.IsActive);
			else
			{
				if (InputManager.Instance.KeyPressed(Keys.Escape))
					_buttonMenu_OnReadyDisable(this, null);
			}
			_textImage.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (!this.IsActive)
				return;

			_backgroundImage.Draw(spriteBatch);
			_buttonMenu?.Draw(spriteBatch);
			_textImage.Draw(spriteBatch);
		}

		private void _buttonMenu_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;
			OnButtonClick?.Invoke(this, new ScreenEventArgs("select", this.GetType().Name, args.Item));
		}

		private void _buttonMenu_OnReadyDisable(object sender, EventArgs e)
		{
			OnReadyDisable?.Invoke(this, new ScreenEventArgs("escape", this.GetType().Name, null));
		}
	}
}
