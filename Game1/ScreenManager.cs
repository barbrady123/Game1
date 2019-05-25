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
using Game1.Screen;
using Game1.Screen.Menu;
using Game1.Screen.Menu.Character;

namespace Game1
{
	public class ScreenManager
	{
		private ContentManager _content;
		private GameScreen _currentScreen;
		private GameScreen _newScreen;
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
			_transitionImage.SourceRect = new Rectangle(0, 0, bounds.Width, bounds.Height);
			_transitionImage.Effects.Add(_fadeOutEffect = new FadeOutEffect(_transitionImage) { Speed = 3.0f });
			_transitionImage.Effects.Add(_fadeInEffect = new FadeInEffect(_transitionImage) { Speed = 2.0f });
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
		}

		private void TransitionScreens(GameScreen newScreen)
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

		private void _currentScreen_OnReadyScreenUnload(object sender, EventArgs e)
		{
			var args = (ScreenEventArgs)e;

			if (args.Type == "change")
			{
				switch (args.Target)
				{
					case "MainMenu":		TransitionScreens(new MainMenu(_bounds));						break;
					case "OptionsMenu":		TransitionScreens(new OptionsMenu(_bounds));					break;
					case "CharacterCreate": TransitionScreens(new CharacterCreateScreen(_bounds));		break;
					case "SexMenu":			TransitionScreens(new SexMenu(_bounds));	break;
				}
			}
		}
	}
}
