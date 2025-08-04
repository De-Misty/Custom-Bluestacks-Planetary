using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000077 RID: 119
	public class SkillIconToolTipPopup : CustomPopUp, IComponentConnector
	{
		// Token: 0x060005CC RID: 1484 RVA: 0x00005E99 File Offset: 0x00004099
		public SkillIconToolTipPopup(CanvasElement canvasElement)
		{
			this.InitializeComponent();
			base.PlacementTarget = ((canvasElement != null) ? canvasElement.mSkillImage : null);
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0002186C File Offset: 0x0001FA6C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/skillicontooltippopup.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x0002189C File Offset: 0x0001FA9C
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
				this.mMaskBorder1 = (Border)target;
				return;
			case 2:
				this.mSkillIconHeaderText = (TextBlock)target;
				return;
			case 3:
				this.mDownArrow = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000315 RID: 789
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder1;

		// Token: 0x04000316 RID: 790
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSkillIconHeaderText;

		// Token: 0x04000317 RID: 791
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path mDownArrow;

		// Token: 0x04000318 RID: 792
		private bool _contentLoaded;
	}
}
