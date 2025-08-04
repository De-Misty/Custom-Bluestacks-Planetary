using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000110 RID: 272
	public class QuitPopupControl : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000B5D RID: 2909 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x06000B5E RID: 2910 RVA: 0x00004786 File Offset: 0x00002986
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

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000B5F RID: 2911 RVA: 0x000092DB File Offset: 0x000074DB
		// (set) Token: 0x06000B60 RID: 2912 RVA: 0x000092E3 File Offset: 0x000074E3
		public bool ShowControlInSeparateWindow { get; set; } = true;

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000B61 RID: 2913 RVA: 0x000092EC File Offset: 0x000074EC
		// (set) Token: 0x06000B62 RID: 2914 RVA: 0x000092F4 File Offset: 0x000074F4
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x06000B63 RID: 2915 RVA: 0x000092FD File Offset: 0x000074FD
		bool IDimOverlayControl.Close()
		{
			this.Close();
			return true;
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x00009307 File Offset: 0x00007507
		public QuitPopupControl(MainWindow window)
		{
			this.ParentWindow = window;
			this.InitializeComponent();
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0000932E File Offset: 0x0000752E
		private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
			ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "click_action_close");
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000B67 RID: 2919 RVA: 0x00009347 File Offset: 0x00007547
		public TextBlock TitleTextBlock
		{
			get
			{
				return this.mTitleText;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000B68 RID: 2920 RVA: 0x0000934F File Offset: 0x0000754F
		public CustomButton CloseBlueStacksButton
		{
			get
			{
				return this.mCloseBlueStacksButton;
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x00009357 File Offset: 0x00007557
		public CustomButton ReturnBlueStacksButton
		{
			get
			{
				return this.mReturnBlueStacksButton;
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x0000935F File Offset: 0x0000755F
		public CustomPictureBox CrossButtonPictureBox
		{
			get
			{
				return this.mCrossButtonPictureBox;
			}
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x00009367 File Offset: 0x00007567
		private void CloseBlueStacksButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
			if (this.HasSuccessfulEventOccured)
			{
				ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "click_action_continue_bluestacks");
				return;
			}
			ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "popup_closed");
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x00009399 File Offset: 0x00007599
		// (set) Token: 0x06000B6D RID: 2925 RVA: 0x000093A1 File Offset: 0x000075A1
		public bool HasSuccessfulEventOccured
		{
			get
			{
				return this.mHasSuccessfulEventOccured;
			}
			set
			{
				if (value)
				{
					this.mHasSuccessfulEventOccured = value;
					this.mTitleGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0BA200");
				}
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000B6E RID: 2926 RVA: 0x000093CC File Offset: 0x000075CC
		// (set) Token: 0x06000B6F RID: 2927 RVA: 0x000093D4 File Offset: 0x000075D4
		public string CurrentPopupTag { get; set; } = string.Empty;

		// Token: 0x06000B70 RID: 2928 RVA: 0x0003FC94 File Offset: 0x0003DE94
		public void AddQuitActionItem(QuitActionItem item)
		{
			bool flag = this.mQuitElementStackPanel.Children.Count != 0;
			QuitActionElement quitActionElement = new QuitActionElement(this.ParentWindow, this)
			{
				Width = 210.0,
				ActionElement = item,
				ParentPopupTag = this.CurrentPopupTag
			};
			if (flag)
			{
				quitActionElement.Margin = new Thickness(32.0, 0.0, 0.0, 0.0);
			}
			this.mQuitElementStackPanel.Children.Add(quitActionElement);
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x0003FD28 File Offset: 0x0003DF28
		internal bool Close()
		{
			try
			{
				this.ParentWindow.HideDimOverlay();
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to close quitpopup from dimoverlay " + ex.ToString());
			}
			return false;
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x000093DD File Offset: 0x000075DD
		private void ReturnBlueStacksButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
			ClientStats.SendLocalQuitPopupStatsAsync(this.CurrentPopupTag, "click_action_return_bluestacks");
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x0003FD70 File Offset: 0x0003DF70
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/quitpopupcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0003FDA0 File Offset: 0x0003DFA0
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
				this.mParentGrid = (Grid)target;
				return;
			case 2:
				this.mTitleGrid = (Grid)target;
				return;
			case 3:
				this.mCrossButtonPictureBox = (CustomPictureBox)target;
				this.mCrossButtonPictureBox.PreviewMouseLeftButtonUp += this.Close_PreviewMouseLeftButtonUp;
				return;
			case 4:
				this.mTitleText = (TextBlock)target;
				return;
			case 5:
				this.mOptionsGrid = (Grid)target;
				return;
			case 6:
				this.mQuitElementStackPanel = (StackPanel)target;
				return;
			case 7:
				this.mFooterGrid = (Grid)target;
				return;
			case 8:
				this.mReturnBlueStacksButton = (CustomButton)target;
				this.mReturnBlueStacksButton.PreviewMouseLeftButtonUp += this.ReturnBlueStacksButton_PreviewMouseLeftButtonUp;
				return;
			case 9:
				this.mCloseBlueStacksButton = (CustomButton)target;
				this.mCloseBlueStacksButton.PreviewMouseLeftButtonUp += this.CloseBlueStacksButton_PreviewMouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040006D9 RID: 1753
		private MainWindow ParentWindow;

		// Token: 0x040006DA RID: 1754
		private bool mHasSuccessfulEventOccured;

		// Token: 0x040006DC RID: 1756
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mParentGrid;

		// Token: 0x040006DD RID: 1757
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTitleGrid;

		// Token: 0x040006DE RID: 1758
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCrossButtonPictureBox;

		// Token: 0x040006DF RID: 1759
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTitleText;

		// Token: 0x040006E0 RID: 1760
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOptionsGrid;

		// Token: 0x040006E1 RID: 1761
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mQuitElementStackPanel;

		// Token: 0x040006E2 RID: 1762
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mFooterGrid;

		// Token: 0x040006E3 RID: 1763
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mReturnBlueStacksButton;

		// Token: 0x040006E4 RID: 1764
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mCloseBlueStacksButton;

		// Token: 0x040006E5 RID: 1765
		private bool _contentLoaded;
	}
}
