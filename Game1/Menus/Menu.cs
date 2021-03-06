﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;
using Game1.Enum;

namespace Game1.Menus
{	
	public abstract class Menu : Component
	{
		public const int MENU_PADDING = 20;
		private const float ENABLED_MENU_ALPHA = 1.0f;
		private const float DISABLED_MENU_ALPHA = 0.4f;
		private const float DEFAULT_ITEM_ALPHA = 1.0f;

		private static readonly Color ActiveItemColor = Color.White;
		private static readonly Color SelectedItemColor = new Color(240, 240, 240);
		private static readonly Color UnselectedItemColor = new Color(100, 100, 100);

		protected int _currentIndex;
		protected List<MenuItem> _items;
		protected MenuLayout _layout; 

		public event EventHandler<ComponentEventArgs> OnCurrentItemChange;
		public event EventHandler<ComponentEventArgs> OnItemSelect;

		public int ItemCount => _items?.Count ?? 0;

		protected override void IsActiveChange()
		{
			if (this.IsActive)
			{
				SetCurrentItemEffects(true);
				DelayInput(1);
			}
			else
			{
				SetCurrentItemEffects(false);
				for (int x = 0; x < _items.Count; x++)
				{
					if (_items[x].Image != null)
						_items[x].Image.Color = (x == _currentIndex) ? Menu.SelectedItemColor : Menu.UnselectedItemColor;
				}
			}
		}

		public int CurrentIndex
		{
			get { return _currentIndex; }
			set {
				if (_currentIndex == value)
					return;
				
				SetCurrentItemEffects(false);
				_currentIndex = value;
				SetCurrentItemEffects(true);
			}
		}

		public void ClearSelection()
		{
			SetCurrentItemEffects(false);
			_currentIndex = -1;
		}

		public Menu(Point position,
						  MenuLayout layout = MenuLayout.Vertical,
						  string background = "brick",
						  SpriteBatchData spriteBatchData = null,
						  bool escapeToDisable = false,
						  bool fireMouseEvents = true,
						  bool inactiveMouseEvents = false,
						  bool drawIfDisabled = false): base(Rectangle.Empty, escapeToDisable, background, spriteBatchData, false, fireMouseEvents, inactiveMouseEvents, drawIfDisabled)
		{
			_currentIndex = -1;
			_layout = layout;
			this.Bounds = SetupMenuItems(position);
			SetupMenuItemPositions();
		}

		public override void LoadContent()
		{			
			base.LoadContent();
		}

		public void UpdatePosition(Point position)
		{
			this.Bounds = new Rectangle(position.X, position.Y, this.Bounds.Width, this.Bounds.Height);
		}

		protected override void BoundsChanged(bool resized)
		{
			base.BoundsChanged(resized);
			SetupMenuItemPositions();
		}

		protected abstract List<string> GetItemData();

		protected virtual Rectangle SetupMenuItems(Point position)
		{		
			var menuOptions = this.GetItemData();
			if (!menuOptions.Any())
				return Rectangle.Empty;

			_items = menuOptions.Select(o => new MenuItem { Id = o.Replace(" ", "").ToLower(), Text = o }).ToList();

			if (position == Util.PointInvalid)
				return Rectangle.Empty;
			var size = this.CalculateMenuSize(menuOptions);
			return new Rectangle(position.X, position.Y, size.Width, size.Height);
		}

