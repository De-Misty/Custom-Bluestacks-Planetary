using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200022B RID: 555
	internal class MyCustomCefV8Handler : CefV8Handler
	{
		// Token: 0x060014B2 RID: 5298 RVA: 0x0000E5C9 File Offset: 0x0000C7C9
		protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
		{
			returnValue = CefV8Value.CreateString("");
			this.ReceiveJsFunctionCall(arguments, ref returnValue);
			exception = null;
			return true;
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x0007CAD4 File Offset: 0x0007ACD4
		private void ReceiveJsFunctionCall(CefV8Value[] arguments, ref CefV8Value returnValue)
		{
			JObject jobject = JObject.Parse(arguments[0].GetStringValue());
			string text = jobject["data"].ToString();
			if (string.IsNullOrEmpty(text) || text.Equals("null", StringComparison.InvariantCultureIgnoreCase))
			{
				text = "[]";
			}
			JArray jarray = JArray.Parse(text);
			object[] array = null;
			if (!jarray.IsNullOrEmpty())
			{
				array = new object[jarray.Count];
				for (int i = 0; i < jarray.Count; i++)
				{
					array[i] = jarray[i].ToString();
				}
			}
			try
			{
				try
				{
					if (jobject.ContainsKey("callbackFunction"))
					{
						this.mCallBackFunction = jobject["callbackFunction"].ToString();
					}
				}
				catch
				{
					Logger.Info("Error in callback function name.");
				}
				if (jobject["calledFunction"].ToString().IndexOf("LogInfo", StringComparison.InvariantCulture) == -1)
				{
					Logger.Debug("Calling function from GM API.." + jobject["calledFunction"].ToString());
				}
				jobject["calledFunction"].ToString();
				object obj;
				try
				{
					Type[] array2 = Type.EmptyTypes;
					if (array != null)
					{
						array2 = Type.GetTypeArray(array);
					}
					obj = base.GetType().GetMethod(jobject["calledFunction"].ToString(), array2).Invoke(this, array);
				}
				catch (Exception)
				{
					obj = base.GetType().GetMethod(jobject["calledFunction"].ToString()).Invoke(this, array);
				}
				if (obj != null)
				{
					returnValue = CefV8Value.CreateString((string)obj);
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Error in ReceiveJSFunctionCall: " + ex.ToString());
			}
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x0007CC98 File Offset: 0x0007AE98
		internal void OnProcessMessageReceived(CefProcessMessage message)
		{
			Logger.Info("message received in render process." + message.Name);
			string name = message.Name;
			if (name != null && name == "SetVmName")
			{
				MyCustomCefV8Handler.vmName = message.Arguments.GetString(0);
			}
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x0007CCE4 File Offset: 0x0007AEE4
		public string isBTVInstalled()
		{
			if (!BTVManager.IsBTVInstalled())
			{
				using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("DownloadBTV"))
				{
					CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
					return "false";
				}
			}
			if (!BTVManager.IsDirectXComponentsInstalled())
			{
				using (CefProcessMessage cefProcessMessage2 = CefProcessMessage.Create("DownloadDirectX"))
				{
					CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage2);
					return "false";
				}
			}
			return "true";
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x0007CD80 File Offset: 0x0007AF80
		public void sendFirebaseNotification(string data)
		{
			Logger.Debug("Got call for sendFirebaseNotification");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("sendFirebaseNotification"))
			{
				cefProcessMessage.Arguments.SetString(0, data);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x0007CDE0 File Offset: 0x0007AFE0
		public void subscribeModule(string tag)
		{
			Logger.Info("Subscribe html module");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("subscribeModule"))
			{
				cefProcessMessage.Arguments.SetString(0, tag);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x0007CE40 File Offset: 0x0007B040
		public void UnsubscribeModule(string tag)
		{
			Logger.Info("Unsubscribe html module");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("unsubscribeModule"))
			{
				cefProcessMessage.Arguments.SetString(0, tag);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x0007CEA0 File Offset: 0x0007B0A0
		public void subscribeToClientTags(string json)
		{
			Logger.Info("Subscribe to client tags");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("subscribeClientTags"))
			{
				cefProcessMessage.Arguments.SetString(0, json);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x0007CF00 File Offset: 0x0007B100
		public void subscribeToVmSpecificClientTags(string json)
		{
			Logger.Info("Subscribe to vm specific client tags");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("subscribeVmSpecificClientTags"))
			{
				cefProcessMessage.Arguments.SetString(0, json);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x0007CF60 File Offset: 0x0007B160
		public void UnsubscribeToClientTags(string json)
		{
			Logger.Info("Unsubscribe to client tags");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("unsubscribeClientTags"))
			{
				cefProcessMessage.Arguments.SetString(0, json);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x0007CFC0 File Offset: 0x0007B1C0
		public void HandleClick(string json)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("HandleClick"))
			{
				cefProcessMessage.Arguments.SetString(0, json);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x0007D014 File Offset: 0x0007B214
		public void UpdateQuestRules(string json)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("UpdateQuestRules"))
			{
				cefProcessMessage.Arguments.SetString(0, json);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x0007D068 File Offset: 0x0007B268
		public void pikaworldprofileadded(string profileId)
		{
			Logger.Debug("Got call for PikaWorldProfileAdded");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("PikaWorldProfileAdded"))
			{
				cefProcessMessage.Arguments.SetString(0, profileId);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x0000E5E6 File Offset: 0x0000C7E6
		public string getPikaWorldUserId()
		{
			return RegistryManager.Instance.PikaWorldId;
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x0007D0C8 File Offset: 0x0007B2C8
		public string getBootTime()
		{
			return RegistryManager.Instance.LastBootTime.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x0007D0EC File Offset: 0x0007B2EC
		public void getGamepadConnectionStatus()
		{
			Logger.Debug("Got call for getGamepadConnectionStatus");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetGamepadConnectionStatus"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x0007D13C File Offset: 0x0007B33C
		public string IsAnyAppRunning()
		{
			string text = "isAnyAppRunning";
			string text2;
			try
			{
				text2 = HTTPUtils.SendRequestToClient(text, null, "Android", 0, null, false, 1, 0, "bgp64").ToLower(CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				Logger.Error("An unexpected error occured in {0}. Err: {1}", new object[]
				{
					text,
					ex.ToString()
				});
				text2 = null;
			}
			return text2;
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x0007D1A8 File Offset: 0x0007B3A8
		public void goToMapsTab()
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GoToMapsTab"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x0007D1F0 File Offset: 0x0007B3F0
		public void clearmapsnotification()
		{
			Logger.Info("Got call from browser for maps clear notification");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ClearMapsNotification"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x0007D240 File Offset: 0x0007B440
		public void installapp(string appIcon, string appName, string apkUrl, string packageName)
		{
			Logger.Info("Get Call from browser of Install App :" + appName);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("InstallApp"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, appIcon);
				arguments.SetString(1, appName);
				arguments.SetString(2, apkUrl);
				arguments.SetString(3, packageName);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x0007D2C0 File Offset: 0x0007B4C0
		public string installapp(string appIcon, string appName, string apkUrl, string packageName, string timestamp)
		{
			Logger.Info("Get Call from browser of Install App with version :" + appName);
			string text;
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("InstallAppVersion"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, appIcon);
				arguments.SetString(1, appName);
				arguments.SetString(2, apkUrl);
				arguments.SetString(3, packageName);
				arguments.SetString(4, timestamp);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
				text = "true";
			}
			return text;
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x0007D354 File Offset: 0x0007B554
		public void installapp_google(string appIcon, string appName, string apkUrl, string packageName)
		{
			Logger.Info("Get Call from browser of Install App from googleplay :" + appName);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("InstallAppGooglePlay"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, appIcon);
				arguments.SetString(1, appName);
				arguments.SetString(2, apkUrl);
				arguments.SetString(3, packageName);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x0007D3D4 File Offset: 0x0007B5D4
		public void installapp_google_popup(string appIcon, string appName, string apkUrl, string packageName)
		{
			Logger.Info("Get Call from browser of Install App from googleplay in popup :" + appName);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("InstallAppGooglePlayPopup"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, appIcon);
				arguments.SetString(1, appName);
				arguments.SetString(2, apkUrl);
				arguments.SetString(3, packageName);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x0007D454 File Offset: 0x0007B654
		public void downloadinstalloem(string oem, string abiValue)
		{
			Logger.Info("Get Call from browser of downloadoem oem: " + oem + ", abiValue: " + abiValue);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("DownloadInstallOem"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, oem);
				arguments.SetString(1, abiValue);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x0007D4C8 File Offset: 0x0007B6C8
		public void canceloemdownload(string oem, string abiValue)
		{
			Logger.Info("Get Call from browser of canceloemdownload oem: " + oem + ", abiValue: " + abiValue);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CancelOemDownload"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, oem);
				arguments.SetString(1, abiValue);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x0007D53C File Offset: 0x0007B73C
		public void launchappindifferentoem(string oem, string abiValue, string vmname, string packageName, string actionWithRemainingInstances)
		{
			Logger.Info(string.Concat(new string[] { "Get Call from browser of launchappindifferentoem oem: ", oem, ", abiValue: ", abiValue, ", packageName: ", packageName, ", actionWithRemainingInstances: ", actionWithRemainingInstances }));
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("LaunchAppInDifferentOem"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, oem);
				arguments.SetString(1, abiValue);
				arguments.SetString(2, vmname);
				arguments.SetString(3, packageName);
				arguments.SetString(4, actionWithRemainingInstances);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014CC RID: 5324 RVA: 0x0007D5F8 File Offset: 0x0007B7F8
		public void uninstallapp(string packageName)
		{
			Logger.Info("Get Call from browser of Uninstall App for packagename :" + packageName);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("UninstallApp"))
			{
				cefProcessMessage.Arguments.SetString(0, packageName);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x0007D65C File Offset: 0x0007B85C
		public void getupdatedgrm()
		{
			Logger.Info("Got call from browser to get updated grm");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetUpdatedGrm"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x0007D6AC File Offset: 0x0007B8AC
		public void retryapkinstall(string apkFilePath)
		{
			Logger.Info("Get Call from browser of RetryApkInstall :" + apkFilePath);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("RetryApkInstall"))
			{
				cefProcessMessage.Arguments.SetString(0, apkFilePath);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x0007D710 File Offset: 0x0007B910
		public void chooseandinstallapk()
		{
			Logger.Info("Get Call from browser of ChooseAndInstallApk :");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ChooseAndInstallApk"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x0007D760 File Offset: 0x0007B960
		public void checkifpremium(string isPremium)
		{
			Logger.Info("Got call from blocker ad browser of premium subscription");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CheckIfPremium"))
			{
				cefProcessMessage.Arguments.SetString(0, isPremium);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0007D7C0 File Offset: 0x0007B9C0
		public void applyTheme(string themeName)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ApplyThemeName"))
			{
				cefProcessMessage.Arguments.SetString(0, themeName);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0000E5F2 File Offset: 0x0000C7F2
		public List<string> getsupportedactiontypes()
		{
			return Enum.GetNames(typeof(GenericAction)).ToList<string>();
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x0007D814 File Offset: 0x0007BA14
		public void getimpressionid(string impressionId)
		{
			Logger.Info("Get call from browser of impression_id :" + impressionId);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetImpressionId"))
			{
				cefProcessMessage.Arguments.SetString(0, impressionId);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
				cefProcessMessage.Dispose();
			}
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0007D880 File Offset: 0x0007BA80
		public string installedapps()
		{
			List<AppInfo> list = new JsonParser("Android").GetAppList().ToList<AppInfo>();
			string text = string.Empty;
			foreach (AppInfo appInfo in list)
			{
				text = text + appInfo.Package + ",";
			}
			return text;
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x0007D8F4 File Offset: 0x0007BAF4
		public string installedappsforvm(string vmName)
		{
			if (string.IsNullOrEmpty(vmName))
			{
				vmName = "Android";
			}
			List<AppInfo> list = new JsonParser(vmName).GetAppList().ToList<AppInfo>();
			string text = string.Empty;
			foreach (AppInfo appInfo in list)
			{
				text = text + appInfo.Package + ",";
			}
			return text;
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0007D974 File Offset: 0x0007BB74
		public void openapp(string appIcon, string appName, string apkUrl, string packageName)
		{
			Logger.Info("Get Call from browser of open App :" + appName);
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("InstallApp"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, appIcon);
				arguments.SetString(1, appName);
				arguments.SetString(2, apkUrl);
				arguments.SetString(3, packageName);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x0007D9F4 File Offset: 0x0007BBF4
		public void showdetails(string url)
		{
			using (CefBrowser browser = CefV8Context.GetCurrentContext().GetBrowser())
			{
				CefProcessMessage cefProcessMessage = CefProcessMessage.Create(url);
				browser.SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
				cefProcessMessage.Dispose();
			}
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x0007DA40 File Offset: 0x0007BC40
		public void searchappcenter(string searchString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("SearchAppcenter"))
			{
				cefProcessMessage.Arguments.SetString(0, searchString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00004786 File Offset: 0x00002986
		public void browser2Client()
		{
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x0000E608 File Offset: 0x0000C808
		public string getguid()
		{
			return RegistryManager.Instance.UserGuid;
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0000E608 File Offset: 0x0000C808
		public string GetUserGUID()
		{
			return RegistryManager.Instance.UserGuid;
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x0007DA94 File Offset: 0x0007BC94
		public void feedback(string email, string appName, string description, string downloadURl)
		{
			string clientVersion = RegistryManager.Instance.ClientVersion;
			description = description.Replace("&", " ");
			description += "\n From Client VER=";
			description += clientVersion;
			string text = string.Concat(new string[] { "-startwithparam \"", email, "&Others&", appName, "&", description, "&", downloadURl, "\"" });
			using (Process process = new Process())
			{
				process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
				Logger.Info("The arguments being passed to log collector is :{0}", new object[] { text });
				process.StartInfo.Arguments = text;
				process.Start();
			}
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x0007DB80 File Offset: 0x0007BD80
		public static void openLogCollector(string data = "")
		{
			if (!string.IsNullOrEmpty(data))
			{
				string clientInstallDir = RegistryManager.Instance.ClientInstallDir;
				if (!string.IsNullOrEmpty(clientInstallDir))
				{
					File.WriteAllText(Path.Combine(clientInstallDir, "logCollectorSourceData.txt"), data);
				}
			}
			string installDir = RegistryStrings.InstallDir;
			using (Process process = new Process())
			{
				process.StartInfo.FileName = Path.Combine(installDir, "HD-LogCollector.exe");
				if (!string.IsNullOrEmpty(MyCustomCefV8Handler.vmName))
				{
					process.StartInfo.Arguments = "-Vmname=" + MyCustomCefV8Handler.vmName;
				}
				Logger.Info("Open log collector through gmApi from dir: " + installDir);
				process.Start();
			}
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x0000E614 File Offset: 0x0000C814
		public void openLogCollector()
		{
			MyCustomCefV8Handler.openLogCollector(string.Empty);
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x0007DC34 File Offset: 0x0007BE34
		public void closesearch()
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CloseSearch"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x0007DC7C File Offset: 0x0007BE7C
		public void refresh_search()
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("RefreshSearch"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x0007DCC4 File Offset: 0x0007BEC4
		public void refresh_search(string searchString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("RefreshSearch"))
			{
				cefProcessMessage.Arguments.SetString(0, searchString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x0007DD18 File Offset: 0x0007BF18
		public void offlinehtmlhomeurl(string url)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("OfflineHtmlHomeUrl"))
			{
				cefProcessMessage.Arguments.SetString(0, url);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0007DD6C File Offset: 0x0007BF6C
		public void refreshhomehtml()
		{
			Logger.Info("Got call from browser to refresh home html");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("RefreshHomeHtml"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x0007DDBC File Offset: 0x0007BFBC
		public void setwebappversion(string version)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("SetWebAppVersion"))
			{
				cefProcessMessage.Arguments.SetString(0, version);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x0000E620 File Offset: 0x0000C820
		public string getwebappversion()
		{
			return RegistryManager.Instance.WebAppVersion;
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x0007DE10 File Offset: 0x0007C010
		public void google_search(string searchString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GoogleSearch"))
			{
				cefProcessMessage.Arguments.SetString(0, searchString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x0007DE64 File Offset: 0x0007C064
		public string getusertoken()
		{
			return new JObject
			{
				{
					"email",
					RegistryManager.Instance.RegisteredEmail
				},
				{
					"token",
					RegistryManager.Instance.Token
				}
			}.ToString(Formatting.None, new JsonConverter[0]);
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x00004786 File Offset: 0x00002986
		public void preinstallapp()
		{
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x0000E62C File Offset: 0x0000C82C
		public void openurl(string url)
		{
			Process.Start(url);
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x0000E635 File Offset: 0x0000C835
		public string prod_ver()
		{
			return RegistryManager.Instance.Version;
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0000E635 File Offset: 0x0000C835
		public string getengineguid()
		{
			return RegistryManager.Instance.Version;
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x0000E635 File Offset: 0x0000C835
		public string EngineVersion()
		{
			return RegistryManager.Instance.Version;
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x0000E641 File Offset: 0x0000C841
		public string ClientVersion()
		{
			return RegistryManager.Instance.ClientVersion;
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x0000E64D File Offset: 0x0000C84D
		public string InstallID()
		{
			return RegistryManager.Instance.InstallID;
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x0007DEB8 File Offset: 0x0007C0B8
		public string IsPremiumUser()
		{
			return RegistryManager.Instance.IsPremium.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x0007DEDC File Offset: 0x0007C0DC
		public string IsGoogleSigninDone(string vmName)
		{
			if (string.IsNullOrEmpty(vmName))
			{
				vmName = "Android";
			}
			if (!RegistryManager.Instance.Guest.ContainsKey(vmName))
			{
				return RegistryManager.Instance.Guest["Android"].IsGoogleSigninDone.ToString(CultureInfo.InvariantCulture);
			}
			return RegistryManager.Instance.Guest[vmName].IsGoogleSigninDone.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x0007DF54 File Offset: 0x0007C154
		public string IsOemAlreadyInstalled(string oem, string abi)
		{
			return InstalledOem.CheckIfOemInstancePresent(oem, abi).ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x0007DF78 File Offset: 0x0007C178
		public string CampaignName()
		{
			string text;
			try
			{
				string campaignJson = RegistryManager.Instance.CampaignJson;
				if (!string.IsNullOrEmpty(campaignJson))
				{
					JObject jobject = JObject.Parse(campaignJson);
					if (jobject["campaign_name"] != null)
					{
						return jobject["campaign_name"].ToString();
					}
				}
				text = "";
			}
			catch (Exception ex)
			{
				Logger.Error("error while sending campaign name in gm api: " + ex.ToString());
				text = "";
			}
			return text;
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x0000E659 File Offset: 0x0000C859
		public string CampaignJson()
		{
			return RegistryManager.Instance.CampaignJson;
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x0000E665 File Offset: 0x0000C865
		public string get_oem()
		{
			return RegistryManager.Instance.Oem;
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x0007DFF8 File Offset: 0x0007C1F8
		public string isAutomationEnabled()
		{
			return RegistryManager.Instance.EnableAutomation.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x0000E608 File Offset: 0x0000C808
		public string bgp_uuid()
		{
			return RegistryManager.Instance.UserGuid;
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x0000E671 File Offset: 0x0000C871
		public string BGPDevUrl()
		{
			return RegistryManager.Instance.BGPDevUrl;
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x0000E67D File Offset: 0x0000C87D
		public string DevCloudUrl()
		{
			return RegistryManager.Instance.Host;
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x0000E689 File Offset: 0x0000C889
		public string GetMachineId()
		{
			return GuidUtils.GetBlueStacksMachineId();
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x0000E690 File Offset: 0x0000C890
		public string GetVersionId()
		{
			return GuidUtils.GetBlueStacksVersionId();
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x0007E01C File Offset: 0x0007C21C
		public string SetFirebaseHost(string hostName)
		{
			object obj = MyCustomCefV8Handler.sLock;
			string text;
			lock (obj)
			{
				if (!string.IsNullOrEmpty(RegistryManager.Instance.CurrentFirebaseHost))
				{
					text = "false";
				}
				else
				{
					RegistryManager.Instance.CurrentFirebaseHost = hostName;
					text = "true";
				}
			}
			return text;
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x0007E07C File Offset: 0x0007C27C
		public void closeAnyPopup()
		{
			Logger.Info("Got call from browser of closeAnyPopup");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CloseAnyPopup"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x0007E0CC File Offset: 0x0007C2CC
		public void closeself()
		{
			Logger.Info("Got call from browser of closeself");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CloseSelf"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x0007E11C File Offset: 0x0007C31C
		public void closequitpopup()
		{
			Logger.Info("Got call from browser of closequitpopup");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CloseBrowserQuitPopup"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x0007E16C File Offset: 0x0007C36C
		public void downloadMacro(string macroData)
		{
			Logger.Info("Got call from browser of downloadmacro");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("DownloadMacro"))
			{
				cefProcessMessage.Arguments.SetString(0, macroData);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x0000E697 File Offset: 0x0000C897
		public string getCurrentMacros()
		{
			Logger.Info("Got call from browser of getcurrentmacros");
			return string.Join("|", BlueStacksUIUtils.GetMacroList().ToArray());
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x0000E6B7 File Offset: 0x0000C8B7
		public string uploadLocalMacro(string macroName)
		{
			Logger.Info("Got call from browser of uploadlocalmacro");
			return BlueStacksUIUtils.GetBase64MacroData(macroName);
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x0007E1CC File Offset: 0x0007C3CC
		public void performOTS()
		{
			Logger.Info("Got call from browser of performOTS");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("PerformOTS"))
			{
				cefProcessMessage.Arguments.SetString(0, this.mCallBackFunction);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x0007E230 File Offset: 0x0007C430
		public void changeControlScheme(string schemeSelected)
		{
			Logger.Info("Got call from browser of changeControlScheme");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ChangeControlScheme"))
			{
				cefProcessMessage.Arguments.SetString(0, schemeSelected);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x0007E290 File Offset: 0x0007C490
		public void closeOnBoarding(string json)
		{
			Logger.Info("Got call from browser of closeOnBoarding");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CloseOnBoarding"))
			{
				cefProcessMessage.Arguments.SetString(0, json);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x0007E2F0 File Offset: 0x0007C4F0
		public void browserLoaded()
		{
			Logger.Info("Got call from browser of browserLoaded");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("BrowserLoaded"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x0007E340 File Offset: 0x0007C540
		public void getSystemInfo()
		{
			Logger.Info("Got call from browser of getSystemInfo");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetSystemInfo"))
			{
				cefProcessMessage.Arguments.SetString(0, this.mCallBackFunction);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x0000E6C9 File Offset: 0x0000C8C9
		public void LogInfo(string info)
		{
			Logger.Info("HtmlLog: " + info);
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x0000E6DB File Offset: 0x0000C8DB
		public string GetSystemRAM()
		{
			return Profile.RAM;
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x0000E6E2 File Offset: 0x0000C8E2
		public string GetLocale()
		{
			return RegistryManager.Instance.UserSelectedLocale;
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x0000E6EE File Offset: 0x0000C8EE
		public string GetSystemCPU()
		{
			return Profile.CPU;
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x0000E6F5 File Offset: 0x0000C8F5
		public string GetSystemGPU()
		{
			return Profile.GPU;
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x0000E6FC File Offset: 0x0000C8FC
		public string GetSystemOS()
		{
			return Profile.OS;
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x0000E703 File Offset: 0x0000C903
		public string GetCurrentSessionId()
		{
			Logger.Info("In GetCurrentSessionId");
			return Stats.GetSessionId();
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x0007E3A4 File Offset: 0x0007C5A4
		public void showWebPage(string title, string webUrl)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ShowWebPage"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, title);
				arguments.SetString(1, webUrl);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x0007E404 File Offset: 0x0007C604
		public void HidePreview()
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("HidePreview"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x0007E44C File Offset: 0x0007C64C
		public void ShowPreview()
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ShowPreview"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x0007E494 File Offset: 0x0007C694
		public void StartObs(string callbackFunction)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("StartObs"))
			{
				cefProcessMessage.Arguments.SetString(0, callbackFunction);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x0007E4E8 File Offset: 0x0007C6E8
		public void StartStreamViewStatsRecorder(string label, string jsonString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("StartStreamViewStatsRecorder"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, label);
				arguments.SetString(1, jsonString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x0000E714 File Offset: 0x0000C914
		public string GetStreamConfig()
		{
			Logger.Info("In GetStreamConfig");
			return StreamManager.GetStreamConfig();
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x0007E548 File Offset: 0x0007C748
		public void LaunchDialog(string jsonString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("LaunchDialog"))
			{
				cefProcessMessage.Arguments.SetString(0, jsonString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x0007E59C File Offset: 0x0007C79C
		public void StartStreamV2(string jsonString, string callbackStreamStatus, string callbackTabChanged)
		{
			Logger.Info("Got StartStreamV2");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("StartStreamV2"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, jsonString);
				arguments.SetString(1, callbackStreamStatus);
				arguments.SetString(2, callbackTabChanged);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x0007E60C File Offset: 0x0007C80C
		public void StopStream()
		{
			Logger.Info("Got StopStream");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("StopStream"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x0000E725 File Offset: 0x0000C925
		public void StartRecord()
		{
			MyCustomCefV8Handler.StartRecordV2("{}");
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x0007E65C File Offset: 0x0007C85C
		public static void StartRecordV2(string jsonString)
		{
			Logger.Info("Got StartRecordV2");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("StartRecordV2"))
			{
				cefProcessMessage.Arguments.SetString(0, jsonString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x0007E6BC File Offset: 0x0007C8BC
		public void StopRecord()
		{
			Logger.Info("Got StopStream");
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("StopRecord"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x0007E70C File Offset: 0x0007C90C
		public void SetSystemVolume(string level)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("SetSystemVolume"))
			{
				cefProcessMessage.Arguments.SetString(0, level);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x0007E760 File Offset: 0x0007C960
		public void SetMicVolume(string level)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("SetMicVolume"))
			{
				cefProcessMessage.Arguments.SetString(0, level);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x0007E7B4 File Offset: 0x0007C9B4
		public void EnableWebcam(string width, string height, string position)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("EnableWebcam"))
			{
				Logger.Info("Got EnableWebcam");
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, width);
				arguments.SetString(1, height);
				arguments.SetString(2, position);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x0007E824 File Offset: 0x0007CA24
		public void DisableWebcamV2(string jsonString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("DisableWebcamV2"))
			{
				Logger.Info("Got DisableWebcamV2");
				cefProcessMessage.Arguments.SetString(0, jsonString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x0007E884 File Offset: 0x0007CA84
		public void MoveWebcam(string horizontal, string vertical)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("MoveWebcam"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, horizontal);
				arguments.SetString(1, vertical);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x0000E731 File Offset: 0x0000C931
		public void SetStreamName(string name)
		{
			Logger.Info("Got SetStreamName: " + name);
			RegistryManager.Instance.StreamName = name;
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x0000E74E File Offset: 0x0000C94E
		public void SetServerLocation(string location)
		{
			Logger.Info("Got SetServerLocation: " + location);
			RegistryManager.Instance.ServerLocation = location;
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x0000E76B File Offset: 0x0000C96B
		public void SetChannelName(string channelName)
		{
			RegistryManager.Instance.ChannelName = channelName;
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x0007E8E4 File Offset: 0x0007CAE4
		public string GetRealtimeAppUsage()
		{
			string text;
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetRealtimeAppUsage"))
			{
				cefProcessMessage.Arguments.SetString(0, this.mCallBackFunction);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
				text = "";
			}
			return text;
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x0007E944 File Offset: 0x0007CB44
		public string GetInstalledAppsJsonforJS()
		{
			string text;
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetInstalledAppsJsonforJS"))
			{
				cefProcessMessage.Arguments.SetString(0, this.mCallBackFunction);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
				text = "";
			}
			return text;
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x0007E9A4 File Offset: 0x0007CBA4
		public string GetInstalledAppsForAllOems()
		{
			string text;
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetInstalledAppsForAllOems"))
			{
				cefProcessMessage.Arguments.SetString(0, this.mCallBackFunction);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
				text = "";
			}
			return text;
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x0007EA04 File Offset: 0x0007CC04
		public string GetCurrentAppInfo()
		{
			string text;
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("GetCurrentAppInfo"))
			{
				cefProcessMessage.Arguments.SetString(0, this.mCallBackFunction);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
				text = "";
			}
			return text;
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x0007EA64 File Offset: 0x0007CC64
		public string GetGMPort()
		{
			Logger.Info("In GetGMPort");
			return RegistryManager.Instance.PartnerServerPort.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x0000E778 File Offset: 0x0000C978
		public string ResetSessionId()
		{
			Logger.Info("In ResetSessionId");
			return Stats.ResetSessionId();
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x0007EA94 File Offset: 0x0007CC94
		public void makeWebCall(string url, string scriptToInvoke)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("makeWebCall"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, url);
				arguments.SetString(1, scriptToInvoke);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0000E789 File Offset: 0x0000C989
		public void ShowWebPageInBrowser(string url)
		{
			Logger.Info("Showing " + url + " in default browser");
			BlueStacksUIUtils.OpenUrl(url);
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x0007EAF4 File Offset: 0x0007CCF4
		public void DialogClickHandler(string jsonString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("DialogClickHandler"))
			{
				cefProcessMessage.Arguments.SetString(0, jsonString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x0007EB48 File Offset: 0x0007CD48
		public void CloseDialog(string jsonString)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CloseDialog"))
			{
				cefProcessMessage.Arguments.SetString(0, jsonString);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x0007EB9C File Offset: 0x0007CD9C
		public void ShowAdvancedSettings()
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ShowAdvancedSettings"))
			{
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x0007EBE4 File Offset: 0x0007CDE4
		public void LaunchFilterWindow(string channel, string sessionId)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("LaunchFilterWindow"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, channel);
				arguments.SetString(1, sessionId);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x0007EC44 File Offset: 0x0007CE44
		public void ChangeFilterTheme(string theme)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("ChangeFilterTheme"))
			{
				cefProcessMessage.Arguments.SetString(0, theme);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x0007EC98 File Offset: 0x0007CE98
		public void UpdateThemeSettings(string currentTheme, string settingsJson)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("UpdateThemeSettings"))
			{
				CefListValue arguments = cefProcessMessage.Arguments;
				arguments.SetString(0, currentTheme);
				arguments.SetString(1, settingsJson);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x0007ECF8 File Offset: 0x0007CEF8
		public void CloseFilterWindow(string jsonArray)
		{
			using (CefProcessMessage cefProcessMessage = CefProcessMessage.Create("CloseFilterWindow"))
			{
				cefProcessMessage.Arguments.SetString(0, jsonArray);
				CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, cefProcessMessage);
			}
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x0000E7A6 File Offset: 0x0000C9A6
		public string IsFacebook()
		{
			if (string.Equals(RegistryManager.Instance.BtvNetwork, "facebook", StringComparison.InvariantCulture))
			{
				return "true";
			}
			return "false";
		}

		// Token: 0x04000D02 RID: 3330
		private static object sLock = new object();

		// Token: 0x04000D03 RID: 3331
		private static string vmName = "";

		// Token: 0x04000D04 RID: 3332
		private string mCallBackFunction;
	}
}
