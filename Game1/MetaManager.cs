using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Effect;
using Game1.Enum;
using Game1.Maps;

namespace Game1
{
	/// <summary>
	/// Manages various metadata for the game in general...
	/// TODO: This is already too much scope..we'll need to break these out...
	/// </summary>
	public static class MetaManager
	{
		private static readonly Dictionary<CharacterInstantEffect, InstantEffect> _instants;
		private static readonly Dictionary<CharacterBuffEffect, BuffEffect> _buffs;
		private static readonly Dictionary<CharacterDebuffEffect, DebuffEffect> _debuffs;
		
		private static readonly Dictionary<string, Interactive> _interactives;
		private static readonly Dictionary<string, Transition> _transitions;
		// TODO: Should be a NPC and Mob collection here...

		static MetaManager()
		{
			_instants = new Dictionary<CharacterInstantEffect, InstantEffect>();
			_buffs = new Dictionary<CharacterBuffEffect, BuffEffect>();
			_debuffs = new Dictionary<CharacterDebuffEffect, DebuffEffect>();

			_interactives = new Dictionary<string, Interactive>();
			_transitions = new Dictionary<string, Transition>();

			// TEMP: This should come from file, etc...
			// Add file for NPCs...add to meta....
			// LootTable handler (probably ItemManager responsibility)
			// Add second tilesheet for solid walls, test map with some "dungeon" feel...
			// Still need "breakable" layer....
			// Test if we want things to be interactable only if highlighted, might be better to flag that object and then see if the player did something, than run additional collisions...???
			// Add interactable - chest (after Lootable functions)



			_instants[CharacterInstantEffect.MinorHeal] = new InstantEffect(
				CharacterInstantEffect.MinorHeal,
				CharacterAttribute.CurrentHP,
				"Minor Heal",
				"Increases Current HP by 1-5 points instantly",
				1,
				5
			);

			_buffs[CharacterBuffEffect.MinorDefense] = new BuffEffect(
				CharacterBuffEffect.MinorDefense,
				CharacterAttribute.Defense,
				"minorDefense",
				"Minor Defense",
				"Increases Defense by 10 for 10 seconds",
				10,
				10,
				null,
				2,
				10,
				60
			);

			_buffs[CharacterBuffEffect.MinorMovementSpeed] = new BuffEffect(
				CharacterBuffEffect.MinorMovementSpeed,
				CharacterAttribute.MovementSpeed,
				"minorMovementSpeed",
				"Minor Movement Speed",
				"Increases Movement Speed by 100% for 20 seconds",
				100,
				20,
				null,
				2,
				20,
				120
			);

			_debuffs[CharacterDebuffEffect.MinorDamageOverTime] = new DebuffEffect(
				CharacterDebuffEffect.MinorDamageOverTime,
				CharacterAttribute.CurrentHP,
				"minorBleed",
				"Minor Bleed",
				"Take 1 HP damage per second for 10 seconds",
				-1,
				10,
				1,
				5,
				10,
				10
			);

			_interactives["rock"] = new Interactive {
				Id = "rock",
				DisplayText = "Rock",
				IconName = "rock",
				Health = 100,
				Effectiveness = new Dictionary<ToolType, float> {	// Should be it's own object with a clean constructor
					{ ToolType.Shovel, 0.5f },
					{ ToolType.Axe, 0.5f },
					{ ToolType.Pickaxe, 1.0f },
				},
				LootTable = new LootTable { new Loot {
					Odds = 100,
					ItemPool = new List<int> { 0, 1 },
					MinQuantity = 1,
					MaxQuantity = 2
				}},
				IsSolid = true,
				Size = new Size(32, 32)
			};

			_transitions["stairs_down"] = new Transition("stairs_down", "Stairs Down", "stairs_down");
			_transitions["stairs_up"] = new Transition("stairs_up", "Stairs Up", "stairs_up");
		}

		public static void ApplyCharacterInstantEffect(CharacterInstantEffect effect, Character character)
		{
			if (!_instants.TryGetValue(effect, out InstantEffect instant))
				throw new Exception($"Unexpected CharacterInstantEffect type {effect} encountered!");

			switch (instant.AffectedAttribute)
			{
				case CharacterAttribute.CurrentHP :
					character.CurrentHP += GameRandom.Next(instant.EffectRangeMin, instant.EffectRangeMax);
					break;
			}
		}

		public static void ApplyCharacterBuffEffect(CharacterBuffEffect effect, Character character)
		{
			if (!_buffs.TryGetValue(effect, out BuffEffect buff))
				throw new Exception($"Unexpected CharacterBuffEffect type {effect} encountered!");

			var currentBuff = character.Buffs.FirstOrDefault(x => x.Effect.Effect == effect);
			if (currentBuff == null)
			{
				character.AddBuff(new CharacterStatus<BuffEffect>(buff, AssetManager.GetIconStatus(buff.IconName)));
			}
			else
			{
				if (currentBuff.Stacks < currentBuff.Effect.MaxEffectStacks)
					currentBuff.Stacks++;
				if (currentBuff.Duration != null) 
					currentBuff.Duration = Math.Min((double)currentBuff.Duration + buff.DurationStack, (int)currentBuff.Effect.MaxDuration);
			}
		}

		public static void ApplyCharacterDebuffEffect(CharacterDebuffEffect effect, Character character)
		{
			if (!_debuffs.TryGetValue(effect, out DebuffEffect debuff))
				throw new Exception($"Unexpected CharacterDebuffEffect type {effect} encountered!");

			var currentDebuff = character.Debuffs.FirstOrDefault(x => x.Effect.Effect == effect);
			if (currentDebuff == null)
			{
				character.AddDebuff(new CharacterStatus<DebuffEffect>(debuff, AssetManager.GetIconStatus(debuff.IconName)));
			}
			else
			{
				if (currentDebuff.Stacks < currentDebuff.Effect.MaxEffectStacks)
					currentDebuff.Stacks++;
				if (currentDebuff.Duration != null) 
					currentDebuff.Duration = Math.Min((double)currentDebuff.Duration + debuff.DurationStack, (int)currentDebuff.Effect.MaxDuration);
			}
		}

		// Another temp test method...
		public static WorldInteractive GetInteractve(string id, Vector2 position)
		{
			if (!_interactives.TryGetValue(id, out Interactive interactive))
				throw new ArgumentException($"No interactive found with id '{id}'");

			return new WorldInteractive(interactive, AssetManager.GetIconInteractive(interactive.IconName), position);
		}

		public static WorldTransition GetTransition(MapTransition transition)
		{
			if (!_transitions.ContainsKey(transition.Id))
				throw new ArgumentException($"No transition found for id '{transition.Id}'");

			return new WorldTransition(
				_transitions[transition.Id],
				transition.Position.ToVector2(),
				AssetManager.GetIconTransition(transition.Id),
				transition.DestinationMap,
				transition.DestinationPosition
			);
		}
	}
}
