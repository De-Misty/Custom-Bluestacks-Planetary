using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000031 RID: 49
	public partial class AppTabButtons : UserControl
	{
		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000364 RID: 868 RVA: 0x0000438E File Offset: 0x0000258E
		public MainWindow ParentWindow
		{
			get
			{
				if (this.mMainWindow == null)
				{
					this.mMainWindow = Window.GetWindow(this) as MainWindow;
				}
				return this.mMainWindow;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000365 RID: 869 RVA: 0x000043AF File Offset: 0x000025AF
		// (set) Token: 0x06000366 RID: 870 RVA: 0x000043B7 File Offset: 0x000025B7
		public EventHandler<EventArgs> EventOnTabChanged { get; set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000367 RID: 871 RVA: 0x000043C0 File Offset: 0x000025C0
		public List<string> ListTabHistory { get; } = new List<string>();

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000368 RID: 872 RVA: 0x0001745C File Offset: 0x0001565C
		public int AreaForTABS
		{
			get
			{
				int num = (int)(base.ActualWidth - 20.0);
				if (num < 0)
				{
					num = 0;
				}
				return num;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000369 RID: 873 RVA: 0x000043C8 File Offset: 0x000025C8
		internal AppTabButton SelectedTab
		{
			get
			{
				return this.ParentWindow.StaticComponents.mSelectedTabButton;
			}
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00017484 File Offset: 0x00015684
		public AppTabButtons()
		{
			this.InitializeComponent();
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				base.SizeChanged += this.Window_SizeChanged;
				base.Loaded += this.AppTabButtons_Loaded;
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x000043DA File Offset: 0x000025DA
		private void AppTabButtons_Loaded(object sender, RoutedEventArgs e)
		{
			base.Loaded -= this.AppTabButtons_Loaded;
			if (!FeatureManager.Instance.IsCustomUIForDMM && RegistryManager.Instance.InstallationType == InstallationTypes.FullEdition)
			{
				Logger.Info("Test logs: AppTabButtons_Loaded()");
				this.AddHomeTab();
			}
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0001750C File Offset: 0x0001570C
		internal void AddHomeTab()
		{
			Logger.Info("Test logs: AddHomeTab()");
			AppTabButton appTabButton = new AppTabButton();
			this.mHomeAppTabButton = appTabButton;
			this.mPanel.Children.Insert(0, appTabButton);
			appTabButton.Init("STRING_HOME", "Home", string.Empty, "home", this.ParentWindow.WelcomeTabParentGrid, "Home");
			BlueStacksUIBinding.Bind(appTabButton.mTabLabel, "STRING_HOME");
			appTabButton.MouseUp += this.AppTabButton_MouseUp;
			this.mDictTabs[appTabButton.PackageName] = appTabButton;
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				appTabButton.Visibility = Visibility.Collapsed;
			}
			this.ResizeTabs();
			this.GoToTab("Home", false, false);
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00004417 File Offset: 0x00002617
		internal void AddHiddenAppTabAndLaunch(string packageName, string activityName)
		{
			this.AddAppTab("", packageName, activityName, "", true, true, false);
			this.ParentWindow.StaticComponents.mSelectedTabButton.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600036E RID: 878 RVA: 0x000175C8 File Offset: 0x000157C8
		internal void AddAppTab(string appName, string packageName, string activityName, string imageName, bool isSwitch, bool isLaunch, bool receivedFromImap = false)
		{
			this.DoExtraHandlingForApp(packageName);
			PostBootCloudInfo mPostBootCloudInfo = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
			bool? flag;
			if (mPostBootCloudInfo == null)
			{
				flag = null;
			}
			else
			{
				NotificationModeInfo gameNotificationAppPackages = mPostBootCloudInfo.GameNotificationAppPackages;
				if (gameNotificationAppPackages == null)
				{
					flag = null;
				}
				else
				{
					AppPackageListObject notificationModeAppPackages = gameNotificationAppPackages.NotificationModeAppPackages;
					flag = ((notificationModeAppPackages != null) ? new bool?(notificationModeAppPackages.IsPackageAvailable(packageName)) : null);
				}
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				this.ParentWindow.EngineInstanceRegistry.LastNotificationEnabledAppLaunched = packageName;
			}
			if (this.mDictTabs.ContainsKey(packageName))
			{
				this.GoToTab(packageName, isLaunch, receivedFromImap);
				return;
			}
			AppTabButton mSelectedTabButton = this.ParentWindow.StaticComponents.mSelectedTabButton;
			AppTabButton appTabButton = new AppTabButton();
			appTabButton.Init(appName, packageName, activityName, imageName, this.ParentWindow.FrontendParentGrid, packageName);
			appTabButton.MouseUp += this.AppTabButton_MouseUp;
			if (this.ParentWindow.mDiscordhandler != null)
			{
				this.ParentWindow.mDiscordhandler.AssignTabChangeEvent(appTabButton);
			}
			if (FeatureManager.Instance.IsCustomUIForDMM && this.ParentWindow.mDmmBottomBar != null)
			{
				AppTabButton appTabButton2 = appTabButton;
				appTabButton2.EventOnTabChanged = (EventHandler<TabChangeEventArgs>)Delegate.Combine(appTabButton2.EventOnTabChanged, new EventHandler<TabChangeEventArgs>(this.ParentWindow.mDmmBottomBar.Tab_Changed));
			}
			this.mDictTabs.Add(packageName, appTabButton);
			this.mPanel.Children.Add(appTabButton);
			if (Oem.Instance.SendAppClickStatsFromClient)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					AppInfo appInfoFromPackageName = new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromPackageName(packageName);
					string text = string.Empty;
					string text2 = string.Empty;
					if (appInfoFromPackageName != null)
					{
						if (!string.IsNullOrEmpty(appInfoFromPackageName.Version))
						{
							text = appInfoFromPackageName.Version;
						}
						if (!string.IsNullOrEmpty(appInfoFromPackageName.VersionName))
						{
							text2 = appInfoFromPackageName.VersionName;
						}
					}
					Stats.SendAppStats(appName, packageName, text, "HomeVersionNotKnown", Stats.AppType.app, this.ParentWindow.mVmName, text2);
				});
			}
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				appTabButton.Visibility = Visibility.Collapsed;
			}
			else if (mSelectedTabButton != null && mSelectedTabButton.IsPortraitModeTab && mSelectedTabButton.mTabType == TabType.AppTab)
			{
				appTabButton.IsPortraitModeTab = true;
			}
			this.ResizeTabs();
			if (isSwitch)
			{
				this.GoToTab(packageName, isLaunch, receivedFromImap);
			}
		}

		// Token: 0x0600036F RID: 879 RVA: 0x000177D4 File Offset: 0x000159D4
		private void DoExtraHandlingForApp(string packageName)
		{
			if (RegistryManager.Instance.FirstAppLaunchState == AppLaunchState.Installed && JsonParser.GetInstalledAppsList(this.ParentWindow.mVmName).Contains(packageName))
			{
				RegistryManager.Instance.FirstAppLaunchState = AppLaunchState.Launched;
			}
			if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(packageName))
			{
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName] = new AppSettings();
			}
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00017858 File Offset: 0x00015A58
		internal void AddWebTab(string url, string tabName, string imageName, bool isSwitch, string tabKey = "", bool forceRefresh = false)
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				return;
			}
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				Process.Start(url);
				return;
			}
			bool flag = false;
			if (!string.IsNullOrEmpty(tabKey))
			{
				flag = true;
			}
			if (this.mDictTabs.ContainsKey(flag ? tabKey : url))
			{
				if (this.mDictTabs[flag ? tabKey : url].GetBrowserControl() == null)
				{
					this.mDictTabs[tabKey].mControlGrid = this.ParentWindow.AddBrowser(url);
					this.mDictTabs[tabKey].Init(tabName, url, imageName, this.mDictTabs[tabKey].mControlGrid, tabKey);
				}
				if (flag && string.Compare(url, this.mDictTabs[tabKey].PackageName, StringComparison.OrdinalIgnoreCase) != 0)
				{
					BrowserControl browserControl = this.mDictTabs[tabKey].GetBrowserControl();
					this.mDictTabs[tabKey].Init(tabName, url, imageName, this.mDictTabs[tabKey].mControlGrid, tabKey);
					if (browserControl != null)
					{
						browserControl.UpdateUrlAndRefresh(url);
					}
				}
				else if (forceRefresh)
				{
					BrowserControl browserControl = this.mDictTabs[flag ? tabKey : url].GetBrowserControl();
					browserControl.UpdateUrlAndRefresh(browserControl.mUrl);
				}
				this.GoToTab(flag ? tabKey : url, true, false);
				return;
			}
			AppTabButton appTabButton = new AppTabButton();
			Grid grid = this.ParentWindow.AddBrowser(url);
			grid.Visibility = Visibility.Visible;
			appTabButton.Init(tabName, url, imageName, grid, flag ? tabKey : url);
			appTabButton.MouseUp += this.AppTabButton_MouseUp;
			if (this.ParentWindow.mDiscordhandler != null)
			{
				this.ParentWindow.mDiscordhandler.AssignTabChangeEvent(appTabButton);
			}
			this.mDictTabs.Add(flag ? tabKey : url, appTabButton);
			this.mPanel.Children.Add(appTabButton);
			this.ResizeTabs();
			if (isSwitch)
			{
				this.GoToTab(flag ? tabKey : url, true, false);
			}
			ClientStats.SendMiscellaneousStatsAsync("WebTabLaunched", RegistryManager.Instance.UserGuid, url, appTabButton.AppLabel, RegistryManager.Instance.Version, Oem.Instance.OEM, null, null, null);
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00017A80 File Offset: 0x00015C80
		internal void KillWebTabs()
		{
			if (RegistryManager.Instance.SwitchKillWebTab)
			{
				foreach (KeyValuePair<string, AppTabButton> keyValuePair in this.mDictTabs)
				{
					if (keyValuePair.Value.mTabType == TabType.WebTab)
					{
						BrowserControl browserControl = null;
						foreach (object obj in keyValuePair.Value.mControlGrid.Children)
						{
							browserControl = obj as BrowserControl;
							if (browserControl != null && browserControl.CefBrowser != null)
							{
								foreach (BrowserControlTags browserControlTags in browserControl.TagsSubscribedDict.Keys)
								{
									BrowserSubscriber mSubscriber = browserControl.mSubscriber;
									if (mSubscriber != null)
									{
										mSubscriber.UnsubscribeTag(browserControlTags);
									}
								}
								browserControl.CefBrowser.Dispose();
								browserControl.CefBrowser = null;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00017BB8 File Offset: 0x00015DB8
		private void AppTabButton_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				string tabKey = (sender as AppTabButton).TabKey;
				if (!string.IsNullOrEmpty(tabKey))
				{
					this.CloseTab(tabKey, true, false, false, false, "");
				}
			}
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00004444 File Offset: 0x00002644
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!this.ParentWindow.mIsFullScreen)
			{
				this.RefreshUI();
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00017BF4 File Offset: 0x00015DF4
		private void RefreshUI()
		{
			if ((DateTime.Now - this.mLastTimeOfSizeChangeEventRecieved).TotalSeconds > 2.0)
			{
				this.mLastTimeOfSizeChangeEventRecieved = DateTime.Now;
				this.mSizeChangedEventCountInLast2Seconds = 1;
			}
			else
			{
				this.mSizeChangedEventCountInLast2Seconds++;
			}
			if (this.mSizeChangedEventCountInLast2Seconds > 500)
			{
				return;
			}
			if (this.ParentWindow.IsUIInPortraitMode)
			{
				this.SwitchToIconMode(true);
			}
			else
			{
				this.SwitchToIconMode(false);
			}
			this.ResizeTabs();
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00017C78 File Offset: 0x00015E78
		private void SwitchToIconMode(bool isSwitchToIconMode)
		{
			if (isSwitchToIconMode)
			{
				this.mTabMinWidth = 38;
				if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
				{
					this.mMoreTabButton.Visibility = Visibility.Hidden;
					this.ParentWindow.mTopBar.mTitleText.Visibility = Visibility.Collapsed;
				}
				else if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.ParentWindow.mTopBar.mTitleTextGrid.Visibility = Visibility.Collapsed;
				}
				this.mMoreTabButton.MakeTabParallelogram(false);
			}
			else
			{
				if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
				{
					this.ParentWindow.mTopBar.mTitleText.Visibility = Visibility.Visible;
				}
				this.mTabMinWidth = 48;
				this.mMoreTabButton.MakeTabParallelogram(true);
			}
			this.ParentWindow.mTopBar.RefreshWarningButton();
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00017D38 File Offset: 0x00015F38
		internal void ResizeTabs()
		{
			if (!this.ParentWindow.mIsFullScreen)
			{
				double num = this.MacroGridHandling();
				num += this.VideoRecordingGridHandling();
				if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num + 40.0)
				{
					this.ParentWindow.mTopBar.mTitleIcon.Visibility = Visibility.Visible;
				}
				else
				{
					this.ParentWindow.mTopBar.mTitleIcon.Visibility = Visibility.Collapsed;
				}
				if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + 140.0 + num + (double)(this.mDictTabs.Count * 48))
				{
					this.ParentWindow.mTopBar.mTitleTextGrid.Visibility = Visibility.Visible;
				}
				else
				{
					this.ParentWindow.mTopBar.mTitleTextGrid.Visibility = Visibility.Collapsed;
				}
				int num2 = this.mPanel.Children.Count + this.mHiddenButtons.Children.Count;
				if (num2 > 0)
				{
					double num3 = (double)this.mTabMinWidth;
					if (this.AreaForTABS >= num2 * this.mTabMinWidth)
					{
						num3 = (double)(this.AreaForTABS / num2);
					}
					for (int i = 0; i < this.mPanel.Children.Count; i++)
					{
						(this.mPanel.Children[i] as AppTabButton).ResizeButton(num3);
					}
					if ((double)this.AreaForTABS >= num3 * (double)num2)
					{
						if (this.mHiddenButtons.Children.Count > 0)
						{
							this.ShowXTabs(this.mHiddenButtons.Children.Count, num3);
						}
					}
					else
					{
						int num4 = this.AreaForTABS / this.mTabMinWidth - 1;
						if (FeatureManager.Instance.IsCustomUIForDMM)
						{
							int num5 = (int)Math.Floor(BlueStacksUIBinding.Instance.CornerRadiusModel["TabMarginPortrait"].TopLeft);
							int num6 = (int)Math.Floor(this.mMoreTabButton.ActualWidth) + num5;
							num4 = (this.AreaForTABS - num6) / (this.mTabMinWidth + num5);
						}
						if (num4 > num2)
						{
							num4 = num2;
						}
						if (num4 > this.mPanel.Children.Count || num2 == 1)
						{
							this.ShowXTabs(num4 - this.mPanel.Children.Count, num3);
						}
						else if (num4 < this.mPanel.Children.Count)
						{
							this.HideXTabs(this.mPanel.Children.Count - num4);
						}
					}
				}
				if (this.mHiddenButtons.Children.Count > 0)
				{
					this.mMoreTabButton.Visibility = Visibility.Visible;
					this.mMoreTabButton.MoreTabsButtonHandling();
					return;
				}
				this.mMoreTabButton.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00017FFC File Offset: 0x000161FC
		private double MacroGridHandling()
		{
			double num = 0.0;
			if (this.ParentWindow.mTopBar.mMacroRecordControl.Visibility == Visibility.Visible)
			{
				num = this.ParentWindow.mTopBar.mMacroRecordControl.MaxWidth;
			}
			else if (this.ParentWindow.mTopBar.mMacroPlayControl.Visibility == Visibility.Visible)
			{
				num = this.ParentWindow.mTopBar.mMacroPlayControl.MaxWidth;
			}
			if (num > 0.0)
			{
				if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num)
				{
					this.ParentWindow.mTopBar.mMacroRecordControl.TimerDisplay.Visibility = Visibility.Visible;
					this.ParentWindow.mTopBar.mMacroPlayControl.mDescriptionPanel.Visibility = Visibility.Visible;
				}
				else
				{
					this.ParentWindow.mTopBar.mMacroRecordControl.TimerDisplay.Visibility = Visibility.Collapsed;
					this.ParentWindow.mTopBar.mMacroPlayControl.mDescriptionPanel.Visibility = Visibility.Collapsed;
				}
			}
			return num;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00018114 File Offset: 0x00016314
		private double VideoRecordingGridHandling()
		{
			double num = 0.0;
			if (this.ParentWindow.mTopBar.mVideoRecordStatusControl.Visibility == Visibility.Visible)
			{
				num = this.ParentWindow.mTopBar.mVideoRecordStatusControl.MaxWidth;
			}
			if (num > 0.0)
			{
				if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num)
				{
					this.ParentWindow.mTopBar.mVideoRecordStatusControl.mDescriptionPanel.Visibility = Visibility.Visible;
				}
				else
				{
					this.ParentWindow.mTopBar.mVideoRecordStatusControl.mDescriptionPanel.Visibility = Visibility.Collapsed;
				}
			}
			return num;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000181C4 File Offset: 0x000163C4
		private void ShowXTabs(int x, double tabWidth)
		{
			for (int i = 0; i < x; i++)
			{
				AppTabButton appTabButton = this.mDictTabs.Values.First<AppTabButton>();
				foreach (AppTabButton appTabButton2 in this.mDictTabs.Values)
				{
					if (this.mHiddenButtons.Children.Contains(appTabButton2))
					{
						appTabButton = appTabButton2;
						break;
					}
				}
				appTabButton.ResizeButton(tabWidth);
				appTabButton.UpdateUIForDropDown(false);
				if (!this.mPanel.Children.Contains(appTabButton))
				{
					this.mHiddenButtons.Children.Remove(appTabButton);
					if (appTabButton.mTabType == TabType.HomeTab)
					{
						this.mPanel.Children.Insert(0, appTabButton);
					}
					else
					{
						this.mPanel.Children.Add(appTabButton);
					}
				}
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000182B0 File Offset: 0x000164B0
		private void HideXTabs(int x)
		{
			for (int i = 0; i < x; i++)
			{
				AppTabButton appTabButton = this.mDictTabs.Values.Last<AppTabButton>();
				for (int j = this.mDictTabs.Count - 1; j >= 0; j--)
				{
					AppTabButton value = this.mDictTabs.ElementAt(j).Value;
					if (this.mPanel.Children.Contains(value))
					{
						appTabButton = value;
						break;
					}
				}
				appTabButton.UpdateUIForDropDown(true);
				if (!this.mHiddenButtons.Children.Contains(appTabButton))
				{
					this.mPanel.Children.Remove(appTabButton);
					this.mHiddenButtons.Children.Add(appTabButton);
				}
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00018364 File Offset: 0x00016564
		internal void CloseTab(string tabKey, bool sendStopAppToAndroid = false, bool forceClose = false, bool dontCheckQuitPopup = false, bool receivedFromImap = false, string topActivityPackageName = "")
		{
			if (this.mDictTabs.ContainsKey(tabKey))
			{
				if (this.ParentWindow.SendClientActions && !receivedFromImap)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>
					{
						{ "EventAction", "TabClosed" },
						{ "tabKey", tabKey },
						{
							"sendStopAppToAndroid",
							sendStopAppToAndroid.ToString(CultureInfo.InvariantCulture)
						},
						{
							"forceClose",
							forceClose.ToString(CultureInfo.InvariantCulture)
						}
					};
					JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
					serializerSettings.Formatting = Formatting.None;
					dictionary.Add("operationData", JsonConvert.SerializeObject(dictionary2, serializerSettings));
					this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
				}
				AppTabButton appTabButton = this.mDictTabs[tabKey];
				if (appTabButton.mTabType == TabType.WebTab)
				{
					BrowserControl browserControl = null;
					foreach (object obj in appTabButton.mControlGrid.Children)
					{
						browserControl = obj as BrowserControl;
						if (browserControl != null)
						{
							break;
						}
					}
					string text = string.Empty;
					if (browserControl != null)
					{
						text = browserControl.mUrl;
						appTabButton.mControlGrid.Children.Remove(browserControl);
						if (browserControl.CefBrowser != null)
						{
							foreach (BrowserControlTags browserControlTags in browserControl.TagsSubscribedDict.Keys)
							{
								BrowserSubscriber mSubscriber = browserControl.mSubscriber;
								if (mSubscriber != null)
								{
									mSubscriber.UnsubscribeTag(browserControlTags);
								}
							}
							browserControl.CefBrowser.Dispose();
						}
					}
					ClientStats.SendMiscellaneousStatsAsync("WebTabClosed", RegistryManager.Instance.UserGuid, text, appTabButton.AppLabel, RegistryManager.Instance.Version, Oem.Instance.OEM, null, null, null);
				}
				if (FeatureManager.Instance.IsCheckForQuitPopup && !RegistryManager.Instance.Guest[this.ParentWindow.mVmName].IsGoogleSigninDone && appTabButton.mTabType == TabType.AppTab && appTabButton.PackageName == "com.android.vending")
				{
					QuitPopupControl quitPopupControl = new QuitPopupControl(this.ParentWindow);
					string text2 = "exit_popup_ots";
					quitPopupControl.CurrentPopupTag = text2;
					BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_YOU_ARE_ONE_STEP_AWAY", "");
					BlueStacksUIBinding.Bind(quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_TAB");
					quitPopupControl.AddQuitActionItem(QuitActionItem.WhyGoogleAccount);
					quitPopupControl.AddQuitActionItem(QuitActionItem.TroubleSigningIn);
					quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
					quitPopupControl.CloseBlueStacksButton.PreviewMouseUp += delegate(object sender, MouseButtonEventArgs e)
					{
						this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
					};
					quitPopupControl.CrossButtonPictureBox.PreviewMouseUp += delegate(object sender, MouseButtonEventArgs e)
					{
						if (string.Equals(topActivityPackageName, "com.bluestacks.appmart", StringComparison.InvariantCulture))
						{
							this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
						}
					};
					this.ParentWindow.HideDimOverlay();
					this.ParentWindow.ShowDimOverlay(quitPopupControl);
					ClientStats.SendLocalQuitPopupStatsAsync(text2, "popup_shown");
					return;
				}
				if (!FeatureManager.Instance.IsCustomUIForDMM && !dontCheckQuitPopup && appTabButton.mTabType == TabType.AppTab && tabKey != this.mLastPackageForQuitPopupDisplayed && !this.ParentWindow.SendClientActions && !receivedFromImap)
				{
					if (this.ParentWindow.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(tabKey, (AppIconModel _) => _.IsInstalledApp))
					{
						if (this.ParentWindow.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(tabKey, (AppIconModel _) => !_.IsAppSuggestionActive))
						{
							ProgressBar progressBar = new ProgressBar
							{
								ProgressText = "STRING_LOADING_MESSAGE",
								Visibility = Visibility.Hidden
							};
							this.ParentWindow.ShowDimOverlay(progressBar);
							this.mLastPackageForQuitPopupDisplayed = tabKey;
							new Thread(delegate
							{
								if (this.ParentWindow.Utils.CheckQuitPopupFromCloud(tabKey))
								{
									return;
								}
								this.Dispatcher.Invoke(new Action(delegate
								{
									this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
								}), new object[0]);
							})
							{
								IsBackground = true
							}.Start();
							return;
						}
					}
				}
				this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
			}
		}

		// Token: 0x0600037C RID: 892 RVA: 0x000187D8 File Offset: 0x000169D8
		internal void CloseTabAfterQuitPopup(string tabKey, bool sendStopAppToAndroid, bool forceClose)
		{
			if (this.mDictTabs.ContainsKey(tabKey))
			{
				AppTabButton appTabButton = this.mDictTabs[tabKey];
				if (appTabButton.mTabType != TabType.HomeTab && this.ParentWindow.mDimOverlay != null && this.ParentWindow.mDimOverlay.Control != null && ((FeatureManager.Instance.IsCustomUIForNCSoft && this.ParentWindow.mDimOverlay.Control.GetType() == this.ParentWindow.ScreenLockInstance.GetType()) || !FeatureManager.Instance.IsCustomUIForNCSoft))
				{
					this.ParentWindow.HideDimOverlay();
					this.mPopup.IsOpen = false;
				}
				this.mLastPackageForQuitPopupDisplayed = "";
				if (appTabButton.mTabType != TabType.HomeTab || forceClose)
				{
					Publisher.PublishMessage(BrowserControlTags.tabClosing, this.ParentWindow.mVmName, new JObject(new JProperty("PackageName", appTabButton.PackageName)));
					(appTabButton.Parent as Panel).Children.Remove(appTabButton);
					this.mDictTabs.Remove(tabKey);
					if (appTabButton.mTabType == TabType.AppTab || appTabButton.mTabType == TabType.HomeTab)
					{
						this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
					}
					if (sendStopAppToAndroid && appTabButton.mTabType == TabType.AppTab)
					{
						this.ParentWindow.mAppHandler.StopAppRequest(appTabButton.PackageName);
					}
					this.ListTabHistory.RemoveAll((string n) => n.Equals(tabKey, StringComparison.OrdinalIgnoreCase));
					if (this.ParentWindow.mDiscordhandler != null)
					{
						this.ParentWindow.mDiscordhandler.RemoveAppFromTimestampList(tabKey);
					}
					if (FeatureManager.Instance.IsCustomUIForDMM && this.ListTabHistory.Count == 0)
					{
						this.ParentWindow.Hide();
						this.ParentWindow.RestoreWindows(false);
						if (this.ParentWindow.mDMMRecommendedWindow != null)
						{
							this.ParentWindow.mDMMRecommendedWindow.Visibility = Visibility.Hidden;
						}
						this.ParentWindow.StaticComponents.mSelectedTabButton.IsPortraitModeTab = false;
					}
					else if (appTabButton.IsSelected)
					{
						if (this.ListTabHistory.Count != 0)
						{
							this.GoToTab(this.ListTabHistory[this.ListTabHistory.Count - 1], true, false);
						}
						else
						{
							Logger.Fatal("No tab to go back to! Ignoring");
						}
					}
					this.ResizeTabs();
				}
			}
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00018A30 File Offset: 0x00016C30
		internal bool GoToTab(string key, bool isLaunch = true, bool receivedFromImap = false)
		{
			Logger.Info("Test logs: GoToTab() key: " + key + ", isPresentInmDict: " + this.mDictTabs.ContainsKey(key).ToString());
			bool flag = false;
			if (InteropWindow.GetForegroundWindow() != this.ParentWindow.Handle)
			{
				this.ParentWindow.mIsFocusComeFromImap = true;
			}
			if (this.mDictTabs.ContainsKey(key))
			{
				if (FeatureManager.Instance.IsCustomUIForDMM && this.ParentWindow.mFrontendGrid.Visibility != Visibility.Visible)
				{
					this.ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
					this.ParentWindow.mDmmProgressControl.Visibility = Visibility.Hidden;
				}
				AppTabButton appTabButton = this.mDictTabs[key];
				if (!appTabButton.IsSelected)
				{
					appTabButton.IsLaunchOnSelection = isLaunch;
					if (KMManager.sGuidanceWindow != null && GuidanceWindow.sIsDirty)
					{
						appTabButton.mIsAnyOperationPendingForTab = true;
					}
					else
					{
						appTabButton.mIsAnyOperationPendingForTab = false;
					}
					appTabButton.Select(true, receivedFromImap);
					flag = true;
					EventHandler<TabChangeEventArgs> eventOnTabChanged = appTabButton.EventOnTabChanged;
					if (eventOnTabChanged != null)
					{
						eventOnTabChanged(null, new TabChangeEventArgs(appTabButton.AppName, appTabButton.PackageName, appTabButton.mTabType));
					}
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00004459 File Offset: 0x00002659
		internal bool GoToTab(int index)
		{
			return this.mDictTabs.Count > index && this.GoToTab(this.mPanel.Children.OfType<AppTabButton>().Last<AppTabButton>().TabKey, true, false);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000448D File Offset: 0x0000268D
		internal AppTabButton GetTab(string packageName)
		{
			if (this.mDictTabs.ContainsKey(packageName))
			{
				return this.mDictTabs[packageName];
			}
			return null;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x000044AB File Offset: 0x000026AB
		private void MoreTabButton_Click(object sender, RoutedEventArgs e)
		{
			this.mPopup.IsOpen = true;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x000044B9 File Offset: 0x000026B9
		private void NotificationPopup_Opened(object sender, EventArgs e)
		{
			this.mMoreTabButton.IsEnabled = false;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x000044C7 File Offset: 0x000026C7
		private void NotificationPopup_Closed(object sender, EventArgs e)
		{
			this.mMoreTabButton.IsEnabled = true;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x000044D5 File Offset: 0x000026D5
		private void NotificaitonPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			new Thread(delegate
			{
				Thread.Sleep(100);
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mPopup.IsOpen = false;
				}), new object[0]);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00018B50 File Offset: 0x00016D50
		internal void EnableAppTabs(bool isEnableTab)
		{
			foreach (KeyValuePair<string, AppTabButton> keyValuePair in this.mDictTabs)
			{
				if (keyValuePair.Value.mTabType == TabType.AppTab)
				{
					keyValuePair.Value.IsEnabled = isEnableTab;
				}
			}
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00018BB8 File Offset: 0x00016DB8
		internal bool IsAppRunning()
		{
			foreach (KeyValuePair<string, AppTabButton> keyValuePair in this.mDictTabs)
			{
				if (keyValuePair.Value.mTabType == TabType.AppTab)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00018C1C File Offset: 0x00016E1C
		internal void RestartTab(string package)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.CloseTab(package, true, true, true, false, "");
			}), new object[0]);
			Thread.Sleep(1000);
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(package, false);
			}), new object[0]);
		}

		// Token: 0x040001C6 RID: 454
		private MainWindow mMainWindow;

		// Token: 0x040001C7 RID: 455
		internal AppTabButton mHomeAppTabButton;

		// Token: 0x040001C8 RID: 456
		private int mTabMinWidth = 48;

		// Token: 0x040001C9 RID: 457
		private DateTime mLastTimeOfSizeChangeEventRecieved = DateTime.Now;

		// Token: 0x040001CA RID: 458
		private int mSizeChangedEventCountInLast2Seconds = 1;

		// Token: 0x040001CB RID: 459
		internal Dictionary<string, AppTabButton> mDictTabs = new Dictionary<string, AppTabButton>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x040001CE RID: 462
		internal string mLastPackageForQuitPopupDisplayed = "";

		// Token: 0x02000032 RID: 50
		private enum TabMode
		{
			// Token: 0x040001D6 RID: 470
			ParallelogramMode,
			// Token: 0x040001D7 RID: 471
			IconMode
		}
	}
}
