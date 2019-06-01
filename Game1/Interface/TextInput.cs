﻿using System;
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
	public class TextInput : IActivatable
	{
		private readonly int _width;
		private readonly int _height;
		private readonly int _borderWidth;
		private readonly int _fontHeight;
		private readonly int _maxVisibleLength;

		private Vector2 _padding;
		private ImageTexture _background;
		private ImageTexture _border;
		private ImageTexture _cursor;
		private ImageText _textImage;
		private int _currentPositionIndex;
		private int _firstVisibleCharIndex;		
		private string _visibleText;
		private bool _isActive;
		private string _text;
		
		private int CurrentPositionIndex
		{
			get { return _currentPositionIndex; }
			set { _currentPositionIndex = Util.Clamp(value, 0, this.Text.Length); }
		}

		private Vector2 TextPosition => this.Position
										+ new Vector2(_borderWidth, _borderWidth)
										+ _padding;

		private int CursorPositionX => (int)(this.TextPosition.X + _textImage.SubstringSize(0, this.CurrentPositionIndex - _firstVisibleCharIndex).X);

		public Vector2 Position { get; set; }
		public string AllowedCharacters { get; set; }
		public string BlockedCharacters { get; set; }
		public int MaxLength { get; set; }

		public string Text
		{
			get { return _text; }
			set { _text = value ?? ""; }
		}
		
		public bool IsActive
		{ 
			get { return _isActive; }
			set {
				if (_isActive != value)
				{
					_isActive = value;
					this.CurrentPositionIndex = 0;
					if (_isActive)
						DelayInput(1);
				}
			}
		}

		private int _delayInputCycles;

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		public event EventHandler OnReadyDisable;

		public TextInput(int width, string text = null, int maxLength = 100, bool isActive = false)
		{
			_width = width;
			_borderWidth = 2;
			_height = 32;
			_padding = new Vector2(4.0f, 2.0f);
			_fontHeight = 24;
			_maxVisibleLength = _width - (_borderWidth * 2) - ((int)_padding.X * 2);
			_firstVisibleCharIndex = 0;
			_visibleText = "";
			_delayInputCycles = 0;
			this.Text = text;
			this.MaxLength = maxLength;
			this.Position = Vector2.Zero;
			this.IsActive = isActive;
			this.AllowedCharacters = "";
			this.BlockedCharacters = "";
			this.CurrentPositionIndex = this.Text.Length;
		}

		public void LoadContent()
		{
			_background = Util.GenerateSolidBackground(_width, _height, Color.Black);
			_background.Position = this.Position;
			_background.LoadContent();
			_border = Util.GenerateBorderTexture(_width, _height, _borderWidth, Color.Gray);
			_border.Position = this.Position;
			_border.LoadContent();
			_textImage = new ImageText(this.Text, true) { 
				Position = this.TextPosition + new Vector2(0.0f, _fontHeight),
				Alignment = ImageAlignment.LeftBottom,
				Scale = new Vector2(1.1f, 1.1f)
			};
			_textImage.LoadContent();
			_cursor = new ImageTexture("Interface/cursor", this.IsActive) { Position = new Vector2(this.CursorPositionX, this.TextPosition.Y) };
			_cursor.AddEffect(new FadeCycleEffect(true) { Speed = 5.0f });
			_cursor.LoadContent();
			CalculateVisibleText(TextInputAction.Right);
		}

		public void UnloadContent()
		{
			_background.UnloadContent();
			_border.UnloadContent();
			_cursor.UnloadContent();
			_textImage.UnloadContent();
		}

		public void Update(GameTime gameTime, bool processInput)
		{
			if (processInput && (_delayInputCycles == 0))
				ProcessInput();
			_delayInputCycles = Math.Max(0, _delayInputCycles - 1);

			_border.Update(gameTime);
			_textImage.UpdateText(_visibleText);
			_textImage.Update(gameTime);
			_cursor.IsActive = this.IsActive;
			_cursor.Position = new Vector2(this.CursorPositionX, _cursor.Position.Y);
			_cursor.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (this.IsActive)
			{				
				_background.Draw(spriteBatch);
				_border.Draw(spriteBatch);
				_cursor.Draw(spriteBatch);
			}
			
			_textImage.Draw(spriteBatch);
		}

		private void ProcessInput()
		{
			if (!this.IsActive)
				return;

			var newText = new StringBuilder(this.Text);
			int cursorMove = 0;
			bool updateText = true;
			bool finalizeText = false;
			TextInputAction action = TextInputAction.None;

			foreach (var key in InputManager.GetPressedKeys())
			{
				switch (key)
				{
					case (Keys.Enter) :		
						OnReadyDisable?.Invoke(this, new TextInputEventArgs('\0', key));
						finalizeText = true;
						continue;
					case (Keys.Escape) :
						OnReadyDisable?.Invoke(this, new TextInputEventArgs('\0', key));
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
				}
			}

			if (updateText)
				this.Text = newText.ToString();
			if (finalizeText)
			{
				this.Text = this.Text.Trim();
				cursorMove = 0;
				action = TextInputAction.Left;
			}

			this.CurrentPositionIndex += cursorMove;
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
