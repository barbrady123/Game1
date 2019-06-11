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
		private ComponentManager _components;
		private GamePlayManager _gameplay;
		private readonly Dialog _dialog;

		public GameScreen(Rectangle bounds): base(bounds, background: "rock")
		{
			_components = new ComponentManager();

			_components.Register(_gameplay = new GamePlayManager(bounds));
			_gameplay.OnReadyDisable += _gameplay_OnReadyDisable;
			_components.Register(_dialog = new Dialog("Paused", DialogButton.Ok, bounds.CenteredRegion(400, 200), null));
			_dialog.OnItemSelect += _dialog_OnItemSelect;
			_dialog.OnReadyDisable += _dialog_OnReadyDisable;

			_components.SetState(_gameplay, ComponentState.All, ComponentState.None);
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

		public override void Update(GameTime gameTime)
		{
			_dialog.Update(gameTime);
			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_gameplay.Update(gameTime);
			base.UpdateActive(gameTime);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{	
			var batchData = SpriteBatchManager.Get("modal");
			batchData.ScissorWindow = _dialog.Bounds;
			_dialog.Draw(batchData.SpriteBatch);
			base.DrawVisible(spriteBatch);
			_gameplay.Draw(spriteBatch);
		}

		private void _dialog_OnItemSelect(object sender, ComponentEventArgs e)
		{
			// Eventually we will care what got clicked here...
			_components.SetState(_gameplay, ComponentState.All, ComponentState.None);
		}

		private void _dialog_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_components.SetState(_gameplay, ComponentState.All, ComponentState.None);
		}

		private void _gameplay_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_components.SetState(_dialog, ComponentState.All, ComponentState.Visible);
		}
	}
}
