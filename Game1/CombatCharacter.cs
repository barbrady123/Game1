using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game1.Effect;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	public class CombatCharacter : WorldCharacter
	{
		private int _currentHP;
		private int _currentMana;
		private bool _inAction;
		private int _baseDefense;

		public virtual int Defense => _baseDefense + DefenseModifier();
		public bool ActiveItemSolid { get; private set; }
		public virtual bool InAction { get; private set; }
		public InventoryItem ActiveItem { get; private set; }
		public virtual bool ActiveItemHoldable => this.ActiveItem?.Item is ItemHoldable;

		public int Level { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Intelligence { get; set; }
		public int Wisdom { get; set; }
		public int Charisma { get; set; }
		public int Constitution { get; set; }
		public List<CharacterStatus<BuffEffect>> Buffs { get; set; }
		public List<CharacterStatus<DebuffEffect>> Debuffs { get; set; }
		public int Gold { get; set; }

		public int MaxHP { get; set; }
		public int CurrentHP 
		{ 
			get { return _currentHP; }
			set
			{ 
				_currentHP = Util.Clamp(value, 0, this.MaxHP);
				if (_currentHP == 0)
					_onDied?.Invoke(this, null);
			}
		}

		public int MaxMana { get; set; }
		public int CurrentMana
		{ 
			get { return _currentMana; }
			set { _currentMana = Util.Clamp(value, 0, this.MaxMana); }
		}

		public virtual void SetActiveItem(InventoryItem item)
		{ 
			if (this.ActiveItem?.Id != item?.Id)
			{
				if (this.InAction)
					return;

				// We're making a copy here so effects can be applied to the in-game image without affecting the inventory image...
				this.ActiveItem = ItemManager.CopyItem(item);
				_onActiveItemChanged?.Invoke(this, new ComponentEventArgs { Meta = this.ActiveItem });
			}
		}

		public Rectangle ActiveItemBounds
		{
			get
			{
				if (!this.ActiveItemSolid)
					return Rectangle.Empty;

				var holdable = (ItemHoldable)this.ActiveItem.Item;

				// This isn't supporting diagonal...should we switch to independent lookup based on this.Motion??
				switch (this.Direction)
				{
					case Cardinal.East  :	return new Rectangle((int)this.Position.X, (int)this.Position.Y, holdable.Range, 1);
					case Cardinal.West	:	return new Rectangle((int)this.Position.X - holdable.Range, (int)this.Position.Y, holdable.Range, 1);
					case Cardinal.North	:	return new Rectangle((int)this.Position.X, (int)this.Position.Y - holdable.Range, 1, holdable.Range);
					case Cardinal.South	:	return new Rectangle((int)this.Position.X, (int)this.Position.Y, 1, holdable.Range);
				}

				throw new Exception($"Invalid direction {this.Direction}");
			}
		}

		public CombatCharacter(Character character, ImageSpriteSheet spriteSheet, Vector2 position) : base(character, spriteSheet, position)
		{
			this.Buffs = new List<CharacterStatus<BuffEffect>>();
			this.Debuffs = new List<CharacterStatus<DebuffEffect>>();
			this.PreviousPosition = -Vector2.One;
			this.IsHighlighted = false;
			this.InAction = false;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			this.ActiveItemSolid = false;
			this.ActiveItem?.Update(gameTime);
			for (int i = this.Buffs.Count - 1; i >= 0; i--)
				this.Buffs[i].Update(gameTime);

			for (int i = this.Debuffs.Count - 1; i >= 0; i--)
				this.Debuffs[i].Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			DrawBehind(spriteBatch, cameraOffset);
			this.SpriteSheet.Draw(spriteBatch, position: this.Position + cameraOffset, highlight: this.IsHighlighted);
			DrawInfront(spriteBatch, cameraOffset);
		}

		public virtual void UseActiveItem()
		{
			if (!(this.ActiveItem?.Item is ItemHoldable held))
				return;

			if (this.InAction)
				return;

			this.InAction = true;

			// This is messy...redo this...
			if ((this.Direction == Cardinal.North) || (this.Direction == Cardinal.West))
			{
				var effect = this.ActiveItem.Icon.AddEffect<UseItemWestEffect>(true);
				effect.OnFullyExtended += Effect_OnFullyExtended;
				effect.OnActiveChange += Effect_OnActiveChange;
			}
			else
			{
				var effect = this.ActiveItem.Icon.AddEffect<UseItemEastEffect>(true);
				effect.OnFullyExtended += Effect_OnFullyExtended;
				effect.OnActiveChange += Effect_OnActiveChange;
			}
		}

		public void AddBuff(CharacterStatus<BuffEffect> buff)
		{
			buff.OnExpired += Buff_OnExpired;
			buff.OnPeriodicTick += Status_OnPeriodicTick;
			this.Buffs.Add(buff);
		}

		public void AddDebuff(CharacterStatus<DebuffEffect> debuff)
		{
			debuff.OnExpired += Debuff_OnExpired;
			debuff.OnPeriodicTick += Status_OnPeriodicTick;
			this.Debuffs.Add(debuff);
		}

		private void Status_OnPeriodicTick(object sender, CharacterStatusEventArgs e)
		{
			switch (e.AffectedAttribute)
			{
				case CharacterAttribute.CurrentHP : this.CurrentHP += e.EffectValue;	break;
			}
		}

		private void Buff_OnExpired(object sender, CharacterStatusEventArgs e)
		{
			this.Buffs.Remove((CharacterStatus<BuffEffect>)sender);
		}

		private void Debuff_OnExpired(object sender, CharacterStatusEventArgs e)
		{
			this.Debuffs.Remove((CharacterStatus<DebuffEffect>)sender);
		}

		private void DrawBehind(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			if (!this.ActiveItemHoldable)
				return;

			if ((this.Direction == Cardinal.North) || (this.Direction == Cardinal.West))
			{
				this.ActiveItem.Icon.OriginOffset = GamePlayCamera.ActiveItemOriginOffsets[this.Direction];
				this.ActiveItem.Icon.Draw(spriteBatch, position: this.Position + cameraOffset + GamePlayCamera.ActiveItemOffsets[this.Direction], scale: GamePlayCamera.ActiveItemScale);
			}
		}

		private void DrawInfront(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			if (!this.ActiveItemHoldable)
				return;

			if ((this.Direction == Cardinal.South) || (this.Direction == Cardinal.East))
			{
				this.ActiveItem.Icon.OriginOffset = GamePlayCamera.ActiveItemOriginOffsets[this.Direction];
				this.ActiveItem.Icon.Draw(spriteBatch, position: this.Position + cameraOffset + GamePlayCamera.ActiveItemOffsets[this.Direction], scale: GamePlayCamera.ActiveItemScale, spriteEffects: SpriteEffects.FlipHorizontally);
			}
		}

		private int DefenseModifier()
		{
			return this.Buffs.Where(b => b.Effect.AffectedAttribute == CharacterAttribute.Defense).Sum(b => b.Effect.EffectValue * b.Stacks);
		}

		private float MovementSpeedModifier()
		{
			return 1.0f + (float)this.Buffs.Where(b => b.Effect.AffectedAttribute == CharacterAttribute.MovementSpeed).Sum(b => b.Effect.EffectValue * b.Stacks) / 100.0f;
		}

		private void Effect_OnActiveChange(object sender, EventArgs e)
		{
			if (!((EffectEventArgs)e).IsActive)
				this.InAction = false;
		}

		private void Effect_OnFullyExtended(object sender, EventArgs e)
		{
			this.ActiveItemSolid = true;
		}

		#region Events
		// World entities probably shouldn't use ComponentEventArgs...
		private event EventHandler<ComponentEventArgs> _onActiveItemChanged;
		public event EventHandler<ComponentEventArgs> OnActiveItemChanged
		{
			add		{ lock(_lock) { _onActiveItemChanged -= value; _onActiveItemChanged += value; } }
			remove	{ lock(_lock) { _onActiveItemChanged -= value; } }
		}

		private event EventHandler<ComponentEventArgs> _onDied;
		public event EventHandler<ComponentEventArgs> OnDied
		{
			add		{ lock(_lock) { _onDied -= value; _onDied += value; } }
			remove	{ lock(_lock) { _onDied -= value; } }
		}
		#endregion
	}
}
