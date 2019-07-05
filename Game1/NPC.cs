﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Game1.Enum;

namespace Game1
{
	public class NPC : Character, ISupportsTooltip
	{
		private const int SameDirectionBias = 99;
		private const int ContinueStopBias = 96;

		public NPC(string name, CharacterSex sex, Vector2 position, int maxHP, int currentHP) : base()
		{
			_movementSpeed = 75.0f;
			this.Name = name;
			this.Sex = sex;
			this.Position = position;
			this.MaxHP = maxHP;
			this.CurrentHP = currentHP;
		}

		public string TooltipText => this.Name;

		public override Vector2 UpdateMotion()
		{
			if (this.Motion == Vector2.Zero)
			{
				if (GameRandom.Percent(NPC.ContinueStopBias))
					return this.Motion;
			}
			else
			{
				if (GameRandom.Percent(NPC.SameDirectionBias))
					return this.Motion;
			}

			return new Vector2(GameRandom.Next(-1, 1), GameRandom.Next(-1, 1));
		}

		#region Events
		public void MouseOut() { _onMouseOut?.Invoke(this, null); }
		private event EventHandler<ComponentEventArgs> _onMouseOut;
		public event EventHandler<ComponentEventArgs> OnMouseOut
		{
			add		{ lock(_lock) { _onMouseOut -= value; _onMouseOut += value; } }
			remove	{ lock(_lock) { _onMouseOut -= value; } }
		}

		public void MouseOver() { _onMouseOver?.Invoke(this, null); }
		private event EventHandler<ComponentEventArgs> _onMouseOver;
		public event EventHandler<ComponentEventArgs> OnMouseOver
		{
			add		{ lock(_lock) { _onMouseOver -= value; _onMouseOver += value; } }
			remove	{ lock(_lock) { _onMouseOver -= value; } }
		}
		#endregion
	}
}
