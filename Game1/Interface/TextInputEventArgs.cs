using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1.Interface
{
	public class TextInputEventArgs : Microsoft.Xna.Framework.TextInputEventArgs
	{
		public string CurrentText { get; set; }
		public string ResultText { get; set; }
		public bool Cancel { get; set; }
		
		public TextInputEventArgs(char character, Keys key = Keys.None, string currentText = null, string resultText = null) : base(character, key)
		{
			this.CurrentText = currentText;
			this.ResultText = resultText;
			this.Cancel = false;
		}
	}
}
