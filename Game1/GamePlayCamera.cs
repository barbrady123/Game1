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
using Game1.Items;

namespace Game1
{
	// TODO : Move this when complete...
	public class CharacterRenderData
	{
		public Character Character { get; set; }
		public ImageTexture SpriteSheet { get; set; }	// TODO : Eventually these should  be a seperate collection and we point to an ID here, to prevent loading duplicate sprite sheets...
		public Vector2 PreviousPosition { get; set; }
		//public SpriteSheetEffect Animation { get; set; }
		public bool ShowActiveItem { get; set; }
		public Vector2 ActiveItemPosition { get; set; }
		public bool FlipActiveItem { get; set; }
	}

	// TODO : Move this when complete...
	public class ItemRenderData
	{
		public WorldItem Item { get; set; }
		public Vector2 Position { get; set; }
	}

	public class GamePlayCamera
	{
		public static readonly Vector2 ActiveItemScale = new Vector2(0.7f, 0.7f);
		public static readonly Vector2 MapItemScale = new Vector2(0.7f, 0.7f);
		private const float OverLayerAlpha = 0.7f;


		private readonly World _world;
		private readonly Rectangle _gameViewArea;
		private readonly SpriteBatchData _spriteBatchData;
		private ImageTexture _terrainTileSheet;
		//private ImageTexture _playerSpriteSheet;
		//private Vector2 _previousPlayerPosition;
		private Rectangle _terrainSourceRect;
		//private SpriteSheetEffect _playerAnimation;
		private List<ImageTexture> _terrainMaps;
		private Dictionary<Character, CharacterRenderData> _renderData;
		private List<ItemRenderData> _itemRenderData;
		private CharacterRenderData _playerRenderData;
		
		public string TerrainTileSheetName { get; set; }
		public Layer[] TerrainLayerData{ get; set; }

