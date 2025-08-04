using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000133 RID: 307
	public class TabChangeEventArgs : EventArgs
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000C5F RID: 3167 RVA: 0x00009C60 File Offset: 0x00007E60
		// (set) Token: 0x06000C60 RID: 3168 RVA: 0x00009C68 File Offset: 0x00007E68
		public string AppName { get; set; } = string.Empty;

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000C61 RID: 3169 RVA: 0x00009C71 File Offset: 0x00007E71
		// (set) Token: 0x06000C62 RID: 3170 RVA: 0x00009C79 File Offset: 0x00007E79
		public string PackageName { get; set; } = string.Empty;

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000C63 RID: 3171 RVA: 0x00009C82 File Offset: 0x00007E82
		// (set) Token: 0x06000C64 RID: 3172 RVA: 0x00009C8A File Offset: 0x00007E8A
		public TabType TabType { get; set; }

		// Token: 0x06000C65 RID: 3173 RVA: 0x00009C93 File Offset: 0x00007E93
		public TabChangeEventArgs(string appName, string packageName, TabType tabType)
		{
			this.AppName = appName;
			this.PackageName = packageName;
			this.TabType = tabType;
		}
	}
}
