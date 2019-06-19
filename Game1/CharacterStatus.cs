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

namespace Game1
{
	public class CharacterStatus<T> where T: StatusEffect
	{
		public T Effect { get; set; }
		public ImageTexture Icon { get; set; }
		public double? Duration { get; set; }
		public int Stacks { get; set; }
		public double CurrentPeriod { get; set; }

		public event EventHandler<CharacterStatusEventArgs> OnExpired;
		public event EventHandler<CharacterStatusEventArgs> OnPeriodicTick;

		public CharacterStatus(T effect, ImageTexture icon)
		{
			this.Effect = effect;
			this.Duration = effect.Duration;
			this.Stacks = 1;
			this.Icon = icon;
			this.Icon.LoadContent();
			this.CurrentPeriod = 0.0;
		}

		public virtual void Update(GameTime gameTime)
		{
			this.Icon?.Update(gameTime);

			if (this.Duration == null)
				return;

			this.Duration = (double)this.Duration - gameTime.ElapsedGameTime.TotalSeconds;
			if (this.Duration <= 0)
			{
				this.Duration = null;
				OnExpired?.Invoke(this, null);
			}

			if (this.Effect.Period == null)
				return;

			this.CurrentPeriod += gameTime.ElapsedGameTime.TotalSeconds;
			if (this.CurrentPeriod >= (int)this.Effect.Period)
			{
				OnPeriodicTick?.Invoke(this, new CharacterStatusEventArgs { AffectedAttribute = this.Effect.AffectedAttribute, EffectValue = this.Effect.EffectValue * this.Stacks });
				this.CurrentPeriod = 0.0;
			}
		}
	}
}
