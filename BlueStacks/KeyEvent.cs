using System;

// Token: 0x0200000F RID: 15
[Serializable]
public class KeyEvent : IMAction
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000133 RID: 307 RVA: 0x00002C53 File Offset: 0x00000E53
	// (set) Token: 0x06000134 RID: 308 RVA: 0x00002C5B File Offset: 0x00000E5B
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

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000135 RID: 309 RVA: 0x00002C64 File Offset: 0x00000E64
	// (set) Token: 0x06000136 RID: 310 RVA: 0x00002C6C File Offset: 0x00000E6C
	public int HoldTime
	{
		get
		{
			return this.mHoldTime;
		}
		set
		{
			this.mHoldTime = value;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000137 RID: 311 RVA: 0x00002C75 File Offset: 0x00000E75
	// (set) Token: 0x06000138 RID: 312 RVA: 0x00002C7D File Offset: 0x00000E7D
	public object KeyDownEvents
	{
		get
		{
			return this.mKeyDownEvents;
		}
		set
		{
			this.mKeyDownEvents = value;
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000139 RID: 313 RVA: 0x00002C86 File Offset: 0x00000E86
	// (set) Token: 0x0600013A RID: 314 RVA: 0x00002C8E File Offset: 0x00000E8E
	public object KeyUpEvents
	{
		get
		{
			return this.mKeyUpEvents;
		}
		set
		{
			this.mKeyUpEvents = value;
		}
	}

	// Token: 0x04000092 RID: 146
	private string mKey;

	// Token: 0x04000093 RID: 147
	private int mHoldTime;

	// Token: 0x04000094 RID: 148
	private object mKeyDownEvents;

	// Token: 0x04000095 RID: 149
	private object mKeyUpEvents;
}
