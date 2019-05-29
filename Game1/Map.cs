using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Game1
{
	public class Map
	{
		private ContentManager _content;
		private ImageTexture _tileSheet;
		private ImageTexture _ground;

		[JsonProperty("mapsizex")]
		public int Width { get; set; }
		[JsonProperty("mapsizey")]
		public int Height { get; set; }
		[JsonProperty("tiledata")]
		public Tile[,] TileData { get; set; }
		[JsonProperty("tilesheet")]
		public string TileSheet { get; set; }

		public Rectangle DrawArea { get; set; }
		public Rectangle SourceRect { get; set; }

		public Vector2 GroundSize => _ground.Texture.Bounds.SizeVector();

		public void LoadContent()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			_tileSheet = new ImageTexture(_content.Load<Texture2D>($"{Game1.TilesheetRoot}\\{this.TileSheet}"));
			_tileSheet.LoadContent();
			_ground = GenerateGroundTexture(Point.Zero, new Point(this.TileData.GetLength(0) - 1, this.TileData.GetLength(1) - 1));
			_ground.IsActive = true;
			_ground.LoadContent();
		}

		public void UnloadContent()
		{
			_ground.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			_ground.SourceRect = this.SourceRect;
			_ground.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_ground.DrawArea = this.DrawArea;
			_ground.Draw(spriteBatch);
		}

		// Should be this a TileSheet function (generate map)...
		private ImageTexture GenerateGroundTexture(Point start, Point end)
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
				// this.TileData[,] coords swapped here to texture representation matches what the JSON "looks like"...when we serialize to binary this will go away...
				int tileIndexX = this.TileData[y,x].TileIndex % Game1.TileSheetSize;
				int tileIndexY = this.TileData[y,x].TileIndex / Game1.TileSheetSize;
				var sourceRect = new Rectangle(tileIndexX * Game1.TileSize, tileIndexY * Game1.TileSize, Game1.TileSize, Game1.TileSize);
				spriteBatch.Draw(_tileSheet.Texture, position, sourceRect, Color.White);
			}

			spriteBatch.End();
			var texture = (Texture2D)renderTarget;
			Game1.Graphics.SetRenderTarget(null);
			return new ImageTexture(texture);
		}
	}
}
