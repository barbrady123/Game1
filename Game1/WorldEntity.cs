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
using Game1.Items;

namespace Game1
{
	public abstract class WorldEntity : IWorldEntity, ISupportsTooltip
	{
		protected readonly object _lock = new object();
		
		public virtual Vector2 Position { get; set; }

		public virtual Rectangle Bounds { get; set; }
 
		public virtual ImageTexture Icon { get; set; }

		public abstract bool IsSolid { get; }

		public virtual bool IsHighlighted { get; set; }

		public virtual string TooltipText => null;

		public abstract void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset);

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
