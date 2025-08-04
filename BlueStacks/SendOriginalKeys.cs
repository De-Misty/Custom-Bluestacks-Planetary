using System;

// Token: 0x02000014 RID: 20
[Serializable]
public class SendOriginalKeys : IMAction
{
	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x0600019D RID: 413 RVA: 0x00003039 File Offset: 0x00001239
	// (set) Token: 0x0600019E RID: 414 RVA: 0x00003041 File Offset: 0x00001241
	public string Comments
	{
		get
		{
			return this.mComments;
		}
		set
		{
			this.mComments = value;
		}
	}

	// Token: 0x040000C5 RID: 197
	private string mComments;
}
