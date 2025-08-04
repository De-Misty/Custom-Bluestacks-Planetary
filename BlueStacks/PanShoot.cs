using System;
using System.ComponentModel;

// Token: 0x02000019 RID: 25
[Description("SubElement")]
[Serializable]
internal class PanShoot : IMAction
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x060001DF RID: 479 RVA: 0x0000336F File Offset: 0x0000156F
	public string Key
	{
		get
		{
			return this.mPan.KeyAction;
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000337C File Offset: 0x0000157C
	// (set) Token: 0x060001E1 RID: 481 RVA: 0x00003389 File Offset: 0x00001589
	[Description("IMAP_CanvasElementY")]
	public double X
	{
		get
		{
			return this.mPan.LButtonX;
		}
		set
		{
			this.mPan.LButtonX = value;
		}
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x060001E2 RID: 482 RVA: 0x00003397 File Offset: 0x00001597
	// (set) Token: 0x060001E3 RID: 483 RVA: 0x000033A4 File Offset: 0x000015A4
	[Description("IMAP_CanvasElementX")]
	public double Y
	{
		get
		{
			return this.mPan.LButtonY;
		}
		set
		{
			this.mPan.LButtonY = value;
		}
	}

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x060001E4 RID: 484 RVA: 0x000033B2 File Offset: 0x000015B2
	// (set) Token: 0x060001E5 RID: 485 RVA: 0x000033BF File Offset: 0x000015BF
	public string LButtonXExpr
	{
		get
		{
			return this.mPan.LButtonXExpr;
		}
		set
		{
			this.mPan.LButtonXExpr = value;
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x060001E6 RID: 486 RVA: 0x000033CD File Offset: 0x000015CD
	// (set) Token: 0x060001E7 RID: 487 RVA: 0x000033DA File Offset: 0x000015DA
	public string LButtonYExpr
	{
		get
		{
			return this.mPan.LButtonYExpr;
		}
		set
		{
			this.mPan.LButtonYExpr = value;
		}
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x060001E8 RID: 488 RVA: 0x000033E8 File Offset: 0x000015E8
	// (set) Token: 0x060001E9 RID: 489 RVA: 0x000033F5 File Offset: 0x000015F5
	public string LButtonXOverlayOffset
	{
		get
		{
			return this.mPan.LButtonXOverlayOffset;
		}
		set
		{
			this.mPan.LButtonXOverlayOffset = value;
		}
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x060001EA RID: 490 RVA: 0x00003403 File Offset: 0x00001603
	// (set) Token: 0x060001EB RID: 491 RVA: 0x00003410 File Offset: 0x00001610
	public string LButtonYOverlayOffset
	{
		get
		{
			return this.mPan.LButtonYOverlayOffset;
		}
		set
		{
			this.mPan.LButtonYOverlayOffset = value;
		}
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x060001EC RID: 492 RVA: 0x0000341E File Offset: 0x0000161E
	// (set) Token: 0x060001ED RID: 493 RVA: 0x0000342B File Offset: 0x0000162B
	public string LButtonShowOnOverlayExpr
	{
		get
		{
			return this.mPan.LButtonShowOnOverlayExpr;
		}
		set
		{
			this.mPan.LButtonShowOnOverlayExpr = value;
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00003439 File Offset: 0x00001639
	internal PanShoot(Pan action)
	{
		this.IsChildAction = true;
		base.Type = KeyActionType.PanShoot;
		this.mPan = action;
		this.ParentAction = action;
	}

	// Token: 0x040000D5 RID: 213
	private Pan mPan;
}
