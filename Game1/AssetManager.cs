using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game1.Enum;
using Game1.Maps;

namespace Game1
{
	public static class AssetManager
	{
		public const string UntrackedAssetName = "__UNTRACKED_ASSET__";

		// Eventually we could create either a new contentmanager or an extension method to wrap the calls that require all this Path.combine junk in the Load<> method...
		private const string GameplayRoot = "Gameplay";
		private const string InterfaceRoot = "Interface";
		private const string IconRoot = GameplayRoot + "\\Icon";
		private const string IconEmptyRoot = IconRoot + "\\Empty";
		private const string TilesheetRoot = GameplayRoot + "\\TileSheet";
		private const string CharacterSpriteSheetRoot = GameplayRoot + "\\Character";
		private const string MobSpriteSheetRoot = GameplayRoot + "\\Mob";
		private const string TransitionRoot = GameplayRoot + "\\Transition";
		private const string StatusIconRoot = GameplayRoot + "\\Status";
		private const string InteractiveIconRoot = GameplayRoot + "\\Interactive";

		private static IServiceProvider _serviceProvider;
		private static GraphicsDevice _graphicsDevice;
		private static string  _contentRoot;

		private static ContentManager _contentGame;
		private static string _playerSpriteSheetName;
		private static Texture2D _playerSpriteSheetHighlight;
		private static Dictionary<string, Texture2D> _iconItemHighlight;

		private static ContentManager _contentMap;
		private static Dictionary<string, Texture2D> _spriteSheetHighlight;
		private static Dictionary<string, Texture2D> _iconInteractiveHighlight;
		private static Dictionary<string, Texture2D> _iconTransitionHighlight;

		private static ContentManager _contentScreen;

		public static void LoadContent(IServiceProvider serviceProvider, GraphicsDevice graphicsDevice, string contentRoot)
		{	
			// Game (app) content...
			_serviceProvider = serviceProvider;
			_graphicsDevice = graphicsDevice;
			_contentRoot = contentRoot;
			_contentGame = new ContentManager(_serviceProvider, _contentRoot);
			_iconItemHighlight = new Dictionary<string, Texture2D>();
			LoadGameAssets();

			// Map content...
			_contentMap = new ContentManager(_serviceProvider, _contentRoot);
			_spriteSheetHighlight = new Dictionary<string, Texture2D>();
			_iconInteractiveHighlight = new Dictionary<string, Texture2D>();
			_iconTransitionHighlight = new Dictionary<string, Texture2D>();

			// Screen content...
			_contentScreen = new ContentManager(_serviceProvider, _contentRoot);
		}

		public static void UnloadContent()
		{
			_contentScreen.Unload();
			DisposeMapContent();
			DisposeGameContent();
		}

		private static void DisposeMapContent()
		{			
			_contentMap.Unload();
			foreach (var h in _spriteSheetHighlight.Values) h.Dispose();
			_spriteSheetHighlight.Clear();
			foreach (var h in _iconInteractiveHighlight.Values) h.Dispose();
			_iconInteractiveHighlight.Clear();
			foreach (var h in _iconTransitionHighlight.Values) h.Dispose();
			_iconTransitionHighlight.Clear();
		}

		private static void DisposeGameContent()
		{
			_contentGame.Unload();
			foreach (var h in _iconItemHighlight.Values) h.Dispose();
			_playerSpriteSheetHighlight?.Dispose();
		}

		public static ImageTexture GetBackground(string name) => new ImageTexture(_contentScreen.Load<Texture2D>(Path.Combine(Game1.BackgroundRoot, name)), ImageAlignment.Centered, true);

		public static Texture2D GetScreenTexture(string path) => _contentScreen.Load<Texture2D>(path);

		public static void UnloadScreenContent()
		{
			_contentScreen.Unload();
		}

		public static void LoadPlayerAssets(Character player)
		{
			string spriteSheet = player.SpriteSheetName.ToLower();
			var standard = _contentGame.Load<Texture2D>(Path.Combine(CharacterSpriteSheetRoot, spriteSheet));

			if (_playerSpriteSheetName != spriteSheet)
			{
				_playerSpriteSheetHighlight = GenerateHighlightTexture(standard);
				_playerSpriteSheetName = spriteSheet;
			}

			player.SpriteSheet = new ImageSpriteSheet(standard, _playerSpriteSheetHighlight, Game1.TileSize, true);
		}

		public static void LoadMapAssets(Map map)
		{
			DisposeMapContent();
			LoadMapTileSheets(map);
			LoadMapSpriteSheets(map);
			LoadMapIcons(map);
		}

		public static Texture2D GetInterfaceElement(string name) => _contentGame.Load<Texture2D>(Path.Combine(InterfaceRoot, name));

		public static Texture2D GetTileSheet(string id) => _contentMap.Load<Texture2D>(Path.Combine(TilesheetRoot, id));

