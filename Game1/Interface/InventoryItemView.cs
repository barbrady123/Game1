using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Items;

namespace Game1.Interface
{
	public class InventoryItemView
	{
		public const int Size = Game1.IconSize + ((InventoryItemView.BorderWidth + InventoryItemView.ImagePadding) * 2);
		public const int BorderWidth = 2;
		public const int ImagePadding = 2;

		private Rectangle _bounds;
		private ImageTexture _background;
		private ImageTexture _border;
		private ImageTexture _highlight;
		private ImageTexture _empty;
		private ImageText _quantity;
		private Vector2 _position;
		private bool _mouseover;
		private int _containerIndex;
		private string _emptyImageName;

		public InventoryItem Item { get; set; }

		public Vector2 CenterPosition { get; set; }

		public bool Highlight { get; set; }

		public event EventHandler OnMouseClick;
		public event EventHandler OnMouseOver;
		public event EventHandler OnMouseOut;
		
		public Vector2 Position
		{ 
			get { return _position; }
			set {
				_position = value;
				// Precalculate this so I we don't have to run this on every update for every item...
				this.CenterPosition = _bounds.CenterVector();
			}
		}

		public InventoryItemView(Rectangle bounds, int containerIndex, string emptyImage)
		{
			_bounds = bounds;
			_containerIndex = containerIndex;
			this.Position = bounds.TopLeftVector();
			_emptyImageName = emptyImage;
			if (!String.IsNullOrWhiteSpace(_emptyImageName))
				_empty = new ImageTexture($"{Game1.IconRoot}\\Empty\\{_emptyImageName}") { Position = this.CenterPosition, Alignment = ImageAlignment.Centered };
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
			_highlight = Util.GenerateBorderTexture(_bounds.Width + 8, _bounds.Height + 8, InventoryItemView.BorderWidth + 2, Color.Red, false);
			_highlight.Alignment = ImageAlignment.Centered;
			_highlight.Position = this.CenterPosition;
			_highlight.LoadContent();
			_border.LoadContent();
			_quantity = new ImageText("", true) { Alignment = ImageAlignment.RightBottom };
			_quantity.Position = _bounds.BottomRightVector(-InventoryItemView.ImagePadding, -InventoryItemView.ImagePadding);
			_quantity.LoadContent();
			_empty?.LoadContent();
		}

		public void UnloadContent()
		{
			_background.UnloadContent();
			_border.UnloadContent();
			_highlight.UnloadContent();
			_quantity.UnloadContent();
			_empty?.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			bool previousMouseOver = _mouseover;
			_highlight.IsActive = this.Highlight;
			_mouseover = InputManager.MouseOver(_bounds);
			if (_empty != null)
				_empty.IsActive = (this.Item?.Item == null);
			if (this.Item?.Item != null)
				_quantity.UpdateText((this.Item.Item.MaxStackSize > 1) ? this.Item.Quantity.ToString() : "");

			if (InputManager.LeftMouseClick(_bounds))
				OnMouseClick?.Invoke(this, new MouseEventArgs(MouseButton.Left, _containerIndex));

			if (_mouseover)
				OnMouseOver?.Invoke(this, new MouseEventArgs(MouseButton.None, _containerIndex));
			else if (previousMouseOver)
				OnMouseOut?.Invoke(this, new MouseEventArgs(MouseButton.None, _containerIndex));
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			_background.Scale = (_mouseover ? new Vector2(1.1f, 1.1f) : Vector2.One);
			_background.Draw(spriteBatch);
			_border.Scale = (_mouseover ? new Vector2(1.1f, 1.1f) : Vector2.One);
			_border.Draw(spriteBatch);
			_highlight.Draw(spriteBatch);
			_empty?.Draw(spriteBatch);
			if (this.Item?.Item?.Icon != null)
			{
				this.Item.Item.Icon.Scale = (_mouseover ? new Vector2(1.1f, 1.1f) : Vector2.One);
				this.Item.Item.Icon.Alpha = (this.Item.InTransition ? 0.3f : 1.0f);
				this.Item.Item.Icon.Draw(spriteBatch, null, this.CenterPosition);
				_quantity.Draw(spriteBatch);
			}
		}
	}
}
