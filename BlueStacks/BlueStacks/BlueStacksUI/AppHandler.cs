using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020002AC RID: 684
	public class AppHandler
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06001968 RID: 6504 RVA: 0x00011198 File Offset: 0x0000F398
		// (set) Token: 0x06001969 RID: 6505 RVA: 0x000111A0 File Offset: 0x0000F3A0
		public SerializableDictionary<string, DateTime> CdnAppdict { get; set; } = new SerializableDictionary<string, DateTime>();

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x0600196A RID: 6506 RVA: 0x000111A9 File Offset: 0x0000F3A9
		// (set) Token: 0x0600196B RID: 6507 RVA: 0x000111B1 File Offset: 0x0000F3B1
		public string mLastAppDisplayed { get; set; } = string.Empty;

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x0600196C RID: 6508 RVA: 0x000111BA File Offset: 0x0000F3BA
		// (set) Token: 0x0600196D RID: 6509 RVA: 0x000111C2 File Offset: 0x0000F3C2
		public string mLastRunAppSentForSynced { get; set; } = string.Empty;

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x0600196E RID: 6510 RVA: 0x000111CB File Offset: 0x0000F3CB
		// (set) Token: 0x0600196F RID: 6511 RVA: 0x000111D3 File Offset: 0x0000F3D3
		public string mAppDisplayedOccured { get; set; } = string.Empty;

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06001970 RID: 6512 RVA: 0x000111DC File Offset: 0x0000F3DC
		public static List<string> ListIgnoredApps { get; } = new List<string>
		{
			"tv.gamepop.home", "com.pop.store", "com.pop.store51", "com.bluestacks.s2p5105", "com.bluestacks.help", "mpi.v23", "com.google.android.gms", "com.google.android.gsf.login", "com.android.deskclock", "me.onemobile.android",
			"me.onemobile.lite.android", "android.rk.RockVideoPlayer.RockVideoPlayer", "com.bluestacks.chartapp", "com.bluestacks.setupapp", "com.android.gallery3d", "com.bluestacks.keymappingtool", "com.baidu.appsearch", "com.bluestacks.s2p", "com.bluestacks.windowsfilemanager", "com.android.quicksearchbox",
			"com.bluestacks.setup", "com.bluestacks.appsettings", "mpi.v23", "com.bluestacks.setup", "com.bluestacks.gamepophome", "com.bluestacks.appfinder", "com.android.providers.downloads.ui", "com.google.android.instantapps.supervisor"
		};

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06001971 RID: 6513 RVA: 0x000111E3 File Offset: 0x0000F3E3
		// (set) Token: 0x06001972 RID: 6514 RVA: 0x00097718 File Offset: 0x00095918
		public bool IsOneTimeSetupCompleted
		{
			get
			{
				if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition && GameConfig.Instance.AppGenericAction == GenericAction.InstallCDN)
				{
					return true;
				}
				if (!this.mIsOneTimeSetupCompleted)
				{
					this.StartOTSCheckThread();
				}
				return this.mIsOneTimeSetupCompleted;
			}
			set
			{
				this.mIsOneTimeSetupCompleted = value;
				this.ParentWindow.EngineInstanceRegistry.IsOneTimeSetupDone = value;
				Logger.Info("One time setup completed. Will perform tasks now");
				object obj = this.sOTSLock;
				lock (obj)
				{
					Logger.Info("Performing OTS completed tasks");
					if (value && this.EventOnOneTimeSetupCompleted != null)
					{
						this.EventOnOneTimeSetupCompleted(this.ParentWindow, new EventArgs());
						this.EventOnOneTimeSetupCompleted = null;
					}
				}
			}
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x000977A0 File Offset: 0x000959A0
		private void StartOTSCheckThread()
		{
			if (this.mOtsCheckThread == null)
			{
				object obj = this.mOtsCheckLock;
				lock (obj)
				{
					if (this.mOtsCheckThread == null)
					{
						try
						{
							this.mOtsCheckThread = new Thread(delegate
							{
								Logger.Info("Checking for if OTS completed");
								while (!this.mIsOneTimeSetupCompleted)
								{
									this.CheckingOneTimeSetupCompleted();
									Thread.Sleep(2 * this.oneSecond);
								}
							})
							{
								IsBackground = true
							};
							this.mOtsCheckThread.Start();
						}
						catch (Exception ex)
						{
							Logger.Error("Failed to create ots check thread.");
							Logger.Error(ex.ToString());
						}
					}
				}
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06001974 RID: 6516 RVA: 0x00011215 File Offset: 0x0000F415
		// (set) Token: 0x06001975 RID: 6517 RVA: 0x0001121D File Offset: 0x0000F41D
		public bool IsGuestReady
		{
			get
			{
				return this.mIsGuestReady;
			}
			set
			{
				this.mIsGuestReady = value;
				if (this.mIsGuestReady)
				{
					this.SignalGuestReady();
				}
			}
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x00097830 File Offset: 0x00095A30
		private void SignalGuestReady()
		{
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				Logger.Info("Boot install: Signal Guest Ready");
				this.ParentWindow.GuestBoot_Completed();
				return;
			}
			this.ParentWindow.Utils.sBootCheckTimer.Enabled = false;
			this.ParentWindow.mEnableLaunchPlayForNCSoft = true;
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x00097884 File Offset: 0x00095A84
		private void CheckingOneTimeSetupCompleted()
		{
			try
			{
				string text = JObject.Parse(HTTPUtils.SendRequestToGuest("isOTSCompleted", null, this.ParentWindow.mVmName, 1000, null, false, 1, 0, "bgp64"))["result"].ToString();
				if (text.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
				{
					Logger.Info("OTS result: {0}", new object[] { text });
					this.IsOneTimeSetupCompleted = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in checking OneTimeSetupCompleted with vmName {0}. Err: {1}", new object[]
				{
					this.ParentWindow.mVmName,
					ex.Message
				});
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06001978 RID: 6520 RVA: 0x00011234 File Offset: 0x0000F434
		// (set) Token: 0x06001979 RID: 6521 RVA: 0x0001123C File Offset: 0x0000F43C
		public string SwitchWhenPackageNameRecieved
		{
			get
			{
				return this.mSwitchWhenPackageNameRecieved;
			}
			set
			{
				this.mSwitchWhenPackageNameRecieved = value;
				if (!string.IsNullOrEmpty(this.mSwitchWhenPackageNameRecieved) && this.mSwitchWhenPackageNameRecieved.Equals(this.mLastAppDisplayed, StringComparison.InvariantCultureIgnoreCase))
				{
					this.AppLaunched(this.mSwitchWhenPackageNameRecieved, true);
				}
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x0600197A RID: 6522 RVA: 0x00011273 File Offset: 0x0000F473
		// (set) Token: 0x0600197B RID: 6523 RVA: 0x0001127B File Offset: 0x0000F47B
		public EventHandler<EventArgs> EventOnOneTimeSetupCompleted { get; set; }

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x0600197C RID: 6524 RVA: 0x00011284 File Offset: 0x0000F484
		// (set) Token: 0x0600197D RID: 6525 RVA: 0x0001128B File Offset: 0x0000F48B
		public static EventHandler<EventArgs> EventOnAppDisplayed { get; set; }

		// Token: 0x0600197E RID: 6526 RVA: 0x00097930 File Offset: 0x00095B30
		internal AppHandler(MainWindow window)
		{
			this.ParentWindow = window;
			string cdnappsTimeStamp = RegistryManager.Instance.CDNAppsTimeStamp;
			if (!string.IsNullOrEmpty(cdnappsTimeStamp))
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(cdnappsTimeStamp)))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(SerializableDictionary<string, DateTime>));
					this.CdnAppdict = (SerializableDictionary<string, DateTime>)xmlSerializer.Deserialize(xmlReader);
				}
			}
			this.mIsOneTimeSetupCompleted = this.ParentWindow.EngineInstanceRegistry.IsOneTimeSetupDone;
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x00097A2C File Offset: 0x00095C2C
		public bool IsAppInstalled(string package)
		{
			bool flag = false;
			string text;
			if (new JsonParser(this.ParentWindow.mVmName).IsAppInstalled(package, out text))
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x00097A58 File Offset: 0x00095C58
		public bool IsAppInstalled(string package, out string version)
		{
			bool flag = false;
			if (new JsonParser(this.ParentWindow.mVmName).IsAppInstalled(package, out version))
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x00097A84 File Offset: 0x00095C84
		public void AppLaunched(string packageName, bool forced = false)
		{
			object obj = this.sLockObject;
			lock (obj)
			{
				if (!this.ParentWindow.mClosed)
				{
					if ((packageName == BlueStacksUIUtils.sUserAccountPackageName || packageName == "com.android.vending") && this.mSwitchWhenPackageNameRecieved == "com.android.vending")
					{
						packageName = this.mSwitchWhenPackageNameRecieved;
						if (string.Compare(this.mLastRunAppSentForSynced, packageName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							this.mSwitchWhenPackageNameRecieved = "";
						}
					}
					if (forced || !string.Equals(packageName, this.mLastAppDisplayed, StringComparison.InvariantCultureIgnoreCase))
					{
						if (!this.mIsOneTimeSetupCompleted)
						{
							if (!string.IsNullOrEmpty(packageName) && (packageName.StartsWith("com.google.android.gms", StringComparison.InvariantCultureIgnoreCase) || packageName.Equals("com.google.android.setupwizard", StringComparison.InvariantCultureIgnoreCase)))
							{
								this.StartOTSCheckThread();
							}
						}
						else
						{
							Logger.Info("SwitchWhenPackageNameRecieved: {0}", new object[] { this.mSwitchWhenPackageNameRecieved });
							this.ParentWindow.ShowLoadingGrid(false);
							bool receivedFromImap = string.Compare(this.mLastRunAppSentForSynced, packageName, StringComparison.OrdinalIgnoreCase) == 0;
							if (receivedFromImap)
							{
								this.mLastRunAppSentForSynced = "";
							}
							if (!string.IsNullOrEmpty(this.mSwitchWhenPackageNameRecieved) && string.Equals(packageName, this.mSwitchWhenPackageNameRecieved, StringComparison.OrdinalIgnoreCase))
							{
								this.mSwitchWhenPackageNameRecieved = string.Empty;
								if (AppHandler.EventOnAppDisplayed == null)
								{
									this.ParentWindow.Dispatcher.Invoke(new Action(delegate
									{
										this.ParentWindow.mTopBar.mAppTabButtons.GoToTab(packageName, receivedFromImap, false);
										Publisher.PublishMessage(BrowserControlTags.tabSwitched, this.ParentWindow.mVmName, new JObject(new JProperty("PackageName", packageName)));
									}), new object[0]);
								}
								else
								{
									EventHandler<EventArgs> eventOnAppDisplayed = AppHandler.EventOnAppDisplayed;
									AppHandler.EventOnAppDisplayed = null;
									eventOnAppDisplayed(this.ParentWindow, new EventArgs());
								}
							}
							else if (this.mDefaultLauncher.Equals(packageName, StringComparison.InvariantCultureIgnoreCase))
							{
								if (!FeatureManager.Instance.IsCustomUIForDMM)
								{
									Logger.Info("Assuming app is crashed/exited going to last tab");
									this.ParentWindow.Dispatcher.Invoke(new Action(delegate
									{
										if (this.ParentWindow.mFrontendGrid != null)
										{
											if (this.ParentWindow.mFrontendGrid.Parent as Grid == this.ParentWindow.FrontendParentGrid)
											{
												if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
												{
													this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey, false, false, true, false, packageName);
												}
												if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
												{
													this.PerformGamingAction("", "");
													return;
												}
											}
											else
											{
												this.ParentWindow.mWelcomeTab.mFrontendPopupControl.HideWindow();
											}
										}
									}), new object[0]);
								}
							}
							else
							{
								AppIconModel icon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
								if (icon != null)
								{
									this.ParentWindow.Dispatcher.Invoke(new Action(delegate
									{
										this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(icon.AppName, icon.PackageName, icon.ActivityName, icon.ImageName, true, false, receivedFromImap);
									}), new object[0]);
								}
							}
							this.mLastAppDisplayed = packageName;
						}
					}
				}
			}
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x00011293 File Offset: 0x0000F493
		public void HandleAppDisplayed(string packageName)
		{
			if (string.Equals(packageName, this.mDefaultLauncher, StringComparison.InvariantCultureIgnoreCase))
			{
				Logger.Info("Home app is displayed...closing tab");
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					if (this.ParentWindow.mFrontendGrid != null && this.ParentWindow.mFrontendGrid.Parent as Grid == this.ParentWindow.FrontendParentGrid && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
					{
						this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey, false, false, true, false, "");
					}
				}), new object[0]);
			}
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x000112D1 File Offset: 0x0000F4D1
		internal void GoHome()
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				VmCmdHandler.RunCommand("home", this.ParentWindow.mVmName);
			});
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x00097D68 File Offset: 0x00095F68
		public string GetDefaultLauncher()
		{
			string text = "com.bluestacks.appmart";
			try
			{
				string text2 = HTTPUtils.SendRequestToGuest("getDefaultLauncher", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
				Logger.Info("GetDefaultLauncher response = " + text2);
				JObject jobject = JObject.Parse(text2);
				string text3 = jobject["result"].ToString().Trim();
				if (text3 == "ok")
				{
					text = jobject["defaultLauncher"].ToString().Trim();
				}
				else if (text3 == "error" && jobject["reason"].ToString().Trim() == "no default launcher")
				{
					text = "none";
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetDefauntLauncher. Err." + ex.ToString());
			}
			return text;
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x000112E5 File Offset: 0x0000F4E5
		internal void StartCustomActivity(Dictionary<string, string> data)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Logger.Info("Starting a custom activity");
					foreach (KeyValuePair<string, string> keyValuePair in data)
					{
						Logger.Debug("Data = {0} , {1}", new object[] { keyValuePair.Key, keyValuePair.Value });
					}
					HTTPUtils.SendRequestToGuest("customStartActivity", data, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in launching custom activity. Err: " + ex.Message);
				}
			});
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x0001130B File Offset: 0x0000F50B
		internal void SetDefaultLauncher(string launcherName)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string> { { "d", launcherName } };
					string text = HTTPUtils.SendRequestToGuest("setDefaultLauncher", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
					Logger.Info("Setlauncher res: {0}", new object[] { text });
					dictionary = new Dictionary<string, string> { { "arg", "" } };
					text = HTTPUtils.SendRequestToGuest("home", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
					Logger.Info("the response for home command is {0}", new object[] { text });
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in SetDefaultLauncher. Err:{0}", new object[] { ex.ToString() });
				}
			});
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x00097E50 File Offset: 0x00096050
		internal void AppUninstalled(string package)
		{
			this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppAfterUninstall(package);
			this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(package, false, false, true, false, "");
			if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(package))
			{
				if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedLandscapeEnabled)
				{
					Utils.SetCustomAppSize(this.ParentWindow.mVmName, package, ScreenMode.original);
					KMManager.SelectSchemeIfPresent(this.ParentWindow, "Portrait", "appuninstalled", false);
					AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedLandscapeEnabled = false;
				}
				if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedPortraitEnabled)
				{
					Utils.SetCustomAppSize(this.ParentWindow.mVmName, package, ScreenMode.original);
					KMManager.SelectSchemeIfPresent(this.ParentWindow, "Landscape", "appuninstalled", false);
					AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedPortraitEnabled = false;
				}
			}
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x00097FA4 File Offset: 0x000961A4
		internal void AppInstalled(string package, bool isUpdate)
		{
			AppInfo appInfo = this.ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(package);
			JObject jobject = new JObject();
			jobject["PackageName"] = package;
			jobject["AppName"] = ((appInfo != null) ? appInfo.Name : null);
			jobject["IsGamepadCompatible"] = ((appInfo != null) ? new bool?(appInfo.IsGamepadCompatible) : null);
			JObject jobject2 = jobject;
			Publisher.PublishMessage(BrowserControlTags.appInstalled, this.ParentWindow.mVmName, jobject2);
			if (FeatureManager.Instance.IsShowAppRecommendations || !RegistryManager.Instance.IsPremium)
			{
				this.ParentWindow.mWelcomeTab.mHomeAppManager.UpdateRecommendedAppsInstallStatus(package);
			}
			GrmHandler.RefreshGrmIndication(package, this.ParentWindow.mVmName);
			GrmHandler.SendUpdateGrmPackagesToAndroid(this.ParentWindow.mVmName);
			GrmHandler.SendUpdateGrmPackagesToBrowser(this.ParentWindow.mVmName);
			GuidanceCloudInfoManager.Instance.AppsGuidanceCloudInfoRefresh();
			if (RegistryManager.Instance.FirstAppLaunchState == AppLaunchState.Fresh)
			{
				RegistryManager.Instance.FirstAppLaunchState = AppLaunchState.Installed;
			}
			if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(package))
			{
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package] = new AppSettings();
			}
			if (!isUpdate)
			{
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsAppOnboardingCompleted = false;
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsGeneralAppOnBoardingCompleted = false;
			}
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x00011331 File Offset: 0x0000F531
		internal void UpdateDefaultLauncher()
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				string text = this.GetDefaultLauncher();
				Logger.Info("DefaultLauncher " + text);
				if (text.Equals("none", StringComparison.InvariantCultureIgnoreCase))
				{
					text = "com.bluestacks.appmart";
					this.SetDefaultLauncher(text);
				}
				if (text.Equals("com.android.provision", StringComparison.InvariantCultureIgnoreCase))
				{
					text = "com.bluestacks.appmart";
				}
				this.mDefaultLauncher = text;
			});
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x00011345 File Offset: 0x0000F545
		internal void SendSearchPlayRequestAsync(string searchQuery)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				if (searchQuery.Contains("search::"))
				{
					searchQuery = searchQuery.Remove(0, 8);
				}
				VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "searchplay {0}", new object[] { searchQuery }), this.ParentWindow.mVmName);
			});
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x0001136B File Offset: 0x0000F56B
		internal void LaunchPlayRequestAsync(string packageName)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "launchplay?pkgname={0}", new object[] { packageName }), this.ParentWindow.mVmName);
			});
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x00011391 File Offset: 0x0000F591
		public void SendRunAppRequestAsync(string package, string activity = "", bool receivedFromImap = false)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				if (this.ParentWindow.SendClientActions && !receivedFromImap)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>
					{
						{ "EventAction", "RunApp" },
						{ "Package", package },
						{ "Activity", activity }
					};
					JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
					serializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
					dictionary.Add("operationData", JsonConvert.SerializeObject(dictionary2, serializerSettings));
					this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
				}
				if (receivedFromImap)
				{
					this.mLastRunAppSentForSynced = package;
					if (package == "com.android.vending")
					{
						this.mSwitchWhenPackageNameRecieved = package;
					}
				}
				if (string.IsNullOrEmpty(activity))
				{
					AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(package);
					if (appIcon != null)
					{
						activity = appIcon.ActivityName;
					}
					if (string.IsNullOrEmpty(activity))
					{
						activity = ".Main";
						Logger.Info("Empty activity name ovveriding .Main for package: " + package);
					}
				}
				if (PackageActivityNames.ThirdParty.AllPUBGPackageNames.Contains(package))
				{
					string displayQualityPubg = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityPubg;
					string gamingResolutionPubg = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionPubg;
					if (string.Equals(displayQualityPubg, "-1", StringComparison.InvariantCulture) && string.Equals(gamingResolutionPubg, "1", StringComparison.InvariantCulture))
					{
						this.SendRunex(package, activity);
						return;
					}
					StringBuilder stringBuilder = new StringBuilder();
					using (JsonWriter jsonWriter = new JsonTextWriter(new StringWriter(stringBuilder)))
					{
						jsonWriter.WriteStartObject();
						if (string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityPubg, "-1", StringComparison.InvariantCulture))
						{
							jsonWriter.WritePropertyName("renderqualitylevel");
							jsonWriter.WriteValue("0");
						}
						else
						{
							jsonWriter.WritePropertyName("renderqualitylevel");
							jsonWriter.WriteValue(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityPubg);
						}
						jsonWriter.WritePropertyName("contentscale");
						jsonWriter.WriteValue(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionPubg);
						jsonWriter.WriteEndObject();
					}
					Dictionary<string, string> dictionary3 = new Dictionary<string, string>
					{
						{
							"component",
							package + "/" + activity
						},
						{
							"extras",
							stringBuilder.ToString()
						}
					};
					string text = HTTPUtils.SendRequestToGuest("customStartActivity", dictionary3, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
					Logger.Info("The response we get is: " + text);
					return;
				}
				else
				{
					if (PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(package))
					{
						int num = int.Parse(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD, CultureInfo.InvariantCulture);
						int num2 = int.Parse(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD, CultureInfo.InvariantCulture);
						int num3 = int.Parse("1", CultureInfo.InvariantCulture);
						StringBuilder stringBuilder2 = new StringBuilder();
						using (JsonWriter jsonWriter2 = new JsonTextWriter(new StringWriter(stringBuilder2)))
						{
							jsonWriter2.WriteStartObject();
							if (string.Equals(num.ToString(CultureInfo.InvariantCulture), "-1", StringComparison.InvariantCulture))
							{
								jsonWriter2.WritePropertyName("QualityLevel");
								jsonWriter2.WriteValue(int.Parse("0", CultureInfo.InvariantCulture));
							}
							else
							{
								jsonWriter2.WritePropertyName("QualityLevel");
								jsonWriter2.WriteValue(num);
							}
							jsonWriter2.WritePropertyName("ResolutionHeight");
							jsonWriter2.WriteValue(num2);
							jsonWriter2.WritePropertyName("FrameRateLevel");
							jsonWriter2.WriteValue(num3);
							jsonWriter2.WriteEndObject();
						}
						Dictionary<string, string> dictionary4 = new Dictionary<string, string>
						{
							{
								"component",
								package + "/" + activity
							},
							{
								"extras",
								stringBuilder2.ToString()
							}
						};
						string text2 = HTTPUtils.SendRequestToGuest("customStartActivity", dictionary4, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
						Logger.Info("The response we get is: " + text2);
						return;
					}
					if ("com.android.chrome".Equals(package, StringComparison.InvariantCultureIgnoreCase))
					{
						HTTPUtils.SendRequestToGuest("launchchrome", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
						return;
					}
					this.SendRunex(package, activity);
					return;
				}
			});
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x000113C5 File Offset: 0x0000F5C5
		internal void SendRunex(string package, string activity)
		{
			VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "runex {0}/{1}", new object[] { package, activity }), this.ParentWindow.mVmName);
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x000113F5 File Offset: 0x0000F5F5
		internal void StopAppRequest(string packageName)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Logger.Info("Will send stop {0} request", new object[] { packageName });
					Dictionary<string, string> dictionary = new Dictionary<string, string> { { "appPackage", packageName } };
					string text = this.ParentWindow.mFrontendHandler.SendFrontendRequest("stopAppInfo", dictionary);
					Logger.Info("the response we get is {0}", new object[] { text });
					Logger.Info(VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "StopApp {0}", new object[] { packageName }), this.ParentWindow.mVmName));
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in StopAppRequest. Err : " + ex.ToString());
				}
			});
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x0001141B File Offset: 0x0000F61B
		internal void SendRequestToRemoveAccountAndCloseWindowASync(bool closeWindow = false)
		{
			Action <>9__1;
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					string text = HTTPUtils.SendRequestToGuest("removeAccountsInfo", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
					Logger.Info("Account removed response: " + text);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in removing account, Ex: " + ex.Message);
				}
				if (closeWindow)
				{
					Dispatcher dispatcher = this.ParentWindow.Dispatcher;
					Action action;
					if ((action = <>9__1) == null)
					{
						action = (<>9__1 = delegate
						{
							this.ParentWindow.ForceCloseWindow(false);
						});
					}
					dispatcher.Invoke(action, new object[0]);
				}
			});
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x00098150 File Offset: 0x00096350
		internal void WriteXMl(bool isAppInstall, string packageName, DateTime timestamp)
		{
			if (isAppInstall)
			{
				this.CdnAppdict[packageName] = timestamp;
				using (StringWriter stringWriter = new StringWriter())
				{
					new XmlSerializer(typeof(SerializableDictionary<string, DateTime>)).Serialize(stringWriter, this.CdnAppdict);
					RegistryManager.Instance.CDNAppsTimeStamp = stringWriter.ToString();
					return;
				}
			}
			if (this.CdnAppdict.ContainsKey(packageName))
			{
				this.CdnAppdict.Remove(packageName);
				using (StringWriter stringWriter2 = new StringWriter())
				{
					new XmlSerializer(typeof(SerializableDictionary<string, DateTime>)).Serialize(stringWriter2, this.CdnAppdict);
					RegistryManager.Instance.CDNAppsTimeStamp = stringWriter2.ToString();
				}
			}
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x00098220 File Offset: 0x00096420
		internal void PerformGamingAction(string pkgName = "", string activityName = "")
		{
			GenericAction action;
			if (string.IsNullOrEmpty(pkgName))
			{
				pkgName = GameConfig.Instance.PkgName;
				activityName = GameConfig.Instance.ActivityName;
				action = GameConfig.Instance.AppGenericAction;
			}
			else
			{
				action = GenericAction.InstallPlay;
			}
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (this.IsAppInstalled(pkgName))
				{
					this.SendRunAppRequestAsync(pkgName, "", false);
					return;
				}
				if (action == GenericAction.InstallPlay)
				{
					this.LaunchPlayRequestAsync(pkgName);
				}
			}), new object[0]);
		}

		// Token: 0x0400100D RID: 4109
		private MainWindow ParentWindow;

		// Token: 0x04001011 RID: 4113
		private Thread mOtsCheckThread;

		// Token: 0x04001012 RID: 4114
		private object mOtsCheckLock = new object();

		// Token: 0x04001013 RID: 4115
		private int oneSecond = 1000;

		// Token: 0x04001015 RID: 4117
		private bool mIsOneTimeSetupCompleted;

		// Token: 0x04001016 RID: 4118
		private bool mIsGuestReady;

		// Token: 0x04001017 RID: 4119
		internal bool mGuestReadyCheckStarted;

		// Token: 0x04001018 RID: 4120
		internal string mDefaultLauncher = "com.bluestacks.appmart";

		// Token: 0x04001019 RID: 4121
		private object sLockObject = new object();

		// Token: 0x0400101A RID: 4122
		private object sOTSLock = new object();

		// Token: 0x0400101B RID: 4123
		private string mSwitchWhenPackageNameRecieved = string.Empty;
	}
}
