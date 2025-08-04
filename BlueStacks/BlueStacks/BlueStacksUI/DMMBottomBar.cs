using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000C8 RID: 200
	public class DMMBottomBar : UserControl, IComponentConnector
	{
		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000822 RID: 2082 RVA: 0x00007359 File Offset: 0x00005559
		// (set) Token: 0x06000823 RID: 2083 RVA: 0x0002DBF4 File Offset: 0x0002BDF4
		internal double CurrentTransparency
		{
			get
			{
				return DMMBottomBar.sCurrentTransparency;
			}
			set
			{
				DMMBottomBar.sCurrentTransparency = value;
				this.transSlider.ValueChanged -= this.TransparencySlider_ValueChanged;
				if (this.ParentWindow.mDMMFST != null)
				{
					this.ParentWindow.mDMMFST.transSlider.ValueChanged -= this.ParentWindow.mDMMFST.TransparencySlider_ValueChanged;
				}
				this.transSlider.Value = DMMBottomBar.sCurrentTransparency;
				if (this.ParentWindow.mDMMFST != null)
				{
					this.ParentWindow.mDMMFST.transSlider.Value = DMMBottomBar.sCurrentTransparency;
				}
				this.transSlider.ValueChanged += this.TransparencySlider_ValueChanged;
				if (this.ParentWindow.mDMMFST != null)
				{
					this.ParentWindow.mDMMFST.transSlider.ValueChanged += this.ParentWindow.mDMMFST.TransparencySlider_ValueChanged;
				}
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000824 RID: 2084 RVA: 0x00007360 File Offset: 0x00005560
		// (set) Token: 0x06000825 RID: 2085 RVA: 0x0002DCDC File Offset: 0x0002BEDC
		internal int CurrentVolume
		{
			get
			{
				return DMMBottomBar.sCurrentVolume;
			}
			set
			{
				DMMBottomBar.sCurrentVolume = value;
				this.mVolumeSlider.Value = (double)DMMBottomBar.sCurrentVolume;
				if (this.ParentWindow.mDMMFST != null && this.ParentWindow.mDMMFST.mVolumeSlider != null)
				{
					this.ParentWindow.mDMMFST.mVolumeSlider.Value = (double)DMMBottomBar.sCurrentVolume;
				}
				if (DMMBottomBar.sCurrentVolume < 1)
				{
					this.VolumeImageName = "volume_mute";
					return;
				}
				if (DMMBottomBar.sCurrentVolume <= 50)
				{
					this.VolumeImageName = "volume_small";
					return;
				}
				this.VolumeImageName = "volume_large";
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x00007367 File Offset: 0x00005567
		// (set) Token: 0x06000827 RID: 2087 RVA: 0x00007379 File Offset: 0x00005579
		public string VolumeImageName
		{
			get
			{
				return (string)base.GetValue(DMMBottomBar.VolumeImageNameProperty);
			}
			set
			{
				base.SetValue(DMMBottomBar.VolumeImageNameProperty, value);
			}
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0002DD70 File Offset: 0x0002BF70
		private static void OnVolumeImageNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as DMMBottomBar).ParentWindow.mDmmBottomBar.mVolumeBtn.ImageName = e.NewValue.ToString();
			(d as DMMBottomBar).ParentWindow.mDmmBottomBar.volumesSliderImage.ImageName = e.NewValue.ToString();
			(d as DMMBottomBar).ParentWindow.mDMMFST.mVolumeBtn.ImageName = e.NewValue.ToString();
			(d as DMMBottomBar).ParentWindow.mDMMFST.volumeSliderImage.ImageName = e.NewValue.ToString();
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x00007387 File Offset: 0x00005587
		public DMMBottomBar()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0002DE18 File Offset: 0x0002C018
		public void Init(MainWindow window)
		{
			this.ParentWindow = window;
			this.VolumeImageName = "volume_small";
			this.mVolumeBtn.ImageName = "volume_small";
			this.CurrentTransparency = (DMMBottomBar.sPreviousTransparency = RegistryManager.Instance.TranslucentControlsTransparency);
			if (this.ParentWindow != null)
			{
				this.ParentWindow.mCommonHandler.VolumeChangedEvent += this.DMMBottomBar_VolumeChangedEvent;
				this.ParentWindow.mCommonHandler.VolumeMutedEvent += this.DMMBottomBar_VolumeMutedEvent;
			}
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x00007395 File Offset: 0x00005595
		private void DMMBottomBar_VolumeMutedEvent(bool muted)
		{
			if (muted)
			{
				this.VolumeImageName = "volume_mute";
				return;
			}
			this.CurrentVolume = (int)this.mVolumeSlider.Value;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0002DEA0 File Offset: 0x0002C0A0
		private void DMMBottomBar_VolumeChangedEvent(int volumeLevel)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.CurrentVolume = volumeLevel;
			}), new object[0]);
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x000073B8 File Offset: 0x000055B8
		internal void Tab_Changed(object sender, EventArgs e)
		{
			this.ParentWindow.mCommonHandler.SetDMMKeymapButtonsAndTransparency();
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x000073CA File Offset: 0x000055CA
		private void FullScreenBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.FullScreenButtonHandler("bottombarDmm", "MouseClick");
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x000073E6 File Offset: 0x000055E6
		private void ScreenshotBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x000073F8 File Offset: 0x000055F8
		private void VolumeBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.mVolumePopup.IsOpen = !this.mVolumePopup.IsOpen;
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x00007413 File Offset: 0x00005613
		private void SettingsBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0002DEE0 File Offset: 0x0002C0E0
		private void RecommendedWindowBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow.WindowState == WindowState.Normal)
			{
				if (this.ParentWindow.mDMMRecommendedWindow == null)
				{
					this.ParentWindow.mDMMRecommendedWindow = new DMMRecommendedWindow(this.ParentWindow);
					this.ParentWindow.mDMMRecommendedWindow.Init(RegistryManager.Instance.DMMRecommendedWindowUrl);
				}
				if (this.ParentWindow.mDMMRecommendedWindow.Visibility != Visibility.Visible)
				{
					this.ParentWindow.mDMMRecommendedWindow.Visibility = Visibility.Visible;
					this.ParentWindow.mIsDMMRecommendedWindowOpen = true;
				}
				else
				{
					this.ParentWindow.mDMMRecommendedWindow.Visibility = Visibility.Hidden;
					this.ParentWindow.mIsDMMRecommendedWindowOpen = false;
				}
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					Thread.Sleep(500);
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						this.ParentWindow.Activate();
					}), new object[0]);
				});
			}
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0002DF9C File Offset: 0x0002C19C
		private void DoNotPromptManageGP_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (string.Equals(this.mDoNotPromptChkBx.ImageName, "bgpcheckbox", StringComparison.InvariantCulture))
			{
				this.mDoNotPromptChkBx.ImageName = "bgpcheckbox_checked";
				RegistryManager.Instance.KeyMappingAvailablePromptEnabled = false;
			}
			else
			{
				this.mDoNotPromptChkBx.ImageName = "bgpcheckbox";
				RegistryManager.Instance.KeyMappingAvailablePromptEnabled = true;
			}
			e.Handled = true;
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0000742A File Offset: 0x0000562A
		private void ClosePopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mKeyMapPopup.IsOpen = false;
			e.Handled = true;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0000743F File Offset: 0x0000563F
		private void KeyMapPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mKeyMapPopup.IsOpen = false;
			this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "bottombarpopup");
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x00007467 File Offset: 0x00005667
		private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.DMMSwitchKeyMapButtonHandler();
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0002E000 File Offset: 0x0002C200
		private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName != null)
			{
				this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "bottombar");
			}
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x00007479 File Offset: 0x00005679
		private void TranslucentControlsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
			this.mChangeTransparencyPopup.PlacementTarget = this.mTranslucentControlsButton;
			this.mChangeTransparencyPopup.IsOpen = true;
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x000074AD File Offset: 0x000056AD
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				this.UpdateLayoutAndBounds();
			}
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x000074BD File Offset: 0x000056BD
		internal void UpdateLayoutAndBounds()
		{
			if (this.mKeyMapPopup.IsOpen)
			{
				this.ShowKeyMapPopup(true);
			}
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0002E05C File Offset: 0x0002C25C
		internal void ShowKeyMapPopup(bool isShow)
		{
			if (isShow)
			{
				new Thread(delegate
				{
					Thread.Sleep(500);
					base.Dispatcher.Invoke(new Action(delegate
					{
						this.mKeyMapPopup.IsOpen = false;
						this.mKeyMapPopup.PlacementTarget = this.mKeyMapButton;
						if (!Array.Exists<string>(RegistryManager.Instance.DisabledGuidancePackages, (string element) => element == this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName) && RegistryManager.Instance.IsAutoShowGuidance && !this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mIsKeyMappingTipDisplayed)
						{
							this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mIsKeyMappingTipDisplayed = true;
							KMManager.HandleInputMapperWindow(this.ParentWindow, "");
							return;
						}
						if (RegistryManager.Instance.KeyMappingAvailablePromptEnabled)
						{
							this.mKeyMapPopup.IsOpen = true;
						}
					}), new object[0]);
				})
				{
					IsBackground = true
				}.Start();
				return;
			}
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mKeyMapPopup.IsOpen = false;
			}), new object[0]);
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x000074D3 File Offset: 0x000056D3
		internal void TransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			this.CurrentTransparency = e.NewValue;
			DMMBottomBar.sPreviousTransparency = e.NewValue;
			this.ChangeTransparency();
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0002E0A8 File Offset: 0x0002C2A8
		private void ChangeTransparency()
		{
			KMManager.ChangeTransparency(this.ParentWindow, this.CurrentTransparency);
			if (this.CurrentTransparency == 0.0)
			{
				KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
				this.ParentWindow.mCommonHandler.SetTranslucentControlsBtnImageForDMM("eye_off");
				return;
			}
			KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
			this.ParentWindow.mCommonHandler.SetTranslucentControlsBtnImageForDMM("eye");
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0002E11C File Offset: 0x0002C31C
		internal void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Slider slider = (Slider)sender;
			if (this.ParentWindow != null)
			{
				this.ParentWindow.Utils.SetVolumeInFrontendAsync((int)slider.Value);
			}
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x000074F2 File Offset: 0x000056F2
		internal void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.CurrentTransparency = ((this.CurrentTransparency == 0.0) ? DMMBottomBar.sPreviousTransparency : 0.0);
			this.ChangeTransparency();
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x00007521 File Offset: 0x00005721
		internal void VolumeSliderImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow != null)
			{
				if (this.ParentWindow.EngineInstanceRegistry.IsMuted)
				{
					this.ParentWindow.Utils.UnmuteApplication(false);
					return;
				}
				this.ParentWindow.Utils.MuteApplication(false);
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0002E150 File Offset: 0x0002C350
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dmmbottombar.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0002E180 File Offset: 0x0002C380
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
				((DMMBottomBar)target).SizeChanged += this.UserControl_SizeChanged;
				return;
			case 2:
				this.DMMBottomGrid = (Grid)target;
				return;
			case 3:
				this.mKeyMapSwitch = (CustomPictureBox)target;
				this.mKeyMapSwitch.PreviewMouseLeftButtonUp += this.SwitchKeyMapButton_PreviewMouseLeftButtonUp;
				return;
			case 4:
				this.mKeyMapButton = (CustomPictureBox)target;
				this.mKeyMapButton.PreviewMouseLeftButtonUp += this.KeyMapButton_PreviewMouseLeftButtonUp;
				return;
			case 5:
				this.mTranslucentControlsButton = (CustomPictureBox)target;
				this.mTranslucentControlsButton.PreviewMouseLeftButtonUp += this.TranslucentControlsButton_PreviewMouseLeftButtonUp;
				return;
			case 6:
				this.mScreenshotBtn = (CustomPictureBox)target;
				this.mScreenshotBtn.PreviewMouseLeftButtonUp += this.ScreenshotBtn_MouseUp;
				return;
			case 7:
				this.mVolumeBtn = (CustomPictureBox)target;
				this.mVolumeBtn.PreviewMouseLeftButtonUp += this.VolumeBtn_MouseUp;
				return;
			case 8:
				this.mFullscreenBtn = (CustomPictureBox)target;
				this.mFullscreenBtn.PreviewMouseLeftButtonUp += this.FullScreenBtn_MouseUp;
				return;
			case 9:
				this.mSettingsBtn = (CustomPictureBox)target;
				this.mSettingsBtn.PreviewMouseLeftButtonUp += this.SettingsBtn_MouseUp;
				return;
			case 10:
				this.mRecommendedWindowBtn = (CustomPictureBox)target;
				this.mRecommendedWindowBtn.PreviewMouseLeftButtonUp += this.RecommendedWindowBtn_PreviewMouseLeftButtonUp;
				return;
			case 11:
				this.mVolumePopup = (CustomPopUp)target;
				return;
			case 12:
				this.volumesSliderImage = (CustomPictureBox)target;
				this.volumesSliderImage.PreviewMouseLeftButtonUp += this.VolumeSliderImage_PreviewMouseLeftButtonUp;
				return;
			case 13:
				this.mVolumeSlider = (Slider)target;
				this.mVolumeSlider.PreviewMouseLeftButtonUp += this.VolumeSlider_PreviewMouseLeftButtonUp;
				return;
			case 14:
				this.mKeyMapPopup = (CustomPopUp)target;
				return;
			case 15:
				((Border)target).MouseLeftButtonUp += this.KeyMapPopup_PreviewMouseLeftButtonUp;
				return;
			case 16:
				((CustomPictureBox)target).MouseLeftButtonUp += this.ClosePopup_MouseLeftButtonUp;
				return;
			case 17:
				this.mKeyMappingPopUp1 = (TextBlock)target;
				return;
			case 18:
				this.mKeyMappingPopUp3 = (TextBlock)target;
				return;
			case 19:
				this.mDoNotPromptChkBx = (CustomPictureBox)target;
				this.mDoNotPromptChkBx.MouseLeftButtonUp += this.DoNotPromptManageGP_MouseLeftButtonUp;
				return;
			case 20:
				this.mKeyMappingDontShowPopUp = (TextBlock)target;
				this.mKeyMappingDontShowPopUp.MouseLeftButtonUp += this.DoNotPromptManageGP_MouseLeftButtonUp;
				return;
			case 21:
				this.DownArrow = (Path)target;
				return;
			case 22:
				this.mChangeTransparencyPopup = (CustomPopUp)target;
				return;
			case 23:
				this.mTranslucentControlsSliderButton = (CustomPictureBox)target;
				this.mTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp;
				return;
			case 24:
				this.transSlider = (Slider)target;
				this.transSlider.ValueChanged += this.TransparencySlider_ValueChanged;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000479 RID: 1145
		private MainWindow ParentWindow;

		// Token: 0x0400047A RID: 1146
		private static double sCurrentTransparency = 0.0;

		// Token: 0x0400047B RID: 1147
		private static double sPreviousTransparency = 0.0;

		// Token: 0x0400047C RID: 1148
		private static int sCurrentVolume = 33;

		// Token: 0x0400047D RID: 1149
		public static readonly DependencyProperty VolumeImageNameProperty = DependencyProperty.Register("VolumeImageName", typeof(string), typeof(DMMBottomBar), new FrameworkPropertyMetadata("volume_small", new PropertyChangedCallback(DMMBottomBar.OnVolumeImageNameChanged)));

		// Token: 0x0400047E RID: 1150
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid DMMBottomGrid;

		// Token: 0x0400047F RID: 1151
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mKeyMapSwitch;

		// Token: 0x04000480 RID: 1152
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mKeyMapButton;

		// Token: 0x04000481 RID: 1153
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTranslucentControlsButton;

		// Token: 0x04000482 RID: 1154
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mScreenshotBtn;

		// Token: 0x04000483 RID: 1155
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mVolumeBtn;

		// Token: 0x04000484 RID: 1156
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mFullscreenBtn;

		// Token: 0x04000485 RID: 1157
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSettingsBtn;

		// Token: 0x04000486 RID: 1158
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mRecommendedWindowBtn;

		// Token: 0x04000487 RID: 1159
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mVolumePopup;

		// Token: 0x04000488 RID: 1160
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox volumesSliderImage;

		// Token: 0x04000489 RID: 1161
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider mVolumeSlider;

		// Token: 0x0400048A RID: 1162
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mKeyMapPopup;

		// Token: 0x0400048B RID: 1163
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mKeyMappingPopUp1;

		// Token: 0x0400048C RID: 1164
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mKeyMappingPopUp3;

		// Token: 0x0400048D RID: 1165
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mDoNotPromptChkBx;

		// Token: 0x0400048E RID: 1166
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mKeyMappingDontShowPopUp;

		// Token: 0x0400048F RID: 1167
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path DownArrow;

		// Token: 0x04000490 RID: 1168
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mChangeTransparencyPopup;

		// Token: 0x04000491 RID: 1169
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTranslucentControlsSliderButton;

		// Token: 0x04000492 RID: 1170
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider transSlider;

		// Token: 0x04000493 RID: 1171
		private bool _contentLoaded;
	}
}