		public GamePlayCamera(World world, Rectangle gameViewArea, SpriteBatchData spriteBatchData)
		{
			_world = world;
			_world.OnItemsChange += _world_OnItemsChange;
			_gameViewArea = gameViewArea;
			_spriteBatchData = spriteBatchData;
			_terrainSourceRect = Rectangle.Empty;
			_playerRenderData = new CharacterRenderData {
				Character = _world.Character,
				//Animation = new SpriteSheetEffect(false),
				PreviousPosition = -Vector2.One
			};
			_renderData = new Dictionary<Character, CharacterRenderData>();
			// This needs to be realtime pulled every cycle...we can't just run this once in the constructor...
			foreach (var npc in _world.NPCs)
			{
				_renderData[npc] = new CharacterRenderData {
					Character = npc,
					//Animation = new SpriteSheetEffect(false),
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

			// This is temp, and has to be in LoadContent because ItemManager needs to have it's LoadContent called before we can access items...
			_itemRenderData = new List<ItemRenderData>();
			UpdateItemRenderData();
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
			// If the world isn't running, we can skip render calculations for the gameplay view...
			if (!_world.State.HasFlag(ComponentState.Active))
				return;

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

			// Items...
			UpdateItemRenderData(gameTime);
		}

		public void Draw()
		{
			Util.WrappedDraw(DrawInternal, _spriteBatchData, _gameViewArea);
		}

		// Drawing a bit off the view for pixel-perfect clipping is fine...but this will potentially draw WAY off the visible map...need some logic to resolve this
		// so we aren't drawing tons of entities that aren't anywhere near visible within the current camera view...
		// Actually, I'm not sure what the performance hit here is, vs just drawing everything and letting it get clipped....TBD....
		// Also...might need to eventually just sort everything by Y-axis and draw in a single loop...right now things draw based on Y position relative to player,
		// but we also care about other objects on the map relative to each other (mobs, etc)....
		public void DrawInternal(SpriteBatch spriteBatch)
		{
			// Terrain maps that are "below" the characters...
			foreach (var map in _terrainMaps.OrderBy(m => m.Index).Where(m => m.Index <= Game1.PlayerDrawIndex))
				map.Draw(spriteBatch);
			// Draw characters that should be "behind" the player...when their Y coord is <= the player's...
			foreach (var data in _renderData.Where(d => d.Value.Character.Position.Y <= _playerRenderData.Character.Position.Y).OrderBy(d => d.Value.Character.Position.Y))
				data.Value.SpriteSheet.Draw(spriteBatch);
			foreach (var item in _itemRenderData)
				item.Item.Item.Icon.Draw(spriteBatch, position: item.Position, scale: GamePlayCamera.MapItemScale);
			// Draw the player...and the active item (if applicable) either "under" or "over" the player depending on direction
			if (!_playerRenderData.FlipActiveItem)
				DrawActivItem(spriteBatch);
			_playerRenderData.SpriteSheet.Draw(spriteBatch);
			if (_playerRenderData.FlipActiveItem)
				DrawActivItem(spriteBatch);
			// Draw characters that should be "in front" of the player...when their Y coor is > the player's...
			foreach (var data in _renderData.Where(d => d.Value.Character.Position.Y > _playerRenderData.Character.Position.Y).OrderBy(d => d.Value.Character.Position.Y))
				data.Value.SpriteSheet.Draw(spriteBatch);
			// Terrain maps that are "above" the characters...
			foreach (var map in _terrainMaps.OrderBy(m => m.Index).Where(m => m.Index > Game1.PlayerDrawIndex))
			{
				map.Alpha = GamePlayCamera.OverLayerAlpha;
				map.Draw(spriteBatch);
			}
		}

		private void DrawActivItem(SpriteBatch spriteBatch)
		{
			if (_playerRenderData.ShowActiveItem)
				_playerRenderData.Character.ActiveItem.Icon.Draw(
					spriteBatch,
					position: _playerRenderData.ActiveItemPosition,
					scale: GamePlayCamera.ActiveItemScale,
					spriteEffects: _playerRenderData.FlipActiveItem ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
		}

		private void UpdateItemRenderData()
		{
			_itemRenderData.Clear();
			foreach (var item in _world.Items)
				_itemRenderData.Add(new ItemRenderData { Item = item });
		}

		private void UpdateCharacterRenderData(GameTime gameTime, CharacterRenderData renderData)
		{
			var character = renderData.Character;
			if (renderData.PreviousPosition != character.Position)
				renderData.SpriteSheet.AddEffect<SpriteSheetEffect>(true);
			else
				renderData.SpriteSheet.StopEffect(typeof(SpriteSheetEffect));
			
			renderData.SpriteSheet.SourceRect = new Rectangle(renderData.SpriteSheet.SourceRect.X, (int)character.Direction * Game1.TileSize, Game1.TileSize, Game1.TileSize);
			renderData.SpriteSheet.Update(gameTime);
			renderData.SpriteSheet.Position = new Vector2(character.Position.X - _terrainSourceRect.X + _gameViewArea.X, character.Position.Y - _terrainSourceRect.Y + _gameViewArea.Y);
			renderData.PreviousPosition = character.Position;
			// This should also show tools when we have them available (Pickaxe, etc)...
			if (renderData.ShowActiveItem = (character.ActiveItem?.Item is ItemWeapon weapon))
			{
				if ((character.Direction == Cardinal.North) || (character.Direction == Cardinal.West))
				{
					renderData.ActiveItemPosition = renderData.SpriteSheet.Position.Offset(character.Direction == Cardinal.North ? 10 : 2, 10);
					character.ActiveItem.Icon.OriginOffset = new Vector2(20, 22);
					renderData.FlipActiveItem = false;

				}
				else
				{
					renderData.ActiveItemPosition = renderData.SpriteSheet.Position.Offset(character.Direction == Cardinal.South ? -13 : -9, 12);
					character.ActiveItem.Icon.OriginOffset = new Vector2(-20, 22);
					renderData.FlipActiveItem = true;
				}
			}
		}

		private void UpdateItemRenderData(GameTime gameTime)
		{	
			foreach (var data in _itemRenderData)
			{
				data.Position = new Vector2(data.Item.Position.X - _terrainSourceRect.X + _gameViewArea.X, data.Item.Position.Y - _terrainSourceRect.Y + _gameViewArea.Y);
			}
		}

		private void LoadCharacterSpriteSheet(CharacterRenderData renderData)
		{
			// TODO: Fix this so it loads unique sprite sheets into collection ONLY and they are referenced by id here...
			renderData.SpriteSheet = new ImageTexture($"{Game1.SpriteSheetRoot}\\{renderData.Character.SpriteSheetName}") { 
				IsActive = true,
				Alignment = ImageAlignment.Centered,
			};
			renderData.SpriteSheet.LoadContent();
			renderData.SpriteSheet.AddEffect<SpriteSheetEffect>(false);
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
				// TODO: Why are we not calling _terrainTileSheet.Draw() here, why bypassing the class draw?? (this is the only place that does this)
				spriteBatch.Draw(_terrainTileSheet.Texture, position, sourceRect, Color.White);
			}

			spriteBatch.End();
			var texture = (Texture2D)renderTarget;
			Game1.Graphics.SetRenderTarget(null);
			return new ImageTexture(texture) { IsActive = true, Position = _gameViewArea.TopLeftVector() };
		}

		private void _world_OnItemsChange(object sender, ComponentEventArgs e)
		{
			UpdateItemRenderData();
		}
	}
}
