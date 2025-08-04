using System;
using System.ComponentModel;
using System.Windows.Input;

// Token: 0x02000009 RID: 9
[Description("Independent")]
[Serializable]
public class FreeLook : IMAction
{
	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000078 RID: 120 RVA: 0x0000253B File Offset: 0x0000073B
	// (set) Token: 0x06000079 RID: 121 RVA: 0x00002543 File Offset: 0x00000743
	[Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
	[Category("Fields")]
	public double X { get; set; } = -1.0;

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x0600007A RID: 122 RVA: 0x0000254C File Offset: 0x0000074C
	// (set) Token: 0x0600007B RID: 123 RVA: 0x00002554 File Offset: 0x00000754
	[Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
	[Category("Fields")]
	public double Y { get; set; } = -1.0;

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x0600007C RID: 124 RVA: 0x0000255D File Offset: 0x0000075D
	// (set) Token: 0x0600007D RID: 125 RVA: 0x00002565 File Offset: 0x00000765
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key { get; set; } = IMAPKeys.GetStringForFile(global::System.Windows.Input.Key.V);

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600007E RID: 126 RVA: 0x0000256E File Offset: 0x0000076E
	// (set) Token: 0x0600007F RID: 127 RVA: 0x00002576 File Offset: 0x00000776
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string Key_alt1 { get; set; } = string.Empty;

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000080 RID: 128 RVA: 0x0000257F File Offset: 0x0000077F
	// (set) Token: 0x06000081 RID: 129 RVA: 0x00002587 File Offset: 0x00000787
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft { get; set; } = IMAPKeys.GetStringForFile(global::System.Windows.Input.Key.Left);

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000082 RID: 130 RVA: 0x00002590 File Offset: 0x00000790
	// (set) Token: 0x06000083 RID: 131 RVA: 0x00002598 File Offset: 0x00000798
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyLeft_alt1 { get; set; } = string.Empty;

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000084 RID: 132 RVA: 0x000025A1 File Offset: 0x000007A1
	// (set) Token: 0x06000085 RID: 133 RVA: 0x000025A9 File Offset: 0x000007A9
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight { get; set; } = IMAPKeys.GetStringForFile(global::System.Windows.Input.Key.Right);

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000086 RID: 134 RVA: 0x000025B2 File Offset: 0x000007B2
	// (set) Token: 0x06000087 RID: 135 RVA: 0x000025BA File Offset: 0x000007BA
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyRight_alt1 { get; set; } = string.Empty;

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000088 RID: 136 RVA: 0x000025C3 File Offset: 0x000007C3
	// (set) Token: 0x06000089 RID: 137 RVA: 0x000025CB File Offset: 0x000007CB
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp { get; set; } = IMAPKeys.GetStringForFile(global::System.Windows.Input.Key.Up);

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x0600008A RID: 138 RVA: 0x000025D4 File Offset: 0x000007D4
	// (set) Token: 0x0600008B RID: 139 RVA: 0x000025DC File Offset: 0x000007DC
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyUp_alt1 { get; set; } = string.Empty;

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x0600008C RID: 140 RVA: 0x000025E5 File Offset: 0x000007E5
	// (set) Token: 0x0600008D RID: 141 RVA: 0x000025ED File Offset: 0x000007ED
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown { get; set; } = IMAPKeys.GetStringForFile(global::System.Windows.Input.Key.Down);

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x0600008E RID: 142 RVA: 0x000025F6 File Offset: 0x000007F6
	// (set) Token: 0x0600008F RID: 143 RVA: 0x000025FE File Offset: 0x000007FE
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyDown_alt1 { get; set; } = string.Empty;

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x06000090 RID: 144 RVA: 0x00002607 File Offset: 0x00000807
	// (set) Token: 0x06000091 RID: 145 RVA: 0x0000260F File Offset: 0x0000080F
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int DeviceType { get; set; }

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x06000092 RID: 146 RVA: 0x00002618 File Offset: 0x00000818
	// (set) Token: 0x06000093 RID: 147 RVA: 0x00002620 File Offset: 0x00000820
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

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x06000094 RID: 148 RVA: 0x00002629 File Offset: 0x00000829
	// (set) Token: 0x06000095 RID: 149 RVA: 0x00002631 File Offset: 0x00000831
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Sensitivity { get; set; } = 1.0;

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000096 RID: 150 RVA: 0x0000263A File Offset: 0x0000083A
	// (set) Token: 0x06000097 RID: 151 RVA: 0x00002642 File Offset: 0x00000842
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Speed { get; set; } = 20.0;

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x06000098 RID: 152 RVA: 0x0000264B File Offset: 0x0000084B
	// (set) Token: 0x06000099 RID: 153 RVA: 0x00002653 File Offset: 0x00000853
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool MouseAcceleration { get; set; }

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x0600009A RID: 154 RVA: 0x0000265C File Offset: 0x0000085C
	// (set) Token: 0x0600009B RID: 155 RVA: 0x00002664 File Offset: 0x00000864
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Delay { get; set; } = 50;

	// Token: 0x04000047 RID: 71
	internal bool mShowOnOverlay = true;
}
