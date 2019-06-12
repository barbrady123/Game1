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
		public EventTrigger Trigger { get; set; }

		public string Value { get; set; }

		public object Meta { get; set; }

		public MouseButton Button { get; set; }

		public char Character { get; set; }

		public Keys Key { get; set; }

		public bool Cancel { get; set; }

		public string CurrentText { get; set; }

		public string Text { get; set; }
	}
}
