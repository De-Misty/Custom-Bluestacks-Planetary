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
	// Token: 0x020001A8 RID: 424
	public class NoInternetControl : UserControl, IComponentConnector
	{
		// Token: 0x060010F2 RID: 4338 RVA: 0x0000C20F File Offset: 0x0000A40F
		public NoInternetControl(BrowserControl browserControl)
		{
			this.InitializeComponent();
			this.AssociatedControl = browserControl;
			BlueStacksUIBinding.Bind(this.mFailureTextBox, "STRING_NAVIGATE_FAILED", "");
			BlueStacksUIBinding.Bind(this.mBlueButton, "STRING_RETRY_CONNECTION_ISSUE_TEXT1");
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x0000C249 File Offset: 0x0000A449
		private void mBlueButton_Click(object sender, RoutedEventArgs e)
		{
			this.AssociatedControl.NavigateTo(this.AssociatedControl.mFailedUrl);
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x0006A548 File Offset: 0x00068748
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/nointernetcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0006A578 File Offset: 0x00068778
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
				this.mFailureTextBox = (TextBlock)target;
				return;
			case 2:
				this.mErrorLine1 = (TextBlock)target;
				return;
			case 3:
				this.mErrorLine2 = (TextBlock)target;
				return;
			case 4:
				this.mBlueButton = (CustomButton)target;
				this.mBlueButton.Click += this.mBlueButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000AD4 RID: 2772
		private BrowserControl AssociatedControl;

		// Token: 0x04000AD5 RID: 2773
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mFailureTextBox;

		// Token: 0x04000AD6 RID: 2774
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mErrorLine1;

		// Token: 0x04000AD7 RID: 2775
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mErrorLine2;

		// Token: 0x04000AD8 RID: 2776
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mBlueButton;

		// Token: 0x04000AD9 RID: 2777
		private bool _contentLoaded;
	}
}
