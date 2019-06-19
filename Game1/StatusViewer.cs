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
	// TODO: Need tooltips on the icons after Componegeddon 2.0 is complete...
	public class StatusViewer<TCharacterStatus, TStatusEffect> : Component where TCharacterStatus: CharacterStatus<TStatusEffect> where TStatusEffect: StatusEffect
	{
		private const int ItemsPerRow = 5;
		private const int IconSize = 32;
		private const int ItemPadding = 7;
		private const int RowPadding = 25;

		protected override Size ContentMargin => new Size(10, 10);

		private readonly Dictionary<TCharacterStatus, CharacterStatusView<TCharacterStatus, TStatusEffect>> _statusViews;
		private readonly List<TCharacterStatus> _statuses;
		private bool _growFromRight;

		public StatusViewer(Rectangle bounds, List<TCharacterStatus> statuses, bool growFromRight = true) : base(bounds, background: null, enabledTooltip: true, fireMouseEvents: false)
		{
			_statusViews = new Dictionary<TCharacterStatus, CharacterStatusView<TCharacterStatus, TStatusEffect>>();
			_statuses = statuses;
			_growFromRight = growFromRight;
			UpdateStatusViews();
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
			UpdateStatusViews();


			for (int i = 0; i < _statuses.Count; i++)
			{
				var position = CalculateItemViewPosition(i);
				_statusViews[_statuses[i]].Bounds = position.ExpandToRectangleTopLeft(IconSize, IconSize);
				_statusViews[_statuses[i]].Update(gameTime);
			}
		}

		private void UpdateStatusViews()
		{
			var currentViewedStatues = _statusViews.Keys.ToList();

			var expiredStatuses = currentViewedStatues.Where(x => !_statuses.Contains(x)).ToList();
			var newStatuses = _statuses.Where(x => !currentViewedStatues.Contains(x)).ToList();

			foreach (var expired in expiredStatuses)
			{
				_statusViews[expired].OnMouseOver -= StatusViewer_OnMouseOver;
				_tooltip.HideIfOwner(_statusViews[expired]);
				_statusViews.Remove(expired);
			}

			foreach (var newStatus in newStatuses)
			{
				_statusViews[newStatus] = new CharacterStatusView<TCharacterStatus, TStatusEffect>(newStatus, IconSize);
				_statusViews[newStatus].OnMouseOver += StatusViewer_OnMouseOver;
			}
		}

		private void StatusViewer_OnMouseOver(object sender, ComponentEventArgs e)
		{
			e.Meta = sender;
			MouseOver(e);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			foreach (var view in _statusViews.Keys)
				_statusViews[view].Draw(spriteBatch);
		}

		private Vector2 CalculateItemViewPosition(int index)
		{
			int row = index / ItemsPerRow;
			int col = index % ItemsPerRow;
			int xPos = 0;

			if (_growFromRight)
				xPos = this.Bounds.Right - this.ContentMargin.Width - (IconSize * (col + 1)) - (ItemPadding * col);
			else
				xPos = this.Bounds.X + this.ContentMargin.Width + (IconSize * col) + (ItemPadding * col);

			int yPos = this.Bounds.Y + this.ContentMargin.Height + (IconSize * row) + (RowPadding * row);

			return new Vector2(xPos, yPos);
		}
	}
}
