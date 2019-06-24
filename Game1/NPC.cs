using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Game1.Enum;

namespace Game1
{
	public class NPC : Character
	{
		private const int SameDirectionBias = 99;
		private const int ContinueStopBias = 95;

		public NPC() : base()
		{
			_movementSpeed = 75.0f;
		}

		public override Vector2 UpdateMotion()
		{
			if (this.Motion == Vector2.Zero)
			{
				if (GameRandom.Next(0, 99) < NPC.ContinueStopBias)
					return this.Motion;
			}
			else
			{
				if (GameRandom.Next(0, 99) < NPC.SameDirectionBias)
					return this.Motion;
			}

			return new Vector2(GameRandom.Next(-1, 1), GameRandom.Next(-1, 1));
		}
	}
}
