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
	// Token: 0x0200026C RID: 620
	public class DimControlWithProgresBar : UserControl, IComponentConnector
	{
		// Token: 0x17000349 RID: 841
		// (get) Token: 0x0600168A RID: 5770 RVA: 0x0000F218 File Offset: 0x0000D418
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

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x0600168B RID: 5771 RVA: 0x0000F239 File Offset: 0x0000D439
		// (set) Token: 0x0600168C RID: 5772 RVA: 0x0000F241 File Offset: 0x0000D441
		public Control ParentControl { get; set; }

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x0600168D RID: 5773 RVA: 0x0000F24A File Offset: 0x0000D44A
		// (set) Token: 0x0600168E RID: 5774 RVA: 0x0000F252 File Offset: 0x0000D452
		public Panel ChildControl { get; set; }

		// Token: 0x0600168F RID: 5775 RVA: 0x0000F25B File Offset: 0x0000D45B
		public DimControlWithProgresBar()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x00086C14 File Offset: 0x00084E14
		internal void Init(Control parentControl, Panel childControl, bool isWindowForced, bool _)
		{
			this.ParentControl = parentControl;
			this.ChildControl = childControl;
			this.FixUpUILayout();
			if (isWindowForced)
			{
				this.mBackButton.Visibility = Visibility.Hidden;
				this.mCloseButton.Visibility = Visibility.Hidden;
			}
			this.ParentWindow.SizeChanged += this.MainWindow_SizeChanged;
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x0000F269 File Offset: 0x0000D469
		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.FixUpUILayout();
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x00086C68 File Offset: 0x00084E68
		private void FixUpUILayout()
		{
			this.mControlGrid.Height = (double)((long)((int)(this.ParentWindow.mWelcomeTab.ActualHeight * 0.8 / (double)this.ParentWindow.mAspectRatio.Denominator)) * this.ParentWindow.mAspectRatio.Denominator);
			if (this.ParentWindow.mWelcomeTab.ActualHeight * 0.9 - this.mControlGrid.Height > 10.0)
			{
				this.mControlGrid.Height = this.ParentWindow.mWelcomeTab.ActualHeight * 0.8;
			}
			this.mControlGrid.Height += 50.0;
			this.mControlGrid.Width = (this.mControlGrid.Height - 50.0) * this.ParentWindow.mAspectRatio.DoubleValue + 10.0;
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x00086D6C File Offset: 0x00084F6C
		internal void ShowContent()
		{
			this.DimBackground();
			if (this.ChildControl != null)
			{
				if (this.ChildControl.Parent != null)
				{
					(this.ChildControl.Parent as Panel).Children.Remove(this.ChildControl);
				}
				this.mControlParentGrid.Children.Add(this.ChildControl);
			}
			this.mControlGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x0000F271 File Offset: 0x0000D471
		private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Cicked DimControl close button");
			this.HideWindow();
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x0000F283 File Offset: 0x0000D483
		internal void DimBackground()
		{
			Logger.Info("Diming popup window");
			if (this.ParentControl != null)
			{
				this.ParentControl.Visibility = Visibility.Visible;
			}
			base.Visibility = Visibility.Visible;
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x0000F2AA File Offset: 0x0000D4AA
		private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked Back Button");
			this.ParentWindow.mCommonHandler.BackButtonHandler(false);
		}

		// Token: 0x06001697 RID: 5783 RVA: 0x0000F2C7 File Offset: 0x0000D4C7
		internal void HideWindow()
		{
			Logger.Debug("Hiding popup window");
			base.Visibility = Visibility.Hidden;
			this.mControlGrid.Visibility = Visibility.Hidden;
			if (this.ParentControl != null)
			{
				this.ParentControl.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x00086DD8 File Offset: 0x00084FD8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dimcontrolwithprogresbar.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x00086E08 File Offset: 0x00085008
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
				this.mControlGrid = (Grid)target;
				return;
			case 2:
				this.mTopBar = (Grid)target;
				return;
			case 3:
				this.mBackButton = (CustomPictureBox)target;
				this.mBackButton.PreviewMouseLeftButtonUp += this.BackButton_PreviewMouseLeftButtonUp;
				return;
			case 4:
				this.mTitleLabel = (Label)target;
				return;
			case 5:
				this.mCloseButton = (CustomPictureBox)target;
				this.mCloseButton.PreviewMouseLeftButtonUp += this.CloseButton_PreviewMouseLeftButtonUp;
				return;
			case 6:
				this.mControlParentGrid = (Grid)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000DC4 RID: 3524
		private MainWindow mMainWindow;

		// Token: 0x04000DC7 RID: 3527
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mControlGrid;

		// Token: 0x04000DC8 RID: 3528
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTopBar;

		// Token: 0x04000DC9 RID: 3529
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mBackButton;

		// Token: 0x04000DCA RID: 3530
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mTitleLabel;

		// Token: 0x04000DCB RID: 3531
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseButton;

		// Token: 0x04000DCC RID: 3532
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mControlParentGrid;

		// Token: 0x04000DCD RID: 3533
		private bool _contentLoaded;
	}
}
