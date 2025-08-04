using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000168 RID: 360
	[Flags]
	public enum RawMouseFlags : ushort
	{
		// Token: 0x04000983 RID: 2435
		MoveRelative = 0,
		// Token: 0x04000984 RID: 2436
		MoveAbsolute = 1,
		// Token: 0x04000985 RID: 2437
		VirtualDesktop = 2,
		// Token: 0x04000986 RID: 2438
		AttributesChanged = 4
	}
}
