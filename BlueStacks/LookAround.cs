using System;
using System.ComponentModel;

// Token: 0x02000017 RID: 23
[Description("SubElement")]
[Serializable]
internal class LookAround : IMAction
{
	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060001BB RID: 443 RVA: 0x0000315A File Offset: 0x0000135A
	// (set) Token: 0x060001BC RID: 444 RVA: 0x00003167 File Offset: 0x00001367
	public string Key
	{
		get
		{
			return this.mPan.KeyLookAround;
		}
		set
		{
			this.mPan.KeyLookAround = value;
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x060001BD RID: 445 RVA: 0x00003175 File Offset: 0x00001375
	// (set) Token: 0x060001BE RID: 446 RVA: 0x00003182 File Offset: 0x00001382
	[Description("IMAP_CanvasElementY")]
	public double X
	{
		get
		{
			return this.mPan.LookAroundX;
		}
		set
		{
			this.mPan.LookAroundX = value;
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060001BF RID: 447 RVA: 0x00003190 File Offset: 0x00001390
	// (set) Token: 0x060001C0 RID: 448 RVA: 0x0000319D File Offset: 0x0000139D
	[Description("IMAP_CanvasElementX")]
	public double Y
	{
		get
		{
			return this.mPan.LookAroundY;
		}
		set
		{
			this.mPan.LookAroundY = value;
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060001C1 RID: 449 RVA: 0x000031AB File Offset: 0x000013AB
	// (set) Token: 0x060001C2 RID: 450 RVA: 0x000031B8 File Offset: 0x000013B8
	public string LookAroundXExpr
	{
		get
		{
			return this.mPan.LookAroundXExpr;
		}
		set
		{
			this.mPan.LookAroundXExpr = value;
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060001C3 RID: 451 RVA: 0x000031C6 File Offset: 0x000013C6
	// (set) Token: 0x060001C4 RID: 452 RVA: 0x000031D3 File Offset: 0x000013D3
	public string LookAroundYExpr
	{
		get
		{
			return this.mPan.LookAroundYExpr;
		}
		set
		{
			this.mPan.LookAroundYExpr = value;
		}
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060001C5 RID: 453 RVA: 0x000031E1 File Offset: 0x000013E1
	// (set) Token: 0x060001C6 RID: 454 RVA: 0x000031EE File Offset: 0x000013EE
	public string LookAroundXOverlayOffset
	{
		get
		{
			return this.mPan.LookAroundXOverlayOffset;
		}
		set
		{
			this.mPan.LookAroundXOverlayOffset = value;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060001C7 RID: 455 RVA: 0x000031FC File Offset: 0x000013FC
	// (set) Token: 0x060001C8 RID: 456 RVA: 0x00003209 File Offset: 0x00001409
	public string LookAroundYOverlayOffset
	{
		get
		{
			return this.mPan.LookAroundYOverlayOffset;
		}
		set
		{
			this.mPan.LookAroundYOverlayOffset = value;
		}
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060001C9 RID: 457 RVA: 0x00003217 File Offset: 0x00001417
	// (set) Token: 0x060001CA RID: 458 RVA: 0x00003224 File Offset: 0x00001424
	public string LookAroundShowOnOverlayExpr
	{
		get
		{
			return this.mPan.LookAroundShowOnOverlayExpr;
		}
		set
		{
			this.mPan.LookAroundShowOnOverlayExpr = value;
		}
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00003232 File Offset: 0x00001432
	internal LookAround(Pan action)
	{
		this.IsChildAction = true;
		base.Type = KeyActionType.LookAround;
		this.mPan = action;
		this.ParentAction = action;
	}

	// Token: 0x040000D3 RID: 211
	private Pan mPan;
}
