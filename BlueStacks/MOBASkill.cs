using System;
using System.ComponentModel;
using System.Windows.Input;

// Token: 0x0200000D RID: 13
[Description("ParentElement")]
[Serializable]
public class MOBASkill : IMAction
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060000EA RID: 234 RVA: 0x00012C0C File Offset: 0x00010E0C
	internal static MOBADpad MOBADpad
	{
		get
		{
			foreach (MOBADpad mobadpad in MOBADpad.sListMOBADpad)
			{
				if (mobadpad.OriginX != -1.0 && mobadpad.OriginY != -1.0)
				{
					return mobadpad;
				}
			}
			return null;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060000EB RID: 235 RVA: 0x0000297A File Offset: 0x00000B7A
	// (set) Token: 0x060000EC RID: 236 RVA: 0x00002982 File Offset: 0x00000B82
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060000ED RID: 237 RVA: 0x0000298B File Offset: 0x00000B8B
	// (set) Token: 0x060000EE RID: 238 RVA: 0x00002993 File Offset: 0x00000B93
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060000EF RID: 239 RVA: 0x0000299C File Offset: 0x00000B9C
	// (set) Token: 0x060000F0 RID: 240 RVA: 0x000029A4 File Offset: 0x00000BA4
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyActivate { get; set; }

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060000F1 RID: 241 RVA: 0x000029AD File Offset: 0x00000BAD
	// (set) Token: 0x060000F2 RID: 242 RVA: 0x000029B5 File Offset: 0x00000BB5
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyActivate_alt1 { get; set; }

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060000F3 RID: 243 RVA: 0x000029BE File Offset: 0x00000BBE
	// (set) Token: 0x060000F4 RID: 244 RVA: 0x000029C6 File Offset: 0x00000BC6
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string KeyAutocastToggle { get; set; }

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060000F5 RID: 245 RVA: 0x000029CF File Offset: 0x00000BCF
	// (set) Token: 0x060000F6 RID: 246 RVA: 0x000029D7 File Offset: 0x00000BD7
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string KeyAutocastToggle_alt1 { get; set; }

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060000F7 RID: 247 RVA: 0x000029E0 File Offset: 0x00000BE0
	// (set) Token: 0x060000F8 RID: 248 RVA: 0x000029E8 File Offset: 0x00000BE8
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double YAxisRatio { get; set; }

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060000F9 RID: 249 RVA: 0x000029F1 File Offset: 0x00000BF1
	// (set) Token: 0x060000FA RID: 250 RVA: 0x000029F9 File Offset: 0x00000BF9
	public string KeyCancel { get; set; } = IMAPKeys.GetStringForFile(Key.Space);

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060000FB RID: 251 RVA: 0x00002A02 File Offset: 0x00000C02
	// (set) Token: 0x060000FC RID: 252 RVA: 0x00002A0A File Offset: 0x00000C0A
	public string KeyCancel_alt1 { get; set; } = string.Empty;

	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060000FD RID: 253 RVA: 0x00002A13 File Offset: 0x00000C13
	// (set) Token: 0x060000FE RID: 254 RVA: 0x00002A1B File Offset: 0x00000C1B
	public double CancelX
	{
		get
		{
			return this.mCancelX;
		}
		set
		{
			this.mCancelX = value;
			this.CheckSkillCancel();
		}
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060000FF RID: 255 RVA: 0x00002A2A File Offset: 0x00000C2A
	// (set) Token: 0x06000100 RID: 256 RVA: 0x00002A32 File Offset: 0x00000C32
	public double CancelY
	{
		get
		{
			return this.mCancelY;
		}
		set
		{
			this.mCancelY = value;
			this.CheckSkillCancel();
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000101 RID: 257 RVA: 0x00002A41 File Offset: 0x00000C41
	// (set) Token: 0x06000102 RID: 258 RVA: 0x00002A5E File Offset: 0x00000C5E
	public static double OriginX
	{
		get
		{
			if (MOBASkill.MOBADpad == null)
			{
				return -1.0;
			}
			return MOBASkill.MOBADpad.OriginX;
		}
		set
		{
			if (MOBASkill.MOBADpad != null)
			{
				MOBASkill.MOBADpad.OriginX = value;
			}
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000103 RID: 259 RVA: 0x00002A72 File Offset: 0x00000C72
	// (set) Token: 0x06000104 RID: 260 RVA: 0x00002A8F File Offset: 0x00000C8F
	public static double OriginY
	{
		get
		{
			if (MOBASkill.MOBADpad == null)
			{
				return -1.0;
			}
			return MOBASkill.MOBADpad.OriginY;
		}
		set
		{
			if (MOBASkill.MOBADpad != null)
			{
				MOBASkill.MOBADpad.OriginY = value;
			}
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000105 RID: 261 RVA: 0x00002AA3 File Offset: 0x00000CA3
	// (set) Token: 0x06000106 RID: 262 RVA: 0x00002AAB File Offset: 0x00000CAB
	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius { get; set; } = 5.0;

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000107 RID: 263 RVA: 0x00002AB4 File Offset: 0x00000CB4
	// (set) Token: 0x06000108 RID: 264 RVA: 0x00002ABC File Offset: 0x00000CBC
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double DeadZoneRadius { get; set; }

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000109 RID: 265 RVA: 0x00002AC5 File Offset: 0x00000CC5
	// (set) Token: 0x0600010A RID: 266 RVA: 0x00002ACD File Offset: 0x00000CCD
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double CancelSpeed { get; set; } = 500.0;

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x0600010B RID: 267 RVA: 0x00002AD6 File Offset: 0x00000CD6
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsCancelSkillEnabled
	{
		get
		{
			return this.mMOBASkillCancel != null;
		}
	}

	// Token: 0x0600010C RID: 268 RVA: 0x00002AE1 File Offset: 0x00000CE1
	private void CheckSkillCancel()
	{
		if (this.mCancelX == -1.0 && this.mCancelY == -1.0)
		{
			this.mMOBASkillCancel = null;
			return;
		}
		if (this.mMOBASkillCancel == null)
		{
			this.mMOBASkillCancel = new MOBASkillCancel(this);
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x0600010D RID: 269 RVA: 0x00002B21 File Offset: 0x00000D21
	// (set) Token: 0x0600010E RID: 270 RVA: 0x00002B29 File Offset: 0x00000D29
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

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x0600010F RID: 271 RVA: 0x00002B32 File Offset: 0x00000D32
	// (set) Token: 0x06000110 RID: 272 RVA: 0x00002B3A File Offset: 0x00000D3A
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int NoCancelOnSwitch { get; set; }

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000111 RID: 273 RVA: 0x00002B43 File Offset: 0x00000D43
	// (set) Token: 0x06000112 RID: 274 RVA: 0x00002B4B File Offset: 0x00000D4B
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int NoCancelTime { get; set; }

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000113 RID: 275 RVA: 0x00002B54 File Offset: 0x00000D54
	// (set) Token: 0x06000114 RID: 276 RVA: 0x00002B5C File Offset: 0x00000D5C
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool AutoAttack { get; set; }

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000115 RID: 277 RVA: 0x00002B65 File Offset: 0x00000D65
	// (set) Token: 0x06000116 RID: 278 RVA: 0x00002B6D File Offset: 0x00000D6D
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool StopMOBADpad { get; set; }

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000117 RID: 279 RVA: 0x00002B76 File Offset: 0x00000D76
	// (set) Token: 0x06000118 RID: 280 RVA: 0x00002B7E File Offset: 0x00000D7E
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool AdvancedMode { get; set; } = true;

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000119 RID: 281 RVA: 0x00002B87 File Offset: 0x00000D87
	// (set) Token: 0x0600011A RID: 282 RVA: 0x00002B8F File Offset: 0x00000D8F
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public bool AutocastEnabled { get; set; } = true;

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x0600011B RID: 283 RVA: 0x00002B98 File Offset: 0x00000D98
	// (set) Token: 0x0600011C RID: 284 RVA: 0x00002BA0 File Offset: 0x00000DA0
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int MinSkillTime { get; set; }

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x0600011D RID: 285 RVA: 0x00002BA9 File Offset: 0x00000DA9
	// (set) Token: 0x0600011E RID: 286 RVA: 0x00002BB1 File Offset: 0x00000DB1
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double MinSwipeRadius { get; set; }

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600011F RID: 287 RVA: 0x00002BBA File Offset: 0x00000DBA
	// (set) Token: 0x06000120 RID: 288 RVA: 0x00002BC2 File Offset: 0x00000DC2
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public int MinSkillHoldTime { get; set; }

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000121 RID: 289 RVA: 0x00002BCB File Offset: 0x00000DCB
	// (set) Token: 0x06000122 RID: 290 RVA: 0x00002BD3 File Offset: 0x00000DD3
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public double Speed { get; set; } = 200.0;

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000123 RID: 291 RVA: 0x00002BDC File Offset: 0x00000DDC
	// (set) Token: 0x06000124 RID: 292 RVA: 0x00002BE4 File Offset: 0x00000DE4
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string OriginXExpr { get; set; } = "";

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000125 RID: 293 RVA: 0x00002BED File Offset: 0x00000DED
	// (set) Token: 0x06000126 RID: 294 RVA: 0x00002BF5 File Offset: 0x00000DF5
	[Description("IMAP_DeveloperModeUIElemnt")]
	[Category("Fields")]
	public string OriginYExpr { get; set; } = "";

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000127 RID: 295 RVA: 0x00002BFE File Offset: 0x00000DFE
	// (set) Token: 0x06000128 RID: 296 RVA: 0x00002C06 File Offset: 0x00000E06
	public string CancelXExpr { get; set; } = "";

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000129 RID: 297 RVA: 0x00002C0F File Offset: 0x00000E0F
	// (set) Token: 0x0600012A RID: 298 RVA: 0x00002C17 File Offset: 0x00000E17
	public string CancelYExpr { get; set; } = "";

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x0600012B RID: 299 RVA: 0x00002C20 File Offset: 0x00000E20
	// (set) Token: 0x0600012C RID: 300 RVA: 0x00002C28 File Offset: 0x00000E28
	public string CancelXOverlayOffset { get; set; } = "";

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x0600012D RID: 301 RVA: 0x00002C31 File Offset: 0x00000E31
	// (set) Token: 0x0600012E RID: 302 RVA: 0x00002C39 File Offset: 0x00000E39
	public string CancelYOverlayOffset { get; set; } = "";

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x0600012F RID: 303 RVA: 0x00002C42 File Offset: 0x00000E42
	// (set) Token: 0x06000130 RID: 304 RVA: 0x00002C4A File Offset: 0x00000E4A
	public string CancelShowOnOverlayExpr { get; set; }

	// Token: 0x04000071 RID: 113
	internal MOBASkillCancel mMOBASkillCancel;

	// Token: 0x0400007B RID: 123
	private double mCancelX = -1.0;

	// Token: 0x0400007C RID: 124
	private double mCancelY = -1.0;

	// Token: 0x04000080 RID: 128
	internal bool mShowOnOverlay = true;
}
