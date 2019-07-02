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
		private readonly StatusViewer<CharacterStatus<BuffEffect>, BuffEffect> _buffs;
		private readonly StatusViewer<CharacterStatus<DebuffEffect>, DebuffEffect> _debuffs;
		private readonly NewItemNotificationViewer _newItems;

		private ImageTexture _gameViewBorder;

		private IWorldEntity _highlightedEntity;

		public GamePlayManager(Rectangle bounds, string playerId) : base(bounds, background: "wood")
		{
			// Might want a couple frame delay before actually running the game?
			_activator.Register(_world = new World(playerId), true, "top");
			_world.Initialize();
			_world.Character.OnHeldItemChanged += InputManager.HandleCursorChange;
			_world.Character.OnGotExternalItem += Character_OnGotExternalItem;
			_world.OnCharacterDied += _world_OnCharacterDied;

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

			// These are going into the global group, so they don't get disabled when things stop the world...of course their time does not countdown
			// since the character is stopped, but the graphics rendering (effects on the icons) still runs....thse should be in a group with the world, but...
			// Since we don't have the ability to set groups back to their previous states, we would have to turn these back on manually like we do the world
			// in every place...so we'll leave this for now but when the ActivationManager is improved with the ability to return to previous state this will be fixed...
			_activator.Register(_buffs = new StatusViewer<CharacterStatus<BuffEffect>, BuffEffect>(
				new Rectangle(this.Bounds.TopRightVector(-300 - this.ContentMargin.Width, this.ContentMargin.Height * 6).ToPoint(), new Point(300, 120)), _world.Character.Buffs), true
			);

			_activator.Register(_debuffs = new StatusViewer<CharacterStatus<DebuffEffect>, DebuffEffect>(
				new Rectangle(this.Bounds.TopRightVector(-300 - this.ContentMargin.Width, this.ContentMargin.Height * 6 + 130).ToPoint(), new Point(300, 120)), _world.Character.Debuffs), true
			);

			// Eventually make a component for display of this stuff on the right...
			_defense = new ImageText("", true) { Position = this.Bounds.TopRightVector(-100 - this.ContentMargin.Width, 400) };

			_activator.Register(_newItems = new NewItemNotificationViewer(
				new Rectangle(
					this.Bounds.Right - NewItemNotification.Size.Width - this.ContentMargin.Width - 20,
					this.Bounds.Bottom - 280 - this.ContentMargin.Width, NewItemNotification.Size.Width, 300
				)), true
			);

			_highlightedEntity = null;
			//AudioManager.Start();
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_world.LoadContent();
			_camera.LoadContent();
			_gameViewBorder.LoadContent();
			_characterWindow.LoadContent();
			_inventoryWindow.LoadContent();
			_hotbarView.LoadContent();
			_hotbarView.ActiveItemChange();
			_barHealth.LoadContent();
			_barMana.LoadContent();
			_buffs.LoadContent();
			_debuffs.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_gameViewBorder.UnloadContent();
			_world.UnloadContent();
			_camera.UnloadContent();
			_characterWindow.UnloadContent();
			_inventoryWindow.UnloadContent();
			_hotbarView.UnloadContent();
			_barHealth.UnloadContent();
			_barMana.UnloadContent();
			_buffs.UnloadContent();
			_debuffs.UnloadContent();
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
			_world.Update(gameTime, InputManager.MouseOver(_gameViewArea));
			_barHealth.Update(gameTime);
			_barMana.Update(gameTime);
			UpdateVisibleStats(gameTime);
			_buffs.Update(gameTime);
			_debuffs.Update(gameTime);
			_newItems.Update(gameTime);
			_gameViewBorder.Update(gameTime);
			// For now we're allowing hotbar to remain active, which is ok since the modals which block the game from running also block
			// all subsequent input handling....however really, we need to make the change mentioned in the InventoryItemView class, so that
			// this doesn't need to remain in the Update loop to keep it's items correctly drawn (should be able to realtime get the items
			// from the underlying inventory container)....
			_hotbarView.Update(gameTime);
			if (_world.IsActive)
			{
				_camera.Update(gameTime);
			}
			base.UpdateActive(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (_highlightedEntity != null)
			{
				_highlightedEntity.IsHighlighted = false;
				_highlightedEntity = null;
			}

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
			else if (InputManager.KeyPressed(Keys.Space))
			{
				// Testing transitions...this probably won't actually be how we interact with them lol
				foreach (var transition in _world.MapObjects.GetEntities<WorldTransition>(_world.Character.Bounds))
					if (Vector2.Distance(transition.Position, _world.Character.Position) < 30.0f)
					{
						TransitionMap(transition);
						break;
					}
			}

			// TEMP: Need to see if it's over anything that should be highlighted...
			if (InputManager.MouseOver(_gameViewArea))
			{
				var mousePosition = InputManager.MousePosition.Offset(-(int)_camera.RenderOffset.X, -(int)_camera.RenderOffset.Y);

				foreach (var entity in _world.MapObjects.GetEntities(mousePosition).Where(e => e != _world.Character))
				{
					if (entity.Bounds.Contains(mousePosition))
					{						
						entity.IsHighlighted = true;
						_highlightedEntity = entity;
						break;
					}
				}

				// There's really no great way to prevent this from also swinging an active item...maybe we shouldn't auto-drop the
				// held item just because it's on the cursor?
				if (InputManager.LeftMouseClick())
				{
					if (_world.Character.IsItemHeld)
						_world.AddItem(_world.Character.DropHeld(), pickup: false);
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
			_debuffs.Draw(spriteBatch);
			_newItems.Draw(spriteBatch);
		}

		private ImageTexture GenerateGameViewBorder()
		{
			var texture = Util.GenerateBorderTexture(
				_gameViewArea.Width + (GamePlayManager.ViewAreaBorderThickness * 2),
				_gameViewArea.Height + (GamePlayManager.ViewAreaBorderThickness * 2),
				GamePlayManager.ViewAreaBorderThickness,
				Color.Black);
			texture.Alignment = ImageAlignment.Centered;
			texture.Position = _gameViewArea.CenterVector();
			return texture;
		}

		private void _characterWindow_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_activator.SetState(_characterWindow, false);
		}

		private void _inventoryView_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_activator.SetState(_inventoryWindow, false);
		}

		// Eventually we may want to encapsulate this in some kind of control that shows all these things and removes this from the GamePlayManager....
		private void UpdateVisibleStats(GameTime gameTime)
		{
			// Currently the only stat is Defense...
			_defense.UpdateText($"Defense: {_world.Character.Defense}");
			_defense.Update(gameTime);
		}

		private void _hotbarView_OnActiveItemChange(object sender, ComponentEventArgs e)
		{
			_world.Character.ActiveItem = (InventoryItem)e.Meta;
		}

		protected override void _dialog_OnReadyDisable(object sender, ComponentEventArgs e)
		{
			_activator.SetState(_dialog, false);
		}

		private void _world_OnCharacterDied(object sender, ComponentEventArgs e)
		{
			ShowNotification("You died!!", this.Bounds, "top");
		}

		private void Character_OnGotExternalItem(object sender, ComponentEventArgs e)
		{
			_newItems.AddNotification(e.Meta as InventoryItem);
		}

		private void TransitionMap(WorldTransition transition)
		{
			// Should show a loading thing here...
			_world.ChangeMap(transition.DestinationMap, transition.DestinationPosition);
			_camera.UnloadContent();
			_camera.LoadContent();
		}
	}
}
