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
	public class MouseEventArgs : ComponentEventArgs
	{
		public MouseEventArgs(MouseButton button = MouseButton.None)
		{
			this.Button = button;
		}
	}
}
