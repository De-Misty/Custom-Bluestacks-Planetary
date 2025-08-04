using System;
using System.IO;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200009D RID: 157
	public class AppRecommendation
	{
		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x000065A7 File Offset: 0x000047A7
		// (set) Token: 0x060006A4 RID: 1700 RVA: 0x000065AF File Offset: 0x000047AF
		[JsonProperty(PropertyName = "extra_payload")]
		public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x000065B8 File Offset: 0x000047B8
		// (set) Token: 0x060006A6 RID: 1702 RVA: 0x000065C0 File Offset: 0x000047C0
		[JsonProperty(PropertyName = "app_icon_id")]
		public string IconId { get; set; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x000065C9 File Offset: 0x000047C9
		// (set) Token: 0x060006A8 RID: 1704 RVA: 0x000065D1 File Offset: 0x000047D1
		[JsonProperty(PropertyName = "app_icon")]
		public string Icon { get; set; }

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x000065DA File Offset: 0x000047DA
		// (set) Token: 0x060006AA RID: 1706 RVA: 0x000065E2 File Offset: 0x000047E2
		[JsonProperty(PropertyName = "game_genre")]
		public string GameGenre { get; set; }

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x000065EB File Offset: 0x000047EB
		// (set) Token: 0x060006AC RID: 1708 RVA: 0x000065F3 File Offset: 0x000047F3
		[JsonProperty(PropertyName = "app_pkg")]
		public string AppPackage { get; set; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x000065FC File Offset: 0x000047FC
		// (set) Token: 0x060006AE RID: 1710 RVA: 0x00006604 File Offset: 0x00004804
		public string ImagePath { get; set; } = string.Empty;

		// Token: 0x060006AF RID: 1711 RVA: 0x000260DC File Offset: 0x000242DC
		internal void DeleteFile()
		{
			try
			{
				File.Delete(this.ImagePath);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't delete AppRecommendation file: " + this.ImagePath);
				Logger.Error(ex.ToString());
			}
		}
	}
}
