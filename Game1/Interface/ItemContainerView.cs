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
using Game1.Screens;
using Game1.Screens.Menu;

namespace Game1.Interface
{
	public class ItemContainerView
	{
		public const int ItemViewSize = Game1.IconSize + ((InventoryItemView.BorderWidth + InventoryItemView.ImagePadding) * 2);
		private const int ItemsPerRow = 10;
		private const int ItemViewPadding = 10;

		private InventoryItemView[] _itemViews;
		private readonly ItemContainer _container;

		private static readonly Size ContentMargin = new Size(10, 10);

		public Rectangle Bounds { get; set; }		

		public bool HightlightActiveItem { get; set; }

		public ItemContainerView(ItemContainer container, Rectangle bounds, bool highlightActiveItem)
		{
			this.Bounds = bounds;
			_container = container;
			_itemViews = new InventoryItemView[_container.Size];
			for (int i = 0; i < _itemViews.Length; i++)
			{
				var position = CalculateItemViewPosition(i);
				_itemViews[i] = new InventoryItemView(position.ExpandToRectangeTopLeft(ItemContainerView.ItemViewSize, ItemContainerView.ItemViewSize)) { Position = position };
			}
			this.HightlightActiveItem = highlightActiveItem;
		}

		public void LoadContent()
		{
			foreach (var item in _itemViews)
				item.LoadContent();
		}

		public void UnloadContent()
		{
			foreach (var item in _itemViews)
				item.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			UpdateItems(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var item in _itemViews)
				item.Draw(spriteBatch);
		}

		private Vector2 CalculateItemViewPosition(int index)
		{
			int row = index / ItemContainerView.ItemsPerRow;
			int col = index % ItemContainerView.ItemsPerRow;

			int xPos = this.Bounds.X
						+ ItemContainerView.ContentMargin.Width
						+ (ItemContainerView.ItemViewSize * col)
						+ (ItemContainerView.ItemViewPadding * col);
			int yPos = this.Bounds.Y
						+ ItemContainerView.ContentMargin.Height
						+ (ItemContainerView.ItemViewSize * row)
						+ (ItemContainerView.ItemViewPadding * row);

			return new Vector2(xPos, yPos);
		}

		private void UpdateItems(GameTime gameTime)
		{
			for (int i = 0; i < _itemViews.Length; i++)
			{
				_itemViews[i].Item = _container[i];
				_itemViews[i].Highlight = this.HightlightActiveItem && (_container.ActiveItemIndex == i);
				_itemViews[i].Update(gameTime);
			}
		}

		public static Size RequiredViewSize(int numItems)
		{
			int width = (ItemViewSize * ItemsPerRow) + (ItemViewPadding * (ItemsPerRow - 1)) + (ContentMargin.Width * 2);
			int numRows = (numItems / ItemsPerRow) + ((numItems % ItemsPerRow) > 0 ? 1 : 0);
			int height = (ItemViewSize * numRows) + (ItemViewPadding * (numRows - 1)) + (ContentMargin.Height * 2);
			return new Size(width, height);
		}

		public static T New<T>(ItemContainer container, Point position, bool hightlightActiveItem) where T: ItemContainerView
		{
			var requiredSize = ItemContainerView.RequiredViewSize(container.Items.Length);
			return (T)Activator.CreateInstance(typeof(T), container, new Rectangle(position.X, position.Y, requiredSize.Width, requiredSize.Height), hightlightActiveItem);
		}
	}
}
