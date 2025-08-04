using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000107 RID: 263
	public class PikaNotificationControl : UserControl, IComponentConnector
	{
		// Token: 0x06000AF1 RID: 2801 RVA: 0x00008EC9 File Offset: 0x000070C9
		public PikaNotificationControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x0003DD90 File Offset: 0x0003BF90
		private void pikanotificationcontrol_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow != null && this.ParentWindow.mGuestBootCompleted)
			{
				this.ParentWindow.Utils.HandleGenericActionFromDictionary(this.mNotificationItem.ExtraPayload, "notification_ribbon", this.mNotificationItem.NotificationMenuImageName);
				ClientStats.SendMiscellaneousStatsAsync("RibbonClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.mNotificationItem.Id, this.mNotificationItem.Title, JsonConvert.SerializeObject(this.mNotificationItem.ExtraPayload), null, null, null);
				List<string> list = new List<string>();
				list.Add(this.mNotificationItem.Id);
				GenericNotificationManager.MarkNotification(list, delegate(GenericNotificationItem x)
				{
					x.IsRead = true;
				});
				IEnumerable<NotificationDrawerItem> enumerable = from _ in (this.ParentWindow.mTopBar.mNotificationDrawerControl.mNotificationScroll.Content as StackPanel).Children.OfType<NotificationDrawerItem>()
					where _.Id == this.mNotificationItem.Id
					select _;
				if (enumerable.Any<NotificationDrawerItem>())
				{
					enumerable.First<NotificationDrawerItem>().ChangeToReadBackground();
				}
				this.ParentWindow.mTopBar.RefreshNotificationCentreButton();
				this.CloseClicked(sender, e);
			}
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x0003DED0 File Offset: 0x0003C0D0
		private void ApplyHoverColors(bool hover)
		{
			if (hover)
			{
				if (string.IsNullOrEmpty(this.mNotificationItem.NotificationDesignItem.HoverBorderColor))
				{
					this.mNotificationItem.NotificationDesignItem.HoverBorderColor = this.mNotificationItem.NotificationDesignItem.BorderColor;
				}
				this.notificationBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.HoverBorderColor));
				if (string.IsNullOrEmpty(this.mNotificationItem.NotificationDesignItem.HoverRibboncolor))
				{
					this.mNotificationItem.NotificationDesignItem.HoverRibboncolor = this.mNotificationItem.NotificationDesignItem.Ribboncolor;
				}
				this.ribbonStroke.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.HoverBorderColor));
				this.ribbonBack.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.HoverRibboncolor));
				if (this.mNotificationItem.NotificationDesignItem.HoverBackGroundGradient.Count == 0)
				{
					this.mNotificationItem.NotificationDesignItem.HoverBackGroundGradient.ClearAddRange(this.mNotificationItem.NotificationDesignItem.BackgroundGradient);
				}
				this.backgroundPanel.Background = new LinearGradientBrush(new GradientStopCollection(this.mNotificationItem.NotificationDesignItem.HoverBackGroundGradient.Select((SerializableKeyValuePair<string, double> _) => new GradientStop((Color)ColorConverter.ConvertFromString(_.Key), _.Value))));
				return;
			}
			this.notificationBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.BorderColor));
			this.ribbonStroke.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.BorderColor));
			this.ribbonBack.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.Ribboncolor));
			this.backgroundPanel.Background = new LinearGradientBrush(new GradientStopCollection(this.mNotificationItem.NotificationDesignItem.BackgroundGradient.Select((SerializableKeyValuePair<string, double> _) => new GradientStop((Color)ColorConverter.ConvertFromString(_.Key), _.Value))));
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x0003E11C File Offset: 0x0003C31C
		internal void Init(GenericNotificationItem notifItem)
		{
			this.mNotificationItem = notifItem;
			this.titleText.Text = notifItem.Title;
			this.titleText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.TitleForeGroundColor));
			this.messageText.Text = notifItem.Message;
			this.messageText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.MessageForeGroundColor));
			this.notificationBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.BorderColor));
			this.ribbonStroke.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.BorderColor));
			if (notifItem.NotificationDesignItem.BackgroundGradient.Count == 0)
			{
				notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFF350", 0.0));
				notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFF8AF", 0.3));
				notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFE940", 0.6));
				notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FCE74E", 0.8));
				notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FDF09C", 0.9));
				notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFE227", 1.0));
			}
			this.backgroundPanel.Background = new LinearGradientBrush(new GradientStopCollection(notifItem.NotificationDesignItem.BackgroundGradient.Select((SerializableKeyValuePair<string, double> _) => new GradientStop((Color)ColorConverter.ConvertFromString(_.Key), _.Value))));
			if (string.IsNullOrEmpty(notifItem.NotificationDesignItem.Ribboncolor))
			{
				this.ribbonBack.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF350"));
			}
			else
			{
				this.ribbonBack.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.Ribboncolor));
			}
			if (string.IsNullOrEmpty(notifItem.NotificationDesignItem.LeftGifName))
			{
				this.pikaGif.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.pikaGif.Visibility = Visibility.Visible;
				this.pikaGif.ImageName = notifItem.NotificationDesignItem.LeftGifName;
			}
			Canvas.SetLeft(this, 0.0);
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00008ED7 File Offset: 0x000070D7
		private void PikaNotificationControl_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mCloseBtn.Visibility = Visibility.Hidden;
			this.ApplyHoverColors(false);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00008EEC File Offset: 0x000070EC
		private void PikaNotificationControl_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mCloseBtn.Visibility = Visibility.Visible;
			this.ApplyHoverColors(true);
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000AF7 RID: 2807 RVA: 0x0003E3BC File Offset: 0x0003C5BC
		// (remove) Token: 0x06000AF8 RID: 2808 RVA: 0x0003E3F4 File Offset: 0x0003C5F4
		public event EventHandler CloseClicked;

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00008F01 File Offset: 0x00007101
		private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Pika notification close button clicked");
			this.CloseClicked(sender, e);
			e.Handled = true;
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x0003E42C File Offset: 0x0003C62C
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (File.Exists(global::System.IO.Path.Combine(RegistryStrings.PromotionDirectory, this.pikaGif.ImageName)))
				{
					ImageSource imageSource = new BitmapImage(new Uri(global::System.IO.Path.Combine(RegistryStrings.PromotionDirectory, this.pikaGif.ImageName)));
					ImageBehavior.SetAnimatedSource(this.pikaGif, imageSource);
				}
				else if (File.Exists(global::System.IO.Path.Combine(CustomPictureBox.AssetsDir, this.pikaGif.ImageName)))
				{
					ImageSource imageSource2 = new BitmapImage(new Uri(global::System.IO.Path.Combine(CustomPictureBox.AssetsDir, this.pikaGif.ImageName)));
					ImageBehavior.SetAnimatedSource(this.pikaGif, imageSource2);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while loading pika notification. " + ex.ToString());
			}
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x0003E4F8 File Offset: 0x0003C6F8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/pikanotificationcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x0003E528 File Offset: 0x0003C728
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
				((PikaNotificationControl)target).AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(this.pikanotificationcontrol_MouseUp));
				((PikaNotificationControl)target).MouseEnter += this.PikaNotificationControl_MouseEnter;
				((PikaNotificationControl)target).MouseLeave += this.PikaNotificationControl_MouseLeave;
				((PikaNotificationControl)target).Loaded += this.UserControl_Loaded;
				return;
			case 2:
				this.mNotificationGrid = (Grid)target;
				return;
			case 3:
				this.ribbonBack = (global::System.Windows.Shapes.Path)target;
				return;
			case 4:
				this.ribbonStroke = (global::System.Windows.Shapes.Path)target;
				return;
			case 5:
				this.backgroundPanel = (StackPanel)target;
				return;
			case 6:
				this.pikaGif = (CustomPictureBox)target;
				return;
			case 7:
				this.titleText = (TextBlock)target;
				return;
			case 8:
				this.messageText = (TextBlock)target;
				return;
			case 9:
				this.notificationBorder = (Border)target;
				return;
			case 10:
				this.mCloseBtn = (CustomPictureBox)target;
				this.mCloseBtn.MouseLeftButtonUp += this.CloseBtn_MouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000684 RID: 1668
		internal MainWindow ParentWindow;

		// Token: 0x04000685 RID: 1669
		private GenericNotificationItem mNotificationItem;

		// Token: 0x04000687 RID: 1671
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mNotificationGrid;

		// Token: 0x04000688 RID: 1672
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path ribbonBack;

		// Token: 0x04000689 RID: 1673
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path ribbonStroke;

		// Token: 0x0400068A RID: 1674
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel backgroundPanel;

		// Token: 0x0400068B RID: 1675
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox pikaGif;

		// Token: 0x0400068C RID: 1676
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock titleText;

		// Token: 0x0400068D RID: 1677
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock messageText;

		// Token: 0x0400068E RID: 1678
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border notificationBorder;

		// Token: 0x0400068F RID: 1679
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseBtn;

		// Token: 0x04000690 RID: 1680
		private bool _contentLoaded;
	}
}
