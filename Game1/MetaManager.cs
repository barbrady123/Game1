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

namespace Game1
{
	/// <summary>
	/// Manages various metadata for the game in general...
	/// </summary>
	public static class MetaManager
	{
		private static readonly ContentManager _content;
		private static Dictionary<string, Texture2D> _statusTextures;
		private static Dictionary<string, Texture2D> _interactiveTextures;

		private static readonly Dictionary<CharacterInstantEffect, InstantEffect> _instants;
		private static readonly Dictionary<CharacterBuffEffect, BuffEffect> _buffs;
		private static readonly Dictionary<CharacterDebuffEffect, DebuffEffect> _debuffs;
		
		private static readonly List<Interactive> _interactives;

		private static readonly Dictionary<string, Texture2D> _spriteSheets;

		static MetaManager()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			_statusTextures = new Dictionary<string, Texture2D>();
			_interactiveTextures = new Dictionary<string, Texture2D>();

			_instants = new Dictionary<CharacterInstantEffect, InstantEffect>();
			_buffs = new Dictionary<CharacterBuffEffect, BuffEffect>();
			_debuffs = new Dictionary<CharacterDebuffEffect, DebuffEffect>();

			_interactives = new List<Interactive>();

			_spriteSheets = new Dictionary<string, Texture2D>();
		}

		public static void LoadContent()
		{
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(Game1.ContentRoot, Game1.StatusIconRoot)))
			{				
				string fileName = Path.GetFileNameWithoutExtension(file);
				_statusTextures[fileName] = _content.Load<Texture2D>(Path.Combine(Game1.StatusIconRoot, fileName));
			}

			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(Game1.ContentRoot, Game1.InteractiveIconRoot)))
			{
				string fileName = Path.GetFileNameWithoutExtension(file);
				_interactiveTextures[fileName] = _content.Load<Texture2D>(Path.Combine(Game1.InteractiveIconRoot, fileName));
			}

			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(Game1.ContentRoot, Game1.SpriteSheetRoot)))
			{
				string fileName = Path.GetFileNameWithoutExtension(file);
				_spriteSheets[fileName] = _content.Load<Texture2D>(Path.Combine(Game1.SpriteSheetRoot, fileName));
			}

			// TEMP: This should come from file, etc...
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

			_interactives.Add(new Interactive {
				DisplayText = "Rock",
				IconName = "rock",
				Health = 100,
				Effectiveness = new Dictionary<ToolType, float> {	// Should be it's own object with a clean constructor
					{ ToolType.Shovel, 0.5f },
					{ ToolType.Axe, 0.5f },
					{ ToolType.Pickaxe, 1.0f },
				},
				LootTable = new List<Loot> { new Loot {
					Odds = 100,
					ItemPool = new List<int> { 0, 1 },
					MinQuantity = 1,
					MaxQuantity = 2
				}},
				IsSolid = true,
				Size = new Size(32, 32)
			});
		}

		public static void UnloadContent()
		{
			_content.Unload();
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
				character.AddBuff(new CharacterStatus<BuffEffect>(buff, new ImageTexture(_statusTextures[buff.IconName], true)));
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
				character.AddDebuff(new CharacterStatus<DebuffEffect>(debuff, new ImageTexture(_statusTextures[debuff.IconName], true)));
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
		public static WorldInteractive GetInteractve(Vector2 position)
		{
			var i = _interactives.First();
			return new WorldInteractive(i, new ImageTexture(_interactiveTextures[i.IconName], true) { Alignment = ImageAlignment.Centered }, position);
		}

		public static ImageSpriteSheet GetSpriteSheet(string name)
		{
			var img = new ImageSpriteSheet(_spriteSheets[name], true);
			img.AddEffect<SpriteSheetEffect>(false);
			return img;
		}
	}
}
