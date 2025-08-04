using System;

// Token: 0x0200000A RID: 10
[Serializable]
public class Alias : IMAction
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x0600009D RID: 157 RVA: 0x0000266D File Offset: 0x0000086D
	// (set) Token: 0x0600009E RID: 158 RVA: 0x00002675 File Offset: 0x00000875
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

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x0600009F RID: 159 RVA: 0x0000267E File Offset: 0x0000087E
	// (set) Token: 0x060000A0 RID: 160 RVA: 0x00002686 File Offset: 0x00000886
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

	// Token: 0x0400004C RID: 76
	private string mKeyIn;

	// Token: 0x0400004D RID: 77
	private string mKeyOut;
}
