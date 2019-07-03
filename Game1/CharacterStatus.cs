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
		protected readonly object _lock = new object();
		public T Effect { get; set; }
		public ImageTexture Icon { get; set; }
		public double? Duration { get; set; }
		public int Stacks { get; set; }
		public double CurrentPeriod { get; set; }

		public CharacterStatus(T effect, ImageTexture icon)
		{
			this.Effect = effect;
			this.Duration = effect.Duration;
			this.Stacks = 1;
			this.Icon = icon;
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
				_onExpired?.Invoke(this, null);
			}

			if (this.Effect.Period == null)
				return;

			this.CurrentPeriod += gameTime.ElapsedGameTime.TotalSeconds;
			if (this.CurrentPeriod >= (int)this.Effect.Period)
			{
				_onPeriodicTick?.Invoke(this, new CharacterStatusEventArgs { AffectedAttribute = this.Effect.AffectedAttribute, EffectValue = this.Effect.EffectValue * this.Stacks });
				this.CurrentPeriod = 0.0;
			}
		}

		#region Events
		private event EventHandler<CharacterStatusEventArgs> _onExpired;
		public event EventHandler<CharacterStatusEventArgs> OnExpired
		{
			add		{ lock(_lock) { _onExpired -= value; _onExpired += value; } }
			remove	{ lock(_lock) { _onExpired -= value; } }
		}

		private event EventHandler<CharacterStatusEventArgs> _onPeriodicTick;
		public event EventHandler<CharacterStatusEventArgs> OnPeriodicTick
		{
			add		{ lock(_lock) { _onPeriodicTick -= value; _onPeriodicTick += value; } }
			remove	{ lock(_lock) { _onPeriodicTick -= value; } }
		}
		#endregion
	}
}
