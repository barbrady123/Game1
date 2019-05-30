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
	public class ItemContainerView : IActivatable
	{
		private const int ItemsPerRow = 10;
		private const int ItemViewSize = Game1.TileSize + ((InventoryItemView.BorderWidth + InventoryItemView.ImagePadding) * 2);
		private static readonly Vector2 ContentMargin = new Vector2(20, 60);
		private const int ItemViewPadding = 10;

		private readonly Rectangle _bounds;

		private InventoryItemView[] _itemViews;
		private readonly ItemContainer _container;
		private ImageTexture _backgroundImage;
		private ImageText _textImage;
		private MenuScreen _buttonMenu;
		private string _text;
		private bool _isActive;

		public string Text
		{ 
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					_textImage.UpdateText(_text);					
				}
			}
		}

		public bool IsActive
		{
			get { return _isActive; }
			set {
				if (_isActive != value)
				{
					_isActive = value;
					if (_isActive)
						DelayInput(1);
				}
			}
		}

		public event EventHandler OnButtonClick;
		public event EventHandler OnReadyDisable;

		private int _delayInputCycles;

		public void DelayInput(int delayCycles)
		{
			_delayInputCycles = Math.Max(0, delayCycles);
		}

		public int? Duration { get; set; }

		public ItemContainerView(ItemContainer container, string text, Rectangle bounds)
		{
			_container = container;
			_text = text ?? "";
			this.IsActive = false;
			_bounds = bounds;
			_backgroundImage = new ImageTexture($"{Game1.BackgroundRoot}/maze", true) { 
				Alignment = ImageAlignment.Centered,
				Scale = Vector2.One,
				DrawArea = _bounds,
				Position = _bounds.SizeVector() / 2
			};
			_itemViews = new InventoryItemView[_container.Size];
			for (int i = 0; i < _itemViews.Length; i++)
			{
				_itemViews[i] = new InventoryItemView(ItemContainerView.ItemViewSize) { Position = CalculateItemViewPosition(i) };
			}

			_textImage = new ImageText(_text, true) { Position = _bounds.CenterVector(yOffset: - _bounds.Height / 2 + 30) };
			//_buttonMenu = new InventoryContainerMenu(new Rectangle(_bounds.X, _bounds.Y + (int)(_bounds.Height * 0.75f),  _bounds.Width, (int)(_bounds.Height * 0.25f))) { IsActive = true };
			//_buttonMenu.OnItemSelect += _buttonMenu_OnItemSelect;
			//_buttonMenu.OnReadyDisable += _buttonMenu_OnReadyDisable;	// Do we need this, or should it just be on this control???
			_delayInputCycles = 0;
		}

		public void LoadContent()
		{
			_backgroundImage.LoadContent();
			//_buttonMenu?.LoadContent();
			_textImage.LoadContent();

			foreach (var item in _itemViews)
				item.LoadContent();
		}

		public void UnloadContent()
		{
			_backgroundImage.UnloadContent();
			//_buttonMenu?.UnloadContent();
			_textImage.UnloadContent();

			foreach (var item in _itemViews)
				item.UnloadContent();
		}

		public void Update(GameTime gameTime)
		{
			if (!this.IsActive)
				return;

			_backgroundImage.Update(gameTime);
			_textImage.Update(gameTime);

			if (_delayInputCycles != 0)
			{
				_delayInputCycles = Math.Max(0, _delayInputCycles - 1);
				return;
			}

			if (_buttonMenu != null)
			{
				_buttonMenu.Update(gameTime, this.IsActive);
			}
			else
			{
				if (InputManager.KeyPressed(Keys.Escape, true))
					_buttonMenu_OnReadyDisable(this, null);
			}
		}

		public void Draw()
		{
			if (!this.IsActive)
				return;

			Util.WrappedDraw(DrawInternal, "modal", _bounds);
		}

		private void DrawInternal(SpriteBatch spriteBatch)
		{
			_backgroundImage.Draw(spriteBatch);
			_buttonMenu?.Draw(spriteBatch);
			_textImage.Draw(spriteBatch);
			foreach (var item in _itemViews)
				item.Draw(spriteBatch);
		}

		private Vector2 CalculateItemViewPosition(int index)
		{
			int row = index / ItemContainerView.ItemsPerRow;
			int col = index % ItemContainerView.ItemsPerRow;

			int xPos = _bounds.X
						+ (int)ItemContainerView.ContentMargin.X
						+ (ItemContainerView.ItemViewSize * col)
						+ (ItemContainerView.ItemViewPadding * col);
			int yPos = _bounds.Y
						+ (int)ItemContainerView.ContentMargin.Y
						+ (ItemContainerView.ItemViewSize * row)
						+ (ItemContainerView.ItemViewPadding * row);

			return new Vector2(xPos, yPos);
		}

		private void _buttonMenu_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;
			OnButtonClick?.Invoke(this, new ScreenEventArgs("select", this.GetType().Name, args.Item));
		}

		private void _buttonMenu_OnReadyDisable(object sender, EventArgs e)
		{
			OnReadyDisable?.Invoke(this, new ScreenEventArgs("escape", this.GetType().Name, null));
		}
	}
}
