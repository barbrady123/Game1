﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public class ComponentEventArgs : EventArgs
	{
		public string Type { get; set; }

		public string Source { get; set; }

		public string Item { get; set; }

		public ComponentEventArgs() { }

		public ComponentEventArgs(string type, string source, string item)
		{
			this.Type = type;
			this.Source = source;
			this.Item = item;
		}
	}
}
