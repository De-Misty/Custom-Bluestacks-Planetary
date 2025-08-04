using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200005C RID: 92
	internal class PostBootCloudInfoManager
	{
		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060004C7 RID: 1223 RVA: 0x0000526D File Offset: 0x0000346D
		private static string BstPostBootFilePath
		{
			get
			{
				return Path.Combine(RegistryStrings.PromotionDirectory, "bst_postboot");
			}
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0000527E File Offset: 0x0000347E
		private PostBootCloudInfoManager()
		{
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060004C9 RID: 1225 RVA: 0x0001E300 File Offset: 0x0001C500
		// (set) Token: 0x060004CA RID: 1226 RVA: 0x00005291 File Offset: 0x00003491
		internal static PostBootCloudInfoManager Instance
		{
			get
			{
				if (PostBootCloudInfoManager.sInstance == null)
				{
					object obj = PostBootCloudInfoManager.sLock;
					lock (obj)
					{
						if (PostBootCloudInfoManager.sInstance == null)
						{
							PostBootCloudInfoManager.sInstance = new PostBootCloudInfoManager();
						}
					}
				}
				return PostBootCloudInfoManager.sInstance;
			}
			set
			{
				PostBootCloudInfoManager.sInstance = value;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x0001E350 File Offset: 0x0001C550
		// (set) Token: 0x060004CC RID: 1228 RVA: 0x00005299 File Offset: 0x00003499
		internal string Url
		{
			get
			{
				if (string.IsNullOrEmpty(this.mUrl))
				{
					this.mUrl = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
					{
						RegistryManager.Instance.Host,
						"/bs4/post_boot"
					}));
				}
				return this.mUrl;
			}
			private set
			{
				this.mUrl = value;
			}
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0001E3A8 File Offset: 0x0001C5A8
		internal JToken GetPostBootData()
		{
			JToken jtoken = null;
			try
			{
				string text = BstHttpClient.Get(this.Url, null, false, "Android", 0, 1, 0, false, "bgp64");
				Logger.Debug("Postboot data Url: " + this.Url);
				jtoken = JToken.Parse(text);
			}
			catch (Exception ex)
			{
				Logger.Error("Error Getting Post Boot Data err: " + ex.ToString());
			}
			return jtoken;
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0001E418 File Offset: 0x0001C618
		internal void GetPostBootDataAsync(MainWindow mainWindow)
		{
			if (this.mPostBootCloudInfo == null)
			{
				new Thread(delegate
				{
					this.mPostBootCloudInfo = new PostBootCloudInfo();
					if (File.Exists(PostBootCloudInfoManager.BstPostBootFilePath))
					{
						this.mPostBootCloudInfo = JsonConvert.DeserializeObject<PostBootCloudInfo>(File.ReadAllText(PostBootCloudInfoManager.BstPostBootFilePath));
					}
					JToken postBootData = this.GetPostBootData();
					if (postBootData != null)
					{
						PostBootCloudInfo postBootCloudInfo = new PostBootCloudInfo();
						PostBootCloudInfoManager.SetMinimizeGameNotificationsPackages(postBootCloudInfo, postBootData);
						PostBootCloudInfoManager.SetOnBoardingGamePackages(postBootCloudInfo, postBootData);
						PostBootCloudInfoManager.SetGameAwareOnboardingPackages(postBootCloudInfo, postBootData);
						PostBootCloudInfoManager.SetCustomCursorGamePackages(postBootCloudInfo, postBootData);
						PostBootCloudInfoManager.SetIgnoreActivities(postBootCloudInfo, postBootData);
						PostBootCloudInfoManager.SaveToFile(postBootCloudInfo);
						this.mPostBootCloudInfo = postBootCloudInfo;
						PostBootCloudInfoManager.SendCustomCursorAppsListToPlayer(mainWindow);
					}
				})
				{
					IsBackground = true
				}.Start();
				return;
			}
			PostBootCloudInfoManager.SendCustomCursorAppsListToPlayer(mainWindow);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001E46C File Offset: 0x0001C66C
		private static void SendCustomCursorAppsListToPlayer(MainWindow mainWindow)
		{
			try
			{
				string text = string.Empty;
				foreach (string text2 in PostBootCloudInfoManager.Instance.mPostBootCloudInfo.AppSpecificCustomCursorInfo.CustomCursorAppPackages.CloudPackageList)
				{
					text += string.Format(CultureInfo.InvariantCulture, "{0} ", new object[] { text2 });
				}
				mainWindow.mFrontendHandler.SendFrontendRequestAsync("sendCustomCursorEnabledApps", new Dictionary<string, string> { 
				{
					"packages",
					text.Trim()
				} });
				Logger.Debug("CURSOR: vmName:{0} packages: {1} ", new object[]
				{
					mainWindow.mVmName,
					text.Trim()
				});
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SendCustomCursorAppsListToPlayer: " + ex.ToString());
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001E560 File Offset: 0x0001C760
		private static void SaveToFile(PostBootCloudInfo postBootCloudInfo)
		{
			try
			{
				string text = JsonConvert.SerializeObject(postBootCloudInfo, Formatting.Indented, Utils.GetSerializerSettings());
				if (!Directory.Exists(RegistryStrings.PromotionDirectory))
				{
					Directory.CreateDirectory(RegistryStrings.PromotionDirectory);
				}
				File.WriteAllText(PostBootCloudInfoManager.BstPostBootFilePath, text);
			}
			catch (Exception)
			{
				Logger.Warning("Error in saving PostBootCloudInfo to file");
			}
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001E5BC File Offset: 0x0001C7BC
		private static void SetMinimizeGameNotificationsPackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
		{
			try
			{
				JToken jtoken = JToken.Parse(res.GetValue("minimize_game_notification_apps"));
				if (jtoken["app_pkg_list"] != null)
				{
					JArray jarray = jtoken["app_pkg_list"] as JArray;
					if (jarray != null)
					{
						currentPostBootCloudInfo.GameNotificationAppPackages.NotificationModeAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
					}
				}
				int num;
				if (jtoken["consecutive_session_count_number"] != null && int.TryParse(jtoken["consecutive_session_count_number"].ToString(), out num))
				{
					currentPostBootCloudInfo.GameNotificationAppPackages.ExitPopupCount = num;
					RegistryManager.Instance.NotificationModeCounter = num;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in parsing game notification packages: " + ex.ToString());
			}
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001E678 File Offset: 0x0001C878
		private static void SetOnBoardingGamePackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
		{
			try
			{
				JToken jtoken = JToken.Parse(res.GetValue("onboarding_tutorial_apps"));
				if (jtoken["app_pkg_list"] != null)
				{
					JArray jarray = jtoken["app_pkg_list"] as JArray;
					if (jarray != null)
					{
						currentPostBootCloudInfo.OnBoardingInfo.OnBoardingAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
					}
				}
				if (jtoken["skip_button_timer"] != null)
				{
					currentPostBootCloudInfo.OnBoardingInfo.OnBoardingSkipTimer = int.Parse(jtoken["skip_button_timer"].ToString(), CultureInfo.InvariantCulture);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in parsing onboarding packages: " + ex.ToString());
			}
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001E72C File Offset: 0x0001C92C
		private static void SetCustomCursorGamePackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
		{
			try
			{
				JToken jtoken = JToken.Parse(res.GetValue("custom_cursor_apps"));
				jtoken = JToken.Parse(jtoken.GetValue("moba"));
				if (jtoken["app_pkg_list"] != null)
				{
					JArray jarray = jtoken["app_pkg_list"] as JArray;
					if (jarray != null)
					{
						currentPostBootCloudInfo.AppSpecificCustomCursorInfo.CustomCursorAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in parsing SetCustomCursorGamePackages: " + ex.ToString());
			}
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001E7BC File Offset: 0x0001C9BC
		private static void SetIgnoreActivities(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
		{
			try
			{
				if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("ignore_activities_for_tab")))
				{
					currentPostBootCloudInfo.IgnoredActivitiesForTabs.ClearSync<string>();
					using (IEnumerator<JToken> enumerator = JArray.Parse(res["ignore_activities_for_tab"].ToString()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JToken jtoken = enumerator.Current;
							currentPostBootCloudInfo.IgnoredActivitiesForTabs.Add(jtoken.ToString());
						}
						goto IL_0071;
					}
				}
				currentPostBootCloudInfo.IgnoredActivitiesForTabs.ClearSync<string>();
				IL_0071:;
			}
			catch (Exception ex)
			{
				Logger.Error("Error while getting ignore activities for tab list: {0}", new object[] { ex });
			}
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001E870 File Offset: 0x0001CA70
		private static void SetGameAwareOnboardingPackages(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
		{
			try
			{
				JToken jtoken = JToken.Parse(res.GetValue("game_aware_onboarding"));
				if (jtoken["app_pkg_list"] != null)
				{
					JArray jarray = jtoken["app_pkg_list"] as JArray;
					if (jarray != null)
					{
						currentPostBootCloudInfo.GameAwareOnboardingInfo.GameAwareOnBoardingAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in parsing game aware onboarding packages: " + ex.ToString());
			}
		}

		// Token: 0x04000287 RID: 647
		private static PostBootCloudInfoManager sInstance = null;

		// Token: 0x04000288 RID: 648
		private static readonly object sLock = new object();

		// Token: 0x04000289 RID: 649
		private const string sPostBootCloudInfoFilename = "bst_postboot";

		// Token: 0x0400028A RID: 650
		internal PostBootCloudInfo mPostBootCloudInfo;

		// Token: 0x0400028B RID: 651
		private string mUrl = string.Empty;
	}
}
