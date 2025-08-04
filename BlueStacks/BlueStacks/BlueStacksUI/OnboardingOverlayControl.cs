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
	// Token: 0x02000071 RID: 113
	public class OnboardingOverlayControl : UserControl, IComponentConnector
	{
		// Token: 0x0600058D RID: 1421 RVA: 0x00005B7D File Offset: 0x00003D7D
		public OnboardingOverlayControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00020D88 File Offset: 0x0001EF88
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/onboardingoverlaycontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00020DB8 File Offset: 0x0001EFB8
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
				this.mBorder = (Border)target;
				return;
			case 2:
				this.mGrid = (Grid)target;
				return;
			case 3:
				this.mOnboardingImg = (CustomPictureBox)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040002E2 RID: 738
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBorder;

		// Token: 0x040002E3 RID: 739
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x040002E4 RID: 740
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mOnboardingImg;

		// Token: 0x040002E5 RID: 741
		private bool _contentLoaded;
	}
}
