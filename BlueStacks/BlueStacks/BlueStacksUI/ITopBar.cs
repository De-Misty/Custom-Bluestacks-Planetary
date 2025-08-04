using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200013B RID: 315
	internal interface ITopBar
	{
		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000CA7 RID: 3239
		// (set) Token: 0x06000CA8 RID: 3240
		string AppName { get; set; }

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000CA9 RID: 3241
		// (set) Token: 0x06000CAA RID: 3242
		string CharacterName { get; set; }

		// Token: 0x06000CAB RID: 3243
		void ShowSyncPanel(bool show = false);

		// Token: 0x06000CAC RID: 3244
		void HideSyncPanel();
	}
}
