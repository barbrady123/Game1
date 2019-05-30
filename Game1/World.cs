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
	public class World
	{
		public List<NPC> NPCs { get; set; }
		public Character Character { get; set; }
		public Map CurrentMap { get; set; }

		public List<Character> AllCharacters => new List<Character>((this.NPCs?.Count ?? 0) + 1) { this.Character }.Concat(this.NPCs).ToList();

		public void Initialize()
		{
			this.CurrentMap = IOManager.ObjectFromFile<Map>(Game1.MapFile);
			this.CurrentMap.GenerateTiles();
			this.Character = IOManager.ObjectFromFile<Character>(Game1.PlayerFile);

			CharacterSex oppositeSex = (this.Character.Sex == CharacterSex.Male) ? CharacterSex.Female : CharacterSex.Male;
			this.NPCs = new List<NPC> {
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (128.0f, 128.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (256.0f, 256.0f) },
				new NPC { Name = Guid.NewGuid().ToString(), Sex = oppositeSex, Position = new Vector2 (1024.0f, 1024.0f) }
			};
		}

		public void LoadContent()
		{
		}

		public void UnloadContent()
		{

		}

		public void Update(GameTime gameTime)
		{
			this.CurrentMap.Update(gameTime);
			this.Character.Update(gameTime);
			foreach (var npc in this.NPCs)
				npc.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{

		}
	}
}
