﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Game1.Enum;
using Game1.Items;
using Game1.Maps;

namespace Game1
{
	public class Transition
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("displayname")]
		public string DisplayName { get; set; }

		[JsonProperty("iconname")]
		public string IconName { get; set; }
	}
}
