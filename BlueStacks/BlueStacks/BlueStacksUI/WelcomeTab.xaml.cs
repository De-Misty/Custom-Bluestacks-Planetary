using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000289 RID: 649
	public partial class WelcomeTab : UserControl
	{
		// Token: 0x17000361 RID: 865
		// (get) Token: 0x0600178A RID: 6026 RVA: 0x0000FD5B File Offset: 0x0000DF5B
		// (set) Token: 0x0600178B RID: 6027 RVA: 0x0000FD63 File Offset: 0x0000DF63
		private BrowserControl mBrowser { get; set; }

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x0600178C RID: 6028 RVA: 0x0000FD6C File Offset: 0x0000DF6C
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

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x0600178D RID: 6029 RVA: 0x0000FD8D File Offset: 0x0000DF8D
		internal bool IsPromotionVisible
		{
			get
			{
				return this.mPromotionGrid.Visibility == Visibility.Visible;
			}
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x0000FD9D File Offset: 0x0000DF9D
		public WelcomeTab()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x0008C510 File Offset: 0x0008A710
		internal void Init()
		{
			HomeApp homeApp = null;
			if (FeatureManager.Instance.IsHtmlHome)
			{
				this.mBrowser = this.AddBrowser(this.ParentWindow.Utils.GetHtmlHomeUrl(), true);
			}
			else
			{
				homeApp = new HomeApp(this.ParentWindow);
				if (!this.mContentGrid.Children.Contains(homeApp))
				{
					this.mContentGrid.Children.Add(homeApp);
				}
			}
			this.mHomeAppManager = new HomeAppManager(homeApp, this.ParentWindow);
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				this.mHomeAppManager.ChangeHomeAppVisibility(Visibility.Hidden);
				this.mBackground.ImageName = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Promo\\boot_promo_0.png");
				this.mBackground.Visibility = Visibility.Visible;
			}
			if (FeatureManager.Instance.IsPromotionDisabled || Opt.Instance.hiddenBootMode)
			{
				this.RemovePromotionGrid();
				this.mHomeAppManager.ChangeHomeAppLoadingGridVisibility(Visibility.Visible);
			}
			this.BrowserFallbackUrlEvent();
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x0008C604 File Offset: 0x0008A804
		private void BrowserFallbackUrlEvent()
		{
			if (!string.IsNullOrEmpty(RegistryManager.Instance.OfflineHtmlHomeUrl))
			{
				this.mBrowser.UpdateUrlAndRefresh(RegistryManager.Instance.OfflineHtmlHomeUrl);
				Stats.SendCommonClientStatsAsync("html_home", "OfflineHtmlHome_loaded", this.ParentWindow.mVmName, "", "", "");
			}
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x0008C660 File Offset: 0x0008A860
		private BrowserControl AddBrowser(string url, bool isFallbackUrlRequired = false)
		{
			BrowserControl browserControl = new BrowserControl();
			if (isFallbackUrlRequired)
			{
				browserControl.BrowserFallbackUrlEvent += this.BrowserFallbackUrlEvent;
			}
			browserControl.InitBaseControl(url, 0f);
			browserControl.Visibility = Visibility.Visible;
			browserControl.ParentWindow = this.ParentWindow;
			this.mContentGrid.Children.Add(browserControl);
			return browserControl;
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x0000FDAB File Offset: 0x0000DFAB
		internal void ReInitHtmlHome()
		{
			this.mBrowser.UpdateUrlAndRefresh(this.ParentWindow.Utils.GetHtmlHomeUrl());
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x0000FDC8 File Offset: 0x0000DFC8
		internal void DisposeHtmHomeBrowser()
		{
			if (this.mBrowser != null)
			{
				this.mBrowser.DisposeBrowser();
				this.mContentGrid.Children.Remove(this.mBrowser);
				this.mBrowser = null;
			}
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x0000FDFA File Offset: 0x0000DFFA
		internal void RemovePromotionGrid()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mPromotionGrid.Visibility = Visibility.Hidden;
				this.mPromotionControl.Stop();
				HomeAppManager homeAppManager = this.mHomeAppManager;
				if (homeAppManager == null)
				{
					return;
				}
				homeAppManager.EnableSearchTextBox(true);
			}), new object[0]);
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x0008C6BC File Offset: 0x0008A8BC
		internal void OpenFrontendAppTabControl(string packageName, PlayStoreAction action)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (action == PlayStoreAction.OpenApp && this.ParentWindow.mAppHandler.IsAppInstalled(packageName) && !"com.android.vending".Equals(packageName, StringComparison.InvariantCultureIgnoreCase))
				{
					AppIconModel appIcon = this.mHomeAppManager.GetAppIcon(packageName);
					if (appIcon != null)
					{
						if (appIcon.AppIncompatType != AppIncompatType.None)
						{
							GrmHandler.HandleCompatibility(appIcon.PackageName, this.ParentWindow.mVmName);
							return;
						}
						this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
						return;
					}
				}
				else if (!string.IsNullOrEmpty(packageName))
				{
					AppIconModel appIcon2 = this.mHomeAppManager.GetAppIcon("com.android.vending");
					if (appIcon2 != null)
					{
						if (action == PlayStoreAction.OpenApp)
						{
							this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon2.AppName, appIcon2.PackageName, appIcon2.ActivityName, appIcon2.ImageName, false, true, false);
							this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
							this.ParentWindow.mAppHandler.LaunchPlayRequestAsync(packageName);
							return;
						}
						if (action == PlayStoreAction.SearchApp)
						{
							this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon2.AppName, appIcon2.PackageName, appIcon2.ActivityName, appIcon2.ImageName, false, true, false);
							this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
							this.ParentWindow.mAppHandler.SendSearchPlayRequestAsync(packageName);
						}
					}
				}
			}), new object[0]);
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x0000FE1A File Offset: 0x0000E01A
		internal void ReloadHomeTabIME()
		{
			BrowserControl mBrowser = this.mBrowser;
			if (mBrowser != null)
			{
				Browser cefBrowser = mBrowser.CefBrowser;
				if (cefBrowser != null)
				{
					cefBrowser.Focus();
				}
			}
			BrowserControl mBrowser2 = this.mBrowser;
			if (mBrowser2 == null)
			{
				return;
			}
			Browser cefBrowser2 = mBrowser2.CefBrowser;
			if (cefBrowser2 == null)
			{
				return;
			}
			cefBrowser2.ReloadIME();
		}

		// Token: 0x04000ED1 RID: 3793
		internal HomeAppManager mHomeAppManager;

		// Token: 0x04000ED3 RID: 3795
		private MainWindow mMainWindow;
	}
}
