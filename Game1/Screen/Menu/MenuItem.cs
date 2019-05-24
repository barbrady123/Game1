using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Screen.Menu
{
	public class MenuItem
	{
		public Image Image { get; set; }
		public string LinkID { get; set; }
		public Action LinkAction { get; set; }
	}
}
