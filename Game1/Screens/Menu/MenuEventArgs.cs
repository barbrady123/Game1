using System;

namespace Game1.Screens.Menu
{
	public class MenuEventArgs : EventArgs
	{
		// TODO: Maybe make this an enum...
		public string Type { get; set; }

		public string Source { get; set; }

		public string Item { get; set; }		

		public MenuEventArgs() { }

		public MenuEventArgs(string type, string source, string item)
		{
			this.Type = type;
			this.Source = source;
			this.Item = item;
		}
	}
}
