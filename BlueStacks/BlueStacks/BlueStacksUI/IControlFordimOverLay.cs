using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000139 RID: 313
	internal interface IControlFordimOverLay
	{
		// Token: 0x06000C97 RID: 3223
		bool Close();

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000C98 RID: 3224
		// (set) Token: 0x06000C99 RID: 3225
		bool IsCloseOnOverLayClick { get; set; }

		// Token: 0x06000C9A RID: 3226
		bool Show();
	}
}
