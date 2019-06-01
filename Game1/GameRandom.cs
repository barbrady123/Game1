using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public static class GameRandom
	{
		private static Random _random = new Random();

		public static int Seed { get; set; }

		public static void InitializeSeed(int seed)
		{
			GameRandom.Seed = seed;
			_random = new Random(seed);
		}

		public static int Next(int min, int max) => _random.Next(min, max + 1);
	}
}
