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
	public class WorldInteractive : WorldEntity
	{
		private int? _health;

		public Interactive Interactive { get; set; }

		public override bool IsSolid => this.Interactive.IsSolid;

		public override string TooltipText => this.Interactive.DisplayText;

		public event EventHandler OnDestroyed;

		public int? Health
		{ 
			get { return _health; }
			set
			{
				if (value == null)		throw new Exception("Cannot set health to null");
				if (_health == null)	throw new Exception("Interactive cannot be damanged");

				if (_health != value)
				{
					_health = Math.Max(0, (int)value);
					if (_health == 0)
						OnDestroyed?.Invoke(this, null);
				}
			}
		}

		public WorldInteractive(Interactive interactive, ImageTexture icon, Vector2 position)
		{
			this.Interactive = interactive ?? throw new ArgumentNullException(nameof(interactive));
			this.Icon = icon ?? throw new ArgumentNullException(nameof(icon));
			_health = interactive.Health;
			this.Position = position;
			this.Bounds = position.ExpandToRectangleCentered(interactive.Size.Width / 2, interactive.Size.Height / 2);
			this.IsHighlighted = false;
		}

		public void Update(GameTime gameTime)
		{
			this.Icon.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			this.Icon.Draw(spriteBatch, position: this.Position + cameraOffset, highlight: this.IsHighlighted);
		}
	}
}
