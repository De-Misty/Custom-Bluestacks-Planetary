using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000065 RID: 101
	[Serializable]
	public class GuidanceEditTextModel : GuidanceEditModel
	{
		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x0000556D File Offset: 0x0000376D
		// (set) Token: 0x06000509 RID: 1289 RVA: 0x00005575 File Offset: 0x00003775
		public TextValidityOptions TextValidityOption
		{
			get
			{
				return this.mTextValidityOption;
			}
			set
			{
				base.SetProperty<TextValidityOptions>(ref this.mTextValidityOption, value, null);
			}
		}

		// Token: 0x040002A7 RID: 679
		private TextValidityOptions mTextValidityOption = TextValidityOptions.Success;
	}
}
