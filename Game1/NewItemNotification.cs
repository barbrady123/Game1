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
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;

namespace Game1
{
	public class NewItemNotification : Component
	{
		public const float BackgroundAlpha = 0.2f;
		public static readonly Size Size = new Size(300, 24);
		protected override Color BorderColor => new Color(20, 20, 50);
		protected override int BorderThickness => 1;

		private readonly InventoryItem _item;
		private readonly ImageText _text;
		private FadeOutEffect _fadeEffect;

		protected override void BoundsChanged(bool resized)
		{
			base.BoundsChanged(resized);
			if (_text != null)
				_text.Position = this.Bounds.TopLeftVector(5, 7);
		}

		public NewItemNotification(InventoryItem item, int duration) : base(Rectangle.Empty, hasBorder: true)
		{
			_item = item;
			string quantity = (item.Quantity > 1) ? $" ({item.Quantity})" : "";
			_text = new ImageText($"Item Received: {item.Item.DisplayName}{quantity}", true, ImageAlignment.LeftTop) { Scale = new Vector2(0.7f, 0.7f) };
			this.Duration = duration;
			this.IsActive = true;
			BoundsChanged(false);
		}

		public void UpdatePosition(Point position)
		{
			this.Bounds = position.ExpandToRectangleTopLeft(Size.Width, Size.Height);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_fadeEffect?.Update(gameTime);
			_background.Alpha = Math.Min(BackgroundAlpha, _text.Alpha);
			_border.Alpha = Math.Min(BackgroundAlpha, _text.Alpha);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_text.Draw(spriteBatch);
		}

		protected override void ReadyDisable(ComponentEventArgs e)
		{
			if (e?.Trigger == EventTrigger.Timer)
			{
				_fadeEffect = _text.AddEffect<FadeOutEffect>(true);
				_fadeEffect.OnActiveChange += _fadeEffect_OnActiveChange;
			}
			else
			{
				base.ReadyDisable(e);
			}
		}

		private void _fadeEffect_OnActiveChange(object sender, EventArgs e)
		{
			ReadyDisable(null);
		}
	}
}
