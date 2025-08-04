using System;
using System.IO;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200009F RID: 159
	public class SearchRecommendation
	{
		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x0000666F File Offset: 0x0000486F
		// (set) Token: 0x060006B8 RID: 1720 RVA: 0x00006677 File Offset: 0x00004877
		public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x00006680 File Offset: 0x00004880
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x00006688 File Offset: 0x00004888
		public string IconId { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x00006691 File Offset: 0x00004891
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x00006699 File Offset: 0x00004899
		public string ImagePath { get; set; } = string.Empty;

		// Token: 0x060006BD RID: 1725 RVA: 0x00026128 File Offset: 0x00024328
		internal void DeleteFile()
		{
			try
			{
				File.Delete(this.ImagePath);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't delete SearchRecommendation file: " + this.ImagePath);
				Logger.Error(ex.ToString());
			}
		}
	}
}
