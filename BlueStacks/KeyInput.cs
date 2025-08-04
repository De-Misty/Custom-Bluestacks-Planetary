using System;

// Token: 0x02000012 RID: 18
[Serializable]
public class KeyInput : IMAction
{
	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000193 RID: 403 RVA: 0x00002FF5 File Offset: 0x000011F5
	// (set) Token: 0x06000194 RID: 404 RVA: 0x00002FFD File Offset: 0x000011FD
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

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000195 RID: 405 RVA: 0x00003006 File Offset: 0x00001206
	// (set) Token: 0x06000196 RID: 406 RVA: 0x0000300E File Offset: 0x0000120E
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

	// Token: 0x040000C1 RID: 193
	private string mKeyIn;

	// Token: 0x040000C2 RID: 194
	private string mKeyOut;
}
