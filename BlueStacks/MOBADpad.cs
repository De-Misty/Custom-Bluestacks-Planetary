using System;
using System.Collections.Generic;
using System.ComponentModel;

// Token: 0x0200000C RID: 12
[Description("Independent")]
[Serializable]
public class MOBADpad : IMAction
{
	// Token: 0x060000D4 RID: 212 RVA: 0x00012AE8 File Offset: 0x00010CE8
	public MOBADpad()
	{
		this.IsChildAction = true;
		base.Type = KeyActionType.MOBADpad;
		MOBADpad.sListMOBADpad.Add(this);
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00012B74 File Offset: 0x00010D74
	public MOBADpad(Dpad action)
	{
		this.IsChildAction = true;
		base.Type = KeyActionType.MOBADpad;
		MOBADpad.sListMOBADpad.Add(this);
		this.mDpad = action;
		this.ParentAction = action;
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060000D6 RID: 214 RVA: 0x00002885 File Offset: 0x00000A85
	// (set) Token: 0x060000D7 RID: 215 RVA: 0x0000288D File Offset: 0x00000A8D
	public double X { get; set; } = -1.0;

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060000D8 RID: 216 RVA: 0x00002896 File Offset: 0x00000A96
	// (set) Token: 0x060000D9 RID: 217 RVA: 0x0000289E File Offset: 0x00000A9E
	public double Y { get; set; } = -1.0;

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060000DA RID: 218 RVA: 0x000028A7 File Offset: 0x00000AA7
	// (set) Token: 0x060000DB RID: 219 RVA: 0x000028AF File Offset: 0x00000AAF
	[Description("IMAP_CanvasElementY")]
	public double OriginX { get; set; } = -1.0;

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060000DC RID: 220 RVA: 0x000028B8 File Offset: 0x00000AB8
	// (set) Token: 0x060000DD RID: 221 RVA: 0x000028C0 File Offset: 0x00000AC0
	[Description("IMAP_CanvasElementX")]
	public double OriginY { get; set; }

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060000DE RID: 222 RVA: 0x000028C9 File Offset: 0x00000AC9
	internal string KeyMove { get; } = "MouseRButton";

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060000DF RID: 223 RVA: 0x000028D1 File Offset: 0x00000AD1
	// (set) Token: 0x060000E0 RID: 224 RVA: 0x000028F0 File Offset: 0x00000AF0
	public double XRadius
	{
		get
		{
			if (this.mDpad == null)
			{
				return -1.0;
			}
			return this.mDpad.XRadius;
		}
		set
		{
			if (this.mDpad != null)
			{
				this.mDpad.XRadius = value;
			}
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060000E1 RID: 225 RVA: 0x00002906 File Offset: 0x00000B06
	// (set) Token: 0x060000E2 RID: 226 RVA: 0x00002925 File Offset: 0x00000B25
	public double DpadSpeed
	{
		get
		{
			if (this.mDpad == null)
			{
				return -1.0;
			}
			return this.mDpad.Speed;
		}
		set
		{
			if (this.mDpad != null)
			{
				this.mDpad.Speed = value;
			}
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060000E3 RID: 227 RVA: 0x0000293B File Offset: 0x00000B3B
	// (set) Token: 0x060000E4 RID: 228 RVA: 0x00002943 File Offset: 0x00000B43
	public double CharSpeed { get; set; } = 10.0;

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000294C File Offset: 0x00000B4C
	// (set) Token: 0x060000E6 RID: 230 RVA: 0x00002954 File Offset: 0x00000B54
	public string OriginXExpr { get; set; } = "";

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000295D File Offset: 0x00000B5D
	// (set) Token: 0x060000E8 RID: 232 RVA: 0x00002965 File Offset: 0x00000B65
	public string OriginYExpr { get; set; } = "";

	// Token: 0x04000067 RID: 103
	internal Dpad mDpad;

	// Token: 0x04000068 RID: 104
	internal static List<MOBADpad> sListMOBADpad = new List<MOBADpad>();
}
