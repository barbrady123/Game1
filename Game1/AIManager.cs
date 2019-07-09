using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
	public static class AIManager
	{
		public static Func<Vector2, Vector2> GetMotionMethod(string id)
		{
			if (id == null)
				return null;

			var method = typeof(AIManager).GetMethod(id);
			return (Func<Vector2, Vector2>)method.CreateDelegate(typeof(Func<Vector2, Vector2>));
		}

		private static Vector2 RandomBias(Vector2 previousMotion)
		{
			const int SameDirectionBias = 99;
			const int ContinueStopBias = 96;
			
			if (previousMotion == Vector2.Zero)
			{
				if (GameRandom.Percent(ContinueStopBias))
					return previousMotion;
			}
			else
			{
				if (GameRandom.Percent(SameDirectionBias))
					return previousMotion;
			}

			return new Vector2(GameRandom.Next(-1, 1), GameRandom.Next(-1, 1));
		}
	}
}
