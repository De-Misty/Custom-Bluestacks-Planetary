using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001DD RID: 477
	public class UpdateDownloadProgress : CustomWindow, IComponentConnector
	{
		// Token: 0x060012CF RID: 4815 RVA: 0x00072988 File Offset: 0x00070B88
		public UpdateDownloadProgress()
		{
			this.InitializeComponent();
			base.IsShowGLWindow = true;
			this.mDetailedChangeLogs.Inlines.Clear();
			this.mDetailedChangeLogs.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
			this.mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
		}

		// Token: 0x060012D0 RID: 4816 RVA: 0x0002CE5C File Offset: 0x0002B05C
		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!e.OriginalSource.GetType().Equals(typeof(CustomPictureBox)))
			{
				try
				{
					base.DragMove();
				}
				catch
				{
				}
			}
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x00006C1B File Offset: 0x00004E1B
		private void HideBtn_Click(object sender, RoutedEventArgs e)
		{
			base.Hide();
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x00006C1B File Offset: 0x00004E1B
		private void mCloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Hide();
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x0000C84D File Offset: 0x0000AA4D
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
			e.Handled = true;
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x000729F4 File Offset: 0x00070BF4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/updatedownloadprogress.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x00072A24 File Offset: 0x00070C24
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
				this.mUpdateDownloadProgressUserControl = (UpdateDownloadProgress)target;
				return;
			case 2:
				this.mMaskBorder = (Border)target;
				return;
			case 3:
				((Grid)target).MouseLeftButtonDown += this.Grid_MouseLeftButtonDown;
				return;
			case 4:
				this.titleLabel = (TextBlock)target;
				return;
			case 5:
				this.mCloseBtn = (CustomPictureBox)target;
				this.mCloseBtn.PreviewMouseLeftButtonUp += this.mCloseBtn_PreviewMouseLeftButtonUp;
				return;
			case 6:
				this.mDetailedChangeLogs = (Hyperlink)target;
				this.mDetailedChangeLogs.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			case 7:
				this.mUpdateDownloadProgressBar = (BlueProgressBar)target;
				return;
			case 8:
				this.mUpdateDownloadProgressPercentage = (Label)target;
				return;
			case 9:
				this.mHideBtn = (CustomButton)target;
				this.mHideBtn.Click += this.HideBtn_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000C1F RID: 3103
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal UpdateDownloadProgress mUpdateDownloadProgressUserControl;

		// Token: 0x04000C20 RID: 3104
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000C21 RID: 3105
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock titleLabel;

		// Token: 0x04000C22 RID: 3106
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseBtn;

		// Token: 0x04000C23 RID: 3107
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mDetailedChangeLogs;

		// Token: 0x04000C24 RID: 3108
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal BlueProgressBar mUpdateDownloadProgressBar;

		// Token: 0x04000C25 RID: 3109
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mUpdateDownloadProgressPercentage;

		// Token: 0x04000C26 RID: 3110
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mHideBtn;

		// Token: 0x04000C27 RID: 3111
		private bool _contentLoaded;
	}
}
