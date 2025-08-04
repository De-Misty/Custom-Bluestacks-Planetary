using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000042 RID: 66
	public class NotificationModeExitPopup : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x060003CA RID: 970 RVA: 0x00004786 File Offset: 0x00002986
		bool IDimOverlayControl.IsCloseOnOverLayClick
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060003CB RID: 971 RVA: 0x00004788 File Offset: 0x00002988
		// (set) Token: 0x060003CC RID: 972 RVA: 0x00004790 File Offset: 0x00002990
		public bool ShowControlInSeparateWindow { get; set; } = true;

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060003CD RID: 973 RVA: 0x00004799 File Offset: 0x00002999
		// (set) Token: 0x060003CE RID: 974 RVA: 0x000047A1 File Offset: 0x000029A1
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060003CF RID: 975 RVA: 0x000047AA File Offset: 0x000029AA
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x000047B2 File Offset: 0x000029B2
		public MainWindow ParentWindow { get; private set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x000047BB File Offset: 0x000029BB
		// (set) Token: 0x060003D2 RID: 978 RVA: 0x000047C3 File Offset: 0x000029C3
		public string PackageName { get; private set; }

		// Token: 0x060003D3 RID: 979 RVA: 0x0001A01C File Offset: 0x0001821C
		public NotificationModeExitPopup(MainWindow window, string packageName)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			this.PackageName = packageName;
			string text;
			if (File.Exists(Path.Combine(RegistryStrings.GadgetDir, packageName) + ".ico"))
			{
				text = Path.Combine(RegistryStrings.GadgetDir, packageName) + ".ico";
			}
			else if (File.Exists(Path.Combine(RegistryStrings.GadgetDir, packageName) + ".png"))
			{
				text = Path.Combine(RegistryStrings.GadgetDir, packageName) + ".png";
			}
			else
			{
				text = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets\\BlueStacks.ico");
			}
			this.mIconBorder.Background = new ImageBrush(new BitmapImage(new Uri(text)));
			string text2 = "notification_mode";
			string text3 = "exitpopup_shown";
			MainWindow parentWindow = this.ParentWindow;
			Stats.SendCommonClientStatsAsync(text2, text3, (parentWindow != null) ? parentWindow.mVmName : null, packageName, "", "");
			BitmapImage bitmapImage = new BitmapImage(new Uri(text));
			CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapImage, new Int32Rect((int)bitmapImage.Width / 10, (int)bitmapImage.Height * 2 / 10, (int)bitmapImage.Width * 8 / 10, (int)bitmapImage.Height * 8 / 10));
			VisualBrush visualBrush = new VisualBrush(new Image
			{
				Source = croppedBitmap,
				Effect = new BlurEffect
				{
					Radius = 24.0,
					KernelType = KernelType.Gaussian
				},
				ClipToBounds = true
			})
			{
				Opacity = 0.4
			};
			this.mBackground.Background = visualBrush;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000047CC File Offset: 0x000029CC
		bool IDimOverlayControl.Close()
		{
			this.Close();
			return true;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0001A1AC File Offset: 0x000183AC
		private void Close()
		{
			try
			{
				this.ParentWindow.HideDimOverlay();
				BlueStacksUIUtils.CloseContainerWindow(this);
				base.Visibility = Visibility.Hidden;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to close CloseBluestacksControl from dimoverlay " + ex.ToString());
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x000047DF File Offset: 0x000029DF
		private void mClosebtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_closed", this.ParentWindow.mVmName, "", "", "");
			this.Close();
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0001A1FC File Offset: 0x000183FC
		private void mYesBtn_Click(object sender, RoutedEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this.mYesBtn, Control.BackgroundProperty, "BlueMouseDownBorderBackground");
			Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_yes", this.ParentWindow.mVmName, "", "", "");
			this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
			RegistryManager.Instance.IsNotificationModeAlwaysOn = true;
			JsonParser jsonParser = new JsonParser(this.ParentWindow.mVmName);
			NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, jsonParser.GetAppNameFromPackage(this.PackageName));
			this.Close();
			this.ParentWindow.MinimizeWindowHandler();
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0001A29C File Offset: 0x0001849C
		private void mCloseBluestacks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_no", this.ParentWindow.mVmName, "", "", "");
			InstanceRegistry engineInstanceRegistry = this.ParentWindow.EngineInstanceRegistry;
			int notificationModePopupShownCount = engineInstanceRegistry.NotificationModePopupShownCount;
			engineInstanceRegistry.NotificationModePopupShownCount = notificationModePopupShownCount + 1;
			this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = false;
			RegistryManager.Instance.IsNotificationModeAlwaysOn = false;
			this.Close();
			this.ParentWindow.CloseWindowHandler(false);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00004810 File Offset: 0x00002A10
		private void mYesBtn_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this.mYesBtn, Control.BackgroundProperty, "BlueMouseInGridBackGround");
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00004827 File Offset: 0x00002A27
		private void mYesBtn_MouseLeave(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this.mYesBtn, Control.BackgroundProperty, "BlueMouseOutGridBackground");
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0001A31C File Offset: 0x0001851C
		private void mPreferenceCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = !this.mPreferenceCheckBox.IsChecked.GetValueOrDefault(true);
			Stats.SendCommonClientStatsAsync("notification_mode", "exit_popup_preference", "Android", "", "", "");
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0001A374 File Offset: 0x00018574
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/notificationmodeexitpopup.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0001A3A4 File Offset: 0x000185A4
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
				this.mBackground = (Border)target;
				return;
			case 2:
				this.mMainGrid = (Grid)target;
				return;
			case 3:
				this.mMaskBorder = (Border)target;
				return;
			case 4:
				this.mClosebtn = (CustomPictureBox)target;
				this.mClosebtn.MouseLeftButtonUp += this.mClosebtn_MouseLeftButtonUp;
				return;
			case 5:
				this.mIconBorder = (Border)target;
				return;
			case 6:
				this.mYesBtn = (Button)target;
				this.mYesBtn.Click += this.mYesBtn_Click;
				this.mYesBtn.MouseEnter += this.mYesBtn_MouseEnter;
				this.mYesBtn.MouseLeave += this.mYesBtn_MouseLeave;
				return;
			case 7:
				this.mCloseBluestacks = (TextBlock)target;
				this.mCloseBluestacks.MouseLeftButtonUp += this.mCloseBluestacks_MouseLeftButtonUp;
				return;
			case 8:
				this.mPreferenceCheckBox = (CustomCheckbox)target;
				this.mPreferenceCheckBox.Checked += this.mPreferenceCheckBox_Checked;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040001FE RID: 510
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBackground;

		// Token: 0x040001FF RID: 511
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMainGrid;

		// Token: 0x04000200 RID: 512
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000201 RID: 513
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mClosebtn;

		// Token: 0x04000202 RID: 514
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mIconBorder;

		// Token: 0x04000203 RID: 515
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Button mYesBtn;

		// Token: 0x04000204 RID: 516
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mCloseBluestacks;

		// Token: 0x04000205 RID: 517
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mPreferenceCheckBox;

		// Token: 0x04000206 RID: 518
		private bool _contentLoaded;
	}
}
