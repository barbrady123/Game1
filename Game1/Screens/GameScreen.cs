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
using Game1.Enum;
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;

namespace Game1.Screens
{
	public class GameScreen : Component
	{
		private GamePlayManager _gameplay;

		public GameScreen(Rectangle bounds, string playerId): base(bounds, true, background: null)
		{
			// Again, all of this stuff should preload on a "Loading" screen instead of realtime doing this while the player waits...
			_activator.Register(_gameplay = new GamePlayManager(bounds, playerId), true, "game");
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_gameplay.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_gameplay.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_gameplay.Update(gameTime);
			base.UpdateActive(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{	
			base.DrawInternal(spriteBatch);
			_gameplay.Draw(spriteBatch);
		}

		protected override void _dialog_OnItemSelect(object sender, ComponentEventArgs e)
		{
			// Eventually we will care what got clicked here...
			_activator.SetState(_gameplay, true);
		}

		protected override void ReadyDisable(ComponentEventArgs e)
		{
			ShowNotification("Game Paused", this.Bounds, "game");
		}
	}
}
