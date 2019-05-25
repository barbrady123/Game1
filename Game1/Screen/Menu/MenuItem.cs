using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1.Screen.Menu
{
	public class MenuItem
	{
		[JsonProperty("text")]
		public string Text { get; set; }
		[JsonProperty("link")]
		public string Link { get; set; }
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("target")]
		public string Target { get; set; }
		[JsonProperty("halign")]
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public ImageText Image { get; set; }
		public Action LinkAction { get; set; }

		public MenuScreen SubMenu { get; set; }
	}
}
