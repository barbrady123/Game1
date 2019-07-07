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
	// TODO: Should this become MapEntityList and have it contained in the Map...given that's actually what this data is...
	// (and then rename all the world shit lol)
	// Character and NPCs should be contained in here...
	public class WorldEntityList
	{
		private readonly int _width;
		private readonly int _height;
		private readonly int _cellSize;
		private WorldCell[,] _cells;
		private Dictionary<IWorldEntity, List<WorldCell>> _entityCells;
		
		public WorldEntityList(int width, int height, int cellSize)
		{
			_width = width;
			_height = height;
			_cellSize = cellSize;
			_cells = new WorldCell[width, height];
			_entityCells = new Dictionary<IWorldEntity, List<WorldCell>>();
		}

		public WorldCell this[int xIndex, int yIndex] => _cells[xIndex, yIndex];

		/// <summary>
		/// Should return an array of all unique entities in the specified cell range...
		/// </summary>
		public IWorldEntity[] GetEntities(int startX, int startY, int endX, int endY)
		{
			var entities = new HashSet<IWorldEntity>();

			for (int y = startY; y <= endY; y++)
			for (int x = startX; x <= endX; x++)
			{
				if (_cells[x, y]?.Any() ?? false)
				{
					foreach (var entity in _cells[x, y].OrderBy(e => e.Position.Y))
						entities.Add(entity);
				}
			}

			// TODO: So, this is odd...basically the end goal here is all WorldTransitions drawn first, then All WorldItems, then everything else as it is from above,
			// which is y-sorted in cell, then y-sorted per cell (and X but we don't care about that)....but I don't want to run through the above loop 3 times just to get 3 sets
			// of entities...so I'm pulling this shenanigans for now, but it might be more efficient to extract these to 3 different hashsets during the above lookup and combine them
			// at the end...ideally though we wouldn't do it per object type, but something more general like a "z-index" type of value...but then we couldn't have a fixed set of
			// collections to store found entities in, so we'd need something like a SortedDictionary keyed on the index value which we combine at the end.  Given I expect a
			// small amount of entities to be found during this process most of the time, this stupid line probably isn't too terrible for now....or I suppose we could just add a
			// "z-index" type property to IWorldEntity, then just do a OrderBy(z-index).ThenBy(y-coord) kind of thing to make this easier LOL....worry about it later
			return new HashSet<IWorldEntity>(entities.OfType<WorldTransition>().Cast<IWorldEntity>().Concat(entities.OfType<WorldItem>().Cast<IWorldEntity>()).Concat(entities)).ToArray();
		}

		public T[] GetEntities<T>(int startX, int startY, int endX, int endY) where T: IWorldEntity
		{
			var entities = new HashSet<T>();

			for (int y = startY; y <= endY; y++)
			for (int x = startX; x <= endX; x++)
			{
				if (_cells[x, y]?.Any() ?? false)
				{
					foreach (var entity in _cells[x, y].OfType<T>().OrderBy(e => e.Position.Y))
						entities.Add(entity);
				}
			}

			return entities.ToArray();
		}

		public IWorldEntity[] GetEntities(Point point)
		{
			return GetEntities(point.X / _cellSize, point.Y / _cellSize, point.X / _cellSize, point.Y / _cellSize);
		}

		public T[] GetEntities<T>(Point point) where T: IWorldEntity
		{
			return GetEntities<T>(point.X / _cellSize, point.Y / _cellSize, point.X / _cellSize, point.Y / _cellSize);
		}

		public IWorldEntity[] GetEntities(Rectangle bounds)
		{
			return GetEntities(bounds.X / _cellSize, bounds.Y / _cellSize, (bounds.Right - 1) / _cellSize, (bounds.Bottom - 1) / _cellSize);
		}

		public T[] GetEntities<T>(Rectangle bounds) where T: IWorldEntity
		{
			return GetEntities<T>(bounds.X / _cellSize, bounds.Y / _cellSize, (bounds.Right - 1) / _cellSize, (bounds.Bottom - 1) / _cellSize);
		}

		public T Add<T>(T entity) where T: IWorldEntity
		{
			var startCell = entity.Bounds.TopLeftPoint().DivideBy(_cellSize);
			var endCell = entity.Bounds.BottomRightPoint().DivideBy(_cellSize);

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
			if (_entityCells.TryGetValue(entity, out var cells) && cells != null)
			{
				foreach (var cell in cells)
					cell.Remove(entity);

				_entityCells[entity].Clear();
			}
		}
	}
}
