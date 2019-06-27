using System;
using System.Collections.Generic;
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

namespace Game1.Maps
{
	public class MapTransition : MapMeta
	{
		[JsonProperty("type")]
		public TransitionType Type { get; set; }

		[JsonProperty("destination")]
		public string DestinationMap { get; set; }

		[JsonProperty("destinationposition")]
		public Point DestinationPosition { get; set; }
	}
}
