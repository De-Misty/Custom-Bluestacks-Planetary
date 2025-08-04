using System;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200006C RID: 108
	public class HelpArticleInfo
	{
		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x0000595F File Offset: 0x00003B5F
		// (set) Token: 0x06000550 RID: 1360 RVA: 0x00005967 File Offset: 0x00003B67
		[JsonProperty(PropertyName = "url")]
		public string HelpArticleUrl { get; set; }
	}
}
