using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000067 RID: 103
	[Serializable]
	public class IMActionItem : ViewModelBase
	{
		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0000559D File Offset: 0x0000379D
		// (set) Token: 0x0600050D RID: 1293 RVA: 0x000055A5 File Offset: 0x000037A5
		public string ActionItem
		{
			get
			{
				return this.mActionItem;
			}
			set
			{
				base.SetProperty<string>(ref this.mActionItem, value, null);
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x0600050E RID: 1294 RVA: 0x000055B6 File Offset: 0x000037B6
		// (set) Token: 0x0600050F RID: 1295 RVA: 0x000055BE File Offset: 0x000037BE
		public IMAction IMAction
		{
			get
			{
				return this.mIMAction;
			}
			set
			{
				base.SetProperty<IMAction>(ref this.mIMAction, value, null);
			}
		}

		// Token: 0x040002A8 RID: 680
		private string mActionItem;

		// Token: 0x040002A9 RID: 681
		private IMAction mIMAction;
	}
}
