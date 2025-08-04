using System;
using System.Collections.Generic;
using System.ComponentModel;

// Token: 0x02000015 RID: 21
[Description("Independent")]
[Serializable]
public class Script : IMAction
{
	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000304A File Offset: 0x0000124A
	// (set) Token: 0x060001A1 RID: 417 RVA: 0x00003052 File Offset: 0x00001252
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

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060001A2 RID: 418 RVA: 0x0000305B File Offset: 0x0000125B
	// (set) Token: 0x060001A3 RID: 419 RVA: 0x00003063 File Offset: 0x00001263
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

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000306C File Offset: 0x0000126C
	// (set) Token: 0x060001A5 RID: 421 RVA: 0x00003074 File Offset: 0x00001274
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

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000307D File Offset: 0x0000127D
	// (set) Token: 0x060001A7 RID: 423 RVA: 0x00003085 File Offset: 0x00001285
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

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000308E File Offset: 0x0000128E
	// (set) Token: 0x060001A9 RID: 425 RVA: 0x00003096 File Offset: 0x00001296
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

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x060001AA RID: 426 RVA: 0x0000309F File Offset: 0x0000129F
	public List<string> Commands { get; } = new List<string>();

	// Token: 0x040000C6 RID: 198
	private double mX = -1.0;

	// Token: 0x040000C7 RID: 199
	private double mY = -1.0;

	// Token: 0x040000C8 RID: 200
	private string mKey;

	// Token: 0x040000C9 RID: 201
	private string mKey_1 = string.Empty;

	// Token: 0x040000CA RID: 202
	internal bool mShowOnOverlay = true;
}
