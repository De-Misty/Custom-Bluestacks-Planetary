using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200006B RID: 107
	[Serializable]
	public class GuidanceViewModel : ViewModelBase
	{
		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x000058FE File Offset: 0x00003AFE
		// (set) Token: 0x06000549 RID: 1353 RVA: 0x00005906 File Offset: 0x00003B06
		public Type PropertyType { get; set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x0600054A RID: 1354 RVA: 0x0000590F File Offset: 0x00003B0F
		// (set) Token: 0x0600054B RID: 1355 RVA: 0x00005917 File Offset: 0x00003B17
		public ObservableCollection<string> GuidanceTexts
		{
			get
			{
				return this.mGuidanceTexts;
			}
			set
			{
				base.SetProperty<ObservableCollection<string>>(ref this.mGuidanceTexts, value, null);
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x0600054C RID: 1356 RVA: 0x00005928 File Offset: 0x00003B28
		// (set) Token: 0x0600054D RID: 1357 RVA: 0x00005930 File Offset: 0x00003B30
		public ObservableCollection<string> GuidanceKeys
		{
			get
			{
				return this.mGuidanceKeys;
			}
			set
			{
				base.SetProperty<ObservableCollection<string>>(ref this.mGuidanceKeys, value, null);
			}
		}

		// Token: 0x040002BB RID: 699
		private ObservableCollection<string> mGuidanceTexts = new ObservableCollection<string>();

		// Token: 0x040002BC RID: 700
		private ObservableCollection<string> mGuidanceKeys = new ObservableCollection<string>();
	}
}
