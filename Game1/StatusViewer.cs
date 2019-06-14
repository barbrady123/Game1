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
using Game1.Enum;

namespace Game1
{
	public class StatusViewer : Component
	{
		private const int MaxItemsPerRow = 10;
		private const int IconSize = 32;

		public StatusViewer(Rectangle bounds) : base(bounds)
		{
			
		}

		public override void LoadContent()
		{
			base.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
		}
	}
}
