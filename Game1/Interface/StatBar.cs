using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
	public class StatBar 
	{
		private readonly int _width;
		private readonly int _height;
		private readonly int _borderWidth;
		private readonly Color _color;

		private Vector2 _padding;
		private ImageTexture _background;
		private ImageTexture _fill;
		private ImageTexture _border;
		private ImageText _textImage;		

		private object _source;
		private string _currentProperty;
		private string _maxProperty;

		private int CurrentValue => (int)_source.GetType().InvokeMember(_currentProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, Type.DefaultBinder, _source, null);
		private int MaxValue => (int)_source.GetType().InvokeMember(_maxProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, Type.DefaultBinder, _source, null);
		private int _previousCurrent;
		private int _previousMax;

		private string Text => $"{CurrentValue} / {MaxValue}";

		public Vector2 Position { get; set; }

		public StatBar(int width, Vector2 position, Color color, object source, string currentProperty, string maxProperty)
		{
			_width = width;
			_borderWidth = 2;
			_height = 24;
			_padding = new Vector2(4.0f, 2.0f);
			this.Position = position;
			_source = source;
			_currentProperty = currentProperty;
			_maxProperty = maxProperty;
			_color = color;
			_previousCurrent = 0;
			_previousMax = 0;
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
				Position = this.Position.Offset(_width / 2, _height / 2 + 2),
				Alignment = ImageAlignment.Centered
			};
			_textImage.LoadContent();
			SetFill();
		}

		public void UnloadContent()
		{
			_background.UnloadContent();
			_border.UnloadContent();
			_textImage.UnloadContent();
			_fill.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			_border.Update(gameTime);
			_textImage.UpdateText(this.Text);
			_textImage.Update(gameTime);
			_fill.Update(gameTime);

			int current = this.CurrentValue;
			int max = this.MaxValue;

			if ((current != _previousCurrent) || (max != _previousMax))
				SetFill(current, max);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_background.Draw(spriteBatch);
			_fill.Draw(spriteBatch);
			_border.Draw(spriteBatch);			
			_textImage.Draw(spriteBatch);
		}

		private void SetFill(int? current = null, int? max = null)
		{
			int currentVal = current ?? this.CurrentValue;
			int maxVal = max ?? this.MaxValue;

			if (_fill != null)
				_fill.UnloadContent();

			_fill = Util.GenerateSolidBackground(_width * currentVal / maxVal, _height, _color);
			_fill.Position = this.Position;
			_fill.LoadContent();

			_previousCurrent = currentVal;
			_previousMax = maxVal;
		}
	}
}
