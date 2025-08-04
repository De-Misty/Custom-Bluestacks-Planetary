using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000294 RID: 660
	public partial class MainWindow : CustomWindow, IDisposable
	{
		// Token: 0x1700036A RID: 874
		// (get) Token: 0x0600180C RID: 6156 RVA: 0x00010309 File Offset: 0x0000E509
		// (set) Token: 0x0600180D RID: 6157 RVA: 0x00010311 File Offset: 0x0000E511
		internal int ParentWindowHeightDiff { get; set; } = 42;

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x0600180E RID: 6158 RVA: 0x0001031A File Offset: 0x0000E51A
		// (set) Token: 0x0600180F RID: 6159 RVA: 0x00010322 File Offset: 0x0000E522
		internal int ParentWindowWidthDiff { get; set; } = 2;

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06001810 RID: 6160 RVA: 0x0001032B File Offset: 0x0000E52B
		public global::System.Windows.Controls.UserControl TopBar
		{
			get
			{
				if (!FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					return this.mTopBar;
				}
				return this.mNCTopBar;
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06001811 RID: 6161 RVA: 0x0008EEEC File Offset: 0x0008D0EC
		internal ITopBar _TopBar
		{
			get
			{
				if (!FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					return this.mTopBar;
				}
				return this.mNCTopBar;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06001812 RID: 6162 RVA: 0x00010346 File Offset: 0x0000E546
		// (set) Token: 0x06001813 RID: 6163 RVA: 0x00010361 File Offset: 0x0000E561
		internal IMConfig SelectedConfig
		{
			get
			{
				if (this.mSelectedConfig == null)
				{
					this.mSelectedConfig = new IMConfig();
				}
				return this.mSelectedConfig;
			}
			set
			{
				this.mSelectedConfig = value;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06001814 RID: 6164 RVA: 0x0001036A File Offset: 0x0000E56A
		// (set) Token: 0x06001815 RID: 6165 RVA: 0x00010385 File Offset: 0x0000E585
		internal IMConfig OriginalLoadedConfig
		{
			get
			{
				if (this.mOriginalLoadedConfig == null)
				{
					this.mOriginalLoadedConfig = new IMConfig();
				}
				return this.mOriginalLoadedConfig;
			}
			set
			{
				this.mOriginalLoadedConfig = value;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001816 RID: 6166 RVA: 0x0001038E File Offset: 0x0000E58E
		// (set) Token: 0x06001817 RID: 6167 RVA: 0x00010396 File Offset: 0x0000E596
		internal Dictionary<string, int> AppNotificationCountDictForEachVM { get; set; } = new Dictionary<string, int>();

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06001818 RID: 6168 RVA: 0x0001039F File Offset: 0x0000E59F
		// (set) Token: 0x06001819 RID: 6169 RVA: 0x000103A7 File Offset: 0x0000E5A7
		internal bool SkipNextGamepadStatus
		{
			get
			{
				return this.mSkipNextGamepadStatus;
			}
			set
			{
				this.mSkipNextGamepadStatus = value;
				if (this.mSkipNextGamepadStatus)
				{
					this.WasGamepadStatusSkipped = value;
				}
			}
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x0008EF18 File Offset: 0x0008D118
		public static void OpenSettingsWindow(MainWindow window, string startTab)
		{
			if (window != null)
			{
				if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed && !KMManager.sGuidanceWindow.IsViewState)
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					customMessageWindow.Owner = window.mDimOverlay;
					customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
					customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CANNOT_OPEN_SETTING", "");
					customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_OK", ""), delegate(object o, EventArgs evt)
					{
					}, null, false, null);
					window.ShowDimOverlay(null);
					customMessageWindow.ShowDialog();
					window.HideDimOverlay();
					return;
				}
				if (MainWindow.SettingsWindow == null)
				{
					MainWindow.SettingsWindow = new SettingsWindow(window, startTab);
					int num = 500;
					int num2 = 750;
					new ContainerWindow(window, MainWindow.SettingsWindow, (double)num2, (double)num, false, true, false, -1.0, null);
					return;
				}
				MainWindow.SettingsWindow.ChangeSettingsTab(window, startTab);
			}
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x000103BF File Offset: 0x0000E5BF
		public static void CloseSettingsWindow(SettingsWindow window)
		{
			MainWindow.SettingsWindow = window;
			if (MainWindow.SettingsWindow != null)
			{
				BlueStacksUIUtils.CloseContainerWindow(MainWindow.SettingsWindow);
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x0600181C RID: 6172 RVA: 0x000103D8 File Offset: 0x0000E5D8
		// (set) Token: 0x0600181D RID: 6173 RVA: 0x000103E0 File Offset: 0x0000E5E0
		internal bool WasGamepadStatusSkipped { get; set; }

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x0600181E RID: 6174 RVA: 0x000103E9 File Offset: 0x0000E5E9
		// (set) Token: 0x0600181F RID: 6175 RVA: 0x0008F030 File Offset: 0x0008D230
		internal bool IsGamepadConnected
		{
			get
			{
				return this.mIsGamepadConnected;
			}
			set
			{
				this.mIsGamepadConnected = value;
				if (RegistryManager.Instance.IsShowToastNotification && !this.SkipNextGamepadStatus)
				{
					this.ShowGamepadToast(value);
				}
				this.SkipNextGamepadStatus = false;
				BlueStacksUIUtils.SendGamepadStatusToBrowsers(value);
				this.mWelcomeTab.mHomeAppManager.UpdateGamepadIcons(value);
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06001820 RID: 6176 RVA: 0x000103F1 File Offset: 0x0000E5F1
		// (set) Token: 0x06001821 RID: 6177 RVA: 0x000103F9 File Offset: 0x0000E5F9
		public DummyTaskbarWindow DummyWindow { get; set; }

		// Token: 0x06001822 RID: 6178 RVA: 0x0008F080 File Offset: 0x0008D280
		private void ShowGamepadToast(bool state)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					if (this.mIsWindowInFocus)
					{
						if (this.toastPopup.IsOpen)
						{
							this.toastTimer.Stop();
							this.toastPopup.IsOpen = false;
						}
						double num = ((10.0 + this.mSidebar.ActualWidth > 0.0) ? this.mSidebar.ActualWidth : (0.0 + this.mWelcomeTab.mHomeAppManager.GetAppRecommendationsGridWidth()));
						if (state)
						{
							this.toastControl.Init(this, LocaleStrings.GetLocalizedString("STRING_GAMEPAD_CONNECTED", ""), null, new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), global::System.Windows.HorizontalAlignment.Right, VerticalAlignment.Bottom, new Thickness?(new Thickness(0.0, 0.0, num, 20.0)), 5, null, null, false);
							this.toastControl.AddImage("gamepad_connected", 16.0, 24.0, new Thickness?(new Thickness(0.0, 5.0, 10.0, 5.0)));
						}
						else
						{
							this.toastControl.Init(this, LocaleStrings.GetLocalizedString("STRING_GAMEPAD_DISCONNECTED", ""), null, new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), global::System.Windows.HorizontalAlignment.Right, VerticalAlignment.Bottom, new Thickness?(new Thickness(0.0, 0.0, num, 20.0)), 5, null, null, false);
							this.toastControl.AddImage("gamepad_disconnected", 19.0, 24.0, new Thickness?(new Thickness(0.0, 5.0, 10.0, 5.0)));
						}
						this.dummyToast.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Right;
						this.dummyToast.VerticalAlignment = VerticalAlignment.Bottom;
						this.toastControl.Visibility = Visibility.Visible;
						this.toastPopup.IsOpen = true;
						this.toastCanvas.Width = this.toastControl.ActualWidth;
						this.toastCanvas.Height = this.toastControl.ActualHeight;
						this.toastPopup.VerticalOffset = -1.0 * this.toastControl.ActualHeight - 50.0;
						this.toastPopup.HorizontalOffset = -20.0;
						this.toastTimer.Start();
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in showing toast popup for gamepad : " + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x0008F0C0 File Offset: 0x0008D2C0
		internal void ShowGeneralToast(string message)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					if (this.mIsWindowInFocus)
					{
						if (this.mGeneraltoast.IsOpen)
						{
							this.toastTimer.Stop();
							this.mGeneraltoast.IsOpen = false;
						}
						this.mGeneraltoastControl.Init(this, message, global::System.Windows.Media.Brushes.Black, new SolidColorBrush(global::System.Windows.Media.Color.FromArgb(85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), global::System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 5, null, null, false);
						this.dummyToast.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
						this.dummyToast.VerticalAlignment = VerticalAlignment.Bottom;
						this.mGeneraltoastControl.Visibility = Visibility.Visible;
						this.mGeneraltoast.IsOpen = true;
						this.mGeneraltoastCanvas.Height = this.mGeneraltoastControl.ActualHeight;
						this.mGeneraltoast.VerticalOffset = -1.0 * this.mGeneraltoastControl.ActualHeight - 50.0;
						this.mGeneraltoast.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
						this.toastTimer.Start();
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in showing general toast popup : " + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x00010402 File Offset: 0x0000E602
		private void ToastTimer_Tick(object sender, EventArgs e)
		{
			this.toastTimer.Stop();
			this.toastPopup.IsOpen = false;
			this.mGeneraltoast.IsOpen = false;
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x00010427 File Offset: 0x0000E627
		internal void CloseFullScreenToastAndStopTimer()
		{
			this.mFullScreenToastTimer.Stop();
			this.mFullScreenToastPopup.IsOpen = false;
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x00010440 File Offset: 0x0000E640
		private void FullScreenToastTimer_Tick(object sender, EventArgs e)
		{
			this.CloseFullScreenToastAndStopTimer();
		}

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06001827 RID: 6183 RVA: 0x0008F100 File Offset: 0x0008D300
		// (remove) Token: 0x06001828 RID: 6184 RVA: 0x0008F138 File Offset: 0x0008D338
		private event EventHandler CloseWindowConfirmationAcceptedHandler;

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x06001829 RID: 6185 RVA: 0x0008F170 File Offset: 0x0008D370
		// (remove) Token: 0x0600182A RID: 6186 RVA: 0x0008F1A8 File Offset: 0x0008D3A8
		private event EventHandler CloseWindowConfirmationResetAccountAcceptedHandler;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x0600182B RID: 6187 RVA: 0x0008F1E0 File Offset: 0x0008D3E0
		// (remove) Token: 0x0600182C RID: 6188 RVA: 0x0008F218 File Offset: 0x0008D418
		public event MainWindow.GuestBootCompletedEventHandler GuestBootCompleted;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x0600182D RID: 6189 RVA: 0x0008F250 File Offset: 0x0008D450
		// (remove) Token: 0x0600182E RID: 6190 RVA: 0x0008F288 File Offset: 0x0008D488
		internal event MainWindow.CursorLockChangedEventHandler CursorLockChangedEvent;

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x0600182F RID: 6191 RVA: 0x0008F2C0 File Offset: 0x0008D4C0
		// (remove) Token: 0x06001830 RID: 6192 RVA: 0x0008F2F8 File Offset: 0x0008D4F8
		internal event MainWindow.FullScreenChangedEventHandler FullScreenChanged;

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x06001831 RID: 6193 RVA: 0x0008F330 File Offset: 0x0008D530
		// (remove) Token: 0x06001832 RID: 6194 RVA: 0x0008F368 File Offset: 0x0008D568
		internal event MainWindow.FrontendGridVisibilityChangedEventHandler FrontendGridVisibilityChanged;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x06001833 RID: 6195 RVA: 0x0008F3A0 File Offset: 0x0008D5A0
		// (remove) Token: 0x06001834 RID: 6196 RVA: 0x0008F3D8 File Offset: 0x0008D5D8
		private event EventHandler mEventOnAllWindowClosed;

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x06001835 RID: 6197 RVA: 0x0008F410 File Offset: 0x0008D610
		// (remove) Token: 0x06001836 RID: 6198 RVA: 0x0008F448 File Offset: 0x0008D648
		private event EventHandler mEventOnInstanceClosed;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x06001837 RID: 6199 RVA: 0x0008F480 File Offset: 0x0008D680
		// (remove) Token: 0x06001838 RID: 6200 RVA: 0x0008F4B8 File Offset: 0x0008D6B8
		internal event EventHandler RestartEngineConfirmationAcceptedHandler;

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x06001839 RID: 6201 RVA: 0x0008F4F0 File Offset: 0x0008D6F0
		// (remove) Token: 0x0600183A RID: 6202 RVA: 0x0008F528 File Offset: 0x0008D728
		internal event EventHandler RestartPcConfirmationAcceptedHandler;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x0600183B RID: 6203 RVA: 0x0008F560 File Offset: 0x0008D760
		// (remove) Token: 0x0600183C RID: 6204 RVA: 0x0008F598 File Offset: 0x0008D798
		internal event MainWindow.BrowserOTSCompletedCallbackEventHandler BrowserOTSCompletedCallback;

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x0600183D RID: 6205 RVA: 0x00010448 File Offset: 0x0000E648
		internal Grid FirebaseBrowserControlGrid
		{
			get
			{
				if (this.mFirebaseBrowserControlGrid == null)
				{
					this.mFirebaseBrowserControlGrid = this.AddBrowser(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/notification"));
				}
				return this.mFirebaseBrowserControlGrid;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x0600183E RID: 6206 RVA: 0x00010478 File Offset: 0x0000E678
		internal ScreenLockControl ScreenLockInstance
		{
			get
			{
				if (this.mScreenLock == null)
				{
					this.mScreenLock = new ScreenLockControl();
				}
				return this.mScreenLock;
			}
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x0008F5D0 File Offset: 0x0008D7D0
		private void GetMacroShortcutKeyMappingsWithRestrictedKeysandNames()
		{
			foreach (BiDirectionalVertex<MacroRecording> biDirectionalVertex in MacroGraph.Instance.Vertices)
			{
				MacroRecording macroRecording = (MacroRecording)biDirectionalVertex;
				if (macroRecording.Shortcut.Length == 1 && !MainWindow.sMacroMapping.ContainsKey(macroRecording.Shortcut))
				{
					MainWindow.sMacroMapping.Add(macroRecording.Shortcut, macroRecording.Name);
				}
				if (macroRecording.PlayOnStart)
				{
					if (this.mAutoRunMacro == null)
					{
						this.mAutoRunMacro = macroRecording;
					}
					else
					{
						macroRecording.PlayOnStart = false;
						CommonHandlers.SaveMacroJson(macroRecording, macroRecording.Name + ".json");
					}
				}
			}
			HTTPUtils.SendRequestToEngineAsync("updateMacroShortcutsDict", MainWindow.sMacroMapping, this.mVmName, 0, null, false, 1, 0);
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06001840 RID: 6208 RVA: 0x00010493 File Offset: 0x0000E693
		internal MacroOverlay MacroOverlayControl
		{
			get
			{
				if (this.mMacroOverlay == null)
				{
					this.mMacroOverlay = new MacroOverlay(this);
				}
				return this.mMacroOverlay;
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06001841 RID: 6209 RVA: 0x000104AF File Offset: 0x0000E6AF
		internal InstanceRegistry EngineInstanceRegistry
		{
			get
			{
				return RegistryManager.Instance.Guest[this.mVmName];
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06001842 RID: 6210 RVA: 0x000104C6 File Offset: 0x0000E6C6
		internal MacroRecorderWindow MacroRecorderWindow
		{
			get
			{
				if (this.mMacroRecorderWindow == null)
				{
					this.mMacroRecorderWindow = new MacroRecorderWindow(this)
					{
						Owner = this
					};
				}
				return this.mMacroRecorderWindow;
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06001843 RID: 6211 RVA: 0x000104E9 File Offset: 0x0000E6E9
		internal BlueStacksUIUtils Utils
		{
			get
			{
				if (this.mUtils == null)
				{
					this.mUtils = new BlueStacksUIUtils(this);
				}
				return this.mUtils;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06001844 RID: 6212 RVA: 0x00010505 File Offset: 0x0000E705
		internal MainWindowsStaticComponents StaticComponents
		{
			get
			{
				if (this.mStaticComponents == null)
				{
					this.mStaticComponents = new MainWindowsStaticComponents();
				}
				return this.mStaticComponents;
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06001845 RID: 6213 RVA: 0x00010520 File Offset: 0x0000E720
		internal bool IsDefaultVM
		{
			get
			{
				return string.Equals(this.mVmName, Strings.CurrentDefaultVmName, StringComparison.InvariantCulture);
			}
		}

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06001846 RID: 6214 RVA: 0x00010533 File Offset: 0x0000E733
		internal Storyboard StoryBoard
		{
			get
			{
				if (this.mStoryBoard == null)
				{
					this.mStoryBoard = base.FindResource("mStoryBoard") as Storyboard;
				}
				return this.mStoryBoard;
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06001847 RID: 6215 RVA: 0x00010559 File Offset: 0x0000E759
		// (set) Token: 0x06001848 RID: 6216 RVA: 0x00010561 File Offset: 0x0000E761
		public Discord mDiscordhandler { get; set; }

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06001849 RID: 6217 RVA: 0x0001056A File Offset: 0x0000E76A
		internal bool SendClientActions
		{
			get
			{
				return this.mIsMacroRecorderActive || this.mIsSynchronisationActive;
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x0600184A RID: 6218 RVA: 0x0001057C File Offset: 0x0000E77C
		// (set) Token: 0x0600184B RID: 6219 RVA: 0x00010583 File Offset: 0x0000E783
		public static SettingsWindow SettingsWindow { get; set; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x0600184C RID: 6220 RVA: 0x0001058B File Offset: 0x0000E78B
		// (set) Token: 0x0600184D RID: 6221 RVA: 0x00010593 File Offset: 0x0000E793
		public bool IsInNotificationMode { get; set; }

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x0600184E RID: 6222 RVA: 0x0001059C File Offset: 0x0000E79C
		// (set) Token: 0x0600184F RID: 6223 RVA: 0x000105A4 File Offset: 0x0000E7A4
		public string mPostBootNotificationAction { get; set; }

		// Token: 0x06001850 RID: 6224 RVA: 0x0008F6A8 File Offset: 0x0008D8A8
		public MainWindow(string vmName, FrontendHandler frontendHandler)
		{
			Logger.Info("main window init");
			this.mVmName = vmName;
			this.GetLockOfCurrentInstance();
			this.SetMultiInstanceEventWaitHandle();
			if (frontendHandler != null)
			{
				this.mFrontendHandler = frontendHandler;
				frontendHandler.ParentWindow = this;
			}
			this.mCommonHandler = new CommonHandlers(this);
			this.InitializeComponent();
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.mWelcomeTab.Init();
				this.mFrontendGrid.Visibility = Visibility.Visible;
			}
			else
			{
				this.ParentWindowHeightDiff = (this.heightDiffScaled = 94);
				this.WelcomeTabParentGrid.Visibility = Visibility.Hidden;
				this.mWelcomeTab.Init();
				this.mWelcomeTab.Visibility = Visibility.Hidden;
				this.mWelcomeTab.mPromotionGrid.Visibility = Visibility.Hidden;
				this.mWelcomeTab.mPromotionControl.IsEnabled = false;
				this.FrontendParentGrid.Visibility = Visibility.Visible;
				this.mDmmProgressControl.Visibility = Visibility.Visible;
				this.mFrontendGrid.Visibility = Visibility.Hidden;
				this.mFrontendGrid.Margin = new Thickness(0.0, 0.0, 0.0, 2.0);
				this.mDmmBottomBar.Visibility = Visibility.Visible;
				this.mDMMFST = new DMMFullScreenTopBar();
				this.mDmmBottomBar.Init(this);
				this.mTopBarPopup.Child = this.mDMMFST;
				this.mDMMFST.Init(this);
				this.mDMMFST.MouseLeave += this.TopBarPopup_MouseLeave;
			}
			base.SizeChanged += this.MainWindow_SizeChanged;
			base.LocationChanged += this.MainWindow_LocationChanged;
			this.SetupInitialSize();
			this.SetWindowTitle(vmName);
			this.mResizeHandler = new WindowWndProcHandler(this);
			this.mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
			this.mAppHandler = new AppHandler(this);
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mTopBar.Visibility = Visibility.Collapsed;
				this.mNCTopBar.Visibility = Visibility.Visible;
			}
			if (this.EngineInstanceRegistry.IsClientOnTop)
			{
				base.Topmost = true;
			}
			this.mCommonHandler.InitShortcuts();
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.mSidebar.InitElements();
			}
			if (!string.IsNullOrEmpty(RegistryManager.Instance.Token))
			{
				this.mIsTokenAvailable = true;
			}
			if (this.IsDefaultVM && this.mAppHandler.IsOneTimeSetupCompleted)
			{
				PromotionObject.PromotionHandler = (EventHandler)Delegate.Combine(PromotionObject.PromotionHandler, new EventHandler(this.MainWindow_PromotionHandler));
			}
			AppRequirementsParser.Instance.RequirementConfigUpdated += this.MainWindow_RequirementConfigUpdated;
			base.PreviewKeyDown += this.MainWindow_PreviewKeyDown;
			base.PreviewKeyUp += this.MainWindow_PreviewKeyUp;
			RegistryManager.Instance.BossKey = this.mCommonHandler.GetShortcutKeyFromName("STRING_BOSSKEY_SETTING", true);
			try
			{
				if (!AppConfigurationManager.Instance.VmAppConfig.ContainsKey(this.mVmName))
				{
					AppConfigurationManager.Instance.VmAppConfig[this.mVmName] = new Dictionary<string, AppSettings>();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("error {0}", new object[] { ex });
			}
			this.mIsWindowLoadedOnce = true;
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x0008FAD8 File Offset: 0x0008DCD8
		private void GetLockOfCurrentInstance()
		{
			Logger.Debug("Getting lock of instance.." + this.mVmName);
			ProcessUtils.CheckAlreadyRunningAndTakeLock(Strings.GetClientInstanceLockName(this.mVmName, "bgp64"), out this.mBlueStacksClientInstanceLock);
			if (this.mBlueStacksClientInstanceLock == null)
			{
				Logger.Error("Client lock is not created for vmName: {0}", new object[] { this.mVmName });
			}
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x0008FB38 File Offset: 0x0008DD38
		private void SetMultiInstanceEventWaitHandle()
		{
			try
			{
				using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(BlueStacks.Common.Utils.GetMultiInstanceEventName(this.mVmName)))
				{
					eventWaitHandle.Set();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while setting event wait handle for vmName: {0} ex: {1}", new object[] { this.mVmName, ex });
			}
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x000105AD File Offset: 0x0000E7AD
		private void MainWindow_RequirementConfigUpdated(object sender, EventArgs args)
		{
			GrmHandler.RequirementConfigUpdated(this.mVmName);
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x000105BA File Offset: 0x0000E7BA
		private void MainWindow_PromotionHandler(object sender, EventArgs e)
		{
			if (this.IsDefaultVM && this.mAppHandler.IsOneTimeSetupCompleted && !this.mGuestBootCompleted)
			{
				this.HandleFLEorAppPopupBeforeBoot();
			}
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x000105DF File Offset: 0x0000E7DF
		private void SetTaskbarProperties()
		{
			base.Icon = new BitmapImage(new Uri(global::System.IO.Path.Combine(RegistryStrings.InstallDir, "app_icon.ico")));
			base.Title = GameConfig.Instance.AppName;
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x00010610 File Offset: 0x0000E810
		internal void RestartFrontend()
		{
			this.mFrontendHandler.mEventOnFrontendClosed -= this.FrontendHandler_StartFrontend;
			this.mFrontendHandler.mEventOnFrontendClosed += this.FrontendHandler_StartFrontend;
			this.CloseFrontend();
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x0008FBA8 File Offset: 0x0008DDA8
		private void FrontendHandler_StartFrontend(object sender, EventArgs e)
		{
			this.mFrontendHandler.StartFrontend();
			if (!FeatureManager.Instance.IsCustomUIForDMM && !this.Utils.IsRequiredFreeRAMAvailable())
			{
				this.mFrontendHandler.mIsSufficientRAMAvailable = false;
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mFrontendHandler.FrontendHandler_ShowLowRAMMessage();
				}), new object[0]);
			}
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x00010646 File Offset: 0x0000E846
		internal void CloseFrontend()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.ShowLoadingGrid(true);
				this.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
				if (this.mWelcomeTab != null)
				{
					this.mWelcomeTab.mFrontendPopupControl.HideWindow();
				}
				if (this.mAppHandler != null)
				{
					this.mAppHandler.IsGuestReady = false;
					this.mAppHandler.mGuestReadyCheckStarted = false;
				}
				this.mFrontendHandler.mFrontendHandle = IntPtr.Zero;
			}), new object[0]);
			this.mFrontendHandler.KillFrontend(true);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x0008FC04 File Offset: 0x0008DE04
		internal void SwitchToPortraitMode(bool isSwitchForPortraitMode)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					if (this.WindowState == WindowState.Normal)
					{
						this.mPreviousWidth = new double?(this.Width);
						this.mPreviousHeight = new double?(this.Height);
					}
					bool flag = false;
					if (isSwitchForPortraitMode && this.WindowState != WindowState.Maximized)
					{
						if (isSwitchForPortraitMode != this.IsUIInPortraitMode)
						{
							flag = true;
							this.IsUIInPortraitMode = true;
							this.mTopBar.RefreshNotificationCentreButton();
							this.mTopBar.UpdateMacroRecordingProgress();
						}
					}
					else
					{
						if (isSwitchForPortraitMode && FeatureManager.Instance.IsCustomUIForDMM)
						{
							this.IsUIInPortraitMode = true;
							this.WindowState = WindowState.Normal;
							this.SetSizeForDMMPortraitMaximisedWindow();
							this.mTopBar.RefreshNotificationCentreButton();
							this.mTopBar.RefreshWarningButton();
							return;
						}
						if (isSwitchForPortraitMode != this.IsUIInPortraitMode)
						{
							flag = true;
							this.IsUIInPortraitMode = false;
							if (this.mIsDmmMaximised)
							{
								this.WindowState = WindowState.Maximized;
							}
							this.mTopBar.UpdateMacroRecordingProgress();
							this.mTopBar.RefreshNotificationCentreButton();
						}
					}
					if (this.WindowState == WindowState.Normal)
					{
						if (FeatureManager.Instance.IsCustomUIForDMM && this.mIsDmmMaximised && this.DmmRestoreWindowRectangle.Height != 0.0)
						{
							this.SetDMMSizeOnRestoreWindow();
						}
						else
						{
							this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight((double)((int)this.Height), false, false), (double)((int)this.Height), flag || (this.IsUIInPortraitMode ^ this.IsUIInPortraitModeWhenMaximized));
						}
					}
					this.mTopBar.RefreshWarningButton();
					this.UIChangesOnMainWindowSizeChanged();
					if (this.mStreamingModeEnabled)
					{
						this.mFrontendHandler.ChangeFrontendToPortraitMode();
					}
					if (StreamManager.Instance != null)
					{
						StreamManager.Instance.OrientationChangeHandler();
					}
					if (KMManager.sGuidanceWindow != null && !this.mIsFullScreen)
					{
						KMManager.sGuidanceWindow.ResizeGuidanceWindow();
					}
				}
				catch (Exception ex)
				{
					this.SetupInitialSize();
					Logger.Info("Error occured setting size." + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x0008FC44 File Offset: 0x0008DE44
		private void SetDMMSizeOnRestoreWindow()
		{
			this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight((double)((int)this.DmmRestoreWindowRectangle.Height), false, false), (double)((int)this.DmmRestoreWindowRectangle.Height), false);
			if (this.mIsDMMMaximizedFromPortrait != this.IsUIInPortraitMode)
			{
				if (this.IsUIInPortraitMode)
				{
					base.Left = this.DmmRestoreWindowRectangle.Left + (this.DmmRestoreWindowRectangle.Width - base.Width) / 2.0;
				}
				else
				{
					base.Left = this.DmmRestoreWindowRectangle.Left - (base.Width - this.DmmRestoreWindowRectangle.Width) / 2.0;
				}
			}
			else
			{
				base.Left = this.DmmRestoreWindowRectangle.Left;
			}
			base.Top = this.DmmRestoreWindowRectangle.Top;
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x0008FD14 File Offset: 0x0008DF14
		private void UIChangesOnMainWindowSizeChanged()
		{
			this.pikaPop.HorizontalOffset += 1.0;
			this.pikaPop.HorizontalOffset -= 1.0;
			this.toastPopup.HorizontalOffset += 1.0;
			this.toastPopup.HorizontalOffset -= 1.0;
			this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
			this.SetMaxSizeOfWindow();
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x0008FDAC File Offset: 0x0008DFAC
		private void SetMaxSizeOfWindow()
		{
			global::System.Windows.Size maxWidthAndHeightOfMonitor = WindowPlacement.GetMaxWidthAndHeightOfMonitor(new WindowInteropHelper(this).Handle);
			this.MaxHeightScaled = (int)maxWidthAndHeightOfMonitor.Height;
			this.MaxWidthScaled = (int)this.GetWidthFromHeight((double)this.MaxHeightScaled, false, false);
			if ((double)this.MaxWidthScaled > maxWidthAndHeightOfMonitor.Width)
			{
				this.MaxWidthScaled = (int)maxWidthAndHeightOfMonitor.Width;
				this.MaxHeightScaled = (int)this.GetHeightFromWidth((double)this.MaxWidthScaled, false, false);
			}
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x0008FE24 File Offset: 0x0008E024
		private void ChangeHeightWidthAndPosition(double width, double height, bool changePosition)
		{
			try
			{
				base.Height = height;
				base.Width = width;
				if (FeatureManager.Instance.IsCustomUIForDMM && !this.mIsWindowResizedOnce)
				{
					double num = (base.Height - (double)this.ParentWindowHeightDiff) * 9.0 / 16.0 + (double)this.ParentWindowWidthDiff;
					base.Left = (SystemParameters.MaximizedPrimaryScreenWidth - base.Width - num) / 2.0;
					base.Top = (SystemParameters.MaximizedPrimaryScreenHeight - base.Height) / 2.0;
					this.mIsWindowResizedOnce = true;
				}
				else if (changePosition)
				{
					if (this.IsUIInPortraitMode)
					{
						base.Left += (this.mPreviousWidth.Value - base.Width) / 2.0;
					}
					else
					{
						base.Left -= (base.Width - this.mPreviousWidth.Value) / 2.0;
					}
				}
				this.mPreviousWidth = new double?(base.Width);
				this.mPreviousHeight = new double?(base.Height);
			}
			catch (Exception ex)
			{
				Logger.Info("Error occured setting size." + ex.ToString());
			}
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x0008FF7C File Offset: 0x0008E17C
		internal void ChangeHeightWidthTopLeft(double width, double height, double top, double left)
		{
			try
			{
				if (base.WindowState == WindowState.Maximized)
				{
					this.RestoreWindows(false);
				}
				base.Height = height / MainWindow.sScalingFactor;
				base.Width = width / MainWindow.sScalingFactor;
				base.Top = top / MainWindow.sScalingFactor;
				base.Left = left / MainWindow.sScalingFactor;
				Sidebar sidebar = this.mSidebar;
				if (sidebar != null)
				{
					sidebar.ArrangeAllSidebarElements();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error occured setting size of the window. err:" + ex.ToString());
			}
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x0009000C File Offset: 0x0008E20C
		private void SetWindowTitle(string vmName)
		{
			base.Title = BlueStacks.Common.Utils.GetDisplayName(vmName, "bgp64");
			this.mTopBar.mTitleText.Text = base.Title;
			bool isCustomUIForNCSoft = FeatureManager.Instance.IsCustomUIForNCSoft;
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				this.mTopBar.mTitleText.Text = GameConfig.Instance.AppName;
				this.mTopBar.mTitleIcon.ImageName = global::System.IO.Path.Combine(RegistryStrings.InstallDir, "app_icon.ico");
				this.SetTaskbarProperties();
			}
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x00010672 File Offset: 0x0000E872
		internal void ShowRerollOverlay()
		{
			this.ShowDimOverlay(this.MacroOverlayControl);
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x00090098 File Offset: 0x0008E298
		internal void HandleGenericNotificationPopup(GenericNotificationItem notifItem)
		{
			GenericNotificationDesignItem designItem = notifItem.NotificationDesignItem;
			if (!RegistryManager.Instance.IsShowRibbonNotification || RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				return;
			}
			Action <>9__1;
			this.pikaNotificationWorkQueue.Enqueue(delegate
			{
				while (!this.mIsWindowInFocus || this.isPikaPopOpen)
				{
					Thread.Sleep(2000);
				}
				Dispatcher dispatcher = this.Dispatcher;
				Action action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate
					{
						this.isPikaPopOpen = true;
						this.pikaPopControl.Init(notifItem);
						Canvas.SetLeft(this.pikaPopControl, 0.0);
						this.pikaPop.IsOpen = true;
						new Storyboard();
						this.pikaCanvas.Width = this.pikaPopControl.ActualWidth;
						this.pikaPop.HorizontalOffset = this.pikaPopControl.ActualWidth * -0.5;
						PennerDoubleAnimation.Equations equations = PennerDoubleAnimation.Equations.QuadEaseInOut;
						double actualWidth = this.pikaPopControl.ActualWidth;
						double num = 0.0;
						int num2 = 700;
						Animator.AnimatePenner(this.pikaPopControl, Canvas.LeftProperty, equations, new double?(actualWidth), num, num2, null);
						string text = "Home";
						if (this.mTopBar.mAppTabButtons.SelectedTab != null)
						{
							text = this.mTopBar.mAppTabButtons.SelectedTab.AppLabel;
						}
						ClientStats.SendMiscellaneousStatsAsync("RibbonShown", RegistryManager.Instance.UserGuid, JsonConvert.SerializeObject(notifItem.ExtraPayload), text, RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, notifItem.Id, notifItem.Title, null);
					});
				}
				dispatcher.Invoke(action, new object[0]);
				this.pikaNotificationTimer.Interval = TimeSpan.FromMilliseconds(designItem.AutoHideTime);
				this.pikaNotificationTimer.Start();
			});
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x00010680 File Offset: 0x0000E880
		private void PikaNotificationTimer_Tick(object sender, EventArgs e)
		{
			this.pikaNotificationTimer.Stop();
			if (this.isPikaPopOpen)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					PennerDoubleAnimation.Equations equations = PennerDoubleAnimation.Equations.QuadEaseInOut;
					double num = 0.0;
					double actualWidth = this.pikaPopControl.ActualWidth;
					int num2 = 400;
					Animator.AnimatePenner(this.pikaPopControl, Canvas.LeftProperty, equations, new double?(num), actualWidth, num2, delegate(object s, EventArgs ev)
					{
						this.pikaPop.IsOpen = false;
						this.isPikaPopOpen = false;
					});
				}), new object[0]);
			}
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x000900FC File Offset: 0x0008E2FC
		internal void ShowDimOverlay(IDimOverlayControl el = null)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.ShowDimOverlayUIThread(el);
			}), new object[0]);
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x0009013C File Offset: 0x0008E33C
		private void ShowDimOverlayUIThread(IDimOverlayControl el = null)
		{
			try
			{
				Logger.Debug("showing dim overlay");
				if (this.mDimOverlay == null || this.mDimOverlay.IsClosed)
				{
					this.mDimOverlay = new DimOverlayControl(this);
				}
				if (PresentationSource.FromVisual(this) != null)
				{
					this.mDimOverlay.Owner = this;
					this.mDimOverlay.Control = el;
					this.mDimOverlay.UpadteSizeLocation();
					this.mDimOverlay.ShowWindow();
					this.mFrontendHandler.ShowGLWindow();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while showing dimoverlay control. " + ex.ToString());
			}
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x000106B3 File Offset: 0x0000E8B3
		internal void HideDimOverlay()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					Logger.Debug("Hide dim overlay");
					if (this.mDimOverlay != null)
					{
						if (this.mIsLockScreenActionPending)
						{
							this.ShowDimOverlay(this.ScreenLockInstance);
						}
						else
						{
							this.mDimOverlay.HideWindow(false);
							this.mDimOverlay.Control = null;
							this.mFrontendHandler.ShowGLWindow();
						}
					}
				}
				catch (Exception)
				{
				}
				base.Focus();
			}), new object[0]);
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x000901E0 File Offset: 0x0008E3E0
		private void MainWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			foreach (object obj in base.OwnedWindows)
			{
				Window window = (Window)obj;
				if (window != null)
				{
					try
					{
						CustomWindow customWindow = (CustomWindow)window;
						if (customWindow == null || customWindow.ShowWithParentWindow)
						{
							window.Visibility = base.Visibility;
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Exception in showing child windows: {0}", new object[] { ex.ToString() });
					}
				}
			}
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x00090284 File Offset: 0x0008E484
		public void MainWindow_StateChanged(object sender, EventArgs e)
		{
			if (base.WindowState != WindowState.Minimized)
			{
				this.SendTempGamepadState(true);
				try
				{
					Uri uri = new Uri(RegistryStrings.ProductIconCompletePath);
					base.Icon = BitmapFrame.Create(uri);
					SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && (string.Equals(x.VmName, this.mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
					notificationItems.Count.ToString(CultureInfo.InvariantCulture);
					if (this.IsInNotificationMode)
					{
						foreach (string text in this.AppNotificationCountDictForEachVM.Keys)
						{
							Stats.SendCommonClientStatsAsync("notification_mode", "notification_number", this.mVmName, text, this.AppNotificationCountDictForEachVM[text].ToString(CultureInfo.InvariantCulture), "NM_On");
						}
						this.AppNotificationCountDictForEachVM.Clear();
						DummyTaskbarWindow dummyWindow = this.DummyWindow;
						if (dummyWindow != null)
						{
							dummyWindow.Close();
						}
						HTTPUtils.SendRequestToAgentAsync("overrideDesktopNotificationSettings", new Dictionary<string, string> { { "override", "False" } }, this.mVmName, 0, null, false, 1, 0, "bgp64");
						this.mIsMinimizedThroughCloseButton = false;
						if (notificationItems.Count > 0 && MainWindow.sShowNotifications)
						{
							new Thread(delegate
							{
								base.Dispatcher.Invoke(new Action(delegate
								{
									BlueStacksUIBinding.BindColor(this.mTopBar.mNotificationCaret, Shape.FillProperty, "SliderButtonColor");
									BlueStacksUIBinding.BindColor(this.mTopBar.mNotificationCaret, Shape.StrokeProperty, "SliderButtonColor");
									BlueStacksUIBinding.BindColor(this.mTopBar.mNotificationCentreDropDownBorder, global::System.Windows.Controls.Control.BorderBrushProperty, "SliderButtonColor");
									this.mTopBar.mNotificationDrawerControl.mAnimationRect.Visibility = Visibility.Visible;
									this.mTopBar.mNotificationCentreButton_MouseLeftButtonUp(null, null);
								}), new object[0]);
							})
							{
								IsBackground = true
							}.Start();
						}
					}
					MainWindow.sShowNotifications = true;
					this.IsInNotificationMode = false;
					goto IL_01D9;
				}
				catch (Exception ex)
				{
					string text2 = "Error in setting window's icon: ";
					Exception ex2 = ex;
					Logger.Error(text2 + ((ex2 != null) ? ex2.ToString() : null));
					goto IL_01D9;
				}
			}
			Logger.Debug("KMP MainWindow_StateChanged " + this.mVmName);
			if (BlueStacksUIUtils.ActivatedWindow == this)
			{
				BlueStacksUIUtils.ActivatedWindow = null;
			}
			AppUsageTimer.StopTimer();
			this.mFrontendHandler.DeactivateFrontend();
			this.mCommonHandler.ClipMouseCursorHandler(true, true, "", "");
			this.mIsWindowInFocus = false;
			if (!this.IsInNotificationMode)
			{
				BlueStacksUIUtils.SetWindowTaskbarIcon(this);
			}
			IL_01D9:
			BlueStacksUIUtils.LastActivatedWindow.mFrontendHandler.UpdateOverlaySizeStatus();
			this.OnResizeMainWindow();
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x000904B4 File Offset: 0x0008E6B4
		internal void SendTempGamepadState(bool enable)
		{
			if (RegistryManager.Instance.GamepadDetectionEnabled)
			{
				if (enable)
				{
					if (!this.IsGamepadConnected)
					{
						if (this.WasGamepadStatusSkipped)
						{
							this.SkipNextGamepadStatus = true;
							this.WasGamepadStatusSkipped = false;
						}
						this.mFrontendHandler.SendFrontendRequestAsync("enableGamepad", new Dictionary<string, string> { { "enable", "true" } });
						return;
					}
				}
				else
				{
					this.SkipNextGamepadStatus = true;
					this.mFrontendHandler.SendFrontendRequestAsync("enableGamepad", new Dictionary<string, string> { { "enable", "false" } });
				}
			}
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x00090540 File Offset: 0x0008E740
		private void MainWindow_Deactivated(object sender, EventArgs e)
		{
			Logger.Debug("KMP MainWindow_Deactivated " + this.mVmName);
			if (BlueStacksUIUtils.ActivatedWindow == this)
			{
				BlueStacksUIUtils.ActivatedWindow = null;
			}
			this.ClosePopUps();
			this.mFrontendHandler.DeactivateFrontend();
			this.mCommonHandler.ClipMouseCursorHandler(true, true, "", "");
			this.mIsWindowInFocus = false;
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x000905A4 File Offset: 0x0008E7A4
		private void MainWindow_Activated(object sender, EventArgs e)
		{
			Logger.Debug("In MainWindow_Activated");
			BlueStacksUIUtils.LastActivatedWindow = this;
			BlueStacksUIUtils.ActivatedWindow = this;
			App.IsApplicationActive = true;
			this.mIsWindowInFocus = true;
			if (!string.IsNullOrEmpty(this.mVmName) && this.mTopBar != null && this.mTopBar.mAppTabButtons != null && this.mTopBar.mAppTabButtons.SelectedTab != null && !string.IsNullOrEmpty(this.mTopBar.mAppTabButtons.SelectedTab.TabKey))
			{
				AppUsageTimer.StartTimer(this.mVmName, this.mTopBar.mAppTabButtons.SelectedTab.TabKey);
			}
			if (this.mFrontendGrid.IsVisible)
			{
				Logger.Debug("KMP MainWindow_Activated focusfrontend " + this.mVmName);
				if (!this.mTopBar.mAppTabButtons.SelectedTab.mGuidanceWindowOpen || this.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastWhenGuidanceOpen)
				{
					KMManager.ShowShootingModeTooltip(this, this.mTopBar.mAppTabButtons.SelectedTab.PackageName);
				}
				else
				{
					this.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastWhenGuidanceOpen = true;
				}
			}
			else
			{
				Logger.Debug("KMP MainWindow_Activated DeactivateFrontend " + this.mVmName);
				this.mFrontendHandler.DeactivateFrontend();
			}
			this.SendTempGamepadState(true);
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x000106D3 File Offset: 0x0000E8D3
		private void MainWindow_SourceInitialized(object sender, EventArgs e)
		{
			this.Handle = ((HwndSource)PresentationSource.FromVisual(this)).Handle;
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x000106EB File Offset: 0x0000E8EB
		internal void MainWindow_ResizeBegin(object sender, EventArgs e)
		{
			this.mIsResizing = true;
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x000906F4 File Offset: 0x0008E8F4
		private void OnResizeMainWindow()
		{
			Sidebar sidebar = this.mSidebar;
			if (sidebar != null)
			{
				sidebar.SetHeight();
			}
			Action<bool> appRecommendationHandler = PromotionObject.AppRecommendationHandler;
			if (appRecommendationHandler != null)
			{
				appRecommendationHandler(false);
			}
			this.mShootingModePopup.HorizontalOffset += 1.0;
			this.mShootingModePopup.HorizontalOffset -= 1.0;
			this.mFullScreenToastPopup.HorizontalOffset += 1.0;
			this.mFullScreenToastPopup.HorizontalOffset -= 1.0;
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x00090790 File Offset: 0x0008E990
		internal void MainWindow_ResizeEnd(object sender, EventArgs e)
		{
			this.mIsResizing = false;
			if (base.WindowState == WindowState.Normal)
			{
				try
				{
					this.EngineInstanceRegistry.WindowPlacement = this.GetPlacement();
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in MainWindow_ResizeEnd. Exception: " + ex.ToString());
				}
			}
			this.UIChangesOnMainWindowSizeChanged();
			this.mFrontendHandler.ShowGLWindow();
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000106F4 File Offset: 0x0000E8F4
		internal void ChangeWindowOrientaion(object sender, ChangeOrientationEventArgs e)
		{
			this.SwitchToPortraitMode(e.IsPotrait);
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x000907F8 File Offset: 0x0008E9F8
		private void SetupInitialSize()
		{
			this.mAspectRatio = new Fraction((long)this.EngineInstanceRegistry.GuestWidth, (long)this.EngineInstanceRegistry.GuestHeight);
			this.mPreviousWidth = new double?(base.Width);
			this.mPreviousHeight = new double?(base.Height);
			this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight(BlueStacksUIUtils.GetDefaultHeight(), false, false), BlueStacksUIUtils.GetDefaultHeight(), true);
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x00010702 File Offset: 0x0000E902
		internal void ChangeOrientationFromClient(bool isPortrait, bool stopFurtherOrientation = true)
		{
			new Thread(delegate
			{
				if (BlueStacks.Common.Utils.IsGuestBooted(this.mVmName, "bgp64"))
				{
					this.SwitchOrientationFromClient(isPortrait, stopFurtherOrientation);
					this.SendOrientationChangeToAndroid(isPortrait);
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x00090864 File Offset: 0x0008EA64
		private void SendOrientationChangeToAndroid(bool isPortrait)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { 
				{
					"d",
					isPortrait ? "1" : "0"
				} };
				HTTPUtils.SendRequestToGuest("guestorientation", dictionary, this.mVmName, 0, null, false, 1, 0, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending GuestOrientation to android: " + ex.ToString());
			}
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x000908D8 File Offset: 0x0008EAD8
		private void SwitchOrientationFromClient(bool orientation, bool stopFurtherOrientation)
		{
			try
			{
				string text = (orientation ? "1" : "0");
				string text2 = (stopFurtherOrientation ? "1" : "0");
				string text3 = (this.StaticComponents.mPreviousSelectedTabWeb ? "1" : "0");
				string text4 = string.Concat(new string[]
				{
					"orientation=",
					text,
					"&package=",
					this.mTopBar.mAppTabButtons.SelectedTab.PackageName,
					"&stopFurtherOrientationChange=",
					text2,
					"&isPreviousSelectedTabWeb=",
					text3
				});
				string text5 = string.Format(CultureInfo.InvariantCulture, "{0}?{1}", new object[] { "switchOrientation", text4 });
				BstHttpClient.Get(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
				{
					string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
					{
						"http://127.0.0.1",
						RegistryManager.Instance.Guest[this.mVmName].FrontendServerPort
					}),
					text5
				}), null, false, this.mVmName, 0, 1, 0, false, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending switch orientation from client: " + ex.ToString());
			}
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x00090A40 File Offset: 0x0008EC40
		internal void HandleDisplaySettingsChanged()
		{
			try
			{
				if (PresentationSource.FromVisual(this) != null)
				{
					MainWindow.sScalingFactor = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
				}
				this.MinWidthScaled = (int)(base.MinWidth * MainWindow.sScalingFactor);
				this.MinHeightScaled = (int)(base.MinHeight * MainWindow.sScalingFactor);
				this.heightDiffScaled = (int)((double)this.ParentWindowHeightDiff * MainWindow.sScalingFactor);
				this.widthDiffScaled = (int)((double)this.ParentWindowWidthDiff * MainWindow.sScalingFactor);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
			}
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x00090AEC File Offset: 0x0008ECEC
		internal void ShowWindow(bool updateBootStartTime = false)
		{
			if (this.mIsWindowLoadedOnce)
			{
				Action <>9__2;
				WaitCallback <>9__1;
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (FeatureManager.Instance.IsCustomUIForDMM)
					{
						KMManager.ShowOverlayWindow(this, false, false);
					}
					this.ShowInTaskbar = true;
					this.Visibility = Visibility.Visible;
					this.Show();
					this.BringIntoView();
					if (this.WindowState == WindowState.Minimized)
					{
						InteropWindow.ShowWindow(this.Handle, 9);
					}
					if (FeatureManager.Instance.IsCustomUIForDMM && this.mDMMRecommendedWindow == null)
					{
						this.mDMMRecommendedWindow = new DMMRecommendedWindow(this);
						this.mDMMRecommendedWindow.Init(RegistryManager.Instance.DMMRecommendedWindowUrl);
						this.mDMMRecommendedWindow.Visibility = Visibility.Visible;
					}
					if (!this.Topmost)
					{
						this.Topmost = true;
						WaitCallback waitCallback;
						if ((waitCallback = <>9__1) == null)
						{
							waitCallback = (<>9__1 = delegate(object obj)
							{
								Dispatcher dispatcher = this.Dispatcher;
								Action action;
								if ((action = <>9__2) == null)
								{
									action = (<>9__2 = delegate
									{
										this.Topmost = false;
									});
								}
								dispatcher.Invoke(action, new object[0]);
							});
						}
						ThreadPool.QueueUserWorkItem(waitCallback);
					}
					if (updateBootStartTime)
					{
						this.mBootStartTime = DateTime.Now;
					}
				}), new object[0]);
			}
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x00090B34 File Offset: 0x0008ED34
		internal double GetWidthFromHeight(double height, bool isScaled = false, bool isIgnoreMinWidth = false)
		{
			if (this.IsUIInPortraitMode)
			{
				if (isScaled)
				{
					try
					{
						return Math.Max((height - (double)this.heightDiffScaled) / this.mAspectRatio.DoubleValue + (double)this.widthDiffScaled, (double)(isIgnoreMinWidth ? 0 : this.MinWidthScaled));
					}
					catch
					{
					}
				}
				return Math.Max((height - (double)this.ParentWindowHeightDiff) / this.mAspectRatio.DoubleValue + (double)this.ParentWindowWidthDiff, isIgnoreMinWidth ? 0.0 : base.MinWidth);
			}
			if (isScaled)
			{
				try
				{
					return Math.Max((height - (double)this.heightDiffScaled) * this.mAspectRatio.DoubleValue + (double)this.widthDiffScaled, (double)(isIgnoreMinWidth ? 0 : this.MinWidthScaled));
				}
				catch
				{
				}
			}
			return Math.Max((height - (double)this.ParentWindowHeightDiff) * this.mAspectRatio.DoubleValue + (double)this.ParentWindowWidthDiff, isIgnoreMinWidth ? 0.0 : base.MinWidth);
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x00090C48 File Offset: 0x0008EE48
		internal double GetHeightFromWidth(double width, bool isScaled = false, bool isIgnoreMinWidth = false)
		{
			if (this.IsUIInPortraitMode)
			{
				if (isScaled)
				{
					try
					{
						return Math.Max((width - (double)this.widthDiffScaled) * this.mAspectRatio.DoubleValue + (double)this.heightDiffScaled, (double)(isIgnoreMinWidth ? 0 : this.MinHeightScaled));
					}
					catch
					{
					}
				}
				return Math.Max((width - (double)this.ParentWindowWidthDiff) * this.mAspectRatio.DoubleValue + (double)this.ParentWindowHeightDiff, isIgnoreMinWidth ? 0.0 : base.MinHeight);
			}
			if (isScaled)
			{
				try
				{
					return Math.Max((width - (double)this.widthDiffScaled) / this.mAspectRatio.DoubleValue + (double)this.heightDiffScaled, (double)(isIgnoreMinWidth ? 0 : this.MinHeightScaled));
				}
				catch
				{
				}
			}
			return Math.Max((width - (double)this.ParentWindowWidthDiff) / this.mAspectRatio.DoubleValue + (double)this.ParentWindowHeightDiff, isIgnoreMinWidth ? 0.0 : base.MinHeight);
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x0001073A File Offset: 0x0000E93A
		private void MainWindow_PreviewMouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			if (!this.mIsResizing)
			{
				base.Cursor = global::System.Windows.Input.Cursors.Arrow;
			}
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x0001074F File Offset: 0x0000E94F
		private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			DMMRecommendedWindow dmmrecommendedWindow = this.mDMMRecommendedWindow;
			if (dmmrecommendedWindow != null)
			{
				dmmrecommendedWindow.UpdateSize();
			}
			this.OnResizeMainWindow();
			GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
			if (sGuidanceWindow != null)
			{
				sGuidanceWindow.UpdateSize();
			}
			this.mTopBar.ClosePopups();
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x00010783 File Offset: 0x0000E983
		private void MainWindow_LocationChanged(object sender, EventArgs e)
		{
			DMMRecommendedWindow dmmrecommendedWindow = this.mDMMRecommendedWindow;
			if (dmmrecommendedWindow != null)
			{
				dmmrecommendedWindow.UpdateLocation();
			}
			this.OnResizeMainWindow();
			GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
			if (sGuidanceWindow == null)
			{
				return;
			}
			sGuidanceWindow.UpdateSize();
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x00090D5C File Offset: 0x0008EF5C
		internal void RestartInstanceAndPerform(EventHandler action = null)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (action != null)
				{
					this.mFrontendHandler.mEventOnFrontendClosed -= action;
					this.mFrontendHandler.mEventOnFrontendClosed += action;
				}
				this.mFrontendHandler.mEventOnFrontendClosed -= this.FrontendHandler_RunInstance;
				this.mFrontendHandler.mEventOnFrontendClosed += this.FrontendHandler_RunInstance;
				this.CloseCurrentInstanceForRestart();
			}), new object[0]);
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x00090D9C File Offset: 0x0008EF9C
		internal void CloseAllWindowAndPerform(EventHandler action = null)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (action != null)
				{
					this.mEventOnAllWindowClosed -= action;
					this.mEventOnAllWindowClosed += action;
				}
				this.ForceCloseWindow(true);
			}), new object[0]);
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x000107AB File Offset: 0x0000E9AB
		internal void FrontendHandler_RunInstance(object sender, EventArgs e)
		{
			this.CloseMainWindow();
			BlueStacksUIUtils.RunInstance(this.mVmName, false);
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x00090DDC File Offset: 0x0008EFDC
		internal void CloseMainWindow()
		{
			HTTPUtils.SendRequestToAgentAsync("instanceStopped", null, this.mVmName, 0, null, false, 1, 0, "bgp64");
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (BlueStacksUIUtils.DictWindows.Keys.Count == 1)
				{
					Publisher.PublishMessage(BrowserControlTags.appPlayerClosing, BlueStacksUIUtils.DictWindows.First<KeyValuePair<string, MainWindow>>().Key, null);
				}
				this.mGuestBootCompleted = false;
				this.mClosing = true;
				base.Close();
			}), new object[0]);
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x000107BF File Offset: 0x0000E9BF
		internal void CloseCurrentInstanceForRestart()
		{
			this.mIsRestart = true;
			Opt.Instance.Json = "";
			this.ForceCloseWindow(false);
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x00090E24 File Offset: 0x0008F024
		internal void ForceCloseWindow(bool isWaitForPlayerClosing = false)
		{
			try
			{
				this.CloseWindowHandler(isWaitForPlayerClosing);
			}
			catch (Exception ex)
			{
				Logger.Error("Error occured in ForceClose" + ex.ToString());
			}
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x00090E64 File Offset: 0x0008F064
		internal void CloseWindow()
		{
			Logger.Info("Calling FpsBind.OnBlueStacksClosing to lock FPS and show notification");
			FpsBind.OnBlueStacksClosing();
			Thread.Sleep(1000);
			BlueStacks.BlueStacksUI.ProgressBar progressBar = new string[] { "HD-Player", "HD-Agent", "BstkSVC", "BlueStacks" };
			for (string text = 0; text < progressBar.Length; text++)
			{
				string[] array = progressBar[text];
				int processesByName = Process.GetProcessesByName(array);
				for (int i = 0; i < processesByName.Length; i++)
				{
					Process process = processesByName[i];
					process.Kill();
					process.WaitForExit(1000);
					Logger.Info("Process " + array + " terminated successfully.");
				}
			}
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x00090F78 File Offset: 0x0008F178
		private void BlueStacksAdvancedExitAcceptedHandler(object sender, MouseButtonEventArgs e)
		{
			string quitDefaultOption = RegistryManager.Instance.QuitDefaultOption;
			if (quitDefaultOption != null && !(quitDefaultOption == "STRING_CLOSE_CURRENT_INSTANCE"))
			{
				if (quitDefaultOption == "STRING_CLOSE_ALL_RUNNING_INSTANCES")
				{
					BlueStacks.Common.Utils.StopClientInstanceAsync("");
					return;
				}
				if (quitDefaultOption == "STRING_RESTART_CURRENT_INSTANCE")
				{
					BlueStacksUIUtils.RestartInstance(this.mVmName);
					return;
				}
			}
			this.CloseWindowHandler(false);
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x000107DE File Offset: 0x0000E9DE
		private void BlueStacksAdvancedExitDeclinedHandler(object sender, MouseButtonEventArgs e)
		{
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition && this.mGuestBootCompleted)
			{
				this.mTopBar.mAppTabButtons.AddHiddenAppTabAndLaunch(GameConfig.Instance.PkgName, GameConfig.Instance.ActivityName);
			}
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x000107DE File Offset: 0x0000E9DE
		private void CloseWindowConfirmationDeniedHandler(object sender, EventArgs e)
		{
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition && this.mGuestBootCompleted)
			{
				this.mTopBar.mAppTabButtons.AddHiddenAppTabAndLaunch(GameConfig.Instance.PkgName, GameConfig.Instance.ActivityName);
			}
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x00010819 File Offset: 0x0000EA19
		internal void MainWindow_CloseWindowConfirmationAcceptedHandler(object sender, EventArgs e)
		{
			this.CloseWindowHandler(false);
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x00090FD8 File Offset: 0x0008F1D8
		private void MainWindow_CloseWindowConfirmationResetAccountAcceptedHandler(object sender, EventArgs e)
		{
			if (base.Visibility == Visibility.Visible)
			{
				this.mFrontendGrid.Visibility = Visibility.Hidden;
				if (this.mIsRestart)
				{
					this.mExitProgressGrid.ProgressText = "STRING_RESTARTING";
				}
				else
				{
					this.mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
				}
				this.mExitProgressGrid.Visibility = Visibility.Visible;
			}
			this.mAppHandler.SendRequestToRemoveAccountAndCloseWindowASync(true);
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x00010822 File Offset: 0x0000EA22
		internal void ShowDimOverlayForUpgrade()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.CloseChildOwnedWindows();
				this.GotoHomeTab();
				this.mWelcomeTab.mFrontendPopupControl.HideWindow();
				this.mExitProgressGrid.ProgressText = "STRING_UPGRADING_TEXT";
				this.mExitProgressGrid.Visibility = Visibility.Visible;
				this.HideDimOverlay();
			}), new object[0]);
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x0009103C File Offset: 0x0008F23C
		private void CloseChildOwnedWindows()
		{
			foreach (object obj in base.OwnedWindows)
			{
				Window window = (Window)obj;
				if (window != null)
				{
					foreach (object obj2 in window.OwnedWindows)
					{
						Window window2 = (Window)obj2;
						if (window2 != null)
						{
							window2.Close();
						}
					}
					window.Close();
				}
			}
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x000910E8 File Offset: 0x0008F2E8
		private void GotoHomeTab()
		{
			if (!FeatureManager.Instance.IsCustomUIForDMM && !this.mTopBar.mAppTabButtons.GoToTab("Home", true, false))
			{
				Logger.Info("Test logs: GotoHomeTab()");
				this.mTopBar.mAppTabButtons.AddHomeTab();
				this.mTopBar.mAppTabButtons.CloseTab("Setup", false, true, false, false, "");
			}
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x00010842 File Offset: 0x0000EA42
		internal void MinimizeWindowHandler()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					this.IsInNotificationMode = true;
					this.mIsMinimizedThroughCloseButton = true;
					HTTPUtils.SendRequestToAgentAsync("overrideDesktopNotificationSettings", new Dictionary<string, string> { { "override", "True" } }, this.mVmName, 0, null, false, 1, 0, "bgp64");
					Dictionary<string, AppTabButton> dictionary = new Dictionary<string, AppTabButton>();
					foreach (KeyValuePair<string, AppTabButton> keyValuePair in this.mTopBar.mAppTabButtons.mDictTabs)
					{
						dictionary.Add(keyValuePair.Key, keyValuePair.Value);
					}
					foreach (KeyValuePair<string, AppTabButton> keyValuePair2 in dictionary)
					{
						this.mTopBar.mAppTabButtons.CloseTabAfterQuitPopup(keyValuePair2.Key, true, false);
					}
					BlueStacksUIUtils.HideUnhideParentWindow(true, this);
				}
				catch (Exception ex)
				{
					Logger.Error("Error occured in MinimizeWindowHandler " + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x00091154 File Offset: 0x0008F354
		internal void CloseWindowHandler(bool isWaitForPlayerClosing = false)
		{
			if (this.mClosed)
			{
				return;
			}
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					if (!this.mClosed)
					{
						HTTPUtils.SendRequestToAgentAsync("notificationStatsOnClosing", null, this.mVmName, 0, null, false, 1, 0, "bgp64");
						this.CloseChildOwnedWindows();
						if (CommonHandlers.sIsRecordingVideo && string.Equals(CommonHandlers.sRecordingInstance, this.mVmName, StringComparison.InvariantCulture))
						{
							this.mCommonHandler.StopRecordVideo();
						}
						this.GotoHomeTab();
						if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
						{
							this.mWelcomeTab.mBackground.Visibility = Visibility.Visible;
						}
						this.mWelcomeTab.mFrontendPopupControl.HideWindow();
						if (this.mIsRestart)
						{
							this.mExitProgressGrid.ProgressText = "STRING_RESTARTING";
						}
						else
						{
							this.mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
						}
						this.mExitProgressGrid.Visibility = Visibility.Visible;
						this.HideDimOverlay();
						this.mClosed = true;
						if (!this.mIsRestart)
						{
							this.mFrontendHandler.mEventOnFrontendClosed -= this.FrontendHandler_CloseMainWindow;
							this.mFrontendHandler.mEventOnFrontendClosed += this.FrontendHandler_CloseMainWindow;
						}
						if (isWaitForPlayerClosing)
						{
							this.mFrontendHandler.KillFrontendAsync();
						}
						else
						{
							this.mFrontendHandler.KillFrontend(false);
						}
						if (this.mDiscordhandler != null)
						{
							this.mDiscordhandler.Dispose();
							this.mDiscordhandler = null;
						}
						if (SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.mVmName))
						{
							SecurityMetrics.SecurityMetricsInstanceList[this.mVmName].SendSecurityBreachesStatsToCloud(true);
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Error occured in CloseWindowHandler " + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x00010862 File Offset: 0x0000EA62
		private void FrontendHandler_CloseMainWindow(object sender, EventArgs e)
		{
			this.CloseMainWindow();
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x0009119C File Offset: 0x0008F39C
		private void UpdateSynchronizerInstancesList()
		{
			try
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
					{
						if (keyValuePair.Value.mSynchronizerWindow != null && keyValuePair.Value.mSynchronizerWindow.IsVisible)
						{
							keyValuePair.Value.mSynchronizerWindow.Init();
						}
					}
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in updating instances for sync operation: " + ex.ToString());
			}
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x00091204 File Offset: 0x0008F404
		private void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			if (this.mClosing)
			{
				this.mClosing = false;
				if (!Opt.Instance.h && base.WindowState == WindowState.Normal)
				{
					try
					{
						this.EngineInstanceRegistry.WindowPlacement = this.GetPlacement();
					}
					catch (Exception ex)
					{
						string text = "Exception in MainWindow_Closing. Exception: ";
						Exception ex2 = ex;
						Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
					}
				}
				BlueStacksUIUtils.DictWindows[this.mVmName].mWelcomeTab.mHomeAppManager.DisposeHtmlSidePanel();
				BlueStacksUIUtils.DictWindows[this.mVmName].mWelcomeTab.DisposeHtmHomeBrowser();
				BlueStacksUIUtils.DictWindows.Remove(this.mVmName);
				this.UpdateSynchronizationState();
				this.UpdateSynchronizerInstancesList();
				if (this.mVmName == "Android")
				{
					BlueStacksUpdater.DownloadCompleted -= this.BlueStacksUpdater_DownloadCompleted;
				}
				EventHandler eventHandler = this.mEventOnInstanceClosed;
				if (eventHandler != null)
				{
					eventHandler(this.mVmName, null);
				}
				this.ReleaseClientGlobalLock();
				if (BlueStacksUIUtils.DictWindows.Count == 0 && !this.mIsRestart)
				{
					AppUsageTimer.SaveData();
					GlobalKeyBoardMouseHooks.UnHookGlobalHooks();
					App.UnwindEvents();
					App.ReleaseLock();
					EventHandler eventHandler2 = this.mEventOnAllWindowClosed;
					if (eventHandler2 != null)
					{
						eventHandler2(this.mVmName, null);
					}
					if (HttpHandlerSetup.Server != null)
					{
						HttpHandlerSetup.Server.Stop();
					}
					BlueStacksUIUtils.sStopStatSendingThread = true;
					if (!ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_MultiInstanceManager_Lockbgp64"))
					{
						if (MainWindow.sIsClosingForBackupRestore)
						{
							BlueStacks.Common.Utils.RunHDQuit(false, true, false);
						}
						else
						{
							BlueStacks.Common.Utils.RunHDQuit(false, true, true);
						}
					}
					global::System.Windows.Application.Current.Shutdown();
				}
				this.mIsRestart = false;
				return;
			}
			e.Cancel = true;
			this.CloseWindow();
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x000913AC File Offset: 0x0008F5AC
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.HandleDisplaySettingsChanged();
			if (string.IsNullOrEmpty(this.EngineInstanceRegistry.WindowPlacement) || !RegistryManager.Instance.IsRememberWindowPositionEnabled)
			{
				base.Left = (SystemParameters.MaximizedPrimaryScreenWidth - base.Width) / 2.0;
				base.Top = (SystemParameters.MaximizedPrimaryScreenHeight - base.Height) / 2.0;
			}
			else
			{
				this.SetPlacement(this.EngineInstanceRegistry.WindowPlacement);
			}
			bool flag = false;
			IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, true);
			if ((base.Left + base.Width + (double)(this.EngineInstanceRegistry.IsSidebarVisible ? 62 : 0)) * MainWindow.sScalingFactor > (double)(fullscreenMonitorSize.Left + fullscreenMonitorSize.Width))
			{
				base.Left = (double)(fullscreenMonitorSize.X + fullscreenMonitorSize.Width) / MainWindow.sScalingFactor - base.Width - 62.0;
				if (base.Left < 0.0)
				{
					base.Left = 0.0;
				}
				flag = true;
			}
			if ((base.Top + base.Height) * MainWindow.sScalingFactor > (double)(fullscreenMonitorSize.Top + fullscreenMonitorSize.Height))
			{
				base.Top = (double)(fullscreenMonitorSize.Y + fullscreenMonitorSize.Height) / MainWindow.sScalingFactor - base.Height;
				if (base.Top < 0.0)
				{
					base.Top = 0.0;
				}
				flag = true;
			}
			if (flag)
			{
				Sidebar sidebar = this.mSidebar;
				if (sidebar != null)
				{
					sidebar.ArrangeAllSidebarElements();
				}
			}
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				double num = (FeatureManager.Instance.IsCustomUIForDMM ? ((base.Height - (double)this.ParentWindowHeightDiff) * 9.0 / 16.0 + (double)this.ParentWindowWidthDiff) : 0.0);
				base.Left -= num / 2.0;
			}
			this.mTopBar.mPreferenceDropDownControl.Init(this);
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mNCTopBar.mSettingsDropDownControl.Init(this);
			}
			this.mFullScreenTopBar.Init(this);
			this.CloseWindowConfirmationAcceptedHandler += this.MainWindow_CloseWindowConfirmationAcceptedHandler;
			this.CloseWindowConfirmationResetAccountAcceptedHandler += this.MainWindow_CloseWindowConfirmationResetAccountAcceptedHandler;
			this.RestartEngineConfirmationAcceptedHandler += this.MainWindow_RestartEngineConfirmationAcceptedHandler;
			this.RestartPcConfirmationAcceptedHandler += this.MainWindow_RestartPcConfirmationHandler;
			GlobalKeyBoardMouseHooks.SetBossKeyHook();
			this.mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium);
			if (this.IsDefaultVM)
			{
				this.pikaNotificationTimer.Interval = TimeSpan.FromMilliseconds(3500.0);
				this.pikaNotificationTimer.Tick += this.PikaNotificationTimer_Tick;
				this.pikaPopControl.ParentWindow = this;
				this.pikaNotificationWorkQueue.Start();
			}
			this.ClientLaunchedStats();
			this.toastTimer.Interval = TimeSpan.FromMilliseconds(3000.0);
			this.toastTimer.Tick += this.ToastTimer_Tick;
			this.mFullScreenToastTimer.Interval = TimeSpan.FromMilliseconds(5000.0);
			this.mFullScreenToastTimer.Tick += this.FullScreenToastTimer_Tick;
			this.SetMaxSizeOfWindow();
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x000916FC File Offset: 0x0008F8FC
		private void ClientLaunchedStats()
		{
			if (RegistryManager.Instance.IsClientUpgraded && RegistryManager.Instance.IsClientFirstLaunch == 1)
			{
				ClientStats.SendClientStatsAsync("update_init", "success", "emulator_activity", "", "", this.mVmName);
				return;
			}
			if (RegistryManager.Instance.IsClientFirstLaunch == 1)
			{
				ClientStats.SendClientStatsAsync("first_init", "success", "emulator_activity", "", "", this.mVmName);
				return;
			}
			ClientStats.SendClientStatsAsync("init", "success", "emulator_activity", "", "", this.mVmName);
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0001086A File Offset: 0x0000EA6A
		internal void CreateFirebaseBrowserControl()
		{
			Logger.Info("In CreateFirebaseBrowserControl");
			(this.FirebaseBrowserControlGrid.Children[0] as BrowserControl).CreateNewBrowser();
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x000917A0 File Offset: 0x0008F9A0
		private void MainWindow_ContentRendered(object sender, EventArgs e)
		{
			if (!this.isSetupDone)
			{
				this.isSetupDone = true;
				if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					if (!this.Utils.IsRequiredFreeRAMAvailable())
					{
						this.mFrontendHandler.mIsSufficientRAMAvailable = false;
						this.mFrontendHandler.FrontendHandler_ShowLowRAMMessage();
					}
					else
					{
						this.Utils.CheckGuestFailedAsync();
					}
					if (this.mVmName == Strings.CurrentDefaultVmName)
					{
						BlueStacksUpdater.DownloadCompleted += this.BlueStacksUpdater_DownloadCompleted;
						BlueStacksUpdater.SetupBlueStacksUpdater(this, true);
					}
				}
				base.ContentRendered -= this.MainWindow_ContentRendered;
			}
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x00010891 File Offset: 0x0000EA91
		private void BlueStacksUpdater_DownloadCompleted(BlueStacks.Common.Tuple<BlueStacksUpdateData, bool> result)
		{
			if (result.Item1.IsUpdateAvailble && result.Item1.IsFullInstaller)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.ShowInstallPopup();
				}), new object[0]);
			}
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x00091838 File Offset: 0x0008FA38
		public void ShowInstallPopup()
		{
			this.ShowDimOverlay(null);
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_UPDATE_AVAILABLE", "");
			customMessageWindow.ImageName = "update_icon";
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_NEW_UPDATE_READY", "");
			customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
			BlueStacksUIBinding.Bind(customMessageWindow.BodyWarningTextBlock, "STRING_NEW_UPDATE_READY_WARNING", "");
			customMessageWindow.BodyWarningTextBlock.Foreground = new SolidColorBrush((global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString("#F09200"));
			customMessageWindow.UrlTextBlock.Visibility = Visibility.Visible;
			customMessageWindow.UrlLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
			customMessageWindow.UrlLink.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
			customMessageWindow.UrlLink.RequestNavigate += this.OpenRecentChangelogs;
			customMessageWindow.CloseButtonHandle(delegate(object s, EventArgs ev)
			{
				ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupCross, "");
			}, null);
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_INSTALL_NOW", delegate(object s, EventArgs ev)
			{
				ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupNow, "");
				BlueStacksUpdater.CheckDownloadedUpdateFileAndUpdate();
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_INSTALL_NEXT_BOOT", delegate(object s, EventArgs ev)
			{
				ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupLater, "");
			}, null, false, null);
			customMessageWindow.Owner = this.mDimOverlay;
			customMessageWindow.ShowDialog();
			this.HideDimOverlay();
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0000C84D File Offset: 0x0000AA4D
		private void OpenRecentChangelogs(object sender, RequestNavigateEventArgs e)
		{
			BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
			e.Handled = true;
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x000108CB File Offset: 0x0000EACB
		private void MainWindow_PreviewKeyUp(object sender, global::System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Snapshot || e.Key == Key.O)
			{
				this.HandleKeyDown(e.Key);
			}
			if (e.SystemKey == Key.Snapshot)
			{
				this.HandleKeyDown(e.SystemKey);
			}
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x00010903 File Offset: 0x0000EB03
		private void MainWindow_PreviewKeyDown(object sender, global::System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.System)
			{
				this.HandleKeyDown(e.SystemKey);
				return;
			}
			this.HandleKeyDown(e.Key);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x000919BC File Offset: 0x0008FBBC
		internal static string GetShortcutKey(Key key)
		{
			string text = string.Empty;
			if (key != Key.None)
			{
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				{
					text = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
				}
				if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
				{
					text = text + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
				}
				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					text = text + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
				}
				text += IMAPKeys.GetStringForFile(key);
			}
			return text;
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x00091A50 File Offset: 0x0008FC50
		internal void HandleKeyDown(Key key)
		{
			string shortcutKey = MainWindow.GetShortcutKey(key);
			Logger.Debug("SHORTCUT: KeyPressed.." + shortcutKey);
			if (this.mCommonHandler.mShortcutsConfigInstance != null)
			{
				foreach (ShortcutKeys shortcutKeys in this.mCommonHandler.mShortcutsConfigInstance.Shortcut)
				{
					if (string.Equals(shortcutKeys.ShortcutKey, shortcutKey, StringComparison.InvariantCulture))
					{
						ClientHotKeys clientHotKeys = (ClientHotKeys)Enum.Parse(typeof(ClientHotKeys), shortcutKeys.ShortcutName);
						this.HandleClientHotKey(clientHotKeys);
						Logger.Debug("SHORTCUT: Shortcut Name.." + shortcutKeys.ShortcutName);
					}
				}
			}
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x00091B10 File Offset: 0x0008FD10
		internal void HandleClientHotKey(ClientHotKeys clienthotKey)
		{
			try
			{
				switch (clienthotKey)
				{
				case ClientHotKeys.STRING_TOGGLE_KEYMAP_WINDOW:
					ThreadPool.QueueUserWorkItem(delegate(object obj)
					{
						try
						{
							base.Dispatcher.Invoke(new Action(delegate
							{
								if (this.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
								{
									this.mCommonHandler.GameGuideButtonHandler("shortcut", "sidebar");
								}
							}), new object[0]);
						}
						catch
						{
						}
					});
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOGGLE_FARM_MODE:
					if (!FeatureManager.Instance.IsFarmingModeDisabled)
					{
						CommonHandlers.ToggleFarmMode(!RegistryManager.Instance.CurrentFarmModeStatus);
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "FarmMode", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.AddWebTab:
					if (!FeatureManager.Instance.IsCustomUIForDMM)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							try
							{
								base.Dispatcher.Invoke(new Action(delegate
								{
									this.mTopBar.mAppTabButtons.AddWebTab("https://www.google.com/", "Google", string.Empty, true, DateTime.Now.ToString(CultureInfo.InvariantCulture), true);
								}), new object[0]);
							}
							catch (Exception ex4)
							{
								Logger.Error("Exception while ading web tab using key shortcut:{0}", new object[] { ex4 });
							}
						});
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOGGLE_OVERLAY:
					ThreadPool.QueueUserWorkItem(delegate(object obj)
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							if (FeatureManager.Instance.IsCustomUIForDMM)
							{
								this.mDmmBottomBar.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(null, null);
								return;
							}
							if (RegistryManager.Instance.TranslucentControlsTransparency != 0.0)
							{
								ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "toggleOff", null);
								KMManager.ShowOverlayWindow(this, false, false);
								this.mCommonHandler.OnOverlayStateChanged(false);
								return;
							}
							if (!KMManager.CheckIfKeymappingWindowVisible(false) && this.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
							{
								ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "toggleOn", null);
								this.mCommonHandler.OnOverlayStateChanged(true);
								KMManager.ShowOverlayWindow(this, true, false);
							}
						}), new object[0]);
					});
					goto IL_0CD4;
				case ClientHotKeys.STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP:
					ThreadPool.QueueUserWorkItem(delegate(object obj)
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							if (this.mCommonHandler != null && !this.mStreamingModeEnabled)
							{
								this.mCommonHandler.FullScreenButtonHandler("sidebar", "shortcut");
							}
						}), new object[0]);
					});
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOGGLE_LOCK_CURSOR:
					if (!FeatureManager.Instance.IsCustomUIForDMM)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							base.Dispatcher.Invoke(new Action(delegate
							{
								if (this.mCommonHandler != null)
								{
									this.mCommonHandler.ClipMouseCursorHandler(false, true, "shortcut", "sidebar");
								}
							}), new object[0]);
						});
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.RestoreWindow:
					if (this.mIsFullScreen && RegistryManager.Instance.UseEscapeToExitFullScreen)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							base.Dispatcher.Invoke(new Action(delegate
							{
								this.RestoreWindows(false);
							}), new object[0]);
						});
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOGGLE_KEYMAPPING_STATE:
					if (this.mTopBar.mAppTabButtons.SelectedTab != null && this.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							base.Dispatcher.Invoke(new Action(delegate
							{
								if (FeatureManager.Instance.IsCustomUIForDMM)
								{
									this.mCommonHandler.DMMSwitchKeyMapButtonHandler();
									return;
								}
								this.mSidebar.KeyMapSwitchButtonHandler(null);
							}), new object[0]);
						});
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_TRANSLATOR_TOOL:
					if (!FeatureManager.Instance.IsCustomUIForDMM)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							base.Dispatcher.Invoke(new Action(delegate
							{
								try
								{
									if (this.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
									{
										this.mCommonHandler.ImageTranslationHandler();
										ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "translatorTool", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
									}
								}
								catch (Exception ex5)
								{
									Logger.Error("error while calling image translation function.." + ex5.ToString());
								}
							}), new object[0]);
						});
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_ALWAYS_ON_TOP:
					this.EngineInstanceRegistry.IsClientOnTop = !this.EngineInstanceRegistry.IsClientOnTop;
					base.Topmost = this.EngineInstanceRegistry.IsClientOnTop;
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, this.EngineInstanceRegistry.IsClientOnTop ? "PinToTopOn" : "PinToTopOff", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				case ClientHotKeys.STRING_INCREASE_VOLUME:
					if (this.mSidebar != null && this.mSidebar.GetElementFromTag("sidebar_volume") != null && this.mSidebar.GetElementFromTag("sidebar_volume").Visibility == Visibility.Visible && this.mSidebar.GetElementFromTag("sidebar_volume").IsEnabled)
					{
						if (this.mSidebar.mVolumeSliderPopupTimer == null)
						{
							this.mSidebar.mVolumeSliderPopupTimer = new DispatcherTimer
							{
								Interval = new TimeSpan(0, 0, 2)
							};
							this.mSidebar.mVolumeSliderPopupTimer.Tick += this.mSidebar.VolumeSliderPopupTimer_Tick;
						}
						else
						{
							this.mSidebar.mVolumeSliderPopupTimer.Stop();
						}
						this.mSidebar.mVolumeSliderPopupTimer.Start();
						this.mSidebar.mVolumeSliderPopup.IsOpen = true;
						this.Utils.VolumeUpHandler();
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeUp", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_DECREASE_VOLUME:
					if (this.mSidebar != null && this.mSidebar.GetElementFromTag("sidebar_volume") != null && this.mSidebar.GetElementFromTag("sidebar_volume").Visibility == Visibility.Visible && this.mSidebar.GetElementFromTag("sidebar_volume").IsEnabled)
					{
						if (this.mSidebar.mVolumeSliderPopupTimer == null)
						{
							this.mSidebar.mVolumeSliderPopupTimer = new DispatcherTimer
							{
								Interval = new TimeSpan(0, 0, 2)
							};
							this.mSidebar.mVolumeSliderPopupTimer.Tick += this.mSidebar.VolumeSliderPopupTimer_Tick;
						}
						else
						{
							this.mSidebar.mVolumeSliderPopupTimer.Stop();
						}
						this.mSidebar.mVolumeSliderPopupTimer.Start();
						this.mSidebar.mVolumeSliderPopup.IsOpen = true;
						this.Utils.VolumeDownHandler();
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeDown", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOGGLE_MUTE_STATE:
					if (this.mSidebar != null)
					{
						this.mCommonHandler.MuteUnmuteButtonHanlder();
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, this.EngineInstanceRegistry.IsMuted ? "VolumeOn" : "VolumeOff", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOOLBAR_CAMERA:
					if (this.mSidebar != null && this.mSidebar.GetElementFromTag("sidebar_screenshot") != null && this.mSidebar.GetElementFromTag("sidebar_screenshot").Visibility == Visibility.Visible && this.mSidebar.GetElementFromTag("sidebar_screenshot").IsEnabled)
					{
						this.mCommonHandler.ScreenShotButtonHandler();
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Screenshot", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_MACRO_RECORDER:
					if (this.mSidebar != null && this.mSidebar.GetElementFromTag("sidebar_macro") != null && this.mSidebar.GetElementFromTag("sidebar_macro").Visibility == Visibility.Visible && this.mSidebar.GetElementFromTag("sidebar_macro").IsEnabled && (FeatureManager.Instance.IsMacroRecorderEnabled || FeatureManager.Instance.IsCustomUIForNCSoft))
					{
						if (this.mIsMacroRecorderActive)
						{
							this.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""), "", "", false);
						}
						else
						{
							this.mCommonHandler.ShowMacroRecorderWindow();
						}
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_SYNCHRONISER:
					if (FeatureManager.Instance.IsOperationsSyncEnabled)
					{
						if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.mVmName) || this.mIsSyncMaster)
						{
							this.ShowSynchronizerWindow();
						}
						ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "OperationSync", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOGGLE_VIDEO_RECORDER:
				case ClientHotKeys.STRING_BOSSKEY_SETTING:
					goto IL_0CD4;
				case ClientHotKeys.STRING_RECORD_SCREEN:
					if (this.mSidebar != null && this.mSidebar.GetElementFromTag("sidebar_video_capture") != null && this.mSidebar.GetElementFromTag("sidebar_video_capture").Visibility == Visibility.Visible && this.mSidebar.GetElementFromTag("sidebar_video_capture").IsEnabled)
					{
						this.mCommonHandler.DownloadAndLaunchRecording("sidebar", "shortcut");
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_HOME:
					this.mCommonHandler.HomeButtonHandler(true, false);
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Home", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				case ClientHotKeys.STRING_BACK:
					this.mCommonHandler.BackButtonHandler(false);
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Back", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				case ClientHotKeys.STRING_SHAKE:
					this.mCommonHandler.ShakeButtonHandler();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Shake", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				case ClientHotKeys.STRING_ROTATE:
					if (this.mSidebar != null)
					{
						this.mSidebar.RotateButtonHandler("shortcut");
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_OPEN_MEDIA_FOLDER:
					CommonHandlers.OpenMediaFolder();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MediaFolder", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				case ClientHotKeys.STRING_TOGGLE_MULTIINSTANCE_WINDOW:
					try
					{
						if (!FeatureManager.Instance.IsCustomUIForNCSoft)
						{
							Process.Start(global::System.IO.Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"));
							ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MultiInstance", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						}
						goto IL_0CD4;
					}
					catch (Exception ex)
					{
						Logger.Error("Couldn't launch MI Manager. Ex: {0}", new object[] { ex.Message });
						goto IL_0CD4;
					}
					break;
				case ClientHotKeys.STRING_SET_LOCATION:
					break;
				case ClientHotKeys.STRING_GAMEPAD_CONTROLS:
					if (this.mTopBar.mAppTabButtons.SelectedTab == null || this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
					{
						goto IL_0CD4;
					}
					if (this.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(this.mTopBar.mAppTabButtons.SelectedTab.PackageName, (AppIconModel _) => _.IsGamepadCompatible) && !this.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad"))
					{
						KMManager.HandleInputMapperWindow(this, "gamepad");
						string text = "sidebar";
						string userGuid = RegistryManager.Instance.UserGuid;
						string text2 = "GamePad";
						string text3 = "shortcut";
						string clientVersion = RegistryManager.Instance.ClientVersion;
						string version = RegistryManager.Instance.Version;
						string oem = RegistryManager.Instance.Oem;
						AppTabButton selectedTab = this.mTopBar.mAppTabButtons.SelectedTab;
						ClientStats.SendMiscellaneousStatsAsync(text, userGuid, text2, text3, clientVersion, version, oem, (selectedTab != null) ? selectedTab.PackageName : null, null);
						goto IL_0CD4;
					}
					goto IL_0CD4;
				case ClientHotKeys.STRING_MINIMIZE_TOOLTIP:
					this.MinimizeWindow();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Minimize", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				case ClientHotKeys.STRING_AUTOMATIC_SORTING:
					CommonHandlers.ArrangeWindow();
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ArrangeWindow", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				case ClientHotKeys.STRING_START_STREAMING:
				{
					bool flag = this.mIsStreaming;
					NCSoftUtils.Instance.SendStreamingEvent(this.mVmName, this.mIsStreaming ? "off" : "on");
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, flag ? "StreamVideoOff" : "StreamVideoOn", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					goto IL_0CD4;
				}
				case ClientHotKeys.STRING_SETTINGS:
				{
					this.mTopBar.mSettingsMenuPopup.IsOpen = false;
					string text4 = string.Empty;
					if (this.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.StaticComponents.mSelectedTabButton.PackageName))
					{
						text4 = "STRING_GAME_SETTINGS";
					}
					ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Setting", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					this.mCommonHandler.LaunchSettingsWindow(text4);
					goto IL_0CD4;
				}
				case ClientHotKeys.STRING_CONTROLS_EDITOR:
					ThreadPool.QueueUserWorkItem(delegate(object obj)
					{
						try
						{
							base.Dispatcher.Invoke(new Action(delegate
							{
								if (this.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
								{
									this.mCommonHandler.KeyMapButtonHandler("shortcut", "sidebar");
								}
							}), new object[0]);
						}
						catch
						{
						}
					});
					goto IL_0CD4;
				case ClientHotKeys.STRING_IMAGE_PICKER:
					try
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string> { { "isInImagePickerMode", "true" } };
						HTTPUtils.SendRequestToEngineAsync("toggleImagePickerMode", dictionary, this.mVmName, 0, null, false, 1, 0);
					}
					catch (Exception ex2)
					{
						Logger.Error("Exception in image picker mode: " + ex2.ToString());
					}
					goto IL_0CD4;
				default:
					goto IL_0CD4;
				}
				this.mCommonHandler.LocationButtonHandler();
				ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "SetLocation", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				IL_0CD4:;
			}
			catch (Exception ex3)
			{
				Logger.Error("Exception in executing shortcut: " + ex3.ToString());
			}
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x0001092B File Offset: 0x0000EB2B
		internal void ResizeMainWindowForKeyMapSidebar()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				bool flag = false;
				IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, false);
				double num = (double)fullscreenMonitorSize.Width;
				if (base.WindowState == WindowState.Maximized || this.mIsFullScreen)
				{
					this.RestoreWindows(false);
					flag = true;
				}
				if (flag || base.ActualWidth * MainWindow.sScalingFactor > num - MainWindow.sScalingFactor * 241.0)
				{
					double num2 = num - 241.0 * MainWindow.sScalingFactor;
					double heightFromWidth = this.GetHeightFromWidth(num2, true, false);
					int num3 = Convert.ToInt32(Math.Floor(((double)fullscreenMonitorSize.Height - heightFromWidth) / 2.0));
					InteropWindow.SetWindowPos(this.Handle, (IntPtr)0, 0, num3, Convert.ToInt32(Math.Floor(num2)), Convert.ToInt32(Math.Floor(heightFromWidth)), 80U);
					return;
				}
				if (base.ActualWidth * MainWindow.sScalingFactor + base.Left * MainWindow.sScalingFactor > num - MainWindow.sScalingFactor * 241.0)
				{
					InteropWindow.SetWindowPos(this.Handle, (IntPtr)0, Convert.ToInt32(Math.Floor(num - MainWindow.sScalingFactor * 241.0 - base.ActualWidth * MainWindow.sScalingFactor)), Convert.ToInt32(Math.Floor(base.Top * MainWindow.sScalingFactor)), Convert.ToInt32(Math.Floor(base.ActualWidth * MainWindow.sScalingFactor)), Convert.ToInt32(Math.Floor(base.ActualHeight * MainWindow.sScalingFactor)), 80U);
				}
			}), new object[0]);
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x0009285C File Offset: 0x00090A5C
		internal Grid AddBrowser(string url)
		{
			Grid grid = new Grid();
			BrowserControl browserControl = new BrowserControl(url)
			{
				Visibility = Visibility.Visible
			};
			CustomPictureBox customPictureBox = new CustomPictureBox
			{
				HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Height = 30.0,
				Width = 30.0,
				ImageName = "loader",
				IsImageToBeRotated = true
			};
			grid.Children.Add(browserControl);
			grid.Children.Add(customPictureBox);
			grid.Visibility = Visibility.Hidden;
			this.mContentGrid.Children.Add(grid);
			return grid;
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x000928F8 File Offset: 0x00090AF8
		internal void Frontend_OrientationChanged(string packagename, bool isPortrait)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (!string.IsNullOrEmpty(packagename))
				{
					AppTabButton tab = this.mTopBar.mAppTabButtons.GetTab(packagename);
					if (tab != null)
					{
						tab.IsPortraitModeTab = isPortrait;
					}
					if (this.AppForcedOrientationDict.ContainsKey(packagename))
					{
						this.AppForcedOrientationDict[packagename] = isPortrait;
					}
				}
				this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
			}), new object[0]);
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x00092940 File Offset: 0x00090B40
		internal void GuestBoot_Completed()
		{
			Logger.Info("BOOT_STAGE: In Guestboot_completed ");
			this.ShowLoadingGrid(false);
			if (!this.mGuestBootCompleted)
			{
				this.mGuestBootCompleted = true;
				Publisher.PublishMessage(BrowserControlTags.bootComplete, this.mVmName, null);
				this.OnGuestBootCompleted();
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					base.Dispatcher.Invoke(new Action(delegate
					{
						this.FrontendParentGrid.Visibility = Visibility.Visible;
					}), new object[0]);
				}
				this.HideQuitPopupIfShown();
				this.mWelcomeTab.mHomeAppManager.InitAppPromotionEvents();
				if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
				{
					base.Dispatcher.Invoke(new Action(delegate
					{
						this.mWelcomeTab.mBackground.Visibility = Visibility.Hidden;
					}), new object[0]);
				}
				AppUsageTimer.StartTimer(this.mVmName, "Home");
				KMManager.GetCurrentParserVersion(this);
				this.Utils.GetCurrentVolumeAtBootAsyncAndSetMuteInstancesState();
				BlueStacksUIUtils.InvokeMIManagerEvents(this.mVmName);
				this.EngineInstanceRegistry.LastBootDate = DateTime.Now.Date.ToShortDateString();
				this.Utils.sBootCheckTimer.Enabled = false;
				this.mTopBar.InitializeSnailButton();
				if (!Opt.Instance.hiddenBootMode)
				{
					this.CheckIfVtxDisabledOrUnavailableAndShowPopup();
				}
				FrontendHandler.UpdateBootTimeInregistry(this.mBootStartTime);
				GrmHandler.RequirementConfigUpdated(this.mVmName);
				if (!FeatureManager.Instance.IsPromotionDisabled)
				{
					this.mWelcomeTab.RemovePromotionGrid();
				}
				if (this.EngineInstanceRegistry.IsMuted || RegistryManager.Instance.AreAllInstancesMuted)
				{
					this.Utils.MuteApplication(false);
				}
				else
				{
					this.Utils.UnmuteApplication(false);
				}
				if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
					{
						if (this.mTopBar.CheckForRam())
						{
							this.mTopBar.AddRamNotification();
						}
						bool flag = false;
						if (!string.IsNullOrEmpty(Opt.Instance.Json))
						{
							JObject jobject = JObject.Parse(Opt.Instance.Json);
							if (jobject["app_pkg"] != null && !string.IsNullOrEmpty(jobject["app_pkg"].ToString().Trim().Trim()))
							{
								this.mAppHandler.PerformGamingAction(jobject["app_pkg"].ToString().Trim(), "");
								flag = true;
							}
						}
						if (!flag)
						{
							this.mAppHandler.PerformGamingAction("", "");
						}
					}
					else
					{
						this.PerformPendingRegistryActionIfAny();
						if (this.EngineInstanceRegistry.IsGoogleSigninDone)
						{
							this.PostGoogleSigninCompleteTask();
						}
						else
						{
							if (this.mTopBar.CheckForRam())
							{
								this.mTopBar.AddRamNotification();
							}
							this.HandleSslConnectionError();
							Action<bool> appSuggestionHandler = PromotionObject.AppSuggestionHandler;
							if (appSuggestionHandler != null)
							{
								appSuggestionHandler(false);
							}
						}
						this.PostBootCompleteTask();
					}
				}
				this.BootCompletedStats();
			}
			if (this.EngineInstanceRegistry.NativeGamepadState == NativeGamepadState.ForceOn)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "isEnabled", "true" } };
				this.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", dictionary);
			}
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x0001094B File Offset: 0x0000EB4B
		private void OnFullScreenChanged(bool isFullScreen)
		{
			MainWindow.FullScreenChangedEventHandler fullScreenChanged = this.FullScreenChanged;
			if (fullScreenChanged == null)
			{
				return;
			}
			fullScreenChanged(this, new MainWindowEventArgs.FullScreenChangedEventArgs
			{
				IsFullscreen = isFullScreen
			});
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x0001096A File Offset: 0x0000EB6A
		private void OnGuestBootCompleted()
		{
			MainWindow.GuestBootCompletedEventHandler guestBootCompleted = this.GuestBootCompleted;
			if (guestBootCompleted == null)
			{
				return;
			}
			guestBootCompleted(this, new EventArgs());
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x00010982 File Offset: 0x0000EB82
		internal void OnCursorLockChanged(bool locked)
		{
			MainWindow.CursorLockChangedEventHandler cursorLockChangedEvent = this.CursorLockChangedEvent;
			if (cursorLockChangedEvent == null)
			{
				return;
			}
			cursorLockChangedEvent(this, new MainWindowEventArgs.CursorLockChangedEventArgs
			{
				IsLocked = locked
			});
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x000109A1 File Offset: 0x0000EBA1
		private QuitPopupControl GetQuitPopupFromDimOverlay()
		{
			if (this.mDimOverlay != null)
			{
				return this.mDimOverlay.Control as QuitPopupControl;
			}
			return null;
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x000109BD File Offset: 0x0000EBBD
		private void HideQuitPopupIfShown()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					QuitPopupControl quitPopupFromDimOverlay = this.GetQuitPopupFromDimOverlay();
					if (quitPopupFromDimOverlay != null)
					{
						quitPopupFromDimOverlay.Close();
						ClientStats.SendLocalQuitPopupStatsAsync(quitPopupFromDimOverlay.CurrentPopupTag, "popup_auto_hidden");
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Couldn't notify QuitPopup for boot complete. Ex: {0}", new object[] { ex });
				}
			}), new object[0]);
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x00092C1C File Offset: 0x00090E1C
		internal void InitDiscord()
		{
			try
			{
				if (RegistryManager.Instance.DiscordEnabled && this.IsDefaultVM)
				{
					if (this.mDiscordhandler == null)
					{
						this.mDiscordhandler = new Discord(this);
					}
					this.mDiscordhandler.Init();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in init discord: {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x00092C8C File Offset: 0x00090E8C
		private void HandleSslConnectionError()
		{
			try
			{
				string text = HTTPUtils.SendRequestToGuest("checkSSLConnection", null, this.mVmName, 10000, null, false, 1, 0, "bgp64");
				JObject jobject = JObject.Parse(text);
				if (string.Equals(jobject["result"].ToString(), "error", StringComparison.InvariantCulture) && jobject["reason"].ToString().Contains("SSLHandshakeException"))
				{
					base.Dispatcher.Invoke(new Action(delegate
					{
						CustomMessageWindow customMessageWindow = new CustomMessageWindow();
						customMessageWindow.ImageName = "security_icon";
						BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_ANTIVIRUS_ISSUE", "");
						BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlockTitle, "STRING_ANTIVIRUS_ISSUE_HEADER", "");
						customMessageWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
						customMessageWindow.BodyTextBlockTitle.FontWeight = FontWeights.Regular;
						customMessageWindow.BodyTextBlock.Inlines.Clear();
						customMessageWindow.BodyTextBlock.Inlines.Add(new TextBlock
						{
							Text = LocaleStrings.GetLocalizedString("STRING_TECHNICAL_TIP", ""),
							FontWeight = FontWeights.Bold
						});
						customMessageWindow.BodyTextBlock.Inlines.Add(" ");
						customMessageWindow.BodyTextBlock.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_ANTIVIRUS_ISSUE_FIX", ""));
						customMessageWindow.AddButton(ButtonColors.Blue, "STRING_SEE_HOW_TO_FIX", delegate(object sender1, EventArgs e1)
						{
							BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
							{
								WebHelper.GetServerHost(),
								"help_articles"
							})) + "&article=failed_ssl_connection");
						}, "external_link", true, null);
						this.ShowDimOverlay(null);
						customMessageWindow.Owner = this;
						customMessageWindow.ShowDialog();
						if (this.mDimOverlay != null && !this.mDimOverlay.OwnedWindows.OfType<ContainerWindow>().Any<ContainerWindow>())
						{
							this.HideDimOverlay();
						}
					}), new object[0]);
				}
				ClientStats.SendMiscellaneousStatsAsync("SslConnectionResponse", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.UserSelectedLocale, text, null, null, null, null);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception when testing whether facing any problem in reaching google. " + ex.ToString());
			}
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x00092D70 File Offset: 0x00090F70
		private void PerformPendingRegistryActionIfAny()
		{
			string pendingAction = RegistryManager.Instance.PendingLaunchAction;
			if (string.IsNullOrEmpty(pendingAction))
			{
				return;
			}
			GenericAction action = (GenericAction)Enum.Parse(typeof(GenericAction), pendingAction.Split(new char[] { ',' })[0].Trim(), true);
			string actionValue = pendingAction.Split(new char[] { ',' })[1].Trim();
			if (action != GenericAction.None)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					Logger.Info("Performing pending registry action: {0}", new object[] { pendingAction });
					if (this.mAppHandler.IsAppInstalled(actionValue))
					{
						this.mAppHandler.SendRunAppRequestAsync(actionValue, "", false);
						return;
					}
					if (action == GenericAction.InstallPlay)
					{
						this.mAppHandler.LaunchPlayRequestAsync(actionValue);
					}
				}), new object[0]);
			}
			RegistryManager.Instance.PendingLaunchAction = string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[]
			{
				GenericAction.None,
				string.Empty
			});
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x000109DD File Offset: 0x0000EBDD
		internal void CheckIfVtxDisabledOrUnavailableAndShowPopup()
		{
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				Logger.Info("In CheckIfVtxDisabledOrUnavailableAndShowPopup");
				base.Dispatcher.Invoke(new Action(delegate
				{
					string deviceCaps = RegistryManager.Instance.DeviceCaps;
					if (string.IsNullOrEmpty(deviceCaps))
					{
						return;
					}
					JObject jobject = JObject.Parse(deviceCaps);
					if (jobject["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && jobject["bios_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
					{
						string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
						{
							WebHelper.GetServerHost(),
							"help_articles"
						}));
						text = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[] { text, "enable_virtualization" });
						this.ShowImprovePerformanceWarningPopup(text, "STRING_VTX_DISABLED_WARNING_MESSAGE");
						return;
					}
					if (jobject["cpu_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
					{
						string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
						{
							WebHelper.GetServerHost(),
							"help_articles"
						}));
						text = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[] { text, "vtx_unavailable" });
						this.ShowImprovePerformanceWarningPopup(text, "STRING_VTX_UNAVAILABLE_WARNING_MESSAGE");
						return;
					}
					if (jobject["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && jobject["bios_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && jobject["engine_enabled"].ToString().Equals(EngineState.raw.ToString(), StringComparison.InvariantCultureIgnoreCase))
					{
						string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
						{
							WebHelper.GetServerHost(),
							"help_articles"
						}));
						text = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[] { text, "disable_antivirus" });
						this.ShowImprovePerformanceWarningPopup(text, "STRING_VTX_DISABLED_WARNING_MESSAGE");
					}
				}), new object[0]);
			}
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x00092E64 File Offset: 0x00091064
		private void ShowImprovePerformanceWarningPopup(string url, string bodyTextKeyValue)
		{
			CustomMessageWindow window = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(window.TitleTextBlock, "STRING_IMPROVE_PERFORMANCE", "");
			window.AddWarning(LocaleStrings.GetLocalizedString("STRING_IMPROVE_PERFORMANCE_WARNING", ""), "message_error");
			window.Owner = this;
			BlueStacksUIBinding.Bind(window.BodyTextBlock, bodyTextKeyValue, "");
			window.AddButton(ButtonColors.Blue, "STRING_CHECK_FAQ", delegate(object sender1, EventArgs e1)
			{
				BlueStacksUIUtils.OpenUrl(url);
			}, null, false, null);
			window.AddButton(ButtonColors.White, "STRING_CONTINUE_ANYWAY", delegate(object sender1, EventArgs e1)
			{
				window.Close();
			}, null, false, null);
			window.Show();
		}

		// Token: 0x060018A9 RID: 6313 RVA: 0x00010A13 File Offset: 0x0000EC13
		internal void CloseBrowserQuitPopup()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (this.mQuitPopupBrowserControl != null)
				{
					this.mQuitPopupBrowserControl.Close();
				}
			}), new object[0]);
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x00092F30 File Offset: 0x00091130
		private void BootCompletedStats()
		{
			if (RegistryManager.Instance.IsClientFirstLaunch == 1)
			{
				if (RegistryManager.Instance.IsEngineUpgraded == 1)
				{
					ClientStats.SendClientStatsAsync("update_init", "success", "engine_activity", "", "", "");
				}
				else
				{
					ClientStats.SendClientStatsAsync("first_init", "success", "engine_activity", "", "", "");
				}
				RegistryManager.Instance.IsClientFirstLaunch = 0;
				NativeMethods.waveOutSetVolume(IntPtr.Zero, uint.MaxValue);
				HTTPUtils.SendRequestToAgentAsync("downloadInstalledAppsCfg", null, this.mVmName, 0, null, false, 1, 0, "bgp64");
				return;
			}
			ClientStats.SendClientStatsAsync("init", "success", "engine_activity", "", "", "");
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x00092FF8 File Offset: 0x000911F8
		private void PostBootCompleteTask()
		{
			this.GetMacroShortcutKeyMappingsWithRestrictedKeysandNames();
			Action<bool> appRecommendationHandler = PromotionObject.AppRecommendationHandler;
			if (appRecommendationHandler != null)
			{
				appRecommendationHandler(false);
			}
			PostBootCloudInfoManager.Instance.GetPostBootDataAsync(this);
			MainWindow.CheckUserPremiumAsync();
			if (RegistryManager.Instance.RequirementConfigUpdateRequired)
			{
				HTTPUtils.SendRequestToGuest("getConfigList", null, this.mVmName, 1000, null, false, 1, 0, "bgp64");
				RegistryManager.Instance.RequirementConfigUpdateRequired = false;
			}
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium);
			}), new object[0]);
			PromotionObject.QuestHandler = (Action)Delegate.Remove(PromotionObject.QuestHandler, new Action(this.HandleQuestForFrontend));
			PromotionObject.QuestHandler = (Action)Delegate.Combine(PromotionObject.QuestHandler, new Action(this.HandleQuestForFrontend));
			this.HandleQuestForFrontend();
			this.mAppHandler.UpdateDefaultLauncher();
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mAppInstaller = new DownloadInstallApk(this);
				if (Oem.Instance.IsDragDropEnabled)
				{
					FileImporter.Init(this);
				}
				this.mWelcomeTab.mPromotionControl.HandlePromotionEventAfterBoot();
			}), new object[0]);
			try
			{
				DownloadInstallApk.SerialWorkQueueInstaller(this.mVmName).Start();
			}
			catch (ThreadStateException ex)
			{
				Logger.Info("Thread Already Started" + ex.ToString());
			}
			if (this.mStartupTabLaunched)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mTopBar.mAppTabButtons.GoToTab(1);
				}), new object[0]);
			}
			else if (this.mPostBootNotificationAction != null)
			{
				AppInfo appInfoFromPackageName = new JsonParser(this.mVmName).GetAppInfoFromPackageName(this.mPostBootNotificationAction);
				if (appInfoFromPackageName != null)
				{
					string text = RegistryStrings.InstallDir + "\\HD-RunApp.exe";
					JObject jobject = new JObject
					{
						{ "app_icon_url", "" },
						{ "app_name", appInfoFromPackageName.Name },
						{ "app_url", "" },
						{ "app_pkg", this.mPostBootNotificationAction }
					};
					string text2 = "-json \"" + jobject.ToString(Formatting.None, new JsonConverter[0]).Replace("\"", "\\\"") + "\"";
					Process.Start(text, string.Format(CultureInfo.InvariantCulture, "{0} -vmname {1}", new object[] { text2, this.mVmName }));
				}
				else
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("click_generic_action", GenericAction.InstallPlay.ToString());
					dictionary.Add("click_action_packagename", this.mPostBootNotificationAction);
					this.Utils.HandleGenericActionFromDictionary(dictionary, "notification_drawer", "");
				}
				this.mPostBootNotificationAction = null;
			}
			this.HandleFLEorAppPopupPostBoot();
			this.mCommonHandler.CheckForMacroScriptOnRestart();
			this.UpdateSynchronizerInstancesList();
			this.InitDiscord();
			BlueStacks.Common.Utils.SetGoogleAdIdAndAndroidIdFromAndroid(this.mVmName);
			if (PromotionObject.Instance != null && PromotionObject.Instance.IsSecurityMetricsEnable)
			{
				SecurityMetrics.Init(this.mVmName);
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mResizeHandler.AddRawInputHandler();
				}), new object[0]);
			}
			RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninPopupShown = true;
			if (new DriveInfo(global::System.IO.Path.GetPathRoot(RegistryManager.Instance.UserDefinedDir)).AvailableFreeSpace < 1073741824L)
			{
				this.ShowLowDiskSpaceWarning();
			}
			if (RegistryManager.Instance.IsEngineUpgraded == 1 && RegistryManager.Instance.IsClientFirstLaunch == 1 && MainWindow.VersionCheckForSmartControl())
			{
				this.ShowAppPopupAfterUpgrade("com.dts.freefireth");
			}
			if (string.Compare(this.mVmName, Strings.CurrentDefaultVmName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				CloudNotificationManager.PostBootCompleted();
			}
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x00093380 File Offset: 0x00091580
		private void ShowAppPopupAfterUpgrade(string packageName)
		{
			try
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (this.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) != null && File.Exists(BlueStacks.Common.Utils.GetInputmapperUserFilePath(packageName)))
					{
						CustomMessageWindow customMessageWindow = new CustomMessageWindow();
						customMessageWindow.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SMART_CONTROLS_ENABLED_0", ""), new object[] { "Garena Free Fire" });
						customMessageWindow.BodyTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_FREEFIRE_NOTIFICATION_MESSAGE", ""), new object[] { "Garena Free Fire" });
						customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
						BlueStacksUIBinding.Bind(customMessageWindow.BodyWarningTextBlock, "STRING_FREEFIRE_NOTIFICATION_DETAIL", "");
						customMessageWindow.BodyWarningTextBlock.FontWeight = FontWeights.Light;
						BlueStacksUIBinding.BindColor(customMessageWindow.BodyWarningTextBlock, global::System.Windows.Controls.Control.ForegroundProperty, "SettingsWindowForegroundDimDimColor");
						customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
						customMessageWindow.UrlTextBlock.Visibility = Visibility.Visible;
						customMessageWindow.UrlLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_FREEFIRE_NOTIFICATION_LINK", ""));
						string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
						{
							WebHelper.GetServerHost(),
							"help_articles"
						})) + "&article=smart_control";
						customMessageWindow.UrlLink.NavigateUri = new Uri(text);
						customMessageWindow.UrlLink.RequestNavigate += this.OpenSmartControlHelp;
						this.ShowDimOverlay(null);
						customMessageWindow.Owner = this.mDimOverlay;
						customMessageWindow.ShowDialog();
						if (this.mDimOverlay != null && this.mDimOverlay.OwnedWindows.OfType<ContainerWindow>().Any<ContainerWindow>())
						{
							this.HideDimOverlay();
						}
					}
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in showing app notifications after upgrade: " + ex.ToString());
			}
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x00010A33 File Offset: 0x0000EC33
		private void OpenSmartControlHelp(object sender, RequestNavigateEventArgs e)
		{
			BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x000933EC File Offset: 0x000915EC
		private static bool VersionCheckForSmartControl()
		{
			if (RegistryManager.Instance.UpgradeVersionList.Length != 0)
			{
				global::System.Version version = new global::System.Version("4.140.00.0000");
				return new global::System.Version(RegistryManager.Instance.UpgradeVersionList.Last<string>()) < version;
			}
			return false;
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x00010A45 File Offset: 0x0000EC45
		private void ShowLowDiskSpaceWarning()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE", "");
				customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE_MESSAGE", "");
				customMessageWindow.AddWarning(LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE_WARNING", ""), "");
				customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
				this.ShowDimOverlay(null);
				customMessageWindow.Owner = this.mDimOverlay;
				customMessageWindow.ShowDialog();
				this.HideDimOverlay();
			}), new object[0]);
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x00093430 File Offset: 0x00091630
		internal void PostGoogleSigninCompleteTask()
		{
			if (!this.mIsTokenAvailable && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
			{
				BlueStacksUIUtils.SendBluestacksLoginRequest(this.mVmName);
			}
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mTopBar.TopBarOptionsPanelElementVisibility(this.mTopBar.mUserAccountBtn, true);
					this.mTopBar.TopBarOptionsPanelElementVisibility(this.mTopBar.mNotificationGrid, true);
				}), new object[0]);
				Action<bool> appSuggestionHandler = PromotionObject.AppSuggestionHandler;
				if (appSuggestionHandler != null)
				{
					appSuggestionHandler(false);
				}
			}
			MainWindow.BrowserOTSCompletedCallbackEventHandler browserOTSCompletedCallback = this.BrowserOTSCompletedCallback;
			if (browserOTSCompletedCallback == null)
			{
				return;
			}
			browserOTSCompletedCallback(this, new MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs
			{
				CallbackFunction = this.mBrowserCallbackFunctionName
			});
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x00010A65 File Offset: 0x0000EC65
		internal static void CheckUserPremiumAsync()
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					PromotionManager.CheckIsUserPremium();
				}
				catch (Exception ex)
				{
					Logger.Error("PostOTSBootComplete: call for premium failed" + ex.ToString());
				}
			});
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x000934BC File Offset: 0x000916BC
		private void HandleQuestForFrontend()
		{
			if (PromotionObject.Instance.QuestHdPlayerRules.Count > 0)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { 
				{
					"data",
					JsonConvert.SerializeObject(PromotionObject.Instance.QuestHdPlayerRules, Formatting.None)
				} };
				this.mFrontendHandler.SendFrontendRequest("setPackagesForInteraction", dictionary);
				PromotionManager.StartQuestRulesProcessor();
			}
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x00093514 File Offset: 0x00091714
		private void HandleFLEorAppPopupPostBoot()
		{
			this.GetFleCampaignJson();
			if (!RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninPopupShown && !RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninDone)
			{
				return;
			}
			if (!string.IsNullOrEmpty(Opt.Instance.Json) && string.Equals("Android", this.mVmName, StringComparison.InvariantCulture))
			{
				JObject jobject = JObject.Parse(Opt.Instance.Json);
				if (jobject["app_pkg"] != null && !string.IsNullOrEmpty(jobject["app_pkg"].ToString().Trim().Trim()))
				{
					new DownloadInstallApk(this).DownloadAndInstallAppFromJson(Opt.Instance.Json);
					return;
				}
			}
			if (!this.mStartupTabLaunched && PromotionObject.Instance.StartupTab.Count > 0)
			{
				if (PromotionObject.Instance.StartupTab.ContainsKey("click_generic_action") && EnumHelper.Parse<GenericAction>(PromotionObject.Instance.StartupTab["click_generic_action"], GenericAction.None) != GenericAction.None && (string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail.Trim()) || string.IsNullOrEmpty(RegistryManager.Instance.Token.Trim())))
				{
					this.mLaunchStartupTabWhenTokenReceived = true;
				}
				if (!this.mLaunchStartupTabWhenTokenReceived)
				{
					base.Dispatcher.Invoke(new Action(delegate
					{
						this.mStartupTabLaunched = true;
						this.Utils.HandleGenericActionFromDictionary(PromotionObject.Instance.StartupTab, "startup_tab", "");
					}), new object[0]);
				}
			}
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0009368C File Offset: 0x0009188C
		public void PublishForFlePopupToBrowser(string json)
		{
			if (!string.IsNullOrEmpty(json))
			{
				JObject jobject = JObject.Parse(json);
				if (jobject["fle_pkg"] != null)
				{
					string text = jobject["fle_pkg"].ToString().Trim();
					Publisher.PublishMessage(BrowserControlTags.showFlePopup, this.mVmName, new JObject(new JProperty("PackageName", text)));
				}
			}
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x000936E8 File Offset: 0x000918E8
		private void GetFleCampaignJson()
		{
			string flecampaignMD = RegistryManager.Instance.FLECampaignMD5;
			if (!string.IsNullOrEmpty(flecampaignMD))
			{
				try
				{
					string text = BstHttpClient.Get(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/getcampaigninfo?md5_hash={1}", new object[]
					{
						RegistryManager.Instance.Host,
						flecampaignMD
					}), null, false, this.mVmName, 0, 1, 0, false, "bgp64");
					RegistryManager.Instance.DeleteFLECampaignMD5();
					RegistryManager.Instance.CampaignJson = text;
				}
				catch
				{
					Logger.Info("Error fetching campaign json");
				}
			}
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x0009377C File Offset: 0x0009197C
		private void HandleFLEorAppPopupBeforeBoot()
		{
			this.GetFleCampaignJson();
			if (!RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninPopupShown && !RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninDone)
			{
				return;
			}
			if (!string.IsNullOrEmpty(Opt.Instance.Json) && string.Equals("Android", this.mVmName, StringComparison.InvariantCulture))
			{
				JObject jobject = JObject.Parse(Opt.Instance.Json);
				if (jobject["app_pkg"] != null && !string.IsNullOrEmpty(jobject["app_pkg"].ToString().Trim().Trim()))
				{
					return;
				}
			}
			if (PromotionObject.Instance.StartupTab.Count > 0)
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (PromotionObject.Instance.StartupTab.ContainsKey("click_generic_action") && (EnumHelper.Parse<GenericAction>(PromotionObject.Instance.StartupTab["click_generic_action"], GenericAction.None) & (GenericAction)36) != (GenericAction)0)
					{
						this.mStartupTabLaunched = true;
						this.Utils.HandleGenericActionFromDictionary(PromotionObject.Instance.StartupTab, "startup_tab", "");
					}
				}), new object[0]);
			}
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x0009385C File Offset: 0x00091A5C
		internal void ShowLoadingGrid(bool isShow)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					if (isShow)
					{
						this.mTopBar.mAppTabButtons.EnableAppTabs(false);
						if (!FeatureManager.Instance.IsCustomUIForDMM)
						{
							this.mWelcomeTab.mHomeAppManager.ChangeHomeAppLoadingGridVisibility(Visibility.Visible);
						}
					}
					else
					{
						this.mTopBar.mAppTabButtons.EnableAppTabs(true);
						if (!FeatureManager.Instance.IsCustomUIForDMM)
						{
							this.mWelcomeTab.mHomeAppManager.ChangeHomeAppLoadingGridVisibility(Visibility.Hidden);
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in ShowLoadingGrid. " + ex.ToString());
				}
				Logger.Info("BOOT_STAGE: Removing progress bar");
			}), new object[0]);
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x00010A8C File Offset: 0x0000EC8C
		internal void ShowControlGrid(Grid controlGrid)
		{
			if (this.mLastVisibleGrid != null && controlGrid != this.mLastVisibleGrid)
			{
				this.mLastVisibleGrid.Visibility = Visibility.Hidden;
			}
			this.mLastVisibleGrid = controlGrid;
			controlGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x0009389C File Offset: 0x00091A9C
		private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!e.OriginalSource.GetType().Equals(typeof(CustomPictureBox)) || this.mTopBar.WindowHeaderGrid.IsMouseOver)
			{
				try
				{
					base.DragMove();
				}
				catch
				{
				}
				this.UIChangesOnMainWindowSizeChanged();
			}
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x00010AB9 File Offset: 0x0000ECB9
		private void TopBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (!e.OriginalSource.GetType().Equals(typeof(CustomPictureBox)))
			{
				if (base.WindowState == WindowState.Maximized)
				{
					this.RestoreWindows(false);
					return;
				}
				this.MaximizeWindow();
			}
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x000938FC File Offset: 0x00091AFC
		internal void RestoreWindows(bool isReArrange = false)
		{
			if (this.mResizeHandler.IsMinMaxEnabled)
			{
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.mTopBarPopup.IsOpen = false;
				}
				if (this.mGeneraltoast.IsOpen)
				{
					this.toastTimer.Stop();
					this.mGeneraltoast.IsOpen = false;
				}
				if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.OnFullScreenChanged(false);
					this.ToggleFullScreenToastVisibility(false, "", "", "");
				}
				this.TopBar.Visibility = Visibility.Visible;
				this.OuterBorder.BorderThickness = new Thickness(1.0);
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.mDmmBottomBar.Visibility = Visibility.Visible;
				}
				if (!isReArrange && this.mIsFullScreenFromMaximized && this.mIsFullScreen)
				{
					IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, true);
					InteropWindow.SetWindowPos(this.Handle, (IntPtr)0, fullscreenMonitorSize.Left, fullscreenMonitorSize.Top, fullscreenMonitorSize.Width, fullscreenMonitorSize.Height, 80U);
					this.UIChangesOnMainWindowSizeChanged();
				}
				else
				{
					this.mIsFullScreenFromMaximized = false;
					if (FeatureManager.Instance.IsCustomUIForDMM && this.mDMMRecommendedWindow != null && this.mIsDMMRecommendedWindowOpen)
					{
						this.mDMMRecommendedWindow.Visibility = Visibility.Visible;
					}
					this.mResizeHandler.mAdjustingWidth = false;
					if (this.mTopBar.mAppTabButtons.SelectedTab != null)
					{
						this.IsUIInPortraitMode = this.mTopBar.mAppTabButtons.SelectedTab.IsPortraitModeTab;
					}
					this.mResizeGrid.Visibility = Visibility.Visible;
					this.FrontendParentGrid.Margin = new Thickness(1.0);
					base.WindowState = WindowState.Normal;
					this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight(this.mPreviousHeight.Value, false, false), this.mPreviousHeight.Value, true);
					this.SwitchToPortraitMode(this.IsUIInPortraitMode);
					this.mIsDmmMaximised = false;
					if (FeatureManager.Instance.IsCustomUIForDMM)
					{
						this.DmmRestoreWindowRectangle = new Rect(0.0, 0.0, 0.0, 0.0);
					}
					else if (this.IsUIInPortraitMode)
					{
						this.mTopBar.RefreshNotificationCentreButton();
					}
					this.mResizeHandler.IsResizingEnabled = true;
					this.mTopBar.mMaximizeButton.ImageName = "maximize";
					BlueStacksUIBinding.Bind(this.mTopBar.mMaximizeButton, "STRING_MAXIMIZE_TOOLTIP");
					this.mNCTopBar.mMaximizeButtonImage.ImageName = "maximize";
					BlueStacksUIBinding.Bind(this.mNCTopBar.mMaximizeButtonImage, "STRING_MAXIMIZE_TOOLTIP");
					this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
					if (KMManager.sGuidanceWindow != null)
					{
						KMManager.sGuidanceWindow.Visibility = Visibility.Visible;
						base.Dispatcher.Invoke(new Action(delegate
						{
							base.Focus();
						}), new object[0]);
					}
				}
				this.mTopBar.UpdateMacroRecordingProgress();
				this.mIsFullScreen = false;
				HTTPUtils.SendRequestToEngineAsync("setIsFullscreen", new Dictionary<string, string> { { "isFullscreen", "false" } }, this.mVmName, 0, null, false, 1, 0);
				this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
			}
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x00010AEE File Offset: 0x0000ECEE
		internal void MinimizeWindow()
		{
			base.WindowState = WindowState.Minimized;
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x00093C30 File Offset: 0x00091E30
		internal void MaximizeWindow()
		{
			if (this.mResizeHandler.IsMinMaxEnabled)
			{
				this.mIsDMMMaximizedFromPortrait = this.IsUIInPortraitMode;
				if (FeatureManager.Instance.IsCustomUIForDMM && this.mDMMRecommendedWindow != null)
				{
					this.mDMMRecommendedWindow.Visibility = Visibility.Hidden;
				}
				if (base.WindowState == WindowState.Normal)
				{
					this.mPreviousWidth = new double?(base.Width);
					this.mPreviousHeight = new double?(base.Height);
				}
				this.mIsDmmMaximised = true;
				if (FeatureManager.Instance.IsCustomUIForDMM && this.IsUIInPortraitMode && !this.mIsFullScreen)
				{
					this.SetDMMRestoreWindowSizeAndPosition();
					this.SetSizeForDMMPortraitMaximisedWindow();
				}
				else
				{
					if (FeatureManager.Instance.IsCustomUIForDMM && !this.mIsFullScreen)
					{
						this.SetDMMRestoreWindowSizeAndPosition();
					}
					this.IsUIInPortraitModeWhenMaximized = this.IsUIInPortraitMode;
					this.IsUIInPortraitMode = !(this.mAspectRatio > 1L);
					base.WindowState = WindowState.Maximized;
				}
				this.mResizeHandler.IsResizingEnabled = false;
				this.mTopBar.mMaximizeButton.ImageName = "restore";
				this.mTopBar.RefreshNotificationCentreButton();
				this.mTopBar.UpdateMacroRecordingProgress();
				BlueStacksUIBinding.Bind(this.mTopBar.mMaximizeButton, "STRING_RESTORE_BUTTON");
				this.mNCTopBar.mMaximizeButtonImage.ImageName = "restore";
				BlueStacksUIBinding.Bind(this.mNCTopBar.mMaximizeButtonImage, "STRING_RESTORE_BUTTON");
				this.mTopBar.RefreshWarningButton();
				this.UIChangesOnMainWindowSizeChanged();
				if (KMManager.sGuidanceWindow != null)
				{
					MainWindow parentWindow = KMManager.sGuidanceWindow.ParentWindow;
					if (string.Equals((parentWindow != null) ? parentWindow.mVmName : null, this.mVmName, StringComparison.InvariantCultureIgnoreCase))
					{
						KMManager.sGuidanceWindow.Visibility = Visibility.Collapsed;
						if (AppConfigurationManager.Instance.VmAppConfig[this.mVmName].ContainsKey(this.mTopBar.mAppTabButtons.SelectedTab.PackageName))
						{
							if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.mTopBar.mAppTabButtons.SelectedTab.PackageName, (AppSettings appSettings) => appSettings.IsCloseGuidanceOnboardingCompleted) && !this.mIsFullScreen)
							{
								Sidebar sidebar = this.mSidebar;
								if (sidebar != null)
								{
									sidebar.ShowViewGuidancePopup();
								}
								AppConfigurationManager.Instance.VmAppConfig[this.mVmName][this.mTopBar.mAppTabButtons.SelectedTab.PackageName].IsCloseGuidanceOnboardingCompleted = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x00093EA4 File Offset: 0x000920A4
		internal void RestrictWindowResize(bool enable)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mResizeHandler.IsMinMaxEnabled = !enable;
				this.mResizeHandler.IsResizingEnabled = !enable;
				this.mTopBar.mMaximizeButton.IsEnabled = !enable;
				this.mNCTopBar.mMaximizeButtonImage.IsEnabled = !enable;
				if (enable)
				{
					this.mTopBar.mMaximizeButton.SetDisabledState();
					this.mNCTopBar.mMaximizeButtonImage.SetDisabledState();
					return;
				}
				this.mTopBar.mMaximizeButton.SetNormalState();
				this.mNCTopBar.mMaximizeButtonImage.SetNormalState();
			}), new object[0]);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x00093EE4 File Offset: 0x000920E4
		internal void FullScreenWindow()
		{
			if (FeatureManager.Instance.IsCustomUIForDMM || this.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
			{
				if (base.WindowState == WindowState.Normal)
				{
					this.mPreviousWidth = new double?(base.Width);
					this.mPreviousHeight = new double?(base.Height);
				}
				this.mIsFullScreen = true;
				this.OuterBorder.BorderThickness = new Thickness(0.0);
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.mDmmBottomBar.Visibility = Visibility.Collapsed;
					this.mDmmBottomBar.ShowKeyMapPopup(false);
				}
				if (!FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.OnFullScreenChanged(true);
				}
				this.TopBar.Visibility = Visibility.Collapsed;
				this.mResizeGrid.Visibility = Visibility.Collapsed;
				this.FrontendParentGrid.Margin = new Thickness(0.0);
				if (base.WindowState == WindowState.Maximized)
				{
					this.mIsFullScreenFromMaximized = true;
					IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, false);
					InteropWindow.SetWindowPos(this.Handle, (IntPtr)0, fullscreenMonitorSize.Left, fullscreenMonitorSize.Top, fullscreenMonitorSize.Width, fullscreenMonitorSize.Height, 80U);
				}
				else
				{
					if (FeatureManager.Instance.IsCustomUIForDMM && base.WindowState != WindowState.Maximized && !this.mIsDmmMaximised)
					{
						this.SetDMMRestoreWindowSizeAndPosition();
					}
					this.MaximizeWindow();
				}
				global::System.Windows.Forms.Cursor.Clip = global::System.Drawing.Rectangle.Empty;
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.mTopBarPopup.IsOpen = true;
				}
				else
				{
					string[] array = LocaleStrings.GetLocalizedString("STRING_FULLSCREEN_EXIT_POPUP_TEXT", "").Split(new char[] { '{', '}' });
					this.ToggleFullScreenToastVisibility(true, array[0], this.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false), array[2]);
				}
				HTTPUtils.SendRequestToEngineAsync("setIsFullscreen", new Dictionary<string, string> { { "isFullscreen", "true" } }, this.mVmName, 0, null, false, 1, 0);
				new Thread(delegate
				{
					Thread.Sleep(1000);
					base.Dispatcher.Invoke(new Action(delegate
					{
						if (FeatureManager.Instance.IsCustomUIForDMM && !this.mDMMFST.IsMouseOver && !this.mDMMFST.mVolumePopup.IsOpen && !this.mDMMFST.mChangeTransparencyPopup.IsOpen)
						{
							this.mTopBarPopup.IsOpen = false;
						}
					}), new object[0]);
				})
				{
					IsBackground = true
				}.Start();
				this.UIChangesOnMainWindowSizeChanged();
				if (KMManager.sGuidanceWindow != null)
				{
					KMManager.sGuidanceWindow.Visibility = Visibility.Collapsed;
				}
			}
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x00010AF7 File Offset: 0x0000ECF7
		public void ToggleFullScreenToastVisibility(bool isFullScreen, string tip = "", string key = "", string info = "")
		{
			if (isFullScreen)
			{
				this.ShowToast(tip, key, info, true);
				return;
			}
			this.CloseFullScreenToastAndStopTimer();
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x00094108 File Offset: 0x00092308
		internal void ShowToast(string tip, string key = "", string info = "", bool isForced = false)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					if (isForced || (this.mIsWindowInFocus && !FeatureManager.Instance.IsCustomUIForDMM))
					{
						if (this.mFullScreenToastPopup.IsOpen)
						{
							this.mFullScreenToastTimer.Stop();
							this.mFullScreenToastPopup.IsOpen = false;
						}
						if (isForced)
						{
							if (string.IsNullOrEmpty(key))
							{
								return;
							}
							this.mFullScreenToastControl.Init(this, tip, key, info);
						}
						else
						{
							this.mFullScreenToastControl.Init(this, tip);
						}
						this.dummyToast.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
						this.dummyToast.VerticalAlignment = VerticalAlignment.Top;
						this.mFullScreenToastControl.Visibility = Visibility.Visible;
						this.mFullScreenToastPopup.IsOpen = true;
						this.mFullScreenToastCanvas.Height = this.mFullScreenToastControl.ActualHeight;
						this.mFullScreenToastPopup.VerticalOffset = this.mFullScreenToastControl.ActualHeight + 20.0;
						this.mFullScreenToastPopup.HorizontalAlignment = global::System.Windows.HorizontalAlignment.Center;
						if (this.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen)
						{
							this.mFullScreenToastTimer.Interval = TimeSpan.FromMilliseconds(3000.0);
						}
						else
						{
							this.mFullScreenToastTimer.Interval = TimeSpan.FromMilliseconds(5000.0);
						}
						this.mFullScreenToastTimer.Start();
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in showing fullscreen toast : " + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x00010B0E File Offset: 0x0000ED0E
		private void SetDMMRestoreWindowSizeAndPosition()
		{
			this.DmmRestoreWindowRectangle = new Rect(base.Left, base.Top, base.Width, base.Height);
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x00094160 File Offset: 0x00092360
		private void SetSizeForDMMPortraitMaximisedWindow()
		{
			double num = SystemParameters.WorkArea.Height;
			double num2 = this.GetWidthFromHeight(num, false, false);
			if (num2 > SystemParameters.WorkArea.Width / MainWindow.sScalingFactor)
			{
				num2 = SystemParameters.WorkArea.Width / MainWindow.sScalingFactor;
				num = this.GetHeightFromWidth(num2, false, false);
			}
			if (num2 < base.MinWidth || num < base.MinHeight)
			{
				num2 = base.MinWidth;
				num = base.MinHeight;
			}
			base.Height = num;
			base.Width = num2;
			base.Left = (SystemParameters.WorkArea.Width - base.Width) / 2.0;
			base.Top = 0.0;
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x0002CE5C File Offset: 0x0002B05C
		private void BottomBar_MouseDown(object sender, MouseButtonEventArgs e)
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

		// Token: 0x060018C5 RID: 6341 RVA: 0x00010B33 File Offset: 0x0000ED33
		private void ResizeGrid_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.mResizeGrid == null)
			{
				this.mResizeGrid = sender as Grid;
				this.WireSizingEvents();
			}
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x0009421C File Offset: 0x0009241C
		private void WireSizingEvents()
		{
			foreach (object obj in this.mResizeGrid.Children)
			{
				global::System.Windows.Shapes.Rectangle rectangle = ((UIElement)obj) as global::System.Windows.Shapes.Rectangle;
				if (rectangle != null)
				{
					rectangle.PreviewMouseLeftButtonDown += this.mResizeHandler.ResizeRectangle_PreviewMouseDown;
					rectangle.MouseMove += this.mResizeHandler.ResizeRectangle_MouseMove;
				}
			}
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x000942AC File Offset: 0x000924AC
		private void FrontendGrid_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
		{
			this.mFrontendHandler.FrontendVisibleChanged((bool)e.NewValue);
			string text = "KMP FrontendGrid_IsVisibleChanged ";
			object newValue = e.NewValue;
			Logger.Debug(text + ((newValue != null) ? newValue.ToString() : null) + this.mVmName);
			if ((bool)e.NewValue)
			{
				this.OnFrontendGridVisible();
			}
			else
			{
				this.OnFrontendGridHidden();
			}
			this.mFrontendHandler.ShowGLWindow();
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x00010B4F File Offset: 0x0000ED4F
		private void OnFrontendGridHidden()
		{
			this.mFrontendHandler.DeactivateFrontend();
			MainWindow.FrontendGridVisibilityChangedEventHandler frontendGridVisibilityChanged = this.FrontendGridVisibilityChanged;
			if (frontendGridVisibilityChanged == null)
			{
				return;
			}
			frontendGridVisibilityChanged(this, new MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs
			{
				IsVisible = false
			});
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x00010B79 File Offset: 0x0000ED79
		private void OnFrontendGridVisible()
		{
			MainWindow.FrontendGridVisibilityChangedEventHandler frontendGridVisibilityChanged = this.FrontendGridVisibilityChanged;
			if (frontendGridVisibilityChanged == null)
			{
				return;
			}
			frontendGridVisibilityChanged(this, new MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs
			{
				IsVisible = true
			});
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x00010B98 File Offset: 0x0000ED98
		private void FrontendGrid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.mFrontendHandler.ShowGLWindow();
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x00094320 File Offset: 0x00092520
		private void FrontendParentGrid_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (this.FrontendParentGrid.Visibility == Visibility.Visible)
			{
				if (!this.FrontendParentGrid.Children.Contains(this.mFrontendGrid))
				{
					if (this.mFrontendGrid.Parent != null)
					{
						(this.mFrontendGrid.Parent as Grid).Children.Remove(this.mFrontendGrid);
					}
					this.FrontendParentGrid.Children.Add(this.mFrontendGrid);
				}
				if (this.mGuestBootCompleted && FeatureManager.Instance.IsCustomUIForDMM)
				{
					this.mDmmProgressControl.Visibility = Visibility.Hidden;
					this.mFrontendGrid.Visibility = Visibility.Visible;
				}
			}
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x00010BA5 File Offset: 0x0000EDA5
		internal void HandleRestartPopup()
		{
			Logger.Info("Showing restart option to the user");
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_ENGINE_FAIL_HEADER", "");
					BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_ENGINE_RESTART", "");
					customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_ENGINE", this.RestartEngineConfirmationAcceptedHandler, null, false, null);
					customMessageWindow.AddButton(ButtonColors.White, "STRING_RESTART_PC", this.RestartPcConfirmationAcceptedHandler, null, false, null);
					this.ShowDimOverlay(null);
					customMessageWindow.Owner = this.mDimOverlay;
					customMessageWindow.ShowDialog();
					this.HideDimOverlay();
				}
				catch (Exception ex)
				{
					Logger.Error("Error window probably closed");
					Logger.Error(ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x00010BCF File Offset: 0x0000EDCF
		internal void MainWindow_RestartEngineConfirmationAcceptedHandler(object sender, EventArgs e)
		{
			BlueStacksUIUtils.RestartInstance(this.mVmName);
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x00010BDC File Offset: 0x0000EDDC
		private void MainWindow_RestartPcConfirmationHandler(object sender, EventArgs e)
		{
			Process.Start("shutdown.exe", "-r -t 0");
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x00010BEE File Offset: 0x0000EDEE
		private void WelcomeTabParentGrid_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			this.mWelcomeTab.Visibility = this.WelcomeTabParentGrid.Visibility;
		}

		// Token: 0x060018D0 RID: 6352 RVA: 0x00010C06 File Offset: 0x0000EE06
		private void PikaPopControl_CloseClicked(object sender, EventArgs e)
		{
			this.pikaPop.IsOpen = false;
			this.isPikaPopOpen = false;
		}

		// Token: 0x060018D1 RID: 6353 RVA: 0x000943C8 File Offset: 0x000925C8
		internal void ClosePopUps()
		{
			this.PikaPopControl_CloseClicked(this, null);
			this.mWelcomeTab.mHomeAppManager.CloseHomeAppPopups();
			this.toastPopup.IsOpen = false;
			this.mShootingModePopup.IsOpen = false;
			this.mFullScreenToastPopup.IsOpen = false;
			this.mFullscreenTopbarPopup.IsOpen = false;
			this.mFullscreenTopbarPopupButton.IsOpen = false;
			this.mFullscreenSidebarPopup.IsOpen = false;
			this.mFullscreenSidebarPopupButton.IsOpen = false;
			if (this.mSidebar.mListPopups != null)
			{
				foreach (CustomPopUp customPopUp in this.mSidebar.mListPopups)
				{
					customPopUp.IsOpen = false;
				}
			}
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x00010C1B File Offset: 0x0000EE1B
		private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked back button setup bottombar ");
			this.mCommonHandler.BackButtonHandler(false);
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x00094498 File Offset: 0x00092698
		private void TopBarPopup_MouseLeave(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				if (!this.mDMMFST.IsMouseOver && !this.mDMMFST.mVolumePopup.IsOpen && !this.mDMMFST.mChangeTransparencyPopup.IsOpen)
				{
					this.mTopBarPopup.IsOpen = false;
					return;
				}
			}
			else if (!this.mFullScreenTopBar.mChangeTransparencyPopup.IsOpen)
			{
				this.mTopBarPopup.IsOpen = false;
			}
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x00094510 File Offset: 0x00092710
		internal void SetMacroPlayBackEventHandle()
		{
			try
			{
				using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(BlueStacksUIUtils.GetMacroPlaybackEventName(this.mVmName)))
				{
					eventWaitHandle.Set();
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to set macro playback event err:" + ex.ToString());
			}
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x00094578 File Offset: 0x00092778
		internal void StartTimerForAppPlayerRestart(int interval)
		{
			this.mMacroTimer = new global::System.Timers.Timer((double)(interval * 60 * 1000));
			this.mMacroTimer.Elapsed -= this.MacroTimer_Elapsed;
			this.mMacroTimer.Elapsed += this.MacroTimer_Elapsed;
			this.mMacroTimer.Start();
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x000945D4 File Offset: 0x000927D4
		private void Fullscreentopbar_opened(object sender, EventArgs e)
		{
			if (this.mTopBarPopup.IsOpen)
			{
				base.MouseMove -= this.MainWindow_MouseMove;
				base.MouseMove += this.MainWindow_MouseMove;
				return;
			}
			base.MouseMove -= this.MainWindow_MouseMove;
		}

		// Token: 0x060018D7 RID: 6359 RVA: 0x00094628 File Offset: 0x00092828
		private void MacroTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (this.mMacroTimer.Enabled)
			{
				this.mMacroTimer.Enabled = false;
				this.mMacroTimer.AutoReset = false;
				this.mMacroTimer.Dispose();
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mTopBar.HideMacroPlaybackFromTopBar();
					if (FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						this.mNCTopBar.HideMacroPlaybackFromTopBar();
					}
					this.mIsMacroPlaying = false;
					this.mMacroPlaying = string.Empty;
					BlueStacksUIUtils.RestartInstance(this.mVmName);
				}), new object[0]);
			}
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x00094684 File Offset: 0x00092884
		internal void ShowSynchronizerWindow()
		{
			this.mTopBar.mSettingsMenuPopup.IsOpen = false;
			if (this.mSynchronizerWindow == null)
			{
				this.mSynchronizerWindow = new SynchronizerWindow(this);
			}
			this.mSynchronizerWindow.Init();
			this.mSynchronizerWindow.Show();
			this.mSynchronizerWindow.ShowWithParentWindow = true;
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x000946D8 File Offset: 0x000928D8
		private void ReleaseClientGlobalLock()
		{
			try
			{
				if (this.mBlueStacksClientInstanceLock != null)
				{
					this.mBlueStacksClientInstanceLock.ReleaseMutex();
					this.mBlueStacksClientInstanceLock.Close();
					this.mBlueStacksClientInstanceLock = null;
				}
			}
			catch (Exception ex)
			{
				string text = "Exception in releasing client global lock..";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x0009473C File Offset: 0x0009293C
		private void MainWindow_MouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			try
			{
				if (this.mIsFullScreen && this.mTopBarPopup.IsOpen && e.GetPosition(this.mDMMFST).Y > 80.0 && !this.mDMMFST.mChangeTransparencyPopup.IsOpen)
				{
					this.mTopBarPopup.IsOpen = false;
				}
			}
			catch
			{
			}
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x000947B0 File Offset: 0x000929B0
		internal void ShowLockScreen()
		{
			if (this.mIsLockScreenActionPending)
			{
				return;
			}
			if (this.EngineInstanceRegistry.IsClientOnTop)
			{
				base.Topmost = false;
				this.EngineInstanceRegistry.IsClientOnTop = false;
			}
			if (this.mDimOverlay != null && this.mDimOverlay.OwnedWindows.Count > 0)
			{
				using (IEnumerator enumerator = this.mDimOverlay.OwnedWindows.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						((Window)obj).Close();
					}
					goto IL_00E0;
				}
			}
			if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null && KMManager.CanvasWindow.SidebarWindow.Visibility == Visibility.Visible)
			{
				KMManager.CanvasWindow.SidebarWindow.Close();
			}
			else if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed && KMManager.sGuidanceWindow.Visibility == Visibility.Visible)
			{
				KMManager.sGuidanceWindow.Close();
			}
			IL_00E0:
			KMManager.ShowOverlayWindow(this, false, false);
			if (this.mMacroRecorderWindow != null)
			{
				this.mCommonHandler.HideMacroRecorderWindow();
			}
			this.mIsLockScreenActionPending = true;
			this.ShowDimOverlay(this.ScreenLockInstance);
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x000948DC File Offset: 0x00092ADC
		internal void HideLockScreen()
		{
			if (this.mDimOverlay == null || this.ScreenLockInstance.Visibility != Visibility.Visible)
			{
				return;
			}
			this.mIsLockScreenActionPending = false;
			this.HideDimOverlay();
			this.ShowWindow(false);
			base.Activate();
			if (RegistryManager.Instance.ShowKeyControlsOverlay && !KMManager.CheckIfKeymappingWindowVisible(false))
			{
				KMManager.ShowOverlayWindow(this, true, false);
			}
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x00094938 File Offset: 0x00092B38
		private void UpdateSynchronizationState()
		{
			this._TopBar.HideSyncPanel();
			if (this.mIsSyncMaster)
			{
				this.mSynchronizerWindow.StopAllSyncOperations();
				return;
			}
			if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.mVmName))
			{
				HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", null, this.mVmName, 0, null, false, 1, 0);
				BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.mVmName);
			}
			foreach (string text in BlueStacksUIUtils.DictWindows.Keys)
			{
				if (text != this.mVmName && BlueStacksUIUtils.DictWindows[text].mSelectedInstancesForSync.Contains(this.mVmName))
				{
					MainWindow mainWindow = BlueStacksUIUtils.DictWindows[text];
					mainWindow.mSelectedInstancesForSync.Remove(this.mVmName);
					if (mainWindow.mSelectedInstancesForSync.Count == 0)
					{
						mainWindow.mIsSynchronisationActive = false;
						mainWindow.mIsSyncMaster = false;
						if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(mainWindow.mVmName))
						{
							BlueStacksUIUtils.sSyncInvolvedInstances.Remove(mainWindow.mVmName);
						}
						mainWindow._TopBar.HideSyncPanel();
						mainWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
					}
				}
			}
		}

		// Token: 0x060018DE RID: 6366 RVA: 0x00094A90 File Offset: 0x00092C90
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				this.ReleaseClientGlobalLock();
				if (this.mMacroTimer != null)
				{
					this.mMacroTimer.Elapsed -= this.MacroTimer_Elapsed;
					this.mMacroTimer.Dispose();
					this.mPostOtsWelcomeWindow.Dispose();
				}
				Discord mDiscordhandler = this.mDiscordhandler;
				if (mDiscordhandler != null)
				{
					mDiscordhandler.Dispose();
				}
				CommonHandlers commonHandlers = this.mCommonHandler;
				if (commonHandlers != null)
				{
					commonHandlers.Dispose();
				}
				MacroRecorderWindow macroRecorderWindow = this.mMacroRecorderWindow;
				if (macroRecorderWindow != null)
				{
					macroRecorderWindow.Dispose();
				}
				BlueStacksUIUtils blueStacksUIUtils = this.mUtils;
				if (blueStacksUIUtils != null)
				{
					blueStacksUIUtils.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x00094B30 File Offset: 0x00092D30
		~MainWindow()
		{
			this.Dispose(false);
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x00010C33 File Offset: 0x0000EE33
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x00010C42 File Offset: 0x0000EE42
		private void mFullscreenSidebarButton_Click(object sender, RoutedEventArgs e)
		{
			this.mSidebar.ToggleSidebarVisibilityInFullscreen(true);
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x00010C50 File Offset: 0x0000EE50
		private void SidebarButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.mIsSideButtonDragging = true;
			this.mOldSideButtonMargin = (sender as global::System.Windows.Controls.Button).Margin;
			this.mSideButtonOldPosition = e.GetPosition(this);
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x00094B60 File Offset: 0x00092D60
		private void SidebarButton_MouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			global::System.Windows.Point position = e.GetPosition(this);
			if (this.mIsSideButtonDragging && position.Y > 0.0 && position.Y < this.mFullscreenSidebarPopupButtonInnerGrid.ActualHeight)
			{
				this.mFullscreenSidebarButton.Margin = new Thickness(0.0, this.mOldSideButtonMargin.Top + 2.0 * (position.Y - this.mSideButtonOldPosition.Y), 0.0, 0.0);
			}
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x00010C77 File Offset: 0x0000EE77
		private void SidebarButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mIsSideButtonDragging)
			{
				this.mIsSideButtonDragging = false;
				this.mSideButtonOldPosition = default(global::System.Windows.Point);
			}
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x00010C94 File Offset: 0x0000EE94
		private void mFullscreenTopbarButton_Click(object sender, RoutedEventArgs e)
		{
			this.mTopbarOptions.ToggleTopbarVisibilityInFullscreen(true);
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x00010CA2 File Offset: 0x0000EEA2
		private void TopbarButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.mIsTopButtonDragging = true;
			this.mOldTopButtonMargin = (sender as global::System.Windows.Controls.Button).Margin;
			this.mTopButtonOldPosition = e.GetPosition(this);
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x00094BF8 File Offset: 0x00092DF8
		private void TopbarButton_MouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			global::System.Windows.Point position = e.GetPosition(this);
			if (this.mIsTopButtonDragging && position.X > 0.0 && position.X < this.mFullscreenTopbarPopupButtonInnerGrid.ActualWidth)
			{
				this.mFullscreenTopbarButton.Margin = new Thickness(this.mOldTopButtonMargin.Left + 2.0 * (position.X - this.mTopButtonOldPosition.X), 0.0, 0.0, 0.0);
			}
		}

		// Token: 0x060018E8 RID: 6376 RVA: 0x00010CC9 File Offset: 0x0000EEC9
		private void TopbarButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.mIsTopButtonDragging)
			{
				this.mIsTopButtonDragging = false;
				this.mTopButtonOldPosition = default(global::System.Windows.Point);
			}
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x00010CE6 File Offset: 0x0000EEE6
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 2)
			{
				((Grid)target).Loaded += this.ResizeGrid_Loaded;
			}
		}

		// Token: 0x04000F32 RID: 3890
		private Mutex mBlueStacksClientInstanceLock;

		// Token: 0x04000F33 RID: 3891
		private int heightDiffScaled = 42;

		// Token: 0x04000F34 RID: 3892
		private int widthDiffScaled = 2;

		// Token: 0x04000F35 RID: 3893
		internal Fraction mAspectRatio = new Fraction(16L, 9L);

		// Token: 0x04000F36 RID: 3894
		private const long OneGB = 1073741824L;

		// Token: 0x04000F37 RID: 3895
		internal int MinWidthScaled;

		// Token: 0x04000F38 RID: 3896
		internal int MinHeightScaled;

		// Token: 0x04000F39 RID: 3897
		internal int MaxHeightScaled = 10000;

		// Token: 0x04000F3A RID: 3898
		internal int MaxWidthScaled = 10000;

		// Token: 0x04000F3B RID: 3899
		internal bool mIsDmmMaximised;

		// Token: 0x04000F3C RID: 3900
		internal bool mIsDMMMaximizedFromPortrait;

		// Token: 0x04000F3D RID: 3901
		internal bool mIsDMMRecommendedWindowOpen = true;

		// Token: 0x04000F3E RID: 3902
		internal Rect DmmRestoreWindowRectangle = new Rect(0.0, 0.0, 0.0, 0.0);

		// Token: 0x04000F3F RID: 3903
		internal DMMFullScreenTopBar mDMMFST;

		// Token: 0x04000F40 RID: 3904
		internal DMMRecommendedWindow mDMMRecommendedWindow;

		// Token: 0x04000F41 RID: 3905
		private bool mIsWindowResizedOnce;

		// Token: 0x04000F42 RID: 3906
		internal bool mIsFullScreenFromMaximized;

		// Token: 0x04000F43 RID: 3907
		internal bool mIsMinimizedThroughCloseButton;

		// Token: 0x04000F44 RID: 3908
		internal bool mIsStreaming;

		// Token: 0x04000F45 RID: 3909
		private bool isSetupDone;

		// Token: 0x04000F46 RID: 3910
		private double? mPreviousWidth;

		// Token: 0x04000F47 RID: 3911
		private double? mPreviousHeight;

		// Token: 0x04000F48 RID: 3912
		internal bool IsUIInPortraitMode;

		// Token: 0x04000F49 RID: 3913
		internal bool IsUIInPortraitModeWhenMaximized;

		// Token: 0x04000F4A RID: 3914
		private Grid mLastVisibleGrid;

		// Token: 0x04000F4B RID: 3915
		internal QuitPopupBrowserControl mQuitPopupBrowserControl;

		// Token: 0x04000F4C RID: 3916
		internal bool mIsFullScreen;

		// Token: 0x04000F4D RID: 3917
		internal static double sScalingFactor = 1.0;

		// Token: 0x04000F4E RID: 3918
		internal static bool sIsClosingForBackupRestore = false;

		// Token: 0x04000F4F RID: 3919
		internal static bool sShowNotifications = true;

		// Token: 0x04000F50 RID: 3920
		internal bool mIsFocusComeFromImap;

		// Token: 0x04000F51 RID: 3921
		private IMConfig mSelectedConfig;

		// Token: 0x04000F52 RID: 3922
		private IMConfig mOriginalLoadedConfig;

		// Token: 0x04000F53 RID: 3923
		internal bool mClosed;

		// Token: 0x04000F54 RID: 3924
		private bool mIsGamepadConnected;

		// Token: 0x04000F55 RID: 3925
		internal Dictionary<string, bool> AppForcedOrientationDict = new Dictionary<string, bool>();

		// Token: 0x04000F57 RID: 3927
		private bool mSkipNextGamepadStatus;

		// Token: 0x04000F58 RID: 3928
		internal string mCallbackEnabled = "False";

		// Token: 0x04000F5B RID: 3931
		private Grid mResizeGrid;

		// Token: 0x04000F5C RID: 3932
		internal bool mIsResizing;

		// Token: 0x04000F5D RID: 3933
		internal EventHandler ResizeBegin;

		// Token: 0x04000F5E RID: 3934
		internal EventHandler ResizeEnd;

		// Token: 0x04000F5F RID: 3935
		private bool mClosing;

		// Token: 0x04000F60 RID: 3936
		internal bool mGuestBootCompleted;

		// Token: 0x04000F61 RID: 3937
		internal bool mEnableLaunchPlayForNCSoft;

		// Token: 0x04000F62 RID: 3938
		internal volatile bool mIsWindowInFocus;

		// Token: 0x04000F6E RID: 3950
		internal string mBrowserCallbackFunctionName;

		// Token: 0x04000F6F RID: 3951
		internal DateTime mBootStartTime = DateTime.Now;

		// Token: 0x04000F70 RID: 3952
		internal bool IsQuitPopupNotficationReceived;

		// Token: 0x04000F71 RID: 3953
		private Grid mFirebaseBrowserControlGrid;

		// Token: 0x04000F72 RID: 3954
		internal static Dictionary<string, string> sMacroMapping = new Dictionary<string, string>();

		// Token: 0x04000F73 RID: 3955
		internal MacroRecording mAutoRunMacro;

		// Token: 0x04000F74 RID: 3956
		private ScreenLockControl mScreenLock;

		// Token: 0x04000F75 RID: 3957
		private MacroOverlay mMacroOverlay;

		// Token: 0x04000F76 RID: 3958
		internal CommonHandlers mCommonHandler;

		// Token: 0x04000F77 RID: 3959
		internal FrontendHandler mFrontendHandler;

		// Token: 0x04000F78 RID: 3960
		internal DownloadInstallApk mAppInstaller;

		// Token: 0x04000F79 RID: 3961
		internal AppHandler mAppHandler;

		// Token: 0x04000F7A RID: 3962
		internal bool mStreamingModeEnabled;

		// Token: 0x04000F7B RID: 3963
		internal PostOtsWelcomeWindowControl mPostOtsWelcomeWindow;

		// Token: 0x04000F7C RID: 3964
		private MacroRecorderWindow mMacroRecorderWindow;

		// Token: 0x04000F7D RID: 3965
		internal SynchronizerWindow mSynchronizerWindow;

		// Token: 0x04000F7E RID: 3966
		internal List<string> mSelectedInstancesForSync = new List<string>();

		// Token: 0x04000F7F RID: 3967
		internal bool mIsMacroPlaying;

		// Token: 0x04000F80 RID: 3968
		internal string mMacroPlaying = string.Empty;

		// Token: 0x04000F81 RID: 3969
		internal bool mIsScriptsPresent;

		// Token: 0x04000F82 RID: 3970
		internal global::System.Timers.Timer mMacroTimer;

		// Token: 0x04000F83 RID: 3971
		internal bool mIsSyncMaster;

		// Token: 0x04000F84 RID: 3972
		private BlueStacksUIUtils mUtils;

		// Token: 0x04000F85 RID: 3973
		internal WindowWndProcHandler mResizeHandler;

		// Token: 0x04000F86 RID: 3974
		private MainWindowsStaticComponents mStaticComponents;

		// Token: 0x04000F87 RID: 3975
		internal string mVmName = Strings.CurrentDefaultVmName;

		// Token: 0x04000F88 RID: 3976
		private bool mIsTokenAvailable;

		// Token: 0x04000F89 RID: 3977
		private readonly bool mIsWindowLoadedOnce;

		// Token: 0x04000F8A RID: 3978
		internal DimOverlayControl mDimOverlay;

		// Token: 0x04000F8B RID: 3979
		internal IntPtr Handle;

		// Token: 0x04000F8C RID: 3980
		internal bool mIsRestart;

		// Token: 0x04000F8D RID: 3981
		private Storyboard mStoryBoard;

		// Token: 0x04000F8E RID: 3982
		internal bool mIsMacroRecorderActive;

		// Token: 0x04000F90 RID: 3984
		internal bool mIsSynchronisationActive;

		// Token: 0x04000F94 RID: 3988
		internal bool mStartupTabLaunched;

		// Token: 0x04000F95 RID: 3989
		internal bool mLaunchStartupTabWhenTokenReceived;

		// Token: 0x04000F96 RID: 3990
		private readonly SerialWorkQueue pikaNotificationWorkQueue = new SerialWorkQueue("pikaNotificationWorkQueue");

		// Token: 0x04000F97 RID: 3991
		private bool isPikaPopOpen;

		// Token: 0x04000F98 RID: 3992
		private readonly DispatcherTimer pikaNotificationTimer = new DispatcherTimer();

		// Token: 0x04000F99 RID: 3993
		private readonly DispatcherTimer toastTimer = new DispatcherTimer();

		// Token: 0x04000F9A RID: 3994
		private readonly DispatcherTimer mFullScreenToastTimer = new DispatcherTimer();

		// Token: 0x04000F9B RID: 3995
		private bool mIsLockScreenActionPending;

		// Token: 0x04000F9C RID: 3996
		private bool disposedValue;

		// Token: 0x04000F9D RID: 3997
		private bool mIsSideButtonDragging;

		// Token: 0x04000F9E RID: 3998
		private global::System.Windows.Point mSideButtonOldPosition;

		// Token: 0x04000F9F RID: 3999
		private Thickness mOldSideButtonMargin;

		// Token: 0x04000FA0 RID: 4000
		private bool mIsTopButtonDragging;

		// Token: 0x04000FA1 RID: 4001
		private global::System.Windows.Point mTopButtonOldPosition;

		// Token: 0x04000FA2 RID: 4002
		private Thickness mOldTopButtonMargin;

		// Token: 0x02000295 RID: 661
		// (Invoke) Token: 0x06001923 RID: 6435
		public delegate void GuestBootCompletedEventHandler(object sender, EventArgs args);

		// Token: 0x02000296 RID: 662
		// (Invoke) Token: 0x06001927 RID: 6439
		internal delegate void CursorLockChangedEventHandler(object sender, MainWindowEventArgs.CursorLockChangedEventArgs args);

		// Token: 0x02000297 RID: 663
		// (Invoke) Token: 0x0600192B RID: 6443
		internal delegate void FullScreenChangedEventHandler(object sender, MainWindowEventArgs.FullScreenChangedEventArgs args);

		// Token: 0x02000298 RID: 664
		// (Invoke) Token: 0x0600192F RID: 6447
		internal delegate void FrontendGridVisibilityChangedEventHandler(object sender, MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs args);

		// Token: 0x02000299 RID: 665
		// (Invoke) Token: 0x06001933 RID: 6451
		internal delegate void BrowserOTSCompletedCallbackEventHandler(object sender, MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs args);
	}
}
