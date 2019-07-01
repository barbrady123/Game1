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
	public class WorldItem : IWorldEntity
	{
		private bool _inRange;

		public bool InRange
		{ 
			get { return _inRange; }
			set
			{				
				if (_inRange && !value)
					this.Pickup = true;

				_inRange = value;
			}
		}

		public bool Pickup { get; set; }

		public InventoryItem Item { get; set; }

		public Vector2 Position { get; set; }

		public Rectangle Bounds { get; set; }

		public bool IsSolid => false;

		public bool IsHighlighted { get; set; }

		public WorldItem(InventoryItem item, Vector2 position, bool pickup)
		{
			_inRange = false;
			this.Item = item;
			this.Item.Icon.Scale = GamePlayCamera.MapItemScale;
			this.Position = position;
			this.Bounds = position.ExpandToRectangleCentered(Game1.DefaultPickupRadius / 2, Game1.DefaultPickupRadius/ 2);
			this.Pickup = pickup;
			this.IsHighlighted = false;
		}

		public void Update(GameTime gameTime)
		{
			this.Item?.Update(gameTime);
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 cameraOffset)
		{
			this.Item.Icon.Highlight = this.IsHighlighted;
			this.Item.Icon.Draw(spriteBatch, position: this.Position + cameraOffset);
		}
	}
}
