using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Game1.Interface.Windows
{
	public class CharacterWindow : Component
	{
		private readonly ComponentManager _components;
		private World _world;
		private InventoryItemView[] _armorItemView;
		private ImageText _characterName;
		private ImageText[] _characterStat;
		private InventoryContextMenu _contextMenu;

		private Tooltip _tooltip;

		public CharacterWindow(Rectangle bounds, World world, SpriteBatchData spriteBatchData = null) : base(bounds, true, "brick", spriteBatchData)
		{
			_components = new ComponentManager();

			_world = world;
			_armorItemView = new InventoryItemView[System.Enum.GetNames(typeof(ArmorSlot)).Length];

			_characterName = new ImageText(_world.Character.Name, true) {
				Alignment = ImageAlignment.Centered,
				Position = this.Bounds.TopCenterVector(yOffset: this.ContentMargin.Height + (FontManager.FontHeight / 2))
			};

			for (int i = 0; i < _armorItemView.Length; i++)
			{
				var position = this.Bounds.TopRightVector(-100, this.ContentMargin.Height + InventoryItemView.Size / 2 + (100 * i)).ExpandToRectangleCentered(InventoryItemView.Size / 2, InventoryItemView.Size / 2);
				_components.Register(_armorItemView[i] = new InventoryItemView(position, i, ((ArmorSlot)i).ToString("g")));
				_armorItemView[i].OnMouseClick += ArmorItemView_OnMouseClick;
				_armorItemView[i].OnMouseOver += ArmorItemView_OnMouseOver;
				_armorItemView[i].OnMouseOut += ArmorItemView_OnMouseOut;
			}
			_components.SetState(_armorItemView, ComponentState.All, null);
			
			_characterStat = new ImageText[System.Enum.GetNames(typeof(CharacterAttribute)).Length];
			for (int i = 0; i < _characterStat.Length; i++)
			{
				var position = this.Bounds.TopLeftVector(this.ContentMargin.Width, this.ContentMargin.Height + 20 + (this.ContentMargin.Height * 2 * i));
				_characterStat[i] = new ImageText(null, true) { Position = position, Alignment = ImageAlignment.LeftCentered };
			}

			_components.Register(_tooltip = new Tooltip(SpriteBatchManager.Get("tooltip")));
			_components.SetState(_tooltip, ComponentState.Active, null);

			_components.Register(_contextMenu = new InventoryContextMenu(SpriteBatchManager.Get("context")));
			_contextMenu.OnMouseOut += _contextMenu_OnMouseOut;
			_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_characterName.LoadContent();
			_tooltip.LoadContent();
			foreach (var armorView in _armorItemView)
				armorView.LoadContent();
			foreach (var stat in _characterStat)
				stat.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_characterName.UnloadContent();
			_tooltip.UnloadContent();
			_contextMenu.UnloadContent();
			foreach (var armorView in _armorItemView)
				armorView.UnloadContent();
			foreach (var stat in _characterStat)
				stat.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			_contextMenu.Update(gameTime);
			_characterName.Update(gameTime);
			_tooltip.Update(gameTime);
			UpdateArmorViews();
			foreach (var armorView in _armorItemView)
				armorView.Update(gameTime);
			UpdateCharacterStats();
			foreach (var stat in _characterStat)
				stat.Update(gameTime);
			base.UpdateActive(gameTime);
			InputManager.BlockAllInput();
		}

		public override void DrawVisible(SpriteBatch spriteBatch)
		{
			// Refactor this spritebatch stuff...
			base.DrawVisible(spriteBatch);
			_characterName.Draw(spriteBatch);
			foreach (var armorView in _armorItemView)
				armorView.Draw(spriteBatch);
			foreach (var stat in _characterStat)
				stat.Draw(spriteBatch);
			_tooltip.Draw(spriteBatch);
			_contextMenu.Draw(spriteBatch);
		}

		private void ArmorItemView_OnMouseClick(object sender, ComponentEventArgs e)
		{
			var itemView = (InventoryItemView)sender;

			if ((e.Button == MouseButton.Right) && (itemView?.Item != null))
				EnableContextMenu(itemView);
		}

		private void ArmorItemView_OnMouseOver(object sender, EventArgs e)
		{
			var overItem = (sender as InventoryItemView).Item;

			if ((overItem != null) && (_contextMenu.Owner != sender))
				_tooltip.Show(overItem.Item.DisplayName, InputManager.MousePosition.Offset(10, 10), 15, sender);
			else
				_tooltip.Reset(sender);
		}

		private void ArmorItemView_OnMouseOut(object sender, EventArgs e)
		{
			_tooltip.Reset(sender);
		}

		private void UpdateArmorViews()
		{
			_armorItemView[(int)ArmorSlot.Head].Item = _world.Character.EquippedArmorHead;
			_armorItemView[(int)ArmorSlot.Chest].Item = _world.Character.EquippedArmorChest;
			_armorItemView[(int)ArmorSlot.Legs].Item = _world.Character.EquippedArmorLegs;
			_armorItemView[(int)ArmorSlot.Feet].Item = _world.Character.EquippedArmorFeet;
		}

		private void UpdateCharacterStats()
		{
			// This is stupid loop, just for display purposes...we are just arbitrarily stopped after the first 6 "core" stats...because now the enumeration
			// this is based on is growing so I can test other things...obviously we wouldn't try to base this UI on a enum list...
			for (int i = 0; i < 6; i++)
			{
				string statName = ((CharacterAttribute)i).ToString("g");
				int currentVal = (int)typeof(Character).InvokeMember(statName, Util.GetPropertyFlags, Type.DefaultBinder, _world.Character, null);
				_characterStat[i].UpdateText($"{statName}: {currentVal}");
			}
		}

		private void _contextMenu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			var itemView = (InventoryItemView)e.Meta;
			switch (e.Value)
			{
				case "unequip"	:	
					var previous = _world.Character.UnequipArmor((ArmorSlot)itemView.Index);
					_world.Character.AddItem(previous);
					break;
				case "drop"		:
					var item = _world.Character.UnequipArmor((ArmorSlot)itemView.Index);
					_world.AddItem(item, pickup: false);
					break;
			}
			DisableContextMenu();
		}

		private void _contextMenu_OnMouseOut(object sender, ComponentEventArgs e)
		{
			DisableContextMenu();
		}

		private void EnableContextMenu(InventoryItemView clickedItemView)
		{
			_contextMenu.Initialize(clickedItemView, InputManager.MousePosition.Offset(-10, -10), true);
			_components.SetState(_contextMenu, ComponentState.All, null);
			_components.ClearState(_armorItemView, ComponentState.TakingInput);	// Is this enough or do we need to kill Active also?
			_tooltip.Reset();
		}

		private void DisableContextMenu()
		{
			_components.AddState(_armorItemView, ComponentState.TakingInput);
			_components.SetState(_contextMenu, ComponentState.None, null);
			_contextMenu.Clear();
		}
	}
}
