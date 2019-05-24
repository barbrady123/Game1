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

namespace Game1
{
	public class ScreenManager
	{
		private IServiceProvider _services;
		private ContentManager _content;
		private GameScreen _currentScreen;
		private GameScreen _newScreen;
		private GraphicsDevice _graphics;
		private bool _isTransitioning;

		private Image _transitionImage;
		private FadeOutEffect _fadeOutEffect;
		private FadeInEffect _fadeInEffect;

		public Vector2 ScreenSize { get; private set; }

		public ScreenManager(GraphicsDevice graphics)
		{
			_graphics = graphics;
			this.ScreenSize = new Vector2(graphics.Viewport.Width, graphics.Viewport.Height);
			_isTransitioning = false;
			_transitionImage = new Image(graphics, "Background/black") { Scale = this.ScreenSize };
			_transitionImage.Effects.Add(_fadeOutEffect = new FadeOutEffect(_transitionImage) { Speed = 3.0f });
			_transitionImage.Effects.Add(_fadeInEffect = new FadeInEffect(_transitionImage) { Speed = 2.0f });
			_fadeInEffect.OnActiveChange += _fadeInEffect_OnActiveChange;
			_fadeOutEffect.OnActiveChange += _fadeOutEffect_OnActiveChange;
		}

		public void LoadContent(IServiceProvider services)
		{
			_services = services;
			_content = new ContentManager(services, "Content");	
			_transitionImage.LoadContent(services);
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
			TransitionScreens(new SplashScreen(_graphics, this.ScreenSize));
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
			_currentScreen.LoadContent(_services);
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

			// TODO: This whole logic structure needs to be redone and generalized...
			if (args.Type == "change")
			{
				switch (args.Target)
				{
					case "MainMenu":	TransitionScreens(new MainMenu(_graphics, this.ScreenSize));	break;
					case "OptionsMenu":	TransitionScreens(new OptionsMenu(_graphics, this.ScreenSize));	break;
				}
			}
		}
	}
}
