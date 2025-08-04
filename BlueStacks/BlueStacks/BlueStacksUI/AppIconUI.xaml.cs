using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200002B RID: 43
	public partial class AppIconUI : Button
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060002FA RID: 762 RVA: 0x00003F6B File Offset: 0x0000216B
		// (set) Token: 0x060002FB RID: 763 RVA: 0x00003F73 File Offset: 0x00002173
		internal AppIconModel mAppIconModel { get; private set; }

		// Token: 0x060002FC RID: 764 RVA: 0x00014108 File Offset: 0x00012308
		public AppIconUI(MainWindow window, AppIconModel model)
		{
			this.ParentWindow = window;
			this.mAppIconModel = model;
			base.DataContext = this.mAppIconModel;
			this.InitializeComponent();
			if (this.mAppIconModel != null)
			{
				this.SetAppIconLocation(this.mAppIconModel.IconLocation, this.mAppIconModel.IconHeight, this.mAppIconModel.IconWidth);
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00003F7C File Offset: 0x0000217C
		internal void InitAppDownloader(DownloadInstallApk downloadInstallApk = null)
		{
			this.mDownloader = downloadInstallApk;
			this.mAppIconModel.mAppDownloadingEvent += this.DownloadingApp;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0001416C File Offset: 0x0001236C
		private void Button_Loaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				base.Loaded -= this.Button_Loaded;
				this.ParentWindow.StaticComponents.ShowAllUninstallButtons += this.Button_MouseHoldAction;
				this.ParentWindow.StaticComponents.HideAllUninstallButtons += this.AppIcon_HideUninstallButton;
				if (this.mAppIconModel.IsGifIcon)
				{
					this.ParentWindow.StaticComponents.PlayAllGifs += this.GifAppIconPlay;
					this.ParentWindow.StaticComponents.PauseAllGifs += this.GifAppIconPause;
				}
				this.mAppIconModel.IsGamepadConnected = this.ParentWindow.IsGamepadConnected;
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0001422C File Offset: 0x0001242C
		private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.threadShowingUninstallButton == null)
			{
				this.threadShowingUninstallButton = new Thread(delegate
				{
					Thread.Sleep(1000);
					if (this.threadShowingUninstallButton != null)
					{
						this.Dispatcher.Invoke(new Action(delegate
						{
							(sender as UIElement).ReleaseMouseCapture();
							this.ParentWindow.StaticComponents.ShowUninstallButtons(true);
							this.threadShowingUninstallButton = null;
						}), new object[0]);
					}
				})
				{
					IsBackground = true
				};
				this.threadShowingUninstallButton.Start();
			}
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00003F9C File Offset: 0x0000219C
		private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.threadShowingUninstallButton != null && this.threadShowingUninstallButton.IsAlive)
			{
				this.threadShowingUninstallButton = null;
			}
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00003FBA File Offset: 0x000021BA
		private void Button_MouseHoldAction(object sender, EventArgs e)
		{
			this.ShowAppUninstallButton(true);
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00003FC3 File Offset: 0x000021C3
		private void AppIcon_HideUninstallButton(object sender, EventArgs e)
		{
			this.ShowAppUninstallButton(false);
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00014280 File Offset: 0x00012480
		private void ShowAppUninstallButton(bool isShow)
		{
			if (isShow && this.mAppIconModel.mIsAppRemovable && (!this.mAppIconModel.IsInstalling || this.mAppIconModel.IsInstallingFailed))
			{
				this.mUnInstallTabButton.Visibility = Visibility.Visible;
				return;
			}
			this.mUnInstallTabButton.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x000142D0 File Offset: 0x000124D0
		private void GamepadIcon_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mIconText.Text = (this.mAppIconModel.IsGamepadConnected ? LocaleStrings.GetLocalizedString("STRING_GAMEPAD_CONNECTED", "") : LocaleStrings.GetLocalizedString("STRING_GAMEPAD_DISCONNECTED", ""));
			this.mAppIconModel.AppNameTooltip = null;
			this.mGamePadToolTipPopup.PlacementTarget = this.mGamepadIcon;
			this.mGamePadToolTipPopup.IsOpen = true;
			this.mGamePadToolTipPopup.StaysOpen = true;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00003FCC File Offset: 0x000021CC
		private void GamepadIcon_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mGamePadToolTipPopup.IsOpen = false;
			this.mAppIconModel.AppNameTooltip = this.mAppIconModel.AppName;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0001434C File Offset: 0x0001254C
		private void Button_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.ParentWindow.StaticComponents.IsDeleteButtonVisible)
			{
				this.ShowAppUninstallButton(false);
			}
			this.mRetryGrid.Visibility = Visibility.Hidden;
			ScaleTransform scaleTransform = new ScaleTransform(1.0, 1.0);
			this.mAppImage.RenderTransformOrigin = new Point(0.0, 0.0);
			this.mAppImage.RenderTransform = scaleTransform;
			this.AppNameTextBox.RenderTransformOrigin = new Point(0.0, 0.0);
			this.AppNameTextBox.RenderTransform = scaleTransform;
			DropShadowEffect dropShadowEffect = new DropShadowEffect
			{
				Color = Colors.Black,
				Direction = 270.0,
				ShadowDepth = 1.0,
				BlurRadius = 6.0,
				Opacity = 0.3
			};
			this.mAppImageBorder.Effect = dropShadowEffect;
			if (this.mAppIconModel.IsAppSuggestionActive)
			{
				this.ParentWindow.mWelcomeTab.mHomeAppManager.CloseAppSuggestionPopup();
			}
			if (this.mAppIconModel.IconLocation == AppIconLocation.Dock || this.mAppIconModel.IconLocation == AppIconLocation.Moreapps)
			{
				this.ParentWindow.mWelcomeTab.mHomeAppManager.ShowDockIconTooltip(this, false);
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x000144A0 File Offset: 0x000126A0
		private void Button_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.mAppIconModel.IsDownloading)
			{
				this.ShowAppUninstallButton(true);
			}
			if (this.mAppIconModel.IsDownLoadingFailed || this.mAppIconModel.IsInstallingFailed)
			{
				this.mRetryGrid.Visibility = Visibility.Visible;
			}
			if (this.mBusyGrid.Visibility == Visibility.Visible)
			{
				return;
			}
			DropShadowEffect dropShadowEffect = new DropShadowEffect();
			BlueStacksUIBinding.BindColor(dropShadowEffect, DropShadowEffect.ColorProperty, "AppIconDropShadowBrush");
			dropShadowEffect.Direction = 270.0;
			dropShadowEffect.ShadowDepth = 1.0;
			dropShadowEffect.BlurRadius = 20.0;
			dropShadowEffect.Opacity = 1.0;
			this.mAppImageBorder.Effect = dropShadowEffect;
			ScaleTransform scaleTransform = new ScaleTransform(1.02, 1.02);
			this.mAppImage.RenderTransformOrigin = new Point(0.5, 0.5);
			this.mAppImage.RenderTransform = scaleTransform;
			this.AppNameTextBox.RenderTransformOrigin = new Point(0.5, 0.5);
			this.AppNameTextBox.RenderTransform = scaleTransform;
			if (this.mAppIconModel.IconLocation == AppIconLocation.Dock || this.mAppIconModel.IconLocation == AppIconLocation.Moreapps)
			{
				this.ParentWindow.mWelcomeTab.mHomeAppManager.ShowDockIconTooltip(this, true);
				dropShadowEffect.BlurRadius = 12.0;
				this.mAppImageBorder.Effect = dropShadowEffect;
			}
			if (this.mAppIconModel.IsAppSuggestionActive)
			{
				this.ParentWindow.mWelcomeTab.mHomeAppManager.OpenAppSuggestionPopup(this.mAppIconModel.AppSuggestionInfo, this.AppNameTextBox, true);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0001464C File Offset: 0x0001284C
		private void UninstallButtonClicked()
		{
			try
			{
				if (this.mAppIconModel.IsDownloading)
				{
					if (this.mDownloader != null)
					{
						this.mDownloader.AbortApkDownload(this.mAppIconModel.PackageName);
						this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, null);
					}
				}
				else if (this.mAppIconModel.IsInstalling)
				{
					if (this.mAppIconModel.IsInstallingFailed)
					{
						this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, null);
					}
				}
				else if (this.mAppIconModel.IsAppSuggestionActive)
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_REMOVE_ICON", "");
					customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ICON_REMOVE", "");
					customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_REMOVE", ""), new EventHandler(this.RemoveAppSuggestion), null, false, null);
					customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_KEEP", ""), null, null, false, null);
					customMessageWindow.Owner = this.ParentWindow;
					this.ParentWindow.ShowDimOverlay(null);
					customMessageWindow.ShowDialog();
					this.ParentWindow.HideDimOverlay();
				}
				else
				{
					CustomMessageWindow customMessageWindow2 = new CustomMessageWindow();
					customMessageWindow2.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNINSTALL_0", ""), new object[] { this.mAppIconModel.AppName });
					customMessageWindow2.BodyTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNINSTALL_0_BS", ""), new object[] { this.mAppIconModel.AppName });
					customMessageWindow2.AddButton(ButtonColors.Red, "STRING_UNINSTALL", delegate(object o, EventArgs e)
					{
						this.AppIcon_UninstallConfirmationClicked(o, e);
					}, null, false, null);
					customMessageWindow2.AddButton(ButtonColors.White, "STRING_NO", null, null, false, null);
					this.ParentWindow.ShowDimOverlay(null);
					customMessageWindow2.Owner = this.ParentWindow.mDimOverlay;
					customMessageWindow2.ShowDialog();
					this.ParentWindow.HideDimOverlay();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in UninstallButtonClicked. Err : " + ex.ToString());
			}
		}

		// Token: 0x06000309 RID: 777 RVA: 0x000148AC File Offset: 0x00012AAC
		private void RemoveAppSuggestion(object sender, EventArgs e)
		{
			this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, null);
			ClientStats.SendMiscellaneousStatsAsync("cross_promotion_icon_removed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, this.mAppIconModel.PackageName, "bgp64", null, null, null);
			try
			{
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
				{
					OmitXmlDeclaration = true,
					Indent = true
				};
				XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
				{
					new XmlQualifiedName("", "")
				});
				string text = global::System.IO.Path.Combine(RegistryStrings.PromotionDirectory, "app_suggestion_removed");
				string text2 = "";
				if (File.Exists(text))
				{
					text2 = File.ReadAllText(text);
				}
				List<string> list = new List<string>();
				if (!string.IsNullOrEmpty(text2))
				{
					list = AppIconUI.DoDeserialize<List<string>>(text2);
				}
				if (!list.Contains(this.mAppIconModel.PackageName))
				{
					if (list.Count >= 20)
					{
						list.RemoveAt(0);
					}
					list.Add(this.mAppIconModel.PackageName);
				}
				using (XmlWriter xmlWriter = XmlWriter.Create(text, xmlWriterSettings))
				{
					new XmlSerializer(typeof(List<string>)).Serialize(xmlWriter, list, xmlSerializerNamespaces);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in writing removed suggested app icon package name in file " + ex.ToString());
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00014A24 File Offset: 0x00012C24
		private static T DoDeserialize<T>(string data) where T : class
		{
			T t;
			using (XmlReader xmlReader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(data))))
			{
				t = (T)((object)new XmlSerializer(typeof(T)).Deserialize(xmlReader));
			}
			return t;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00014A80 File Offset: 0x00012C80
		private void AppIcon_UninstallConfirmationClicked(object _1, EventArgs _2)
		{
			Logger.Info("Clicked app icon uninstall popup package name {0}", new object[] { this.mAppIconModel.PackageName });
			this.ParentWindow.mAppInstaller.UninstallApp(this.mAppIconModel.PackageName);
			this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppIcon(this.mAppIconModel.PackageName, null);
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00014AE8 File Offset: 0x00012CE8
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("Clicked app icon, package name {0}", new object[] { this.mAppIconModel.PackageName });
			if (this.mUnInstallTabButton.IsMouseOver)
			{
				this.UninstallButtonClicked();
				return;
			}
			if (this.mErrorGrid.IsVisible)
			{
				this.mErrorGrid.Visibility = Visibility.Hidden;
				if (this.mAppIconModel.IsDownLoadingFailed)
				{
					DownloadInstallApk downloadInstallApk = this.mDownloader;
					if (downloadInstallApk == null)
					{
						return;
					}
					downloadInstallApk.DownloadApk(this.mAppIconModel.ApkUrl, this.mAppIconModel.PackageName, false, false, "");
					return;
				}
				else if (this.mAppIconModel.IsInstallingFailed)
				{
					DownloadInstallApk downloadInstallApk2 = this.mDownloader;
					if (downloadInstallApk2 == null)
					{
						return;
					}
					downloadInstallApk2.InstallApk(this.mAppIconModel.PackageName, this.mAppIconModel.ApkFilePath, false, false, "");
					return;
				}
			}
			else if (!this.mAppIconModel.IsDownloading)
			{
				if (this.mAppIconModel.IsInstalling)
				{
					if (this.mDownloader == null)
					{
						this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(this.mAppIconModel.PackageName, this.mAppIconModel.AppName, PlayStoreAction.OpenApp, false);
						return;
					}
				}
				else
				{
					if (this.mAppIconModel.IsRerollIcon)
					{
						this.HandleRerollClick();
						return;
					}
					if (this.mAppIconModel.IsAppSuggestionActive)
					{
						this.HandleAppSuggestionClick();
						if (this.mAppIconModel.IsRedDotVisible)
						{
							this.mAppIconModel.IsRedDotVisible = false;
							HomeAppManager.AddPackageInRedDotShownRegistry(this.mAppIconModel.PackageName);
							return;
						}
					}
					else if (!string.IsNullOrEmpty(this.mAppIconModel.PackageName))
					{
						if (string.Equals(this.mAppIconModel.PackageName, "help_center", StringComparison.InvariantCulture))
						{
							this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(BlueStacksUIUtils.GetHelpCenterUrl(), "STRING_FEEDBACK", "help_center", true, "STRING_FEEDBACK", false);
							return;
						}
						if (string.Equals(this.mAppIconModel.PackageName, "instance_manager", StringComparison.InvariantCulture))
						{
							BlueStacksUIUtils.LaunchMultiInstanceManager();
							return;
						}
						if (string.Equals(this.mAppIconModel.PackageName, "macro_recorder", StringComparison.InvariantCulture))
						{
							this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
							return;
						}
						this.ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(this.mAppIconModel.PackageName, true);
					}
				}
			}
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00003FF0 File Offset: 0x000021F0
		private void HandleAppSuggestionClick()
		{
			this.ParentWindow.Utils.HandleGenericActionFromDictionary(this.mAppIconModel.AppSuggestionInfo.ExtraPayload, "my_apps", this.mAppIconModel.ImageName);
			this.SendAppSuggestionIconClickStats();
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00014D28 File Offset: 0x00012F28
		private void SendAppSuggestionIconClickStats()
		{
			ClientStats.SendPromotionAppClickStatsAsync(new Dictionary<string, string>
			{
				{ "op", "init" },
				{ "status", "success" },
				{
					"app_pkg",
					this.mAppIconModel.PackageName
				},
				{
					"extraPayload",
					JsonConvert.SerializeObject(this.mAppIconModel.AppSuggestionInfo.ExtraPayload)
				},
				{
					"app_name",
					this.mAppIconModel.AppName
				},
				{
					"app_promotion_id",
					this.mAppIconModel.mPromotionId
				},
				{ "promotion_type", "cross_promotion" }
			}, "app_activity");
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00014DD8 File Offset: 0x00012FD8
		private void HandleRerollClick()
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_REROLL_0", ""), new object[] { this.mAppIconModel.AppName });
			customMessageWindow.BodyTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_START_REROLL", ""), new object[] { this.mAppIconModel.AppName });
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_REROLL_APP_PREFIX", new EventHandler(this.StartRerollAccepted), null, false, null);
			this.ParentWindow.ShowDimOverlay(null);
			customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
			customMessageWindow.ShowDialog();
			if (customMessageWindow.ClickedButton != ButtonColors.White)
			{
				this.ParentWindow.HideDimOverlay();
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00014EC0 File Offset: 0x000130C0
		private void StartRerollAccepted(object sender, EventArgs e)
		{
			this.ParentWindow.ShowRerollOverlay();
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startReroll", new Dictionary<string, string>
			{
				{
					"packageName",
					this.mAppIconModel.PackageName
				},
				{ "rerollName", "" }
			});
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00014F18 File Offset: 0x00013118
		private void GifAppIconPlay()
		{
			try
			{
				this.mGifController = ImageBehavior.GetAnimationController(this.mAppImage);
				if (this.mGifController != null)
				{
					this.mGifController.Play();
				}
				else if (this.mAppIconModel.ImageName != null)
				{
					ImageSource imageSource = new BitmapImage(new Uri(this.mAppIconModel.ImageName));
					ImageBehavior.SetAnimatedSource(this.mAppImage, imageSource);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in animating appicon for package " + this.mAppIconModel.PackageName + Environment.NewLine + ex.ToString());
			}
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00014FB4 File Offset: 0x000131B4
		private void GifAppIconPause()
		{
			try
			{
				this.mGifController = ImageBehavior.GetAnimationController(this.mAppImage);
				this.mGifController.Pause();
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to pause gif. Err : " + ex.Message);
			}
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00015008 File Offset: 0x00013208
		private void DownloadingApp(AppIconDownloadingPhases downloadPhase)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				switch (downloadPhase)
				{
				case AppIconDownloadingPhases.DownloadStarted:
					this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
					this.mErrorGrid.Visibility = Visibility.Hidden;
					this.mProgressGrid.Visibility = Visibility.Visible;
					return;
				case AppIconDownloadingPhases.DownloadFailed:
					this.mErrorGrid.Visibility = Visibility.Visible;
					return;
				case AppIconDownloadingPhases.Downloading:
					this.mProgressGrid.Visibility = Visibility.Visible;
					return;
				case AppIconDownloadingPhases.DownloadCompleted:
					this.mProgressGrid.Visibility = Visibility.Hidden;
					this.mBusyGrid.Visibility = Visibility.Visible;
					return;
				case AppIconDownloadingPhases.InstallStarted:
					this.ShowAppUninstallButton(false);
					this.mErrorGrid.Visibility = Visibility.Hidden;
					this.mBusyGrid.Visibility = Visibility.Visible;
					return;
				case AppIconDownloadingPhases.InstallFailed:
					if (!this.mAppIconModel.mIsAppInstalled)
					{
						this.mBusyGrid.Visibility = Visibility.Hidden;
						this.mErrorGrid.Visibility = Visibility.Visible;
						return;
					}
					break;
				case AppIconDownloadingPhases.InstallCompleted:
					this.mBusyGrid.Visibility = Visibility.Hidden;
					this.mDownloader = null;
					this.mAppIconModel.mAppDownloadingEvent -= this.DownloadingApp;
					break;
				default:
					return;
				}
			}), new object[0]);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00015048 File Offset: 0x00013248
		private void SetAppIconLocation(AppIconLocation iconLocation, double height, double width)
		{
			if (iconLocation == AppIconLocation.Dock)
			{
				if (width > 68.0)
				{
					this.mMainGrid.ColumnDefinitions[3].Width = new GridLength(width - 68.0);
				}
				else
				{
					this.mMainGrid.ColumnDefinitions[2].Width = new GridLength(width);
				}
				if (height < 68.0)
				{
					this.mMainGrid.RowDefinitions[1].Height = new GridLength(height);
				}
				GridLength gridLength = new GridLength(0.0);
				this.mMainGrid.ColumnDefinitions[1].Width = gridLength;
				this.mMainGrid.ColumnDefinitions[4].Width = gridLength;
				base.Margin = new Thickness(0.0, 43.0 - height, 0.0, 0.0);
				this.mAppImage.Height = height;
				this.mAppImage.Width = width;
				RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(new Point(0.0, 0.0), new Point(width, height)), 10.0, 10.0);
				this.mAppImage.Clip = rectangleGeometry;
				this.AppNameTextBox.Visibility = Visibility.Collapsed;
				this.mAppIconModel.AppNameTooltip = null;
				return;
			}
			if (iconLocation != AppIconLocation.Moreapps)
			{
				return;
			}
			if (width > 68.0)
			{
				this.mMainGrid.ColumnDefinitions[3].Width = new GridLength(width - 68.0);
			}
			else
			{
				this.mMainGrid.ColumnDefinitions[2].Width = new GridLength(width);
			}
			if (height < 68.0)
			{
				this.mMainGrid.RowDefinitions[1].Height = new GridLength(height);
			}
			GridLength gridLength2 = new GridLength(0.0);
			this.mMainGrid.ColumnDefinitions[1].Width = gridLength2;
			this.mMainGrid.ColumnDefinitions[4].Width = gridLength2;
			this.mMainGrid.RowDefinitions[3].Height = gridLength2;
			this.mMainGrid.RowDefinitions[5].Height = gridLength2;
			base.Margin = new Thickness(0.0, 43.0 - height, 0.0, 0.0);
			this.mAppImage.Height = height;
			this.mAppImage.Width = width;
			RectangleGeometry rectangleGeometry2 = new RectangleGeometry(new Rect(new Point(0.0, 0.0), new Point(width, height)), 10.0, 10.0);
			this.mAppImage.Clip = rectangleGeometry2;
			this.AppNameTextBox.Visibility = Visibility.Collapsed;
			this.mAppIconModel.AppNameTooltip = null;
		}

		// Token: 0x04000171 RID: 369
		private Thread threadShowingUninstallButton;

		// Token: 0x04000172 RID: 370
		private ImageAnimationController mGifController;

		// Token: 0x04000173 RID: 371
		private MainWindow ParentWindow;

		// Token: 0x04000175 RID: 373
		private DownloadInstallApk mDownloader;
	}
}
