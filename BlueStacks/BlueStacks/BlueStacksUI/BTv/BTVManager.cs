using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI.BTv
{
	// Token: 0x020002BD RID: 701
	internal sealed class BTVManager : IDisposable
	{
		// Token: 0x060019C1 RID: 6593 RVA: 0x00011551 File Offset: 0x0000F751
		private BTVManager()
		{
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x060019C2 RID: 6594 RVA: 0x00099498 File Offset: 0x00097698
		public static BTVManager Instance
		{
			get
			{
				if (BTVManager.instance == null)
				{
					object obj = BTVManager.syncRoot;
					lock (obj)
					{
						if (BTVManager.instance == null)
						{
							BTVManager.instance = new BTVManager();
						}
					}
				}
				return BTVManager.instance;
			}
		}

		// Token: 0x1700038F RID: 911
		// (set) Token: 0x060019C3 RID: 6595 RVA: 0x00011564 File Offset: 0x0000F764
		public static bool sWritingToFile
		{
			set
			{
				HTTPServer.FileWriteComplete = !value;
			}
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x000994F0 File Offset: 0x000976F0
		public void StartBlueStacksTV()
		{
			using (Process process = new Process())
			{
				string installDir = RegistryStrings.InstallDir;
				process.StartInfo.FileName = Path.Combine(installDir, "BlueStacksTV.exe");
				process.StartInfo.Arguments = "-u";
				process.Start();
				Thread.Sleep(1000);
				new Thread(new ThreadStart(this.StartPingBTVThread))
				{
					IsBackground = true
				}.Start();
			}
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x00004786 File Offset: 0x00002986
		private void BtvWindow_Closing(object sender, CancelEventArgs e)
		{
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0009957C File Offset: 0x0009777C
		internal static void BringToFront(CustomWindow win)
		{
			try
			{
				win.Dispatcher.Invoke(new Action(delegate
				{
					if (win.WindowState == WindowState.Minimized)
					{
						win.WindowState = WindowState.Normal;
					}
					win.Visibility = Visibility.Visible;
					win.Show();
					win.BringIntoView();
					if (!win.Topmost)
					{
						win.Topmost = true;
						win.Topmost = false;
					}
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("An error was triggered in bringing BTv downloader to front", new object[] { ex.Message });
			}
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x000995E8 File Offset: 0x000977E8
		public static void ReportObsErrorHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			Logger.Info("Got ReportObsErrorHandler");
			HTTPUtils.ParseRequest(req);
			try
			{
				StreamManager.Instance.ReportObsError("obs_error");
				StreamManager.Instance = null;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ReportObsHandler");
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x0001156F File Offset: 0x0000F76F
		private void CancelBTvDownload(object sender, EventArgs e)
		{
			Logger.Info("User cancelled BTV download");
			this.sDownloading = false;
			if (this.sDownloader != null)
			{
				this.sDownloader.AbortDownload();
				if (BTVManager.IsBTVInstalled())
				{
					Directory.Delete(RegistryStrings.ObsDir, true);
				}
			}
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x00099644 File Offset: 0x00097844
		private void CancelDownloadConfirmation(object sender, EventArgs e)
		{
			MainWindow mainWindow = null;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
			}
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DOWNLOAD_IN_PROGRESS", "");
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_BTV_DOWNLOAD_CANCEL", "");
			customMessageWindow.AddButton(ButtonColors.Red, "STRING_CANCEL", new EventHandler(this.CancelBTvDownload), null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CONTINUE", null, null, false, null);
			customMessageWindow.Owner = mainWindow;
			customMessageWindow.ShowDialog();
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x000115A7 File Offset: 0x0000F7A7
		internal static bool IsBTVInstalled()
		{
			return Directory.Exists(RegistryStrings.BtvDir) && Directory.Exists(RegistryStrings.ObsDir);
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x000996D8 File Offset: 0x000978D8
		internal static bool IsDirectXComponentsInstalled()
		{
			string systemDirectory = Environment.SystemDirectory;
			foreach (string text in new string[] { "D3DX10_43.DLL", "D3D10_1.DLL", "DXGI.DLL", "D3DCompiler_43.dll" })
			{
				if (!File.Exists(Path.Combine(systemDirectory, text)))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x00099738 File Offset: 0x00097938
		public void MaybeDownloadAndLaunchBTv(MainWindow parentWindow)
		{
			BTVManager.<>c__DisplayClass25_0 CS$<>8__locals1 = new BTVManager.<>c__DisplayClass25_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.parentWindow = parentWindow;
			if (BTVManager.IsBTVInstalled())
			{
				this.StartBlueStacksTV();
				return;
			}
			if (this.sDownloading && this.sWindow != null)
			{
				BTVManager.BringToFront(this.sWindow);
				return;
			}
			ExtensionPopupControl btvExtPopup = new ExtensionPopupControl();
			btvExtPopup.LoadExtensionPopupFromFolder("BTVExtensionPopup");
			btvExtPopup.DownloadClicked += delegate(object o, EventArgs e)
			{
				BlueStacksUIUtils.CloseContainerWindow(btvExtPopup);
				CS$<>8__locals1.<>4__this.sDownloading = true;
				CS$<>8__locals1.<>4__this.sWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(CS$<>8__locals1.<>4__this.sWindow.TitleTextBlock, "STRING_BTV_DOWNLOAD", "");
				BlueStacksUIBinding.Bind(CS$<>8__locals1.<>4__this.sWindow.BodyTextBlock, "STRING_BTV_INSTALL_WAIT", "");
				BlueStacksUIBinding.Bind(CS$<>8__locals1.<>4__this.sWindow.BodyWarningTextBlock, "STRING_BTV_WARNING", "");
				CS$<>8__locals1.<>4__this.sWindow.AddButton(ButtonColors.Blue, "STRING_CANCEL", new EventHandler(CS$<>8__locals1.<>4__this.CancelDownloadConfirmation), null, false, null);
				CS$<>8__locals1.<>4__this.sWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
				CS$<>8__locals1.<>4__this.sWindow.ProgressBarEnabled = true;
				CS$<>8__locals1.<>4__this.sWindow.IsWindowMinizable = true;
				CS$<>8__locals1.<>4__this.sWindow.IsWindowClosable = false;
				CS$<>8__locals1.<>4__this.sWindow.ImageName = "BTVTopBar";
				CS$<>8__locals1.<>4__this.sWindow.ShowInTaskbar = true;
				CS$<>8__locals1.<>4__this.sWindow.Owner = CS$<>8__locals1.parentWindow;
				CS$<>8__locals1.<>4__this.sWindow.IsShowGLWindow = true;
				CS$<>8__locals1.<>4__this.sWindow.Show();
				ThreadStart threadStart;
				if ((threadStart = CS$<>8__locals1.<>9__1) == null)
				{
					threadStart = (CS$<>8__locals1.<>9__1 = delegate
					{
						if (!string.IsNullOrEmpty(RegistryManager.Instance.BtvDevServer))
						{
							BTVManager.sBTvUrl = RegistryManager.Instance.BtvDevServer;
						}
						string redirectedUrl = BTVManager.GetRedirectedUrl(BTVManager.sBTvUrl);
						if (redirectedUrl == null)
						{
							Logger.Error("The download url was null");
							return;
						}
						string fileName = Path.GetFileName(new Uri(redirectedUrl).LocalPath);
						string downloadPath = Path.Combine(Path.GetTempPath(), fileName);
						CS$<>8__locals1.<>4__this.sDownloader = new LegacyDownloader(3, redirectedUrl, downloadPath);
						LegacyDownloader legacyDownloader = CS$<>8__locals1.<>4__this.sDownloader;
						LegacyDownloader.UpdateProgressCallback updateProgressCallback;
						if ((updateProgressCallback = CS$<>8__locals1.<>9__2) == null)
						{
							updateProgressCallback = (CS$<>8__locals1.<>9__2 = delegate(int percent)
							{
								CS$<>8__locals1.<>4__this.sWindow.Dispatcher.Invoke(new Action(delegate
								{
									CS$<>8__locals1.<>4__this.sWindow.CustomProgressBar.Value = (double)percent;
								}), new object[0]);
							});
						}
						legacyDownloader.Download(updateProgressCallback, delegate(string filePath)
						{
							Dispatcher dispatcher = CS$<>8__locals1.<>4__this.sWindow.Dispatcher;
							Action action;
							if ((action = CS$<>8__locals1.<>9__6) == null)
							{
								action = (CS$<>8__locals1.<>9__6 = delegate
								{
									CS$<>8__locals1.<>4__this.sWindow.CustomProgressBar.Value = 100.0;
									CS$<>8__locals1.<>4__this.sWindow.Close();
								});
							}
							dispatcher.Invoke(action, new object[0]);
							Logger.Info("Successfully downloaded BlueStacks TV");
							CS$<>8__locals1.<>4__this.sDownloading = false;
							BTVManager.ExtractBTv(downloadPath);
							Dispatcher dispatcher2 = CS$<>8__locals1.parentWindow.Dispatcher;
							Action action2;
							if ((action2 = CS$<>8__locals1.<>9__7) == null)
							{
								action2 = (CS$<>8__locals1.<>9__7 = delegate
								{
									CS$<>8__locals1.parentWindow.mTopBar.mBtvButton.ImageName = "btv";
								});
							}
							dispatcher2.Invoke(action2, new object[0]);
						}, delegate(Exception ex)
						{
							Logger.Error("Failed to download file: {0}. err: {1}", new object[] { downloadPath, ex.Message });
						}, null, null, null);
					});
				}
				new Thread(threadStart)
				{
					IsBackground = true
				}.Start();
			};
			btvExtPopup.Height = CS$<>8__locals1.parentWindow.ActualHeight * 0.8;
			btvExtPopup.Width = btvExtPopup.Height * 16.0 / 9.0;
			new ContainerWindow(CS$<>8__locals1.parentWindow, btvExtPopup, (double)((int)btvExtPopup.Width), (double)((int)btvExtPopup.Height), false, true, false, -1.0, null);
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x00099854 File Offset: 0x00097A54
		internal static void ReportOpenGLCaptureError(HttpListenerRequest req, HttpListenerResponse res)
		{
			Logger.Info("Got open gl CaptureError");
			try
			{
				StreamManager.Instance.ReportObsError("opengl_capture_error");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ReportObsHandler");
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x000998A4 File Offset: 0x00097AA4
		internal static void ReportCaptureError(HttpListenerRequest req, HttpListenerResponse res)
		{
			Logger.Info("Got ReportCaptureError");
			HTTPUtils.ParseRequest(req);
			try
			{
				StreamManager.Instance.ReportObsError("capture_error");
				StreamManager.Instance = null;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ReportObsHandler");
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x00099900 File Offset: 0x00097B00
		internal static void ObsStatusHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			Logger.Info("Got ObsStatus {0} request from {1}", new object[]
			{
				req.HttpMethod,
				req.RemoteEndPoint.ToString()
			});
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (requestData.Data.Count > 0 && requestData.Data.AllKeys[0] == "Error")
				{
					if (!StreamManager.sStopInitOBSQueue)
					{
						if (string.Equals(requestData.Data[0], "OBSAlreadyRunning", StringComparison.InvariantCulture))
						{
							StreamManager.sStopInitOBSQueue = true;
						}
						if (StreamManager.Instance != null)
						{
							new Thread(delegate
							{
								StreamManager.Instance.ReportObsError(requestData.Data[0]);
							})
							{
								IsBackground = true
							}.Start();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ObsStatus");
				Logger.Error(ex.ToString());
			}
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x000999F0 File Offset: 0x00097BF0
		internal static string GetRedirectedUrl(string url)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			httpWebRequest.AllowAutoRedirect = true;
			string text = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + text;
			httpWebRequest.Headers.Add("x_oem", RegistryManager.Instance.Oem);
			httpWebRequest.Headers.Set("x_email", RegistryManager.Instance.RegisteredEmail);
			httpWebRequest.Headers.Add("x_guid", RegistryManager.Instance.UserGuid);
			httpWebRequest.Headers.Add("x_prod_ver", RegistryManager.Instance.Version);
			httpWebRequest.Headers.Add("x_home_app_ver", RegistryManager.Instance.ClientVersion);
			string text2;
			try
			{
				using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
				{
					using (Stream responseStream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
						{
							JObject jobject = JObject.Parse(streamReader.ReadToEnd());
							if (jobject["success"].ToObject<bool>())
							{
								text2 = jobject["file_url"].ToString();
							}
							else
							{
								text2 = null;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in getting redirected url for BTV " + ex.ToString());
				text2 = null;
			}
			return text2;
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x00099B94 File Offset: 0x00097D94
		internal static bool ExtractBTv(string downloadPath)
		{
			try
			{
				if (File.Exists(downloadPath))
				{
					if (MiscUtils.Extract7Zip(downloadPath, RegistryManager.Instance.UserDefinedDir) == 0)
					{
						return true;
					}
					Logger.Error("Could not extract BTv zip file.");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Could not extract BTv zip file. Error: " + ex.ToString());
			}
			return false;
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x00099BF8 File Offset: 0x00097DF8
		public void StartPingBTVThread()
		{
			object obj = this.sPingBTVLock;
			lock (obj)
			{
				Logger.Info("Starting btv ping thread");
				for (;;)
				{
					this.PingBTV();
					if (this.sStopPingBTVThread)
					{
						break;
					}
					Thread.Sleep(5000);
				}
			}
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x000115C4 File Offset: 0x0000F7C4
		public void ShowStreamWindow()
		{
			if (!ProcessUtils.FindProcessByName("BlueStacksTV"))
			{
				this.StartBlueStacksTV();
				return;
			}
			BTVManager.SendBTVAsyncRequest("showstreamwindow", null);
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x000115E4 File Offset: 0x0000F7E4
		public void HideStreamWindow()
		{
			if (ProcessUtils.FindProcessByName("BlueStacksTV"))
			{
				BTVManager.SendBTVAsyncRequest("hidestreamwindow", null);
			}
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x000115FD File Offset: 0x0000F7FD
		public void HideStreamWindowFromTaskbar()
		{
			BTVManager.SendBTVAsyncRequest("hidestreamwindowfromtaskbar", null);
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x00099C50 File Offset: 0x00097E50
		public static void GetStreamDimensionInfo(out int startX, out int startY, out int width, out int height)
		{
			Point p = default(Point);
			MainWindow activatedWindow = null;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
			}
			activatedWindow.Dispatcher.Invoke(new Action(delegate
			{
				p = activatedWindow.mFrontendGrid.TranslatePoint(new Point(0.0, 0.0), activatedWindow.mFrontendGrid);
			}), new object[0]);
			startX = Convert.ToInt32(p.X) * SystemUtils.GetDPI() / 96;
			startY = Convert.ToInt32(p.Y) * SystemUtils.GetDPI() / 96;
			width = (int)activatedWindow.mFrontendGrid.ActualWidth * SystemUtils.GetDPI() / 96;
			height = (int)activatedWindow.mFrontendGrid.ActualHeight * SystemUtils.GetDPI() / 96;
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x00099D2C File Offset: 0x00097F2C
		public void PingBTV()
		{
			bool flag = false;
			bool flag2 = false;
			try
			{
				string text = BTVManager.SendBTVRequest("ping", null);
				JArray.Parse(text);
				JObject jobject = JObject.Parse(text[0].ToString(CultureInfo.InvariantCulture));
				if (jobject["success"].ToObject<bool>())
				{
					flag = jobject["recording"].ToObject<bool>();
					flag2 = jobject["streaming"].ToObject<bool>();
				}
				Logger.Info("Ping BTV response recording: {0}, streaming: {1}", new object[] { flag, flag2 });
				this.sStopPingBTVThread = false;
			}
			catch (Exception ex)
			{
				this.sStopPingBTVThread = true;
				Logger.Error("PingBTV : {0}", new object[] { ex.Message });
			}
			this.sRecording = flag;
			this.sStreaming = flag2;
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x00099E0C File Offset: 0x0009800C
		public void SetFrontendPosition(int width, int height, bool isPortrait)
		{
			if (ProcessUtils.FindProcessByName("BlueStacksTV"))
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
						"isPortrait",
						isPortrait.ToString(CultureInfo.InvariantCulture)
					}
				};
				BTVManager.SendBTVRequest("setfrontendposition", dictionary);
			}
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x00099E7C File Offset: 0x0009807C
		public void WindowResized()
		{
			if (ProcessUtils.FindProcessByName("BlueStacksTV"))
			{
				try
				{
					BTVManager.SendBTVRequest("windowresized", null);
				}
				catch (Exception ex)
				{
					Logger.Error("{0}", new object[] { ex });
				}
			}
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x0001160A File Offset: 0x0000F80A
		public void StreamStarted()
		{
			BTVManager.sWritingToFile = true;
			this.sRecording = true;
			this.sStreaming = true;
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x00011620 File Offset: 0x0000F820
		public void StreamStopped()
		{
			BTVManager.sWritingToFile = false;
			this.sStreaming = false;
			this.sRecording = false;
			BTVManager.RestrictWindowResize(false);
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x0001163C File Offset: 0x0000F83C
		public void RecordStarted()
		{
			BTVManager.sWritingToFile = true;
			this.sRecording = true;
			this.sWasRecording = true;
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x00099ECC File Offset: 0x000980CC
		public void SetConfig()
		{
			int num;
			int num2;
			int num3;
			int num4;
			BTVManager.GetStreamDimensionInfo(out num, out num2, out num3, out num4);
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"startX",
					num.ToString(CultureInfo.InvariantCulture)
				},
				{
					"startY",
					num2.ToString(CultureInfo.InvariantCulture)
				},
				{
					"width",
					num3.ToString(CultureInfo.InvariantCulture)
				},
				{
					"height",
					num4.ToString(CultureInfo.InvariantCulture)
				}
			};
			BTVManager.SendBTVRequest("setconfig", dictionary);
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x00011652 File Offset: 0x0000F852
		public void RecordStopped()
		{
			BTVManager.sWritingToFile = false;
			this.sRecording = false;
			BTVManager.RestrictWindowResize(false);
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x00011667 File Offset: 0x0000F867
		public void SendTabChangeData(string[] tabChangedData)
		{
			new Thread(delegate
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"type",
						tabChangedData[0]
					},
					{
						"name",
						tabChangedData[1]
					},
					{
						"data",
						tabChangedData[2]
					}
				};
				BTVManager.SendBTVRequest("tabchangeddata", dictionary);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x060019E0 RID: 6624 RVA: 0x00099F58 File Offset: 0x00098158
		public static void ReplayBufferSaved()
		{
			MainWindow mainWindow = null;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
			}
			mainWindow.Dispatcher.Invoke(new Action(delegate
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "Flash Video (*.flv)|*.flv",
					FilterIndex = 1,
					RestoreDirectory = true,
					FileName = "Replay"
				};
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					string fileName = saveFileDialog.FileName;
					string text = "replay.flv";
					File.Copy(Path.Combine(RegistryManager.Instance.ClientInstallDir, text), fileName);
				}
			}), new object[0]);
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x00011691 File Offset: 0x0000F891
		public void Stop()
		{
			if (this.sStreaming || this.sRecording)
			{
				BTVManager.SendBTVRequest("sessionswitch", null);
				this.sWasRecording = false;
			}
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x000116B6 File Offset: 0x0000F8B6
		public void CloseBTV()
		{
			this.sWasRecording = false;
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x000116BF File Offset: 0x0000F8BF
		public void CheckNewFiltersAvailable()
		{
			BTVManager.SendBTVRequest("checknewfilters", null);
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x000116CD File Offset: 0x0000F8CD
		public static void SendBTVAsyncRequest(string request, Dictionary<string, string> data)
		{
			new Thread(delegate
			{
				Logger.Info("Sending btv async request");
				BTVManager.SendBTVRequest(request, data);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x000116FE File Offset: 0x0000F8FE
		public static string SendBTVRequest(string _1, Dictionary<string, string> _2)
		{
			return "";
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x00099FB8 File Offset: 0x000981B8
		public static void RestrictWindowResize(bool enable)
		{
			MainWindow activatedWindow = null;
			if (BlueStacksUIUtils.DictWindows.Count > 0)
			{
				activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
			}
			activatedWindow.Dispatcher.Invoke(new Action(delegate
			{
				activatedWindow.RestrictWindowResize(enable);
			}), new object[0]);
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x0009A020 File Offset: 0x00098220
		public void RecordVideoOfApp()
		{
			if (StreamManager.Instance == null)
			{
				StreamManager.Instance = new StreamManager(BlueStacksUIUtils.DictWindows.Values.First<MainWindow>());
			}
			string text;
			string text2;
			StreamManager.GetStreamConfig(out text, out text2);
			StreamManager.Instance.Init(text, text2);
			StreamManager.Instance.SetHwnd(text);
			StreamManager.Instance.EnableVideoRecording(true);
			StreamManager.Instance.StartObs();
			StreamManager.Instance.StartRecordForVideo();
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x00011705 File Offset: 0x0000F905
		private void StopRecordVideo()
		{
			StreamManager.Instance.StopRecord();
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x00004786 File Offset: 0x00002986
		internal void RecordStartedVideo()
		{
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x00011711 File Offset: 0x0000F911
		public void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				this.disposedValue = true;
			}
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x0009A08C File Offset: 0x0009828C
		~BTVManager()
		{
			this.Dispose(false);
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x00011724 File Offset: 0x0000F924
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04001048 RID: 4168
		private static volatile BTVManager instance;

		// Token: 0x04001049 RID: 4169
		private static object syncRoot = new object();

		// Token: 0x0400104A RID: 4170
		public bool sStreaming;

		// Token: 0x0400104B RID: 4171
		public static string sNetwork = "twitch";

		// Token: 0x0400104C RID: 4172
		public bool sRecording;

		// Token: 0x0400104D RID: 4173
		public bool sWasRecording;

		// Token: 0x0400104E RID: 4174
		public bool sStopPingBTVThread;

		// Token: 0x0400104F RID: 4175
		public object sPingBTVLock = new object();

		// Token: 0x04001050 RID: 4176
		private CustomMessageWindow sWindow;

		// Token: 0x04001051 RID: 4177
		private bool sDownloading;

		// Token: 0x04001052 RID: 4178
		private LegacyDownloader sDownloader;

		// Token: 0x04001053 RID: 4179
		private static string sBTvUrl = "https://cloud.bluestacks.com/bs4/btv/GetBTVFile";

		// Token: 0x04001054 RID: 4180
		private bool disposedValue;
	}
}
