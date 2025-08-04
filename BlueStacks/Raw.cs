using System;

// Token: 0x02000013 RID: 19
[Serializable]
public class Raw : IMAction
{
	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000198 RID: 408 RVA: 0x00003017 File Offset: 0x00001217
	// (set) Token: 0x06000199 RID: 409 RVA: 0x0000301F File Offset: 0x0000121F
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

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x0600019A RID: 410 RVA: 0x00003028 File Offset: 0x00001228
	// (set) Token: 0x0600019B RID: 411 RVA: 0x00003030 File Offset: 0x00001230
	public object Sequence
	{
		get
		{
			return this.mSequence;
		}
		set
		{
			this.mSequence = value;
		}
	}

	// Token: 0x040000C3 RID: 195
	private string mKey;

	// Token: 0x040000C4 RID: 196
	private object mSequence;
}
