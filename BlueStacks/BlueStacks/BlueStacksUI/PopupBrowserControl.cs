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
	// Token: 0x02000131 RID: 305
	public class PopupBrowserControl : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x06000C41 RID: 3137 RVA: 0x00009B8F File Offset: 0x00007D8F
		bool IDimOverlayControl.Close()
		{
			base.Visibility = Visibility.Hidden;
			this.ClosePopupBrowser();
			return true;
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000C42 RID: 3138 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x06000C43 RID: 3139 RVA: 0x00004786 File Offset: 0x00002986
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

		// Token: 0x06000C44 RID: 3140 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000C45 RID: 3141 RVA: 0x00009B9F File Offset: 0x00007D9F
		// (set) Token: 0x06000C46 RID: 3142 RVA: 0x00009BA7 File Offset: 0x00007DA7
		public bool ShowControlInSeparateWindow { get; set; } = true;

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000C47 RID: 3143 RVA: 0x00009BB0 File Offset: 0x00007DB0
		// (set) Token: 0x06000C48 RID: 3144 RVA: 0x00009BB8 File Offset: 0x00007DB8
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x06000C49 RID: 3145 RVA: 0x00009BC1 File Offset: 0x00007DC1
		public PopupBrowserControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x00044D9C File Offset: 0x00042F9C
		public void Init(string url, string title, MainWindow window)
		{
			this.mTitle.Text = title;
			this.mBrowser.mUrl = url;
			this.mBrowser.mGrid = new Grid();
			this.mBrowser.Content = this.mBrowser.mGrid;
			this.mBrowser.CreateNewBrowser();
			if (window != null)
			{
				window.SizeChanged += this.Window_SizeChanged;
			}
			this.ParentWindow = window;
			this.mBrowser.ParentWindow = window;
			this.FixUpUILayout();
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x00009BD6 File Offset: 0x00007DD6
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.FixUpUILayout();
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x00044E20 File Offset: 0x00043020
		private void FixUpUILayout()
		{
			if (this.ParentWindow.mIsFullScreen || this.ParentWindow.WindowState == WindowState.Maximized)
			{
				base.Width = 880.0;
				base.Height = 530.0;
				return;
			}
			base.Width = 780.0;
			base.Height = 480.0;
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x00044E88 File Offset: 0x00043088
		public void ClosePopupBrowser()
		{
			ClientStats.SendPopupBrowserStatsInMiscASync("closed", this.mBrowser.mUrl);
			if (this.ParentWindow != null)
			{
				this.ParentWindow.HideDimOverlay();
			}
			if (this.mBrowser.CefBrowser != null)
			{
				this.mBrowser.DisposeBrowser();
			}
			base.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x00009BDE File Offset: 0x00007DDE
		private void CloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ClosePopupBrowser();
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x00044EDC File Offset: 0x000430DC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/popupbrowsercontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x00044F0C File Offset: 0x0004310C
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
				this.mGridBorder = (Border)target;
				return;
			case 2:
				this.mOuterGrid = (Grid)target;
				return;
			case 3:
				this.mTitle = (TextBlock)target;
				return;
			case 4:
				this.CloseBtn = (CustomPictureBox)target;
				this.CloseBtn.PreviewMouseLeftButtonUp += this.CloseBtn_PreviewMouseLeftButtonUp;
				return;
			case 5:
				this.mGrid = (Grid)target;
				return;
			case 6:
				this.mBrowser = (BrowserControl)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000783 RID: 1923
		private MainWindow ParentWindow;

		// Token: 0x04000786 RID: 1926
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mGridBorder;

		// Token: 0x04000787 RID: 1927
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOuterGrid;

		// Token: 0x04000788 RID: 1928
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTitle;

		// Token: 0x04000789 RID: 1929
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox CloseBtn;

		// Token: 0x0400078A RID: 1930
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x0400078B RID: 1931
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal BrowserControl mBrowser;

		// Token: 0x0400078C RID: 1932
		private bool _contentLoaded;
	}
}
