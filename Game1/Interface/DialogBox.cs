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
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1.Interface
{
	// TODO: Support text wrap/multiline...
	public class DialogBox : IActivatable
	{
		private readonly Rectangle _bounds;

		private ImageTexture _backgroundImage;
		private ImageText _textImage;
		private MenuScreen _buttonMenu;
		private string _text;
		private bool _isActive;

		public string Text
		{ 
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					_textImage.UpdateText(_text);					
				}
			}
		}

		public DialogButton Buttons { get; set; }
		
		public bool IsActive
		{
			get { return _isActive; }
			set {
				if (_isActive != value)
				{
					_isActive = value;
					if (_isActive)
						DelayInput(1);
					else
						this.Duration = null;
				}
			}
		}

		public event EventHandler OnButtonClick;
		public event EventHandler OnReadyDisable;

		private int _delayInputCycles;

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		public int? Duration { get; set; }

		public DialogBox(string text, DialogButton buttons, Rectangle bounds, int? duration)
		{
			_text = text ?? "";
			this.Buttons = buttons;
			this.IsActive = false;
			this.Duration = duration;
			_bounds = bounds;
			_backgroundImage = new ImageTexture($"{Game1.BackgroundRoot}/black", true) { 
				Alignment = ImageAlignment.Centered,
				Scale = _bounds.SizeVector(),
				DrawArea = _bounds,
				Position = _bounds.SizeVector() / 2
			};
			_textImage = new ImageText(_text, true) { Position = _bounds.CenterVector(yOffset: - _bounds.Height / 3) };
			switch (this.Buttons)
			{
				case DialogButton.Ok :	
					_buttonMenu = new OkMenu(new Rectangle(_bounds.X, _bounds.Y + (int)(_bounds.Height * 0.75f),  _bounds.Width, (int)(_bounds.Height * 0.25f))) { IsActive = true };
					_buttonMenu.OnItemSelect += _buttonMenu_OnItemSelect;
					_buttonMenu.OnReadyDisable += _buttonMenu_OnReadyDisable;
					break;
			}
			_delayInputCycles = 0;
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

			if (this.Duration != null)
			{
				if (this.Duration <= 0)
				{
					OnReadyDisable?.Invoke(this, new ScreenEventArgs("timer", this.GetType().Name, null));
					this.Duration = null;
				}
				else
				{
					this.Duration--;
				}
			}

			_backgroundImage.Update(gameTime);
			_textImage.Update(gameTime);

			if (_delayInputCycles != 0)
			{
				_delayInputCycles = Math.Max(0, _delayInputCycles - 1);
				return;
			}

			if (_buttonMenu != null)
			{
				_buttonMenu.Update(gameTime, this.IsActive);
			}
			else
			{
				if (InputManager.KeyPressed(Keys.Escape, true))
					_buttonMenu_OnReadyDisable(this, null);
			}
		}

		public void Draw()
		{
			if (!this.IsActive)
				return;
			
			Util.WrappedDraw(DrawInternal, "modal", _bounds);
		}

		private void DrawInternal(SpriteBatch spriteBatch)
		{
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
