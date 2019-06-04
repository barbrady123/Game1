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
	public class CharacterWindow : Window
	{
		public override Size ContentMargin => new Size(50, this.Title == null ? 20 : 60);
		private Character _character;
		private InventoryItemView[] _armorItemView;
		private ImageText[] _characterStat;
		private InventoryContextMenu _contextMenu;

		private Tooltip _tooltip;

		public CharacterWindow(Rectangle bounds, Character character) : base(bounds, "brick", character.Name, null, true, true)
		{
			var viewPosition = bounds.TopLeftPoint(this.ContentMargin.Width, this.ContentMargin.Height);
			_character = character;
			_armorItemView = new InventoryItemView[System.Enum.GetNames(typeof(ArmorSlot)).Length];
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
				var position = this.Bounds.TopLeftVector(this.ContentMargin.Width, this.ContentMargin.Height + 20 + (this.ContentMargin.Height / 2 * i));
				_characterStat[i] = new ImageText(null, true) { Position = position, Alignment = ImageAlignment.LeftCentered };
			}

			_tooltip = new Tooltip();
			_contextMenu = null;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_tooltip.LoadContent();
			foreach (var armorView in _armorItemView)
				armorView.LoadContent();
			foreach (var stat in _characterStat)
				stat.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_tooltip.UnloadContent();
			foreach (var armorView in _armorItemView)
				armorView.UnloadContent();
			foreach (var stat in _characterStat)
				stat.UnloadContent();
		}

		public override void UpdateReady(GameTime gameTime, bool processInput)
		{
			base.UpdateReady(gameTime, processInput);
			_tooltip.Update(gameTime, processInput);
			UpdateArmorViews();
			foreach (var armorView in _armorItemView)
				armorView.Update(gameTime);
			UpdateCharacterStats();
			foreach (var stat in _characterStat)
				stat.Update(gameTime);
			_contextMenu?.Update(gameTime, processInput);
		}

		public override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_tooltip.Draw(spriteBatch);
			foreach (var armorView in _armorItemView)
				armorView.Draw(spriteBatch);
			foreach (var stat in _characterStat)
				stat.Draw(spriteBatch);
			if (_contextMenu != null)
			{
				var batchData = SpriteBatchManager.Get("context");
				batchData.ScissorWindow = _contextMenu.Bounds;
				_contextMenu?.Draw(batchData.SpriteBatch);
			}
		}

		protected override void BeforeReadyDisable(ScreenEventArgs args)
		{
			base.BeforeReadyDisable(args);
		}

		private void ArmorItemView_OnMouseClick(object sender, EventArgs e)
		{
			var args = (MouseEventArgs)e;
			if ((args.Button == MouseButton.Right) && (_armorItemView[args.SourceIndex]?.Item != null))
			{
				_contextMenu = new InventoryContextMenu(sender, "armorview", args.SourceIndex,  InputManager.MousePosition.Offset(-10, -10), _armorItemView[args.SourceIndex].Item, true) { IsActive = true };
				_contextMenu.LoadContent();
				_contextMenu.OnMouseOut += _contextMenu_OnMouseOut;
				_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;
			}
		}

		private void ArmorItemView_OnMouseOver(object sender, EventArgs e)
		{
			var args = (MouseEventArgs)e;
			var overItem = (sender as InventoryItemView).Item;
			int overIndex = args.SourceIndex;

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

		private void _contextMenu_OnItemSelect(object sender, EventArgs e)
		{
			var args = (MenuEventArgs)e;
			switch (args.Item)
			{
				case "unequip"	:	
					_character.UnequipArmor((ArmorSlot)args.SourceIndex);
					_character.PutItem(_character.Backpack);
					break;
				case "split"	:	break;
				case "cancel"	:	break;
			}
			_contextMenu = null;
		}

		private void _contextMenu_OnMouseOut(object sender, EventArgs e)
		{
			_contextMenu = null;
		}
	}
}
