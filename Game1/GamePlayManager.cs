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
using Game1.Interface;
using Game1.Interface.Windows;
using Game1.Items;

namespace Game1
{
	public class GamePlayManager : Component
	{
		public const int StatBarSize = 300;
		public const int ViewAreaBorderThickness = 3;

		protected override Size ContentMargin => new Size(25, 25);

		private readonly ComponentManager _components;
		private readonly Rectangle _gameViewArea;
		private readonly GamePlayCamera _camera;
		private readonly World _world;
		private readonly CharacterWindow _characterWindow;
		private readonly InventoryWindow _inventoryWindow;
		private readonly HotbarView _hotbarView;
		private readonly Dialog _tooltip;
		private readonly StatBar _barHealth;
		private readonly StatBar  _barMana;
		private readonly ImageText _defense;

		private ImageTexture _gameViewBorder;

		public GamePlayManager(Rectangle bounds) : base(bounds, true, "stone")
		{
			_components = new ComponentManager();

			_components.Register(_world = new World());
			_world.Initialize();

			_gameViewArea = new Rectangle(
				this.ContentMargin.Width + GamePlayManager.ViewAreaBorderThickness,
				this.ContentMargin.Height + GamePlayManager.ViewAreaBorderThickness,
				Game1.TileSize * Game1.GameViewAreaWidth,
				Game1.TileSize * Game1.GameViewAreaHeight);
			_gameViewBorder = GenerateGameViewBorder();

			_camera = new GamePlayCamera(_world, _gameViewArea);

			_components.Register(_characterWindow = new CharacterWindow(this.Bounds.CenteredRegion(870, 575), _world.Character));
			_characterWindow.OnReadyDisable += _characterWindow_OnReadyDisable;

			_components.Register(_inventoryWindow = new InventoryWindow(this.Bounds.CenteredRegion(870, 575),  _world.Character, "Backpack"));
			_inventoryWindow.OnReadyDisable += _inventoryView_OnReadyDisable;

			_components.Register(_hotbarView = ItemContainerView.New<HotbarView>(_world.Character.HotBar, new Point(this.ContentMargin.Width, _gameViewArea.Bottom + this.ContentMargin.Height), true));
			_hotbarView.OnMouseClick += _hotbarView_OnMouseClick;
			_components.SetState(_hotbarView, ComponentState.All, null);

			_tooltip = new Dialog(null, DialogButton.None, Rectangle.Empty, null);
			_components.Register(_barHealth = new StatBar(
				GamePlayManager.StatBarSize, 
				this.Bounds.TopRightVector((-GamePlayManager.StatBarSize / 2) - this.ContentMargin.Width, this.ContentMargin.Height + StatBar.Height / 2),
				Color.Red,
				_world.Character,
				"CurrentHP",
				"MaxHP"
			));
			_components.SetState(_barHealth, ComponentState.ActiveVisible, null);
			_components.Register(_barMana = new StatBar(
				GamePlayManager.StatBarSize,
				this.Bounds.TopRightVector((-GamePlayManager.StatBarSize / 2) - this.ContentMargin.Width, this.ContentMargin.Height * 3 + StatBar.Height / 2),
				Color.Blue,
				_world.Character,
				"CurrentMana",
				"MaxMana"
			));
			_components.SetState(_barMana, ComponentState.ActiveVisible, null);

			// Eventually make ImageText(ure) consistent with the components so we can register them also (or create containers for basic images/text)...
			_defense = new ImageText("", true) { Position = this.Bounds.TopRightVector(-100 - this.ContentMargin.Width, this.ContentMargin.Height * 6) };

			// Might want a couple frame delay before actually running the game?
			_components.SetState(_world, ComponentState.Active, null);
		}

