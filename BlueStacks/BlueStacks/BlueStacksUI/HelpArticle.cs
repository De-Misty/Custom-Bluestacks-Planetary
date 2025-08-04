using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000090 RID: 144
	public class HelpArticle
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x000061E5 File Offset: 0x000043E5
		// (set) Token: 0x06000638 RID: 1592 RVA: 0x000061ED File Offset: 0x000043ED
		[JsonProperty(PropertyName = "gamepad", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public HelpArticleInfo Gamepad { get; set; }

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x000061F6 File Offset: 0x000043F6
		// (set) Token: 0x0600063A RID: 1594 RVA: 0x000061FE File Offset: 0x000043FE
		[JsonProperty(PropertyName = "moba", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public HelpArticleInfo Moba { get; set; }

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x00006207 File Offset: 0x00004407
		// (set) Token: 0x0600063C RID: 1596 RVA: 0x0000620F File Offset: 0x0000440F
		[JsonProperty(PropertyName = "pan", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public HelpArticleInfo Pan { get; set; }

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x00006218 File Offset: 0x00004418
		// (set) Token: 0x0600063E RID: 1598 RVA: 0x00006220 File Offset: 0x00004420
		[JsonProperty(PropertyName = "special", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public HelpArticleInfo Special { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x0600063F RID: 1599 RVA: 0x00006229 File Offset: 0x00004429
		// (set) Token: 0x06000640 RID: 1600 RVA: 0x00006231 File Offset: 0x00004431
		[JsonProperty(PropertyName = "default", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public HelpArticleInfo Default { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000641 RID: 1601 RVA: 0x0000623A File Offset: 0x0000443A
		// (set) Token: 0x06000642 RID: 1602 RVA: 0x00006242 File Offset: 0x00004442
		[JsonProperty(PropertyName = "package")]
		public string Package { get; set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000643 RID: 1603 RVA: 0x0000624B File Offset: 0x0000444B
		// (set) Token: 0x06000644 RID: 1604 RVA: 0x00006253 File Offset: 0x00004453
		[JsonProperty(PropertyName = "schemespecific")]
		public Dictionary<string, HelpArticleInfo> SchemeSpecific { get; set; } = new Dictionary<string, HelpArticleInfo>();

		// Token: 0x170001F4 RID: 500
		public object this[string propertyName]
		{
			get
			{
				PropertyInfo property = typeof(HelpArticle).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
				if (property == null)
				{
					return null;
				}
				return property.GetValue(this, null);
			}
		}
	}
}
