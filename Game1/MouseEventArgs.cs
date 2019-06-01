using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;

namespace Game1
{
	// TODO: May eventually need multi-button support
	public class MouseEventArgs : EventArgs
	{
		public MouseButton Button { get; set; }

		public int SourceIndex { get; set; }

		public MouseEventArgs(MouseButton button, int sourceIndex)
		{
			this.Button = button;
			this.SourceIndex = sourceIndex;
		}
	}
}
