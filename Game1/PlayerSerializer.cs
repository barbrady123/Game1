using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Game1.Enum;
using Game1.Effect;
using Game1.Items;

namespace Game1
{
	public class InventoryItemMeta
	{
		public Guid InventoryId { get; set; }
		public string ItemId { get; set; }
		public int Quantity { get; set; }
	}

	public class StatusMeta
	{
		public double? Duration { get; set; }
		public int Stacks { get; set; }
		public double CurrentPeriod { get; set; }
	}

	public class BuffStatusMeta : StatusMeta
	{
		public CharacterBuffEffect Effect { get; set; }
	}

	public class DebuffStatusMeta : StatusMeta
	{
		public CharacterDebuffEffect Effect { get; set; }
	}

	public class PlayerSerializer
	{
		public string Location { get; set; }
		public Vector2 Position { get; set; }
		public Cardinal Direction { get; set; }
		public string Id { get; set; }
		public CreatureType CreatureType { get; set; }
		public string Name { get; set; }
		public string SpriteSheetName { get; set; }
		public CharacterSex Sex { get; set; }
		public int Gold { get; set; }
		public int MaxHP { get; set; }
		public int CurrentHP { get; set; }
		public int MaxMana { get; set; }
		public int CurrentMana { get; set; }
		public int Experience { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Constitution { get; set; }
		public int Intelligence { get; set; }
		public int Wisdom { get; set; }
		public int Charisma { get; set; }

		public List<BuffStatusMeta> Buffs { get; set; }
		public List<DebuffStatusMeta> Debuffs { get; set; }

		public InventoryItemMeta EquippedArmorHead { get; set; }
		public InventoryItemMeta EquippedArmorChest { get; set; }
		public InventoryItemMeta EquippedArmorLegs { get; set; }
		public InventoryItemMeta EquippedArmorFeet { get; set; }

		public List<InventoryItemMeta> HotBar { get; set; }
		public List<InventoryItemMeta> Backpack { get; set; }

		public PlayerSerializer(Player player)
		{
			this.Location = player.Location;
			this.Position = player.Position;
			this.Direction = player.Direction;
			this.Id = player.Character.Id;
			this.CreatureType = player.Character.CreatureType;
			this.Name = player.Character.Name;
			this.SpriteSheetName = player.Character.SpriteSheetName;
			this.Sex = player.Character.Sex;
			this.Gold = player.Gold;
			this.MaxHP = player.MaxHP;
			this.CurrentHP = player.CurrentHP;
			this.MaxMana = player.MaxMana;
			this.CurrentMana = player.CurrentMana;
			this.Experience = player.Experience;
			this.Strength = player.Strength;
			this.Dexterity = player.Dexterity;
			this.Constitution = player.Constitution;
			this.Intelligence = player.Intelligence;
			this.Wisdom = player.Wisdom;
			this.Charisma = player.Charisma;
			this.Buffs = player.Buffs.Select(x => new BuffStatusMeta { Effect = x.Effect.Effect, Duration = x.Duration, Stacks = x.Stacks, CurrentPeriod = x.CurrentPeriod }).ToList();
			this.Debuffs = player.Debuffs.Select(x => new DebuffStatusMeta { Effect = x.Effect.Effect, Duration = x.Duration, Stacks = x.Stacks, CurrentPeriod = x.CurrentPeriod }).ToList();
			this.EquippedArmorHead = (player.EquippedArmorHead != null) ? new InventoryItemMeta { InventoryId = player.EquippedArmorHead.Id, ItemId = player.EquippedArmorHead.Item.Id, Quantity = player.EquippedArmorHead.Quantity } : null;
			this.EquippedArmorChest = (player.EquippedArmorChest != null) ? new InventoryItemMeta { InventoryId = player.EquippedArmorChest.Id, ItemId = player.EquippedArmorChest.Item.Id, Quantity = player.EquippedArmorChest.Quantity } : null;
			this.EquippedArmorLegs = (player.EquippedArmorLegs != null) ? new InventoryItemMeta { InventoryId = player.EquippedArmorLegs.Id, ItemId = player.EquippedArmorLegs.Item.Id, Quantity = player.EquippedArmorLegs.Quantity } : null;
			this.EquippedArmorFeet = (player.EquippedArmorFeet != null) ? new InventoryItemMeta { InventoryId = player.EquippedArmorFeet.Id, ItemId = player.EquippedArmorFeet.Item.Id, Quantity = player.EquippedArmorFeet.Quantity } : null;
			this.HotBar = new List<InventoryItemMeta>();
			foreach (var slot in player.HotBar)
				this.HotBar.Add(slot != null ? new InventoryItemMeta { InventoryId = slot.Id, ItemId = slot.Item.Id, Quantity = slot.Quantity } : null);
			this.Backpack = new List<InventoryItemMeta>();
			foreach (var slot in player.Backpack)
				this.Backpack.Add(slot != null ? new InventoryItemMeta { InventoryId = slot.Id, ItemId = slot.Item.Id, Quantity = slot.Quantity } : null);
		}

		public Player ToPlayer()
		{
			var player = new Player(
				new Character {
					Id = this.Id,
					CreatureType = this.CreatureType,
					Name = this.Name,
					Sex = this.Sex,
					SpriteSheetName = this.SpriteSheetName,
					MovementSpeed = Game1.DefaultPlayerMovementSpeed
				},
				AssetManager.GetSpriteSheet(this.SpriteSheetName),
				this.Position
			)
			{
				Location = this.Location,
				Direction = this.Direction,
				Gold = this.Gold,
				MaxHP = this.MaxHP,
				CurrentHP = this.CurrentHP,
				MaxMana = this.MaxMana,
				CurrentMana = this.CurrentMana,
				Experience = this.Experience,
				Strength = this.Strength,
				Dexterity = this.Dexterity,
				Constitution = this.Constitution,
				Intelligence = this.Intelligence,
				Wisdom = this.Wisdom,
				Charisma = this.Charisma,

			};

			foreach (var buff in this.Buffs)
			{
				var buffEffect = MetaManager.ApplyCharacterBuffEffect(buff.Effect, player);
				buffEffect.Duration = buff.Duration;
				buffEffect.Stacks = buff.Stacks;
				buffEffect.CurrentPeriod = buff.CurrentPeriod;
			}

			foreach (var debuff in this.Debuffs)
			{
				var debuffEffect = MetaManager.ApplyCharacterDebuffEffect(debuff.Effect, player);
				debuffEffect.Duration = debuff.Duration;
				debuffEffect.Stacks = debuff.Stacks;
				debuffEffect.CurrentPeriod = debuff.CurrentPeriod;
			}

			int index = 0;
			foreach (var item in this.HotBar)
			{
				if (item != null)
				{
					var inventoryItem = ItemManager.GetItem(item.ItemId, item.Quantity);
					inventoryItem.Id = item.InventoryId;
					player.HotBar.AddItem(inventoryItem, index);
				}
				index++;
			}

			index = 0;
			foreach (var item in this.Backpack)
			{
				if (item != null)
				{
					var inventoryItem = ItemManager.GetItem(item.ItemId, item.Quantity);
					inventoryItem.Id = item.InventoryId;
					player.Backpack.AddItem(inventoryItem, index);
				}
				index++;
			}

			return player;
		}
	}
}
