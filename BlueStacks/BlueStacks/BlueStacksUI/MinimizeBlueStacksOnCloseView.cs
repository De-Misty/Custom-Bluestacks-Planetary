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
	// Token: 0x0200006E RID: 110
	public class MinimizeBlueStacksOnCloseView : UserControl, IComponentConnector
	{
		// Token: 0x0600056B RID: 1387 RVA: 0x00005A73 File Offset: 0x00003C73
		public MinimizeBlueStacksOnCloseView(MainWindow window)
		{
			this.InitializeComponent();
			base.DataContext = new MinimizeBlueStacksOnCloseViewModel(window);
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0002081C File Offset: 0x0001EA1C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/minimizebluestacksonclose/view/minimizebluestacksoncloseview.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x0002084C File Offset: 0x0001EA4C
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
				this.mTitleGrid = (Grid)target;
				return;
			case 2:
				this.mCrossButtonPictureBox = (CustomPictureBox)target;
				return;
			case 3:
				this.mTitleText = (TextBlock)target;
				return;
			case 4:
				this.mHeaderText = (TextBlock)target;
				return;
			case 5:
				this.mMinimizeRadioBtn = (CustomRadioButton)target;
				return;
			case 6:
				this.mMinimizeBtnBodyText = (TextBlock)target;
				return;
			case 7:
				this.mQuitRadioBtn = (CustomRadioButton)target;
				return;
			case 8:
				this.mQuitBtnBodyText = (TextBlock)target;
				return;
			case 9:
				this.mDoNotShowChkBox = (CustomCheckbox)target;
				return;
			case 10:
				this.mBtnActionPanel = (StackPanel)target;
				return;
			case 11:
				this.mCancelBtn = (CustomButton)target;
				return;
			case 12:
				this.mMinimizeBtn = (CustomButton)target;
				return;
			case 13:
				this.mQuitBtn = (CustomButton)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040002C6 RID: 710
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTitleGrid;

		// Token: 0x040002C7 RID: 711
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCrossButtonPictureBox;

		// Token: 0x040002C8 RID: 712
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTitleText;

		// Token: 0x040002C9 RID: 713
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mHeaderText;

		// Token: 0x040002CA RID: 714
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mMinimizeRadioBtn;

		// Token: 0x040002CB RID: 715
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMinimizeBtnBodyText;

		// Token: 0x040002CC RID: 716
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mQuitRadioBtn;

		// Token: 0x040002CD RID: 717
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mQuitBtnBodyText;

		// Token: 0x040002CE RID: 718
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mDoNotShowChkBox;

		// Token: 0x040002CF RID: 719
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mBtnActionPanel;

		// Token: 0x040002D0 RID: 720
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mCancelBtn;

		// Token: 0x040002D1 RID: 721
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mMinimizeBtn;

		// Token: 0x040002D2 RID: 722
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mQuitBtn;

		// Token: 0x040002D3 RID: 723
		private bool _contentLoaded;
	}
}
