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
	public class CharacterBuff : CharacterStatus
	{
		public BuffEffect Buff { get; set; }

		public CharacterBuff(BuffEffect buff, ImageTexture icon) : base(buff.Duration, icon)
		{
			this.Buff = buff;
		}
	}
}
