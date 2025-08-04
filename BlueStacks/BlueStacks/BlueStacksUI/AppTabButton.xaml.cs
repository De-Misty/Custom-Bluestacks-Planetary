using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200002F RID: 47
	public partial class AppTabButton : Button
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x0600031E RID: 798 RVA: 0x000040AC File Offset: 0x000022AC
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

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600031F RID: 799 RVA: 0x000040CD File Offset: 0x000022CD
		// (set) Token: 0x06000320 RID: 800 RVA: 0x000040D5 File Offset: 0x000022D5
		public EventHandler<TabChangeEventArgs> EventOnTabChanged { get; set; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000321 RID: 801 RVA: 0x000040DE File Offset: 0x000022DE
		// (set) Token: 0x06000322 RID: 802 RVA: 0x000040E6 File Offset: 0x000022E6
		internal bool IsSelectedSchemeStatSent { get; set; }

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000323 RID: 803 RVA: 0x000040EF File Offset: 0x000022EF
		// (set) Token: 0x06000324 RID: 804 RVA: 0x000040F7 File Offset: 0x000022F7
		internal bool IsCursorClipped { get; set; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000325 RID: 805 RVA: 0x00004100 File Offset: 0x00002300
		// (set) Token: 0x06000326 RID: 806 RVA: 0x00004108 File Offset: 0x00002308
		internal GameOnboardingControl OnboardingControl { get; set; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000327 RID: 807 RVA: 0x00004111 File Offset: 0x00002311
		// (set) Token: 0x06000328 RID: 808 RVA: 0x00004119 File Offset: 0x00002319
		public bool IsPortraitModeTab
		{
			get
			{
				return this.mIsPortraitModeTab;
			}
			set
			{
				this.mIsPortraitModeTab = value;
				if (this.IsSelected && this.ParentWindow.IsUIInPortraitMode != this.mIsPortraitModeTab)
				{
					this.ParentWindow.SwitchToPortraitMode(this.mIsPortraitModeTab);
				}
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000414E File Offset: 0x0000234E
		// (set) Token: 0x0600032A RID: 810 RVA: 0x00004156 File Offset: 0x00002356
		public bool IsMoreTabsButton
		{
			get
			{
				return this.mIsMoreTabsButton;
			}
			set
			{
				this.mIsMoreTabsButton = value;
				this.mAppTabIcon.IsEnabled = false;
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0000416B File Offset: 0x0000236B
		// (set) Token: 0x0600032C RID: 812 RVA: 0x00004173 File Offset: 0x00002373
		public bool IsButtonInDropDown { get; set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x0600032D RID: 813 RVA: 0x0000417C File Offset: 0x0000237C
		// (set) Token: 0x0600032E RID: 814 RVA: 0x00004184 File Offset: 0x00002384
		public bool IsSelected { get; private set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0000418D File Offset: 0x0000238D
		// (set) Token: 0x06000330 RID: 816 RVA: 0x00004195 File Offset: 0x00002395
		public bool IsShootingModeTooltipEnabled { get; set; } = true;

		// Token: 0x06000331 RID: 817 RVA: 0x00015738 File Offset: 0x00013938
		internal void Select(bool value, bool receivedFromImap = false)
		{
			if (this.ParentWindow.StaticComponents.mSelectedTabButton != this || !value)
			{
				if (this.ParentWindow.StaticComponents.mSelectedTabButton != null)
				{
					if (!string.Equals(KMManager.sPackageName, this.PackageName, StringComparison.InvariantCulture))
					{
						KMManager.CloseWindows();
						if (KMManager.sGuidanceWindow != null)
						{
							return;
						}
					}
					AppTabButton mSelectedTabButton = this.ParentWindow.StaticComponents.mSelectedTabButton;
					if (mSelectedTabButton.mTabType == TabType.HomeTab)
					{
						this.mIsSwitchedBackFromHomeTab = true;
					}
					else
					{
						this.mIsSwitchedBackFromHomeTab = false;
					}
					this.ParentWindow.StaticComponents.mSelectedTabButton = null;
					mSelectedTabButton.Select(false, false);
					if (mSelectedTabButton.IsCursorClipped && this.mTabType == TabType.AppTab)
					{
						this.IsCursorClipped = true;
					}
					mSelectedTabButton.IsCursorClipped = false;
					this.ParentWindow.StaticComponents.mPreviousSelectedTabWeb = mSelectedTabButton.mTabType == TabType.WebTab;
				}
				this.ParentWindow.ToggleFullScreenToastVisibility(false, "", "", "");
				this.IsSelected = value;
				if (this.IsSelected)
				{
					Publisher.PublishMessage(BrowserControlTags.tabSwitched, this.ParentWindow.mVmName, new JObject(new JProperty("PackageName", this.PackageName)));
					this.ParentWindow.mTopBar.mAppTabButtons.ListTabHistory.RemoveAll((string n) => n.Equals(this.TabKey, StringComparison.OrdinalIgnoreCase));
					this.ParentWindow.mTopBar.mAppTabButtons.ListTabHistory.Add(this.TabKey);
					this.ParentWindow.StaticComponents.mSelectedTabButton = this;
					this.ParentWindow.Utils.ResetPendingUIOperations();
					if (this.mTabType == TabType.WebTab)
					{
						if (this.ParentWindow.mIsFullScreen)
						{
							this.ParentWindow.RestoreWindows(false);
						}
						BrowserControl browserControl = this.GetBrowserControl();
						if (browserControl == null)
						{
							this.mControlGrid = this.ParentWindow.AddBrowser(this.PackageName);
							this.Init(this.AppName, this.PackageName, this.mAppTabIcon.ImageName, this.mControlGrid, this.TabKey);
						}
						else
						{
							try
							{
								object[] array = new object[0];
								if (browserControl.CefBrowser != null)
								{
									browserControl.CefBrowser.CallJs("webtabselected", array);
								}
							}
							catch (Exception ex)
							{
								Logger.Warning("Ignoring webtabselected exception. " + ex.Message);
							}
						}
						this.ParentWindow.ChangeWindowOrientaion(this, new ChangeOrientationEventArgs(this.ParentWindow.mAspectRatio < 1L));
					}
					this.ParentWindow.ShowControlGrid(this.mControlGrid);
					if (this.mTabType == TabType.AppTab || this.mTabType == TabType.HomeTab)
					{
						if (this.ParentWindow.AppForcedOrientationDict.ContainsKey(this.PackageName))
						{
							this.ParentWindow.ChangeOrientationFromClient(this.ParentWindow.AppForcedOrientationDict[this.PackageName], true);
						}
						else
						{
							this.ParentWindow.ChangeOrientationFromClient(false, false);
						}
					}
					if (this.mTabType == TabType.HomeTab)
					{
						this.ParentWindow.mWelcomeTab.mHomeAppManager.HomeTabSwitchActions(true);
						if (this.ParentWindow.mIsFullScreen)
						{
							this.ParentWindow.RestoreWindows(false);
						}
					}
					else if (!FeatureManager.Instance.IsCustomUIForDMM && this.mTabType == TabType.AppTab && !this.ParentWindow.mSidebar.mIsOverlayTooltipClosed && !this.mIsOverlayTooltipDisplayed && KMManager.KeyMappingFilesAvailable(this.PackageName))
					{
						this.mIsOverlayTooltipDisplayed = true;
						this.ParentWindow.mSidebar.ShowOverlayTooltip(true, false);
					}
					if (this.mTabType != TabType.HomeTab)
					{
						this.ParentWindow.mWelcomeTab.mHomeAppManager.HomeTabSwitchActions(false);
					}
					AppUsageTimer.StartTimer(this.ParentWindow.mVmName, this.TabKey);
					if (this.IsLaunchOnSelection)
					{
						this.LaunchApp();
					}
					else
					{
						this.IsLaunchOnSelection = true;
					}
					BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
					BlueStacksUIBinding.BindColor(this.mTabLabel, Control.ForegroundProperty, "SelectedTabForegroundColor");
					BlueStacksUIBinding.BindColor(this.mBorder, Border.BorderBrushProperty, "SelectedTabBorderColor");
					if (!FeatureManager.Instance.IsCustomUIForDMM)
					{
						if (this.mTabType == TabType.AppTab)
						{
							this.ParentWindow.mTopBar.mAppTabButtons.KillWebTabs();
							AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(this.PackageName);
							if (((appIcon != null) ? new bool?(appIcon.IsGamepadCompatible) : null).GetValueOrDefault())
							{
								this.ParentWindow.mCommonHandler.OnGamepadButtonVisibilityChanged(true);
							}
							else
							{
								this.ParentWindow.mCommonHandler.OnGamepadButtonVisibilityChanged(false);
							}
							KMManager.LoadIMActions(this.ParentWindow, this.PackageName);
							this.ParentWindow.mCallbackEnabled = "False";
							Logger.Info("Callback: Select(): " + this.ParentWindow.mCallbackEnabled);
							KMManager.mOnboardingCounter = 1;
							if (!this.IsSelectedSchemeStatSent)
							{
								ClientStats.SendMiscellaneousStatsAsync("SelectedSchemeName", RegistryManager.Instance.UserGuid, this.PackageName, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, null, null, null, null, null);
								this.IsSelectedSchemeStatSent = true;
							}
							if (this.mTabType == TabType.AppTab && !this.mIsKeyMappingTipDisplayed && !this.ParentWindow.SendClientActions && !receivedFromImap && KMManager.KeyMappingFilesAvailable(this.PackageName))
							{
								this.mIsKeyMappingTipDisplayed = true;
								if (this.ParentWindow.SelectedConfig != null && this.ParentWindow.SelectedConfig.SelectedControlScheme != null && this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls != null)
								{
									if (this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any((IMAction action) => action.Guidance.Any<KeyValuePair<string, string>>()))
									{
										this.ParentWindow.mSidebar.ShowKeyMapPopup(true);
									}
								}
							}
							else if (this.mGuidanceWindowOpen)
							{
								KMManager.HandleInputMapperWindow(this.ParentWindow, "");
							}
							if (RegistryManager.Instance.ShowKeyControlsOverlay && !KMManager.CheckIfKeymappingWindowVisible(false))
							{
								KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
							}
							if (this.mIsSwitchedBackFromHomeTab && KMManager.KeyMappingFilesAvailable(this.PackageName))
							{
								this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleLoadConfigOnTabSwitch", new Dictionary<string, string> { { "package", this.PackageName } });
							}
							this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
							this.ParentWindow.mCommonHandler.SetCustomCursorForApp(this.PackageName);
							this.mIsNativeGamepadEnabledForApp = this.ParentWindow.EngineInstanceRegistry.NativeGamepadState != NativeGamepadState.ForceOff;
							if (this.ParentWindow.EngineInstanceRegistry.NativeGamepadState == NativeGamepadState.Auto)
							{
								bool flag = this.ParentWindow.mCommonHandler.CheckNativeGamepadState(this.PackageName);
								this.mIsNativeGamepadEnabledForApp = flag;
								this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", new Dictionary<string, string> { 
								{
									"isEnabled",
									flag.ToString(CultureInfo.InvariantCulture)
								} });
							}
						}
						else
						{
							KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
							if (this.ParentWindow.mCommonHandler != null)
							{
								this.ParentWindow.mCommonHandler.ClipMouseCursorHandler(true, true, "", "");
							}
						}
						if (this.mTabType == TabType.HomeTab)
						{
							this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
						}
						else
						{
							this.ParentWindow.mWelcomeTab.mHomeAppManager.CloseAppSuggestionPopup();
						}
						List<GenericNotificationItem> list = new List<GenericNotificationItem>();
						foreach (GenericNotificationItem genericNotificationItem in PromotionManager.sPassedDeferredNotificationsList.Where((GenericNotificationItem _) => string.Compare(_.DeferredApp, this.PackageName, StringComparison.OrdinalIgnoreCase) == 0))
						{
							BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].HandleGenericNotificationPopup(genericNotificationItem);
							GenericNotificationManager.AddNewNotification(genericNotificationItem, false);
							BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mTopBar.RefreshNotificationCentreButton();
							list.Add(genericNotificationItem);
						}
						foreach (GenericNotificationItem genericNotificationItem2 in list)
						{
							PromotionManager.sPassedDeferredNotificationsList.Remove(genericNotificationItem2);
						}
						if (this.ParentWindow.SendClientActions && !receivedFromImap)
						{
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							Dictionary<string, string> dictionary2 = new Dictionary<string, string>
							{
								{ "EventAction", "TabSelected" },
								{ "tabKey", this.TabKey }
							};
							JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
							serializerSettings.Formatting = Formatting.None;
							dictionary.Add("operationData", JsonConvert.SerializeObject(dictionary2, serializerSettings));
							this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
						}
					}
					if (this.mTabType == TabType.AppTab && KMManager.KeyMappingFilesAvailable(this.PackageName) && this.ParentWindow.SelectedConfig.ControlSchemes != null && this.ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
					{
						this.ParentWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(true);
					}
					else
					{
						this.ParentWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(false);
					}
					EventHandler<EventArgs> eventOnTabChanged = this.ParentWindow.mTopBar.mAppTabButtons.EventOnTabChanged;
					if (eventOnTabChanged != null)
					{
						eventOnTabChanged(null, null);
					}
					if (this.mRestartPubgTab)
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							CustomMessageWindow customMessageWindow = new CustomMessageWindow();
							string text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RESTART_OBJECT", ""), new object[] { "PUBG Mobile" });
							BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, text, "");
							string text2 = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CHANGED_RESTART_APP_MESSAGE", ""), new object[] { "PUBG Mobile" });
							BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, text2, "");
							customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_BLUESTACKS", new EventHandler(this.RestartConfirmationAcceptedHandler), null, false, null);
							customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
							this.ParentWindow.ShowDimOverlay(null);
							customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
							customMessageWindow.ShowDialog();
							this.ParentWindow.HideDimOverlay();
						}), new object[0]);
						this.mRestartPubgTab = false;
					}
					if (this.mRestartCODTab)
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							CustomMessageWindow customMessageWindow2 = new CustomMessageWindow();
							string text3 = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RESTART_OBJECT", ""), new object[] { "Call of Duty: Mobile" });
							BlueStacksUIBinding.Bind(customMessageWindow2.TitleTextBlock, text3, "");
							string text4 = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CHANGED_RESTART_APP_MESSAGE", ""), new object[] { "Call of Duty: Mobile" });
							BlueStacksUIBinding.Bind(customMessageWindow2.BodyTextBlock, text4, "");
							customMessageWindow2.AddButton(ButtonColors.Blue, "STRING_RESTART_BLUESTACKS", new EventHandler(this.RestartConfirmationAcceptedHandler), null, false, null);
							customMessageWindow2.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
							this.ParentWindow.ShowDimOverlay(null);
							customMessageWindow2.Owner = this.ParentWindow.mDimOverlay;
							customMessageWindow2.ShowDialog();
							this.ParentWindow.HideDimOverlay();
						}), new object[0]);
						this.mRestartCODTab = false;
					}
					if (this.mTabType == TabType.AppTab && File.Exists(Utils.GetInputmapperDefaultFilePath(this.PackageName)) && Oem.Instance.IsShowGameBlurb)
					{
						PostBootCloudInfo mPostBootCloudInfo = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
						bool? flag2;
						if (mPostBootCloudInfo == null)
						{
							flag2 = null;
						}
						else
						{
							AppPackageListObject onBoardingAppPackages = mPostBootCloudInfo.OnBoardingInfo.OnBoardingAppPackages;
							flag2 = ((onBoardingAppPackages != null) ? new bool?(onBoardingAppPackages.IsPackageAvailable(this.PackageName)) : null);
						}
						bool? flag3 = flag2;
						if (flag3.GetValueOrDefault())
						{
							if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.PackageName, (AppSettings appSetting) => appSetting.IsAppOnboardingCompleted))
							{
								this.OnboardingControl = new GameOnboardingControl(this.ParentWindow, this.PackageName, "applaunch");
								GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
								if (sGuidanceWindow != null)
								{
									sGuidanceWindow.DimOverLayVisibility(Visibility.Visible);
								}
								this.ParentWindow.ShowDimOverlay(this.OnboardingControl);
								goto IL_0AB1;
							}
						}
						this.ParentWindow.ShowDimOverlay(null);
						if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (AppSettings appSettings) => appSettings.IsGeneralAppOnBoardingCompleted))
						{
							this.ShowDefaultBlurbOnboarding();
						}
						this.ParentWindow.HideDimOverlay();
					}
					IL_0AB1:
					if (this.ParentWindow.mTopBar.mAppTabButtons.mMoreTabButton.Visibility == Visibility.Visible)
					{
						this.MoreTabsButtonHandling();
						return;
					}
				}
				else
				{
					BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
					BlueStacksUIBinding.BindColor(this.mTabLabel, Control.ForegroundProperty, "TabForegroundColor");
					BlueStacksUIBinding.BindColor(this.mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
				}
			}
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000419E File Offset: 0x0000239E
		private void RestartConfirmationAcceptedHandler(object sender, EventArgs e)
		{
			Logger.Info("Restarting Pubg/COD Tab.");
			new Thread(delegate
			{
				this.ParentWindow.mTopBar.mAppTabButtons.RestartTab(this.PackageName);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000333 RID: 819 RVA: 0x000041C7 File Offset: 0x000023C7
		// (set) Token: 0x06000334 RID: 820 RVA: 0x000041CF File Offset: 0x000023CF
		public string PackageName { get; set; } = string.Empty;

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000335 RID: 821 RVA: 0x000041D8 File Offset: 0x000023D8
		// (set) Token: 0x06000336 RID: 822 RVA: 0x000041E0 File Offset: 0x000023E0
		public string TabKey { get; set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000337 RID: 823 RVA: 0x000041E9 File Offset: 0x000023E9
		// (set) Token: 0x06000338 RID: 824 RVA: 0x000041F1 File Offset: 0x000023F1
		public string AppName { get; set; } = string.Empty;

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000339 RID: 825 RVA: 0x000041FA File Offset: 0x000023FA
		// (set) Token: 0x0600033A RID: 826 RVA: 0x00004202 File Offset: 0x00002402
		public bool IsDMMKeymapEnabled
		{
			get
			{
				return this.mIsDMMKeyMapEnabled;
			}
			set
			{
				this.mIsDMMKeyMapEnabled = value;
				this.IsDMMKeymapUIVisible = value;
				this.ParentWindow.mCommonHandler.SetDMMKeymapButtonsAndTransparency();
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600033B RID: 827 RVA: 0x00004222 File Offset: 0x00002422
		// (set) Token: 0x0600033C RID: 828 RVA: 0x0000422A File Offset: 0x0000242A
		public bool IsDMMKeymapUIVisible { get; set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x0600033D RID: 829 RVA: 0x00004233 File Offset: 0x00002433
		public string AppLabel
		{
			get
			{
				return this.mTabLabel.Content.ToString();
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x0600033E RID: 830 RVA: 0x00004245 File Offset: 0x00002445
		// (set) Token: 0x0600033F RID: 831 RVA: 0x0000424D File Offset: 0x0000244D
		public bool IsLaunchOnSelection { get; set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000340 RID: 832 RVA: 0x00004256 File Offset: 0x00002456
		// (set) Token: 0x06000341 RID: 833 RVA: 0x0000425E File Offset: 0x0000245E
		public Grid mControlGrid { get; set; }

		// Token: 0x06000342 RID: 834 RVA: 0x00004267 File Offset: 0x00002467
		public AppTabButton()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000343 RID: 835 RVA: 0x00016280 File Offset: 0x00014480
		internal void Init(string appName, string packageName, string activityName, string imageName, Grid controlGrid, string tabKey)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(tabKey))
			{
				flag = true;
			}
			this.Init(appName, packageName, imageName, controlGrid, flag ? tabKey : packageName);
			this.mActivityName = activityName;
			this.mTabType = TabType.AppTab;
			if (string.Equals(packageName, "Home", StringComparison.InvariantCulture) || string.Equals(packageName, "Setup", StringComparison.InvariantCulture))
			{
				this.mTabType = TabType.HomeTab;
				BlueStacksUIBinding.BindCornerRadius(this, FrameworkElement.MarginProperty, "TabMarginLandScape");
			}
		}

		// Token: 0x06000344 RID: 836 RVA: 0x000162F0 File Offset: 0x000144F0
		internal void Init(string title, string url, string imageName, Grid controlGrid, string tabKey)
		{
			BlueStacksUIBinding.Bind(this, title, FrameworkElement.ToolTipProperty);
			BlueStacksUIBinding.Bind(this.mTabLabel, title);
			this.AppName = title;
			this.PackageName = url;
			this.TabKey = tabKey;
			this.mTabType = TabType.WebTab;
			this.mControlGrid = controlGrid;
			if (!this.IsSelected)
			{
				this.mControlGrid.Visibility = Visibility.Hidden;
			}
			if (string.IsNullOrEmpty(imageName))
			{
				this.mImageColumn.Width = new GridLength(0.0);
				return;
			}
			this.mAppTabIcon.ImageName = imageName;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0001637C File Offset: 0x0001457C
		internal void ResizeButton(double tabWidth)
		{
			if (this.ParentWindow.IsUIInPortraitMode)
			{
				this.MakeTabParallelogram(false);
			}
			else
			{
				this.MakeTabParallelogram(true);
			}
			if (tabWidth != base.ActualWidth)
			{
				DoubleAnimation doubleAnimation = new DoubleAnimation
				{
					From = new double?(base.ActualWidth),
					To = new double?(tabWidth),
					Duration = new Duration(TimeSpan.FromMilliseconds(200.0))
				};
				base.BeginAnimation(FrameworkElement.WidthProperty, doubleAnimation);
			}
		}

		// Token: 0x06000346 RID: 838 RVA: 0x000163F8 File Offset: 0x000145F8
		internal void MakeTabParallelogram(bool isSkewTab)
		{
			if (isSkewTab)
			{
				BlueStacksUIBinding.BindCornerRadius(this, FrameworkElement.MarginProperty, "TabMarginLandScape");
			}
			else
			{
				BlueStacksUIBinding.BindCornerRadius(this, FrameworkElement.MarginProperty, "TabMarginPortrait");
			}
			if (isSkewTab != this.mIsTabsSkewed)
			{
				if (isSkewTab)
				{
					this.mIsTabsSkewed = true;
					this.CloseTabButtonPortrait.Visibility = Visibility.Hidden;
					this.ParallelogramGrid.RenderTransform = new SkewTransform(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY);
					DoubleAnimation doubleAnimation = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, TimeSpan.FromMilliseconds(200.0));
					DoubleAnimation doubleAnimation2 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY, TimeSpan.FromMilliseconds(200.0));
					doubleAnimation2.Completed += this.SkewY_Completed;
					this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleXProperty, doubleAnimation);
					this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleYProperty, doubleAnimation2);
					BlueStacksUIBinding.BindCornerRadius(this, FrameworkElement.MarginProperty, "TabMarginLandScape");
				}
				else
				{
					this.mIsTabsSkewed = false;
					this.CloseTabButtonLandScape.Visibility = Visibility.Hidden;
					this.ParallelogramGrid.RenderTransform = new SkewTransform(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY);
					DoubleAnimation doubleAnimation3 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX, TimeSpan.FromMilliseconds(200.0));
					DoubleAnimation doubleAnimation4 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY, TimeSpan.FromMilliseconds(200.0));
					this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleXProperty, doubleAnimation3);
					this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleYProperty, doubleAnimation4);
					doubleAnimation4.Completed += this.SkewY_Completed;
					BlueStacksUIBinding.BindCornerRadius(this, FrameworkElement.MarginProperty, "TabMarginPortrait");
				}
				if (this.mIsMoreTabsButton)
				{
					this.mBorder.Visibility = Visibility.Visible;
					BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
				}
			}
		}

		// Token: 0x06000347 RID: 839 RVA: 0x000042A4 File Offset: 0x000024A4
		private void SkewY_Completed(object sender, EventArgs e)
		{
			if (this.mIsTabsSkewed)
			{
				BlueStacksUIBinding.BindTransform(this.ParallelogramGrid, UIElement.RenderTransformProperty, "TabTransform");
				return;
			}
			BlueStacksUIBinding.BindTransform(this.ParallelogramGrid, UIElement.RenderTransformProperty, "TabTransformPortrait");
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00016664 File Offset: 0x00014864
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (!this.IsMoreTabsButton)
			{
				bool flag = sender.GetHashCode() == this.ParentWindow.StaticComponents.mSelectedTabButton.GetHashCode();
				bool flag2 = this.CloseTabButtonLandScape.IsMouseOver || this.CloseTabButtonPortrait.IsMouseOver || this.CloseTabButtonDropDown.IsMouseOver;
				if (flag)
				{
					if (flag2)
					{
						if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed)
						{
							this.HandlePendingOperationsForTab("guidance");
						}
						if (KMManager.sGuidanceWindow == null)
						{
							this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.TabKey, true, false, false, false, "");
							return;
						}
					}
				}
				else
				{
					if (flag2)
					{
						this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.TabKey, true, false, false, false, "");
						return;
					}
					if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed)
					{
						this.HandlePendingOperationsForTab("guidance");
					}
					if (KMManager.sGuidanceWindow == null)
					{
						this.Select(true, false);
						this.Button_PreviewMouseUp(null, null);
						EventHandler<TabChangeEventArgs> eventOnTabChanged = this.EventOnTabChanged;
						if (eventOnTabChanged == null)
						{
							return;
						}
						eventOnTabChanged(this, new TabChangeEventArgs(this.AppName, this.PackageName, this.mTabType));
					}
				}
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x000042D9 File Offset: 0x000024D9
		private void HandlePendingOperationsForTab(string pendingOperation)
		{
			if (pendingOperation != null && pendingOperation == "guidance")
			{
				KMManager.CloseWindows();
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0001679C File Offset: 0x0001499C
		internal void UpdateUIForDropDown(bool isInDropDown)
		{
			if (isInDropDown)
			{
				this.IsButtonInDropDown = true;
				this.MakeTabParallelogram(false);
				base.MinWidth = 150.0;
				this.mTabLabel.Margin = new Thickness(3.0, 1.0, 3.0, 1.0);
				if (!this.IsSelected)
				{
					this.mBorder.Background = Brushes.Transparent;
				}
				this.mBorder.BorderThickness = new Thickness(0.0);
				return;
			}
			this.IsButtonInDropDown = false;
			this.mBorder.BorderThickness = new Thickness(1.0);
			base.MinWidth = 0.0;
			this.mTabLabel.Margin = new Thickness(3.0, 1.0, 24.0, 1.0);
			if (this.IsSelected)
			{
				BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
				BlueStacksUIBinding.BindColor(this.mBorder, Border.BorderBrushProperty, "SelectedTabBorderColor");
				return;
			}
			BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
			BlueStacksUIBinding.BindColor(this.mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
		}

		// Token: 0x0600034B RID: 843 RVA: 0x000168F4 File Offset: 0x00014AF4
		internal void LaunchApp()
		{
			if (!string.IsNullOrEmpty(this.PackageName) && this.mTabType == TabType.AppTab)
			{
				this.ParentWindow.mAppHandler.SendRunAppRequestAsync(this.PackageName, this.mActivityName, false);
				return;
			}
			if ((this.mTabType == TabType.HomeTab || this.mTabType == TabType.WebTab) && RegistryManager.Instance.SwitchToAndroidHome)
			{
				this.ParentWindow.mAppHandler.GoHome();
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00016964 File Offset: 0x00014B64
		private void Button_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!this.IsSelected)
			{
				BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
			}
			if (this.IsButtonInDropDown)
			{
				if (this.mTabType != TabType.HomeTab)
				{
					this.CloseTabButtonDropDown.Visibility = Visibility.Visible;
				}
			}
			else if (this.mTabType != TabType.HomeTab && !this.mIsMoreTabsButton)
			{
				this.CloseTabButtonLandScape.Visibility = Visibility.Visible;
				if (!this.mIsTabsSkewed)
				{
					this.CloseTabButtonPortrait.Visibility = Visibility.Visible;
				}
			}
			if (this.IsMoreTabsButton)
			{
				this.mAppTabIcon.SetHoverImage();
				BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
			}
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00016A08 File Offset: 0x00014C08
		private void Button_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.IsSelected)
			{
				BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
			}
			if (this.IsMoreTabsButton)
			{
				this.mAppTabIcon.SetDefaultImage();
				BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
			}
			this.CloseTabButtonLandScape.Visibility = Visibility.Hidden;
			this.CloseTabButtonPortrait.Visibility = Visibility.Hidden;
			this.CloseTabButtonDropDown.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00016A80 File Offset: 0x00014C80
		private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.IsButtonInDropDown)
			{
				if (this.IsMoreTabsButton)
				{
					this.mAppTabIcon.SetClickedImage();
					BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
					return;
				}
				if (!this.IsSelected)
				{
					BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
					BlueStacksUIBinding.BindColor(this.mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
				}
			}
		}

		// Token: 0x0600034F RID: 847 RVA: 0x00016AF0 File Offset: 0x00014CF0
		private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!this.IsButtonInDropDown)
			{
				if (!this.IsSelected)
				{
					BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
				}
				if (this.mIsMoreTabsButton)
				{
					if (base.IsMouseOver)
					{
						this.mAppTabIcon.SetHoverImage();
						BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
						return;
					}
					this.mAppTabIcon.SetDefaultImage();
					BlueStacksUIBinding.BindColor(this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
				}
			}
		}

		// Token: 0x06000350 RID: 848 RVA: 0x000042F0 File Offset: 0x000024F0
		private void Button_IsEnabledChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (base.IsEnabled)
			{
				base.Opacity = 1.0;
				return;
			}
			base.Opacity = 0.3;
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00016B74 File Offset: 0x00014D74
		internal BrowserControl GetBrowserControl()
		{
			BrowserControl browserControl;
			try
			{
				browserControl = this.mControlGrid.Children[0] as BrowserControl;
			}
			catch (Exception ex)
			{
				Logger.Warning("No BrowserControl associated with tabkey: " + this.TabKey + " Error: " + ex.ToString());
				browserControl = null;
			}
			return browserControl;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00004319 File Offset: 0x00002519
		internal void EnableKeymapForDMM(bool enable)
		{
			this.mIsDMMKeyMapEnabled = enable;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00016BD0 File Offset: 0x00014DD0
		internal void MoreTabsButtonHandling()
		{
			AppTabButton mMoreTabButton = this.ParentWindow.mTopBar.mAppTabButtons.mMoreTabButton;
			mMoreTabButton.mTabLabel.Visibility = Visibility.Collapsed;
			mMoreTabButton.mDownArrowGrid.Visibility = Visibility.Visible;
			if (this.ParentWindow.mTopBar.mAppTabButtons.mHiddenButtons.Children.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton))
			{
				mMoreTabButton.mAppTabIcon.ImageName = this.ParentWindow.StaticComponents.mSelectedTabButton.mAppTabIcon.ImageName;
				return;
			}
			mMoreTabButton.mAppTabIcon.ImageName = (this.ParentWindow.mTopBar.mAppTabButtons.mHiddenButtons.Children[0] as AppTabButton).mAppTabIcon.ImageName;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00016C9C File Offset: 0x00014E9C
		internal void ShowBlurbOnboarding(JObject res)
		{
			if (res["is_show_blurbs"].ToObject<bool>())
			{
				if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.PackageName, (AppSettings appSettings) => appSettings.IsGeneralAppOnBoardingCompleted))
				{
					GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
					if (sGuidanceWindow != null)
					{
						sGuidanceWindow.DimOverLayVisibility(Visibility.Collapsed);
					}
					if (res.ContainsKey("blurbs"))
					{
						JArray jarray = JArray.Parse(res["blurbs"].ToString());
						for (int i = 0; i < jarray.Count; i++)
						{
							JObject jobject = JObject.Parse(jarray[i].ToString());
							if (Enum.IsDefined(typeof(OnboardingBlurbTags), jobject["tag"].ToString()))
							{
								switch ((OnboardingBlurbTags)Enum.Parse(typeof(OnboardingBlurbTags), jobject["tag"].ToString()))
								{
								case OnboardingBlurbTags.schemeselect:
								{
									IMConfig selectedConfig = this.ParentWindow.SelectedConfig;
									bool flag;
									if (selectedConfig == null)
									{
										flag = false;
									}
									else
									{
										Dictionary<string, IMControlScheme> controlSchemesDict = selectedConfig.ControlSchemesDict;
										int? num = ((controlSchemesDict != null) ? new int?(controlSchemesDict.Count) : null);
										int num2 = 1;
										flag = (num.GetValueOrDefault() > num2) & (num != null);
									}
									if (flag)
									{
										GuidanceWindow sGuidanceWindow2 = KMManager.sGuidanceWindow;
										OnBoardingPopupWindow onBoardingPopupWindow = ((sGuidanceWindow2 != null) ? sGuidanceWindow2.GuidanceSchemeOnboardingBlurb() : null);
										if (onBoardingPopupWindow != null)
										{
											KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow);
										}
									}
									break;
								}
								case OnboardingBlurbTags.guidancedescription:
								{
									GuidanceWindow sGuidanceWindow3 = KMManager.sGuidanceWindow;
									OnBoardingPopupWindow onBoardingPopupWindow2 = ((sGuidanceWindow3 != null) ? sGuidanceWindow3.GuidanceOnboardingBlurb() : null);
									if (onBoardingPopupWindow2 != null)
									{
										KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow2);
									}
									break;
								}
								case OnboardingBlurbTags.fullscreen:
								{
									Sidebar mSidebar = this.ParentWindow.mSidebar;
									OnBoardingPopupWindow onBoardingPopupWindow3 = ((mSidebar != null) ? mSidebar.FullscreenOnboardingBlurb() : null);
									if (onBoardingPopupWindow3 != null)
									{
										KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow3);
									}
									break;
								}
								case OnboardingBlurbTags.guidancevideo:
								{
									GuidanceWindow sGuidanceWindow4 = KMManager.sGuidanceWindow;
									OnBoardingPopupWindow onBoardingPopupWindow4 = ((sGuidanceWindow4 != null) ? sGuidanceWindow4.GuidanceVideoOnboardingBlurb() : null);
									if (onBoardingPopupWindow4 != null)
									{
										KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow4);
									}
									break;
								}
								case OnboardingBlurbTags.guidanceclose:
									if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
									{
										AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsCloseGuidanceOnboardingCompleted = false;
									}
									break;
								}
							}
						}
						this.StartBlurbOnboarding();
						return;
					}
					IMConfig selectedConfig2 = this.ParentWindow.SelectedConfig;
					bool flag2;
					if (selectedConfig2 == null)
					{
						flag2 = false;
					}
					else
					{
						Dictionary<string, IMControlScheme> controlSchemesDict2 = selectedConfig2.ControlSchemesDict;
						int? num = ((controlSchemesDict2 != null) ? new int?(controlSchemesDict2.Count) : null);
						int num2 = 1;
						flag2 = (num.GetValueOrDefault() > num2) & (num != null);
					}
					if (flag2)
					{
						GuidanceWindow sGuidanceWindow5 = KMManager.sGuidanceWindow;
						OnBoardingPopupWindow onBoardingPopupWindow5 = ((sGuidanceWindow5 != null) ? sGuidanceWindow5.GuidanceSchemeOnboardingBlurb() : null);
						if (onBoardingPopupWindow5 != null)
						{
							KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow5);
						}
					}
					this.ShowDefaultBlurbOnboarding();
				}
			}
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00016F80 File Offset: 0x00015180
		internal void ShowDefaultBlurbOnboarding()
		{
			GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
			OnBoardingPopupWindow onBoardingPopupWindow = ((sGuidanceWindow != null) ? sGuidanceWindow.GuidanceVideoOnboardingBlurb() : null);
			if (onBoardingPopupWindow != null)
			{
				KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow);
			}
			GuidanceWindow sGuidanceWindow2 = KMManager.sGuidanceWindow;
			OnBoardingPopupWindow onBoardingPopupWindow2 = ((sGuidanceWindow2 != null) ? sGuidanceWindow2.GuidanceOnboardingBlurb() : null);
			if (onBoardingPopupWindow2 != null)
			{
				KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow2);
			}
			Sidebar mSidebar = this.ParentWindow.mSidebar;
			OnBoardingPopupWindow onBoardingPopupWindow3 = ((mSidebar != null) ? mSidebar.FullscreenOnboardingBlurb() : null);
			if (onBoardingPopupWindow3 != null)
			{
				KMManager.onBoardingPopupWindows.Add(onBoardingPopupWindow3);
			}
			if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
			{
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsCloseGuidanceOnboardingCompleted = false;
			}
			this.StartBlurbOnboarding();
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0001704C File Offset: 0x0001524C
		internal void StartBlurbOnboarding()
		{
			foreach (OnBoardingPopupWindow onBoardingPopupWindow in KMManager.onBoardingPopupWindows.ToList<OnBoardingPopupWindow>())
			{
				KMManager.onBoardingPopupWindows.Remove(onBoardingPopupWindow);
				if (!onBoardingPopupWindow.IsBlurbRelatedToGuidance)
				{
					this.ParentWindow.HideDimOverlay();
				}
				if (KMManager.onBoardingPopupWindows.Count == 0)
				{
					onBoardingPopupWindow.IsLastPopup = true;
				}
				onBoardingPopupWindow.ShowDialog();
			}
			if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
			{
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsGeneralAppOnBoardingCompleted = true;
			}
		}

		// Token: 0x04000195 RID: 405
		private MainWindow mMainWindow;

		// Token: 0x04000196 RID: 406
		internal const int IconModeMinWidth = 38;

		// Token: 0x04000197 RID: 407
		internal const int ParallelogramModeMinWidth = 48;

		// Token: 0x04000198 RID: 408
		private bool mIsPortraitModeTab;

		// Token: 0x04000199 RID: 409
		internal TabType mTabType;

		// Token: 0x0400019B RID: 411
		internal bool mRestartPubgTab;

		// Token: 0x0400019C RID: 412
		internal bool mRestartCODTab;

		// Token: 0x0400019D RID: 413
		internal bool mIsKeyMappingTipDisplayed;

		// Token: 0x0400019E RID: 414
		internal bool mIsOverlayTooltipDisplayed;

		// Token: 0x0400019F RID: 415
		internal bool mIsShootingModeToastDisplayed;

		// Token: 0x040001A0 RID: 416
		internal bool mShootingModeToastIsOpen;

		// Token: 0x040001A1 RID: 417
		internal bool mGuidanceWindowOpen;

		// Token: 0x040001A2 RID: 418
		internal bool mShootingModeToastWhenGuidanceOpen;

		// Token: 0x040001A3 RID: 419
		internal bool mIsAnyOperationPendingForTab;

		// Token: 0x040001A4 RID: 420
		internal bool mIsSwitchedBackFromHomeTab;

		// Token: 0x040001A5 RID: 421
		internal bool mIsNativeGamepadEnabledForApp;

		// Token: 0x040001A9 RID: 425
		private bool mIsMoreTabsButton;

		// Token: 0x040001B0 RID: 432
		private bool mIsDMMKeyMapEnabled;

		// Token: 0x040001B3 RID: 435
		private string mActivityName = string.Empty;

		// Token: 0x040001B4 RID: 436
		private bool mIsTabsSkewed = true;
	}
}
