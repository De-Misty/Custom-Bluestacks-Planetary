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
	// Token: 0x020000EA RID: 234
	public class Gl3CustomWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x060009C3 RID: 2499 RVA: 0x000082FD File Offset: 0x000064FD
		public Gl3CustomWindow(MainWindow parentWindow)
		{
			this.mParentWindow = parentWindow;
			this.InitializeComponent();
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00036D90 File Offset: 0x00034F90
		private void mGetButton_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("Clicked Restart to opengl button");
			if (RegistryManager.Instance.GLES3 && this.mParentWindow.EngineInstanceRegistry.GlRenderMode != 1)
			{
				this.mParentWindow.EngineInstanceRegistry.GlRenderMode = 1;
				BlueStacksUIUtils.RestartInstance(this.mParentWindow.mVmName);
				return;
			}
			base.Close();
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00008312 File Offset: 0x00006512
		private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked Gl3 custom window close button");
			base.Close();
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00036DF0 File Offset: 0x00034FF0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/gl3customwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x00036E20 File Offset: 0x00035020
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
				this.mMaskBorder = (Border)target;
				return;
			case 2:
				this.mParentGrid = (Grid)target;
				return;
			case 3:
				this.mTextBlockGrid = (Grid)target;
				return;
			case 4:
				this.mCustomMessageBoxCloseButton = (CustomPictureBox)target;
				this.mCustomMessageBoxCloseButton.PreviewMouseLeftButtonUp += this.Close_PreviewMouseLeftButtonUp;
				return;
			case 5:
				this.mTitleText = (TextBlock)target;
				return;
			case 6:
				this.mTitleIcon = (CustomPictureBox)target;
				return;
			case 7:
				this.mBodyTextBlock = (TextBlock)target;
				return;
			case 8:
				this.mHintGrid = (Grid)target;
				return;
			case 9:
				this.mHintTextBlock = (TextBlock)target;
				return;
			case 10:
				this.mHintGrid1 = (Grid)target;
				return;
			case 11:
				this.mHintTextBlock1 = (TextBlock)target;
				return;
			case 12:
				this.mButton = (CustomButton)target;
				this.mButton.Click += this.mGetButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000597 RID: 1431
		private MainWindow mParentWindow;

		// Token: 0x04000598 RID: 1432
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000599 RID: 1433
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mParentGrid;

		// Token: 0x0400059A RID: 1434
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTextBlockGrid;

		// Token: 0x0400059B RID: 1435
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCustomMessageBoxCloseButton;

		// Token: 0x0400059C RID: 1436
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTitleText;

		// Token: 0x0400059D RID: 1437
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTitleIcon;

		// Token: 0x0400059E RID: 1438
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mBodyTextBlock;

		// Token: 0x0400059F RID: 1439
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mHintGrid;

		// Token: 0x040005A0 RID: 1440
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mHintTextBlock;

		// Token: 0x040005A1 RID: 1441
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mHintGrid1;

		// Token: 0x040005A2 RID: 1442
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mHintTextBlock1;

		// Token: 0x040005A3 RID: 1443
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mButton;

		// Token: 0x040005A4 RID: 1444
		private bool _contentLoaded;
	}
}
