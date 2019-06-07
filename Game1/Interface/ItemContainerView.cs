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
		private const int ItemsPerRow = 10;
		private const int ItemViewPadding = 10;

		private InventoryItemView[] _itemViews;
		
		public ItemContainer Container { get; private set; }

		private static readonly Size ContentMargin = new Size(10, 10);

		public Rectangle Bounds { get; set; }		

		public bool HightlightActiveItem { get; set; }

		public event EventHandler OnMouseClick;
		public event EventHandler OnMouseOver;
		public event EventHandler OnMouseOut;

		public ItemContainerView(ItemContainer container, Rectangle bounds, bool highlightActiveItem)
		{
			this.Bounds = bounds;
			this.Container = container;
			_itemViews = new InventoryItemView[this.Container.Size];
			for (int i = 0; i < _itemViews.Length; i++)
			{
				var position = CalculateItemViewPosition(i);
				_itemViews[i] = new InventoryItemView(position.ExpandToRectangeTopLeft(InventoryItemView.Size, InventoryItemView.Size), i, null);
				_itemViews[i].OnMouseClick += ItemContainerView_OnMouseClick;
				_itemViews[i].OnMouseOver += ItemContainerView_OnMouseOver;
				_itemViews[i].OnMouseOut += ItemContainerView_OnMouseOut;
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

		public void Update(GameTime gameTime, bool processInput)
		{
			UpdateItems(gameTime, processInput);
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
						+ (InventoryItemView.Size * col)
						+ (ItemContainerView.ItemViewPadding * col);
			int yPos = this.Bounds.Y
						+ ItemContainerView.ContentMargin.Height
						+ (InventoryItemView.Size * row)
						+ (ItemContainerView.ItemViewPadding * row);

			return new Vector2(xPos, yPos);
		}

		private void UpdateItems(GameTime gameTime, bool processInput)
		{
			for (int i = 0; i < _itemViews.Length; i++)
			{
				_itemViews[i].Item = this.Container[i];
				_itemViews[i].Highlight = processInput && this.HightlightActiveItem && (this.Container.ActiveItemIndex == i);
				_itemViews[i].Update(gameTime, processInput);
			}
		}

		public static Size RequiredViewSize(int numItems)
		{
			int width = (InventoryItemView.Size * ItemsPerRow) + (ItemViewPadding * (ItemsPerRow - 1)) + (ContentMargin.Width * 2);
			int numRows = (numItems / ItemsPerRow) + ((numItems % ItemsPerRow) > 0 ? 1 : 0);
			int height = (InventoryItemView.Size * numRows) + (ItemViewPadding * (numRows - 1)) + (ContentMargin.Height * 2);
			return new Size(width, height);
		}

		public static T New<T>(ItemContainer container, Point position, bool hightlightActiveItem) where T: ItemContainerView
		{
			var requiredSize = ItemContainerView.RequiredViewSize(container.Items.Length);
			return (T)Activator.CreateInstance(typeof(T), container, new Rectangle(position.X, position.Y, requiredSize.Width, requiredSize.Height), hightlightActiveItem);
		}

		private void ItemContainerView_OnMouseClick(object sender, EventArgs e)
		{
			OnMouseClick?.Invoke(this, e);
		}

		private void ItemContainerView_OnMouseOver(object sender, EventArgs e)
		{
			OnMouseOver?.Invoke(this, e);
		}

		private void ItemContainerView_OnMouseOut(object sender, EventArgs e)
		{
			OnMouseOut?.Invoke(this, e);
		}
	}
}
