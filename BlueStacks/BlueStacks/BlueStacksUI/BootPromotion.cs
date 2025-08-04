using System;
using System.IO;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000AB RID: 171
	[Serializable]
	public class BootPromotion
	{
		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060006E9 RID: 1769 RVA: 0x00006862 File Offset: 0x00004A62
		// (set) Token: 0x060006EA RID: 1770 RVA: 0x0000686A File Offset: 0x00004A6A
		public int Order { get; set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060006EB RID: 1771 RVA: 0x00006873 File Offset: 0x00004A73
		// (set) Token: 0x060006EC RID: 1772 RVA: 0x0000687B File Offset: 0x00004A7B
		public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060006ED RID: 1773 RVA: 0x00006884 File Offset: 0x00004A84
		// (set) Token: 0x060006EE RID: 1774 RVA: 0x0000688C File Offset: 0x00004A8C
		public string Id { get; set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x00006895 File Offset: 0x00004A95
		// (set) Token: 0x060006F0 RID: 1776 RVA: 0x0000689D File Offset: 0x00004A9D
		public string ButtonText { get; set; } = string.Empty;

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060006F1 RID: 1777 RVA: 0x000068A6 File Offset: 0x00004AA6
		// (set) Token: 0x060006F2 RID: 1778 RVA: 0x000068AE File Offset: 0x00004AAE
		public string ImagePath { get; set; } = string.Empty;

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060006F3 RID: 1779 RVA: 0x000068B7 File Offset: 0x00004AB7
		// (set) Token: 0x060006F4 RID: 1780 RVA: 0x000068BF File Offset: 0x00004ABF
		public string ImageUrl { get; set; } = string.Empty;

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060006F5 RID: 1781 RVA: 0x000068C8 File Offset: 0x00004AC8
		// (set) Token: 0x060006F6 RID: 1782 RVA: 0x000068D0 File Offset: 0x00004AD0
		public string ThemeEnabled { get; set; } = string.Empty;

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060006F7 RID: 1783 RVA: 0x000068D9 File Offset: 0x00004AD9
		// (set) Token: 0x060006F8 RID: 1784 RVA: 0x000068E1 File Offset: 0x00004AE1
		public string ThemeName { get; set; } = string.Empty;

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060006F9 RID: 1785 RVA: 0x000068EA File Offset: 0x00004AEA
		// (set) Token: 0x060006FA RID: 1786 RVA: 0x000068F2 File Offset: 0x00004AF2
		public string PromoBtnClickStatusText { get; set; } = string.Empty;

		// Token: 0x060006FB RID: 1787 RVA: 0x00027050 File Offset: 0x00025250
		internal void DeleteFile()
		{
			try
			{
				File.Delete(this.ImagePath);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't delete bootpromo file: " + this.ImagePath);
				Logger.Error(ex.ToString());
			}
		}
	}
}
