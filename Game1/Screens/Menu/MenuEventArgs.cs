using System;

namespace Game1.Screens.Menu
{
	public class MenuEventArgs : EventArgs
	{
		// TODO: Maybe make this an enum...
		public string Type { get; set; }

		public string Source { get; set; }

		public int? SourceIndex { get; set; }

		public string Item { get; set; }		

		public MenuEventArgs() { }

		public MenuEventArgs(string type, string source, int? sourceIndex, string item)
		{
			this.Type = type;
			this.Source = source;
			this.SourceIndex = sourceIndex;
			this.Item = item;
		}
	}
}
