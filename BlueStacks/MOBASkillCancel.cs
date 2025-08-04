using System;
using System.ComponentModel;

// Token: 0x02000018 RID: 24
[Description("SubElementDependent")]
[Serializable]
internal class MOBASkillCancel : IMAction
{
	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060001CC RID: 460 RVA: 0x00003257 File Offset: 0x00001457
	// (set) Token: 0x060001CD RID: 461 RVA: 0x00003264 File Offset: 0x00001464
	public string Key
	{
		get
		{
			return this.mMOBASkill.KeyCancel;
		}
		set
		{
			this.mMOBASkill.KeyCancel = value;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060001CE RID: 462 RVA: 0x00003272 File Offset: 0x00001472
	// (set) Token: 0x060001CF RID: 463 RVA: 0x0000327F File Offset: 0x0000147F
	public string Key_alt1
	{
		get
		{
			return this.mMOBASkill.KeyCancel_alt1;
		}
		set
		{
			this.mMOBASkill.KeyCancel_alt1 = value;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000328D File Offset: 0x0000148D
	// (set) Token: 0x060001D1 RID: 465 RVA: 0x0000329A File Offset: 0x0000149A
	[Description("IMAP_CanvasElementY")]
	public double X
	{
		get
		{
			return this.mMOBASkill.CancelX;
		}
		set
		{
			this.mMOBASkill.CancelX = value;
		}
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x060001D2 RID: 466 RVA: 0x000032A8 File Offset: 0x000014A8
	// (set) Token: 0x060001D3 RID: 467 RVA: 0x000032B5 File Offset: 0x000014B5
	[Description("IMAP_CanvasElementX")]
	public double Y
	{
		get
		{
			return this.mMOBASkill.CancelY;
		}
		set
		{
			this.mMOBASkill.CancelY = value;
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x060001D4 RID: 468 RVA: 0x000032C3 File Offset: 0x000014C3
	// (set) Token: 0x060001D5 RID: 469 RVA: 0x000032D0 File Offset: 0x000014D0
	public string MOBASkillCancelXExpr
	{
		get
		{
			return this.mMOBASkill.CancelXExpr;
		}
		set
		{
			this.mMOBASkill.CancelXExpr = value;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x060001D6 RID: 470 RVA: 0x000032DE File Offset: 0x000014DE
	// (set) Token: 0x060001D7 RID: 471 RVA: 0x000032EB File Offset: 0x000014EB
	public string MOBASkillCancelYExpr
	{
		get
		{
			return this.mMOBASkill.CancelYExpr;
		}
		set
		{
			this.mMOBASkill.CancelYExpr = value;
		}
	}

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x060001D8 RID: 472 RVA: 0x000032F9 File Offset: 0x000014F9
	// (set) Token: 0x060001D9 RID: 473 RVA: 0x00003306 File Offset: 0x00001506
	public string MOBASkillCancelOffsetX
	{
		get
		{
			return this.mMOBASkill.CancelXOverlayOffset;
		}
		set
		{
			this.mMOBASkill.CancelXOverlayOffset = value;
		}
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x060001DA RID: 474 RVA: 0x00003314 File Offset: 0x00001514
	// (set) Token: 0x060001DB RID: 475 RVA: 0x00003321 File Offset: 0x00001521
	public string MOBASkillCancelOffsetY
	{
		get
		{
			return this.mMOBASkill.CancelYOverlayOffset;
		}
		set
		{
			this.mMOBASkill.CancelYOverlayOffset = value;
		}
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x060001DC RID: 476 RVA: 0x0000332F File Offset: 0x0000152F
	// (set) Token: 0x060001DD RID: 477 RVA: 0x0000333C File Offset: 0x0000153C
	public string MOBASkillShowOnOverlayExpr
	{
		get
		{
			return this.mMOBASkill.CancelShowOnOverlayExpr;
		}
		set
		{
			this.mMOBASkill.CancelShowOnOverlayExpr = value;
		}
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000334A File Offset: 0x0000154A
	internal MOBASkillCancel(MOBASkill action)
	{
		this.IsChildAction = true;
		base.Type = KeyActionType.MOBASkillCancel;
		this.mMOBASkill = action;
		this.ParentAction = action;
	}

	// Token: 0x040000D4 RID: 212
	internal MOBASkill mMOBASkill;
}
