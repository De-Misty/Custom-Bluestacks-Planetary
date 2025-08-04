using System;
using System.ComponentModel;
using System.Windows.Input;

// Token: 0x02000006 RID: 6
[Description("Independent")]
[Serializable]
public class MouseZoom : IMAction
{
	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000025 RID: 37 RVA: 0x00011EF4 File Offset: 0x000100F4
	// (set) Token: 0x06000026 RID: 38 RVA: 0x00011F70 File Offset: 0x00010170
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

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000027 RID: 39 RVA: 0x00011FE8 File Offset: 0x000101E8
	// (set) Token: 0x06000028 RID: 40 RVA: 0x00012060 File Offset: 0x00010260
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

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000029 RID: 41 RVA: 0x000021CC File Offset: 0x000003CC
	// (set) Token: 0x0600002A RID: 42 RVA: 0x000021D4 File Offset: 0x000003D4
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

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600002B RID: 43 RVA: 0x00002200 File Offset: 0x00000400
	// (set) Token: 0x0600002C RID: 44 RVA: 0x00002208 File Offset: 0x00000408
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

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600002D RID: 45 RVA: 0x00002235 File Offset: 0x00000435
	// (set) Token: 0x0600002E RID: 46 RVA: 0x00002247 File Offset: 0x00000447
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

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600002F RID: 47 RVA: 0x0000225A File Offset: 0x0000045A
	// (set) Token: 0x06000030 RID: 48 RVA: 0x00002262 File Offset: 0x00000462
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

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000031 RID: 49 RVA: 0x00002271 File Offset: 0x00000471
	// (set) Token: 0x06000032 RID: 50 RVA: 0x00002279 File Offset: 0x00000479
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

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000033 RID: 51 RVA: 0x00002288 File Offset: 0x00000488
	// (set) Token: 0x06000034 RID: 52 RVA: 0x000120D8 File Offset: 0x000102D8
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

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000035 RID: 53 RVA: 0x00002290 File Offset: 0x00000490
	// (set) Token: 0x06000036 RID: 54 RVA: 0x00002298 File Offset: 0x00000498
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

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000037 RID: 55 RVA: 0x000022A1 File Offset: 0x000004A1
	// (set) Token: 0x06000038 RID: 56 RVA: 0x000022A9 File Offset: 0x000004A9
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

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000039 RID: 57 RVA: 0x000022B2 File Offset: 0x000004B2
	// (set) Token: 0x0600003A RID: 58 RVA: 0x000022BA File Offset: 0x000004BA
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

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600003B RID: 59 RVA: 0x000022C3 File Offset: 0x000004C3
	// (set) Token: 0x0600003C RID: 60 RVA: 0x000022CB File Offset: 0x000004CB
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

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x0600003D RID: 61 RVA: 0x000022D4 File Offset: 0x000004D4
	// (set) Token: 0x0600003E RID: 62 RVA: 0x000022DC File Offset: 0x000004DC
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

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600003F RID: 63 RVA: 0x000022E5 File Offset: 0x000004E5
	// (set) Token: 0x06000040 RID: 64 RVA: 0x000022ED File Offset: 0x000004ED
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

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000041 RID: 65 RVA: 0x000022F6 File Offset: 0x000004F6
	// (set) Token: 0x06000042 RID: 66 RVA: 0x000022FE File Offset: 0x000004FE
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

	// Token: 0x06000043 RID: 67 RVA: 0x000121A0 File Offset: 0x000103A0
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

	// Token: 0x04000011 RID: 17
	private double mX = -1.0;

	// Token: 0x04000012 RID: 18
	private double mY = -1.0;

	// Token: 0x04000013 RID: 19
	private double mX1 = -1.0;

	// Token: 0x04000014 RID: 20
	private double mY1 = -1.0;

	// Token: 0x04000015 RID: 21
	private double mX2 = -1.0;

	// Token: 0x04000016 RID: 22
	private double mY2 = -1.0;

	// Token: 0x04000017 RID: 23
	private double mRadius = 20.0;

	// Token: 0x04000018 RID: 24
	private string mKey;

	// Token: 0x04000019 RID: 25
	private string mKey_1 = string.Empty;

	// Token: 0x0400001A RID: 26
	private string mKeyModifier = IMAPKeys.GetStringForFile(global::System.Windows.Input.Key.LeftCtrl);

	// Token: 0x0400001B RID: 27
	private string mKeyModifier_1;

	// Token: 0x0400001C RID: 28
	private double mSpeed = 40.0;

	// Token: 0x0400001D RID: 29
	private double mAmplitude = 25.0;

	// Token: 0x0400001E RID: 30
	private bool mOverride = true;
}
