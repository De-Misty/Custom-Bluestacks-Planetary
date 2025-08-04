using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200010B RID: 267
	public class PostOtsWelcomeWindowControl : UserControl, IDisposable, IComponentConnector
	{
		// Token: 0x06000B17 RID: 2839 RVA: 0x00009018 File Offset: 0x00007218
		public PostOtsWelcomeWindowControl(MainWindow ParentWindow)
		{
			this.InitializeComponent();
			this.ParentWindow = ParentWindow;
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0003EDD8 File Offset: 0x0003CFD8
		private void PostOtsWelcome_Loaded(object sender, RoutedEventArgs e)
		{
			Logger.Info("PostOtsWelcome window loaded");
			this.loginSyncTimer = new Timer(10000.0);
			this.loginSyncTimer.Elapsed += this.OnLoginSyncTimeout;
			this.loginSyncTimer.AutoReset = false;
			if (!string.IsNullOrEmpty(RegistryManager.Instance.Token))
			{
				this.ChangeBasedonTokenReceived("true");
				return;
			}
			this.StartingTimer();
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0003EE4C File Offset: 0x0003D04C
		public void ChangeBasedonTokenReceived(string status)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					Logger.Info("In ChangeBasedonTokenReceived");
					this.mLoadingImage.Visibility = Visibility.Collapsed;
					if (status.Equals("true", StringComparison.InvariantCultureIgnoreCase))
					{
						this.mPostOtsImage.ImageName = "success_ots_icon";
						this.mPostOtsWarning.Visibility = Visibility.Collapsed;
						this.mCloseButton.Visibility = Visibility.Collapsed;
						BlueStacksUIBinding.Bind(this.mPostOtsLabel, "STRING_POST_OTS_SUCCESS_MESSAGE");
						BlueStacksUIBinding.Bind(this.mPostOtsButton, "STRING_POST_OTS_SUCCESS_BUTTON_MESSAGE");
						this.mSuccess = new bool?(true);
					}
					else
					{
						this.mPostOtsImage.ImageName = "failure_ots_icon";
						this.mPostOtsWarning.Visibility = Visibility.Visible;
						this.mCloseButton.Visibility = Visibility.Visible;
						BlueStacksUIBinding.Bind(this.mPostOtsLabel, "STRING_POST_OTS_FAILED_MESSAGE");
						BlueStacksUIBinding.Bind(this.mPostOtsButton, "STRING_POST_OTS_FAILED_BUTTON_MESSAGE");
						this.mSuccess = new bool?(false);
					}
					if (this.loginSyncTimer != null)
					{
						this.loginSyncTimer.Stop();
					}
					this.mPostOtsButton.IsEnabled = true;
				}
				catch (Exception ex)
				{
					Logger.Error(string.Concat(new string[]
					{
						" Exception in ChangeBasedOnTokenReceived Status: ",
						status,
						Environment.NewLine,
						"Error: ",
						ex.ToString()
					}));
				}
			}), new object[0]);
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0000902D File Offset: 0x0000722D
		private void StartingTimer()
		{
			Logger.Info("Starting Timer");
			this.loginSyncTimer.Stop();
			this.loginSyncTimer.Start();
			this.loginSyncTimer.Enabled = true;
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x0003EE8C File Offset: 0x0003D08C
		private void OnLoginSyncTimeout(object source, ElapsedEventArgs e)
		{
			try
			{
				Logger.Error("Login Sync timed out.");
				if (this.mSuccess == null)
				{
					this.ChangeBasedonTokenReceived("false");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in login sync timer timeout " + ex.ToString());
			}
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x0003EEE8 File Offset: 0x0003D0E8
		private void mPostOtsButton_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("mPostOtsButton clicked");
			if (this.mSuccess != null)
			{
				if (this.mSuccess.Value)
				{
					this.loginSyncTimer.Dispose();
					BlueStacksUIUtils.CloseContainerWindow(this);
					return;
				}
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.mPostOtsImage.ImageName = "syncing_ots_icon";
					this.mLoadingImage.Visibility = Visibility.Visible;
					this.mPostOtsWarning.Visibility = Visibility.Collapsed;
					this.mCloseButton.Visibility = Visibility.Collapsed;
					BlueStacksUIBinding.Bind(this.mPostOtsLabel, "STRING_POST_OTS_SYNCING_MESSAGE");
					BlueStacksUIBinding.Bind(this.mPostOtsButton, "STRING_POST_OTS_SYNCING_BUTTON_MESSAGE");
					this.mPostOtsButton.IsEnabled = false;
				}), new object[0]);
				this.SendRetryBluestacksLoginRequest(this.ParentWindow.mVmName);
			}
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0003EF5C File Offset: 0x0003D15C
		private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				Logger.Info("Clicked postotswelcome window close button");
				this.ParentWindow.CloseWindow();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in closing bluestacks from postotswelcome window, " + ex.ToString());
			}
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x0003EFA8 File Offset: 0x0003D1A8
		private void SendRetryBluestacksLoginRequest(string vmName)
		{
			try
			{
				Logger.Info("Sending retry call for token to android, since token is not received successfully");
				this.mSuccess = null;
				this.StartingTimer();
				BlueStacksUIUtils.SendBluestacksLoginRequest(vmName);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SendRetryBluestacksLoginRequest: " + ex.ToString());
			}
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x0000905B File Offset: 0x0000725B
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.loginSyncTimer != null)
				{
					this.loginSyncTimer.Elapsed -= this.OnLoginSyncTimeout;
					this.loginSyncTimer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0003F004 File Offset: 0x0003D204
		~PostOtsWelcomeWindowControl()
		{
			this.Dispose(false);
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x00009098 File Offset: 0x00007298
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x0003F034 File Offset: 0x0003D234
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/postotswelcomewindowcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x0003F064 File Offset: 0x0003D264
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
				((PostOtsWelcomeWindowControl)target).Loaded += this.PostOtsWelcome_Loaded;
				return;
			case 2:
				this.mCloseButton = (CustomPictureBox)target;
				this.mCloseButton.MouseLeftButtonUp += this.CloseButton_MouseLeftButtonUp;
				return;
			case 3:
				this.mPostOtsImage = (CustomPictureBox)target;
				return;
			case 4:
				this.mLoadingImage = (CustomPictureBox)target;
				return;
			case 5:
				this.mPostOtsLabel = (Label)target;
				return;
			case 6:
				this.mPostOtsWarning = (TextBlock)target;
				return;
			case 7:
				this.mPostOtsButton = (CustomButton)target;
				this.mPostOtsButton.Click += this.mPostOtsButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040006AB RID: 1707
		private bool? mSuccess;

		// Token: 0x040006AC RID: 1708
		private Timer loginSyncTimer;

		// Token: 0x040006AD RID: 1709
		private MainWindow ParentWindow;

		// Token: 0x040006AE RID: 1710
		private bool disposedValue;

		// Token: 0x040006AF RID: 1711
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseButton;

		// Token: 0x040006B0 RID: 1712
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPostOtsImage;

		// Token: 0x040006B1 RID: 1713
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mLoadingImage;

		// Token: 0x040006B2 RID: 1714
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mPostOtsLabel;

		// Token: 0x040006B3 RID: 1715
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mPostOtsWarning;

		// Token: 0x040006B4 RID: 1716
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mPostOtsButton;

		// Token: 0x040006B5 RID: 1717
		private bool _contentLoaded;
	}
}
