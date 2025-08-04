using System;
using System.ComponentModel;
using System.Windows.Input;

// Token: 0x02000011 RID: 17
[Description("ParentElement")]
[Serializable]
public class Pan : IMAction
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000155 RID: 341 RVA: 0x00002D63 File Offset: 0x00000F63
	// (set) Token: 0x06000156 RID: 342 RVA: 0x00002D6B File Offset: 0x00000F6B
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
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000157 RID: 343 RVA: 0x00002D74 File Offset: 0x00000F74
	// (set) Token: 0x06000158 RID: 344 RVA: 0x00002D7C File Offset: 0x00000F7C
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
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000159 RID: 345 RVA: 0x00002D85 File Offset: 0x00000F85
	// (set) Token: 0x0600015A RID: 346 RVA: 0x00002D8D File Offset: 0x00000F8D
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyStartStop
	{
		get
		{
			return this.mKeyStartStop;
		}
		set
		{
			this.mKeyStartStop = value;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x0600015B RID: 347 RVA: 0x00002D96 File Offset: 0x00000F96
	// (set) Token: 0x0600015C RID: 348 RVA: 0x00002D9E File Offset: 0x00000F9E
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeyStartStop_alt1
	{
		get
		{
			return this.mKeyStartStop_1;
		}
		set
		{
			this.mKeyStartStop_1 = value;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x0600015D RID: 349 RVA: 0x00002DA7 File Offset: 0x00000FA7
	// (set) Token: 0x0600015E RID: 350 RVA: 0x00002DAF File Offset: 0x00000FAF
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySuspend
	{
		get
		{
			return this.mKeySuspend;
		}
		set
		{
			this.mKeySuspend = value;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x0600015F RID: 351 RVA: 0x00002DB8 File Offset: 0x00000FB8
	// (set) Token: 0x06000160 RID: 352 RVA: 0x00002DC0 File Offset: 0x00000FC0
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string KeySuspend_alt1
	{
		get
		{
			return this.mKeySuspend_1;
		}
		set
		{
			this.mKeySuspend_1 = value;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000161 RID: 353 RVA: 0x00002DC9 File Offset: 0x00000FC9
	// (set) Token: 0x06000162 RID: 354 RVA: 0x00002DD1 File Offset: 0x00000FD1
	public double LookAroundX
	{
		get
		{
			return this.mLookAroundX;
		}
		set
		{
			this.mLookAroundX = value;
			this.CheckLookAround();
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000163 RID: 355 RVA: 0x00002DE0 File Offset: 0x00000FE0
	// (set) Token: 0x06000164 RID: 356 RVA: 0x00002DE8 File Offset: 0x00000FE8
	public double LookAroundY
	{
		get
		{
			return this.mLookAroundY;
		}
		set
		{
			this.mLookAroundY = value;
			this.CheckLookAround();
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000165 RID: 357 RVA: 0x00002DF7 File Offset: 0x00000FF7
	// (set) Token: 0x06000166 RID: 358 RVA: 0x00002DFF File Offset: 0x00000FFF
	public string KeyLookAround
	{
		get
		{
			return this.mKeyLookAround;
		}
		set
		{
			this.mKeyLookAround = value;
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x06000167 RID: 359 RVA: 0x00002E08 File Offset: 0x00001008
	// (set) Token: 0x06000168 RID: 360 RVA: 0x00002E10 File Offset: 0x00001010
	public double LButtonX
	{
		get
		{
			return this.mLButtonX;
		}
		set
		{
			this.mLButtonX = value;
			this.CheckShootOnClick();
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000169 RID: 361 RVA: 0x00002E1F File Offset: 0x0000101F
	// (set) Token: 0x0600016A RID: 362 RVA: 0x00002E27 File Offset: 0x00001027
	public double LButtonY
	{
		get
		{
			return this.mLButtonY;
		}
		set
		{
			this.mLButtonY = value;
			this.CheckShootOnClick();
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x0600016B RID: 363 RVA: 0x00002E36 File Offset: 0x00001036
	internal string KeyAction
	{
		get
		{
			return this.mKeyAction;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x0600016C RID: 364 RVA: 0x00002E3E File Offset: 0x0000103E
	// (set) Token: 0x0600016D RID: 365 RVA: 0x00002E46 File Offset: 0x00001046
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double Sensitivity
	{
		get
		{
			return this.mSensitivity;
		}
		set
		{
			this.mSensitivity = value;
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x0600016E RID: 366 RVA: 0x00002E4F File Offset: 0x0000104F
	// (set) Token: 0x0600016F RID: 367 RVA: 0x00002E57 File Offset: 0x00001057
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public int Tweaks
	{
		get
		{
			return this.mTweaks;
		}
		set
		{
			this.mTweaks = value;
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000170 RID: 368 RVA: 0x00002E60 File Offset: 0x00001060
	// (set) Token: 0x06000171 RID: 369 RVA: 0x00002E68 File Offset: 0x00001068
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double SensitivityRatioY
	{
		get
		{
			return this.mSensitivityRatioY;
		}
		set
		{
			this.mSensitivityRatioY = value;
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x06000172 RID: 370 RVA: 0x00002E71 File Offset: 0x00001071
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsLookAroundEnabled
	{
		get
		{
			return this.mLookAround != null;
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000173 RID: 371 RVA: 0x00002E7C File Offset: 0x0000107C
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	internal bool IsShootOnClickEnabled
	{
		get
		{
			return this.mPanShoot != null;
		}
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00002E87 File Offset: 0x00001087
	private void CheckLookAround()
	{
		if (this.mLookAroundX == -1.0 && this.mLookAroundY == -1.0)
		{
			this.mLookAround = null;
			return;
		}
		if (this.mLookAround == null)
		{
			this.mLookAround = new LookAround(this);
		}
	}

	// Token: 0x06000175 RID: 373 RVA: 0x00002EC7 File Offset: 0x000010C7
	private void CheckShootOnClick()
	{
		if (this.mLButtonX == -1.0 && this.mLButtonY == -1.0)
		{
			this.mPanShoot = null;
			return;
		}
		if (this.mPanShoot == null)
		{
			this.mPanShoot = new PanShoot(this);
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000176 RID: 374 RVA: 0x00002F07 File Offset: 0x00001107
	// (set) Token: 0x06000177 RID: 375 RVA: 0x00002F0F File Offset: 0x0000110F
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

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000178 RID: 376 RVA: 0x00002F18 File Offset: 0x00001118
	// (set) Token: 0x06000179 RID: 377 RVA: 0x00002F20 File Offset: 0x00001120
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public bool MouseAcceleration
	{
		get
		{
			return this.mMouseAcceleration;
		}
		set
		{
			this.mMouseAcceleration = value;
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x0600017A RID: 378 RVA: 0x00002F29 File Offset: 0x00001129
	// (set) Token: 0x0600017B RID: 379 RVA: 0x00002F31 File Offset: 0x00001131
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public string GamepadStick
	{
		get
		{
			return this.mGamepadStick;
		}
		set
		{
			this.mGamepadStick = value;
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x0600017C RID: 380 RVA: 0x00002F3A File Offset: 0x0000113A
	// (set) Token: 0x0600017D RID: 381 RVA: 0x00002F42 File Offset: 0x00001142
	[Description("IMAP_PopupUIElement")]
	[Category("Fields")]
	public double GamepadSensitivity
	{
		get
		{
			return this.mGamepadSensitivity;
		}
		set
		{
			this.mGamepadSensitivity = value;
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x0600017E RID: 382 RVA: 0x00002F4B File Offset: 0x0000114B
	// (set) Token: 0x0600017F RID: 383 RVA: 0x00002F53 File Offset: 0x00001153
	public string LButtonXExpr { get; set; }

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000180 RID: 384 RVA: 0x00002F5C File Offset: 0x0000115C
	// (set) Token: 0x06000181 RID: 385 RVA: 0x00002F64 File Offset: 0x00001164
	public string LButtonYExpr { get; set; }

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000182 RID: 386 RVA: 0x00002F6D File Offset: 0x0000116D
	// (set) Token: 0x06000183 RID: 387 RVA: 0x00002F75 File Offset: 0x00001175
	public string LButtonXOverlayOffset { get; set; }

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000184 RID: 388 RVA: 0x00002F7E File Offset: 0x0000117E
	// (set) Token: 0x06000185 RID: 389 RVA: 0x00002F86 File Offset: 0x00001186
	public string LButtonYOverlayOffset { get; set; }

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000186 RID: 390 RVA: 0x00002F8F File Offset: 0x0000118F
	// (set) Token: 0x06000187 RID: 391 RVA: 0x00002F97 File Offset: 0x00001197
	public string LookAroundXExpr { get; set; }

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000188 RID: 392 RVA: 0x00002FA0 File Offset: 0x000011A0
	// (set) Token: 0x06000189 RID: 393 RVA: 0x00002FA8 File Offset: 0x000011A8
	public string LookAroundYExpr { get; set; }

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x0600018A RID: 394 RVA: 0x00002FB1 File Offset: 0x000011B1
	// (set) Token: 0x0600018B RID: 395 RVA: 0x00002FB9 File Offset: 0x000011B9
	public string LookAroundXOverlayOffset { get; set; }

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x0600018C RID: 396 RVA: 0x00002FC2 File Offset: 0x000011C2
	// (set) Token: 0x0600018D RID: 397 RVA: 0x00002FCA File Offset: 0x000011CA
	public string LookAroundYOverlayOffset { get; set; }

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x0600018E RID: 398 RVA: 0x00002FD3 File Offset: 0x000011D3
	// (set) Token: 0x0600018F RID: 399 RVA: 0x00002FDB File Offset: 0x000011DB
	public string LButtonShowOnOverlayExpr { get; set; }

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000190 RID: 400 RVA: 0x00002FE4 File Offset: 0x000011E4
	// (set) Token: 0x06000191 RID: 401 RVA: 0x00002FEC File Offset: 0x000011EC
	public string LookAroundShowOnOverlayExpr { get; set; }

	// Token: 0x040000A2 RID: 162
	internal LookAround mLookAround;

	// Token: 0x040000A3 RID: 163
	internal PanShoot mPanShoot;

	// Token: 0x040000A4 RID: 164
	private double mX = -1.0;

	// Token: 0x040000A5 RID: 165
	private double mY = -1.0;

	// Token: 0x040000A6 RID: 166
	private string mKeyStartStop = IMAPKeys.GetStringForFile(Key.F1);

	// Token: 0x040000A7 RID: 167
	private string mKeyStartStop_1 = string.Empty;

	// Token: 0x040000A8 RID: 168
	private string mKeySuspend = IMAPKeys.GetStringForFile(Key.LeftAlt);

	// Token: 0x040000A9 RID: 169
	private string mKeySuspend_1 = string.Empty;

	// Token: 0x040000AA RID: 170
	private double mLookAroundX = -1.0;

	// Token: 0x040000AB RID: 171
	private double mLookAroundY = -1.0;

	// Token: 0x040000AC RID: 172
	private string mKeyLookAround = IMAPKeys.GetStringForFile(Key.V);

	// Token: 0x040000AD RID: 173
	private double mLButtonX = -1.0;

	// Token: 0x040000AE RID: 174
	private double mLButtonY = -1.0;

	// Token: 0x040000AF RID: 175
	private string mKeyAction = "MouseLButton";

	// Token: 0x040000B0 RID: 176
	private double mSensitivity = 1.0;

	// Token: 0x040000B1 RID: 177
	private int mTweaks;

	// Token: 0x040000B2 RID: 178
	private double mSensitivityRatioY = 1.0;

	// Token: 0x040000B3 RID: 179
	internal bool mShowOnOverlay = true;

	// Token: 0x040000B4 RID: 180
	private bool mMouseAcceleration;

	// Token: 0x040000B5 RID: 181
	private string mGamepadStick = "";

	// Token: 0x040000B6 RID: 182
	private double mGamepadSensitivity = 1.0;
}
