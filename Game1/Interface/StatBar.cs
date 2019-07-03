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

		private Func<int> CurrentProperty;
		private Func<int> MaxProperty;

		private int _previousCurrent;
		private int _previousMax;

		private string Text => $"{CurrentProperty()} / {MaxProperty()}";

		public StatBar(int width, Vector2 position, Color color, object source, string currentProperty, string maxProperty) : base(position.ExpandToRectangleCentered(width / 2, StatBar.Height / 2), hasBorder: true)
		{
			_source = source;
			var propInfo = typeof(Character).GetProperty(currentProperty, typeof(int));
			CurrentProperty = (Func<int>) Delegate.CreateDelegate(typeof(Func<int>), source, propInfo.GetGetMethod());
			propInfo = typeof(Character).GetProperty(maxProperty, typeof(int));
			MaxProperty = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), source, propInfo.GetGetMethod());
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
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_fill?.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_textImage.UpdateText(this.Text);
			_textImage.Update(gameTime);
			_fill?.Update(gameTime);

			int current = CurrentProperty();
			int max = MaxProperty();

			if ((current != _previousCurrent) || (max != _previousMax))
				SetFill(current, max);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_fill?.Draw(spriteBatch);
			_textImage.Draw(spriteBatch);
		}

		private void SetFill(int current, int max)
		{
			var fillBarBounds = this.Bounds.CenteredRegion(this.Bounds.Width - (2 * this.BorderThickness), this.Bounds.Height - (2 * this.BorderThickness));

			_fill?.UnloadContent();
			_fill = Util.GenerateSolidBackground((int)(fillBarBounds.Width * (max > 0 ? (float)current / (float)max : 0.0f)), fillBarBounds.Height, _barColor);
			_fill.Alignment = ImageAlignment.LeftTop;
			_fill.Position = fillBarBounds.TopLeftVector();

			_previousCurrent = current;
			_previousMax = max;
		}
	}
}
