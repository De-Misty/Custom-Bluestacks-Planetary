using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020002B8 RID: 696
	public class CustomScrollViewer : ScrollViewer, IComponentConnector
	{
		// Token: 0x060019AE RID: 6574 RVA: 0x00099020 File Offset: 0x00097220
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/customscrollviewer.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x000114CB File Offset: 0x0000F6CB
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			this._contentLoaded = true;
		}

		// Token: 0x04001038 RID: 4152
		private bool _contentLoaded;
	}
}
