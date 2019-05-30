using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Items;

namespace Game1.Interface
{
	public class InventoryItemView
	{
		public const int BorderWidth = 2;
		public const int ImagePadding = 2;

		private int _size;
		private Rectangle _bounds;
		private ImageTexture _background;
		private ImageTexture _border;
		private ImageText _quantity;

		public InventoryItem Item { get; set; }

		public Vector2 Position { get; set; }

		public InventoryItemView(int size)
		{
			_size = size;
		}

		public void LoadContent()
		{
			_background = Util.GenerateSolidBackground(_size, _size, Color.Black);
			_background.DrawArea = new Rectangle((int)this.Position.X, (int)this.Position.Y, _size, _size);
			_background.LoadContent();
			_border = Util.GenerateBorderTexture(_size, _size, InventoryItemView.BorderWidth, Color.Gray);
			_border.DrawArea = new Rectangle((int)this.Position.X, (int)this.Position.Y, _size, _size);
			_border.LoadContent();
		}

		public void UnloadContent()
		{
			_background.LoadContent();
			_border.LoadContent();
		}

		public void Update(GameTime gameTime)
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_background.Draw(spriteBatch);
			_border.Draw(spriteBatch);
		}
	}
}
