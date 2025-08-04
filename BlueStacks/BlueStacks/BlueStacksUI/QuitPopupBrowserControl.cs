using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200010F RID: 271
	public class QuitPopupBrowserControl : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x06000B3E RID: 2878 RVA: 0x00004786 File Offset: 0x00002986
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

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x000091DA File Offset: 0x000073DA
		// (set) Token: 0x06000B40 RID: 2880 RVA: 0x000091E2 File Offset: 0x000073E2
		public bool ShowControlInSeparateWindow { get; set; } = true;

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000B41 RID: 2881 RVA: 0x000091EB File Offset: 0x000073EB
		// (set) Token: 0x06000B42 RID: 2882 RVA: 0x000091F3 File Offset: 0x000073F3
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x06000B43 RID: 2883 RVA: 0x00005AAF File Offset: 0x00003CAF
		bool IDimOverlayControl.Close()
		{
			return true;
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000B45 RID: 2885 RVA: 0x000091FC File Offset: 0x000073FC
		// (set) Token: 0x06000B46 RID: 2886 RVA: 0x00009204 File Offset: 0x00007404
		private MainWindow ParentWindow { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000B47 RID: 2887 RVA: 0x0000920D File Offset: 0x0000740D
		// (set) Token: 0x06000B48 RID: 2888 RVA: 0x00009215 File Offset: 0x00007415
		private BrowserControl mBrowser { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x0000921E File Offset: 0x0000741E
		// (set) Token: 0x06000B4A RID: 2890 RVA: 0x00009226 File Offset: 0x00007426
		internal string PackageName { get; set; } = "";

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000B4B RID: 2891 RVA: 0x0000922F File Offset: 0x0000742F
		// (set) Token: 0x06000B4C RID: 2892 RVA: 0x00009237 File Offset: 0x00007437
		internal string QuitPopupUrl { get; set; } = string.Empty;

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000B4D RID: 2893 RVA: 0x00009240 File Offset: 0x00007440
		// (set) Token: 0x06000B4E RID: 2894 RVA: 0x00009248 File Offset: 0x00007448
		internal bool IsForceReload { get; set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000B4F RID: 2895 RVA: 0x00009251 File Offset: 0x00007451
		// (set) Token: 0x06000B50 RID: 2896 RVA: 0x00009259 File Offset: 0x00007459
		internal bool ShowOnQuit { get; set; }

		// Token: 0x06000B51 RID: 2897 RVA: 0x00009262 File Offset: 0x00007462
		public QuitPopupBrowserControl(MainWindow mainWindow)
		{
			this.ParentWindow = mainWindow;
			this.InitializeComponent();
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x00009294 File Offset: 0x00007494
		internal void SetQuitPopParams(string url, string package, bool isForceReload, bool showOnQuit)
		{
			this.QuitPopupUrl = url;
			this.IsForceReload = isForceReload;
			this.PackageName = package;
			this.ShowOnQuit = showOnQuit;
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0003F8E8 File Offset: 0x0003DAE8
		internal void Init(string appPackage)
		{
			base.Width = 740.0;
			base.Height = 490.0;
			this.PackageName = appPackage;
			this.mCloseButton.Content = (string.IsNullOrEmpty(appPackage) ? LocaleStrings.GetLocalizedString("STRING_CLOSE_BLUESTACKS", "") : LocaleStrings.GetLocalizedString("STRING_CLOSE_GAME", ""));
			ClientStats.SendMiscellaneousStatsAsync("quitpopupdisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.QuitPopupUrl, this.PackageName, null, null, null, null);
			if (this.mBrowser == null)
			{
				this.LoadBrowser();
			}
			this.ParentWindow.mQuitPopupBrowserLoadGrid.Children.Remove(this.mBrowser);
			this.mBrowserGrid.Children.Add(this.mBrowser);
			this.mBrowser.Visibility = Visibility.Visible;
			this.ParentWindow.ShowDimOverlay(this);
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x000092B3 File Offset: 0x000074B3
		internal void LoadBrowser()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.DisposeBrowser();
				this.mBrowser = new BrowserControl();
				this.mBrowser.InitBaseControl(this.QuitPopupUrl, 0f);
				this.mBrowser.ParentWindow = this.ParentWindow;
				this.ParentWindow.mQuitPopupBrowserLoadGrid.Children.Add(this.mBrowser);
				this.ParentWindow.mQuitPopupBrowserLoadGrid.Visibility = Visibility.Hidden;
				this.mBrowser.CreateNewBrowser();
			}), new object[0]);
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0003F9D4 File Offset: 0x0003DBD4
		internal void RefreshBrowserUrl(string url)
		{
			try
			{
				this.QuitPopupUrl = url;
				this.mBrowser.UpdateUrlAndRefresh(url);
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in refreshing quitpopup borwser url: " + ex.ToString());
			}
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x0003FA20 File Offset: 0x0003DC20
		private void DisposeBrowser()
		{
			if (this.mBrowser != null)
			{
				this.mBrowser.DisposeBrowser();
				this.mBrowserGrid.Children.Remove(this.mBrowser);
				this.ParentWindow.mQuitPopupBrowserLoadGrid.Children.Remove(this.mBrowser);
				this.mBrowser = null;
			}
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x000092D3 File Offset: 0x000074D3
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x0003FA78 File Offset: 0x0003DC78
		internal void Close()
		{
			try
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.DisposeBrowser();
					this.ParentWindow.mTopBar.mAppTabButtons.mLastPackageForQuitPopupDisplayed = "";
					base.Visibility = Visibility.Hidden;
					ClientStats.SendMiscellaneousStatsAsync("quitpopupclosed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.QuitPopupUrl, this.PackageName, null, null, null, null);
					this.ParentWindow.HideDimOverlay();
					if (string.IsNullOrEmpty(this.PackageName))
					{
						this.ParentWindow.ForceCloseWindow(false);
						return;
					}
					this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.PackageName, true, false, false, false, "");
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception when trying to close quit popup. " + ex.ToString());
			}
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0003FAD0 File Offset: 0x0003DCD0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/quitpopupbrowsercontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0003FB00 File Offset: 0x0003DD00
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
				this.mGrid = (Grid)target;
				return;
			case 2:
				this.mBrowserGrid = (Grid)target;
				return;
			case 3:
				this.mCloseButton = (CustomButton)target;
				this.mCloseButton.Click += this.CloseButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040006D3 RID: 1747
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x040006D4 RID: 1748
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBrowserGrid;

		// Token: 0x040006D5 RID: 1749
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mCloseButton;

		// Token: 0x040006D6 RID: 1750
		private bool _contentLoaded;
	}
}
