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
	// NOTE: This is not a "true" camera (no matrix transformation)...I should probably figure that shit out and port it to that at some point...:)
	public class GamePlayCamera
	{
		public static readonly Vector2 ActiveItemScale = new Vector2(0.7f, 0.7f);
		public static readonly Vector2 MapItemScale = new Vector2(0.7f, 0.7f);
		private const float OverLayerAlpha = 0.7f;
		public static readonly Dictionary<Cardinal, Vector2> ActiveItemOffsets = new Dictionary<Cardinal, Vector2>
		{
			{ Cardinal.North, new Vector2(10, 10) },
			{ Cardinal.South, new Vector2(-13, 12) },
			{ Cardinal.East, new Vector2(-9, 12) },
			{ Cardinal.West, new Vector2(2, 10) },
		};
		public static readonly Dictionary<Cardinal, Vector2> ActiveItemOriginOffsets = new Dictionary<Cardinal, Vector2>
		{
			{ Cardinal.North, new Vector2(20, 22) },
			{ Cardinal.South, new Vector2(-20, 22) },
			{ Cardinal.East, new Vector2(-20, 22) },
			{ Cardinal.West, new Vector2(20, 22) },
		};

		private readonly World _world;
		private readonly Rectangle _gameViewArea;
		private readonly SpriteBatchData _spriteBatchData;
		private ImageTexture _terrainTileSheet;
		private Rectangle _terrainSourceRect;
		private Vector2 _renderOffset;
		private List<ImageTexture> _terrainMaps;
		
		public string TerrainTileSheetName { get; set; }
		public Layer[] TerrainLayerData{ get; set; }

		public GamePlayCamera(World world, Rectangle gameViewArea, SpriteBatchData spriteBatchData)
		{
			_world = world;
			_gameViewArea = gameViewArea;
			_spriteBatchData = spriteBatchData;
			_terrainSourceRect = Rectangle.Empty;
			_renderOffset = Vector2.Zero;
		}

		public Vector2 MapSize => _terrainMaps?.FirstOrDefault()?.Texture.Bounds.SizeVector() ?? Vector2.Zero;

		public void LoadContent()
		{
			LoadTerrainTileSheet();
		}

		public void UnloadContent()
		{
			_terrainTileSheet?.UnloadContent();
			foreach (var map in _terrainMaps)
				map.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			if (_world.Character.Moved)
				SetTerrainSourceRect();
		}

		private void SetTerrainSourceRect()
		{
			// Terrain...
			var map = _terrainMaps.First();	// there should just be a map size, not this silly thing...
			var drawAreaPadding = _gameViewArea.SizeVector() / 2;

			int sourceX = Util.Clamp((int)( _world.Character.Position.X - drawAreaPadding.X), 0, (int)(map.Texture.Width - _gameViewArea.Width));
			int sourceY = Util.Clamp((int)( _world.Character.Position.Y - drawAreaPadding.Y), 0, (int)(map.Texture.Height - _gameViewArea.Height));

			_terrainSourceRect = new Rectangle(sourceX, sourceY, _gameViewArea.Width, _gameViewArea.Height);
			foreach (var terrainMap in _terrainMaps)
				terrainMap.SourceRect = _terrainSourceRect;

			// TEST: going to just store this offset and apply it realtime in Draw instead of calculating and caching this everywhere in Update...
			_renderOffset = new Vector2(-sourceX + _gameViewArea.X, -sourceY + _gameViewArea.Y);
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
			foreach (var npc in _world.NPCs.Where(n => n.Position.Y <= _world.Character.Position.Y).OrderBy(n => n.Position.Y))
				npc.Draw(spriteBatch, _renderOffset);

			// Items on ground...
			foreach (var item in _world.Items)
				item.Item.Icon.Draw(spriteBatch, position: item.Position + _renderOffset, scale: GamePlayCamera.MapItemScale);

			// Interactive objects...
			foreach (var interactive in _world.Interactives)
				interactive.Icon.Draw(spriteBatch, position: interactive.Position + _renderOffset);

			_world.Character.Draw(spriteBatch, _renderOffset);

			// Draw characters that should be "in front" of the player...when their Y coor is > the player's...
			foreach (var npc in _world.NPCs.Where(n => n.Position.Y > _world.Character.Position.Y).OrderBy(n => n.Position.Y))
				npc.Draw(spriteBatch, _renderOffset);

			// Terrain maps that are "above" the characters...
			foreach (var map in _terrainMaps.OrderBy(m => m.Index).Where(m => m.Index > Game1.PlayerDrawIndex))
			{
				map.Alpha = GamePlayCamera.OverLayerAlpha;
				map.Draw(spriteBatch);
			}
		}

		private void LoadTerrainTileSheet()
		{
			if (_terrainTileSheet != null)
				_terrainTileSheet.UnloadContent();

			_terrainTileSheet = new ImageTexture($"{Game1.TilesheetRoot}\\{this.TerrainTileSheetName}");
			_terrainTileSheet.LoadContent();
			LoadTerrainMaps();
			SetTerrainSourceRect();
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
		// NOTE: This type of technique is only valid for completely STATIC map data...everything interactive will have to be rendered on top of this...
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
	}
}
