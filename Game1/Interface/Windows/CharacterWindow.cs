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
		private Character _character;
		private InventoryItemView[] _armorItemView;
		private ImageText _characterName;
		private ImageText[] _characterStat;
		private InventoryContextMenu _contextMenu;

		private Tooltip _tooltip;

		public CharacterWindow(Rectangle bounds, Character character) : base(bounds, true, "brick")
		{
			_character = character;
			_armorItemView = new InventoryItemView[System.Enum.GetNames(typeof(ArmorSlot)).Length];

			_characterName = new ImageText(_character.Name, true) {
				Alignment = ImageAlignment.Centered,
				Position = this.Bounds.TopCenterVector(yOffset: this.ContentMargin.Height + (FontManager.FontHeight / 2))
			};

			for (int i = 0; i < _armorItemView.Length; i++)
			{
				var position = this.Bounds.TopRightVector(-100, this.ContentMargin.Height + InventoryItemView.Size / 2 + (100 * i)).ExpandToRectangleCentered(InventoryItemView.Size / 2, InventoryItemView.Size / 2);
				_armorItemView[i] = new InventoryItemView(position, i, ((ArmorSlot)i).ToString("g"));
				_armorItemView[i].OnMouseClick += ArmorItemView_OnMouseClick;
				_armorItemView[i].OnMouseOver += ArmorItemView_OnMouseOver;
				_armorItemView[i].OnMouseOut += ArmorItemView_OnMouseOut;
			}
			
			_characterStat = new ImageText[System.Enum.GetNames(typeof(CharacterAttribute)).Length];
			for (int i = 0; i < _characterStat.Length; i++)
			{
				var position = this.Bounds.TopLeftVector(this.ContentMargin.Width, this.ContentMargin.Height + 20 + (this.ContentMargin.Height * 2 * i));
				_characterStat[i] = new ImageText(null, true) { Position = position, Alignment = ImageAlignment.LeftCentered };
			}

			_components = new ComponentManager();
			_components.Register(_tooltip = new Tooltip());
			_components.SetState(_tooltip, ComponentState.Active, null);

			_contextMenu = null;
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
			foreach (var armorView in _armorItemView)
				armorView.UnloadContent();
			foreach (var stat in _characterStat)
				stat.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_characterName.Update(gameTime);
			_tooltip.Update(gameTime);
			UpdateArmorViews();
			foreach (var armorView in _armorItemView)
				armorView.Update(gameTime);
			UpdateCharacterStats();
			foreach (var stat in _characterStat)
				stat.Update(gameTime);
			// Need to fix this...probably bad now 
			_contextMenu?.Update(gameTime);
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

			if (_contextMenu != null)
			{
				var contextBatch = SpriteBatchManager.Get("context");
				contextBatch.ScissorWindow = _contextMenu.Bounds;
				_contextMenu?.Draw(contextBatch.SpriteBatch);
			}

			var tooltipBatch = SpriteBatchManager.Get("tooltip");
			_tooltip.Draw(tooltipBatch.SpriteBatch);
		}

		private void ArmorItemView_OnMouseClick(object sender, ComponentEventArgs e)
		{
			var itemView = (InventoryItemView)sender;

			if ((e.Button == MouseButton.Right) && (itemView?.Item != null))
			{
				// This should use the manager not set the state here...
				_contextMenu = new InventoryContextMenu(itemView, InputManager.MousePosition.Offset(-10, -10), itemView.Item, true) { State = ComponentState.All };
				_contextMenu.LoadContent();
				_contextMenu.OnMouseOut += _contextMenu_OnMouseOut;
				_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;
			}
		}

		private void ArmorItemView_OnMouseOver(object sender, EventArgs e)
		{
			var args = (MouseEventArgs)e;
			var overItem = (sender as InventoryItemView).Item;

			if ((overItem != null) && (_contextMenu?.Owner != sender))
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
			_armorItemView[(int)ArmorSlot.Head].Item = _character.EquippedArmorHead;
			_armorItemView[(int)ArmorSlot.Chest].Item = _character.EquippedArmorChest;
			_armorItemView[(int)ArmorSlot.Legs].Item = _character.EquippedArmorLegs;
			_armorItemView[(int)ArmorSlot.Feet].Item = _character.EquippedArmorFeet;
		}

		private void UpdateCharacterStats()
		{
			for (int i = 0; i < _characterStat.Length; i++)
			{
				string statName = ((CharacterAttribute)i).ToString("g");
				int currentVal = (int)typeof(Character).InvokeMember(statName, Util.GetPropertyFlags, Type.DefaultBinder, _character, null);
				_characterStat[i].UpdateText($"{statName}: {currentVal}");
			}
		}

		private void _contextMenu_OnItemSelect(object sender, ComponentEventArgs e)
		{
			var itemView = (InventoryItemView)e.Sender;

			switch (e.Item)
			{
				case "unequip"	:	
					_character.UnequipArmor((ArmorSlot)itemView.Index);
					_character.PutItem(_character.Backpack);
					break;
				case "split"	:
					// TODO: Need to implement this...(also need to display stack size on Held item)...
					// Need a "split" popup screen....
					break;
			}
			_contextMenu = null;
		}

		private void _contextMenu_OnMouseOut(object sender, ComponentEventArgs e)
		{
			_contextMenu = null;
		}
	}
}
