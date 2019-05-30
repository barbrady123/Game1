using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Game1.Enum;

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
		public const int SpriteSheetDefaultFrame = 0;
		public const Cardinal SpriteSheetDefaultDirection = Cardinal.South;
		public const int GameViewAreaWidth = 15;
		public const int GameViewAreaHeight = 15;
		public const int PlayerDrawIndex = 100;

		GraphicsDeviceManager _graphicsManager;
		//SpriteBatch _spriteBatch;
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
			GameRandom.InitializeSeed(9872343);
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
			// General use...
			SpriteBatchManager.Add(new SpriteBatch(GraphicsDevice), null, 100, "general");
			// Gameplay camera drawing...
			var gameplayBatch = new SpriteBatch(GraphicsDevice);
			gameplayBatch.GraphicsDevice.ScissorRectangle = new Rectangle(28, 28, 960, 960);
			SpriteBatchManager.Add(new SpriteBatch(GraphicsDevice), new RasterizerState { ScissorTestEnable = true }, 200, "gameplay");

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
			foreach (var data in SpriteBatchManager.GetAllDataSorted())
			{
				data.SpriteBatch.Begin(samplerState: SamplerState.LinearWrap, rasterizerState: data.RasterizeState);
			}
			
			// General use one is used in the main Draw pipeline...
			_screenManager.Draw(SpriteBatchManager.Get("general"));

			foreach (var data in SpriteBatchManager.GetAllDataSorted())
			{
				data.SpriteBatch.End();
			}

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
