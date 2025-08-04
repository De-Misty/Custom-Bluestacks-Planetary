using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using BlueStacks.Common.Grm;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows7.Multitouch;
using Windows7.Multitouch.Manipulation;
using Windows7.Multitouch.WPF;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000210 RID: 528
	public class BrowserControl : UserControl, IDisposable
	{
		// Token: 0x1700032A RID: 810
		// (get) Token: 0x0600141B RID: 5147 RVA: 0x0000DEDD File Offset: 0x0000C0DD
		// (set) Token: 0x0600141C RID: 5148 RVA: 0x0000DF0B File Offset: 0x0000C10B
		public MainWindow ParentWindow
		{
			get
			{
				if (this.mMainWindow == null)
				{
					base.Dispatcher.Invoke(new Action(delegate
					{
						this.mMainWindow = Window.GetWindow(this) as MainWindow;
					}), new object[0]);
				}
				return this.mMainWindow;
			}
			set
			{
				this.mMainWindow = value;
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x0600141D RID: 5149 RVA: 0x0000DF14 File Offset: 0x0000C114
		// (set) Token: 0x0600141E RID: 5150 RVA: 0x0000DF30 File Offset: 0x0000C130
		public NoInternetControl NoInternetControl
		{
			get
			{
				if (this.mNoInternetControl == null)
				{
					this.mNoInternetControl = new NoInternetControl(this);
				}
				return this.mNoInternetControl;
			}
			set
			{
				this.mNoInternetControl = value;
			}
		}

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x0600141F RID: 5151 RVA: 0x00079790 File Offset: 0x00077990
		// (remove) Token: 0x06001420 RID: 5152 RVA: 0x000797C8 File Offset: 0x000779C8
		public event ProcessMessageEventHandler ProcessMessageRecieved;

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06001421 RID: 5153 RVA: 0x00079800 File Offset: 0x00077A00
		// (remove) Token: 0x06001422 RID: 5154 RVA: 0x00079838 File Offset: 0x00077A38
		public event Action BrowserLoadCompleteEvent;

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x0000DF39 File Offset: 0x0000C139
		// (set) Token: 0x06001424 RID: 5156 RVA: 0x0000DF41 File Offset: 0x0000C141
		public Grid mGrid { get; set; }

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x0000DF4A File Offset: 0x0000C14A
		public Dictionary<BrowserControlTags, JObject> TagsSubscribedDict { get; } = new Dictionary<BrowserControlTags, JObject>();

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06001426 RID: 5158 RVA: 0x0000DF52 File Offset: 0x0000C152
		// (set) Token: 0x06001427 RID: 5159 RVA: 0x0000DF5A File Offset: 0x0000C15A
		public BrowserSubscriber mSubscriber { get; set; }

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x0000DF63 File Offset: 0x0000C163
		// (set) Token: 0x06001429 RID: 5161 RVA: 0x00079870 File Offset: 0x00077A70
		internal Browser CefBrowser
		{
			get
			{
				return this.mBrowser;
			}
			set
			{
				this.mBrowser = value;
				if (this.mBrowser == null)
				{
					foreach (BrowserControlTags browserControlTags in this.TagsSubscribedDict.Keys)
					{
						BrowserSubscriber mSubscriber = this.mSubscriber;
						if (mSubscriber != null)
						{
							mSubscriber.UnsubscribeTag(browserControlTags);
						}
					}
				}
			}
		}

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x0600142A RID: 5162 RVA: 0x000798E4 File Offset: 0x00077AE4
		// (remove) Token: 0x0600142B RID: 5163 RVA: 0x0007991C File Offset: 0x00077B1C
		public event Action BrowserFallbackUrlEvent;

		// Token: 0x0600142C RID: 5164 RVA: 0x00079954 File Offset: 0x00077B54
		public BrowserControl()
		{
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x0000DF6B File Offset: 0x0000C16B
		public void UpdateUrlAndRefresh(string newUrl)
		{
			this.mUrl = newUrl;
			if (this.CefBrowser != null)
			{
				this.CefBrowser.StartUrl = this.mUrl;
				this.SetVisibilityOfLoader(Visibility.Visible);
				this.CefBrowser.NavigateTo(this.mUrl);
			}
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x0000DFA5 File Offset: 0x0000C1A5
		internal void NavigateTo(string url)
		{
			if (this.CefBrowser != null)
			{
				this.SetVisibilityOfLoader(Visibility.Visible);
				this.CefBrowser.NavigateTo(url);
			}
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0000DFC2 File Offset: 0x0000C1C2
		public void RefreshBrowser()
		{
			if (this.CefBrowser != null)
			{
				this.CefBrowser.Refresh();
			}
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x000799A4 File Offset: 0x00077BA4
		public BrowserControl(string url)
		{
			this.InitBaseControl(url, 0f);
			this.mSubscriber = new BrowserSubscriber(this);
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x00079A0C File Offset: 0x00077C0C
		public void CallJsForMaps(string methodName, string appName, string packageName)
		{
			object[] array = new object[] { "" };
			if (!string.IsNullOrEmpty(appName) || !string.IsNullOrEmpty(packageName))
			{
				JObject jobject = new JObject
				{
					{ "name", appName },
					{ "pkg", packageName }
				};
				array[0] = jobject.ToString(Formatting.None, new JsonConverter[0]);
			}
			if (this.CefBrowser != null)
			{
				this.CefBrowser.CallJs(methodName, array);
			}
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00079A88 File Offset: 0x00077C88
		internal void InitBaseControl(string url, float zoomLevel = 0f)
		{
			this.customZoomLevel = zoomLevel;
			this.mUrl = url;
			base.Visibility = Visibility.Hidden;
			base.IsVisibleChanged += this.BrowserControl_IsVisibleChanged;
			this.mGrid = new Grid();
			base.Content = this.mGrid;
			if (FeatureManager.Instance.IsCreateBrowserOnStart)
			{
				this.CreateNewBrowser();
			}
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00079AE8 File Offset: 0x00077CE8
		public void DisposeBrowser()
		{
			BrowserControl.sAllBrowserControls.Remove(this);
			if (this.CefBrowser != null)
			{
				this.mGrid.Children.Remove(this.CefBrowser);
				this.CefBrowser.Dispose();
				this.mBrowserHost = null;
				this.CefBrowser = null;
			}
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0000DFD7 File Offset: 0x0000C1D7
		private void BrowserControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (base.IsVisible)
			{
				Logger.Info("Install Boot: BrowserControl_IsVisibleChanged");
				this.CreateNewBrowser();
			}
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0000DFF1 File Offset: 0x0000C1F1
		internal void WelcomeTab_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs _)
		{
			if (FeatureManager.Instance.IsBrowserKilledOnTabSwitch && ((UIElement)sender).Visibility != Visibility.Visible)
			{
				this.DisposeBrowser();
			}
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x00079B38 File Offset: 0x00077D38
		internal void CreateNewBrowser()
		{
			if (this.CefBrowser == null && !string.IsNullOrEmpty(this.mUrl))
			{
				Logger.Info("Install Boot: CreateNewBrowser");
				this.CefBrowser = new Browser(this.customZoomLevel);
				BrowserControl.sAllBrowserControls.Add(this);
				this.CefBrowser.StartUrl = this.mUrl;
				this.mGrid.Children.Add(this.CefBrowser);
				this.CefBrowser.LoadEnd += this.MBrowser_LoadEnd;
				this.CefBrowser.ProcessMessageRecieved += this.Browser_ProcessMessageRecieved;
				this.CefBrowser.Loaded += this.Browser_Loaded;
				this.CefBrowser.LoadError += this.Browser_LoadError;
				this.CefBrowser.LoadingStateChange += this.Browser_LoadingStateChange;
				this.CefBrowser.OnBeforePopup += this.CefBrowser_OnBeforePopup;
				this.CefBrowser.mWPFCefBrowserExceptionHandler += this.Browser_WPFCefBrowserExceptionHandler;
				if (RegistryManager.Instance.CefDevEnv == 1)
				{
					this.CefBrowser.mAllowDevTool = true;
					this.CefBrowser.mDevToolHeader = this.mUrl;
				}
				Logger.Info("Install Boot: CreateNewBrowser complete");
				try
				{
					this.AddTouchHandler();
				}
				catch (Exception ex)
				{
					Logger.Info("Install Boot: CreateNewBrowser error");
					Logger.Error("exception adding touch handler: {0}", new object[] { ex });
				}
			}
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x00079CBC File Offset: 0x00077EBC
		private bool CefBrowser_OnBeforePopup(string url)
		{
			bool flag;
			try
			{
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					BlueStacksUIUtils.OpenUrl(url);
				}
				else
				{
					OpenExternalBrowserLinks openExternalBrowserLinks = OpenExternalBrowserLinks.externalbrowser;
					if (Enum.IsDefined(typeof(OpenExternalBrowserLinks), RegistryManager.Instance.OpenExternalLink))
					{
						openExternalBrowserLinks = (OpenExternalBrowserLinks)Enum.Parse(typeof(OpenExternalBrowserLinks), RegistryManager.Instance.OpenExternalLink);
					}
					switch (openExternalBrowserLinks)
					{
					case OpenExternalBrowserLinks.externalbrowser:
						BlueStacksUIUtils.OpenUrl(url);
						break;
					case OpenExternalBrowserLinks.sametab:
						this.UpdateUrlAndRefresh(url);
						break;
					case OpenExternalBrowserLinks.newtab:
					{
						MainWindow parentWindow = this.ParentWindow;
						if (parentWindow != null)
						{
							parentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(url, "Browser", "cef_tab", "");
						}
						break;
					}
					}
				}
				flag = true;
			}
			catch (Exception ex)
			{
				Logger.Warning("Error in opening external links from the cef browser: " + ex.ToString());
				flag = false;
			}
			return flag;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x00079D98 File Offset: 0x00077F98
		private void SendMessageToBrowserRenderProcess(CefProcessMessage message)
		{
			try
			{
				this.CefBrowser.GetHost().GetBrowser().SendProcessMessage(CefProcessId.Renderer, message);
			}
			catch (Exception ex)
			{
				string text = "exception in sending IPC message to cef render process..";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x00079DF0 File Offset: 0x00077FF0
		private void MBrowser_LoadEnd(object sender, LoadEndEventArgs e)
		{
			try
			{
				this.SetVisibilityOfLoader(Visibility.Hidden);
				if (this.ParentWindow != null)
				{
					using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("SetVmName"))
					{
						cefProcessMessage.Arguments.SetString(0, this.ParentWindow.mVmName);
						this.SendMessageToBrowserRenderProcess(cefProcessMessage);
					}
				}
			}
			catch (Exception ex)
			{
				string text = "Error in browser_loadend ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x00079E80 File Offset: 0x00078080
		private void Browser_LoadError(object sender, LoadErrorEventArgs e)
		{
			if (this.CefBrowser != null)
			{
				Logger.Warning("Cef error code: {0}, error text: {1}", new object[] { e.ErrorCode, e.ErrorText });
				if (e.ErrorCode == CefErrorCode.InternetDisconnected || e.ErrorCode == CefErrorCode.TunnelConnectionFailed || e.ErrorCode == CefErrorCode.ConnectionReset || e.ErrorCode == (CefErrorCode)(-21) || e.ErrorCode == (CefErrorCode)(-130) || (e.ErrorCode == CefErrorCode.NameNotResolved && !Utils.CheckForInternetConnection()))
				{
					this.mFailedUrl = e.FailedUrl;
					base.Dispatcher.Invoke(new Action(delegate
					{
						if (this.BrowserFallbackUrlEvent != null)
						{
							this.BrowserFallbackUrlEvent();
							return;
						}
						if (!this.mGrid.Children.Contains(this.NoInternetControl))
						{
							this.mGrid.Children.Add(this.NoInternetControl);
						}
					}), new object[0]);
				}
			}
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x00079F30 File Offset: 0x00078130
		private void SetVisibilityOfLoader(Visibility visibility)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				try
				{
					Grid grid = this.Parent as Grid;
					if (grid != null)
					{
						IEnumerable<CustomPictureBox> enumerable = grid.Children.OfType<CustomPictureBox>();
						if (enumerable != null && enumerable.Any<CustomPictureBox>())
						{
							enumerable.First<CustomPictureBox>().Visibility = visibility;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in set visibility of web page loader : " + ex.ToString());
				}
			}), new object[0]);
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x0000E012 File Offset: 0x0000C212
		private void Browser_WPFCefBrowserExceptionHandler(object sender, Exception e)
		{
			Logger.Error("Handle Error in wpf cef browser.." + e.ToString());
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x00079F70 File Offset: 0x00078170
		private void Browser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
		{
			try
			{
				if (this.customZoomLevel == 0f)
				{
					this.CefBrowser.SetZoomLevel(this.zoomLevel);
				}
				if (e.IsLoading)
				{
					base.Dispatcher.Invoke(new Action(delegate
					{
						if (this.mGrid.Children.Contains(this.NoInternetControl))
						{
							this.mGrid.Children.Remove(this.NoInternetControl);
						}
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while setting zoom in browser with url {0} and error :{1}", new object[]
				{
					this.mUrl,
					ex.ToString()
				});
			}
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x00079FF8 File Offset: 0x000781F8
		private void Browser_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.customZoomLevel == 0f)
				{
					Matrix transformToDevice = PresentationSource.FromVisual((Visual)sender).CompositionTarget.TransformToDevice;
					ScaleTransform scaleTransform = new ScaleTransform(1.0 / transformToDevice.M11, 1.0 / transformToDevice.M22);
					if (scaleTransform.CanFreeze)
					{
						scaleTransform.Freeze();
					}
					this.CefBrowser.LayoutTransform = scaleTransform;
					this.zoomLevel = Math.Log(transformToDevice.M11) / Math.Log(1.2);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while getting zoom of browser with url {0} and error :{1}", new object[]
				{
					this.mUrl,
					ex.ToString()
				});
			}
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x0007A0C8 File Offset: 0x000782C8
		private void AddTouchHandler()
		{
			try
			{
				if (Handler.DigitizerCapabilities.IsMultiTouchReady)
				{
					Logger.Info("adding touch handler");
					ManipulationProcessor mManipulationProcessor = new ManipulationProcessor(ProcessorManipulations.TRANSLATE_Y);
					this.mBrowserHost = this.CefBrowser.GetHost();
					Factory.EnableStylusEvents(this.ParentWindow);
					base.StylusDown += delegate(object s, StylusDownEventArgs e)
					{
						mManipulationProcessor.ProcessDown((uint)e.StylusDevice.Id, e.GetPosition(this).ToDrawingPointF());
					};
					base.StylusUp += delegate(object s, StylusEventArgs e)
					{
						mManipulationProcessor.ProcessUp((uint)e.StylusDevice.Id, e.GetPosition(this).ToDrawingPointF());
					};
					base.StylusMove += delegate(object s, StylusEventArgs e)
					{
						mManipulationProcessor.ProcessMove((uint)e.StylusDevice.Id, e.GetPosition(this).ToDrawingPointF());
					};
					mManipulationProcessor.ManipulationDelta += this.ProcessManipulationDelta;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("exception in adding touch handler: {0}", new object[] { ex });
			}
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x0007A194 File Offset: 0x00078394
		private void ProcessManipulationDelta(object sender, Windows7.Multitouch.Manipulation.ManipulationDeltaEventArgs e)
		{
			Logger.Debug("ProcessManipulationDelta.." + e.TranslationDelta.Height.ToString() + "..." + e.Location.ToString());
			if (this.mBrowserHost == null)
			{
				this.mBrowserHost = this.CefBrowser.GetHost();
			}
			CefMouseEvent cefMouseEvent = new CefMouseEvent
			{
				X = (int)e.Location.X,
				Y = (int)e.Location.Y
			};
			this.mBrowserHost.SendMouseWheelEvent(cefMouseEvent, 0, (int)e.TranslationDelta.Height);
			this.mBrowserHost.SendMouseMoveEvent(new CefMouseEvent(0, 0, CefEventFlags.None), false);
		}

		// Token: 0x06001441 RID: 5185 RVA: 0x0007A260 File Offset: 0x00078460
		private void Browser_ProcessMessageRecieved(object sender, ProcessMessageEventArgs e)
		{
			Logger.Info("Browser to client web call :" + e.Message.Name);
			if (string.Equals(e.Message.Name, "InstallApp", StringComparison.InvariantCulture))
			{
				CefListValue arguments = e.Message.Arguments;
				string @string = arguments.GetString(0);
				string string2 = arguments.GetString(1);
				string string3 = arguments.GetString(2);
				string string4 = arguments.GetString(3);
				this.InstallApp(@string, string2, string3, string4, "");
				this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(string4, this.SOURCE_APPCENTER);
				return;
			}
			if (string.Equals(e.Message.Name, "InstallAppVersion", StringComparison.InvariantCulture))
			{
				CefListValue arguments2 = e.Message.Arguments;
				string string5 = arguments2.GetString(0);
				string string6 = arguments2.GetString(1);
				string string7 = arguments2.GetString(2);
				string string8 = arguments2.GetString(3);
				string string9 = arguments2.GetString(4);
				this.InstallApp(string5, string6, string7, string8, string9);
				this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(string8, this.SOURCE_APPCENTER);
				return;
			}
			if (string.Equals(e.Message.Name, "InstallAppGooglePlay", StringComparison.InvariantCulture))
			{
				CefListValue arguments3 = e.Message.Arguments;
				arguments3.GetString(0);
				arguments3.GetString(1);
				arguments3.GetString(2);
				string string10 = arguments3.GetString(3);
				this.ShowAppInPlayStore(string10);
				this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(string10, this.SOURCE_APPCENTER);
				return;
			}
			if (string.Equals(e.Message.Name, "InstallAppGooglePlayPopup", StringComparison.InvariantCulture))
			{
				CefListValue arguments4 = e.Message.Arguments;
				arguments4.GetString(0);
				string string11 = arguments4.GetString(1);
				arguments4.GetString(2);
				string string12 = arguments4.GetString(3);
				this.ShowAppInPlayStorePopup(string12, string11);
				this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(string12, this.SOURCE_APPCENTER);
				return;
			}
			if (string.Equals(e.Message.Name, "DownloadInstallOem", StringComparison.InvariantCulture))
			{
				CefListValue arguments5 = e.Message.Arguments;
				string oem2 = arguments5.GetString(0);
				string abiValue2 = arguments5.GetString(1);
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (!string.IsNullOrEmpty(oem2))
					{
						AppPlayerModel appPlayerModel = InstalledOem.GetAppPlayerModel(oem2, abiValue2);
						if (appPlayerModel != null)
						{
							new DownloadInstallOem(this.ParentWindow).DownloadOem(appPlayerModel);
						}
					}
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "CancelOemDownload", StringComparison.InvariantCulture))
			{
				CefListValue arguments6 = e.Message.Arguments;
				string oem3 = arguments6.GetString(0);
				string abiValue3 = arguments6.GetString(1);
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (!string.IsNullOrEmpty(oem3))
					{
						AppPlayerModel appPlayerModel2 = InstalledOem.GetAppPlayerModel(oem3, abiValue3);
						if (appPlayerModel2 == null)
						{
							return;
						}
						appPlayerModel2.CancelOemDownload();
					}
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "LaunchAppInDifferentOem", StringComparison.InvariantCulture))
			{
				CefListValue arguments7 = e.Message.Arguments;
				string oem = arguments7.GetString(0);
				string abiValue = arguments7.GetString(1);
				string vmname = arguments7.GetString(2);
				string packageName2 = arguments7.GetString(3);
				string actionWithRemainingInstances = arguments7.GetString(4);
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (!string.IsNullOrEmpty(oem))
					{
						InstalledOem.LaunchOemInstance(oem, abiValue, vmname, packageName2, actionWithRemainingInstances);
					}
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "UninstallApp", StringComparison.InvariantCulture))
			{
				CefListValue arguments8 = e.Message.Arguments;
				string packageName = arguments8.GetString(0);
				base.Dispatcher.Invoke(new Action(delegate
				{
					DownloadInstallApk mAppInstaller = this.ParentWindow.mAppInstaller;
					if (mAppInstaller == null)
					{
						return;
					}
					mAppInstaller.UninstallApp(packageName);
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "GetUpdatedGrm", StringComparison.InvariantCulture))
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					GrmHandler.SendUpdateGrmPackagesToBrowser(this.ParentWindow.mVmName);
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "RetryApkInstall", StringComparison.InvariantCulture))
			{
				CefListValue arguments9 = e.Message.Arguments;
				string apkFilePath = arguments9.GetString(0);
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (File.Exists(apkFilePath))
					{
						new DownloadInstallApk(this.ParentWindow).InstallApk(apkFilePath, false);
					}
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "ChooseAndInstallApk", StringComparison.InvariantCulture))
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					new DownloadInstallApk(this.ParentWindow).ChooseAndInstallApk();
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "GoogleSearch", StringComparison.InvariantCulture))
			{
				string string13 = e.Message.Arguments.GetString(0);
				this.SearchAppInPlayStore(string13);
				ClientStats.SendGPlayClickStats(new Dictionary<string, string>
				{
					{ "query", string13 },
					{ "source", "bs3_appsearch" }
				});
				return;
			}
			if (string.Equals(e.Message.Name, "CloseSearch", StringComparison.InvariantCulture))
			{
				this.CloseSearch();
				return;
			}
			if (string.Equals(e.Message.Name, "RefreshSearch", StringComparison.InvariantCulture))
			{
				CefListValue arguments10 = e.Message.Arguments;
				string text = string.Empty;
				if (arguments10.Count > 0)
				{
					text = arguments10.GetString(0);
				}
				this.RefreshSearch(text);
				return;
			}
			if (string.Equals(e.Message.Name, "OfflineHtmlHomeUrl", StringComparison.InvariantCulture))
			{
				string string14 = e.Message.Arguments.GetString(0);
				RegistryManager.Instance.OfflineHtmlHomeUrl = string14;
				return;
			}
			if (string.Equals(e.Message.Name, "RefreshHomeHtml", StringComparison.InvariantCulture))
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					this.ParentWindow.Utils.RefreshHtmlHomeUrl();
				}), new object[0]);
				return;
			}
			if (string.Equals(e.Message.Name, "SetWebAppVersion", StringComparison.InvariantCulture))
			{
				string string15 = e.Message.Arguments.GetString(0);
				RegistryManager.Instance.WebAppVersion = string15;
				return;
			}
			if (string.Equals(e.Message.Name, "ShowWebPage", StringComparison.InvariantCulture))
			{
				CefListValue arguments11 = e.Message.Arguments;
				string string16 = arguments11.GetString(0);
				string string17 = arguments11.GetString(1);
				this.ShowWebPage(string16, string17);
				return;
			}
			if (!string.Equals(e.Message.Name, "CloseBlockerAd", StringComparison.InvariantCulture))
			{
				if (string.Equals(e.Message.Name, "CheckIfPremium", StringComparison.InvariantCulture))
				{
					string string18 = e.Message.Arguments.GetString(0);
					this.CheckIfPremium(string18);
					return;
				}
				if (!string.Equals(e.Message.Name, "GetImpressionId", StringComparison.InvariantCulture))
				{
					if (string.Equals(e.Message.Name, "sendFirebaseNotification", StringComparison.InvariantCulture))
					{
						string json = e.Message.Arguments.GetString(0);
						base.Dispatcher.Invoke(new Action(delegate
						{
							CloudNotificationManager.Instance.HandleCloudNotification(json, this.ParentWindow.mVmName);
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "PikaWorldProfileAdded", StringComparison.InvariantCulture))
					{
						string string19 = e.Message.Arguments.GetString(0);
						RegistryManager.Instance.PikaWorldId = string19;
						return;
					}
					if (string.Equals(e.Message.Name, "subscribeModule", StringComparison.InvariantCulture))
					{
						string string20 = e.Message.Arguments.GetString(0);
						char[] array = new char[] { ',' };
						string[] array2 = string20.Split(array, StringSplitOptions.None);
						this.PopulateTagsInfo(array2, array2[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "unsubscribeModule", StringComparison.InvariantCulture))
					{
						string string21 = e.Message.Arguments.GetString(0);
						char[] array3 = new char[] { ',' };
						string[] array4 = string21.Split(array3, StringSplitOptions.None);
						this.RemoveTagsInfo(array4);
						return;
					}
					if (string.Equals(e.Message.Name, "subscribeVmSpecificClientTags", StringComparison.InvariantCulture))
					{
						Dictionary<string, List<string>> dictionary = JToken.Parse(e.Message.Arguments.GetString(0)).ToObject<Dictionary<string, List<string>>>();
						if (this.mSubscriber == null)
						{
							this.mSubscriber = new BrowserSubscriber(this);
						}
						using (Dictionary<string, List<string>>.Enumerator enumerator = dictionary.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
								foreach (string text2 in keyValuePair.Value)
								{
									if (Enum.IsDefined(typeof(BrowserControlTags), text2))
									{
										BrowserControlTags browserControlTags = (BrowserControlTags)Enum.Parse(typeof(BrowserControlTags), text2);
										if (!this.TagsSubscribedDict.ContainsKey(browserControlTags))
										{
											JObject jobject = new JObject();
											jobject["ClientTag"] = text2;
											jobject["CallbackFunction"] = keyValuePair.Key;
											jobject["IsReceiveFromAllVm"] = false;
											JObject jobject2 = jobject;
											this.TagsSubscribedDict.Add(browserControlTags, jobject2);
										}
										BrowserSubscriber mSubscriber = this.mSubscriber;
										if (mSubscriber != null)
										{
											mSubscriber.SubscribeTag(browserControlTags);
										}
										if (browserControlTags == BrowserControlTags.getVmInfo)
										{
											Publisher.PublishMessage(BrowserControlTags.getVmInfo, this.ParentWindow.mVmName, new JObject(new JProperty("VmId", Utils.GetVmIdFromVmName(this.ParentWindow.mVmName))));
										}
										if (browserControlTags == BrowserControlTags.bootComplete && this.ParentWindow.mGuestBootCompleted)
										{
											Logger.Info("Sending boot complete to browser immediately");
											Publisher.PublishMessage(BrowserControlTags.bootComplete, this.ParentWindow.mVmName, null);
										}
									}
								}
							}
							return;
						}
					}
					if (string.Equals(e.Message.Name, "subscribeClientTags", StringComparison.InvariantCulture))
					{
						JArray jarray = JArray.Parse(e.Message.Arguments.GetString(0));
						if (this.mSubscriber == null)
						{
							this.mSubscriber = new BrowserSubscriber(this);
						}
						for (int i = 0; i < jarray.Count; i++)
						{
							JObject jobject3 = JObject.Parse(jarray[i].ToString());
							if (Enum.IsDefined(typeof(BrowserControlTags), jobject3["ClientTag"].ToString()))
							{
								BrowserControlTags browserControlTags2 = (BrowserControlTags)Enum.Parse(typeof(BrowserControlTags), jobject3["ClientTag"].ToString());
								if (!this.TagsSubscribedDict.ContainsKey(browserControlTags2))
								{
									this.TagsSubscribedDict.Add(browserControlTags2, jobject3);
								}
								BrowserSubscriber mSubscriber2 = this.mSubscriber;
								if (mSubscriber2 != null)
								{
									mSubscriber2.SubscribeTag(browserControlTags2);
								}
								if (browserControlTags2 == BrowserControlTags.getVmInfo)
								{
									Publisher.PublishMessage(BrowserControlTags.getVmInfo, this.ParentWindow.mVmName, new JObject(new JProperty("VmId", Utils.GetVmIdFromVmName(this.ParentWindow.mVmName))));
								}
								if (browserControlTags2 == BrowserControlTags.bootComplete && this.ParentWindow.mGuestBootCompleted)
								{
									Logger.Info("Sending boot complete to browser immediately");
									Publisher.PublishMessage(BrowserControlTags.bootComplete, this.ParentWindow.mVmName, null);
								}
							}
						}
						return;
					}
					if (string.Equals(e.Message.Name, "unsubscribeClientTags", StringComparison.InvariantCulture))
					{
						using (List<string>.Enumerator enumerator2 = JArray.Parse(e.Message.Arguments.GetString(0)).ToObject<List<string>>().GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string text3 = enumerator2.Current;
								if (Enum.IsDefined(typeof(BrowserControlTags), text3))
								{
									BrowserControlTags browserControlTags3 = (BrowserControlTags)Enum.Parse(typeof(BrowserControlTags), text3);
									if (this.TagsSubscribedDict.ContainsKey(browserControlTags3))
									{
										this.TagsSubscribedDict.Remove(browserControlTags3);
										BrowserSubscriber mSubscriber3 = this.mSubscriber;
										if (mSubscriber3 != null)
										{
											mSubscriber3.UnsubscribeTag(browserControlTags3);
										}
									}
								}
							}
							return;
						}
					}
					if (string.Equals(e.Message.Name, "ApplyThemeName", StringComparison.InvariantCulture))
					{
						string themeName = e.Message.Arguments.GetString(0);
						base.Dispatcher.Invoke(new Action(delegate
						{
							this.ParentWindow.Utils.ApplyTheme(themeName);
							this.ParentWindow.Utils.RestoreWallpaperImageForAllVms();
							BlueStacksUIColorManager.ApplyTheme(themeName);
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "GoToMapsTab", StringComparison.InvariantCulture))
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							this.GoToMapsTab();
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "HandleClick", StringComparison.InvariantCulture))
					{
						string text4 = "";
						try
						{
							text4 = e.Message.Arguments.GetString(0);
							JToken res2 = JToken.Parse(text4);
							base.Dispatcher.Invoke(new Action(delegate
							{
								this.ParentWindow.HideDimOverlay();
								this.ParentWindow.Utils.HandleGenericActionFromDictionary(res2.ToSerializableDictionary<string>(), "handle_browser_click", "");
							}), new object[0]);
							return;
						}
						catch (Exception ex)
						{
							Logger.Error(string.Concat(new string[]
							{
								"Error when processing click action received from gmapi. JsonString: ",
								text4,
								Environment.NewLine,
								"Error: ",
								ex.ToString()
							}));
							return;
						}
					}
					if (string.Equals(e.Message.Name, "UpdateQuestRules", StringComparison.InvariantCulture))
					{
						string text5 = "";
						try
						{
							text5 = e.Message.Arguments.GetString(0);
							PromotionManager.ReadQuests(JToken.Parse(text5), true);
							return;
						}
						catch (Exception ex2)
						{
							Logger.Error(string.Concat(new string[]
							{
								"Error when processing UpdateQuestRules. JsonString: ",
								text5,
								Environment.NewLine,
								"Error: ",
								ex2.ToString()
							}));
							return;
						}
					}
					if (string.Equals(e.Message.Name, "GetGamepadConnectionStatus", StringComparison.InvariantCulture))
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							if (this.ParentWindow != null)
							{
								BlueStacksUIUtils.SendGamepadStatusToBrowsers(this.ParentWindow.IsGamepadConnected);
							}
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "CloseAnyPopup", StringComparison.InvariantCulture))
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							if (this.ParentWindow != null)
							{
								this.ParentWindow.HideDimOverlay();
							}
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "SearchAppcenter", StringComparison.OrdinalIgnoreCase))
					{
						string searchText = e.Message.Arguments.GetString(0);
						base.Dispatcher.Invoke(new Action(delegate
						{
							this.ParentWindow.mCommonHandler.SearchAppCenter(searchText);
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "DownloadMacro", StringComparison.OrdinalIgnoreCase))
					{
						string macroData = e.Message.Arguments.GetString(0);
						base.Dispatcher.Invoke(new Action(delegate
						{
							this.ParentWindow.Utils.DownloadAndUpdateMacro(macroData);
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "ChangeControlScheme", StringComparison.OrdinalIgnoreCase))
					{
						string schemeSelected = e.Message.Arguments.GetString(0);
						base.Dispatcher.Invoke(new Action(delegate
						{
							GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
							if (sGuidanceWindow == null)
							{
								return;
							}
							sGuidanceWindow.SelectControlScheme(schemeSelected);
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "CloseOnBoarding", StringComparison.OrdinalIgnoreCase))
					{
						string string22 = e.Message.Arguments.GetString(0);
						Logger.Info("CloseOnBoarding response from browser : " + string22);
						JObject res = JObject.Parse(string22);
						base.Dispatcher.Invoke(new Action(delegate
						{
							GameOnboardingControl onboardingControl = this.ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl;
							if (onboardingControl != null)
							{
								onboardingControl.Close();
							}
							this.ParentWindow.StaticComponents.mSelectedTabButton.ShowBlurbOnboarding(res);
							this.ParentWindow.HideDimOverlay();
							GuidanceWindow sGuidanceWindow2 = KMManager.sGuidanceWindow;
							if (sGuidanceWindow2 == null)
							{
								return;
							}
							sGuidanceWindow2.DimOverLayVisibility(Visibility.Collapsed);
						}), new object[0]);
						return;
					}
					if (string.Equals(e.Message.Name, "BrowserLoaded", StringComparison.OrdinalIgnoreCase))
					{
						base.Dispatcher.Invoke(new Action(delegate
						{
							Action browserLoadCompleteEvent = this.BrowserLoadCompleteEvent;
							if (browserLoadCompleteEvent == null)
							{
								return;
							}
							browserLoadCompleteEvent();
						}), new object[0]);
						return;
					}
					try
					{
						object[] array5 = null;
						if (e.Message.Arguments.Count > 0)
						{
							array5 = new object[e.Message.Arguments.Count];
							for (int j = 0; j < e.Message.Arguments.Count; j++)
							{
								if (e.Message.Arguments.GetString(j) != null)
								{
									array5[j] = e.Message.Arguments.GetString(j).ToString(CultureInfo.InvariantCulture);
									Logger.Info("web api call.." + e.Message.Name + "..with args.." + e.Message.Arguments.GetString(j).ToString(CultureInfo.InvariantCulture));
								}
								else
								{
									array5[j] = string.Empty;
								}
							}
						}
						base.GetType().GetMethod(e.Message.Name).Invoke(this, array5);
					}
					catch (Exception ex3)
					{
						Logger.Error("Error in executing gmapi " + e.Message.Name + ": " + ex3.ToString());
					}
					ProcessMessageEventHandler processMessageRecieved = this.ProcessMessageRecieved;
					if (processMessageRecieved == null)
					{
						return;
					}
					processMessageRecieved(sender, e);
				}
			}
		}

		// Token: 0x06001442 RID: 5186 RVA: 0x0007B3B8 File Offset: 0x000795B8
		internal void RemoveTagsInfo(string[] tagsList)
		{
			foreach (string text in tagsList)
			{
				if (BrowserControl.mFirebaseTagsSubscribed.Contains(text))
				{
					BrowserControl.mFirebaseTagsSubscribed.Remove(text);
				}
			}
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x0007B3F4 File Offset: 0x000795F4
		public void PopulateTagsInfo(string[] tagsList, string methodName)
		{
			if (tagsList != null)
			{
				foreach (string text in tagsList)
				{
					if (!string.Equals(text, methodName, StringComparison.InvariantCultureIgnoreCase))
					{
						BrowserControl.mFirebaseTagsSubscribed.Add(text);
					}
				}
			}
			this.mFirebaseCallbackMethod = methodName;
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x0000E029 File Offset: 0x0000C229
		public void GoToMapsTab()
		{
			if (this.ParentWindow != null)
			{
				this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("pikaworld", true, false);
			}
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x0007B434 File Offset: 0x00079634
		private void CheckIfPremium(string isPremium)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (isPremium.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				{
					RegistryManager.Instance.IsPremium = true;
					this.ParentWindow.mTopBar.ChangeUserPremiumButton(true);
				}
				else
				{
					RegistryManager.Instance.IsPremium = false;
					this.ParentWindow.mTopBar.ChangeUserPremiumButton(false);
				}
				Action<bool> appRecommendationHandler = PromotionObject.AppRecommendationHandler;
				if (appRecommendationHandler == null)
				{
					return;
				}
				appRecommendationHandler(true);
			}), new object[0]);
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x0007B474 File Offset: 0x00079674
		private void InstallApp(string iconUrl, string appName, string apkUrl, string package, string timestamp = "")
		{
			if (!string.IsNullOrEmpty(package))
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					new DownloadInstallApk(this.ParentWindow).DownloadAndInstallApp(iconUrl, appName, apkUrl, package, false, true, timestamp);
				}), new object[0]);
			}
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x0007B4E0 File Offset: 0x000796E0
		private void ShowAppInPlayStore(string packageName)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(packageName, PlayStoreAction.OpenApp);
			}), new object[0]);
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x0007B520 File Offset: 0x00079720
		private void ShowAppInPlayStorePopup(string packageName, string appName)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(packageName, appName, PlayStoreAction.OpenApp, false);
				this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Visibility = Visibility.Visible;
			}), new object[0]);
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x0007B568 File Offset: 0x00079768
		private void SearchAppInPlayStore(string searchString)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (searchString != null)
				{
					this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(searchString, PlayStoreAction.SearchApp);
				}
			}), new object[0]);
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x0000E050 File Offset: 0x0000C250
		private void CloseSearch()
		{
			if (this.CefBrowser != null)
			{
				this.CefBrowser.NavigateTo(this.mUrl);
			}
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x00004786 File Offset: 0x00002986
		private void RefreshSearch(string _)
		{
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x0007B5A8 File Offset: 0x000797A8
		public void ShowWebPage(string title, string webUrl)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (title == null)
				{
					title = "";
				}
				if (this.ParentWindow != null)
				{
					this.ParentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(webUrl, title, "cef_tab", "");
					return;
				}
				MainWindow activatedWindow = null;
				if (BlueStacksUIUtils.DictWindows.Count > 0)
				{
					activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
				}
				activatedWindow.Dispatcher.Invoke(new Action(delegate
				{
					activatedWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(webUrl, title, "cef_tab", "");
				}), new object[0]);
			}), new object[0]);
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x0000E06B File Offset: 0x0000C26B
		public void CloseSelf()
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				AppTabButton selectedTab = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
				if (selectedTab != null)
				{
					this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(selectedTab.TabKey, false, false, false, false, "");
				}
			}), new object[0]);
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x0000E090 File Offset: 0x0000C290
		public void CloseBrowserQuitPopup()
		{
			this.ParentWindow.CloseBrowserQuitPopup();
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x0000E09D File Offset: 0x0000C29D
		internal void ReInitBrowser(string url)
		{
			this.CefBrowser.Dispose();
			this.CefBrowser = null;
			this.mUrl = url;
			this.CreateNewBrowser();
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0007B5F0 File Offset: 0x000797F0
		public static void DownloadBTV()
		{
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
				window.Dispatcher.Invoke(new Action(delegate
				{
					BTVManager.Instance.MaybeDownloadAndLaunchBTv(window);
				}), new object[0]);
			}
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x0007B658 File Offset: 0x00079858
		public static void DownloadDirectX()
		{
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				MainWindow activatedWindow = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
				activatedWindow.Dispatcher.Invoke(new Action(delegate
				{
					string directXDownloadURL = "http://www.microsoft.com/en-us/download/details.aspx?id=35";
					CustomMessageWindow window = new CustomMessageWindow();
					window.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ADDITIONAL_FILES_REQUIRED", "");
					window.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SOME_WINDOW_FILES_MISSING", "");
					window.AddHyperLinkInUI(directXDownloadURL, new Uri(directXDownloadURL), delegate(object o, RequestNavigateEventArgs arg)
					{
						BlueStacksUIUtils.OpenUrl(arg.Uri.ToString());
						window.CloseWindow();
					});
					window.AddButton(ButtonColors.Blue, "STRING_DOWNLOAD_NOW", delegate(object o, EventArgs args)
					{
						BlueStacksUIUtils.OpenUrl(directXDownloadURL);
					}, null, false, null);
					window.AddButton(ButtonColors.White, "STRING_NO", null, null, false, null);
					window.Owner = activatedWindow;
					window.ShowDialog();
					activatedWindow.BringIntoView();
				}), new object[0]);
			}
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0000E0BE File Offset: 0x0000C2BE
		public static void SetSystemVolume(string level)
		{
			StreamManager.Instance.SetSystemVolume(level);
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0000E0CB File Offset: 0x0000C2CB
		public static void SetMicVolume(string level)
		{
			if (string.Equals((level != null) ? level.Trim() : null, "0", StringComparison.InvariantCultureIgnoreCase))
			{
				StreamManager.mIsMicDisabled = true;
			}
			StreamManager.Instance.SetMicVolume(level);
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0000E0F7 File Offset: 0x0000C2F7
		public static void EnableWebcam(string width, string height, string position)
		{
			StreamManager.EnableWebcam(width, height, position);
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0000E101 File Offset: 0x0000C301
		public static void DisableWebcamV2(string jsonString)
		{
			StreamManager.Instance.DisableWebcamV2(jsonString);
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0000E10E File Offset: 0x0000C30E
		public static void MoveWebcam(string horizontal, string vertical)
		{
			StreamManager.Instance.MoveWebcam(horizontal, vertical);
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x0000E11C File Offset: 0x0000C31C
		public static void StopRecord()
		{
			if (StreamManager.Instance != null)
			{
				Logger.Info("Got StopRecord");
				StreamManager.Instance.StopRecord(true);
			}
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x0000E13A File Offset: 0x0000C33A
		public static void StopStream()
		{
			StreamManager.Instance.StopStream();
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x00004786 File Offset: 0x00002986
		public static void ShowPreview()
		{
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x00004786 File Offset: 0x00002986
		public static void HidePreview()
		{
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x0000E146 File Offset: 0x0000C346
		public void StartObs(string _)
		{
			this.InitStreamManager();
			StreamManager.Instance.StartObs();
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0007B6C0 File Offset: 0x000798C0
		public void GetRealtimeAppUsage(string callBackFunction)
		{
			try
			{
				Dictionary<string, Dictionary<string, long>> realtimeDictionary = AppUsageTimer.GetRealtimeDictionary();
				if (!string.IsNullOrEmpty(callBackFunction))
				{
					this.CallBackToHtml(callBackFunction, JSONUtils.GetJSONObjectString<long>(realtimeDictionary[this.ParentWindow.mVmName]));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while sending realtime dictionary to gmapi" + ex.ToString());
			}
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x0007B724 File Offset: 0x00079924
		public void GetInstalledAppsForAllOems(string callBackFunction)
		{
			string text = "Android";
			MainWindow parentWindow = this.ParentWindow;
			if (!string.IsNullOrEmpty((parentWindow != null) ? parentWindow.mVmName : null))
			{
				text = this.ParentWindow.mVmName;
			}
			JArray jarray = new JArray();
			foreach (string text2 in InstalledOem.InstalledCoexistingOemList)
			{
				JArray jarray2 = new JArray();
				foreach (string text3 in RegistryManager.RegistryManagers[text2].VmList)
				{
					if (!string.Equals(text2, "bgp64", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text3, text, StringComparison.InvariantCultureIgnoreCase))
					{
						string text4 = Path.Combine(RegistryManager.RegistryManagers[text2].DataDir, "UserData\\Gadget\\apps_" + text3 + ".json");
						string text5 = "[]";
						using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
						{
							if (mutex.WaitOne())
							{
								try
								{
									StreamReader streamReader = new StreamReader(text4);
									text5 = streamReader.ReadToEnd();
									streamReader.Close();
								}
								catch (Exception ex)
								{
									Logger.Error("Failed to read apps json file... Err : " + ex.ToString());
								}
								finally
								{
									mutex.ReleaseMutex();
								}
							}
						}
						string suffix = InstalledOem.GetAppPlayerModel(text2, Utils.GetValueInBootParams("abivalue", text3, string.Empty, text2)).Suffix;
						if (string.IsNullOrEmpty(RegistryManager.RegistryManagers[text2].Guest[text3].DisplayName))
						{
							string text6;
							if (string.Equals(text3, "Android", StringComparison.InvariantCultureIgnoreCase))
							{
								text6 = Strings.ProductDisplayName + " " + suffix;
							}
							else
							{
								text6 = string.Concat(new string[]
								{
									Strings.ProductDisplayName,
									" ",
									Utils.GetVmIdFromVmName(text3),
									" ",
									suffix
								});
							}
							RegistryManager.RegistryManagers[text2].Guest[text3].DisplayName = text6.Trim();
						}
						jarray2.Add(new JObject
						{
							{ "vmname", text3 },
							{
								"vmdisplayname",
								RegistryManager.RegistryManagers[text2].Guest[text3].DisplayName
							},
							{
								"abiValue",
								Utils.GetValueInBootParams("abivalue", text3, string.Empty, text2)
							},
							{
								"oemSuffix",
								string.IsNullOrEmpty(suffix) ? "N-32" : suffix
							},
							{
								"filedata",
								JArray.Parse(text5)
							}
						});
					}
				}
				jarray.Add(new JObject
				{
					{ "oem", text2 },
					{ "vmlist", jarray2 }
				});
			}
			if (!string.IsNullOrEmpty(callBackFunction))
			{
				string text7 = new JObject(new JProperty("oemlist", jarray.ToString(Formatting.None, new JsonConverter[0]))).ToString(Formatting.None, new JsonConverter[0]);
				text7 = text7.Replace("\n", "");
				text7 = text7.Replace("\r", "");
				text7 = Regex.Replace(text7, "\\s+", " ", RegexOptions.Multiline);
				this.CallBackToHtml(callBackFunction, text7);
			}
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x0007BAE0 File Offset: 0x00079CE0
		public void GetSystemInfo(string callbackFunction)
		{
			int num = 0;
			try
			{
				num = (int)(new ComputerInfo().TotalPhysicalMemory / 1048576UL);
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format("Exception when finding ram {0}", ex));
			}
			bool flag = false;
			string text = "";
			try
			{
				if (!string.IsNullOrEmpty(RegistryManager.RegistryManagers["bgp64"].AvailableGPUDetails))
				{
					flag = true;
					text = RegistryManager.RegistryManagers["bgp64"].AvailableGPUDetails;
				}
			}
			catch (Exception ex2)
			{
				string text2 = "Exception in getting gpu details ";
				Exception ex3 = ex2;
				Logger.Error(text2 + ((ex3 != null) ? ex3.ToString() : null));
			}
			try
			{
				GlMode glModeForVm = Utils.GetGlModeForVm(this.ParentWindow.mVmName);
				EngineState engineState = EngineState.plus;
				if (RegistryManager.Instance.CurrentEngine == "raw")
				{
					engineState = EngineState.raw;
				}
				int guestWidth = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth;
				int guestHeight = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight;
				string text3 = guestWidth.ToString(CultureInfo.InvariantCulture) + "x" + guestHeight.ToString(CultureInfo.InvariantCulture);
				JObject jobject = new JObject
				{
					{
						"physicalCpu",
						Environment.ProcessorCount
					},
					{ "physicalRam", num },
					{ "isGpuAvailable", flag },
					{ "gpuText", text },
					{
						"engineMode",
						engineState.ToString()
					},
					{
						"glMode",
						glModeForVm.ToString()
					},
					{
						"ram",
						RegistryManager.Instance.Guest[this.ParentWindow.mVmName].Memory
					},
					{
						"cpuAllocated",
						RegistryManager.Instance.Guest[this.ParentWindow.mVmName].VCPUs
					},
					{
						"dpi",
						Utils.GetDpiFromBootParameters(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].BootParameters)
					},
					{
						"fps",
						RegistryManager.Instance.Guest[this.ParentWindow.mVmName].FPS
					},
					{ "res", text3 },
					{
						"installedOems",
						string.Join(",", InstalledOem.AllInstalledOemList.ToArray())
					},
					{
						"pcode",
						Utils.GetValueInBootParams("pcode", this.ParentWindow.mVmName, "", "bgp64")
					},
					{
						"astcOption",
						RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ASTCOption.ToString()
					},
					{
						"abiValue",
						Utils.GetValueInBootParams("abivalue", this.ParentWindow.mVmName, "", "bgp64")
					}
				};
				if (!string.IsNullOrEmpty(callbackFunction))
				{
					this.CallBackToHtml(callbackFunction, jobject.ToString(Formatting.None, new JsonConverter[0]));
				}
			}
			catch (Exception ex4)
			{
				string text4 = "Exception in getting system info details ";
				Exception ex5 = ex4;
				Logger.Error(text4 + ((ex5 != null) ? ex5.ToString() : null));
			}
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0007BEB4 File Offset: 0x0007A0B4
		public void GetInstalledAppsJsonforJS(string callBackFunction)
		{
			bool flag = false;
			string text = "Android";
			MainWindow parentWindow = this.ParentWindow;
			if (!string.IsNullOrEmpty((parentWindow != null) ? parentWindow.mVmName : null))
			{
				text = this.ParentWindow.mVmName;
			}
			string text2 = Path.Combine(RegistryStrings.GadgetDir, "apps_" + text + ".json");
			string text3 = "[" + Environment.NewLine + "]";
			using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
			{
				if (mutex.WaitOne())
				{
					try
					{
						StreamReader streamReader = new StreamReader(text2);
						text3 = streamReader.ReadToEnd();
						streamReader.Close();
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to read apps json file... Err : " + ex.ToString());
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
			if (flag)
			{
				text3 = text3.Replace("\"", "\\\"");
			}
			text3 = text3.Replace("\n", "");
			text3 = text3.Replace("\r", "");
			text3 = Regex.Replace(text3, "\\s+", " ", RegexOptions.Multiline);
			if (!string.IsNullOrEmpty(callBackFunction))
			{
				this.CallBackToHtml(callBackFunction, text3);
			}
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0007BFF8 File Offset: 0x0007A1F8
		public void PerformOTS(string callbackFunction)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon("com.android.vending");
				if (appIcon != null)
				{
					this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
				}
			}), new object[0]);
			if (!string.IsNullOrEmpty(callbackFunction))
			{
				this.ParentWindow.mBrowserCallbackFunctionName = callbackFunction;
				this.ParentWindow.BrowserOTSCompletedCallback += this.ParentWindow_BrowserOTSCompletedCallback;
			}
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x0007C050 File Offset: 0x0007A250
		private void ParentWindow_BrowserOTSCompletedCallback(object sender, MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs args)
		{
			string text = RegistryManager.Instance.Token + "@@" + RegistryManager.Instance.RegisteredEmail;
			this.CallBackToHtml(args.CallbackFunction, text);
			string communityWebTabKey = LocaleStrings.GetLocalizedString("STRING_MACRO_COMMUNITY", "");
			Action <>9__1;
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(communityWebTabKey) && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey != communityWebTabKey)
				{
					Dispatcher dispatcher = this.ParentWindow.Dispatcher;
					Action action;
					if ((action = <>9__1) == null)
					{
						action = (<>9__1 = delegate
						{
							this.ParentWindow.mTopBar.mAppTabButtons.GoToTab(communityWebTabKey, true, false);
						});
					}
					dispatcher.Invoke(action, new object[0]);
				}
			}), new object[0]);
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x0007C0CC File Offset: 0x0007A2CC
		public string GetCurrentAppInfo(string callBackFunction)
		{
			MainWindow mainWindow = null;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
			}
			AppTabButton selectedTab = mainWindow.mTopBar.mAppTabButtons.SelectedTab;
			if (selectedTab == null)
			{
				return "{}";
			}
			string appName = selectedTab.AppName;
			string packageName = selectedTab.PackageName;
			JObject jobject = new JObject
			{
				{ "type", "app" },
				{ "name", appName },
				{ "data", packageName }
			};
			if (!string.IsNullOrEmpty(callBackFunction))
			{
				this.CallBackToHtml(callBackFunction, jobject.ToString(Formatting.None, new JsonConverter[0]));
			}
			return jobject.ToString(Formatting.None, new JsonConverter[0]);
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x0000E159 File Offset: 0x0000C359
		public void StartStreamV2(string jsonString, string callbackStreamStatus, string callbackTabChanged)
		{
			Logger.Info("Got StartStreamV2");
			this.InitStreamManager();
			if (StreamManager.Instance.mReplayBufferEnabled)
			{
				StreamManager.Instance.StartReplayBuffer();
			}
			Logger.Info("Got StartStream");
			StreamManager.Instance.StartStream(jsonString, callbackStreamStatus, callbackTabChanged);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x0007C18C File Offset: 0x0007A38C
		private StreamManager InitStreamManager()
		{
			if (StreamManager.Instance == null)
			{
				StreamManager.Instance = new StreamManager(this.CefBrowser);
			}
			else
			{
				string text;
				string text2;
				StreamManager.GetStreamConfig(out text, out text2);
				StreamManager.Instance.SetHwnd(text);
			}
			return StreamManager.Instance;
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x0007C1CC File Offset: 0x0007A3CC
		public void makeWebCall(string url, string scriptToInvoke)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
			httpWebRequest.Method = "GET";
			httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
			httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
			string text = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + text;
			new Uri(url);
			try
			{
				Logger.Info("making a webcall at url=" + url);
				string text2 = null;
				using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
						{
							text2 = streamReader.ReadToEnd();
						}
					}
				}
				object[] array = new object[] { text2 };
				this.CefBrowser.CallJs(scriptToInvoke, array);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in MakeWebCall. err : " + ex.ToString());
				string text3 = "";
				object[] array2 = new object[] { text3 };
				this.CefBrowser.CallJs(scriptToInvoke, array2);
			}
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x0007C328 File Offset: 0x0007A528
		public static void LaunchDialog(string jsonString)
		{
			try
			{
				JObject jobject = JObject.Parse(jsonString);
				if (jobject["parameter"] != null)
				{
					jobject["parameter"].ToString();
				}
				if (BlueStacksUIUtils.DictWindows.Count > 0)
				{
					BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in launchDialog gmApi : " + ex.ToString());
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00004786 File Offset: 0x00002986
		public static void CloseFilterWindow(string _)
		{
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x0007C3A4 File Offset: 0x0007A5A4
		public void CallBackToHtml(string callBackFunction, string data)
		{
			if (data != null)
			{
				data = ((data != null) ? data.Replace("\\", "\\\\") : null);
				string text = callBackFunction + "('" + ((data != null) ? data.Replace("'", "&#39;").Replace("%27", "&#39;") : null) + "')";
				Browser cefBrowser = this.CefBrowser;
				if (cefBrowser == null)
				{
					return;
				}
				string text2 = text;
				Browser cefBrowser2 = this.CefBrowser;
				cefBrowser.ExecuteJavaScript(text2, (cefBrowser2 != null) ? cefBrowser2.getURL() : null, 0);
			}
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0007C428 File Offset: 0x0007A628
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.CefBrowser != null)
				{
					this.CefBrowser.LoadEnd -= this.MBrowser_LoadEnd;
					this.CefBrowser.ProcessMessageRecieved -= this.Browser_ProcessMessageRecieved;
					this.CefBrowser.Loaded -= this.Browser_Loaded;
					this.CefBrowser.LoadError -= this.Browser_LoadError;
					this.CefBrowser.LoadingStateChange -= this.Browser_LoadingStateChange;
					this.CefBrowser.mWPFCefBrowserExceptionHandler -= this.Browser_WPFCefBrowserExceptionHandler;
					this.CefBrowser.Dispose();
					CefBrowserHost cefBrowserHost = this.mBrowserHost;
					if (cefBrowserHost != null)
					{
						cefBrowserHost.Dispose();
					}
					foreach (BrowserControlTags browserControlTags in this.TagsSubscribedDict.Keys)
					{
						BrowserSubscriber mSubscriber = this.mSubscriber;
						if (mSubscriber != null)
						{
							mSubscriber.UnsubscribeTag(browserControlTags);
						}
					}
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x0007C550 File Offset: 0x0007A750
		~BrowserControl()
		{
			this.Dispose(false);
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x0000E199 File Offset: 0x0000C399
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000CB4 RID: 3252
		private string SOURCE_APPCENTER = "BSAppCenter";

		// Token: 0x04000CB5 RID: 3253
		private float customZoomLevel;

		// Token: 0x04000CB6 RID: 3254
		private MainWindow mMainWindow;

		// Token: 0x04000CB7 RID: 3255
		private NoInternetControl mNoInternetControl;

		// Token: 0x04000CB8 RID: 3256
		private Browser mBrowser;

		// Token: 0x04000CB9 RID: 3257
		internal string mUrl;

		// Token: 0x04000CBA RID: 3258
		private double zoomLevel = 1.0;

		// Token: 0x04000CC0 RID: 3264
		internal static List<BrowserControl> sAllBrowserControls = new List<BrowserControl>();

		// Token: 0x04000CC1 RID: 3265
		internal static List<string> mFirebaseTagsSubscribed = new List<string>();

		// Token: 0x04000CC2 RID: 3266
		internal string mFirebaseCallbackMethod = string.Empty;

		// Token: 0x04000CC3 RID: 3267
		internal string mFailedUrl = string.Empty;

		// Token: 0x04000CC4 RID: 3268
		private CefBrowserHost mBrowserHost;

		// Token: 0x04000CC6 RID: 3270
		private bool disposedValue;
	}
}
