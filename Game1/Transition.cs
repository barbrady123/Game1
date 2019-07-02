using System;
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
		public string Id { get; set; }

		public string DisplayName { get; set; }

		public string IconName { get; set; }

		public Transition(string id, string displayName, string iconName)
		{
			this.Id = id;
			this.DisplayName = displayName;
			this.IconName = iconName;
		}
	}
}
