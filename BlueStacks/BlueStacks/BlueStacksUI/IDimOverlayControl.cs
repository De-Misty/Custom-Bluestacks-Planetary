using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200013A RID: 314
	internal interface IDimOverlayControl
	{
		// Token: 0x06000C9B RID: 3227
		bool Close();

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000C9C RID: 3228
		// (set) Token: 0x06000C9D RID: 3229
		bool IsCloseOnOverLayClick { get; set; }

		// Token: 0x06000C9E RID: 3230
		bool Show();

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000C9F RID: 3231
		// (set) Token: 0x06000CA0 RID: 3232
		bool ShowControlInSeparateWindow { get; set; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000CA1 RID: 3233
		// (set) Token: 0x06000CA2 RID: 3234
		bool ShowTransparentWindow { get; set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000CA3 RID: 3235
		// (set) Token: 0x06000CA4 RID: 3236
		double Height { get; set; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000CA5 RID: 3237
		// (set) Token: 0x06000CA6 RID: 3238
		double Width { get; set; }
	}
}
