using System;
using System.ComponentModel;
using BlueStacks.Common;

// Token: 0x0200001B RID: 27
[Description("Dependent")]
[Serializable]
public class Swipe : IMAction
{
	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000200 RID: 512 RVA: 0x000034F3 File Offset: 0x000016F3
	// (set) Token: 0x06000201 RID: 513 RVA: 0x00012FA0 File Offset: 0x000111A0
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X1
	{
		get
		{
			return this.mX1;
		}
		set
		{
			this.mX1 = value;
			if (this.Direction == Direction.Up || this.Direction == Direction.Down)
			{
				this.mX2 = this.X1;
				return;
			}
			if (this.Direction == Direction.Left)
			{
				this.mX2 = Math.Round(this.X1 - this.mRadius, 2);
				return;
			}
			if (this.Direction == Direction.Right)
			{
				this.mX2 = Math.Round(this.X1 + this.mRadius, 2);
			}
		}
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000202 RID: 514 RVA: 0x000034FB File Offset: 0x000016FB
	// (set) Token: 0x06000203 RID: 515 RVA: 0x00013018 File Offset: 0x00011218
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y1
	{
		get
		{
			return this.mY1;
		}
		set
		{
			this.mY1 = value;
			if (this.Direction == Direction.Left || this.Direction == Direction.Right)
			{
				this.mY2 = this.Y1;
				return;
			}
			if (this.Direction == Direction.Up)
			{
				this.mY2 = Math.Round(this.Y1 - this.mRadius, 2);
				return;
			}
			if (this.Direction == Direction.Down)
			{
				this.mY2 = Math.Round(this.Y1 + this.mRadius, 2);
			}
		}
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000204 RID: 516 RVA: 0x00003503 File Offset: 0x00001703
	// (set) Token: 0x06000205 RID: 517 RVA: 0x0000350B File Offset: 0x0000170B
	public double X2
	{
		get
		{
			return this.mX2;
		}
		set
		{
			this.mX2 = value;
			this.CheckDirection();
		}
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000206 RID: 518 RVA: 0x0000351A File Offset: 0x0000171A
	// (set) Token: 0x06000207 RID: 519 RVA: 0x00003522 File Offset: 0x00001722
	public double Y2
	{
		get
		{
			return this.mY2;
		}
		set
		{
			this.mY2 = value;
			this.CheckDirection();
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000208 RID: 520 RVA: 0x00003531 File Offset: 0x00001731
	// (set) Token: 0x06000209 RID: 521 RVA: 0x00013090 File Offset: 0x00011290
	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	internal double Radius
	{
		get
		{
			return this.mRadius;
		}
		set
		{
			this.mRadius = value;
			if (this.Direction == Direction.Left)
			{
				this.Y2 = this.Y1;
				this.X2 = Math.Round(this.X1 - value, 2);
				Logger.Debug("SWIPE_L: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
				return;
			}
			if (this.Direction == Direction.Right)
			{
				this.Y2 = this.Y1;
				this.X2 = Math.Round(this.X1 + value, 2);
				Logger.Debug("SWIPE_R: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
				return;
			}
			if (this.Direction == Direction.Up)
			{
				this.X2 = this.X1;
				this.Y2 = Math.Round(this.Y1 - value, 2);
				Logger.Debug("SWIPE_U: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
				return;
			}
			if (this.Direction == Direction.Down)
			{
				this.X2 = this.X1;
				this.Y2 = Math.Round(this.Y1 + value, 2);
				Logger.Debug("SWIPE_D: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
			}
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x0600020A RID: 522 RVA: 0x00003539 File Offset: 0x00001739
	// (set) Token: 0x0600020B RID: 523 RVA: 0x00003541 File Offset: 0x00001741
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

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x0600020C RID: 524 RVA: 0x0000354A File Offset: 0x0000174A
	// (set) Token: 0x0600020D RID: 525 RVA: 0x00003552 File Offset: 0x00001752
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool Hold
	{
		get
		{
			return this.mHold;
		}
		set
		{
			this.mHold = value;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x0600020E RID: 526 RVA: 0x0000355B File Offset: 0x0000175B
	// (set) Token: 0x0600020F RID: 527 RVA: 0x00003563 File Offset: 0x00001763
	[Description("IMAP_PopupUIElementNotCommon")]
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

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000210 RID: 528 RVA: 0x0000356C File Offset: 0x0000176C
	// (set) Token: 0x06000211 RID: 529 RVA: 0x00003574 File Offset: 0x00001774
	[Description("IMAP_PopupUIElementNotCommon")]
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

	// Token: 0x06000212 RID: 530 RVA: 0x0001320C File Offset: 0x0001140C
	private void CheckDirection()
	{
		if (this.X1 != this.X2)
		{
			if (this.Y1 == this.Y2)
			{
				if (this.X1 > this.X2)
				{
					this.Direction = Direction.Left;
					this.mRadius = Math.Round(this.X1 - this.X2, 2);
					return;
				}
				this.Direction = Direction.Right;
				this.mRadius = Math.Round(this.X2 - this.X1, 2);
			}
			return;
		}
		if (this.Y1 > this.Y2)
		{
			this.Direction = Direction.Up;
			this.mRadius = Math.Round(this.Y1 - this.Y2, 2);
			return;
		}
		this.Direction = Direction.Down;
		this.mRadius = Math.Round(this.Y2 - this.Y1, 2);
	}

	// Token: 0x040000DE RID: 222
	private double mX1 = -1.0;

	// Token: 0x040000DF RID: 223
	private double mY1 = -1.0;

	// Token: 0x040000E0 RID: 224
	private double mX2;

	// Token: 0x040000E1 RID: 225
	private double mY2;

	// Token: 0x040000E2 RID: 226
	private double mRadius = 10.0;

	// Token: 0x040000E3 RID: 227
	private double mSpeed = 200.0;

	// Token: 0x040000E4 RID: 228
	private bool mHold;

	// Token: 0x040000E5 RID: 229
	private string mKey;

	// Token: 0x040000E6 RID: 230
	private string mKey_1;
}
