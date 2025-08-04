using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200008F RID: 143
	public class CustomThumbnail
	{
		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x0000614B File Offset: 0x0000434B
		// (set) Token: 0x0600062A RID: 1578 RVA: 0x00006153 File Offset: 0x00004353
		[JsonProperty(PropertyName = "pan", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public VideoThumbnailInfo Pan { get; set; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x0600062B RID: 1579 RVA: 0x0000615C File Offset: 0x0000435C
		// (set) Token: 0x0600062C RID: 1580 RVA: 0x00006164 File Offset: 0x00004364
		[JsonProperty(PropertyName = "moba", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public VideoThumbnailInfo Moba { get; set; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x0000616D File Offset: 0x0000436D
		// (set) Token: 0x0600062E RID: 1582 RVA: 0x00006175 File Offset: 0x00004375
		[JsonProperty(PropertyName = "gamepad", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public VideoThumbnailInfo Gamepad { get; set; }

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x0000617E File Offset: 0x0000437E
		// (set) Token: 0x06000630 RID: 1584 RVA: 0x00006186 File Offset: 0x00004386
		[JsonProperty(PropertyName = "special", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public VideoThumbnailInfo Special { get; set; }

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x0000618F File Offset: 0x0000438F
		// (set) Token: 0x06000632 RID: 1586 RVA: 0x00006197 File Offset: 0x00004397
		[JsonProperty(PropertyName = "package")]
		public string Package { get; set; }

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000633 RID: 1587 RVA: 0x000061A0 File Offset: 0x000043A0
		// (set) Token: 0x06000634 RID: 1588 RVA: 0x000061A8 File Offset: 0x000043A8
		[JsonProperty(PropertyName = "schemespecific")]
		public Dictionary<string, VideoThumbnailInfo> SchemeSpecific { get; set; } = new Dictionary<string, VideoThumbnailInfo>();

		// Token: 0x170001EC RID: 492
		public object this[string propertyName]
		{
			get
			{
				PropertyInfo property = typeof(CustomThumbnail).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
				if (property == null)
				{
					return null;
				}
				return property.GetValue(this, null);
			}
		}
	}
}
