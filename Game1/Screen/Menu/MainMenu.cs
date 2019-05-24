using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;

namespace Game1.Screen.Menu
{
	public class MainMenu : MenuScreen
	{
		public MainMenu(GraphicsDevice graphics, Vector2 size): base(graphics, size)
		{
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Start New Game", true), LinkID = "new"});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Options", true), LinkID = "options"});
			_items.Add(new MenuItem { Image = new Image(graphics, null, "Exit Game", true), LinkID = "exit"});
		}

		public override void LoadContent(IServiceProvider services)
		{
			base.LoadContent(services);
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