		public override void LoadContent()
		{
			base.LoadContent();
			ItemManager.LoadContent();
			_world.LoadContent();
			_camera.TerrainTileSheetName = _world.CurrentMap.TileSheet;
			_camera.TerrainLayerData  = _world.CurrentMap.Layers;
			_camera.LoadContent();
			_gameViewBorder.LoadContent();
			_characterWindow.LoadContent();
			_inventoryWindow.LoadContent();
			_hotbarView.LoadContent();
			_tooltip.LoadContent();
			_barHealth.LoadContent();
			_barMana.LoadContent();
			_defense.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			ItemManager.UnloadContent();
			_gameViewBorder.UnloadContent();
			_world.UnloadContent();
			_camera.UnloadContent();
			_characterWindow.UnloadContent();
			_inventoryWindow.UnloadContent();
			_hotbarView.UnloadContent();
			_tooltip.UnloadContent();
			_barHealth.UnloadContent();
			_barMana.UnloadContent();
			_defense.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			_characterWindow.Update(gameTime);
			_inventoryWindow.Update(gameTime);
			// HotBarView should be a component...
			_hotbarView.Update(gameTime);
			_tooltip.Update(gameTime);
			_barHealth.Update(gameTime);
			_barMana.Update(gameTime);
			UpdateVisibleStats();
			_defense.Update(gameTime);
			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_gameViewBorder.Update(gameTime);
			_world.Update(gameTime);
			_camera.Update(gameTime);		
			base.UpdateActive(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			// TODO: Move this to a better location...
			if (InputManager.KeyPressed(Keys.I))
			{
				// Techincally input shouldn't get this far if a window is open....resolve...
				if (!_characterWindow.State.HasFlag(ComponentState.Visible))
				{
					_components.ClearState(_world, ComponentState.Active);
					_components.SetState(_inventoryWindow, ComponentState.All, null);
				}
			}
			else if (InputManager.KeyPressed(Keys.C))
			{
				if (!_inventoryWindow.State.HasFlag(ComponentState.Visible))
				{
					_components.ClearState(_world, ComponentState.Active);
					_components.SetState(_characterWindow, ComponentState.All, null);
				}
			}

			// Hot bar functionality...this code should be moved...
			var hotbar = _world.Character.HotBar;
			int hotbarIndex = hotbar.ActiveItemIndex;
			if (InputManager.KeyPressed(Keys.Right) || (InputManager.MouseScrollAmount < 0))
			{
				hotbar.ActiveItemIndex = (hotbarIndex < hotbar.Size - 1) ? hotbarIndex + 1 : 0;
			}
			if (InputManager.KeyPressed(Keys.Left)  || (InputManager.MouseScrollAmount > 0))
			{
				hotbar.ActiveItemIndex = (hotbarIndex > 0 ) ? hotbarIndex - 1 : hotbar.Size - 1;
			}
			foreach (var key in InputManager.GetPressedKeys())
			{
				char newChar = InputManager.KeyToChar(key, false);
				if (Int32.TryParse(newChar.ToString(), out int val))
					hotbar.ActiveItemIndex = (val == 0 ? 10 : val) - 1;
			}
			// TEMP....
			if (InputManager.KeyPressed(Keys.H))
			{
				_world.Character.CurrentHP += 3;
				_world.Character.CurrentMana -= 1;
			}

			base.UpdateInput(gameTime);
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			base.DrawVisible(spriteBatch);
			_gameViewBorder.Draw(spriteBatch);
			_camera.Draw();
			var batchData = SpriteBatchManager.Get("modal");
			batchData.ScissorWindow = (_characterWindow.State.HasFlag(ComponentState.Visible) ? _characterWindow.Bounds : (_inventoryWindow.State.HasFlag(ComponentState.Visible) ? _inventoryWindow.Bounds : Rectangle.Empty));
			_characterWindow.Draw(batchData.SpriteBatch);
			_inventoryWindow.Draw(batchData.SpriteBatch);

			_hotbarView.Draw(spriteBatch);
			_tooltip.Draw(spriteBatch);
			_barHealth.Draw(spriteBatch);
			_barMana.Draw(spriteBatch);
			_defense.Draw(spriteBatch);
		}

		private ImageTexture GenerateGameViewBorder()
		{
			var texture = Util.GenerateBorderTexture(
				_gameViewArea.Width + (GamePlayManager.ViewAreaBorderThickness * 2),
				_gameViewArea.Height + (GamePlayManager.ViewAreaBorderThickness * 2),
				GamePlayManager.ViewAreaBorderThickness,
				Color.DarkSlateBlue);
			texture.Alignment = ImageAlignment.Centered;
			texture.Position = _gameViewArea.CenterVector();
			return texture;
		}

		private void _characterWindow_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_components.SetState(_characterWindow, ComponentState.None, null);
			_components.SetState(_world, ComponentState.Active, null);
		}

		private void _inventoryView_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_components.SetState(_inventoryWindow, ComponentState.None, null);
			_components.SetState(_world, ComponentState.Active, null);
		}

		private void _hotbarView_OnMouseClick(object sender, ComponentEventArgs e)
		{
			// Of course, this check shouldn't be necessary once the refactor is done,
			// as an inactive GamePlayManager should not be processing mouse clicks on any child...
			if (!this.State.HasFlag(ComponentState.Active))
				return; 

			var itemClicked = (InventoryItemView)e.Sender;
			if (itemClicked != null)
				_hotbarView.Container.ActiveItemIndex = itemClicked.Index;
		}

		// Eventually we may want to encapsulate this in some kind of control that shows all these things and removes this from the GamePlayManager....
		private void UpdateVisibleStats()
		{
			// Currently the only stat is Defense...
			_defense.UpdateText($"Defense: {_world.Character.Defense}");
		}
	}
}
