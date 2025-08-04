using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000F1 RID: 241
	public class NotificationDrawerItem : UserControl, IComponentConnector
	{
		// Token: 0x06000A10 RID: 2576 RVA: 0x000085D1 File Offset: 0x000067D1
		public NotificationDrawerItem()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000A11 RID: 2577 RVA: 0x000085DF File Offset: 0x000067DF
		// (set) Token: 0x06000A12 RID: 2578 RVA: 0x000085E7 File Offset: 0x000067E7
		public string PackageName { get; set; }

		// Token: 0x06000A13 RID: 2579 RVA: 0x00037FFC File Offset: 0x000361FC
		internal void InitFromGenricNotificationItem(GenericNotificationItem item, MainWindow parentWin)
		{
			this.ParentWindow = parentWin;
			this.Id = item.Id;
			this.PackageName = item.Package;
			this.titleText.Text = item.Title;
			this.messageText.Text = item.Message;
			if (!item.IsRead)
			{
				this.ChangeToUnreadBackground();
			}
			else
			{
				this.ChangeToReadBackground();
			}
			if (string.Equals(item.Title, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
			{
				this.mSnoozeBtn.IsEnabled = false;
				this.mSnoozeBtn.Opacity = 0.5;
			}
			if (!string.IsNullOrEmpty(item.NotificationMenuImageName) && !string.IsNullOrEmpty(item.NotificationMenuImageUrl) && !File.Exists(Path.Combine(RegistryStrings.PromotionDirectory, item.NotificationMenuImageName)))
			{
				item.NotificationMenuImageName = Utils.TinyDownloader(item.NotificationMenuImageUrl, item.NotificationMenuImageName, RegistryStrings.PromotionDirectory, false);
			}
			this.icon.ImageName = item.NotificationMenuImageName;
			this.dateText.Text = DateTimeHelper.GetReadableDateTimeString(item.CreationTime);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00038108 File Offset: 0x00036308
		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			string text = RegistryStrings.InstallDir + "\\HD-RunApp.exe";
			GenericNotificationItem notificationItem = GenericNotificationManager.Instance.GetNotificationItem(this.Id);
			JsonParser jsonParser = new JsonParser(this.ParentWindow.mVmName);
			if (this.ParentWindow != null && this.ParentWindow.mGuestBootCompleted)
			{
				if (notificationItem == null)
				{
					return;
				}
				ClientStats.SendMiscellaneousStatsAsync("NotificationDrawerItemClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, notificationItem.Id, notificationItem.Title, JsonConvert.SerializeObject(notificationItem.ExtraPayload), notificationItem.ExtraPayload.ContainsKey("campaign_id") ? notificationItem.ExtraPayload["campaign_id"] : "", null, null);
				List<string> list = new List<string>();
				list.Add(notificationItem.Id);
				GenericNotificationManager.MarkNotification(list, delegate(GenericNotificationItem x)
				{
					x.IsRead = true;
				});
				this.ChangeToReadBackground();
				this.ParentWindow.mTopBar.RefreshNotificationCentreButton();
				if (notificationItem.ExtraPayload.Keys.Count > 0)
				{
					this.ParentWindow.Utils.HandleGenericActionFromDictionary(notificationItem.ExtraPayload, "notification_drawer", notificationItem.NotificationMenuImageName);
					return;
				}
				try
				{
					if (string.Compare(notificationItem.Title, "Successfully copied files:", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(notificationItem.Title, "Cannot copy files:", StringComparison.OrdinalIgnoreCase) == 0)
					{
						NotificationPopup.LaunchExplorer(notificationItem.Message);
						return;
					}
					Logger.Info("launching " + notificationItem.Title);
					AppInfo appInfoFromPackageName = jsonParser.GetAppInfoFromPackageName(this.PackageName);
					if (appInfoFromPackageName != null)
					{
						JObject jobject = new JObject
						{
							{ "app_icon_url", "" },
							{ "app_name", appInfoFromPackageName.Name },
							{ "app_url", "" },
							{ "app_pkg", this.PackageName }
						};
						string text2 = "-json \"" + jobject.ToString(Formatting.None, new JsonConverter[0]).Replace("\"", "\\\"") + "\"";
						Process.Start(text, string.Format(CultureInfo.InvariantCulture, "{0} -vmname {1}", new object[]
						{
							text2,
							this.ParentWindow.mVmName
						}));
					}
					else
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("click_generic_action", GenericAction.InstallPlay.ToString());
						dictionary.Add("click_action_packagename", notificationItem.Package);
						this.ParentWindow.Utils.HandleGenericActionFromDictionary(dictionary, "notification_drawer", "");
					}
					return;
				}
				catch (Exception ex)
				{
					Logger.Error(ex.ToString());
					return;
				}
				finally
				{
					this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen = false;
				}
			}
			if (notificationItem != null)
			{
				this.ParentWindow.mPostBootNotificationAction = this.PackageName;
				this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen = false;
			}
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x000085F0 File Offset: 0x000067F0
		internal void ChangeToUnreadBackground()
		{
			base.Background = Brushes.Transparent;
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x000085FD File Offset: 0x000067FD
		internal void ChangeToReadBackground()
		{
			base.Opacity = 0.5;
			base.Background = Brushes.Transparent;
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00008619 File Offset: 0x00006819
		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mNotificationActions.Visibility = Visibility.Visible;
			BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00038438 File Offset: 0x00036638
		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.mMutePopup.IsOpen)
			{
				this.mNotificationActions.Visibility = Visibility.Collapsed;
				if (GenericNotificationManager.Instance.GetNotificationItem(this.Id) != null)
				{
					if (!GenericNotificationManager.Instance.GetNotificationItem(this.Id).IsRead)
					{
						this.ChangeToUnreadBackground();
						return;
					}
					this.ChangeToReadBackground();
				}
			}
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00008637 File Offset: 0x00006837
		private void mCloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
			e.Handled = true;
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x0000865B File Offset: 0x0000685B
		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundColor");
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00007C8F File Offset: 0x00005E8F
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00038494 File Offset: 0x00036694
		private void Lbl1Hour_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Hour, this.titleText.Text);
			string text;
			string text2;
			string text3;
			new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out text, out text2, out text3);
			Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + (sender as TextBlock).Text, "");
			this.mMutePopup.IsOpen = false;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[]
			{
				this.titleText.Text,
				(sender as TextBlock).Text
			});
			NotificationDrawer.SnoozeInfoGridTimer.Start();
			e.Handled = true;
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x000385B8 File Offset: 0x000367B8
		private void Lbl1Day_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Day, this.titleText.Text);
			string text;
			string text2;
			string text3;
			new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out text, out text2, out text3);
			Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + (sender as TextBlock).Text, "");
			this.mMutePopup.IsOpen = false;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[]
			{
				this.titleText.Text,
				(sender as TextBlock).Text
			});
			NotificationDrawer.SnoozeInfoGridTimer.Start();
			e.Handled = true;
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x000386DC File Offset: 0x000368DC
		private void Lbl1Week_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Week, this.titleText.Text);
			string text;
			string text2;
			string text3;
			new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out text, out text2, out text3);
			Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + (sender as TextBlock).Text, "");
			this.mMutePopup.IsOpen = false;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[]
			{
				this.titleText.Text,
				(sender as TextBlock).Text
			});
			NotificationDrawer.SnoozeInfoGridTimer.Start();
			e.Handled = true;
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00038800 File Offset: 0x00036A00
		private void LblForever_MouseUp(object sender, MouseButtonEventArgs e)
		{
			NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.titleText.Text);
			string text;
			string text2;
			string text3;
			new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out text, out text2, out text3);
			Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", "Android", text, "Muted_" + (sender as TextBlock).Text, "");
			this.mMutePopup.IsOpen = false;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
			this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), new object[]
			{
				this.titleText.Text,
				(sender as TextBlock).Text
			});
			NotificationDrawer.SnoozeInfoGridTimer.Start();
			e.Handled = true;
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x00008672 File Offset: 0x00006872
		private void mSnoozeBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mMutePopup.IsOpen = !this.mMutePopup.IsOpen;
			e.Handled = true;
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x00038924 File Offset: 0x00036B24
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/genericnotification/notificationdraweritem.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00038954 File Offset: 0x00036B54
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
				((NotificationDrawerItem)target).MouseLeftButtonUp += this.UserControl_MouseLeftButtonUp;
				((NotificationDrawerItem)target).MouseEnter += this.UserControl_MouseEnter;
				((NotificationDrawerItem)target).MouseLeave += this.UserControl_MouseLeave;
				return;
			case 2:
				this.icon = (CustomPictureBox)target;
				return;
			case 3:
				this.titleText = (TextBlock)target;
				return;
			case 4:
				this.dateText = (TextBlock)target;
				return;
			case 5:
				this.mNotificationActions = (Grid)target;
				return;
			case 6:
				this.mSnoozeBtn = (CustomPictureBox)target;
				this.mSnoozeBtn.MouseLeftButtonUp += this.mSnoozeBtn_MouseLeftButtonUp;
				return;
			case 7:
				this.mMutePopup = (CustomPopUp)target;
				return;
			case 8:
				((Grid)target).MouseEnter += this.Grid_MouseEnter;
				((Grid)target).MouseLeave += this.Grid_MouseLeave;
				return;
			case 9:
				this.mLbl1Hour = (TextBlock)target;
				this.mLbl1Hour.MouseUp += this.Lbl1Hour_MouseUp;
				return;
			case 10:
				((Grid)target).MouseEnter += this.Grid_MouseEnter;
				((Grid)target).MouseLeave += this.Grid_MouseLeave;
				return;
			case 11:
				this.mLbl1Day = (TextBlock)target;
				this.mLbl1Day.MouseUp += this.Lbl1Day_MouseUp;
				return;
			case 12:
				((Grid)target).MouseEnter += this.Grid_MouseEnter;
				((Grid)target).MouseLeave += this.Grid_MouseLeave;
				return;
			case 13:
				this.mLbl1Week = (TextBlock)target;
				this.mLbl1Week.MouseUp += this.Lbl1Week_MouseUp;
				return;
			case 14:
				((Grid)target).MouseEnter += this.Grid_MouseEnter;
				((Grid)target).MouseLeave += this.Grid_MouseLeave;
				return;
			case 15:
				this.mLblForever = (TextBlock)target;
				this.mLblForever.MouseUp += this.LblForever_MouseUp;
				return;
			case 16:
				this.mCloseBtn = (CustomPictureBox)target;
				this.mCloseBtn.MouseLeftButtonUp += this.mCloseBtn_MouseLeftButtonUp;
				return;
			case 17:
				this.messageText = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040005D1 RID: 1489
		internal string Id;

		// Token: 0x040005D2 RID: 1490
		private MainWindow ParentWindow;

		// Token: 0x040005D4 RID: 1492
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox icon;

		// Token: 0x040005D5 RID: 1493
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock titleText;

		// Token: 0x040005D6 RID: 1494
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock dateText;

		// Token: 0x040005D7 RID: 1495
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mNotificationActions;

		// Token: 0x040005D8 RID: 1496
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSnoozeBtn;

		// Token: 0x040005D9 RID: 1497
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mMutePopup;

		// Token: 0x040005DA RID: 1498
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mLbl1Hour;

		// Token: 0x040005DB RID: 1499
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mLbl1Day;

		// Token: 0x040005DC RID: 1500
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mLbl1Week;

		// Token: 0x040005DD RID: 1501
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mLblForever;

		// Token: 0x040005DE RID: 1502
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseBtn;

		// Token: 0x040005DF RID: 1503
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock messageText;

		// Token: 0x040005E0 RID: 1504
		private bool _contentLoaded;
	}
}
