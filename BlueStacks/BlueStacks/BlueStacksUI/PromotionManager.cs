using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200019B RID: 411
	internal class PromotionManager
	{
		// Token: 0x06001014 RID: 4116 RVA: 0x00066C2C File Offset: 0x00064E2C
		internal static BootPromotion AddBootPromotion(JToken promoImage)
		{
			BootPromotion bootPromotion = new BootPromotion();
			string value = promoImage.GetValue("image_url");
			bootPromotion.ImageUrl = promoImage.GetValue("image_url");
			bootPromotion.Id = promoImage.GetValue("id");
			string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[] { "BootPromo", bootPromotion.Id });
			bootPromotion.Order = int.Parse(promoImage.GetValue("order"), CultureInfo.InvariantCulture);
			if (!JsonExtensions.IsNullOrEmptyBrackets(promoImage.GetValue("extra_payload")))
			{
				bootPromotion.ExtraPayload.ClearAddRange(promoImage["extra_payload"].ToSerializableDictionary<string>());
				PromotionManager.PopulateAndDownloadFavicon(bootPromotion.ExtraPayload, text + "_" + bootPromotion.Id, false);
			}
			bootPromotion.ButtonText = promoImage.GetValue("button_text");
			bootPromotion.ThemeEnabled = promoImage.GetValue("theme_enabled");
			bootPromotion.ThemeName = promoImage.GetValue("theme_name");
			bootPromotion.ImagePath = Utils.TinyDownloader(value, text, RegistryStrings.PromotionDirectory, false);
			bootPromotion.PromoBtnClickStatusText = promoImage.GetValue("promo_button_click_status_text");
			return bootPromotion;
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x00066D54 File Offset: 0x00064F54
		internal static SearchRecommendation AddSearchRecommendation(JToken searchItem)
		{
			SearchRecommendation searchRecommendation = new SearchRecommendation
			{
				IconId = searchItem.GetValue("app_icon_id")
			};
			string value = searchItem.GetValue("app_icon");
			string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[] { "recommendation", searchRecommendation.IconId });
			searchRecommendation.ImagePath = Utils.TinyDownloader(value, text, RegistryStrings.PromotionDirectory, false);
			if (!JsonExtensions.IsNullOrEmptyBrackets(searchItem.GetValue("extra_payload")))
			{
				searchRecommendation.ExtraPayload.ClearAddRange(searchItem["extra_payload"].ToSerializableDictionary<string>());
			}
			return searchRecommendation;
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x00066DEC File Offset: 0x00064FEC
		internal static void SendAppUsageStats()
		{
			string urlWithParams = WebHelper.GetUrlWithParams(RegistryManager.Instance.Host + "/bs3/stats/v4/usage");
			string text = AppUsageTimer.DecryptString(RegistryManager.Instance.AInfo);
			if (!string.IsNullOrEmpty(text))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "usage", text } };
				try
				{
					BstHttpClient.Post(urlWithParams, dictionary, null, false, string.Empty, 0, 1, 0, false, "bgp64");
					RegistryManager.Instance.AInfo = string.Empty;
				}
				catch (Exception ex)
				{
					Logger.Error(ex.ToString());
					Logger.Error("Post failed. url = {0}", new object[] { urlWithParams });
				}
			}
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x00066E98 File Offset: 0x00065098
		internal static string AddDiscordClientVersionInUrl(string url)
		{
			string text = string.Empty;
			string text2 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Discord";
			try
			{
				text = (string)Utils.GetRegistryHKCUValue(text2, "DisplayVersion", string.Empty);
				if (string.IsNullOrEmpty(text))
				{
					text = (string)Utils.GetRegistryHKLMValue(text2, "DisplayVersion", string.Empty);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("exception in getting discord client version.." + ex.ToString());
			}
			url += "&discord_version=";
			url += text;
			return url;
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x00066F28 File Offset: 0x00065128
		private static Dictionary<string, string> GetPromotionCallData()
		{
			Dictionary<string, string> data = PromotionManager.GetInstalledAppsData();
			Dictionary<string, string> resolutionData = BlueStacksUIUtils.GetResolutionData();
			try
			{
				resolutionData.ToList<KeyValuePair<string, string>>().ForEach(delegate(KeyValuePair<string, string> kvp)
				{
					data[kvp.Key] = kvp.Value;
				});
				Logger.Info("RESOLUTION : " + data["resolution"]);
				Logger.Info("RESOLUTION TYPE : " + data["resolution_type"]);
			}
			catch (Exception ex)
			{
				Logger.Error("Merge dictionary failed. Ex : " + ex.ToString());
			}
			return data;
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x00066FD4 File Offset: 0x000651D4
		internal static Dictionary<string, string> GetInstalledAppsData()
		{
			List<AppInfo> list = new JsonParser("Android").GetAppList().ToList<AppInfo>();
			JArray jarray = new JArray();
			foreach (AppInfo appInfo in list)
			{
				JObject jobject = new JObject();
				string package = appInfo.Package;
				string name = appInfo.Name;
				jobject.Add(package, name);
				jarray.Add(jobject);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"installed_apps",
				jarray.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0])
			} };
			dictionary.Add("all_installed_apps", Utils.GetInstalledAppDataFromAllVms());
			dictionary.Add("campaign_json", RegistryManager.Instance.CampaignJson);
			dictionary.Add("email", RegistryManager.Instance.RegisteredEmail);
			if (!string.IsNullOrEmpty(Opt.Instance.Json))
			{
				JObject jobject2 = JObject.Parse(Opt.Instance.Json);
				if (jobject2["fle_pkg"] != null)
				{
					dictionary.Add("fle_packagename", jobject2["fle_pkg"].ToString().Trim());
				}
			}
			if (RegistryManager.Instance.IsClientFirstLaunch == 1)
			{
				if (RegistryManager.Instance.IsClientUpgraded)
				{
					dictionary.Add("first_boot_update", bool.TrueString);
				}
				else
				{
					dictionary.Add("first_boot", bool.TrueString);
				}
			}
			try
			{
				string text = Path.Combine(RegistryStrings.PromotionDirectory, "app_suggestion_removed");
				if (File.Exists(text))
				{
					string text2 = File.ReadAllText(text);
					List<string> list2 = new List<string>();
					if (!string.IsNullOrEmpty(text2))
					{
						list2 = PromotionManager.DoDeserialize<List<string>>(text2);
					}
					jarray = new JArray();
					foreach (string text3 in list2)
					{
						jarray.Add(text3);
					}
					dictionary.Add("cross_promotion_closed_apps_list", jarray.ToString(Newtonsoft.Json.Formatting.None, new JsonConverter[0]));
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Error in adding cross promotion closed app list " + ex.ToString());
				if (!dictionary.ContainsKey("cross_promotion_closed_apps_list"))
				{
					dictionary.Add("cross_promotion_closed_apps_list", "[]");
				}
			}
			return dictionary;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x0000BAC9 File Offset: 0x00009CC9
		internal static void ReloadPromotionsAsync()
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				if (PromotionObject.Instance == null)
				{
					PromotionObject.LoadDataFromFile();
				}
				try
				{
					PromotionManager.SendAppUsageStats();
					PromotionManager.CheckIsUserPremium();
					JToken promotionData = PromotionManager.GetPromotionData();
					if (promotionData != null)
					{
						PromotionManager.SetBootPromotion(promotionData);
						PromotionManager.SetDiscordId(promotionData);
						PromotionManager.SetFeatures(promotionData);
						PromotionManager.SetMyAppsCrossPromotion(promotionData);
						PromotionManager.SetMyAppsBackgroundPromotion(promotionData);
						PromotionManager.SetSearchRecommendations(promotionData);
						PromotionManager.SetAppRecommendations(promotionData);
						PromotionManager.SetStartupTab(promotionData);
						PromotionManager.SetIconOrder(promotionData);
						PromotionManager.ReadQuests(promotionData, false);
						PromotionManager.PopulateAppSpecificRules(promotionData);
						PromotionManager.SetSecurityMetrics(promotionData);
						PromotionManager.SetCustomCursorRuleForApp(promotionData);
					}
					PromotionObject.Save();
					PromotionObject.Instance.PromotionLoaded();
				}
				catch (Exception ex)
				{
					Logger.Info("Error Loading Promotions" + ex.ToString());
				}
			});
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x0000BAF0 File Offset: 0x00009CF0
		private static JToken GetPromotionData()
		{
			return null;
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x00067230 File Offset: 0x00065430
		private static void PopulateAppSpecificRules(JToken res)
		{
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("macro_rules")))
				{
					foreach (JToken jtoken in JArray.Parse(res["macro_rules"].ToString()))
					{
						PromotionObject.Instance.AppSpecificRulesList.Add(jtoken.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in PopulateAppSpecificRules: " + ex.ToString());
			}
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x000672D4 File Offset: 0x000654D4
		private static void SetSearchRecommendations(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("search_recommendations")))
				{
					foreach (KeyValuePair<string, SearchRecommendation> keyValuePair in PromotionObject.Instance.SearchRecommendations)
					{
						keyValuePair.Value.DeleteFile();
					}
					PromotionObject.Instance.SearchRecommendations.ClearSync<string, SearchRecommendation>();
				}
				else
				{
					SerializableDictionary<string, SearchRecommendation> tempDict = new SerializableDictionary<string, SearchRecommendation>();
					foreach (JToken jtoken in JArray.Parse(res["search_recommendations"].ToString()).ToObject<List<JToken>>())
					{
						string value = jtoken.GetValue("app_icon_id");
						if (!JsonExtensions.IsNullOrEmptyBrackets(value))
						{
							SearchRecommendation searchRecommendation;
							if (PromotionObject.Instance.SearchRecommendations.ContainsKey(value))
							{
								searchRecommendation = PromotionObject.Instance.SearchRecommendations[value];
							}
							else
							{
								searchRecommendation = PromotionManager.AddSearchRecommendation(jtoken);
							}
							if (searchRecommendation != null)
							{
								tempDict[searchRecommendation.IconId] = searchRecommendation;
							}
						}
					}
					IEnumerable<string> enumerable = PromotionObject.Instance.SearchRecommendations.Values.Select((SearchRecommendation _) => _.ImagePath);
					Func<string, bool> func;
					Func<string, bool> <>9__1;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (string _) => !tempDict.Values.Select((SearchRecommendation x) => x.ImagePath).Contains(_));
					}
					foreach (string text in enumerable.Where(func))
					{
						try
						{
							File.Delete(text);
						}
						catch (Exception)
						{
						}
					}
					PromotionObject.Instance.SearchRecommendations.ClearAddRange(tempDict);
				}
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.SearchRecommendations.ClearSync<string, SearchRecommendation>();
				Logger.Info("Error Loading Search Recommendations" + ex.ToString());
			}
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x00067548 File Offset: 0x00065748
		private static void SetAppRecommendations(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("app_recommendations")))
				{
					foreach (AppRecommendation appRecommendation in PromotionObject.Instance.AppRecommendations.AppSuggestions)
					{
						appRecommendation.DeleteFile();
					}
					PromotionObject.Instance.AppRecommendations = new AppRecommendationSection();
				}
				else
				{
					List<AppRecommendationSection> list = JsonConvert.DeserializeObject<List<AppRecommendationSection>>(res["app_recommendations"].ToString(), Utils.GetSerializerSettings());
					if (list != null)
					{
						foreach (AppRecommendation appRecommendation2 in list[0].AppSuggestions)
						{
							if (!JsonExtensions.IsNullOrEmptyBrackets(appRecommendation2.IconId))
							{
								string icon = appRecommendation2.Icon;
								string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[] { "AppRecommendation", appRecommendation2.IconId });
								appRecommendation2.ImagePath = Utils.TinyDownloader(icon, text, RegistryStrings.PromotionDirectory, false);
							}
						}
					}
					PromotionObject.Instance.AppRecommendations = list[0];
				}
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.AppRecommendations = new AppRecommendationSection();
				Logger.Info("Error Loading App Recommendations" + ex.ToString());
			}
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x000676E8 File Offset: 0x000658E8
		private static void SetStartupTab(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("startup_tab")))
				{
					PromotionObject.Instance.StartupTab.ClearSync<string, string>();
				}
				else if (!JsonExtensions.IsNullOrEmptyBrackets(res["startup_tab"].GetValue("extra_payload")))
				{
					PromotionObject.Instance.StartupTab.ClearAddRange(res["startup_tab"]["extra_payload"].ToSerializableDictionary<string>());
					PromotionManager.PopulateAndDownloadFavicon(PromotionObject.Instance.StartupTab, "startup_favicon", false);
				}
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.StartupTab.ClearSync<string, string>();
				Logger.Error("Exception while setting the startup tab. " + ex.ToString());
			}
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x000677AC File Offset: 0x000659AC
		public static void PopulateAndDownloadFavicon(IDictionary<string, string> payload, string id, bool redownload = false)
		{
			if (payload.ContainsKey("click_action_app_icon_id"))
			{
				id += payload["click_action_app_icon_id"];
			}
			if (payload.ContainsKey("click_action_app_icon_url"))
			{
				string text = Utils.TinyDownloader(payload["click_action_app_icon_url"], id, RegistryStrings.PromotionDirectory, redownload);
				if (!string.IsNullOrEmpty(text))
				{
					payload["icon_path"] = text;
				}
			}
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x00067814 File Offset: 0x00065A14
		private static void SetIconOrder(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("order")))
				{
					PromotionObject.Instance.SetDefaultOrder(true);
				}
				else
				{
					PromotionManager.SetMyAppsOrder(res["order"]);
					PromotionManager.SetDockOrder(res["order"]);
					PromotionManager.SetMoreAppsOrder(res["order"]);
				}
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.SetDefaultOrder(false);
				Logger.Info("Error Loading icon order" + ex.ToString());
			}
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x000678A8 File Offset: 0x00065AA8
		private static void SetMoreAppsOrder(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("more_apps_order")))
				{
					PromotionObject.Instance.SetDefaultMoreAppsOrder(true);
				}
				else
				{
					PromotionObject.Instance.MoreAppsDockOrder.ClearSync<string, int>();
					foreach (KeyValuePair<string, int> keyValuePair in res["more_apps_order"].ToSerializableDictionary<int>())
					{
						PromotionObject.Instance.MoreAppsDockOrder[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.SetDefaultMoreAppsOrder(true);
				Logger.Info("Error Loading more_apps_order" + ex.ToString());
			}
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x0006797C File Offset: 0x00065B7C
		private static void SetMyAppsOrder(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("myapps_order")))
				{
					PromotionObject.Instance.SetDefaultMyAppsOrder(true);
				}
				else
				{
					PromotionObject.Instance.MyAppsOrder.ClearSync<string, int>();
					foreach (KeyValuePair<string, int> keyValuePair in res["myapps_order"].ToSerializableDictionary<int>())
					{
						PromotionObject.Instance.MyAppsOrder[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.SetDefaultMyAppsOrder(true);
				Logger.Info("Error Loading My apps order" + ex.ToString());
			}
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x00067A50 File Offset: 0x00065C50
		private static void SetDiscordId(JToken res)
		{
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("discord_client_id")))
				{
					PromotionObject.Instance.DiscordClientID = res.GetValue("discord_client_id");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while getting discord id : {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x00067AB4 File Offset: 0x00065CB4
		private static void SetFeatures(JToken res)
		{
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("is_root_access_enabled")))
				{
					PromotionObject.Instance.IsRootAccessEnabled = res["is_root_access_enabled"].ToObject<bool>();
				}
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("is_timeline_stats4_enabled")))
				{
					RegistryManager.Instance.IsTimelineStats4Enabled = res["is_timeline_stats4_enabled"].ToObject<bool>();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in SetFeatures: {0}", new object[] { ex });
			}
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("geo")))
				{
					string text = res["geo"].ToString();
					if (!string.IsNullOrEmpty(text))
					{
						RegistryManager.Instance.Geo = text;
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Error while getting geo feature: {0}", new object[] { ex2 });
			}
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x00067BA0 File Offset: 0x00065DA0
		private static void SetDockOrder(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("dock_order")))
				{
					PromotionObject.Instance.SetDefaultDockOrder(true);
				}
				else
				{
					PromotionObject.Instance.DockOrder.ClearSync<string, int>();
					JToken jtoken = res["dock_order"];
					SerializableDictionary<string, int> serializableDictionary = ((jtoken != null) ? jtoken.ToSerializableDictionary<int>() : null);
					if (serializableDictionary != null && serializableDictionary.Count > 0)
					{
						using (Dictionary<string, int>.Enumerator enumerator = serializableDictionary.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<string, int> keyValuePair = enumerator.Current;
								PromotionObject.Instance.DockOrder[keyValuePair.Key] = keyValuePair.Value;
							}
							goto IL_00A7;
						}
					}
					PromotionObject.Instance.SetDefaultDockOrder(true);
				}
				IL_00A7:;
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.SetDefaultDockOrder(true);
				Logger.Info("Error Loading dock order" + ex.ToString());
			}
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x00067C98 File Offset: 0x00065E98
		private static void SetBootPromotion(JToken res)
		{
			try
			{
				if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("boot_promotion_obj")))
				{
					foreach (KeyValuePair<string, BootPromotion> keyValuePair in PromotionObject.Instance.DictBootPromotions)
					{
						keyValuePair.Value.DeleteFile();
					}
					PromotionObject.Instance.DictBootPromotions.ClearSync<string, BootPromotion>();
				}
				else
				{
					JToken jtoken = JToken.Parse(res.GetValue("boot_promotion_obj"));
					if (jtoken["boot_promotion_display_time"] != null)
					{
						PromotionObject.Instance.BootPromoDisplaytime = jtoken["boot_promotion_display_time"].ToObject<int>();
					}
					SerializableDictionary<string, BootPromotion> serializableDictionary = new SerializableDictionary<string, BootPromotion>();
					foreach (JToken jtoken2 in JArray.Parse(jtoken["boot_promotion_images"].ToString()))
					{
						string value = jtoken2.GetValue("id");
						if (!JsonExtensions.IsNullOrEmptyBrackets(value))
						{
							BootPromotion bootPromotion;
							if (PromotionObject.Instance.DictBootPromotions.ContainsKey(value))
							{
								bootPromotion = PromotionObject.Instance.DictBootPromotions[value];
							}
							else
							{
								bootPromotion = PromotionManager.AddBootPromotion(jtoken2);
							}
							if (bootPromotion != null)
							{
								serializableDictionary[bootPromotion.Id] = bootPromotion;
							}
						}
					}
					PromotionObject.Instance.DictBootPromotions.ClearAddRange(serializableDictionary);
				}
			}
			catch (Exception ex)
			{
				PromotionObject.Instance.DictBootPromotions.ClearSync<string, BootPromotion>();
				Logger.Info("Error Loading Boot Promotions" + ex.ToString());
			}
			PromotionObject.mIsBootPromotionLoading = false;
			EventHandler bootPromotionHandler = PromotionObject.BootPromotionHandler;
			if (bootPromotionHandler != null)
			{
				bootPromotionHandler(PromotionObject.Instance, new EventArgs());
			}
			try
			{
				foreach (KeyValuePair<string, BootPromotion> keyValuePair2 in PromotionObject.Instance.DictOldBootPromotions)
				{
					if (!PromotionObject.Instance.DictBootPromotions.ContainsKey(keyValuePair2.Key))
					{
						keyValuePair2.Value.DeleteFile();
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Warning("Error Loading myapp cross Promotions" + ex2.ToString());
			}
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x00067F30 File Offset: 0x00066130
		private static void SetMyAppsBackgroundPromotion(JToken res)
		{
			bool flag = false;
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("myapps_background_id")))
				{
					if (!string.Equals(PromotionObject.Instance.BackgroundPromotionID, res.GetValue("myapps_background_id"), StringComparison.InvariantCulture))
					{
						PromotionObject.Instance.BackgroundPromotionID = res.GetValue("myapps_background_id");
						PromotionObject.Instance.BackgroundPromotionImagePath = Utils.TinyDownloader(res.GetValue("myapps_background_url"), "BackPromo", RegistryStrings.PromotionDirectory, true);
					}
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Error Loading myapp background Promotions" + ex.ToString());
				flag = true;
			}
			if (flag)
			{
				PromotionObject.Instance.BackgroundPromotionID = "";
				PromotionObject.Instance.BackgroundPromotionImagePath = "";
				IOUtils.DeleteIfExists(new List<string> { Path.Combine(RegistryStrings.PromotionDirectory, "BackPromo") });
			}
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00068018 File Offset: 0x00066218
		internal static void SetMyAppsCrossPromotion(JToken res)
		{
			bool flag = false;
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("myapps_cross_promotion")))
				{
					List<AppSuggestionPromotion> list = res["myapps_cross_promotion"].ToObject<IEnumerable<AppSuggestionPromotion>>().ToList<AppSuggestionPromotion>();
					if (list == null)
					{
						list = new List<AppSuggestionPromotion>();
					}
					else
					{
						using (IEnumerator<JToken> enumerator = JArray.Parse(res["myapps_cross_promotion"].ToString()).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								JToken x = enumerator.Current;
								if (x["extra_payload"] != null && x["app_icon_id"] != null)
								{
									list.Where((AppSuggestionPromotion _) => _.AppIconId == x["app_icon_id"].ToString()).First<AppSuggestionPromotion>().ExtraPayload.ClearAddRange(x["extra_payload"].ToSerializableDictionary<string>());
								}
							}
						}
					}
					object obj = ((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot;
					lock (obj)
					{
						using (List<AppSuggestionPromotion>.Enumerator enumerator2 = PromotionObject.Instance.AppSuggestionList.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								AppSuggestionPromotion item = enumerator2.Current;
								if (!list.Any((AppSuggestionPromotion _) => string.Equals(_.AppIconId, item.AppIconId, StringComparison.InvariantCulture)))
								{
									IOUtils.DeleteIfExists(new List<string> { Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + item.AppIconId) });
									PromotionManager.DeleteFavicon(item.ExtraPayload);
								}
								if (!list.Any((AppSuggestionPromotion _) => string.Equals(_.IconBorderId, item.IconBorderId, StringComparison.InvariantCulture)))
								{
									IOUtils.DeleteIfExists(new List<string>
									{
										Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border.png"),
										Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border_hover.png"),
										Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border_click.png")
									});
								}
							}
						}
						PromotionObject.Instance.AppSuggestionList.ClearAddRange(list);
						foreach (AppSuggestionPromotion appSuggestionPromotion in PromotionObject.Instance.AppSuggestionList)
						{
							appSuggestionPromotion.AppIconPath = Utils.TinyDownloader(appSuggestionPromotion.AppIcon, "AppSuggestion" + appSuggestionPromotion.AppIconId, RegistryStrings.PromotionDirectory, false);
							if (!string.IsNullOrEmpty(appSuggestionPromotion.IconBorderId) && appSuggestionPromotion.IsIconBorder)
							{
								Utils.TinyDownloader(appSuggestionPromotion.IconBorderUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border.png", RegistryStrings.PromotionDirectory, false);
								Utils.TinyDownloader(appSuggestionPromotion.IconBorderHoverUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border_hover.png", RegistryStrings.PromotionDirectory, false);
								Utils.TinyDownloader(appSuggestionPromotion.IconBorderClickUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border_click.png", RegistryStrings.PromotionDirectory, false);
							}
						}
						goto IL_0322;
					}
				}
				flag = true;
				IL_0322:;
			}
			catch (Exception ex)
			{
				Logger.Info("Error Loading myapp cross Promotions" + ex.ToString());
				flag = true;
			}
			if (flag)
			{
				object obj = ((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot;
				lock (obj)
				{
					PromotionObject.Instance.AppSuggestionList.ClearSync<AppSuggestionPromotion>();
				}
				flag = false;
			}
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x0000BAF3 File Offset: 0x00009CF3
		private static void DeleteFavicon(IDictionary<string, string> payload)
		{
			if (payload.ContainsKey("favicon_path"))
			{
				IOUtils.DeleteIfExists(new List<string> { payload["favicon_path"] });
			}
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x00068430 File Offset: 0x00066630
		internal static void ReadQuests(JToken res, bool writePromo)
		{
			bool flag = false;
			SerializableDictionary<string, long[]> serializableDictionary = new SerializableDictionary<string, long[]>();
			SerializableDictionary<string, long> serializableDictionary2 = new SerializableDictionary<string, long>();
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("quest")))
				{
					PromotionObject.Instance.QuestName = res["quest"].GetValue("quest_name");
					PromotionObject.Instance.QuestActionType = res["quest"].GetValue("action_type");
					List<QuestRule> list = res["quest"]["details"].ToObject<IEnumerable<QuestRule>>().ToList<QuestRule>();
					using (List<QuestRule>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							QuestRule rule = enumerator.Current;
							if (PromotionObject.Instance.QuestRules.Any((QuestRule _) => string.Equals(_.RuleId, rule.RuleId, StringComparison.InvariantCulture)))
							{
								if (!serializableDictionary2.ContainsKey(rule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
								{
									serializableDictionary2[rule.AppPackage.ToLower(CultureInfo.InvariantCulture)] = long.MaxValue;
								}
								if (PromotionObject.Instance.ResetQuestRules.ContainsKey(rule.RuleId))
								{
									serializableDictionary.Add(rule.RuleId, PromotionObject.Instance.ResetQuestRules[rule.RuleId]);
								}
							}
							else
							{
								serializableDictionary2[rule.AppPackage] = 0L;
								long totalTimeForPackageAcrossInstances = AppUsageTimer.GetTotalTimeForPackageAcrossInstances(rule.AppPackage);
								long num = 0L;
								if (PromotionManager.combinedPackages.ContainsKey(rule.AppPackage))
								{
									num = PromotionManager.combinedPackages[rule.AppPackage];
								}
								serializableDictionary.Add(rule.RuleId, new long[] { num, totalTimeForPackageAcrossInstances });
							}
						}
					}
					PromotionObject.Instance.QuestRules.ClearAddRange(list);
					PromotionObject.Instance.ResetQuestRules.ClearAddRange(serializableDictionary);
					PromotionObject.Instance.QuestHdPlayerRules.ClearAddRange(serializableDictionary2);
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Error Loading promotion quests" + ex.ToString());
				flag = true;
			}
			if (flag)
			{
				PromotionObject.Instance.QuestName = "";
				PromotionObject.Instance.QuestActionType = "";
				PromotionObject.Instance.QuestRules.ClearSync<QuestRule>();
				PromotionObject.Instance.QuestHdPlayerRules.ClearSync<string, long>();
				flag = false;
			}
			if (writePromo)
			{
				PromotionObject.Save();
				Action questHandler = PromotionObject.QuestHandler;
				if (questHandler == null)
				{
					return;
				}
				questHandler();
			}
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x00014A24 File Offset: 0x00012C24
		private static T DoDeserialize<T>(string data) where T : class
		{
			T t;
			using (XmlReader xmlReader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(data))))
			{
				t = (T)((object)new XmlSerializer(typeof(T)).Deserialize(xmlReader));
			}
			return t;
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x0006870C File Offset: 0x0006690C
		internal static void AddNewMyAppsCrossPromotion(JToken res)
		{
			try
			{
				AppSuggestionPromotion appSuggestionPromotion = res["myapps_cross_promotion"].ToObject<AppSuggestionPromotion>();
				if (appSuggestionPromotion != null && res["myapps_cross_promotion"]["extra_payload"] != null && res["myapps_cross_promotion"]["app_icon_id"] != null)
				{
					appSuggestionPromotion.ExtraPayload.ClearAddRange(res["myapps_cross_promotion"]["extra_payload"].ToSerializableDictionary<string>());
					PromotionManager.PopulateAndDownloadFavicon(appSuggestionPromotion.ExtraPayload, "AppSuggestion", false);
				}
				List<AppSuggestionPromotion> list = new List<AppSuggestionPromotion>();
				object syncRoot = ((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot;
				lock (syncRoot)
				{
					foreach (AppSuggestionPromotion appSuggestionPromotion2 in PromotionObject.Instance.AppSuggestionList)
					{
						if (string.Equals(appSuggestionPromotion.AppIconId, appSuggestionPromotion2.AppIconId, StringComparison.InvariantCulture))
						{
							list.Add(appSuggestionPromotion2);
							IOUtils.DeleteIfExists(new List<string> { Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + appSuggestionPromotion2.AppIconId) });
						}
					}
					foreach (AppSuggestionPromotion appSuggestionPromotion3 in list)
					{
						PromotionObject.Instance.AppSuggestionList.Remove(appSuggestionPromotion3);
					}
					appSuggestionPromotion.AppIconPath = Utils.TinyDownloader(appSuggestionPromotion.AppIcon, "AppSuggestion" + appSuggestionPromotion.AppIconId, RegistryStrings.PromotionDirectory, false);
					if (!string.IsNullOrEmpty(appSuggestionPromotion.IconBorderId) && appSuggestionPromotion.IsIconBorder)
					{
						Utils.TinyDownloader(appSuggestionPromotion.IconBorderUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border.png", RegistryStrings.PromotionDirectory, false);
						Utils.TinyDownloader(appSuggestionPromotion.IconBorderHoverUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border_hover.png", RegistryStrings.PromotionDirectory, false);
						Utils.TinyDownloader(appSuggestionPromotion.IconBorderClickUrl, appSuggestionPromotion.IconBorderId + "app_suggestion_icon_border_click.png", RegistryStrings.PromotionDirectory, false);
					}
					PromotionObject.Instance.AppSuggestionList.Add(appSuggestionPromotion);
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Error Loading myapp cross Promotions by notification: " + ex.ToString());
			}
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x000689A4 File Offset: 0x00066BA4
		internal static void CheckIsUserPremium()
		{
			string registeredEmail = RegistryManager.Instance.RegisteredEmail;
			string token = RegistryManager.Instance.Token;
			string userGuid = RegistryManager.Instance.UserGuid;
			string version = RegistryManager.Instance.Version;
			string clientVersion = RegistryManager.Instance.ClientVersion;
			string text = "bgp64";
			if (!string.IsNullOrEmpty(registeredEmail) && !string.IsNullOrEmpty(token))
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "{0}/bs-accounts/getuser?email={1}&guid={2}&token={3}&eng_ver={4}&client_ver={5}&oem={6}", new object[]
				{
					RegistryManager.Instance.Host,
					registeredEmail,
					userGuid,
					token,
					version,
					clientVersion,
					text
				});
				string text3;
				for (;;)
				{
					try
					{
						text3 = BstHttpClient.Get(text2, null, false, "", 0, 1, 0, false, "bgp64");
					}
					catch
					{
						Thread.Sleep(20000);
						continue;
					}
					break;
				}
				Logger.Debug("Response string from cloud for bs-accounts/getuser : " + text3);
				try
				{
					JObject jobject = JObject.Parse(text3);
					if (string.Equals(jobject["status"].ToString().Trim(), "success", StringComparison.InvariantCulture))
					{
						RegistryManager.Instance.RegisteredEmail = jobject["message"]["email"].ToString().Trim();
						if (string.Compare(jobject["message"]["subscription_status"].ToString().Trim(), "PAID", StringComparison.OrdinalIgnoreCase) == 0)
						{
							RegistryManager.Instance.IsPremium = true;
						}
						else
						{
							RegistryManager.Instance.IsPremium = false;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to parse string received from cloud... Err : " + ex.ToString());
				}
			}
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00068B54 File Offset: 0x00066D54
		internal static void StartQuestRulesProcessor()
		{
			foreach (QuestRule questRule in PromotionObject.Instance.QuestRules)
			{
				if (questRule.IsRecurring)
				{
					if (!PromotionManager.mDictRecurringCount.ContainsKey(questRule.RuleId))
					{
						PromotionManager.mDictRecurringCount.Add(questRule.RuleId, questRule.RecurringCount);
					}
					else
					{
						PromotionManager.mDictRecurringCount[questRule.RuleId] = questRule.RecurringCount;
					}
				}
			}
			if (PromotionManager.mQuestTimer.Enabled)
			{
				if (PromotionObject.Instance.QuestHdPlayerRules.Count == 0)
				{
					PromotionManager.mQuestTimer.Stop();
					return;
				}
			}
			else if (PromotionObject.Instance.QuestHdPlayerRules.Count > 0)
			{
				PromotionManager.mQuestTimer.Elapsed -= PromotionManager.QuestTimer_Elapsed;
				PromotionManager.mQuestTimer.Elapsed += PromotionManager.QuestTimer_Elapsed;
				PromotionManager.mQuestTimer.Start();
			}
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x00068C5C File Offset: 0x00066E5C
		private static void QuestTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				PromotionManager.combinedPackages.Clear();
				foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
				{
					string text = mainWindow.mFrontendHandler.SendFrontendRequest("getInteractionForPackage", null);
					if (!string.IsNullOrEmpty(text))
					{
						Logger.Debug("Package interaction Json received from frontend: " + text);
						foreach (KeyValuePair<string, long> keyValuePair in (JToken.Parse(text).ToDictionary<long>() as Dictionary<string, long>))
						{
							if (PromotionManager.combinedPackages.ContainsKey(keyValuePair.Key))
							{
								Dictionary<string, long> dictionary = PromotionManager.combinedPackages;
								string key = keyValuePair.Key;
								dictionary[key] += keyValuePair.Value;
							}
							else
							{
								PromotionManager.combinedPackages.Add(keyValuePair.Key, keyValuePair.Value);
							}
						}
					}
				}
				List<QuestRuleState> list = new List<QuestRuleState>();
				Dictionary<string, long> dictionary2 = new Dictionary<string, long>();
				List<QuestRule> list2 = new List<QuestRule>();
				string text2 = string.Empty;
				foreach (QuestRule questRule in PromotionObject.Instance.QuestRules.Where((QuestRule _) => !PromotionManager.mRuleIdAlreadyPassed.Contains(_.RuleId)))
				{
					if (PromotionManager.combinedPackages.ContainsKey(questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
					{
						if (PromotionManager.combinedPackages.ContainsKey("?"))
						{
							foreach (KeyValuePair<string, long> keyValuePair2 in PromotionManager.combinedPackages)
							{
								if (!keyValuePair2.Key.ToString(CultureInfo.InvariantCulture).Equals("?", StringComparison.OrdinalIgnoreCase))
								{
									text2 = keyValuePair2.Key.ToString(CultureInfo.InvariantCulture);
								}
							}
						}
						if ((long)questRule.MinUserInteraction <= PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)] - PromotionObject.Instance.ResetQuestRules[questRule.RuleId][0])
						{
							list2.Add(questRule);
							if (dictionary2.ContainsKey(questRule.AppPackage))
							{
								dictionary2[questRule.AppPackage] = PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)];
							}
							else
							{
								dictionary2.Add(questRule.AppPackage, PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)]);
							}
							Logger.Debug("Interaction rule passed for package " + questRule.AppPackage + PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)].ToString());
						}
					}
				}
				foreach (QuestRule questRule2 in list2.Where((QuestRule _) => !PromotionManager.mRuleIdAlreadyPassed.Contains(_.RuleId)))
				{
					QuestRuleState questRuleState = new QuestRuleState();
					if (string.Equals(questRule2.AppPackage, "*", StringComparison.InvariantCulture))
					{
						long totalTimeForAllPackages = AppUsageTimer.GetTotalTimeForAllPackages();
						if ((long)questRule2.AppUsageTime <= totalTimeForAllPackages - PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][1])
						{
							questRuleState.TotalTime = totalTimeForAllPackages;
							questRuleState.QuestRules = questRule2;
							questRuleState.Interaction = dictionary2[questRule2.AppPackage];
							list.Add(questRuleState);
							if (PromotionManager.combinedPackages.ContainsKey(questRule2.AppPackage.ToLower(CultureInfo.InvariantCulture)))
							{
								PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][0] = PromotionManager.combinedPackages[questRule2.AppPackage.ToLower(CultureInfo.InvariantCulture)];
								PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][1] = questRuleState.TotalTime;
							}
						}
					}
					else if (string.Equals(questRule2.AppPackage, "?", StringComparison.InvariantCulture))
					{
						long totalTimeForPackageAfterReset = AppUsageTimer.GetTotalTimeForPackageAfterReset(text2);
						if ((long)questRule2.AppUsageTime <= totalTimeForPackageAfterReset - PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][1])
						{
							questRuleState.TotalTime = totalTimeForPackageAfterReset;
							questRuleState.QuestRules = questRule2;
							questRuleState.Interaction = dictionary2[questRule2.AppPackage];
							list.Add(questRuleState);
							if (PromotionManager.combinedPackages.ContainsKey(questRule2.AppPackage.ToLower(CultureInfo.InvariantCulture)))
							{
								PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][0] = PromotionManager.combinedPackages[questRule2.AppPackage.ToLower(CultureInfo.InvariantCulture)];
								PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][1] = questRuleState.TotalTime;
							}
						}
					}
					else
					{
						long totalTimeForPackageAfterReset2 = AppUsageTimer.GetTotalTimeForPackageAfterReset(questRule2.AppPackage.ToLower(CultureInfo.InvariantCulture));
						if ((long)questRule2.AppUsageTime <= totalTimeForPackageAfterReset2 - PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][1])
						{
							questRuleState.TotalTime = totalTimeForPackageAfterReset2;
							questRuleState.QuestRules = questRule2;
							questRuleState.Interaction = dictionary2[questRule2.AppPackage];
							list.Add(questRuleState);
							if (PromotionManager.combinedPackages.ContainsKey(questRule2.AppPackage.ToLower(CultureInfo.InvariantCulture)))
							{
								PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][0] = PromotionManager.combinedPackages[questRule2.AppPackage.ToLower(CultureInfo.InvariantCulture)];
								PromotionObject.Instance.ResetQuestRules[questRule2.RuleId][1] = questRuleState.TotalTime;
							}
						}
					}
				}
				if (list.Count > 0)
				{
					SerializableDictionary<string, long> serializableDictionary = new SerializableDictionary<string, long>();
					bool flag = false;
					foreach (QuestRule questRule3 in PromotionObject.Instance.QuestRules)
					{
						serializableDictionary[questRule3.AppPackage.ToLower(CultureInfo.InvariantCulture)] = long.MaxValue;
					}
					using (List<QuestRuleState>.Enumerator enumerator5 = list.GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							QuestRuleState ruleState = enumerator5.Current;
							string text3 = ruleState.QuestRules.CloudHandler;
							string jsonobjectString = JSONUtils.GetJSONObjectString(AppUsageTimer.GetRealtimeDictionary());
							if (string.IsNullOrEmpty(text3))
							{
								text3 = "/pika_points/quest_rule_accomplished";
							}
							string text4 = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
							{
								WebHelper.GetServerHost(),
								text3
							}));
							text4 += string.Format(CultureInfo.InvariantCulture, "&email={5}&quest_name={0}&rule_id={1}&app_pkg={2}&usage_time={3}&user_interactions={4}&usage_data={6}", new object[]
							{
								PromotionObject.Instance.QuestName,
								ruleState.QuestRules.RuleId,
								ruleState.QuestRules.AppPackage,
								ruleState.TotalTime,
								ruleState.Interaction,
								RegistryManager.Instance.RegisteredEmail,
								jsonobjectString
							});
							int i = 3;
							while (i > 0)
							{
								try
								{
									string text5 = BstHttpClient.Get(text4, null, false, string.Empty, 5000, 1, 0, false, "bgp64");
									Logger.Info("Quest rule passed response from cloud " + text5 + " ruleId: " + ruleState.QuestRules.RuleId);
									break;
								}
								catch (Exception ex)
								{
									Logger.Warning("Exception while calling cloud for quest rule passed, RETRYING " + i.ToString() + Environment.NewLine + ex.ToString());
									i--;
									Thread.Sleep(1000);
								}
							}
							if (i == 0)
							{
								Logger.Error("Could not send quest rule passed, to cloud after retries.");
							}
							if (!ruleState.QuestRules.IsRecurring)
							{
								PromotionManager.mRuleIdAlreadyPassed.Add(ruleState.QuestRules.RuleId);
							}
							else if (PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] == -1)
							{
								if (string.Equals(ruleState.QuestRules.AppPackage, "*", StringComparison.InvariantCulture))
								{
									AppUsageTimer.GetTotalTimeForAllPackages();
								}
								else
								{
									AppUsageTimer.GetTotalTimeForPackageAfterReset(ruleState.QuestRules.AppPackage);
								}
								if (PromotionObject.Instance.QuestRules.Any((QuestRule _) => string.Equals(_.RuleId, ruleState.QuestRules.RuleId, StringComparison.InvariantCulture)) && serializableDictionary.ContainsKey(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)))
								{
									if (ruleState.QuestRules.RecurringCount != -1)
									{
										QuestRule questRules = ruleState.QuestRules;
										int num = questRules.RecurringCount;
										questRules.RecurringCount = num - 1;
									}
									serializableDictionary[ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)] = 0L;
									flag = true;
								}
							}
							else
							{
								PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] = PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] - 1;
								if (PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] == 0)
								{
									PromotionManager.mRuleIdAlreadyPassed.Add(ruleState.QuestRules.RuleId);
								}
								else if (PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] > 0)
								{
									if (string.Equals(ruleState.QuestRules.AppPackage, "*", StringComparison.InvariantCulture))
									{
										long totalTimeForAllPackages2 = AppUsageTimer.GetTotalTimeForAllPackages();
										AppUsageTimer.AddPackageForReset("*", totalTimeForAllPackages2);
									}
									else
									{
										long totalTimeForPackageAfterReset3 = AppUsageTimer.GetTotalTimeForPackageAfterReset(ruleState.QuestRules.AppPackage);
										AppUsageTimer.AddPackageForReset(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture), totalTimeForPackageAfterReset3);
									}
									if (PromotionObject.Instance.QuestRules.Any((QuestRule _) => string.Equals(_.RuleId, ruleState.QuestRules.RuleId, StringComparison.InvariantCulture)) && serializableDictionary.ContainsKey(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)))
									{
										if (ruleState.QuestRules.RecurringCount != -1)
										{
											QuestRule questRules2 = ruleState.QuestRules;
											int num = questRules2.RecurringCount;
											questRules2.RecurringCount = num - 1;
										}
										serializableDictionary[ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)] = 0L;
										flag = true;
									}
								}
							}
						}
					}
					if (flag)
					{
						PromotionObject.Instance.QuestHdPlayerRules.ClearAddRange(serializableDictionary);
						PromotionObject.Save();
						Action questHandler = PromotionObject.QuestHandler;
						if (questHandler != null)
						{
							questHandler();
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception in QuestTimer_Elapsed " + ex2.ToString());
			}
			try
			{
				List<GenericNotificationItem> list3 = new List<GenericNotificationItem>();
				foreach (KeyValuePair<GenericNotificationItem, long> keyValuePair3 in PromotionManager.sDeferredNotificationsList)
				{
					if (AppUsageTimer.GetTotalTimeForPackageAfterReset(keyValuePair3.Key.DeferredApp.ToLower(CultureInfo.InvariantCulture)) - keyValuePair3.Value >= keyValuePair3.Key.DeferredAppUsage)
					{
						if (string.Equals(BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mTopBar.mAppTabButtons.SelectedTab.PackageName, keyValuePair3.Key.DeferredApp, StringComparison.InvariantCulture))
						{
							BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].HandleGenericNotificationPopup(keyValuePair3.Key);
							GenericNotificationManager.AddNewNotification(keyValuePair3.Key, false);
							BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].Dispatcher.Invoke(new Action(delegate
							{
								BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mTopBar.RefreshNotificationCentreButton();
							}), new object[0]);
							list3.Add(keyValuePair3.Key);
						}
						else
						{
							PromotionManager.sPassedDeferredNotificationsList.Add(keyValuePair3.Key);
						}
					}
				}
				foreach (GenericNotificationItem genericNotificationItem in list3)
				{
					PromotionManager.sDeferredNotificationsList.Remove(genericNotificationItem);
				}
			}
			catch (Exception ex3)
			{
				Logger.Error("Exception in checking deferred notification: " + ex3.ToString());
			}
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x00069A60 File Offset: 0x00067C60
		private static void SetSecurityMetrics(JToken res)
		{
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("security_metrics_enable_user")))
				{
					PromotionObject.Instance.IsSecurityMetricsEnable = res["security_metrics_enable_user"].ToObject<bool>();
				}
				else
				{
					PromotionObject.Instance.IsSecurityMetricsEnable = false;
				}
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("security_metrics_blacklisted_apps")))
				{
					PromotionObject.Instance.BlackListedApplicationsList.ClearSync<string>();
					using (IEnumerator<JToken> enumerator = JArray.Parse(res["security_metrics_blacklisted_apps"].ToString()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JToken jtoken = enumerator.Current;
							PromotionObject.Instance.BlackListedApplicationsList.Add(jtoken.ToString());
						}
						goto IL_00B6;
					}
				}
				PromotionObject.Instance.BlackListedApplicationsList.ClearSync<string>();
				IL_00B6:;
			}
			catch (Exception ex)
			{
				Logger.Error("Error while getting security metrics info: {0}", new object[] { ex.ToString() });
				PromotionObject.Instance.IsSecurityMetricsEnable = false;
				PromotionObject.Instance.BlackListedApplicationsList.ClearSync<string>();
			}
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00069B78 File Offset: 0x00067D78
		private static void SetCustomCursorRuleForApp(JToken res)
		{
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("exclude_custom_cursor")))
				{
					PromotionObject.Instance.CustomCursorExcludedAppsList.ClearSync<string>();
					using (IEnumerator<JToken> enumerator = JArray.Parse(res["exclude_custom_cursor"].ToString()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JToken jtoken = enumerator.Current;
							PromotionObject.Instance.CustomCursorExcludedAppsList.Add(jtoken.ToString());
						}
						goto IL_007D;
					}
				}
				PromotionObject.Instance.CustomCursorExcludedAppsList.ClearSync<string>();
				IL_007D:;
			}
			catch (Exception ex)
			{
				Logger.Error("Error while getting custom cursor exclude list of apps: {0}", new object[] { ex });
			}
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00069C38 File Offset: 0x00067E38
		private static string AddSamsungStoreParamsIfPresent(string url)
		{
			try
			{
				url = url + "&samsung_store_present=" + RegistryManager.Instance.IsSamsungStorePresent.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to add samsung store parameter. Ex : " + ex.ToString());
			}
			return url;
		}

		// Token: 0x04000A75 RID: 2677
		internal static Dictionary<string, long> combinedPackages = new Dictionary<string, long>();

		// Token: 0x04000A76 RID: 2678
		private static global::System.Timers.Timer mQuestTimer = new global::System.Timers.Timer(15000.0);

		// Token: 0x04000A77 RID: 2679
		private static Dictionary<string, int> mDictRecurringCount = new Dictionary<string, int>();

		// Token: 0x04000A78 RID: 2680
		private static List<string> mRuleIdAlreadyPassed = new List<string>();

		// Token: 0x04000A79 RID: 2681
		internal static Dictionary<GenericNotificationItem, long> sDeferredNotificationsList = new Dictionary<GenericNotificationItem, long>();

		// Token: 0x04000A7A RID: 2682
		internal static List<GenericNotificationItem> sPassedDeferredNotificationsList = new List<GenericNotificationItem>();
	}
}
