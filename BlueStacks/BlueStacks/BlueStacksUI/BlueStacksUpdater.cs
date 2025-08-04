using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000195 RID: 405
	internal class BlueStacksUpdater
	{
		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000FE8 RID: 4072 RVA: 0x0000B8E4 File Offset: 0x00009AE4
		// (set) Token: 0x06000FE9 RID: 4073 RVA: 0x0000B8EB File Offset: 0x00009AEB
		internal static bool IsDownloadingInHiddenMode { get; set; } = true;

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x0000B8F3 File Offset: 0x00009AF3
		// (set) Token: 0x06000FEB RID: 4075 RVA: 0x0000B8FA File Offset: 0x00009AFA
		internal static BlueStacksUpdater.UpdateState SUpdateState
		{
			get
			{
				return BlueStacksUpdater.sUpdateState;
			}
			set
			{
				BlueStacksUpdater.sUpdateState = value;
				Action stateChanged = BlueStacksUpdater.StateChanged;
				if (stateChanged == null)
				{
					return;
				}
				stateChanged();
			}
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000FEC RID: 4076 RVA: 0x00065F6C File Offset: 0x0006416C
		// (remove) Token: 0x06000FED RID: 4077 RVA: 0x00065FA0 File Offset: 0x000641A0
		internal static event Action<BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>> DownloadCompleted;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000FEE RID: 4078 RVA: 0x00065FD4 File Offset: 0x000641D4
		// (remove) Token: 0x06000FEF RID: 4079 RVA: 0x00066008 File Offset: 0x00064208
		internal static event Action StateChanged;

		// Token: 0x06000FF0 RID: 4080 RVA: 0x0006603C File Offset: 0x0006423C
		public static void SetupBlueStacksUpdater(MainWindow window, bool isStartup)
		{
			BlueStacksUpdater.ParentWindow = window;
			if (BlueStacksUpdater.sCheckUpdateBackgroundWorker == null)
			{
				BlueStacksUpdater.sCheckUpdateBackgroundWorker = new BackgroundWorker();
				BlueStacksUpdater.sCheckUpdateBackgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
				{
					bool flag = (bool)e.Argument;
					BlueStacksUpdateData blueStacksUpdateData = BlueStacksUpdater.CheckForUpdate(!flag);
					BlueStacksUpdater.sBstUpdateData = blueStacksUpdateData;
					e.Result = new BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>(blueStacksUpdateData, flag);
				};
				BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerCompleted += BlueStacksUpdater.CheckUpdateBackgroundWorker_RunWorkerCompleted;
			}
			if (!BlueStacksUpdater.sCheckUpdateBackgroundWorker.IsBusy)
			{
				BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerAsync(isStartup);
				return;
			}
			Logger.Info("Not launching update checking thread, since already running");
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x000660C8 File Offset: 0x000642C8
		private static void CheckUpdateBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			BlueStacks.Common.Tuple<BlueStacksUpdateData, bool> tuple = (BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>)e.Result;
			BlueStacksUpdateData item = tuple.Item1;
			bool item2 = tuple.Item2;
			if (item.IsUpdateAvailble)
			{
				BlueStacksUpdater.ParentWindow.mTopBar.mConfigButton.ImageName = "cfgmenu_update";
				BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatus.Visibility = Visibility.Visible;
				BlueStacksUIBinding.Bind(BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_DOWNLOAD_UPDATE", "");
				BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Visibility = Visibility.Collapsed;
				if (!item.IsFullInstaller)
				{
					Logger.Info("Only client installer update, starting download.");
					BlueStacksUpdater.DownloadNow(item, true);
					return;
				}
				if (item2)
				{
					if (item.UpdateType.Equals("hard", StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.Info("Forced full installer update, starting download.");
						BlueStacksUpdater.DownloadNow(item, true);
						return;
					}
					if (item.UpdateType.Equals("soft", StringComparison.InvariantCultureIgnoreCase) && string.Compare(item.EngineVersion.Trim(), RegistryManager.Instance.LastUpdateSkippedVersion.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
					{
						ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopup, "");
						UpdatePrompt updatePrompt = new UpdatePrompt(item)
						{
							Height = 215.0,
							Width = 400.0
						};
						new ContainerWindow(BlueStacksUpdater.ParentWindow, updatePrompt, (double)((int)updatePrompt.Width), (double)((int)updatePrompt.Height), false, true, false, -1.0, null);
						return;
					}
				}
			}
			else
			{
				BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.NO_UPDATE;
			}
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x0006624C File Offset: 0x0006444C
		private static BlueStacksUpdateData CheckForUpdate(bool isManualCheck)
		{
			BlueStacksUpdateData blueStacksUpdateData = new BlueStacksUpdateData();
			BlueStacksUpdateData blueStacksUpdateData2;
			try
			{
				string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/check_upgrade");
				Logger.Debug("The URL for checking upgrade: {0}", new object[] { urlWithParams });
				string text;
				string text2;
				string text3;
				SystemUtils.GetOSInfo(out text, out text2, out text3);
				string text4 = InstallerArchitectures.AMD64;
				if (!SystemUtils.IsOs64Bit())
				{
					text4 = InstallerArchitectures.X86;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "installer_arch", text4 },
					{ "os", text },
					{
						"manual_check",
						isManualCheck.ToString(CultureInfo.InvariantCulture)
					}
				};
				string text5 = BstHttpClient.Post(urlWithParams, dictionary, null, false, string.Empty, 5000, 1, 0, false, "bgp64");
				Logger.Info("Response received for check for update: " + Environment.NewLine + text5);
				JObject jobject = JObject.Parse(text5);
				if (jobject["update_available"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase) && RegistryManager.Instance.FailedUpgradeVersion != jobject["update_details"]["client_version"].ToString())
				{
					blueStacksUpdateData.IsUpdateAvailble = true;
					blueStacksUpdateData.UpdateType = jobject["update_details"]["upgrade_type"].ToString();
					blueStacksUpdateData.IsFullInstaller = jobject["update_details"]["is_full_installer"].ToObject<bool>();
					blueStacksUpdateData.Md5 = jobject["update_details"]["md5"].ToString();
					blueStacksUpdateData.ClientVersion = jobject["update_details"]["client_version"].ToString();
					blueStacksUpdateData.EngineVersion = jobject["update_details"]["engine_version"].ToString();
					blueStacksUpdateData.DownloadUrl = jobject["update_details"]["download_url"].ToString();
					blueStacksUpdateData.DetailedChangeLogsUrl = jobject["update_details"]["detailed_changelogs_url"].ToString();
					if (!Directory.Exists(RegistryManager.Instance.SetupFolder))
					{
						Directory.CreateDirectory(RegistryManager.Instance.SetupFolder);
					}
					if (blueStacksUpdateData.IsFullInstaller)
					{
						blueStacksUpdateData.UpdateDownloadLocation = Path.Combine(RegistryManager.Instance.SetupFolder, "BlueStacksInstaller_" + blueStacksUpdateData.ClientVersion + "_full.exe");
					}
					else
					{
						blueStacksUpdateData.UpdateDownloadLocation = Path.Combine(RegistryManager.Instance.SetupFolder, "BlueStacksInstaller_" + blueStacksUpdateData.ClientVersion + "_client.zip");
					}
					RegistryManager.Instance.DownloadedUpdateFile = blueStacksUpdateData.UpdateDownloadLocation;
					BlueStacksUpdater.sBstUpdateData = blueStacksUpdateData;
					BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.UPDATE_AVAILABLE;
				}
				blueStacksUpdateData2 = blueStacksUpdateData;
			}
			catch (Exception ex)
			{
				Logger.Warning("Got error in checking for upgrade: {0}", new object[] { ex.ToString() });
				blueStacksUpdateData = new BlueStacksUpdateData
				{
					IsTryAgain = true
				};
				blueStacksUpdateData2 = blueStacksUpdateData;
			}
			return blueStacksUpdateData2;
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x00066548 File Offset: 0x00064748
		private static void DownloadUpdate(BlueStacksUpdateData bluestacksUpdateData)
		{
			BlueStacksUpdater.sDownloader = new Downloader();
			BlueStacksUpdater.sDownloader.DownloadException += BlueStacksUpdater.Downloader_DownloadException;
			BlueStacksUpdater.sDownloader.DownloadProgressPercentChanged += BlueStacksUpdater.Downloader_DownloadProgressPercentChanged;
			BlueStacksUpdater.sDownloader.DownloadFileCompleted += BlueStacksUpdater.Downloader_DownloadFileCompleted;
			BlueStacksUpdater.sDownloader.UnsupportedResume += BlueStacksUpdater.Downloader_UnsupportedResume;
			BlueStacksUpdater.sDownloader.DownloadFile(bluestacksUpdateData.DownloadUrl, bluestacksUpdateData.UpdateDownloadLocation);
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x000665D0 File Offset: 0x000647D0
		private static void Downloader_DownloadProgressPercentChanged(double percentDouble)
		{
			Logger.Info("File downloaded {0}%", new object[] { percentDouble });
			int percent = Convert.ToInt32(Math.Floor(percentDouble));
			BlueStacksUpdater.ParentWindow.mTopBar.ChangeDownloadPercent(percent);
			if (BlueStacksUpdater.sUpdateDownloadProgress != null)
			{
				BlueStacksUpdater.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					BlueStacksUpdater.sUpdateDownloadProgress.mUpdateDownloadProgressPercentage.Content = percent.ToString() + "%";
					BlueStacksUpdater.sUpdateDownloadProgress.mUpdateDownloadProgressBar.Value = (double)percent;
					BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Content = percent.ToString() + "%";
				}), new object[0]);
			}
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x0006664C File Offset: 0x0006484C
		private static void Downloader_UnsupportedResume(HttpStatusCode sc)
		{
			Logger.Error("UnsupportedResume, HTTPStatusCode: {0}", new object[] { sc });
			File.Delete(BlueStacksUpdater.sBstUpdateData.UpdateDownloadLocation);
			BlueStacksUpdater.sDownloader.DownloadFile(BlueStacksUpdater.sBstUpdateData.DownloadUrl, BlueStacksUpdater.sBstUpdateData.UpdateDownloadLocation);
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x0000B911 File Offset: 0x00009B11
		private static void Downloader_DownloadFileCompleted(object sender, EventArgs args)
		{
			string text = "File downloaded successfully at {0}";
			object[] array = new object[1];
			int num = 0;
			BlueStacksUpdateData blueStacksUpdateData = BlueStacksUpdater.sBstUpdateData;
			array[num] = ((blueStacksUpdateData != null) ? blueStacksUpdateData.UpdateDownloadLocation : null);
			Logger.Info(text, array);
			BlueStacksUpdater.DownloadComplete();
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x000666A0 File Offset: 0x000648A0
		private static void Downloader_DownloadException(Exception e)
		{
			Logger.Error("Failed to download file: {0}. err: {1}", new object[]
			{
				BlueStacksUpdater.sBstUpdateData.DownloadUrl,
				e.Message
			});
			BlueStacksUpdater.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_UPGRADE_FAILED", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SOME_ERROR_OCCURED_DOWNLOAD", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RETRY", new EventHandler(BlueStacksUpdater.RetryDownload), null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", new EventHandler(BlueStacksUpdater.DownloadCancelled), null, false, null);
				BlueStacksUpdater.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.Owner = BlueStacksUpdater.ParentWindow.mDimOverlay;
				customMessageWindow.ShowDialog();
				BlueStacksUpdater.ParentWindow.HideDimOverlay();
				BlueStacksUpdater.sUpdateDownloadProgress.Hide();
			}), new object[0]);
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x0000B93C File Offset: 0x00009B3C
		private static void DownloadCancelled(object sender, EventArgs e)
		{
			BlueStacksUpdater.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatus.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x0000B973 File Offset: 0x00009B73
		private static void RetryDownload(object sender, EventArgs e)
		{
			new Thread(delegate
			{
				BlueStacksUpdater.sDownloader.DownloadFile(BlueStacksUpdater.sBstUpdateData.DownloadUrl, BlueStacksUpdater.sBstUpdateData.UpdateDownloadLocation);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x00066708 File Offset: 0x00064908
		private static void DownloadComplete()
		{
			Logger.Info("Installer download completed");
			BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.DOWNLOADED;
			BlueStacksUpdater.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				BlueStacksUIBinding.Bind(BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_INSTALL_UPDATE", "");
				BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Visibility = Visibility.Collapsed;
				if (BlueStacksUpdater.sUpdateDownloadProgress != null)
				{
					BlueStacksUpdater.sUpdateDownloadProgress.Close();
				}
			}), new object[0]);
			BlueStacksUpdater.DownloadCompleted(new BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>(BlueStacksUpdater.sBstUpdateData, BlueStacksUpdater.IsDownloadingInHiddenMode));
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x0000B9A5 File Offset: 0x00009BA5
		internal static void DownloadNow(BlueStacksUpdateData bstUpdateData, bool hiddenMode)
		{
			new Thread(delegate
			{
				BlueStacksUpdater.IsDownloadingInHiddenMode = hiddenMode;
				BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.DOWNLOADING;
				if (File.Exists(bstUpdateData.UpdateDownloadLocation))
				{
					BlueStacksUpdater.DownloadComplete();
					return;
				}
				BlueStacksUpdater.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					BlueStacksUIBinding.Bind(BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_DOWNLOADING_UPDATE", "");
					BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Visibility = Visibility.Visible;
					BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Content = "0%";
					BlueStacksUpdater.sUpdateDownloadProgress = new UpdateDownloadProgress();
					BlueStacksUpdater.sUpdateDownloadProgress.mUpdateDownloadProgressPercentage.Content = "0%";
					BlueStacksUpdater.sUpdateDownloadProgress.Owner = BlueStacksUpdater.ParentWindow;
					if (!hiddenMode)
					{
						BlueStacksUpdater.sUpdateDownloadProgress.Show();
					}
				}), new object[0]);
				BlueStacksUpdater.DownloadUpdate(bstUpdateData);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x0000B9D6 File Offset: 0x00009BD6
		internal static void ShowDownloadProgress()
		{
			if (BlueStacksUpdater.sUpdateDownloadProgress != null)
			{
				BlueStacksUpdater.sUpdateDownloadProgress.Show();
			}
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x00066774 File Offset: 0x00064974
		internal static void CheckDownloadedUpdateFileAndUpdate()
		{
			using (BackgroundWorker backgroundWorker = new BackgroundWorker())
			{
				backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs args)
				{
					BlueStacksUpdater.HandleUpgrade(RegistryManager.Instance.DownloadedUpdateFile);
				};
				backgroundWorker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args2)
				{
					App.ExitApplication();
				};
				backgroundWorker.RunWorkerAsync();
			}
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x0000B9E9 File Offset: 0x00009BE9
		internal static void HandleUpgrade(string downloadedFilePath)
		{
			if (BlueStacksUpdater.CheckIfUpdateIsFullOrClientOnly(downloadedFilePath) == BlueStacksUpdater.UpdateType.ClientOnly)
			{
				BlueStacksUpdater.HandleClientOnlyUpgrade(downloadedFilePath);
			}
			else
			{
				BlueStacksUpdater.HandleFullUpgrade(downloadedFilePath);
			}
			RegistryManager.Instance.UpdaterFileDeletePath = RegistryManager.Instance.DownloadedUpdateFile;
			RegistryManager.Instance.DownloadedUpdateFile = "";
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x0000BA25 File Offset: 0x00009C25
		private static void HandleFullUpgrade(string downloadedFilePath)
		{
			Logger.Info("In HandleFullUpgrade");
			BluestacksProcessHelper.RunUpdateInstaller(downloadedFilePath, "-u -upgradesourcepath BluestacksUI", false);
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x000667F4 File Offset: 0x000649F4
		private static void HandleClientOnlyUpgrade(string downloadedFilePath)
		{
			Logger.Info("In HandleClientOnlyUpgrade");
			try
			{
				int num = BlueStacksUpdater.ExtractingClientInstaller(downloadedFilePath);
				if (num == 0)
				{
					BluestacksProcessHelper.RunUpdateInstaller(Path.Combine(Path.Combine(RegistryManager.Instance.SetupFolder, Path.GetFileNameWithoutExtension(downloadedFilePath)), "Bootstrapper.exe"), "", false);
				}
				else
				{
					Logger.Warning("Update extraction failed, ExitCode: {0}", new object[] { num });
					File.Delete(downloadedFilePath);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some Error in Client Upgrade err: ", new object[] { ex.ToString() });
			}
		}

		// Token: 0x06001001 RID: 4097 RVA: 0x00066890 File Offset: 0x00064A90
		internal static bool CheckIfDownloadedFileExist()
		{
			string downloadedUpdateFile = RegistryManager.Instance.DownloadedUpdateFile;
			if (!string.IsNullOrEmpty(downloadedUpdateFile) && File.Exists(downloadedUpdateFile))
			{
				return true;
			}
			string updaterFileDeletePath = RegistryManager.Instance.UpdaterFileDeletePath;
			if (!string.IsNullOrEmpty(updaterFileDeletePath) && File.Exists(updaterFileDeletePath) && RegistryManager.Instance.IsClientFirstLaunch == 1)
			{
				try
				{
					File.Delete(updaterFileDeletePath);
					RegistryManager.Instance.UpdaterFileDeletePath = "";
				}
				catch (Exception ex)
				{
					Logger.Warning("Error in Deleting Updater File : ", new object[] { ex.ToString() });
				}
			}
			return false;
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x0000BA3D File Offset: 0x00009C3D
		private static BlueStacksUpdater.UpdateType CheckIfUpdateIsFullOrClientOnly(string downloadedFilePath)
		{
			if (string.Equals(Path.GetExtension(downloadedFilePath), ".zip", StringComparison.InvariantCultureIgnoreCase))
			{
				return BlueStacksUpdater.UpdateType.ClientOnly;
			}
			return BlueStacksUpdater.UpdateType.FullUpdate;
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x00066928 File Offset: 0x00064B28
		private static int ExtractingClientInstaller(string updateFile)
		{
			string text = Path.Combine(RegistryManager.Instance.SetupFolder, Path.GetFileNameWithoutExtension(updateFile));
			Logger.Info("Extracting Zip file {0} at {1}", new object[] { updateFile, text });
			return MiscUtils.Extract7Zip(updateFile, text);
		}

		// Token: 0x04000A59 RID: 2649
		private static MainWindow ParentWindow;

		// Token: 0x04000A5A RID: 2650
		internal static BackgroundWorker sCheckUpdateBackgroundWorker;

		// Token: 0x04000A5B RID: 2651
		private static UpdateDownloadProgress sUpdateDownloadProgress;

		// Token: 0x04000A5C RID: 2652
		internal static BlueStacksUpdateData sBstUpdateData = null;

		// Token: 0x04000A5D RID: 2653
		internal static Downloader sDownloader;

		// Token: 0x04000A5F RID: 2655
		private static BlueStacksUpdater.UpdateState sUpdateState = BlueStacksUpdater.UpdateState.NO_UPDATE;

		// Token: 0x02000196 RID: 406
		internal enum UpdateState
		{
			// Token: 0x04000A63 RID: 2659
			NO_UPDATE,
			// Token: 0x04000A64 RID: 2660
			UPDATE_AVAILABLE,
			// Token: 0x04000A65 RID: 2661
			DOWNLOADING,
			// Token: 0x04000A66 RID: 2662
			DOWNLOADED
		}

		// Token: 0x02000197 RID: 407
		internal enum UpdateType
		{
			// Token: 0x04000A68 RID: 2664
			FullUpdate,
			// Token: 0x04000A69 RID: 2665
			ClientOnly
		}
	}
}
