using System;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001DC RID: 476
	internal class BlueStacksUpdateData
	{
		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060012B8 RID: 4792 RVA: 0x0000D60C File Offset: 0x0000B80C
		// (set) Token: 0x060012B9 RID: 4793 RVA: 0x0000D614 File Offset: 0x0000B814
		public string ClientVersion { get; set; } = "";

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060012BA RID: 4794 RVA: 0x0000D61D File Offset: 0x0000B81D
		// (set) Token: 0x060012BB RID: 4795 RVA: 0x0000D625 File Offset: 0x0000B825
		public string EngineVersion { get; set; } = "";

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060012BC RID: 4796 RVA: 0x0000D62E File Offset: 0x0000B82E
		// (set) Token: 0x060012BD RID: 4797 RVA: 0x0000D636 File Offset: 0x0000B836
		public bool IsFullInstaller { get; set; }

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060012BE RID: 4798 RVA: 0x0000D63F File Offset: 0x0000B83F
		// (set) Token: 0x060012BF RID: 4799 RVA: 0x0000D647 File Offset: 0x0000B847
		public string Md5 { get; set; } = "";

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060012C0 RID: 4800 RVA: 0x0000D650 File Offset: 0x0000B850
		// (set) Token: 0x060012C1 RID: 4801 RVA: 0x0000D658 File Offset: 0x0000B858
		public string UpdateType { get; set; } = "";

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060012C2 RID: 4802 RVA: 0x0000D661 File Offset: 0x0000B861
		// (set) Token: 0x060012C3 RID: 4803 RVA: 0x0000D669 File Offset: 0x0000B869
		public string DownloadUrl { get; set; } = "";

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060012C4 RID: 4804 RVA: 0x0000D672 File Offset: 0x0000B872
		// (set) Token: 0x060012C5 RID: 4805 RVA: 0x0000D67A File Offset: 0x0000B87A
		public List<string> UpdateDescrption { get; set; } = new List<string>();

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060012C6 RID: 4806 RVA: 0x0000D683 File Offset: 0x0000B883
		// (set) Token: 0x060012C7 RID: 4807 RVA: 0x0000D68B File Offset: 0x0000B88B
		public bool IsUpdateAvailble { get; set; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060012C8 RID: 4808 RVA: 0x0000D694 File Offset: 0x0000B894
		// (set) Token: 0x060012C9 RID: 4809 RVA: 0x0000D69C File Offset: 0x0000B89C
		public string UpdateDownloadLocation { get; set; } = "";

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x060012CA RID: 4810 RVA: 0x0000D6A5 File Offset: 0x0000B8A5
		// (set) Token: 0x060012CB RID: 4811 RVA: 0x0000D6AD File Offset: 0x0000B8AD
		public string DetailedChangeLogsUrl { get; set; } = "";

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x0000D6B6 File Offset: 0x0000B8B6
		// (set) Token: 0x060012CD RID: 4813 RVA: 0x0000D6BE File Offset: 0x0000B8BE
		public bool IsTryAgain { get; set; }
	}
}
