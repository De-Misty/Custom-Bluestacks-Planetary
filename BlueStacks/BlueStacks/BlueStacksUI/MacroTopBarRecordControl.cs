using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000C5 RID: 197
	public class MacroTopBarRecordControl : UserControl, IComponentConnector
	{
		// Token: 0x060007E1 RID: 2017 RVA: 0x00007094 File Offset: 0x00005294
		public MacroTopBarRecordControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x0002C0A0 File Offset: 0x0002A2A0
		internal void Init(MainWindow window)
		{
			this.ParentWindow = window;
			this.mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Render, new EventHandler(this.T_Tick), Dispatcher.CurrentDispatcher);
			this.mPlayMacroImg.Visibility = Visibility.Collapsed;
			this.mPauseMacroImg.Visibility = Visibility.Visible;
			this.mBlinkRecordingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), DispatcherPriority.Render, new EventHandler(this.BlinkRecordingIcon_Tick), Dispatcher.CurrentDispatcher);
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				BlueStacksUIBinding.Bind(this.ParentWindow.mNCTopBar.mMacroRecordingTooltip, "STRING_PAUSE_RECORDING_TOOLTIP", "");
				return;
			}
			BlueStacksUIBinding.Bind(this.ParentWindow.mTopBar.mMacroRecordingTooltip, "STRING_PAUSE_RECORDING_TOOLTIP", "");
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x000070A9 File Offset: 0x000052A9
		private void BlinkRecordingIcon_Tick(object sender, EventArgs e)
		{
			this.ToggleRecordingIcon();
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x0002C16C File Offset: 0x0002A36C
		private void PauseMacroRecording_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("pauseRecordingCombo", null);
			this.PauseTimer();
			this.mPauseMacroImg.Visibility = Visibility.Collapsed;
			this.mPlayMacroImg.Visibility = Visibility.Visible;
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_pause", null, RecordingTypes.SingleRecording.ToString(), null, null, null);
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x000070B1 File Offset: 0x000052B1
		private void ResumeMacroRecording_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startRecordingCombo", null);
			this.ResumeTimer();
			this.mPlayMacroImg.Visibility = Visibility.Collapsed;
			this.mPauseMacroImg.Visibility = Visibility.Visible;
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x0002C1E4 File Offset: 0x0002A3E4
		private void StopMacroRecording_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.StopMacroRecording();
			this.mBlinkRecordingIconTimer.Stop();
			this.mRecordingImage.ImageName = "recording_macro_title_bar";
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_stop", null, RecordingTypes.SingleRecording.ToString(), null, null, null);
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x000070E7 File Offset: 0x000052E7
		internal void StopTimer()
		{
			this.mTimer.Stop();
			this.mBlinkRecordingIconTimer.Stop();
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x000070FF File Offset: 0x000052FF
		internal void StartTimer()
		{
			this.mTimer.Start();
			this.mStartTime = DateTime.Now;
			this.mBlinkRecordingIconTimer.Start();
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x0002C254 File Offset: 0x0002A454
		private void T_Tick(object sender, EventArgs e)
		{
			TimeSpan timeSpan = DateTime.Now - this.mStartTime;
			string text = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", new object[]
			{
				timeSpan.Minutes,
				timeSpan.Seconds,
				timeSpan.Milliseconds / 10
			});
			this.TimerDisplay.Text = text;
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x00007122 File Offset: 0x00005322
		private void ToggleRecordingIcon()
		{
			if (this.mShowRecordingIcon)
			{
				this.mRecordingImage.ImageName = "recording_macro_active";
				this.mShowRecordingIcon = false;
				return;
			}
			this.mRecordingImage.ImageName = "recording_macro";
			this.mShowRecordingIcon = true;
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0000715B File Offset: 0x0000535B
		internal void PauseTimer()
		{
			this.mTimer.IsEnabled = false;
			this.mTimer.Stop();
			this.mPauseTime = DateTime.Now;
			this.mBlinkRecordingIconTimer.Stop();
			this.mShowRecordingIcon = true;
			this.ToggleRecordingIcon();
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x0002C2C4 File Offset: 0x0002A4C4
		internal void ResumeTimer()
		{
			TimeSpan timeSpan = DateTime.Now - this.mPauseTime;
			this.mStartTime += timeSpan;
			this.mTimer.IsEnabled = true;
			this.mTimer.Start();
			this.mBlinkRecordingIconTimer.Start();
			this.mShowRecordingIcon = true;
			this.ToggleRecordingIcon();
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0002C324 File Offset: 0x0002A524
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrotopbarrecordcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0002C354 File Offset: 0x0002A554
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
				this.mMaskBorder = (Border)target;
				return;
			case 2:
				this.mRecordingImage = (CustomPictureBox)target;
				return;
			case 3:
				this.TimerDisplay = (TextBlock)target;
				return;
			case 4:
				this.mPauseMacroImg = (CustomPictureBox)target;
				this.mPauseMacroImg.PreviewMouseLeftButtonUp += this.PauseMacroRecording_MouseLeftButtonUp;
				return;
			case 5:
				this.mPlayMacroImg = (CustomPictureBox)target;
				this.mPlayMacroImg.PreviewMouseLeftButtonUp += this.ResumeMacroRecording_MouseLeftButtonUp;
				return;
			case 6:
				this.mStopMacroImg = (CustomPictureBox)target;
				this.mStopMacroImg.PreviewMouseLeftButtonUp += this.StopMacroRecording_PreviewMouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400043F RID: 1087
		private MainWindow ParentWindow;

		// Token: 0x04000440 RID: 1088
		private DispatcherTimer mTimer;

		// Token: 0x04000441 RID: 1089
		private DispatcherTimer mBlinkRecordingIconTimer;

		// Token: 0x04000442 RID: 1090
		private DateTime mStartTime;

		// Token: 0x04000443 RID: 1091
		private DateTime mPauseTime;

		// Token: 0x04000444 RID: 1092
		private bool mShowRecordingIcon = true;

		// Token: 0x04000445 RID: 1093
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000446 RID: 1094
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mRecordingImage;

		// Token: 0x04000447 RID: 1095
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock TimerDisplay;

		// Token: 0x04000448 RID: 1096
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPauseMacroImg;

		// Token: 0x04000449 RID: 1097
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPlayMacroImg;

		// Token: 0x0400044A RID: 1098
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mStopMacroImg;

		// Token: 0x0400044B RID: 1099
		private bool _contentLoaded;
	}
}
