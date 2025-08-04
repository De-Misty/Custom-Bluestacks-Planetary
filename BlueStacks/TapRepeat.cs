using System;
using System.ComponentModel;

// Token: 0x0200001A RID: 26
[Description("Independent")]
[Serializable]
public class TapRepeat : IMAction
{
	// Token: 0x170000EC RID: 236
	// (get) Token: 0x060001EF RID: 495 RVA: 0x0000345E File Offset: 0x0000165E
	// (set) Token: 0x060001F0 RID: 496 RVA: 0x00003466 File Offset: 0x00001666
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

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000346F File Offset: 0x0000166F
	// (set) Token: 0x060001F2 RID: 498 RVA: 0x00003477 File Offset: 0x00001677
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

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x060001F3 RID: 499 RVA: 0x00003480 File Offset: 0x00001680
	// (set) Token: 0x060001F4 RID: 500 RVA: 0x00003488 File Offset: 0x00001688
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

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x060001F5 RID: 501 RVA: 0x00003491 File Offset: 0x00001691
	// (set) Token: 0x060001F6 RID: 502 RVA: 0x00003499 File Offset: 0x00001699
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

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060001F7 RID: 503 RVA: 0x000034A2 File Offset: 0x000016A2
	// (set) Token: 0x060001F8 RID: 504 RVA: 0x000034AA File Offset: 0x000016AA
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Count
	{
		get
		{
			return this.mCount;
		}
		set
		{
			this.mCount = value;
		}
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060001F9 RID: 505 RVA: 0x000034B3 File Offset: 0x000016B3
	// (set) Token: 0x060001FA RID: 506 RVA: 0x000034BB File Offset: 0x000016BB
	public int Delay
	{
		get
		{
			return this.mDelay;
		}
		set
		{
			this.mDelay = 1000 / (2 * this.Count);
		}
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060001FB RID: 507 RVA: 0x000034D1 File Offset: 0x000016D1
	// (set) Token: 0x060001FC RID: 508 RVA: 0x000034D9 File Offset: 0x000016D9
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

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060001FD RID: 509 RVA: 0x000034E2 File Offset: 0x000016E2
	// (set) Token: 0x060001FE RID: 510 RVA: 0x000034EA File Offset: 0x000016EA
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool RepeatUntilKeyUp
	{
		get
		{
			return this.mRepeatUntilKeyUp;
		}
		set
		{
			this.mRepeatUntilKeyUp = value;
		}
	}

	// Token: 0x040000D6 RID: 214
	private double mX = -1.0;

	// Token: 0x040000D7 RID: 215
	private double mY = -1.0;

	// Token: 0x040000D8 RID: 216
	private string mKey;

	// Token: 0x040000D9 RID: 217
	private string mKey_alt1;

	// Token: 0x040000DA RID: 218
	private int mCount = 5;

	// Token: 0x040000DB RID: 219
	private int mDelay = 100;

	// Token: 0x040000DC RID: 220
	internal bool mShowOnOverlay = true;

	// Token: 0x040000DD RID: 221
	private bool mRepeatUntilKeyUp = true;
}
