using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;
using Game1.Enum;

namespace Game1.Interface
{
	public class TextInput : Component
	{
		public const int Height = 32;
		private static readonly Size TextPadding = new Size(4, 2);
		private const float ActiveTextAlpha = 1.0f;
		private const float InactiveTextAlpha = 0.7f;

		private readonly int _width;
		private readonly int _maxVisibleLength;

		private ImageTexture _cursor;
		private ImageText _textImage;
		private int _currentPositionIndex;
		private int _firstVisibleCharIndex;		
		private string _visibleText;
		private string _text;

		protected override Color BorderColor => Color.Gray;

		private int CurrentPositionIndex
		{
			get { return _currentPositionIndex; }
			set { _currentPositionIndex = Util.Clamp(value, 0, this.Text.Length); }
		}

		private Vector2 TextPosition => this.Bounds.TopLeftVector(this.BorderThickness + TextInput.TextPadding.Width, this.BorderThickness + TextInput.TextPadding.Height);
		private int CursorPositionX => (int)(this.TextPosition.X + _textImage.SubstringSize(0, this.CurrentPositionIndex - _firstVisibleCharIndex).X);

		public string AllowedCharacters { get; set; }
		public string BlockedCharacters { get; set; }
		public int MaxLength { get; set; }

		public string Text
		{
			get { return _text; }
			set {
				value = value ?? "";
				if (_text != value)
				{
					_text = value;
					CalculateVisibleText(TextInputAction.Add);
				}
			}
		}

		protected override void StateChange()
		{
			this.CurrentPositionIndex = 0;
			if (this.State.HasFlag(ComponentState.Active))
				DelayInput(1);
		}

		public event EventHandler<ComponentEventArgs> OnBeforeTextUpdate;

