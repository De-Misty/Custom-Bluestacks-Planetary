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
	// Token: 0x020000C2 RID: 194
	public class MacroTopBarPlayControl : UserControl, IComponentConnector
	{
		// Token: 0x17000221 RID: 545
		// (get) Token: 0x060007C1 RID: 1985 RVA: 0x00006F87 File Offset: 0x00005187
		// (set) Token: 0x060007C2 RID: 1986 RVA: 0x00006F8F File Offset: 0x0000518F
		public DateTime mStartTime { get; set; }

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060007C3 RID: 1987 RVA: 0x0002BC1C File Offset: 0x00029E1C
		// (remove) Token: 0x060007C4 RID: 1988 RVA: 0x0002BC54 File Offset: 0x00029E54
		internal event MacroTopBarPlayControl.ScriptPlayDelegate ScriptPlayEvent;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x060007C5 RID: 1989 RVA: 0x0002BC8C File Offset: 0x00029E8C
		// (remove) Token: 0x060007C6 RID: 1990 RVA: 0x0002BCC4 File Offset: 0x00029EC4
		internal event MacroTopBarPlayControl.ScriptStopDelegate ScriptStopEvent;

		// Token: 0x060007C7 RID: 1991 RVA: 0x00006F98 File Offset: 0x00005198
		public MacroTopBarPlayControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x00006FAD File Offset: 0x000051AD
		internal void OnScriptPlayEvent(string tag)
		{
			MacroTopBarPlayControl.ScriptPlayDelegate scriptPlayEvent = this.ScriptPlayEvent;
			if (scriptPlayEvent == null)
			{
				return;
			}
			scriptPlayEvent(tag);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00006FC0 File Offset: 0x000051C0
		private void BlinkPlayingIcon_Tick(object sender, EventArgs e)
		{
			this.ToggleRecordingIcon();
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00006FC8 File Offset: 0x000051C8
		internal void StopTimer()
		{
			this.mBlinkPlayingIconTimer.Stop();
			this.mTimer.Stop();
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00006FE0 File Offset: 0x000051E0
		internal void StartTimer()
		{
			this.mBlinkPlayingIconTimer.Start();
			this.mTimer.Start();
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00006FF8 File Offset: 0x000051F8
		private void ToggleRecordingIcon()
		{
			if (this.mShowPlayingIcon)
			{
				this.RecordingImage.ImageName = "recording_macro_title_play";
				this.mShowPlayingIcon = false;
				return;
			}
			this.RecordingImage.ImageName = "recording_macro";
			this.mShowPlayingIcon = true;
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00007031 File Offset: 0x00005231
		internal void OnScriptStopEvent(string tag)
		{
			MacroTopBarPlayControl.ScriptStopDelegate scriptStopEvent = this.ScriptStopEvent;
			if (scriptStopEvent == null)
			{
				return;
			}
			scriptStopEvent(tag);
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0002BCFC File Offset: 0x00029EFC
		internal void Init(MainWindow parentWindow, MacroRecording record)
		{
			this.ParentWindow = parentWindow;
			this.mOperationsRecord = record;
			this.mRunningScript.Text = this.mOperationsRecord.Name;
			this.mRunningIterations.Visibility = Visibility.Visible;
			this.mRunningScript.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				LocaleStrings.GetLocalizedString("STRING_PLAYING", ""),
				this.mRunningScript.Text
			});
			if (this.mBlinkPlayingIconTimer == null)
			{
				this.mBlinkPlayingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), DispatcherPriority.Render, new EventHandler(this.BlinkPlayingIcon_Tick), Dispatcher.CurrentDispatcher);
				this.mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Render, new EventHandler(this.T_Tick), Dispatcher.CurrentDispatcher);
				this.StartTimer();
			}
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0002BDDC File Offset: 0x00029FDC
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

		// Token: 0x060007D0 RID: 2000 RVA: 0x00007044 File Offset: 0x00005244
		internal void IncreaseIteration(int iteration)
		{
			this.mRunningIterations.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RUNNING_X_TIME", ""), new object[] { Strings.AddOrdinal(iteration) });
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x00004786 File Offset: 0x00002986
		private void PauseMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x00007079 File Offset: 0x00005279
		private void PlayMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.OnScriptPlayEvent(this.mOperationsRecord.Name);
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0000708C File Offset: 0x0000528C
		private void StopMacro_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.StopMacro();
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x0002BE4C File Offset: 0x0002A04C
		public void StopMacro()
		{
			this.StopTimer();
			this.mBlinkPlayingIconTimer = null;
			this.ParentWindow.mCommonHandler.StopMacroScriptHandling();
			MacroTopBarPlayControl.ScriptStopDelegate scriptStopEvent = this.ScriptStopEvent;
			if (scriptStopEvent != null)
			{
				scriptStopEvent(this.mOperationsRecord.Name);
			}
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_stop", null, this.mOperationsRecord.RecordingType.ToString(), null, null, null);
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x0002BED4 File Offset: 0x0002A0D4
		internal void UpdateUiForIterationTillTime()
		{
			this.mRunningIterations.Visibility = Visibility.Collapsed;
			this.mTimerDisplay.Visibility = Visibility.Visible;
			this.mRunningScript.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}-{1}sec", new object[]
			{
				this.mOperationsRecord.Name,
				this.mOperationsRecord.LoopTime
			});
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0002BF3C File Offset: 0x0002A13C
		internal void UpdateUiMacroPlaybackForInfiniteTime(int iteration)
		{
			this.mTimerDisplay.Visibility = Visibility.Collapsed;
			this.mRunningIterations.Visibility = Visibility.Visible;
			this.mRunningIterations.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RUNNING_X_TIME", ""), new object[] { Strings.AddOrdinal(iteration) });
			this.mRunningScript.ToolTip = string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { this.mOperationsRecord.Name });
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0002BFC4 File Offset: 0x0002A1C4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrotopbarplaycontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0002BFF4 File Offset: 0x0002A1F4
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
				this.RecordingImage = (CustomPictureBox)target;
				return;
			case 3:
				this.mDescriptionPanel = (StackPanel)target;
				return;
			case 4:
				this.mRunningScript = (TextBlock)target;
				return;
			case 5:
				this.mRunningIterations = (TextBlock)target;
				return;
			case 6:
				this.mTimerDisplay = (TextBlock)target;
				return;
			case 7:
				this.StopMacroImg = (CustomPictureBox)target;
				this.StopMacroImg.PreviewMouseLeftButtonUp += this.StopMacro_PreviewMouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400042F RID: 1071
		private MainWindow ParentWindow;

		// Token: 0x04000430 RID: 1072
		internal MacroRecording mOperationsRecord;

		// Token: 0x04000431 RID: 1073
		private DispatcherTimer mBlinkPlayingIconTimer;

		// Token: 0x04000432 RID: 1074
		private DispatcherTimer mTimer;

		// Token: 0x04000434 RID: 1076
		private bool mShowPlayingIcon = true;

		// Token: 0x04000437 RID: 1079
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000438 RID: 1080
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox RecordingImage;

		// Token: 0x04000439 RID: 1081
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mDescriptionPanel;

		// Token: 0x0400043A RID: 1082
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mRunningScript;

		// Token: 0x0400043B RID: 1083
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mRunningIterations;

		// Token: 0x0400043C RID: 1084
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTimerDisplay;

		// Token: 0x0400043D RID: 1085
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox StopMacroImg;

		// Token: 0x0400043E RID: 1086
		private bool _contentLoaded;

		// Token: 0x020000C3 RID: 195
		// (Invoke) Token: 0x060007DA RID: 2010
		internal delegate void ScriptPlayDelegate(string tag);

		// Token: 0x020000C4 RID: 196
		// (Invoke) Token: 0x060007DE RID: 2014
		internal delegate void ScriptStopDelegate(string tag);
	}
}
