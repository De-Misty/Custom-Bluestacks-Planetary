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
	// Token: 0x02000166 RID: 358
	public class OverlayLabelControl : UserControl, IComponentConnector
	{
		// Token: 0x06000EC5 RID: 3781 RVA: 0x0000AF83 File Offset: 0x00009183
		public OverlayLabelControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0005DDA4 File Offset: 0x0005BFA4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/overlaylabelcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x0005DDD4 File Offset: 0x0005BFD4
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
				this.clipMask = (Border)target;
				return;
			case 3:
				this.mGrid = (Grid)target;
				return;
			case 4:
				this.lbl = (Label)target;
				return;
			case 5:
				this.img = (CustomPictureBox)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000970 RID: 2416
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBorder;

		// Token: 0x04000971 RID: 2417
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border clipMask;

		// Token: 0x04000972 RID: 2418
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x04000973 RID: 2419
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label lbl;

		// Token: 0x04000974 RID: 2420
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox img;

		// Token: 0x04000975 RID: 2421
		private bool _contentLoaded;
	}
}
