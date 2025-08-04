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
	// Token: 0x020000B2 RID: 178
	public class DimOverlayControl : CustomWindow, IComponentConnector
	{
		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x00006B8D File Offset: 0x00004D8D
		// (set) Token: 0x0600073A RID: 1850 RVA: 0x0002819C File Offset: 0x0002639C
		public new MainWindow Owner
		{
			get
			{
				return (MainWindow)base.Owner;
			}
			set
			{
				if (value != null)
				{
					if (value != base.Owner)
					{
						value.LocationChanged += this.ParentWindow_LocationChanged;
						value.SizeChanged += this.ParentWindow_SizeChanged;
					}
				}
				else if (base.Owner != null)
				{
					base.Owner.LocationChanged -= this.ParentWindow_LocationChanged;
					base.Owner.SizeChanged -= this.ParentWindow_SizeChanged;
				}
				base.Owner = value;
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x0600073B RID: 1851 RVA: 0x00006B9A File Offset: 0x00004D9A
		// (set) Token: 0x0600073C RID: 1852 RVA: 0x00006BA2 File Offset: 0x00004DA2
		internal IDimOverlayControl Control
		{
			get
			{
				return this.mControl;
			}
			set
			{
				if (value != null)
				{
					this.AddControl(value);
				}
				this.mControl = value;
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x0600073D RID: 1853 RVA: 0x00006BB5 File Offset: 0x00004DB5
		// (set) Token: 0x0600073E RID: 1854 RVA: 0x00006BBD File Offset: 0x00004DBD
		public bool IsWindowVisible { get; set; }

		// Token: 0x0600073F RID: 1855 RVA: 0x00006BC6 File Offset: 0x00004DC6
		private void ParentWindow_LocationChanged(object sender, EventArgs e)
		{
			this.UpadteSizeLocation();
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x00006BC6 File Offset: 0x00004DC6
		private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpadteSizeLocation();
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x00006BCE File Offset: 0x00004DCE
		public DimOverlayControl(MainWindow owner)
		{
			this.Owner = owner;
			this.InitializeComponent();
			base.IsShowGLWindow = true;
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x00028218 File Offset: 0x00026418
		internal void AddControl(IDimOverlayControl el)
		{
			if (!el.ShowControlInSeparateWindow)
			{
				if (!this.mGrid.Children.Contains(el as UIElement))
				{
					this.mGrid.Children.Add(el as UIElement);
					return;
				}
				(el as UIElement).Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x0002826C File Offset: 0x0002646C
		internal void RemoveControl()
		{
			if (!this.Control.ShowControlInSeparateWindow)
			{
				if (this.mGrid.Children.Contains(this.Control as UIElement))
				{
					this.mGrid.Children.Remove(this.Control as UIElement);
					this.Control.Close();
					return;
				}
			}
			else
			{
				if (this.Control != null)
				{
					BlueStacksUIUtils.RemoveChildFromParent((UIElement)this.Control);
					this.Control.Close();
				}
				if (this.cw != null)
				{
					this.cw.Close();
				}
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x00028304 File Offset: 0x00026504
		internal void UpadteSizeLocation()
		{
			if (this.Owner != null && PresentationSource.FromVisual(this.Owner) != null)
			{
				Point point = this.Owner.PointToScreen(new Point(0.0, 0.0));
				Point point2 = PresentationSource.FromVisual(this.Owner).CompositionTarget.TransformFromDevice.Transform(point);
				base.Left = point2.X;
				base.Top = point2.Y;
				base.Width = this.Owner.ActualWidth;
				base.Height = this.Owner.ActualHeight;
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x000283A8 File Offset: 0x000265A8
		internal void ShowWindow()
		{
			if (!this.IsWindowVisible)
			{
				this.IsWindowVisible = true;
				base.Show();
				if (this.Control != null)
				{
					if (!this.Control.ShowControlInSeparateWindow)
					{
						this.Control.Show();
						return;
					}
					this.Control.Show();
					this.cw = new ContainerWindow(this.Owner, (UserControl)this.Control, this.Control.Width, this.Control.Height, false, false, this.Control.ShowTransparentWindow, -1.0, null);
					this.cw.Show();
				}
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00028450 File Offset: 0x00026650
		internal void HideWindow(bool isFromOverlayClick)
		{
			if (this.IsWindowVisible)
			{
				if (this.Control != null)
				{
					if (!isFromOverlayClick)
					{
						this.IsWindowVisible = false;
						this.RemoveControl();
						this.Hide();
						return;
					}
					if (this.Control.IsCloseOnOverLayClick)
					{
						this.IsWindowVisible = false;
						this.RemoveControl();
						this.Hide();
						return;
					}
					if (this.cw != null)
					{
						this.cw.Focus();
						return;
					}
				}
				else
				{
					this.IsWindowVisible = false;
					this.Hide();
				}
			}
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00006BEA File Offset: 0x00004DEA
		public new void Hide()
		{
			if (!this.IsWindowVisible)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					base.Hide();
				}), new object[0]);
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x000284C8 File Offset: 0x000266C8
		private void DimWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.System && e.SystemKey == Key.F4 && this.Control != null && FeatureManager.Instance.IsCustomUIForNCSoft && this.Control.GetType() == BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].ScreenLockInstance.GetType() && BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].ScreenLockInstance.IsVisible)
			{
				e.Handled = true;
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00006C12 File Offset: 0x00004E12
		private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.HideWindow(true);
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00028548 File Offset: 0x00026748
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dimoverlaycontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00028578 File Offset: 0x00026778
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
				((DimOverlayControl)target).KeyDown += this.DimWindow_KeyDown;
				return;
			case 2:
				this.mGrid = (Grid)target;
				return;
			case 3:
				((Grid)target).MouseUp += this.Grid_MouseUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040003DB RID: 987
		private IDimOverlayControl mControl;

		// Token: 0x040003DD RID: 989
		private ContainerWindow cw;

		// Token: 0x040003DE RID: 990
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x040003DF RID: 991
		private bool _contentLoaded;
	}
}
