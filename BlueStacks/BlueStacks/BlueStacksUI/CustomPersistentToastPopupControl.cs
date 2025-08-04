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
	// Token: 0x020000B0 RID: 176
	public class CustomPersistentToastPopupControl : UserControl, IComponentConnector
	{
		// Token: 0x0600071E RID: 1822 RVA: 0x00006A24 File Offset: 0x00004C24
		public CustomPersistentToastPopupControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x00027BA8 File Offset: 0x00025DA8
		public CustomPersistentToastPopupControl(Window window)
		{
			this.InitializeComponent();
			if (window != null)
			{
				Grid grid = new Grid();
				object content = window.Content;
				window.Content = grid;
				grid.Children.Add(content as UIElement);
				grid.Children.Add(this);
			}
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00027BF8 File Offset: 0x00025DF8
		public bool Init(MainWindow window, string text)
		{
			this.ParentWindow = window;
			if (this.ParentWindow != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsShootingModeTooltipEnabled && RegistryManager.Instance.IsShootingModeTooltipVisible)
			{
				base.Visibility = Visibility.Visible;
				this.mPersistentToastTextblock.Text = text;
				this.mPersistentToastPopupBorder.HorizontalAlignment = HorizontalAlignment.Center;
				this.mPersistentToastPopupBorder.VerticalAlignment = VerticalAlignment.Center;
				base.UpdateLayout();
				return true;
			}
			return false;
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x00006A32 File Offset: 0x00004C32
		private void MCloseIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mCloseSettingsPopup.IsOpen = true;
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x00006A40 File Offset: 0x00004C40
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			(sender as Grid).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FFFFFF"));
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00006A61 File Offset: 0x00004C61
		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			(sender as Grid).Background = Brushes.Transparent;
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00006A73 File Offset: 0x00004C73
		private void mNeverShowAgain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mCloseSettingsPopup.IsOpen = false;
			base.Visibility = Visibility.Collapsed;
			RegistryManager.Instance.IsShootingModeTooltipVisible = false;
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00006A93 File Offset: 0x00004C93
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mCloseSettingsPopup.IsOpen = false;
			base.Visibility = Visibility.Collapsed;
			this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsShootingModeTooltipEnabled = false;
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00027C70 File Offset: 0x00025E70
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/custompersistenttoastpopupcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x00027CA0 File Offset: 0x00025EA0
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
				this.mPersistentToastPopupBorder = (Border)target;
				return;
			case 2:
				this.mCloseIcon = (CustomPictureBox)target;
				this.mCloseIcon.MouseLeftButtonUp += this.MCloseIcon_MouseLeftButtonUp;
				return;
			case 3:
				this.mPersistentToastTextblock = (TextBlock)target;
				return;
			case 4:
				this.mCloseSettingsPopup = (CustomPopUp)target;
				return;
			case 5:
				this.dummyGrid = (Grid)target;
				return;
			case 6:
				this.mCloseSettingsPopupBorder = (Border)target;
				return;
			case 7:
				this.mMaskBorder1 = (Border)target;
				return;
			case 8:
				((Grid)target).MouseEnter += this.Grid_MouseEnter;
				((Grid)target).MouseLeave += this.Grid_MouseLeave;
				((Grid)target).MouseLeftButtonUp += this.mNeverShowAgain_MouseLeftButtonUp;
				return;
			case 9:
				((Grid)target).MouseEnter += this.Grid_MouseEnter;
				((Grid)target).MouseLeave += this.Grid_MouseLeave;
				((Grid)target).MouseLeftButtonUp += this.Close_MouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040003C2 RID: 962
		private MainWindow ParentWindow;

		// Token: 0x040003C3 RID: 963
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mPersistentToastPopupBorder;

		// Token: 0x040003C4 RID: 964
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseIcon;

		// Token: 0x040003C5 RID: 965
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mPersistentToastTextblock;

		// Token: 0x040003C6 RID: 966
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mCloseSettingsPopup;

		// Token: 0x040003C7 RID: 967
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid dummyGrid;

		// Token: 0x040003C8 RID: 968
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mCloseSettingsPopupBorder;

		// Token: 0x040003C9 RID: 969
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder1;

		// Token: 0x040003CA RID: 970
		private bool _contentLoaded;
	}
}
