using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000B5 RID: 181
	public class FullScreenToastPopupControl : UserControl, IComponentConnector
	{
		// Token: 0x0600075E RID: 1886 RVA: 0x00006CDB File Offset: 0x00004EDB
		public FullScreenToastPopupControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x00028E3C File Offset: 0x0002703C
		public FullScreenToastPopupControl(MainWindow window)
		{
			this.InitializeComponent();
			if (window != null)
			{
				this.ParentWindow = window;
				Grid grid = new Grid();
				object content = window.Content;
				window.Content = grid;
				grid.Children.Add(content as UIElement);
				grid.Children.Add(this);
			}
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x00028E94 File Offset: 0x00027094
		public void Init(MainWindow window, string text)
		{
			if (window != null)
			{
				this.ParentWindow = window;
				this.mToastPanel.MaxWidth = this.ParentWindow.ActualWidth - 15.0;
			}
			this.mTipTextblock.Text = text;
			this.mInfoTextblock.Visibility = Visibility.Collapsed;
			this.mKeyBorder.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00028EF0 File Offset: 0x000270F0
		public void Init(MainWindow window, string tip, string key, string info)
		{
			if (window != null)
			{
				this.ParentWindow = window;
				this.mToastPanel.MaxWidth = this.ParentWindow.ActualWidth - 15.0;
			}
			this.mTipTextblock.Text = tip;
			this.mKeyTextBlock.Text = key;
			this.mInfoTextblock.Text = info;
			this.mInfoTextblock.Visibility = Visibility.Visible;
			this.mKeyBorder.Visibility = Visibility.Visible;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00028F64 File Offset: 0x00027164
		public void ShowPopup(double seconds = 4.0)
		{
			base.Visibility = Visibility.Visible;
			base.Opacity = 0.0;
			DoubleAnimation doubleAnimation = new DoubleAnimation
			{
				From = new double?(0.0),
				To = new double?(seconds),
				Duration = new Duration(TimeSpan.FromSeconds(0.3))
			};
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(doubleAnimation);
			Storyboard.SetTarget(doubleAnimation, this);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(UIElement.OpacityProperty));
			storyboard.Completed += delegate
			{
				this.Visibility = Visibility.Visible;
				DoubleAnimation doubleAnimation2 = new DoubleAnimation
				{
					From = new double?(seconds),
					To = new double?(0.0),
					FillBehavior = FillBehavior.Stop,
					BeginTime = new TimeSpan?(TimeSpan.FromSeconds(seconds)),
					Duration = new Duration(TimeSpan.FromSeconds(seconds / 2.0))
				};
				Storyboard storyboard2 = new Storyboard();
				storyboard2.Children.Add(doubleAnimation2);
				Storyboard.SetTarget(doubleAnimation2, this);
				Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(UIElement.OpacityProperty));
				storyboard2.Completed += delegate
				{
					this.Visibility = Visibility.Collapsed;
				};
				storyboard2.Begin();
			};
			storyboard.Begin();
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00006CE9 File Offset: 0x00004EE9
		private void ToastIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.CloseFullScreenToastAndStopTimer();
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00029020 File Offset: 0x00027220
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/fullscreentoastpopupcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00029050 File Offset: 0x00027250
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
				this.mToastPopupBorder = (Border)target;
				return;
			case 2:
				this.mMaskBorder = (Border)target;
				return;
			case 3:
				this.mToastPanel = (DockPanel)target;
				return;
			case 4:
				this.mTipTextblock = (TextBlock)target;
				return;
			case 5:
				this.mKeyBorder = (Border)target;
				return;
			case 6:
				this.mKeyTextBlock = (TextBlock)target;
				return;
			case 7:
				this.mInfoTextblock = (TextBlock)target;
				return;
			case 8:
				this.mToastIcon = (CustomPictureBox)target;
				this.mToastIcon.MouseLeftButtonUp += this.ToastIcon_MouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040003ED RID: 1005
		private MainWindow ParentWindow;

		// Token: 0x040003EE RID: 1006
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mToastPopupBorder;

		// Token: 0x040003EF RID: 1007
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x040003F0 RID: 1008
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal DockPanel mToastPanel;

		// Token: 0x040003F1 RID: 1009
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTipTextblock;

		// Token: 0x040003F2 RID: 1010
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mKeyBorder;

		// Token: 0x040003F3 RID: 1011
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mKeyTextBlock;

		// Token: 0x040003F4 RID: 1012
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mInfoTextblock;

		// Token: 0x040003F5 RID: 1013
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mToastIcon;

		// Token: 0x040003F6 RID: 1014
		private bool _contentLoaded;
	}
}
