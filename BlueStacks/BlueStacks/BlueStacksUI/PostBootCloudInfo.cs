using System;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000040 RID: 64
	public class PostBootCloudInfo
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060003BC RID: 956 RVA: 0x000046EA File Offset: 0x000028EA
		// (set) Token: 0x060003BD RID: 957 RVA: 0x000046F2 File Offset: 0x000028F2
		public NotificationModeInfo GameNotificationAppPackages { get; set; } = new NotificationModeInfo();

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060003BE RID: 958 RVA: 0x000046FB File Offset: 0x000028FB
		// (set) Token: 0x060003BF RID: 959 RVA: 0x00004703 File Offset: 0x00002903
		public OnBoardingInfo OnBoardingInfo { get; set; } = new OnBoardingInfo();

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x0000470C File Offset: 0x0000290C
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x00004714 File Offset: 0x00002914
		public AppSpecificCustomCursorInfo AppSpecificCustomCursorInfo { get; set; } = new AppSpecificCustomCursorInfo();

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x0000471D File Offset: 0x0000291D
		public List<string> IgnoredActivitiesForTabs { get; } = new List<string>();

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x00004725 File Offset: 0x00002925
		// (set) Token: 0x060003C4 RID: 964 RVA: 0x0000472D File Offset: 0x0000292D
		public GameAwareOnboardingInfo GameAwareOnboardingInfo { get; set; } = new GameAwareOnboardingInfo();
	}
}
