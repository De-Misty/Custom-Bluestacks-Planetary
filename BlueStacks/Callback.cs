using System;
using System.ComponentModel;

// Token: 0x02000004 RID: 4
[Description("Independent")]
[Serializable]
public class Callback : IMAction
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
	// (set) Token: 0x06000004 RID: 4 RVA: 0x00002088 File Offset: 0x00000288
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000005 RID: 5 RVA: 0x00002091 File Offset: 0x00000291
	// (set) Token: 0x06000006 RID: 6 RVA: 0x00002099 File Offset: 0x00000299
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000007 RID: 7 RVA: 0x000020A2 File Offset: 0x000002A2
	// (set) Token: 0x06000008 RID: 8 RVA: 0x000020AA File Offset: 0x000002AA
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Id { get; set; } = "";

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000009 RID: 9 RVA: 0x000020B3 File Offset: 0x000002B3
	// (set) Token: 0x0600000A RID: 10 RVA: 0x000020BB File Offset: 0x000002BB
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Action { get; set; } = "";

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600000B RID: 11 RVA: 0x000020C4 File Offset: 0x000002C4
	// (set) Token: 0x0600000C RID: 12 RVA: 0x000020CC File Offset: 0x000002CC
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

	// Token: 0x04000005 RID: 5
	internal bool mShowOnOverlay;
}
