namespace Game1
{
	public static class StringExtensions
	{
		public static string SubstringByIndex(this string text, int startIndex, int endIndex) => text.Substring(startIndex, endIndex - startIndex + 1);
	}
}
