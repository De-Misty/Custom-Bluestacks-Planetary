using System;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200003F RID: 63
	[Serializable]
	public class OnBoardingInfo
	{
		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x000046B9 File Offset: 0x000028B9
		// (set) Token: 0x060003B8 RID: 952 RVA: 0x000046C1 File Offset: 0x000028C1
		public AppPackageListObject OnBoardingAppPackages { get; set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x000046CA File Offset: 0x000028CA
		// (set) Token: 0x060003BA RID: 954 RVA: 0x000046D2 File Offset: 0x000028D2
		[JsonProperty(PropertyName = "skip_button_timer", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public int OnBoardingSkipTimer { get; set; } = 5;
	}
}
