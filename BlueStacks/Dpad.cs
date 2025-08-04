using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using BlueStacks.BlueStacksUI;
using BlueStacks.Common;
using Newtonsoft.Json;

// Token: 0x0200000B RID: 11
[Description("Independent")]
[Serializable]
public class Dpad : IMAction
{
	// Token: 0x060000A2 RID: 162 RVA: 0x00012948 File Offset: 0x00010B48
	public Dpad()
	{
		base.Type = KeyActionType.Dpad;
		Dpad.sListDpad.Add(this);
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060000A3 RID: 163 RVA: 0x00002697 File Offset: 0x00000897
	// (set) Token: 0x060000A4 RID: 164 RVA: 0x0000269F File Offset: 0x0000089F
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
			if (this.IsMOBADpadEnabled)
			{
				this.mMOBADpad.X = value;
			}
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x060000A5 RID: 165 RVA: 0x000026BC File Offset: 0x000008BC
	// (set) Token: 0x060000A6 RID: 166 RVA: 0x000026C4 File Offset: 0x000008C4
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
			if (this.IsMOBADpadEnabled)
			{
				this.mMOBADpad.Y = value;
			}
		}
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060000A7 RID: 167 RVA: 0x000026E1 File Offset: 0x000008E1
	// (set) Token: 0x060000A8 RID: 168 RVA: 0x000026E9 File Offset: 0x000008E9
	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius { get; set; } = 6.0;

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060000A9 RID: 169 RVA: 0x00012A28 File Offset: 0x00010C28
	[JsonIgnore]
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string DpadTitle
	{
		get
		{
			return string.Concat(new string[]
			{
				LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyUp.ToString(CultureInfo.InvariantCulture)), ""),
				" ",
				LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyLeft.ToString(CultureInfo.InvariantCulture)), ""),
				" ",
				LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyDown.ToString(CultureInfo.InvariantCulture)), ""),
				" ",
				LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyRight.ToString(CultureInfo.InvariantCulture)), "")
			}).Trim();
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060000AA RID: 170 RVA: 0x000026F2 File Offset: 0x000008F2
	// (set) Token: 0x060000AB RID: 171 RVA: 0x000026FA File Offset: 0x000008FA
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp { get; set; } = IMAPKeys.GetStringForFile(Key.W);

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060000AC RID: 172 RVA: 0x00002703 File Offset: 0x00000903
	// (set) Token: 0x060000AD RID: 173 RVA: 0x0000270B File Offset: 0x0000090B
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp_alt1 { get; set; } = string.Empty;

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060000AE RID: 174 RVA: 0x00002714 File Offset: 0x00000914
	// (set) Token: 0x060000AF RID: 175 RVA: 0x0000271C File Offset: 0x0000091C
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft { get; set; } = IMAPKeys.GetStringForFile(Key.A);

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060000B0 RID: 176 RVA: 0x00002725 File Offset: 0x00000925
	// (set) Token: 0x060000B1 RID: 177 RVA: 0x0000272D File Offset: 0x0000092D
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft_alt1 { get; set; } = string.Empty;

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060000B2 RID: 178 RVA: 0x00002736 File Offset: 0x00000936
	// (set) Token: 0x060000B3 RID: 179 RVA: 0x0000273E File Offset: 0x0000093E
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown { get; set; } = IMAPKeys.GetStringForFile(Key.S);

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060000B4 RID: 180 RVA: 0x00002747 File Offset: 0x00000947
	// (set) Token: 0x060000B5 RID: 181 RVA: 0x0000274F File Offset: 0x0000094F
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown_alt1 { get; set; } = string.Empty;

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060000B6 RID: 182 RVA: 0x00002758 File Offset: 0x00000958
	// (set) Token: 0x060000B7 RID: 183 RVA: 0x00002760 File Offset: 0x00000960
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight { get; set; } = IMAPKeys.GetStringForFile(Key.D);

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060000B8 RID: 184 RVA: 0x00002769 File Offset: 0x00000969
	// (set) Token: 0x060000B9 RID: 185 RVA: 0x00002771 File Offset: 0x00000971
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight_alt1 { get; set; } = string.Empty;

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060000BA RID: 186 RVA: 0x0000277A File Offset: 0x0000097A
	// (set) Token: 0x060000BB RID: 187 RVA: 0x00002782 File Offset: 0x00000982
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string GamepadStick { get; set; } = "";

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060000BC RID: 188 RVA: 0x0000278B File Offset: 0x0000098B
	// (set) Token: 0x060000BD RID: 189 RVA: 0x00002793 File Offset: 0x00000993
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier1 { get; set; }

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x060000BE RID: 190 RVA: 0x0000279C File Offset: 0x0000099C
	// (set) Token: 0x060000BF RID: 191 RVA: 0x000027A4 File Offset: 0x000009A4
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier1_alt1 { get; set; }

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x060000C0 RID: 192 RVA: 0x000027AD File Offset: 0x000009AD
	// (set) Token: 0x060000C1 RID: 193 RVA: 0x000027B5 File Offset: 0x000009B5
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius1 { get; set; }

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060000C2 RID: 194 RVA: 0x000027BE File Offset: 0x000009BE
	// (set) Token: 0x060000C3 RID: 195 RVA: 0x000027C6 File Offset: 0x000009C6
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier2 { get; set; }

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060000C4 RID: 196 RVA: 0x000027CF File Offset: 0x000009CF
	// (set) Token: 0x060000C5 RID: 197 RVA: 0x000027D7 File Offset: 0x000009D7
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySpeedModifier2_alt1 { get; set; }

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060000C6 RID: 198 RVA: 0x000027E0 File Offset: 0x000009E0
	// (set) Token: 0x060000C7 RID: 199 RVA: 0x000027E8 File Offset: 0x000009E8
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius2 { get; set; }

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060000C8 RID: 200 RVA: 0x000027F1 File Offset: 0x000009F1
	// (set) Token: 0x060000C9 RID: 201 RVA: 0x000027F9 File Offset: 0x000009F9
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Speed { get; set; } = 200.0;

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060000CA RID: 202 RVA: 0x00002802 File Offset: 0x00000A02
	// (set) Token: 0x060000CB RID: 203 RVA: 0x0000280A File Offset: 0x00000A0A
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int ActivationTime { get; set; }

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060000CC RID: 204 RVA: 0x00002813 File Offset: 0x00000A13
	// (set) Token: 0x060000CD RID: 205 RVA: 0x0000281B File Offset: 0x00000A1B
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double ActivationSpeed { get; set; }

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060000CE RID: 206 RVA: 0x00002824 File Offset: 0x00000A24
	// (set) Token: 0x060000CF RID: 207 RVA: 0x0000282C File Offset: 0x00000A2C
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double DeadzoneRadius { get; set; }

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060000D0 RID: 208 RVA: 0x00002835 File Offset: 0x00000A35
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsMOBADpadEnabled
	{
		get
		{
			return this.mMOBADpad.OriginX != -1.0 && this.mMOBADpad.OriginY != -1.0;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x060000D1 RID: 209 RVA: 0x00002868 File Offset: 0x00000A68
	// (set) Token: 0x060000D2 RID: 210 RVA: 0x00002870 File Offset: 0x00000A70
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

	// Token: 0x0400004E RID: 78
	internal static List<Dpad> sListDpad = new List<Dpad>();

	// Token: 0x0400004F RID: 79
	internal MOBADpad mMOBADpad = new MOBADpad();

	// Token: 0x04000050 RID: 80
	private double mX = -1.0;

	// Token: 0x04000051 RID: 81
	private double mY = -1.0;

	// Token: 0x04000066 RID: 102
	internal bool mShowOnOverlay = true;
}
