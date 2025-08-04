using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001A5 RID: 421
	public class AppIconPromotionObject
	{
		// Token: 0x170002CB RID: 715
		// (get) Token: 0x060010B3 RID: 4275 RVA: 0x0000BFE8 File Offset: 0x0000A1E8
		// (set) Token: 0x060010B4 RID: 4276 RVA: 0x0000BFF0 File Offset: 0x0000A1F0
		public string AppPromotionID { get; set; } = string.Empty;

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x060010B5 RID: 4277 RVA: 0x0000BFF9 File Offset: 0x0000A1F9
		// (set) Token: 0x060010B6 RID: 4278 RVA: 0x0000C001 File Offset: 0x0000A201
		public GenericAction AppPromotionAction { get; set; } = GenericAction.InstallPlay;

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x060010B7 RID: 4279 RVA: 0x0000C00A File Offset: 0x0000A20A
		// (set) Token: 0x060010B8 RID: 4280 RVA: 0x0000C012 File Offset: 0x0000A212
		public string AppPromotionPackage { get; set; } = string.Empty;

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x060010B9 RID: 4281 RVA: 0x0000C01B File Offset: 0x0000A21B
		// (set) Token: 0x060010BA RID: 4282 RVA: 0x0000C023 File Offset: 0x0000A223
		public string AppPromotionName { get; set; } = string.Empty;

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x060010BB RID: 4283 RVA: 0x0000C02C File Offset: 0x0000A22C
		// (set) Token: 0x060010BC RID: 4284 RVA: 0x0000C034 File Offset: 0x0000A234
		public string AppPromotionActionParam { get; set; } = string.Empty;

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x060010BD RID: 4285 RVA: 0x0000C03D File Offset: 0x0000A23D
		// (set) Token: 0x060010BE RID: 4286 RVA: 0x0000C045 File Offset: 0x0000A245
		public string AppPromotionImagePath { get; set; } = string.Empty;
	}
}
