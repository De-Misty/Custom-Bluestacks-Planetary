using System;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001A6 RID: 422
	[JsonObject(MemberSerialization.OptIn)]
	public class AppSuggestionPromotion
	{
		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x060010C0 RID: 4288 RVA: 0x0000C04E File Offset: 0x0000A24E
		// (set) Token: 0x060010C1 RID: 4289 RVA: 0x0000C056 File Offset: 0x0000A256
		[JsonProperty("app_pkg", NullValueHandling = NullValueHandling.Ignore)]
		public string AppPackage { get; set; } = string.Empty;

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x060010C2 RID: 4290 RVA: 0x0000C05F File Offset: 0x0000A25F
		// (set) Token: 0x060010C3 RID: 4291 RVA: 0x0000C067 File Offset: 0x0000A267
		[JsonProperty("app_activity", NullValueHandling = NullValueHandling.Ignore)]
		public string AppActivity { get; set; }

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x060010C4 RID: 4292 RVA: 0x0000C070 File Offset: 0x0000A270
		// (set) Token: 0x060010C5 RID: 4293 RVA: 0x0000C078 File Offset: 0x0000A278
		[JsonProperty("show_red_dot", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsShowRedDot { get; set; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x060010C6 RID: 4294 RVA: 0x0000C081 File Offset: 0x0000A281
		// (set) Token: 0x060010C7 RID: 4295 RVA: 0x0000C089 File Offset: 0x0000A289
		[JsonProperty("app_name", NullValueHandling = NullValueHandling.Ignore)]
		public string AppName { get; set; }

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x060010C8 RID: 4296 RVA: 0x0000C092 File Offset: 0x0000A292
		// (set) Token: 0x060010C9 RID: 4297 RVA: 0x0000C09A File Offset: 0x0000A29A
		[JsonProperty("app_icon", NullValueHandling = NullValueHandling.Ignore)]
		public string AppIcon { get; set; }

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x060010CA RID: 4298 RVA: 0x0000C0A3 File Offset: 0x0000A2A3
		// (set) Token: 0x060010CB RID: 4299 RVA: 0x0000C0AB File Offset: 0x0000A2AB
		[JsonProperty("app_icon_id", NullValueHandling = NullValueHandling.Ignore)]
		public string AppIconId { get; set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x060010CC RID: 4300 RVA: 0x0000C0B4 File Offset: 0x0000A2B4
		// (set) Token: 0x060010CD RID: 4301 RVA: 0x0000C0BC File Offset: 0x0000A2BC
		[JsonProperty("tooltip", NullValueHandling = NullValueHandling.Ignore)]
		public string ToolTip { get; set; }

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x060010CE RID: 4302 RVA: 0x0000C0C5 File Offset: 0x0000A2C5
		// (set) Token: 0x060010CF RID: 4303 RVA: 0x0000C0CD File Offset: 0x0000A2CD
		[JsonProperty("cross_promotion_pkg", NullValueHandling = NullValueHandling.Ignore)]
		public string CrossPromotionPackage { get; set; }

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x060010D0 RID: 4304 RVA: 0x0000C0D6 File Offset: 0x0000A2D6
		// (set) Token: 0x060010D1 RID: 4305 RVA: 0x0000C0DE File Offset: 0x0000A2DE
		[JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
		public string AppLocation { get; set; } = string.Empty;

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x060010D2 RID: 4306 RVA: 0x0000C0E7 File Offset: 0x0000A2E7
		// (set) Token: 0x060010D3 RID: 4307 RVA: 0x0000C0EF File Offset: 0x0000A2EF
		[JsonProperty("is_email_required", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsEmailRequired { get; set; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x060010D4 RID: 4308 RVA: 0x0000C0F8 File Offset: 0x0000A2F8
		// (set) Token: 0x060010D5 RID: 4309 RVA: 0x0000C100 File Offset: 0x0000A300
		public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x060010D6 RID: 4310 RVA: 0x0000C109 File Offset: 0x0000A309
		// (set) Token: 0x060010D7 RID: 4311 RVA: 0x0000C111 File Offset: 0x0000A311
		[JsonProperty("is_animation", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsAnimation { get; set; }

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x060010D8 RID: 4312 RVA: 0x0000C11A File Offset: 0x0000A31A
		// (set) Token: 0x060010D9 RID: 4313 RVA: 0x0000C122 File Offset: 0x0000A322
		[JsonProperty("animation_time", NullValueHandling = NullValueHandling.Ignore)]
		public int AnimationTime { get; set; }

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x060010DA RID: 4314 RVA: 0x0000C12B File Offset: 0x0000A32B
		// (set) Token: 0x060010DB RID: 4315 RVA: 0x0000C133 File Offset: 0x0000A333
		[JsonProperty("is_icon_border", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsIconBorder { get; set; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x060010DC RID: 4316 RVA: 0x0000C13C File Offset: 0x0000A33C
		// (set) Token: 0x060010DD RID: 4317 RVA: 0x0000C144 File Offset: 0x0000A344
		[JsonProperty("icon_border_url", NullValueHandling = NullValueHandling.Ignore)]
		public string IconBorderUrl { get; set; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x060010DE RID: 4318 RVA: 0x0000C14D File Offset: 0x0000A34D
		// (set) Token: 0x060010DF RID: 4319 RVA: 0x0000C155 File Offset: 0x0000A355
		[JsonProperty("icon_border_click_url", NullValueHandling = NullValueHandling.Ignore)]
		public string IconBorderClickUrl { get; set; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x060010E0 RID: 4320 RVA: 0x0000C15E File Offset: 0x0000A35E
		// (set) Token: 0x060010E1 RID: 4321 RVA: 0x0000C166 File Offset: 0x0000A366
		[JsonProperty("icon_border_id", NullValueHandling = NullValueHandling.Ignore)]
		public string IconBorderId { get; set; }

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x060010E2 RID: 4322 RVA: 0x0000C16F File Offset: 0x0000A36F
		// (set) Token: 0x060010E3 RID: 4323 RVA: 0x0000C177 File Offset: 0x0000A377
		[JsonProperty("icon_border_hover_url", NullValueHandling = NullValueHandling.Ignore)]
		public string IconBorderHoverUrl { get; set; }

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x060010E4 RID: 4324 RVA: 0x0000C180 File Offset: 0x0000A380
		// (set) Token: 0x060010E5 RID: 4325 RVA: 0x0000C188 File Offset: 0x0000A388
		public string AppIconPath { get; set; }

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x060010E6 RID: 4326 RVA: 0x0000C191 File Offset: 0x0000A391
		// (set) Token: 0x060010E7 RID: 4327 RVA: 0x0000C199 File Offset: 0x0000A399
		[JsonProperty("app_icon_width", NullValueHandling = NullValueHandling.Ignore)]
		public double IconWidth { get; set; }

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060010E8 RID: 4328 RVA: 0x0000C1A2 File Offset: 0x0000A3A2
		// (set) Token: 0x060010E9 RID: 4329 RVA: 0x0000C1AA File Offset: 0x0000A3AA
		[JsonProperty("app_icon_height", NullValueHandling = NullValueHandling.Ignore)]
		public double IconHeight { get; set; }
	}
}
