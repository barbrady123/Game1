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
using Game1.Items;

namespace Game1
{
	public class Component
	{
		private int _delayInputCycles;

		public ComponentState State { get; set; }
		public int? Duration { get; set; }

		public event EventHandler<ComponentEventArgs> OnReadyDisable;
		public event EventHandler OnMouseOver;
		public event EventHandler OnMouseIn;
		public event EventHandler OnMouseOut;

		public Component()
		{
			_delayInputCycles = 0;
		}

		public virtual void LoadContent()
		{
		}

		public virtual void UnloadContent()
		{
		}

		public virtual void Update(GameTime gameTime)
		{
			// State:
			//    Visible
			//    Active: Effects running, Menu item alpha @ 1.0 vs less, Highlights?? (could just be a part of mouse position detection)
			//    TakingInput
			//    DetectingMousePosition (in/out/over) - or this can always happen and the flag just indicates whether to fire the events or not
			if (this.State.HasFlag(ComponentState.Active))
				UpdateDuration(gameTime);
		}

		private void UpdateDuration(GameTime gameTime)
		{
			if (this.Duration != null)
			{
				if (this.Duration <= 0)
				{
					ReadyDisable(new ComponentEventArgs("timer", this.GetType().Name, null));
					// Should we auto-disable here?
					this.Duration = null;
				}
				else
				{
					this.Duration--;
				}
			}

			UpdateActive(gameTime);
		}

		public virtual void UpdateActive(GameTime gameTime)
		{
			// Active code here that isn't related to input....
			UpdateDelayInput(gameTime);
		}

		private void UpdateDelayInput(GameTime gameTime)
		{
			if (_delayInputCycles != 0)
			{
				_delayInputCycles = Math.Max(0, _delayInputCycles - 1);
				return;
			}

			if (this.State.HasFlag(ComponentState.DetectingMousePosition))
				UpdateMousePosition(gameTime);
			if (this.State.HasFlag(ComponentState.TakingInput))
				UpdateInput(gameTime);
		}

		public virtual void UpdateMousePosition(GameTime gameTime)
		{
		}

		public virtual void UpdateInput(GameTime gameTime)
		{

		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			// Do we still need to wrap this call to DrawInternal?  Be better to make this "dumb" to the concept and use the manager to get the spritebatches for these calls....
			if (this.State.HasFlag(ComponentState.Visible))
				DrawVisible(spriteBatch);
		}

		public virtual void DrawVisible(SpriteBatch spriteBatch)
		{
		}

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		protected virtual void ReadyDisable(ComponentEventArgs e) => OnReadyDisable?.Invoke(this, e);

		protected virtual void MouseOver(EventArgs e)  => OnMouseOver?.Invoke(this, e);

		protected virtual void MouseIn(EventArgs e) => OnMouseIn?.Invoke(this, e);

		protected virtual void MouseOut(EventArgs e) => OnMouseOut?.Invoke(this, e);
	}
}
