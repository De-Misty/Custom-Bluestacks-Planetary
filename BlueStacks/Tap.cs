using System;
using System.ComponentModel;

// Token: 0x0200001C RID: 28
[Description("Independent")]
[Serializable]
public class Tap : IMAction
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000214 RID: 532 RVA: 0x0000357D File Offset: 0x0000177D
	// (set) Token: 0x06000215 RID: 533 RVA: 0x00003585 File Offset: 0x00001785
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X
	{
		get
		{
			return this.mX;
		}
		set
		{
			this.mX = value;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000216 RID: 534 RVA: 0x0000358E File Offset: 0x0000178E
	// (set) Token: 0x06000217 RID: 535 RVA: 0x00003596 File Offset: 0x00001796
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y
	{
		get
		{
			return this.mY;
		}
		set
		{
			this.mY = value;
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000218 RID: 536 RVA: 0x0000359F File Offset: 0x0000179F
	// (set) Token: 0x06000219 RID: 537 RVA: 0x000035A7 File Offset: 0x000017A7
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key
	{
		get
		{
			return this.mKey;
		}
		set
		{
			this.mKey = value;
		}
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x0600021A RID: 538 RVA: 0x000035B0 File Offset: 0x000017B0
	// (set) Token: 0x0600021B RID: 539 RVA: 0x000035B8 File Offset: 0x000017B8
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key_alt1
	{
		get
		{
			return this.mKey_1;
		}
		set
		{
			this.mKey_1 = value;
		}
	}

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x0600021C RID: 540 RVA: 0x000035C1 File Offset: 0x000017C1
	// (set) Token: 0x0600021D RID: 541 RVA: 0x000035C9 File Offset: 0x000017C9
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

	// Token: 0x040000E7 RID: 231
	private double mX = -1.0;

	// Token: 0x040000E8 RID: 232
	private double mY = -1.0;

	// Token: 0x040000E9 RID: 233
	private string mKey;

	// Token: 0x040000EA RID: 234
	private string mKey_1 = string.Empty;

	// Token: 0x040000EB RID: 235
	internal bool mShowOnOverlay = true;
}
