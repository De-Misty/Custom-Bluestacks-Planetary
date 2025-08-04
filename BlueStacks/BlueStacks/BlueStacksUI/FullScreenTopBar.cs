using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200026D RID: 621
	public class FullScreenTopBar : UserControl, IComponentConnector
	{
		// Token: 0x0600169A RID: 5786 RVA: 0x0000F2FA File Offset: 0x0000D4FA
		public FullScreenTopBar()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x00086EBC File Offset: 0x000850BC
		internal void Init(MainWindow window)
		{
			this.ParentWindow = window;
			if (!DesignerProperties.GetIsInDesignMode(this) && !RegistryManager.Instance.UseEscapeToExitFullScreen)
			{
				this.mEscCheckbox.ImageName = "checkbox_new";
			}
			this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
			if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
			{
				this.mKeyMapSwitchFullScreen.Visibility = Visibility.Collapsed;
				this.mKeyMapButtonFullScreen.Visibility = Visibility.Collapsed;
				this.mLocationButtonFullScreen.Visibility = Visibility.Collapsed;
				this.mShakeButtonFullScreen.Visibility = Visibility.Collapsed;
				this.mGamePadButtonFullScreen.Visibility = Visibility.Collapsed;
				this.mTranslucentControlsButtonFullScreen.Visibility = Visibility.Collapsed;
			}
			this.mMacroRecorderFullScreen.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x0000F308 File Offset: 0x0000D508
		private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.BackButtonHandler(false);
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x0000F31B File Offset: 0x0000D51B
		private void HomeButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.HomeButtonHandler(true, false);
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x00086F6C File Offset: 0x0008516C
		private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("SwitchKeyMapClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "fullscreentopbar", null, null, null, null, null);
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x0000F32F File Offset: 0x0000D52F
		private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBarPopup.IsOpen = false;
			this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "fullscreentopbar");
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x0000F35C File Offset: 0x0000D55C
		private void FullScreenButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.FullScreenButtonHandler("fullScreenTopbar", "MouseClick");
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0000F378 File Offset: 0x0000D578
		private void LocationButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.LocationButtonHandler();
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x0000F38A File Offset: 0x0000D58A
		private void ScreenShotButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBarPopup.IsOpen = false;
			this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x0000F3AD File Offset: 0x0000D5AD
		private void ShakeButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.ShakeButtonHandler();
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x00086FA4 File Offset: 0x000851A4
		private void mEscCheckbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (RegistryManager.Instance.UseEscapeToExitFullScreen)
			{
				this.mEscCheckbox.ImageName = "checkbox_new";
				RegistryManager.Instance.UseEscapeToExitFullScreen = false;
				return;
			}
			this.mEscCheckbox.ImageName = "checkbox_new_checked";
			RegistryManager.Instance.UseEscapeToExitFullScreen = true;
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x0000F3BF File Offset: 0x0000D5BF
		private void mMacroRecorderLandscape_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x0000F3D1 File Offset: 0x0000D5D1
		private void GamePadButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBarPopup.IsOpen = false;
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x00086FF4 File Offset: 0x000851F4
		private void TranslucentControlsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryManager.Instance.ShowKeyControlsOverlay = true;
			RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
			KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
			this.mChangeTransparencyPopup.PlacementTarget = this.mTranslucentControlsButtonFullScreen;
			this.mChangeTransparencyPopup.IsOpen = true;
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x00087044 File Offset: 0x00085244
		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			KMManager.ChangeTransparency(this.ParentWindow, this.transSlider.Value);
			if (this.transSlider.Value == 0.0)
			{
				if (!RegistryManager.Instance.ShowKeyControlsOverlay)
				{
					KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
				}
				this.ParentWindow.mCommonHandler.OnOverlayStateChanged(false);
			}
			else
			{
				KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
				this.ParentWindow.mCommonHandler.OnOverlayStateChanged(true);
			}
			this.lastSliderValue = this.transSlider.Value;
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x000870D8 File Offset: 0x000852D8
		private void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.transSlider.Value == 0.0)
			{
				this.transSlider.Value = this.lastSliderValue;
				return;
			}
			double value = this.transSlider.Value;
			this.transSlider.Value = 0.0;
			this.lastSliderValue = value;
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x0000F3E4 File Offset: 0x0000D5E4
		private void mChangeTransparencyPopup_Closed(object sender, EventArgs e)
		{
			if (!this.ParentWindow.mFullScreenTopBar.IsMouseOver)
			{
				this.ParentWindow.mTopBarPopup.IsOpen = false;
			}
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x00087134 File Offset: 0x00085334
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/fullscreentopbar.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x00087164 File Offset: 0x00085364
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
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.BackButton_PreviewMouseLeftButtonUp;
				return;
			case 2:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.HomeButton_PreviewMouseLeftButtonUp;
				return;
			case 3:
				this.mEscCheckbox = (CustomPictureBox)target;
				this.mEscCheckbox.MouseLeftButtonUp += this.mEscCheckbox_MouseLeftButtonUp;
				return;
			case 4:
				this.mGamePadButtonFullScreen = (CustomPictureBox)target;
				this.mGamePadButtonFullScreen.PreviewMouseLeftButtonUp += this.GamePadButton_PreviewMouseLeftButtonUp;
				return;
			case 5:
				this.mMacroRecorderFullScreen = (CustomPictureBox)target;
				this.mMacroRecorderFullScreen.PreviewMouseLeftButtonUp += this.mMacroRecorderLandscape_PreviewMouseLeftButtonUp;
				return;
			case 6:
				this.mKeyMapSwitchFullScreen = (CustomPictureBox)target;
				this.mKeyMapSwitchFullScreen.PreviewMouseLeftButtonUp += this.SwitchKeyMapButton_PreviewMouseLeftButtonUp;
				return;
			case 7:
				this.mKeyMapButtonFullScreen = (CustomPictureBox)target;
				this.mKeyMapButtonFullScreen.PreviewMouseLeftButtonUp += this.KeyMapButton_PreviewMouseLeftButtonUp;
				return;
			case 8:
				this.mTranslucentControlsButtonFullScreen = (CustomPictureBox)target;
				this.mTranslucentControlsButtonFullScreen.PreviewMouseLeftButtonUp += this.TranslucentControlsButton_PreviewMouseLeftButtonUp;
				return;
			case 9:
				this.mFullScreenButton = (CustomPictureBox)target;
				this.mFullScreenButton.PreviewMouseLeftButtonUp += this.FullScreenButton_PreviewMouseLeftButtonUp;
				return;
			case 10:
				this.mLocationButtonFullScreen = (CustomPictureBox)target;
				this.mLocationButtonFullScreen.PreviewMouseLeftButtonUp += this.LocationButton_PreviewMouseLeftButtonUp;
				return;
			case 11:
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.ScreenShotButton_PreviewMouseLeftButtonUp;
				return;
			case 12:
				this.mShakeButtonFullScreen = (CustomPictureBox)target;
				this.mShakeButtonFullScreen.PreviewMouseLeftButtonUp += this.ShakeButton_PreviewMouseLeftButtonUp;
				return;
			case 13:
				this.mChangeTransparencyPopup = (CustomPopUp)target;
				return;
			case 14:
				this.borderSlider = (Border)target;
				return;
			case 15:
				this.mTranslucentControlsSliderButton = (CustomPictureBox)target;
				this.mTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp;
				return;
			case 16:
				this.transSlider = (Slider)target;
				this.transSlider.ValueChanged += this.Slider_ValueChanged;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000DCE RID: 3534
		private MainWindow ParentWindow;

		// Token: 0x04000DCF RID: 3535
		private double lastSliderValue;

		// Token: 0x04000DD0 RID: 3536
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mEscCheckbox;

		// Token: 0x04000DD1 RID: 3537
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mGamePadButtonFullScreen;

		// Token: 0x04000DD2 RID: 3538
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMacroRecorderFullScreen;

		// Token: 0x04000DD3 RID: 3539
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mKeyMapSwitchFullScreen;

		// Token: 0x04000DD4 RID: 3540
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mKeyMapButtonFullScreen;

		// Token: 0x04000DD5 RID: 3541
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTranslucentControlsButtonFullScreen;

		// Token: 0x04000DD6 RID: 3542
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mFullScreenButton;

		// Token: 0x04000DD7 RID: 3543
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mLocationButtonFullScreen;

		// Token: 0x04000DD8 RID: 3544
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mShakeButtonFullScreen;

		// Token: 0x04000DD9 RID: 3545
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mChangeTransparencyPopup;

		// Token: 0x04000DDA RID: 3546
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border borderSlider;

		// Token: 0x04000DDB RID: 3547
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTranslucentControlsSliderButton;

		// Token: 0x04000DDC RID: 3548
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider transSlider;

		// Token: 0x04000DDD RID: 3549
		private bool _contentLoaded;
	}
}
