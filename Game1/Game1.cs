using System;
using System.IO;
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
		public static readonly string GameName = typeof(Game1).Name;
		public static readonly string GameStorageRoot = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{Game1.GameName}";
		public static readonly string PlayerFile = $"{Game1.GameStorageRoot}\\player";
		public static readonly string MapFile = $"{Game1.MapRoot}\\map";
		public const string ContentRoot = "Content";
		public const string FontsRoot = "Fonts";
		public const string BackgroundRoot = "Background";
		public const string MenuConfigRoot = "Load\\Menu";
		public const string MapRoot = "Load\\Map";
		public const string TilesheetRoot = "Gameplay\\TileSheet";
		public const string SpriteSheetRoot = "Gameplay\\Character";
		public const int TileSize = 64;
		public const int TileSheetSize = 10;
		public const int SpriteSheetWalkFrameCount = 9;

		GraphicsDeviceManager _graphicsManager;
		SpriteBatch _spriteBatch;
		ScreenManager _screenManager;
		GameConfiguration _config;

		public static Game Instance { get; private set; }

		public static IServiceProvider ServiceProvider { get; private set; }

		public static GraphicsDevice Graphics { get; private set; }

		public Game1()
		{
			Game1.Instance = this;
			_graphicsManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = Game1.ContentRoot;
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
			_graphicsManager.PreferredBackBufferWidth = _config.WindowWidth;
			_graphicsManager.PreferredBackBufferHeight = _config.WindowHeight;
			_graphicsManager.ApplyChanges();
			Game1.Graphics = GraphicsDevice;
			_screenManager = new ScreenManager(GraphicsDevice.Viewport.Bounds);
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			Game1.ServiceProvider = Content.ServiceProvider;
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_screenManager.LoadContent();
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
			InputManager.Update();
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
