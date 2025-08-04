using System;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001B1 RID: 433
	internal class BootBanner
	{
		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06001119 RID: 4377 RVA: 0x0000C3E0 File Offset: 0x0000A5E0
		// (set) Token: 0x0600111A RID: 4378 RVA: 0x0000C3E8 File Offset: 0x0000A5E8
		[JsonProperty("frequency")]
		public string Frequency { get; set; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x0600111B RID: 4379 RVA: 0x0000C3F1 File Offset: 0x0000A5F1
		// (set) Token: 0x0600111C RID: 4380 RVA: 0x0000C3F9 File Offset: 0x0000A5F9
		[JsonProperty("click_action_packagename")]
		public string ClickActionPackagename { get; set; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600111D RID: 4381 RVA: 0x0000C402 File Offset: 0x0000A602
		// (set) Token: 0x0600111E RID: 4382 RVA: 0x0000C40A File Offset: 0x0000A60A
		[JsonProperty("click_generic_action")]
		public string ClickGenericAction { get; set; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600111F RID: 4383 RVA: 0x0000C413 File Offset: 0x0000A613
		// (set) Token: 0x06001120 RID: 4384 RVA: 0x0000C41B File Offset: 0x0000A61B
		[JsonProperty("click_action_value")]
		public string ClickActionValue { get; set; }

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06001121 RID: 4385 RVA: 0x0000C424 File Offset: 0x0000A624
		// (set) Token: 0x06001122 RID: 4386 RVA: 0x0000C42C File Offset: 0x0000A62C
		[JsonProperty("id")]
		public string Id { get; set; }

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06001123 RID: 4387 RVA: 0x0000C435 File Offset: 0x0000A635
		// (set) Token: 0x06001124 RID: 4388 RVA: 0x0000C43D File Offset: 0x0000A63D
		[JsonProperty("button_text")]
		public string ButtonText { get; set; }

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06001125 RID: 4389 RVA: 0x0000C446 File Offset: 0x0000A646
		// (set) Token: 0x06001126 RID: 4390 RVA: 0x0000C44E File Offset: 0x0000A64E
		[JsonProperty("order")]
		public string Order { get; set; }

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x0000C457 File Offset: 0x0000A657
		// (set) Token: 0x06001128 RID: 4392 RVA: 0x0000C45F File Offset: 0x0000A65F
		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06001129 RID: 4393 RVA: 0x0000C468 File Offset: 0x0000A668
		// (set) Token: 0x0600112A RID: 4394 RVA: 0x0000C470 File Offset: 0x0000A670
		[JsonProperty("hash_tags")]
		public string HashTags { get; set; }
	}
}
