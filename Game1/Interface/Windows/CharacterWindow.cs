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
		private World _world;
		private InventoryItemView[] _armorItemView;
		private ImageText _characterName;
		private ImageText[] _characterStat;
		private InventoryContextMenu _contextMenu;

		public CharacterWindow(Rectangle bounds, World world, SpriteBatchData spriteBatchData = null) : base(bounds, true, "brick", spriteBatchData, killFurtherInput: true, drawIfDisabled: false, fireMouseEvents: false, enabledTooltip: true)
		{
			_world = world;
			_armorItemView = new InventoryItemView[System.Enum.GetNames(typeof(ArmorSlot)).Length];

			_characterName = new ImageText(_world.Character.Name, true) {
				Alignment = ImageAlignment.Centered,
				Position = this.Bounds.TopCenterVector(yOffset: this.ContentMargin.Height + (FontManager.FontHeight / 2))
			};

			for (int i = 0; i < _armorItemView.Length; i++)
			{
				var position = this.Bounds.TopRightVector(-100, this.ContentMargin.Height + InventoryItemView.Size / 2 + (100 * i)).ExpandToRectangleCentered(InventoryItemView.Size / 2, InventoryItemView.Size / 2);
				_activator.Register(_armorItemView[i] = new InventoryItemView(position, i, ((ArmorSlot)i).ToString("g"), true), true, $"armor{i}");
				_armorItemView[i].OnMouseRightClick += ArmorItemView_OnMouseRightClick;
				_armorItemView[i].OnMouseOver += ArmorItemView_OnMouseOver;
			}
			
			_characterStat = new ImageText[System.Enum.GetNames(typeof(CharacterAttribute)).Length];
			for (int i = 0; i < _characterStat.Length; i++)
			{
				var position = this.Bounds.TopLeftVector(this.ContentMargin.Width, this.ContentMargin.Height + 20 + (this.ContentMargin.Height * 2 * i));
				_characterStat[i] = new ImageText(null, true) { Position = position, Alignment = ImageAlignment.LeftCentered };
			}

			_activator.Register(_contextMenu = new InventoryContextMenu(SpriteBatchManager.Get("context")), false, new[] { "popup", "armor0", "armor1", "armor2", "armor3" });
			_contextMenu.OnMouseOut += _contextMenu_OnMouseOut;
			_contextMenu.OnItemSelect += _contextMenu_OnItemSelect;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			_characterName.LoadContent();
			foreach (var armorView in _armorItemView)
				armorView.LoadContent();
			foreach (var stat in _characterStat)
				stat.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			_characterName.UnloadContent();
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
			UpdateArmorViews();
			foreach (var armorView in _armorItemView)
				armorView.Update(gameTime);
			UpdateCharacterStats();
			foreach (var stat in _characterStat)
				stat.Update(gameTime);
			base.UpdateActive(gameTime);
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_characterName.Draw(spriteBatch);
			foreach (var armorView in _armorItemView)
				armorView.Draw(spriteBatch);
			foreach (var stat in _characterStat)
				stat.Draw(spriteBatch);
			_contextMenu.Draw(spriteBatch);
		}

		private void ArmorItemView_OnMouseRightClick(object sender, ComponentEventArgs e)
		{
			var itemView = (InventoryItemView)sender;
			if (itemView?.Item != null)
				EnableContextMenu(itemView);
		}

		private void ArmorItemView_OnMouseOver(object sender, ComponentEventArgs e)
		{
			e.Meta = sender;
			MouseOver(e);
		}

		protected override void ReadyDisable(ComponentEventArgs e)
		{
			DisableContextMenu();
			base.ReadyDisable(e);
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
			// Make this a more consistent method (once we have DynamicComponent)...
			_contextMenu.Initialize(clickedItemView, InputManager.MousePosition.Offset(-10, -10), true);
			_activator.SetState(_contextMenu, true);
			// Do we still need these types of calls for the dynamic stuff? Shouldn't making them inactive be enough?
			//_tooltip.Reset();
		}

		private void DisableContextMenu()
		{
			_activator.SetState(_armorItemView, true);
			// Do we still need these types of calls for the dynamic stuff? Shouldn't making them inactive be enough?
			//_contextMenu.Clear();
		}
	}
}
