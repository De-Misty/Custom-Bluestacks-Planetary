using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI.BTv
{
	// Token: 0x020002C9 RID: 713
	public class StreamManager : IDisposable
	{
		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06001A09 RID: 6665 RVA: 0x0001187A File Offset: 0x0000FA7A
		// (set) Token: 0x06001A0A RID: 6666 RVA: 0x00011881 File Offset: 0x0000FA81
		public static string ObsServerBaseURL { get; set; } = "http://localhost";

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06001A0B RID: 6667 RVA: 0x00011889 File Offset: 0x0000FA89
		// (set) Token: 0x06001A0C RID: 6668 RVA: 0x00011890 File Offset: 0x0000FA90
		public static int ObsServerPort { get; set; } = 2851;

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06001A0D RID: 6669 RVA: 0x00011898 File Offset: 0x0000FA98
		// (set) Token: 0x06001A0E RID: 6670 RVA: 0x0001189F File Offset: 0x0000FA9F
		public static string DEFAULT_NETWORK { get; set; } = "twitch";

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001A0F RID: 6671 RVA: 0x000118A7 File Offset: 0x0000FAA7
		// (set) Token: 0x06001A10 RID: 6672 RVA: 0x000118AE File Offset: 0x0000FAAE
		public static bool DEFAULT_ENABLE_FILTER { get; set; } = true;

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06001A11 RID: 6673 RVA: 0x000118B6 File Offset: 0x0000FAB6
		// (set) Token: 0x06001A12 RID: 6674 RVA: 0x000118BD File Offset: 0x0000FABD
		public static bool DEFAULT_SQUARE_THEME { get; set; } = false;

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06001A13 RID: 6675 RVA: 0x000118C5 File Offset: 0x0000FAC5
		// (set) Token: 0x06001A14 RID: 6676 RVA: 0x000118CC File Offset: 0x0000FACC
		public static string DEFAULT_LAYOUT_THEME { get; set; } = null;

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06001A15 RID: 6677 RVA: 0x000118D4 File Offset: 0x0000FAD4
		// (set) Token: 0x06001A16 RID: 6678 RVA: 0x000118DB File Offset: 0x0000FADB
		public static bool sStopInitOBSQueue { get; set; } = false;

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06001A17 RID: 6679 RVA: 0x000118E3 File Offset: 0x0000FAE3
		// (set) Token: 0x06001A18 RID: 6680 RVA: 0x000118EB File Offset: 0x0000FAEB
		public string mCallbackStreamStatus { get; set; }

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06001A19 RID: 6681 RVA: 0x000118F4 File Offset: 0x0000FAF4
		// (set) Token: 0x06001A1A RID: 6682 RVA: 0x000118FC File Offset: 0x0000FAFC
		public string mCallbackAppInfo { get; set; }

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06001A1B RID: 6683 RVA: 0x00011905 File Offset: 0x0000FB05
		// (set) Token: 0x06001A1C RID: 6684 RVA: 0x0001190D File Offset: 0x0000FB0D
		public bool mIsObsRunning { get; set; }

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06001A1D RID: 6685 RVA: 0x00011916 File Offset: 0x0000FB16
		// (set) Token: 0x06001A1E RID: 6686 RVA: 0x0001191E File Offset: 0x0000FB1E
		public bool mIsInitCalled { get; set; }

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001A1F RID: 6687 RVA: 0x00011927 File Offset: 0x0000FB27
		// (set) Token: 0x06001A20 RID: 6688 RVA: 0x0001192F File Offset: 0x0000FB2F
		public bool mIsStreaming { get; set; }

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06001A21 RID: 6689 RVA: 0x00011938 File Offset: 0x0000FB38
		// (set) Token: 0x06001A22 RID: 6690 RVA: 0x00011940 File Offset: 0x0000FB40
		public bool mStoppingOBS { get; set; }

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06001A23 RID: 6691 RVA: 0x00011949 File Offset: 0x0000FB49
		// (set) Token: 0x06001A24 RID: 6692 RVA: 0x00011951 File Offset: 0x0000FB51
		public bool mIsReconnecting { get; set; }

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06001A25 RID: 6693 RVA: 0x0001195A File Offset: 0x0000FB5A
		// (set) Token: 0x06001A26 RID: 6694 RVA: 0x00011962 File Offset: 0x0000FB62
		public string mNetwork { get; set; } = StreamManager.DEFAULT_NETWORK;

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06001A27 RID: 6695 RVA: 0x0001196B File Offset: 0x0000FB6B
		// (set) Token: 0x06001A28 RID: 6696 RVA: 0x00011973 File Offset: 0x0000FB73
		public bool mSquareTheme { get; set; } = StreamManager.DEFAULT_SQUARE_THEME;

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06001A29 RID: 6697 RVA: 0x0001197C File Offset: 0x0000FB7C
		// (set) Token: 0x06001A2A RID: 6698 RVA: 0x00011984 File Offset: 0x0000FB84
		public string mLayoutTheme { get; set; } = StreamManager.DEFAULT_LAYOUT_THEME;

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06001A2B RID: 6699 RVA: 0x0001198D File Offset: 0x0000FB8D
		// (set) Token: 0x06001A2C RID: 6700 RVA: 0x00011995 File Offset: 0x0000FB95
		public string mLastCameraLayoutTheme { get; set; } = StreamManager.DEFAULT_LAYOUT_THEME;

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06001A2D RID: 6701 RVA: 0x0001199E File Offset: 0x0000FB9E
		// (set) Token: 0x06001A2E RID: 6702 RVA: 0x000119A6 File Offset: 0x0000FBA6
		public bool mAppViewLayout { get; set; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06001A2F RID: 6703 RVA: 0x000119AF File Offset: 0x0000FBAF
		// (set) Token: 0x06001A30 RID: 6704 RVA: 0x000119B7 File Offset: 0x0000FBB7
		public bool mEnableFilter { get; set; } = StreamManager.DEFAULT_ENABLE_FILTER;

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06001A31 RID: 6705 RVA: 0x000119C0 File Offset: 0x0000FBC0
		// (set) Token: 0x06001A32 RID: 6706 RVA: 0x000119C7 File Offset: 0x0000FBC7
		public static string CamStatus { get; set; }

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06001A33 RID: 6707 RVA: 0x000119CF File Offset: 0x0000FBCF
		// (set) Token: 0x06001A34 RID: 6708 RVA: 0x000119D7 File Offset: 0x0000FBD7
		public bool mReplayBufferEnabled { get; set; }

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06001A35 RID: 6709 RVA: 0x000119E0 File Offset: 0x0000FBE0
		// (set) Token: 0x06001A36 RID: 6710 RVA: 0x000119E8 File Offset: 0x0000FBE8
		public bool mCLRBrowserRunning { get; set; }

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06001A37 RID: 6711 RVA: 0x000119F1 File Offset: 0x0000FBF1
		// (set) Token: 0x06001A38 RID: 6712 RVA: 0x000119F9 File Offset: 0x0000FBF9
		public string mCurrentFilterAppPkg { get; set; }

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06001A39 RID: 6713 RVA: 0x00011A02 File Offset: 0x0000FC02
		// (set) Token: 0x06001A3A RID: 6714 RVA: 0x00011A09 File Offset: 0x0000FC09
		public static StreamManager Instance { get; set; } = null;

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x06001A3B RID: 6715 RVA: 0x0009A5D4 File Offset: 0x000987D4
		// (remove) Token: 0x06001A3C RID: 6716 RVA: 0x0009A60C File Offset: 0x0009880C
		public event EventHandler<CustomVolumeEventArgs> EventGetSystemVolume;

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06001A3D RID: 6717 RVA: 0x0009A644 File Offset: 0x00098844
		// (remove) Token: 0x06001A3E RID: 6718 RVA: 0x0009A67C File Offset: 0x0009887C
		public event EventHandler<CustomVolumeEventArgs> EventGetMicVolume;

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06001A3F RID: 6719 RVA: 0x0009A6B4 File Offset: 0x000988B4
		// (remove) Token: 0x06001A40 RID: 6720 RVA: 0x0009A6EC File Offset: 0x000988EC
		public event EventHandler<CustomVolumeEventArgs> EventGetCameraDetails;

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06001A41 RID: 6721 RVA: 0x0009A724 File Offset: 0x00098924
		// (remove) Token: 0x06001A42 RID: 6722 RVA: 0x0009A75C File Offset: 0x0009895C
		public event EventHandler<CustomVolumeEventArgs> EventGetMicDetails;

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06001A43 RID: 6723 RVA: 0x00011A11 File Offset: 0x0000FC11
		// (set) Token: 0x06001A44 RID: 6724 RVA: 0x00011A19 File Offset: 0x0000FC19
		public bool isWindowCaptureActive { get; set; }

		// Token: 0x06001A45 RID: 6725 RVA: 0x0009A794 File Offset: 0x00098994
		public StreamManager(Browser browser)
		{
			StreamManager.Instance = this;
			this.mBrowser = browser;
			this.mReplayBufferEnabled = RegistryManager.Instance.ReplayBufferEnabled == 1;
			if (RegistryManager.Instance.CamStatus == 1)
			{
				StreamManager.CamStatus = "true";
			}
			else
			{
				StreamManager.CamStatus = "false";
			}
			StreamManager.mSelectedCamera = RegistryManager.Instance.SelectedCam;
			this.mObsCommandEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
			MainWindow mainWindow = null;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
			}
			this.mWindow = mainWindow;
			this.CopySceneConfigFile(this.mWindow, false);
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x0009A8E0 File Offset: 0x00098AE0
		public StreamManager(MainWindow window)
		{
			StreamManager.Instance = this;
			this.mReplayBufferEnabled = true;
			if (RegistryManager.Instance.CamStatus == 1)
			{
				StreamManager.CamStatus = "true";
			}
			else
			{
				StreamManager.CamStatus = "false";
			}
			StreamManager.mSelectedCamera = RegistryManager.Instance.SelectedCam;
			this.mObsCommandEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
			this.mWindow = window;
			this.CopySceneConfigFile(this.mWindow, !RegistryManager.Instance.IsGameCaptureSupportedInMachine);
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x0009AA04 File Offset: 0x00098C04
		public void CopySceneConfigFile(MainWindow activatedWindow, bool forceWindowCaptureMode = false)
		{
			Logger.Debug("In Scene config file copy method with glmode: {0}", new object[] { (activatedWindow != null) ? new int?(activatedWindow.EngineInstanceRegistry.GlRenderMode) : null });
			string text = Path.Combine(RegistryStrings.ObsDir, "sceneCollection");
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				if (activatedWindow.EngineInstanceRegistry.GlRenderMode != 1 || forceWindowCaptureMode)
				{
					string text2 = Path.Combine(RegistryStrings.ObsDir, "SceneConfigFiles\\scenes_window.xconfig");
					string text3 = Path.Combine(RegistryStrings.ObsDir, "scenes.xconfig");
					string text4 = Path.Combine(RegistryStrings.ObsDir, "sceneCollection\\scenes.xconfig");
					File.Copy(text2, text3, true);
					File.Copy(text2, text4, true);
					this.isWindowCaptureActive = true;
				}
				else
				{
					string text5 = Path.Combine(RegistryStrings.ObsDir, "SceneConfigFiles\\scenes_graphics.xconfig");
					string text6 = Path.Combine(RegistryStrings.ObsDir, "scenes.xconfig");
					string text7 = Path.Combine(RegistryStrings.ObsDir, "sceneCollection\\scenes.xconfig");
					File.Copy(text5, text6, true);
					File.Copy(text5, text7, true);
					this.isWindowCaptureActive = false;
				}
				Logger.Debug("Is window capture active..: {0}", new object[] { this.isWindowCaptureActive });
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in obs scene config file : {0}", new object[] { ex });
			}
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x0009AB54 File Offset: 0x00098D54
		internal void OrientationChangeHandler()
		{
			try
			{
				if (this.isWindowCaptureActive)
				{
					this.SetCaptureSize();
				}
				this.RefreshCapture();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in OrientationChangeHandler : " + ex.ToString());
			}
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x00011A22 File Offset: 0x0000FC22
		private void RefreshCapture()
		{
			this.SendObsRequest("refreshCapture", null, null, null, 0, false);
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x0009ABA0 File Offset: 0x00098DA0
		public void SendCurrentAppInfoAtTabChange()
		{
			try
			{
				if (this.mBrowser != null)
				{
					if (!string.IsNullOrEmpty(this.mBrowser.getURL()))
					{
						AppTabButton selectedTab = this.mWindow.mTopBar.mAppTabButtons.SelectedTab;
						if (selectedTab != null)
						{
							selectedTab.mTabType.ToString();
							string appName = selectedTab.AppName;
							string packageName = selectedTab.PackageName;
							string text = new JObject
							{
								{ "type", "app" },
								{ "name", appName },
								{ "data", packageName }
							}.ToString(Formatting.None, new JsonConverter[0]);
							string text2 = "getCurrentAppInfo('" + text.Replace("'", "&#39;").Replace("%27", "&#39;") + "')";
							this.mBrowser.ExecuteJavaScript(text2, this.mBrowser.getURL(), 0);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in send current app status on tab changed " + ex.ToString());
			}
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x00011A34 File Offset: 0x0000FC34
		public void StartObs()
		{
			if (!this.mIsInitCalled)
			{
				this.InitObs();
			}
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x0009ACD0 File Offset: 0x00098ED0
		private void InitObs()
		{
			this.mIsInitCalled = true;
			Utils.KillCurrentOemProcessByName("HD-OBS", null);
			if (!ProcessUtils.FindProcessByName("HD-OBS") && !StreamManager.sStopInitOBSQueue)
			{
				StreamManager.StartOBS();
			}
			if (StreamManager.sStopInitOBSQueue)
			{
				return;
			}
			try
			{
				string text = this.SendObsRequestInternal("ping", null);
				Logger.Info("response for ping is {0}", new object[] { text });
				this.mIsObsRunning = true;
			}
			catch (Exception ex)
			{
				if (StreamManager.sStopInitOBSQueue || StreamManager.Instance == null)
				{
					return;
				}
				Logger.Error("Exception in InitObs. err: " + ex.ToString());
				Thread.Sleep(100);
				if (this.mObsRetryCount <= 0)
				{
					this.ShutDownForcefully();
					throw new Exception("Could not start OBS.");
				}
				this.mObsRetryCount--;
				this.InitObs();
			}
			this.mObsRetryCount = 2;
			new Thread(delegate
			{
				this.ProcessObsCommandQueue();
			})
			{
				IsBackground = true
			}.Start();
			if (this.mReplayBufferEnabled)
			{
				this.SetReplayBufferSavePath();
			}
			this.GetParametersFromOBS();
			this.EnableSource("BlueStacks");
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x0009ADF0 File Offset: 0x00098FF0
		private void SetBackGroundImagePath()
		{
			this.EnableSource("BackGroundImage");
			string text = Path.Combine(RegistryStrings.ObsDir, "backgrounds\\Background3.jpg");
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "path", text } };
			this.SendObsRequest("setBackground", dictionary, null, null, 0);
		}

		// Token: 0x06001A4E RID: 6734 RVA: 0x0009AE3C File Offset: 0x0009903C
		public void SetSceneConfiguration(string layoutTheme)
		{
			this.mAppViewLayout = false;
			this.mLayoutTheme = layoutTheme;
			if (layoutTheme == null)
			{
				this.SendObsRequest("resettooriginalscene", null, null, null, 0, false);
				return;
			}
			this.SetCaptureSize();
			this.DisableSource("CLR Browser");
			try
			{
				JObject jobject = JObject.Parse(layoutTheme);
				Logger.Info(layoutTheme);
				bool flag = StreamManager.IsPortraitApp();
				if (jobject["isPortrait"] != null)
				{
					flag = jobject["isPortrait"].ToObject<bool>();
				}
				JObject jobject2;
				if (flag)
				{
					jobject2 = JObject.Parse(jobject["portrait"].ToString());
				}
				else
				{
					jobject2 = JObject.Parse(jobject["landscape"].ToString());
				}
				if (jobject2["BlueStacksWebcam"] != null)
				{
					bool flag2;
					if (bool.TryParse(jobject2["BlueStacksWebcam"]["enableWebCam"].ToString(), out flag2) && flag2)
					{
						this.SetCameraPosition(Convert.ToInt32(jobject2["BlueStacksWebcam"]["x"].ToString(), CultureInfo.InvariantCulture), Convert.ToInt32(jobject2["BlueStacksWebcam"]["y"].ToString(), CultureInfo.InvariantCulture), Convert.ToInt32(jobject2["BlueStacksWebcam"]["width"].ToString(), CultureInfo.InvariantCulture), Convert.ToInt32(jobject2["BlueStacksWebcam"]["height"].ToString(), CultureInfo.InvariantCulture), flag2 ? 1 : 0);
					}
					else
					{
						this.DisableWebcamAndClearDictionary();
					}
				}
				if (jobject2["BlueStacks"] != null)
				{
					string text = jobject2["BlueStacks"]["width"].ToString();
					string text2 = jobject2["BlueStacks"]["height"].ToString();
					string text3 = jobject2["BlueStacks"]["x"].ToString();
					string text4 = jobject2["BlueStacks"]["y"].ToString();
					if (!this.isWindowCaptureActive)
					{
						text = text2;
						if (jobject["name"] != null)
						{
							string text5 = jobject["name"].ToString();
							if (string.Equals(text5, "layout_2", StringComparison.InvariantCulture) || string.Equals(text5, "layout_3", StringComparison.InvariantCulture))
							{
								text3 = "22";
								if (flag && string.Equals(text5, "layout_3", StringComparison.InvariantCulture))
								{
									text3 = "0";
								}
							}
							else if (string.Equals(text5, "layout_1", StringComparison.InvariantCulture))
							{
								text3 = "47";
							}
						}
					}
					this.SetFrontendPosition(0, 0, Convert.ToInt32(text, CultureInfo.InvariantCulture), Convert.ToInt32(text2, CultureInfo.InvariantCulture));
					this.SetPosition(Convert.ToInt32(text3, CultureInfo.InvariantCulture), Convert.ToInt32(text4, CultureInfo.InvariantCulture));
					this.EnableSource("BlueStacks");
				}
				else
				{
					this.DisableSource("BlueStacks");
				}
				string text6 = jobject["order"].ToString();
				string text7 = jobject["logo"].ToString();
				text6 += ",BackGroundImage";
				string text8 = "watermarkFB,watermark,watermarkGif";
				if (jobject["allLogo"] != null)
				{
					text8 = jobject["allLogo"].ToString();
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "order", text6 },
					{ "logo", text7 },
					{ "allLogo", text8 }
				};
				this.SendObsRequest("setorderandlogo", dictionary, null, null, 0, false);
			}
			catch (Exception ex)
			{
				Logger.Error("SetSceneConfiguration: Error {0}", new object[] { ex });
			}
		}

		// Token: 0x06001A4F RID: 6735 RVA: 0x0009B1E4 File Offset: 0x000993E4
		public static bool IsPortraitApp()
		{
			int frontendWidth = RegistryManager.Instance.FrontendWidth;
			int frontendHeight = RegistryManager.Instance.FrontendHeight;
			return frontendWidth <= frontendHeight;
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x0009B20C File Offset: 0x0009940C
		private static void StartOBS()
		{
			Logger.Info("starting obs");
			string obsBinaryPath = RegistryStrings.ObsBinaryPath;
			string text = "-port " + RegistryManager.Instance.PartnerServerPort.ToString();
			if (SystemUtils.IsOs64Bit())
			{
				text += " -64bit";
			}
			if (!string.IsNullOrEmpty(Strings.OEMTag))
			{
				text = text + " -oem " + Strings.OEMTag;
			}
			ProcessUtils.GetProcessObject(obsBinaryPath, text, false).Start();
			Logger.Info("OBS started");
			StreamManager.ObsServerPort = RegistryManager.Instance.OBSServerPort;
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x0009B29C File Offset: 0x0009949C
		public void SetHwnd(string handle)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "hwnd", handle } };
			this.SendObsRequest("sethwnd", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A52 RID: 6738 RVA: 0x0009B2CC File Offset: 0x000994CC
		public void SetSavePath(string path = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = path ?? Path.Combine(RegistryStrings.BtvDir, "stream.flv");
			this.mLastVideoFilePath = text;
			dictionary.Add("savepath", text);
			this.SendObsRequest("setsavepath", dictionary, null, "SetSaveFailed", 0, false);
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x00011A44 File Offset: 0x0000FC44
		private void SetSaveFailed()
		{
			Logger.Error("Exception in SetSaveFailed");
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x0009B31C File Offset: 0x0009951C
		private void SetReplayBufferSavePath()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = Path.Combine(RegistryStrings.BtvDir, "replay.flv");
			dictionary.Add("savepath", text);
			this.SendObsRequest("setreplaybuffersavepath", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x0009B35C File Offset: 0x0009955C
		public static void SetStreamDimension(out int startX, out int startY, out int width, out int height)
		{
			try
			{
				BTVManager.GetStreamDimensionInfo(out startX, out startY, out width, out height);
			}
			catch (Exception ex)
			{
				Logger.Error("Got Exception in getting stream dimension... Err : " + ex.ToString());
				startX = (startY = (width = (height = 0)));
			}
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x0009B3B0 File Offset: 0x000995B0
		public void SetFrontendPosition()
		{
			int num;
			int num2;
			int num3;
			int num4;
			StreamManager.SetStreamDimension(out num, out num2, out num3, out num4);
			int @int = Utils.GetInt(RegistryManager.Instance.FrontendHeight, num4);
			int num5;
			if (this.isWindowCaptureActive)
			{
				num5 = RegistryManager.Instance.FrontendWidth;
			}
			else
			{
				num5 = (int)this.GetWidthFromHeight((double)@int);
			}
			this.SetFrontendPosition(num5, @int, num, num2, num3, num4);
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x0009B40C File Offset: 0x0009960C
		public void SetFrontendPosition(int frontendWidth, int frontendHeight)
		{
			int num;
			int num2;
			int num3;
			int num4;
			StreamManager.SetStreamDimension(out num, out num2, out num3, out num4);
			this.SetFrontendPosition(frontendWidth, frontendHeight, num, num2, num3, num4);
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x0009B434 File Offset: 0x00099634
		public void SetFrontendPosition(int frontendWidth, int frontendHeight, int startX, int startY, int width, int height)
		{
			startY += (height - frontendHeight) / 2;
			startX += (width - frontendWidth) / 2;
			if (this.mEnableFilter)
			{
				int num = frontendWidth * 100 / (frontendHeight * 16 / 9);
				int num2 = (100 - num) / 2;
				this.SetFrontendPosition(num2, 0, num, 100);
				this.SetPosition(num2, 0);
			}
			else
			{
				this.SetFrontendPosition(0, 0, 100, 100);
				if (!this.mSquareTheme)
				{
					this.SetPosition(0, 0);
				}
			}
			this.SetCaptureSize(startX, startY, frontendWidth, frontendHeight);
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x0009B4B0 File Offset: 0x000996B0
		public void SetFrontendPosition(int startX, int startY, int width, int height)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"width",
					width.ToString(CultureInfo.InvariantCulture)
				},
				{
					"height",
					height.ToString(CultureInfo.InvariantCulture)
				},
				{
					"y",
					startY.ToString(CultureInfo.InvariantCulture)
				},
				{
					"x",
					startX.ToString(CultureInfo.InvariantCulture)
				},
				{ "source", "BlueStacks" }
			};
			this.SendObsRequest("setsourceposition", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x0009B540 File Offset: 0x00099740
		public void SetPosition(int startX, int startY)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"y",
					startY.ToString(CultureInfo.InvariantCulture)
				},
				{
					"x",
					startX.ToString(CultureInfo.InvariantCulture)
				},
				{ "source", "BlueStacks" }
			};
			this.SendObsRequest("setposition", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x0009B5A4 File Offset: 0x000997A4
		public void SetFrontEndCaptureSize(int width, int height)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"width",
					width.ToString(CultureInfo.InvariantCulture)
				},
				{
					"height",
					height.ToString(CultureInfo.InvariantCulture)
				},
				{ "source", "BlueStacks" }
			};
			this.SendObsRequest("SetFrontendCaptureSize", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x0009B608 File Offset: 0x00099808
		public void SetCaptureSize()
		{
			int num;
			int num2;
			int num3;
			int num4;
			StreamManager.SetStreamDimension(out num, out num2, out num3, out num4);
			int @int = Utils.GetInt(RegistryManager.Instance.FrontendHeight, num4);
			num2 += (num4 - @int) / 2;
			int num5 = (this.isWindowCaptureActive ? RegistryManager.Instance.FrontendWidth : ((int)this.GetWidthFromHeight((double)@int)));
			num += (num3 - num5) / 2;
			Logger.Info("frontendWidth for set capture size : " + num5.ToString());
			Logger.Info("frontendHeight for set capture size : " + @int.ToString());
			this.SetCaptureSize(num, num2, num5, @int);
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x0009B6A0 File Offset: 0x000998A0
		private void SetCaptureSize(int startX, int startY, int width, int height)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Logger.Info("width for set capture size : " + width.ToString());
			Logger.Info("height for set capture size : " + height.ToString());
			dictionary.Add("width", width.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("height", height.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("x", startX.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("y", startY.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("source", "BlueStacks");
			this.SendObsRequest("setcapturesize", dictionary, null, null, 0);
			if (this.isWindowCaptureActive)
			{
				this.SetFrontEndCaptureSize(RegistryManager.Instance.FrontendWidth, RegistryManager.Instance.FrontendHeight);
				if (StreamManager.IsPortraitApp())
				{
					this.SetPosition(35, 0);
					return;
				}
				this.SetPosition(0, 0);
			}
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x0009B798 File Offset: 0x00099998
		public void ResetCLRBrowser(bool isSetFrontEndPosition = true)
		{
			this.DisableSource("CLR Browser");
			if (isSetFrontEndPosition)
			{
				this.SetFrontendPosition();
			}
			if (string.Compare(StreamManager.CamStatus, "true", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.EnableWebcamInternal("320", "240", "3");
			}
			else
			{
				this.DisableWebcamAndClearDictionary();
			}
			this.mCLRBrowserRunning = false;
			this.mCurrentFilterAppPkg = null;
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x0009B7F8 File Offset: 0x000999F8
		internal void EnableVideoRecording(bool enable)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (enable)
			{
				dictionary.Add("Enable", "1");
			}
			else
			{
				dictionary.Add("Enable", "0");
			}
			this.SendObsRequest("EnableVideoRecording", dictionary, null, null, 0);
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x0009B840 File Offset: 0x00099A40
		private void SetCameraPosition(int x, int y, int width, int height, int render)
		{
			Logger.Info("camera position width is : " + width.ToString() + " and height is :" + height.ToString());
			StreamManager.mIsWebcamDisabled = false;
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "source", "BlueStacksWebcam" },
				{
					"width",
					width.ToString(CultureInfo.InvariantCulture)
				},
				{
					"height",
					height.ToString(CultureInfo.InvariantCulture)
				},
				{
					"x",
					x.ToString(CultureInfo.InvariantCulture)
				},
				{
					"y",
					y.ToString(CultureInfo.InvariantCulture)
				},
				{
					"render",
					render.ToString(CultureInfo.InvariantCulture)
				}
			};
			if (StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera))
			{
				dictionary.Add("camera", StreamManager.mDictCameraDetails[StreamManager.mSelectedCamera]);
			}
			else
			{
				dictionary["camera"] = string.Empty;
			}
			StreamManager.mDictLastCameraPosition = dictionary;
			this.SendObsRequest("setcameraposition", dictionary, "WebcamConfigured", null, 0, false);
		}

		// Token: 0x06001A61 RID: 6753 RVA: 0x0009B958 File Offset: 0x00099B58
		internal void UpdateCameraPosition(string camName)
		{
			this.DisableWebcam();
			if (!string.IsNullOrEmpty(camName))
			{
				StreamManager.mSelectedCamera = camName;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["source"] = "BlueStacksWebcam";
			dictionary["width"] = "100";
			dictionary["height"] = "100";
			dictionary["x"] = "0";
			dictionary["y"] = "0";
			dictionary["render"] = "1";
			Dictionary<string, string> dictionary2 = dictionary;
			if (StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera))
			{
				dictionary2["camera"] = StreamManager.mDictCameraDetails[StreamManager.mSelectedCamera];
			}
			else
			{
				dictionary2["camera"] = string.Empty;
			}
			this.SendObsRequest("setcameraposition", dictionary2, "WebcamConfigured", null, 0, false);
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x0009BA30 File Offset: 0x00099C30
		internal void ChangeCamera()
		{
			this.DisableWebcam();
			if (!StreamManager.mIsWebcamDisabled)
			{
				if (StreamManager.mDictLastCameraPosition.Count == 0)
				{
					StreamManager.mDictLastCameraPosition["source"] = "BlueStacksWebcam";
					StreamManager.mDictLastCameraPosition["width"] = "17";
					StreamManager.mDictLastCameraPosition["height"] = "23";
					StreamManager.mDictLastCameraPosition["x"] = "0";
					StreamManager.mDictLastCameraPosition["y"] = "77";
					StreamManager.mDictLastCameraPosition["render"] = "1";
				}
				if (StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera))
				{
					StreamManager.mDictLastCameraPosition["camera"] = StreamManager.mDictCameraDetails[StreamManager.mSelectedCamera];
				}
				else
				{
					StreamManager.mDictLastCameraPosition["camera"] = string.Empty;
				}
				this.SendObsRequest("setcameraposition", StreamManager.mDictLastCameraPosition, "WebcamConfigured", null, 0, false);
			}
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x0009BB30 File Offset: 0x00099D30
		public void SetClrBrowserConfig(string width, string height, string url)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "width", width },
				{ "height", height },
				{ "url", url }
			};
			this.SendObsRequest("setclrbrowserconfig", dictionary, null, null, 0);
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x00011A50 File Offset: 0x0000FC50
		public void ObsErrorStatus(string erroReason)
		{
			this.mIsStreaming = false;
			this.mFailureReason = "Error starting stream : " + erroReason;
			this.SendStreamStatus(false, false);
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x0009BB78 File Offset: 0x00099D78
		public void ReportObsError(string errorReason)
		{
			try
			{
				Logger.Info("error reason in obs :" + errorReason);
				string text = "stream_interrupted_error";
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (string.Equals(errorReason, "ConnectionSuccessfull", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!this.mIsStreamStarted)
					{
						this.mIsStreamStarted = true;
						text = "obs_connected";
					}
					else
					{
						text = "stream_resumed";
					}
				}
				else if (!this.mIsStreamStarted)
				{
					text = "went_live_error";
				}
				if (string.Equals(errorReason, "OBSAlreadyRunning", StringComparison.InvariantCultureIgnoreCase))
				{
					text = "obs_already_running";
					this.ReportStreamStatsToCloud(text, errorReason);
					dictionary.Add("reason", text);
					StreamManager.ReportObsErrorHandler(text);
				}
				else if (string.Equals(errorReason, "capture_error", StringComparison.InvariantCultureIgnoreCase))
				{
					text = "capture_error";
					this.ReportStreamStatsToCloud(text, errorReason);
					StreamManager.sStopInitOBSQueue = true;
					dictionary.Add("reason", text);
					StreamManager.ReportObsErrorHandler(text);
				}
				else if (string.Equals(errorReason, "opengl_capture_error", StringComparison.InvariantCultureIgnoreCase))
				{
					text = "opengl_capture_error";
					this.ReportStreamStatsToCloud(text, errorReason);
					StreamManager.sStopInitOBSQueue = true;
					dictionary.Add("reason", text);
					StreamManager.ReportObsErrorHandler(text);
				}
				else if (string.Equals(errorReason, "AccessDenied", StringComparison.InvariantCultureIgnoreCase) || string.Equals(errorReason, "ConnectServerError", StringComparison.InvariantCultureIgnoreCase) || string.Equals(errorReason, "obs_error", StringComparison.InvariantCultureIgnoreCase))
				{
					errorReason = "Error starting stream : " + errorReason;
					this.ReportStreamStatsToCloud(text, errorReason);
					dictionary.Add("reason", "obs_error");
					StreamManager.ReportObsErrorHandler("obs_error");
				}
				else if (string.Equals(errorReason, "ConnectionSuccessfull", StringComparison.InvariantCultureIgnoreCase))
				{
					this.ReportStreamStatsToCloud(text, errorReason);
				}
				else
				{
					errorReason = "Error starting stream : " + errorReason;
					this.ReportStreamStatsToCloud(text, errorReason);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to report obs error.. Err : " + ex.ToString());
			}
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x0009BD40 File Offset: 0x00099F40
		public void RestartOBSInWindowCaptureMode()
		{
			try
			{
				this.ShutDownForcefully();
				this.CopySceneConfigFile(this.mWindow, true);
				Logger.Info("restarting obs in window capture mode");
				if (File.Exists(this.mLastVideoFilePath))
				{
					File.Delete(this.mLastVideoFilePath);
				}
				this.mWindow.mCommonHandler.RecordVideoOfApp();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in restart obs in window capture mode: {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x0009BDC0 File Offset: 0x00099FC0
		private static void ReportObsErrorHandler(string reason)
		{
			Logger.Error("Obs reported an error. " + reason);
			if (string.Equals(reason, "opengl_capture_error", StringComparison.OrdinalIgnoreCase))
			{
				RegistryManager.Instance.IsGameCaptureSupportedInMachine = false;
				StreamManager.Instance.RestartOBSInWindowCaptureMode();
				return;
			}
			StreamManager instance = StreamManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.mWindow.Dispatcher.Invoke(new Action(delegate
			{
				StreamManager.Instance.mWindow.mCommonHandler.ShowErrorRecordingVideoPopup();
			}), new object[0]);
		}

		// Token: 0x06001A68 RID: 6760 RVA: 0x0009BE40 File Offset: 0x0009A040
		private void ReportStreamStatsToCloud(string eventType, string reason)
		{
			Logger.Info("StreamStats eventType: {0}, reason: {1}", new object[] { eventType, reason });
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("event_type", eventType);
			dictionary.Add("error_code", reason);
			dictionary.Add("guid", RegistryManager.Instance.UserGuid);
			dictionary.Add("streaming_platform", this.mNetwork);
			dictionary.Add("session_id", Stats.GetSessionId());
			dictionary.Add("prod_ver", "4.220.0.4001");
			dictionary.Add("created_at", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture));
		}

		// Token: 0x06001A69 RID: 6761 RVA: 0x00011A72 File Offset: 0x0000FC72
		internal void GetParametersFromOBS()
		{
			this.SendObsRequest("getmicvolume", null, "SetMicVolumeLocal", null, 0, false);
			this.SendObsRequest("getsystemvolume", null, "SetSystemVolumeLocal", null, 0, false);
		}

		// Token: 0x06001A6A RID: 6762 RVA: 0x0009BEEC File Offset: 0x0009A0EC
		private void SetMicLocal(string response)
		{
			try
			{
				List<string> list = response.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				List<string> list2 = response.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				StreamManager.mSelectedMic = response.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[2];
				StreamManager.mDictMicDetails.Clear();
				StreamManager.mSelectedMic = Regex.Replace(StreamManager.mSelectedMic, "[^\\u0000-\\u007F]+", string.Empty);
				if (list2.Count == 0)
				{
					StreamManager.mSelectedMic = string.Empty;
					StreamManager.mIsMicDisabled = true;
				}
				else
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (!string.Equals(list2[i], "Default", StringComparison.InvariantCulture) && !string.Equals(list2[i], "Disable", StringComparison.InvariantCulture))
						{
							StreamManager.mDictMicDetails.Add(Regex.Replace(list[i], "[^\\u0000-\\u007F]+", string.Empty), list2[i]);
						}
					}
					if (StreamManager.mDictMicDetails.Count == 0)
					{
						StreamManager.mSelectedMic = string.Empty;
						StreamManager.mIsMicDisabled = true;
					}
					else if (!StreamManager.mDictMicDetails.ContainsKey(StreamManager.mSelectedMic))
					{
						StreamManager.mSelectedMic = StreamManager.mDictMicDetails.Keys.ToList<string>()[0];
					}
				}
				EventHandler<CustomVolumeEventArgs> eventGetMicDetails = this.EventGetMicDetails;
				if (eventGetMicDetails != null)
				{
					eventGetMicDetails(this, new CustomVolumeEventArgs(StreamManager.mDictMicDetails, StreamManager.mSelectedMic));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in SetMicLocal. response: " + response);
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x0009C0B8 File Offset: 0x0009A2B8
		private void SetMicVolumeLocal(string volumeResponse)
		{
			try
			{
				StreamManager.mMicVolume = JObject.Parse(volumeResponse)["volume"].ToObject<int>();
				EventHandler<CustomVolumeEventArgs> eventGetMicVolume = this.EventGetMicVolume;
				if (eventGetMicVolume != null)
				{
					eventGetMicVolume(this, new CustomVolumeEventArgs(StreamManager.mMicVolume));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in SetMicVolumeLocal. response: " + volumeResponse);
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x0009C12C File Offset: 0x0009A32C
		private void SetCameraLocal(string cameraResponse)
		{
			try
			{
				List<string> list = cameraResponse.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				List<string> list2 = cameraResponse.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				StreamManager.mDictCameraDetails.Clear();
				if (list2.Count == 0)
				{
					StreamManager.mSelectedCamera = string.Empty;
					StreamManager.mIsWebcamDisabled = true;
				}
				else
				{
					for (int i = 0; i < list2.Count; i++)
					{
						StreamManager.mDictCameraDetails.Add(Regex.Replace(list[i], "[^\\u0000-\\u007F]+", string.Empty).Trim(), list2[i]);
					}
					if (!StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera))
					{
						StreamManager.mSelectedCamera = Regex.Replace(cameraResponse.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries)[2], "[^\\u0000-\\u007F]+", string.Empty);
					}
				}
				EventHandler<CustomVolumeEventArgs> eventGetCameraDetails = this.EventGetCameraDetails;
				if (eventGetCameraDetails != null)
				{
					eventGetCameraDetails(this, new CustomVolumeEventArgs(StreamManager.mDictCameraDetails, StreamManager.mSelectedCamera));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in SetCameraLocal. response: " + cameraResponse);
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x0009C290 File Offset: 0x0009A490
		private void SetSystemVolumeLocal(string volumeResponse)
		{
			try
			{
				StreamManager.mSystemVolume = JObject.Parse(volumeResponse)["volume"].ToObject<int>();
				EventHandler<CustomVolumeEventArgs> eventGetSystemVolume = this.EventGetSystemVolume;
				if (eventGetSystemVolume != null)
				{
					eventGetSystemVolume(this, new CustomVolumeEventArgs(StreamManager.mSystemVolume));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in SetSystemVolumeLocal. response: " + volumeResponse);
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x00004786 File Offset: 0x00002986
		public static void EnableWebcam(string _1, string _2, string _3)
		{
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x0009C304 File Offset: 0x0009A504
		public void DisableSource(string source)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "source", source },
				{ "render", "0" }
			};
			this.SendObsRequest("setrender", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x0009C344 File Offset: 0x0009A544
		public void EnableSource(string source)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "source", source },
				{ "render", "1" }
			};
			this.SendObsRequest("setrender", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x0009C384 File Offset: 0x0009A584
		public void EnableWebcamInternal(string widthStr, string heightStr, string position)
		{
			int num = Convert.ToInt32(widthStr, CultureInfo.InvariantCulture);
			int num2 = Convert.ToInt32(heightStr, CultureInfo.InvariantCulture);
			if (this.mStreamHeight == 0 || this.mStreamWidth == 0)
			{
				return;
			}
			num = num * 100 / this.mStreamWidth;
			num2 = num2 * 100 / this.mStreamHeight;
			new Dictionary<string, string>();
			int num3 = 0;
			int num4 = 0;
			if (string.Equals(position, "2", StringComparison.InvariantCultureIgnoreCase))
			{
				num3 = 100 - num;
			}
			else if (string.Equals(position, "3", StringComparison.InvariantCultureIgnoreCase))
			{
				num4 = 100 - num2;
			}
			else if (string.Equals(position, "4", StringComparison.InvariantCultureIgnoreCase))
			{
				num3 = 100 - num;
				num4 = 100 - num;
			}
			this.SetCameraPosition(num3, num4, num, num2, 1);
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x00004786 File Offset: 0x00002986
		public void DisableWebcamV2(string _)
		{
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x00011A9C File Offset: 0x0000FC9C
		public void DisableWebcamAndClearDictionary()
		{
			StreamManager.mDictLastCameraPosition.Clear();
			StreamManager.mIsWebcamDisabled = true;
			this.DisableWebcam();
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x00011AB4 File Offset: 0x0000FCB4
		internal void DisableWebcam()
		{
			this.DisableSource("BlueStacksWebcam");
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0009C42C File Offset: 0x0009A62C
		private void WebcamConfigured(string response)
		{
			try
			{
				JObject jobject = JObject.Parse(response);
				if (jobject["success"].ToObject<bool>())
				{
					StreamManager.CamStatus = jobject["webcam"].ToObject<bool>().ToString(CultureInfo.InvariantCulture);
					if (Convert.ToBoolean(StreamManager.CamStatus, CultureInfo.InvariantCulture))
					{
						RegistryManager.Instance.CamStatus = 1;
					}
					else
					{
						RegistryManager.Instance.CamStatus = 0;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Setting WebCamRegistry. response: {0} err : {1}", new object[]
				{
					response,
					ex.ToString()
				});
			}
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x00011AC1 File Offset: 0x0000FCC1
		public void ResetFlvStream()
		{
			this.SendObsRequest("resetflvstream", null, null, null, 0);
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0009C4D0 File Offset: 0x0009A6D0
		public void SetSquareConfig(int startX, int startY, int width, int height)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Logger.Info("Window size: ({0}, {1})", new object[] { width, height });
			this.widthDiff = 0;
			int num;
			int num2;
			MiscUtils.GetStreamWidthAndHeight(width, height, out num, out num2);
			int num3 = (int)this.GetWidthFromHeight((double)num2);
			Logger.Info("Stream size: ({0}, {1})", new object[] { num3, num2 });
			width = num3;
			height = num2;
			height = width;
			string text;
			int num4;
			if (num2 <= 720)
			{
				text = "main";
				num4 = 2500;
			}
			else
			{
				text = "high";
				num4 = 3500;
			}
			float num5 = 1f;
			Logger.Info("x : " + startX.ToString());
			Logger.Info("y : " + startY.ToString());
			Logger.Info("width : " + width.ToString());
			Logger.Info("height : " + height.ToString());
			dictionary.Clear();
			dictionary.Add("startX", startX.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("startY", startY.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("width", width.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("height", height.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("x264Profile", text);
			dictionary.Add("maxBitrate", num4.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("downscale", num5.ToString(CultureInfo.InvariantCulture));
			this.mStreamWidth = width;
			this.mStreamHeight = height;
			this.SendObsRequest("setconfig", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x00011AD2 File Offset: 0x0000FCD2
		public void SetConfig(int startX, int startY, int width, int height)
		{
			if (this.mSquareTheme)
			{
				this.SetSquareConfig(startX, startY, width, height);
				return;
			}
			this.SetDefaultConfig(startX, startY, width, height);
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x00011AF3 File Offset: 0x0000FCF3
		public void RestartRecord()
		{
			this.StopRecord(true);
			this.StartRecord();
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0009C68C File Offset: 0x0009A88C
		public void SetDefaultConfig(int startX, int startY, int width, int height)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Logger.Info("Window size: ({0}, {1})", new object[] { width, height });
			this.widthDiff = 0;
			int num;
			int num2;
			MiscUtils.GetStreamWidthAndHeight(width, height, out num, out num2);
			int num3 = (int)this.GetWidthFromHeight((double)num2);
			width = num3;
			height = num2;
			int num4 = width * 9 / 16;
			Logger.Info("Stream size: ({0}, {1})", new object[] { num3, num2 });
			string text;
			int num5;
			if (num2 == 540)
			{
				text = "main";
				num5 = 1200;
			}
			else if (num2 == 720)
			{
				text = "main";
				num5 = 2500;
			}
			else
			{
				text = "high";
				num5 = 3500;
			}
			float num6 = (float)height / (float)num2;
			Logger.Info("x : " + startX.ToString());
			Logger.Info("y : " + startY.ToString());
			Logger.Info("width : " + width.ToString());
			Logger.Info("height : " + height.ToString());
			dictionary.Clear();
			dictionary.Add("startX", startX.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("startY", startY.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("width", width.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("height", height.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("x264Profile", text);
			dictionary.Add("maxBitrate", num5.ToString(CultureInfo.InvariantCulture));
			dictionary.Add("downscale", num6.ToString(CultureInfo.InvariantCulture));
			this.mStreamWidth = width;
			this.mStreamHeight = height;
			this.SendObsRequest("setconfig", dictionary, null, null, 0, false);
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x0009C868 File Offset: 0x0009AA68
		public void StartStream(string key, string location, string callbackStreamStatus, string callbackAppInfo)
		{
			string text = "1";
			this.mCallbackStreamStatus = callbackStreamStatus;
			if (this.mCallbackAppInfo == null)
			{
				this.mCallbackAppInfo = callbackAppInfo;
			}
			this.SetStreamSettings(text, key, location);
			this.SendObsRequest("startstream", null, "StreamStarted", null, 0);
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x0009C8B0 File Offset: 0x0009AAB0
		public void StartStream(string jsonString, string callbackStreamStatus, string callbackAppInfo)
		{
			Logger.Info(jsonString);
			JObject jobject = JObject.Parse(jsonString);
			string text = jobject["key"].ToString();
			string text2 = jobject["service"].ToString();
			string text3 = jobject["streamUrl"].ToString();
			jobject["server"].ToString();
			this.mCallbackStreamStatus = callbackStreamStatus;
			this.mCallbackAppInfo = callbackAppInfo;
			this.SetStreamSettings(text2, text, text3);
			this.SendObsRequest("startstream", null, "StreamStarted", null, 0);
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x00011B02 File Offset: 0x0000FD02
		public void StopStream()
		{
			this.SendObsRequest("stopstream", null, "StreamStopped", null, 0);
		}

		// Token: 0x06001A7E RID: 6782 RVA: 0x0009C938 File Offset: 0x0009AB38
		private void SendStatus(string path, Dictionary<string, string> data)
		{
			try
			{
				if (string.Equals(path, "streamstarted", StringComparison.InvariantCulture))
				{
					BTVManager.Instance.StreamStarted();
				}
				else if (string.Equals(path, "streamstopped", StringComparison.InvariantCulture))
				{
					BTVManager.Instance.StreamStopped();
				}
				else if (string.Equals(path, "recordstarted", StringComparison.InvariantCulture))
				{
					BTVManager.Instance.RecordStarted();
				}
				else if (string.Equals(path, "recordstopped", StringComparison.InvariantCulture))
				{
					BTVManager.Instance.RecordStopped();
					CommonHandlers.sIsRecordingVideo = false;
					if (BlueStacksUIUtils.DictWindows.ContainsKey(CommonHandlers.sRecordingInstance))
					{
						BlueStacksUIUtils.DictWindows[CommonHandlers.sRecordingInstance].mCommonHandler.RecordingStopped();
					}
					CommonHandlers.sRecordingInstance = "";
					this.ShutDownForcefully();
				}
				else if (string.Equals(path, "streamstatus", StringComparison.InvariantCulture))
				{
					if (string.Compare(data["isstreaming"], "true", StringComparison.OrdinalIgnoreCase) == 0)
					{
						BTVManager.Instance.sStreaming = true;
					}
					else
					{
						BTVManager.Instance.sStreaming = false;
					}
				}
				else if (string.Equals(path, "replaybuffersaved", StringComparison.InvariantCulture))
				{
					BTVManager.ReplayBufferSaved();
				}
				else if (string.Equals(path, "RecordStartedVideo", StringComparison.InvariantCulture) && BlueStacksUIUtils.DictWindows.ContainsKey(CommonHandlers.sRecordingInstance))
				{
					BlueStacksUIUtils.DictWindows[CommonHandlers.sRecordingInstance].mCommonHandler.RecordingStarted();
				}
				Logger.Info("Successfully sent status for {0}", new object[] { path });
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send post request for {0}... Err : {1}", new object[]
				{
					path,
					ex.ToString()
				});
			}
		}

		// Token: 0x06001A7F RID: 6783 RVA: 0x0009CAD8 File Offset: 0x0009ACD8
		public void StartRecordForVideo()
		{
			if (this.mIsObsRunning)
			{
				int num;
				int num2;
				int num3;
				int num4;
				StreamManager.SetStreamDimension(out num, out num2, out num3, out num4);
				this.SetConfig(num, num2, num3, num4);
				this.SetSceneConfiguration(this.mLayoutTheme);
				this.ResetCLRBrowser(true);
			}
			this.SendObsRequest("startrecord", null, "RecordStartedVideo", null, 0);
		}

		// Token: 0x06001A80 RID: 6784 RVA: 0x00011B17 File Offset: 0x0000FD17
		public void StartRecord()
		{
			this.StartRecord(this.mNetwork, this.mEnableFilter, this.mSquareTheme, this.mLayoutTheme, this.mCallbackAppInfo);
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x00011B3D File Offset: 0x0000FD3D
		public void StartRecordInit(string network, bool enableFilter, bool squareTheme, string layoutTheme, string callbackAppInfo)
		{
			this.mNetwork = network;
			this.mEnableFilter = enableFilter;
			this.mSquareTheme = squareTheme;
			this.mLayoutTheme = layoutTheme;
			this.mCallbackAppInfo = callbackAppInfo;
		}

		// Token: 0x06001A82 RID: 6786 RVA: 0x0009CB2C File Offset: 0x0009AD2C
		public void StartRecord(string network, bool enableFilter, bool squareTheme, string layoutTheme, string callbackAppInfo)
		{
			object obj = this.stoppingOBSLock;
			lock (obj)
			{
				this.mEnableFilter = enableFilter;
				this.mSquareTheme = squareTheme;
				this.mCallbackAppInfo = callbackAppInfo;
				StreamManager.SendNetworkName(network);
				if (layoutTheme != null)
				{
					this.mLayoutTheme = Utils.GetString(RegistryManager.Instance.LayoutTheme, layoutTheme);
					this.mLastCameraLayoutTheme = RegistryManager.Instance.LastCameraLayoutTheme;
					this.mAppViewLayout = RegistryManager.Instance.AppViewLayout == 1;
					if (string.IsNullOrEmpty(this.mLastCameraLayoutTheme))
					{
						this.mLastCameraLayoutTheme = layoutTheme;
					}
				}
				else
				{
					this.mLayoutTheme = layoutTheme;
				}
				this.mNetwork = network;
				if (this.mIsObsRunning)
				{
					int num;
					int num2;
					int num3;
					int num4;
					StreamManager.SetStreamDimension(out num, out num2, out num3, out num4);
					this.SetConfig(num, num2, num3, num4);
					this.SetSceneConfiguration(this.mLayoutTheme);
				}
				this.EnableVideoRecording(false);
				this.SendObsRequest("startrecord", null, "RecordStarted", null, 0);
			}
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x00011B64 File Offset: 0x0000FD64
		public void StopRecord()
		{
			this.StopRecord(false);
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x0009CC28 File Offset: 0x0009AE28
		public void StopRecord(bool immediate)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { 
			{
				"immediate",
				immediate ? "1" : "0"
			} };
			this.SendObsRequest("stoprecord", dictionary, "RecordStopped", null, 0);
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x0009CC68 File Offset: 0x0009AE68
		public void SendAppInfo(string type, string name, string data)
		{
			if (this.mCallbackAppInfo == null)
			{
				return;
			}
			JObject jobject = new JObject
			{
				{ "type", type },
				{ "name", name },
				{ "data", data }
			};
			object[] array = new object[] { jobject.ToString(Formatting.None, new JsonConverter[0]) };
			this.mBrowser.CallJs(this.mCallbackAppInfo, array);
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x0009CCE0 File Offset: 0x0009AEE0
		public static string GetStreamConfig()
		{
			string streamName = RegistryManager.Instance.StreamName;
			string serverLocation = RegistryManager.Instance.ServerLocation;
			JObject jobject = new JObject
			{
				{ "streamName", streamName },
				{
					"camStatus",
					Convert.ToBoolean(StreamManager.CamStatus, CultureInfo.InvariantCulture)
				},
				{
					"micVolume",
					StreamManager.mMicVolume
				},
				{
					"systemVolume",
					StreamManager.mSystemVolume
				},
				{ "serverLocation", serverLocation }
			};
			Logger.Info("GetStreamConfig: " + jobject.ToString(Formatting.None, new JsonConverter[0]));
			return jobject.ToString();
		}

		// Token: 0x06001A87 RID: 6791 RVA: 0x00011B6D File Offset: 0x0000FD6D
		private void StreamStarted(string _)
		{
			this.SendStatus("streamstarted", null);
			this.mIsStreaming = true;
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x0009CD98 File Offset: 0x0009AF98
		private void StreamStopped(string _)
		{
			this.SendStatus("streamstopped", null);
			this.mIsStreaming = false;
			this.mIsStreamStarted = false;
			this.SendObsRequest("close", null, null, null, 0);
			new Thread(delegate
			{
				this.KillOBS();
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06001A89 RID: 6793 RVA: 0x00011B82 File Offset: 0x0000FD82
		public static void killOBSForcelly()
		{
			Utils.KillCurrentOemProcessByName("HD-OBS", null);
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x0009CDEC File Offset: 0x0009AFEC
		public void KillOBS()
		{
			if (this.mStoppingOBS)
			{
				return;
			}
			object obj = this.stoppingOBSLock;
			lock (obj)
			{
				this.mStoppingOBS = true;
				try
				{
					int num = 0;
					int num2 = 20;
					while (num < num2 && Process.GetProcessesByName("HD-OBS").Length != 0)
					{
						num++;
						if (num < num2)
						{
							Logger.Info("Waiting for HD-OBS to quit gracefully, retry: {0}", new object[] { num });
							Thread.Sleep(200);
						}
					}
					if (num >= num2)
					{
						Utils.KillCurrentOemProcessByName("HD-OBS", null);
					}
					StreamManager.StartOBS();
				}
				catch (Exception ex)
				{
					Logger.Info("Failed to kill HD-OBS.exe...Err : " + ex.ToString());
				}
				this.mStoppingOBS = false;
			}
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x00011B8F File Offset: 0x0000FD8F
		private void RecordStarted(string _)
		{
			this.SendStatus("recordstarted", null);
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x00011B9D File Offset: 0x0000FD9D
		private void RecordStopped(string _)
		{
			this.SendStatus("recordstopped", null);
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x00011BAB File Offset: 0x0000FDAB
		private void RecordStartedVideo(string _)
		{
			this.SendStatus("RecordStartedVideo", null);
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x00011BB9 File Offset: 0x0000FDB9
		public void StartReplayBuffer()
		{
			this.SendObsRequest("startreplaybuffer", null, null, null, 0);
		}

		// Token: 0x06001A8F RID: 6799 RVA: 0x00011BCA File Offset: 0x0000FDCA
		public void StopReplayBuffer()
		{
			this.SendObsRequest("stopreplaybuffer", null, null, null, 2000);
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x0009CEB4 File Offset: 0x0009B0B4
		private void SetStreamSettings(string service, string playPath, string url)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "service", service },
				{ "playPath", playPath },
				{ "url", url }
			};
			this.SendObsRequest("setstreamsettings", dictionary, null, null, 0);
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x0009CEFC File Offset: 0x0009B0FC
		public void SetSystemVolume(string level)
		{
			StreamManager.mSystemVolume = Convert.ToInt32(level, CultureInfo.InvariantCulture);
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "volume", level } };
			this.SendObsRequest("setsystemvolume", dictionary, null, null, 0);
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x0009CF3C File Offset: 0x0009B13C
		internal void SetMic(string micName)
		{
			micName = StreamManager.mDictMicDetails[micName];
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "micId", micName } };
			this.SendObsRequest("setmic", dictionary, null, null, 0);
		}

		// Token: 0x06001A93 RID: 6803 RVA: 0x0009CF78 File Offset: 0x0009B178
		public void SetMicVolume(string level)
		{
			StreamManager.mMicVolume = Convert.ToInt32(level, CultureInfo.InvariantCulture);
			if (StreamManager.mMicVolume > 0)
			{
				StreamManager.mIsMicDisabled = false;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "volume", level } };
			this.SendObsRequest("setmicvolume", dictionary, null, null, 0);
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x0009CFC4 File Offset: 0x0009B1C4
		private void StartPollingOBS()
		{
			while (this.mIsObsRunning)
			{
				try
				{
					JObject jobject = JObject.Parse(Regex.Replace(this.SendObsRequestInternal("getstatus", null), "\\r\\n?|\\n", ""));
					bool flag = jobject["streaming"].ToObject<bool>();
					bool flag2 = jobject["reconnecting"].ToObject<bool>();
					if (!flag)
					{
						try
						{
							this.mFailureReason = jobject["reason"].ToString();
						}
						catch
						{
						}
					}
					if (flag != this.mIsStreaming)
					{
						this.SendStreamStatus(flag, flag2);
					}
					this.mIsStreaming = flag;
					this.mIsReconnecting = flag2;
					Dictionary<string, string> dictionary = new Dictionary<string, string> { 
					{
						"isstreaming",
						this.mIsStreaming.ToString(CultureInfo.InvariantCulture)
					} };
					this.SendStatus("streamstatus", dictionary);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in StartPollingOBS err : " + ex.ToString());
					if (!ProcessUtils.FindProcessByName("HD-OBS"))
					{
						this.mIsObsRunning = false;
						this.mIsStreaming = false;
						this.mIsReconnecting = false;
						this.mCLRBrowserRunning = false;
						this.mIsStreamStarted = false;
						if (!this.mStoppingOBS)
						{
							this.UpdateFailureReason();
						}
						this.SendStreamStatus(false, false);
						this.InitObs();
						this.mStoppingOBS = false;
						break;
					}
				}
				Thread.Sleep(5000);
			}
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x0009D12C File Offset: 0x0009B32C
		private void UpdateFailureReason()
		{
			if (string.IsNullOrEmpty(this.mFailureReason))
			{
				string text = "";
				string text2 = "yyyy-MM-dd-HHmm-ss";
				DateTime dateTime = DateTime.MinValue;
				string[] files = Directory.GetFiles(Path.Combine(RegistryStrings.BtvDir, "OBS\\Logs\\"));
				for (int i = 0; i < files.Length; i++)
				{
					text = Path.GetFileNameWithoutExtension(files[i]);
					DateTime dateTime2;
					if (DateTime.TryParseExact(text, text2, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime2) && dateTime < dateTime2)
					{
						dateTime = dateTime2;
					}
				}
				if (!dateTime.Equals(DateTime.MinValue))
				{
					text = File.ReadAllLines(Path.Combine(RegistryStrings.BtvDir, "OBS\\Logs\\") + dateTime.ToString("yyyy-MM-dd-HHmm-ss", CultureInfo.InvariantCulture) + ".log").Last<string>();
				}
				this.mFailureReason = "OBS crashed: " + text;
			}
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x0009D200 File Offset: 0x0009B400
		private static void SendNetworkName(string network)
		{
			try
			{
				BTVManager.sNetwork = network;
				RegistryManager.Instance.BtvNetwork = network;
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send network name... Err : " + ex.ToString());
			}
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x0009D248 File Offset: 0x0009B448
		private void SendStreamStatus(bool streaming, bool reconnecting)
		{
			Logger.Info(string.Concat(new string[]
			{
				"Sending stream status with data :: streaming : ",
				streaming.ToString(),
				", reconnecting : ",
				reconnecting.ToString(),
				", obsRunning : ",
				this.mIsObsRunning.ToString(),
				", failureReason : ",
				this.mFailureReason
			}));
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("obs", this.mIsObsRunning.ToString(CultureInfo.InvariantCulture));
				dictionary.Add("streaming", streaming.ToString(CultureInfo.InvariantCulture));
				dictionary.Add("reconnecting", reconnecting.ToString(CultureInfo.InvariantCulture));
				dictionary.Add("reason", this.mFailureReason.ToString(CultureInfo.InvariantCulture));
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send stream status... Err : " + ex.ToString());
			}
		}

		// Token: 0x06001A98 RID: 6808 RVA: 0x0009D34C File Offset: 0x0009B54C
		public void ResizeStream(string width, string height)
		{
			if (StreamManager.mObsCommandQueue != null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "width", width },
					{ "height", height }
				};
				this.SendObsRequest("windowresized", dictionary, null, null, 0);
			}
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x00011BDF File Offset: 0x0000FDDF
		public void ShowObs()
		{
			this.SendObsRequest("show", null, null, null, 0);
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x00011BF0 File Offset: 0x0000FDF0
		public void HideObs()
		{
			this.SendObsRequest("hide", null, null, null, 0);
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x0009D390 File Offset: 0x0009B590
		public void MoveWebcam(string horizontal, string vertical)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "horizontal", horizontal },
				{ "vertical", vertical }
			};
			this.SendObsRequest("movewebcam", dictionary, null, null, 0);
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x00011C01 File Offset: 0x0000FE01
		public void Shutdown()
		{
			if (this.mIsStreaming)
			{
				this.StopStream();
			}
			if (StreamManager.mObsCommandQueue != null)
			{
				this.mIsObsRunning = false;
				this.mIsStreamStarted = false;
				this.SendObsRequest("close", null, "CloseSuccess", "CloseFailed", 0);
			}
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x00011C3D File Offset: 0x0000FE3D
		public static void CloseSuccess(string _)
		{
			StreamManager.Instance = null;
		}

		// Token: 0x06001A9E RID: 6814 RVA: 0x00011C45 File Offset: 0x0000FE45
		public static void CloseFailed()
		{
			Utils.KillCurrentOemProcessByName("HD-OBS", null);
			StreamManager.Instance = null;
		}

		// Token: 0x06001A9F RID: 6815 RVA: 0x0009D3CC File Offset: 0x0009B5CC
		public static void StopOBS()
		{
			StreamManager.Instance.mStoppingOBS = true;
			StreamManager.sStopInitOBSQueue = true;
			StreamManager.Instance.Shutdown();
			int num = 0;
			while (Process.GetProcessesByName("HD-OBS").Length != 0 && num < 20)
			{
				Thread.Sleep(500);
				num++;
			}
			if (num == 20)
			{
				Logger.Info("Killing hd-obs as normal close failed");
				StreamManager.CloseFailed();
			}
		}

		// Token: 0x06001AA0 RID: 6816 RVA: 0x00011C58 File Offset: 0x0000FE58
		public void SaveReplayBuffer()
		{
			this.SendObsRequest("savereplaybuffer", null, null, null, 0);
		}

		// Token: 0x06001AA1 RID: 6817 RVA: 0x00011C69 File Offset: 0x0000FE69
		public void ReplayBufferSaved()
		{
			this.SendStatus("replaybuffersaved", null);
		}

		// Token: 0x06001AA2 RID: 6818 RVA: 0x00011C77 File Offset: 0x0000FE77
		public void SendObsRequest(string request, Dictionary<string, string> data, string responseCallback, string failureCallback, int pauseTime)
		{
			this.SendObsRequest(request, data, responseCallback, failureCallback, pauseTime, true);
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x0009D42C File Offset: 0x0009B62C
		public void SendObsRequest(string request, Dictionary<string, string> data, string responseCallback, string failureCallback, int pauseTime, bool waitForInit)
		{
			Logger.Info("got obs request: " + request);
			if (data != null && !data.ContainsKey("randomVal"))
			{
				data.Add("randomVal", "0");
			}
			new Thread(delegate
			{
				StreamManager.ObsCommand obsCommand = new StreamManager.ObsCommand(request, data, responseCallback, failureCallback, pauseTime);
				object obj = this.mInitOBSLock;
				lock (obj)
				{
					if (!this.mIsInitCalled)
					{
						this.InitObs();
					}
				}
				if (StreamManager.mObsCommandQueue == null)
				{
					StreamManager.mObsCommandQueue = new Queue<StreamManager.ObsCommand>();
				}
				if (waitForInit)
				{
					obj = this.mInitOBSLock;
					lock (obj)
					{
						object obj2 = this.mObsCommandQueueObject;
						lock (obj2)
						{
							StreamManager.mObsCommandQueue.Enqueue(obsCommand);
							this.mObsCommandEventHandle.Set();
							return;
						}
					}
				}
				obj = this.mObsCommandQueueObject;
				lock (obj)
				{
					StreamManager.mObsCommandQueue.Enqueue(obsCommand);
					this.mObsCommandEventHandle.Set();
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x0009D4D4 File Offset: 0x0009B6D4
		private string SendObsRequestInternal(string request, Dictionary<string, string> data)
		{
			Logger.Info("waiting to send request: " + request);
			object obj = this.mObsSendRequestObject;
			string text2;
			lock (obj)
			{
				string text = string.Empty;
				if (this.mIsObsRunning)
				{
					text = BstHttpClient.Post(string.Format(CultureInfo.InvariantCulture, "{0}:{1}/{2}", new object[]
					{
						StreamManager.ObsServerBaseURL,
						StreamManager.ObsServerPort,
						request
					}), data, null, false, "Android", 0, 1, 0, false, "bgp64");
				}
				text2 = text;
			}
			return text2;
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x0009D570 File Offset: 0x0009B770
		private void ProcessObsCommandQueue()
		{
			while (this.mIsObsRunning)
			{
				this.mObsCommandEventHandle.WaitOne();
				while (StreamManager.mObsCommandQueue.Count != 0)
				{
					object obj = this.mObsCommandQueueObject;
					StreamManager.ObsCommand obsCommand;
					lock (obj)
					{
						if (StreamManager.mObsCommandQueue.Count == 0)
						{
							break;
						}
						obsCommand = StreamManager.mObsCommandQueue.Dequeue();
					}
					string text = string.Empty;
					try
					{
						text = this.SendObsRequestInternal(obsCommand.mRequest, obsCommand.mData);
						Logger.Info("Got response {0} for {1}", new object[] { text, obsCommand.mRequest });
						if (obsCommand.mResponseCallback != null)
						{
							base.GetType().GetMethod(obsCommand.mResponseCallback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(this, new object[] { text });
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Exception when sending " + obsCommand.mRequest);
						Logger.Error(ex.ToString());
						try
						{
							if (obsCommand.mFailureCallback != null)
							{
								base.GetType().GetMethod(obsCommand.mFailureCallback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(this, new object[0]);
							}
						}
						catch (Exception ex2)
						{
							Logger.Error("Error in failure call back for call {} and error {1}", new object[] { obsCommand.mFailureCallback, ex2 });
						}
					}
					Thread.Sleep(obsCommand.mPauseTime);
				}
			}
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x0009D6E4 File Offset: 0x0009B8E4
		public void Init(string appHandle, string pid)
		{
			Logger.Info("App Handle : {0} and Process Id : {1}", new object[] { appHandle, pid });
			if (string.IsNullOrEmpty(this.mAppHandle) && string.IsNullOrEmpty(this.mAppPid))
			{
				this.mAppHandle = appHandle;
				this.mAppPid = pid;
			}
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x00011C87 File Offset: 0x0000FE87
		private double GetWidthFromHeight(double height)
		{
			return (height - (double)this.heightDiff) * this.mAspectRatio.DoubleValue + (double)this.widthDiff;
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x0009D734 File Offset: 0x0009B934
		public static void GetStreamConfig(out string handle, out string pid)
		{
			try
			{
				MainWindow activatedWindow = null;
				if (BlueStacksUIUtils.DictWindows.Count > 0)
				{
					activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
				}
				handle = activatedWindow.mFrontendHandler.mFrontendHandle.ToString();
				activatedWindow.Dispatcher.Invoke(new Action(delegate
				{
					activatedWindow.RestrictWindowResize(true);
				}), new object[0]);
				Process currentProcess = Process.GetCurrentProcess();
				pid = currentProcess.Id.ToString(CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to get window handle and process id... Err : " + ex.ToString());
				string text;
				pid = (text = null);
				handle = text;
			}
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x0009D7FC File Offset: 0x0009B9FC
		public void ShutDownForcefully()
		{
			try
			{
				StreamManager.killOBSForcelly();
				this.mIsObsRunning = false;
				StreamManager.Instance.mStoppingOBS = true;
				StreamManager.mObsCommandQueue.Clear();
				StreamManager.Instance = null;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in shutdown obs : {0}", new object[] { ex });
			}
		}

		// Token: 0x06001AAA RID: 6826 RVA: 0x00011CA6 File Offset: 0x0000FEA6
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				Browser browser = this.mBrowser;
				if (browser != null)
				{
					browser.Dispose();
				}
				EventWaitHandle eventWaitHandle = this.mObsCommandEventHandle;
				if (eventWaitHandle != null)
				{
					eventWaitHandle.Close();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06001AAB RID: 6827 RVA: 0x0009D85C File Offset: 0x0009BA5C
		~StreamManager()
		{
			this.Dispose(false);
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x00011CDB File Offset: 0x0000FEDB
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04001072 RID: 4210
		private static Queue<StreamManager.ObsCommand> mObsCommandQueue;

		// Token: 0x04001073 RID: 4211
		private object mObsCommandQueueObject = new object();

		// Token: 0x04001074 RID: 4212
		private object mObsSendRequestObject = new object();

		// Token: 0x04001075 RID: 4213
		private object mInitOBSLock = new object();

		// Token: 0x04001076 RID: 4214
		private EventWaitHandle mObsCommandEventHandle;

		// Token: 0x0400107F RID: 4223
		private bool mIsStreamStarted;

		// Token: 0x04001080 RID: 4224
		private string mFailureReason = "";

		// Token: 0x04001081 RID: 4225
		private static int mMicVolume;

		// Token: 0x04001082 RID: 4226
		internal static string mSelectedCamera;

		// Token: 0x04001083 RID: 4227
		internal static string mSelectedMic = string.Empty;

		// Token: 0x04001084 RID: 4228
		internal static bool mIsWebcamDisabled = true;

		// Token: 0x04001085 RID: 4229
		internal static bool mIsMicDisabled = true;

		// Token: 0x04001086 RID: 4230
		internal static Dictionary<string, string> mDictCameraDetails = new Dictionary<string, string>();

		// Token: 0x04001087 RID: 4231
		internal static Dictionary<string, string> mDictMicDetails = new Dictionary<string, string>();

		// Token: 0x04001088 RID: 4232
		internal static Dictionary<string, string> mDictLastCameraPosition = new Dictionary<string, string>();

		// Token: 0x0400108F RID: 4239
		private static int mSystemVolume;

		// Token: 0x04001092 RID: 4242
		private string mAppHandle = "";

		// Token: 0x04001093 RID: 4243
		private string mAppPid = "";

		// Token: 0x04001094 RID: 4244
		internal int mStreamWidth;

		// Token: 0x04001095 RID: 4245
		internal int mStreamHeight;

		// Token: 0x04001098 RID: 4248
		private object stoppingOBSLock = new object();

		// Token: 0x04001099 RID: 4249
		private Browser mBrowser;

		// Token: 0x0400109F RID: 4255
		private int heightDiff;

		// Token: 0x040010A0 RID: 4256
		private int widthDiff = 14;

		// Token: 0x040010A1 RID: 4257
		internal Fraction mAspectRatio = new Fraction(16L, 9L);

		// Token: 0x040010A2 RID: 4258
		private MainWindow mWindow;

		// Token: 0x040010A4 RID: 4260
		private string mLastVideoFilePath;

		// Token: 0x040010A5 RID: 4261
		private int mObsRetryCount = 2;

		// Token: 0x040010A6 RID: 4262
		private bool disposedValue;

		// Token: 0x020002CA RID: 714
		private class ObsCommand
		{
			// Token: 0x06001AB0 RID: 6832 RVA: 0x00011CFA File Offset: 0x0000FEFA
			public ObsCommand(string request, Dictionary<string, string> data, string responseCallback, string failureCallback, int pauseTime)
			{
				this.mRequest = request;
				this.mData = data;
				this.mResponseCallback = responseCallback;
				this.mFailureCallback = failureCallback;
				this.mPauseTime = pauseTime;
			}

			// Token: 0x040010A7 RID: 4263
			public string mRequest;

			// Token: 0x040010A8 RID: 4264
			public Dictionary<string, string> mData;

			// Token: 0x040010A9 RID: 4265
			public string mResponseCallback;

			// Token: 0x040010AA RID: 4266
			public string mFailureCallback;

			// Token: 0x040010AB RID: 4267
			public int mPauseTime;
		}
	}
}
