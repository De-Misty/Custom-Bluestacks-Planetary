using System;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200003E RID: 62
	[Serializable]
	public class NotificationModeInfo
	{
		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x00004688 File Offset: 0x00002888
		// (set) Token: 0x060003B3 RID: 947 RVA: 0x00004690 File Offset: 0x00002890
		public AppPackageListObject NotificationModeAppPackages { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00004699 File Offset: 0x00002899
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x000046A1 File Offset: 0x000028A1
		[JsonProperty(PropertyName = "consecutive_session_count_number", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public int ExitPopupCount { get; set; } = 3;
	}
}
