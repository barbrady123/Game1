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
	public class ItemContainerView : Component
	{
		private const int ItemsPerRow = 10;
		private const int ItemViewPadding = 10;

		private InventoryItemView[] _itemViews;
		private int _activeItemIndex;
		
		public int ActiveItemIndex
		{
			get { return _activeItemIndex; }
			set 
			{
				int val = Util.Clamp(value, 0, this.Container.Size - 1);
				if (_activeItemIndex != val)
				{
					_activeItemIndex = val;
					ActiveItemChange();
				}
			}
		}

		public ItemContainer Container { get; private set; }

		protected override Size ContentMargin => new Size(10, 10);

		public bool HightlightActiveItem { get; set; }

		public event EventHandler<ComponentEventArgs> OnMouseClick;
		public event EventHandler<ComponentEventArgs> OnActiveItemChange;

		public int Size => this.Container?.Size ?? 0;

		public ItemContainerView(ItemContainer container, Rectangle bounds, bool highlightActiveItem) : base(bounds, background: null, fireMouseEvents: false, enabledTooltip: true)
		{
			this.Container = container;
			this.Container.OnItemChanged += Container_OnItemChanged;
			_itemViews = new InventoryItemView[this.Container.Size];
			for (int i = 0; i < _itemViews.Length; i++)
			{
				var position = CalculateItemViewPosition(i);
				_itemViews[i] = new InventoryItemView(position.ExpandToRectangeTopLeft(InventoryItemView.Size, InventoryItemView.Size), i, null, this);
				_itemViews[i].OnMouseClick += ItemContainerView_OnMouseClick;
				_itemViews[i].OnMouseOver += ItemContainerView_OnMouseOver;
				_itemViews[i].OnMouseOut += ItemContainerView_OnMouseOut;				
				_itemViews[i].IsActive = true;
			}
			this.HightlightActiveItem = highlightActiveItem;
			this.ActiveItemIndex = 0;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			foreach (var item in _itemViews)
				item.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			foreach (var item in _itemViews)
				item.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			UpdateItems(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			foreach (var item in _itemViews)
				item.Draw(spriteBatch);
		}

		public void ActiveItemChange()
		{
			OnActiveItemChange?.Invoke(this, new ComponentEventArgs { Index = _activeItemIndex, Meta = this.Container[_activeItemIndex] });
		}

		private Vector2 CalculateItemViewPosition(int index)
		{
			int row = index / ItemContainerView.ItemsPerRow;
			int col = index % ItemContainerView.ItemsPerRow;

			int xPos = this.Bounds.X
						+ this.ContentMargin.Width
						+ (InventoryItemView.Size * col)
						+ (ItemContainerView.ItemViewPadding * col);
			int yPos = this.Bounds.Y
						+ this.ContentMargin.Height
						+ (InventoryItemView.Size * row)
						+ (ItemContainerView.ItemViewPadding * row);

			return new Vector2(xPos, yPos);
		}

		private void UpdateItems(GameTime gameTime)
		{
			for (int i = 0; i < _itemViews.Length; i++)
			{
				_itemViews[i].Item = this.Container[i];
				_itemViews[i].Highlight = this.HightlightActiveItem && (this.ActiveItemIndex == i);
				_itemViews[i].Update(gameTime);
			}
		}

		public static Size RequiredViewSize(int numItems)
		{
			// This no longer includes the containing window padding (20,20)....
			int width = (InventoryItemView.Size * ItemsPerRow) + (ItemViewPadding * (ItemsPerRow - 1)) + (ItemViewPadding * 2);
			int numRows = (numItems / ItemsPerRow) + ((numItems % ItemsPerRow) > 0 ? 1 : 0);
			int height = (InventoryItemView.Size * numRows) + (ItemViewPadding * (numRows - 1)) + (ItemViewPadding* 2);
			return new Size(width, height);
		}

		public static T New<T>(ItemContainer container, Point position, bool hightlightActiveItem) where T: ItemContainerView
		{
			var requiredSize = ItemContainerView.RequiredViewSize(container.Items.Length);
			return (T)Activator.CreateInstance(typeof(T), container, new Rectangle(position.X, position.Y, requiredSize.Width, requiredSize.Height), hightlightActiveItem);
		}

		private void ItemContainerView_OnMouseClick(object sender, ComponentEventArgs e)
		{
			e.Meta = sender;
			OnMouseClick?.Invoke(this, e);
		}

		private void ItemContainerView_OnMouseOver(object sender, ComponentEventArgs e)
		{
			e.Meta = sender;
			MouseOver(e);
		}

		private void ItemContainerView_OnMouseOut(object sender, ComponentEventArgs e)
		{
			e.Meta = sender;
			MouseOut(e);
		}

		private void Container_OnItemChanged(object sender, ComponentEventArgs e)
		{
			if (e.Index == this.ActiveItemIndex)
				ActiveItemChange();
		}
	}
}
