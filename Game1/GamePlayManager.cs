using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;
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

		private readonly Rectangle _gameViewArea;
		private readonly GamePlayCamera _camera;
		private readonly World _world;
		private readonly CharacterWindow _characterWindow;
		private readonly InventoryWindow _inventoryWindow;
		private readonly HotbarView _hotbarView;
		private readonly StatBar _barHealth;
		private readonly StatBar  _barMana;
		private readonly ImageText _defense;
		private readonly StatusViewer<CharacterBuff> _buffs;

		private ImageTexture _gameViewBorder;

		public GamePlayManager(Rectangle bounds) : base(bounds, true, "stone")
		{
			// Might want a couple frame delay before actually running the game?
			_activator.Register(_world = new World(), true, "top");
			_world.Initialize();
			_world.Character.OnHeldItemChanged += InputManager.HandleCursorChange;

			_gameViewArea = new Rectangle(
				this.ContentMargin.Width + GamePlayManager.ViewAreaBorderThickness,
				this.ContentMargin.Height + GamePlayManager.ViewAreaBorderThickness,
				Game1.TileSize * Game1.GameViewAreaWidth,
				Game1.TileSize * Game1.GameViewAreaHeight);
			_gameViewBorder = GenerateGameViewBorder();

			_camera = new GamePlayCamera(_world, _gameViewArea, SpriteBatchManager.Get("gameplay"));

			var modalSpriteBatchData = SpriteBatchManager.Get("modal");
			_activator.Register(_characterWindow = new CharacterWindow(this.Bounds.CenteredRegion(870, 575), _world, modalSpriteBatchData), false, "top");
			_characterWindow.OnReadyDisable += _characterWindow_OnReadyDisable;

			_activator.Register(_inventoryWindow = new InventoryWindow(this.Bounds.CenteredRegion(870, 575),  _world, "Backpack", modalSpriteBatchData), false, "top");
			_inventoryWindow.OnReadyDisable += _inventoryView_OnReadyDisable;

			_activator.Register(_hotbarView = ItemContainerView.New<HotbarView>(_world.Character.HotBar, new Point(this.ContentMargin.Width, _gameViewArea.Bottom + this.ContentMargin.Height), true), true);
			_hotbarView.OnActiveItemChange += _hotbarView_OnActiveItemChange;

			_activator.Register(_barHealth = new StatBar(
				GamePlayManager.StatBarSize, 
				this.Bounds.TopRightVector((-GamePlayManager.StatBarSize / 2) - this.ContentMargin.Width, this.ContentMargin.Height + StatBar.Height / 2),
				Color.Red,
				_world.Character,
				"CurrentHP",
				"MaxHP"
			), true);
			_activator.Register(_barMana = new StatBar(
				GamePlayManager.StatBarSize,
				this.Bounds.TopRightVector((-GamePlayManager.StatBarSize / 2) - this.ContentMargin.Width, this.ContentMargin.Height * 3 + StatBar.Height / 2),
				Color.Blue,
				_world.Character,
				"CurrentMana",
				"MaxMana"
			), true);

			_activator.Register(_buffs = new StatusViewer<CharacterBuff>(new Rectangle(this.Bounds.TopRightVector(-300 - this.ContentMargin.Width, this.ContentMargin.Height * 6).ToPoint(), new Point(300, 120)), _world.Character.Buffs), true);

			// Eventually make a component for display of this stuff on the right...
			_defense = new ImageText("", true) { Position = this.Bounds.TopRightVector(-100 - this.ContentMargin.Width, 350) };
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
			_hotbarView.ActiveItemChange();
			_barHealth.LoadContent();
			_barMana.LoadContent();
			_buffs.LoadContent();
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
			_barHealth.UnloadContent();
			_barMana.UnloadContent();
			_buffs.UnloadContent();
			_defense.UnloadContent();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void UpdateActive(GameTime gameTime)
		{
			// Modal windows go first, block all input after update...
			_characterWindow.Update(gameTime);
			_inventoryWindow.Update(gameTime);
			_hotbarView.Update(gameTime);
			_barHealth.Update(gameTime);
			_barMana.Update(gameTime);
			UpdateVisibleStats();
			_buffs.Update(gameTime);
			_defense.Update(gameTime);
			_gameViewBorder.Update(gameTime);
			_world.Update(gameTime);
			_camera.Update(gameTime);		
			base.UpdateActive(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			// TODO: Move this to a better location...should be it's own component with no UI, so it can be disabled correctly below
			if (InputManager.KeyPressed(Keys.I))
			{
				_activator.SetState(_inventoryWindow, true);
			}
			else if (InputManager.KeyPressed(Keys.C))
			{
				_activator.SetState(_characterWindow, true);
			}
			else if (InputManager.KeyPressed(Keys.OemTilde))
			{
				Game1.Instance.ToggleFullScreen();
			}

			if (InputManager.LeftMouseClick(_gameViewArea))
			{
				if (_world.Character.IsItemHeld)
				{
					_world.AddItem(_world.Character.DropHeld(), pickup: false);
				}
				else if (_world.Character.ActiveItem?.Item is ItemWeapon weapon)
				{
					// TEMP...probably shouldn't be here...need like a Character.UseActiveItem() method or something...
					if ((_world.Character.Direction == Cardinal.North) || (_world.Character.Direction == Cardinal.West))
						_world.Character.ActiveItem.Icon.AddEffect<UseItemWestEffect>(true);
					else
						_world.Character.ActiveItem.Icon.AddEffect<UseItemEastEffect>(true);
				}
			}

			base.UpdateInput(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_gameViewBorder.Draw(spriteBatch);
			_camera.Draw();
			_characterWindow.Draw(spriteBatch);
			_inventoryWindow.Draw(spriteBatch);
			_hotbarView.Draw(spriteBatch);
			_barHealth.Draw(spriteBatch);
			_barMana.Draw(spriteBatch);
			_defense.Draw(spriteBatch);
			_buffs.Draw(spriteBatch);
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
			_activator.SetState(_world, true);
		}

		private void _inventoryView_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_activator.SetState(_world, true);
		}

		// Eventually we may want to encapsulate this in some kind of control that shows all these things and removes this from the GamePlayManager....
		private void UpdateVisibleStats()
		{
			// Currently the only stat is Defense...
			_defense.UpdateText($"Defense: {_world.Character.Defense}");
		}

		private void _hotbarView_OnActiveItemChange(object sender, ComponentEventArgs e)
		{
			_world.Character.ActiveItem = (InventoryItem)e.Meta;
		}
	}
}
