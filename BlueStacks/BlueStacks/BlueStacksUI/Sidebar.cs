using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000D4 RID: 212
	public class Sidebar : UserControl, IComponentConnector
	{
		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x0000791C File Offset: 0x00005B1C
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

		// Token: 0x060008AF RID: 2223 RVA: 0x00031238 File Offset: 0x0002F438
		public Sidebar()
		{
			this.InitializeComponent();
			this.mMoreButton.Image.ImageName = "sidebar_options_close";
			if (this.mListPopups == null)
			{
				this.mListPopups = new List<CustomPopUp>(8) { this.mChangeTransparencyPopup, this.mVolumeSliderPopup, this.mOverlayTooltip, this.mMacroButtonPopup, this.mGameControlButtonPopup, this.mRecordScreenPopup, this.mScreenshotPopup, this.mMoreElements };
			}
			BlueStacksUIBinding.Instance.PropertyChanged += this.BlueStacksUIBinding_PropertyChanged;
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x0000793D File Offset: 0x00005B3D
		private void BlueStacksUIBinding_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "LocaleModel")
			{
				this.ParentWindow.mCommonHandler.ReloadTooltips();
			}
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x00031314 File Offset: 0x0002F514
		internal void BindEvents()
		{
			this.ParentWindow.CursorLockChangedEvent += this.ParentWindow_CursorLockChangedEvent;
			this.ParentWindow.FullScreenChanged += this.ParentWindow_FullScreenChangedEvent;
			this.ParentWindow.FrontendGridVisibilityChanged += this.ParentWindow_FrontendGridVisibleChangedEvent;
			this.ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += this.ParentWindow_ScreenRecordingStateChangedEvent;
			this.ParentWindow.mCommonHandler.OverlayStateChangedEvent += this.ParentWindow_OverlayStateChangedEvent;
			this.ParentWindow.mCommonHandler.MacroButtonVisibilityChangedEvent += this.ParentWindow_MacroButtonVisibilityChangedEvent;
			this.ParentWindow.mCommonHandler.OperationSyncButtonVisibilityChangedEvent += this.ParentWindow_OperationSyncButtonVisibilityChangedEvent;
			this.ParentWindow.mCommonHandler.ScreenRecorderStateTransitioningEvent += this.ParentWindow_ScreenRecordingInitingEvent;
			this.ParentWindow.mCommonHandler.OBSResponseTimeoutEvent += this.ParentWindow_OBSResponseTimeoutEvent;
			this.ParentWindow.mCommonHandler.BTvDownloaderMinimizedEvent += this.ParentWindow_BTvDownloaderMinimizedEvent;
			this.ParentWindow.mCommonHandler.GamepadButtonVisibilityChangedEvent += this.ParentWindow_GamepadButtonVisibilityChangedEvent;
			this.ParentWindow.mCommonHandler.VolumeChangedEvent += this.ParentWindow_VolumeChangedEvent;
			this.ParentWindow.mCommonHandler.VolumeMutedEvent += this.ParentWindow_VolumeMutedEvent;
			PromotionObject.AppSpecificRulesHandler = (EventHandler)Delegate.Combine(PromotionObject.AppSpecificRulesHandler, new EventHandler(this.PromotionUpdated));
			this.ParentWindow.mCommonHandler.GameGuideButtonVisibilityChangedEvent += this.ParentWindow_GameGuideButtonVisibilityChangedEvent;
			if (this.ParentWindow.mGuestBootCompleted)
			{
				this.ToggleBootCompletedState();
				return;
			}
			this.ParentWindow.GuestBootCompleted += this.ParentWindow_GuestBootCompletedEvent;
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x00007961 File Offset: 0x00005B61
		private void ParentWindow_GameGuideButtonVisibilityChangedEvent(bool visibility)
		{
			this.ChangeElementState("sidebar_gameguide", visibility);
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x0000796F File Offset: 0x00005B6F
		private void PromotionUpdated(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x00007981 File Offset: 0x00005B81
		public void UpdateMuteAllInstancesCheckbox()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (RegistryManager.Instance.AreAllInstancesMuted)
				{
					this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox_checked";
					return;
				}
				this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox";
			}), new object[0]);
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x000314E8 File Offset: 0x0002F6E8
		private void ParentWindow_VolumeMutedEvent(bool muted)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (muted)
				{
					this.mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_muted_popup";
					this.UpdateImage("sidebar_volume", "sidebar_volume_muted");
				}
				else
				{
					this.mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_popup";
					this.UpdateToDefaultImage("sidebar_volume");
				}
				this.UpdateMuteAllInstancesCheckbox();
			}), new object[0]);
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x0003152C File Offset: 0x0002F72C
		private void ParentWindow_VolumeChangedEvent(int volumeLevel)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.mVolumeSlider.Value = (double)volumeLevel;
				this.mCurrentVolumeValue.Text = volumeLevel.ToString(CultureInfo.InvariantCulture);
			}), new object[0]);
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x000079A6 File Offset: 0x00005BA6
		private void ParentWindow_GamepadButtonVisibilityChangedEvent(bool visibility)
		{
			this.ChangeElementState("sidebar_gamepad", visibility);
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x00031570 File Offset: 0x0002F770
		private void ParentWindow_BTvDownloaderMinimizedEvent()
		{
			this.RecordScreenPopupHeader.Visibility = Visibility.Collapsed;
			this.RecordScreenPopupBody.Visibility = Visibility.Visible;
			this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
			BlueStacksUIBinding.Bind(this.RecordScreenPopupBody, "STRING_DOWNLOAD_BACKGROUND", "");
			this.mRecordScreenPopup.StaysOpen = false;
			this.mRecordScreenPopup.IsOpen = true;
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x000079B4 File Offset: 0x00005BB4
		private void ParentWindow_OBSResponseTimeoutEvent()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
				elementFromTag.Image.IsImageToBeRotated = false;
				Sidebar.UpdateToDefaultImage(elementFromTag);
			}), new object[0]);
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x000315D0 File Offset: 0x0002F7D0
		private void ParentWindow_ScreenRecordingInitingEvent()
		{
			SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
			Sidebar.UpdateImage(elementFromTag, "sidebar_video_loading");
			elementFromTag.Image.Visibility = Visibility.Hidden;
			elementFromTag.Image.IsImageToBeRotated = true;
			elementFromTag.Image.Visibility = Visibility.Visible;
			this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
			this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
			this.mRecordScreenClose.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x000079D9 File Offset: 0x00005BD9
		private void ParentWindow_OperationSyncButtonVisibilityChangedEvent(bool isVisible)
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				return;
			}
			this.ToggleElementVisibilty("sidebar_operation", isVisible);
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x0003163C File Offset: 0x0002F83C
		private void ToggleElementVisibilty(SidebarElement ele, bool isVisible)
		{
			if (ele != null)
			{
				if (isVisible)
				{
					ele.Visibility = Visibility.Visible;
				}
				else
				{
					ele.Visibility = Visibility.Collapsed;
				}
				if (ele.IsInMainSidebar)
				{
					int num = this.mElementsStackPanel.Children.IndexOf(ele);
					int num2 = this.mListSidebarElements.IndexOf(ele);
					if (num != -1 && num != num2)
					{
						this.mElementsStackPanel.Children.RemoveAt(num);
						int count = this.mElementsStackPanel.Children.Count;
						if (num2 >= count)
						{
							ele.IsInMainSidebar = false;
						}
						else
						{
							this.mElementsStackPanel.Children.Insert(num2 + 1, ele);
						}
					}
				}
				this.FixMarginOfSurroundingElement(ele);
				this.UpdateTotalVisibleElementCount();
				this.ArrangeAllSidebarElements();
			}
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x000316E8 File Offset: 0x0002F8E8
		private SidebarElement GetPreviousVisibleSidebarElement(SidebarElement ele)
		{
			SidebarElement previousSidebarElement = this.GetPreviousSidebarElement(ele);
			if (previousSidebarElement.Visibility != Visibility.Visible)
			{
				return this.GetPreviousSidebarElement(previousSidebarElement);
			}
			return previousSidebarElement;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x00031710 File Offset: 0x0002F910
		private SidebarElement GetPreviousSidebarElement(SidebarElement ele)
		{
			int num = this.mListSidebarElements.IndexOf(ele);
			if (num != 0)
			{
				return this.mListSidebarElements[num - 1];
			}
			return ele;
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00031740 File Offset: 0x0002F940
		private void FixMarginOfSurroundingElement(SidebarElement currentElement)
		{
			if (currentElement != null && currentElement.Visibility == Visibility.Visible)
			{
				if (currentElement.IsLastElementOfGroup)
				{
					if (currentElement.IsCurrentLastElementOfGroup)
					{
						return;
					}
					currentElement.IsCurrentLastElementOfGroup = true;
					Sidebar.IncreaseElementBottomMarginIfLast(currentElement);
					SidebarElement previousVisibleSidebarElement = this.GetPreviousVisibleSidebarElement(currentElement);
					if (previousVisibleSidebarElement != currentElement)
					{
						previousVisibleSidebarElement.IsCurrentLastElementOfGroup = false;
						Sidebar.DecreaseElementBottomMargin(previousVisibleSidebarElement);
						Thickness margin = previousVisibleSidebarElement.Margin;
						margin.Bottom = 2.0;
						previousVisibleSidebarElement.Margin = margin;
						return;
					}
				}
			}
			else if (currentElement.IsCurrentLastElementOfGroup)
			{
				currentElement.IsCurrentLastElementOfGroup = false;
				SidebarElement previousVisibleSidebarElement2 = this.GetPreviousVisibleSidebarElement(currentElement);
				if (previousVisibleSidebarElement2 != currentElement)
				{
					previousVisibleSidebarElement2.IsCurrentLastElementOfGroup = true;
					Sidebar.IncreaseElementBottomMarginIfLast(previousVisibleSidebarElement2);
				}
			}
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x000317D8 File Offset: 0x0002F9D8
		private void ToggleElementVisibilty(string elementKey, bool isVisible)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.ToggleElementVisibilty(this.GetElementFromTag(elementKey), isVisible);
			}), new object[0]);
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x000079F4 File Offset: 0x00005BF4
		private void ParentWindow_MacroButtonVisibilityChangedEvent(bool isVisible)
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				return;
			}
			this.ToggleElementVisibilty("sidebar_macro", isVisible);
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x00031824 File Offset: 0x0002FA24
		private void ParentWindow_OverlayStateChangedEvent(bool isEnabled)
		{
			SidebarElement elementFromTag = this.GetElementFromTag("sidebar_overlay");
			if (isEnabled)
			{
				Sidebar.UpdateToDefaultImage(elementFromTag);
				if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
				{
					if (this.mLastSliderValue == 0.0)
					{
						RegistryManager.Instance.TranslucentControlsTransparency = 0.5;
						this.transSlider.Value = 0.5;
					}
					else
					{
						RegistryManager.Instance.TranslucentControlsTransparency = this.mLastSliderValue;
						this.transSlider.Value = this.mLastSliderValue;
					}
				}
			}
			else
			{
				Sidebar.UpdateImage(elementFromTag, "sidebar_overlay_inactive");
				double value = this.transSlider.Value;
				this.transSlider.Value = 0.0;
				this.mLastSliderValue = value;
			}
			KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x00007A0F File Offset: 0x00005C0F
		private void MSidebarPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mMoreElements.IsOpen)
			{
				this.mMoreElements.IsOpen = false;
			}
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x000318FC File Offset: 0x0002FAFC
		private void ParentWindow_ScreenRecordingStateChangedEvent(bool isRecording)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
				elementFromTag.Image.IsImageToBeRotated = false;
				if (isRecording)
				{
					Sidebar.UpdateImage(elementFromTag, "sidebar_video_capture_active");
					this.ChangeElementState("sidebar_fullscreen", false);
					BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_STOP_RECORDING", "");
					this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
					this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
					this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
					this.mRecordScreenClose.Visibility = Visibility.Collapsed;
				}
				else
				{
					Sidebar.UpdateToDefaultImage(elementFromTag);
					this.RecordScreenPopupBody.Visibility = Visibility.Visible;
					this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
					BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_RECORDING_SAVED", "");
					BlueStacksUIBinding.Bind(this.RecordScreenPopupBody, "STRING_CLICK_TO_SEE_VIDEO", "");
					this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
					this.RecordScreenPopupHyperlink.Visibility = Visibility.Visible;
					BlueStacksUIBinding.Bind(this.mRecorderClickLink, "STRING_CLICK_TO_SEE_VIDEO", "");
					this.RecordScreenPopupBody.Visibility = Visibility.Visible;
					this.mRecordScreenClose.Visibility = Visibility.Visible;
					if (this.ParentWindow.mIsWindowInFocus && elementFromTag.IsInMainSidebar)
					{
						this.mRecordScreenPopup.PlacementTarget = elementFromTag;
						this.mRecordScreenPopup.StaysOpen = false;
						this.mRecordScreenPopup.IsOpen = true;
					}
					if (RegistryManager.Instance.IsShowToastNotification)
					{
						this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_RECORDING_SAVED", ""));
					}
					if (this.ParentWindow.mFrontendGrid.IsVisible)
					{
						this.ChangeElementState("sidebar_fullscreen", true);
					}
				}
				this.SetVideoRecordingTooltip(isRecording);
			}), new object[0]);
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00031940 File Offset: 0x0002FB40
		internal void ShowScreenshotSavedPopup(string screenshotPath)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				SidebarElement elementFromTag = this.GetElementFromTag("sidebar_screenshot");
				this.SetSidebarElementTooltip(elementFromTag, "STRING_TOOLBAR_CAMERA");
				if (this.ParentWindow.mIsWindowInFocus && elementFromTag.IsInMainSidebar)
				{
					this.mScreenshotPopup.PlacementTarget = elementFromTag;
					this.mScreenshotPopup.StaysOpen = false;
					this.mScreenshotPopup.IsOpen = true;
					this.currentScreenshotSavedPath = screenshotPath;
				}
			}), new object[0]);
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x00031984 File Offset: 0x0002FB84
		private void ParentWindow_FrontendGridVisibleChangedEvent(object sender, MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs args)
		{
			this.ChangeElementState("sidebar_lock_cursor", args.IsVisible);
			if (!CommonHandlers.sIsRecordingVideo)
			{
				this.ChangeElementState("sidebar_fullscreen", args.IsVisible);
			}
			this.ChangeElementState("sidebar_toggle", args.IsVisible);
			this.ChangeElementState("sidebar_controls", args.IsVisible);
			this.ChangeElementState("sidebar_overlay", args.IsVisible);
			this.ChangeElementState("sidebar_back", args.IsVisible);
			this.ChangeElementState("sidebar_home", args.IsVisible);
			this.ChangeElementState("sidebar_screenshot", args.IsVisible);
			this.ChangeElementState("sidebar_video_capture", args.IsVisible);
			if (!args.IsVisible)
			{
				this.ChangeElementState("sidebar_gamepad", args.IsVisible);
				this.ChangeElementState("sidebar_gameguide", args.IsVisible);
			}
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00031A5C File Offset: 0x0002FC5C
		private void InitDefaultSettings()
		{
			if (this.ParentWindow.mIsFullScreen)
			{
				this.UpdateImage("sidebar_fullscreen", "sidebar_fullscreen_minimize");
			}
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.ToggleElementVisibilty("sidebar_macro", false);
				this.ToggleElementVisibilty("sidebar_operation", false);
				if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
				{
					this.UpdateImage("sidebar_overlay", "sidebar_overlay_inactive");
					this.mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive_popup";
				}
			}
			else
			{
				this.ToggleElementVisibilty("sidebar_overlay", false);
				this.ToggleElementVisibilty("sidebar_overlay_inactive", false);
			}
			this.SetupVolumeInitState();
			this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
			this.mOverlayPopUpMessage.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_ON_SCREEN_CONTROLS_BODY", ""), new object[] { this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_OVERLAY", false) });
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00031B58 File Offset: 0x0002FD58
		private void SetupVolumeInitState()
		{
			if (this.ParentWindow.EngineInstanceRegistry.IsMuted)
			{
				this.UpdateImage("sidebar_volume", "sidebar_volume_muted");
				this.mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_muted_popup";
			}
			this.UpdateMuteAllInstancesCheckbox();
			this.mVolumeSlider.Value = (double)this.ParentWindow.Utils.CurrentVolumeLevel;
			this.mCurrentVolumeValue.Text = this.ParentWindow.Utils.CurrentVolumeLevel.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00007A2A File Offset: 0x00005C2A
		internal void HideSideBarInFullscreen()
		{
			this.ParentWindow.mFullscreenSidebarPopupButton.IsOpen = false;
			this.ParentWindow.mFullscreenSidebarPopup.IsOpen = false;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00004786 File Offset: 0x00002986
		internal void ToggleSidebarVisibilityInFullscreen(bool isVisible)
		{
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00004786 File Offset: 0x00002986
		internal void ToggleSidebarButtonVisibilityInFullscreen(bool isVisible)
		{
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x00031BE4 File Offset: 0x0002FDE4
		private void ParentWindow_FullScreenChangedEvent(object sender, MainWindowEventArgs.FullScreenChangedEventArgs args)
		{
			object obj = this.mSyncRoot;
			lock (obj)
			{
				this.mIsInFullscreenMode = args.IsFullscreen;
				this.SetupSidebarForFullscreen(this.mIsInFullscreenMode);
				if (this.mIsInFullscreenMode)
				{
					this.UpdateImage("sidebar_fullscreen", "sidebar_fullscreen_minimize");
				}
				else
				{
					this.UpdateImage("sidebar_fullscreen", "sidebar_fullscreen");
					this.ParentWindow.mFullscreenSidebarPopup.IsOpen = false;
					this.ParentWindow.mFullscreenSidebarPopupButton.IsOpen = false;
				}
				this.ArrangeAllSidebarElements();
			}
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x00031C84 File Offset: 0x0002FE84
		private void SetupSidebarForFullscreen(bool isFullScreen)
		{
			if (isFullScreen)
			{
				if (this.ParentWindow.mMainWindowTopGrid.Children.Contains(this))
				{
					this.ParentWindow.mMainWindowTopGrid.Children.Remove(this);
					this.ParentWindow.mFullscreenSidebarPopupInnerGrid.Children.Add(this);
				}
				base.Visibility = Visibility.Visible;
				return;
			}
			if (this.ParentWindow.mFullscreenSidebarPopupInnerGrid.Children.Contains(this))
			{
				this.ParentWindow.mFullscreenSidebarPopupInnerGrid.Children.Remove(this);
				this.ParentWindow.mMainWindowTopGrid.Children.Add(this);
			}
			if (this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
			{
				base.Visibility = Visibility.Visible;
				return;
			}
			base.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00007A4E File Offset: 0x00005C4E
		private void ParentWindow_CursorLockChangedEvent(object sender, MainWindowEventArgs.CursorLockChangedEventArgs args)
		{
			if (args.IsLocked)
			{
				this.UpdateImage("sidebar_lock_cursor", "sidebar_lock_cursor_active");
				return;
			}
			this.UpdateImage("sidebar_lock_cursor", "sidebar_lock_cursor");
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x00007A79 File Offset: 0x00005C79
		private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
		{
			this.ToggleBootCompletedState();
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00007A81 File Offset: 0x00005C81
		private void ToggleBootCompletedState()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.ChangeElementState("sidebar_stream_video", true);
				this.ChangeElementState("sidebar_volume", true);
				this.ChangeElementState("sidebar_macro", true);
				this.ChangeElementState("sidebar_operation", true);
				this.ChangeElementState("sidebar_location", true);
				this.ChangeElementState("sidebar_rotate", true);
			}), new object[0]);
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00007AA6 File Offset: 0x00005CA6
		private static void ChangeElementState(SidebarElement ele, bool isEnabled)
		{
			if (ele != null)
			{
				ele.IsEnabled = isEnabled;
			}
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x00007AB2 File Offset: 0x00005CB2
		private void ChangeElementState(string elementTag, bool isEnabled)
		{
			Sidebar.ChangeElementState(this.GetElementFromTag(elementTag), isEnabled);
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x00031D48 File Offset: 0x0002FF48
		private void Sidebar_Loaded(object sender, RoutedEventArgs e)
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				return;
			}
			if (!this.mIsLoadedOnce)
			{
				this.mIsLoadedOnce = true;
				this.BindEvents();
				this.SetPlacementTargets();
				this.InitDefaultSettings();
				this.mMacroBookmarkPopup.SetParentWindowAndBindEvents(this.ParentWindow);
				this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
			}
			this.ParentWindow.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
			this.SetVideoRecordingTooltipForNCSoft();
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x00031DC8 File Offset: 0x0002FFC8
		private void MMacroButtonAndPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.mMacroButtonPopup.IsOpen)
			{
				if (this.mMacroBookmarkTimer == null)
				{
					this.mMacroBookmarkTimer = new DispatcherTimer
					{
						Interval = new TimeSpan(0, 0, 0, 0, 500)
					};
					this.mMacroBookmarkTimer.Tick += this.MMacroBookmarkTimer_Tick;
				}
				else
				{
					this.mMacroBookmarkTimer.Stop();
				}
				this.mMacroBookmarkTimer.Start();
			}
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x00031E38 File Offset: 0x00030038
		private void MMacroBookmarkTimer_Tick(object sender, EventArgs e)
		{
			if (!this.mMacroButtonPopup.IsMouseOver && !this.GetElementFromTag("sidebar_macro").IsMouseOver)
			{
				this.mMacroButtonPopup.IsOpen = false;
				if (this.mIsInFullscreenMode && !base.IsMouseOver)
				{
					this.ToggleSidebarVisibilityInFullscreen(false);
				}
			}
			(sender as DispatcherTimer).Stop();
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x00031E94 File Offset: 0x00030094
		private void MacroButtonHandler(object sender, EventArgs e)
		{
			if (this.ParentWindow.mIsMacroRecorderActive)
			{
				this.ParentWindow.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""), "", "", false);
				return;
			}
			if (RegistryManager.Instance.BookmarkedScriptList.Length != 0 && !this.mMoreElements.IsOpen)
			{
				this.mMacroButtonPopup.IsOpen = true;
				return;
			}
			this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
			this.mMacroButtonPopup.IsOpen = false;
			this.ToggleSidebarVisibilityInFullscreen(false);
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x00031F5C File Offset: 0x0003015C
		private void OperationSyncHandler(object sender, EventArgs e)
		{
			this.ParentWindow.ShowSynchronizerWindow();
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "OperationSync", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x00031FB4 File Offset: 0x000301B4
		private void KeymapToggleHandler(object sender, EventArgs e)
		{
			/*
An exception occurred when decompiling this method (060008D8)

ICSharpCode.Decompiler.DecompilerException: Error decompiling System.Void BlueStacks.BlueStacksUI.Sidebar::KeymapToggleHandler(System.Object,System.EventArgs)

 ---> System.ArgumentOutOfRangeException: length ('-1') must be a non-negative value. (Parameter 'length')
Actual value was -1.
   at System.ArgumentOutOfRangeException.ThrowNegative[T](T value, String paramName)
   at System.Array.CopyImpl(Array sourceArray, Int32 sourceIndex, Array destinationArray, Int32 destinationIndex, Int32 length, Boolean reliable)
   at System.Array.Copy(Array sourceArray, Array destinationArray, Int32 length)
   at ICSharpCode.Decompiler.ILAst.ILAstBuilder.StackSlot.ModifyStack(StackSlot[] stack, Int32 popCount, Int32 pushCount, ByteCode pushDefinition) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstBuilder.cs:line 51
   at ICSharpCode.Decompiler.ILAst.ILAstBuilder.StackAnalysis(MethodDef methodDef) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstBuilder.cs:line 403
   at ICSharpCode.Decompiler.ILAst.ILAstBuilder.Build(MethodDef methodDef, Boolean optimize, DecompilerContext context) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstBuilder.cs:line 278
   at ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(IEnumerable`1 parameters, MethodDebugInfoBuilder& builder) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:line 117
   at ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(MethodDef methodDef, DecompilerContext context, AutoPropertyProvider autoPropertyProvider, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, StringBuilder sb, MethodDebugInfoBuilder& stmtsBuilder) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:line 88
   --- End of inner exception stack trace ---
   at ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(MethodDef methodDef, DecompilerContext context, AutoPropertyProvider autoPropertyProvider, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, StringBuilder sb, MethodDebugInfoBuilder& stmtsBuilder) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:line 92
   at ICSharpCode.Decompiler.Ast.AstBuilder.AddMethodBody(EntityDeclaration methodNode, EntityDeclaration& updatedNode, MethodDef method, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, MethodKind methodKind) in D:\a\dnSpy\dnSpy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstBuilder.cs:line 1686
*/;
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x00031FCC File Offset: 0x000301CC
		private void KeyMapControlsButton_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Right Click on keymap control UI button ");
			try
			{
				KMManager.sIsDeveloperModeOn = (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
				KMManager.LoadIMActions(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
				KMManager.ShowAdvancedSettings(this.ParentWindow);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception on right click on keymap button: " + ex.ToString());
			}
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x0003206C File Offset: 0x0003026C
		internal void KeyMapSwitchButtonHandler(SidebarElement ele)
		{
			bool fromSideBar = true;
			if (ele == null)
			{
				ele = this.GetElementFromTag("sidebar_toggle");
				fromSideBar = false;
			}
			if (ele == null)
			{
				return;
			}
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (!KMManager.sIsComboRecordingOn)
				{
					if (string.Equals(ele.Image.ImageName, "sidebar_toggle_off", StringComparison.InvariantCulture))
					{
						Sidebar.UpdateToDefaultImage(ele);
						this.ParentWindow.mFrontendHandler.EnableKeyMapping(true);
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ToggleKeymapOn", fromSideBar ? "MouseClick" : "Shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						return;
					}
					Sidebar.UpdateImage(ele, "sidebar_toggle_off");
					this.ParentWindow.mFrontendHandler.EnableKeyMapping(false);
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ToggleKeymapOff", fromSideBar ? "MouseClick" : "Shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				}
			}), new object[0]);
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x000320E0 File Offset: 0x000302E0
		private void SetPlacementTargets()
		{
			this.mChangeTransparencyPopup.PlacementTarget = this.GetElementFromTag("sidebar_overlay");
			this.mVolumeSliderPopup.PlacementTarget = this.GetElementFromTag("sidebar_volume");
			this.mOverlayTooltip.PlacementTarget = this.GetElementFromTag("sidebar_overlay");
			this.mRecordScreenPopup.PlacementTarget = this.GetElementFromTag("sidebar_video_capture");
			this.mScreenshotPopup.PlacementTarget = this.GetElementFromTag("sidebar_screenshot");
			this.mMacroButtonPopup.PlacementTarget = this.GetElementFromTag("sidebar_macro");
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00032174 File Offset: 0x00030374
		internal void InitElements()
		{
			SidebarConfig.sFilePath = global::System.IO.Path.Combine(RegistryStrings.GadgetDir, string.Format(CultureInfo.InvariantCulture, "SidebarConfig_{0}.json", new object[] { this.ParentWindow.mVmName }));
			foreach (List<string> list in SidebarConfig.Instance.GroupElements)
			{
				this.CreateAndAddElementsToStackPanel(list);
			}
			this.InitStaticElements();
			this.UpdateTotalVisibleElementCount();
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x0003220C File Offset: 0x0003040C
		private void UpdateTotalVisibleElementCount()
		{
			this.mTotalVisibleElements = this.mListSidebarElements.Where((SidebarElement item) => item.Visibility == Visibility.Visible).Count<SidebarElement>();
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mTotalVisibleElements--;
				return;
			}
			this.mTotalVisibleElements -= 3;
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x00032278 File Offset: 0x00030478
		private void InitStaticElements()
		{
			SidebarElement sidebarElement = this.CreateElement("sidebar_gameguide", "STRING_TOGGLE_KEYMAP_WINDOW", new EventHandler(this.OpenGameGuideButtonHandler));
			this.AddElement(sidebarElement, false);
			Sidebar.ChangeElementState(sidebarElement, true);
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				sidebarElement = this.CreateElement("sidebar_settings", "STRING_SETTINGS", new EventHandler(this.GoSettingsHandler));
				this.AddElement(sidebarElement, true);
			}
			sidebarElement = this.CreateElement("sidebar_back", "STRING_BACK", new EventHandler(this.GoBackHandler));
			this.AddElement(sidebarElement, true);
			Sidebar.ChangeElementState(sidebarElement, false);
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x00032310 File Offset: 0x00030510
		private void CreateAndAddElementsToStackPanel(List<string> ls)
		{
			SidebarElement sidebarElement = null;
			foreach (string text in ls)
			{
				if (text == null)
				{
					goto IL_0609;
				}
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1613331775U)
				{
					if (num <= 964859351U)
					{
						if (num <= 784514210U)
						{
							if (num != 33101451U)
							{
								if (num != 784514210U)
								{
									goto IL_0609;
								}
								if (!(text == "sidebar_lock_cursor"))
								{
									goto IL_0609;
								}
								sidebarElement = this.CreateElement("sidebar_lock_cursor", "STRING_TOGGLE_LOCK_CURSOR", new EventHandler(this.LockCursorHandler));
								this.AddElement(sidebarElement, false);
								Sidebar.ChangeElementState(sidebarElement, false);
							}
							else
							{
								if (!(text == "sidebar_gamepad"))
								{
									goto IL_0609;
								}
								sidebarElement = this.CreateElement("sidebar_gamepad", "STRING_GAMEPAD_CONTROLS", new EventHandler(this.GamepadControlsWindowHandler));
								this.AddElement(sidebarElement, false);
								Sidebar.ChangeElementState(sidebarElement, false);
							}
						}
						else if (num != 785324652U)
						{
							if (num != 964859351U)
							{
								goto IL_0609;
							}
							if (!(text == "sidebar_rotate"))
							{
								goto IL_0609;
							}
							sidebarElement = this.CreateElement("sidebar_rotate", "STRING_ROTATE", new EventHandler(this.RotateHandler));
							this.AddElement(sidebarElement, false);
							Sidebar.ChangeElementState(sidebarElement, false);
						}
						else
						{
							if (!(text == "sidebar_macro"))
							{
								goto IL_0609;
							}
							sidebarElement = this.CreateElement("sidebar_macro", "STRING_MACRO_RECORDER", new EventHandler(this.MacroButtonHandler));
							sidebarElement.MouseLeave += this.MMacroButtonAndPopup_MouseLeave;
							this.AddElement(sidebarElement, false);
						}
					}
					else if (num <= 1275655550U)
					{
						if (num != 1198477406U)
						{
							if (num != 1275655550U)
							{
								goto IL_0609;
							}
							if (!(text == "sidebar_video_capture"))
							{
								goto IL_0609;
							}
							sidebarElement = this.CreateElement("sidebar_video_capture", "STRING_RECORD_SCREEN", new EventHandler(this.ScreenRecorderButtonHandler));
							this.AddElement(sidebarElement, false);
							Sidebar.ChangeElementState(sidebarElement, false);
						}
						else
						{
							if (!(text == "sidebar_shake"))
							{
								goto IL_0609;
							}
							sidebarElement = this.CreateElement("sidebar_shake", "STRING_SHAKE", new EventHandler(this.ShakeHandler));
							this.AddElement(sidebarElement, false);
						}
					}
					else if (num != 1508932284U)
					{
						if (num != 1613331775U)
						{
							goto IL_0609;
						}
						if (!(text == "sidebar_location"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_location", "STRING_SET_LOCATION", new EventHandler(this.LocationHandler));
						this.AddElement(sidebarElement, false);
						Sidebar.ChangeElementState(sidebarElement, false);
					}
					else
					{
						if (!(text == "sidebar_screenshot"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_screenshot", "STRING_TOOLBAR_CAMERA", new EventHandler(this.ScreenshotHandler));
						this.AddElement(sidebarElement, false);
						Sidebar.ChangeElementState(sidebarElement, false);
					}
				}
				else if (num <= 2204537713U)
				{
					if (num <= 1828502634U)
					{
						if (num != 1740645932U)
						{
							if (num != 1828502634U)
							{
								goto IL_0609;
							}
							if (!(text == "sidebar_controls"))
							{
								goto IL_0609;
							}
							sidebarElement = this.CreateElement("sidebar_controls", "STRING_CONTROLS_EDITOR", new EventHandler(this.GameControlButtonHandler));
							sidebarElement.PreviewMouseRightButtonUp += this.KeyMapControlsButton_PreviewMouseRightButtonUp;
							sidebarElement.MouseLeave += this.GameControlButtonPopup_MouseLeave;
							this.AddElement(sidebarElement, false);
							Sidebar.ChangeElementState(sidebarElement, false);
						}
						else
						{
							if (!(text == "sidebar_stream_video"))
							{
								goto IL_0609;
							}
							sidebarElement = this.CreateElement("sidebar_stream_video", "STRING_START_STREAMING", new EventHandler(this.StreamingHandler));
							this.AddElement(sidebarElement, false);
							Sidebar.ChangeElementState(sidebarElement, false);
						}
					}
					else if (num != 1902815577U)
					{
						if (num != 2204537713U)
						{
							goto IL_0609;
						}
						if (!(text == "sidebar_operation"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_operation", "STRING_SYNCHRONISER", new EventHandler(this.OperationSyncHandler));
						this.AddElement(sidebarElement, false);
					}
					else
					{
						if (!(text == "sidebar_media_folder"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_media_folder", "STRING_OPEN_MEDIA_FOLDER", new EventHandler(this.MediaFolderHandler));
						this.AddElement(sidebarElement, false);
					}
				}
				else if (num <= 3250257129U)
				{
					if (num != 2888420628U)
					{
						if (num != 3250257129U)
						{
							goto IL_0609;
						}
						if (!(text == "sidebar_fullscreen"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_fullscreen", "STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", new EventHandler(this.FullScreenHandler));
						this.AddElement(sidebarElement, false);
						Sidebar.ChangeElementState(sidebarElement, false);
					}
					else
					{
						if (!(text == "sidebar_toggle"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_toggle", "STRING_TOGGLE_KEYMAPPING_STATE", new EventHandler(this.KeymapToggleHandler));
						this.AddElement(sidebarElement, false);
						Sidebar.ChangeElementState(sidebarElement, false);
					}
				}
				else if (num != 3462101782U)
				{
					if (num != 3723511498U)
					{
						if (num != 3976567256U)
						{
							goto IL_0609;
						}
						if (!(text == "sidebar_overlay"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_overlay", "STRING_TOGGLE_OVERLAY", new EventHandler(this.KeymappingControlsTransparencyButtonHandler));
						this.AddElement(sidebarElement, false);
						sidebarElement.MouseLeave += this.ChangeTransparencyPopup_MouseLeave;
						Sidebar.ChangeElementState(sidebarElement, false);
					}
					else
					{
						if (!(text == "sidebar_volume"))
						{
							goto IL_0609;
						}
						sidebarElement = this.CreateElement("sidebar_volume", "STRING_CHANGE_VOLUME", new EventHandler(this.VolumeButtonHandler));
						this.AddElement(sidebarElement, false);
						sidebarElement.MouseLeave += this.VolumeSliderPopup_MouseLeave;
						Sidebar.ChangeElementState(sidebarElement, false);
					}
				}
				else
				{
					if (!(text == "sidebar_mm"))
					{
						goto IL_0609;
					}
					sidebarElement = this.CreateElement("sidebar_mm", "STRING_TOGGLE_MULTIINSTANCE_WINDOW", new EventHandler(this.MIManagerHandler));
					this.AddElement(sidebarElement, false);
				}
				IL_061D:
				if (text == ls.Last<string>())
				{
					sidebarElement.IsLastElementOfGroup = true;
					sidebarElement.IsCurrentLastElementOfGroup = true;
					Sidebar.IncreaseElementBottomMarginIfLast(sidebarElement);
					continue;
				}
				continue;
				IL_0609:
				Logger.Warning("Unhandled sidebar element found: {0}", new object[] { text });
				goto IL_061D;
			}
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x00032994 File Offset: 0x00030B94
		private void StreamingHandler(object sender, EventArgs e)
		{
			bool mIsStreaming = this.ParentWindow.mIsStreaming;
			NCSoftUtils.Instance.SendStreamingEvent(this.ParentWindow.mVmName, mIsStreaming ? "off" : "on");
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, mIsStreaming ? "StreamVideoOff" : "StreamVideoOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x00032A1C File Offset: 0x00030C1C
		private void GamepadControlsWindowHandler(object sender, EventArgs e)
		{
			if (!this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad"))
			{
				KMManager.HandleInputMapperWindow(this.ParentWindow, "gamepad");
			}
			string text = "sidebar";
			string userGuid = RegistryManager.Instance.UserGuid;
			string text2 = "GamePad";
			string text3 = "MouseClick";
			string clientVersion = RegistryManager.Instance.ClientVersion;
			string version = RegistryManager.Instance.Version;
			string oem = RegistryManager.Instance.Oem;
			AppTabButton selectedTab = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
			ClientStats.SendMiscellaneousStatsAsync(text, userGuid, text2, text3, clientVersion, version, oem, (selectedTab != null) ? selectedTab.PackageName : null, null);
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x00032AB0 File Offset: 0x00030CB0
		private void MediaFolderHandler(object sender, EventArgs e)
		{
			CommonHandlers.OpenMediaFolder();
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MediaFolder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x00032B00 File Offset: 0x00030D00
		private void GoBackHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.BackButtonHandler(false);
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Back", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x00007AC1 File Offset: 0x00005CC1
		private void GoHomeHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.HomeButtonHandler(true, false);
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x00032B5C File Offset: 0x00030D5C
		private void GoSettingsHandler(object sender, EventArgs e)
		{
			string text = string.Empty;
			if (this.ParentWindow.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName))
			{
				text = "STRING_GAME_SETTINGS";
			}
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			this.ParentWindow.mCommonHandler.LaunchSettingsWindow(text);
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x00032BFC File Offset: 0x00030DFC
		private void MIManagerHandler(object sender, EventArgs e)
		{
			try
			{
				Process.Start(global::System.IO.Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"), "-IsMIMLaunchedFromClient");
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MultiInstance", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't launch MI Manager. Ex: {0}", new object[] { ex.Message });
			}
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00007AD5 File Offset: 0x00005CD5
		private void RotateHandler(object sender, EventArgs e)
		{
			this.RotateButtonHandler("MouseClick");
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x00032C90 File Offset: 0x00030E90
		internal void RotateButtonHandler(string action)
		{
			this.mIsUIInPortraitModeBeforeChange = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsPortraitModeTab;
			this.ParentWindow.AppForcedOrientationDict[this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName] = !this.mIsUIInPortraitModeBeforeChange;
			this.ParentWindow.ChangeOrientationFromClient(!this.mIsUIInPortraitModeBeforeChange, true);
			string text = (this.mIsUIInPortraitModeBeforeChange ? "landscape" : "portrait");
			string text2 = "sidebar";
			string userGuid = RegistryManager.Instance.UserGuid;
			string text3 = "Rotate";
			string clientVersion = RegistryManager.Instance.ClientVersion;
			string version = RegistryManager.Instance.Version;
			string oem = RegistryManager.Instance.Oem;
			string text4 = text;
			AppTabButton selectedTab = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
			ClientStats.SendMiscellaneousStatsAsync(text2, userGuid, text3, action, clientVersion, version, oem, text4, (selectedTab != null) ? selectedTab.PackageName : null);
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00032D7C File Offset: 0x00030F7C
		private void ShakeHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.ShakeButtonHandler();
			string text = "sidebar";
			string userGuid = RegistryManager.Instance.UserGuid;
			string text2 = "Shake";
			string text3 = "MouseClick";
			string clientVersion = RegistryManager.Instance.ClientVersion;
			string version = RegistryManager.Instance.Version;
			string oem = RegistryManager.Instance.Oem;
			AppTabButton selectedTab = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
			ClientStats.SendMiscellaneousStatsAsync(text, userGuid, text2, text3, clientVersion, version, oem, (selectedTab != null) ? selectedTab.PackageName : null, null);
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00032DF8 File Offset: 0x00030FF8
		private void LocationHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.LocationButtonHandler();
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "SetLocation", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00007AE2 File Offset: 0x00005CE2
		private void LockCursorHandler(object sender, EventArgs e)
		{
			new Sidebar.ControlWindow().ShowDialog();
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x00007AEF File Offset: 0x00005CEF
		private void ScreenshotHandler(object sender, EventArgs e)
		{
			ToolTipService.SetToolTip(sender as SidebarElement, null);
			this.mScreenshotPopup.IsOpen = false;
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				Thread.Sleep(100);
				this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Screenshot", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			});
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x00032E54 File Offset: 0x00031054
		private void VolumeButtonHandler(object sender, EventArgs e)
		{
			if (!this.GetElementFromTag("sidebar_volume").IsInMainSidebar)
			{
				this.mVolumeSliderPopup.StaysOpen = false;
			}
			else
			{
				this.mVolumeSliderPopup.StaysOpen = true;
			}
			if (this.mVolumeSliderPopup.IsOpen)
			{
				this.mVolumeSliderPopup.IsOpen = false;
				return;
			}
			this.mVolumeSliderPopup.IsOpen = true;
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x00007B1B File Offset: 0x00005D1B
		private void FullScreenHandler(object sender, EventArgs e)
		{
			if (!this.ParentWindow.mStreamingModeEnabled)
			{
				this.ParentWindow.mCommonHandler.FullScreenButtonHandler("sidebar", "MouseClick");
			}
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x00032EB4 File Offset: 0x000310B4
		internal SidebarElement GetElementFromTag(string tag)
		{
			if (this.mListSidebarElements.Count >= 1)
			{
				return this.mListSidebarElements.Where((SidebarElement item) => (string)item.Tag == tag).FirstOrDefault<SidebarElement>();
			}
			return null;
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x00007B44 File Offset: 0x00005D44
		public void AddElement(SidebarElement ele, bool isStaticElement = false)
		{
			if (isStaticElement)
			{
				this.mStaticButtonsStackPanel.Children.Add(ele);
				return;
			}
			this.mElementsStackPanel.Children.Add(ele);
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x00007B6E File Offset: 0x00005D6E
		public void UpdateToDefaultImage(string tag)
		{
			Sidebar.UpdateToDefaultImage(this.GetElementFromTag(tag));
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x00007B7C File Offset: 0x00005D7C
		public void UpdateImage(string tag, string newImage)
		{
			Sidebar.UpdateImage(this.GetElementFromTag(tag), newImage);
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x00007B8B File Offset: 0x00005D8B
		public static void UpdateToDefaultImage(SidebarElement ele)
		{
			if (ele != null)
			{
				ele.Image.ImageName = (string)ele.Tag;
			}
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x00007BA6 File Offset: 0x00005DA6
		public static void UpdateImage(SidebarElement ele, string newImage)
		{
			if (ele != null)
			{
				ele.Image.ImageName = newImage;
			}
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00032EFC File Offset: 0x000310FC
		private static void DecreaseElementBottomMargin(SidebarElement ele)
		{
			Thickness margin = ele.Margin;
			margin.Bottom = 2.0;
			ele.Margin = margin;
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x00032F28 File Offset: 0x00031128
		private static void IncreaseElementBottomMarginIfLast(SidebarElement ele)
		{
			if (ele.IsCurrentLastElementOfGroup)
			{
				Thickness margin = ele.Margin;
				margin.Bottom = 10.0;
				ele.Margin = margin;
			}
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00032F5C File Offset: 0x0003115C
		private SidebarElement CreateElement(string imageName, string toolTipKey, EventHandler evt)
		{
			SidebarElement sidebarElement = new SidebarElement
			{
				Margin = new Thickness(0.0, 2.0, 0.0, 2.0),
				Visibility = Visibility.Visible,
				mSidebarElementTooltipKey = toolTipKey
			};
			sidebarElement.Image.ImageName = imageName;
			sidebarElement.Tag = imageName;
			sidebarElement.PreviewMouseLeftButtonUp += this.SidebarElement_PreviewMouseLeftButtonUp;
			sidebarElement.IsVisibleChanged += this.SidebarElement_IsVisibleChanged;
			this.SetSidebarElementTooltip(sidebarElement, toolTipKey);
			this.mDictActions.Add(sidebarElement, evt);
			this.mListSidebarElements.Add(sidebarElement);
			return sidebarElement;
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x00033008 File Offset: 0x00031208
		internal void SetSidebarElementTooltip(SidebarElement ele, string toolTipKey)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				string text;
				if (ele.Tag.ToString() == "sidebar_volume")
				{
					text = Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_INCREASE_VOLUME", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_INCREASE_VOLUME", false), " ");
					text = text + "\n" + Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_DECREASE_VOLUME", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_DECREASE_VOLUME", false), " ");
					this.mVolumeMuteUnmuteImage.ToolTip = Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_TOGGLE_MUTE_STATE", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_MUTE_STATE", false), "\n");
				}
				else
				{
					text = Sidebar.GetTooltip(LocaleStrings.GetLocalizedString(toolTipKey, ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName(toolTipKey, false), "\n");
				}
				ele.ToolTip = new ToolTip
				{
					Content = text
				};
			}), new object[0]);
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x00033054 File Offset: 0x00031254
		private static string GetTooltip(string text, string shortcut, string delimiter = "\n")
		{
			if (!string.IsNullOrEmpty(shortcut))
			{
				return string.Format(CultureInfo.InvariantCulture, string.Concat(new string[] { text, delimiter, "(", shortcut, ")" }), new object[0]);
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x000330AC File Offset: 0x000312AC
		private void SidebarElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!this.mIsOneSidebarElementLoadedBinded && (bool)e.NewValue)
			{
				this.mIsOneSidebarElementLoadedBinded = true;
				SidebarElement sidebarElement = sender as SidebarElement;
				if (sidebarElement != null && this.mSidebarElementApproxHeight == 0.0)
				{
					int num = (int)sidebarElement.Height + 2 * (int)sidebarElement.Margin.Top;
					int num2 = this.mListSidebarElements.Where((SidebarElement item) => item.IsLastElementOfGroup).Count<SidebarElement>();
					int num3 = num * this.mElementsStackPanel.Children.Count + (num2 - 1) * 8;
					this.mSidebarElementApproxHeight = (double)(num3 / this.mElementsStackPanel.Children.Count + 2);
					Logger.Info("Aprrox: {0}", new object[] { this.mSidebarElementApproxHeight });
				}
			}
			this.UpdateTotalVisibleElementCount();
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x000331A0 File Offset: 0x000313A0
		private void SidebarElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			SidebarElement sidebarElement = sender as SidebarElement;
			if (this.mDictActions.ContainsKey(sidebarElement))
			{
				EventHandler eventHandler = this.mDictActions[sidebarElement];
				if (eventHandler == null)
				{
					return;
				}
				eventHandler(sidebarElement, new EventArgs());
			}
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x00007BB7 File Offset: 0x00005DB7
		private void MSidebar_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ArrangeAllSidebarElements();
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x000331E0 File Offset: 0x000313E0
		internal void ArrangeAllSidebarElements()
		{
			try
			{
				if (base.Visibility == Visibility.Visible)
				{
					int num = Math.Min((int)(Math.Max(0.0, base.ActualHeight - this.mBottomGrid.ActualHeight - 54.0) / this.mSidebarElementApproxHeight), this.mTotalVisibleElements);
					List<SidebarElement> list = (from item in this.mElementsStackPanel.Children.OfType<SidebarElement>()
						where item.Visibility == Visibility.Visible
						select item).ToList<SidebarElement>();
					int count = list.Count;
					if (count > num)
					{
						int num2 = count - num;
						for (int i = 1; i <= num2; i++)
						{
							SidebarElement sidebarElement = list[count - i];
							this.mElementsStackPanel.Children.Remove(sidebarElement);
							sidebarElement.IsInMainSidebar = false;
						}
					}
					else if (count < num)
					{
						int num3 = num - count;
						SidebarElement[] array = this.mListSidebarElements.Where((SidebarElement item) => !item.IsInMainSidebar).ToArray<SidebarElement>();
						for (int j = 0; j < num3; j++)
						{
							if (array.Length > j)
							{
								SidebarElement sidebarElement2 = array[j];
								sidebarElement2.IsInMainSidebar = true;
								this.AddToVisibleElementsPanel(sidebarElement2);
							}
						}
					}
					if (this.mListSidebarElements.Any((SidebarElement x) => !x.IsInMainSidebar))
					{
						this.mMoreButton.Visibility = Visibility.Collapsed;
					}
					else
					{
						this.mMoreButton.Visibility = Visibility.Collapsed;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("XXX SR: An error occured while rearranging elements. Ex: {0}", new object[] { ex });
			}
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x000333A0 File Offset: 0x000315A0
		private void AddToVisibleElementsPanel(SidebarElement ele)
		{
			StackPanel stackPanel = ele.Parent as StackPanel;
			if (stackPanel != null)
			{
				stackPanel.Children.Remove(ele);
			}
			this.mElementsStackPanel.Children.Add(ele);
			Sidebar.IncreaseElementBottomMarginIfLast(ele);
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x00007BBF File Offset: 0x00005DBF
		public void SetHeight()
		{
			base.Height = this.ParentWindow.mContentGrid.ActualHeight;
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x000333E0 File Offset: 0x000315E0
		private void MMoreElements_Opened(object sender, EventArgs e)
		{
			this.SidebarPopupContentClear();
			this.mSidebarPopup.InitAllElements(this.mListSidebarElements.Where((SidebarElement x) => !x.IsInMainSidebar));
			Sidebar.UpdateImage(this.mMoreButton, "sidebar_options_open");
			BlueStacksUIBinding.Bind(this.mMoreButton, "STRING_CLOSE");
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00007BD7 File Offset: 0x00005DD7
		private void MMoreElements_Closed(object sender, EventArgs e)
		{
			this.SidebarPopupContentClear();
			Sidebar.UpdateImage(this.mMoreButton, "sidebar_options_close");
			BlueStacksUIBinding.Bind(this.mMoreButton, "STRING_MORE_BUTTON");
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x00033448 File Offset: 0x00031648
		private void SidebarPopupContentClear()
		{
			foreach (object obj in this.mSidebarPopup.mMainStackPanel.Children)
			{
				((StackPanel)obj).Children.Clear();
			}
			this.mSidebarPopup.mMainStackPanel.Children.Clear();
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void MSidebarPopup_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x00007C08 File Offset: 0x00005E08
		private void MMoreButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mMoreElements.IsOpen = !this.mMoreElements.IsOpen;
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x000334C4 File Offset: 0x000316C4
		private void MMoreButton_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.mMoreButton.IsMouseOver)
			{
				if (this.mListPopups.All((CustomPopUp x) => !x.IsMouseOver) && this.mMoreElements.IsOpen)
				{
					if (this.mSidebarPopupTimer == null)
					{
						this.mSidebarPopupTimer = new DispatcherTimer
						{
							Interval = new TimeSpan(0, 0, 0, 0, 500)
						};
						this.mSidebarPopupTimer.Tick += this.SidebarPopupTimer_Tick;
					}
					else
					{
						this.mSidebarPopupTimer.Stop();
					}
					this.mSidebarPopupTimer.Start();
				}
			}
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00033570 File Offset: 0x00031770
		private void SidebarPopupTimer_Tick(object sender, EventArgs e)
		{
			if (!this.mMoreButton.IsMouseOver)
			{
				if (this.mListPopups.All((CustomPopUp x) => !x.IsMouseOver))
				{
					this.mListPopups.Select(delegate(CustomPopUp c)
					{
						c.IsOpen = false;
						return c;
					}).ToList<CustomPopUp>();
					if (this.mIsInFullscreenMode && !base.IsMouseOver)
					{
						this.ToggleSidebarVisibilityInFullscreen(false);
					}
				}
			}
			(sender as DispatcherTimer).Stop();
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void ClosePopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x00033608 File Offset: 0x00031808
		private void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.transSlider.Value == 0.0)
			{
				this.transSlider.Value = this.mLastSliderValue;
				this.mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_popup";
				return;
			}
			double value = this.transSlider.Value;
			this.transSlider.Value = 0.0;
			this.mLastSliderValue = value;
			this.mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive_popup";
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00033684 File Offset: 0x00031884
		private void TransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			KMManager.ChangeTransparency(this.ParentWindow, this.transSlider.Value);
			if (this.transSlider.Value == 0.0)
			{
				this.mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_inactive_popup";
				if (!RegistryManager.Instance.ShowKeyControlsOverlay)
				{
					KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
				}
				this.ParentWindow_OverlayStateChangedEvent(false);
			}
			else
			{
				this.mTranslucentControlsSliderButton.ImageName = "sidebar_overlay_popup";
				KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
				this.ParentWindow_OverlayStateChangedEvent(true);
			}
			this.mLastSliderValue = this.transSlider.Value;
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00007C23 File Offset: 0x00005E23
		private void OverlayTooltipCPB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mOverlayTooltip.IsOpen = false;
			this.mIsOverlayTooltipClosed = true;
			e.Handled = true;
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00033724 File Offset: 0x00031924
		private void OverlayDoNotShowCheckbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (string.Equals(this.mOverlayDoNotShowCheckboxImage.ImageName, "bgpcheckbox", StringComparison.InvariantCulture))
			{
				this.mOverlayDoNotShowCheckboxImage.ImageName = "bgpcheckbox_checked";
				RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
			}
			else
			{
				this.mOverlayDoNotShowCheckboxImage.ImageName = "bgpcheckbox";
				RegistryManager.Instance.OverlayAvailablePromptEnabled = true;
			}
			e.Handled = true;
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x00007C3F File Offset: 0x00005E3F
		private void ScreenRecorderButtonHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.DownloadAndLaunchRecording("sidebar", "MouseClick");
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x00007C5B File Offset: 0x00005E5B
		private void RecordScreenPopupClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mRecordScreenPopup.IsOpen = false;
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x00033788 File Offset: 0x00031988
		private void RecordScreenPopup_Closed(object sender, EventArgs e)
		{
			if (CommonHandlers.sIsRecordingVideo)
			{
				BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_STOP_RECORDING", "");
				this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
				this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
				this.mRecordScreenClose.Visibility = Visibility.Collapsed;
			}
			else
			{
				BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_RECORD_SCREEN", "");
				BlueStacksUIBinding.Bind(this.RecordScreenPopupBody, "STRING_RECORD_SCREEN_PLAYING", "");
				this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
				this.RecordScreenPopupBody.Visibility = Visibility.Visible;
				this.mRecordScreenClose.Visibility = Visibility.Collapsed;
			}
			this.mRecordScreenPopup.StaysOpen = true;
			this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x00033840 File Offset: 0x00031A40
		private void KeymappingControlsTransparencyButtonHandler(object sender, EventArgs e)
		{
			RegistryManager.Instance.ShowKeyControlsOverlay = true;
			RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
			KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
			if (!this.GetElementFromTag("sidebar_overlay").IsInMainSidebar)
			{
				this.mChangeTransparencyPopup.StaysOpen = false;
			}
			else
			{
				this.mChangeTransparencyPopup.StaysOpen = true;
			}
			this.mChangeTransparencyPopup.IsOpen = true;
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00007C69 File Offset: 0x00005E69
		private void RecordScreenPopupHyperlink_Click(object sender, RoutedEventArgs e)
		{
			CommonHandlers.OpenMediaFolderWithFileSelected(CommonHandlers.mSavedVideoRecordingFilePath);
			this.mRecordScreenPopup.IsOpen = false;
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x000338E8 File Offset: 0x00031AE8
		private void RecordScreenClose_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				this.RecordScreenPopupHeader.Margin = new Thickness(0.0, 0.0, 20.0, 0.0);
				return;
			}
			this.RecordScreenPopupHeader.Margin = new Thickness(0.0);
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x00033954 File Offset: 0x00031B54
		internal void SidebarVisiblityChanged(Visibility currentVisibility)
		{
			if (!this.mIsInFullscreenMode && base.IsLoaded)
			{
				if (currentVisibility == Visibility.Visible)
				{
					this.ParentWindow.ParentWindowWidthDiff = 62;
					this.ParentWindow.Width = this.ParentWindow.ActualWidth + base.Width;
					this.ArrangeAllSidebarElements();
				}
				else
				{
					this.ParentWindow.ParentWindowWidthDiff = 2;
					this.ParentWindow.Width = Math.Max(this.ParentWindow.ActualWidth - base.Width, this.ParentWindow.MinWidth);
				}
				this.ParentWindow.HandleDisplaySettingsChanged();
				this.ParentWindow.Height = this.ParentWindow.GetHeightFromWidth(this.ParentWindow.Width, false, false);
			}
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x00007C81 File Offset: 0x00005E81
		private void MMacroButtonPopup_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mMacroButtonPopup.IsOpen = true;
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x00033A14 File Offset: 0x00031C14
		internal void ShowOverlayTooltip(bool isShow, bool force = false)
		{
			if (this.GetElementFromTag("sidebar_overlay") == null || !RegistryManager.Instance.OverlayAvailablePromptEnabled)
			{
				return;
			}
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (isShow)
				{
					this.mIsPendingShowOverlayTooltip = true;
					this.ActualOverlayTooltip(force);
					return;
				}
				this.mOverlayTooltip.IsOpen = false;
			}), new object[0]);
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x00033A74 File Offset: 0x00031C74
		private void ActualOverlayTooltip(bool force = false)
		{
			if (RegistryManager.Instance.OverlayAvailablePromptEnabled && !this.mIsOverlayTooltipClosed && this.mIsPendingShowOverlayTooltip && (!RegistryManager.Instance.IsAutoShowGuidance || Array.Exists<string>(RegistryManager.Instance.DisabledGuidancePackages, (string element) => element == this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName) || force) && !this.mIsInFullscreenMode && !FeatureManager.Instance.IsCustomUIForNCSoft && base.Visibility == Visibility.Visible)
			{
				this.mIsPendingShowOverlayTooltip = false;
				this.mOverlayTooltip.IsOpen = true;
			}
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x00033AFC File Offset: 0x00031CFC
		private void ActualKeymapPopup()
		{
			if (!RegistryManager.Instance.OverlayAvailablePromptEnabled || this.mIsOverlayTooltipClosed)
			{
				string packageName = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
				if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(packageName))
				{
					AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName] = new AppSettings();
				}
				if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName].IsKeymappingTooltipShown)
				{
					return;
				}
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName].IsKeymappingTooltipShown = true;
			}
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00033BD0 File Offset: 0x00031DD0
		internal void ShowKeyMapPopup(bool isShow)
		{
			if (this.GetElementFromTag("sidebar_controls") == null)
			{
				return;
			}
			if (isShow)
			{
				if (!Array.Exists<string>(RegistryManager.Instance.DisabledGuidancePackages, (string element) => element == this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName) && RegistryManager.Instance.IsAutoShowGuidance)
				{
					KMManager.HandleInputMapperWindow(this.ParentWindow, "");
					return;
				}
				this.ActualKeymapPopup();
			}
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x00033C30 File Offset: 0x00031E30
		private void OpenMacroGridPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
			this.mMacroButtonPopup.IsOpen = false;
			this.ToggleSidebarVisibilityInFullscreen(false);
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "MacroBookmarkPopup", null);
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x00007C8F File Offset: 0x00005E8F
		private void OpenMacroGridMouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x00007CA6 File Offset: 0x00005EA6
		private void OpenMacroGridMouseLeave(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "SidebarBackground");
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x00033CA4 File Offset: 0x00031EA4
		private void VolumeImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.MuteUnmuteButtonHanlder();
			if (this.ParentWindow.EngineInstanceRegistry.IsMuted)
			{
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				return;
			}
			ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOff", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x00033D50 File Offset: 0x00031F50
		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			this.mCurrentVolumeValue.Text = Math.Round(e.NewValue).ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x00033D80 File Offset: 0x00031F80
		private void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			int num = Convert.ToInt32(this.mVolumeSlider.Value);
			this.ParentWindow.Utils.SetVolumeInFrontendAsync(num);
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00033DB0 File Offset: 0x00031FB0
		private void MuteInstancesCheckboxImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mMuteInstancesCheckboxImage.ImageName.Equals("bgpcheckbox", StringComparison.OrdinalIgnoreCase))
			{
				this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox_checked";
				RegistryManager.Instance.AreAllInstancesMuted = true;
				this.SendMuteUnmuteRequestToAllInstances(true);
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOffAll", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
			else
			{
				this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox";
				RegistryManager.Instance.AreAllInstancesMuted = false;
				this.SendMuteUnmuteRequestToAllInstances(false);
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOnAll", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
			e.Handled = true;
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x00033EA0 File Offset: 0x000320A0
		private void SendMuteUnmuteRequestToAllInstances(bool isMute)
		{
			foreach (string text in RegistryManager.Instance.VmList)
			{
				if (isMute)
				{
					if (BlueStacksUIUtils.DictWindows.Keys.Contains(text))
					{
						BlueStacksUIUtils.DictWindows[text].Utils.MuteApplication(true);
					}
				}
				else if (BlueStacksUIUtils.DictWindows.Keys.Contains(text))
				{
					BlueStacksUIUtils.DictWindows[text].Utils.UnmuteApplication(true);
				}
			}
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x00033F20 File Offset: 0x00032120
		private void ChangeTransparencyPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.mChangeTransparencyPopup.IsOpen)
			{
				if (this.mChangeTransparencyPopupTimer == null)
				{
					this.mChangeTransparencyPopupTimer = new DispatcherTimer
					{
						Interval = new TimeSpan(0, 0, 0, 0, 500)
					};
					this.mChangeTransparencyPopupTimer.Tick += this.ChangeTransparencyPopupTimer_Tick;
				}
				else
				{
					this.mChangeTransparencyPopupTimer.Stop();
				}
				this.mChangeTransparencyPopupTimer.Start();
			}
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x00033F90 File Offset: 0x00032190
		private void ChangeTransparencyPopupTimer_Tick(object sender, EventArgs e)
		{
			if (!this.mChangeTransparencyPopup.IsMouseOver && !this.GetElementFromTag("sidebar_overlay").IsMouseOver)
			{
				this.mChangeTransparencyPopup.IsOpen = false;
				if (this.mIsInFullscreenMode && !base.IsMouseOver)
				{
					this.ToggleSidebarVisibilityInFullscreen(false);
				}
			}
			(sender as DispatcherTimer).Stop();
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x00007CBD File Offset: 0x00005EBD
		private void ChangeTransparencyPopup_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!this.GetElementFromTag("sidebar_overlay").IsInMainSidebar)
			{
				this.mChangeTransparencyPopup.StaysOpen = false;
			}
			else
			{
				this.mChangeTransparencyPopup.StaysOpen = true;
			}
			this.mChangeTransparencyPopup.IsOpen = true;
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x00007CF7 File Offset: 0x00005EF7
		private void VolumeSliderPopup_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mVolumeSliderPopup.IsOpen = true;
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x00033FEC File Offset: 0x000321EC
		private void VolumeSliderPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.mVolumeSliderPopup.IsOpen)
			{
				if (this.mVolumeSliderPopupTimer == null)
				{
					this.mVolumeSliderPopupTimer = new DispatcherTimer
					{
						Interval = new TimeSpan(0, 0, 1)
					};
					this.mVolumeSliderPopupTimer.Tick += this.VolumeSliderPopupTimer_Tick;
				}
				else
				{
					this.mVolumeSliderPopupTimer.Stop();
				}
				this.mVolumeSliderPopupTimer.Start();
			}
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x00034058 File Offset: 0x00032258
		internal void VolumeSliderPopupTimer_Tick(object sender, EventArgs e)
		{
			if (!this.mVolumeSliderPopup.IsMouseOver && !this.GetElementFromTag("sidebar_volume").IsMouseOver)
			{
				this.mVolumeSliderPopup.IsOpen = false;
				if (this.mIsInFullscreenMode && !base.IsMouseOver)
				{
					this.ToggleSidebarVisibilityInFullscreen(false);
				}
			}
			(sender as DispatcherTimer).Stop();
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x00007D05 File Offset: 0x00005F05
		private void ScreenshotPopupClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mScreenshotPopup.IsOpen = false;
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x00007D13 File Offset: 0x00005F13
		private void ScreenshotPopupHyperlink_Click(object sender, RoutedEventArgs e)
		{
			CommonHandlers.OpenMediaFolderWithFileSelected(this.currentScreenshotSavedPath);
			this.mScreenshotPopup.IsOpen = false;
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x00007D2C File Offset: 0x00005F2C
		private void GameControlButtonPopup_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!this.GetElementFromTag("sidebar_controls").IsInMainSidebar)
			{
				this.mGameControlButtonPopup.StaysOpen = false;
			}
			else
			{
				this.mGameControlButtonPopup.StaysOpen = true;
			}
			this.mGameControlButtonPopup.IsOpen = true;
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x000340B4 File Offset: 0x000322B4
		private void GameControlButtonPopup_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.mGameControlButtonPopup.IsOpen)
			{
				if (this.mGameControlBookmarkTimer == null)
				{
					this.mGameControlBookmarkTimer = new DispatcherTimer
					{
						Interval = new TimeSpan(0, 0, 0, 0, 500)
					};
					this.mGameControlBookmarkTimer.Tick += this.GameControlBookmarkTimer_Tick;
				}
				else
				{
					this.mGameControlBookmarkTimer.Stop();
				}
				this.mGameControlBookmarkTimer.Start();
			}
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x00034124 File Offset: 0x00032324
		private void GameControlBookmarkTimer_Tick(object sender, EventArgs e)
		{
			if (!this.mGameControlButtonPopup.IsMouseOver && !this.GetElementFromTag("sidebar_controls").IsMouseOver)
			{
				this.mGameControlButtonPopup.IsOpen = false;
				if (this.ParentWindow.mIsFullScreen)
				{
					this.ToggleSidebarVisibilityInFullscreen(false);
				}
			}
			(sender as DispatcherTimer).Stop();
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0003417C File Offset: 0x0003237C
		private void GameControlButtonHandler(object sender, EventArgs e)
		{
			bool flag = true;
			this.mBookmarkedSchemesStackPanel.Children.Clear();
			foreach (IMControlScheme imcontrolScheme in this.ParentWindow.SelectedConfig.ControlSchemes)
			{
				if (imcontrolScheme.IsBookMarked)
				{
					SchemeBookmarkControl schemeBookmarkControl = new SchemeBookmarkControl(imcontrolScheme, this.ParentWindow);
					this.mBookmarkedSchemesStackPanel.Children.Add(schemeBookmarkControl);
					flag = false;
				}
			}
			if (!flag)
			{
				if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed)
				{
					KMManager.sGuidanceWindow.Close();
				}
				if (!this.GetElementFromTag("sidebar_controls").IsInMainSidebar)
				{
					this.mGameControlButtonPopup.StaysOpen = false;
				}
				else
				{
					this.mGameControlButtonPopup.StaysOpen = true;
				}
				this.mGameControlButtonPopup.IsOpen = true;
				return;
			}
			this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "sidebar");
			this.mGameControlButtonPopup.IsOpen = false;
			this.ToggleSidebarVisibilityInFullscreen(false);
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x00004786 File Offset: 0x00002986
		private void OpenGameControlPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x00004786 File Offset: 0x00002986
		private void OpenGameControlMouseEnter(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x00004786 File Offset: 0x00002986
		private void OpenGameControlMouseLeave(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x00034294 File Offset: 0x00032494
		internal void ChangeVideoRecordingImage(string imageName)
		{
			SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
			if (elementFromTag != null)
			{
				elementFromTag.Image.IsImageToBeRotated = false;
				Sidebar.UpdateImage(elementFromTag, imageName);
			}
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x000342C4 File Offset: 0x000324C4
		private void SetVideoRecordingTooltipForNCSoft()
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
				if (elementFromTag != null)
				{
					ToolTip toolTip = elementFromTag.ToolTip as ToolTip;
					toolTip.Content = Convert.ToString(toolTip.Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""));
				}
			}
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x00034330 File Offset: 0x00032530
		internal void SetVideoRecordingTooltip(bool isRecording)
		{
			SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
			if (elementFromTag != null)
			{
				ToolTip toolTip = elementFromTag.ToolTip as ToolTip;
				if (isRecording)
				{
					if (FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						toolTip.Content = Convert.ToString(toolTip.Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""), LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""));
						return;
					}
					toolTip.Content = Convert.ToString(toolTip.Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""), LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""));
					return;
				}
				else
				{
					if (FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						toolTip.Content = Convert.ToString(toolTip.Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""));
						return;
					}
					toolTip.Content = Convert.ToString(toolTip.Content, CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""));
				}
			}
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x00007D66 File Offset: 0x00005F66
		private void VolumeSliderPopup_Closed(object sender, EventArgs e)
		{
			if (!this.GetElementFromTag("sidebar_volume").IsInMainSidebar)
			{
				this.MMoreButton_MouseLeave(null, null);
			}
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x00007D82 File Offset: 0x00005F82
		private void GameControlButtonPopup_Closed(object sender, EventArgs e)
		{
			if (!this.GetElementFromTag("sidebar_controls").IsInMainSidebar)
			{
				this.MMoreButton_MouseLeave(null, null);
			}
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x00007D9E File Offset: 0x00005F9E
		private void ChangeTransparencyPopup_Closed(object sender, EventArgs e)
		{
			if (!this.GetElementFromTag("sidebar_overlay").IsInMainSidebar)
			{
				this.MMoreButton_MouseLeave(null, null);
			}
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x00007DBA File Offset: 0x00005FBA
		private void OpenGameGuideButtonHandler(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.GameGuideButtonHandler("MouseClick", "sidebar");
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x00034460 File Offset: 0x00032660
		public OnBoardingPopupWindow FullscreenOnboardingBlurb()
		{
			if (this.ParentWindow.mSidebar.Visibility == Visibility.Collapsed || string.IsNullOrEmpty(this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false)))
			{
				return null;
			}
			SidebarElement sidebarElement = (from SidebarElement s in this.ParentWindow.mSidebar.mElementsStackPanel.Children
				where s.Image.ImageName == "sidebar_fullscreen"
				select s).FirstOrDefault<SidebarElement>();
			if (sidebarElement == null)
			{
				return null;
			}
			OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName)
			{
				Owner = this.ParentWindow,
				PlacementTarget = sidebarElement,
				Title = "FullScreenBlurb",
				LeftMargin = 310,
				TopMargin = 45,
				IsBlurbRelatedToGuidance = false,
				HeaderContent = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_HEADER", "")
			};
			onBoardingPopupWindow.bodyTextBlock.Visibility = Visibility.Collapsed;
			onBoardingPopupWindow.bodyContentBlurbControl.Visibility = Visibility.Visible;
			onBoardingPopupWindow.bodyContentBlurbControl.FirstMessage.Text = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_MESSAGE", "");
			onBoardingPopupWindow.bodyContentBlurbControl.KeyMessage.Text = this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false);
			onBoardingPopupWindow.bodyContentBlurbControl.SecondMessage.Text = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_FULL_SCREEN_MESSAGE", "");
			onBoardingPopupWindow.PopArrowAlignment = PopupArrowAlignment.Right;
			onBoardingPopupWindow.SetValue(Window.LeftProperty, sidebarElement.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin);
			onBoardingPopupWindow.SetValue(Window.TopProperty, sidebarElement.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin);
			onBoardingPopupWindow.RightArrow.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
			onBoardingPopupWindow.RightArrow.VerticalAlignment = VerticalAlignment.Top;
			return onBoardingPopupWindow;
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x000346A8 File Offset: 0x000328A8
		public void ShowViewGuidancePopup()
		{
			SidebarElement sidebarElement = (from SidebarElement s in this.ParentWindow.mSidebar.mStaticButtonsStackPanel.Children
				where s.Image.ImageName == "sidebar_gameguide" || s.Image.ImageName == "sidebar_gameguide_active"
				select s).FirstOrDefault<SidebarElement>();
			if (sidebarElement == null)
			{
				return;
			}
			this.mGameControlsBlurbPopup.PlacementTarget = sidebarElement;
			this.mGameControlsBlurbPopup.IsOpen = true;
			this.mGameControlsBlurbPopup.IsTopmost = true;
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x00034724 File Offset: 0x00032924
		private void OnBoardingPopupNext_Click(object sender, RoutedEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("general-onboarding", "okay_clicked", this.ParentWindow.mVmName, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, "ViewControlBlurb", "");
			this.mGameControlsBlurbPopup.IsOpen = false;
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0003477C File Offset: 0x0003297C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/sidebar.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x000347AC File Offset: 0x000329AC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.mSidebar = (Sidebar)target;
				this.mSidebar.SizeChanged += this.MSidebar_SizeChanged;
				this.mSidebar.Loaded += this.Sidebar_Loaded;
				return;
			case 2:
				this.mBorder = (Border)target;
				return;
			case 3:
				this.mGrid = (Grid)target;
				return;
			case 4:
				this.mTopGrid = (Grid)target;
				return;
			case 5:
				this.mElementsStackPanel = (StackPanel)target;
				return;
			case 6:
				this.mMoreButton = (SidebarElement)target;
				return;
			case 7:
				this.mChangeTransparencyPopup = (CustomPopUp)target;
				return;
			case 8:
				this.mMaskBorder2 = (Border)target;
				return;
			case 9:
				this.mTranslucentControlsSliderButton = (CustomPictureBox)target;
				this.mTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp;
				return;
			case 10:
				this.transSlider = (Slider)target;
				this.transSlider.ValueChanged += this.TransparencySlider_ValueChanged;
				return;
			case 11:
				this.mVolumeSliderPopup = (CustomPopUp)target;
				return;
			case 12:
				this.mMaskBorder3 = (Border)target;
				return;
			case 13:
				this.mVolumeMuteUnmuteImage = (CustomPictureBox)target;
				this.mVolumeMuteUnmuteImage.PreviewMouseLeftButtonUp += this.VolumeImage_PreviewMouseLeftButtonUp;
				return;
			case 14:
				this.mVolumeSlider = (Slider)target;
				this.mVolumeSlider.PreviewMouseLeftButtonUp += this.VolumeSlider_PreviewMouseLeftButtonUp;
				this.mVolumeSlider.ValueChanged += this.VolumeSlider_ValueChanged;
				return;
			case 15:
				this.mCurrentVolumeValue = (TextBlock)target;
				return;
			case 16:
				this.mMuteInstancesCheckboxImage = (CustomPictureBox)target;
				this.mMuteInstancesCheckboxImage.MouseLeftButtonUp += this.MuteInstancesCheckboxImage_MouseLeftButtonUp;
				return;
			case 17:
				this.mMuteAllInstancesTextBlock = (TextBlock)target;
				this.mMuteAllInstancesTextBlock.MouseLeftButtonUp += this.MuteInstancesCheckboxImage_MouseLeftButtonUp;
				return;
			case 18:
				this.mOverlayTooltip = (CustomPopUp)target;
				return;
			case 19:
				this.mMaskBorder4 = (Border)target;
				return;
			case 20:
				((CustomPictureBox)target).MouseLeftButtonUp += this.OverlayTooltipCPB_MouseLeftButtonUp;
				return;
			case 21:
				this.mOverlayPopUpTitle = (TextBlock)target;
				return;
			case 22:
				this.mOverlayPopUpMessage = (TextBlock)target;
				return;
			case 23:
				this.mOverlayDoNotShowCheckboxImage = (CustomPictureBox)target;
				this.mOverlayDoNotShowCheckboxImage.MouseLeftButtonUp += this.OverlayDoNotShowCheckbox_MouseLeftButtonUp;
				return;
			case 24:
				this.mOverlayDontShowPopUp = (TextBlock)target;
				this.mOverlayDontShowPopUp.MouseLeftButtonUp += this.OverlayDoNotShowCheckbox_MouseLeftButtonUp;
				return;
			case 25:
				this.mMacroButtonPopup = (CustomPopUp)target;
				return;
			case 26:
				this.mMaskBorder5 = (Border)target;
				return;
			case 27:
				this.mMacroBookmarkPopup = (MacroBookmarksPopup)target;
				return;
			case 28:
				this.mCustomiseSectionTag = (Grid)target;
				return;
			case 29:
				this.mCustomiseSectionBorderLine = (Separator)target;
				return;
			case 30:
				((Grid)target).PreviewMouseLeftButtonUp += this.OpenMacroGridPreviewMouseLeftButtonUp;
				((Grid)target).MouseEnter += this.OpenMacroGridMouseEnter;
				((Grid)target).MouseLeave += this.OpenMacroGridMouseLeave;
				return;
			case 31:
				this.mOpenMacroTextbox = (TextBlock)target;
				return;
			case 32:
				this.mGameControlButtonPopup = (CustomPopUp)target;
				return;
			case 33:
				this.mMaskBorder6 = (Border)target;
				return;
			case 34:
				this.mBookmarkedSchemesStackPanel = (StackPanel)target;
				return;
			case 35:
				((Grid)target).PreviewMouseLeftButtonUp += this.OpenGameControlPreviewMouseLeftButtonUp;
				((Grid)target).MouseEnter += this.OpenGameControlMouseEnter;
				((Grid)target).MouseLeave += this.OpenGameControlMouseLeave;
				return;
			case 36:
				this.mOpenGameControlTextbox = (TextBlock)target;
				return;
			case 37:
				this.mRecordScreenPopup = (CustomPopUp)target;
				return;
			case 38:
				this.mMaskBorder7 = (Border)target;
				return;
			case 39:
				this.mRecordScreenClose = (CustomPictureBox)target;
				this.mRecordScreenClose.IsVisibleChanged += this.RecordScreenClose_IsVisibleChanged;
				this.mRecordScreenClose.MouseLeftButtonUp += this.RecordScreenPopupClose_MouseLeftButtonUp;
				return;
			case 40:
				this.RecordScreenPopupHeader = (TextBlock)target;
				return;
			case 41:
				this.RecordScreenPopupBody = (TextBlock)target;
				return;
			case 42:
				this.RecordScreenPopupHyperlink = (TextBlock)target;
				return;
			case 43:
				((Hyperlink)target).Click += this.RecordScreenPopupHyperlink_Click;
				return;
			case 44:
				this.mRecorderClickLink = (TextBlock)target;
				return;
			case 45:
				this.mScreenshotPopup = (CustomPopUp)target;
				return;
			case 46:
				this.mMaskBorder8 = (Border)target;
				return;
			case 47:
				this.mScreenshotClose = (CustomPictureBox)target;
				this.mScreenshotClose.MouseLeftButtonUp += this.ScreenshotPopupClose_MouseLeftButtonUp;
				return;
			case 48:
				this.ScreenshotPopupHeader = (TextBlock)target;
				return;
			case 49:
				this.ScreenshotPopupHyperlink = (TextBlock)target;
				return;
			case 50:
				((Hyperlink)target).Click += this.ScreenshotPopupHyperlink_Click;
				return;
			case 51:
				this.mScreenshotClickLink = (TextBlock)target;
				return;
			case 52:
				this.mGameControlsBlurbPopup = (CustomPopUp)target;
				return;
			case 53:
				this.mMaskBorder10 = (Border)target;
				return;
			case 54:
				this.ContentGrid = (Grid)target;
				return;
			case 55:
				this.headerTextBlock = (TextBlock)target;
				return;
			case 56:
				this.bodyTextBlock = (TextBlock)target;
				return;
			case 57:
				this.OkayButton = (CustomButton)target;
				this.OkayButton.Click += this.OnBoardingPopupNext_Click;
				return;
			case 58:
				this.RightArrow = (global::System.Windows.Shapes.Path)target;
				return;
			case 59:
				this.mMoreElements = (CustomPopUp)target;
				return;
			case 60:
				this.mPopupGrid = (Grid)target;
				return;
			case 61:
				this.mMaskBorder = (Border)target;
				return;
			case 62:
				this.mSidebarPopup = (SidebarPopup)target;
				return;
			case 63:
				this.mBottomGrid = (Grid)target;
				return;
			case 64:
				this.mStaticButtonsStackPanel = (StackPanel)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040004F5 RID: 1269
		private Dictionary<SidebarElement, EventHandler> mDictActions = new Dictionary<SidebarElement, EventHandler>();

		// Token: 0x040004F6 RID: 1270
		internal List<SidebarElement> mListSidebarElements = new List<SidebarElement>();

		// Token: 0x040004F7 RID: 1271
		private int mTotalVisibleElements;

		// Token: 0x040004F8 RID: 1272
		private double mSidebarElementApproxHeight;

		// Token: 0x040004F9 RID: 1273
		internal bool mIsUIInPortraitModeBeforeChange;

		// Token: 0x040004FA RID: 1274
		internal bool mIsOverlayTooltipClosed;

		// Token: 0x040004FB RID: 1275
		private bool mIsPendingShowOverlayTooltip;

		// Token: 0x040004FC RID: 1276
		internal double mLastSliderValue;

		// Token: 0x040004FD RID: 1277
		private bool mIsLoadedOnce;

		// Token: 0x040004FE RID: 1278
		private bool mIsInFullscreenMode;

		// Token: 0x040004FF RID: 1279
		private DispatcherTimer mMacroBookmarkTimer;

		// Token: 0x04000500 RID: 1280
		private DispatcherTimer mGameControlBookmarkTimer;

		// Token: 0x04000501 RID: 1281
		private DispatcherTimer mChangeTransparencyPopupTimer;

		// Token: 0x04000502 RID: 1282
		internal DispatcherTimer mVolumeSliderPopupTimer;

		// Token: 0x04000503 RID: 1283
		private DispatcherTimer mSidebarPopupTimer;

		// Token: 0x04000504 RID: 1284
		private string currentScreenshotSavedPath;

		// Token: 0x04000505 RID: 1285
		private readonly object mSyncRoot = new object();

		// Token: 0x04000506 RID: 1286
		private MainWindow mMainWindow;

		// Token: 0x04000507 RID: 1287
		private bool mIsOneSidebarElementLoadedBinded;

		// Token: 0x04000508 RID: 1288
		internal List<CustomPopUp> mListPopups;

		// Token: 0x04000509 RID: 1289
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Sidebar mSidebar;

		// Token: 0x0400050A RID: 1290
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBorder;

		// Token: 0x0400050B RID: 1291
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x0400050C RID: 1292
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTopGrid;

		// Token: 0x0400050D RID: 1293
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mElementsStackPanel;

		// Token: 0x0400050E RID: 1294
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SidebarElement mMoreButton;

		// Token: 0x0400050F RID: 1295
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mChangeTransparencyPopup;

		// Token: 0x04000510 RID: 1296
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder2;

		// Token: 0x04000511 RID: 1297
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTranslucentControlsSliderButton;

		// Token: 0x04000512 RID: 1298
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider transSlider;

		// Token: 0x04000513 RID: 1299
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mVolumeSliderPopup;

		// Token: 0x04000514 RID: 1300
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder3;

		// Token: 0x04000515 RID: 1301
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mVolumeMuteUnmuteImage;

		// Token: 0x04000516 RID: 1302
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider mVolumeSlider;

		// Token: 0x04000517 RID: 1303
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mCurrentVolumeValue;

		// Token: 0x04000518 RID: 1304
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMuteInstancesCheckboxImage;

		// Token: 0x04000519 RID: 1305
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMuteAllInstancesTextBlock;

		// Token: 0x0400051A RID: 1306
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mOverlayTooltip;

		// Token: 0x0400051B RID: 1307
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder4;

		// Token: 0x0400051C RID: 1308
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mOverlayPopUpTitle;

		// Token: 0x0400051D RID: 1309
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mOverlayPopUpMessage;

		// Token: 0x0400051E RID: 1310
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mOverlayDoNotShowCheckboxImage;

		// Token: 0x0400051F RID: 1311
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mOverlayDontShowPopUp;

		// Token: 0x04000520 RID: 1312
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mMacroButtonPopup;

		// Token: 0x04000521 RID: 1313
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder5;

		// Token: 0x04000522 RID: 1314
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal MacroBookmarksPopup mMacroBookmarkPopup;

		// Token: 0x04000523 RID: 1315
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mCustomiseSectionTag;

		// Token: 0x04000524 RID: 1316
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Separator mCustomiseSectionBorderLine;

		// Token: 0x04000525 RID: 1317
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mOpenMacroTextbox;

		// Token: 0x04000526 RID: 1318
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mGameControlButtonPopup;

		// Token: 0x04000527 RID: 1319
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder6;

		// Token: 0x04000528 RID: 1320
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mBookmarkedSchemesStackPanel;

		// Token: 0x04000529 RID: 1321
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mOpenGameControlTextbox;

		// Token: 0x0400052A RID: 1322
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mRecordScreenPopup;

		// Token: 0x0400052B RID: 1323
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder7;

		// Token: 0x0400052C RID: 1324
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mRecordScreenClose;

		// Token: 0x0400052D RID: 1325
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock RecordScreenPopupHeader;

		// Token: 0x0400052E RID: 1326
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock RecordScreenPopupBody;

		// Token: 0x0400052F RID: 1327
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock RecordScreenPopupHyperlink;

		// Token: 0x04000530 RID: 1328
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mRecorderClickLink;

		// Token: 0x04000531 RID: 1329
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mScreenshotPopup;

		// Token: 0x04000532 RID: 1330
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder8;

		// Token: 0x04000533 RID: 1331
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mScreenshotClose;

		// Token: 0x04000534 RID: 1332
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock ScreenshotPopupHeader;

		// Token: 0x04000535 RID: 1333
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock ScreenshotPopupHyperlink;

		// Token: 0x04000536 RID: 1334
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mScreenshotClickLink;

		// Token: 0x04000537 RID: 1335
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mGameControlsBlurbPopup;

		// Token: 0x04000538 RID: 1336
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder10;

		// Token: 0x04000539 RID: 1337
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid ContentGrid;

		// Token: 0x0400053A RID: 1338
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock headerTextBlock;

		// Token: 0x0400053B RID: 1339
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock bodyTextBlock;

		// Token: 0x0400053C RID: 1340
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton OkayButton;

		// Token: 0x0400053D RID: 1341
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path RightArrow;

		// Token: 0x0400053E RID: 1342
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mMoreElements;

		// Token: 0x0400053F RID: 1343
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mPopupGrid;

		// Token: 0x04000540 RID: 1344
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000541 RID: 1345
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SidebarPopup mSidebarPopup;

		// Token: 0x04000542 RID: 1346
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBottomGrid;

		// Token: 0x04000543 RID: 1347
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mStaticButtonsStackPanel;

		// Token: 0x04000544 RID: 1348
		private bool _contentLoaded;

		// Token: 0x020000DF RID: 223
		public class ControlWindow : Window
		{
			// Token: 0x06000963 RID: 2403
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern int SuspendThread(IntPtr hThread);

			// Token: 0x06000964 RID: 2404
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern int ResumeThread(IntPtr hThread);

			// Token: 0x06000965 RID: 2405
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);

			// Token: 0x06000966 RID: 2406
			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern bool CloseHandle(IntPtr hObject);

			// Token: 0x06000967 RID: 2407 RVA: 0x00035444 File Offset: 0x00033644
			public ControlWindow()
			{
				base.Title = "Domination";
				base.WindowStyle = WindowStyle.None;
				base.Width = 300.0;
				base.Height = 400.0;
				base.ResizeMode = ResizeMode.NoResize;
				base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				base.AllowsTransparency = true;
				base.Background = Brushes.Transparent;
				base.MouseLeftButtonDown += delegate(object s, MouseButtonEventArgs e)
				{
					base.Opacity = 0.7;
					base.DragMove();
				};
				base.MouseLeftButtonUp += delegate(object s, MouseButtonEventArgs e)
				{
					base.Opacity = 1.0;
				};
				Border border = new Border
				{
					CornerRadius = new CornerRadius(15.0),
					Background = new LinearGradientBrush
					{
						StartPoint = new Point(0.0, 0.0),
						EndPoint = new Point(0.0, 1.0),
						GradientStops = new GradientStopCollection
						{
							new GradientStop(Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.0),
							new GradientStop(Color.FromArgb(byte.MaxValue, 50, 50, 50), 1.0)
						}
					}
				};
				Grid grid = new Grid();
				border.Child = grid;
				Button button = new Button
				{
					Content = "✕",
					Width = 30.0,
					Height = 30.0,
					HorizontalAlignment = HorizontalAlignment.Right,
					VerticalAlignment = VerticalAlignment.Top,
					Margin = new Thickness(0.0, 10.0, 10.0, 0.0),
					FontFamily = new FontFamily("Segoe UI"),
					FontSize = 14.0,
					FontWeight = FontWeights.Bold,
					Foreground = Brushes.White,
					BorderThickness = new Thickness(0.0),
					Background = new LinearGradientBrush
					{
						StartPoint = new Point(0.0, 0.0),
						EndPoint = new Point(0.0, 1.0),
						GradientStops = new GradientStopCollection
						{
							new GradientStop(Color.FromRgb(50, 50, 50), 0.0),
							new GradientStop(Color.FromRgb(80, 80, 80), 1.0)
						}
					}
				};
				button.Style = this.CreateButtonStyle(true);
				button.Click += delegate(object s, RoutedEventArgs e)
				{
					base.Close();
				};
				grid.Children.Add(button);
				StackPanel stackPanel = new StackPanel
				{
					Margin = new Thickness(20.0)
				};
				grid.Children.Add(stackPanel);
				TextBlock textBlock = new TextBlock
				{
					Text = "TG: @De_MISTIYT",
					FontFamily = new FontFamily("Segoe UI"),
					FontSize = 24.0,
					FontWeight = FontWeights.Bold,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(0.0, 0.0, 0.0, 20.0),
					Foreground = new LinearGradientBrush
					{
						StartPoint = new Point(0.0, 0.0),
						EndPoint = new Point(1.0, 0.0),
						GradientStops = new GradientStopCollection
						{
							new GradientStop(Color.FromRgb(200, 200, 200), 0.0),
							new GradientStop(Colors.White, 1.0)
						}
					}
				};
				stackPanel.Children.Add(textBlock);
				stackPanel.Children.Add(this.CreateStyledButton("Додж", new RoutedEventHandler(this.ProcessFreeze)));
				stackPanel.Children.Add(this.CreateStyledButton("GO TO GAME", new RoutedEventHandler(this.ProcessResume)));
				stackPanel.Children.Add(this.CreateStyledButton("BAD GAME", new RoutedEventHandler(this.ProcessKill)));
				stackPanel.Children.Add(this.CreateStyledButton("CLOSE", delegate(object s, RoutedEventArgs e)
				{
					base.Close();
				}));
				TextBlock textBlock2 = new TextBlock();
				Hyperlink hyperlink = new Hyperlink
				{
					NavigateUri = new Uri("https://t.me/de_mistiyt"),
					Foreground = new LinearGradientBrush
					{
						StartPoint = new Point(0.0, 0.0),
						EndPoint = new Point(1.0, 0.0),
						GradientStops = new GradientStopCollection
						{
							new GradientStop(Color.FromRgb(200, 200, 200), 0.0),
							new GradientStop(Colors.White, 1.0)
						}
					}
				};
				hyperlink.Inlines.Add("@de_mistiyt");
				hyperlink.RequestNavigate += delegate(object s, RequestNavigateEventArgs e)
				{
					Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
					{
						UseShellExecute = true
					});
				};
				hyperlink.TextDecorations = null;
				hyperlink.FocusVisualStyle = null;
				textBlock2.Inlines.Add(hyperlink);
				StackPanel stackPanel2 = new StackPanel
				{
					Margin = new Thickness(10.0),
					HorizontalAlignment = HorizontalAlignment.Center
				};
				stackPanel2.Children.Add(textBlock2);
				stackPanel.Children.Add(stackPanel2);
				base.Content = border;
			}

			// Token: 0x06000968 RID: 2408 RVA: 0x00035A08 File Offset: 0x00033C08
			private Button CreateButton(string content, Color color, RoutedEventHandler clickHandler)
			{
				Button button = new Button
				{
					Content = content,
					Width = 200.0,
					Height = 50.0,
					Margin = new Thickness(10.0),
					Background = new SolidColorBrush(color),
					Foreground = Brushes.White,
					FontSize = 16.0,
					FontWeight = FontWeights.SemiBold,
					BorderThickness = new Thickness(0.0),
					Cursor = Cursors.Hand,
					Effect = new DropShadowEffect
					{
						Color = Colors.Black,
						BlurRadius = 5.0,
						ShadowDepth = 1.0
					},
					Opacity = 1.0
				};
				Style style = new Style(typeof(Button));
				style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(color)));
				style.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.White));
				style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0.0)));
				style.Setters.Add(new Setter(UIElement.EffectProperty, new DropShadowEffect
				{
					Color = Colors.Black,
					BlurRadius = 5.0,
					ShadowDepth = 1.0
				}));
				button.Style = style;
				button.MouseEnter += delegate(object s, MouseEventArgs e)
				{
					this.AnimateButton(button, 0.8, 0.95);
				};
				button.MouseLeave += delegate(object s, MouseEventArgs e)
				{
					this.AnimateButton(button, 1.0, 1.0);
				};
				button.PreviewMouseDown += delegate(object s, MouseButtonEventArgs e)
				{
					this.AnimateButton(button, 0.7, 0.9);
				};
				button.PreviewMouseUp += delegate(object s, MouseButtonEventArgs e)
				{
					this.AnimateButton(button, 0.8, 0.95);
				};
				button.Click += clickHandler;
				return button;
			}

			// Token: 0x06000969 RID: 2409 RVA: 0x00035C24 File Offset: 0x00033E24
			private void AnimateButton(Button button, double opacity, double scale)
			{
				base.Dispatcher.BeginInvoke(new Action(delegate
				{
					DoubleAnimation doubleAnimation = new DoubleAnimation(opacity, TimeSpan.FromMilliseconds(100.0));
					ScaleTransform scaleTransform = new ScaleTransform(1.0, 1.0);
					button.RenderTransform = scaleTransform;
					DoubleAnimation doubleAnimation2 = new DoubleAnimation(scale, TimeSpan.FromMilliseconds(100.0));
					button.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
					scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation2);
					scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation2);
				}), DispatcherPriority.Render, Array.Empty<object>());
			}

			// Token: 0x0600096A RID: 2410 RVA: 0x00007F74 File Offset: 0x00006174
			private void ProcessFreeze(object sender, RoutedEventArgs e)
			{
				this.ProcessAction("HD-Player", true);
			}

			// Token: 0x0600096B RID: 2411 RVA: 0x00007F82 File Offset: 0x00006182
			private void ProcessResume(object sender, RoutedEventArgs e)
			{
				this.ProcessAction("HD-Player", false);
			}

			// Token: 0x0600096C RID: 2412 RVA: 0x00035C6C File Offset: 0x00033E6C
			private void ProcessKill(object sender, RoutedEventArgs e)
			{
				this.ProcessAction("HD-Player", false);
				foreach (Process process in Process.GetProcessesByName("HD-Player"))
				{
					try
					{
						process.Kill();
					}
					catch (Exception ex)
					{
						MessageBox.Show("Ошибка при завершении процесса: " + ex.Message);
					}
				}
			}

			// Token: 0x0600096D RID: 2413 RVA: 0x00035CD4 File Offset: 0x00033ED4
			private void ProcessAction(string processName, bool freeze)
			{
				Process[] processesByName = Process.GetProcessesByName(processName);
				for (int i = 0; i < processesByName.Length; i++)
				{
					foreach (object obj in processesByName[i].Threads)
					{
						ProcessThread processThread = (ProcessThread)obj;
						IntPtr intPtr = IntPtr.Zero;
						try
						{
							intPtr = Sidebar.ControlWindow.OpenThread(2, false, processThread.Id);
							if (intPtr != IntPtr.Zero)
							{
								if (freeze)
								{
									Sidebar.ControlWindow.SuspendThread(intPtr);
								}
								else
								{
									Sidebar.ControlWindow.ResumeThread(intPtr);
								}
							}
						}
						catch (Exception ex)
						{
							MessageBox.Show("Ошибка при работе с потоком: " + ex.Message);
						}
						finally
						{
							if (intPtr != IntPtr.Zero)
							{
								Sidebar.ControlWindow.CloseHandle(intPtr);
							}
						}
					}
				}
			}

			// Token: 0x06000970 RID: 2416 RVA: 0x00035DD0 File Offset: 0x00033FD0
			private Button CreateStyledButton(string content, RoutedEventHandler clickHandler)
			{
				Button button = new Button();
				button.Content = content;
				button.Width = 200.0;
				button.Height = 35.0;
				button.Margin = new Thickness(10.0);
				button.FontFamily = new FontFamily("Segoe UI");
				button.FontSize = 12.0;
				button.FontWeight = FontWeights.Bold;
				button.Foreground = Brushes.White;
				button.BorderThickness = new Thickness(0.0);
				button.Background = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(Color.FromRgb(50, 50, 50), 0.0),
						new GradientStop(Color.FromRgb(80, 80, 80), 1.0)
					}
				};
				button.Style = this.CreateButtonStyle(false);
				button.Click += clickHandler;
				return button;
			}

			// Token: 0x06000971 RID: 2417 RVA: 0x00035F08 File Offset: 0x00034108
			private Style CreateButtonStyle(bool isCloseButton)
			{
				Style style = new Style(typeof(Button));
				style.Setters.Add(new Setter(Control.BackgroundProperty, new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(Color.FromRgb(50, 50, 50), 0.0),
						new GradientStop(Color.FromRgb(80, 80, 80), 1.0)
					}
				}));
				style.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.White));
				style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0.0)));
				style.Setters.Add(new Setter(Control.TemplateProperty, this.CreateButtonTemplate(isCloseButton)));
				Trigger trigger = new Trigger
				{
					Property = UIElement.IsMouseOverProperty,
					Value = true
				};
				trigger.Setters.Add(new Setter(Control.BackgroundProperty, new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(Color.FromRgb(80, 80, 80), 0.0),
						new GradientStop(Color.FromRgb(100, 100, 100), 1.0)
					}
				}));
				style.Triggers.Add(trigger);
				return style;
			}

			// Token: 0x06000972 RID: 2418 RVA: 0x000360E8 File Offset: 0x000342E8
			private ControlTemplate CreateButtonTemplate(bool isCloseButton)
			{
				ControlTemplate controlTemplate = new ControlTemplate(typeof(Button));
				FrameworkElementFactory frameworkElementFactory = new FrameworkElementFactory(typeof(Border));
				frameworkElementFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius((double)(isCloseButton ? 6 : 9)));
				frameworkElementFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Control.BackgroundProperty));
				frameworkElementFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0.0));
				FrameworkElementFactory frameworkElementFactory2 = new FrameworkElementFactory(typeof(ContentPresenter));
				frameworkElementFactory2.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
				frameworkElementFactory2.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
				frameworkElementFactory.AppendChild(frameworkElementFactory2);
				controlTemplate.VisualTree = frameworkElementFactory;
				return controlTemplate;
			}
		}
	}
}
