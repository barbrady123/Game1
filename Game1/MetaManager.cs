using System;
using System.Collections.Generic;
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
		private static readonly Dictionary<CharacterInstantEffect, InstantEffect> _instants;
		private static readonly Dictionary<CharacterBuffEffect, BuffEffect> _buffs;

		static MetaManager()
		{
			_content = new ContentManager(Game1.ServiceProvider, Game1.ContentRoot);
			_instants = new Dictionary<CharacterInstantEffect, InstantEffect>();
			_buffs = new Dictionary<CharacterBuffEffect, BuffEffect>();
		}

		public static void LoadContent()
		{
			// Eventually we'll load icon content here

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
				null,
				"Minor Defense",
				"Increases Defense by 10 for 5 minutes",
				10,
				5,
				null,
				2,
				60,
				600
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
				// Should we break up actual buffs from instant effects???
				case CharacterAttribute.CurrentHP :
					character.CurrentHP += GameRandom.Next(instant.EffectRangeMin, instant.EffectRangeMax);
					break;
			}
		}

		public static void ApplyCharacterBuffEffect(CharacterBuffEffect effect, Character character)
		{
			if (!_buffs.TryGetValue(effect, out BuffEffect buff))
				throw new Exception($"Unexpected CharacterBuffEffect type {effect} encountered!");

			switch (buff.AffectedAttribute)
			{
				case CharacterAttribute.Defense :
				// and others that apply a buff...
					var currentBuff = character.Buffs.FirstOrDefault(x => x.Buff.Effect == effect);
					if (currentBuff == null)
					{
						character.AddBuff(new CharacterBuff(buff));
					}
					else
					{
						if (currentBuff.Stacks < currentBuff.Buff.MaxEffectStacks)
							currentBuff.Stacks++;
						if (currentBuff.Duration != null) 
							currentBuff.Duration = Math.Min((double)currentBuff.Duration + buff.DurationStack, (int)currentBuff.Buff.MaxDuration);
					}
					break;
			}
		}
	}
}
