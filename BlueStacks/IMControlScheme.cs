using System;
using System.Collections.Generic;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

// Token: 0x02000021 RID: 33
[Serializable]
public class IMControlScheme
{
	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000279 RID: 633 RVA: 0x0000399B File Offset: 0x00001B9B
	// (set) Token: 0x0600027A RID: 634 RVA: 0x000039A3 File Offset: 0x00001BA3
	public List<IMAction> GameControls { get; private set; } = new List<IMAction>();

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x0600027B RID: 635 RVA: 0x000039AC File Offset: 0x00001BAC
	// (set) Token: 0x0600027C RID: 636 RVA: 0x000039B4 File Offset: 0x00001BB4
	public List<JObject> Images { get; private set; } = new List<JObject>();

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x0600027D RID: 637 RVA: 0x000039BD File Offset: 0x00001BBD
	// (set) Token: 0x0600027E RID: 638 RVA: 0x000039C5 File Offset: 0x00001BC5
	public string Name { get; set; }

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x0600027F RID: 639 RVA: 0x000039CE File Offset: 0x00001BCE
	// (set) Token: 0x06000280 RID: 640 RVA: 0x000039D6 File Offset: 0x00001BD6
	public bool BuiltIn { get; set; }

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000281 RID: 641 RVA: 0x000039DF File Offset: 0x00001BDF
	// (set) Token: 0x06000282 RID: 642 RVA: 0x000039E7 File Offset: 0x00001BE7
	public bool Selected { get; set; }

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000283 RID: 643 RVA: 0x000039F0 File Offset: 0x00001BF0
	// (set) Token: 0x06000284 RID: 644 RVA: 0x000039F8 File Offset: 0x00001BF8
	public bool IsBookMarked { get; set; }

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000285 RID: 645 RVA: 0x00003A01 File Offset: 0x00001C01
	// (set) Token: 0x06000286 RID: 646 RVA: 0x00003A09 File Offset: 0x00001C09
	public bool IsCategoryVisible { get; set; } = true;

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000287 RID: 647 RVA: 0x00003A12 File Offset: 0x00001C12
	// (set) Token: 0x06000288 RID: 648 RVA: 0x00003A1A File Offset: 0x00001C1A
	public string KeyboardLayout { get; set; } = InteropWindow.MapLayoutName(null);

	// Token: 0x06000289 RID: 649 RVA: 0x00013A10 File Offset: 0x00011C10
	public IMControlScheme DeepCopy()
	{
		IMControlScheme imcontrolScheme = (IMControlScheme)base.MemberwiseClone();
		List<IMAction> gameControls = this.GameControls;
		imcontrolScheme.SetGameControls((gameControls != null) ? gameControls.DeepCopy<List<IMAction>>() : null);
		imcontrolScheme.SetImages(this.Images.ConvertAll<JObject>((JObject jt) => (JObject)((jt != null) ? jt.DeepClone() : null)));
		return imcontrolScheme;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00003A23 File Offset: 0x00001C23
	public void SetGameControls(List<IMAction> gameControls)
	{
		this.GameControls = gameControls;
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00003A2C File Offset: 0x00001C2C
	public void SetImages(List<JObject> images)
	{
		List<JObject> list;
		if (images == null)
		{
			list = null;
		}
		else
		{
			list = images.ConvertAll<JObject>((JObject jt) => (JObject)((jt != null) ? jt.DeepClone() : null));
		}
		this.Images = list;
	}
}
