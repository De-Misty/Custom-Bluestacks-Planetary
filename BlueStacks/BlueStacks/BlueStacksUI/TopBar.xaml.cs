using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200028D RID: 653
	public partial class TopBar : UserControl, ITopBar
	{
		// Token: 0x17000365 RID: 869
		// (get) Token: 0x060017A4 RID: 6052 RVA: 0x0000FE8E File Offset: 0x0000E08E
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

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x060017A5 RID: 6053 RVA: 0x0008C97C File Offset: 0x0008AB7C
		// (remove) Token: 0x060017A6 RID: 6054 RVA: 0x0008C9B4 File Offset: 0x0008ABB4
		public event PercentageChangedEventHandler PercentChanged;

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060017A7 RID: 6055 RVA: 0x0000BAF0 File Offset: 0x00009CF0
		// (set) Token: 0x060017A8 RID: 6056 RVA: 0x00004786 File Offset: 0x00002986
		string ITopBar.AppName
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x060017A9 RID: 6057 RVA: 0x0000BAF0 File Offset: 0x00009CF0
		// (set) Token: 0x060017AA RID: 6058 RVA: 0x00004786 File Offset: 0x00002986
		string ITopBar.CharacterName
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x0008C9EC File Offset: 0x0008ABEC
		public static Point GetMousePosition()
		{
			NativeMethods.Win32Point win32Point = default(NativeMethods.Win32Point);
			NativeMethods.GetCursorPos(ref win32Point);
			return new Point((double)win32Point.X, (double)win32Point.Y);
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x0008CA1C File Offset: 0x0008AC1C
		public TopBar()
		{
			this.mOptionsPriorityPanel = new SortedList<int, KeyValuePair<FrameworkElement, double>>();
			this.mMinimumExpectedTopBarWidth = 320.0;
			this.MB_MULTIPLIER = 1048576UL;
			this.InitializeComponent();
			if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
			{
				this.TopBarOptionsPanelElementVisibility(this.mUserAccountBtn, false);
				this.TopBarOptionsPanelElementVisibility(this.mWarningButton, false);
			}
			else
			{
				if (!FeatureManager.Instance.IsUserAccountBtnEnabled)
				{
					this.TopBarOptionsPanelElementVisibility(this.mUserAccountBtn, false);
				}
				if (!FeatureManager.Instance.IsWarningBtnEnabled)
				{
					this.TopBarOptionsPanelElementVisibility(this.mWarningButton, false);
				}
				this.TopBarOptionsPanelElementVisibility(this.mHelpButton, FeatureManager.Instance.IsTopbarHelpEnabled);
			}
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.mConfigButton.Visibility = Visibility.Collapsed;
				this.TopBarOptionsPanelElementVisibility(this.mNotificationGrid, false);
				this.WindowHeaderGrid.Visibility = Visibility.Collapsed;
				this.TopBarOptionsPanelElementVisibility(this.mUserAccountBtn, false);
				this.mWarningButton.ToolTip = null;
				this.mSidebarButton.Visibility = Visibility.Collapsed;
			}
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				this.TopBarOptionsPanelElementVisibility(this.mUserAccountBtn, false);
				this.TopBarOptionsPanelElementVisibility(this.mNotificationGrid, false);
			}
			if (!string.Equals(this.mTitleIcon.ImageName, Strings.TitleBarIconImageName, StringComparison.InvariantCulture))
			{
				this.mTitleIcon.ImageName = Strings.TitleBarIconImageName;
			}
			double? num = Strings.TitleBarProductIconWidth;
			double? num2;
			if (num2 != null)
			{
				FrameworkElement frameworkElement = this.mTitleIcon;
				num = Strings.TitleBarProductIconWidth;
				frameworkElement.Width = num2.Value;
			}
			num = Strings.TitleBarTextMaxWidth;
			if (num2 != null)
			{
				FrameworkElement frameworkElement2 = this.mTitleText;
				num = Strings.TitleBarTextMaxWidth;
				frameworkElement2.MaxWidth = num2.Value;
			}
			this.mVersionText.Text = "t.me/de_mistiyt";
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x0008CBD0 File Offset: 0x0008ADD0
		private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
		{
			if (this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible && base.Visibility == Visibility.Visible && this.ParentWindow.mSidebar.Visibility != Visibility.Visible && !FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					this.ParentWindow.mCommonHandler.FlipSidebarVisibility(this.mSidebarButton, null);
				}), new object[0]);
			}
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x0000FEAF File Offset: 0x0000E0AF
		internal void ChangeDownloadPercent(int percent)
		{
			PercentageChangedEventHandler percentChanged = this.PercentChanged;
			if (percentChanged == null)
			{
				return;
			}
			percentChanged(this, new PercentageChangedEventArgs
			{
				Percentage = percent
			});
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x0008CC38 File Offset: 0x0008AE38
		internal void InitializeSnailButton()
		{
			if (FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsWarningBtnEnabled)
			{
				return;
			}
			string deviceCaps = RegistryManager.Instance.DeviceCaps;
			if (string.IsNullOrEmpty(deviceCaps))
			{
				this.mSnailMode = PerformanceState.VtxEnabled;
				this.TopBarOptionsPanelElementVisibility(this.mWarningButton, false);
				return;
			}
			JObject deviceCapsData = JObject.Parse(deviceCaps);
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (deviceCapsData["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && deviceCapsData["bios_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
				{
					if (deviceCapsData["engine_enabled"].ToString().Equals(EngineState.raw.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						this.mSnailMode = PerformanceState.VtxDisabled;
						this.TopBarOptionsPanelElementVisibility(this.mWarningButton, true);
					}
				}
				else if (deviceCapsData["cpu_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
				{
					this.mSnailMode = PerformanceState.NoVtxSupport;
					this.TopBarOptionsPanelElementVisibility(this.mWarningButton, true);
				}
				else if (deviceCapsData["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && deviceCapsData["bios_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
				{
					this.mSnailMode = PerformanceState.VtxEnabled;
					this.TopBarOptionsPanelElementVisibility(this.mWarningButton, false);
				}
				this.RefreshWarningButton();
			}), new object[0]);
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x0008CCC0 File Offset: 0x0008AEC0
		internal void RefreshWarningButton()
		{
			if (FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsWarningBtnEnabled)
			{
				return;
			}
			if (this.mSnailMode != PerformanceState.VtxEnabled)
			{
				this.TopBarOptionsPanelElementVisibility(this.mWarningButton, true);
				this.AddVtxNotification();
				return;
			}
			this.TopBarOptionsPanelElementVisibility(this.mWarningButton, false);
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x0000FECE File Offset: 0x0000E0CE
		internal void AddVtxNotification()
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				return;
			}
			base.Dispatcher.Invoke(new Action(delegate
			{
				bool flag = true;
				GenericNotificationItem genericNotificationItem = new GenericNotificationItem
				{
					CreationTime = DateTime.Now,
					IsDeferred = false,
					Priority = NotificationPriority.Important,
					ShowRibbon = false,
					Id = "VtxNotification",
					NotificationMenuImageName = "SlowPerformance.png",
					Title = LocaleStrings.GetLocalizedString("STRING_DISABLED_VT_TITLE", ""),
					Message = LocaleStrings.GetLocalizedString("STRING_DISABLED_VT", "")
				};
				SerializableDictionary<string, string> serializableDictionary = new SerializableDictionary<string, string>
				{
					{ "click_generic_action", "UserBrowser" },
					{
						"click_action_value",
						WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
						{
							WebHelper.GetServerHost(),
							"help_articles"
						})) + "&article=enable_virtualization"
					}
				};
				genericNotificationItem.ExtraPayload.ClearAddRange(serializableDictionary);
				GenericNotificationManager.AddNewNotification(genericNotificationItem, flag);
				this.RefreshNotificationCentreButton();
			}), new object[0]);
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x0000FEFB File Offset: 0x0000E0FB
		internal void AddRamNotification()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				bool flag = true;
				GenericNotificationItem genericNotificationItem = new GenericNotificationItem
				{
					IsDeferred = false,
					Priority = NotificationPriority.Important,
					ShowRibbon = false,
					Id = "ramNotification",
					NotificationMenuImageName = "SlowPerformance.png",
					Title = LocaleStrings.GetLocalizedString("STRING_RAM_NOTIF_TITLE", ""),
					Message = LocaleStrings.GetLocalizedString("STRING_RAM_NOTIF", "")
				};
				SerializableDictionary<string, string> serializableDictionary = new SerializableDictionary<string, string>
				{
					{ "click_generic_action", "UserBrowser" },
					{
						"click_action_value",
						WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
						{
							WebHelper.GetServerHost(),
							"help_articles"
						})) + "&article=bs3_nougat_min_requirements"
					}
				};
				genericNotificationItem.ExtraPayload.ClearAddRange(serializableDictionary);
				GenericNotificationManager.AddNewNotification(genericNotificationItem, flag);
				this.RefreshNotificationCentreButton();
			}), new object[0]);
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x0008CD10 File Offset: 0x0008AF10
		private void UserAccountButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked account button");
			if (this.ParentWindow.mGuestBootCompleted && this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted)
			{
				if (FeatureManager.Instance.IsOpenActivityFromAccountIcon)
				{
					this.mAppTabButtons.AddAppTab("STRING_ACCOUNT", BlueStacksUIUtils.sUserAccountPackageName, BlueStacksUIUtils.sUserAccountActivityName, "account_tab", true, true, false);
					return;
				}
				string text = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/bluestacks_account");
				text += "&email=";
				text += RegistryManager.Instance.RegisteredEmail;
				text += "&token=";
				text += RegistryManager.Instance.Token;
				this.mAppTabButtons.AddWebTab(text, "STRING_ACCOUNT", "account_tab", true, "account_tab", false);
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x0008CDE8 File Offset: 0x0008AFE8
		private void ConfigButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mPreferenceDropDownControl.LateInit();
			this.mSettingsMenuPopup.IsOpen = true;
			this.mSettingsMenuPopup.HorizontalOffset = -(this.mPreferenceDropDownBorder.ActualWidth - 40.0);
			this.mConfigButton.ImageName = "cfgmenu_hover";
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x0000FF1B File Offset: 0x0000E11B
		private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked minimize button");
			this.ParentWindow.MinimizeWindow();
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x0000FF32 File Offset: 0x0000E132
		internal void MaxmizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked Maximize\\Restore button");
			if (this.ParentWindow.WindowState == WindowState.Normal && !this.ParentWindow.mIsDmmMaximised)
			{
				this.ParentWindow.MaximizeWindow();
				return;
			}
			this.ParentWindow.RestoreWindows(false);
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x0000FF70 File Offset: 0x0000E170
		internal void SetConfigIndicator(string config)
		{
			this.mLocalConfigIndicator.Visibility = (string.Equals(config, ".config_user.db", StringComparison.InvariantCultureIgnoreCase) ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x0008CE40 File Offset: 0x0008B040
		private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked close Bluestacks button");
			Stats.SendCommonClientStatsAsync("notification_mode", "BlueStacks_close", this.ParentWindow.mVmName, "", "", "");
			if (!RegistryManager.Instance.IsNotificationModeAlwaysOn || string.Compare("Android", this.ParentWindow.mVmName, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				Stats.SendCommonClientStatsAsync("notification_mode", "notification_mode", this.ParentWindow.mVmName, string.Empty, "off", "");
				this.ParentWindow.CloseWindow();
				return;
			}
			if (this.ParentWindow.Utils.CheckQuitPopupLocal())
			{
				return;
			}
			Stats.SendCommonClientStatsAsync("notification_mode", "notification_mode", this.ParentWindow.mVmName, string.Empty, "on", "");
			this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
			this.ParentWindow.IsInNotificationMode = true;
			foreach (string text in this.ParentWindow.AppNotificationCountDictForEachVM.Keys)
			{
				Stats.SendCommonClientStatsAsync("notification_mode", "notification_number", this.ParentWindow.mVmName, text, this.ParentWindow.AppNotificationCountDictForEachVM[text].ToString(CultureInfo.InvariantCulture), "NM_Off");
			}
			this.ParentWindow.AppNotificationCountDictForEachVM.Clear();
			this.ParentWindow.MinimizeWindowHandler();
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x0000FF8F File Offset: 0x0000E18F
		private void NotificationPopup_Opened(object sender, EventArgs e)
		{
			this.mConfigButton.IsEnabled = false;
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x0000FF9D File Offset: 0x0000E19D
		private void NotificationPopup_Closed(object sender, EventArgs e)
		{
			this.mConfigButton.IsEnabled = true;
			this.mConfigButton.ImageName = "cfgmenu";
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x0000FFBB File Offset: 0x0000E1BB
		internal void ChangeUserPremiumButton(bool isPremium)
		{
			if (isPremium)
			{
				this.mUserAccountBtn.ImageName = BlueStacksUIUtils.sPremiumUserImageName;
				return;
			}
			this.mUserAccountBtn.ImageName = BlueStacksUIUtils.sLoggedInImageName;
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void PreferenceDropDownControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x0008CFDC File Offset: 0x0008B1DC
		private void mWarningButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked warning button for speed up Bluestacks ");
			this.mWarningButton.ImageName = "warning";
			SpeedUpBlueStacks speedUpBlueStacks = new SpeedUpBlueStacks();
			if (this.mSnailMode == PerformanceState.NoVtxSupport)
			{
				speedUpBlueStacks.mUpgradeComputer.Visibility = Visibility.Visible;
			}
			else if (this.mSnailMode == PerformanceState.VtxDisabled)
			{
				speedUpBlueStacks.mEnableVt.Visibility = Visibility.Visible;
			}
			new ContainerWindow(this.ParentWindow, speedUpBlueStacks, 640.0, 200.0, false, true, false, -1.0, null);
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x0000FFE1 File Offset: 0x0000E1E1
		private void mBtvButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked btv button");
			BTVManager.Instance.StartBlueStacksTV();
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x0008D064 File Offset: 0x0008B264
		private void TopBar_Loaded(object sender, RoutedEventArgs e)
		{
			if (FeatureManager.Instance.IsBTVEnabled && string.Equals(Strings.CurrentDefaultVmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture))
			{
				this.TopBarOptionsPanelElementVisibility(this.mBtvButton, true);
			}
			this.RefreshNotificationCentreButton();
			if (!this.ParentWindow.mGuestBootCompleted)
			{
				this.ParentWindow.mCommonHandler.SetSidebarImageProperties(false, this.mSidebarButton, null);
				this.ParentWindow.GuestBootCompleted += this.ParentWindow_GuestBootCompletedEvent;
			}
			this.ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += this.TopBar_ScreenRecordingStateChangedEvent;
			VideoRecordingStatus videoRecordingStatus = this.mVideoRecordStatusControl;
			videoRecordingStatus.RecordingStoppedEvent = (Action)Delegate.Combine(videoRecordingStatus.RecordingStoppedEvent, new Action(this.TopBar_RecordingStoppedEvent));
			if (this.ParentWindow.mVmName == "Android" && this.mTitleIcon.ToolTip.ToString().Equals(Strings.ProductTopBarDisplayName, StringComparison.OrdinalIgnoreCase))
			{
				this.mTitleIcon.ToolTip = new ToolTip
				{
					Content = (Strings.ProductDisplayName ?? "")
				};
			}
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x0000FFF7 File Offset: 0x0000E1F7
		private void TopBar_RecordingStoppedEvent()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.mVideoRecordStatusControl.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x0008D180 File Offset: 0x0008B380
		private void TopBar_ScreenRecordingStateChangedEvent(bool isRecording)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (isRecording)
				{
					if (this.mVideoRecordStatusControl.Visibility != Visibility.Visible && CommonHandlers.sIsRecordingVideo)
					{
						this.mVideoRecordStatusControl.Init(this.ParentWindow);
						this.mVideoRecordStatusControl.Visibility = Visibility.Visible;
						return;
					}
				}
				else
				{
					this.mVideoRecordStatusControl.ResetTimer();
					this.mVideoRecordStatusControl.Visibility = Visibility.Collapsed;
				}
			}), new object[0]);
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x0008D1C4 File Offset: 0x0008B3C4
		public void mNotificationCentreButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked notification_centre button");
			this.mNotificationDrawerControl.Width = 320.0;
			SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && (string.Equals(x.VmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
			this.mNotificationDrawerControl.Populate(notificationItems);
			ClientStats.SendMiscellaneousStatsAsync("NotificationBellIconClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, null, null, null, null);
			GenericNotificationManager.MarkNotification(notificationItems.Keys, delegate(GenericNotificationItem x)
			{
				if (x.IsReceivedStatSent && !x.IsDeleted && !x.IsShown && !x.IsAndroidNotification)
				{
					x.IsShown = true;
					ClientStats.SendMiscellaneousStatsAsync("notification_shown", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, x.Id, x.Title, x.ExtraPayload.ContainsKey("campaign_id") ? x.ExtraPayload["campaign_id"] : "", null, null, null);
				}
			});
			this.mNotificationDrawerControl.UpdateNotificationCount();
			if (sender != null)
			{
				this.mNotificationCentreButton.ImageName = "notification";
				this.mNotificationCountBadge.Visibility = Visibility.Collapsed;
			}
			else
			{
				NotificationDrawer.DrawerAnimationTimer.Start();
			}
			this.mNotificationCentrePopup.IsOpen = true;
			this.mNotificationDrawerControl.mNotificationScroll.ScrollToTop();
			this.mNotificationCentreButton.ImageName = "notification_hover";
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x0008D2C0 File Offset: 0x0008B4C0
		internal bool CheckForRam()
		{
			int num = 0;
			try
			{
				num = (int)(ulong.Parse(new ComputerInfo().TotalPhysicalMemory.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) / this.MB_MULTIPLIER);
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
			return num < 4096;
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x0008D324 File Offset: 0x0008B524
		internal void RefreshNotificationCentreButton()
		{
			if (this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone && FeatureManager.Instance.IsShowNotificationCentre && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
			{
				this.TopBarOptionsPanelElementVisibility(this.mNotificationGrid, true);
				if (GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsRead && !x.IsDeleted && x.Priority == NotificationPriority.Important).Count > 0)
				{
					this.mNotificationCentreButton.ImageName = "notification_crucial";
				}
				else
				{
					this.mNotificationCentreButton.ImageName = "notification";
				}
				this.mNotificationDrawerControl.UpdateNotificationCount();
				return;
			}
			this.TopBarOptionsPanelElementVisibility(this.mNotificationGrid, false);
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x0008D3D4 File Offset: 0x0008B5D4
		internal void mNotificationCentreDropDownBorder_LayoutUpdated(object sender, EventArgs e)
		{
			RectangleGeometry rectangleGeometry = new RectangleGeometry();
			Rect rect = new Rect
			{
				Height = this.mNotificationCentreDropDownBorder.ActualHeight,
				Width = this.mNotificationCentreDropDownBorder.ActualWidth
			};
			BlueStacksUIBinding.BindCornerRadiusToDouble(rectangleGeometry, RectangleGeometry.RadiusXProperty, "PreferenceDropDownRadius");
			BlueStacksUIBinding.BindCornerRadiusToDouble(rectangleGeometry, RectangleGeometry.RadiusYProperty, "PreferenceDropDownRadius");
			rectangleGeometry.Rect = rect;
			this.mNotificationCentreDropDownBorder.Clip = rectangleGeometry;
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x0008D448 File Offset: 0x0008B648
		internal void ShowRecordingIcons()
		{
			this.mMacroRecordControl.Init(this.ParentWindow);
			this.mMacroRecordControl.Visibility = Visibility.Visible;
			this.mMacroRecordControl.StartTimer();
			if (!this.ParentWindow.mIsFullScreen)
			{
				this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.IsOpen = true;
				this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.StaysOpen = true;
				this.mMacroRecordingPopupTimer = new DispatcherTimer
				{
					Interval = new TimeSpan(0, 0, 0, 5, 0)
				};
				this.mMacroRecordingPopupTimer.Tick += this.MacroRecordingPopupTimer_Tick;
				this.mMacroRecordingPopupTimer.Start();
			}
		}

		// Token: 0x060017C7 RID: 6087 RVA: 0x0001001C File Offset: 0x0000E21C
		private void MacroRecordingPopupTimer_Tick(object sender, EventArgs e)
		{
			this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.IsOpen = false;
			(sender as DispatcherTimer).Stop();
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x0008D4F4 File Offset: 0x0008B6F4
		internal void HideRecordingIcons()
		{
			this.mConfigButton.Visibility = Visibility.Visible;
			if (this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
			{
				this.TopBarOptionsPanelElementVisibility(this.mNotificationGrid, true);
				this.TopBarOptionsPanelElementVisibility(this.mUserAccountBtn, true);
			}
			this.mMacroRecordControl.Visibility = Visibility.Collapsed;
			this.mMacroRecorderToolTipPopup.IsOpen = false;
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x0008D554 File Offset: 0x0008B754
		internal void ShowMacroPlaybackOnTopBar(MacroRecording record)
		{
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.mMacroPlayControl.Init(this.ParentWindow, record);
				this.mMacroPlayControl.Visibility = Visibility.Visible;
				if (!this.ParentWindow.mIsFullScreen)
				{
					this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.IsOpen = true;
					this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.StaysOpen = true;
					this.mMacroRunningPopupTimer = new DispatcherTimer
					{
						Interval = new TimeSpan(0, 0, 0, 5, 0)
					};
					this.mMacroRunningPopupTimer.Tick += this.MacroRunningPopupTimer_Tick;
					this.mMacroRunningPopupTimer.Start();
				}
			}
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x0001003F File Offset: 0x0000E23F
		private void MacroRunningPopupTimer_Tick(object sender, EventArgs e)
		{
			this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.IsOpen = false;
			(sender as DispatcherTimer).Stop();
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x0008D604 File Offset: 0x0008B804
		internal void HideMacroPlaybackFromTopBar()
		{
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.mConfigButton.Visibility = Visibility.Visible;
				if (this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
				{
					this.TopBarOptionsPanelElementVisibility(this.mNotificationGrid, true);
					this.TopBarOptionsPanelElementVisibility(this.mUserAccountBtn, true);
				}
				this.mMacroPlayControl.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060017CC RID: 6092 RVA: 0x0008D664 File Offset: 0x0008B864
		internal void UpdateMacroRecordingProgress()
		{
			if (this.ParentWindow.mIsMacroPlaying || this.ParentWindow.mIsMacroRecorderActive)
			{
				this.mConfigButton.Visibility = Visibility.Visible;
				if (this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
				{
					this.TopBarOptionsPanelElementVisibility(this.mNotificationGrid, true);
					this.TopBarOptionsPanelElementVisibility(this.mUserAccountBtn, true);
				}
			}
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x00010062 File Offset: 0x0000E262
		internal void ShowSyncIcon()
		{
			this.TopBarOptionsPanelElementVisibility(this.mOperationsSyncGrid, true);
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x00010071 File Offset: 0x0000E271
		internal void HideSyncIcon()
		{
			this.TopBarOptionsPanelElementVisibility(this.mOperationsSyncGrid, false);
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00010080 File Offset: 0x0000E280
		private void MSidebarButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MainWindow parentWindow = this.ParentWindow;
			if (parentWindow == null)
			{
				return;
			}
			CommonHandlers mCommonHandler = parentWindow.mCommonHandler;
			if (mCommonHandler == null)
			{
				return;
			}
			mCommonHandler.FlipSidebarVisibility(sender as CustomPictureBox, null);
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x000100A3 File Offset: 0x0000E2A3
		private void TopBar_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				this.TopBarButtonsHandling();
			}
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x0008D6C4 File Offset: 0x0008B8C4
		private void TopBarButtonsHandling()
		{
			double num = base.ActualWidth - 180.0 - (double)(this.mAppTabButtons.mDictTabs.Count * 48);
			double num2 = this.mOptionsDockPanel.ActualWidth;
			if (num2 > num)
			{
				using (IEnumerator<KeyValuePair<FrameworkElement, double>> enumerator = this.mOptionsPriorityPanel.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<FrameworkElement, double> keyValuePair = enumerator.Current;
						if (keyValuePair.Key.Visibility == Visibility.Visible)
						{
							keyValuePair.Key.Visibility = Visibility.Collapsed;
							num2 -= keyValuePair.Value;
						}
						if (num2 < num)
						{
							break;
						}
					}
					return;
				}
			}
			for (int i = this.mOptionsPriorityPanel.Count - 1; i >= 0; i--)
			{
				KeyValuePair<FrameworkElement, double> value = this.mOptionsPriorityPanel.ElementAt(i).Value;
				if (value.Key.Visibility == Visibility.Collapsed)
				{
					if (num2 + value.Value >= num)
					{
						break;
					}
					value.Key.Visibility = Visibility.Visible;
					num2 += value.Value;
				}
			}
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x0008D7D8 File Offset: 0x0008B9D8
		private bool ContainsKey(FrameworkElement element)
		{
			foreach (KeyValuePair<FrameworkElement, double> keyValuePair in this.mOptionsPriorityPanel.Values)
			{
				if (keyValuePair.Key == element)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x0008D834 File Offset: 0x0008BA34
		private void RemoveKey(FrameworkElement element)
		{
			foreach (KeyValuePair<int, KeyValuePair<FrameworkElement, double>> keyValuePair in this.mOptionsPriorityPanel)
			{
				if (keyValuePair.Value.Key == element)
				{
					this.mOptionsPriorityPanel.Remove(keyValuePair.Key);
					break;
				}
			}
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x0008D8A4 File Offset: 0x0008BAA4
		internal void TopBarOptionsPanelElementVisibility(FrameworkElement element, bool isVisible)
		{
			if (isVisible)
			{
				double num = base.ActualWidth - 180.0 - (double)(this.mAppTabButtons.mDictTabs.Count * 48);
				if (this.mOptionsDockPanel.ActualWidth + element.Width < num)
				{
					element.Visibility = Visibility.Visible;
				}
				else
				{
					element.Visibility = Visibility.Collapsed;
				}
				if (!this.ContainsKey(element))
				{
					this.mOptionsPriorityPanel.Add(int.Parse(element.Tag.ToString(), CultureInfo.InvariantCulture), new KeyValuePair<FrameworkElement, double>(element, element.Width));
					return;
				}
			}
			else
			{
				element.Visibility = Visibility.Collapsed;
				if (this.ContainsKey(element))
				{
					this.RemoveKey(element);
				}
			}
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x000100B3 File Offset: 0x0000E2B3
		void ITopBar.ShowSyncPanel(bool isSource)
		{
			this.mOperationsSyncGrid.Visibility = Visibility.Visible;
			if (isSource)
			{
				this.mPlayPauseSyncButton.ImageName = "pause_title_bar";
				this.mPlayPauseSyncButton.Visibility = Visibility.Visible;
				this.mStopSyncButton.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x000100EC File Offset: 0x0000E2EC
		void ITopBar.HideSyncPanel()
		{
			this.mOperationsSyncGrid.Visibility = Visibility.Collapsed;
			this.mPlayPauseSyncButton.Visibility = Visibility.Collapsed;
			this.mStopSyncButton.Visibility = Visibility.Collapsed;
			this.mSyncInstancesToolTipPopup.IsOpen = false;
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x0008D950 File Offset: 0x0008BB50
		private void PlayPauseSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if ((sender as CustomPictureBox).ImageName.Equals("pause_title_bar", StringComparison.InvariantCultureIgnoreCase))
			{
				(sender as CustomPictureBox).ImageName = "play_title_bar";
				this.ParentWindow.mSynchronizerWindow.PauseAllSyncOperations();
				return;
			}
			(sender as CustomPictureBox).ImageName = "pause_title_bar";
			this.ParentWindow.mSynchronizerWindow.PlayAllSyncOperations();
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x0001011E File Offset: 0x0000E31E
		private void StopSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			((ITopBar)this).HideSyncPanel();
			this.ParentWindow.mSynchronizerWindow.StopAllSyncOperations();
			if (RegistryManager.Instance.IsShowToastNotification)
			{
				this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STOPPED", ""));
			}
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x0001015C File Offset: 0x0000E35C
		private void OperationsSyncGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.ParentWindow.mIsSynchronisationActive)
			{
				this.mSyncInstancesToolTipPopup.IsOpen = true;
			}
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x00010177 File Offset: 0x0000E377
		private void OperationsSyncGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.ParentWindow.mIsSynchronisationActive && !this.mOperationsSyncGrid.IsMouseOver && !this.mSyncInstancesToolTipPopup.IsMouseOver)
			{
				this.mSyncInstancesToolTipPopup.IsOpen = false;
			}
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x000101AC File Offset: 0x0000E3AC
		private void SyncInstancesToolTip_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.mOperationsSyncGrid.IsMouseOver && !this.mSyncInstancesToolTipPopup.IsMouseOver)
			{
				this.mSyncInstancesToolTipPopup.IsOpen = false;
			}
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x0008D9B8 File Offset: 0x0008BBB8
		internal void ClosePopups()
		{
			if (this.mMacroRecorderToolTipPopup.IsOpen)
			{
				this.mMacroRecorderToolTipPopup.IsOpen = false;
			}
			if (this.mMacroRunningToolTipPopup.IsOpen)
			{
				this.mMacroRunningToolTipPopup.IsOpen = false;
			}
			if (this.mNotificationCentrePopup.IsOpen)
			{
				this.mNotificationCentrePopup.IsOpen = false;
			}
			if (this.mSettingsMenuPopup.IsOpen)
			{
				this.mSettingsMenuPopup.IsOpen = false;
			}
			if (this.mSyncInstancesToolTipPopup.IsOpen)
			{
				this.mSyncInstancesToolTipPopup.IsOpen = false;
			}
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x0008DA44 File Offset: 0x0008BC44
		private void HelpButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			string helpCenterUrl = BlueStacksUIUtils.GetHelpCenterUrl();
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				BlueStacksUIUtils.OpenUrl(helpCenterUrl);
				return;
			}
			this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(helpCenterUrl, "STRING_FEEDBACK", "help_center", true, "FEEDBACK_TEXT", false);
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x0008DA94 File Offset: 0x0008BC94
		private void mNotificationCentrePopup_Closed(object sender, EventArgs e)
		{
			GenericNotificationManager.MarkNotification(new List<string>(GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && string.Equals(x.VmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture)).Keys), delegate(GenericNotificationItem x)
			{
				x.IsRead = true;
			});
			this.mNotificationDrawerControl.UpdateNotificationCount();
			this.mNotificationCentreButton.ImageName = "notification";
			this.mNotificationCentreButton.IsEnabled = true;
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x000101D4 File Offset: 0x0000E3D4
		private void mNotificationCentrePopup_Opened(object sender, EventArgs e)
		{
			this.mNotificationCentreButton.IsEnabled = false;
		}

		// Token: 0x04000EDE RID: 3806
		private MainWindow mMainWindow;

		// Token: 0x04000EE0 RID: 3808
		private SortedList<int, KeyValuePair<FrameworkElement, double>> mOptionsPriorityPanel;

		// Token: 0x04000EE1 RID: 3809
		internal double mMinimumExpectedTopBarWidth;

		// Token: 0x04000EE2 RID: 3810
		internal PerformanceState mSnailMode;

		// Token: 0x04000EE3 RID: 3811
		private DispatcherTimer mMacroRunningPopupTimer;

		// Token: 0x04000EE4 RID: 3812
		private DispatcherTimer mMacroRecordingPopupTimer;

		// Token: 0x04000EE5 RID: 3813
		private ulong MB_MULTIPLIER;
	}
}
