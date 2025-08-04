using System;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001A4 RID: 420
	[JsonObject(MemberSerialization.OptIn)]
	public class QuestRule
	{
		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x060010A4 RID: 4260 RVA: 0x0000BF5E File Offset: 0x0000A15E
		// (set) Token: 0x060010A5 RID: 4261 RVA: 0x0000BF66 File Offset: 0x0000A166
		[JsonProperty("rule_id")]
		public string RuleId { get; set; }

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x060010A6 RID: 4262 RVA: 0x0000BF6F File Offset: 0x0000A16F
		// (set) Token: 0x060010A7 RID: 4263 RVA: 0x0000BF77 File Offset: 0x0000A177
		[JsonProperty("app_pkg", NullValueHandling = NullValueHandling.Ignore)]
		public string AppPackage { get; set; } = string.Empty;

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x060010A8 RID: 4264 RVA: 0x0000BF80 File Offset: 0x0000A180
		// (set) Token: 0x060010A9 RID: 4265 RVA: 0x0000BF88 File Offset: 0x0000A188
		[JsonProperty("usage_time", NullValueHandling = NullValueHandling.Ignore)]
		public int AppUsageTime { get; set; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x060010AA RID: 4266 RVA: 0x0000BF91 File Offset: 0x0000A191
		// (set) Token: 0x060010AB RID: 4267 RVA: 0x0000BF99 File Offset: 0x0000A199
		[JsonProperty("user_interactions", NullValueHandling = NullValueHandling.Ignore)]
		public int MinUserInteraction { get; set; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x060010AC RID: 4268 RVA: 0x0000BFA2 File Offset: 0x0000A1A2
		// (set) Token: 0x060010AD RID: 4269 RVA: 0x0000BFAA File Offset: 0x0000A1AA
		[JsonProperty("recurring", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsRecurring { get; set; }

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x060010AE RID: 4270 RVA: 0x0000BFB3 File Offset: 0x0000A1B3
		// (set) Token: 0x060010AF RID: 4271 RVA: 0x0000BFBB File Offset: 0x0000A1BB
		[JsonProperty("num_of_occurances", NullValueHandling = NullValueHandling.Ignore)]
		public int RecurringCount { get; set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x060010B0 RID: 4272 RVA: 0x0000BFC4 File Offset: 0x0000A1C4
		// (set) Token: 0x060010B1 RID: 4273 RVA: 0x0000BFCC File Offset: 0x0000A1CC
		[JsonProperty("cloud_handler", NullValueHandling = NullValueHandling.Ignore)]
		public string CloudHandler { get; set; }
	}
}
