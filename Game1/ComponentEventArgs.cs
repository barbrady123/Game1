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
	/// </summary>
	public class ComponentEventArgs : EventArgs
	{
		public string Type { get; set; }

		public string Source { get; set; }

		public string Item { get; set; }

		public MouseButton Button { get; set; }

		public int SourceIndex { get; set; }

		public char Character { get; }

		public Keys Key { get; }

		public string CurrentText { get; set; }

		public string ResultText { get; set; }

		public bool Cancel { get; set; }

		public string Text { get; set; }

		public ComponentEventArgs() { }

		public ComponentEventArgs(string type, string source = null, string item = null)
		{
			this.Type = type;
			this.Source = source;
			this.Item = item;
		}

		public ComponentEventArgs(MouseButton button, int sourceIndex = -1)
		{
			this.Button = button;
			this.SourceIndex = sourceIndex;
		}
	
		public ComponentEventArgs(char character, Keys key = Keys.None, string currentText = null, string resultText = null)
		{
			this.Character = character;
			this.Key = key;
			this.CurrentText = currentText;
			this.ResultText = resultText;
			this.Cancel = false;
		}

		public ComponentEventArgs(string type, string source, int? sourceIndex, string item)
		{
			this.Type = type;
			this.Source = source;
			this.SourceIndex = sourceIndex ?? -1;
			this.Item = item;
		}
	}
}
