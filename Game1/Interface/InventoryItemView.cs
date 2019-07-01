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
	// TODO: We may want to change this so that it is aware of it's container (already aware of index), and require InventoryItemViews to always have a underlying container, so that
	// the references to the actual item slot is easy.  For now that means the equipped armor slots need to change, but that kinda makes sense to have them in a "armor container" type
	// of encapsulation anyway...this way we always have a non-null direct reference to every item in the character's inventory....also we shouldn't need awareness of the ContainingView
	// here, because we really only use that in a few places to get to the underlying container anyway...
	public class InventoryItemView : Component, ISupportsContextMenu
	{
		public const int Size = Game1.IconSize + ((InventoryItemView.BorderWidth + InventoryItemView.ImagePadding) * 2);
		public const int BorderWidth = 2;
		public const int ImagePadding = 2;
		public static readonly Color HighlightColor = Color.Red;
		public static readonly Vector2 MouseOverScale = new Vector2(1.1f, 1.1f);

		protected override int BorderThickness => InventoryItemView.BorderWidth;
		public override string TooltipText => this.Item?.Item.DisplayName;

		private ImageTexture _highlightBorder;
		private ImageTexture _emptyIcon;
		private ImageText _quantity;
		private string _emptyImageName;

		public ItemContainerView ContainingView { get; set; }
		public InventoryItem Item { get; set; }
		public int Index { get; set; }

		public bool Highlight { get; set; }
		public bool IsEquippedSlot { get; set; }

		public InventoryItemView(Rectangle bounds, int index, string emptyImage, bool isEquippedSlot, ItemContainerView containingView = null) : base(bounds, background: "black", hasBorder: true)
		{
			this.Index = index;
			this.ContainingView = containingView;
			_emptyImageName = emptyImage;
			if (!String.IsNullOrWhiteSpace(_emptyImageName))
				_emptyIcon = new ImageTexture($"{Game1.IconRoot}\\Empty\\{_emptyImageName}") { Position = this.Bounds.CenterVector(), Alignment = ImageAlignment.Centered };
			this.Highlight = false;
			this.IsEquippedSlot = isEquippedSlot;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_highlightBorder = Util.GenerateBorderTexture(this.Bounds.Width + 8, this.Bounds.Height + 8, this.BorderThickness + 2, InventoryItemView.HighlightColor, false);
			_highlightBorder.Alignment = ImageAlignment.Centered;
			_highlightBorder.Position = this.Bounds.CenterVector();
			_highlightBorder.LoadContent();
			_quantity = new ImageText("", true) { Alignment = ImageAlignment.RightBottom };
			_quantity.Position = this.Bounds.BottomRightVector(-InventoryItemView.ImagePadding -2, -InventoryItemView.ImagePadding);
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

			this.Item?.Update(gameTime);
			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{			
			base.UpdateActive(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);
			_background.Scale = (_mouseover ? InventoryItemView.MouseOverScale : Vector2.One);
			_border.Scale = (_mouseover ? InventoryItemView.MouseOverScale : Vector2.One);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_highlightBorder.Draw(spriteBatch);
			_emptyIcon?.Draw(spriteBatch);
			if (this.Item != null)
			{
				this.Item.Icon.Position = this.Bounds.CenterVector();
				this.Item.Icon.Scale = (_mouseover ? InventoryItemView.MouseOverScale : Vector2.One);
				this.Item.Icon.Draw(spriteBatch);
				_quantity.Draw(spriteBatch);
			}
		}

		public override List<string> GetContextMenuOptions()
		{
			var items = new List<string>();
			if (this.Item == null)
				return items;

			switch (this.Item.Item)
			{
				case ItemArmor armor:
					items.Add(this.IsEquippedSlot ? "Unequip" : "Equip");			
					break;
				case ItemConsumable consumable:
					switch (consumable.Type)
					{
						case ConsumableType.Eat : items.Add("Eat");		break;
						case ConsumableType.Drink : items.Add("Drink");	break;
						case ConsumableType.Read : items.Add("Read");	break;
					}
					break;
			}

			if (this.Item.Quantity > 1)
				items.Add("Split");

			items.Add("Drop");
			items.Add("Cancel");

			return items;
		}
	}
}
