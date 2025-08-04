using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200009E RID: 158
	public class AppRecommendationSection
	{
		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x0000662B File Offset: 0x0000482B
		// (set) Token: 0x060006B2 RID: 1714 RVA: 0x00006633 File Offset: 0x00004833
		[JsonProperty(PropertyName = "section_header")]
		public string AppSuggestionHeader { get; set; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060006B3 RID: 1715 RVA: 0x0000663C File Offset: 0x0000483C
		// (set) Token: 0x060006B4 RID: 1716 RVA: 0x00006644 File Offset: 0x00004844
		[JsonProperty(PropertyName = "client_show_count", DefaultValueHandling = DefaultValueHandling.Populate)]
		[DefaultValue(3)]
		public int ClientShowCount { get; set; } = 3;

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x0000664D File Offset: 0x0000484D
		[JsonProperty(PropertyName = "suggested_apps")]
		public List<AppRecommendation> AppSuggestions { get; } = new List<AppRecommendation>();
	}
}
