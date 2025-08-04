using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200005E RID: 94
	[Serializable]
	public class GuidanceCategoryEditModel : ViewModelBase
	{
		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x000052B4 File Offset: 0x000034B4
		// (set) Token: 0x060004DA RID: 1242 RVA: 0x000052BC File Offset: 0x000034BC
		public ObservableCollection<GuidanceEditModel> GuidanceEditModels
		{
			get
			{
				return this.mGuidanceEditModels;
			}
			set
			{
				base.SetProperty<ObservableCollection<GuidanceEditModel>>(ref this.mGuidanceEditModels, value, null);
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x000052CD File Offset: 0x000034CD
		// (set) Token: 0x060004DC RID: 1244 RVA: 0x000052D5 File Offset: 0x000034D5
		public string Category
		{
			get
			{
				return this.mCategory;
			}
			set
			{
				base.SetProperty<string>(ref this.mCategory, value, null);
			}
		}

		// Token: 0x0400028E RID: 654
		private ObservableCollection<GuidanceEditModel> mGuidanceEditModels = new ObservableCollection<GuidanceEditModel>();

		// Token: 0x0400028F RID: 655
		private string mCategory;
	}
}
