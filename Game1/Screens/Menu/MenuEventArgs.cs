using System;

namespace Game1.Screens.Menu
{
	public class MenuEventArgs : ComponentEventArgs
	{
		public MenuEventArgs(string type, object source)
		{
			// Do we really even need the type here?
			this.Type = type;
			this.Source = source;
		}
	}
}
