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
	public class WorldItem
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

		public WorldItem()
		{
			_inRange = false;
		}

		public void Update(GameTime gameTime)
		{
			// This will be needed later, for things like expiration, etc...
		}
	}
}
