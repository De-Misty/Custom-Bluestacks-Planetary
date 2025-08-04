using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001B6 RID: 438
	public class NotificationsSettings : UserControl, IComponentConnector
	{
		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600115F RID: 4447 RVA: 0x0000C718 File Offset: 0x0000A918
		// (set) Token: 0x06001160 RID: 4448 RVA: 0x0000C71F File Offset: 0x0000A91F
		public static NotificationsSettings Instance { get; set; }

		// Token: 0x06001161 RID: 4449 RVA: 0x0006C340 File Offset: 0x0006A540
		public NotificationsSettings(MainWindow window)
		{
			NotificationsSettings.Instance = this;
			this.InitializeComponent();
			this.ParentWindow = window;
			base.Visibility = Visibility.Hidden;
			this.mVmName = ((window != null) ? window.mVmName : null);
			this.mNotificationModeToggleButton.BoolValue = RegistryManager.Instance.IsNotificationModeAlwaysOn;
			this.mNotificationSoundToggleButton.BoolValue = RegistryManager.Instance.IsNotificationSoundsActive;
			this.mRibbonNotificationsToggleButton.BoolValue = RegistryManager.Instance.IsShowRibbonNotification;
			this.mToastNotificationsToggleButton.BoolValue = RegistryManager.Instance.IsShowToastNotification;
			if (!string.Equals(this.mVmName, "Android", StringComparison.InvariantCultureIgnoreCase))
			{
				this.mNotificationModeSettingsSection.Visibility = Visibility.Collapsed;
				this.mMinimzeOnCloseCheckBox.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.mExpandededArrow.Visibility = Visibility.Collapsed;
				this.mNotifModeInfoGrid.Visibility = Visibility.Collapsed;
				this.mCollapsedArrow.Visibility = Visibility.Visible;
			}
			this.mScroll.ScrollChanged += BluestacksUIColor.ScrollBarScrollChanged;
			this.mMinimzeOnCloseCheckBox.IsChecked = new bool?(!this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose);
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0006C46C File Offset: 0x0006A66C
		private void NotificationSettings_Loaded(object sender, RoutedEventArgs e)
		{
			NotificationManager.Instance.ReloadNotificationDetails();
			List<AppInfo> list = new JsonParser(this.mVmName).GetAppList().ToList<AppInfo>();
			bool flag = true;
			foreach (AppInfo appInfo in list)
			{
				bool flag2 = !this.AddNotificationToggleButton(appInfo.Name, appInfo.Img, appInfo.Package);
				flag = flag && flag2;
			}
			if (list.Count > 0)
			{
				this.mAppSpecificNotificationsToggleButton.BoolValue = !flag;
			}
			else
			{
				this.mAppSpecificNotificationsToggleButton.BoolValue = true;
			}
			if (this.mAppSpecificNotificationsToggleButton.BoolValue)
			{
				this.mStackPanel.Visibility = Visibility.Visible;
				return;
			}
			this.mStackPanel.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0006C544 File Offset: 0x0006A744
		private bool AddNotificationToggleButton(string name, string imageName, string package)
		{
			string text = global::System.IO.Path.Combine(RegistryStrings.GadgetDir, imageName);
			AppNotificationsToggleButton appNotificationsToggleButton = new AppNotificationsToggleButton(this.ParentWindow, name, text, package)
			{
				Margin = new Thickness(0.0, 0.0, 0.0, 12.0)
			};
			this.mStackPanel.Children.Add(appNotificationsToggleButton);
			return appNotificationsToggleButton.mAppNotificationStatus.BoolValue;
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0006C5B8 File Offset: 0x0006A7B8
		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			InstanceRegistry engineInstanceRegistry = this.ParentWindow.EngineInstanceRegistry;
			bool? isChecked = this.mMinimzeOnCloseCheckBox.IsChecked;
			bool flag = true;
			engineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = !((isChecked.GetValueOrDefault() == flag) & (isChecked != null));
			Stats.SendCommonClientStatsAsync("notification_mode", "donotshow_checkbox", "Android", string.Empty, (!this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose).ToString(CultureInfo.InvariantCulture), "");
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0006C638 File Offset: 0x0006A838
		private void ReadMoreLinkMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("notification_mode", "readarticle", this.ParentWindow.mVmName, KMManager.sPackageName, "", "");
			Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=notification_mode_help");
			e.Handled = true;
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0006C6B0 File Offset: 0x0006A8B0
		private void mReadMoreSection_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mCollapsedArrow.Visibility == Visibility.Visible)
			{
				this.mCollapsedArrow.Visibility = Visibility.Collapsed;
				this.mExpandededArrow.Visibility = Visibility.Visible;
				this.mNotifModeInfoGrid.Visibility = Visibility.Visible;
				return;
			}
			this.mExpandededArrow.Visibility = Visibility.Collapsed;
			this.mNotifModeInfoGrid.Visibility = Visibility.Collapsed;
			this.mCollapsedArrow.Visibility = Visibility.Visible;
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x0006C714 File Offset: 0x0006A914
		private void mNotificationModeToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryManager.Instance.IsNotificationModeAlwaysOn = !this.mNotificationModeToggleButton.BoolValue;
			Stats.SendCommonClientStatsAsync("notification_mode", RegistryManager.Instance.IsNotificationModeAlwaysOn ? "toggle_on" : "toggle_off", "Android", "", "", "");
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0006C770 File Offset: 0x0006A970
		private void mNotificationSoundToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryManager.Instance.IsNotificationSoundsActive = !this.mNotificationSoundToggleButton.BoolValue;
			Stats.SendCommonClientStatsAsync("notification_mode", "notification_sound_toggle", "Android", string.Empty, RegistryManager.Instance.IsNotificationSoundsActive.ToString(CultureInfo.InvariantCulture), "");
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x0006C7CC File Offset: 0x0006A9CC
		private void mToastNotificationsToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryManager.Instance.IsShowToastNotification = !this.mToastNotificationsToggleButton.BoolValue;
			Stats.SendCommonClientStatsAsync("notification_mode", "toast_notification_toggle", "Android", string.Empty, RegistryManager.Instance.IsShowToastNotification.ToString(CultureInfo.InvariantCulture), "");
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x0006C828 File Offset: 0x0006AA28
		private void mRibbonNotificationsToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryManager.Instance.IsShowRibbonNotification = !this.mRibbonNotificationsToggleButton.BoolValue;
			Stats.SendCommonClientStatsAsync("notification_mode", "ribbon_notification_toggle", "Android", string.Empty, RegistryManager.Instance.IsShowRibbonNotification.ToString(CultureInfo.InvariantCulture), "");
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x0006C884 File Offset: 0x0006AA84
		private void mAppSpecificNotificationsToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("notification_mode", "all_apps_notifications_muted_toggle", "Android", string.Empty, (!this.mAppSpecificNotificationsToggleButton.BoolValue).ToString(CultureInfo.InvariantCulture), "");
			if (!this.mAppSpecificNotificationsToggleButton.BoolValue)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, NotificationManager.Instance.ShowNotificationText);
				this.mStackPanel.Visibility = Visibility.Visible;
			}
			else
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, NotificationManager.Instance.ShowNotificationText);
				this.mStackPanel.Visibility = Visibility.Collapsed;
			}
			foreach (object obj in this.mStackPanel.Children)
			{
				AppNotificationsToggleButton appNotificationsToggleButton = (AppNotificationsToggleButton)obj;
				appNotificationsToggleButton.mAppNotificationStatus.BoolValue = !this.mAppSpecificNotificationsToggleButton.BoolValue;
				if (appNotificationsToggleButton.mAppNotificationStatus.BoolValue)
				{
					NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, appNotificationsToggleButton.mAppTitle.Text);
				}
				else
				{
					NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, appNotificationsToggleButton.mAppTitle.Text);
				}
			}
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x0000C727 File Offset: 0x0000A927
		private void mRibbonHelp_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mRibbonPopup.IsOpen = true;
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x0000C735 File Offset: 0x0000A935
		private void mRibbonHelp_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mRibbonPopup.IsOpen = false;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x0000C743 File Offset: 0x0000A943
		private void mToastHelp_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mToastPopup.IsOpen = true;
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0000C751 File Offset: 0x0000A951
		private void mToastHelp_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mToastPopup.IsOpen = false;
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x0006C9BC File Offset: 0x0006ABBC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/notificationssettings.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x0006C9EC File Offset: 0x0006ABEC
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
				((NotificationsSettings)target).Loaded += this.NotificationSettings_Loaded;
				return;
			case 2:
				this.mScroll = (ScrollViewer)target;
				return;
			case 3:
				this.mNotificationModeSettingsSection = (Grid)target;
				return;
			case 4:
				this.mMinimzeOnCloseCheckBox = (CustomCheckbox)target;
				this.mMinimzeOnCloseCheckBox.Click += this.CheckBox_Click;
				return;
			case 5:
				this.mReadMoreSection = (Label)target;
				this.mReadMoreSection.MouseLeftButtonUp += this.mReadMoreSection_MouseLeftButtonUp;
				return;
			case 6:
				this.mCollapsedArrow = (global::System.Windows.Shapes.Path)target;
				return;
			case 7:
				this.mExpandededArrow = (global::System.Windows.Shapes.Path)target;
				return;
			case 8:
				this.mNotificationModeToggleButton = (CustomToggleButtonWithState)target;
				this.mNotificationModeToggleButton.PreviewMouseLeftButtonUp += this.mNotificationModeToggleButton_PreviewMouseLeftButtonUp;
				return;
			case 9:
				this.mNotifModeInfoGrid = (Border)target;
				return;
			case 10:
				((TextBlock)target).MouseLeftButtonDown += this.ReadMoreLinkMouseLeftButtonUp;
				return;
			case 11:
				this.mNotificationSoundToggleButton = (CustomToggleButtonWithState)target;
				this.mNotificationSoundToggleButton.PreviewMouseLeftButtonUp += this.mNotificationSoundToggleButton_PreviewMouseLeftButtonUp;
				return;
			case 12:
				this.mRibbonHelp = (CustomPictureBox)target;
				this.mRibbonHelp.MouseEnter += this.mRibbonHelp_MouseEnter;
				this.mRibbonHelp.MouseLeave += this.mRibbonHelp_MouseLeave;
				return;
			case 13:
				this.mRibbonNotificationsToggleButton = (CustomToggleButtonWithState)target;
				this.mRibbonNotificationsToggleButton.PreviewMouseLeftButtonUp += this.mRibbonNotificationsToggleButton_PreviewMouseLeftButtonUp;
				return;
			case 14:
				this.mRibbonPopup = (CustomPopUp)target;
				return;
			case 15:
				this.mToastHelp = (CustomPictureBox)target;
				this.mToastHelp.MouseEnter += this.mToastHelp_MouseEnter;
				this.mToastHelp.MouseLeave += this.mToastHelp_MouseLeave;
				return;
			case 16:
				this.mToastNotificationsToggleButton = (CustomToggleButtonWithState)target;
				this.mToastNotificationsToggleButton.PreviewMouseLeftButtonUp += this.mToastNotificationsToggleButton_PreviewMouseLeftButtonUp;
				return;
			case 17:
				this.mToastPopup = (CustomPopUp)target;
				return;
			case 18:
				this.mAppSpecificNotificationsToggleButton = (CustomToggleButtonWithState)target;
				this.mAppSpecificNotificationsToggleButton.PreviewMouseLeftButtonUp += this.mAppSpecificNotificationsToggleButton_PreviewMouseLeftButtonUp;
				return;
			case 19:
				this.mStackPanel = (StackPanel)target;
				return;
			case 20:
				this.mInfoIcon = (CustomPictureBox)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000B37 RID: 2871
		private MainWindow ParentWindow;

		// Token: 0x04000B38 RID: 2872
		private string mVmName = "Android";

		// Token: 0x04000B39 RID: 2873
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mScroll;

		// Token: 0x04000B3A RID: 2874
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mNotificationModeSettingsSection;

		// Token: 0x04000B3B RID: 2875
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mMinimzeOnCloseCheckBox;

		// Token: 0x04000B3C RID: 2876
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mReadMoreSection;

		// Token: 0x04000B3D RID: 2877
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path mCollapsedArrow;

		// Token: 0x04000B3E RID: 2878
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path mExpandededArrow;

		// Token: 0x04000B3F RID: 2879
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomToggleButtonWithState mNotificationModeToggleButton;

		// Token: 0x04000B40 RID: 2880
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mNotifModeInfoGrid;

		// Token: 0x04000B41 RID: 2881
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomToggleButtonWithState mNotificationSoundToggleButton;

		// Token: 0x04000B42 RID: 2882
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mRibbonHelp;

		// Token: 0x04000B43 RID: 2883
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomToggleButtonWithState mRibbonNotificationsToggleButton;

		// Token: 0x04000B44 RID: 2884
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mRibbonPopup;

		// Token: 0x04000B45 RID: 2885
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mToastHelp;

		// Token: 0x04000B46 RID: 2886
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomToggleButtonWithState mToastNotificationsToggleButton;

		// Token: 0x04000B47 RID: 2887
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mToastPopup;

		// Token: 0x04000B48 RID: 2888
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomToggleButtonWithState mAppSpecificNotificationsToggleButton;

		// Token: 0x04000B49 RID: 2889
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mStackPanel;

		// Token: 0x04000B4A RID: 2890
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mInfoIcon;

		// Token: 0x04000B4B RID: 2891
		private bool _contentLoaded;
	}
}
