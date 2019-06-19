using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	public interface ISupportsTooltip
	{
		string TooltipText { get; }
		event EventHandler<ComponentEventArgs> OnMouseOut;
		event EventHandler<ComponentEventArgs> OnMouseOver;
	}
}
