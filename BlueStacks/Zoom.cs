using System;
using System.ComponentModel;
using System.Windows.Input;

// Token: 0x0200001E RID: 30
[Description("Independent")]
[Serializable]
public class Zoom : IMAction
{
	// Token: 0x17000110 RID: 272
	// (get) Token: 0x0600023C RID: 572 RVA: 0x000133EC File Offset: 0x000115EC
	// (set) Token: 0x0600023D RID: 573 RVA: 0x00013468 File Offset: 0x00011668
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	internal double X
	{
		get
		{
			if (this.mX1 == -1.0 && this.mX2 == -1.0)
			{
				this.mX = -1.0;
			}
			else if (this.Direction == Direction.Left || this.Direction == Direction.Right)
			{
				this.mX = this.mX1 + this.mRadius;
			}
			else
			{
				this.mX = this.mX1;
			}
			return this.mX;
		}
		set
		{
			this.mX = value;
			if (this.Direction == Direction.Right)
			{
				this.mX2 = Math.Round(this.mX + this.mRadius, 2);
				this.mX1 = Math.Round(this.mX - this.mRadius, 2);
				return;
			}
			if (this.Direction == Direction.Up)
			{
				this.mX1 = Math.Round(this.mX, 2);
				this.mX2 = this.X1;
			}
		}
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x0600023E RID: 574 RVA: 0x000134E0 File Offset: 0x000116E0
	// (set) Token: 0x0600023F RID: 575 RVA: 0x00013558 File Offset: 0x00011758
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	internal double Y
	{
		get
		{
			if (this.mY1 == -1.0 && this.mY2 == -1.0)
			{
				this.mY = -1.0;
			}
			else if (this.Direction == Direction.Up || this.Direction == Direction.Down)
			{
				this.mY = this.mY1 + this.mRadius;
			}
			else
			{
				this.mY = this.mY1;
			}
			return this.mY;
		}
		set
		{
			this.mY = value;
			if (this.Direction == Direction.Right)
			{
				this.mY1 = Math.Round(this.mY, 2);
				this.mY2 = this.Y1;
				return;
			}
			if (this.Direction == Direction.Up)
			{
				this.mY2 = Math.Round(this.mY + this.mRadius, 2);
				this.mY1 = Math.Round(this.mY - this.mRadius, 2);
			}
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000240 RID: 576 RVA: 0x000036F8 File Offset: 0x000018F8
	// (set) Token: 0x06000241 RID: 577 RVA: 0x00003700 File Offset: 0x00001900
	public double X1
	{
		get
		{
			return this.mX1;
		}
		set
		{
			this.mX1 = value;
			this.CheckDirection();
			if (this.Direction == Direction.Up || this.Direction == Direction.Down)
			{
				this.mX2 = this.X1;
			}
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000242 RID: 578 RVA: 0x0000372C File Offset: 0x0000192C
	// (set) Token: 0x06000243 RID: 579 RVA: 0x00003734 File Offset: 0x00001934
	public double Y1
	{
		get
		{
			return this.mY1;
		}
		set
		{
			this.mY1 = value;
			this.CheckDirection();
			if (this.Direction == Direction.Left || this.Direction == Direction.Right)
			{
				this.mY2 = this.Y1;
			}
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000244 RID: 580 RVA: 0x00003761 File Offset: 0x00001961
	// (set) Token: 0x06000245 RID: 581 RVA: 0x00003773 File Offset: 0x00001973
	[Category("Fields")]
	internal double Size
	{
		get
		{
			return this.Radius * 2.0;
		}
		set
		{
			this.Radius = value / 2.0;
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x06000246 RID: 582 RVA: 0x00003786 File Offset: 0x00001986
	// (set) Token: 0x06000247 RID: 583 RVA: 0x0000378E File Offset: 0x0000198E
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

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000248 RID: 584 RVA: 0x0000379D File Offset: 0x0000199D
	// (set) Token: 0x06000249 RID: 585 RVA: 0x000037A5 File Offset: 0x000019A5
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

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x0600024A RID: 586 RVA: 0x000037B4 File Offset: 0x000019B4
	// (set) Token: 0x0600024B RID: 587 RVA: 0x000135D0 File Offset: 0x000117D0
	[Description("IMAP_CanvasElementRadius")]
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
			if (this.Direction == Direction.Right)
			{
				this.mX2 = Math.Round(this.mX + this.mRadius, 2);
				this.mX1 = Math.Round(this.mX - this.mRadius, 2);
				this.mY1 = Math.Round(this.mY, 2);
				this.mY2 = this.Y1;
				return;
			}
			if (this.Direction == Direction.Up)
			{
				this.mY2 = Math.Round(this.mY + this.mRadius, 2);
				this.mY1 = Math.Round(this.mY - this.mRadius, 2);
				this.mX1 = Math.Round(this.mX, 2);
				this.mX2 = this.X1;
			}
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x0600024C RID: 588 RVA: 0x000037BC File Offset: 0x000019BC
	// (set) Token: 0x0600024D RID: 589 RVA: 0x000037C4 File Offset: 0x000019C4
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyIn
	{
		get
		{
			return this.mKeyIn;
		}
		set
		{
			this.mKeyIn = value;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x0600024E RID: 590 RVA: 0x000037CD File Offset: 0x000019CD
	// (set) Token: 0x0600024F RID: 591 RVA: 0x000037D5 File Offset: 0x000019D5
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyIn_alt1
	{
		get
		{
			return this.mKeyIn_1;
		}
		set
		{
			this.mKeyIn_1 = value;
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000250 RID: 592 RVA: 0x000037DE File Offset: 0x000019DE
	// (set) Token: 0x06000251 RID: 593 RVA: 0x000037E6 File Offset: 0x000019E6
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyOut
	{
		get
		{
			return this.mKeyOut;
		}
		set
		{
			this.mKeyOut = value;
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000252 RID: 594 RVA: 0x000037EF File Offset: 0x000019EF
	// (set) Token: 0x06000253 RID: 595 RVA: 0x000037F7 File Offset: 0x000019F7
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyOut_alt1
	{
		get
		{
			return this.mKeyOut_1;
		}
		set
		{
			this.mKeyOut_1 = value;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000254 RID: 596 RVA: 0x00003800 File Offset: 0x00001A00
	// (set) Token: 0x06000255 RID: 597 RVA: 0x00003808 File Offset: 0x00001A08
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyModifier
	{
		get
		{
			return this.mKeyModifier;
		}
		set
		{
			this.mKeyModifier = value;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000256 RID: 598 RVA: 0x00003811 File Offset: 0x00001A11
	// (set) Token: 0x06000257 RID: 599 RVA: 0x00003819 File Offset: 0x00001A19
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyModifier_alt1
	{
		get
		{
			return this.mKeyModifier_1;
		}
		set
		{
			this.mKeyModifier_1 = value;
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000258 RID: 600 RVA: 0x00003822 File Offset: 0x00001A22
	// (set) Token: 0x06000259 RID: 601 RVA: 0x0000382A File Offset: 0x00001A2A
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

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x0600025A RID: 602 RVA: 0x00003833 File Offset: 0x00001A33
	// (set) Token: 0x0600025B RID: 603 RVA: 0x0000383B File Offset: 0x00001A3B
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

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x0600025C RID: 604 RVA: 0x00003844 File Offset: 0x00001A44
	// (set) Token: 0x0600025D RID: 605 RVA: 0x0000384C File Offset: 0x00001A4C
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Acceleration
	{
		get
		{
			return this.mAcceleration;
		}
		set
		{
			this.mAcceleration = value;
		}
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x0600025E RID: 606 RVA: 0x00003855 File Offset: 0x00001A55
	// (set) Token: 0x0600025F RID: 607 RVA: 0x0000385D File Offset: 0x00001A5D
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Mode
	{
		get
		{
			return this.mMode;
		}
		set
		{
			this.mMode = value;
		}
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000260 RID: 608 RVA: 0x00003866 File Offset: 0x00001A66
	// (set) Token: 0x06000261 RID: 609 RVA: 0x0000386E File Offset: 0x00001A6E
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

	// Token: 0x06000262 RID: 610 RVA: 0x00013698 File Offset: 0x00011898
	private void CheckDirection()
	{
		if (this.X1 == this.X2)
		{
			this.Direction = Direction.Up;
			this.mRadius = Math.Round(Math.Abs(this.Y2 - this.Y1) / 2.0, 2);
			return;
		}
		if (this.Y1 == this.Y2)
		{
			this.Direction = Direction.Right;
			this.mRadius = Math.Round(Math.Abs(this.X2 - this.X1) / 2.0, 2);
		}
	}

	// Token: 0x040000FA RID: 250
	private double mX = -1.0;

	// Token: 0x040000FB RID: 251
	private double mY = -1.0;

	// Token: 0x040000FC RID: 252
	private double mX1 = -1.0;

	// Token: 0x040000FD RID: 253
	private double mY1 = -1.0;

	// Token: 0x040000FE RID: 254
	private double mX2 = -1.0;

	// Token: 0x040000FF RID: 255
	private double mY2 = -1.0;

	// Token: 0x04000100 RID: 256
	private double mRadius = 20.0;

	// Token: 0x04000101 RID: 257
	private string mKeyIn = IMAPKeys.GetStringForFile(Key.OemPlus);

	// Token: 0x04000102 RID: 258
	private string mKeyIn_1 = string.Empty;

	// Token: 0x04000103 RID: 259
	private string mKeyOut = IMAPKeys.GetStringForFile(Key.OemMinus);

	// Token: 0x04000104 RID: 260
	private string mKeyOut_1 = string.Empty;

	// Token: 0x04000105 RID: 261
	private string mKeyModifier;

	// Token: 0x04000106 RID: 262
	private string mKeyModifier_1;

	// Token: 0x04000107 RID: 263
	private double mSpeed = 1.0;

	// Token: 0x04000108 RID: 264
	private double mAmplitude = 20.0;

	// Token: 0x04000109 RID: 265
	private double mAcceleration = 1.0;

	// Token: 0x0400010A RID: 266
	private int mMode;

	// Token: 0x0400010B RID: 267
	private bool mOverride = true;
}
