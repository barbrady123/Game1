using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public class FixedQueue<T> : Queue<T> where T: class
	{
		public int Size { get; private set; }

		public FixedQueue(int size)
		{
			this.Size = size;
		}

		public FixedQueue(IEnumerable<T> collection, int size) : base(collection)
		{
			if (collection.Count() > size)
				throw new ArgumentException("Collection is larger than fixed queue size!");

			this.Size = size;
		}

		public new T Enqueue(T item)
		{
			T oldestItem = null;

			while (base.Count >= this.Size)
				oldestItem = base.Dequeue();

			base.Enqueue(item);
			return oldestItem;
		}
	}
}
