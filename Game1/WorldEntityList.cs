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
using Game1.Maps;

namespace Game1
{
	public class WorldEntityList
	{
		private readonly int _width;
		private readonly int _height;
		private WorldCell[,] _cells;
		private Dictionary<IWorldEntity, List<WorldCell>> _entityCells;
		
		public WorldEntityList(int width, int height)
		{
			_width = width;
			_height = height;
			_cells = new WorldCell[width, height];
			_entityCells = new Dictionary<IWorldEntity, List<WorldCell>>();
		}

		public WorldCell this[int xIndex, int yIndex] => _cells[xIndex, yIndex];

		/// <summary>
		/// Should return an array of all unique entities in the specified cell range...
		/// </summary>
		public IWorldEntity[] GetEntities(int startX, int startY, int endX, int endY)
		{
			/* shouldn't need to clamp these... 
			startX = Util.Clamp(startX, 0, _width);
			startY = Util.Clamp(startY, 0, _height);
			endX = Util.Clamp(endX, 0, _width);
			endY = Util.Clamp(endY, 0, _height);
			*/

			var entities = new HashSet<IWorldEntity>();

			for (int y = startY; y <= endY; y++)
			for (int x = startX; x <= endX; x++)
			{
				if (_cells[x, y]?.Any() ?? false)
				{
					foreach (var entity in _cells[x, y])
						entities.Add(entity);
				}
			}

			return entities.ToArray();
		}

		public IWorldEntity[] GetEntities(Rectangle bounds)
		{
			return GetEntities(bounds.X / Game1.TileSize, bounds.Y / Game1.TileSize, (bounds.Right - 1) / Game1.TileSize, (bounds.Bottom - 1) / Game1.TileSize);
		}

		public T Add<T>(T entity) where T: IWorldEntity
		{
			// TODO: Really, the cell resolution here should be customizable, also...weird dependency on Game1 here...
			var startCell = entity.Bounds.TopLeftPoint().DivideBy(Game1.TileSize);
			var endCell = entity.Bounds.BottomRightPoint().DivideBy(Game1.TileSize);

			for (int y = startCell.Y; y <= endCell.Y; y++)
			for (int x = startCell.X; x <= endCell.X; x++)
			{
				if (_cells[x, y] != null)
					_cells[x, y].Add(entity);
				else
					_cells[x, y] = new WorldCell(entity);
				
				if (_entityCells.TryGetValue(entity, out var entityCells) && entityCells != null)
					entityCells.Add(_cells[x, y]);
				else
					_entityCells[entity] = new List<WorldCell> { _cells[x, y] };
			}

			return entity;
		}

		public void Move(IWorldEntity entity)
		{
			Remove(entity);
			Add(entity);
		}

		public void Remove(IWorldEntity entity)
		{
			if (_entityCells.TryGetValue(entity, out List<WorldCell> cells) && cells != null)
			{
				foreach (var cell in cells)
					cell.Remove(entity);

				_entityCells[entity].Clear();
			}
		}
	}
}
