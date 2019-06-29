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
using Game1.Items;
using System.Collections;

namespace Game1
{
	public class WorldCell : IEnumerable<IWorldEntity>
	{
		private List<IWorldEntity> _entities;

		public WorldCell(IWorldEntity entity)
		{
			_entities = new List<IWorldEntity> { entity };
		}

		public void Add(IWorldEntity entity)
		{
			_entities.Add(entity);
		}

		public void Remove(IWorldEntity entity)
		{
			if (_entities != null)
				_entities.Remove(entity);
		}

		public IEnumerator<IWorldEntity> GetEnumerator() => _entities.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public bool Any => _entities?.Any() ?? false;		
	}
}
