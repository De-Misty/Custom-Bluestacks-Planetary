using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000F3 RID: 243
	public class NCSoftTopBar : UserControl, ITopBar, IComponentConnector
	{
		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000A27 RID: 2599 RVA: 0x000086A9 File Offset: 0x000068A9
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

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000A28 RID: 2600 RVA: 0x000086CA File Offset: 0x000068CA
		// (set) Token: 0x06000A29 RID: 2601 RVA: 0x000086D7 File Offset: 0x000068D7
		string ITopBar.AppName
		{
			get
			{
				return this.mAppName.Text;
			}
			set
			{
				this.mAppName.Text = value;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000A2A RID: 2602 RVA: 0x000086E5 File Offset: 0x000068E5
		// (set) Token: 0x06000A2B RID: 2603 RVA: 0x000086F2 File Offset: 0x000068F2
		string ITopBar.CharacterName
		{
			get
			{
				return this.mCharacterName.Text;
			}
			set
			{
				this.mCharacterName.Text = value;
			}
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00008700 File Offset: 0x00006900
		public NCSoftTopBar()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x0000870E File Offset: 0x0000690E
		public void ChangeTopBarColor(string color)
		{
			BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, color);
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00038BE4 File Offset: 0x00036DE4
		private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
		{
			if (this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible && base.Visibility == Visibility.Visible && this.ParentWindow.mSidebar.Visibility != Visibility.Visible && !FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					this.ParentWindow.mCommonHandler.FlipSidebarVisibility(this.mSidebarButtonImage, this.mSidebarButtonText);
				}), new object[0]);
			}
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00038C4C File Offset: 0x00036E4C
		private void NCSoftTopBar_Loaded(object sender, RoutedEventArgs e)
		{
			if (!this.ParentWindow.mGuestBootCompleted)
			{
				this.ParentWindow.mCommonHandler.SetSidebarImageProperties(false, this.mSidebarButtonImage, this.mSidebarButtonText);
				this.ParentWindow.GuestBootCompleted += this.ParentWindow_GuestBootCompletedEvent;
			}
			this.ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += this.NCTopBar_ScreenRecordingStateChangedEvent;
			VideoRecordingStatus videoRecordingStatus = this.mVideoRecordStatusControl;
			videoRecordingStatus.RecordingStoppedEvent = (Action)Delegate.Combine(videoRecordingStatus.RecordingStoppedEvent, new Action(this.NCTopBar_RecordingStoppedEvent));
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x00038CE0 File Offset: 0x00036EE0
		private void NCTopBar_ScreenRecordingStateChangedEvent(bool isRecording)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (isRecording)
				{
					if (this.mVideoRecordingStatusGrid.Visibility != Visibility.Visible && CommonHandlers.sIsRecordingVideo)
					{
						this.mVideoRecordStatusControl.Init(this.ParentWindow);
						this.mVideoRecordingStatusGrid.Visibility = Visibility.Visible;
						return;
					}
				}
				else
				{
					this.mVideoRecordStatusControl.ResetTimer();
					this.mVideoRecordingStatusGrid.Visibility = Visibility.Collapsed;
				}
			}), new object[0]);
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x0000871C File Offset: 0x0000691C
		private void NCTopBar_RecordingStoppedEvent()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.mVideoRecordingStatusGrid.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00008741 File Offset: 0x00006941
		private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked minimize button");
			this.ParentWindow.MinimizeWindow();
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00008758 File Offset: 0x00006958
		internal void MaxmizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked Maximize\\Restore button");
			if (this.ParentWindow.WindowState == WindowState.Normal)
			{
				this.ParentWindow.MaximizeWindow();
				return;
			}
			this.ParentWindow.RestoreWindows(false);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x00008789 File Offset: 0x00006989
		private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked close Bluestacks button");
			this.ParentWindow.CloseWindow();
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x00038D24 File Offset: 0x00036F24
		private void SettingsButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if ((sender as Grid).Children[0].Visibility == Visibility.Visible)
			{
				this.mSettingsDropDownControl.LateInit();
				this.mSettingsDropdownPopup.IsOpen = true;
				this.mSettingsButtonImage.ImageName = "cfgmenu_selected";
				return;
			}
			this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00038D88 File Offset: 0x00036F88
		private void PinOnTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!this.ParentWindow.EngineInstanceRegistry.IsClientOnTop)
			{
				this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
				this.ParentWindow.Topmost = true;
				return;
			}
			this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
			this.ParentWindow.Topmost = false;
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x000087A0 File Offset: 0x000069A0
		private void MSidebarButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.FlipSidebarVisibility(this.mSidebarButtonImage, this.mSidebarButtonText);
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x000087BE File Offset: 0x000069BE
		internal void ShowMacroPlaybackOnTopBar(MacroRecording record)
		{
			if (this.ParentWindow.IsUIInPortraitMode)
			{
				this.mSettingsButton.Visibility = Visibility.Collapsed;
			}
			this.mMacroPlayControl.Init(this.ParentWindow, record);
			this.mMacroPlayGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x000087F7 File Offset: 0x000069F7
		internal void HideMacroPlaybackFromTopBar()
		{
			this.mSettingsButton.Visibility = Visibility.Visible;
			this.mMacroPlayGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00038DE4 File Offset: 0x00036FE4
		internal void ShowRecordingIcons()
		{
			if (this.ParentWindow.IsUIInPortraitMode)
			{
				this.mSettingsButton.Visibility = Visibility.Collapsed;
			}
			this.mMacroRecordControl.Init(this.ParentWindow);
			this.mMacroRecordGrid.Visibility = Visibility.Visible;
			this.mMacroRecordControl.StartTimer();
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x00008811 File Offset: 0x00006A11
		internal void HideRecordingIcons()
		{
			this.mSettingsButton.Visibility = Visibility.Visible;
			this.mMacroRecordGrid.Visibility = Visibility.Collapsed;
			this.mMacroRecorderToolTipPopup.IsOpen = false;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x00008837 File Offset: 0x00006A37
		private void NCSoftTopBar_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			DesignerProperties.GetIsInDesignMode(this);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void SettingsDropDownControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00008840 File Offset: 0x00006A40
		private void SettingsPopup_Opened(object sender, EventArgs e)
		{
			this.mSettingsDropdownPopup.IsEnabled = false;
			this.mSettingsButtonImage.ImageName = "cfgmenu";
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x0000885E File Offset: 0x00006A5E
		private void SettingsPopup_Closed(object sender, EventArgs e)
		{
			this.mSettingsDropdownPopup.IsEnabled = true;
			this.mSettingsButtonImage.ImageName = "cfgmenu";
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x0000887C File Offset: 0x00006A7C
		void ITopBar.ShowSyncPanel(bool isSource)
		{
			this.mOperationsSyncGrid.Visibility = Visibility.Visible;
			if (isSource)
			{
				this.mStopSyncButton.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00008899 File Offset: 0x00006A99
		void ITopBar.HideSyncPanel()
		{
			this.mOperationsSyncGrid.Visibility = Visibility.Collapsed;
			this.mStopSyncButton.Visibility = Visibility.Collapsed;
			this.mSyncInstancesToolTipPopup.IsOpen = false;
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x00038E34 File Offset: 0x00037034
		private void PlayPauseSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if ((sender as CustomPictureBox).ImageName.Equals("pause_title_bar", StringComparison.InvariantCultureIgnoreCase))
			{
				(sender as CustomPictureBox).ImageName = "play_title_bar";
				this.ParentWindow.mSynchronizerWindow.PauseAllSyncOperations();
				return;
			}
			(sender as CustomPictureBox).ImageName = "pause_title_bar";
			this.ParentWindow.mSynchronizerWindow.PlayAllSyncOperations();
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x000088BF File Offset: 0x00006ABF
		private void StopSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			((ITopBar)this).HideSyncPanel();
			this.ParentWindow.mSynchronizerWindow.StopAllSyncOperations();
			if (RegistryManager.Instance.IsShowToastNotification)
			{
				this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STOPPED", ""));
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x000088FD File Offset: 0x00006AFD
		private void OperationsSyncGrid_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.ParentWindow.mIsSynchronisationActive)
			{
				this.mSyncInstancesToolTipPopup.IsOpen = true;
			}
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00008918 File Offset: 0x00006B18
		private void OperationsSyncGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.ParentWindow.mIsSynchronisationActive && !this.mOperationsSyncGrid.IsMouseOver && !this.mSyncInstancesToolTipPopup.IsMouseOver)
			{
				this.mSyncInstancesToolTipPopup.IsOpen = false;
			}
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x0000894D File Offset: 0x00006B4D
		private void SyncInstancesToolTip_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.mOperationsSyncGrid.IsMouseOver && !this.mSyncInstancesToolTipPopup.IsMouseOver)
			{
				this.mSyncInstancesToolTipPopup.IsOpen = false;
			}
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x00038E9C File Offset: 0x0003709C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/ncsofttopbar.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00038ECC File Offset: 0x000370CC
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
				((NCSoftTopBar)target).Loaded += this.NCSoftTopBar_Loaded;
				((NCSoftTopBar)target).SizeChanged += this.NCSoftTopBar_SizeChanged;
				return;
			case 2:
				this.mMainGrid = (Grid)target;
				return;
			case 3:
				this.mTitleIcon = (CustomPictureBox)target;
				return;
			case 4:
				this.mWindowHeaderGrid = (StackPanel)target;
				return;
			case 5:
				this.mAppName = (TextBlock)target;
				return;
			case 6:
				this.mGamenameSeparator = (Line)target;
				return;
			case 7:
				this.mCharacterName = (TextBlock)target;
				return;
			case 8:
				this.mStreamingTopbarGrid = (Grid)target;
				return;
			case 9:
				this.mBorder = (Border)target;
				return;
			case 10:
				this.mNcTopBarControlGrid = (Grid)target;
				return;
			case 11:
				this.mMacroRecordGrid = (Grid)target;
				return;
			case 12:
				this.mMacroRecordControl = (MacroTopBarRecordControl)target;
				return;
			case 13:
				this.mMacroPlayGrid = (Grid)target;
				return;
			case 14:
				this.mMacroPlayControl = (MacroTopBarPlayControl)target;
				return;
			case 15:
				this.mVideoRecordingStatusGrid = (Grid)target;
				return;
			case 16:
				this.mVideoRecordStatusControl = (VideoRecordingStatus)target;
				return;
			case 17:
				this.mOperationsSyncGrid = (Grid)target;
				this.mOperationsSyncGrid.MouseEnter += this.OperationsSyncGrid_MouseEnter;
				this.mOperationsSyncGrid.MouseLeave += this.OperationsSyncGrid_MouseLeave;
				return;
			case 18:
				this.mSyncMaskBorder = (Border)target;
				return;
			case 19:
				this.mStopSyncButton = (CustomPictureBox)target;
				this.mStopSyncButton.PreviewMouseLeftButtonUp += this.StopSyncButton_PreviewMouseLeftButtonUp;
				return;
			case 20:
				this.mControlBtnPanel = (StackPanel)target;
				return;
			case 21:
				this.mSettingsButton = (Grid)target;
				this.mSettingsButton.PreviewMouseLeftButtonUp += this.SettingsButton_MouseLeftButtonUp;
				return;
			case 22:
				this.mSettingsButtonImage = (CustomPictureBox)target;
				return;
			case 23:
				this.mSettingsButtonText = (TextBlock)target;
				return;
			case 24:
				this.mMinimizeButton = (Grid)target;
				this.mMinimizeButton.PreviewMouseLeftButtonUp += this.MinimizeButton_MouseLeftButtonUp;
				return;
			case 25:
				this.mMinimizeButtonImage = (CustomPictureBox)target;
				return;
			case 26:
				this.mMinimizeButtonText = (TextBlock)target;
				return;
			case 27:
				this.mMaximizeButton = (Grid)target;
				this.mMaximizeButton.PreviewMouseLeftButtonUp += this.MaxmizeButton_MouseLeftButtonUp;
				return;
			case 28:
				this.mMaximizeButtonImage = (CustomPictureBox)target;
				return;
			case 29:
				this.mMaximizeButtonText = (TextBlock)target;
				return;
			case 30:
				this.mCloseButton = (Grid)target;
				this.mCloseButton.PreviewMouseLeftButtonUp += this.CloseButton_MouseLeftButtonUp;
				return;
			case 31:
				this.mCloseButtonImage = (CustomPictureBox)target;
				return;
			case 32:
				this.mCloseButtonText = (TextBlock)target;
				return;
			case 33:
				this.mSidebarButton = (Grid)target;
				this.mSidebarButton.PreviewMouseLeftButtonUp += this.MSidebarButton_MouseLeftButtonUp;
				return;
			case 34:
				this.mSidebarButtonImage = (CustomPictureBox)target;
				return;
			case 35:
				this.mSidebarButtonText = (TextBlock)target;
				return;
			case 36:
				this.mMacroRecorderToolTipPopup = (CustomPopUp)target;
				return;
			case 37:
				this.dummyGrid = (Grid)target;
				return;
			case 38:
				this.mMacroRecordingTooltip = (TextBlock)target;
				return;
			case 39:
				this.mUpArrow = (Path)target;
				return;
			case 40:
				this.mMacroRunningToolTipPopup = (CustomPopUp)target;
				return;
			case 41:
				this.grid = (Grid)target;
				return;
			case 42:
				this.mMacroRunningTooltip = (TextBlock)target;
				return;
			case 43:
				this.mSettingsDropdownPopup = (CustomPopUp)target;
				this.mSettingsDropdownPopup.Opened += this.SettingsPopup_Opened;
				this.mSettingsDropdownPopup.Closed += this.SettingsPopup_Closed;
				return;
			case 44:
				this.mSettingsDropdownBorder = (Border)target;
				return;
			case 45:
				this.mGrid = (Grid)target;
				return;
			case 46:
				this.mMaskBorder = (Border)target;
				return;
			case 47:
				this.mSettingsDropDownControl = (SettingsWindowDropdown)target;
				return;
			case 48:
				this.mSyncInstancesToolTipPopup = (CustomPopUp)target;
				return;
			case 49:
				this.mDummyGrid = (Grid)target;
				return;
			case 50:
				this.mUpwardArrow = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040005E3 RID: 1507
		private MainWindow mMainWindow;

		// Token: 0x040005E4 RID: 1508
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMainGrid;

		// Token: 0x040005E5 RID: 1509
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mTitleIcon;

		// Token: 0x040005E6 RID: 1510
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mWindowHeaderGrid;

		// Token: 0x040005E7 RID: 1511
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mAppName;

		// Token: 0x040005E8 RID: 1512
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Line mGamenameSeparator;

		// Token: 0x040005E9 RID: 1513
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mCharacterName;

		// Token: 0x040005EA RID: 1514
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mStreamingTopbarGrid;

		// Token: 0x040005EB RID: 1515
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBorder;

		// Token: 0x040005EC RID: 1516
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mNcTopBarControlGrid;

		// Token: 0x040005ED RID: 1517
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMacroRecordGrid;

		// Token: 0x040005EE RID: 1518
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal MacroTopBarRecordControl mMacroRecordControl;

		// Token: 0x040005EF RID: 1519
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMacroPlayGrid;

		// Token: 0x040005F0 RID: 1520
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal MacroTopBarPlayControl mMacroPlayControl;

		// Token: 0x040005F1 RID: 1521
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mVideoRecordingStatusGrid;

		// Token: 0x040005F2 RID: 1522
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal VideoRecordingStatus mVideoRecordStatusControl;

		// Token: 0x040005F3 RID: 1523
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOperationsSyncGrid;

		// Token: 0x040005F4 RID: 1524
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mSyncMaskBorder;

		// Token: 0x040005F5 RID: 1525
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mStopSyncButton;

		// Token: 0x040005F6 RID: 1526
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mControlBtnPanel;

		// Token: 0x040005F7 RID: 1527
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSettingsButton;

		// Token: 0x040005F8 RID: 1528
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSettingsButtonImage;

		// Token: 0x040005F9 RID: 1529
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSettingsButtonText;

		// Token: 0x040005FA RID: 1530
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMinimizeButton;

		// Token: 0x040005FB RID: 1531
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMinimizeButtonImage;

		// Token: 0x040005FC RID: 1532
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMinimizeButtonText;

		// Token: 0x040005FD RID: 1533
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMaximizeButton;

		// Token: 0x040005FE RID: 1534
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMaximizeButtonImage;

		// Token: 0x040005FF RID: 1535
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMaximizeButtonText;

		// Token: 0x04000600 RID: 1536
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mCloseButton;

		// Token: 0x04000601 RID: 1537
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseButtonImage;

		// Token: 0x04000602 RID: 1538
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mCloseButtonText;

		// Token: 0x04000603 RID: 1539
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSidebarButton;

		// Token: 0x04000604 RID: 1540
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSidebarButtonImage;

		// Token: 0x04000605 RID: 1541
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSidebarButtonText;

		// Token: 0x04000606 RID: 1542
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mMacroRecorderToolTipPopup;

		// Token: 0x04000607 RID: 1543
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid dummyGrid;

		// Token: 0x04000608 RID: 1544
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMacroRecordingTooltip;

		// Token: 0x04000609 RID: 1545
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path mUpArrow;

		// Token: 0x0400060A RID: 1546
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mMacroRunningToolTipPopup;

		// Token: 0x0400060B RID: 1547
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid grid;

		// Token: 0x0400060C RID: 1548
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMacroRunningTooltip;

		// Token: 0x0400060D RID: 1549
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mSettingsDropdownPopup;

		// Token: 0x0400060E RID: 1550
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mSettingsDropdownBorder;

		// Token: 0x0400060F RID: 1551
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x04000610 RID: 1552
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000611 RID: 1553
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SettingsWindowDropdown mSettingsDropDownControl;

		// Token: 0x04000612 RID: 1554
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mSyncInstancesToolTipPopup;

		// Token: 0x04000613 RID: 1555
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mDummyGrid;

		// Token: 0x04000614 RID: 1556
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path mUpwardArrow;

		// Token: 0x04000615 RID: 1557
		private bool _contentLoaded;
	}
}
