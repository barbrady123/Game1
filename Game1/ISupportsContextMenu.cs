using System.Collections.Generic;

namespace Game1
{
	public interface ISupportsContextMenu
	{
		List<string> GetContextMenuOptions();
	}
}
