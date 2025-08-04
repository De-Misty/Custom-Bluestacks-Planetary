using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using BlueStacks.BlueStacksUI.Controls;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000288 RID: 648
	public class PreferenceDropDownControl : UserControl, IComponentConnector, IStyleConnector
	{
		// Token: 0x17000360 RID: 864
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x0000FBF2 File Offset: 0x0000DDF2
		// (set) Token: 0x0600175B RID: 5979 RVA: 0x0000FBFA File Offset: 0x0000DDFA
		public MainWindow ParentWindow { get; set; }

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x0600175C RID: 5980 RVA: 0x0008AE68 File Offset: 0x00089068
		// (remove) Token: 0x0600175D RID: 5981 RVA: 0x0008AEA0 File Offset: 0x000890A0
		private event EventHandler LogoutConfirmationResetAccountAcceptedHandler;

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x0600175E RID: 5982 RVA: 0x0008AED8 File Offset: 0x000890D8
		// (remove) Token: 0x0600175F RID: 5983 RVA: 0x0008AF10 File Offset: 0x00089110
		private event EventHandler RestoreDefaultConfirmationClicked;

		// Token: 0x06001760 RID: 5984 RVA: 0x0008AF48 File Offset: 0x00089148
		public PreferenceDropDownControl()
		{
			this.InitializeComponent();
			this.LogoutConfirmationResetAccountAcceptedHandler += this.PreferenceDropDownControl_CloseWindowConfirmationResetAccountAcceptedHandler;
			this.RestoreDefaultConfirmationClicked += this.PreferenceDropDownControl_RestoreDefaultConfirmationClicked;
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				this.mSpeedUpBstGrid.Visibility = Visibility.Collapsed;
				this.mUpgradeToFullBlueStacks.Visibility = Visibility.Visible;
			}
			if (!FeatureManager.Instance.IsShowSpeedUpTips)
			{
				this.mSpeedUpBstGrid.Visibility = Visibility.Collapsed;
			}
			if (!FeatureManager.Instance.IsShowHelpCenter)
			{
				this.mHelpCenterGrid.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x0008AFDC File Offset: 0x000891DC
		private void PreferenceDropDownControl_RestoreDefaultConfirmationClicked(object sender, EventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "RestoreDefaultWallpaper", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "Premium", null);
			this.mChooseWallpaperPopup.IsOpen = false;
			this.ParentWindow.Utils.RestoreWallpaperImageForAllVms();
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x0008B048 File Offset: 0x00089248
		internal void Init(MainWindow parentWindow)
		{
			this.ParentWindow = parentWindow;
			if (Oem.Instance.IsRemoveAccountOnExit)
			{
				this.mLogoutButtonGrid.Visibility = Visibility.Visible;
			}
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				this.mUpgradeToFullTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UPGRADE_TO_STANDARD_BST", "").Replace(GameConfig.Instance.AppName, "BlueStacks");
			}
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x0008B0B0 File Offset: 0x000892B0
		internal void LateInit()
		{
			if (FeatureManager.Instance.ShowClientOnTopPreference)
			{
				if (this.ParentWindow.EngineInstanceRegistry.IsClientOnTop)
				{
					this.mPinToTopToggleButton.ImageName = this.mPinToTopToggleButton.ImageName.Replace("_off", "_on");
				}
				else
				{
					this.mPinToTopToggleButton.ImageName = this.mPinToTopToggleButton.ImageName.Replace("_on", "_off");
				}
			}
			else
			{
				this.mPinToTopGrid.Visibility = Visibility.Collapsed;
			}
			if (FeatureManager.Instance.IsThemeEnabled && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
			{
				this.mChangeSkinGrid.Visibility = Visibility.Visible;
			}
			if (this.ParentWindow != null && this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone && !FeatureManager.Instance.IsWallpaperChangeDisabled && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition && !FeatureManager.Instance.IsHtmlHome)
			{
				this.mChangeWallpaperGrid.Visibility = Visibility.Visible;
			}
			this.mAutoAlignGrid.MouseLeftButtonUp += this.AutoAlign_MouseLeftButtonUp;
			this.mAutoAlignGrid.Opacity = 1.0;
			if (!FeatureManager.Instance.IsOperationsSyncEnabled)
			{
				this.mSyncGrid.Visibility = Visibility.Collapsed;
			}
			else if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName) && !this.ParentWindow.mIsSyncMaster)
			{
				this.mSyncGrid.PreviewMouseLeftButtonUp -= this.SyncGrid_MouseLeftButtonUp;
				this.mSyncGrid.MouseEnter -= this.Grid_MouseEnter;
				this.mSyncGrid.Opacity = 0.5;
			}
			else
			{
				this.mSyncGrid.PreviewMouseLeftButtonUp -= this.SyncGrid_MouseLeftButtonUp;
				this.mSyncGrid.PreviewMouseLeftButtonUp += this.SyncGrid_MouseLeftButtonUp;
				this.mSyncGrid.MouseEnter -= this.Grid_MouseEnter;
				this.mSyncGrid.MouseEnter += this.Grid_MouseEnter;
				this.mSyncGrid.Opacity = 1.0;
			}
			this.SectionsTagVisibilityToggling();
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x0000FC03 File Offset: 0x0000DE03
		internal void SectionsTagVisibilityToggling()
		{
			this.mCustomiseSectionTag.Visibility = (PreferenceDropDownControl.CheckSectionTagVisibility(this.mCustomiseSection) ? Visibility.Visible : Visibility.Collapsed);
			this.mHelpandsupportSectionTag.Visibility = (PreferenceDropDownControl.CheckSectionTagVisibility(this.mHelpandsupportSection) ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0008B2CC File Offset: 0x000894CC
		private static bool CheckSectionTagVisibility(Grid sectionGrid)
		{
			using (IEnumerator enumerator = sectionGrid.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if ((((UIElement)enumerator.Current) as Grid).Visibility == Visibility.Visible)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x00007C8F File Offset: 0x00005E8F
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x00006A61 File Offset: 0x00004C61
		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			(sender as Grid).Background = Brushes.Transparent;
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x0008B330 File Offset: 0x00089530
		private void EngineSettingGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked settings button");
			this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
			string text = string.Empty;
			if (this.ParentWindow.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName))
			{
				text = "STRING_GAME_SETTINGS";
			}
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			this.ParentWindow.mCommonHandler.LaunchSettingsWindow(text);
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x0008B3F0 File Offset: 0x000895F0
		private void ReportProblemGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked report problem button");
			using (Process process = new Process())
			{
				process.StartInfo.Arguments = "-vmname:" + this.ParentWindow.mVmName;
				process.StartInfo.FileName = global::System.IO.Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
				process.Start();
			}
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ReportProblem", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x0008B4A8 File Offset: 0x000896A8
		private void LogoutButtonGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked logout button");
			if (this.ParentWindow.mGuestBootCompleted)
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_LOGOUT_BLUESTACKS3", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_REMOVE_GOOGLE_ACCOUNT", "");
				customMessageWindow.AddButton(ButtonColors.Red, "STRING_LOGOUT_BUTTON", this.LogoutConfirmationResetAccountAcceptedHandler, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
				ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "Logout", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x0000FC3D File Offset: 0x0000DE3D
		private void PreferenceDropDownControl_CloseWindowConfirmationResetAccountAcceptedHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mAppHandler.SendRequestToRemoveAccountAndCloseWindowASync(false);
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x0008B590 File Offset: 0x00089790
		private void SpeedUpBstGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked SpeedUp BlueStacks button");
			SpeedUpBlueStacks speedUpBlueStacks = new SpeedUpBlueStacks();
			if (this.ParentWindow.mTopBar.mSnailMode == PerformanceState.VtxDisabled)
			{
				speedUpBlueStacks.mEnableVt.Visibility = Visibility.Visible;
			}
			speedUpBlueStacks.mUpgradeComputer.Visibility = Visibility.Visible;
			speedUpBlueStacks.mPowerPlan.Visibility = Visibility.Visible;
			speedUpBlueStacks.mConfigureAntivirus.Visibility = Visibility.Visible;
			new ContainerWindow(this.ParentWindow, speedUpBlueStacks, 640.0, 440.0, false, true, false, -1.0, null);
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "SpeedUpBlueStacks", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x0008B65C File Offset: 0x0008985C
		private void mHelpCenterGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			string helpCenterUrl = BlueStacksUIUtils.GetHelpCenterUrl();
			this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "HelpCentre", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				BlueStacksUIUtils.OpenUrl(helpCenterUrl);
				return;
			}
			this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(helpCenterUrl, "STRING_FEEDBACK", "help_center", true, "FEEDBACK_TEXT", false);
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x0008B700 File Offset: 0x00089900
		private void mChangeSkinGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ChangeThemeWindow changeThemeWindow = new ChangeThemeWindow(this.ParentWindow);
			int num = 504;
			int num2 = 652;
			new ContainerWindow(this.ParentWindow, changeThemeWindow, (double)num2, (double)num, false, true, false, -1.0, null);
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeSkin", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x0000FC50 File Offset: 0x0000DE50
		private void NotificationPopup_Opened(object sender, EventArgs e)
		{
			this.dummyGridForSize2.Visibility = Visibility.Visible;
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x0000FC5E File Offset: 0x0000DE5E
		private void NotificationPopup_Closed(object sender, EventArgs e)
		{
			this.mChangeWallpaperGrid.Background = Brushes.Transparent;
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0008B784 File Offset: 0x00089984
		private void ChooseNewGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
			if (RegistryManager.Instance.IsPremium)
			{
				ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeWallPaperButton", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "Premium", null);
				this.ParentWindow.Utils.ChooseWallpaper();
				return;
			}
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeWallPaperButton", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "NonPremium", null);
			string text = "/bluestacks_account?extra=section:plans";
			string text2 = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + text);
			text2 += "&email=";
			text2 += RegistryManager.Instance.RegisteredEmail;
			text2 += "&token=";
			text2 += RegistryManager.Instance.Token;
			this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(text2, "STRING_ACCOUNT", "account_tab", true, "account_tab", false);
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x0008B8C0 File Offset: 0x00089AC0
		private void SetDefaultGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (File.Exists(HomeAppManager.BackgroundImagePath))
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_LBL_RESTORE_DEFAULT", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_RESTORE_DEFAULT_WALLPAPER", "");
				customMessageWindow.AddButton(ButtonColors.Red, "STRING_RESTORE_BUTTON", this.RestoreDefaultConfirmationClicked, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
			}
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x0008B960 File Offset: 0x00089B60
		private void mChangeWallpaperGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (File.Exists(HomeAppManager.BackgroundImagePath))
			{
				this.mChangeWallpaperGrid.MouseLeftButtonUp -= this.ChooseNewGrid_MouseLeftButtonUp;
				this.mWallpaperPopup.PlacementTarget = this.mChooseNewGrid;
				this.mChooseWallpaperPopup.IsOpen = false;
				this.mChooseWallpaperPopup.IsOpen = true;
			}
			else
			{
				if (!RegistryManager.Instance.IsPremium)
				{
					this.mWallpaperPopup.PlacementTarget = this.mChangeWallpaperGrid;
					this.mWallpaperPopup.IsOpen = false;
					this.mWallpaperPopup.IsOpen = true;
				}
				this.mChangeWallpaperGrid.MouseLeftButtonUp -= this.ChooseNewGrid_MouseLeftButtonUp;
				this.mChangeWallpaperGrid.MouseLeftButtonUp += this.ChooseNewGrid_MouseLeftButtonUp;
			}
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x0008BA34 File Offset: 0x00089C34
		private void mChangeWallpaperGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			(sender as Grid).Background = Brushes.Transparent;
			if (!this.mChangeWallpaperGrid.IsMouseOver && !this.mChooseWallpaperPopupGrid.IsMouseOver && !this.mWallpaperPopupGrid.IsMouseOver)
			{
				this.mChooseWallpaperPopup.IsOpen = false;
				this.mWallpaperPopup.IsOpen = false;
			}
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x0000FC70 File Offset: 0x0000DE70
		private void ChooseNewGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!RegistryManager.Instance.IsPremium)
			{
				this.mWallpaperPopup.IsOpen = true;
			}
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x0000FC9F File Offset: 0x0000DE9F
		private void ChooseNewGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.mChooseNewGrid.IsMouseOver && !this.mWallpaperPopupGrid.IsMouseOver)
			{
				this.mWallpaperPopup.IsOpen = false;
			}
			(sender as Grid).Background = Brushes.Transparent;
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x0000FCD7 File Offset: 0x0000DED7
		private void SetDefaultGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (File.Exists(HomeAppManager.BackgroundImagePath))
			{
				(sender as Grid).Background = Brushes.Transparent;
			}
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x0000FCF5 File Offset: 0x0000DEF5
		private void SetDefaultGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (File.Exists(HomeAppManager.BackgroundImagePath))
			{
				BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
			}
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x0000FD18 File Offset: 0x0000DF18
		private void mWallpaperPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.mChooseNewGrid.IsMouseOver)
			{
				this.mWallpaperPopup.IsOpen = false;
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x0008BA90 File Offset: 0x00089C90
		private void mChooseWallpaperPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.mChangeWallpaperGrid.IsMouseOver && !this.mChooseWallpaperPopupGrid.IsMouseOver && !this.mWallpaperPopupGrid.IsMouseOver)
			{
				this.mChooseWallpaperPopup.IsOpen = false;
				this.mWallpaperPopup.IsOpen = false;
			}
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x0008BADC File Offset: 0x00089CDC
		private void mUpgradeToFullBlueStacks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			string text = LocaleStrings.GetLocalizedString("STRING_UPGRADE_TO_STANDARD_BST", "");
			string text2 = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				LocaleStrings.GetLocalizedString("STRING_CONTINUING_WILL_UPGRADE_TO_STD_BST", ""),
				LocaleStrings.GetLocalizedString("STRING_LAUNCH_BLUESTACKS_FROM_DESK_SHORTCUT", "")
			});
			text = text.Replace(GameConfig.Instance.AppName, "BlueStacks");
			text2 = text2.Replace(GameConfig.Instance.AppName, "BlueStacks");
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_YES", new EventHandler(this.UpgradeToFullBstHandler), null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_NO", null, null, false, null);
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, text, "");
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, text2, "");
			this.ParentWindow.ShowDimOverlay(null);
			customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
			customMessageWindow.ShowDialog();
			this.ParentWindow.HideDimOverlay();
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "UpgradeBlueStacks", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x0008BC20 File Offset: 0x00089E20
		private void UpgradeToFullBstHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mWelcomeTab.mBackground.Visibility = Visibility.Visible;
			this.ParentWindow.ShowDimOverlayForUpgrade();
			using (BackgroundWorker backgroundWorker = new BackgroundWorker())
			{
				backgroundWorker.DoWork += this.MBWUpdateToFullVersion_DoWork;
				backgroundWorker.RunWorkerCompleted += this.MBWUpdateToFullVersion_RunWorkerCompleted;
				backgroundWorker.RunWorkerAsync();
			}
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x0000FD33 File Offset: 0x0000DF33
		private void MBWUpdateToFullVersion_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler(null, null);
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x0000FD42 File Offset: 0x0000DF42
		private void MBWUpdateToFullVersion_DoWork(object sender, DoWorkEventArgs e)
		{
			Utils.UpgradeToFullVersionAndCreateBstShortcut(true);
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x0008BC9C File Offset: 0x00089E9C
		private void SyncGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
			this.ParentWindow.ShowSynchronizerWindow();
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "OperationSync", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x0008BD08 File Offset: 0x00089F08
		private void mUpgradeBluestacksStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mUpgradeBluestacksStatusTextBlock.Text.ToString(CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
			{
				ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingsGearDwnld, "");
				UpdatePrompt updatePrompt = new UpdatePrompt(BlueStacksUpdater.sBstUpdateData)
				{
					Height = 215.0,
					Width = 400.0
				};
				new ContainerWindow(this.ParentWindow, updatePrompt, (double)((int)updatePrompt.Width), (double)((int)updatePrompt.Height), false, true, false, -1.0, null);
				return;
			}
			if (this.mUpgradeBluestacksStatusTextBlock.Text.ToString(CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_DOWNLOADING_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
			{
				this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
				BlueStacksUpdater.ShowDownloadProgress();
				return;
			}
			if (this.mUpgradeBluestacksStatusTextBlock.Text.ToString(CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_INSTALL_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
			{
				this.ParentWindow.ShowInstallPopup();
			}
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x0008BE24 File Offset: 0x0008A024
		internal void ToggleStreamingMode(bool enable)
		{
			if (enable)
			{
				this.mStreaminModeToggleButton.ImageName = this.mStreaminModeToggleButton.ImageName.Replace("_off", "_on");
				return;
			}
			this.mStreaminModeToggleButton.ImageName = this.mStreaminModeToggleButton.ImageName.Replace("_on", "_off");
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x0008BE80 File Offset: 0x0008A080
		private void AutoAlign_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
			CommonHandlers.ArrangeWindow();
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "AutoAlign", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x0008BEE8 File Offset: 0x0008A0E8
		private void PinToTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox.ImageName.Contains("_off"))
			{
				customPictureBox.ImageName = "toggle_on";
				this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
				this.ParentWindow.Topmost = true;
				ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "PinToTopOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				return;
			}
			customPictureBox.ImageName = "toggle_off";
			this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
			this.ParentWindow.Topmost = false;
			ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "PinToTopOff", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x0008BFDC File Offset: 0x0008A1DC
		private void Streaming_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox.ImageName.Contains("_off"))
			{
				customPictureBox.ImageName = "toggle_on";
				this.ParentWindow.mFrontendHandler.ToggleStreamingMode(true);
				ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "StreamingModeStart", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
			else
			{
				customPictureBox.ImageName = "toggle_off";
				this.ParentWindow.mFrontendHandler.ToggleStreamingMode(false);
				ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "StreamingModeStop", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
			this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x0000FD4A File Offset: 0x0000DF4A
		private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (sender != null)
			{
				(sender as TextBlock).SetTextblockTooltip();
			}
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x0008C0D0 File Offset: 0x0008A2D0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/preferencedropdowncontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x0008C100 File Offset: 0x0008A300
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 2:
				this.EngineSettingGrid = (Grid)target;
				return;
			case 3:
				this.mEngineSettingsButtonImage = (CustomPictureBox)target;
				return;
			case 4:
				this.mSettingsBtnNotification = (Ellipse)target;
				return;
			case 5:
				this.mPinToTopGrid = (Grid)target;
				return;
			case 6:
				this.mPinToTopImage = (CustomPictureBox)target;
				return;
			case 7:
				this.mPinToTopToggleButton = (CustomPictureBox)target;
				return;
			case 8:
				this.mStreamingMode = (Grid)target;
				return;
			case 9:
				this.mStreamingModeImage = (CustomPictureBox)target;
				return;
			case 10:
				this.mStreaminModeToggleButton = (CustomPictureBox)target;
				return;
			case 11:
				this.mMultiInstanceSectionTag = (Grid)target;
				return;
			case 12:
				this.mMultiInstanceSectionBorderLine = (Separator)target;
				return;
			case 13:
				this.mMultiInstanceSection = (Grid)target;
				return;
			case 14:
				this.mSyncGrid = (Grid)target;
				return;
			case 15:
				this.mSyncOperationsImage = (CustomPictureBox)target;
				return;
			case 16:
				this.mAutoAlignGrid = (Grid)target;
				return;
			case 17:
				this.mAutoAlignImage = (CustomPictureBox)target;
				return;
			case 18:
				this.mUpgradeBluestacksStatus = (Grid)target;
				return;
			case 19:
				this.mUpdateImage = (CustomPictureBox)target;
				return;
			case 20:
				this.mUpgradeBluestacksStatusTextBlock = (TextBlock)target;
				return;
			case 21:
				this.mUpdateDownloadProgressPercentage = (Label)target;
				return;
			case 22:
				this.mUpgradeToFullBlueStacks = (Grid)target;
				return;
			case 23:
				this.mUpgradeToFullTextBlock = (TextBlock)target;
				return;
			case 24:
				this.mLogoutButtonGrid = (Grid)target;
				return;
			case 25:
				this.mCustomiseSectionTag = (Grid)target;
				return;
			case 26:
				this.mCustomiseSectionBorderLine = (Separator)target;
				return;
			case 27:
				this.mCustomiseSection = (Grid)target;
				return;
			case 28:
				this.mChangeSkinGrid = (Grid)target;
				return;
			case 29:
				this.mChangeSkinImage = (CustomPictureBox)target;
				return;
			case 30:
				this.mChangeWallpaperGrid = (Grid)target;
				return;
			case 31:
				this.mChangeWallpaperImage = (CustomPictureBox)target;
				return;
			case 32:
				this.mHelpandsupportSectionTag = (Grid)target;
				return;
			case 33:
				this.mHelpAndSupportSectionBorderLine = (Separator)target;
				return;
			case 34:
				this.mHelpandsupportSection = (Grid)target;
				return;
			case 35:
				this.ReportProblemGrid = (Grid)target;
				return;
			case 36:
				this.mHelpCenterGrid = (Grid)target;
				return;
			case 37:
				this.mHelpCenterImage = (CustomPictureBox)target;
				return;
			case 38:
				this.mSpeedUpBstGrid = (Grid)target;
				return;
			case 39:
				this.mSpeedUpBstImage = (CustomPictureBox)target;
				return;
			case 40:
				this.mWallpaperPopup = (CustomPopUp)target;
				return;
			case 41:
				this.mWallpaperPopupGrid = (Grid)target;
				return;
			case 42:
				this.dummyGridForSize = (Grid)target;
				return;
			case 43:
				this.mWallpaperPopupBorder = (Border)target;
				return;
			case 44:
				this.mMaskBorder = (Border)target;
				return;
			case 45:
				this.mTitleText = (TextBlock)target;
				return;
			case 46:
				this.mBodyText = (TextBlock)target;
				return;
			case 47:
				this.RightArrow = (global::System.Windows.Shapes.Path)target;
				return;
			case 48:
				this.mChooseWallpaperPopup = (CustomPopUp)target;
				return;
			case 49:
				this.mChooseWallpaperPopupGrid = (Grid)target;
				return;
			case 50:
				this.dummyGridForSize2 = (Grid)target;
				return;
			case 51:
				this.mPopupGridBorder = (Border)target;
				return;
			case 52:
				this.mMaskBorder2 = (Border)target;
				return;
			case 53:
				this.mChooseNewGrid = (Grid)target;
				return;
			case 54:
				this.mSetDefaultGrid = (Grid)target;
				return;
			case 55:
				this.mRestoreDefaultText = (TextBlock)target;
				return;
			case 56:
				this.mRightArrow = (global::System.Windows.Shapes.Path)target;
				return;
			default:
				this._contentLoaded = false;
				return;
			}
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x0008C4C8 File Offset: 0x0008A6C8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				EventSetter eventSetter = new EventSetter();
				eventSetter.Event = FrameworkElement.SizeChangedEvent;
				eventSetter.Handler = new SizeChangedEventHandler(this.TextBlock_SizeChanged);
				((Style)target).Setters.Add(eventSetter);
			}
		}

		// Token: 0x04000E99 RID: 3737
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid EngineSettingGrid;

		// Token: 0x04000E9A RID: 3738
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mEngineSettingsButtonImage;

		// Token: 0x04000E9B RID: 3739
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Ellipse mSettingsBtnNotification;

		// Token: 0x04000E9C RID: 3740
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mPinToTopGrid;

		// Token: 0x04000E9D RID: 3741
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPinToTopImage;

		// Token: 0x04000E9E RID: 3742
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPinToTopToggleButton;

		// Token: 0x04000E9F RID: 3743
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mStreamingMode;

		// Token: 0x04000EA0 RID: 3744
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mStreamingModeImage;

		// Token: 0x04000EA1 RID: 3745
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mStreaminModeToggleButton;

		// Token: 0x04000EA2 RID: 3746
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMultiInstanceSectionTag;

		// Token: 0x04000EA3 RID: 3747
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Separator mMultiInstanceSectionBorderLine;

		// Token: 0x04000EA4 RID: 3748
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMultiInstanceSection;

		// Token: 0x04000EA5 RID: 3749
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSyncGrid;

		// Token: 0x04000EA6 RID: 3750
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSyncOperationsImage;

		// Token: 0x04000EA7 RID: 3751
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mAutoAlignGrid;

		// Token: 0x04000EA8 RID: 3752
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mAutoAlignImage;

		// Token: 0x04000EA9 RID: 3753
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mUpgradeBluestacksStatus;

		// Token: 0x04000EAA RID: 3754
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mUpdateImage;

		// Token: 0x04000EAB RID: 3755
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mUpgradeBluestacksStatusTextBlock;

		// Token: 0x04000EAC RID: 3756
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mUpdateDownloadProgressPercentage;

		// Token: 0x04000EAD RID: 3757
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mUpgradeToFullBlueStacks;

		// Token: 0x04000EAE RID: 3758
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mUpgradeToFullTextBlock;

		// Token: 0x04000EAF RID: 3759
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mLogoutButtonGrid;

		// Token: 0x04000EB0 RID: 3760
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mCustomiseSectionTag;

		// Token: 0x04000EB1 RID: 3761
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Separator mCustomiseSectionBorderLine;

		// Token: 0x04000EB2 RID: 3762
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mCustomiseSection;

		// Token: 0x04000EB3 RID: 3763
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mChangeSkinGrid;

		// Token: 0x04000EB4 RID: 3764
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mChangeSkinImage;

		// Token: 0x04000EB5 RID: 3765
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mChangeWallpaperGrid;

		// Token: 0x04000EB6 RID: 3766
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mChangeWallpaperImage;

		// Token: 0x04000EB7 RID: 3767
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mHelpandsupportSectionTag;

		// Token: 0x04000EB8 RID: 3768
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Separator mHelpAndSupportSectionBorderLine;

		// Token: 0x04000EB9 RID: 3769
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mHelpandsupportSection;

		// Token: 0x04000EBA RID: 3770
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid ReportProblemGrid;

		// Token: 0x04000EBB RID: 3771
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mHelpCenterGrid;

		// Token: 0x04000EBC RID: 3772
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mHelpCenterImage;

		// Token: 0x04000EBD RID: 3773
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSpeedUpBstGrid;

		// Token: 0x04000EBE RID: 3774
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSpeedUpBstImage;

		// Token: 0x04000EBF RID: 3775
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mWallpaperPopup;

		// Token: 0x04000EC0 RID: 3776
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mWallpaperPopupGrid;

		// Token: 0x04000EC1 RID: 3777
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid dummyGridForSize;

		// Token: 0x04000EC2 RID: 3778
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mWallpaperPopupBorder;

		// Token: 0x04000EC3 RID: 3779
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000EC4 RID: 3780
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTitleText;

		// Token: 0x04000EC5 RID: 3781
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mBodyText;

		// Token: 0x04000EC6 RID: 3782
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path RightArrow;

		// Token: 0x04000EC7 RID: 3783
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mChooseWallpaperPopup;

		// Token: 0x04000EC8 RID: 3784
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mChooseWallpaperPopupGrid;

		// Token: 0x04000EC9 RID: 3785
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid dummyGridForSize2;

		// Token: 0x04000ECA RID: 3786
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mPopupGridBorder;

		// Token: 0x04000ECB RID: 3787
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder2;

		// Token: 0x04000ECC RID: 3788
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mChooseNewGrid;

		// Token: 0x04000ECD RID: 3789
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSetDefaultGrid;

		// Token: 0x04000ECE RID: 3790
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mRestoreDefaultText;

		// Token: 0x04000ECF RID: 3791
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path mRightArrow;

		// Token: 0x04000ED0 RID: 3792
		private bool _contentLoaded;
	}
}
