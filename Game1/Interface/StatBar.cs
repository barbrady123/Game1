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
	public class StatBar : Component
	{
		public const int Height = 24;

		protected override Color BorderColor => Color.Gray;
		private readonly Color _barColor;

		private ImageTexture _fill;
		private ImageText _textImage;		

		private object _source;
		private string _currentProperty;
		private string _maxProperty;

		private int CurrentValue => (int)_source.GetType().InvokeMember(_currentProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, Type.DefaultBinder, _source, null);
		private int MaxValue => (int)_source.GetType().InvokeMember(_maxProperty, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, Type.DefaultBinder, _source, null);
		private int _previousCurrent;
		private int _previousMax;

		private string Text => $"{CurrentValue} / {MaxValue}";

		public StatBar(int width, Vector2 position, Color color, object source, string currentProperty, string maxProperty) : base(position.ExpandToRectangleCentered(width / 2, StatBar.Height / 2), hasBorder: true)
		{
			_source = source;
			_currentProperty = currentProperty;
			_maxProperty = maxProperty;
			_barColor = color;
			_previousCurrent = -1;
			_previousMax = -1;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_textImage = new ImageText(this.Text, true) { 
				Position = this.Bounds.CenterVector(yOffset: 2),
				Alignment = ImageAlignment.Centered
			};
			_textImage.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_textImage.UnloadContent();
			_fill.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_textImage.UpdateText(this.Text);
			_textImage.Update(gameTime);
			_fill?.Update(gameTime);

			int current = this.CurrentValue;
			int max = this.MaxValue;

			if ((current != _previousCurrent) || (max != _previousMax))
				SetFill(current, max);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_fill.Draw(spriteBatch);
			_textImage.Draw(spriteBatch);
		}

		private void SetFill(int? current = null, int? max = null)
		{
			int currentVal = current ?? this.CurrentValue;
			int maxVal = max ?? this.MaxValue;

			var fillBarBounds = this.Bounds.CenteredRegion(this.Bounds.Width - (2 * this.BorderThickness), this.Bounds.Height - (2 * this.BorderThickness));

			_fill?.UnloadContent();
			_fill = Util.GenerateSolidBackground((int)(fillBarBounds.Width * (maxVal > 0 ? (float)currentVal / (float)maxVal : 0.0f)), fillBarBounds.Height, _barColor);
			_fill.Alignment = ImageAlignment.LeftTop;
			_fill.Position = fillBarBounds.TopLeftVector();
			_fill.LoadContent();

			_previousCurrent = currentVal;
			_previousMax = maxVal;
		}
	}
}