		protected virtual void SetupMenuItemPositions()
		{
			if ((!(_items?.Any() ?? false)) || (this.Bounds == Rectangle.Empty))
				return;

			int menuSize = 0;

			foreach (var item in _items)
			{
				item.Image = new ImageText(item.Text, true, ImageAlignment.LeftTop);
				menuSize += (menuSize > 0 ? MENU_PADDING : 0) + (_layout == MenuLayout.Vertical ? (int)item.Image.Size.Y : (int)item.Image.Size.X);
			}

			int locX = this.Bounds.X;
			int locY = this.Bounds.Y;

			if (_layout == MenuLayout.Vertical)
			{
				locX += (int)(this.Bounds.Width / 2);
				locY += (int)(this.Bounds.Height - menuSize) / 2;
			}
			else
			{
				locX += (int)(this.Bounds.Width - menuSize) / 2;
				locY += (int)(this.Bounds.Height / 2);
			}

			foreach (var item in _items)
			{
				var size = item.Image.Size;
				item.Image.Position = new Vector2(
					locX - ((_layout == MenuLayout.Vertical) ? (int)(size.X / 2) : 0),
					locY - ((_layout == MenuLayout.Horizontal) ? (int)(size.Y / 2) : 0));
				item.Bounds = item.Image.Position.ExpandToRectangleTopLeft(size.X, size.Y);
				if (_layout == MenuLayout.Vertical)
					locY += (int)item.Image.Size.Y + MENU_PADDING;
				else
					locX += (int)item.Image.Size.X + MENU_PADDING;
			}
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			for (int x = 0; x < _items.Count; x++)
				_items[x].Image.Color = Menu.ActiveItemColor;

			foreach (var item in _items)
				item.Image.Update(gameTime);

			base.UpdateActive(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);

			bool mouseOverItem = false;

			for (int i = 0; i < _items.Count; i++)
			{
				if (!InputManager.MouseOver(_items[i].Bounds))
					continue;

				mouseOverItem = true;

				if (this.CurrentIndex != i)
				{
					this.CurrentIndex = i;
					CurrentItemChange(new ComponentEventArgs { Value = _items[_currentIndex]?.Id });
				}

				if (InputManager.LeftMouseClick())
					ItemSelect(new ComponentEventArgs { Value = _items[_currentIndex]?.Id });

				break;
			}

			if (!mouseOverItem)
				this.CurrentIndex = -1;
		}


		public void SelectItem(int index)
		{
			if (!Util.InRange(index, 0, _items.Count))
				return;

			this.CurrentIndex = index;
			ItemSelect(new ComponentEventArgs { Value = _items[_currentIndex]?.Id });
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);

			foreach (var item in _items)
				item.Image.Draw(spriteBatch, alphaBlend: this.IsActive ? Menu.ENABLED_MENU_ALPHA : Menu.DISABLED_MENU_ALPHA);
		}

		public int SetById(string id)
		{
			for (int i = 0; i < _items.Count; i++)
				if (_items[i].Id == id)
				{
					this.CurrentIndex = i;
					return i;
				}

			return -1;
		}

		private void SetCurrentItemEffects(bool active)
		{
			if ((_currentIndex < 0) || !_items.Any())
				return;

			if (active)
			{
				_items[_currentIndex].Image.AddEffect<FadeCycleEffect>(true);
			}
			else
			{
				_items[_currentIndex].Image.StopEffect(typeof(FadeCycleEffect));
			}
		}

		public Size CalculateMenuSize(List<string> options, bool includePadding = true)
		{
			if (options == null)
			{
				options = this.GetItemData();
				if (!options.Any())
					return new Size(0, 0);
			}

			float width = 0;
			float height = 0;

			foreach (var option in options)
			{
				var size = FontManager.MeasureString(option);
				width = (_layout == MenuLayout.Vertical) ? Math.Max(width, size.X) : width + size.X;
				height = (_layout == MenuLayout.Vertical) ? height + size.Y : Math.Max(height, size.Y);
			}

			int paddingWidth = (includePadding ? Menu.MENU_PADDING * 2 : 0);
			if (_layout == MenuLayout.Horizontal)
				paddingWidth += Menu.MENU_PADDING * (options.Count - 1);

			int paddingHeight = (includePadding ? Menu.MENU_PADDING * 2 : 0);
			if (_layout == MenuLayout.Vertical)
				paddingHeight += Menu.MENU_PADDING * (options.Count - 1);

			return new Size((int)width + paddingWidth, (int)height + paddingHeight);
		}
		
		protected virtual void CurrentItemChange(ComponentEventArgs args)
		{
			OnCurrentItemChange?.Invoke(this, args);
		}

		protected virtual void ItemSelect(ComponentEventArgs e)
		{
			OnItemSelect?.Invoke(this, e);
			InputManager.BlockAllInput();
		}
	}
}
