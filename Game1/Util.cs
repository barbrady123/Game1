using System;

namespace Game1
{
	public static class Util
	{
		public static int Clamp(int input, int min, int max) => Math.Min(Math.Max(input, min), max);

		public static bool InRange(int input, int min, int max, bool inclusiveMin = true, bool inclusiveMax = true) => (input >= (inclusiveMin ? min : min + 1)) && (input <= (inclusiveMax ? max : max - 1));
	}
}
