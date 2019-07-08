using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Game1.Enum;

namespace Game1
{
	public class Character
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("type")]
		public CreatureType CreatureType { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("spritesheet")]
		public string SpriteSheetName { get; set; }

		[JsonProperty("sex")]
		public CharacterSex Sex { get; set; }

		[JsonProperty("movementspeed")]
		public float MovementSpeed { get; set; }

		[JsonProperty("motion")]
		public string Motion { get; set; }
	}
}
