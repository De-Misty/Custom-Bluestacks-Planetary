using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001A7 RID: 423
	public class QuestRuleState
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x0000C1DC File Offset: 0x0000A3DC
		// (set) Token: 0x060010EC RID: 4332 RVA: 0x0000C1E4 File Offset: 0x0000A3E4
		public long Interaction { get; set; }

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x060010ED RID: 4333 RVA: 0x0000C1ED File Offset: 0x0000A3ED
		// (set) Token: 0x060010EE RID: 4334 RVA: 0x0000C1F5 File Offset: 0x0000A3F5
		public long TotalTime { get; set; }

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x0000C1FE File Offset: 0x0000A3FE
		// (set) Token: 0x060010F0 RID: 4336 RVA: 0x0000C206 File Offset: 0x0000A406
		public QuestRule QuestRules { get; set; }
	}
}
