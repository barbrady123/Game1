﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1
{
	public class NPC : Character
	{
		private const int SameDirectionBias = 99;
		private const int ContinueStopBias = 80;

		public NPC() : base()
		{
			this.Speed = 75.0f;
		}

		public override Vector2 UpdateMotion()
		{
			if (_latestMotion == Vector2.Zero)
			{
				if (GameRandom.Next(0, 99) < NPC.ContinueStopBias)
					return _latestMotion;
			}
			else
			{
				if (GameRandom.Next(0, 99) < NPC.SameDirectionBias)
					return _latestMotion;
			}

			return new Vector2(GameRandom.Next(-1, 1), GameRandom.Next(-1, 1));
		}
	}
}
