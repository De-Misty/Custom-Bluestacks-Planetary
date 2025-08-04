using System;
using BlueStacks.BlueStacksUI;

// Token: 0x02000023 RID: 35
[Serializable]
public class MetaData
{
	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000291 RID: 657 RVA: 0x00003AAF File Offset: 0x00001CAF
	// (set) Token: 0x06000292 RID: 658 RVA: 0x00003AB7 File Offset: 0x00001CB7
	public string ParserVersion { get; set; } = KMManager.ParserVersion;

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000293 RID: 659 RVA: 0x00003AC0 File Offset: 0x00001CC0
	// (set) Token: 0x06000294 RID: 660 RVA: 0x00003AC8 File Offset: 0x00001CC8
	public string Comment { get; set; }
}
