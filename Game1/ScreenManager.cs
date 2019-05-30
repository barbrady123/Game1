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
using Game1.Screens;
using Game1.Screens.Menu;
using Game1.Screens.Menu.Character;

namespace Game1
{
	public class ScreenManager
	{
		private ContentManager _content;
		private Screen _currentScreen;
		private Screen _newScreen;
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
			_transitionImage.DrawArea = bounds;
			_fadeOutEffect = _transitionImage.AddEffect(new FadeOutEffect() { Speed = 3.0f });
			_fadeInEffect = _transitionImage.AddEffect(new FadeInEffect() { Speed = 2.0f });
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
			if (_content != null)
				_content.Unload();
			_transitionImage.UnloadContent();
			_currentScreen?.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			_currentScreen?.Update(gameTime, !_isTransitioning);
			if (_isTransitioning)
				_transitionImage.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_currentScreen?.Draw(spriteBatch);
			if (_isTransitioning)
				_transitionImage.Draw(spriteBatch);
		}

		public void StartScreen()
		{
			TransitionScreens(new SplashScreen(_bounds));
			//TransitionScreens(new CharacterCreateScreen(_bounds));
		}

		private void TransitionScreens(Screens.Screen newScreen)
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
			_newScreen = null;
			_currentScreen.OnReadyScreenUnload += _currentScreen_OnReadyScreenUnload;
			if (_currentScreen is MenuScreen menuScreen)
			{				
				menuScreen.OnItemSelect += MenuScreen_OnItemSelect;
				menuScreen.OnReadyDisable += MenuScreen_OnReadyDisable;
				menuScreen.IsActive = true;
			}

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

		private void MenuScreen_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			if (args.Type == "select")
			{
				switch (args.Source)
				{
					case "MainMenu" : switch (args.Item)
					{
						case "startnewgame":	TransitionScreens(new CharacterCreateScreen(_bounds));	break;
						case "options":			TransitionScreens(new OptionsMenu(_bounds));			break;
						case "exitgame":		Game1.Instance.Exit();									break;
						default: break;
					}
					break;
					case "OptionsMenu" : switch (args.Item)
					{
						case "back":	TransitionScreens(new MainMenu(_bounds));	break;
						case "sex":		TransitionScreens(new SexMenu(_bounds));	break;
					}
					break;
				}
			}
		}

		private void _currentScreen_OnReadyScreenUnload(object sender, EventArgs e)
		{
			var args = (ScreenEventArgs)e;

			switch (args.Source)
			{
				case "SplashScreen": switch (args.Type)
				{
					case "continue":	TransitionScreens(new MainMenu(_bounds));	break;
					case "exit" :		Game1.Instance.Exit();						break;
				}
				break;
				case "CharacterCreateScreen": switch (args.Type)
				{
					case "back" : TransitionScreens(new MainMenu(_bounds));		break;
					// This should NOT go directly to game screen...we need a "loading" transition screen with a call back (probably just part of the ScreenManager)....
					case "game" : TransitionScreens(new GameScreen(_bounds));	break;
				}
				break;
			}
		}

		private void MenuScreen_OnReadyDisable(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;

			switch (args.Source)
			{
				case "MainMenu": switch (args.Type)
				{
					case "escape":	Game1.Instance.Exit();	break;
				}
				break;
			}
		}
	}
}
