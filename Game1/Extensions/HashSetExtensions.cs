using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public static class HashSetExtensions
	{
		public static T AddItem<T>(this HashSet<T> set, T item)
		{
			set.Add(item);
			return item;
		}

		public static T RemoveItem<T>(this HashSet<T> set, T item)
		{
			set.Remove(item);
			return item;
		}
	}
}
