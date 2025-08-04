using System;
using System.Collections.Generic;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000092 RID: 146
	internal class GuidanceCloudInfo
	{
		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000651 RID: 1617 RVA: 0x000062E7 File Offset: 0x000044E7
		public Dictionary<string, CustomThumbnail> CustomThumbnails { get; } = new Dictionary<string, CustomThumbnail>();

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000652 RID: 1618 RVA: 0x000062EF File Offset: 0x000044EF
		public Dictionary<GuidanceVideoType, VideoThumbnailInfo> DefaultThumbnails { get; } = new Dictionary<GuidanceVideoType, VideoThumbnailInfo>();

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000653 RID: 1619 RVA: 0x000062F7 File Offset: 0x000044F7
		public Dictionary<string, HelpArticle> HelpArticles { get; } = new Dictionary<string, HelpArticle>();

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x000062FF File Offset: 0x000044FF
		public List<GameSetting> GameSettings { get; } = new List<GameSetting>();
	}
}
