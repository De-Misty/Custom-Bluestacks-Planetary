using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000209 RID: 521
	internal class DownloadInstallApk
	{
		// Token: 0x060013F7 RID: 5111 RVA: 0x0000DD80 File Offset: 0x0000BF80
		public DownloadInstallApk(MainWindow mainWindow)
		{
			this.ParentWindow = mainWindow;
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x0000DD8F File Offset: 0x0000BF8F
		public static SerialWorkQueue SerialWorkQueueInstaller(string vmName)
		{
			if (!DownloadInstallApk.mSerialQueue.ContainsKey(vmName))
			{
				DownloadInstallApk.mSerialQueue[vmName] = new SerialWorkQueue();
			}
			return DownloadInstallApk.mSerialQueue[vmName];
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x000787AC File Offset: 0x000769AC
		internal void DownloadAndInstallAppFromJson(string campaignJson)
		{
			try
			{
				JObject jobject = JObject.Parse(campaignJson);
				string iconUrl = "";
				string appName = "";
				string apkUrl = "";
				string package = "";
				jobject.AssignIfContains("app_icon_url", delegate(string x)
				{
					iconUrl = x.Trim();
				});
				jobject.AssignIfContains("app_name", delegate(string x)
				{
					appName = x.Trim();
				});
				jobject.AssignIfContains("app_url", delegate(string x)
				{
					apkUrl = x.Trim();
				});
				jobject.AssignIfContains("app_pkg", delegate(string x)
				{
					package = x.Trim();
				});
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					if (string.IsNullOrEmpty(apkUrl))
					{
						this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(package, PlayStoreAction.OpenApp);
						return;
					}
					this.DownloadAndInstallApp(iconUrl, appName, apkUrl, package, true, true, "");
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Info("Error in Fle. Json string : " + campaignJson + "Error: " + ex.ToString());
			}
		}

		// Token: 0x060013FA RID: 5114 RVA: 0x000788AC File Offset: 0x00076AAC
		internal void DownloadAndInstallApp(string iconUrl, string appName, string apkUrl, string packageName, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp = "")
		{
			if (this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) == null || this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName).IsAppSuggestionActive)
			{
				this.DownloadApk(iconUrl, appName, apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
				return;
			}
			if (!this.ParentWindow.mAppHandler.IsAppInstalled(packageName))
			{
				this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
				return;
			}
			if (string.IsNullOrEmpty(timestamp))
			{
				this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false);
				return;
			}
			bool flag = true;
			DateTime dateTime = DateTime.Parse(timestamp, CultureInfo.InvariantCulture);
			DateTime dateTime2 = DateTime.MaxValue;
			if (this.ParentWindow.mAppHandler.CdnAppdict.ContainsKey(packageName))
			{
				dateTime2 = this.ParentWindow.mAppHandler.CdnAppdict[packageName];
				if (dateTime <= dateTime2)
				{
					flag = false;
				}
			}
			if (flag)
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_INSTALL_UPDATE", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_APP_UPGRADE", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_UPGRADE_TEXT", delegate(object sender1, EventArgs e1)
				{
					this.DownloadApk(iconUrl, appName, apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_CONTINUE_ANYWAY", delegate(object sender1, EventArgs e1)
				{
					this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false);
				}, null, false, null);
				customMessageWindow.Owner = this.ParentWindow;
				customMessageWindow.ShowDialog();
				return;
			}
			this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false);
		}

		// Token: 0x060013FB RID: 5115 RVA: 0x00078ACC File Offset: 0x00076CCC
		internal void DownloadApk(string iconUrl, string appName, string apkUrl, string packageName, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp)
		{
			if (!string.IsNullOrEmpty(apkUrl))
			{
				Logger.Info("apkUrl: " + apkUrl);
				Utils.TinyDownloader(iconUrl, packageName + ".png", "", false);
				this.ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(packageName, appName, apkUrl, this);
				this.DownloadApk(apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
			}
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x00078B34 File Offset: 0x00076D34
		public void DownloadApk(string apkUrl, string packageName, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp = "")
		{
			string text = Path.Combine(RegistryStrings.DataDir, "DownloadedApk");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string text2 = Regex.Replace(packageName + ".apk", "[\\x22\\\\\\/:*?|<>]", " ");
			string apkFilePath = Path.Combine(text, text2);
			Logger.Info("Downloading Apk file to: " + apkFilePath);
			this.ParentWindow.mWelcomeTab.mHomeAppManager.DownloadStarted(packageName);
			ClientStats.SendClientStatsAsync("download", "unknown", "app_install", packageName, "", "");
			this.mDownloadThread = new Thread(delegate
			{
				string apkUrl2 = apkUrl;
				if (DownloadInstallApk.IsContainsGoogleAdId(apkUrl2))
				{
					apkUrl = this.AddGoogleAdidWithApk(apkUrl2);
				}
				apkUrl = BlueStacksUIUtils.GetFinalRedirectedUrl(apkUrl);
				if (!string.IsNullOrEmpty(apkUrl))
				{
					this.mIsDownloading = true;
					this.mDownloader = new LegacyDownloader(3, apkUrl, apkFilePath);
					this.mDownloader.Download(delegate(int percent)
					{
						this.ParentWindow.mWelcomeTab.mHomeAppManager.UpdateAppDownloadProgress(packageName, percent);
					}, delegate(string filePath)
					{
						ClientStats.SendClientStatsAsync("download", "success", "app_install", packageName, "", "");
						this.mIsDownloading = false;
						this.ParentWindow.mWelcomeTab.mHomeAppManager.DownloadCompleted(packageName, filePath);
						this.InstallApk(packageName, filePath, isLaunchAfterInstall, isDeleteApk, timestamp);
						DownloadInstallApk.sDownloadedApkList.Add(packageName);
					}, delegate(Exception ex)
					{
						ClientStats.SendClientStatsAsync("download", "fail", "app_install", packageName, "", "");
						this.ParentWindow.mWelcomeTab.mHomeAppManager.DownloadFailed(packageName);
						Logger.Error("Failed to download file: {0}. err: {1}", new object[] { apkFilePath, ex.Message });
					}, null, null, null);
				}
			})
			{
				IsBackground = true
			};
			this.mDownloadThread.Start();
		}

		// Token: 0x060013FD RID: 5117 RVA: 0x00078C3C File Offset: 0x00076E3C
		private string AddGoogleAdidWithApk(string apkUrl)
		{
			Logger.Info("In AddGoogleAdidWithApk");
			string text = "google_aid=00000000-0000-0000-0000-000000000000";
			string text4;
			try
			{
				JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getGoogleAdID", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64"));
				if (jobject["result"].ToString() == "ok")
				{
					string text2 = jobject["googleadid"].ToString();
					string text3 = string.Format(CultureInfo.InvariantCulture, "google_aid={0}", new object[] { text2 });
					text4 = apkUrl.Replace("google_aid={google_aid}", text3);
				}
				else
				{
					text4 = apkUrl.Replace("google_aid={google_aid}", text);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in adding google adId" + ex.ToString());
				text4 = apkUrl.Replace("google_aid={google_aid}", text);
			}
			return text4;
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x0000DDB9 File Offset: 0x0000BFB9
		private static bool IsContainsGoogleAdId(string apkUrl)
		{
			return apkUrl.ToLower(CultureInfo.InvariantCulture).Contains("google_aid={google_aid}");
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x00078D20 File Offset: 0x00076F20
		internal void AbortApkDownload(string packageName)
		{
			ClientStats.SendClientStatsAsync("download", "cancel", "app_install", packageName, "", "");
			if (this.mDownloader != null)
			{
				this.mDownloader.AbortDownload();
			}
			if (this.mDownloadThread != null)
			{
				this.mDownloadThread.Abort();
			}
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x00078D74 File Offset: 0x00076F74
		internal void ChooseAndInstallApk()
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Android Files (*.apk, *.xapk)|*.apk; *.xapk",
				Multiselect = true,
				RestoreDirectory = true
			})
			{
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					foreach (string text in openFileDialog.FileNames)
					{
						Logger.Info("File Selected : " + text);
						this.InstallApk(text, true);
					}
				}
			}
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x00078DF8 File Offset: 0x00076FF8
		internal void InstallApk(string apkPath, bool addToChooseApkList = false)
		{
			Logger.Info("Console: Installing apk: {0}", new object[] { apkPath });
			string package = string.Empty;
			string appName = string.Empty;
			if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
			{
				JToken jtoken = Utils.ExtractInfoFromXapk(apkPath);
				if (jtoken != null)
				{
					package = jtoken.GetValue("package_name");
					appName = jtoken.GetValue("name");
					Logger.Debug("Package name from manifest.json.." + package);
				}
			}
			else
			{
				AppInfoExtractor apkInfo = AppInfoExtractor.GetApkInfo(apkPath);
				package = apkInfo.PackageName;
				appName = apkInfo.AppName;
			}
			if (addToChooseApkList)
			{
				DownloadInstallApk.sApkInstalledFromChooser.Add(package);
			}
			if (!string.IsNullOrEmpty(package))
			{
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					this.ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(package, appName, string.Empty, this);
				}), new object[0]);
			}
			this.InstallApk(package, apkPath, false, false, "");
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x00078F08 File Offset: 0x00077108
		internal void InstallApk(string packageName, string apkPath, bool isLaunchAfterInstall, bool isDeleteApk, string timestamp = "")
		{
			this.ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallStart(packageName, apkPath);
			DownloadInstallApk.SerialWorkQueueInstaller(this.ParentWindow.mVmName).Enqueue(delegate
			{
				Logger.Info("Installing apk: {0}", new object[] { apkPath });
				int num = BluestacksProcessHelper.RunApkInstaller(apkPath, true, this.ParentWindow.mVmName);
				Logger.Info("Apk installer exit code: {0}", new object[] { num });
				if (num == 0)
				{
					if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
					{
						ClientStats.SendClientStatsAsync("install_from_download", "success", "app_install", packageName, "", "");
						DownloadInstallApk.sDownloadedApkList.Remove(packageName);
						this.UpdateCdnAppEntry(true, packageName, timestamp);
					}
					else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
					{
						ClientStats.SendClientStatsAsync("install", "success", "app_install", packageName, "", "");
						DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
					}
					this.ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallCompleted(packageName);
					if (isLaunchAfterInstall)
					{
						this.ParentWindow.Utils.RunAppOrCreateTabButton(packageName);
					}
					Logger.Info("Installation successful.");
					if (isDeleteApk)
					{
						File.Delete(apkPath);
					}
					Logger.Info("Install Completed : " + packageName);
					return;
				}
				if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install_from_download", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture), "");
					DownloadInstallApk.sDownloadedApkList.Remove(packageName);
				}
				else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture), "");
					DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
				}
				ClientStats.SendGeneralStats("apk_inst_error", new Dictionary<string, string>
				{
					{
						"errcode",
						Convert.ToString(num, CultureInfo.InvariantCulture)
					},
					{ "precode", "0" },
					{ "app_pkg", packageName }
				});
				this.ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallFailed(packageName);
			});
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x00078F8C File Offset: 0x0007718C
		internal int InstallFLEApk(string packageName, string apkPath)
		{
			Logger.Info("Installing apk: {0}", new object[] { apkPath });
			int num = BluestacksProcessHelper.RunApkInstaller(apkPath, true, this.ParentWindow.mVmName);
			Logger.Info("Apk installer exit code: {0}", new object[] { num });
			if (num == 0)
			{
				if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install_from_download", "success", "app_install", packageName, "", "");
					DownloadInstallApk.sDownloadedApkList.Remove(packageName);
					this.UpdateCdnAppEntry(true, packageName, "");
				}
				else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install", "success", "app_install", packageName, "", "");
					DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
				}
				this.ParentWindow.Utils.RunAppOrCreateTabButton(packageName);
				Logger.Info("Installation successful.");
				File.Delete(apkPath);
			}
			else
			{
				if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install_from_download", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture), "");
					DownloadInstallApk.sDownloadedApkList.Remove(packageName);
				}
				else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
				{
					ClientStats.SendClientStatsAsync("install", "fail", "app_install", packageName, num.ToString(CultureInfo.InvariantCulture), "");
					DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
				}
				ClientStats.SendGeneralStats("apk_inst_error", new Dictionary<string, string>
				{
					{
						"errcode",
						Convert.ToString(num, CultureInfo.InvariantCulture)
					},
					{ "precode", "0" },
					{ "app_pkg", packageName }
				});
			}
			Logger.Info("Install Completed : " + packageName);
			return num;
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x00079158 File Offset: 0x00077358
		internal void UninstallApp(string packageName)
		{
			DownloadInstallApk.SerialWorkQueueInstaller(this.ParentWindow.mVmName).Enqueue(delegate
			{
				Logger.Info("Uninstall started : " + packageName);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["package"] = packageName;
				Dictionary<string, string> dictionary2 = dictionary;
				try
				{
					JArray jarray = JArray.Parse(HTTPUtils.SendRequestToAgent("uninstall", dictionary2, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64", true));
					try
					{
						if (!JObject.Parse(jarray[0].ToString())["success"].ToObject<bool>())
						{
							ClientStats.SendClientStatsAsync("uninstall", "fail", "app_install", packageName, "", "");
						}
						else
						{
							this.UpdateCdnAppEntry(false, packageName, "");
						}
					}
					catch
					{
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to uninstall app. Err: " + ex.Message);
				}
				Logger.Info("Uninstall completed for " + packageName);
			});
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0007919C File Offset: 0x0007739C
		internal void UpdateCdnAppEntry(bool isAdd, string packageName, string timestamp)
		{
			DateTime dateTime = DateTime.MinValue;
			if (!string.IsNullOrEmpty(timestamp))
			{
				dateTime = DateTime.Parse(timestamp, CultureInfo.InvariantCulture);
			}
			this.ParentWindow.mAppHandler.WriteXMl(isAdd, packageName, dateTime);
		}

		// Token: 0x04000C8D RID: 3213
		internal const string mIconUrl = "https://cloud.bluestacks.com/app/icon?pkg={0}&fallback=false";

		// Token: 0x04000C8E RID: 3214
		private Thread mDownloadThread;

		// Token: 0x04000C8F RID: 3215
		public bool mIsDownloading;

		// Token: 0x04000C90 RID: 3216
		private LegacyDownloader mDownloader;

		// Token: 0x04000C91 RID: 3217
		private static Dictionary<string, SerialWorkQueue> mSerialQueue = new Dictionary<string, SerialWorkQueue>();

		// Token: 0x04000C92 RID: 3218
		internal static List<string> sDownloadedApkList = new List<string>();

		// Token: 0x04000C93 RID: 3219
		internal static List<string> sApkInstalledFromChooser = new List<string>();

		// Token: 0x04000C94 RID: 3220
		private MainWindow ParentWindow;
	}
}
