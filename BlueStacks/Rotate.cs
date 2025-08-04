using System;
using System.ComponentModel;
using System.Windows.Input;

// Token: 0x02000010 RID: 16
[Description("Independent")]
[Serializable]
public class Rotate : IMAction
{
	// Token: 0x17000096 RID: 150
	// (get) Token: 0x0600013C RID: 316 RVA: 0x00002C97 File Offset: 0x00000E97
	// (set) Token: 0x0600013D RID: 317 RVA: 0x00002C9F File Offset: 0x00000E9F
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x0600013E RID: 318 RVA: 0x00002CA8 File Offset: 0x00000EA8
	// (set) Token: 0x0600013F RID: 319 RVA: 0x00002CB0 File Offset: 0x00000EB0
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000140 RID: 320 RVA: 0x00002CB9 File Offset: 0x00000EB9
	// (set) Token: 0x06000141 RID: 321 RVA: 0x00002CC1 File Offset: 0x00000EC1
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyClock { get; set; } = IMAPKeys.GetStringForFile(Key.X);

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000142 RID: 322 RVA: 0x00002CCA File Offset: 0x00000ECA
	// (set) Token: 0x06000143 RID: 323 RVA: 0x00002CD2 File Offset: 0x00000ED2
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyAntiClock { get; set; } = IMAPKeys.GetStringForFile(Key.Z);

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000144 RID: 324 RVA: 0x00002CDB File Offset: 0x00000EDB
	// (set) Token: 0x06000145 RID: 325 RVA: 0x00002CE3 File Offset: 0x00000EE3
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyClock_alt1 { get; set; } = string.Empty;

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000146 RID: 326 RVA: 0x00002CEC File Offset: 0x00000EEC
	// (set) Token: 0x06000147 RID: 327 RVA: 0x00002CF4 File Offset: 0x00000EF4
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyAntiClock_alt1 { get; set; } = string.Empty;

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000148 RID: 328 RVA: 0x00002CFD File Offset: 0x00000EFD
	// (set) Token: 0x06000149 RID: 329 RVA: 0x00002D05 File Offset: 0x00000F05
	[Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
	[Category("Fields")]
	public double XRadius { get; set; } = 6.0;

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x0600014A RID: 330 RVA: 0x00002D0E File Offset: 0x00000F0E
	// (set) Token: 0x0600014B RID: 331 RVA: 0x00002D16 File Offset: 0x00000F16
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Speed { get; set; } = 60;

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x0600014C RID: 332 RVA: 0x00002D1F File Offset: 0x00000F1F
	// (set) Token: 0x0600014D RID: 333 RVA: 0x00002D27 File Offset: 0x00000F27
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int ActivationTime { get; set; } = 100;

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x0600014E RID: 334 RVA: 0x00002D30 File Offset: 0x00000F30
	// (set) Token: 0x0600014F RID: 335 RVA: 0x00002D38 File Offset: 0x00000F38
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Tweaks { get; set; } = 1;

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000150 RID: 336 RVA: 0x00002D41 File Offset: 0x00000F41
	// (set) Token: 0x06000151 RID: 337 RVA: 0x00002D49 File Offset: 0x00000F49
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double StartingAngle { get; set; } = 90.0;

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000152 RID: 338 RVA: 0x00002D52 File Offset: 0x00000F52
	// (set) Token: 0x06000153 RID: 339 RVA: 0x00002D5A File Offset: 0x00000F5A
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

	// Token: 0x040000A1 RID: 161
	internal bool mShowOnOverlay = true;
}
