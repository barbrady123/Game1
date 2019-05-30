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

namespace Game1
{
	// TODO : Move this when complete...
	public class CharacterRenderData
	{
		public Character Character { get; set; }
		public ImageTexture SpriteSheet { get; set; }	// TODO : Eventually these should  be a seperate collection and we point to an ID here, to prevent loading duplicate sprite sheets...
		public Vector2 PreviousPosition { get; set; }
		public SpriteSheetEffect Animation { get; set; }
	}

	public class GamePlayCamera
	{
		private readonly World _world;
		private readonly Rectangle _gameViewArea;
		private ImageTexture _terrainTileSheet;
		//private ImageTexture _playerSpriteSheet;
		//private Vector2 _previousPlayerPosition;
		private Rectangle _terrainSourceRect;
		//private SpriteSheetEffect _playerAnimation;
		private List<ImageTexture> _terrainMaps;
		private Dictionary<Character, CharacterRenderData> _renderData;
		private CharacterRenderData _playerRenderData;
		
		public string TerrainTileSheetName { get; set; }
		public Layer[] TerrainLayerData{ get; set; }

		public GamePlayCamera(World world, Rectangle gameViewArea)
		{
			_world = world;
			_gameViewArea = gameViewArea;
			_terrainSourceRect = Rectangle.Empty;
			_playerRenderData = new CharacterRenderData {
				Character = _world.Character,
				Animation = new SpriteSheetEffect(false),
				PreviousPosition = -Vector2.One
			};
			_renderData = new Dictionary<Character, CharacterRenderData>();
			foreach (var npc in _world.NPCs)
			{
				_renderData[npc] = new CharacterRenderData {
					Character = npc,
					Animation = new SpriteSheetEffect(false),
					PreviousPosition = -Vector2.One
				};
			}
		}

		public Vector2 MapSize => _terrainMaps?.FirstOrDefault()?.Texture.Bounds.SizeVector() ?? Vector2.Zero;

		public void LoadContent()
		{
			LoadTerrainTileSheet();
			LoadCharacterSpriteSheet(_playerRenderData);
			foreach (var data in _renderData.Values)
				LoadCharacterSpriteSheet(data);
		}

		public void UnloadContent()
		{
			_terrainTileSheet?.UnloadContent();
			foreach (var map in _terrainMaps)
				map.UnloadContent();
			_playerRenderData.SpriteSheet?.UnloadContent();
			foreach (var data in _renderData)
				data.Value.SpriteSheet?.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			var playerPosition = _world.Character.Position;

			if (_playerRenderData.PreviousPosition != playerPosition)
			{
				// Terrain...
				var map = _terrainMaps.First();
				var drawAreaPadding = _gameViewArea.SizeVector() / 2;
				int sourceX = 0;
				int sourceY = 0;
			
				if (playerPosition.X > drawAreaPadding.X)
				{
					if (playerPosition.X > map.Texture.Width - (drawAreaPadding.X))
						sourceX = map.Texture.Width - _gameViewArea.Width;
					else
						sourceX = (int)(playerPosition.X - drawAreaPadding.X);
				}

				if (playerPosition.Y > drawAreaPadding.Y)
				{
					if (playerPosition.Y > map.Texture.Height - (drawAreaPadding.Y))
						sourceY = map.Texture.Height - _gameViewArea.Height;
					else
						sourceY = (int)(playerPosition.Y - drawAreaPadding.Y);
				}

				_terrainSourceRect = new Rectangle(sourceX, sourceY, _gameViewArea.Width, _gameViewArea.Height);
				foreach (var terrainMap in _terrainMaps)
					terrainMap.SourceRect = _terrainSourceRect;
			}

			// Player...
			UpdateCharacterRenderData(gameTime, _playerRenderData);
			// Other Characters...
			foreach (var data in _renderData)
				UpdateCharacterRenderData(gameTime, data.Value);
		}

