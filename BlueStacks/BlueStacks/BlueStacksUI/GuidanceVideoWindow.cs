using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000161 RID: 353
	public class GuidanceVideoWindow : CustomWindow, IDisposable, IComponentConnector
	{
		// Token: 0x06000E9E RID: 3742 RVA: 0x0000AE0E File Offset: 0x0000900E
		public GuidanceVideoWindow(MainWindow parentWindow)
		{
			this.ParentWindow = parentWindow;
			this.InitializeComponent();
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x0005CA38 File Offset: 0x0005AC38
		private void GuidanceVideoWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs eventArgs)
		{
			if (base.IsVisible)
			{
				ClientStats.SendKeyMappingUIStatsAsync("video_clicked", KMManager.sPackageName, KMManager.sVideoMode.ToString());
				this.mBrowser = new BrowserControl();
				BrowserControl browserControl = this.mBrowser;
				string sPackageName = KMManager.sPackageName;
				string text = KMManager.sVideoMode.ToString().ToLower(CultureInfo.InvariantCulture);
				MainWindow parentWindow = this.ParentWindow;
				string text2;
				if (parentWindow == null)
				{
					text2 = null;
				}
				else
				{
					IMConfig selectedConfig = parentWindow.SelectedConfig;
					if (selectedConfig == null)
					{
						text2 = null;
					}
					else
					{
						IMControlScheme selectedControlScheme = selectedConfig.SelectedControlScheme;
						text2 = ((selectedControlScheme != null) ? selectedControlScheme.Name : null);
					}
				}
				browserControl.InitBaseControl(BlueStacksUIUtils.GetVideoTutorialUrl(sPackageName, text, text2), 0f);
				this.mBrowser.ParentWindow = this.ParentWindow;
				this.mBrowser.Visibility = Visibility.Visible;
				this.mBrowserGrid.Children.Add(this.mBrowser);
			}
			try
			{
				if ((bool)eventArgs.NewValue)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary["allInstances"] = "False";
					dictionary["explicit"] = "False";
					Dictionary<string, string> dictionary2 = dictionary;
					HTTPUtils.SendRequestToEngineAsync("mute", dictionary2, this.ParentWindow.mVmName, 0, null, false, 1, 0);
					this.ParentWindow.mCommonHandler.OnVolumeMuted(true);
				}
				else if (!RegistryManager.Instance.AreAllInstancesMuted && !this.ParentWindow.EngineInstanceRegistry.IsMuted)
				{
					Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
					dictionary3["allInstances"] = "False";
					Dictionary<string, string> dictionary4 = dictionary3;
					HTTPUtils.SendRequestToEngineAsync("unmute", dictionary4, this.ParentWindow.mVmName, 0, null, false, 1, 0);
					this.ParentWindow.mCommonHandler.OnVolumeMuted(false);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
			}
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x0000AE23 File Offset: 0x00009023
		internal void CloseWindow()
		{
			if (this.mBrowser != null)
			{
				this.mBrowser.DisposeBrowser();
				this.mBrowserGrid.Children.Remove(this.mBrowser);
				this.mBrowser = null;
			}
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x00006D61 File Offset: 0x00004F61
		private void CloseButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x0000AE55 File Offset: 0x00009055
		private void mWindow_Closing(object sender, CancelEventArgs e)
		{
			this.CloseWindow();
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x0000AE5D File Offset: 0x0000905D
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				BrowserControl browserControl = this.mBrowser;
				if (browserControl != null)
				{
					browserControl.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x0005CBFC File Offset: 0x0005ADFC
		~GuidanceVideoWindow()
		{
			this.Dispose(false);
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x0000AE81 File Offset: 0x00009081
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0005CC2C File Offset: 0x0005AE2C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/guidancevideowindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x0005CC5C File Offset: 0x0005AE5C
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
				this.mWindow = (GuidanceVideoWindow)target;
				this.mWindow.IsVisibleChanged += this.GuidanceVideoWindow_IsVisibleChanged;
				this.mWindow.Closing += this.mWindow_Closing;
				return;
			case 2:
				this.mMainBrowserGrid = (Grid)target;
				return;
			case 3:
				this.mMaskBorder = (Border)target;
				return;
			case 4:
				this.mBrowserGrid = (Grid)target;
				return;
			case 5:
				((CustomPictureBox)target).PreviewMouseUp += this.CloseButton_PreviewMouseUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400094E RID: 2382
		private BrowserControl mBrowser;

		// Token: 0x0400094F RID: 2383
		internal MainWindow ParentWindow;

		// Token: 0x04000950 RID: 2384
		private bool disposedValue;

		// Token: 0x04000951 RID: 2385
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal GuidanceVideoWindow mWindow;

		// Token: 0x04000952 RID: 2386
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMainBrowserGrid;

		// Token: 0x04000953 RID: 2387
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000954 RID: 2388
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBrowserGrid;

		// Token: 0x04000955 RID: 2389
		private bool _contentLoaded;
	}
}
