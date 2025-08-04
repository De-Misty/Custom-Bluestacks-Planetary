using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000EF RID: 239
	public class NotificationDrawer : UserControl, IComponentConnector
	{
		// Token: 0x060009F2 RID: 2546 RVA: 0x00008477 File Offset: 0x00006677
		public NotificationDrawer()
		{
			this.InitializeComponent();
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060009F3 RID: 2547 RVA: 0x00008485 File Offset: 0x00006685
		public MainWindow ParentWindow
		{
			get
			{
				if (this.mMainWindow == null)
				{
					this.mMainWindow = Window.GetWindow(this) as MainWindow;
				}
				return this.mMainWindow;
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060009F4 RID: 2548 RVA: 0x000084A6 File Offset: 0x000066A6
		// (set) Token: 0x060009F5 RID: 2549 RVA: 0x000084AD File Offset: 0x000066AD
		public static Timer SnoozeInfoGridTimer
		{
			get
			{
				return NotificationDrawer.snoozeInfoGridTimer;
			}
			set
			{
				NotificationDrawer.snoozeInfoGridTimer = value;
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060009F6 RID: 2550 RVA: 0x000084B5 File Offset: 0x000066B5
		// (set) Token: 0x060009F7 RID: 2551 RVA: 0x000084BC File Offset: 0x000066BC
		public static Timer DrawerAnimationTimer { get; set; } = new Timer(2000.0);

		// Token: 0x060009F8 RID: 2552 RVA: 0x000375BC File Offset: 0x000357BC
		internal void Populate(SerializableDictionary<string, GenericNotificationItem> items)
		{
			new List<NotificationDrawerItem>();
			new List<NotificationDrawerItem>();
			new List<string>();
			new List<string>();
			StackPanel stackPanel = this.mNotificationScroll.Content as StackPanel;
			Panel panel = this.mImportantNotificationScroll.Content as StackPanel;
			stackPanel.Children.Clear();
			panel.Children.Clear();
			this.mSnoozeInfoGrid.Visibility = Visibility.Collapsed;
			NotificationDrawer.SnoozeInfoGridTimer.Elapsed -= this.mSnoozeInfoGridTimer_Elapsed;
			NotificationDrawer.SnoozeInfoGridTimer.Elapsed += this.mSnoozeInfoGridTimer_Elapsed;
			NotificationDrawer.SnoozeInfoGridTimer.AutoReset = false;
			NotificationDrawer.DrawerAnimationTimer.Elapsed -= this.DrawerAnimationTimer_Elapsed;
			NotificationDrawer.DrawerAnimationTimer.Elapsed += this.DrawerAnimationTimer_Elapsed;
			NotificationDrawer.DrawerAnimationTimer.AutoReset = false;
			foreach (KeyValuePair<string, GenericNotificationItem> keyValuePair in items.Where((KeyValuePair<string, GenericNotificationItem> _) => !_.Value.IsDeleted))
			{
				this.AddNotificationItem(keyValuePair.Value);
			}
			this.HideUnhideNoNotification();
			this.UpdateNotificationCount();
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x000084C4 File Offset: 0x000066C4
		private void DrawerAnimationTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				BlueStacksUIBinding.BindColor(this.ParentWindow.mTopBar.mNotificationCaret, Shape.FillProperty, "ContextMenuItemBackgroundColor");
				BlueStacksUIBinding.BindColor(this.ParentWindow.mTopBar.mNotificationCaret, Shape.StrokeProperty, "ContextMenuItemBackgroundColor");
				BlueStacksUIBinding.BindColor(this.ParentWindow.mTopBar.mNotificationCentreDropDownBorder, Control.BorderBrushProperty, "PopupBorderBrush");
				this.ParentWindow.mTopBar.mNotificationDrawerControl.mAnimationRect.Visibility = Visibility.Collapsed;
				this.ParentWindow.mTopBar.mNotificationCentreButton.ImageName = "notification";
				this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Collapsed;
				if (this.ParentWindow.IsActive)
				{
					this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen = true;
				}
			}), new object[0]);
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x000084E4 File Offset: 0x000066E4
		private void mSnoozeInfoGridTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mSnoozeInfoGrid.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00037704 File Offset: 0x00035904
		private void HideUnhideNoNotification()
		{
			StackPanel stackPanel = this.mNotificationScroll.Content as StackPanel;
			StackPanel stackPanel2 = this.mImportantNotificationScroll.Content as StackPanel;
			if (!stackPanel2.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>() && !stackPanel.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
			{
				this.grdImportantUpdates.Visibility = Visibility.Collapsed;
				this.grdNormalUpdates.Visibility = Visibility.Visible;
				this.noNotifControl.Visibility = Visibility.Visible;
				this.mNotificationScroll.Visibility = Visibility.Collapsed;
				this.ParentWindow.mTopBar.mNotificationCentreDropDownBorder_LayoutUpdated(null, null);
				this.noNotification = true;
				return;
			}
			if (!stackPanel2.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
			{
				this.grdImportantUpdates.Visibility = Visibility.Collapsed;
			}
			if (!stackPanel.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
			{
				this.grdNormalUpdates.Visibility = Visibility.Collapsed;
			}
			if (this.noNotification)
			{
				this.noNotifControl.Visibility = Visibility.Collapsed;
				this.mNotificationScroll.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00037800 File Offset: 0x00035A00
		private void AddNotificationItem(GenericNotificationItem notifItem)
		{
			try
			{
				NotificationDrawerItem notificationDrawerItem = new NotificationDrawerItem();
				notificationDrawerItem.InitFromGenricNotificationItem(notifItem, this.ParentWindow);
				if (notifItem.Priority == NotificationPriority.Important)
				{
					StackPanel stackPanel = this.mImportantNotificationScroll.Content as StackPanel;
					Separator separator = new Separator();
					Style style = base.FindResource(ToolBar.SeparatorStyleKey) as Style;
					if (style != null)
					{
						separator.Style = style;
					}
					BlueStacksUIBinding.BindColor(separator, Panel.BackgroundProperty, "HorizontalSeparator");
					separator.Margin = new Thickness(0.0);
					if (stackPanel.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
					{
						stackPanel.Children.Insert(0, separator);
					}
					stackPanel.Children.Insert(0, notificationDrawerItem);
					this.grdImportantUpdates.Visibility = Visibility.Visible;
				}
				else
				{
					StackPanel stackPanel2 = this.mNotificationScroll.Content as StackPanel;
					Separator separator2 = new Separator();
					Style style2 = base.FindResource(ToolBar.SeparatorStyleKey) as Style;
					if (style2 != null)
					{
						separator2.Style = style2;
					}
					BlueStacksUIBinding.BindColor(separator2, Panel.BackgroundProperty, "HorizontalSeparator");
					separator2.Margin = new Thickness(0.0);
					if (stackPanel2.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
					{
						stackPanel2.Children.Insert(0, separator2);
					}
					stackPanel2.Children.Insert(0, notificationDrawerItem);
					this.grdNormalUpdates.Visibility = Visibility.Visible;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Could not add notificationdraweritem. Id " + notifItem.Id + "Error:" + ex.ToString());
			}
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00008504 File Offset: 0x00006704
		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			this.RemoveAllNotificationItems();
			e.Handled = true;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0003799C File Offset: 0x00035B9C
		public void RemoveAllNotificationItems()
		{
			StackPanel stackPanel = this.mNotificationScroll.Content as StackPanel;
			GenericNotificationManager.MarkNotification(from _ in stackPanel.Children.OfType<NotificationDrawerItem>()
				select _.Id, delegate(GenericNotificationItem _)
			{
				_.IsDeleted = true;
			});
			stackPanel.Children.Clear();
			this.noNotifControl.Visibility = Visibility.Visible;
			this.mNotificationScroll.Visibility = Visibility.Collapsed;
			this.noNotification = true;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00037A38 File Offset: 0x00035C38
		public void RemoveNotificationItem(string id)
		{
			StackPanel stackPanel = this.mNotificationScroll.Content as StackPanel;
			foreach (NotificationDrawerItem notificationDrawerItem in stackPanel.Children.OfType<NotificationDrawerItem>())
			{
				if (string.Equals(notificationDrawerItem.Id, id, StringComparison.InvariantCultureIgnoreCase))
				{
					List<string> list = new List<string>();
					list.Add(id);
					GenericNotificationManager.MarkNotification(list, delegate(GenericNotificationItem x)
					{
						x.IsDeleted = true;
					});
					int num = stackPanel.Children.IndexOf(notificationDrawerItem);
					stackPanel.Children.Remove(notificationDrawerItem);
					if (stackPanel.Children.Count > num)
					{
						stackPanel.Children.RemoveAt(num);
						break;
					}
					break;
				}
			}
			if (stackPanel.Children.Count == 0)
			{
				this.noNotifControl.Visibility = Visibility.Visible;
				this.mNotificationScroll.Visibility = Visibility.Collapsed;
				this.noNotification = true;
			}
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00037B38 File Offset: 0x00035D38
		public void UpdateNotificationCount()
		{
			SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && string.Equals(x.VmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture));
			if (notificationItems.Count > 0 && !this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen)
			{
				Border border = new Border
				{
					VerticalAlignment = VerticalAlignment.Center,
					Height = 14.0,
					MaxWidth = 24.0
				};
				TextBlock textBlock = new TextBlock
				{
					Text = notificationItems.Count.ToString(CultureInfo.InvariantCulture),
					FontSize = 10.0,
					MaxWidth = 24.0,
					FontWeight = FontWeights.Bold,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Padding = new Thickness(3.0, 0.0, 3.0, 1.0)
				};
				if (notificationItems.Count > 99)
				{
					textBlock.Text = "99+";
				}
				BlueStacksUIBinding.BindColor(textBlock, Control.ForegroundProperty, "SettingsWindowTitleBarForeGround");
				BlueStacksUIBinding.BindColor(border, Control.BackgroundProperty, "XPackPopupColor");
				border.CornerRadius = new CornerRadius(7.0);
				border.Child = textBlock;
				Canvas.SetLeft(border, 20.0);
				Canvas.SetTop(border, 9.0);
				if (this.ParentWindow.mTopBar.mNotificationCountBadge != null)
				{
					if (GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsRead && !x.IsDeleted && x.Priority == NotificationPriority.Important).Count > 0)
					{
						this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Collapsed;
					}
					else
					{
						this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Visible;
					}
					this.ParentWindow.mTopBar.mNotificationCountBadge.Children.Clear();
					this.ParentWindow.mTopBar.mNotificationCountBadge.Children.Add(border);
					return;
				}
			}
			else
			{
				this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00037D58 File Offset: 0x00035F58
		private void mSettingsbtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				Stats.SendCommonClientStatsAsync("notification_mode", "bell_settings_clicked", this.ParentWindow.mVmName, "", "", "");
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					MainWindow.OpenSettingsWindow(this.ParentWindow, "STRING_NOTIFICATION");
				}), new object[0]);
			}
			catch (Exception ex)
			{
				string text = "Error in opening settings window";
				Exception ex2 = ex;
				Logger.Info(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00037DE4 File Offset: 0x00035FE4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/genericnotification/notificationdrawer.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00037E14 File Offset: 0x00036014
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
				this.mNotificationDrawer = (NotificationDrawer)target;
				return;
			case 2:
				this.grdImportantUpdates = (Grid)target;
				return;
			case 3:
				this.mImportantNotificationScroll = (ScrollViewer)target;
				return;
			case 4:
				this.grdNormalUpdates = (Grid)target;
				return;
			case 5:
				this.mNotificationText = (TextBlock)target;
				return;
			case 6:
				((CustomButton)target).Click += this.ClearButton_Click;
				return;
			case 7:
				this.mSettingsbtn = (CustomPictureBox)target;
				this.mSettingsbtn.MouseLeftButtonUp += this.mSettingsbtn_MouseLeftButtonUp;
				return;
			case 8:
				this.mSnoozeInfoGrid = (Grid)target;
				return;
			case 9:
				this.mSnoozeInfoBlock = (TextBlock)target;
				return;
			case 10:
				this.mNotificationScroll = (ScrollViewer)target;
				return;
			case 11:
				this.noNotifControl = (Grid)target;
				return;
			case 12:
				this.mAnimationRect = (Rectangle)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040005BC RID: 1468
		private bool noNotification;

		// Token: 0x040005BD RID: 1469
		private MainWindow mMainWindow;

		// Token: 0x040005BE RID: 1470
		private static Timer snoozeInfoGridTimer = new Timer(2000.0);

		// Token: 0x040005BF RID: 1471
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal NotificationDrawer mNotificationDrawer;

		// Token: 0x040005C0 RID: 1472
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid grdImportantUpdates;

		// Token: 0x040005C1 RID: 1473
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mImportantNotificationScroll;

		// Token: 0x040005C2 RID: 1474
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid grdNormalUpdates;

		// Token: 0x040005C3 RID: 1475
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mNotificationText;

		// Token: 0x040005C4 RID: 1476
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSettingsbtn;

		// Token: 0x040005C5 RID: 1477
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSnoozeInfoGrid;

		// Token: 0x040005C6 RID: 1478
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSnoozeInfoBlock;

		// Token: 0x040005C7 RID: 1479
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mNotificationScroll;

		// Token: 0x040005C8 RID: 1480
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid noNotifControl;

		// Token: 0x040005C9 RID: 1481
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Rectangle mAnimationRect;

		// Token: 0x040005CA RID: 1482
		private bool _contentLoaded;
	}
}
