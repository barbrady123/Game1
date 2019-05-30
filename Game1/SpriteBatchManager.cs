using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
	// TODO: Move elsewhere...
	public class SpriteBatchData
	{
		public SpriteBatch SpriteBatch { get; set; }
		public RasterizerState RasterizeState { get; set; }
		public Rectangle ScissorWindow { get; set; }
		public int Index { get; set; }
		public string Id { get; set; }
	}

	public static class SpriteBatchManager
	{
		private static readonly List<SpriteBatchData> _batches = new List<SpriteBatchData>();

		public static string Add(SpriteBatch batch, RasterizerState rasterizeState, int index, string id)
		{
			if (String.IsNullOrWhiteSpace(id))
				id = Guid.NewGuid().ToString();

			_batches.Add(new SpriteBatchData { SpriteBatch = batch, RasterizeState = rasterizeState, Index = index, Id = id, ScissorWindow = Rectangle.Empty });
			return id;
		}

		public static SpriteBatchData Get(string id)
		{
			return _batches.FirstOrDefault(b => b.Id == id);
		}

		public static SpriteBatchData[] GetAllSorted()
		{
			return _batches.OrderBy(b => b.Index).ToArray();
		}
	}
}