		private void UpdateCharacterRenderData(GameTime gameTime, CharacterRenderData renderData)
		{
			var character = renderData.Character;
			renderData.Animation.IsActive = (renderData.PreviousPosition != character.Position);
			renderData.SpriteSheet.SourceRect = new Rectangle(renderData.SpriteSheet.SourceRect.X, (int)character.Direction * Game1.TileSize, Game1.TileSize, Game1.TileSize);
			renderData.SpriteSheet.Update(gameTime);
			renderData.SpriteSheet.Position = new Vector2(character.Position.X - _terrainSourceRect.X, character.Position.Y - _terrainSourceRect.Y);
			renderData.PreviousPosition = character.Position;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			var gameplayBatch = SpriteBatchManager.Get("gameplay");
			gameplayBatch.GraphicsDevice.ScissorRectangle = _gameViewArea;
			foreach (var map in _terrainMaps.OrderBy(m => m.Index).Where(m => m.Index <= Game1.PlayerDrawIndex))
				map.Draw(gameplayBatch);
			foreach (var data in _renderData.Where(d => d.Value.Character.Position.Y <= _playerRenderData.Character.Position.Y).OrderBy(d => d.Value.Character.Position.Y))
				data.Value.SpriteSheet.Draw(gameplayBatch);
			_playerRenderData.SpriteSheet.Draw(gameplayBatch);
			foreach (var data in _renderData.Where(d => d.Value.Character.Position.Y > _playerRenderData.Character.Position.Y).OrderBy(d => d.Value.Character.Position.Y))
				data.Value.SpriteSheet.Draw(gameplayBatch);
			foreach (var map in _terrainMaps.OrderBy(m => m.Index).Where(m => m.Index > Game1.PlayerDrawIndex))
				map.Draw(gameplayBatch);
		}

		private void LoadCharacterSpriteSheet(CharacterRenderData renderData)
		{
			renderData.SpriteSheet = new ImageTexture($"{Game1.SpriteSheetRoot}\\{renderData.Character.SpriteSheetName}") { 
				IsActive = true,
				Alignment = ImageAlignment.Centered,
				DrawArea = _gameViewArea
			};
			renderData.SpriteSheet.LoadContent();
			renderData.SpriteSheet.AddEffect(renderData.Animation);
		}

		private void LoadTerrainTileSheet()
		{
			if (_terrainTileSheet != null)
				_terrainTileSheet.UnloadContent();

			_terrainTileSheet = new ImageTexture($"{Game1.TilesheetRoot}\\{this.TerrainTileSheetName}");
			_terrainTileSheet.LoadContent();
			LoadTerrainMaps();
		}

		private void LoadTerrainMaps()
		{
			if (_terrainMaps?.Any() ?? false)
				foreach (var terrainMap in _terrainMaps)
					terrainMap.UnloadContent();
					
			_terrainMaps = new List<ImageTexture>();

			// Right now first layer dictates size....but this should proably be explicitly set in the map...
			var firstLayer = this.TerrainLayerData.First(l => l.IsActive);

			foreach (var layer in this.TerrainLayerData.Where(l => l.IsActive))
			{
				var terrainMap = GenerateTerrainMap(layer, Point.Zero, new Point(firstLayer.TileData.GetLength(0) - 1, firstLayer.TileData.GetLength(1) - 1));
				terrainMap.Index = layer.Index;
				_terrainMaps.Add(terrainMap);
			}
		}

		// Eventually we'll load the map in chunks...
		private ImageTexture GenerateTerrainMap(Layer layer, Point start, Point end)
		{
			var renderTarget = new RenderTarget2D(Game1.Graphics, (end.X - start.X + 1) * Game1.TileSize, (end.Y - start.Y + 1) * Game1.TileSize);
			Game1.Graphics.SetRenderTarget(renderTarget);
			Game1.Graphics.Clear(Color.Transparent);
			var spriteBatch = new SpriteBatch(Game1.Graphics);
			spriteBatch.Begin();

			for (int y = 0; y < end.Y - start.Y + 1; y++)
			for (int x = 0; x < end.X - start.X + 1; x++)
			{
				var position = new Vector2((x + start.X) * Game1.TileSize, (y + start.Y) * Game1.TileSize);
				// this.TileData[,] coords swapped here so texture representation matches what the JSON "looks like"...when we serialize to binary this will go away...
				int tileIndex = layer.TileData[y,x].TileIndex;
				if (tileIndex < 0)
					continue;

				int tileIndexX = tileIndex % Game1.TileSheetSize;
				int tileIndexY = tileIndex / Game1.TileSheetSize;
				var sourceRect = new Rectangle(tileIndexX * Game1.TileSize, tileIndexY * Game1.TileSize, Game1.TileSize, Game1.TileSize);
				spriteBatch.Draw(_terrainTileSheet.Texture, position, sourceRect, Color.White);
			}

			spriteBatch.End();
			var texture = (Texture2D)renderTarget;
			Game1.Graphics.SetRenderTarget(null);
			return new ImageTexture(texture) { IsActive = true, DrawArea = _gameViewArea };
		}
	}
}
