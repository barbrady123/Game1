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
		private int _visibleCellStartX;
		private int _visibleCellStartY;
		private int _visibleCellEndX;
		private int _visibleCellEndY;

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
		private Dictionary<Layer, ImageTexture> _tileSheets;
		private Dictionary<Layer, ImageTexture> _staticMaps;
		private Vector2 _renderOffset;

		public Vector2 RenderOffset => _renderOffset;
		
		public GamePlayCamera(World world, Rectangle gameViewArea, SpriteBatchData spriteBatchData)
		{
			_world = world;
			_gameViewArea = gameViewArea;
			_spriteBatchData = spriteBatchData;
			_renderOffset = Vector2.Zero;
			_tileSheets = new Dictionary<Layer, ImageTexture>();
			_staticMaps = new Dictionary<Layer, ImageTexture>();
		}

		public void LoadContent()
		{
			LoadTileSheets();
		}

		public void UnloadContent()
		{
			foreach (var map in _staticMaps.Values)
				map.UnloadContent();

			foreach (var tileSheet in _tileSheets.Values)
				tileSheet.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			if (_world.Character.Moved)
				SetStaticMapsSourceRect();
		}

		public void Draw()
		{
			Util.WrappedDraw(DrawInternal, _spriteBatchData, _gameViewArea);
		}

		public void DrawInternal(SpriteBatch spriteBatch)
		{
			// Need a seperate param for "cameraOffset"...or just convert to matrix transformation....

			// Terrain maps that are "below" the characters...
			foreach (var layer in _world.CurrentMap.Layers.Where(l => l.Type == LayerType.Terrain))
				_staticMaps[layer].Draw(spriteBatch);

			// Draw characters that should be "behind" the player...when their Y coord is <= the player's...
			foreach (var npc in _world.NPCs.Where(n => n.Position.Y <= _world.Character.Position.Y).OrderBy(n => n.Position.Y))
				npc.Draw(spriteBatch, _renderOffset);

			foreach (var item in _world.MapObjects.GetEntities(_visibleCellStartX, _visibleCellStartY, _visibleCellEndX, _visibleCellEndY))
				item.Draw(spriteBatch, _renderOffset);

			_world.Character.Draw(spriteBatch, _renderOffset);

			// Draw characters that should be "in front" of the player...when their Y coor is > the player's...
			foreach (var npc in _world.NPCs.Where(n => n.Position.Y > _world.Character.Position.Y).OrderBy(n => n.Position.Y))
				npc.Draw(spriteBatch, _renderOffset);

			// We're drawing these as "above" the player, but probably doesn't matter...
			foreach (var layer in _world.CurrentMap.Layers.Where(l => l.Type == LayerType.Solid))
				_staticMaps[layer].Draw(spriteBatch);

			foreach (var layer in _world.CurrentMap.Layers.Where(l => l.Type == LayerType.Top))
				_staticMaps[layer].Draw(spriteBatch);
		}

		private void LoadTileSheets()
		{
			foreach (var sheet in _tileSheets.Values)
				sheet.UnloadContent();
			_tileSheets.Clear();

			foreach (var layer in _world.CurrentMap.StaticLayers)
			{
				// TODO: Should load up the tile sheets elsewhere, so they aren't duplicated (though typically we wouldn't use the same tilesheet for multiple layers anyway)
				var tileSheet = new ImageTexture($"{Game1.TilesheetRoot}\\{layer.TileSheet}");
				tileSheet.LoadContent();
				_tileSheets.Add(layer, tileSheet);
			}

			GenerateStaticMaps();
			SetStaticMapsSourceRect();
		}

		private void GenerateStaticMaps()
		{
			foreach (var staticMap in _staticMaps.Values)
				staticMap.UnloadContent();
			_staticMaps.Clear();

			foreach (var layer in _world.CurrentMap.StaticLayers)
				_staticMaps.Add(layer, GenerateStaticMap(layer, Point.Zero, new Point(_world.CurrentMap.Width - 1, _world.CurrentMap.Height - 1)));
		}

		private ImageTexture GenerateStaticMap(Layer layer, Point start, Point end)
		{
			var renderTarget = new RenderTarget2D(Game1.Graphics, (end.X - start.X + 1) * Game1.TileSize, (end.Y - start.Y + 1) * Game1.TileSize);
			Game1.Graphics.SetRenderTarget(renderTarget);
			Game1.Graphics.Clear(Color.Transparent);
			var spriteBatch = new SpriteBatch(Game1.Graphics);
			var tileSheetTexture = _tileSheets[layer].Texture;
			spriteBatch.Begin();

			for (int y = 0; y < end.Y - start.Y + 1; y++)
			for (int x = 0; x < end.X - start.X + 1; x++)
			{
				var position = new Vector2((x + start.X) * Game1.TileSize, (y + start.Y) * Game1.TileSize);
				// this.TileData[,] coords swapped here so texture representation matches what the JSON "looks like"...when we serialize to binary this will go away...
				int tileIndex = layer.TileData[y,x];
				if (tileIndex < 0)
					continue;

				int tileIndexX = tileIndex % Game1.TileSheetSize;
				int tileIndexY = tileIndex / Game1.TileSheetSize;
				var sourceRect = new Rectangle(tileIndexX * Game1.TileSize, tileIndexY * Game1.TileSize, Game1.TileSize, Game1.TileSize);
				spriteBatch.Draw(tileSheetTexture, position, sourceRect, Color.White);
			}

			spriteBatch.End();
			var texture = (Texture2D)renderTarget;
			Game1.Graphics.SetRenderTarget(null);
			return new ImageTexture(texture) {
				IsActive = true,
				Position = _gameViewArea.TopLeftVector(),
				Alpha = (layer.Type == LayerType.Top) ? GamePlayCamera.OverLayerAlpha : 1.0f
			};
		}

		private void SetStaticMapsSourceRect()
		{
			var mapSize = _world.CurrentMap.Size;

			var drawAreaPadding = _gameViewArea.SizeVector() / 2;
			int sourceX = Util.Clamp((int)( _world.Character.Position.X - drawAreaPadding.X), 0, (int)(mapSize.Width - _gameViewArea.Width));
			int sourceY = Util.Clamp((int)( _world.Character.Position.Y - drawAreaPadding.Y), 0, (int)(mapSize.Height - _gameViewArea.Height));
			var visibleSourceRect = new Rectangle(sourceX, sourceY, _gameViewArea.Width, _gameViewArea.Height);

			foreach (var staticMap in _staticMaps.Values)
				staticMap.SourceRect = visibleSourceRect;

			_renderOffset = new Vector2(-sourceX + _gameViewArea.X, -sourceY + _gameViewArea.Y);

			_visibleCellStartX = sourceX / Game1.TileSize;
			_visibleCellStartY = sourceY / Game1.TileSize;
			_visibleCellEndX = (sourceX + _gameViewArea.Width - 1) / Game1.TileSize;
			_visibleCellEndY = (sourceY + _gameViewArea.Height - 1) / Game1.TileSize;
		}
	}
}
