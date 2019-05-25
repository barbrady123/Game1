using System;

namespace Game1.Screen.Menu
{
	public class MenuEventArgs : EventArgs
	{
		// TODO: Maybe make this an enum...
		public string Type { get; set; }

		public string Target { get; set; }

		public MenuEventArgs() { }

		public MenuEventArgs(string type, string target)
		{
			this.Type = type;
			this.Target = target;
		}
	}
}
