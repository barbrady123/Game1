using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Enum
{
	[Flags]
	public enum ComponentState
	{
		Visible = 0x01,

		Active = 0x02,

		TakingInput = 0x04,

		DetectingMousePosition = 0x08,

		ActiveVisible = Visible | Active,

		AllInput = TakingInput | DetectingMousePosition,

		ActiveAllInput = Active | AllInput,

		All = Visible | Active | TakingInput | DetectingMousePosition
	}
}
