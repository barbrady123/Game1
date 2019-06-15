using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1
{
	public class StatusViewer<T> : Component where T: CharacterStatus
	{
		private const int ItemsPerRow = 3;
		private const int IconSize = 32;
		private const int ItemPadding = 5;
		private const int RowPadding = 15;

		protected override Size ContentMargin => new Size(10, 10);

		private readonly List<T> _statuses;
		private bool _growFromRight;

		public StatusViewer(Rectangle bounds, List<T> statuses, bool growFromRight = true) : base(bounds, background: null)
		{
			_statuses = statuses;
			_growFromRight = growFromRight;
		}

		public override void LoadContent()
		{
			base.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			for (int i = 0; i < _statuses.Count; i++)
				_statuses[i].Icon.Position = CalculateItemViewPosition(i);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			foreach (var status in _statuses)
			{
				status.Icon.Draw(spriteBatch);
				spriteBatch.DrawString(FontManager.Get(), DurationToText(status.Duration), status.Icon.Position.Offset(0, 34), Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
			}
		}

		private string DurationToText(double? value)
		{
			if (value == null)
				return "";

			int minutes = (int)value / 60;
			int seconds = (int)value % 60;

			return $"{minutes}:{seconds.ToString("00")}";
		}

		private Vector2 CalculateItemViewPosition(int index)
		{
			int row = index / StatusViewer<T>.ItemsPerRow;
			int col = index % StatusViewer<T>.ItemsPerRow;
			int xPos = 0;

			if (_growFromRight)
				xPos = this.Bounds.Right - this.ContentMargin.Width - (StatusViewer<T>.IconSize * (col + 1)) - (StatusViewer<T>.ItemPadding * col);
			else
				xPos = this.Bounds.X + this.ContentMargin.Width + (StatusViewer<T>.IconSize * col) + (StatusViewer<T>.ItemPadding * col);

			int yPos = this.Bounds.Y + this.ContentMargin.Height + (StatusViewer<T>.IconSize * row) + (StatusViewer<T>.ItemPadding * row);

			return new Vector2(xPos, yPos);
		}
	}
}
