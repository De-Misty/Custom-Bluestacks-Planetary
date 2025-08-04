using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000041 RID: 65
	public class BlurbMessageControl : UserControl, IComponentConnector
	{
		// Token: 0x060003C6 RID: 966 RVA: 0x00004775 File Offset: 0x00002975
		public BlurbMessageControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00019F98 File Offset: 0x00018198
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/blurbmessagecontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00019FC8 File Offset: 0x000181C8
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
				this.FirstMessage = (Run)target;
				return;
			case 2:
				this.KeyMessage = (TextBlock)target;
				return;
			case 3:
				this.SecondMessage = (Run)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040001F6 RID: 502
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Run FirstMessage;

		// Token: 0x040001F7 RID: 503
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock KeyMessage;

		// Token: 0x040001F8 RID: 504
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Run SecondMessage;

		// Token: 0x040001F9 RID: 505
		private bool _contentLoaded;
	}
}
