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

		private Rectangle _bounds;
		private ImageTexture _background;
		private ImageTexture _border;
		private ImageTexture _highlight;
		private ImageText _quantity;
		private Vector2 _position;

		public InventoryItem Item { get; set; }

		public Vector2 CenterPosition { get; set; }

		public bool Highlight { get; set; }
		
		public Vector2 Position
		{ 
			get { return _position; }
			set {
				_position = value;
				// Precalculate this so I we don't have to run this on every update for every item...
				this.CenterPosition = _bounds.CenterVector();
			}
		}

		public InventoryItemView(Rectangle bounds)
		{
			_bounds = bounds;
		}

		public void LoadContent()
		{
			this.Highlight = false;
			_background = Util.GenerateSolidBackground(_bounds.Width, _bounds.Height, Color.Black);
			_background.Alignment = ImageAlignment.Centered;
			_background.Position = this.CenterPosition;
			_background.LoadContent();
			_border = Util.GenerateBorderTexture(_bounds.Width, _bounds.Height, InventoryItemView.BorderWidth, Color.Gray);
			_border.Alignment = ImageAlignment.Centered;
			_border.Position = this.CenterPosition;
			_border.LoadContent();
			_highlight = Util.GenerateBorderTexture(_bounds.Width + 8, _bounds.Height + 8, InventoryItemView.BorderWidth + 2, Color.Red);
			_highlight.Alignment = ImageAlignment.Centered;
			_highlight.Position = this.CenterPosition;
			_highlight.LoadContent();
			_border.LoadContent();
			_quantity = new ImageText("", true) { Alignment = ImageAlignment.RightBottom };
			_quantity.Position = _bounds.BottomRightVector(-InventoryItemView.ImagePadding, -InventoryItemView.ImagePadding);
			_quantity.LoadContent();
		}

		public void UnloadContent()
		{
			_background.UnloadContent();
			_border.UnloadContent();
			_highlight.UnloadContent();
			_quantity.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			if (this.Item?.Item != null)
				_quantity.UpdateText((this.Item.Item.MaxStackSize > 1) ? this.Item.Quantity.ToString() : "");
			_highlight.IsActive = this.Highlight;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_background.Draw(spriteBatch);
			_border.Draw(spriteBatch);
			_highlight.Draw(spriteBatch);
			this.Item?.Item?.Icon?.Draw(spriteBatch, null, this.CenterPosition);
			_quantity.Draw(spriteBatch);
		}
	}
}
