using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200005F RID: 95
	[Serializable]
	public class GuidanceCategoryViewModel : ViewModelBase
	{
		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x000052F9 File Offset: 0x000034F9
		// (set) Token: 0x060004DF RID: 1247 RVA: 0x00005301 File Offset: 0x00003501
		public ObservableCollection<GuidanceViewModel> GuidanceViewModels
		{
			get
			{
				return this.sGuidanceViewModels;
			}
			set
			{
				base.SetProperty<ObservableCollection<GuidanceViewModel>>(ref this.sGuidanceViewModels, value, null);
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00005312 File Offset: 0x00003512
		// (set) Token: 0x060004E1 RID: 1249 RVA: 0x0000531A File Offset: 0x0000351A
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

		// Token: 0x04000290 RID: 656
		private ObservableCollection<GuidanceViewModel> sGuidanceViewModels = new ObservableCollection<GuidanceViewModel>();

		// Token: 0x04000291 RID: 657
		private string mCategory;
	}
}
