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
using Game1.Effect;
using Game1.Screens;
using Game1.Screens.Menu;
using Game1.Screens.Menu.Character;

namespace Game1
{
	public class ScreenManager
	{
		private ContentManager _content;
		private Component _currentScreen;
		private Component _newScreen;
		private bool _isTransitioning;

		private ImageTexture _transitionImage;
		private FadeOutEffect _fadeOutEffect;
		private FadeInEffect _fadeInEffect;

		private Rectangle _bounds;

		public ScreenManager(Rectangle bounds)
		{
			_bounds = bounds;
			_isTransitioning = false;
			_transitionImage = new ImageTexture($"{Game1.BackgroundRoot}/black") { Scale = new Vector2(_bounds.Width, _bounds.Height) };
			_transitionImage.Position = bounds.TopLeftVector();
			_fadeOutEffect = _transitionImage.AddEffect<FadeOutEffect>(false);
			_fadeOutEffect.Speed = 3.0f;
			_fadeInEffect = _transitionImage.AddEffect<FadeInEffect>(false);
			_fadeInEffect.Speed = 3.0f;
			_fadeInEffect.OnActiveChange += _fadeInEffect_OnActiveChange;
			_fadeOutEffect.OnActiveChange += _fadeOutEffect_OnActiveChange;
		}

		public void LoadContent()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);	
			_transitionImage.LoadContent();
		}

		public void UnloadContent()
		{
			_content?.Unload();
			_transitionImage.UnloadContent();
			_currentScreen?.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			if (_isTransitioning)
			{
				_transitionImage.Update(gameTime);
				InputManager.BlockAllInput();
			}

			if (_currentScreen != null)
				_currentScreen.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_currentScreen?.Draw(spriteBatch);
			if (_isTransitioning)
				_transitionImage.Draw(spriteBatch);
		}

		public void StartScreen()
		{
			//TransitionScreens(new SplashScreen(_bounds));
			//TransitionScreens(new MainMenu(_bounds));
			//TransitionScreens(new CharacterCreateScreen(_bounds));
			TransitionScreens(new GameScreen(_bounds));
		}

		private void TransitionScreens(Component newScreen)
		{
			if (_isTransitioning)
				return;

			_isTransitioning = true;
			_transitionImage.IsActive = true;
			_newScreen = newScreen;

			// Fade-in black (fade out current screen)...
			if (_currentScreen != null)
			{
				_transitionImage.Alpha = 0.0f;
				_fadeInEffect.IsActive = true;
			}
			else
			{
				// Fade-out black (fade in new screen)...
				_transitionImage.Alpha = 1.0f;
				_fadeOutEffect.IsActive = true;
				SetNewScreenToCurrent();
			}
		}

		private void SetNewScreenToCurrent()
		{
			_currentScreen?.UnloadContent();
			_currentScreen = _newScreen;
			_currentScreen.IsActive = true;
			_newScreen = null;
			_currentScreen.OnReadyDisable += _currentScreen_OnReadyDisable;
			if (_currentScreen is MenuScreen menuScreen)
				menuScreen.OnItemSelect += MenuScreen_OnItemSelect;

			_currentScreen.LoadContent();
		}

		private void _fadeInEffect_OnActiveChange(object sender, EventArgs e)
		{
			if (_isTransitioning && !((EffectEventArgs)e).IsActive)
			{
				// Fade-out black (fade in new screen)...
				SetNewScreenToCurrent();
				_fadeOutEffect.IsActive = true;
			}
		}

		private void _fadeOutEffect_OnActiveChange(object sender, EventArgs e)
		{
			if (_isTransitioning && !((EffectEventArgs)e).IsActive)
			{
				_isTransitioning = false;
				_transitionImage.IsActive = false;		
			}
		}

		// Really, we should have a menu screen wrapper so we don't get direct menu screen events....
		private void MenuScreen_OnItemSelect(object sender, ComponentEventArgs e)
		{
			switch (sender)
			{
				case MainMenu _ : switch (e.Value)
				{
					case "startnewgame":	TransitionScreens(new CharacterCreateScreen(_bounds));	break;
					case "options":			TransitionScreens(new OptionsMenu(_bounds));			break;
					case "exitgame":		Game1.Instance.Exit();									break;
					default: break;
				}
				break;
				case OptionsMenu _ : switch (e.Value)
				{
					case "back":	TransitionScreens(new MainMenu(_bounds));	break;
				}
				break;
			}
		}

		private void _currentScreen_OnReadyDisable(object sender, ComponentEventArgs e)
		{	
			switch (sender)
			{
				case SplashScreen _ : switch (e.Trigger)
				{
					case EventTrigger.ButtonClick:
					case EventTrigger.KeyPressed :	TransitionScreens(new MainMenu(_bounds));	break;
					case EventTrigger.Escape :		Game1.Instance.Exit();						break;
				}
				break;
				case MainMenu _ : switch (e.Value)
				{
					case "startnewgame":	TransitionScreens(new CharacterCreateScreen(_bounds));	break;
					case "options":			TransitionScreens(new OptionsMenu(_bounds));			break;
					case "exitgame":		Game1.Instance.Exit();									break;
					default: break;
				}
				break;
				case OptionsMenu _ : switch (e.Value)
				{
					case "back":	TransitionScreens(new MainMenu(_bounds));	break;
				}
				break;
				case CharacterCreateScreen _ : switch (e.Value)
				{
					case "escape" :
					case "cancel" : TransitionScreens(new MainMenu(_bounds));		break;
					// This should NOT go directly to game screen...we need a "loading" transition screen with a call back (probably just part of the ScreenManager)....
					case "startgame" : TransitionScreens(new GameScreen(_bounds));	break;
				}
				break;
			}
		}
	}
}
