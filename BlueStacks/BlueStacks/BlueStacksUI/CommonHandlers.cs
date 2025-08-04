using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001E0 RID: 480
	internal class CommonHandlers : IDisposable
	{
		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060012DE RID: 4830 RVA: 0x000730D8 File Offset: 0x000712D8
		// (remove) Token: 0x060012DF RID: 4831 RVA: 0x00073110 File Offset: 0x00071310
		public event CommonHandlers.MacroBookmarkChanged MacroBookmarkChangedEvent;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060012E0 RID: 4832 RVA: 0x00073148 File Offset: 0x00071348
		// (remove) Token: 0x060012E1 RID: 4833 RVA: 0x00073180 File Offset: 0x00071380
		public event CommonHandlers.MacroSettingsChanged MacroSettingChangedEvent;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x060012E2 RID: 4834 RVA: 0x000731B8 File Offset: 0x000713B8
		// (remove) Token: 0x060012E3 RID: 4835 RVA: 0x000731F0 File Offset: 0x000713F0
		public event CommonHandlers.ShortcutKeysChanged ShortcutKeysChangedEvent;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x060012E4 RID: 4836 RVA: 0x00073228 File Offset: 0x00071428
		// (remove) Token: 0x060012E5 RID: 4837 RVA: 0x00073260 File Offset: 0x00071460
		public event CommonHandlers.ShortcutKeysRefresh ShortcutKeysRefreshEvent;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060012E6 RID: 4838 RVA: 0x00073298 File Offset: 0x00071498
		// (remove) Token: 0x060012E7 RID: 4839 RVA: 0x000732D0 File Offset: 0x000714D0
		public event CommonHandlers.MacroDeleted MacroDeletedEvent;

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x060012E8 RID: 4840 RVA: 0x00073308 File Offset: 0x00071508
		// (remove) Token: 0x060012E9 RID: 4841 RVA: 0x00073340 File Offset: 0x00071540
		public event CommonHandlers.OverlayStateChanged OverlayStateChangedEvent;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x060012EA RID: 4842 RVA: 0x00073378 File Offset: 0x00071578
		// (remove) Token: 0x060012EB RID: 4843 RVA: 0x000733B0 File Offset: 0x000715B0
		public event CommonHandlers.MacroButtonVisibilityChanged MacroButtonVisibilityChangedEvent;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x060012EC RID: 4844 RVA: 0x000733E8 File Offset: 0x000715E8
		// (remove) Token: 0x060012ED RID: 4845 RVA: 0x00073420 File Offset: 0x00071620
		public event CommonHandlers.OperationSyncButtonVisibilityChanged OperationSyncButtonVisibilityChangedEvent;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x060012EE RID: 4846 RVA: 0x00073458 File Offset: 0x00071658
		// (remove) Token: 0x060012EF RID: 4847 RVA: 0x00073490 File Offset: 0x00071690
		public event CommonHandlers.OBSResponseTimeout OBSResponseTimeoutEvent;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x060012F0 RID: 4848 RVA: 0x000734C8 File Offset: 0x000716C8
		// (remove) Token: 0x060012F1 RID: 4849 RVA: 0x00073500 File Offset: 0x00071700
		public event CommonHandlers.ScreenRecorderStateTransitioning ScreenRecorderStateTransitioningEvent;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x060012F2 RID: 4850 RVA: 0x00073538 File Offset: 0x00071738
		// (remove) Token: 0x060012F3 RID: 4851 RVA: 0x00073570 File Offset: 0x00071770
		public event CommonHandlers.BTvDownloaderMinimized BTvDownloaderMinimizedEvent;

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x060012F4 RID: 4852 RVA: 0x000735A8 File Offset: 0x000717A8
		// (remove) Token: 0x060012F5 RID: 4853 RVA: 0x000735E0 File Offset: 0x000717E0
		public event CommonHandlers.GamepadButtonVisibilityChanged GamepadButtonVisibilityChangedEvent;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x060012F6 RID: 4854 RVA: 0x00073618 File Offset: 0x00071818
		// (remove) Token: 0x060012F7 RID: 4855 RVA: 0x00073650 File Offset: 0x00071850
		public event CommonHandlers.ScreenRecordingStateChanged ScreenRecordingStateChangedEvent;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x060012F8 RID: 4856 RVA: 0x00073688 File Offset: 0x00071888
		// (remove) Token: 0x060012F9 RID: 4857 RVA: 0x000736C0 File Offset: 0x000718C0
		public event CommonHandlers.VolumeChanged VolumeChangedEvent;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x060012FA RID: 4858 RVA: 0x000736F8 File Offset: 0x000718F8
		// (remove) Token: 0x060012FB RID: 4859 RVA: 0x00073730 File Offset: 0x00071930
		public event CommonHandlers.VolumeMuted VolumeMutedEvent;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x060012FC RID: 4860 RVA: 0x00073768 File Offset: 0x00071968
		// (remove) Token: 0x060012FD RID: 4861 RVA: 0x000737A0 File Offset: 0x000719A0
		public event CommonHandlers.GameGuideButtonVisibilityChanged GameGuideButtonVisibilityChangedEvent;

		// Token: 0x060012FE RID: 4862 RVA: 0x0000D6C7 File Offset: 0x0000B8C7
		internal void OnVolumeMuted(bool muted)
		{
			CommonHandlers.VolumeMuted volumeMutedEvent = this.VolumeMutedEvent;
			if (volumeMutedEvent == null)
			{
				return;
			}
			volumeMutedEvent(muted);
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0000D6DA File Offset: 0x0000B8DA
		internal void OnVolumeChanged(int volumeLevel)
		{
			CommonHandlers.VolumeChanged volumeChangedEvent = this.VolumeChangedEvent;
			if (volumeChangedEvent == null)
			{
				return;
			}
			volumeChangedEvent(volumeLevel);
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x0000D6ED File Offset: 0x0000B8ED
		internal void OnScreenRecordingStateChanged(bool isRecording)
		{
			CommonHandlers.ScreenRecordingStateChanged screenRecordingStateChangedEvent = this.ScreenRecordingStateChangedEvent;
			if (screenRecordingStateChangedEvent == null)
			{
				return;
			}
			screenRecordingStateChangedEvent(isRecording);
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0000D700 File Offset: 0x0000B900
		internal void OnGamepadButtonVisibilityChanged(bool visiblity)
		{
			CommonHandlers.GamepadButtonVisibilityChanged gamepadButtonVisibilityChangedEvent = this.GamepadButtonVisibilityChangedEvent;
			if (gamepadButtonVisibilityChangedEvent == null)
			{
				return;
			}
			gamepadButtonVisibilityChangedEvent(visiblity);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0000D713 File Offset: 0x0000B913
		internal void OnGameGuideButtonVisibilityChanged(bool visiblity)
		{
			CommonHandlers.GameGuideButtonVisibilityChanged gameGuideButtonVisibilityChangedEvent = this.GameGuideButtonVisibilityChangedEvent;
			if (gameGuideButtonVisibilityChangedEvent == null)
			{
				return;
			}
			gameGuideButtonVisibilityChangedEvent(visiblity);
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x0000D726 File Offset: 0x0000B926
		private void OnOBSResponseTimeout()
		{
			CommonHandlers.OBSResponseTimeout obsresponseTimeoutEvent = this.OBSResponseTimeoutEvent;
			if (obsresponseTimeoutEvent == null)
			{
				return;
			}
			obsresponseTimeoutEvent();
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0000D738 File Offset: 0x0000B938
		private void OnBTvDownloaderMinimized()
		{
			CommonHandlers.BTvDownloaderMinimized btvDownloaderMinimizedEvent = this.BTvDownloaderMinimizedEvent;
			if (btvDownloaderMinimizedEvent == null)
			{
				return;
			}
			btvDownloaderMinimizedEvent();
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0000D74A File Offset: 0x0000B94A
		internal void OnScreenRecorderStateTransitioning()
		{
			CommonHandlers.ScreenRecorderStateTransitioning screenRecorderStateTransitioningEvent = this.ScreenRecorderStateTransitioningEvent;
			if (screenRecorderStateTransitioningEvent == null)
			{
				return;
			}
			screenRecorderStateTransitioningEvent();
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0000D75C File Offset: 0x0000B95C
		internal void OnMacroButtonVisibilityChanged(bool isVisible)
		{
			CommonHandlers.MacroButtonVisibilityChanged macroButtonVisibilityChangedEvent = this.MacroButtonVisibilityChangedEvent;
			if (macroButtonVisibilityChangedEvent == null)
			{
				return;
			}
			macroButtonVisibilityChangedEvent(isVisible);
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0000D76F File Offset: 0x0000B96F
		internal void OnOperationSyncButtonVisibilityChanged(bool isVisible)
		{
			CommonHandlers.OperationSyncButtonVisibilityChanged operationSyncButtonVisibilityChangedEvent = this.OperationSyncButtonVisibilityChangedEvent;
			if (operationSyncButtonVisibilityChangedEvent == null)
			{
				return;
			}
			operationSyncButtonVisibilityChangedEvent(isVisible);
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x000737D8 File Offset: 0x000719D8
		internal void OnMacroBookmarkChanged(string fileName, bool wasBookmarked)
		{
			foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
			{
				CommonHandlers mCommonHandler = keyValuePair.Value.mCommonHandler;
				if (mCommonHandler != null)
				{
					CommonHandlers.MacroBookmarkChanged macroBookmarkChangedEvent = mCommonHandler.MacroBookmarkChangedEvent;
					if (macroBookmarkChangedEvent != null)
					{
						macroBookmarkChangedEvent(fileName, wasBookmarked);
					}
				}
			}
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x00073848 File Offset: 0x00071A48
		internal static void OnMacroSettingChanged(MacroRecording record)
		{
			foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
			{
				CommonHandlers mCommonHandler = keyValuePair.Value.mCommonHandler;
				if (mCommonHandler != null)
				{
					CommonHandlers.MacroSettingsChanged macroSettingChangedEvent = mCommonHandler.MacroSettingChangedEvent;
					if (macroSettingChangedEvent != null)
					{
						macroSettingChangedEvent(record);
					}
				}
			}
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x000738B8 File Offset: 0x00071AB8
		internal static void OnMacroDeleted(string fileName)
		{
			foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
			{
				CommonHandlers mCommonHandler = keyValuePair.Value.mCommonHandler;
				if (mCommonHandler != null)
				{
					CommonHandlers.MacroDeleted macroDeletedEvent = mCommonHandler.MacroDeletedEvent;
					if (macroDeletedEvent != null)
					{
						macroDeletedEvent(fileName);
					}
				}
			}
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x0000D782 File Offset: 0x0000B982
		internal void OnShortcutKeysChanged(bool isEnabled)
		{
			CommonHandlers.ShortcutKeysChanged shortcutKeysChangedEvent = this.ShortcutKeysChangedEvent;
			if (shortcutKeysChangedEvent == null)
			{
				return;
			}
			shortcutKeysChangedEvent(isEnabled);
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0000D795 File Offset: 0x0000B995
		internal void OnShortcutKeysRefresh()
		{
			CommonHandlers.ShortcutKeysRefresh shortcutKeysRefreshEvent = this.ShortcutKeysRefreshEvent;
			if (shortcutKeysRefreshEvent == null)
			{
				return;
			}
			shortcutKeysRefreshEvent();
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0000D7A7 File Offset: 0x0000B9A7
		internal void OnOverlayStateChanged(bool isEnabled)
		{
			CommonHandlers.OverlayStateChanged overlayStateChangedEvent = this.OverlayStateChangedEvent;
			if (overlayStateChangedEvent == null)
			{
				return;
			}
			overlayStateChangedEvent(isEnabled);
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0000D7BA File Offset: 0x0000B9BA
		internal CommonHandlers(MainWindow window)
		{
			this.ParentWindow = window;
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0000D7C9 File Offset: 0x0000B9C9
		public void LocationButtonHandler()
		{
			this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab("STRING_MAP", "com.location.provider", "com.location.provider.MapsActivity", "ico_fakegps", true, true, false);
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x00073928 File Offset: 0x00071B28
		public void ImageTranslationHandler()
		{
			Logger.Info("Saving screenshot automatically for image translater");
			if (ImageTranslateControl.Instance == null)
			{
				using (Bitmap bitmap = this.CaptureSreenShot())
				{
					ImageTranslateControl imageTranslateControl = new ImageTranslateControl(this.ParentWindow);
					imageTranslateControl.GetTranslateImage(bitmap);
					this.ParentWindow.ShowDimOverlay(imageTranslateControl);
				}
			}
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0000D7F7 File Offset: 0x0000B9F7
		internal static void ToggleFarmMode(bool newStatus)
		{
			RegistryManager.Instance.CurrentFarmModeStatus = newStatus;
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
				{
					try
					{
						keyValuePair.Value.mFrontendHandler.SendFrontendRequestAsync("farmModeHandler", new Dictionary<string, string> { 
						{
							"enable",
							RegistryManager.Instance.CurrentFarmModeStatus.ToString(CultureInfo.InvariantCulture)
						} });
					}
					catch
					{
					}
				}
			});
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x00073988 File Offset: 0x00071B88
		internal void SearchAppCenter(string searchString)
		{
			AppTabButton tab = this.ParentWindow.mTopBar.mAppTabButtons.GetTab("appcenter");
			bool flag;
			if (tab == null)
			{
				flag = null != null;
			}
			else
			{
				BrowserControl browserControl = tab.GetBrowserControl();
				flag = ((browserControl != null) ? browserControl.CefBrowser : null) != null;
			}
			if (flag)
			{
				tab.GetBrowserControl().CefBrowser.ExecuteJavaScript(string.Format(CultureInfo.InvariantCulture, "openSearch(\"{0}\")", new object[] { HttpUtility.UrlEncode(searchString) }), tab.GetBrowserControl().CefBrowser.StartUrl, 0);
				this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("appcenter", true, false);
				return;
			}
			this.ParentWindow.Utils.HandleApplicationBrowserClick(BlueStacksUIUtils.GetAppCenterUrl(null) + "&query=" + HttpUtility.UrlEncode(searchString), LocaleStrings.GetLocalizedString("STRING_APP_CENTER", ""), "appcenter", false, "");
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0000D829 File Offset: 0x0000BA29
		internal void HideMacroRecorderWindow()
		{
			this.ParentWindow.MacroRecorderWindow.Owner = null;
			this.ParentWindow.MacroRecorderWindow.Hide();
			this.ParentWindow.MacroRecorderWindow.ShowWithParentWindow = false;
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0000D85D File Offset: 0x0000BA5D
		internal void RefreshMacroRecorderWindow()
		{
			this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
			this.ParentWindow.MacroRecorderWindow.Init();
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x00073A68 File Offset: 0x00071C68
		internal static void RefreshAllMacroRecorderWindow()
		{
			try
			{
				foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
				{
					if (keyValuePair.Value.MacroRecorderWindow != null)
					{
						keyValuePair.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
						keyValuePair.Value.MacroRecorderWindow.Init();
					}
				}
			}
			catch (Exception ex)
			{
				string text = "Error in refreshing operation recorder window";
				Exception ex2 = ex;
				Logger.Debug(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x00073B1C File Offset: 0x00071D1C
		internal static void RefreshAllMacroWindowWithScroll()
		{
			try
			{
				foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
				{
					if (keyValuePair.Value.MacroRecorderWindow != null)
					{
						keyValuePair.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
						keyValuePair.Value.MacroRecorderWindow.Init();
						keyValuePair.Value.MacroRecorderWindow.mScriptsListScrollbar.ScrollToEnd();
					}
				}
			}
			catch (Exception ex)
			{
				string text = "Error in refreshing operation recorder window";
				Exception ex2 = ex;
				Logger.Debug(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x00073BE4 File Offset: 0x00071DE4
		internal void ShowMacroRecorderWindow()
		{
			this.ParentWindow.MacroRecorderWindow.Owner = this.ParentWindow;
			this.ParentWindow.MacroRecorderWindow.ShowWithParentWindow = true;
			this.ParentWindow.MacroRecorderWindow.Show();
			this.ParentWindow.Activate();
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x00073C34 File Offset: 0x00071E34
		private Bitmap CaptureSreenShot()
		{
			global::System.Windows.Point point = this.ParentWindow.mContentGrid.PointToScreen(new global::System.Windows.Point(0.0, 0.0));
			global::System.Windows.Point point2 = new global::System.Windows.Point((double)Convert.ToInt32(point.X), (double)Convert.ToInt32(point.Y));
			global::System.Windows.Point point3 = this.ParentWindow.mContentGrid.PointToScreen(new global::System.Windows.Point((double)((int)this.ParentWindow.mContentGrid.ActualWidth), (double)((int)this.ParentWindow.mContentGrid.ActualHeight - 40)));
			global::System.Drawing.Size size = new global::System.Drawing.Size(Convert.ToInt32(point3.X - point2.X), Convert.ToInt32(point3.Y - point2.Y));
			Bitmap bitmap = new Bitmap(size.Width, size.Height);
			global::System.Drawing.Point point4 = new global::System.Drawing.Point((int)point2.X, (int)point2.Y);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.CopyFromScreen(point4, global::System.Drawing.Point.Empty, size);
			}
			return bitmap;
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x00073D5C File Offset: 0x00071F5C
		public void ScreenShotButtonHandler()
		{
			try
			{
				string text = DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss", CultureInfo.InvariantCulture);
				string text2 = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.AppName;
				if (FeatureManager.Instance.IsCustomUIForNCSoft && !string.IsNullOrEmpty(this.ParentWindow.mNCTopBar.mAppName.Text))
				{
					text2 = this.ParentWindow.mNCTopBar.mAppName.Text;
				}
				string text3 = text2 + "_Screenshot_" + text + ".jpg";
				string text4 = Path.Combine(Path.GetTempPath(), text3);
				this.ParentWindow.mFrontendHandler.GetScreenShot(text4);
				try
				{
					if (FeatureManager.Instance.IsCustomUIForDMM)
					{
						using (SoundPlayer soundPlayer = new SoundPlayer(Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "camera_shutter_click.wav")))
						{
							soundPlayer.Play();
						}
					}
				}
				catch
				{
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in screenshot button handler: {0}", new object[] { ex });
			}
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x00073E98 File Offset: 0x00072098
		internal void PostScreenShotWork(string screenshotFileFullPath, bool showScreenShotSaved)
		{
			try
			{
				Logger.Debug("screen shot path..." + screenshotFileFullPath);
				if (RegistryManager.Instance.IsScreenshotsLocationPopupEnabled)
				{
					this.ShowScreenShotFolderUpdatePopup();
				}
				string text = RegistryManager.Instance.ScreenShotsPath;
				if (!StringExtensions.IsValidPath(text))
				{
					string screenshotDefaultPath = RegistryStrings.ScreenshotDefaultPath;
					if (!Directory.Exists(screenshotDefaultPath))
					{
						Directory.CreateDirectory(screenshotDefaultPath);
					}
					RegistryManager.Instance.ScreenShotsPath = screenshotDefaultPath;
					text = screenshotDefaultPath;
				}
				string fileName = Path.GetFileName(screenshotFileFullPath);
				string text2 = Path.Combine(text, fileName);
				Logger.Debug("Screen shot filename.." + text2);
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				File.Move(screenshotFileFullPath, text2);
				ClientStats.SendMiscellaneousStatsAsync("MediaFileSaveSuccess", RegistryManager.Instance.UserGuid, "ScreenShot", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				if (showScreenShotSaved && RegistryManager.Instance.IsShowToastNotification)
				{
					this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_SAVED", ""));
				}
				if (showScreenShotSaved && this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
				{
					this.ParentWindow.mSidebar.ShowScreenshotSavedPopup(text2);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in post screenshot work: {0}", new object[] { ex });
			}
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x00073FFC File Offset: 0x000721FC
		private void ShowScreenShotFolderUpdatePopup()
		{
			RegistryManager.Instance.IsScreenshotsLocationPopupEnabled = false;
			string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_OPEN_MEDIA_FOLDER", "");
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHOOSE_CUSTOM", new EventHandler(this.ChooseCustomFolder), null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_USE_CURRENT", null, null, false, null);
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_CHOOSE_FOLDER_TEXT", "");
			customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
			customMessageWindow.BodyWarningTextBlock.Text = screenShotsPath;
			BlueStacksUIBinding.BindColor(customMessageWindow.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
			this.ParentWindow.ShowDimOverlay(null);
			customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
			customMessageWindow.ShowDialog();
			this.ParentWindow.HideDimOverlay();
			ClientStats.SendMiscellaneousStatsAsync("MediaFilesPathSet", RegistryManager.Instance.UserGuid, "PathChangeFromPopUp", screenShotsPath, RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null);
		}

		// Token: 0x0600131C RID: 4892 RVA: 0x0000D889 File Offset: 0x0000BA89
		internal void AddCoordinatesToScriptText(double x, double y)
		{
			if (KMManager.sIsInScriptEditingMode && KMManager.CanvasWindow != null)
			{
				AdvancedGameControlWindow sidebarWindow = KMManager.CanvasWindow.SidebarWindow;
				if (sidebarWindow == null)
				{
					return;
				}
				sidebarWindow.InsertXYInScript(x, y);
			}
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x00074118 File Offset: 0x00072318
		private void ChooseCustomFolder(object sender, EventArgs e)
		{
			string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
			if (!Directory.Exists(screenShotsPath))
			{
				Directory.CreateDirectory(screenShotsPath);
			}
			this.ShowFolderBrowserDialog(screenShotsPath);
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x00074148 File Offset: 0x00072348
		internal void ShowFolderBrowserDialog(string screenshotPath)
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = screenshotPath,
				ShowNewFolderButton = true
			})
			{
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					string selectedPath = folderBrowserDialog.SelectedPath;
					Logger.Info("dialoge selected path.." + folderBrowserDialog.SelectedPath);
					bool flag = Utils.CheckWritePermissionForFolder(selectedPath);
					Logger.Info("Permission.." + flag.ToString() + "..path.." + selectedPath);
					if (!flag)
					{
						this.ShowInvalidPathPopUp();
					}
					else
					{
						RegistryManager.Instance.ScreenShotsPath = selectedPath;
					}
				}
				else
				{
					RegistryManager.Instance.ScreenShotsPath = screenshotPath;
				}
			}
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x000741F0 File Offset: 0x000723F0
		private void ShowInvalidPathPopUp()
		{
			string defaultPicturePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bluestacks");
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_OPEN_MEDIA_FOLDER", "");
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHOOSE_ANOTHER", new EventHandler(this.ChooseCustomFolder), null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_USE_DEFAULT", delegate(object o, EventArgs e)
			{
				RegistryManager.Instance.ScreenShotsPath = defaultPicturePath;
			}, null, false, null);
			customMessageWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
			customMessageWindow.BodyTextBlockTitle.Text = LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_INVALID_PATH", "");
			BlueStacksUIBinding.BindColor(customMessageWindow.BodyTextBlockTitle, TextBlock.ForegroundProperty, "DeleteComboTextForeground");
			customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_USE_DEFAULT", "");
			customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
			customMessageWindow.BodyWarningTextBlock.Text = defaultPicturePath;
			BlueStacksUIBinding.BindColor(customMessageWindow.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
			customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
			customMessageWindow.ShowDialog();
			customMessageWindow.Close();
		}

		// Token: 0x06001320 RID: 4896 RVA: 0x0000D8AF File Offset: 0x0000BAAF
		public void ShakeButtonHandler()
		{
			this.ParentWindow.Utils.ShakeWindow();
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("shake", null);
		}

		// Token: 0x06001321 RID: 4897 RVA: 0x00074310 File Offset: 0x00072510
		public void BackButtonHandler(bool receivedFromImap = false)
		{
			if (this.ParentWindow.mGuestBootCompleted)
			{
				new Thread(delegate
				{
					VmCmdHandler.RunCommand("back", this.ParentWindow.mVmName);
				})
				{
					IsBackground = true
				}.Start();
				if (this.ParentWindow.SendClientActions && !receivedFromImap)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					Dictionary<string, string> dictionary2 = new Dictionary<string, string> { { "EventAction", "BackButton" } };
					JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
					serializerSettings.Formatting = Formatting.None;
					dictionary.Add("operationData", JsonConvert.SerializeObject(dictionary2, serializerSettings));
					this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
				}
			}
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x000743A8 File Offset: 0x000725A8
		public void OpenBrowserInPopup(Dictionary<string, string> payload)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					string localizedString = LocaleStrings.GetLocalizedString(payload["click_action_title"], "");
					string text = payload["click_action_value"].Trim();
					string urlWithParams = WebHelper.GetUrlWithParams(text);
					ClientStats.SendPopupBrowserStatsInMiscASync("request", text);
					PopupBrowserControl popupBrowserControl = new PopupBrowserControl();
					popupBrowserControl.Init(urlWithParams, localizedString, this.ParentWindow);
					ClientStats.SendPopupBrowserStatsInMiscASync("impression", text);
					this.ParentWindow.ShowDimOverlay(popupBrowserControl);
				}
				catch (Exception ex)
				{
					Logger.Error("Couldn't open popup. An exception occured. {0}", new object[] { ex });
				}
			}), new object[0]);
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x000743EC File Offset: 0x000725EC
		public void HomeButtonHandler(bool isLaunch = true, bool receivedFromImap = false)
		{
			this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", isLaunch, false);
			if (this.ParentWindow.SendClientActions && !receivedFromImap)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>
				{
					{ "EventAction", "HomeButton" },
					{
						"IsLaunch",
						isLaunch.ToString(CultureInfo.InvariantCulture)
					}
				};
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = Formatting.None;
				dictionary.Add("operationData", JsonConvert.SerializeObject(dictionary2, serializerSettings));
				this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", dictionary);
			}
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x00074490 File Offset: 0x00072690
		public void FullScreenButtonHandler(string source, string actionPerformed)
		{
			if (!this.ParentWindow.mResizeHandler.IsMinMaxEnabled)
			{
				return;
			}
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (this.ParentWindow.mIsFullScreen)
				{
					this.ParentWindow.RestoreWindows(false);
					this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("false");
					ClientStats.SendMiscellaneousStatsAsync(source, RegistryManager.Instance.UserGuid, "RestoreFullscreen", actionPerformed, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					return;
				}
				this.ParentWindow.FullScreenWindow();
				this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("true");
				ClientStats.SendMiscellaneousStatsAsync(source, RegistryManager.Instance.UserGuid, "Fullscreen", actionPerformed, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}), new object[0]);
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x000744F0 File Offset: 0x000726F0
		internal void AddToastPopup(Window window, string message, double duration = 1.3, bool isShowCloseImage = false)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					CustomToastPopupControl customToastPopupControl = new CustomToastPopupControl(window);
					if (isShowCloseImage)
					{
						customToastPopupControl.Init(window, message, global::System.Windows.Media.Brushes.Black, null, global::System.Windows.HorizontalAlignment.Center, VerticalAlignment.Top, null, 12, null, null, isShowCloseImage);
						customToastPopupControl.Margin = new Thickness(0.0, 40.0, 0.0, 0.0);
					}
					else
					{
						customToastPopupControl.Init(window, message, global::System.Windows.Media.Brushes.Black, null, global::System.Windows.HorizontalAlignment.Center, VerticalAlignment.Center, null, 12, null, null, false);
					}
					customToastPopupControl.ShowPopup(duration);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in showing toast popup: " + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x00074544 File Offset: 0x00072744
		internal void HandleClientOperation(string operationString)
		{
			try
			{
				JObject jobject = JObject.Parse(operationString);
				string text = (string)jobject["EventAction"];
				if (text != null)
				{
					if (!(text == "RunApp"))
					{
						if (!(text == "BackButton"))
						{
							if (!(text == "HomeButton"))
							{
								if (!(text == "TabSelected"))
								{
									if (text == "TabClosed")
									{
										string tabKey2 = jobject["tabKey"].ToObject<string>();
										bool sendStopAppToAndroid = jobject["sendStopAppToAndroid"].ToObject<bool>();
										bool forceClose = jobject["forceClose"].ToObject<bool>();
										if (!string.IsNullOrEmpty(tabKey2))
										{
											this.ParentWindow.Dispatcher.Invoke(new Action(delegate
											{
												this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(tabKey2, sendStopAppToAndroid, forceClose, true, true, "");
											}), new object[0]);
										}
									}
								}
								else
								{
									string tabKey = jobject["tabKey"].ToObject<string>();
									if (!string.IsNullOrEmpty(tabKey))
									{
										this.ParentWindow.Dispatcher.Invoke(new Action(delegate
										{
											this.ParentWindow.mTopBar.mAppTabButtons.GoToTab(tabKey, true, true);
										}), new object[0]);
									}
								}
							}
							else
							{
								bool isLaunch = jobject["IsLaunch"].ToObject<bool>();
								this.ParentWindow.Dispatcher.Invoke(new Action(delegate
								{
									this.HomeButtonHandler(isLaunch, true);
								}), new object[0]);
							}
						}
						else
						{
							this.BackButtonHandler(true);
						}
					}
					else
					{
						this.ParentWindow.mAppHandler.SendRunAppRequestAsync((string)jobject["Package"], (string)jobject["Activity"], true);
					}
				}
			}
			catch (Exception ex)
			{
				string text2 = "Exception in HandleClientOperation. OperationString: ";
				string text3 = " Error:";
				Exception ex2 = ex;
				Logger.Error(text2 + operationString + text3 + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0000D8D7 File Offset: 0x0000BAD7
		private bool CheckForMacroVisibility()
		{
			return !this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted || CommonHandlers.ShowMacroForSelectedApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey);
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x00074770 File Offset: 0x00072970
		private static bool ShowMacroForSelectedApp(string appPackage)
		{
			if (PromotionObject.Instance.AppSpecificRulesList != null)
			{
				foreach (string text in PromotionObject.Instance.AppSpecificRulesList)
				{
					string text2 = text;
					if (text.EndsWith("*", StringComparison.InvariantCulture))
					{
						text2 = text.Substring(0, text.Length - 2);
					}
					if (text2.StartsWith("~", StringComparison.InvariantCulture))
					{
						if (appPackage.StartsWith(text2.Substring(1), StringComparison.InvariantCulture))
						{
							return false;
						}
					}
					else if (appPackage.StartsWith(text2, StringComparison.InvariantCulture))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x00074824 File Offset: 0x00072A24
		private static bool IsCustomCursorEnableForApp(string appPackage)
		{
			bool flag;
			try
			{
				if (!RegistryManager.Instance.CustomCursorEnabled || !FeatureManager.Instance.IsCustomCursorEnabled)
				{
					flag = false;
				}
				else
				{
					string text = string.Empty;
					if (PromotionObject.Instance.CustomCursorExcludedAppsList != null)
					{
						foreach (string text2 in PromotionObject.Instance.CustomCursorExcludedAppsList)
						{
							text = text2;
							if (text2.EndsWith("*", StringComparison.InvariantCulture))
							{
								text = text2.Substring(0, text2.Length - 1);
							}
							if (text.StartsWith("~", StringComparison.InvariantCulture))
							{
								if (appPackage.StartsWith(text.Substring(1), StringComparison.InvariantCulture))
								{
									return true;
								}
							}
							else if (appPackage.StartsWith(text, StringComparison.InvariantCulture))
							{
								return false;
							}
						}
					}
					flag = true;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0000D911 File Offset: 0x0000BB11
		internal void SetCustomCursorForApp(string appPackage)
		{
			this.ToggleCursorStyle(CommonHandlers.IsCustomCursorEnableForApp(appPackage));
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x00074910 File Offset: 0x00072B10
		internal void ClipMouseCursorHandler(bool forceDisable = false, bool switchState = true, string statAction = "", string sourceLocation = "")
		{
			try
			{
				if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					if (forceDisable)
					{
						this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped = false;
					}
					else if (switchState)
					{
						this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped = !this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped;
					}
					if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped)
					{
						RECT rect = default(RECT);
						if (this.ParentWindow.StaticComponents.mLastMappableWindowHandle == IntPtr.Zero)
						{
							this.ParentWindow.StaticComponents.mLastMappableWindowHandle = this.ParentWindow.mFrontendHandler.mFrontendHandle;
						}
						InteropWindow.GetWindowRect(this.ParentWindow.StaticComponents.mLastMappableWindowHandle, ref rect);
						global::System.Drawing.Point point = new global::System.Drawing.Point(rect.Left, rect.Top);
						global::System.Drawing.Size size = new global::System.Drawing.Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
						global::System.Windows.Forms.Cursor.Clip = new Rectangle(point, size);
						this.ParentWindow.OnCursorLockChanged(true);
						this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("true");
						if (!string.IsNullOrEmpty(statAction))
						{
							ClientStats.SendMiscellaneousStatsAsync(sourceLocation, RegistryManager.Instance.UserGuid, "LockMouseCursor", statAction, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, null);
							if (RegistryManager.Instance.IsShowToastNotification)
							{
								this.ParentWindow.ShowGeneralToast(string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNLOCK_CURSOR", ""), new object[] { this.GetShortcutKeyFromName("STRING_TOGGLE_LOCK_CURSOR", false) }));
							}
						}
					}
					else
					{
						global::System.Windows.Forms.Cursor.Clip = Rectangle.Empty;
						this.ParentWindow.OnCursorLockChanged(false);
						this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("false");
						if (!string.IsNullOrEmpty(statAction))
						{
							ClientStats.SendMiscellaneousStatsAsync(sourceLocation, RegistryManager.Instance.UserGuid, "UnlockMouseCursor", statAction, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, null);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ClipMouseCursorHandler. Exception: " + ex.ToString());
			}
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x00074C04 File Offset: 0x00072E04
		internal string GetShortcutKeyFromName(string shortcutName, bool isBossKey = false)
		{
			try
			{
				if (this.mShortcutsConfigInstance != null)
				{
					using (List<ShortcutKeys>.Enumerator enumerator = this.mShortcutsConfigInstance.Shortcut.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ShortcutKeys shortcutKeys = enumerator.Current;
							if (string.Equals(shortcutKeys.ShortcutName, shortcutName, StringComparison.InvariantCulture))
							{
								if (isBossKey)
								{
									return shortcutKeys.ShortcutKey;
								}
								string[] array = shortcutKeys.ShortcutKey.Split(new char[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
								string text = string.Empty;
								foreach (string text2 in array)
								{
									text = text + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text2), "") + " + ";
								}
								if (!string.IsNullOrEmpty(text))
								{
									return text.Substring(0, text.Length - 3);
								}
							}
						}
						goto IL_013B;
					}
					goto IL_00E8;
					IL_013B:
					goto IL_0157;
				}
				IL_00E8:
				string text3;
				if (shortcutName != null)
				{
					if (shortcutName == "STRING_TOGGLE_LOCK_CURSOR")
					{
						text3 = "Ctrl + Shift + F8";
						goto IL_0136;
					}
					if (shortcutName == "STRING_TOGGLE_KEYMAP_WINDOW")
					{
						text3 = "Ctrl + Shift + H";
						goto IL_0136;
					}
					if (shortcutName == "STRING_TOGGLE_OVERLAY")
					{
						text3 = "Ctrl + Shift + F6";
						goto IL_0136;
					}
				}
				text3 = "";
				IL_0136:
				return text3;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetShortcutKeyFromName: " + ex.ToString());
			}
			IL_0157:
			return "";
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x00074DA4 File Offset: 0x00072FA4
		internal static void SaveMacroJson(MacroRecording record, string destFileName)
		{
			try
			{
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = Formatting.Indented;
				string text = JsonConvert.SerializeObject(record, serializerSettings);
				if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
				{
					Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
				}
				File.WriteAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, Path.GetFileName(destFileName.ToLower(CultureInfo.InvariantCulture).Trim())), text);
			}
			catch (Exception ex)
			{
				Logger.Error("Could not serialize the macro recording object. Ex: {0}", new object[] { ex });
			}
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x00074E2C File Offset: 0x0007302C
		internal void ToggleMacroAndSyncVisibility()
		{
			try
			{
				if (FeatureManager.Instance.ForceEnableMacroAndSync)
				{
					this.OnMacroButtonVisibilityChanged(true);
					this.OnOperationSyncButtonVisibilityChanged(true);
				}
				else if (FeatureManager.Instance.IsMacroRecorderEnabled || FeatureManager.Instance.IsOperationsSyncEnabled)
				{
					bool flag = this.CheckForMacroVisibility();
					if (FeatureManager.Instance.IsMacroRecorderEnabled)
					{
						this.OnMacroButtonVisibilityChanged(flag);
					}
					if (FeatureManager.Instance.IsOperationsSyncEnabled)
					{
						this.OnOperationSyncButtonVisibilityChanged(flag);
					}
				}
				else
				{
					this.OnMacroButtonVisibilityChanged(false);
					this.OnOperationSyncButtonVisibilityChanged(false);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ToggleMacroAndSyncVisibility: " + ex.ToString());
			}
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x00074ED4 File Offset: 0x000730D4
		private void ToggleCursorStyle(bool enable)
		{
			try
			{
				Dictionary<string, string> data = new Dictionary<string, string>();
				if (enable)
				{
					data.Add("path", RegistryStrings.CursorPath);
				}
				else
				{
					data.Add("path", string.Empty);
				}
				ThreadPool.QueueUserWorkItem(delegate(object obj1)
				{
					try
					{
						HTTPUtils.SendRequestToEngine("setCursorStyle", data, this.ParentWindow.mVmName, 3000, null, false, 1, 0, "");
						this.SetDefaultCursorForClient();
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
						this.SetDefaultCursorForClient();
					}
				});
			}
			catch (Exception)
			{
				this.SetDefaultCursorForClient();
			}
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x0000D91F File Offset: 0x0000BB1F
		private void SetDefaultCursorForClient()
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj1)
			{
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					try
					{
						Mouse.OverrideCursor = null;
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to set default cursor for client... err : " + ex.ToString());
					}
				}), new object[0]);
			});
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x0000D933 File Offset: 0x0000BB33
		public void LaunchSettingsWindow(string tabName = "")
		{
			if (MainWindow.SettingsWindow == null)
			{
				MainWindow.OpenSettingsWindow(this.ParentWindow, tabName);
			}
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0000D948 File Offset: 0x0000BB48
		public void DMMSwitchKeyMapButtonHandler()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName.EndsWith("_off", StringComparison.InvariantCulture))
				{
					this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName.Replace("_off", string.Empty);
					BlueStacksUIBinding.Bind(this.ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
					BlueStacksUIBinding.Bind(this.ParentWindow.mDMMFST.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
					this.ParentWindow.mFrontendHandler.EnableKeyMapping(true);
					this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.EnableKeymapForDMM(true);
				}
				else
				{
					CustomPictureBox mKeyMapSwitch = this.ParentWindow.mDmmBottomBar.mKeyMapSwitch;
					mKeyMapSwitch.ImageName += "_off";
					BlueStacksUIBinding.Bind(this.ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_DISABLED");
					BlueStacksUIBinding.Bind(this.ParentWindow.mDMMFST.mKeyMapSwitch, "STRING_KEYMAPPING_DISABLED");
					this.ParentWindow.mFrontendHandler.EnableKeyMapping(false);
					this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.EnableKeymapForDMM(false);
				}
				this.ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName;
			}), new object[0]);
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x00074F58 File Offset: 0x00073158
		public void SetDMMKeymapButtonsAndTransparency()
		{
			if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsDMMKeymapUIVisible)
			{
				this.ParentWindow.mCommonHandler.EnableKeymapButtonsForDmm(Visibility.Visible);
				this.ParentWindow.mDmmBottomBar.ShowKeyMapPopup(true);
				KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
				BlueStacksUIBinding.Bind(this.ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
				if (this.ParentWindow.mDmmBottomBar.CurrentTransparency > 0.0)
				{
					this.SetTranslucentControlsBtnImageForDMM("eye");
				}
				else
				{
					this.SetTranslucentControlsBtnImageForDMM("eye_off");
				}
			}
			else
			{
				this.ParentWindow.mCommonHandler.EnableKeymapButtonsForDmm(Visibility.Collapsed);
				this.ParentWindow.mDmmBottomBar.ShowKeyMapPopup(false);
				KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
			}
			if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsDMMKeymapEnabled)
			{
				this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = "keymapswitch";
				this.ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = "keymapswitch";
				this.ParentWindow.mFrontendHandler.EnableKeyMapping(true);
				return;
			}
			this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = "keymapswitch_off";
			this.ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = "keymapswitch_off";
			this.ParentWindow.mFrontendHandler.EnableKeyMapping(false);
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x000750D8 File Offset: 0x000732D8
		public void EnableKeymapButtonsForDmm(Visibility isVisible)
		{
			this.ParentWindow.mDmmBottomBar.mKeyMapButton.Visibility = isVisible;
			this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.Visibility = isVisible;
			this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.Visibility = isVisible;
			this.ParentWindow.mDMMFST.mKeyMapButton.Visibility = isVisible;
			this.ParentWindow.mDMMFST.mKeyMapSwitch.Visibility = isVisible;
			this.ParentWindow.mDMMFST.mTranslucentControlsButton.Visibility = isVisible;
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x0007516C File Offset: 0x0007336C
		internal void SetTranslucentControlsBtnImageForDMM(string imageName)
		{
			this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName = imageName;
			this.ParentWindow.mDmmBottomBar.mTranslucentControlsSliderButton.ImageName = this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
			this.ParentWindow.mDMMFST.mTranslucentControlsButton.ImageName = this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
			this.ParentWindow.mDMMFST.mTranslucentControlsSliderButton.ImageName = this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x00075210 File Offset: 0x00073410
		internal void KeyMapButtonHandler(string action, string location)
		{
			KMManager.ShowAdvancedSettings(this.ParentWindow);
			ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "KeyMap", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x00075260 File Offset: 0x00073460
		public void DMMScreenshotHandler()
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
			{
				ShowNewFolderButton = true,
				Description = LocaleStrings.GetLocalizedString("STRING_CHOOSE_SCREENSHOT_FOLDER_TEXT", "")
			})
			{
				if (folderBrowserDialog.ShowDialog(Utils.GetIWin32Window(this.ParentWindow.Handle)) == DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
				{
					string text = (Directory.Exists(folderBrowserDialog.SelectedPath) ? folderBrowserDialog.SelectedPath : RegistryStrings.ScreenshotDefaultPath);
					RegistryManager.Instance.ScreenShotsPath = text;
				}
			}
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x000752F8 File Offset: 0x000734F8
		public void RecordVideoOfApp()
		{
			Logger.Debug("OBS start or stop status: {0}", new object[] { CommonHandlers.sIsOBSStartingStopping });
			if (!CommonHandlers.sIsOBSStartingStopping)
			{
				CommonHandlers.sIsOBSStartingStopping = true;
				if (RegistryManager.Instance.IsScreenshotsLocationPopupEnabled)
				{
					this.ShowScreenShotFolderUpdatePopup();
				}
				string text = RegistryManager.Instance.ScreenShotsPath;
				if (!StringExtensions.IsValidPath(text))
				{
					if (!Directory.Exists(RegistryStrings.ScreenshotDefaultPath))
					{
						Directory.CreateDirectory(RegistryStrings.ScreenshotDefaultPath);
					}
					RegistryManager.Instance.ScreenShotsPath = RegistryStrings.ScreenshotDefaultPath;
					text = RegistryStrings.ScreenshotDefaultPath;
				}
				string text2 = DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss.ff", CultureInfo.InvariantCulture);
				string text3 = string.Format(CultureInfo.InvariantCulture, Strings.ProductTopBarDisplayName + "_Recording_{0}.mp4", new object[] { text2 });
				string filePath = Path.Combine(text, text3);
				if (text == RegistryStrings.ScreenshotDefaultPath && !Directory.Exists(RegistryStrings.ScreenshotDefaultPath))
				{
					Directory.CreateDirectory(RegistryStrings.ScreenshotDefaultPath);
				}
				ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingStarting", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				Action <>9__1;
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						CommonHandlers.sRecordingInstance = this.ParentWindow.mVmName;
						if (StreamManager.Instance == null)
						{
							StreamManager.Instance = new StreamManager(this.ParentWindow);
						}
						string text4 = this.ParentWindow.mFrontendHandler.mFrontendHandle.ToString();
						Dispatcher dispatcher = this.ParentWindow.Dispatcher;
						Action action;
						if ((action = <>9__1) == null)
						{
							action = (<>9__1 = delegate
							{
								this.ParentWindow.RestrictWindowResize(true);
								this.OnScreenRecorderStateTransitioning();
								this.StartLoadingTimeoutTimer();
							});
						}
						dispatcher.Invoke(action, new object[0]);
						Process currentProcess = Process.GetCurrentProcess();
						StreamManager.Instance.Init(text4, currentProcess.Id.ToString(CultureInfo.InvariantCulture));
						StreamManager.sStopInitOBSQueue = false;
						try
						{
							StreamManager.Instance.StartObs();
						}
						catch (Exception ex)
						{
							Logger.Error("Exception in StartObs: {0}", new object[] { ex });
							this.ShowErrorRecordingVideoPopup();
							return;
						}
						StreamManager.Instance.SetMicVolume("0");
						StreamManager.Instance.SetHwnd(text4);
						StreamManager.Instance.SetSavePath(filePath);
						CommonHandlers.mSavedVideoRecordingFilePath = filePath;
						StreamManager.Instance.EnableVideoRecording(true);
						StreamManager.Instance.StartRecordForVideo();
						CommonHandlers.sIsRecordingVideo = true;
					}
					catch (Exception ex2)
					{
						Logger.Error("Error in RecordVideoOfApp: {0}", new object[] { ex2 });
					}
				});
				return;
			}
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x00075454 File Offset: 0x00073654
		private void StartLoadingTimeoutTimer()
		{
			if (this.mObsResponseTimeoutTimer == null)
			{
				this.mObsResponseTimeoutTimer = new global::System.Timers.Timer(20000.0);
				this.mObsResponseTimeoutTimer.Elapsed += this.ObsResponseTimeoutTimer_Elapsed;
				this.mObsResponseTimeoutTimer.AutoReset = false;
			}
			if (!this.mObsResponseTimeoutTimer.Enabled)
			{
				this.mObsResponseTimeoutTimer.Start();
			}
		}

		// Token: 0x0600133A RID: 4922 RVA: 0x000754B8 File Offset: 0x000736B8
		private void ObsResponseTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.OnOBSResponseTimeout();
			CommonHandlers.sIsRecordingVideo = false;
			CommonHandlers.sIsOBSStartingStopping = false;
			CommonHandlers.sRecordingInstance = "";
			this.ParentWindow.RestrictWindowResize(false);
			if (StreamManager.Instance != null)
			{
				StreamManager.Instance.ShutDownForcefully();
			}
			this.ShowErrorRecordingVideoPopup();
		}

		// Token: 0x0600133B RID: 4923 RVA: 0x00075504 File Offset: 0x00073704
		internal void StopRecordVideo()
		{
			try
			{
				this.OnScreenRecorderStateTransitioning();
				this.StartLoadingTimeoutTimer();
				StreamManager.Instance.StopRecord();
			}
			catch (Exception ex)
			{
				Logger.Error("error in stop record video : {0}", new object[] { ex });
			}
		}

		// Token: 0x0600133C RID: 4924 RVA: 0x00075550 File Offset: 0x00073750
		internal void RecordingStopped()
		{
			global::System.Timers.Timer timer = this.mObsResponseTimeoutTimer;
			if (timer != null)
			{
				timer.Stop();
			}
			this.ParentWindow.RestrictWindowResize(false);
			this.OnScreenRecordingStateChanged(false);
			ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingDone", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x000755C4 File Offset: 0x000737C4
		internal void DownloadAndLaunchRecording(string location, string action)
		{
			Logger.Debug("value of sRecordingInstance: {0} and sIsRecordingVideo: {1}", new object[]
			{
				CommonHandlers.sRecordingInstance,
				CommonHandlers.sIsRecordingVideo
			});
			if (!CommonHandlers.sIsRecordingVideo)
			{
				if (Directory.Exists(RegistryStrings.ObsDir) && File.Exists(RegistryStrings.ObsBinaryPath))
				{
					if (!RegistryManager.Instance.IsBTVCheckedAfterUpdate && !CommonHandlers.IsBtvLatestVersionDownloaded())
					{
						this.DownloadObsPopup();
						ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingDownload", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					}
					else if (!ProcessUtils.FindProcessByName("HD-OBS"))
					{
						if (!this.InsufficientSpacePopup())
						{
							this.RecordVideoOfApp();
							ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingStart", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						}
					}
					else
					{
						this.ShowAlreadyRunningPopUpForOBS();
					}
				}
				else
				{
					this.DownloadObsPopup();
					ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingDownload", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				}
				RegistryManager.Instance.IsBTVCheckedAfterUpdate = true;
				return;
			}
			if (string.Equals(CommonHandlers.sRecordingInstance, this.ParentWindow.mVmName, StringComparison.InvariantCulture))
			{
				this.StopRecordVideo();
				ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingStop", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				return;
			}
			this.ShowAlreadyRunningPopUpForOBS();
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x00075770 File Offset: 0x00073970
		private bool InsufficientSpacePopup()
		{
			bool recording = true;
			double num = CommonHandlers.FindAvailableSpaceinMB(RegistryManager.Instance.ScreenShotsPath);
			EventHandler <>9__0;
			MouseButtonEventHandler <>9__1;
			while (num < 30.0 && recording)
			{
				RegistryManager.Instance.IsScreenshotsLocationPopupEnabled = false;
				string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_INSUFFICIENT_SPACE", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHANGE_PATH", new EventHandler(this.ChooseCustomFolder), null, false, null);
				ButtonColors buttonColors = ButtonColors.White;
				string text = "STRING_STOP_RECORDING";
				EventHandler eventHandler;
				if ((eventHandler = <>9__0) == null)
				{
					eventHandler = (<>9__0 = delegate(object o, EventArgs evt)
					{
						recording = false;
					});
				}
				customMessageWindow.AddButton(buttonColors, text, eventHandler, null, false, null);
				UIElement closeButton = customMessageWindow.CloseButton;
				MouseButtonEventHandler mouseButtonEventHandler;
				if ((mouseButtonEventHandler = <>9__1) == null)
				{
					mouseButtonEventHandler = (<>9__1 = delegate(object o, MouseButtonEventArgs evt)
					{
						recording = false;
					});
				}
				closeButton.PreviewMouseUp += mouseButtonEventHandler;
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_INSUFFICIENT_RECORDING_SPACE", "");
				customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
				customMessageWindow.BodyWarningTextBlock.Text = screenShotsPath;
				BlueStacksUIBinding.BindColor(customMessageWindow.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
				num = CommonHandlers.FindAvailableSpaceinMB(RegistryManager.Instance.ScreenShotsPath);
			}
			return !recording;
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x000758E4 File Offset: 0x00073AE4
		private static double FindAvailableSpaceinMB(string path)
		{
			double num = double.MaxValue;
			string pathRoot = Path.GetPathRoot(path);
			double num2 = Math.Pow(2.0, 20.0);
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				if (driveInfo.IsReady && driveInfo.Name == pathRoot)
				{
					num = (double)driveInfo.AvailableFreeSpace / num2;
				}
			}
			return num;
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x0007595C File Offset: 0x00073B5C
		private void ShowAlreadyRunningPopUpForOBS()
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_NOT_START_RECORDER", "");
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_RECORDER_ALREADY_RUNNING", "");
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
			customMessageWindow.Owner = this.ParentWindow;
			this.ParentWindow.ShowDimOverlay(null);
			customMessageWindow.ShowDialog();
			this.ParentWindow.HideDimOverlay();
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x000759D4 File Offset: 0x00073BD4
		private static bool IsBtvLatestVersionDownloaded()
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(new Uri(CommonHandlers.GetBtvUrl()).LocalPath);
			return string.Compare(RegistryManager.Instance.CurrentBtvVersionInstalled, fileNameWithoutExtension, StringComparison.InvariantCulture) >= 0;
		}

		// Token: 0x06001342 RID: 4930 RVA: 0x00075A10 File Offset: 0x00073C10
		private static string GetBtvUrl()
		{
			string text = WebHelper.GetUrlWithParams(RegistryManager.Instance.Host + "/bs4/btv/GetBTVFile");
			if (!string.IsNullOrEmpty(RegistryManager.Instance.BtvDevServer))
			{
				text = RegistryManager.Instance.BtvDevServer;
			}
			return BTVManager.GetRedirectedUrl(text);
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x00075A5C File Offset: 0x00073C5C
		private void DownloadObsPopup()
		{
			if (CommonHandlers.sDownloading && CommonHandlers.sWindow != null && !CommonHandlers.sWindow.IsClosed)
			{
				this.DownloadObs(null, null);
				return;
			}
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RECORDER_REQUIRED", "");
				customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_VIDEO_RECORDER_DOWNLOAD_BODY", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_DOWNLOAD_NOW", new EventHandler(this.DownloadObs), null, false, null);
				customMessageWindow.Owner = this.ParentWindow;
				customMessageWindow.ContentMaxWidth = 450.0;
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
			}), new object[0]);
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x00075AB0 File Offset: 0x00073CB0
		private void DownloadObs(object sender, EventArgs e)
		{
			if (CommonHandlers.sDownloading && CommonHandlers.sWindow != null && !CommonHandlers.sWindow.IsClosed)
			{
				BTVManager.BringToFront(CommonHandlers.sWindow);
				return;
			}
			if (!BTVManager.IsDirectXComponentsInstalled())
			{
				CustomMessageWindow downloadReqWindow = new CustomMessageWindow();
				downloadReqWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ADDITIONAL_FILES_REQUIRED", "");
				downloadReqWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SOME_WINDOW_FILES_MISSING", "");
				string directXDownloadURL = "http://www.microsoft.com/en-us/download/details.aspx?id=35";
				downloadReqWindow.AddHyperLinkInUI(directXDownloadURL, new Uri(directXDownloadURL), delegate(object o, RequestNavigateEventArgs arg)
				{
					BlueStacksUIUtils.OpenUrl(arg.Uri.ToString());
					downloadReqWindow.CloseWindow();
				});
				downloadReqWindow.AddButton(ButtonColors.Blue, "STRING_DOWNLOAD_NOW", delegate(object o, EventArgs args)
				{
					BlueStacksUIUtils.OpenUrl(directXDownloadURL);
				}, null, false, null);
				downloadReqWindow.Owner = this.ParentWindow;
				downloadReqWindow.ContentMaxWidth = 450.0;
				this.ParentWindow.ShowDimOverlay(null);
				downloadReqWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
				return;
			}
			CommonHandlers.sDownloading = true;
			CommonHandlers.sWindow = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(CommonHandlers.sWindow.TitleTextBlock, "STRING_DOWNLOAD_ADDITIONAL", "");
			BlueStacksUIBinding.Bind(CommonHandlers.sWindow.BodyWarningTextBlock, "STRING_NOT_CLOSE_DOWNLOAD_COMPLETE", "");
			CommonHandlers.sWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
			CommonHandlers.sWindow.BodyTextBlock.Visibility = Visibility.Collapsed;
			CommonHandlers.sWindow.CloseButtonHandle(new Predicate<object>(this.RecorderDownloadCancelledHandler), null);
			CustomMessageWindow customMessageWindow = CommonHandlers.sWindow;
			customMessageWindow.MinimizeEventHandler = (EventHandler)Delegate.Combine(customMessageWindow.MinimizeEventHandler, new EventHandler(this.BtvDownloadWindowMinimizedHandler));
			CommonHandlers.sWindow.ProgressBarEnabled = true;
			CommonHandlers.sWindow.IsWindowMinizable = true;
			CommonHandlers.sWindow.IsWindowClosable = true;
			CommonHandlers.sWindow.ShowInTaskbar = false;
			CommonHandlers.sWindow.IsWithoutButtons = true;
			CommonHandlers.sWindow.ContentMaxWidth = 450.0;
			CommonHandlers.sWindow.IsDraggable = true;
			CommonHandlers.sWindow.Owner = this.ParentWindow;
			CommonHandlers.sWindow.IsShowGLWindow = true;
			CommonHandlers.sWindow.Show();
			new Thread(delegate
			{
				string btvUrl = CommonHandlers.GetBtvUrl();
				if (btvUrl == null)
				{
					Logger.Error("The download url was null");
					this.ShowErrorDownloadingRecorder();
					return;
				}
				string fileName = Path.GetFileName(new Uri(btvUrl).LocalPath);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(new Uri(btvUrl).LocalPath);
				string downloadPath = Path.Combine(Path.GetTempPath(), fileName);
				this.mDownloader = new LegacyDownloader(3, btvUrl, downloadPath);
				this.mDownloader.Download(delegate(int percent)
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						if (CommonHandlers.sWindow == null)
						{
							return;
						}
						CommonHandlers.sWindow.CustomProgressBar.Value = (double)percent;
					}), new object[0]);
				}, delegate(string filePath)
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						if (CommonHandlers.sWindow == null)
						{
							return;
						}
						CommonHandlers.sWindow.CustomProgressBar.Value = 100.0;
					}), new object[0]);
					Logger.Info("Successfully downloaded BlueStacks TV");
					RegistryManager.Instance.CurrentBtvVersionInstalled = fileNameWithoutExtension;
					if (BTVManager.ExtractBTv(downloadPath))
					{
						Utils.DeleteFile(downloadPath);
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							CustomMessageWindow customMessageWindow2 = CommonHandlers.sWindow;
							if (customMessageWindow2 != null)
							{
								customMessageWindow2.Close();
							}
							CommonHandlers.sWindow = null;
							if (this.ParentWindow.mClosed)
							{
								return;
							}
							CustomMessageWindow customMessageWindow3 = new CustomMessageWindow();
							BlueStacksUIBinding.Bind(customMessageWindow3.TitleTextBlock, "STRING_RECORDER_DOWNLOADED", "");
							BlueStacksUIBinding.Bind(customMessageWindow3.BodyTextBlock, "STRING_RECORDER_READY_BODY", "");
							customMessageWindow3.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
							customMessageWindow3.Owner = this.ParentWindow;
							customMessageWindow3.ContentMaxWidth = 450.0;
							this.ParentWindow.ShowDimOverlay(null);
							customMessageWindow3.ShowDialog();
							this.ParentWindow.HideDimOverlay();
						}), new object[0]);
						return;
					}
					Utils.DeleteFile(downloadPath);
					this.ShowErrorDownloadingRecorder();
				}, delegate(Exception ex)
				{
					Logger.Error("Failed to download file: {0}. err: {1}", new object[] { downloadPath, ex.Message });
					if (ex.InnerException is OperationCanceledException)
					{
						return;
					}
					this.ShowErrorDownloadingRecorder();
				}, null, delegate(long size)
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						if (CommonHandlers.sWindow == null)
						{
							return;
						}
						CommonHandlers.sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOADING", "");
						CommonHandlers.sWindow.ProgressPercentageTextBlock.Content = ((float)size / 1048576f).ToString("F", CultureInfo.InvariantCulture) + " MB / " + this.mRecorderSizeMb.ToString("F", CultureInfo.InvariantCulture) + " MB ";
						this.mDownloadedSize = size;
					}), new object[0]);
				}, delegate(long size)
				{
					this.mRecorderSizeMb = (float)size / 1048576f;
				});
				CommonHandlers.sDownloading = false;
			})
			{
				IsBackground = true
			}.Start();
			this.mDownloadStatusTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 5)
			};
			this.mDownloadStatusTimer.Tick += this.DownloadStatusTimerTick;
			this.mDownloadStatusTimer.Start();
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x0000D96D File Offset: 0x0000BB6D
		private void BtvDownloadWindowMinimizedHandler(object sender, EventArgs e)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.OnBTvDownloaderMinimized();
				this.ParentWindow.Focus();
			}), new object[0]);
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x00075D48 File Offset: 0x00073F48
		private void DownloadStatusTimerTick(object sender, EventArgs e)
		{
			if ((!CommonHandlers.sDownloading && CommonHandlers.sWindow != null) || CommonHandlers.sWindow == null)
			{
				this.mDownloadStatusTimer.Stop();
				return;
			}
			try
			{
				if (this.mLastSizeChecked != this.mDownloadedSize)
				{
					this.mLastSizeChecked = this.mDownloadedSize;
					CommonHandlers.sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOADING", "");
				}
				else
				{
					CommonHandlers.sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_WAITING_FOR_INTERNET", "");
				}
			}
			catch (Exception ex)
			{
				string text = "Exception in DownloadStatusTimerTick. Exception: ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x0000D992 File Offset: 0x0000BB92
		private void ShowErrorDownloadingRecorder()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DOWNLOAD_FAILED", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_ERROR_RECORDER_DOWNLOAD", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CLOSE", null, null, false, null);
				customMessageWindow.Owner = this.ParentWindow;
				customMessageWindow.ContentMaxWidth = 450.0;
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
				CustomMessageWindow customMessageWindow2 = CommonHandlers.sWindow;
				if (customMessageWindow2 != null)
				{
					customMessageWindow2.Close();
				}
				CommonHandlers.sWindow = null;
			}), new object[0]);
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x0000D9B7 File Offset: 0x0000BBB7
		internal void ShowErrorRecordingVideoPopup()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RECORDING_ERROR", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_RECORDING_ERROR_BODY", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
				customMessageWindow.Owner = this.ParentWindow;
				customMessageWindow.ContentMaxWidth = 450.0;
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
			}), new object[0]);
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x00075E00 File Offset: 0x00074000
		private bool RecorderDownloadCancelledHandler(object sender)
		{
			CustomMessageWindow cancelDownloadConfirmation = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(cancelDownloadConfirmation.TitleTextBlock, "STRING_DOWNLOAD_IN_PROGRESS", "");
			BlueStacksUIBinding.Bind(cancelDownloadConfirmation.BodyTextBlock, "STRING_DOWNLOAD_NOT_COMPLETE", "");
			cancelDownloadConfirmation.AddButton(ButtonColors.Red, "STRING_CANCEL", delegate(object o, EventArgs args)
			{
				CustomMessageWindow customMessageWindow = CommonHandlers.sWindow;
				if (customMessageWindow != null)
				{
					customMessageWindow.Close();
				}
				CommonHandlers.sWindow = null;
				LegacyDownloader legacyDownloader = this.mDownloader;
				if (legacyDownloader == null)
				{
					return;
				}
				legacyDownloader.AbortDownload();
			}, null, false, null);
			cancelDownloadConfirmation.AddButton(ButtonColors.White, "STRING_CONTINUE", delegate(object o, EventArgs args)
			{
				cancelDownloadConfirmation.DialogResult = new bool?(true);
			}, null, false, null);
			cancelDownloadConfirmation.Owner = this.ParentWindow;
			this.ParentWindow.ShowDimOverlay(null);
			bool? flag = cancelDownloadConfirmation.ShowDialog();
			this.ParentWindow.HideDimOverlay();
			bool? flag2 = flag;
			bool flag3 = true;
			return (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x00075EE4 File Offset: 0x000740E4
		public void RecordingStarted()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				CommonHandlers.sIsOBSStartingStopping = false;
				global::System.Timers.Timer timer = this.mObsResponseTimeoutTimer;
				if (timer != null)
				{
					timer.Stop();
				}
				this.OnScreenRecordingStateChanged(true);
				if (RegistryManager.Instance.IsShowToastNotification)
				{
					this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_RECORDING_STARTED", ""));
				}
			}), new object[0]);
			ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingStarted", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x00075F58 File Offset: 0x00074158
		public void StopMacroRecording()
		{
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopRecordingCombo", null);
			foreach (object obj in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
			{
				CommonHandlers.EnableScriptControl((SingleMacroControl)obj);
			}
			this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
			this.ParentWindow.MacroRecorderWindow.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
			this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Visibility = Visibility.Visible;
			this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.IsOpen = false;
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.ParentWindow.mNCTopBar.mMacroRecordGrid.Visibility = Visibility.Collapsed;
				this.ParentWindow.mNCTopBar.mMacroRecordControl.StopTimer();
				return;
			}
			this.ParentWindow.mTopBar.mMacroRecordControl.Visibility = Visibility.Collapsed;
			this.ParentWindow.mTopBar.mMacroRecordControl.StopTimer();
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x00076090 File Offset: 0x00074290
		public void StartMacroRecording()
		{
			this.ParentWindow.mIsMacroRecorderActive = true;
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.ParentWindow.mNCTopBar.ShowRecordingIcons();
			}
			else
			{
				this.ParentWindow.mTopBar.ShowRecordingIcons();
			}
			this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Collapsed;
			this.ParentWindow.MacroRecorderWindow.mStopMacroRecordingBtn.Visibility = Visibility.Visible;
			foreach (object obj in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
			{
				CommonHandlers.DisableScriptControl((SingleMacroControl)obj);
			}
			this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
			this.ParentWindow.Focus();
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startRecordingCombo", null);
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x00076190 File Offset: 0x00074390
		internal void InitUiOnMacroPlayback(MacroRecording recording)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.Focus();
				this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.IsEnabled = false;
				this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Opacity = 0.6;
				if (FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					this.ParentWindow.mNCTopBar.ShowMacroPlaybackOnTopBar(recording);
					this.ParentWindow.mNCTopBar.mMacroPlayControl.mStartTime = DateTime.Now;
				}
				else
				{
					this.ParentWindow.mTopBar.ShowMacroPlaybackOnTopBar(recording);
					this.ParentWindow.mTopBar.mMacroPlayControl.mStartTime = DateTime.Now;
				}
				this.ParentWindow.mMacroPlaying = recording.Name;
				if (recording.RestartPlayer)
				{
					this.ParentWindow.StartTimerForAppPlayerRestart(recording.RestartPlayerAfterMinutes);
				}
			}), new object[0]);
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x000761D4 File Offset: 0x000743D4
		internal void PlayMacroScript(MacroRecording record)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
				this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.IsEnabled = false;
				this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Opacity = 0.6;
				foreach (object obj in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
				{
					SingleMacroControl singleMacroControl = (SingleMacroControl)obj;
					if (singleMacroControl.mRecording.Name != record.Name)
					{
						CommonHandlers.DisableScriptControl(singleMacroControl);
					}
					else
					{
						singleMacroControl.mEditNameImg.IsEnabled = false;
					}
				}
				this.ParentWindow.MacroRecorderWindow.RunMacroOperation(record);
			}), new object[0]);
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x00076218 File Offset: 0x00074418
		internal void FullMacroScriptPlayHandler(MacroRecording record)
		{
			string name = record.Name;
			this.ParentWindow.mCommonHandler.PlayMacroScript(record);
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptPlayEvent(name);
				return;
			}
			this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptPlayEvent(name);
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x0000D9DC File Offset: 0x0000BBDC
		internal void StopMacroScriptHandling()
		{
			this.ParentWindow.MacroRecorderWindow.mBGMacroPlaybackWorker.CancelAsync();
			this.StopMacroPlaybackOperation();
			this.ParentWindow.SetMacroPlayBackEventHandle();
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x00076278 File Offset: 0x00074478
		internal void StopMacroPlaybackOperation()
		{
			Logger.Info("In StopMacroPlaybackOperation");
			this.ParentWindow.mIsMacroPlaying = false;
			foreach (object obj in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
			{
				CommonHandlers.EnableScriptControl((SingleMacroControl)obj);
			}
			this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
			this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.IsEnabled = true;
			this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Opacity = 1.0;
			this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Visibility = Visibility.Visible;
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.ParentWindow.mNCTopBar.HideMacroPlaybackFromTopBar();
			}
			else
			{
				this.ParentWindow.mTopBar.HideMacroPlaybackFromTopBar();
			}
			this.ParentWindow.mMacroPlaying = string.Empty;
			if (this.ParentWindow.mMacroTimer != null && this.ParentWindow.mMacroTimer.Enabled)
			{
				this.ParentWindow.mMacroTimer.Enabled = false;
				this.ParentWindow.mMacroTimer.AutoReset = false;
				this.ParentWindow.mMacroTimer.Dispose();
			}
			this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.IsOpen = false;
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopMacroPlayback", null);
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x00076410 File Offset: 0x00074610
		public static void EnableScriptControl(SingleMacroControl mScriptControl)
		{
			mScriptControl.Opacity = 1.0;
			mScriptControl.mBookmarkImg.IsEnabled = true;
			mScriptControl.mEditNameImg.IsEnabled = true;
			mScriptControl.mPlayScriptImg.IsEnabled = true;
			mScriptControl.mScriptSettingsImg.IsEnabled = true;
			mScriptControl.mDeleteScriptImg.IsEnabled = true;
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x00076468 File Offset: 0x00074668
		public static void DisableScriptControl(SingleMacroControl mScriptControl)
		{
			mScriptControl.Opacity = 0.4;
			mScriptControl.mBookmarkImg.IsEnabled = false;
			mScriptControl.mEditNameImg.IsEnabled = false;
			mScriptControl.mPlayScriptImg.IsEnabled = false;
			mScriptControl.mScriptSettingsImg.IsEnabled = false;
			mScriptControl.mDeleteScriptImg.IsEnabled = false;
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x000764C0 File Offset: 0x000746C0
		internal void CheckForMacroScriptOnRestart()
		{
			foreach (MacroRecording macroRecording in from MacroRecording macro in MacroGraph.Instance.Vertices
				where macro.PlayOnStart
				select macro)
			{
				this.InitUiAndPlayMacroScript(macroRecording);
			}
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x0007653C File Offset: 0x0007473C
		private void InitUiAndPlayMacroScript(MacroRecording record)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.RefreshMacroRecorderWindow();
				this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptPlayEvent(record.Name.ToLower(CultureInfo.InvariantCulture));
				this.PlayMacroScript(record);
			}), new object[0]);
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x00076580 File Offset: 0x00074780
		public static void OpenMediaFolder()
		{
			if (Directory.Exists(RegistryManager.Instance.ScreenShotsPath))
			{
				using (Process process = new Process())
				{
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.FileName = RegistryManager.Instance.ScreenShotsPath;
					process.Start();
				}
			}
		}

		// Token: 0x06001357 RID: 4951 RVA: 0x000765E8 File Offset: 0x000747E8
		public static void OpenMediaFolderWithFileSelected(string selectedFile)
		{
			if (Directory.Exists(RegistryManager.Instance.ScreenShotsPath))
			{
				using (Process process = new Process())
				{
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.FileName = "explorer.exe";
					process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "/select,\"{0}\"", new object[] { selectedFile });
					process.Start();
				}
			}
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x00076670 File Offset: 0x00074870
		internal void SetSidebarImageProperties(bool isVisible, CustomPictureBox cpb, TextBlock tb)
		{
			if (isVisible)
			{
				if (cpb != null)
				{
					cpb.ImageName = "sidebar_hide";
					BlueStacksUIBinding.Bind(cpb, "STRING_CLOSE_SIDEBAR");
				}
				if (tb != null)
				{
					BlueStacksUIBinding.Bind(tb, "STRING_CLOSE_SIDEBAR", "");
					return;
				}
			}
			else
			{
				if (cpb != null)
				{
					cpb.ImageName = "sidebar_show";
					BlueStacksUIBinding.Bind(cpb, "STRING_OPEN_SIDEBAR");
				}
				if (tb != null)
				{
					BlueStacksUIBinding.Bind(tb, "STRING_OPEN_SIDEBAR", "");
				}
			}
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x000766DC File Offset: 0x000748DC
		internal void FlipSidebarVisibility(CustomPictureBox cpb, TextBlock tb)
		{
			if (cpb.ImageName == "sidebar_hide")
			{
				this.ParentWindow.mSidebar.Visibility = Visibility.Collapsed;
				cpb.ImageName = "sidebar_show";
				BlueStacksUIBinding.Bind(cpb, "STRING_OPEN_SIDEBAR");
				if (tb != null)
				{
					BlueStacksUIBinding.Bind(tb, "STRING_OPEN_SIDEBAR", "");
				}
				this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible = false;
			}
			else
			{
				this.ParentWindow.mSidebar.Visibility = Visibility.Visible;
				cpb.ImageName = "sidebar_hide";
				BlueStacksUIBinding.Bind(cpb, "STRING_CLOSE_SIDEBAR");
				if (tb != null)
				{
					BlueStacksUIBinding.Bind(tb, "STRING_CLOSE_SIDEBAR", "");
				}
				this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible = true;
			}
			this.ParentWindow.mSidebar.SidebarVisiblityChanged(this.ParentWindow.mSidebar.Visibility);
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x000767B4 File Offset: 0x000749B4
		internal void InitShortcuts()
		{
			try
			{
				this.mShortcutsConfigInstance = ShortcutConfig.LoadShortcutsConfig();
				if (this.mShortcutsConfigInstance != null)
				{
					List<ShortcutKeys> list = new List<ShortcutKeys>();
					foreach (ShortcutKeys shortcutKeys in this.mShortcutsConfigInstance.Shortcut)
					{
						if (string.Equals(shortcutKeys.ShortcutName, "STRING_MACRO_RECORDER", StringComparison.InvariantCulture))
						{
							if (!FeatureManager.Instance.IsMacroRecorderEnabled && !FeatureManager.Instance.IsCustomUIForNCSoft)
							{
								list.Add(shortcutKeys);
							}
						}
						else if (string.Equals(shortcutKeys.ShortcutName, "STRING_SYNCHRONISER", StringComparison.InvariantCulture))
						{
							if (!FeatureManager.Instance.IsOperationsSyncEnabled)
							{
								list.Add(shortcutKeys);
							}
						}
						else if (string.Equals(shortcutKeys.ShortcutName, "STRING_TOGGLE_FARM_MODE", StringComparison.InvariantCulture) && FeatureManager.Instance.IsFarmingModeDisabled)
						{
							list.Add(shortcutKeys);
						}
					}
					foreach (ShortcutKeys shortcutKeys2 in list)
					{
						this.mShortcutsConfigInstance.Shortcut.Remove(shortcutKeys2);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("error while init shortcut : {0}", new object[] { ex });
			}
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x00076940 File Offset: 0x00074B40
		internal void SaveAndReloadShortcuts()
		{
			try
			{
				this.mShortcutsConfigInstance.SaveUserDefinedShortcuts();
				CommonHandlers.ReloadShortcutsForAllInstances();
				Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_save", null, null, null, null, null, "Android", 0);
			}
			catch (Exception ex)
			{
				Logger.Error("Error saving shortcut registry" + ex.ToString());
			}
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x000769B8 File Offset: 0x00074BB8
		internal static void ReloadShortcutsForAllInstances()
		{
			foreach (string text in Utils.GetRunningInstancesList())
			{
				if (BlueStacksUIUtils.DictWindows.ContainsKey(text))
				{
					BlueStacksUIUtils.DictWindows[text].mCommonHandler.InitShortcuts();
					BlueStacksUIUtils.DictWindows[text].mCommonHandler.ReloadBossKey();
					BlueStacksUIUtils.DictWindows[text].mCommonHandler.ReloadTooltips();
				}
				HTTPUtils.SendRequestToEngineAsync("reloadShortcutsConfig", null, text, 0, null, false, 1, 0);
			}
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x00076A60 File Offset: 0x00074C60
		internal void ReloadTooltips()
		{
			foreach (SidebarElement sidebarElement in this.ParentWindow.mSidebar.mListSidebarElements)
			{
				this.ParentWindow.mSidebar.SetSidebarElementTooltip(sidebarElement, sidebarElement.mSidebarElementTooltipKey);
			}
		}

		// Token: 0x0600135E RID: 4958 RVA: 0x0000DA04 File Offset: 0x0000BC04
		private void ReloadBossKey()
		{
			RegistryManager.Instance.BossKey = this.GetShortcutKeyFromName("STRING_BOSSKEY_SETTING", true);
			if (string.IsNullOrEmpty(RegistryManager.Instance.BossKey))
			{
				GlobalKeyBoardMouseHooks.UnsetKey();
				return;
			}
			GlobalKeyBoardMouseHooks.SetKey(RegistryManager.Instance.BossKey);
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x00076AD0 File Offset: 0x00074CD0
		internal static void ArrangeWindowInTiles()
		{
			CommonHandlers.<>c__DisplayClass175_0 CS$<>8__locals1 = new CommonHandlers.<>c__DisplayClass175_0();
			CS$<>8__locals1.columns = RegistryManager.Instance.TileWindowColumnCount;
			CS$<>8__locals1.rows = (long)Math.Ceiling((double)BlueStacksUIUtils.DictWindows.Count / (double)CS$<>8__locals1.columns);
			double num = (double)Screen.PrimaryScreen.WorkingArea.Height;
			CS$<>8__locals1.y = (double)Screen.PrimaryScreen.WorkingArea.Top;
			CS$<>8__locals1.x = (double)Screen.PrimaryScreen.WorkingArea.Left;
			int num2 = 0;
			using (Dictionary<string, MainWindow>.Enumerator enumerator = BlueStacksUIUtils.DictWindows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CommonHandlers.<>c__DisplayClass175_1 CS$<>8__locals2 = new CommonHandlers.<>c__DisplayClass175_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.item = enumerator.Current;
					double windowWidth = (double)((long)Screen.PrimaryScreen.WorkingArea.Width / CS$<>8__locals2.CS$<>8__locals1.columns);
					double windowHeight = (double)((long)Screen.PrimaryScreen.WorkingArea.Height / CS$<>8__locals2.CS$<>8__locals1.rows);
					double overlapWidth = 0.0;
					double overlapHeight = 0.0;
					CS$<>8__locals2.item.Value.Dispatcher.Invoke(new Action(delegate
					{
						if (CS$<>8__locals2.item.Value.WindowState == WindowState.Minimized || CS$<>8__locals2.item.Value.WindowState == WindowState.Maximized)
						{
							CS$<>8__locals2.item.Value.RestoreWindows(true);
						}
						KMManager.CloseWindows();
						if (CS$<>8__locals2.item.Value.mAspectRatio < 1L)
						{
							if (CS$<>8__locals2.item.Value.GetWidthFromHeight(windowHeight, true, true) > windowWidth)
							{
								windowHeight = CS$<>8__locals2.item.Value.GetHeightFromWidth(windowWidth, true, true);
							}
							else
							{
								windowWidth = CS$<>8__locals2.item.Value.GetWidthFromHeight(windowHeight, true, true);
							}
							if (windowWidth < (double)CS$<>8__locals2.item.Value.MinWidthScaled)
							{
								windowWidth = (double)CS$<>8__locals2.item.Value.MinWidthScaled;
								windowHeight = CS$<>8__locals2.item.Value.GetHeightFromWidth(windowWidth, true, true);
								CommonHandlers.CalculateOverlappingLength(windowWidth, windowHeight, CS$<>8__locals2.CS$<>8__locals1.rows, CS$<>8__locals2.CS$<>8__locals1.columns, out overlapWidth, out overlapHeight);
							}
							CS$<>8__locals2.item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, CS$<>8__locals2.CS$<>8__locals1.y, CS$<>8__locals2.CS$<>8__locals1.x);
						}
						else
						{
							if (CS$<>8__locals2.item.Value.GetHeightFromWidth(windowWidth, true, true) > windowHeight)
							{
								windowWidth = CS$<>8__locals2.item.Value.GetWidthFromHeight(windowHeight, true, true);
							}
							else
							{
								windowHeight = CS$<>8__locals2.item.Value.GetHeightFromWidth(windowWidth, true, true);
							}
							if (windowHeight < (double)CS$<>8__locals2.item.Value.MinHeightScaled)
							{
								windowHeight = (double)CS$<>8__locals2.item.Value.MinHeightScaled;
								windowWidth = CS$<>8__locals2.item.Value.GetWidthFromHeight(windowHeight, true, true);
								CommonHandlers.CalculateOverlappingLength(windowWidth, windowHeight, CS$<>8__locals2.CS$<>8__locals1.rows, CS$<>8__locals2.CS$<>8__locals1.columns, out overlapWidth, out overlapHeight);
							}
							CS$<>8__locals2.item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, CS$<>8__locals2.CS$<>8__locals1.y, CS$<>8__locals2.CS$<>8__locals1.x);
						}
						if (!CS$<>8__locals2.item.Value.Topmost)
						{
							CS$<>8__locals2.item.Value.Topmost = true;
							WaitCallback waitCallback;
							if ((waitCallback = CS$<>8__locals2.<>9__1) == null)
							{
								waitCallback = (CS$<>8__locals2.<>9__1 = delegate(object obj)
								{
									Dispatcher dispatcher = CS$<>8__locals2.item.Value.Dispatcher;
									Action action;
									if ((action = CS$<>8__locals2.<>9__2) == null)
									{
										action = (CS$<>8__locals2.<>9__2 = delegate
										{
											CS$<>8__locals2.item.Value.Topmost = false;
										});
									}
									dispatcher.Invoke(action, new object[0]);
								});
							}
							ThreadPool.QueueUserWorkItem(waitCallback);
						}
					}), new object[0]);
					CS$<>8__locals2.CS$<>8__locals1.x = CS$<>8__locals2.CS$<>8__locals1.x + (windowWidth - overlapWidth);
					num = Math.Min(num, windowHeight);
					num2++;
					if ((long)num2 % CS$<>8__locals2.CS$<>8__locals1.columns == 0L)
					{
						CS$<>8__locals2.CS$<>8__locals1.y = CS$<>8__locals2.CS$<>8__locals1.y + Math.Max(num - overlapHeight, 0.0);
						CS$<>8__locals2.CS$<>8__locals1.x = 0.0;
					}
				}
			}
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x00076D28 File Offset: 0x00074F28
		internal static void CalculateOverlappingLength(double windowWidth, double windowHeight, long rows, long columns, out double overlapWidth, out double overlapHeight)
		{
			overlapHeight = 0.0;
			overlapWidth = 0.0;
			if (windowWidth * (double)columns > (double)Screen.PrimaryScreen.WorkingArea.Width)
			{
				double num = windowWidth * (double)columns - (double)Screen.PrimaryScreen.WorkingArea.Width;
				overlapWidth = num / (double)(columns - 1L);
			}
			if (windowHeight * (double)rows > (double)Screen.PrimaryScreen.WorkingArea.Height)
			{
				double num2 = windowHeight * (double)rows - (double)Screen.PrimaryScreen.WorkingArea.Height;
				overlapHeight = Math.Max(overlapHeight, num2 / (double)(rows - 1L));
			}
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x00076DD0 File Offset: 0x00074FD0
		internal static void ArrangeWindowInCascade()
		{
			double num = (double)Screen.PrimaryScreen.WorkingArea.Top;
			double num2 = (double)Screen.PrimaryScreen.WorkingArea.Bottom;
			double num3 = (double)Screen.PrimaryScreen.WorkingArea.Left;
			double num4 = (double)Screen.PrimaryScreen.WorkingArea.Right;
			double num5 = (double)Screen.PrimaryScreen.WorkingArea.Width;
			double num6 = (double)Screen.PrimaryScreen.WorkingArea.Height;
			double windowWidth = (double)((int)(num5 / 3.0));
			double windowHeight = (double)((int)(num6 / 3.0));
			double y = num;
			double x = num3;
			using (Dictionary<string, MainWindow>.Enumerator enumerator = BlueStacksUIUtils.DictWindows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, MainWindow> item = enumerator.Current;
					IntPtr handle = item.Value.Handle;
					item.Value.Dispatcher.Invoke(new Action(delegate
					{
						if (item.Value.WindowState == WindowState.Minimized)
						{
							item.Value.RestoreWindows(false);
						}
						KMManager.CloseWindows();
						windowHeight = item.Value.GetHeightFromWidth(windowWidth, false, false);
						item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, y, x);
						item.Value.Focus();
					}), new object[0]);
					x += 40.0;
					y += 40.0;
					if (y >= num2 || x >= num4)
					{
						y = num + 40.0;
						x = num3 + 40.0;
					}
				}
			}
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x00076FC4 File Offset: 0x000751C4
		public void SetNcSoftStreamingStatus(string status)
		{
			if (status.Equals("on", StringComparison.InvariantCultureIgnoreCase))
			{
				SidebarElement elementFromTag = this.ParentWindow.mSidebar.GetElementFromTag("sidebar_stream_video");
				this.ParentWindow.mSidebar.UpdateImage("sidebar_stream_video", "sidebar_stream_video_active");
				elementFromTag.Image.Width = 44.0;
				elementFromTag.Image.Height = 44.0;
				this.ParentWindow.mNCTopBar.ChangeTopBarColor("StreamingTopBarColor");
				this.ParentWindow.mNCTopBar.mStreamingTopbarGrid.Visibility = Visibility.Visible;
				this.ParentWindow.mIsStreaming = true;
				return;
			}
			SidebarElement elementFromTag2 = this.ParentWindow.mSidebar.GetElementFromTag("sidebar_stream_video");
			this.ParentWindow.mSidebar.UpdateImage("sidebar_stream_video", "sidebar_stream_video");
			elementFromTag2.Image.Width = 24.0;
			elementFromTag2.Image.Height = 24.0;
			this.ParentWindow.mNCTopBar.ChangeTopBarColor("TopBarColor");
			this.ParentWindow.mNCTopBar.mStreamingTopbarGrid.Visibility = Visibility.Collapsed;
			this.ParentWindow.mIsStreaming = false;
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x0000DA42 File Offset: 0x0000BC42
		internal static void ArrangeWindow()
		{
			if (RegistryManager.Instance.ArrangeWindowMode == 0)
			{
				CommonHandlers.ArrangeWindowInTiles();
				return;
			}
			CommonHandlers.ArrangeWindowInCascade();
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x00077100 File Offset: 0x00075300
		internal void MuteUnmuteButtonHanlder()
		{
			if (this.ParentWindow.EngineInstanceRegistry.IsMuted || RegistryManager.Instance.AreAllInstancesMuted)
			{
				this.ParentWindow.Utils.UnmuteApplication(false);
				return;
			}
			this.ParentWindow.Utils.MuteApplication(false);
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x00077150 File Offset: 0x00075350
		internal static string GetMacroName(string baseSchemeName = "Macro")
		{
			int length = baseSchemeName.Length;
			int num = 1;
			for (;;)
			{
				if (!(from MacroRecording macro in MacroGraph.Instance.Vertices
					select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
				{
					baseSchemeName.ToLower(CultureInfo.InvariantCulture),
					num
				}).Trim()))
				{
					break;
				}
				num++;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[] { baseSchemeName, num });
		}

		// Token: 0x06001366 RID: 4966 RVA: 0x0000DA5B File Offset: 0x0000BC5B
		internal void MouseMoveOverFrontend()
		{
			if (KMManager.sIsInScriptEditingMode && !this.ParentWindow.mIsWindowInFocus)
			{
				Logger.Info("Script focused");
				this.ParentWindow.mFrontendHandler.FocusFrontend();
			}
		}

		// Token: 0x06001367 RID: 4967 RVA: 0x0000DA8D File Offset: 0x0000BC8D
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.mObsResponseTimeoutTimer != null)
				{
					this.mObsResponseTimeoutTimer.Elapsed -= this.ObsResponseTimeoutTimer_Elapsed;
					this.mObsResponseTimeoutTimer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06001368 RID: 4968 RVA: 0x000771FC File Offset: 0x000753FC
		~CommonHandlers()
		{
			this.Dispose(false);
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0000DACA File Offset: 0x0000BCCA
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0007722C File Offset: 0x0007542C
		internal static void ReloadMacroShortcutsForAllInstances()
		{
			foreach (string text in Utils.GetRunningInstancesList())
			{
				if (BlueStacksUIUtils.DictWindows.ContainsKey(text))
				{
					HTTPUtils.SendRequestToEngineAsync("updateMacroShortcutsDict", MainWindow.sMacroMapping, text, 0, null, false, 1, 0);
				}
			}
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0007729C File Offset: 0x0007549C
		internal void GameGuideButtonHandler(string action, string location)
		{
			if (!this.ToggleGamepadAndKeyboardGuidance("default"))
			{
				KMManager.HandleInputMapperWindow(this.ParentWindow, "");
				string packageName = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
			}
			ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "GameGuide", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x0000DAD9 File Offset: 0x0000BCD9
		internal static string GetCompleteMacroRecordingPath(string macroName)
		{
			return Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroName.ToLower(CultureInfo.InvariantCulture) + ".json");
		}

		// Token: 0x0600136D RID: 4973 RVA: 0x00077318 File Offset: 0x00075518
		internal bool ToggleGamepadAndKeyboardGuidance(string selectedTab)
		{
			if (KMManager.CheckIfKeymappingWindowVisible(true))
			{
				if (KMManager.sGuidanceWindow != null)
				{
					GuidanceWindow.HideOnNextLaunch(true);
					KMManager.sGuidanceWindow.mIsOnboardingPopupToBeShownOnGuidanceClose = true;
					KMManager.CloseWindows();
					this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
					this.ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
				}
				else
				{
					string text = "default";
					if (KMManager.sGuidanceWindow.mIsGamePadTabSelected)
					{
						text = "gamepad";
					}
					if (!string.Equals(text, selectedTab, StringComparison.InvariantCultureIgnoreCase))
					{
						KMManager.sGuidanceWindow.GuidanceWindowTabSelected(selectedTab);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600136E RID: 4974 RVA: 0x0000DAFA File Offset: 0x0000BCFA
		internal void ToggleScrollOnEdgeMode(string enable)
		{
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("toggleScrollOnEdgeFeature", new Dictionary<string, string> { { "isEnabled", enable } });
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x000773AC File Offset: 0x000755AC
		internal bool CheckNativeGamepadState(string packageName)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "packageName", packageName } };
				string text = JObject.Parse(HTTPUtils.SendRequestToGuest("checknativegamepadstatus", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64"))["isEnabled"].ToString().Trim();
				Logger.Debug("NATIVE_GAMEPAD: isEnabled: " + text);
				if (text.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in CheckNativeGampeadState: " + ex.ToString());
			}
			return false;
		}

		// Token: 0x04000C2B RID: 3115
		private MainWindow ParentWindow;

		// Token: 0x04000C2C RID: 3116
		internal static bool sIsRecordingVideo = false;

		// Token: 0x04000C2D RID: 3117
		internal static string sRecordingInstance = "";

		// Token: 0x04000C2E RID: 3118
		private static bool sIsOBSStartingStopping = false;

		// Token: 0x04000C2F RID: 3119
		private static bool sDownloading;

		// Token: 0x04000C30 RID: 3120
		private LegacyDownloader mDownloader;

		// Token: 0x04000C31 RID: 3121
		private static CustomMessageWindow sWindow;

		// Token: 0x04000C32 RID: 3122
		internal ShortcutConfig mShortcutsConfigInstance;

		// Token: 0x04000C33 RID: 3123
		internal static string mSavedVideoRecordingFilePath = null;

		// Token: 0x04000C44 RID: 3140
		private global::System.Timers.Timer mObsResponseTimeoutTimer;

		// Token: 0x04000C45 RID: 3141
		private long mDownloadedSize;

		// Token: 0x04000C46 RID: 3142
		private long mLastSizeChecked;

		// Token: 0x04000C47 RID: 3143
		private DispatcherTimer mDownloadStatusTimer;

		// Token: 0x04000C48 RID: 3144
		private float mRecorderSizeMb;

		// Token: 0x04000C49 RID: 3145
		private bool disposedValue;

		// Token: 0x020001E1 RID: 481
		// (Invoke) Token: 0x0600137F RID: 4991
		public delegate void MacroBookmarkChanged(string fileName, bool wasBookmarked);

		// Token: 0x020001E2 RID: 482
		// (Invoke) Token: 0x06001383 RID: 4995
		public delegate void MacroSettingsChanged(MacroRecording record);

		// Token: 0x020001E3 RID: 483
		// (Invoke) Token: 0x06001387 RID: 4999
		public delegate void ShortcutKeysChanged(bool isEnabled);

		// Token: 0x020001E4 RID: 484
		// (Invoke) Token: 0x0600138B RID: 5003
		public delegate void ShortcutKeysRefresh();

		// Token: 0x020001E5 RID: 485
		// (Invoke) Token: 0x0600138F RID: 5007
		public delegate void MacroDeleted(string fileName);

		// Token: 0x020001E6 RID: 486
		// (Invoke) Token: 0x06001393 RID: 5011
		public delegate void OverlayStateChanged(bool isEnabled);

		// Token: 0x020001E7 RID: 487
		// (Invoke) Token: 0x06001397 RID: 5015
		public delegate void MacroButtonVisibilityChanged(bool isVisible);

		// Token: 0x020001E8 RID: 488
		// (Invoke) Token: 0x0600139B RID: 5019
		public delegate void OperationSyncButtonVisibilityChanged(bool isVisible);

		// Token: 0x020001E9 RID: 489
		// (Invoke) Token: 0x0600139F RID: 5023
		public delegate void OBSResponseTimeout();

		// Token: 0x020001EA RID: 490
		// (Invoke) Token: 0x060013A3 RID: 5027
		public delegate void ScreenRecorderStateTransitioning();

		// Token: 0x020001EB RID: 491
		// (Invoke) Token: 0x060013A7 RID: 5031
		public delegate void BTvDownloaderMinimized();

		// Token: 0x020001EC RID: 492
		// (Invoke) Token: 0x060013AB RID: 5035
		public delegate void GamepadButtonVisibilityChanged(bool visibility);

		// Token: 0x020001ED RID: 493
		// (Invoke) Token: 0x060013AF RID: 5039
		public delegate void ScreenRecordingStateChanged(bool isRecording);

		// Token: 0x020001EE RID: 494
		// (Invoke) Token: 0x060013B3 RID: 5043
		public delegate void VolumeChanged(int volumeLevel);

		// Token: 0x020001EF RID: 495
		// (Invoke) Token: 0x060013B7 RID: 5047
		public delegate void VolumeMuted(bool muted);

		// Token: 0x020001F0 RID: 496
		// (Invoke) Token: 0x060013BB RID: 5051
		public delegate void GameGuideButtonVisibilityChanged(bool visibility);
	}
}
