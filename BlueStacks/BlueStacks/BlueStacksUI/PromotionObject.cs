using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001A3 RID: 419
	public class PromotionObject
	{
		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600104C RID: 4172 RVA: 0x00069DA8 File Offset: 0x00067FA8
		// (remove) Token: 0x0600104D RID: 4173 RVA: 0x00069DDC File Offset: 0x00067FDC
		private static event EventHandler mBootPromotionHandler;

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x0600104E RID: 4174 RVA: 0x0000BC41 File Offset: 0x00009E41
		// (set) Token: 0x0600104F RID: 4175 RVA: 0x0000BC48 File Offset: 0x00009E48
		internal static EventHandler BootPromotionHandler
		{
			get
			{
				return PromotionObject.mBootPromotionHandler;
			}
			set
			{
				PromotionObject.mBootPromotionHandler = value;
			}
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x00069E10 File Offset: 0x00068010
		internal void SetDefaultMoreAppsOrder(bool overwrite = true)
		{
			if (this.MoreAppsDockOrder.Count == 0 || overwrite)
			{
				this.MoreAppsDockOrder.ClearAddRange(new SerializableDictionary<string, int>
				{
					{ "com.android.chrome", 2 },
					{ "com.android.camera2", 2 },
					{ "com.bluestacks.settings", 3 },
					{ "com.bluestacks.filemanager", 4 },
					{ "instance_manager", 5 },
					{ "help_center", 6 }
				});
			}
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x0000BC50 File Offset: 0x00009E50
		internal void SetDefaultDockOrder(bool overwrite = true)
		{
			if (this.DockOrder.Count == 0 || overwrite)
			{
				this.DockOrder.ClearAddRange(new SerializableDictionary<string, int>
				{
					{ "appcenter", 1 },
					{ "pikaworld", 2 }
				});
			}
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x0000BC8C File Offset: 0x00009E8C
		internal void SetDefaultMyAppsOrder(bool overwrite = true)
		{
			if (this.MyAppsOrder.Count == 0 || overwrite)
			{
				this.MyAppsOrder.ClearAddRange(new SerializableDictionary<string, int> { { "com.android.vending", 1 } });
			}
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x0000BCBC File Offset: 0x00009EBC
		internal void SetDefaultOrder(bool overwrite = true)
		{
			this.SetDefaultMyAppsOrder(overwrite);
			this.SetDefaultDockOrder(overwrite);
			this.SetDefaultMoreAppsOrder(overwrite);
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06001054 RID: 4180 RVA: 0x00069E88 File Offset: 0x00068088
		// (remove) Token: 0x06001055 RID: 4181 RVA: 0x00069EBC File Offset: 0x000680BC
		private static event EventHandler mBackgroundPromotionHandler;

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06001056 RID: 4182 RVA: 0x0000BCD3 File Offset: 0x00009ED3
		// (set) Token: 0x06001057 RID: 4183 RVA: 0x0000BCDA File Offset: 0x00009EDA
		internal static EventHandler BackgroundPromotionHandler
		{
			get
			{
				return PromotionObject.mBackgroundPromotionHandler;
			}
			set
			{
				PromotionObject.mBackgroundPromotionHandler = value;
				if (!PromotionObject.mIsPromotionLoading)
				{
					PromotionObject.mBackgroundPromotionHandler(PromotionObject.Instance, new EventArgs());
				}
			}
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06001058 RID: 4184 RVA: 0x00069EF0 File Offset: 0x000680F0
		// (remove) Token: 0x06001059 RID: 4185 RVA: 0x00069F24 File Offset: 0x00068124
		private static event EventHandler mPromotionHandler;

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x0600105A RID: 4186 RVA: 0x0000BCFD File Offset: 0x00009EFD
		// (set) Token: 0x0600105B RID: 4187 RVA: 0x0000BD04 File Offset: 0x00009F04
		internal static EventHandler PromotionHandler
		{
			get
			{
				return PromotionObject.mPromotionHandler;
			}
			set
			{
				PromotionObject.mPromotionHandler = value;
				if (!PromotionObject.mIsPromotionLoading)
				{
					PromotionObject.mPromotionHandler(PromotionObject.Instance, new EventArgs());
				}
			}
		}

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x0600105C RID: 4188 RVA: 0x00069F58 File Offset: 0x00068158
		// (remove) Token: 0x0600105D RID: 4189 RVA: 0x00069F8C File Offset: 0x0006818C
		private static event EventHandler mAppSpecificRulesHandler;

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x0600105E RID: 4190 RVA: 0x0000BD27 File Offset: 0x00009F27
		// (set) Token: 0x0600105F RID: 4191 RVA: 0x0000BD2E File Offset: 0x00009F2E
		internal static EventHandler AppSpecificRulesHandler
		{
			get
			{
				return PromotionObject.mAppSpecificRulesHandler;
			}
			set
			{
				PromotionObject.mAppSpecificRulesHandler = value;
				if (!PromotionObject.mIsPromotionLoading)
				{
					PromotionObject.mAppSpecificRulesHandler(PromotionObject.Instance, new EventArgs());
				}
			}
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06001060 RID: 4192 RVA: 0x00069FC0 File Offset: 0x000681C0
		// (remove) Token: 0x06001061 RID: 4193 RVA: 0x00069FF4 File Offset: 0x000681F4
		private static event Action<bool> mAppSuggestionHandler;

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06001062 RID: 4194 RVA: 0x0000BD51 File Offset: 0x00009F51
		// (set) Token: 0x06001063 RID: 4195 RVA: 0x0000BD58 File Offset: 0x00009F58
		internal static Action<bool> AppSuggestionHandler
		{
			get
			{
				return PromotionObject.mAppSuggestionHandler;
			}
			set
			{
				PromotionObject.mAppSuggestionHandler = value;
			}
		}

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06001064 RID: 4196 RVA: 0x0006A028 File Offset: 0x00068228
		// (remove) Token: 0x06001065 RID: 4197 RVA: 0x0006A05C File Offset: 0x0006825C
		private static event Action<bool> mAppRecommendationHandler;

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06001066 RID: 4198 RVA: 0x0000BD60 File Offset: 0x00009F60
		// (set) Token: 0x06001067 RID: 4199 RVA: 0x0000BD67 File Offset: 0x00009F67
		internal static Action<bool> AppRecommendationHandler
		{
			get
			{
				return PromotionObject.mAppRecommendationHandler;
			}
			set
			{
				PromotionObject.mAppRecommendationHandler = value;
			}
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06001068 RID: 4200 RVA: 0x0006A090 File Offset: 0x00068290
		// (remove) Token: 0x06001069 RID: 4201 RVA: 0x0006A0C4 File Offset: 0x000682C4
		private static event Action mQuestHandler;

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x0600106A RID: 4202 RVA: 0x0000BD6F File Offset: 0x00009F6F
		// (set) Token: 0x0600106B RID: 4203 RVA: 0x0000BD76 File Offset: 0x00009F76
		internal static Action QuestHandler
		{
			get
			{
				return PromotionObject.mQuestHandler;
			}
			set
			{
				PromotionObject.mQuestHandler = value;
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x0600106C RID: 4204 RVA: 0x0000BD7E File Offset: 0x00009F7E
		private static string FilePath
		{
			get
			{
				return Path.Combine(RegistryStrings.PromotionDirectory, "bst_promotion");
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0006A0F8 File Offset: 0x000682F8
		internal static void LoadDataFromFile()
		{
			try
			{
				if (File.Exists(PromotionObject.FilePath))
				{
					using (XmlReader xmlReader = XmlReader.Create(PromotionObject.FilePath, new XmlReaderSettings
					{
						ProhibitDtd = true
					}))
					{
						Logger.Info("vikramTest: Loading PromotionObject Settings from " + PromotionObject.FilePath);
						PromotionObject.Instance = (PromotionObject)new XmlSerializer(typeof(PromotionObject)).Deserialize(xmlReader);
						Logger.Info("vikramTest: Done loading promotionObject.");
						PromotionObject.Instance.QuestHdPlayerRules.ClearSync<string, long>();
						PromotionObject.Instance.QuestRules.ClearSync<QuestRule>();
						PromotionObject.Instance.ResetQuestRules.ClearSync<string, long[]>();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error Loading PromotionObject Settings " + ex.ToString());
			}
			finally
			{
				if (PromotionObject.Instance == null)
				{
					PromotionObject.Instance = new PromotionObject();
				}
				if (PromotionObject.Instance.DockOrder.Count == 0)
				{
					PromotionObject.Instance.SetDefaultDockOrder(true);
				}
				PromotionObject.CacheOldBootPromotions();
			}
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x0000BD8F File Offset: 0x00009F8F
		private static void CacheOldBootPromotions()
		{
			PromotionObject.Instance.DictOldBootPromotions.ClearAddRange(PromotionObject.Instance.DictBootPromotions);
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x0006A218 File Offset: 0x00068418
		internal static void Save()
		{
			try
			{
				if (!Directory.Exists(Directory.GetParent(PromotionObject.FilePath).FullName))
				{
					Directory.CreateDirectory(Directory.GetParent(PromotionObject.FilePath).FullName);
				}
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(PromotionObject.FilePath, Encoding.UTF8)
				{
					Formatting = Formatting.Indented
				})
				{
					new XmlSerializer(typeof(PromotionObject)).Serialize(xmlTextWriter, PromotionObject.Instance);
					xmlTextWriter.Flush();
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x0006A2C0 File Offset: 0x000684C0
		internal void PromotionLoaded()
		{
			PromotionObject.mIsPromotionLoading = false;
			EventHandler eventHandler = PromotionObject.mBootPromotionHandler;
			if (eventHandler != null)
			{
				eventHandler(this, new EventArgs());
			}
			EventHandler eventHandler2 = PromotionObject.mBackgroundPromotionHandler;
			if (eventHandler2 != null)
			{
				eventHandler2(this, new EventArgs());
			}
			Action action = PromotionObject.mQuestHandler;
			if (action != null)
			{
				action();
			}
			Action<bool> action2 = PromotionObject.mAppSuggestionHandler;
			if (action2 != null)
			{
				action2(true);
			}
			Action<bool> action3 = PromotionObject.mAppRecommendationHandler;
			if (action3 != null)
			{
				action3(true);
			}
			EventHandler eventHandler3 = PromotionObject.mPromotionHandler;
			if (eventHandler3 != null)
			{
				eventHandler3(this, new EventArgs());
			}
			EventHandler eventHandler4 = PromotionObject.mAppSpecificRulesHandler;
			if (eventHandler4 == null)
			{
				return;
			}
			eventHandler4(this, new EventArgs());
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06001071 RID: 4209 RVA: 0x0000BDAA File Offset: 0x00009FAA
		[XmlIgnore]
		public List<string> AppSpecificRulesList { get; } = new List<string>();

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06001072 RID: 4210 RVA: 0x0000BDB2 File Offset: 0x00009FB2
		public List<string> CustomCursorExcludedAppsList { get; } = new List<string> { "com.android.vending" };

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06001073 RID: 4211 RVA: 0x0000BDBA File Offset: 0x00009FBA
		// (set) Token: 0x06001074 RID: 4212 RVA: 0x0000BDC2 File Offset: 0x00009FC2
		[XmlIgnore]
		public bool IsRootAccessEnabled { get; set; }

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06001075 RID: 4213 RVA: 0x0000BDCB File Offset: 0x00009FCB
		// (set) Token: 0x06001076 RID: 4214 RVA: 0x0000BDD3 File Offset: 0x00009FD3
		public string MyAppsPromotionID { get; set; } = string.Empty;

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06001077 RID: 4215 RVA: 0x0000BDDC File Offset: 0x00009FDC
		// (set) Token: 0x06001078 RID: 4216 RVA: 0x0000BDE4 File Offset: 0x00009FE4
		public string MyAppsCrossPromotionID { get; set; } = string.Empty;

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06001079 RID: 4217 RVA: 0x0000BDED File Offset: 0x00009FED
		// (set) Token: 0x0600107A RID: 4218 RVA: 0x0000BDF5 File Offset: 0x00009FF5
		public string BackgroundPromotionID { get; set; } = string.Empty;

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x0600107B RID: 4219 RVA: 0x0000BDFE File Offset: 0x00009FFE
		// (set) Token: 0x0600107C RID: 4220 RVA: 0x0000BE06 File Offset: 0x0000A006
		public string BackgroundPromotionImagePath { get; set; } = string.Empty;

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x0600107D RID: 4221 RVA: 0x0000BE0F File Offset: 0x0000A00F
		// (set) Token: 0x0600107E RID: 4222 RVA: 0x0000BE17 File Offset: 0x0000A017
		public SerializableDictionary<string, AppIconPromotionObject> DictAppsPromotions { get; set; } = new SerializableDictionary<string, AppIconPromotionObject>();

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x0600107F RID: 4223 RVA: 0x0000BE20 File Offset: 0x0000A020
		// (set) Token: 0x06001080 RID: 4224 RVA: 0x0000BE28 File Offset: 0x0000A028
		public string QuestName { get; set; }

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06001081 RID: 4225 RVA: 0x0000BE31 File Offset: 0x0000A031
		// (set) Token: 0x06001082 RID: 4226 RVA: 0x0000BE39 File Offset: 0x0000A039
		public string QuestActionType { get; set; }

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06001083 RID: 4227 RVA: 0x0000BE42 File Offset: 0x0000A042
		public List<QuestRule> QuestRules { get; } = new List<QuestRule>();

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06001084 RID: 4228 RVA: 0x0000BE4A File Offset: 0x0000A04A
		// (set) Token: 0x06001085 RID: 4229 RVA: 0x0000BE52 File Offset: 0x0000A052
		public SerializableDictionary<string, long[]> ResetQuestRules { get; set; } = new SerializableDictionary<string, long[]>();

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06001086 RID: 4230 RVA: 0x0000BE5B File Offset: 0x0000A05B
		// (set) Token: 0x06001087 RID: 4231 RVA: 0x0000BE63 File Offset: 0x0000A063
		public SerializableDictionary<string, long> QuestHdPlayerRules { get; set; } = new SerializableDictionary<string, long>();

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x0000BE6C File Offset: 0x0000A06C
		// (set) Token: 0x06001089 RID: 4233 RVA: 0x0000BE74 File Offset: 0x0000A074
		public SerializableDictionary<string, int> MyAppsOrder { get; set; } = new SerializableDictionary<string, int>();

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x0000BE7D File Offset: 0x0000A07D
		// (set) Token: 0x0600108B RID: 4235 RVA: 0x0000BE85 File Offset: 0x0000A085
		public SerializableDictionary<string, int> DockOrder { get; set; } = new SerializableDictionary<string, int>
		{
			{ "appcenter", 1 },
			{ "com.android.vending", 2 },
			{ "pikaworld", 3 },
			{ "macro_recorder", 4 },
			{ "instance_manager", 5 },
			{ "help_center", 6 }
		};

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x0600108C RID: 4236 RVA: 0x0000BE8E File Offset: 0x0000A08E
		// (set) Token: 0x0600108D RID: 4237 RVA: 0x0000BE96 File Offset: 0x0000A096
		public SerializableDictionary<string, int> MoreAppsDockOrder { get; set; } = new SerializableDictionary<string, int>
		{
			{ "appcenter", 1 },
			{ "com.android.vending", 2 },
			{ "pikaworld", 3 },
			{ "macro_recorder", 4 },
			{ "instance_manager", 5 },
			{ "help_center", 6 }
		};

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x0000BE9F File Offset: 0x0000A09F
		// (set) Token: 0x0600108F RID: 4239 RVA: 0x0000BEA7 File Offset: 0x0000A0A7
		internal SerializableDictionary<string, BootPromotion> DictOldBootPromotions { get; set; } = new SerializableDictionary<string, BootPromotion>();

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x0000BEB0 File Offset: 0x0000A0B0
		// (set) Token: 0x06001091 RID: 4241 RVA: 0x0000BEB8 File Offset: 0x0000A0B8
		public int BootPromoDisplaytime { get; set; } = 4000;

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x0000BEC1 File Offset: 0x0000A0C1
		// (set) Token: 0x06001093 RID: 4243 RVA: 0x0000BEC9 File Offset: 0x0000A0C9
		public SerializableDictionary<string, BootPromotion> DictBootPromotions { get; set; } = new SerializableDictionary<string, BootPromotion>();

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06001094 RID: 4244 RVA: 0x0000BED2 File Offset: 0x0000A0D2
		// (set) Token: 0x06001095 RID: 4245 RVA: 0x0000BEDA File Offset: 0x0000A0DA
		public SerializableDictionary<string, SearchRecommendation> SearchRecommendations { get; set; } = new SerializableDictionary<string, SearchRecommendation>();

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x0000BEE3 File Offset: 0x0000A0E3
		// (set) Token: 0x06001097 RID: 4247 RVA: 0x0000BEEB File Offset: 0x0000A0EB
		public AppRecommendationSection AppRecommendations { get; set; } = new AppRecommendationSection();

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06001098 RID: 4248 RVA: 0x0000BEF4 File Offset: 0x0000A0F4
		public List<AppSuggestionPromotion> AppSuggestionList { get; } = new List<AppSuggestionPromotion>();

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06001099 RID: 4249 RVA: 0x0000BEFC File Offset: 0x0000A0FC
		public List<string> BlackListedApplicationsList { get; } = new List<string>();

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x0600109A RID: 4250 RVA: 0x0000BF04 File Offset: 0x0000A104
		// (set) Token: 0x0600109B RID: 4251 RVA: 0x0000BF0C File Offset: 0x0000A10C
		public SerializableDictionary<string, string> StartupTab { get; set; } = new SerializableDictionary<string, string>();

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x0600109C RID: 4252 RVA: 0x0000BF15 File Offset: 0x0000A115
		// (set) Token: 0x0600109D RID: 4253 RVA: 0x0000BF1D File Offset: 0x0000A11D
		public bool IsShowOtsFeedback { get; set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x0600109E RID: 4254 RVA: 0x0000BF26 File Offset: 0x0000A126
		// (set) Token: 0x0600109F RID: 4255 RVA: 0x0000BF2E File Offset: 0x0000A12E
		public string DiscordClientID { get; set; }

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x060010A0 RID: 4256 RVA: 0x0000BF37 File Offset: 0x0000A137
		// (set) Token: 0x060010A1 RID: 4257 RVA: 0x0000BF3F File Offset: 0x0000A13F
		public bool IsSecurityMetricsEnable { get; set; }

		// Token: 0x04000A89 RID: 2697
		private static bool mIsPromotionLoading = true;

		// Token: 0x04000A8A RID: 2698
		internal static volatile bool mIsBootPromotionLoading = true;

		// Token: 0x04000A92 RID: 2706
		private const string sPromotionFilename = "bst_promotion";

		// Token: 0x04000A93 RID: 2707
		internal static PromotionObject Instance = null;
	}
}
