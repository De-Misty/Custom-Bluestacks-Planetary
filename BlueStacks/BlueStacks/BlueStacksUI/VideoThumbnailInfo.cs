using System;
using System.IO;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000091 RID: 145
	public class VideoThumbnailInfo
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000647 RID: 1607 RVA: 0x00006290 File Offset: 0x00004490
		// (set) Token: 0x06000648 RID: 1608 RVA: 0x00006298 File Offset: 0x00004498
		[JsonProperty(PropertyName = "thumbnail_id")]
		public string ThumbnailId { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000649 RID: 1609 RVA: 0x000062A1 File Offset: 0x000044A1
		// (set) Token: 0x0600064A RID: 1610 RVA: 0x000062A9 File Offset: 0x000044A9
		[JsonProperty(PropertyName = "thumbnail_url")]
		public string ThumbnailUrl { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x0600064B RID: 1611 RVA: 0x000062B2 File Offset: 0x000044B2
		// (set) Token: 0x0600064C RID: 1612 RVA: 0x000062BA File Offset: 0x000044BA
		[JsonProperty(PropertyName = "type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public GuidanceVideoType ThumbnailType { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x0600064D RID: 1613 RVA: 0x000062C3 File Offset: 0x000044C3
		// (set) Token: 0x0600064E RID: 1614 RVA: 0x000062CB File Offset: 0x000044CB
		public string ImagePath { get; set; } = string.Empty;

		// Token: 0x0600064F RID: 1615 RVA: 0x00024898 File Offset: 0x00022A98
		internal void DeleteFile()
		{
			try
			{
				File.Delete(this.ImagePath);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't delete AppRecommendation file: " + this.ImagePath);
				Logger.Error(ex.ToString());
			}
		}
	}
}