		public TextInput(int width, Vector2 position, string text = null, int maxLength = 100) : base(position.ExpandToRectangleCentered(width / 2, TextInput.Height / 2), true, hasBorder: true)
		{
			_width = width;
			_maxVisibleLength = _width - (this.BorderThickness * 2) - (TextInput.TextPadding.Width * 2);
			_firstVisibleCharIndex = 0;
			_visibleText = "";
			_text = text ?? "";
			this.MaxLength = maxLength;
			this.AllowedCharacters = "";
			this.BlockedCharacters = "";
			this.CurrentPositionIndex = this.Text.Length;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_textImage = new ImageText(this.Text, true) { 
				Position = this.TextPosition + new Vector2(0.0f, FontManager.FontHeight),
				Alignment = ImageAlignment.LeftBottom,
				Scale = new Vector2(1.1f, 1.1f)
			};
			_textImage.LoadContent();
			_cursor = new ImageTexture("Interface/cursor", true) { Position = new Vector2(this.CursorPositionX, this.TextPosition.Y) };
			_cursor.AddEffect(new FadeCycleEffect(true) { Speed = 5.0f });
			_cursor.LoadContent();
			CalculateVisibleText(TextInputAction.Right);
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_cursor.UnloadContent();
			_textImage.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			_background.IsActive = this.State.HasFlag(ComponentState.Active);
			_border.IsActive = this.State.HasFlag(ComponentState.Active);
			_cursor.IsActive = this.State.HasFlag(ComponentState.TakingInput);
			_textImage.Alpha = this.State.HasFlag(ComponentState.Active) ? TextInput.ActiveTextAlpha : TextInput.InactiveTextAlpha;
			_textImage.UpdateText(_visibleText);
			_textImage.Update(gameTime);
			base.Update(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			_cursor.Position = new Vector2(this.CursorPositionX, _cursor.Position.Y);
			_cursor.Update(gameTime);
			ProcessInput();
			base.UpdateInput(gameTime);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_cursor.Draw(spriteBatch);
			_textImage.Draw(spriteBatch);
		}

		private void ProcessInput()
		{
			string currentText = this.Text;
			var newText = new StringBuilder(this.Text);
			int cursorMove = 0;
			bool updateText = true;
			bool finalizeText = false;
			bool textAdded = false;
			TextInputAction action = TextInputAction.None;

			foreach (var key in InputManager.GetPressedKeys())
			{
				switch (key)
				{
					case (Keys.Enter) :		
						ReadyDisable(new TextInputEventArgs('\0', key));
						finalizeText = true;
						continue;
					case (Keys.Escape) :
						InputManager.BlockKey(Keys.Escape);
						ReadyDisable(new TextInputEventArgs('\0', key));
						updateText = false;
						finalizeText = true;
						continue;
					case (Keys.Back) :
						if (this.CurrentPositionIndex > 0)
						{
							newText.Remove(this.CurrentPositionIndex - 1, 1);
							cursorMove--;
							action = TextInputAction.Backspace;
						}
						continue;
					case (Keys.Left) :
						if (this.CurrentPositionIndex > 0)
						{	
							cursorMove--; 
							action = TextInputAction.Left;
						}
						continue;
					case (Keys.Right) :
						if (this.CurrentPositionIndex < this.Text.Length)
						{
							cursorMove++;
							action = TextInputAction.Right;
						}
						continue;
					case (Keys.Home) :
						this.CurrentPositionIndex = 0;
						action = TextInputAction.Left;
						continue;
					case (Keys.End) :
						this.CurrentPositionIndex = this.Text.Length;
						action = TextInputAction.Right;
						continue;
					case (Keys.Delete):
						if (_currentPositionIndex < this.Text.Length)
						{
							newText.Remove(this.CurrentPositionIndex, 1);
							action = TextInputAction.Delete;
						}
						continue;
				}
				
				char newChar = InputManager.KeyToChar(key, InputManager.KeyDown(new[] { Keys.LeftShift, Keys.RightShift} ) || InputManager.CapsLock);
				if (newChar == '\0')
					continue;		

				if ((String.IsNullOrEmpty(this.AllowedCharacters) || this.AllowedCharacters.Contains(newChar)) && (!this.BlockedCharacters.Contains(newChar)) && (newText.Length < this.MaxLength))
				{
					newText.Insert(this.CurrentPositionIndex, newChar);
					cursorMove++;
					action = TextInputAction.Add;
					textAdded = true;
				}
			}

			string updatedText = newText.ToString();

			if (finalizeText)
			{
				updatedText = updatedText.Trim();
				cursorMove = 0;
				action = TextInputAction.Left;
			}

			if (updateText && (updatedText != _text))
			{
				var eventArgs = new TextInputEventArgs('\0', Keys.None, _text, updatedText);
				if (textAdded)
					OnBeforeTextUpdate?.Invoke(this, eventArgs);
				if (!eventArgs.Cancel)
					_text = updatedText;
			}

			this.CurrentPositionIndex += cursorMove;

			if ((_text != currentText) || (cursorMove != 0))
				CalculateVisibleText(action);
		}

		private void CalculateVisibleText(TextInputAction action)
		{
			int totalLen = (int)_textImage.StringSize(this.Text).X;
			if (totalLen <= _maxVisibleLength)
			{
				_firstVisibleCharIndex = 0;
				_visibleText = this.Text;
				return;
			}

			int firstVisible;
			int lastVisible;
			string substring;
			bool isShrunk = false;

			switch (action)
			{
				case TextInputAction.Add:
				case TextInputAction.Right:
				case TextInputAction.Delete:
					firstVisible = _firstVisibleCharIndex;
					lastVisible = Math.Min(this.Text.Length - 1, this.CurrentPositionIndex - 1);
					substring = this.Text.SubstringByIndex(firstVisible, lastVisible);

					while (_textImage.StringLength(substring) > _maxVisibleLength)
					{
						firstVisible++;
						substring = this.Text.SubstringByIndex(firstVisible, lastVisible);
						isShrunk = true;
					}

					if (!isShrunk)
					{
						isShrunk = false;
						while (lastVisible < this.Text.Length - 1)
						{
							lastVisible++;
							substring = this.Text.SubstringByIndex(firstVisible, lastVisible);
							if (_textImage.StringLength(substring) > _maxVisibleLength)
							{
								lastVisible--;
								isShrunk = true;
								break;
							}
						}

						if (!isShrunk)
						{
							while ((_textImage.StringLength(substring) <= _maxVisibleLength) && (firstVisible > 0))
							{
								firstVisible--;
								substring = this.Text.SubstringByIndex(firstVisible, lastVisible);
							}

							firstVisible++;
						}
					}

					_firstVisibleCharIndex = firstVisible;
					_visibleText = this.Text.SubstringByIndex(firstVisible, lastVisible);
					return;
				case TextInputAction.Backspace:
				case TextInputAction.Left:
					firstVisible = Math.Min(_firstVisibleCharIndex, this.CurrentPositionIndex);
					lastVisible = this.Text.Length - 1;
					substring = this.Text.SubstringByIndex(firstVisible, lastVisible);

					while (_textImage.StringLength(substring) > _maxVisibleLength)
					{
						lastVisible--;
						substring = this.Text.SubstringByIndex(firstVisible, lastVisible);
						isShrunk = true;
					}

					if (!isShrunk)
					{
						isShrunk = false;
						while (_textImage.StringLength(substring) <= _maxVisibleLength)
						{
							firstVisible--;
							substring = this.Text.SubstringByIndex(firstVisible, lastVisible);
						}

						firstVisible++;
					}

					_firstVisibleCharIndex = firstVisible;
					_visibleText = this.Text.SubstringByIndex(firstVisible, lastVisible);
					return;
			}
		}
	}
}
