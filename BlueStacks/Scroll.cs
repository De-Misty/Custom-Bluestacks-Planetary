using System;
using System.ComponentModel;

// Token: 0x02000007 RID: 7
[Description("Independent")]
[Serializable]
public class Scroll : IMAction
{
	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000045 RID: 69 RVA: 0x00002307 File Offset: 0x00000507
	// (set) Token: 0x06000046 RID: 70 RVA: 0x0000230F File Offset: 0x0000050F
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

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000047 RID: 71 RVA: 0x00002318 File Offset: 0x00000518
	// (set) Token: 0x06000048 RID: 72 RVA: 0x00002320 File Offset: 0x00000520
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

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000049 RID: 73 RVA: 0x00002329 File Offset: 0x00000529
	// (set) Token: 0x0600004A RID: 74 RVA: 0x00002331 File Offset: 0x00000531
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

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x0600004B RID: 75 RVA: 0x0000233A File Offset: 0x0000053A
	// (set) Token: 0x0600004C RID: 76 RVA: 0x00002342 File Offset: 0x00000542
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Amplitude
	{
		get
		{
			return this.mAmplitude;
		}
		set
		{
			this.mAmplitude = value;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x0600004D RID: 77 RVA: 0x0000234B File Offset: 0x0000054B
	// (set) Token: 0x0600004E RID: 78 RVA: 0x00002353 File Offset: 0x00000553
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool Override
	{
		get
		{
			return this.mOverride;
		}
		set
		{
			this.mOverride = value;
		}
	}

	// Token: 0x0400001F RID: 31
	private double mX = -1.0;

	// Token: 0x04000020 RID: 32
	private double mY = -1.0;

	// Token: 0x04000021 RID: 33
	private double mSpeed = 100.0;

	// Token: 0x04000022 RID: 34
	private double mAmplitude = 20.0;

	// Token: 0x04000023 RID: 35
	private bool mOverride;
}
