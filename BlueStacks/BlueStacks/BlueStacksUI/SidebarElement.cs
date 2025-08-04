using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000E5 RID: 229
	public class SidebarElement : UserControl, IComponentConnector
	{
		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000989 RID: 2441 RVA: 0x0000808D File Offset: 0x0000628D
		public CustomPictureBox Image
		{
			get
			{
				return this.mImage;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600098A RID: 2442 RVA: 0x00008095 File Offset: 0x00006295
		// (set) Token: 0x0600098B RID: 2443 RVA: 0x0000809D File Offset: 0x0000629D
		public bool IsLastElementOfGroup { get; set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x0600098C RID: 2444 RVA: 0x000080A6 File Offset: 0x000062A6
		// (set) Token: 0x0600098D RID: 2445 RVA: 0x000080AE File Offset: 0x000062AE
		public bool IsCurrentLastElementOfGroup { get; set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x0600098E RID: 2446 RVA: 0x000080B7 File Offset: 0x000062B7
		// (set) Token: 0x0600098F RID: 2447 RVA: 0x000080BF File Offset: 0x000062BF
		public bool IsInMainSidebar { get; set; } = true;

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000990 RID: 2448 RVA: 0x000080C8 File Offset: 0x000062C8
		// (set) Token: 0x06000991 RID: 2449 RVA: 0x000080D0 File Offset: 0x000062D0
		public string mSidebarElementTooltipKey { get; set; } = string.Empty;

		// Token: 0x06000992 RID: 2450 RVA: 0x000080D9 File Offset: 0x000062D9
		public SidebarElement()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x000080F9 File Offset: 0x000062F9
		private void SidebarElement_Loaded(object sender, RoutedEventArgs e)
		{
			this.SetColor(false);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x00008102 File Offset: 0x00006302
		private void MImage_Loaded(object sender, RoutedEventArgs e)
		{
			this.mImage = sender as CustomPictureBox;
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x00036240 File Offset: 0x00034440
		private void SetColor(bool isPressed = false)
		{
			if (isPressed)
			{
				BlueStacksUIBinding.BindColor(this.mBorder, Control.BorderBrushProperty, "SidebarElementClick");
				BlueStacksUIBinding.BindColor(this.mBorder, Control.BackgroundProperty, "SidebarElementClick");
				BlueStacksUIBinding.BindColor(this, Control.ForegroundProperty, "SidebarElementClick");
				return;
			}
			if (base.IsMouseOver)
			{
				BlueStacksUIBinding.BindColor(this.mBorder, Control.BorderBrushProperty, "SidebarElementHover");
				BlueStacksUIBinding.BindColor(this.mBorder, Control.BackgroundProperty, "SidebarElementHover");
				BlueStacksUIBinding.BindColor(this, Control.ForegroundProperty, "SidebarElementHover");
				return;
			}
			BlueStacksUIBinding.BindColor(this.mBorder, Control.BorderBrushProperty, "SidebarElementNormal");
			BlueStacksUIBinding.BindColor(this.mBorder, Control.BackgroundProperty, "SidebarElementNormal");
			BlueStacksUIBinding.BindColor(this, Control.ForegroundProperty, "SidebarElementNormal");
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x000080F9 File Offset: 0x000062F9
		private void SidebarElement_MouseEnter(object sender, MouseEventArgs e)
		{
			this.SetColor(false);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x000080F9 File Offset: 0x000062F9
		private void SidebarElement_MouseLeave(object sender, MouseEventArgs e)
		{
			this.SetColor(false);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00008110 File Offset: 0x00006310
		private void SidebarElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.SetColor(true);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00008119 File Offset: 0x00006319
		private void SidebarElement_IsEnabledChanged(object _, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				base.Opacity = 1.0;
				return;
			}
			base.Opacity = 0.5;
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x000080F9 File Offset: 0x000062F9
		private void SidebarElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			this.SetColor(false);
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00036308 File Offset: 0x00034508
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/sidebarelement.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00036338 File Offset: 0x00034538
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
				this.mSidebarElement = (SidebarElement)target;
				this.mSidebarElement.MouseEnter += this.SidebarElement_MouseEnter;
				this.mSidebarElement.MouseLeave += this.SidebarElement_MouseLeave;
				this.mSidebarElement.PreviewMouseDown += this.SidebarElement_PreviewMouseDown;
				this.mSidebarElement.PreviewMouseUp += this.SidebarElement_PreviewMouseUp;
				this.mSidebarElement.Loaded += this.SidebarElement_Loaded;
				this.mSidebarElement.IsEnabledChanged += this.SidebarElement_IsEnabledChanged;
				return;
			case 2:
				this.mBorder = (Border)target;
				return;
			case 3:
				this.mGrid = (Grid)target;
				return;
			case 4:
				this.mImage = (CustomPictureBox)target;
				this.mImage.Loaded += this.MImage_Loaded;
				return;
			case 5:
				this.mElementNotification = (Ellipse)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000579 RID: 1401
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SidebarElement mSidebarElement;

		// Token: 0x0400057A RID: 1402
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBorder;

		// Token: 0x0400057B RID: 1403
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x0400057C RID: 1404
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mImage;

		// Token: 0x0400057D RID: 1405
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Ellipse mElementNotification;

		// Token: 0x0400057E RID: 1406
		private bool _contentLoaded;
	}
}
