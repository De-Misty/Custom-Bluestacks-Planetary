using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200008D RID: 141
	internal sealed class GuidanceCloudInfoManager
	{
		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x000060CF File Offset: 0x000042CF
		private static string BstGuidanceFilePath
		{
			get
			{
				return Path.Combine(RegistryStrings.PromotionDirectory, "bst_guidance");
			}
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x000060E0 File Offset: 0x000042E0
		private GuidanceCloudInfoManager()
		{
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x00023F84 File Offset: 0x00022184
		// (set) Token: 0x0600061B RID: 1563 RVA: 0x000060F3 File Offset: 0x000042F3
		public static GuidanceCloudInfoManager Instance
		{
			get
			{
				if (GuidanceCloudInfoManager.sInstance == null)
				{
					object obj = GuidanceCloudInfoManager.sLock;
					lock (obj)
					{
						if (GuidanceCloudInfoManager.sInstance == null)
						{
							GuidanceCloudInfoManager.sInstance = new GuidanceCloudInfoManager();
						}
					}
				}
				return GuidanceCloudInfoManager.sInstance;
			}
			set
			{
				GuidanceCloudInfoManager.sInstance = value;
			}
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00023FD4 File Offset: 0x000221D4
		private static JToken GetGuidanceCloudInfoData()
		{
			JToken jtoken = null;
			try
			{
				string urlWithParams = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", new object[]
				{
					RegistryManager.Instance.Host,
					"bs4",
					"guidance_window"
				}));
				string text = BstHttpClient.Post(urlWithParams, new Dictionary<string, string> { 
				{
					"app_pkgs",
					GuidanceCloudInfoManager.GetInstalledAppDataFromAllVms()
				} }, null, false, "Android", 0, 1, 0, false, "bgp64");
				Logger.Debug("Guidance Cloud Info Url: " + urlWithParams);
				Logger.Debug("Guidance Cloud Info Response: " + text);
				jtoken = JToken.Parse(text);
			}
			catch (Exception ex)
			{
				Logger.Warning("Error Getting GetGuidanceCloudInfoData " + ex.ToString());
			}
			return jtoken;
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0002409C File Offset: 0x0002229C
		private static string GetInstalledAppDataFromAllVms()
		{
			string[] vmList = RegistryManager.Instance.VmList;
			JArray jarray = new JArray();
			try
			{
				string[] array = vmList;
				for (int i = 0; i < array.Length; i++)
				{
					foreach (AppInfo appInfo in new JsonParser(array[i]).GetAppList().ToList<AppInfo>())
					{
						string package = appInfo.Package;
						jarray.Add(package);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in getting all installed apps from all Vms: {0}", new object[] { ex.ToString() });
			}
			return jarray.ToString(Formatting.None, new JsonConverter[0]);
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x000060FB File Offset: 0x000042FB
		internal void AppsGuidanceCloudInfoRefresh()
		{
			new Thread(delegate
			{
				if (File.Exists(GuidanceCloudInfoManager.BstGuidanceFilePath))
				{
					this.mGuidanceCloudInfo = JsonConvert.DeserializeObject<GuidanceCloudInfo>(File.ReadAllText(GuidanceCloudInfoManager.BstGuidanceFilePath), Utils.GetSerializerSettings());
				}
				JToken guidanceCloudInfoData = GuidanceCloudInfoManager.GetGuidanceCloudInfoData();
				if (guidanceCloudInfoData != null)
				{
					GuidanceCloudInfo guidanceCloudInfo = new GuidanceCloudInfo();
					GuidanceCloudInfoManager.SetAppsVideoThumbnail(guidanceCloudInfo, guidanceCloudInfoData);
					GuidanceCloudInfoManager.SetAppsReadArticle(guidanceCloudInfo, guidanceCloudInfoData);
					GuidanceCloudInfoManager.SetGameSettings(guidanceCloudInfo, guidanceCloudInfoData);
					GuidanceCloudInfoManager.SaveToFile(guidanceCloudInfo);
					this.mGuidanceCloudInfo = guidanceCloudInfo;
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00024164 File Offset: 0x00022364
		private static void SaveToFile(GuidanceCloudInfo guidanceCloudInfo)
		{
			try
			{
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = Formatting.Indented;
				string text = JsonConvert.SerializeObject(guidanceCloudInfo, serializerSettings);
				if (!Directory.Exists(RegistryStrings.PromotionDirectory))
				{
					Directory.CreateDirectory(RegistryStrings.PromotionDirectory);
				}
				File.WriteAllText(GuidanceCloudInfoManager.BstGuidanceFilePath, text);
			}
			catch (Exception)
			{
				Logger.Warning("Error in saving GuidanceCloudInfo to file");
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x000241C8 File Offset: 0x000223C8
		private static void SetAppsVideoThumbnail(GuidanceCloudInfo currentAppsGuidanceCloudInfo, JToken res)
		{
			try
			{
				foreach (JToken jtoken in JArray.Parse(res.GetValue("custom_thumbnails").ToString(CultureInfo.InvariantCulture)))
				{
					CustomThumbnail customThumbnail = JsonConvert.DeserializeObject<CustomThumbnail>(jtoken.ToString(), Utils.GetSerializerSettings());
					foreach (object obj in Enum.GetValues(typeof(GuidanceVideoType)))
					{
						GuidanceVideoType guidanceVideoType = (GuidanceVideoType)obj;
						if (guidanceVideoType == GuidanceVideoType.SchemeSpecific)
						{
							using (Dictionary<string, VideoThumbnailInfo>.Enumerator enumerator3 = ((Dictionary<string, VideoThumbnailInfo>)customThumbnail[guidanceVideoType.ToString()]).GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									KeyValuePair<string, VideoThumbnailInfo> keyValuePair = enumerator3.Current;
									VideoThumbnailInfo value = keyValuePair.Value;
									value.ThumbnailType = guidanceVideoType;
									value.ImagePath = Utils.TinyDownloader(value.ThumbnailUrl, "VideoThumbnail_" + customThumbnail.Package + value.ThumbnailId, RegistryStrings.PromotionDirectory, false);
								}
								continue;
							}
						}
						if (customThumbnail[guidanceVideoType.ToString()] != null)
						{
							VideoThumbnailInfo videoThumbnailInfo = (VideoThumbnailInfo)customThumbnail[guidanceVideoType.ToString()];
							videoThumbnailInfo.ThumbnailType = guidanceVideoType;
							videoThumbnailInfo.ImagePath = Utils.TinyDownloader(videoThumbnailInfo.ThumbnailUrl, "VideoThumbnail_" + customThumbnail.Package + videoThumbnailInfo.ThumbnailId, RegistryStrings.PromotionDirectory, false);
						}
					}
					currentAppsGuidanceCloudInfo.CustomThumbnails[customThumbnail.Package] = customThumbnail;
				}
				foreach (JToken jtoken2 in JArray.Parse(res.GetValue("default_thumbnails").ToString(CultureInfo.InvariantCulture)))
				{
					VideoThumbnailInfo videoThumbnailInfo2 = JsonConvert.DeserializeObject<VideoThumbnailInfo>(jtoken2.ToString(), Utils.GetSerializerSettings());
					videoThumbnailInfo2.ImagePath = Utils.TinyDownloader(videoThumbnailInfo2.ThumbnailUrl, "VideoThumbnail_DefaultPackage_" + videoThumbnailInfo2.ThumbnailId, RegistryStrings.PromotionDirectory, false);
					currentAppsGuidanceCloudInfo.DefaultThumbnails[videoThumbnailInfo2.ThumbnailType] = videoThumbnailInfo2;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Error Loading Apps VideoThumbnail" + ex.ToString());
			}
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00024498 File Offset: 0x00022698
		private static void SetAppsReadArticle(GuidanceCloudInfo currentAppsGuidanceCloudInfo, JToken res)
		{
			try
			{
				foreach (JToken jtoken in JArray.Parse(res.GetValue("help_article").ToString(CultureInfo.InvariantCulture)))
				{
					HelpArticle helpArticle = JsonConvert.DeserializeObject<HelpArticle>(jtoken.ToString(), Utils.GetSerializerSettings());
					currentAppsGuidanceCloudInfo.HelpArticles[helpArticle.Package] = helpArticle;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Error Loading Apps ReadArticle" + ex.ToString());
			}
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0002453C File Offset: 0x0002273C
		private static void SetGameSettings(GuidanceCloudInfo guidanceCloudInfoDict, JToken res)
		{
			try
			{
				JArray jarray = res["game_settings"] as JArray;
				if (jarray != null)
				{
					foreach (JToken jtoken in jarray)
					{
						try
						{
							GameSetting gameSetting = new GameSetting
							{
								SettingType = jtoken["setting_type"].Value<string>()
							};
							JArray jarray2 = jtoken["setting_data"] as JArray;
							if (jarray2 != null)
							{
								foreach (JToken jtoken2 in jarray2)
								{
									Dictionary<string, object> dictionary = new Dictionary<string, object>();
									JArray jarray3 = jtoken2["app_pkg_list"] as JArray;
									if (jarray3 != null)
									{
										dictionary.Add("app_pkg_list", new AppPackageListObject(jarray3.ToObject<List<string>>()));
									}
									string settingType = gameSetting.SettingType;
									if (settingType != null && settingType == "OrientationMode")
									{
										dictionary.Add("mode", jtoken2["mode"].Value<string>());
										gameSetting.SettingsData.Add(dictionary);
									}
								}
								guidanceCloudInfoDict.GameSettings.Add(gameSetting);
							}
						}
						catch (Exception ex)
						{
							string text = "Error while loading game settings from cloud data ";
							Exception ex2 = ex;
							Logger.Warning(text + ((ex2 != null) ? ex2.ToString() : null));
						}
					}
				}
			}
			catch (Exception ex3)
			{
				string text2 = "Error while loading game settings from cloud data ";
				Exception ex4 = ex3;
				Logger.Warning(text2 + ((ex4 != null) ? ex4.ToString() : null));
			}
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00024720 File Offset: 0x00022920
		internal static string GetCloudOrientationForPackage(string package)
		{
			string text = string.Empty;
			if (GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo != null && GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.GameSettings.Any<GameSetting>())
			{
				GameSetting gameSetting = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.GameSettings.Where((GameSetting setting) => string.Equals(setting.SettingType, "OrientationMode", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault<GameSetting>();
				if (gameSetting != null)
				{
					foreach (Dictionary<string, object> dictionary in gameSetting.SettingsData)
					{
						if (dictionary.ContainsKey("mode") && dictionary.ContainsKey("app_pkg_list"))
						{
							AppPackageListObject appPackageListObject = dictionary["app_pkg_list"] as AppPackageListObject;
							if (appPackageListObject != null && appPackageListObject.IsPackageAvailable(package))
							{
								text = dictionary["mode"].ToString().ToLower(CultureInfo.InvariantCulture);
								break;
							}
						}
					}
				}
			}
			return text;
		}

		// Token: 0x04000339 RID: 825
		private static GuidanceCloudInfoManager sInstance = null;

		// Token: 0x0400033A RID: 826
		private static readonly object sLock = new object();

		// Token: 0x0400033B RID: 827
		private const string sGuidanceCloudInfoFilename = "bst_guidance";

		// Token: 0x0400033C RID: 828
		internal GuidanceCloudInfo mGuidanceCloudInfo = new GuidanceCloudInfo();
	}
}
