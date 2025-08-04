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
	// Token: 0x020000E8 RID: 232
	public class DMMFullScreenTopBar : UserControl, IComponentConnector
	{
		// Token: 0x060009AF RID: 2479 RVA: 0x00008181 File Offset: 0x00006381
		public DMMFullScreenTopBar()
		{
			this.InitializeComponent();
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x0000818F File Offset: 0x0000638F
		internal void Init(MainWindow window)
		{
			this.ParentWindow = window;
			if (!DesignerProperties.GetIsInDesignMode(this) && !RegistryManager.Instance.UseEscapeToExitFullScreen)
			{
				this.mEscCheckbox.ImageName = "checkbox_new";
			}
			this.mVolumeBtn.ImageName = "volume_small";
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x000369EC File Offset: 0x00034BEC
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

		// Token: 0x060009B2 RID: 2482 RVA: 0x000081CC File Offset: 0x000063CC
		private void ScreenshotBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBarPopup.IsOpen = false;
			this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x000081EF File Offset: 0x000063EF
		private void VolumeBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.mVolumePopup.IsOpen = !this.mVolumePopup.IsOpen;
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x0000820A File Offset: 0x0000640A
		private void WindowedBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.FullScreenButtonHandler("fullscreentopbarDmm", "MouseClick");
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x00008226 File Offset: 0x00006426
		private void SettingsBtn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x0000823D File Offset: 0x0000643D
		internal void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow != null)
			{
				this.ParentWindow.mDmmBottomBar.VolumeSlider_PreviewMouseLeftButtonUp(sender, e);
			}
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00008259 File Offset: 0x00006459
		private void VolumeSliderImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow != null)
			{
				this.ParentWindow.mDmmBottomBar.VolumeSliderImage_PreviewMouseLeftButtonUp(sender, e);
			}
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00008275 File Offset: 0x00006475
		private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.DMMSwitchKeyMapButtonHandler();
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00036A3C File Offset: 0x00034C3C
		private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBarPopup.IsOpen = false;
			if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName != null)
			{
				this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "fullscreentopbar");
			}
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x00008287 File Offset: 0x00006487
		private void TranslucentControlsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
			this.mChangeTransparencyPopup.PlacementTarget = this.mTranslucentControlsButton;
			this.mChangeTransparencyPopup.IsOpen = true;
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x000082BB File Offset: 0x000064BB
		internal void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mDmmBottomBar.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(sender, e);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x000082CF File Offset: 0x000064CF
		internal void TransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			this.ParentWindow.mDmmBottomBar.TransparencySlider_ValueChanged(sender, e);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00036AA8 File Offset: 0x00034CA8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dmmfullscreentopbar.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00036AD8 File Offset: 0x00034CD8
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
				this.mEscCheckbox = (CustomPictureBox)target;
				this.mEscCheckbox.MouseLeftButtonUp += this.mEscCheckbox_MouseLeftButtonUp;
				return;
			case 2:
				this.mKeyMapSwitch = (CustomPictureBox)target;
				this.mKeyMapSwitch.PreviewMouseLeftButtonUp += this.SwitchKeyMapButton_PreviewMouseLeftButtonUp;
				return;
			case 3:
				this.mKeyMapButton = (CustomPictureBox)target;
				this.mKeyMapButton.PreviewMouseLeftButtonUp += this.KeyMapButton_PreviewMouseLeftButtonUp;
				return;
			case 4:
				this.mTranslucentControlsButton = (CustomPictureBox)target;
				this.mTranslucentControlsButton.PreviewMouseLeftButtonUp += this.TranslucentControlsButton_PreviewMouseLeftButtonUp;
				return;
			case 5:
				this.mScreenshotBtn = (CustomPictureBox)target;
				this.mScreenshotBtn.PreviewMouseLeftButtonUp += this.ScreenshotBtn_MouseUp;
				return;
			case 6:
				this.mVolumeBtn = (CustomPictureBox)target;
				this.mVolumeBtn.PreviewMouseLeftButtonUp += this.VolumeBtn_MouseUp;
				return;
			case 7:
				this.mWindowedBtn = (CustomPictureBox)target;
				this.mWindowedBtn.PreviewMouseLeftButtonUp += this.WindowedBtn_MouseUp;
				return;
			case 8:
				this.mSettingsBtn = (CustomPictureBox)target;
				this.mSettingsBtn.PreviewMouseLeftButtonUp += this.SettingsBtn_MouseUp;
				return;
			case 9:
				this.mVolumePopup = (CustomPopUp)target;
				return;
			case 10:
				this.volumeSliderImage = (CustomPictureBox)target;
				this.volumeSliderImage.PreviewMouseLeftButtonUp += this.VolumeSliderImage_PreviewMouseLeftButtonUp;
				return;
			case 11:
				this.mVolumeSlider = (Slider)target;
				this.mVolumeSlider.PreviewMouseLeftButtonUp += this.VolumeSlider_PreviewMouseLeftButtonUp;
				return;
			case 12:
				this.mChangeTransparencyPopup = (CustomPopUp)target;
				return;
			case 13:
				this.mTranslucentControlsSliderButton = (CustomPictureBox)target;
				this.mTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp;
				return;
			case 14:
				this.transSlider = (Slider)target;
				this.transSlider.ValueChanged += this.TransparencySlider_ValueChanged;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000585 RID: 1413
		private MainWindow ParentWindow;

		// Token: 0x04000586 RID: 1414
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mEscCheckbox;

		// Token: 0x04000587 RID: 1415
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mKeyMapSwitch;

		// Token: 0x04000588 RID: 1416
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mKeyMapButton;

		// Token: 0x04000589 RID: 1417
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTranslucentControlsButton;

		// Token: 0x0400058A RID: 1418
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mScreenshotBtn;

		// Token: 0x0400058B RID: 1419
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mVolumeBtn;

		// Token: 0x0400058C RID: 1420
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mWindowedBtn;

		// Token: 0x0400058D RID: 1421
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSettingsBtn;

		// Token: 0x0400058E RID: 1422
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mVolumePopup;

		// Token: 0x0400058F RID: 1423
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox volumeSliderImage;

		// Token: 0x04000590 RID: 1424
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider mVolumeSlider;

		// Token: 0x04000591 RID: 1425
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mChangeTransparencyPopup;

		// Token: 0x04000592 RID: 1426
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTranslucentControlsSliderButton;

		// Token: 0x04000593 RID: 1427
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Slider transSlider;

		// Token: 0x04000594 RID: 1428
		private bool _contentLoaded;
	}
}
