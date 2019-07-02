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
using Game1.Menus;

namespace Game1.Interface.Windows
{
	public class CharacterWindow : Component
	{
		private World _world;
		private InventoryItemView[] _armorItemView;
		private ImageText _characterName;
		private ImageText[] _characterStat;

		public CharacterWindow(	Rectangle bounds,
								World world,
								SpriteBatchData spriteBatchData = null) : base(bounds: bounds,
																			   readyDisableOnEscape: true,
																			   background: "brick",
																			   spriteBatchData: spriteBatchData,
																			   drawIfDisabled: false,
																			   fireMouseEvents: false,
																			   enabledTooltip: true,
																			   enabledContextMenu: true)
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
				_armorItemView[i].OnMouseOver += ArmorItemView_OnMouseOver;
				_armorItemView[i].OnMouseRightClick += ArmorItemView_OnMouseRightClick;
			}
			
			_characterStat = new ImageText[System.Enum.GetNames(typeof(CharacterAttribute)).Length];
			for (int i = 0; i < _characterStat.Length; i++)
			{
				var position = this.Bounds.TopLeftVector(this.ContentMargin.Width, this.ContentMargin.Height + 20 + (this.ContentMargin.Height * 2 * i));
				_characterStat[i] = new ImageText(null, true) { Position = position, Alignment = ImageAlignment.LeftCentered };
			}
		}

		public override void LoadContent()
		{
			base.LoadContent();
			foreach (var armorView in _armorItemView)
				armorView.LoadContent();
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
			foreach (var armorView in _armorItemView)
				armorView.UnloadContent();
		}

		public override void UpdateActive(GameTime gameTime)
		{
			base.UpdateActive(gameTime);
			_characterName.Update(gameTime);
			UpdateArmorViews();
			foreach (var armorView in _armorItemView)
				armorView.Update(gameTime);
			UpdateCharacterStats();
			foreach (var stat in _characterStat)
				stat.Update(gameTime);
			InputManager.BlockAllInput();
		}

		protected override void DrawInternal(SpriteBatch spriteBatch)
		{
			base.DrawInternal(spriteBatch);
			_characterName.Draw(spriteBatch);
			foreach (var armorView in _armorItemView)
				armorView.Draw(spriteBatch);
			foreach (var stat in _characterStat)
				stat.Draw(spriteBatch);
		}

		private void ArmorItemView_OnMouseOver(object sender, ComponentEventArgs e)
		{
			e.Meta = sender;
			MouseOver(e);
		}

		private void ArmorItemView_OnMouseRightClick(object sender, ComponentEventArgs e)
		{
			e.Meta = sender;
			MouseRightClick(e);
		}

		protected override void ReadyDisable(ComponentEventArgs e)
		{
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

		protected override void ContextMenuSelect(ComponentEventArgs e)
		{
			// e.Meta type check is needed since a component supporting ContextMenu could have different objects that we need menus for...
			if (!(e.Meta is InventoryItemView itemView))
				return;

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
		}
	}
}
