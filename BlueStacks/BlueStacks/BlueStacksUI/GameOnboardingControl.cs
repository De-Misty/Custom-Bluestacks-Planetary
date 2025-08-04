using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200006F RID: 111
	public class GameOnboardingControl : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x0600056F RID: 1391 RVA: 0x00004786 File Offset: 0x00002986
		bool IDimOverlayControl.IsCloseOnOverLayClick
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x00005A8D File Offset: 0x00003C8D
		// (set) Token: 0x06000571 RID: 1393 RVA: 0x00005A95 File Offset: 0x00003C95
		public bool ShowControlInSeparateWindow { get; set; } = true;

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000572 RID: 1394 RVA: 0x00005A9E File Offset: 0x00003C9E
		// (set) Token: 0x06000573 RID: 1395 RVA: 0x00005AA6 File Offset: 0x00003CA6
		public bool ShowTransparentWindow { get; set; } = true;

		// Token: 0x06000574 RID: 1396 RVA: 0x00005AAF File Offset: 0x00003CAF
		bool IDimOverlayControl.Close()
		{
			return true;
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x00005AB2 File Offset: 0x00003CB2
		// (set) Token: 0x06000577 RID: 1399 RVA: 0x00005ABA File Offset: 0x00003CBA
		private BrowserControl mBrowser { get; set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x00005AC3 File Offset: 0x00003CC3
		// (set) Token: 0x06000579 RID: 1401 RVA: 0x00005ACB File Offset: 0x00003CCB
		public string PackageName { get; set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x00005AD4 File Offset: 0x00003CD4
		// (set) Token: 0x0600057B RID: 1403 RVA: 0x00005ADC File Offset: 0x00003CDC
		public MainWindow ParentWindow { get; set; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x00005AE5 File Offset: 0x00003CE5
		// (set) Token: 0x0600057D RID: 1405 RVA: 0x00005AED File Offset: 0x00003CED
		public Grid controlGrid { get; set; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x00005AF6 File Offset: 0x00003CF6
		// (set) Token: 0x0600057F RID: 1407 RVA: 0x00005AFE File Offset: 0x00003CFE
		public string InitiatedSource { get; set; }

		// Token: 0x06000580 RID: 1408 RVA: 0x00005B07 File Offset: 0x00003D07
		public GameOnboardingControl(MainWindow mainWindow, string packageName, string source)
		{
			this.PackageName = packageName;
			this.ParentWindow = mainWindow;
			this.InitiatedSource = source;
			this.InitializeComponent();
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0002094C File Offset: 0x0001EB4C
		private void Control_Loaded(object sender, RoutedEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("onboarding-tutorial", "client_impression", this.ParentWindow.mVmName, this.PackageName, "", "");
			this.mBrowser = new BrowserControl();
			this.mBrowser.BrowserLoadCompleteEvent += this.BrowserLoadCompleteEvent;
			this.mBrowser.InitBaseControl(BlueStacksUIUtils.GetOnboardingUrl(this.PackageName, this.InitiatedSource), 0f);
			this.mBrowser.Visibility = Visibility.Visible;
			this.mBrowser.ParentWindow = this.ParentWindow;
			this.mBrowserGrid.Children.Add(this.mBrowser);
			this.controlGrid = this.AddBrowser();
			this.controlGrid.Visibility = Visibility.Visible;
			this.mBrowserGrid.Children.Add(this.controlGrid);
			this.dispatcherTimer = new DispatcherTimer();
			this.dispatcherTimer.Tick += this.DispatcherTimer_Tick;
			this.dispatcherTimer.Interval = new TimeSpan(0, 0, PostBootCloudInfoManager.Instance.mPostBootCloudInfo.OnBoardingInfo.OnBoardingSkipTimer);
			this.dispatcherTimer.Start();
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x00020A7C File Offset: 0x0001EC7C
		internal Grid AddBrowser()
		{
			Grid grid = new Grid();
			CustomPictureBox customPictureBox = new CustomPictureBox
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Height = 30.0,
				Width = 30.0,
				ImageName = "loader",
				IsImageToBeRotated = true
			};
			grid.Children.Add(customPictureBox);
			grid.Visibility = Visibility.Hidden;
			return grid;
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x00020AE8 File Offset: 0x0001ECE8
		private void BrowserLoadCompleteEvent()
		{
			AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsAppOnboardingCompleted = true;
			this.mBrowserGrid.Children.Remove(this.controlGrid);
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00005B38 File Offset: 0x00003D38
		private void DispatcherTimer_Tick(object _1, EventArgs _2)
		{
			this.mSkipOnboardingButton.Visibility = Visibility.Visible;
			this.dispatcherTimer.Stop();
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00020B38 File Offset: 0x0001ED38
		private void SkipOnboardingButton_Click(object sender, RoutedEventArgs e)
		{
			AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsAppOnboardingCompleted = true;
			Stats.SendCommonClientStatsAsync("onboarding-tutorial", "onboarding_skipped", this.ParentWindow.mVmName, this.PackageName, "", "");
			this.Close();
			GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
			if (sGuidanceWindow != null)
			{
				sGuidanceWindow.DimOverLayVisibility(Visibility.Collapsed);
			}
			if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (AppSettings appSettings) => appSettings.IsGeneralAppOnBoardingCompleted))
			{
				this.ParentWindow.StaticComponents.mSelectedTabButton.ShowDefaultBlurbOnboarding();
			}
			this.ParentWindow.HideDimOverlay();
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x00005B51 File Offset: 0x00003D51
		private void Control_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.System && e.SystemKey == Key.F4)
			{
				e.Handled = true;
			}
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00020C18 File Offset: 0x0001EE18
		internal bool Close()
		{
			try
			{
				if (this.mBrowser != null)
				{
					this.mBrowser.DisposeBrowser();
					this.mBrowserGrid.Children.Remove(this.mBrowser);
					this.mBrowser = null;
				}
				BlueStacksUIUtils.CloseContainerWindow(this);
				base.Visibility = Visibility.Hidden;
				Stats.SendCommonClientStatsAsync("onboarding-tutorial", "client_closed", this.ParentWindow.mVmName, this.PackageName, "", "");
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to close gameonboardingcontrol from dimoverlay " + ex.ToString());
			}
			return false;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00020CBC File Offset: 0x0001EEBC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/gameonboardingcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00020CEC File Offset: 0x0001EEEC
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
				((GameOnboardingControl)target).Loaded += this.Control_Loaded;
				((GameOnboardingControl)target).KeyDown += this.Control_KeyDown;
				return;
			case 2:
				this.mBrowserGrid = (Grid)target;
				return;
			case 3:
				this.mSkipOnboardingButton = (CustomButton)target;
				this.mSkipOnboardingButton.Click += this.SkipOnboardingButton_Click;
				return;
			case 4:
				this.mBrowserGridTemp = (Grid)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040002D7 RID: 727
		private DispatcherTimer dispatcherTimer;

		// Token: 0x040002DC RID: 732
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBrowserGrid;

		// Token: 0x040002DD RID: 733
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mSkipOnboardingButton;

		// Token: 0x040002DE RID: 734
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBrowserGridTemp;

		// Token: 0x040002DF RID: 735
		private bool _contentLoaded;
	}
}
