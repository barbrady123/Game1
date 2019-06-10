using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1
{
	/// <summary>
	/// This just makes handling nested events and such WAAAAAY easier...even though it's kinda lame to have so many properties for one event arg type...
	/// We should maybe derive child classes that correspond better to the actions but still have all of the fields (just constructors basically)...
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough]
	public class ComponentEventArgs : EventArgs
	{
		public object Sender { get; set; }

		// We may not need this and Sender...
		public object Source { get; set; }

		public string Type { get; set; }

		public string Item { get; set; }

		public MouseButton Button { get; set; }

		//public int? SourceIndex { get; set; }

		public char Character { get; set; }

		public Keys Key { get; set; }

		public string CurrentText { get; set; }

		public string ResultText { get; set; }

		public bool Cancel { get; set; }

		public string Text { get; set; }

		public ComponentEventArgs InnerEventArgs { get; set; }

		public ComponentEventArgs() { }

		// ScreenEventArgs
		public ComponentEventArgs(string type, string source = null, string item = null)
		{
			this.Type = type;
			this.Source = source;
			this.Item = item;
		}

		public ComponentEventArgs(ComponentEventArgs innerEventArgs, object sender)
		{
			this.InnerEventArgs = innerEventArgs;
			this.Sender = sender;
		}
	}
}
