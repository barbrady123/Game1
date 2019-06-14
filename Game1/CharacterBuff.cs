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
	public class CharacterBuff
	{
		public BuffEffect Buff { get; set; }
		public double? Duration { get; set; }
		public int Stacks { get; set; }

		public event EventHandler<ComponentEventArgs> OnExpired;

		public CharacterBuff(BuffEffect buff)
		{
			this.Buff = buff;
			this.Duration = buff.Duration;
		}

		public void Update(GameTime gameTime)
		{
			if (this.Duration == null)
				return;

			this.Duration = (double)this.Duration - gameTime.ElapsedGameTime.TotalSeconds;
			if (this.Duration <= 0)
			{
				this.Duration = null;
				OnExpired?.Invoke(this, null);
			}
		}
	}
}
