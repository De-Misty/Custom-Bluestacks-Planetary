using System;
using System.ComponentModel;
using System.Windows.Input;

// Token: 0x0200001D RID: 29
[Description("Independent")]
[Serializable]
public class Tilt : IMAction
{
	// Token: 0x17000102 RID: 258
	// (get) Token: 0x0600021F RID: 543 RVA: 0x0000360A File Offset: 0x0000180A
	// (set) Token: 0x06000220 RID: 544 RVA: 0x00003612 File Offset: 0x00001812
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

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000221 RID: 545 RVA: 0x0000361B File Offset: 0x0000181B
	// (set) Token: 0x06000222 RID: 546 RVA: 0x00003623 File Offset: 0x00001823
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

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000223 RID: 547 RVA: 0x0000362C File Offset: 0x0000182C
	// (set) Token: 0x06000224 RID: 548 RVA: 0x00003634 File Offset: 0x00001834
	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Radius
	{
		get
		{
			return this.mRadius;
		}
		set
		{
			this.mRadius = value;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000225 RID: 549 RVA: 0x0000363D File Offset: 0x0000183D
	// (set) Token: 0x06000226 RID: 550 RVA: 0x00003645 File Offset: 0x00001845
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp
	{
		get
		{
			return this.mKeyUp;
		}
		set
		{
			this.mKeyUp = value;
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000227 RID: 551 RVA: 0x0000364E File Offset: 0x0000184E
	// (set) Token: 0x06000228 RID: 552 RVA: 0x00003656 File Offset: 0x00001856
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp_alt1
	{
		get
		{
			return this.mKeyUp_1;
		}
		set
		{
			this.mKeyUp_1 = value;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000229 RID: 553 RVA: 0x0000365F File Offset: 0x0000185F
	// (set) Token: 0x0600022A RID: 554 RVA: 0x00003667 File Offset: 0x00001867
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown
	{
		get
		{
			return this.mKeyDown;
		}
		set
		{
			this.mKeyDown = value;
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x0600022B RID: 555 RVA: 0x00003670 File Offset: 0x00001870
	// (set) Token: 0x0600022C RID: 556 RVA: 0x00003678 File Offset: 0x00001878
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown_alt1
	{
		get
		{
			return this.mKeyDown_1;
		}
		set
		{
			this.mKeyDown_1 = value;
		}
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x0600022D RID: 557 RVA: 0x00003681 File Offset: 0x00001881
	// (set) Token: 0x0600022E RID: 558 RVA: 0x00003689 File Offset: 0x00001889
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft
	{
		get
		{
			return this.mKeyLeft;
		}
		set
		{
			this.mKeyLeft = value;
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x0600022F RID: 559 RVA: 0x00003692 File Offset: 0x00001892
	// (set) Token: 0x06000230 RID: 560 RVA: 0x0000369A File Offset: 0x0000189A
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft_alt1
	{
		get
		{
			return this.mKeyLeft_1;
		}
		set
		{
			this.mKeyLeft_1 = value;
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000231 RID: 561 RVA: 0x000036A3 File Offset: 0x000018A3
	// (set) Token: 0x06000232 RID: 562 RVA: 0x000036AB File Offset: 0x000018AB
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight
	{
		get
		{
			return this.mKeyRight;
		}
		set
		{
			this.mKeyRight = value;
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000233 RID: 563 RVA: 0x000036B4 File Offset: 0x000018B4
	// (set) Token: 0x06000234 RID: 564 RVA: 0x000036BC File Offset: 0x000018BC
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight_alt1
	{
		get
		{
			return this.mKeyRight_1;
		}
		set
		{
			this.mKeyRight_1 = value;
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000235 RID: 565 RVA: 0x000036C5 File Offset: 0x000018C5
	// (set) Token: 0x06000236 RID: 566 RVA: 0x000036CD File Offset: 0x000018CD
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double MaxAngle
	{
		get
		{
			return this.mMaxAngle;
		}
		set
		{
			this.mMaxAngle = value;
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000237 RID: 567 RVA: 0x000036D6 File Offset: 0x000018D6
	// (set) Token: 0x06000238 RID: 568 RVA: 0x000036DE File Offset: 0x000018DE
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Speed
	{
		get
		{
			return this.mSpeed;
		}
		set
		{
			this.mSpeed = value;
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000239 RID: 569 RVA: 0x000036E7 File Offset: 0x000018E7
	// (set) Token: 0x0600023A RID: 570 RVA: 0x000036EF File Offset: 0x000018EF
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool AutoReset
	{
		get
		{
			return this.mAutoReset;
		}
		set
		{
			this.mAutoReset = value;
		}
	}

	// Token: 0x040000EC RID: 236
	private double mX = -1.0;

	// Token: 0x040000ED RID: 237
	private double mY = -1.0;

	// Token: 0x040000EE RID: 238
	private double mRadius = 10.0;

	// Token: 0x040000EF RID: 239
	private string mKeyUp = IMAPKeys.GetStringForFile(Key.Up);

	// Token: 0x040000F0 RID: 240
	private string mKeyUp_1 = string.Empty;

	// Token: 0x040000F1 RID: 241
	private string mKeyDown = IMAPKeys.GetStringForFile(Key.Down);

	// Token: 0x040000F2 RID: 242
	private string mKeyDown_1 = string.Empty;

	// Token: 0x040000F3 RID: 243
	private string mKeyLeft = IMAPKeys.GetStringForFile(Key.Left);

	// Token: 0x040000F4 RID: 244
	private string mKeyLeft_1 = string.Empty;

	// Token: 0x040000F5 RID: 245
	private string mKeyRight = IMAPKeys.GetStringForFile(Key.Right);

	// Token: 0x040000F6 RID: 246
	private string mKeyRight_1 = string.Empty;

	// Token: 0x040000F7 RID: 247
	private double mMaxAngle = 20.0;

	// Token: 0x040000F8 RID: 248
	private double mSpeed = 90.0;

	// Token: 0x040000F9 RID: 249
	private bool mAutoReset = true;
}
