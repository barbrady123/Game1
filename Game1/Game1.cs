﻿using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Game1
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;
		ScreenManager _screenManager;
		GameConfiguration _config;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			LoadConfiguration();
			_graphics.PreferredBackBufferWidth = _config.WindowWidth;
			_graphics.PreferredBackBufferHeight = _config.WindowHeight;
			_graphics.ApplyChanges();
			_screenManager = new ScreenManager(GraphicsDevice);
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_screenManager.LoadContent(Content.ServiceProvider);
			_screenManager.StartScreen();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			_screenManager.UnloadContent();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// TODO: Get rid of this generic hook at some point...
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			InputManager.Instance.Update();
			_screenManager.Update(gameTime);
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin();
			_screenManager.Draw(_spriteBatch);
			_spriteBatch.End();
			base.Draw(gameTime);
		}

		private void LoadConfiguration()
		{
			using (var reader = new StreamReader("config.json"))
			{
				_config = JsonConvert.DeserializeObject<GameConfiguration>(reader.ReadToEnd());
			}
		}
	}
}
