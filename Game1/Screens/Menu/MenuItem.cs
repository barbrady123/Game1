﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using Game1.Enum;

namespace Game1.Screens.Menu
{
	public class MenuItem
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("text")]
		public string Text { get; set; }
		[JsonProperty("link")]
		public string Link { get; set; }
		[JsonIgnore]
		public ImageText Image { get; set; }		
		[JsonIgnore]
		public Action LinkAction { get; set; }
		[JsonIgnore]
		public Rectangle Bounds { get; set; }
	}
}
