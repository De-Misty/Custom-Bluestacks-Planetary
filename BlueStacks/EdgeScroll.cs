using System;
using System.ComponentModel;

// Token: 0x02000005 RID: 5
[Description("Independent")]
[Serializable]
public class EdgeScroll : IMAction
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600000E RID: 14 RVA: 0x00002111 File Offset: 0x00000311
	// (set) Token: 0x0600000F RID: 15 RVA: 0x00002119 File Offset: 0x00000319
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000010 RID: 16 RVA: 0x00002122 File Offset: 0x00000322
	// (set) Token: 0x06000011 RID: 17 RVA: 0x0000212A File Offset: 0x0000032A
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000012 RID: 18 RVA: 0x00002133 File Offset: 0x00000333
	// (set) Token: 0x06000013 RID: 19 RVA: 0x0000213B File Offset: 0x0000033B
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XVelocity { get; set; } = 100.0;

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000014 RID: 20 RVA: 0x00002144 File Offset: 0x00000344
	// (set) Token: 0x06000015 RID: 21 RVA: 0x0000214C File Offset: 0x0000034C
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double YVelocity { get; set; } = 100.0;

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000016 RID: 22 RVA: 0x00002155 File Offset: 0x00000355
	// (set) Token: 0x06000017 RID: 23 RVA: 0x0000215D File Offset: 0x0000035D
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XActiveMargin { get; set; } = 3.0;

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000018 RID: 24 RVA: 0x00002166 File Offset: 0x00000366
	// (set) Token: 0x06000019 RID: 25 RVA: 0x0000216E File Offset: 0x0000036E
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double YActiveMargin { get; set; } = 3.0;

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600001A RID: 26 RVA: 0x00002177 File Offset: 0x00000377
	// (set) Token: 0x0600001B RID: 27 RVA: 0x0000217F File Offset: 0x0000037F
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int ResetDelay { get; set; } = 150;

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600001C RID: 28 RVA: 0x00002188 File Offset: 0x00000388
	// (set) Token: 0x0600001D RID: 29 RVA: 0x00002190 File Offset: 0x00000390
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double SpeedUpFactor { get; set; } = 2.0;

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600001E RID: 30 RVA: 0x00002199 File Offset: 0x00000399
	// (set) Token: 0x0600001F RID: 31 RVA: 0x000021A1 File Offset: 0x000003A1
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int SpeedUpWaitTime { get; set; } = 200;

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000020 RID: 32 RVA: 0x000021AA File Offset: 0x000003AA
	// (set) Token: 0x06000021 RID: 33 RVA: 0x000021B2 File Offset: 0x000003B2
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool EdgeScrollEnabled { get; set; } = true;

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000022 RID: 34 RVA: 0x000021BB File Offset: 0x000003BB
	// (set) Token: 0x06000023 RID: 35 RVA: 0x000021C3 File Offset: 0x000003C3
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool ShowOnOverlay
	{
		get
		{
			return this.mShowOnOverlay;
		}
		set
		{
			this.mShowOnOverlay = value;
		}
	}

	// Token: 0x04000010 RID: 16
	internal bool mShowOnOverlay;
}
