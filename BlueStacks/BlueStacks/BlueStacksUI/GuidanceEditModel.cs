using System;
using System.Collections.ObjectModel;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000064 RID: 100
	[Serializable]
	public abstract class GuidanceEditModel : ViewModelBase
	{
		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x000054A0 File Offset: 0x000036A0
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x000054A8 File Offset: 0x000036A8
		public string GuidanceText
		{
			get
			{
				return this.mGuidanceText;
			}
			set
			{
				base.SetProperty<string>(ref this.mGuidanceText, value, null);
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x000054B9 File Offset: 0x000036B9
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x000054C1 File Offset: 0x000036C1
		public bool IsEnabled
		{
			get
			{
				return this.mIsEnabled;
			}
			set
			{
				base.SetProperty<bool>(ref this.mIsEnabled, value, null);
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x000054D2 File Offset: 0x000036D2
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x000054DA File Offset: 0x000036DA
		public string OriginalGuidanceKey
		{
			get
			{
				return this.mOriginalGuidanceKey;
			}
			set
			{
				this.mOriginalGuidanceKey = value;
				this.GuidanceKey = this.mOriginalGuidanceKey;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x000054EF File Offset: 0x000036EF
		// (set) Token: 0x06000500 RID: 1280 RVA: 0x000054F7 File Offset: 0x000036F7
		public string GuidanceKey
		{
			get
			{
				return this.mGuidanceKey;
			}
			set
			{
				base.SetProperty<string>(ref this.mGuidanceKey, value, null);
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000501 RID: 1281 RVA: 0x00005508 File Offset: 0x00003708
		// (set) Token: 0x06000502 RID: 1282 RVA: 0x00005510 File Offset: 0x00003710
		public Type PropertyType
		{
			get
			{
				return this.mPropertyType;
			}
			set
			{
				base.SetProperty<Type>(ref this.mPropertyType, value, null);
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000503 RID: 1283 RVA: 0x00005521 File Offset: 0x00003721
		// (set) Token: 0x06000504 RID: 1284 RVA: 0x00005529 File Offset: 0x00003729
		public KeyActionType ActionType
		{
			get
			{
				return this.mActionType;
			}
			set
			{
				base.SetProperty<KeyActionType>(ref this.mActionType, value, null);
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000505 RID: 1285 RVA: 0x0000553A File Offset: 0x0000373A
		// (set) Token: 0x06000506 RID: 1286 RVA: 0x00005542 File Offset: 0x00003742
		public ObservableCollection<IMActionItem> IMActionItems
		{
			get
			{
				return this.mIMActionItems;
			}
			set
			{
				base.SetProperty<ObservableCollection<IMActionItem>>(ref this.mIMActionItems, value, null);
			}
		}

		// Token: 0x040002A0 RID: 672
		private string mGuidanceText;

		// Token: 0x040002A1 RID: 673
		private bool mIsEnabled = true;

		// Token: 0x040002A2 RID: 674
		private string mOriginalGuidanceKey;

		// Token: 0x040002A3 RID: 675
		private string mGuidanceKey;

		// Token: 0x040002A4 RID: 676
		private Type mPropertyType;

		// Token: 0x040002A5 RID: 677
		private KeyActionType mActionType;

		// Token: 0x040002A6 RID: 678
		private ObservableCollection<IMActionItem> mIMActionItems = new ObservableCollection<IMActionItem>();
	}
}
