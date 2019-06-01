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
	public class GameScreen : Screen
	{
		private GamePlayManager _gameplay;
		private readonly Dialog _dialog;
		private readonly ActivationManager _activation;

		public GameScreen(Rectangle bounds): base(bounds, "rock")
		{
			_activation = new ActivationManager();
			_activation.Add(_gameplay = new GamePlayManager(bounds) { IsActive = true });
			// Dialog
			_activation.Add(_dialog = new Dialog("Paused", DialogButton.Ok, bounds.CenteredRegion(400, 200), null));
			_dialog.OnButtonClick += _dialogBox_OnButtonClick;
			_dialog.OnReadyDisable += _dialogBox_OnButtonClick;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_gameplay.LoadContent();
			_dialog.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_gameplay.UnloadContent();
			_dialog.UnloadContent();
		}

		public override void Update(GameTime gameTime, bool processInput)
		{
			base.Update(gameTime, processInput);
			_gameplay.Update(gameTime);
			_dialog.Update(gameTime, processInput);

			if (InputManager.KeyPressed(Keys.Escape))
				_activation.Activate(_dialog);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{	
			base.Draw(spriteBatch);
			_gameplay.Draw(spriteBatch);
			_dialog.Draw(spriteBatch);
		}

		private void _dialogBox_OnButtonClick(object sender, EventArgs e)
		{
			_activation.Activate(_gameplay);
		}
	}
}
