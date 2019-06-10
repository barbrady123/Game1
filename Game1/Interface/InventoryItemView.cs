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
	public class InventoryItemView : Component
	{
		public const int Size = Game1.IconSize + ((InventoryItemView.BorderWidth + InventoryItemView.ImagePadding) * 2);
		public const int BorderWidth = 2;
		public const int ImagePadding = 2;
		public static readonly Color HighlightColor = Color.Red;
		public static readonly Vector2 MouseOverScale = new Vector2(1.1f, 1.1f);

		protected override int BorderThickness => InventoryItemView.BorderWidth;

		private ImageTexture _highlightBorder;
		private ImageTexture _emptyIcon;
		private ImageText _quantity;
		private string _emptyImageName;

		public ItemContainerView ContainingView { get; set; }
		public InventoryItem Item { get; set; }
		public int Index { get; set; }

		public bool Highlight { get; set; }

		public event EventHandler<ComponentEventArgs> OnMouseClick;
		
		public InventoryItemView(Rectangle bounds, int index, string emptyImage, ItemContainerView containingView = null) : base(bounds, background: "black", hasBorder: true)
		{
			this.Index = index;
			this.ContainingView = containingView;
			_emptyImageName = emptyImage;
			if (!String.IsNullOrWhiteSpace(_emptyImageName))
				_emptyIcon = new ImageTexture($"{Game1.IconRoot}\\Empty\\{_emptyImageName}") { Position = this.Bounds.CenterVector(), Alignment = ImageAlignment.Centered };
			this.Highlight = false;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_highlightBorder = Util.GenerateBorderTexture(this.Bounds.Width + 8, this.Bounds.Height + 8, this.BorderThickness + 2, InventoryItemView.HighlightColor, false);
			_highlightBorder.Alignment = ImageAlignment.Centered;
			_highlightBorder.Position = this.Bounds.CenterVector();
			_highlightBorder.LoadContent();
			_quantity = new ImageText("", true) { Alignment = ImageAlignment.RightBottom };
			_quantity.Position = this.Bounds.BottomRightVector(-InventoryItemView.ImagePadding, -InventoryItemView.ImagePadding);
			_quantity.LoadContent();
			_emptyIcon?.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_highlightBorder.UnloadContent();
			_quantity.UnloadContent();
			_emptyIcon?.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			if (_emptyIcon != null)
				_emptyIcon.IsActive = (this.Item == null);

			if (this.Item != null)
				_quantity.UpdateText((this.Item.Item.MaxStackSize > 1) ? this.Item.Quantity.ToString() : "");

			_highlightBorder.IsActive = this.Highlight;

			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (InputManager.LeftMouseClick(this.Bounds))
				OnMouseClick?.Invoke(this, new MouseEventArgs(MouseButton.Left));
			if (InputManager.RightMouseClick(this.Bounds))
				OnMouseClick?.Invoke(this, new MouseEventArgs(MouseButton.Right));

			base.UpdateInput(gameTime);
		}

		public override void UpdateMousePosition(GameTime gameTime)
		{
			base.UpdateMousePosition(gameTime);
			_background.Scale = (_mouseover ? InventoryItemView.MouseOverScale : Vector2.One);
			_border.Scale = (_mouseover ? InventoryItemView.MouseOverScale : Vector2.One);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_highlightBorder.Draw(spriteBatch);
			_emptyIcon?.Draw(spriteBatch);
			if (this.Item != null)
			{
				this.Item.Item.Icon.Scale = (_mouseover ? InventoryItemView.MouseOverScale : Vector2.One);
				this.Item.Item.Icon.Draw(spriteBatch, null, this.Bounds.CenterVector());
				_quantity.Draw(spriteBatch);
			}
		}
	}
}
