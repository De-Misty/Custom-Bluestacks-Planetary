using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000169 RID: 361
	[Flags]
	public enum RawMouseButtons : ushort
	{
		// Token: 0x04000988 RID: 2440
		None = 0,
		// Token: 0x04000989 RID: 2441
		LeftDown = 1,
		// Token: 0x0400098A RID: 2442
		LeftUp = 2,
		// Token: 0x0400098B RID: 2443
		RightDown = 4,
		// Token: 0x0400098C RID: 2444
		RightUp = 8,
		// Token: 0x0400098D RID: 2445
		MiddleDown = 16,
		// Token: 0x0400098E RID: 2446
		MiddleUp = 32,
		// Token: 0x0400098F RID: 2447
		Button4Down = 64,
		// Token: 0x04000990 RID: 2448
		Button4Up = 128,
		// Token: 0x04000991 RID: 2449
		Button5Down = 256,
		// Token: 0x04000992 RID: 2450
		Button5Up = 512,
		// Token: 0x04000993 RID: 2451
		MouseWheel = 1024
	}
}
