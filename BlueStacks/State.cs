using System;
using System.ComponentModel;

// Token: 0x02000016 RID: 22
[Description("Independent")]
[Serializable]
public class State : IMAction
{
	// Token: 0x170000CC RID: 204
	// (get) Token: 0x060001AC RID: 428 RVA: 0x000030A7 File Offset: 0x000012A7
	// (set) Token: 0x060001AD RID: 429 RVA: 0x000030AF File Offset: 0x000012AF
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

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x060001AE RID: 430 RVA: 0x000030B8 File Offset: 0x000012B8
	// (set) Token: 0x060001AF RID: 431 RVA: 0x000030C0 File Offset: 0x000012C0
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

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x060001B0 RID: 432 RVA: 0x000030C9 File Offset: 0x000012C9
	// (set) Token: 0x060001B1 RID: 433 RVA: 0x000030D1 File Offset: 0x000012D1
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Name
	{
		get
		{
			return this.mName;
		}
		set
		{
			this.mName = value;
		}
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060001B2 RID: 434 RVA: 0x000030DA File Offset: 0x000012DA
	// (set) Token: 0x060001B3 RID: 435 RVA: 0x000030E2 File Offset: 0x000012E2
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

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060001B4 RID: 436 RVA: 0x000030EB File Offset: 0x000012EB
	// (set) Token: 0x060001B5 RID: 437 RVA: 0x000030F3 File Offset: 0x000012F3
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key_alt1
	{
		get
		{
			return this.mKey_alt1;
		}
		set
		{
			this.mKey_alt1 = value;
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060001B6 RID: 438 RVA: 0x000030FC File Offset: 0x000012FC
	// (set) Token: 0x060001B7 RID: 439 RVA: 0x00003104 File Offset: 0x00001304
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Model
	{
		get
		{
			return this.mModel;
		}
		set
		{
			this.mModel = value;
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000310D File Offset: 0x0000130D
	// (set) Token: 0x060001B9 RID: 441 RVA: 0x00003115 File Offset: 0x00001315
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Delay
	{
		get
		{
			return this.mDelay;
		}
		set
		{
			this.mDelay = value;
		}
	}

	// Token: 0x040000CC RID: 204
	private double mX = -1.0;

	// Token: 0x040000CD RID: 205
	private double mY = -1.0;

	// Token: 0x040000CE RID: 206
	private string mName = string.Empty;

	// Token: 0x040000CF RID: 207
	private string mKey;

	// Token: 0x040000D0 RID: 208
	private string mKey_alt1;

	// Token: 0x040000D1 RID: 209
	private string mModel = string.Empty;

	// Token: 0x040000D2 RID: 210
	private int mDelay;
}
