using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public static class ListExtensions
	{
		public static T AddItem<T>(this List<T> list, T item)
		{
			list.Add(item);
			return item;
		}
	}
}
