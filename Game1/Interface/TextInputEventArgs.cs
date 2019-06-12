using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1.Interface
{
	public class TextInputEventArgs : ComponentEventArgs
	{
		public TextInputEventArgs(char character, Keys key = Keys.None, string currentText = null, string resultText = null)
		{
			this.Character = character;
			this.Key = key;
			this.CurrentText = currentText;
			this.Text = resultText;
			this.Cancel = false;
		}
	}
}
