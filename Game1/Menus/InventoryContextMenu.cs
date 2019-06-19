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
using Game1.Interface;
using Game1.Items;

namespace Game1.Menus
{
	// Seems like we don't even need this class anymore???  ContextMenu is flexible enough....?
	public class InventoryContextMenu : ContextMenu
	{
		public InventoryContextMenu(SpriteBatchData spriteBatchData = null) : base(null, spriteBatchData)	{ }
	}
}
