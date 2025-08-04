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
	// Token: 0x020000E9 RID: 233
	public class DMMProgressControl : UserControl, IComponentConnector
	{
		// Token: 0x060009C0 RID: 2496 RVA: 0x00036CFC File Offset: 0x00034EFC
		public DMMProgressControl()
		{
			this.InitializeComponent();
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				BlueStacksUIBinding.Bind(this.BootText, "STRING_BOOT_TIME", "");
				if (RegistryManager.Instance.LastBootTime / 400 <= 0)
				{
					RegistryManager.Instance.LastBootTime = 120000;
					RegistryManager.Instance.NoOfBootCompleted = 0;
				}
			}
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x00036D60 File Offset: 0x00034F60
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/dmmprogresscontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x000082E3 File Offset: 0x000064E3
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
				this.BootText = (TextBlock)target;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x04000595 RID: 1429
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock BootText;

		// Token: 0x04000596 RID: 1430
		private bool _contentLoaded;
	}
}
