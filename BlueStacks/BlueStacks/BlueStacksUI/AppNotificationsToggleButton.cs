using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000043 RID: 67
	public class AppNotificationsToggleButton : UserControl, IComponentConnector
	{
		// Token: 0x060003DF RID: 991 RVA: 0x0001A4D8 File Offset: 0x000186D8
		public AppNotificationsToggleButton(MainWindow window, string name, string imagePath, string packageName)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			this.PackageName = packageName;
			this.mAppTitle.Text = name;
			this.mAppIcon.Source = CustomPictureBox.GetBitmapImage(imagePath, "", true);
			if (NotificationManager.Instance.DictNotificationItems.Keys.Contains(name))
			{
				if (NotificationManager.Instance.DictNotificationItems[name].MuteState != MuteState.MutedForever)
				{
					this.mAppNotificationStatus.BoolValue = true;
					return;
				}
				this.mAppNotificationStatus.BoolValue = false;
				return;
			}
			else
			{
				NotificationManager instance = NotificationManager.Instance;
				MainWindow parentWindow = this.ParentWindow;
				MuteState defaultState = instance.GetDefaultState((parentWindow != null) ? parentWindow.mVmName : null);
				NotificationManager.Instance.DictNotificationItems.Add(name, new NotificationItem(name, defaultState, DateTime.Now));
				if (defaultState != MuteState.MutedForever)
				{
					this.mAppNotificationStatus.BoolValue = true;
					return;
				}
				this.mAppNotificationStatus.BoolValue = false;
				return;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x0000483E File Offset: 0x00002A3E
		// (set) Token: 0x060003E1 RID: 993 RVA: 0x00004846 File Offset: 0x00002A46
		public MainWindow ParentWindow { get; private set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x0000484F File Offset: 0x00002A4F
		// (set) Token: 0x060003E3 RID: 995 RVA: 0x00004857 File Offset: 0x00002A57
		public string PackageName { get; private set; }

		// Token: 0x060003E4 RID: 996 RVA: 0x0001A5C4 File Offset: 0x000187C4
		private void CustomToggleButtonWithState_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mAppNotificationStatus.BoolValue)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.mAppTitle.Text);
				Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_preferences", "Android", this.PackageName, "Mute", "");
				return;
			}
			NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, this.mAppTitle.Text);
			Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_preferences", "Android", this.PackageName, "UnMute", "");
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0001A654 File Offset: 0x00018854
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/appnotificationstogglebutton.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0001A684 File Offset: 0x00018884
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
				this.mAppIcon = (CustomPictureBox)target;
				return;
			case 2:
				this.mAppTitle = (TextBlock)target;
				return;
			case 3:
				this.mAppNotificationStatus = (CustomToggleButtonWithState)target;
				this.mAppNotificationStatus.PreviewMouseLeftButtonUp += this.CustomToggleButtonWithState_PreviewMouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000209 RID: 521
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mAppIcon;

		// Token: 0x0400020A RID: 522
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mAppTitle;

		// Token: 0x0400020B RID: 523
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomToggleButtonWithState mAppNotificationStatus;

		// Token: 0x0400020C RID: 524
		private bool _contentLoaded;
	}
}
