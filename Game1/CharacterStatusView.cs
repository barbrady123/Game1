using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Effect;

namespace Game1
{
	public class CharacterStatusView<TCharacterStatus, TStatusEffect> : Component where TCharacterStatus: CharacterStatus<TStatusEffect> where TStatusEffect: StatusEffect
	{
		private TCharacterStatus _status;
		private readonly int _iconSize;

		protected override void BoundsChanged(bool resized) { }

		public override string TooltipText => _status.Effect.Text;

		public CharacterStatusView(TCharacterStatus status, int iconSize) : base(background: null)
		{
			_status = status;
			_iconSize = iconSize;
			this.IsActive = true;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			_status.Icon.Position = this.Bounds.TopLeftVector();
			if (_status.Duration < 10.0f)
			{
				var effect =_status.Icon.AddEffect<FadeCycleEffect>(true);
				effect.Speed = (_status.Duration < 5.0f) ? 2.5f : 1.2f;
			}
			else
			{
				_status.Icon.StopEffect(typeof(FadeCycleEffect));
			}

			_status.Icon.Update(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			_status.Icon.Draw(spriteBatch);
			spriteBatch.DrawString(FontManager.Get(), DurationToText(_status.Duration), _status.Icon.Position.Offset(0, 34), Color.White, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
			if (_status.Stacks > 1)
				spriteBatch.DrawString(FontManager.Get(), _status.Stacks.ToString(), _status.Icon.Position.Offset(_iconSize - 15, _iconSize - 15), Color.White, 0.0f , Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
		}

		private string DurationToText(double? value)
		{
			if (value == null)
				return "";

			int minutes = (int)value / 60;
			int seconds = (int)value % 60;

			return $"{minutes}:{seconds.ToString("00")}";
		}
	}
}
