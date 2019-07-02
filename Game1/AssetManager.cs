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
		private const string GameplayRoot = "Gameplay";
		private const string InterfaceRoot = "Interface";
		private const string IconRoot = GameplayRoot + "\\Icon";
		private const string IconEmptyRoot = IconRoot + "\\Empty";
		private const string TilesheetRoot = GameplayRoot + "\\TileSheet";
		private const string SpriteSheetRoot = GameplayRoot + "\\Character";
		private const string TransitionRoot = GameplayRoot + "\\Transition";
		private const string StatusIconRoot = GameplayRoot + "\\Status";
		private const string InteractiveIconRoot = GameplayRoot + "\\Interactive";

		private static IServiceProvider _serviceProvider;
		private static GraphicsDevice _graphicsDevice;
		private static string  _contentRoot;

		private static ContentManager _contentGame;
		private static string _playerSpriteSheetName;
		private static (Texture2D standard, Texture2D highlight) _playerSpriteSheet;
		private static Dictionary<string, Texture2D> _interfaceElements;
		private static Dictionary<string, (Texture2D standard, Texture2D highlight)> _iconItems;
		private static Dictionary<string, Texture2D> _iconStatuses;
		private static Dictionary<string, Texture2D> _iconEmptySlots;

		private static ContentManager _contentMap;
		private static Dictionary<string, Texture2D> _tileSheets;
		private static Dictionary<string, (Texture2D standard, Texture2D highlight)> _spriteSheets;
		private static Dictionary<string, (Texture2D standard, Texture2D highlight)> _iconInteractives;
		private static Dictionary<string, (Texture2D standard, Texture2D highlight)> _iconTransitions;

		public static void LoadContent(IServiceProvider serviceProvider, GraphicsDevice graphicsDevice, string contentRoot)
		{
			_serviceProvider = serviceProvider;
			_graphicsDevice = graphicsDevice;
			_contentRoot = contentRoot;
			_contentGame = new ContentManager(_serviceProvider, _contentRoot);
			_interfaceElements = new Dictionary<string, Texture2D>();
			_iconItems = new Dictionary<string, (Texture2D standard, Texture2D highlight)>();
			_iconStatuses = new Dictionary<string, Texture2D>();
			_iconEmptySlots = new Dictionary<string, Texture2D>();
			LoadGameAssets();
			_contentMap = new ContentManager(_serviceProvider, _contentRoot);
			_tileSheets = new Dictionary<string, Texture2D>();
			_spriteSheets = new Dictionary<string, (Texture2D standard, Texture2D highlight)>();
			_iconInteractives = new Dictionary<string, (Texture2D standard, Texture2D highlight)>();
			_iconTransitions = new Dictionary<string, (Texture2D standard, Texture2D highlight)>();
		}

		public static void UnloadContent()
		{
			_contentMap?.Unload();
			_contentGame?.Unload();
		}

		private static void LoadGameAssets()
		{
			// Interface elements....
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, InterfaceRoot)))
			{
				string fileName = Path.GetFileNameWithoutExtension(file);
				_interfaceElements[fileName] = _contentGame.Load<Texture2D>(Path.Combine(InterfaceRoot, fileName));
			}

			// Item icons...
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, IconRoot)))
			{				
				string fileName = Path.GetFileNameWithoutExtension(file);
				var standard = _contentGame.Load<Texture2D>(Path.Combine(IconRoot, fileName));
				var highlight = GenerateHighlightTexture(standard);
				_iconItems[fileName] = (standard, highlight);
			}

			// Status icons...
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, StatusIconRoot)))
			{				
				string fileName = Path.GetFileNameWithoutExtension(file);
				_iconStatuses[fileName] = _contentGame.Load<Texture2D>(Path.Combine(StatusIconRoot, fileName));
			}

			// Empty equipment slot icons...
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(_contentRoot, IconEmptyRoot)))
			{
				string fileName = Path.GetFileNameWithoutExtension(file);
				_iconEmptySlots[fileName] = _contentGame.Load<Texture2D>(Path.Combine(IconEmptyRoot, fileName));
			}
		}

		public static void LoadPlayerAssets(Character player)
		{
			string spriteSheet = player.SpriteSheetName.ToLower();

			if (_playerSpriteSheetName != spriteSheet)
			{
				var standard = _contentGame.Load<Texture2D>(Path.Combine(SpriteSheetRoot, spriteSheet));
				var highlight = GenerateHighlightTexture(standard);
				_playerSpriteSheet = (standard, highlight);
				_playerSpriteSheetName = spriteSheet;
			}

			player.SpriteSheet = new ImageSpriteSheet(_playerSpriteSheet.standard, _playerSpriteSheet.highlight, true);
		}

		public static void LoadMapAssets(Map map)
		{
			_tileSheets.Clear();
			_spriteSheets.Clear();
			_iconInteractives.Clear();
			_iconTransitions.Clear();
			_contentMap.Unload();
			LoadMapTileSheets(map);
			LoadMapSpriteSheets(map);
			LoadMapIcons(map);
		}

		public static Texture2D GetInterfaceElement(string name)
		{
			if (!_interfaceElements.TryGetValue(name, out Texture2D texture))
				throw new Exception($"No interface element with name {name} found!");

			return _interfaceElements[name];
		}

		public static Texture2D GetTileSheet(string id) => _tileSheets[id];

		public static ImageSpriteSheet GetSpriteSheet(string id)
		{
			if (!_spriteSheets.ContainsKey(id))
				LoadSpriteSheet(id);

			(var general, var highlight) = _spriteSheets[id];
			return new ImageSpriteSheet(general, highlight, true);
		}

		public static ImageTexture GetIconStatus(string iconName)
		{
			if (!_iconStatuses.TryGetValue(iconName, out Texture2D texture))
				throw new Exception($"No status icon with name {iconName} found!");

			return new ImageTexture(texture, true);
		}

		public static ImageTexture GetIconItem(string iconName)
		{
			if (!_iconItems.TryGetValue(iconName, out (Texture2D standard, Texture2D highlight) textures))
				throw new Exception($"No item icon with name {iconName} found!");

			return new ImageTexture(textures.standard, textures.highlight, true) { Alignment = ImageAlignment.Centered };
		}

		public static ImageTexture GetIconEmptySlot(string iconName)
		{
			if (!_iconEmptySlots.TryGetValue(iconName, out Texture2D texture))
				throw new Exception($"No empty slot icon with name {iconName} found!");

			return new ImageTexture(texture, true) { Alignment = ImageAlignment.Centered };
		}

		public static ImageTexture GetIconInteractive(string iconName)
		{
			if (!_iconInteractives.TryGetValue(iconName, out (Texture2D standard, Texture2D highlight) textures))
				throw new Exception($"No interactive icon with name {iconName} found!");

			return new ImageTexture(textures.standard, textures.highlight, true) { Alignment = ImageAlignment.Centered };
		}

		public static ImageTexture GetIconTransition(string iconName)
		{
			if (!_iconTransitions.TryGetValue(iconName, out (Texture2D standard, Texture2D highlight) textures))
				throw new Exception($"No transition icon with name {iconName} found!");

			return new ImageTexture(textures.standard, textures.highlight, true) { Alignment = ImageAlignment.Centered };
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
				if (!_spriteSheets.ContainsKey(spriteSheet))
					LoadSpriteSheet(spriteSheet);
		}

		private static void LoadMapTileSheets(Map map)
		{
			foreach (string tileSheet in map.Layers.Select(l => l.TileSheet.ToLower()).Where(l => !String.IsNullOrWhiteSpace(l)).Distinct())
				if (!_tileSheets.ContainsKey(tileSheet))
					_tileSheets[tileSheet] = _contentMap.Load<Texture2D>(Path.Combine(TilesheetRoot, tileSheet));
		}

		private static void LoadSpriteSheet(string name)
		{
			if (_spriteSheets.ContainsKey(name))
				return;

			var standard = _contentMap.Load<Texture2D>(Path.Combine(SpriteSheetRoot, name));
			var highlight = GenerateHighlightTexture(standard);
			_spriteSheets[name] = (standard, highlight);
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
			{
				if (!_iconInteractives.ContainsKey(interactive.Id))
				{
					var standard = _contentMap.Load<Texture2D>(Path.Combine(InteractiveIconRoot, interactive.Id));
					var highlight = GenerateHighlightTexture(standard);
					_iconInteractives[interactive.Id] = (standard, highlight);
				}
			}

			foreach (var transition in map.Transitions)
			{
				if (!_iconTransitions.ContainsKey(transition.Id))
				{
					var standard = _contentMap.Load<Texture2D>(Path.Combine(TransitionRoot, transition.Id));
					var highlight = GenerateHighlightTexture(standard);
					_iconTransitions[transition.Id] = (standard, highlight);
				}
			}
		}
	}
}
