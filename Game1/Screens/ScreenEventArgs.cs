using System;

namespace Game1.Screens
{
	public class ScreenEventArgs : EventArgs
	{
		// TODO: Maybe make this an enum...
		public string Type { get; set; }

		public string Source { get; set; }

		public string Item { get; set; }

		public ScreenEventArgs() { }

		public ScreenEventArgs(string type, string source, string item)
		{
			this.Type = type;
			this.Source = source;
			this.Item = item;
		}
	}
}
