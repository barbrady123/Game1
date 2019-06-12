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
		private static readonly Dictionary<string, SpriteBatchData> _batches = new Dictionary<string, SpriteBatchData>();

		public static string Add(SpriteBatch batch, RasterizerState rasterizeState, int index, string id)
		{
			if (String.IsNullOrWhiteSpace(id))
				id = Guid.NewGuid().ToString();

			batch.Name = id;
			_batches[id] = new SpriteBatchData { SpriteBatch = batch, RasterizeState = rasterizeState, Index = index, Id = id, ScissorWindow = Rectangle.Empty };
			return id;
		}

		public static SpriteBatchData Get(string id) => _batches[id];

		public static SpriteBatchData[] GetAllSorted() => _batches.Values.OrderBy(b => b.Index).ToArray();
	}
}
