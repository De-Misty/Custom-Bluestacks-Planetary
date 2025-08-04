using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200003D RID: 61
	internal class GameSetting
	{
		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060003AD RID: 941 RVA: 0x00004653 File Offset: 0x00002853
		// (set) Token: 0x060003AE RID: 942 RVA: 0x0000465B File Offset: 0x0000285B
		[JsonProperty(PropertyName = "setting_type")]
		public string SettingType { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060003AF RID: 943 RVA: 0x00004664 File Offset: 0x00002864
		// (set) Token: 0x060003B0 RID: 944 RVA: 0x0000466C File Offset: 0x0000286C
		public List<Dictionary<string, object>> SettingsData { get; set; } = new List<Dictionary<string, object>>();
	}
}
