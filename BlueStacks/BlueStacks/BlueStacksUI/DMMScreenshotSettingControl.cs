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
	// Token: 0x02000128 RID: 296
	public class DMMScreenshotSettingControl : UserControl, IComponentConnector
	{
		// Token: 0x06000BEC RID: 3052 RVA: 0x000097A2 File Offset: 0x000079A2
		public DMMScreenshotSettingControl(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			base.Visibility = Visibility.Hidden;
			this.mChooseFolderTextBlock.Text = RegistryManager.Instance.ScreenShotsPath;
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x000097D3 File Offset: 0x000079D3
		private void ChooseScreenshotFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.DMMScreenshotHandler();
			this.mChooseFolderTextBlock.Text = RegistryManager.Instance.ScreenShotsPath;
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00042C04 File Offset: 0x00040E04
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/dmmscreenshotsettingcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x000097FA File Offset: 0x000079FA
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
				this.mChooseFolderTextBlock = (TextBox)target;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			((Grid)target).PreviewMouseLeftButtonUp += this.ChooseScreenshotFolder_MouseLeftButtonUp;
		}

		// Token: 0x04000743 RID: 1859
		private MainWindow ParentWindow;

		// Token: 0x04000744 RID: 1860
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBox mChooseFolderTextBlock;

		// Token: 0x04000745 RID: 1861
		private bool _contentLoaded;
	}
}
