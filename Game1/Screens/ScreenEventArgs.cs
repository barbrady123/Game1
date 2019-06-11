using System;

namespace Game1.Screens
{
	[System.Diagnostics.DebuggerStepThrough]
	public class ScreenEventArgs : ComponentEventArgs
	{
		public ScreenEventArgs(string type, string source, string item)
		{
			this.Type = type;
			this.Source = source;
			this.Item = item;
		}
	}
}
