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
	// Token: 0x020001BA RID: 442
	public class UpdatePrompt : UserControl, IComponentConnector
	{
		// Token: 0x0600118C RID: 4492 RVA: 0x0006E638 File Offset: 0x0006C838
		internal UpdatePrompt(BlueStacksUpdateData bstUpdateData)
		{
			this.InitializeComponent();
			this.mBstUpdateData = bstUpdateData;
			if (string.IsNullOrEmpty(bstUpdateData.EngineVersion))
			{
				this.mLabelVersion.Content = "";
				this.mLabelVersion.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.mLabelVersion.Content = "v" + bstUpdateData.EngineVersion;
			}
			BlueStacksUIBinding.Bind(this.titleLabel, "STRING_BLUESTACKS_UPDATE_AVAILABLE");
			BlueStacksUIBinding.Bind(this.bodyLabel, "STRING_UPDATE_AVAILABLE");
			BlueStacksUIBinding.Bind(this.mDownloadNewButton, "STRING_DOWNLOAD_UPDATE");
			this.mCloseBtn.Visibility = Visibility.Visible;
			this.mDetailedChangeLogs.NavigateUri = new Uri(bstUpdateData.DetailedChangeLogsUrl);
			this.mDetailedChangeLogs.Inlines.Clear();
			this.mDetailedChangeLogs.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x0000C7EA File Offset: 0x0000A9EA
		private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked UpdateNow Menu Close button");
			RegistryManager.Instance.LastUpdateSkippedVersion = this.mBstUpdateData.EngineVersion;
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopupCross, "");
			BlueStacksUIUtils.CloseContainerWindow(this);
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x0000C820 File Offset: 0x0000AA20
		private void DownloadNowButton_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("Clicked Download_Now button");
			ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopupDwnld, "");
			BlueStacksUpdater.DownloadNow(this.mBstUpdateData, false);
			BlueStacksUIUtils.CloseContainerWindow(this);
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x0000C84D File Offset: 0x0000AA4D
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
			e.Handled = true;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x0006E720 File Offset: 0x0006C920
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/updateprompt.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x0006E750 File Offset: 0x0006C950
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
				this.titleLabel = (Label)target;
				return;
			case 2:
				this.mCloseBtn = (CustomPictureBox)target;
				this.mCloseBtn.MouseLeftButtonUp += this.CloseBtn_MouseLeftButtonUp;
				return;
			case 3:
				this.bodyLabel = (Label)target;
				return;
			case 4:
				this.mLabelVersion = (Label)target;
				return;
			case 5:
				this.mDetailedChangeLogs = (Hyperlink)target;
				this.mDetailedChangeLogs.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			case 6:
				this.mDownloadNewButton = (CustomButton)target;
				this.mDownloadNewButton.Click += this.DownloadNowButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000B7E RID: 2942
		private BlueStacksUpdateData mBstUpdateData;

		// Token: 0x04000B7F RID: 2943
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label titleLabel;

		// Token: 0x04000B80 RID: 2944
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseBtn;

		// Token: 0x04000B81 RID: 2945
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label bodyLabel;

		// Token: 0x04000B82 RID: 2946
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mLabelVersion;

		// Token: 0x04000B83 RID: 2947
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mDetailedChangeLogs;

		// Token: 0x04000B84 RID: 2948
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mDownloadNewButton;

		// Token: 0x04000B85 RID: 2949
		private bool _contentLoaded;
	}
}
