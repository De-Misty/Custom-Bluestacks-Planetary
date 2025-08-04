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
	// Token: 0x02000132 RID: 306
	public class VideoRecordingStatus : UserControl, IComponentConnector
	{
		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000C52 RID: 3154 RVA: 0x00009BE6 File Offset: 0x00007DE6
		// (set) Token: 0x06000C53 RID: 3155 RVA: 0x00009BEE File Offset: 0x00007DEE
		public DateTime mStartTime { get; set; }

		// Token: 0x06000C54 RID: 3156 RVA: 0x00009BF7 File Offset: 0x00007DF7
		public VideoRecordingStatus()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x00009C0C File Offset: 0x00007E0C
		private void StopRecord_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ResetTimer();
			Action recordingStoppedEvent = this.RecordingStoppedEvent;
			if (recordingStoppedEvent != null)
			{
				recordingStoppedEvent();
			}
			this.ParentWindow.mCommonHandler.StopRecordVideo();
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x00009C35 File Offset: 0x00007E35
		private void BlinkPlayingIcon_Tick(object sender, EventArgs e)
		{
			this.ToggleRecordingIcon();
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x00044FA8 File Offset: 0x000431A8
		internal void StopTimer()
		{
			DispatcherTimer dispatcherTimer = this.mBlinkPlayingIconTimer;
			if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
			{
				DispatcherTimer dispatcherTimer2 = this.mBlinkPlayingIconTimer;
				if (dispatcherTimer2 != null)
				{
					dispatcherTimer2.Stop();
				}
			}
			DispatcherTimer dispatcherTimer3 = this.mTimer;
			if (dispatcherTimer3 != null && dispatcherTimer3.IsEnabled)
			{
				DispatcherTimer dispatcherTimer4 = this.mTimer;
				if (dispatcherTimer4 == null)
				{
					return;
				}
				dispatcherTimer4.Stop();
			}
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x00009C3D File Offset: 0x00007E3D
		internal void StartTimer()
		{
			this.mBlinkPlayingIconTimer.Start();
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x00045000 File Offset: 0x00043200
		private void ToggleRecordingIcon()
		{
			if (this.mToggleBlinkImage)
			{
				this.mRecordingImage.ImageName = "sidebar_video_capture";
			}
			else
			{
				this.mRecordingImage.ImageName = "sidebar_video_capture_active";
			}
			if (FeatureManager.Instance.IsCustomUIForNCSoft && this.ParentWindow.mSidebar != null)
			{
				this.ParentWindow.mSidebar.ChangeVideoRecordingImage(this.mRecordingImage.ImageName);
			}
			this.mToggleBlinkImage = !this.mToggleBlinkImage;
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x0004507C File Offset: 0x0004327C
		internal void Init(MainWindow parentWindow)
		{
			this.ParentWindow = parentWindow;
			if (this.mBlinkPlayingIconTimer == null)
			{
				this.mBlinkPlayingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), DispatcherPriority.Render, new EventHandler(this.BlinkPlayingIcon_Tick), Dispatcher.CurrentDispatcher);
				this.mStartTime = DateTime.Now;
				this.mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Render, new EventHandler(this.T_Tick), Dispatcher.CurrentDispatcher);
				this.StartTimer();
			}
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x000450FC File Offset: 0x000432FC
		private void T_Tick(object sender, EventArgs e)
		{
			TimeSpan timeSpan = DateTime.Now - this.mStartTime;
			string text = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", new object[]
			{
				timeSpan.Minutes,
				timeSpan.Seconds,
				timeSpan.Milliseconds / 10
			});
			this.mTimerDisplay.Text = text;
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x00009C4A File Offset: 0x00007E4A
		internal void ResetTimer()
		{
			this.StopTimer();
			this.mBlinkPlayingIconTimer = null;
			this.mTimer = null;
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x0004516C File Offset: 0x0004336C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/videorecordingstatus.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0004519C File Offset: 0x0004339C
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
				this.mDescriptionPanel = (StackPanel)target;
				return;
			case 4:
				this.mRunningVideo = (TextBlock)target;
				return;
			case 5:
				this.mTimerDisplay = (TextBlock)target;
				return;
			case 6:
				this.mStopVideoRecordImg = (CustomPictureBox)target;
				this.mStopVideoRecordImg.PreviewMouseLeftButtonUp += this.StopRecord_PreviewMouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400078D RID: 1933
		private MainWindow ParentWindow;

		// Token: 0x0400078E RID: 1934
		private DispatcherTimer mBlinkPlayingIconTimer;

		// Token: 0x0400078F RID: 1935
		private DispatcherTimer mTimer;

		// Token: 0x04000791 RID: 1937
		private bool mToggleBlinkImage = true;

		// Token: 0x04000792 RID: 1938
		internal Action RecordingStoppedEvent;

		// Token: 0x04000793 RID: 1939
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000794 RID: 1940
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mRecordingImage;

		// Token: 0x04000795 RID: 1941
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mDescriptionPanel;

		// Token: 0x04000796 RID: 1942
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mRunningVideo;

		// Token: 0x04000797 RID: 1943
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTimerDisplay;

		// Token: 0x04000798 RID: 1944
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mStopVideoRecordImg;

		// Token: 0x04000799 RID: 1945
		private bool _contentLoaded;
	}
}
