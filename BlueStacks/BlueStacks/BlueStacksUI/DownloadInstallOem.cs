using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000056 RID: 86
	internal class DownloadInstallOem
	{
		// Token: 0x06000478 RID: 1144 RVA: 0x00004EFB File Offset: 0x000030FB
		public DownloadInstallOem(MainWindow mainWindow)
		{
			this.ParentWindow = mainWindow;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0001C784 File Offset: 0x0001A984
		public void DownloadOem(AppPlayerModel appPlayerModel)
		{
			this.currentDownloadingOem = appPlayerModel;
			if (InstalledOem.InstalledCoexistingOemList.Contains(appPlayerModel.AppPlayerOem))
			{
				using (BackgroundWorker backgroundWorker = new BackgroundWorker())
				{
					backgroundWorker.DoWork += this.BGCreateNewInstance_DoWork;
					backgroundWorker.RunWorkerCompleted += this.BGCreateNewInstance_RunWorkerCompleted;
					backgroundWorker.RunWorkerAsync();
					return;
				}
			}
			Publisher.PublishMessage(BrowserControlTags.oemDownloadStarted, this.ParentWindow.mVmName, null);
			if (!appPlayerModel.DownLoadOem(delegate(Exception _1)
			{
				JObject jobject2 = new JObject();
				jobject2["MessageTitle"] = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", "");
				jobject2["MessageBody"] = LocaleStrings.GetLocalizedString("STRING_ERROR_RECORDER_DOWNLOAD", "");
				jobject2["ActionType"] = "failed";
				Publisher.PublishMessage(BrowserControlTags.oemDownloadFailed, this.ParentWindow.mVmName, jobject2);
			}, delegate(long size)
			{
				decimal num = decimal.Divide(size, this.mSizeInBytes) * 100m;
				if (this.mLastPercentSend + 4 < (int)num)
				{
					JObject jobject3 = new JObject();
					jobject3["DownloadPercent"] = num;
					Publisher.PublishMessage(BrowserControlTags.oemDownloadCurrentProgress, this.ParentWindow.mVmName, jobject3);
					this.mLastPercentSend = (int)num;
				}
			}, delegate(object _1, EventArgs _2)
			{
				Publisher.PublishMessage(BrowserControlTags.oemDownloadCompleted, this.ParentWindow.mVmName, null);
				this.InstallOemOperation();
			}, delegate(long fileSize)
			{
				this.mSizeInBytes = fileSize;
			}, delegate(HttpStatusCode _1)
			{
				JObject jobject4 = new JObject();
				jobject4["MessageTitle"] = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", "");
				jobject4["MessageBody"] = LocaleStrings.GetLocalizedString("STRING_FAILED_DOWNLOAD_RETRY", "");
				jobject4["ActionType"] = "retry";
				Publisher.PublishMessage(BrowserControlTags.oemDownloadFailed, this.ParentWindow.mVmName, jobject4);
			}, false))
			{
				JObject jobject = new JObject();
				jobject["MessageTitle"] = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", "");
				jobject["MessageBody"] = LocaleStrings.GetLocalizedString("STRING_ERROR_RECORDER_DOWNLOAD", "");
				jobject["ActionType"] = "failed";
				Publisher.PublishMessage(BrowserControlTags.oemDownloadFailed, this.ParentWindow.mVmName, jobject);
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0001C8C0 File Offset: 0x0001AAC0
		private void BGCreateNewInstance_DoWork(object sender, DoWorkEventArgs e)
		{
			Publisher.PublishMessage(BrowserControlTags.oemDownloadStarted, this.ParentWindow.mVmName, null);
			Publisher.PublishMessage(BrowserControlTags.oemDownloadCompleted, this.ParentWindow.mVmName, null);
			Publisher.PublishMessage(BrowserControlTags.oemInstallStarted, this.ParentWindow.mVmName, null);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				JObject jobject = new JObject
				{
					{ "cpu", 2 },
					{ "ram", 2048 },
					{ "dpi", 240 },
					{
						"abi",
						this.currentDownloadingOem.AbiValue
					},
					{ "resolutionwidth", 1920 },
					{ "resolutionheight", 1080 }
				};
				dictionary2["settings"] = jobject.ToString(Formatting.None, new JsonConverter[0]);
				dictionary2["vmtype"] = "fresh";
				dictionary2["vmname"] = string.Format(CultureInfo.InvariantCulture, "Android_{0}", new object[] { Utils.GetVmIdToCreate(this.currentDownloadingOem.AppPlayerOem) });
				JObject jobject2 = JObject.Parse(HTTPUtils.SendRequestToAgent("createInstance", dictionary2, "Android", 240000, null, false, 1, 0, this.currentDownloadingOem.AppPlayerOem, true));
				if (jobject2["success"].ToObject<bool>())
				{
					string text = JObject.Parse(jobject2["vmconfig"].ToString().Trim())["vmname"].ToString().Trim();
					dictionary["vmname"] = text;
					dictionary["status"] = "success";
				}
				else
				{
					dictionary["status"] = "fail";
					dictionary["reason"] = jobject2["reason"].ToString().Trim();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("error in creating new instance" + ex.ToString());
				dictionary["status"] = "fail";
				dictionary["reason"] = "UnknownException";
			}
			finally
			{
				e.Result = dictionary;
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0001CB2C File Offset: 0x0001AD2C
		private void BGCreateNewInstance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dictionary<string, string> dictionary = e.Result as Dictionary<string, string>;
			if (dictionary != null)
			{
				if (dictionary["status"].Equals("success", StringComparison.InvariantCultureIgnoreCase))
				{
					InstalledOem.SetInstalledCoexistingOems();
					RegistryManager.RegistryManagers[this.currentDownloadingOem.AppPlayerOem].Guest[dictionary["vmname"]].DisplayName = string.Concat(new string[]
					{
						Strings.ProductDisplayName,
						" ",
						Utils.GetVmIdFromVmName(dictionary["vmname"]),
						" ",
						this.currentDownloadingOem.Suffix
					});
					Publisher.PublishMessage(BrowserControlTags.oemInstallCompleted, this.ParentWindow.mVmName, null);
					return;
				}
				if (dictionary["status"].Equals("fail", StringComparison.InvariantCultureIgnoreCase))
				{
					JObject jobject = new JObject();
					jobject["MessageTitle"] = LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", "");
					jobject["MessageBody"] = LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", "");
					jobject["ActionType"] = "failed";
					Publisher.PublishMessage(BrowserControlTags.oemInstallFailed, this.ParentWindow.mVmName, jobject);
				}
			}
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0001CC78 File Offset: 0x0001AE78
		private void InstallOemOperation()
		{
			try
			{
				Publisher.PublishMessage(BrowserControlTags.oemInstallStarted, this.ParentWindow.mVmName, null);
				int num = this.currentDownloadingOem.InstallOem();
				if (num != 0 || !RegistryManager.CheckOemInRegistry(this.currentDownloadingOem.AppPlayerOem, "Android"))
				{
					Logger.Warning("Installation failed: " + num.ToString());
					string text = ((num == 0) ? LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", "") : InstallerErrorHandling.AssignErrorStringForInstallerExitCodes(num, "STRING_INSTALLATION_FAILED"));
					JObject jobject = new JObject();
					jobject["MessageTitle"] = LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", "");
					jobject["MessageBody"] = text;
					jobject["ActionType"] = "failed";
					Publisher.PublishMessage(BrowserControlTags.oemInstallFailed, this.ParentWindow.mVmName, jobject);
				}
				else
				{
					InstalledOem.SetInstalledCoexistingOems();
					if (this.currentDownloadingOem.AppPlayerOem.Contains("bgp64"))
					{
						Utils.UpdateValueInBootParams("abivalue", this.currentDownloadingOem.AbiValue.ToString(CultureInfo.InvariantCulture), "Android", true, this.currentDownloadingOem.AppPlayerOem);
					}
					RegistryManager.RegistryManagers[this.currentDownloadingOem.AppPlayerOem].Guest["Android"].DisplayName = Strings.ProductDisplayName + " " + this.currentDownloadingOem.Suffix;
					Publisher.PublishMessage(BrowserControlTags.oemInstallCompleted, this.ParentWindow.mVmName, null);
				}
			}
			catch (Exception ex)
			{
				string text2 = "Failed after running installer process: ";
				Exception ex2 = ex;
				Logger.Error(text2 + ((ex2 != null) ? ex2.ToString() : null));
				JObject jobject2 = new JObject();
				jobject2["MessageTitle"] = LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", "");
				jobject2["MessageBody"] = LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", "");
				jobject2["ActionType"] = "failed";
				Publisher.PublishMessage(BrowserControlTags.oemInstallFailed, this.ParentWindow.mVmName, jobject2);
			}
		}

		// Token: 0x04000274 RID: 628
		private MainWindow ParentWindow;

		// Token: 0x04000275 RID: 629
		private AppPlayerModel currentDownloadingOem;

		// Token: 0x04000276 RID: 630
		private long mSizeInBytes;

		// Token: 0x04000277 RID: 631
		private int mLastPercentSend;
	}
}
