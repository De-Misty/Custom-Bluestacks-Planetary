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
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200026E RID: 622
	public partial class HomeApp : UserControl
	{
		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060016AE RID: 5806 RVA: 0x0000F409 File Offset: 0x0000D609
		private MainWindow ParentWindow
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

		// Token: 0x060016AF RID: 5807 RVA: 0x000873B4 File Offset: 0x000855B4
		public HomeApp(MainWindow window)
		{
			this.sAppRecommendationsPool = new List<RecommendedApps>();
			this.defaultSearchBoxText = LocaleStrings.GetLocalizedString("STRING_SEARCH", "");
			this.InitializeComponent();
			this.mMainWindow = window;
			this.SetWallpaper();
			this.InstalledAppsDrawer = this.InstalledAppsDrawerScrollBar.Content as WrapPanel;
			this.mLoadingGrid.ProgressText = "STRING_LOADING_ENGINE";
			this.mLoadingGrid.Visibility = Visibility.Collapsed;
			if (!DesignerProperties.GetIsInDesignMode(this) && PromotionObject.Instance != null)
			{
				PromotionObject.BackgroundPromotionHandler = (EventHandler)Delegate.Combine(PromotionObject.BackgroundPromotionHandler, new EventHandler(this.HomeApp_BackgroundPromotionHandler));
			}
			if (!FeatureManager.Instance.IsMultiInstanceControlsGridVisible)
			{
				this.mMultiInstanceControlsGrid.Visibility = Visibility.Hidden;
			}
			if (!FeatureManager.Instance.IsAppSettingsAvailable)
			{
				this.mAppSettings.Visibility = Visibility.Hidden;
				this.mGridSeparator.Visibility = Visibility.Hidden;
			}
			this.searchHoverTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(700.0)
			};
			this.OpenSearchSuggestions();
			this.Mask.CornerRadius = new CornerRadius(0.0, this.searchTextBoxBorder.CornerRadius.TopRight, this.searchTextBoxBorder.CornerRadius.BottomRight, 0.0);
			this.GetSearchTextFromCloud();
			this.InstalledAppsDrawerScrollBar.Margin = new Thickness(20.0, 0.0, 1.0, 0.0);
			this.InstalledAppsDrawerScrollBar.VerticalAlignment = VerticalAlignment.Top;
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x0008754C File Offset: 0x0008574C
		internal void AddDockPanelIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
		{
			int num = 0;
			using (IEnumerator<AppIconUI> enumerator = this.mDockPanel.Children.OfType<AppIconUI>().GetEnumerator())
			{
				while (enumerator.MoveNext() && enumerator.Current.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
				{
					num++;
				}
			}
			AppIconUI newAppIconUI = this.GetNewAppIconUI(icon, downloadInstallApk);
			this.mDockPanel.Children.Insert(num, newAppIconUI);
			this.mDockPanel.Children.Remove(this.moreAppsIcon);
			this.mDockPanel.Children.Add(this.moreAppsIcon);
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x000875FC File Offset: 0x000857FC
		internal void AddMoreAppsDockPanelIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
		{
			int num = 0;
			using (IEnumerator<AppIconUI> enumerator = this.mMoreAppsDockPanel.Children.OfType<AppIconUI>().GetEnumerator())
			{
				while (enumerator.MoveNext() && enumerator.Current.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
				{
					num++;
				}
			}
			AppIconUI newAppIconUI = this.GetNewAppIconUI(icon, downloadInstallApk);
			this.mMoreAppsDockPanel.Children.Insert(num, newAppIconUI);
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x00087680 File Offset: 0x00085880
		internal void AddInstallDrawerIcon(AppIconModel icon, DownloadInstallApk downloadInstallApk = null)
		{
			int num = 0;
			using (IEnumerator<AppIconUI> enumerator = this.InstalledAppsDrawer.Children.OfType<AppIconUI>().GetEnumerator())
			{
				while (enumerator.MoveNext() && enumerator.Current.mAppIconModel.MyAppPriority <= icon.MyAppPriority)
				{
					num++;
				}
			}
			AppIconUI newAppIconUI = this.GetNewAppIconUI(icon, downloadInstallApk);
			this.InstalledAppsDrawer.Children.Insert(num, newAppIconUI);
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x00087704 File Offset: 0x00085904
		internal void RemoveAppIconFromUI(AppIconModel appIcon)
		{
			this.InstalledAppsDrawer.Children.Remove((from AppIconUI s in this.InstalledAppsDrawer.Children
				where s.mAppIconModel.PackageName == appIcon.PackageName
				select s).FirstOrDefault<AppIconUI>());
			this.mDockPanel.Children.Remove((from AppIconUI s in this.mDockPanel.Children
				where s.mAppIconModel.PackageName == appIcon.PackageName
				select s).FirstOrDefault<AppIconUI>());
			this.mMoreAppsDockPanel.Children.Remove((from AppIconUI s in this.mMoreAppsDockPanel.Children
				where s.mAppIconModel.PackageName == appIcon.PackageName
				select s).FirstOrDefault<AppIconUI>());
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0000F42A File Offset: 0x0000D62A
		internal void InitUIAppPromotionEvents()
		{
			if (PromotionObject.Instance != null)
			{
				PromotionObject.AppRecommendationHandler = (Action<bool>)Delegate.Combine(PromotionObject.AppRecommendationHandler, new Action<bool>(this.ShowAppRecommendations));
			}
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0000F453 File Offset: 0x0000D653
		private void HomeApp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource == this.InstalledAppsDrawerScrollBar)
			{
				this.ParentWindow.StaticComponents.ShowUninstallButtons(false);
			}
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x000877C0 File Offset: 0x000859C0
		internal void InitMoreAppsIcon()
		{
			AppIconModel appIconModel = new AppIconModel();
			appIconModel.AppName = LocaleStrings.GetLocalizedString("STRING_MORE_APPS", "");
			appIconModel.ImageName = "moreapps";
			appIconModel.AddToDock(50.0, 50.0);
			this.moreAppsIcon = new AppIconUI(this.ParentWindow, appIconModel);
			this.moreAppsIcon.Click += this.MoreAppsIcon_Click;
			this.mDockPanel.Children.Add(this.moreAppsIcon);
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x0008784C File Offset: 0x00085A4C
		private void MoreAppsIcon_Click(object sender, RoutedEventArgs e)
		{
			AppIconUI appIconUI = sender as AppIconUI;
			this.mDockAppIconToolTipPopup.IsOpen = false;
			this.mMoreAppsDockPopup.PlacementTarget = appIconUI.mAppImage;
			this.mMoreAppsDockPopup.IsOpen = true;
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x0000F474 File Offset: 0x0000D674
		private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mMoreAppsDockPopup.IsOpen = false;
			this.mMoreAppsDockPopup.StaysOpen = false;
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x0008788C File Offset: 0x00085A8C
		private void InstalledAppsDrawerScrollBar_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			double verticalOffset = this.InstalledAppsDrawerScrollBar.VerticalOffset;
			if (this.InstalledAppsDrawerScrollBar.ComputedVerticalScrollBarVisibility != Visibility.Visible)
			{
				this.InstalledAppsDrawerScrollBar.OpacityMask = null;
				return;
			}
			if (verticalOffset <= 1.0)
			{
				this.InstalledAppsDrawerScrollBar.OpacityMask = BluestacksUIColor.mTopOpacityMask;
				return;
			}
			if (verticalOffset == this.InstalledAppsDrawerScrollBar.ScrollableHeight)
			{
				this.InstalledAppsDrawerScrollBar.OpacityMask = BluestacksUIColor.mBottomOpacityMask;
				return;
			}
			this.InstalledAppsDrawerScrollBar.OpacityMask = BluestacksUIColor.mScrolledOpacityMask;
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x0000F48E File Offset: 0x0000D68E
		private void mCloseAppSuggPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mSuggestedAppPopUp.IsOpen = false;
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x0008790C File Offset: 0x00085B0C
		private AppIconUI GetNewAppIconUI(AppIconModel iconModel, DownloadInstallApk downloadInstallApk = null)
		{
			AppIconUI appIconUI = new AppIconUI(this.ParentWindow, iconModel);
			if (downloadInstallApk != null)
			{
				appIconUI.InitAppDownloader(downloadInstallApk);
			}
			return appIconUI;
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060016BC RID: 5820 RVA: 0x0000F49C File Offset: 0x0000D69C
		// (set) Token: 0x060016BD RID: 5821 RVA: 0x0000F4A4 File Offset: 0x0000D6A4
		internal bool SideHtmlBrowserInited { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060016BE RID: 5822 RVA: 0x0000F4AD File Offset: 0x0000D6AD
		// (set) Token: 0x060016BF RID: 5823 RVA: 0x0000F4B5 File Offset: 0x0000D6B5
		internal BrowserControl SideHtmlBrowser { get; set; }

		// Token: 0x060016C0 RID: 5824 RVA: 0x00087934 File Offset: 0x00085B34
		internal void InitiateSideHtmlBrowser()
		{
			object obj = HomeApp.syncRoot;
			lock (obj)
			{
				if (FeatureManager.Instance.IsHtmlSideBar && !this.SideHtmlBrowserInited && CefHelper.CefInited && !RegistryManager.Instance.IsPremium)
				{
					this.CreateSideHtmlBrowserControl();
				}
			}
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x0000F4BE File Offset: 0x0000D6BE
		private void CreateSideHtmlBrowserControl()
		{
			this.SideHtmlBrowserInited = true;
			base.Dispatcher.Invoke(new Action(delegate
			{
				BrowserControl browserControl = new BrowserControl(BlueStacksUIUtils.GetHtmlSidePanelUrl())
				{
					Visibility = Visibility.Visible
				};
				CustomPictureBox customPictureBox = new CustomPictureBox
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Height = 30.0,
					Width = 30.0,
					ImageName = "loader",
					IsImageToBeRotated = true
				};
				this.mAppRecommendationsGrid.Children.Add(browserControl);
				this.mAppRecommendationsGrid.Children.Add(customPictureBox);
				browserControl.CreateNewBrowser();
				this.SideHtmlBrowser = browserControl;
			}), new object[0]);
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00087994 File Offset: 0x00085B94
		private void ChangeSideRecommendationsVisibility(bool isAppRecommendationsVisible, bool isSearchBarVisible)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (!isAppRecommendationsVisible)
				{
					if (!isAppRecommendationsVisible)
					{
						this.mAppRecommendationsGrid.Visibility = Visibility.Collapsed;
						this.InstalledAppsDrawerScrollBar.SetValue(Grid.ColumnSpanProperty, 2);
						this.mMultiInstanceControlsGrid.SetValue(Grid.ColumnSpanProperty, 2);
						if (isSearchBarVisible)
						{
							this.mSearchGrid.Visibility = Visibility.Visible;
							this.mSearchGrid.Margin = new Thickness(0.0, 20.0, 20.0, 0.0);
							this.mSearchGrid.Width = 350.0;
							this.mIsShowSearchRecommendations = true;
							this.mSearchRecommendationBorder.Margin = new Thickness(0.0, 59.0, 20.0, 0.0);
							this.mSearchRecommendationBorder.Width = 350.0;
							return;
						}
						this.mSearchGrid.Visibility = Visibility.Collapsed;
					}
					return;
				}
				this.mAppRecommendationsGrid.Visibility = Visibility.Visible;
				this.InstalledAppsDrawerScrollBar.SetValue(Grid.ColumnSpanProperty, 1);
				this.mMultiInstanceControlsGrid.SetValue(Grid.ColumnSpanProperty, 1);
				if (isSearchBarVisible)
				{
					this.mDiscoverApps.Visibility = Visibility.Visible;
					this.appRecomScrollViewer.Visibility = Visibility.Visible;
					this.mSearchGrid.Visibility = Visibility.Visible;
					this.mSearchGrid.Margin = new Thickness(20.0, 54.0, 20.0, 0.0);
					this.mSearchGrid.Width = 240.0;
					this.mIsShowSearchRecommendations = false;
					this.mSearchRecommendationBorder.Margin = new Thickness(20.0, 88.0, 20.0, 0.0);
					this.mSearchRecommendationBorder.Width = 240.0;
					return;
				}
				this.mSearchGrid.Visibility = Visibility.Collapsed;
				this.mDiscoverApps.Visibility = Visibility.Collapsed;
				this.appRecomScrollViewer.Visibility = Visibility.Collapsed;
				this.mAppRecommendationsGrid.Width = 345.0;
			}), new object[0]);
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x000879DC File Offset: 0x00085BDC
		private void ShowAppRecommendations(bool isContentReloadRequired)
		{
			try
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					if ((this.ParentWindow != null && this.ParentWindow.ActualWidth <= 700.0) || FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsSearchBarVisible || RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
					{
						if (this.mCurrentSidePanelVisibility != null)
						{
							SidePanelVisibility? sidePanelVisibility = this.mCurrentSidePanelVisibility;
							SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.BothSearchBarAndSidepanelHidden;
							if ((sidePanelVisibility.GetValueOrDefault() == sidePanelVisibility2) & (sidePanelVisibility != null))
							{
								return;
							}
						}
						this.ChangeSideRecommendationsVisibility(false, false);
						this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.BothSearchBarAndSidepanelHidden);
						return;
					}
					if (!RegistryManager.Instance.IsPremium)
					{
						if (!FeatureManager.Instance.IsHtmlSideBar)
						{
							if (!FeatureManager.Instance.IsShowAppRecommendations)
							{
								goto IL_00EA;
							}
							AppRecommendationSection appRecommendations = PromotionObject.Instance.AppRecommendations;
							if (appRecommendations != null && appRecommendations.AppSuggestions.Count == 0)
							{
								goto IL_00EA;
							}
						}
						if (!FeatureManager.Instance.IsHtmlSideBar)
						{
							if (isContentReloadRequired || !this.mIsSidePanelContentLoadedOnce)
							{
								this.mAppRecommendationSectionsPanel.Children.Clear();
								AppRecommendationSection appRecommendations2 = PromotionObject.Instance.AppRecommendations;
								RecommendedAppsSection recommendedAppsSection = new RecommendedAppsSection(appRecommendations2.AppSuggestionHeader);
								recommendedAppsSection.AddSuggestedApps(this.ParentWindow, appRecommendations2.AppSuggestions, appRecommendations2.ClientShowCount);
								if (recommendedAppsSection.mAppRecommendationsPanel.Children.Count != 0)
								{
									this.mAppRecommendationSectionsPanel.Children.Add(recommendedAppsSection);
									this.mAppRecommendationSectionsPanel.Visibility = Visibility.Visible;
									this.mAppRecommendationsGenericMessages.Visibility = Visibility.Collapsed;
									this.SendAppRecommendationsImpressionStats();
								}
								this.mIsSidePanelContentLoadedOnce = true;
							}
							if (this.mCurrentSidePanelVisibility != null)
							{
								SidePanelVisibility? sidePanelVisibility = this.mCurrentSidePanelVisibility;
								SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.BothSearchBarAndSidepanelVisible;
								if ((sidePanelVisibility.GetValueOrDefault() == sidePanelVisibility2) & (sidePanelVisibility != null))
								{
									return;
								}
							}
							this.ChangeSideRecommendationsVisibility(true, true);
							this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.BothSearchBarAndSidepanelVisible);
							return;
						}
						if (!this.SideHtmlBrowserInited)
						{
							this.InitiateSideHtmlBrowser();
						}
						if (!RegistryManager.Instance.IsPremium)
						{
							if (this.mCurrentSidePanelVisibility != null)
							{
								SidePanelVisibility? sidePanelVisibility = this.mCurrentSidePanelVisibility;
								SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.OnlySidepanelVisible;
								if ((sidePanelVisibility.GetValueOrDefault() == sidePanelVisibility2) & (sidePanelVisibility != null))
								{
									return;
								}
							}
							this.ChangeSideRecommendationsVisibility(true, false);
							this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.OnlySidepanelVisible);
							return;
						}
						return;
					}
					IL_00EA:
					if (this.mCurrentSidePanelVisibility != null)
					{
						SidePanelVisibility? sidePanelVisibility = this.mCurrentSidePanelVisibility;
						SidePanelVisibility sidePanelVisibility2 = SidePanelVisibility.OnlySearchBarVisible;
						if ((sidePanelVisibility.GetValueOrDefault() == sidePanelVisibility2) & (sidePanelVisibility != null))
						{
							return;
						}
					}
					this.ChangeSideRecommendationsVisibility(false, true);
					this.mCurrentSidePanelVisibility = new SidePanelVisibility?(SidePanelVisibility.OnlySearchBarVisible);
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in showing app recommendations, " + ex.ToString());
			}
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x00087A48 File Offset: 0x00085C48
		internal void UpdateRecommendedAppsInstallStatus(string package)
		{
			if (this.mAppRecommendationSectionsPanel.Children.Count > 0)
			{
				int num = -1;
				RecommendedAppsSection recommendedAppsSection = this.mAppRecommendationSectionsPanel.Children[0] as RecommendedAppsSection;
				RecommendedApps recommendedApps = null;
				for (int i = 0; i < recommendedAppsSection.mAppRecommendationsPanel.Children.Count; i++)
				{
					RecommendedApps recommendedApps2 = recommendedAppsSection.mAppRecommendationsPanel.Children[i] as RecommendedApps;
					if (recommendedApps2.AppRecomendation.AppPackage.Equals(package, StringComparison.InvariantCultureIgnoreCase))
					{
						num = i;
						recommendedApps = recommendedApps2;
						break;
					}
				}
				if (num != -1)
				{
					recommendedAppsSection.mAppRecommendationsPanel.Children.RemoveAt(num);
					if (this.sAppRecommendationsPool.Count > 0)
					{
						int num2 = 1;
						for (int j = 0; j < this.sAppRecommendationsPool.Count; j++)
						{
							RecommendedApps recommendedApps3 = this.sAppRecommendationsPool[j];
							if (!this.ParentWindow.mAppHandler.IsAppInstalled(recommendedApps3.AppRecomendation.ExtraPayload["click_action_packagename"]))
							{
								recommendedApps3.RecommendedAppPosition = recommendedApps.RecommendedAppPosition;
								recommendedAppsSection.mAppRecommendationsPanel.Children.Insert(num, recommendedApps3);
								JArray jarray = new JArray();
								JObject jobject = new JObject
								{
									{
										"app_loc",
										(recommendedApps3.AppRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay"
									},
									{
										"app_pkg",
										recommendedApps3.AppRecomendation.ExtraPayload["click_action_packagename"]
									},
									{
										"is_installed",
										this.ParentWindow.mAppHandler.IsAppInstalled(recommendedApps3.AppRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false"
									},
									{
										"app_position",
										recommendedApps3.RecommendedAppPosition.ToString(CultureInfo.InvariantCulture)
									},
									{
										"app_rank",
										recommendedApps3.RecommendedAppRank.ToString(CultureInfo.InvariantCulture)
									}
								};
								jarray.Add(jobject);
								ClientStats.SendFrontendClickStats("apps_recommendation", "impression", null, null, null, null, null, jarray.ToString(Formatting.None, new JsonConverter[0]));
								break;
							}
							num2++;
						}
						this.sAppRecommendationsPool.RemoveRange(0, num2);
					}
					if (recommendedAppsSection.mAppRecommendationsPanel.Children.Count == 0)
					{
						this.mAppRecommendationSectionsPanel.Children.Remove(recommendedAppsSection);
					}
					if (this.mAppRecommendationSectionsPanel.Children.Count == 0)
					{
						this.mAppRecommendationSectionsPanel.Visibility = Visibility.Collapsed;
						this.mAppRecommendationsGenericMessages.Visibility = Visibility.Visible;
					}
				}
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00087D0C File Offset: 0x00085F0C
		private void SendAppRecommendationsImpressionStats()
		{
			JArray jarray = new JArray();
			RecommendedAppsSection recommendedAppsSection = this.mAppRecommendationSectionsPanel.Children[0] as RecommendedAppsSection;
			for (int i = 0; i < recommendedAppsSection.mAppRecommendationsPanel.Children.Count; i++)
			{
				RecommendedApps recommendedApps = recommendedAppsSection.mAppRecommendationsPanel.Children[i] as RecommendedApps;
				JObject jobject = new JObject
				{
					{
						"app_loc",
						(recommendedApps.AppRecomendation.ExtraPayload["click_generic_action"] == "InstallCDN") ? "cdn" : "gplay"
					},
					{
						"app_pkg",
						recommendedApps.AppRecomendation.ExtraPayload["click_action_packagename"]
					},
					{
						"is_installed",
						this.ParentWindow.mAppHandler.IsAppInstalled(recommendedApps.AppRecomendation.ExtraPayload["click_action_packagename"]) ? "true" : "false"
					},
					{
						"app_position",
						recommendedApps.RecommendedAppPosition.ToString(CultureInfo.InvariantCulture)
					},
					{
						"app_rank",
						recommendedApps.RecommendedAppRank.ToString(CultureInfo.InvariantCulture)
					}
				};
				jarray.Add(jobject);
			}
			ClientStats.SendFrontendClickStats("apps_recommendation", "impression", null, null, null, null, null, jarray.ToString(Formatting.None, new JsonConverter[0]));
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x0000F4E5 File Offset: 0x0000D6E5
		private void Search_MouseEnter(object sender, MouseEventArgs e)
		{
			this.searchHoverTimer.Start();
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x0000F4F2 File Offset: 0x0000D6F2
		private void search_MouseLeave(object sender, MouseEventArgs e)
		{
			this.searchHoverTimer.Stop();
			if (!this.mSearchTextBox.IsFocused && !this.mSearchRecommendationBorder.IsMouseOver && !this.mSearchGrid.IsMouseOver)
			{
				this.HideSearchSuggestions();
			}
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x00087E8C File Offset: 0x0008608C
		private void OpenSearchSuggestions()
		{
			try
			{
				if (this.mSearchRecommendationBorder.Visibility != Visibility.Visible && (string.IsNullOrEmpty(this.mSearchTextBox.Text) || this.mSearchTextBox.Text == this.defaultSearchBoxText) && PromotionObject.Instance.SearchRecommendations.Count > 0 && this.mIsShowSearchRecommendations)
				{
					this.searchRecomItems.Children.Clear();
					Separator separator = new Separator
					{
						Margin = new Thickness(0.0),
						Style = (base.FindResource(ToolBar.SeparatorStyleKey) as Style)
					};
					BlueStacksUIBinding.BindColor(separator, Control.BackgroundProperty, "VerticalSeparator");
					this.searchRecomItems.Children.Add(separator);
					Label label = new Label
					{
						Content = LocaleStrings.GetLocalizedString("STRING_MOST_SEARCHED_APPS", "")
					};
					BlueStacksUIBinding.BindColor(label, Control.ForegroundProperty, "SearchGridForegroundColor");
					label.FontSize = 14.0;
					label.Padding = new Thickness(10.0, 5.0, 5.0, 5.0);
					this.searchRecomItems.Children.Add(label);
					foreach (KeyValuePair<string, SearchRecommendation> keyValuePair in PromotionObject.Instance.SearchRecommendations)
					{
						RecommendedAppItem recommendedAppItem = new RecommendedAppItem();
						recommendedAppItem.Populate(this.ParentWindow, keyValuePair.Value);
						recommendedAppItem.Padding = new Thickness(5.0, 0.0, 0.0, 0.0);
						this.searchRecomItems.Children.Add(recommendedAppItem);
					}
					this.mSearchRecommendationBorder.CornerRadius = new CornerRadius(0.0, 0.0, this.searchTextBoxBorder.CornerRadius.BottomRight, this.searchTextBoxBorder.CornerRadius.BottomLeft);
					this.Mask.CornerRadius = new CornerRadius(0.0, this.searchTextBoxBorder.CornerRadius.TopRight, 0.0, 0.0);
					this.searchTextBoxBorder.CornerRadius = new CornerRadius(this.searchTextBoxBorder.CornerRadius.TopLeft, this.searchTextBoxBorder.CornerRadius.TopRight, 0.0, 0.0);
					this.mSearchRecommendationBorder.Visibility = Visibility.Visible;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception when trying to open search recommendations. " + ex.ToString());
			}
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00088194 File Offset: 0x00086394
		private void HideSearchSuggestions()
		{
			if (this.mSearchRecommendationBorder.Visibility == Visibility.Visible)
			{
				this.searchTextBoxBorder.CornerRadius = new CornerRadius(this.searchTextBoxBorder.CornerRadius.TopLeft, this.searchTextBoxBorder.CornerRadius.TopRight, this.mSearchRecommendationBorder.CornerRadius.BottomRight, this.mSearchRecommendationBorder.CornerRadius.BottomLeft);
				this.Mask.CornerRadius = new CornerRadius(0.0, this.searchTextBoxBorder.CornerRadius.TopRight, this.mSearchRecommendationBorder.CornerRadius.BottomRight, 0.0);
				this.mSearchRecommendationBorder.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x00088264 File Offset: 0x00086464
		private void SearchTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (this.ParentWindow.mWelcomeTab.IsPromotionVisible)
			{
				return;
			}
			if (this.mSearchTextBox.Text == this.defaultSearchBoxText)
			{
				this.mSearchTextBox.Text = string.Empty;
			}
			this.OpenSearchSuggestions();
			BlueStacksUIBinding.BindColor(this.mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundFocusedColor");
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x000882C8 File Offset: 0x000864C8
		private void SearchTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (!this.mSearchRecommendationBorder.IsMouseOver)
			{
				this.HideSearchSuggestions();
			}
			if (this.ParentWindow.mWelcomeTab.IsPromotionVisible)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.mSearchTextBox.Text))
			{
				this.mSearchTextBox.Text = this.defaultSearchBoxText;
			}
			if (string.Equals(this.mSearchTextBox.Text, this.defaultSearchBoxText, StringComparison.InvariantCulture))
			{
				BlueStacksUIBinding.BindColor(this.mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundColor");
				return;
			}
			BlueStacksUIBinding.BindColor(this.mSearchTextBox, Control.ForegroundProperty, "SearchGridForegroundFocusedColor");
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0000F52C File Offset: 0x0000D72C
		private void CustomPictureBox_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.SearchApp();
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x0000F534 File Offset: 0x0000D734
		private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (this.mSearchRecommendationBorder.Visibility == Visibility.Visible)
			{
				this.HideSearchSuggestions();
			}
			if (e.Key == Key.Return)
			{
				this.SearchApp();
			}
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x00088364 File Offset: 0x00086564
		private void SearchApp()
		{
			if (this.ParentWindow.mWelcomeTab.IsPromotionVisible)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.mSearchTextBox.Text))
			{
				this.ParentWindow.mCommonHandler.SearchAppCenter(this.mSearchTextBox.Text);
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0000F558 File Offset: 0x0000D758
		private void GetSearchTextFromCloud()
		{
			new Thread(delegate
			{
				try
				{
					this.defaultSearchBoxText = LocaleStrings.GetLocalizedString("STRING_SEARCH", "");
					string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/app_center_searchdefaultquery");
					Logger.Debug("url for search api :" + urlWithParams);
					string text = BstHttpClient.Get(urlWithParams, null, false, string.Empty, 0, 1, 0, false, "bgp64");
					Logger.Debug("result for app_center_searchdefaultquery : " + text);
					JObject jobject = JObject.Parse(text);
					if ((bool)jobject["success"])
					{
						string text2 = jobject["result"].ToString().Trim();
						if (!string.IsNullOrEmpty(text2))
						{
							this.defaultSearchBoxText = text2;
						}
						Logger.Debug("response from search text cloud api :" + text2);
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("Failed to fetch text from cloud... Err : " + ex.ToString());
				}
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mSearchTextBox.Text = this.defaultSearchBoxText;
				}), new object[0]);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0000F577 File Offset: 0x0000D777
		private void SetWallpaper()
		{
			if (File.Exists(HomeAppManager.BackgroundImagePath))
			{
				this.mBackgroundImage.IsFullImagePath = true;
				this.mBackgroundImage.ImageName = HomeAppManager.BackgroundImagePath;
			}
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x0000F5A1 File Offset: 0x0000D7A1
		private void HomeApp_BackgroundPromotionHandler(object sender, EventArgs e)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (string.IsNullOrEmpty(PromotionObject.Instance.BackgroundPromotionImagePath))
				{
					this.SetWallpaper();
					return;
				}
				this.mBackgroundImage.IsFullImagePath = true;
				this.mBackgroundImage.ImageName = PromotionObject.Instance.BackgroundPromotionImagePath;
			}), new object[0]);
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x000883B4 File Offset: 0x000865B4
		internal void RestoreWallpaperImage()
		{
			this.mBackgroundImage.IsFullImagePath = false;
			this.mBackgroundImage.ImageName = "fancybg.jpg";
			try
			{
				if (File.Exists(HomeAppManager.BackgroundImagePath))
				{
					File.Delete(HomeAppManager.BackgroundImagePath);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in deletion of image:" + ex.ToString());
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x0000F5C1 File Offset: 0x0000D7C1
		internal void ApplyWallpaperImage()
		{
			this.mBackgroundImage.ImageName = HomeAppManager.BackgroundImagePath;
			this.mBackgroundImage.ReloadImages();
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x0000F5DE File Offset: 0x0000D7DE
		private void mAppSettingsPopup_Opened(object sender, EventArgs e)
		{
			this.mAppSettings.IsEnabled = false;
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x0000F5EC File Offset: 0x0000D7EC
		private void mAppSettingsPopup_Closed(object sender, EventArgs e)
		{
			this.mAppSettings.IsEnabled = true;
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x0000F5FA File Offset: 0x0000D7FA
		private void mInstallApkGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			new DownloadInstallApk(this.ParentWindow).ChooseAndInstallApk();
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x0000F60C File Offset: 0x0000D80C
		private void mDeleteAppsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.StaticComponents.ShowUninstallButtons(true);
			this.mAppSettingsPopup.IsOpen = false;
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x00007C8F File Offset: 0x00005E8F
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x00006A61 File Offset: 0x00004C61
		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			(sender as Grid).Background = Brushes.Transparent;
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x0000F62B File Offset: 0x0000D82B
		private void mAppSettings_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mAppSettingsPopup.IsOpen = true;
		}

		// Token: 0x04000DDE RID: 3550
		private WrapPanel InstalledAppsDrawer;

		// Token: 0x04000DDF RID: 3551
		private static object syncRoot = new object();

		// Token: 0x04000DE0 RID: 3552
		private AppIconUI moreAppsIcon;

		// Token: 0x04000DE1 RID: 3553
		private MainWindow mMainWindow;

		// Token: 0x04000DE4 RID: 3556
		private SidePanelVisibility? mCurrentSidePanelVisibility;

		// Token: 0x04000DE5 RID: 3557
		private bool mIsSidePanelContentLoadedOnce;

		// Token: 0x04000DE6 RID: 3558
		internal List<RecommendedApps> sAppRecommendationsPool;

		// Token: 0x04000DE7 RID: 3559
		private DispatcherTimer searchHoverTimer;

		// Token: 0x04000DE8 RID: 3560
		private string defaultSearchBoxText;

		// Token: 0x04000DE9 RID: 3561
		private bool mIsShowSearchRecommendations;
	}
}
