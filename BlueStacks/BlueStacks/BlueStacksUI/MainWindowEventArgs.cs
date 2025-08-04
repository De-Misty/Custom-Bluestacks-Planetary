using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200007B RID: 123
	internal class MainWindowEventArgs
	{
		// Token: 0x0200007C RID: 124
		internal class CursorLockChangedEventArgs : EventArgs
		{
			// Token: 0x170001E0 RID: 480
			// (get) Token: 0x060005DD RID: 1501 RVA: 0x00005EC0 File Offset: 0x000040C0
			// (set) Token: 0x060005DE RID: 1502 RVA: 0x00005EC8 File Offset: 0x000040C8
			public bool IsLocked { get; set; }
		}

		// Token: 0x0200007D RID: 125
		internal class FullScreenChangedEventArgs : EventArgs
		{
			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00005ED9 File Offset: 0x000040D9
			// (set) Token: 0x060005E1 RID: 1505 RVA: 0x00005EE1 File Offset: 0x000040E1
			public bool IsFullscreen { get; set; }
		}

		// Token: 0x0200007E RID: 126
		internal class FrontendGridVisibilityChangedEventArgs : EventArgs
		{
			// Token: 0x170001E2 RID: 482
			// (get) Token: 0x060005E3 RID: 1507 RVA: 0x00005EEA File Offset: 0x000040EA
			// (set) Token: 0x060005E4 RID: 1508 RVA: 0x00005EF2 File Offset: 0x000040F2
			public bool IsVisible { get; set; }
		}

		// Token: 0x0200007F RID: 127
		internal class BrowserOTSCompletedCallbackEventArgs : EventArgs
		{
			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00005EFB File Offset: 0x000040FB
			// (set) Token: 0x060005E7 RID: 1511 RVA: 0x00005F03 File Offset: 0x00004103
			internal string CallbackFunction { get; set; }
		}
	}
}
