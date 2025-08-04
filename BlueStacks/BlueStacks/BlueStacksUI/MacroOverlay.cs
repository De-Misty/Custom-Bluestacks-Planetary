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
	// Token: 0x020000EE RID: 238
	public class MacroOverlay : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060009E0 RID: 2528 RVA: 0x000083C6 File Offset: 0x000065C6
		// (set) Token: 0x060009E1 RID: 2529 RVA: 0x000083CE File Offset: 0x000065CE
		public MainWindow ParentWindow { get; set; }

		// Token: 0x060009E2 RID: 2530 RVA: 0x000083D7 File Offset: 0x000065D7
		public MacroOverlay()
		{
			this.InitializeComponent();
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x000083E5 File Offset: 0x000065E5
		public MacroOverlay(MainWindow mainWindow)
		{
			this.ParentWindow = mainWindow;
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x000083F4 File Offset: 0x000065F4
		private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.HideOverlay();
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("abortReroll", null);
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x00008412 File Offset: 0x00006612
		private void HideOverlay()
		{
			this.ParentWindow.HideDimOverlay();
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00037504 File Offset: 0x00035704
		internal void ShowPromptAndHideOverlay()
		{
			if (base.Visibility == Visibility.Visible)
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_REROLL_COMPLETED", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_REROLL_COMPLETED_SUCCESS", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
				this.HideOverlay();
			}
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00007861 File Offset: 0x00005A61
		bool IDimOverlayControl.Close()
		{
			base.Visibility = Visibility.Hidden;
			return true;
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x060009E8 RID: 2536 RVA: 0x0000841F File Offset: 0x0000661F
		// (set) Token: 0x060009E9 RID: 2537 RVA: 0x00008427 File Offset: 0x00006627
		bool IDimOverlayControl.IsCloseOnOverLayClick
		{
			get
			{
				return this.mIsCloseOnOverLayClick;
			}
			set
			{
				this.mIsCloseOnOverLayClick = value;
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060009EA RID: 2538 RVA: 0x00008430 File Offset: 0x00006630
		// (set) Token: 0x060009EB RID: 2539 RVA: 0x00008438 File Offset: 0x00006638
		public bool ShowControlInSeparateWindow { get; set; }

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060009EC RID: 2540 RVA: 0x00008441 File Offset: 0x00006641
		// (set) Token: 0x060009ED RID: 2541 RVA: 0x00008449 File Offset: 0x00006649
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x060009EE RID: 2542 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0003758C File Offset: 0x0003578C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrooverlay.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x00008452 File Offset: 0x00006652
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((CustomPictureBox)target).PreviewMouseLeftButtonUp += this.CloseButton_PreviewMouseLeftButtonUp;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x040005B7 RID: 1463
		private bool mIsCloseOnOverLayClick;

		// Token: 0x040005BA RID: 1466
		private bool _contentLoaded;
	}
}
