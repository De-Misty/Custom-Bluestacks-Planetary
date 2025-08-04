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
	// Token: 0x02000165 RID: 357
	public class ImportSchemesWindowControl : UserControl, IComponentConnector
	{
		// Token: 0x06000EC0 RID: 3776 RVA: 0x0000AF49 File Offset: 0x00009149
		public ImportSchemesWindowControl(ImportSchemesWindow importSchemesWindow, MainWindow window)
		{
			this.InitializeComponent();
			this.mImportSchemesWindow = importSchemesWindow;
			this.ParentWindow = window;
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0000AF65 File Offset: 0x00009165
		private void box_Checked(object sender, RoutedEventArgs e)
		{
			this.mImportSchemesWindow.Box_Checked(sender, e);
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0000AF74 File Offset: 0x00009174
		private void box_Unchecked(object sender, RoutedEventArgs e)
		{
			this.mImportSchemesWindow.Box_Unchecked(sender, e);
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0005DCE4 File Offset: 0x0005BEE4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/importschemeswindowcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0005DD14 File Offset: 0x0005BF14
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
				this.mContent = (CustomCheckbox)target;
				this.mContent.Checked += this.box_Checked;
				this.mContent.Unchecked += this.box_Unchecked;
				return;
			case 2:
				this.mBlock = (Grid)target;
				return;
			case 3:
				this.mImportName = (CustomTextBox)target;
				return;
			case 4:
				this.mWarningMsg = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000969 RID: 2409
		internal ImportSchemesWindow mImportSchemesWindow;

		// Token: 0x0400096A RID: 2410
		internal MainWindow ParentWindow;

		// Token: 0x0400096B RID: 2411
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mContent;

		// Token: 0x0400096C RID: 2412
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBlock;

		// Token: 0x0400096D RID: 2413
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mImportName;

		// Token: 0x0400096E RID: 2414
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mWarningMsg;

		// Token: 0x0400096F RID: 2415
		private bool _contentLoaded;
	}
}
