using System;

namespace Game1.Screen
{
	public class ScreenEventArgs : EventArgs
	{
		// TODO: Maybe make this an enum...
		public string Type { get; set; }

		public string Target { get; set; }

		public ScreenEventArgs() { }

		public ScreenEventArgs(string type, string target)
		{
			this.Type = type;
			this.Target = target;
		}
	}
}
