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
	public class ItemContainerViewNew
	{
		public const int ItemViewSize = Game1.IconSize + ((InventoryItemView.BorderWidth + InventoryItemView.ImagePadding) * 2);
		private const int ItemsPerRow = 10;
		private const int ItemViewPadding = 10;

		private InventoryItemView[] _itemViews;
		private readonly ItemContainer _container;

		private static readonly Size ContentMargin = new Size(10, 10);

		public Rectangle Bounds { get; set; }

		public ItemContainerViewNew(ItemContainer container, Rectangle bounds)
		{
			this.Bounds = bounds;
			_container = container;
			_itemViews = new InventoryItemView[_container.Size];
			for (int i = 0; i < _itemViews.Length; i++)
			{
				var position = CalculateItemViewPosition(i);
				_itemViews[i] = new InventoryItemView(position.ExpandToRectangeTopLeft(ItemContainerViewNew.ItemViewSize, ItemContainerViewNew.ItemViewSize)) { Position = position };
			}
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
			int row = index / ItemContainerViewNew.ItemsPerRow;
			int col = index % ItemContainerViewNew.ItemsPerRow;

			int xPos = this.Bounds.X
						+ ItemContainerViewNew.ContentMargin.Width
						+ (ItemContainerViewNew.ItemViewSize * col)
						+ (ItemContainerViewNew.ItemViewPadding * col);
			int yPos = this.Bounds.Y
						+ ItemContainerViewNew.ContentMargin.Height
						+ (ItemContainerViewNew.ItemViewSize * row)
						+ (ItemContainerViewNew.ItemViewPadding * row);

			return new Vector2(xPos, yPos);
		}

		private void UpdateItems(GameTime gameTime)
		{
			for (int i = 0; i < _itemViews.Length; i++)
			{
				_itemViews[i].Item = _container[i];
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

		public static ItemContainerViewNew New(ItemContainer container, Point position)
		{
			var requiredSize = ItemContainerViewNew.RequiredViewSize(container.Items.Length);
			return new ItemContainerViewNew(container, new Rectangle(position.X, position.Y, requiredSize.Width, requiredSize.Height));			
		}
	}
}
