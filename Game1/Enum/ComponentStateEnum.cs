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
		None = 0x00,

		Visible = 0x01,

		Active = 0x02,

		TakingInput = 0x04,

		ActiveVisible = Visible | Active,

		ActiveInput = Active | TakingInput,

		All = Visible | Active | TakingInput
	}
}
