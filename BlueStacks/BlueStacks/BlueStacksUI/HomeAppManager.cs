using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000058 RID: 88
	public class HomeAppManager
	{
		// Token: 0x0600048D RID: 1165 RVA: 0x00004F89 File Offset: 0x00003189
		public HomeAppManager(HomeApp homeApp, MainWindow parentWindow)
		{
			this.mHomeApp = homeApp;
			this.mParentWindow = parentWindow;
			this.InitSystemIcons();
			this.InitIcons();
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00004FB6 File Offset: 0x000031B6
		internal void InitAppPromotionEvents()
		{
			if (PromotionObject.Instance != null)
			{
				PromotionObject.AppSuggestionHandler = (Action<bool>)Delegate.Combine(PromotionObject.AppSuggestionHandler, new Action<bool>(this.HomeApp_AppSuggestionHandler));
				HomeApp homeApp = this.mHomeApp;
				if (homeApp == null)
				{
					return;
				}
				homeApp.InitUIAppPromotionEvents();
			}
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00004FEF File Offset: 0x000031EF
		private void HomeApp_AppSuggestionHandler(bool checkForAnimationIcon)
		{
			MainWindow mainWindow = this.mParentWindow;
			if (mainWindow == null)
			{
				return;
			}
			mainWindow.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					this.RemoveIconIfExists();
					object syncRoot = ((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot;
					lock (syncRoot)
					{
						foreach (AppSuggestionPromotion appSuggestionPromotion in PromotionObject.Instance.AppSuggestionList)
						{
							if (!new JsonParser(this.mParentWindow.mVmName).IsAppInstalled(appSuggestionPromotion.AppPackage))
							{
								if (HomeAppManager.CheckIfPresentInRedDotShownRegistry(appSuggestionPromotion.AppPackage))
								{
									appSuggestionPromotion.IsShowRedDot = false;
								}
								this.AddAppSuggestionIcon(appSuggestionPromotion);
							}
							else
							{
								if (!HomeAppManager.CheckIfPresentInRedDotShownRegistry(appSuggestionPromotion.AppPackage) && appSuggestionPromotion.IsShowRedDot)
								{
									AppIconModel appIcon = this.GetAppIcon(appSuggestionPromotion.AppPackage);
									if (appIcon != null)
									{
										appIcon.AddRedDot();
									}
								}
								else
								{
									appSuggestionPromotion.IsShowRedDot = false;
								}
								AppIconModel appIcon2 = this.GetAppIcon(appSuggestionPromotion.AppPackage);
								if (appIcon2 != null)
								{
									appIcon2.AddPromotionBorderInstalledApp(appSuggestionPromotion);
								}
							}
						}
					}
					bool flag = this.dictAppIcons.Keys.Intersect(PackageActivityNames.ThirdParty.AllOneStorePackageNames).Any<string>();
					foreach (string text in PackageActivityNames.ThirdParty.AllOneStorePackageNames)
					{
						Utils.EnableDisableApp(text, flag, this.mParentWindow.mVmName);
					}
					this.mParentWindow.StaticComponents.PlayPauseGifs(true);
				}
				catch (Exception ex)
				{
					Logger.Error("Error in HomeApp_AppSuggestionHandler", new object[] { ex });
				}
			}), new object[0]);
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0001D228 File Offset: 0x0001B428
		private void RemoveIconIfExists()
		{
			List<string> list = new List<string>();
			JsonParser jsonParser = new JsonParser(this.mParentWindow.mVmName);
			using (Dictionary<string, AppIconModel>.ValueCollection.Enumerator enumerator = this.dictAppIcons.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AppIconModel icon = enumerator.Current;
					object syncRoot = ((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot;
					lock (syncRoot)
					{
						if (!icon.IsAppSuggestionActive)
						{
							if (icon.IsInstalledApp)
							{
								continue;
							}
							if (!PromotionObject.Instance.AppSuggestionList.Any((AppSuggestionPromotion _) => string.Equals(_.AppLocation, "more_apps", StringComparison.InvariantCulture)))
							{
								continue;
							}
						}
						if (!PromotionObject.Instance.AppSuggestionList.Any((AppSuggestionPromotion _) => string.Equals(_.AppPackage, icon.PackageName, StringComparison.InvariantCultureIgnoreCase)))
						{
							if (!jsonParser.IsAppInstalled(icon.PackageName))
							{
								list.Add(icon.PackageName);
							}
							else
							{
								icon.RemovePromotionBorderInstalledApp();
							}
						}
					}
				}
			}
			foreach (string text in list)
			{
				this.RemoveAppIcon(text, null);
			}
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x0001D3A8 File Offset: 0x0001B5A8
		private void InitSystemIcons()
		{
			List<AppInfo> list = new JsonParser(string.Empty).GetAppList().ToList<AppInfo>();
			HomeApp homeApp = this.mHomeApp;
			if (homeApp != null)
			{
				homeApp.InitMoreAppsIcon();
			}
			foreach (AppInfo appInfo in list)
			{
				if (string.Compare(appInfo.Package, "com.android.vending", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(appInfo.Package, "com.google.android.play.games", StringComparison.OrdinalIgnoreCase) != 0)
				{
					AppIconModel newIconForKey = this.GetNewIconForKey(appInfo.Package);
					newIconForKey.Init(appInfo);
					newIconForKey.IsInstalledApp = false;
					newIconForKey.AddToMoreAppsDock(55.0, 55.0);
					HomeApp homeApp2 = this.mHomeApp;
					if (homeApp2 != null)
					{
						homeApp2.AddMoreAppsDockPanelIcon(newIconForKey, null);
					}
				}
				else
				{
					AppIconModel newIconForKey2 = this.GetNewIconForKey(appInfo.Package);
					newIconForKey2.Init(appInfo);
					newIconForKey2.IsInstalledApp = false;
					newIconForKey2.mIsAppRemovable = false;
					newIconForKey2.AddToInstallDrawer();
					HomeApp homeApp3 = this.mHomeApp;
					if (homeApp3 != null)
					{
						homeApp3.AddInstallDrawerIcon(newIconForKey2, null);
					}
				}
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0001D4CC File Offset: 0x0001B6CC
		internal void InitIcons()
		{
			foreach (AppInfo appInfo in new JsonParser(this.mParentWindow.mVmName).GetAppList().ToList<AppInfo>())
			{
				this.AddIcon(appInfo);
			}
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x0001D534 File Offset: 0x0001B734
		internal AppInfo AddAppIcon(string package)
		{
			AppInfo appInfoFromPackageName = new JsonParser(this.mParentWindow.mVmName).GetAppInfoFromPackageName(package);
			if (appInfoFromPackageName != null)
			{
				this.AddIcon(appInfoFromPackageName);
			}
			return appInfoFromPackageName;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0001D564 File Offset: 0x0001B764
		private void AddAppSuggestionIcon(AppSuggestionPromotion appSuggestionInfo)
		{
			string appPackage = appSuggestionInfo.AppPackage;
			double num = 50.0;
			double num2 = 50.0;
			AppIconModel newIconForKey = this.GetNewIconForKey(appPackage);
			try
			{
				if (newIconForKey != null)
				{
					newIconForKey.IsAppSuggestionActive = true;
					newIconForKey.PackageName = appPackage;
					if (appSuggestionInfo.IsShowRedDot)
					{
						newIconForKey.IsRedDotVisible = true;
					}
					newIconForKey.Init(appSuggestionInfo);
					if (!appSuggestionInfo.IsEmailRequired || RegistryManager.Instance.Guest[this.mParentWindow.mVmName].IsGoogleSigninDone)
					{
						if (string.Equals(appSuggestionInfo.AppLocation, "dock", StringComparison.InvariantCultureIgnoreCase))
						{
							if (appSuggestionInfo.IconHeight != 0.0)
							{
								num = appSuggestionInfo.IconHeight;
							}
							if (appSuggestionInfo.IconWidth != 0.0)
							{
								num2 = appSuggestionInfo.IconWidth;
							}
							newIconForKey.AddToDock(num, num2);
							HomeApp homeApp = this.mHomeApp;
							if (homeApp != null)
							{
								homeApp.AddDockPanelIcon(newIconForKey, null);
							}
						}
						else if (string.Equals(appSuggestionInfo.AppLocation, "more_apps", StringComparison.InvariantCultureIgnoreCase))
						{
							newIconForKey.AddToMoreAppsDock(55.0, 55.0);
							HomeApp homeApp2 = this.mHomeApp;
							if (homeApp2 != null)
							{
								homeApp2.AddMoreAppsDockPanelIcon(newIconForKey, null);
							}
						}
						else
						{
							newIconForKey.AddToInstallDrawer();
							HomeApp homeApp3 = this.mHomeApp;
							if (homeApp3 != null)
							{
								homeApp3.AddInstallDrawerIcon(newIconForKey, null);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in adding app suggestion icon: " + ex.ToString());
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0001D6DC File Offset: 0x0001B8DC
		internal void AddIconWithRedDot(string appPackage)
		{
			object syncRoot = ((ICollection)PromotionObject.Instance.AppSuggestionList).SyncRoot;
			lock (syncRoot)
			{
				JsonParser jsonParser = new JsonParser(this.mParentWindow.mVmName);
				foreach (AppSuggestionPromotion appSuggestionPromotion in PromotionObject.Instance.AppSuggestionList)
				{
					if (string.Equals(appSuggestionPromotion.AppPackage, appPackage, StringComparison.InvariantCulture))
					{
						if (!jsonParser.IsAppInstalled(appSuggestionPromotion.AppPackage))
						{
							HomeAppManager.RemovePackageInRedDotShownRegistry(appSuggestionPromotion.AppPackage);
							this.AddAppSuggestionIcon(appSuggestionPromotion);
						}
						else
						{
							HomeAppManager.RemovePackageInRedDotShownRegistry(appSuggestionPromotion.AppPackage);
							AppIconModel appIcon = this.GetAppIcon(appPackage);
							if (appIcon != null)
							{
								appIcon.AddRedDot();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x0001D7B4 File Offset: 0x0001B9B4
		internal void AddMacroAppIcon(string package)
		{
			if (!string.IsNullOrEmpty(package))
			{
				string text = package + "_macro";
				AppIconModel newIconForKey = this.GetNewIconForKey(text);
				string text2 = LocaleStrings.GetLocalizedString("STRING_REROLL_APP_PREFIX", "") + " - " + newIconForKey.AppName;
				newIconForKey.InitRerollIcon(package, text2);
				newIconForKey.AddToInstallDrawer();
				HomeApp homeApp = this.mHomeApp;
				if (homeApp == null)
				{
					return;
				}
				homeApp.AddInstallDrawerIcon(newIconForKey, null);
			}
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0001D820 File Offset: 0x0001BA20
		internal void AddAppIcon(string package, string appName, string apkUrl, DownloadInstallApk downloader)
		{
			if (!string.IsNullOrEmpty(package))
			{
				AppIconModel newIconForKey = this.GetNewIconForKey(package);
				newIconForKey.Init(package, appName, apkUrl);
				newIconForKey.AddToInstallDrawer();
				HomeApp homeApp = this.mHomeApp;
				if (homeApp == null)
				{
					return;
				}
				homeApp.AddInstallDrawerIcon(newIconForKey, downloader);
			}
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001D860 File Offset: 0x0001BA60
		private void AddIcon(AppInfo item)
		{
			AppIconModel newIconForKey = this.GetNewIconForKey(item.Package);
			newIconForKey.Init(item);
			newIconForKey.AddToInstallDrawer();
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			homeApp.AddInstallDrawerIcon(newIconForKey, null);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001D89C File Offset: 0x0001BA9C
		private AppIconModel GetNewIconForKey(string key)
		{
			AppIconModel appIconModel = new AppIconModel();
			this.RemoveAppIcon(key, appIconModel);
			this.dictAppIcons[key] = appIconModel;
			return appIconModel;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00005019 File Offset: 0x00003219
		internal bool CheckDictAppIconFor(string packagename, Predicate<AppIconModel> pred)
		{
			return this.dictAppIcons.ContainsKey(packagename) && pred(this.dictAppIcons[packagename]);
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001D8C8 File Offset: 0x0001BAC8
		internal AppIconModel GetAppIcon(string packageName)
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft && packageName == BlueStacksUIUtils.sUserAccountPackageName)
			{
				Logger.Info("Setting packageName to com.android.vending when com.uncube.account is received");
				packageName = "com.android.vending";
			}
			AppIconModel appIconModel = null;
			if (this.dictAppIcons.ContainsKey(packageName) && !string.IsNullOrEmpty(packageName))
			{
				appIconModel = this.dictAppIcons[packageName];
			}
			return appIconModel;
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0000503D File Offset: 0x0000323D
		internal AppIconModel GetMacroAppIcon(string packageName)
		{
			return this.GetAppIcon(packageName + "_macro");
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0001D928 File Offset: 0x0001BB28
		internal void RemoveAppIcon(string package, AppIconModel newAppIconCreated = null)
		{
			if (package != null && this.dictAppIcons.ContainsKey(package))
			{
				if (newAppIconCreated != null)
				{
					newAppIconCreated.AppIncompatType = this.dictAppIcons[package].AppIncompatType;
				}
				HomeApp homeApp = this.mHomeApp;
				if (homeApp != null)
				{
					homeApp.RemoveAppIconFromUI(this.dictAppIcons[package]);
				}
				this.dictAppIcons.Remove(package);
			}
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001D98C File Offset: 0x0001BB8C
		internal void RemoveAppAfterUninstall(string package)
		{
			GrmHandler.RemovePackageFromGrmList(package, this.mParentWindow.mVmName);
			this.RemoveAppIcon(package, null);
			this.RemoveAppIcon(package + "_macro", null);
			try
			{
				string text = Path.Combine(RegistryStrings.GadgetDir, Regex.Replace(package + ".png", "[\\x22\\\\\\/:*?|<>]", " "));
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Not able to delete image file : " + ex.ToString());
			}
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001DA24 File Offset: 0x0001BC24
		internal void UpdateGamepadIcons(bool isGamepadConnected)
		{
			foreach (KeyValuePair<string, AppIconModel> keyValuePair in this.dictAppIcons)
			{
				if (keyValuePair.Value.IsGamepadCompatible)
				{
					keyValuePair.Value.IsGamepadConnected = isGamepadConnected;
				}
			}
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001DA8C File Offset: 0x0001BC8C
		internal void OpenApp(string packageName, bool isCheckForGrm = true)
		{
			AppIconModel appIcon = this.GetAppIcon(packageName);
			if (appIcon != null)
			{
				if (appIcon.AppIncompatType > AppIncompatType.None && isCheckForGrm)
				{
					GrmHandler.HandleCompatibility(appIcon.PackageName, this.mParentWindow.mVmName);
					return;
				}
				this.mParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, false, true, false);
				this.mParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = appIcon.PackageName;
				this.mParentWindow.mAppHandler.SendRunAppRequestAsync(appIcon.PackageName, "", false);
				if (appIcon.IsRedDotVisible)
				{
					appIcon.IsRedDotVisible = false;
					HomeAppManager.AddPackageInRedDotShownRegistry(appIcon.PackageName);
				}
				HomeAppManager.SendStats(appIcon.PackageName);
			}
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001DB54 File Offset: 0x0001BD54
		private static void SendStats(string packageName)
		{
			if (packageName == "com.android.vending")
			{
				ClientStats.SendGPlayClickStats(new Dictionary<string, string> { { "source", "bs3_myapps" } });
			}
			ClientStats.SendClientStatsAsync("init", "success", "app_activity", packageName, "", "");
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x0001DBA8 File Offset: 0x0001BDA8
		private static bool CheckIfPresentInRedDotShownRegistry(string package)
		{
			string redDotShownOnIcon = RegistryManager.Instance.RedDotShownOnIcon;
			if (!string.IsNullOrEmpty(redDotShownOnIcon))
			{
				char[] array = new char[] { ',' };
				foreach (string text in redDotShownOnIcon.Split(array, StringSplitOptions.None))
				{
					if (!string.IsNullOrEmpty(package) && text.Equals(package, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001DC08 File Offset: 0x0001BE08
		private static void RemovePackageInRedDotShownRegistry(string appPackage)
		{
			string redDotShownOnIcon = RegistryManager.Instance.RedDotShownOnIcon;
			char[] array = new char[] { ',' };
			string[] array2 = (from w in redDotShownOnIcon.Split(array, StringSplitOptions.None)
				where !w.Contains(appPackage)
				select w).ToArray<string>();
			string text = string.Empty;
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + text2.ToString(CultureInfo.InvariantCulture) + ",";
				}
			}
			RegistryManager.Instance.RedDotShownOnIcon = text;
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001DCA8 File Offset: 0x0001BEA8
		internal static void AddPackageInRedDotShownRegistry(string appPackage)
		{
			string text = RegistryManager.Instance.RedDotShownOnIcon;
			if (!string.IsNullOrEmpty(text))
			{
				text = text + "," + appPackage;
			}
			else
			{
				text = appPackage;
			}
			RegistryManager.Instance.RedDotShownOnIcon = text;
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001DCE4 File Offset: 0x0001BEE4
		internal void DownloadStarted(string packageName)
		{
			if (this.dictAppIcons.ContainsKey(packageName))
			{
				this.dictAppIcons[packageName].DownloadStarted();
				JObject jobject = new JObject();
				jobject["PackageName"] = packageName;
				jobject["AppName"] = this.dictAppIcons[packageName].AppName;
				jobject["ApkUrl"] = this.dictAppIcons[packageName].ApkUrl;
				JObject jobject2 = jobject;
				Publisher.PublishMessage(BrowserControlTags.apkDownloadStarted, this.mParentWindow.mVmName, jobject2);
			}
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001DD7C File Offset: 0x0001BF7C
		internal void UpdateAppDownloadProgress(string packageName, int percent)
		{
			if (this.dictAppIcons.ContainsKey(packageName))
			{
				this.dictAppIcons[packageName].UpdateAppDownloadProgress(percent);
				JObject jobject = new JObject();
				jobject["PackageName"] = packageName;
				jobject["DownloadPercent"] = percent;
				JObject jobject2 = jobject;
				Publisher.PublishMessage(BrowserControlTags.apkDownloadCurrentProgress, this.mParentWindow.mVmName, jobject2);
			}
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001DDE4 File Offset: 0x0001BFE4
		internal void DownloadFailed(string packageName)
		{
			if (this.dictAppIcons.ContainsKey(packageName))
			{
				this.dictAppIcons[packageName].DownloadFailed();
				if (FeatureManager.Instance.IsHtmlHome)
				{
					this.RemoveAppIcon(packageName, null);
				}
				Publisher.PublishMessage(BrowserControlTags.apkDownloadFailed, this.mParentWindow.mVmName, new JObject(new JProperty("PackageName", packageName)));
			}
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001DE48 File Offset: 0x0001C048
		internal void DownloadCompleted(string packageName, string filePath)
		{
			if (this.dictAppIcons.ContainsKey(packageName))
			{
				this.dictAppIcons[packageName].DownloadCompleted(filePath);
				Publisher.PublishMessage(BrowserControlTags.apkDownloadCompleted, this.mParentWindow.mVmName, new JObject(new JProperty("PackageName", packageName)));
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001DE98 File Offset: 0x0001C098
		internal void ApkInstallStart(string packageName, string filePath)
		{
			if (this.dictAppIcons.ContainsKey(packageName))
			{
				this.dictAppIcons[packageName].ApkInstallStart(filePath);
				JObject jobject = new JObject();
				jobject["PackageName"] = packageName;
				jobject["AppName"] = this.dictAppIcons[packageName].AppName;
				jobject["ApkFilePath"] = filePath;
				JObject jobject2 = jobject;
				Publisher.PublishMessage(BrowserControlTags.apkInstallStarted, this.mParentWindow.mVmName, jobject2);
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001DF24 File Offset: 0x0001C124
		internal void ApkInstallFailed(string packageName)
		{
			if (this.dictAppIcons.ContainsKey(packageName))
			{
				this.dictAppIcons[packageName].ApkInstallFailed();
				if (FeatureManager.Instance.IsHtmlHome)
				{
					this.RemoveAppIcon(packageName, null);
				}
				Publisher.PublishMessage(BrowserControlTags.apkInstallFailed, this.mParentWindow.mVmName, new JObject(new JProperty("PackageName", packageName)));
			}
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x0001DF88 File Offset: 0x0001C188
		internal void ApkInstallCompleted(string packageName)
		{
			if (this.dictAppIcons.ContainsKey(packageName))
			{
				this.dictAppIcons[packageName].ApkInstallCompleted();
				Publisher.PublishMessage(BrowserControlTags.apkInstallCompleted, this.mParentWindow.mVmName, new JObject(new JProperty("PackageName", packageName)));
			}
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0001DFD8 File Offset: 0x0001C1D8
		internal void HomeTabSwitchActions(bool isHomeTabSelected)
		{
			if (isHomeTabSelected)
			{
				HomeApp homeApp = this.mHomeApp;
				if (homeApp != null && homeApp.mSearchTextBox.IsFocused)
				{
					this.SetSearchTextBoxFocus(100);
				}
				this.mParentWindow.mWelcomeTab.ReloadHomeTabIME();
				this.mParentWindow.StaticComponents.PlayPauseGifs(true);
				return;
			}
			this.mParentWindow.StaticComponents.PlayPauseGifs(false);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00005050 File Offset: 0x00003250
		internal void SetSearchTextBoxFocus(int delay)
		{
			HomeApp homeApp = this.mHomeApp;
			MiscUtils.SetFocusAsync((homeApp != null) ? homeApp.mSearchTextBox : null, delay);
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0000506A File Offset: 0x0000326A
		internal void EnableSearchTextBox(bool isEnable)
		{
			if (this.mHomeApp != null)
			{
				this.mHomeApp.mSearchTextBox.IsEnabled = isEnable;
			}
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00005085 File Offset: 0x00003285
		internal void ChangeHomeAppVisibility(Visibility visibility)
		{
			if (this.mHomeApp != null)
			{
				this.mHomeApp.Visibility = visibility;
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0000509B File Offset: 0x0000329B
		internal void RestoreWallpaper()
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			homeApp.RestoreWallpaperImage();
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x000050AD File Offset: 0x000032AD
		internal void ApplyWallpaper()
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			homeApp.ApplyWallpaperImage();
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x000050BF File Offset: 0x000032BF
		internal void ClearAppRecommendationPool()
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			homeApp.sAppRecommendationsPool.Clear();
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x000050D6 File Offset: 0x000032D6
		internal void AddToAppRecommendationPool(RecommendedApps recomApp)
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			homeApp.sAppRecommendationsPool.Add(recomApp);
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x000050EE File Offset: 0x000032EE
		internal void UpdateRecommendedAppsInstallStatus(string package)
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			homeApp.UpdateRecommendedAppsInstallStatus(package);
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x00005101 File Offset: 0x00003301
		internal void InitiateHtmlSidePanel()
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp != null && !homeApp.SideHtmlBrowserInited)
			{
				HomeApp homeApp2 = this.mHomeApp;
				if (homeApp2 == null)
				{
					return;
				}
				homeApp2.InitiateSideHtmlBrowser();
			}
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0000512A File Offset: 0x0000332A
		internal void DisposeHtmlSidePanel()
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			BrowserControl sideHtmlBrowser = homeApp.SideHtmlBrowser;
			if (sideHtmlBrowser == null)
			{
				return;
			}
			sideHtmlBrowser.DisposeBrowser();
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00005146 File Offset: 0x00003346
		internal void ReinitHtmlSidePanel()
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null)
			{
				return;
			}
			BrowserControl sideHtmlBrowser = homeApp.SideHtmlBrowser;
			if (sideHtmlBrowser == null)
			{
				return;
			}
			sideHtmlBrowser.ReInitBrowser(BlueStacksUIUtils.GetHtmlSidePanelUrl());
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00005167 File Offset: 0x00003367
		internal void CloseHomeAppPopups()
		{
			if (this.mHomeApp != null)
			{
				this.mHomeApp.mSuggestedAppPopUp.IsOpen = false;
				this.mHomeApp.mMoreAppsDockPopup.IsOpen = false;
			}
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x00005193 File Offset: 0x00003393
		internal void ChangeHomeAppLoadingGridVisibility(Visibility visibility)
		{
			if (this.mHomeApp != null)
			{
				this.mHomeApp.mLoadingGrid.Visibility = visibility;
			}
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x000051AE File Offset: 0x000033AE
		internal double GetAppRecommendationsGridWidth()
		{
			HomeApp homeApp = this.mHomeApp;
			if (homeApp == null || homeApp.mAppRecommendationsGrid.ActualWidth <= 0.0)
			{
				return 0.0;
			}
			return this.mHomeApp.mAppRecommendationsGrid.ActualWidth;
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0001E03C File Offset: 0x0001C23C
		internal void ShowDockIconTooltip(AppIconUI icon, bool isOpen)
		{
			if (this.mHomeApp != null)
			{
				if (isOpen)
				{
					this.mHomeApp.mDockIconText.Text = icon.mAppIconModel.AppName;
					this.mHomeApp.mDockAppIconToolTipPopup.PlacementTarget = icon.mAppImage;
					this.mHomeApp.mDockAppIconToolTipPopup.IsOpen = true;
					this.mHomeApp.mDockAppIconToolTipPopup.StaysOpen = true;
					return;
				}
				this.mHomeApp.mDockAppIconToolTipPopup.IsOpen = false;
			}
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x000051EE File Offset: 0x000033EE
		internal void CloseAppSuggestionPopup()
		{
			if (this.mHomeApp != null)
			{
				this.mHomeApp.mSuggestedAppPopUp.IsOpen = false;
			}
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0001E0BC File Offset: 0x0001C2BC
		internal void OpenAppSuggestionPopup(AppSuggestionPromotion appInfoForShowingPopup, UIElement appNameTextBlock, bool staysOpen = true)
		{
			if (this.mHomeApp != null && appInfoForShowingPopup.ToolTip != null)
			{
				this.mHomeApp.mSuggestedAppPopUp.PlacementTarget = appNameTextBlock;
				this.mHomeApp.mSuggestedAppPopUp.IsOpen = true;
				this.mHomeApp.mSuggestedAppPopUp.StaysOpen = staysOpen;
				this.mHomeApp.mAppSuggestionPopUp.Text = appInfoForShowingPopup.ToolTip;
			}
		}

		// Token: 0x0400027F RID: 639
		private Dictionary<string, AppIconModel> dictAppIcons = new Dictionary<string, AppIconModel>();

		// Token: 0x04000280 RID: 640
		private HomeApp mHomeApp;

		// Token: 0x04000281 RID: 641
		private MainWindow mParentWindow;

		// Token: 0x04000282 RID: 642
		internal static string BackgroundImagePath = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Client\\Assets\\backgroundImage");
	}
}
