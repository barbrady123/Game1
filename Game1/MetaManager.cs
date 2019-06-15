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
using Game1.Enum;

namespace Game1
{
	/// <summary>
	/// Manages various metadata for the game in general...
	/// </summary>
	public static class MetaManager
	{
		private static readonly ContentManager _content;
		private static Dictionary<string, Texture2D> _textures;

		private static readonly Dictionary<CharacterInstantEffect, InstantEffect> _instants;
		private static readonly Dictionary<CharacterBuffEffect, BuffEffect> _buffs;

		static MetaManager()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			_textures = new Dictionary<string, Texture2D>();

			_instants = new Dictionary<CharacterInstantEffect, InstantEffect>();
			_buffs = new Dictionary<CharacterBuffEffect, BuffEffect>();
		}

		public static void LoadContent()
		{
			foreach (var file in IOManager.EnumerateDirectory(Path.Combine(Game1.ContentRoot, Game1.StatusIconRoot)))
			{				
				string fileName = Path.GetFileNameWithoutExtension(file);
				_textures[fileName] = _content.Load<Texture2D>(Path.Combine(Game1.StatusIconRoot, fileName));
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

			var currentBuff = character.Buffs.FirstOrDefault(x => x.Buff.Effect == effect);
			if (currentBuff == null)
			{
				character.AddBuff(new CharacterBuff(buff, new ImageTexture(_textures[buff.IconName], true)));
			}
			else
			{
				if (currentBuff.Stacks < currentBuff.Buff.MaxEffectStacks)
					currentBuff.Stacks++;
				if (currentBuff.Duration != null) 
					currentBuff.Duration = Math.Min((double)currentBuff.Duration + buff.DurationStack, (int)currentBuff.Buff.MaxDuration);
			}
		}
	}
}