		public static ImageSpriteSheet GetSpriteSheet(string id)
		{
			if (!_spriteSheetHighlight.TryGetValue(id, out Texture2D highlight))
				throw new Exception($"No spritesheet highlight with name {id} found!");

			var spriteSheet = new ImageSpriteSheet(_contentMap.Load<Texture2D>(Path.Combine(CharacterSpriteSheetRoot, id)), highlight, Game1.TileSize, true);
			spriteSheet.SetFrame(0);
			return spriteSheet;
		}

		public static ImageTexture GetIconStatus(string iconName) => new ImageTexture(_contentGame.Load<Texture2D>(Path.Combine(StatusIconRoot, iconName)), null, true);

		public static ImageTexture GetIconItem(string iconName)
		{
			if (!_iconItemHighlight.TryGetValue(iconName, out Texture2D highlight))
				throw new Exception($"No icon highlight with name {iconName} found!");

			return new ImageTexture(_contentGame.Load<Texture2D>(Path.Combine(IconRoot, iconName)), highlight, ImageAlignment.Centered, true);
		}

		public static ImageTexture GetIconEmptySlot(string iconName) => new ImageTexture(_contentGame.Load<Texture2D>(Path.Combine(IconEmptyRoot, iconName)), ImageAlignment.Centered, true);

		public static ImageTexture GetIconInteractive(string iconName)
		{
			if (!_iconInteractiveHighlight.TryGetValue(iconName, out Texture2D highlight))
				throw new Exception($"No icon highlight with name {iconName} found!");

			return new ImageTexture(_contentMap.Load<Texture2D>(Path.Combine(InteractiveIconRoot, iconName)), highlight, ImageAlignment.Centered, true);
		}

		public static ImageTexture GetIconTransition(string iconName)
		{
			if (!_iconTransitionHighlight.TryGetValue(iconName, out Texture2D highlight))
				throw new Exception($"No icon highlight with name {iconName} found!");

			return new ImageTexture(_contentMap.Load<Texture2D>(Path.Combine(TransitionRoot, iconName)), highlight, ImageAlignment.Centered, true);
		}

		private static void LoadGameAssets()
		{
			// Interface elements....
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, InterfaceRoot)))
				_contentGame.Load<Texture2D>(Path.Combine(InterfaceRoot, file));

			// Item icons...
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, IconRoot)))
				_iconItemHighlight[file] = GenerateHighlightTexture(_contentGame.Load<Texture2D>(Path.Combine(IconRoot, file)));

			// Status icons...
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, StatusIconRoot)))
				_contentGame.Load<Texture2D>(Path.Combine(StatusIconRoot, file));

			// Empty equipment slot icons...
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, IconEmptyRoot)))
				_contentGame.Load<Texture2D>(Path.Combine(IconEmptyRoot, file));
		}

		private static void LoadMapSpriteSheets(Map map)
		{
			// TODO: We need npc/mob metadata before we can really do this correctly...
			// Should be able to pull npc/mob ids out of map file and load the correct spritesheets for each (plus need player character's spritesheet always)
			// For now we'll just load male/female from the Sex enum since we know that's everything
			var mapSpriteSheets = new HashSet<string>();
			foreach (CharacterSex sex in (CharacterSex[])System.Enum.GetValues(typeof(CharacterSex)))
				mapSpriteSheets.Add(sex.ToString("g").ToLower());

			foreach (string spriteSheet in mapSpriteSheets)
				_spriteSheetHighlight[spriteSheet] = GenerateHighlightTexture(_contentMap.Load<Texture2D>(Path.Combine(CharacterSpriteSheetRoot, spriteSheet)));
		}

		private static void LoadMapTileSheets(Map map)
		{
			foreach (string tileSheet in map.Layers.Select(l => l.TileSheet.ToLower()).Where(l => !String.IsNullOrWhiteSpace(l)).Distinct())
				_contentMap.Load<Texture2D>(Path.Combine(TilesheetRoot, tileSheet));
		}

		private static Texture2D GenerateHighlightTexture(Texture2D texture)
		{
			var data = new Color[texture.Width * texture.Height];
			texture.GetData(data);
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].A > 0)
					data[i] = Color.White;
			}

			var highlight = new Texture2D(_graphicsDevice, texture.Width, texture.Height);
			highlight.SetData(data);
			return highlight;
			// We might want to just generate the entire highlight texture at this point (RenderTarget the 4 draw below)...
			// Long-long term, we could just write a seperate script to generate those and keep/load them with the textures to begin with...
			// Also, sharing these across ImageTextures with the same underlying texture would save RAM...
		}

		private static void LoadMapIcons(Map map)
		{
			foreach (var interactive in map.Interactives)
				_iconInteractiveHighlight[interactive.Id] = GenerateHighlightTexture(_contentMap.Load<Texture2D>(Path.Combine(InteractiveIconRoot, interactive.Id)));

			foreach (var transition in map.Transitions)
				_iconTransitionHighlight[transition.Id] = GenerateHighlightTexture(_contentMap.Load<Texture2D>(Path.Combine(TransitionRoot, transition.Id)));
		}
	}
}
