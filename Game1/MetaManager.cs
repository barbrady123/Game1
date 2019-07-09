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
		public const string MetaEffectRoot = Game1.MetaRoot + "\\Effect";

		private static readonly Dictionary<CharacterInstantEffect, InstantEffect> _instants;
		private static readonly Dictionary<CharacterBuffEffect, BuffEffect> _buffs;
		private static readonly Dictionary<CharacterDebuffEffect, DebuffEffect> _debuffs;
		
		private static readonly Dictionary<string, Interactive> _interactives;
		private static readonly Dictionary<string, Transition> _transitions;

		private static readonly Dictionary<string, Character> _characters;

		// TODO: Should be a Mob collection here...

		static MetaManager()
		{
			_instants = IOManager.ObjectFromFile<List<InstantEffect>>(Path.Combine(MetaEffectRoot, "effect_instant")).ToDictionary(e => e.Effect);
			_buffs = IOManager.ObjectFromFile<List<BuffEffect>>(Path.Combine(MetaEffectRoot, "effect_buff")).ToDictionary(e => e.Effect);
			_debuffs = IOManager.ObjectFromFile<List<DebuffEffect>>(Path.Combine(MetaEffectRoot, "effect_debuff")).ToDictionary(e => e.Effect);
			_transitions = IOManager.ObjectFromFile<List<Transition>>(Path.Combine(Game1.MetaRoot, "transition")).ToDictionary(e => e.Id);
			_characters = IOManager.ObjectFromFile<List<Character>>(Path.Combine(Game1.MetaRoot, "npc")).ToDictionary(e => e.Id);

			_interactives = new Dictionary<string, Interactive>();

			// LootTable handler (probably ItemManager responsibility)
			// Still need "breakable" layer....
			// Add interactable - chest (after Lootable functions)

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
		}

		public static void ApplyCharacterInstantEffect(CharacterInstantEffect effect, CombatCharacter character)
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

		public static CharacterStatus<BuffEffect> ApplyCharacterBuffEffect(CharacterBuffEffect effect, CombatCharacter character)
		{
			if (!_buffs.TryGetValue(effect, out BuffEffect buff))
				throw new Exception($"Unexpected CharacterBuffEffect type {effect} encountered!");

			var currentBuff = character.Buffs.FirstOrDefault(x => x.Effect.Effect == effect);
			if (currentBuff == null)
			{
				currentBuff = new CharacterStatus<BuffEffect>(buff, AssetManager.GetIconStatus(buff.IconName));
				character.AddBuff(currentBuff);
			}
			else
			{
				if (currentBuff.Stacks < currentBuff.Effect.MaxEffectStacks)
					currentBuff.Stacks++;
				if (currentBuff.Duration != null) 
					currentBuff.Duration = Math.Min((double)currentBuff.Duration + buff.DurationStack, (int)currentBuff.Effect.MaxDuration);
			}

			return currentBuff;
		}

		public static CharacterStatus<DebuffEffect> ApplyCharacterDebuffEffect(CharacterDebuffEffect effect, CombatCharacter character)
		{
			if (!_debuffs.TryGetValue(effect, out DebuffEffect debuff))
				throw new Exception($"Unexpected CharacterDebuffEffect type {effect} encountered!");

			var currentDebuff = character.Debuffs.FirstOrDefault(x => x.Effect.Effect == effect);
			if (currentDebuff == null)
			{
				currentDebuff = new CharacterStatus<DebuffEffect>(debuff, AssetManager.GetIconStatus(debuff.IconName));
				character.AddDebuff(currentDebuff);
			}
			else
			{
				if (currentDebuff.Stacks < currentDebuff.Effect.MaxEffectStacks)
					currentDebuff.Stacks++;
				if (currentDebuff.Duration != null) 
					currentDebuff.Duration = Math.Min((double)currentDebuff.Duration + debuff.DurationStack, (int)currentDebuff.Effect.MaxDuration);
			}

			return currentDebuff;
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

		public static WorldCharacter GetCharacter(string id, Vector2 position)
		{
			if (!_characters.TryGetValue(id, out Character character))
				throw new ArgumentException($"No character found with id '{id}'");

			return new WorldCharacter(character, AssetManager.GetSpriteSheet(character.SpriteSheetName), position);
		}

		// This shold eventually be loaded in as well...
		public static readonly Size HumanoidBoundsSize = new Size(28, 54);
		public static Size GetCreatureSize(CreatureType type)
		{
			switch (type)
			{
				case CreatureType.Humanoid: return HumanoidBoundsSize;
			}

			throw new ArgumentException($"Unknown CreatureType {type}");
		}
	}
}
