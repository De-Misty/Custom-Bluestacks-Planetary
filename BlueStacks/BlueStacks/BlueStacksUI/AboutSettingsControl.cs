using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001B2 RID: 434
	public class AboutSettingsControl : UserControl, IComponentConnector
	{
		// Token: 0x0600112C RID: 4396 RVA: 0x0006B4D0 File Offset: 0x000696D0
		public AboutSettingsControl(MainWindow window, SettingsWindow _)
		{
			this.ParentWindow = window;
			this.InitializeComponent();
			base.Visibility = Visibility.Hidden;
			base.Loaded += this.AboutSettingsControl_Loaded;
			this.mVersionLabel.Content = "Domination";
			this.mVersionLabel.FontSize = 24.0;
			this.mVersionLabel.Margin = new Thickness(0.0, -30.0, 0.0, 0.0);
			this.mSupportLabel.SetValue(ContentControl.ContentProperty, "Telegram chanel: ");
			this.mWebsiteLabel.SetValue(ContentControl.ContentProperty, "YouTube chanel: ");
			this.mSupportMailLabel.SetValue(ContentControl.ContentProperty, "Discord Server: ");
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x0006B5A0 File Offset: 0x000697A0
		private void AboutSettingsControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
			{
				this.ContactInfoGrid.Visibility = Visibility.Hidden;
			}
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				this.mPoweredByBSGrid.Visibility = Visibility.Visible;
				this.grid_0.Visibility = Visibility.Hidden;
			}
			this.mSupportEMailHyperlink.Inlines.Clear();
			this.mSupportEMailHyperlink.Inlines.Add("Click");
			this.mSupportEMailHyperlink.NavigateUri = new Uri("https://discord.gg/swKYX9duJG");
			this.mlink1.Inlines.Clear();
			this.mlink1.Inlines.Add("Click");
			this.mlink1.NavigateUri = new Uri("https://www.youtube.com/@D3M1ST1");
			this.mlink2.Inlines.Clear();
			this.mlink2.Inlines.Add("Click");
			this.mlink2.NavigateUri = new Uri("https://t.me/de_mistiyt");
			this.HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
			BlueStacksUpdater.StateChanged -= this.BlueStacksUpdater_StateChanged;
			BlueStacksUpdater.StateChanged += this.BlueStacksUpdater_StateChanged;
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0000C479 File Offset: 0x0000A679
		private void BlueStacksUpdater_StateChanged()
		{
			this.HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x00004786 File Offset: 0x00002986
		private void HandleUpdateStateGridVisibility(BlueStacksUpdater.UpdateState state)
		{
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x0000C486 File Offset: 0x0000A686
		private void ShowLatestVersionGrid()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mUpdateInfoGrid.Visibility = Visibility.Collapsed;
				this.mCheckUpdateBtn.Visibility = Visibility.Collapsed;
				this.mStatusLabel.Visibility = Visibility.Collapsed;
				BlueStacksUIBinding.Bind(this.mStatusLabel, "STRING_LATEST_VERSION", "");
				this.mCheckingGrid.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x0000C4A6 File Offset: 0x0000A6A6
		private void ShowInternetConnectionErrorGrid()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mUpdateInfoGrid.Visibility = Visibility.Collapsed;
				this.mCheckUpdateBtn.HorizontalAlignment = HorizontalAlignment.Right;
				this.mCheckUpdateBtn.Visibility = Visibility.Collapsed;
				BlueStacksUIBinding.Bind(this.mCheckUpdateBtn, "STRING_RETRY_CONNECTION_ISSUE_TEXT1");
				this.mStatusLabel.Visibility = Visibility.Visible;
				BlueStacksUIBinding.Bind(this.mStatusLabel, "STRING_POST_OTS_FAILED_WARNING_MESSAGE", "");
				this.mCheckingGrid.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00004786 File Offset: 0x00002986
		private void mCheckUpdateBtn_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00004786 File Offset: 0x00002986
		private void HandleCheckForUpdateResult(object sender, RunWorkerCompletedEventArgs e)
		{
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0000C4C6 File Offset: 0x0000A6C6
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
			e.Handled = true;
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x00004786 File Offset: 0x00002986
		private void mTermsOfUseLink_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x0006B6C8 File Offset: 0x000698C8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/aboutsettingscontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x0006B6F8 File Offset: 0x000698F8
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.mPoweredByBSGrid = (Grid)target;
				return;
			case 2:
			case 3:
				this.mProductTextGrid = (Grid)target;
				this.mPoweredByBSGrid.Visibility = Visibility.Hidden;
				return;
			case 4:
				this.mVersionLabel = (Label)target;
				return;
			case 5:
				this.mUpdateInfoGrid = (Grid)target;
				this.mUpdateInfoGrid.Visibility = Visibility.Collapsed;
				return;
			case 6:
				this.bodyLabel = (Label)target;
				this.bodyLabel.Visibility = Visibility.Collapsed;
				return;
			case 7:
				this.mLabelVersion = (Label)target;
				return;
			case 8:
				this.mDetailedChangeLogs = (Hyperlink)target;
				this.mDetailedChangeLogs.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			case 9:
				this.mCheckUpdateBtn = (CustomButton)target;
				this.mCheckUpdateBtn.Click += this.mCheckUpdateBtn_Click;
				this.mCheckUpdateBtn.Visibility = Visibility.Collapsed;
				return;
			case 10:
				this.mStatusLabel = (TextBlock)target;
				this.mStatusLabel.Visibility = Visibility.Collapsed;
				return;
			case 11:
				this.mCheckingGrid = (Grid)target;
				this.mCheckingGrid.Visibility = Visibility.Collapsed;
				return;
			case 12:
				this.ContactInfoGrid = (Grid)target;
				this.ContactInfoGrid.Margin = new Thickness(265.0, -145.0, 0.0, 0.0);
				return;
			case 13:
				this.mWebsiteLabel = (Label)target;
				return;
			case 14:
				((Hyperlink)target).RequestNavigate += this.Hyperlink_RequestNavigate;
				((Hyperlink)target).Inlines.Clear();
				((Hyperlink)target).Inlines.Add("text 1");
				this.mlink1 = (Hyperlink)target;
				return;
			case 15:
				this.mSupportLabel = (Label)target;
				return;
			case 16:
				((Hyperlink)target).RequestNavigate += this.Hyperlink_RequestNavigate;
				((Hyperlink)target).Inlines.Clear();
				((Hyperlink)target).Inlines.Add("text 2");
				this.mlink2 = (Hyperlink)target;
				return;
			case 17:
				this.mSupportMailLabel = (Label)target;
				return;
			case 18:
				this.mSupportEMailHyperlink = (Hyperlink)target;
				this.mSupportEMailHyperlink.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			case 19:
				this.mTermsOfUse = (TextBlock)target;
				this.mTermsOfUse.Visibility = Visibility.Collapsed;
				return;
			case 20:
				this.mTermsOfUseLink = (Hyperlink)target;
				this.mTermsOfUseLink.RequestNavigate += this.Hyperlink_RequestNavigate;
				this.mTermsOfUseLink.Loaded += this.mTermsOfUseLink_Loaded;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000B0C RID: 2828
		private MainWindow ParentWindow;

		// Token: 0x04000B0D RID: 2829
		internal Grid mPoweredByBSGrid;

		// Token: 0x04000B0E RID: 2830
		internal Grid mBSIconAndNameGrid;

		// Token: 0x04000B0F RID: 2831
		internal Grid mProductTextGrid;

		// Token: 0x04000B10 RID: 2832
		internal Label mVersionLabel;

		// Token: 0x04000B11 RID: 2833
		internal Grid mUpdateInfoGrid;

		// Token: 0x04000B12 RID: 2834
		internal Label bodyLabel;

		// Token: 0x04000B13 RID: 2835
		internal Label mLabelVersion;

		// Token: 0x04000B14 RID: 2836
		internal Hyperlink mDetailedChangeLogs;

		// Token: 0x04000B15 RID: 2837
		internal CustomButton mCheckUpdateBtn;

		// Token: 0x04000B16 RID: 2838
		internal TextBlock mStatusLabel;

		// Token: 0x04000B17 RID: 2839
		internal Grid mCheckingGrid;

		// Token: 0x04000B18 RID: 2840
		internal Grid ContactInfoGrid;

		// Token: 0x04000B19 RID: 2841
		internal Label mWebsiteLabel;

		// Token: 0x04000B1A RID: 2842
		internal Label mSupportLabel;

		// Token: 0x04000B1B RID: 2843
		internal Label mSupportMailLabel;

		// Token: 0x04000B1C RID: 2844
		internal Hyperlink mSupportEMailHyperlink;

		// Token: 0x04000B1D RID: 2845
		internal TextBlock mTermsOfUse;

		// Token: 0x04000B1E RID: 2846
		internal Hyperlink mTermsOfUseLink;

		// Token: 0x04000B1F RID: 2847
		private bool _contentLoaded;

		// Token: 0x04000B20 RID: 2848
		internal Grid grid_0;

		// Token: 0x04000B21 RID: 2849
		internal Hyperlink mlink1;

		// Token: 0x04000B22 RID: 2850
		internal Hyperlink mlink2;
	}
}
