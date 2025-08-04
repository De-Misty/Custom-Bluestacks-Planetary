using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200022E RID: 558
	internal class HTTPHandler
	{
		// Token: 0x06001542 RID: 5442 RVA: 0x0000E850 File Offset: 0x0000CA50
		private static void WriteSuccessJsonArray(HttpListenerResponse res)
		{
			HTTPUtils.Write(new JArray
			{
				new JObject
				{
					new JProperty("success", true)
				}
			}.ToString(Formatting.None, new JsonConverter[0]), res);
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x0007F2A8 File Offset: 0x0007D4A8
		private static void WriteErrorJsonArray(string reason, HttpListenerResponse res)
		{
			HTTPUtils.Write(new JArray
			{
				new JObject
				{
					new JProperty("success", false),
					new JProperty("reason", reason)
				}
			}.ToString(Formatting.None, new JsonConverter[0]), res);
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x0000E88A File Offset: 0x0000CA8A
		private static void WriteErrorJSONObjectWithoutReason(HttpListenerResponse res)
		{
			HTTPUtils.Write(new JObject { { "success", false } }.ToString(Formatting.None, new JsonConverter[0]), res);
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x0000E8B4 File Offset: 0x0000CAB4
		public static void PingHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			HTTPHandler.WriteSuccessJsonWithVmName(HTTPUtils.ParseRequest(req).RequestVmName, res);
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x0007F300 File Offset: 0x0007D500
		internal static void EnableWndProcLogging(HttpListenerRequest _1, HttpListenerResponse _2)
		{
			try
			{
				WindowWndProcHandler.isLogWndProc = !WindowWndProcHandler.isLogWndProc;
				Logger.Info("Got request for EnableWndProcLogging" + WindowWndProcHandler.isLogWndProc.ToString());
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in set EnableWndProcLogging... Err : " + ex.ToString());
			}
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x0007F360 File Offset: 0x0007D560
		internal static void EnableKeyboardHookLogging(HttpListenerRequest _1, HttpListenerResponse _2)
		{
			try
			{
				GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging = !GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in set EnableKeyboardHookLogging... Err : " + ex.ToString());
			}
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x0007F3A4 File Offset: 0x0007D5A4
		internal static void EnableDebugLogs(HttpListenerRequest _1, HttpListenerResponse res)
		{
			try
			{
				Logger.EnableDebugLogs();
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in EnableDebugLogs... Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x0007F3EC File Offset: 0x0007D5EC
		public static void SendAppDisplayed(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				JObject jobject = new JObject();
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					jobject.Add("success", BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.mAppDisplayedOccured);
				}
				HTTPUtils.Write(jobject.ToString(Formatting.None, new JsonConverter[0]), res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server SendAppDisplayed. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x0007F488 File Offset: 0x0007D688
		internal static void RestartFrontend(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].RestartFrontend();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server RestartFrontend. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x0007F4F4 File Offset: 0x0007D6F4
		internal static void GCCollect(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				HTTPHandler.WriteSuccessJsonWithVmName(requestData.RequestVmName, res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server GCCollect. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x0007F554 File Offset: 0x0007D754
		public static void IsBlueStacksUIVisible(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				JObject jobject = new JObject();
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					jobject.Add("success", BlueStacksUIUtils.DictWindows[requestData.RequestVmName].IsVisible);
				}
				HTTPUtils.Write(jobject.ToString(Formatting.None, new JsonConverter[0]), res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server IsBlueStacksUIVisible. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x0007F5E8 File Offset: 0x0007D7E8
		internal static void ToggleFarmMode(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				CommonHandlers.ToggleFarmMode(bool.Parse(requestData.Data["state"]));
				HTTPHandler.WriteSuccessJsonWithVmName(requestData.RequestVmName, res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server ToggleFarmMode. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x0007F650 File Offset: 0x0007D850
		internal static void ToggleStreamingMode(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				bool state = bool.Parse(requestData.Data["state"]);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					MainWindow mWindow = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
					mWindow.Dispatcher.Invoke(new Action(delegate
					{
						mWindow.mTopBar.mPreferenceDropDownControl.ToggleStreamingMode(state);
						mWindow.mFrontendHandler.ToggleStreamingMode(state);
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonWithVmName(requestData.RequestVmName, res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server ToggleStreamingMode. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x0007F70C File Offset: 0x0007D90C
		internal static void GamepadGuidanceButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					MainWindow mWindow = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
					if (mWindow != BlueStacksUIUtils.LastActivatedWindow || (RegistryManager.Instance.GamepadDetectionEnabled && mWindow.IsGamepadConnected && mWindow.mTopBar.mAppTabButtons.SelectedTab.mIsNativeGamepadEnabledForApp))
					{
						return;
					}
					mWindow.Dispatcher.Invoke(new Action(delegate
					{
						if (KMManager.CheckIfKeymappingWindowVisible(true))
						{
							KMManager.CloseWindows();
							mWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
							return;
						}
						if (!KeymapCanvasWindow.sIsDirty)
						{
							KMManager.HandleInputMapperWindow(mWindow, "gamepad");
						}
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonWithVmName(requestData.RequestVmName, res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GamepadGuidanceButtonHandler. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x0007F7F4 File Offset: 0x0007D9F4
		internal static void SetCurrentVolumeFromAndroidHandler(HttpListenerRequest req, HttpListenerResponse _)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				int num = Convert.ToInt32(requestData.Data["volume"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestVmName].Utils.SetVolumeLevelFromAndroid(num);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to set volume level. Er : " + ex.ToString());
			}
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x0007F878 File Offset: 0x0007DA78
		internal static void ReinitRegistry(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RegistryManager.ClearRegistryMangerInstance();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to reinit registry. Err : " + ex.ToString());
			}
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x0007F8B4 File Offset: 0x0007DAB4
		internal static void UpdateCrc(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestVmName].Dispatcher.Invoke(new Action(delegate
					{
						uint num;
						float num2;
						float num3;
						if (uint.TryParse(requestData.Data["Crc"], out num) && float.TryParse(requestData.Data["X"], out num2) && float.TryParse(requestData.Data["Y"], out num3))
						{
							string text = string.Format(CultureInfo.InvariantCulture, "X: {0}   Y: {1}   Crc: {2}", new object[]
							{
								num2.ToString(CultureInfo.InvariantCulture),
								num3.ToString(CultureInfo.InvariantCulture),
								num.ToString("X", CultureInfo.InvariantCulture)
							});
							Logger.Info("IMAGEPICKER: " + text);
							global::System.Windows.Forms.MessageBox.Show(text);
							global::System.Windows.Forms.Clipboard.SetText(text);
						}
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed in UpdateCrc. Err : " + ex.ToString());
			}
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0007F940 File Offset: 0x0007DB40
		internal static void ConfigFileChanged(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				string config = requestData.Data["config"];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					MainWindow window = BlueStacksUIUtils.DictWindows[requestVmName];
					window.Dispatcher.Invoke(new Action(delegate
					{
						window.mTopBar.SetConfigIndicator(config);
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to set GameInfo err : " + ex.ToString());
			}
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0007F9E4 File Offset: 0x0007DBE4
		internal static void CheckCallbackEnabledStatus(HttpListenerRequest req, HttpListenerResponse res)
		{
			JArray jarray = new JArray();
			JObject jobject = new JObject();
			try
			{
				string text = HTTPUtils.ParseRequest(req).Data["vmname"];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(text))
				{
					MainWindow mainWindow = BlueStacksUIUtils.DictWindows[text];
					Logger.Info("Callback: vmname: " + text + " value: " + mainWindow.mCallbackEnabled);
					jobject.Add("success", true);
					jobject.Add("Enabled", mainWindow.mCallbackEnabled);
					jarray.Add(jobject);
					HTTPUtils.Write(jarray.ToString(Formatting.None, new JsonConverter[0]), res);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to get callback status. Err : " + ex.Message);
				jobject.Add("success", false);
				jobject.Add("status", ex.Message);
				jarray.Add(jobject);
				HTTPUtils.Write(jarray.ToString(Formatting.None, new JsonConverter[0]), res);
			}
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0007FAF8 File Offset: 0x0007DCF8
		internal static void AddNotificationInDrawer(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.Data["vmname"];
				string text = requestData.Data["pkg"];
				string text2 = requestData.Data["app_name"];
				string text3 = requestData.Data["msg"];
				string text4 = requestData.Data["id"];
				string text5 = "bluestackslogo";
				string text6 = text5;
				JsonParser jsonParser = new JsonParser(vmName);
				try
				{
					if (!string.IsNullOrEmpty(text6))
					{
						AppInfo appInfoFromPackageName = jsonParser.GetAppInfoFromPackageName(text);
						if (appInfoFromPackageName != null)
						{
							if (File.Exists(Path.Combine(RegistryStrings.GadgetDir, appInfoFromPackageName.Img)))
							{
								text5 = Path.Combine(RegistryStrings.GadgetDir, appInfoFromPackageName.Img);
								text6 = appInfoFromPackageName.Img;
							}
						}
						else
						{
							Logger.Info("GetAppInfoFromAppName returns false");
						}
					}
				}
				catch
				{
					Logger.Error("Error loading app icon file");
				}
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					MainWindow mainWindow = BlueStacksUIUtils.DictWindows[vmName];
					GenericNotificationItem genericNotificationItem = new GenericNotificationItem();
					genericNotificationItem.Title = text2;
					genericNotificationItem.Message = text3;
					genericNotificationItem.ShowRibbon = true;
					genericNotificationItem.NotificationMenuImageUrl = text5;
					genericNotificationItem.NotificationMenuImageName = text5;
					genericNotificationItem.IsAndroidNotification = true;
					genericNotificationItem.Id = text4;
					genericNotificationItem.VmName = vmName;
					genericNotificationItem.Package = text;
					if (text == null)
					{
						text = Strings.ProductDisplayName;
					}
					if (string.Equals(text2, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
					{
						if (BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.ContainsKey(Strings.ProductDisplayName))
						{
							Dictionary<string, int> appNotificationCountDictForEachVM = BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM;
							string text7 = Strings.ProductDisplayName;
							int num = appNotificationCountDictForEachVM[text7];
							appNotificationCountDictForEachVM[text7] = num + 1;
						}
						else
						{
							BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.Add(Strings.ProductDisplayName, 1);
						}
					}
					else if (BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.ContainsKey(text))
					{
						Dictionary<string, int> appNotificationCountDictForEachVM2 = BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM;
						string text7 = text;
						int num = appNotificationCountDictForEachVM2[text7];
						appNotificationCountDictForEachVM2[text7] = num + 1;
					}
					else
					{
						BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.Add(text, 1);
					}
					GenericNotificationManager.AddNewNotification(genericNotificationItem, false);
					Predicate<GenericNotificationItem> <>9__1;
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
						Predicate<GenericNotificationItem> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = (GenericNotificationItem x) => !x.IsDeleted && (string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
						}
						SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems(predicate);
						BlueStacksUIUtils.DictWindows[vmName].mTopBar.mNotificationDrawerControl.Populate(notificationItems);
						if (BlueStacksUIUtils.DictWindows[vmName].WindowState == WindowState.Minimized)
						{
							BlueStacksUIUtils.SetWindowTaskbarIcon(BlueStacksUIUtils.DictWindows[vmName]);
						}
					}), new object[0]);
				}
				if (RegistryManager.Instance.IsNotificationSoundsActive && BlueStacksUIUtils.DictWindows[vmName].StaticComponents.mSelectedTabButton.mTabType != TabType.AppTab)
				{
					MediaPlayer mediaPlayer = new MediaPlayer();
					mediaPlayer.Open(new Uri(Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "NotificationSound.wav")));
					mediaPlayer.Play();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed in UpdateCrc. Err : " + ex.ToString());
			}
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x0007FE3C File Offset: 0x0007E03C
		internal static void MarkNotificationInDrawer(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.Data["vmname"];
				string text = requestData.Data["id"];
				List<string> list = new List<string>();
				list.Add(text);
				GenericNotificationManager.MarkNotification(list, delegate(GenericNotificationItem x)
				{
					x.IsRead = true;
				});
				Predicate<GenericNotificationItem> <>9__2;
				BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
				{
					BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
					Predicate<GenericNotificationItem> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = (GenericNotificationItem x) => !x.IsDeleted && (string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
					}
					SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems(predicate);
					BlueStacksUIUtils.DictWindows[vmName].mTopBar.mNotificationDrawerControl.Populate(notificationItems);
					if (BlueStacksUIUtils.DictWindows[vmName].WindowState == WindowState.Minimized)
					{
						BlueStacksUIUtils.SetWindowTaskbarIcon(BlueStacksUIUtils.DictWindows[vmName]);
					}
				}), new object[0]);
			}
			catch (Exception ex)
			{
				string text2 = "Error in marking notification read: ";
				Exception ex2 = ex;
				Logger.Error(text2 + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x0007FF0C File Offset: 0x0007E10C
		internal static void NCSetGameInfoOnTopBarHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				string gameName = requestData.Data["game"];
				string characterName = requestData.Data["character"];
				MainWindow mWindow = BlueStacksUIUtils.DictWindows[requestVmName];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					mWindow.Dispatcher.Invoke(new Action(delegate
					{
						mWindow.mNCTopBar.mAppName.Text = gameName;
						mWindow.mNCTopBar.mAppName.ToolTip = gameName;
						mWindow.mNCTopBar.mGamenameSeparator.Visibility = Visibility.Visible;
						mWindow.mNCTopBar.mCharacterName.Text = characterName;
						mWindow.mNCTopBar.mCharacterName.ToolTip = characterName;
					}), new object[0]);
					HTTPHandler.WriteSuccessJsonWithVmName(requestVmName, res);
				}
				else
				{
					HTTPHandler.WriteErrorJsonArray("Client Instance not running", res);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to set GameInfo err : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x0007FFE4 File Offset: 0x0007E1E4
		internal static void OpenCFGReorderTool(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0].Dispatcher.Invoke(new Action(delegate
				{
					CFGReorderWindow.Instance.Show();
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't open cfg reorder window. Ex: {0}", new object[] { ex });
			}
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x00080060 File Offset: 0x0007E260
		internal static void OpenThemeEditor(HttpListenerRequest _1, HttpListenerResponse _2)
		{
			try
			{
				if (RegistryManager.Instance.OpenThemeEditor)
				{
					BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0].Dispatcher.Invoke(new Action(delegate
					{
						ThemeEditorWindow.Instance.Show();
					}), new object[0]);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x000800D4 File Offset: 0x0007E2D4
		internal static void MuteAllInstancesHandler(HttpListenerRequest req, HttpListenerResponse _)
		{
			bool flag = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["muteInstance"], CultureInfo.InvariantCulture);
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
			{
				if (flag)
				{
					mainWindow.Utils.MuteApplication(true);
				}
				else
				{
					mainWindow.Utils.UnmuteApplication(true);
				}
			}
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x00080164 File Offset: 0x0007E364
		internal static void AccountSetupCompleted(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName) && FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					NCSoftUtils.Instance.SendGoogleLoginEventAsync(requestVmName);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in AccountSetupCompleted Handler: " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x000801E0 File Offset: 0x0007E3E0
		internal static void GetHeightWidth(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && BlueStacksUIUtils.DictWindows[vmName] != null)
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						try
						{
							MainWindow mainWindow = BlueStacksUIUtils.DictWindows[vmName];
							JArray jarray = new JArray();
							JObject jobject = new JObject
							{
								new JProperty("success", true)
							};
							JObject jobject2 = new JObject
							{
								new JProperty("cHeight", mainWindow.ActualHeight),
								new JProperty("cWidth", mainWindow.ActualWidth),
								new JProperty("gHeight", mainWindow.mContentGrid.ActualHeight),
								new JProperty("gWidth", mainWindow.mContentGrid.ActualWidth)
							};
							jarray.Add(jobject);
							jarray.Add(new JObject
							{
								new JProperty("result", jobject2)
							});
							HTTPUtils.Write(jarray.ToString(Formatting.None, new JsonConverter[0]), res);
						}
						catch (Exception ex2)
						{
							Logger.Error("Some error in finding MainWindow instance err: " + ex2.ToString());
							HTTPHandler.WriteErrorJsonArray(ex2.Message, res);
						}
					}), new object[0]);
				}
				else
				{
					HTTPHandler.WriteErrorJsonArray("Client Instance not running", res);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to GetHeightWidth err : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x000802AC File Offset: 0x0007E4AC
		internal static void ScreenLock(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				bool lockScreen = Convert.ToBoolean(requestData.Data["lock"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						if (lockScreen)
						{
							BlueStacksUIUtils.DictWindows[vmName].ShowLockScreen();
							return;
						}
						BlueStacksUIUtils.DictWindows[vmName].HideLockScreen();
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to lock screen err : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x00080370 File Offset: 0x0007E570
		internal static void SetStreamingStatus(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				string status = requestData.Data["status"];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.SetNcSoftStreamingStatus(status);
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to SetStreamingStatus err : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x00080428 File Offset: 0x0007E628
		internal static void PlayerScriptModifierKeyUp(HttpListenerRequest req, HttpListenerResponse _)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				double x = Convert.ToDouble(requestData.Data["X"], CultureInfo.InvariantCulture);
				double y = Convert.ToDouble(requestData.Data["Y"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.AddCoordinatesToScriptText(x, y);
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to handle player script modifier key up: " + ex.ToString());
			}
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x000804F8 File Offset: 0x0007E6F8
		internal static void LaunchPlay(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string text = requestData.Data["vmname"];
				string text2 = requestData.Data["package"];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(text))
				{
					BlueStacksUIUtils.DictWindows[text].Utils.HandleLaunchPlay(text2);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to launch play store err : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x0008058C File Offset: 0x0007E78C
		internal static void FullScreenSidebarHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				bool isVisible = Convert.ToBoolean(requestData.Data["visible"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					MainWindow window = BlueStacksUIUtils.ActivatedWindow;
					if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
					{
						window.Dispatcher.Invoke(new Action(delegate
						{
							window.mSidebar.ToggleSidebarVisibilityInFullscreen(isVisible);
						}), new object[0]);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in FullScreenSidebarHandler : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x00080668 File Offset: 0x0007E868
		internal static void HideTopSideBarHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
			{
				MainWindow window = BlueStacksUIUtils.ActivatedWindow;
				if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
				{
					window.Dispatcher.Invoke(new Action(delegate
					{
						window.mSidebar.HideSideBarInFullscreen();
						window.mTopbarOptions.HideTopBarInFullscreen();
					}), new object[0]);
				}
			}
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x000806EC File Offset: 0x0007E8EC
		internal static void FullScreenTopbarButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				bool isVisible = Convert.ToBoolean(requestData.Data["visible"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					MainWindow window = BlueStacksUIUtils.ActivatedWindow;
					if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
					{
						window.Dispatcher.Invoke(new Action(delegate
						{
							window.mTopbarOptions.ToggleTopbarButtonVisibilityInFullscreen(isVisible);
						}), new object[0]);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in FullScreenTopbarButtonHandler : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x000807C8 File Offset: 0x0007E9C8
		internal static void FullScreenSidebarButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				bool isVisible = Convert.ToBoolean(requestData.Data["visible"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					MainWindow window = BlueStacksUIUtils.ActivatedWindow;
					if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
					{
						window.Dispatcher.Invoke(new Action(delegate
						{
							window.mSidebar.ToggleSidebarButtonVisibilityInFullscreen(isVisible);
						}), new object[0]);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Errro in FullScreenSidebarButtonHandler : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x000808A4 File Offset: 0x0007EAA4
		internal static void FullScreenTopbarHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				bool isVisible = Convert.ToBoolean(requestData.Data["visible"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					MainWindow window = BlueStacksUIUtils.DictWindows[requestVmName];
					if (window != null && window.mIsFullScreen && !window.mFrontendHandler.IsShootingModeActivated)
					{
						window.Dispatcher.Invoke(new Action(delegate
						{
							if (!window.mTopBarPopup.IsOpen & isVisible)
							{
								window.mTopBarPopup.IsOpen = true;
								return;
							}
							window.mTopBarPopup.IsOpen = false;
						}), new object[0]);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error FullScreenTopbarHandler : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00080984 File Offset: 0x0007EB84
		internal static void HandleGamepadConnection(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				bool flag = bool.Parse(requestData.Data["status"]);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].IsGamepadConnected = flag;
					if (!HTTPHandler.mSendGamepadStats)
					{
						ClientStats.SendMiscellaneousStatsAsync("GamePadConnectedStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, null, null, null, null, null, null);
						HTTPHandler.mSendGamepadStats = true;
					}
				}
				HTTPHandler.WriteSuccessJsonWithVmName(requestData.RequestVmName, res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleGamepadConnection. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00080A44 File Offset: 0x0007EC44
		internal static void TileWindow(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				HTTPUtils.ParseRequest(req);
				CommonHandlers.ArrangeWindowInTiles();
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in tiling window. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x00080A94 File Offset: 0x0007EC94
		internal static void CascadeWindow(HttpListenerRequest _, HttpListenerResponse res)
		{
			try
			{
				CommonHandlers.ArrangeWindowInCascade();
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Cascading window. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x00080AE4 File Offset: 0x0007ECE4
		internal static void UpdateLocale(HttpListenerRequest req, HttpListenerResponse res)
		{
			Logger.Info("Got UpdateLocale {0} request from {1}", new object[]
			{
				req.HttpMethod,
				req.RemoteEndPoint.ToString()
			});
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				string text = requestData.Data["locale"].ToString(CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					RegistryManager.Instance.UserSelectedLocale = text;
					Utils.UpdateValueInBootParams("LANG", text, requestVmName, false, "bgp64");
					BlueStacksUIUtils.DictWindows[requestVmName].Dispatcher.Invoke(new Action(delegate
					{
						LocaleStrings.InitLocalization(null, "Android", false);
					}), new object[0]);
					HTTPUtils.SendRequestToAgentAsync("reinitlocalization", null, "Android", 0, null, false, 1, 0, "bgp64");
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in UpdateLocale: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x00080BFC File Offset: 0x0007EDFC
		internal static void ScreenshotCaptured(HttpListenerRequest req, HttpListenerResponse res)
		{
			Logger.Info("Got ScreenshotCaptured {0} request from {1}", new object[]
			{
				req.HttpMethod,
				req.RemoteEndPoint.ToString()
			});
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				string path = requestData.Data["path"].ToString(CultureInfo.InvariantCulture);
				string text = requestData.Data["showSavedInfo"];
				if (text == null)
				{
					text = "false";
				}
				bool showSaved;
				bool.TryParse(text, out showSaved);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.PostScreenShotWork(path, showSaved);
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ScreenshotCaptured: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x00080D0C File Offset: 0x0007EF0C
		internal static void ClientHotkeyHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				string text = requestData.Data["keyevent"].ToString(CultureInfo.InvariantCulture);
				ClientHotKeys clientHotKey = (ClientHotKeys)Enum.Parse(typeof(ClientHotKeys), text);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].HandleClientHotKey(clientHotKey);
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ClientHotkeyHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x00080DE4 File Offset: 0x0007EFE4
		internal static void AndroidLocaleChanged(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				BlueStacksUIUtils.UpdateLocale(RegistryManager.Instance.UserSelectedLocale, requestData.RequestVmName);
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in AndroidLocaleChanged. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x00080E44 File Offset: 0x0007F044
		internal static void HandleClientOperation(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					string text = requestData.Data["data"];
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mCommonHandler.HandleClientOperation(text);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleClientOperation. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x00080ECC File Offset: 0x0007F0CC
		internal static void MacroPlaybackCompleteHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].SetMacroPlayBackEventHandle();
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in MacroPlaybackCompleteHandler. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x00080F64 File Offset: 0x0007F164
		internal static void HandleClientGamepadButtonHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						string text = requestData.Data["data"];
						bool flag;
						if (bool.TryParse(requestData.Data["isDown"], out flag))
						{
							KMManager.UpdateUIForGamepadEvent(text, flag);
							return;
						}
						Logger.Error("Error in HandleClientGamepadButtonHandler: Could not parse gamepad event isDown:'{0}'", new object[] { requestData.Data["isDown"] });
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleClientGamepadButtonHandler. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x00080FFC File Offset: 0x0007F1FC
		internal static void SaveComboEvents(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string events = requestData.Data["events"];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mIsMacroRecorderActive)
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].MacroRecorderWindow.SaveOperation(events);
					}
					else
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
						{
							KMManager.mComboEvents = events;
						}), new object[0]);
					}
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SaveComboEvents. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x000810DC File Offset: 0x0007F2DC
		internal static void MacroCompleted(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].MacroOverlayControl.ShowPromptAndHideOverlay();
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in MacroCompleted. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x0008117C File Offset: 0x0007F37C
		internal static void ShowMaintenanceWarning(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				string message = requestData.Data["message"];
				HTTPHandler.WriteJSON(new Dictionary<string, string> { { "result_code", "0" } }, res);
				BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("BlueStacks", "") + " " + LocaleStrings.GetLocalizedString("STRING_WARNING", "");
					customMessageWindow.BodyTextBlock.Text = message;
					customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), null, null, false, null);
					customMessageWindow.Owner = BlueStacksUIUtils.DictWindows[vmName];
					BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay(null);
					customMessageWindow.ShowDialog();
					BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to ShowMaintenanceWarning app... Err : " + ex.ToString());
				HTTPHandler.WriteJSON(new Dictionary<string, string> { { "result_code", "-1" } }, res);
			}
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x0000E8C7 File Offset: 0x0000CAC7
		internal static void WriteJSON(Dictionary<string, string> data, HttpListenerResponse res)
		{
			HTTPUtils.Write(JSONUtils.GetJSONArrayString(data), res);
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x00081248 File Offset: 0x0007F448
		internal static void LaunchDefaultWebApp(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string text = requestData.Data["action"];
				Logger.Info("Action : " + text);
				if (text != null)
				{
					if (!(text == "browser"))
					{
						if (!(text == "email"))
						{
							goto IL_023A;
						}
					}
					else
					{
						string text2 = requestData.Data["url"];
						Logger.Info("Url : " + text2);
						try
						{
							Process.Start(text2);
							HTTPHandler.WriteSuccessJsonArray(res);
							goto IL_0245;
						}
						catch
						{
							HTTPHandler.WriteErrorJsonArray("Invalid or empty url", res);
							goto IL_0245;
						}
					}
					string text3 = "";
					string text4 = "";
					string text5 = "";
					string text6 = "";
					string text7 = "";
					string text8 = "";
					try
					{
						text3 = requestData.Data["to"];
						text4 = requestData.Data["cc"];
						text5 = requestData.Data["bcc"];
						text6 = requestData.Data["message"];
						text7 = requestData.Data["subject"];
						text8 = requestData.Data["mailto"];
					}
					catch
					{
					}
					bool flag = false;
					if (!string.IsNullOrEmpty(text3))
					{
						flag = text3.Split(new char[] { '@' }).Length > 1;
					}
					Logger.Info(string.Concat(new string[] { "to : ", text3, ", cc : ", text4, ", bcc : ", text5, ", subject = ", text7 }));
					Logger.Info("mailto : " + text8);
					string text9;
					if (flag)
					{
						text9 = string.Concat(new string[] { "mailto:", text3, "?cc=", text4, "&bcc=", text5, "&subject=", text7, "&body=", text6 });
					}
					else
					{
						if (string.IsNullOrEmpty(text8))
						{
							HTTPHandler.WriteErrorJsonArray("to and mailto field cannot be empty", res);
							goto IL_0245;
						}
						text9 = text8;
					}
					Logger.Info("mail to request : " + text9);
					Process.Start(text9);
					HTTPHandler.WriteSuccessJsonArray(res);
					goto IL_0245;
				}
				IL_023A:
				HTTPHandler.WriteErrorJsonArray("wrong or empty action", res);
				IL_0245:;
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to LaunchDefaultWebApp app... Err : " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x00081510 File Offset: 0x0007F710
		public static void GetRunningInstances(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				List<string> list = new List<string>(BlueStacksUIUtils.DictWindows.Keys);
				HTTPUtils.ParseRequest(req);
				JObject jobject = new JObject();
				string text = string.Join(",", list.ToArray());
				Logger.Info("Running instances: " + text);
				jobject.Add("success", true);
				jobject.Add("vmname", text);
				HTTPUtils.Write(jobject.ToString(Formatting.None, new JsonConverter[0]), res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetRunningInstances");
				Logger.Error(ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x000815BC File Offset: 0x0007F7BC
		internal static void IsAnyAppRunning(HttpListenerRequest _1, HttpListenerResponse res)
		{
			try
			{
				JObject jobject = new JObject { { "success", true } };
				bool isAppRunning = false;
				if (BlueStacksUIUtils.DictWindows.Count > 0)
				{
					MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
					window.Dispatcher.Invoke(new Action(delegate
					{
						isAppRunning = window.mTopBar.mAppTabButtons.IsAppRunning();
					}), new object[0]);
					jobject.Add("isanyapprunning", isAppRunning);
				}
				HTTPUtils.Write(jobject.ToString(Formatting.None, new JsonConverter[0]), res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in IsAnyAppRunning: {0}", new object[] { ex });
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x000816A0 File Offset: 0x0007F8A0
		internal static void GetCurrentAppDetails(HttpListenerRequest _1, HttpListenerResponse res)
		{
			try
			{
				JObject jobject = new JObject { { "success", true } };
				if (BlueStacksUIUtils.DictWindows.Count > 0)
				{
					MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
					string pkg = string.Empty;
					string appName = string.Empty;
					string tabType = string.Empty;
					window.Dispatcher.Invoke(new Action(delegate
					{
						pkg = window.mTopBar.mAppTabButtons.SelectedTab.PackageName;
						appName = (string)window.mTopBar.mAppTabButtons.SelectedTab.mTabLabel.Content;
						tabType = window.mTopBar.mAppTabButtons.SelectedTab.mTabType.ToString();
					}), new object[0]);
					jobject.Add("pkgname", pkg);
					jobject.Add("appname", appName);
					jobject.Add("tabtype", tabType);
				}
				HTTPUtils.Write(jobject.ToString(Formatting.None, new JsonConverter[0]), res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetCurrentAppDetails: {0}", new object[] { ex });
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x000817D0 File Offset: 0x0007F9D0
		internal static void ShowSettingWindow(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					MainWindow window = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
					window.Dispatcher.Invoke(new Action(delegate
					{
						MainWindow.OpenSettingsWindow(window, "STRING_NOTIFICATION");
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server ShowSettingWindow: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x00081874 File Offset: 0x0007FA74
		internal static void LaunchWebTab(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					MainWindow window = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
					window.Dispatcher.Invoke(new Action(delegate
					{
						window.mTopBar.mAppTabButtons.AddWebTab(requestData.Data["url"], requestData.Data["name"], requestData.Data["image"], true, "", false);
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server OneTimeSetupCompletedHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x00081920 File Offset: 0x0007FB20
		internal static void OneTimeSetupCompletedHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				MainWindow mainWindow = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "OTS Completed", "OTS Completed", null, RegistryManager.Instance.CurrentEngine, mainWindow.EngineInstanceRegistry.GlMode.ToString(CultureInfo.InvariantCulture), mainWindow.EngineInstanceRegistry.GlRenderMode.ToString(CultureInfo.InvariantCulture));
					mainWindow.mAppHandler.IsOneTimeSetupCompleted = true;
					ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "OTS Completed", "OTS Completed", RegistryManager.Instance.InstallID, RegistryManager.Instance.CurrentEngine, mainWindow.EngineInstanceRegistry.GlMode.ToString(CultureInfo.InvariantCulture), mainWindow.EngineInstanceRegistry.GlRenderMode.ToString(CultureInfo.InvariantCulture));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server OneTimeSetupCompletedHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x00081A78 File Offset: 0x0007FC78
		internal static void AppJsonChangedHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.InitIcons();
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in AppjsonChangedHabdler " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00081B18 File Offset: 0x0007FD18
		internal static void StartInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
				Logger.Info("start instance vm name :" + requestVmName);
				RegistryManager.ClearRegistryMangerInstance();
				BlueStacksUIUtils.RunInstance(requestVmName, false);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server StartInstanceHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x00081B84 File Offset: 0x0007FD84
		internal static void StopInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestVmName].ForceCloseWindow(false);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server StopInstanceHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x00081BF8 File Offset: 0x0007FDF8
		internal static void MinimizeInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].MinimizeWindow();
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server MinimizeInstanceHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x00081C9C File Offset: 0x0007FE9C
		internal static void HideBluestacksHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				HTTPUtils.ParseRequest(req);
				Logger.Info("Hide Bluestacks received");
				BlueStacksUIUtils.HideUnhideBlueStacks(true);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server HideBluestacksHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x00081CF8 File Offset: 0x0007FEF8
		internal static void OpenOrInstallPackageHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				string text = requestData.Data["json"].ToString(CultureInfo.InvariantCulture);
				if (!string.IsNullOrEmpty(text))
				{
					JObject jobject = JObject.Parse(text);
					if (jobject != null && jobject["campaign_id"] != null)
					{
						RegistryManager.Instance.ClientLaunchParams = text;
					}
				}
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					HTTPHandler.ShowWindowHandler(req, res);
					if (!string.IsNullOrEmpty(text))
					{
						if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.IsOneTimeSetupCompleted)
						{
							BlueStacksUIUtils.DictWindows[requestVmName].PublishForFlePopupToBrowser(text);
							new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestVmName]).DownloadAndInstallAppFromJson(text);
						}
						else
						{
							Opt.Instance.Json = text;
						}
					}
				}
				else
				{
					Opt.Instance.Json = text;
					HTTPHandler.ShowWindowHandler(req, res);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in OpenOrInstallPackageHandler. Err : " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x00081E10 File Offset: 0x00080010
		internal static void GuestBootCompleted(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.IsGuestReady = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in server GuestBootCompleted: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x00081E88 File Offset: 0x00080088
		public static void AppDisplayedHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string text2 = requestData.Data["packageName"];
				string text3 = requestData.Data["appDisplayed"];
				object obj = HTTPHandler.sLockObject;
				lock (obj)
				{
					if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
					{
						MainWindow mainWindow = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
						if (FeatureManager.Instance.IsCustomUIForDMM)
						{
							mainWindow.mAppHandler.HandleAppDisplayed(text2);
						}
						if (!mainWindow.EngineInstanceRegistry.IsOneTimeSetupDone && text2 != "com.bluestacks.appmart" && !mainWindow.mGuestBootCompleted)
						{
							int num = 20;
							while (!mainWindow.mAppHandler.IsGuestReady && num > 0)
							{
								num--;
								Thread.Sleep(1000);
							}
							if (text2 == mainWindow.mAppHandler.GetDefaultLauncher())
							{
								if (!FeatureManager.Instance.IsCustomUIForNCSoft)
								{
									Logger.Info("BOOT_STAGE: Calling guestboot_completed from AppDisplayedHandler");
									mainWindow.mAppHandler.IsGuestReady = true;
								}
								else
								{
									mainWindow.Utils.sBootCheckTimer.Enabled = false;
									mainWindow.mEnableLaunchPlayForNCSoft = true;
								}
							}
						}
					}
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server AppDisplayedHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x00082064 File Offset: 0x00080264
		public static void AppLaunchedHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string text2 = requestData.Data["package"];
				string text3 = requestData.Data["activity"];
				string text4 = requestData.Data["callingPackage"];
				Logger.Info("Package: {0}, activity: {1}, callingPackage: {2}", new object[] { text2, text3, text4 });
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					MainWindow mainWindow = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
					if (!RegistryManager.Instance.Guest[requestData.RequestVmName].IsGoogleSigninDone)
					{
						object obj = HTTPHandler.syncRoot;
						lock (obj)
						{
							if (string.Compare(HTTPHandler.mPreviousActivityReported.Replace("/", ""), text3.Replace("/", ""), StringComparison.OrdinalIgnoreCase) != 0)
							{
								HTTPHandler.mPreviousActivityReported = text3;
								ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, text2, text3, RegistryManager.Instance.InstallID, RegistryManager.Instance.CurrentEngine, mainWindow.EngineInstanceRegistry.GlMode.ToString(CultureInfo.InvariantCulture), mainWindow.EngineInstanceRegistry.GlRenderMode.ToString(CultureInfo.InvariantCulture));
							}
						}
					}
					if (FeatureManager.Instance.IsCustomUIForNCSoft && !mainWindow.mGuestBootCompleted && !text2.Equals("com.bluestacks.appmart", StringComparison.OrdinalIgnoreCase) && !text2.Equals("com.android.provision", StringComparison.OrdinalIgnoreCase))
					{
						mainWindow.mAppHandler.IsGuestReady = true;
					}
					if (mainWindow.mGuestBootCompleted)
					{
						PostBootCloudInfo mPostBootCloudInfo = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
						if (!((mPostBootCloudInfo != null) ? new bool?(mPostBootCloudInfo.IgnoredActivitiesForTabs.Contains(text3, StringComparer.InvariantCultureIgnoreCase)) : null).GetValueOrDefault())
						{
							PostBootCloudInfo mPostBootCloudInfo2 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
							if (!((mPostBootCloudInfo2 != null) ? new bool?(mPostBootCloudInfo2.IgnoredActivitiesForTabs.Contains(text3.TrimStart(text2 + "/"), StringComparer.InvariantCultureIgnoreCase)) : null).GetValueOrDefault())
							{
								mainWindow.mAppHandler.AppLaunched(text2, false);
							}
						}
					}
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server AppLaunchedHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0008234C File Offset: 0x0008054C
		public static void AppCrashedHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string vmName = requestData.RequestVmName;
				string package = requestData.Data["package"];
				Logger.Info("package: " + package);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.CloseTab("app:" + package, false, false, true, false, "");
					}), new object[0]);
					if (FeatureManager.Instance.IsCustomUIForNCSoft && !NCSoftUtils.Instance.BlackListedApps.Any((string pkg) => package.StartsWith(pkg, StringComparison.InvariantCulture)))
					{
						NCSoftUtils.Instance.SendAppCrashEvent("check android logs", vmName);
					}
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server AppCrashedHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x000824AC File Offset: 0x000806AC
		internal static void AppInfoUpdated(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string package = requestData.Data["packageName"];
				if (!string.IsNullOrEmpty(requestData.Data["macro"]) && requestData.Data["macro"].Equals("true", StringComparison.InvariantCultureIgnoreCase))
				{
					foreach (string text in requestData.Data.AllKeys)
					{
						Logger.Debug("Key: {0}, Value: {1}", new object[]
						{
							text,
							requestData.Data[text]
						});
					}
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
						{
							if (keyValuePair.Value.mWelcomeTab.mHomeAppManager.GetAppIcon(package) != null && keyValuePair.Value.mWelcomeTab.mHomeAppManager.GetMacroAppIcon(package) == null)
							{
								keyValuePair.Value.mWelcomeTab.mHomeAppManager.AddMacroAppIcon(package);
							}
						}
					}), new object[0]);
					HTTPHandler.WriteSuccessJsonArray(res);
				}
				if (!string.IsNullOrEmpty(requestData.Data["videoPresent"]) && BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					string text2 = requestData.Data["videoPresent"].ToString(CultureInfo.InvariantCulture);
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "packageName", package },
						{ "videoPresent", text2 }
					};
					HTTPUtils.SendRequestToAgentAsync("appJsonUpdatedForVideo", dictionary, requestData.RequestVmName, 0, null, true, 1, 0, "bgp64");
				}
				KMManager.ControlSchemesHandlingWhileCfgUpdateFromCloud(package);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server AppInfoDownload: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x00082668 File Offset: 0x00080868
		public static void CloseTabHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string package = requestData.Data["package"];
				Logger.Info("package: " + package);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						try
						{
							BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.CloseTab(package, false, false, true, false, "");
						}
						catch (Exception ex2)
						{
							Logger.Error("Exception in closing tab. Err : ", new object[] { ex2.ToString() });
						}
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server TabCloseHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x0008278C File Offset: 0x0008098C
		public static void ShowAppHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string package = requestData.Data["package"];
				string activity = requestData.Data["activity"];
				string text2 = requestData.Data["title"];
				Logger.Info("package: " + package);
				Logger.Info("activity: " + activity);
				Logger.Info("title : " + text2);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					new Thread(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
						{
							if (!string.IsNullOrEmpty(package) && !string.IsNullOrEmpty(activity))
							{
								BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.SendRunAppRequestAsync(package, activity, false);
							}
						}), new object[0]);
					})
					{
						IsBackground = true
					}.Start();
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server ShowAppHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x00082900 File Offset: 0x00080B00
		public static void ShowWindowHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string text = requestData.RequestVmName;
				if (requestData.Data.AllKeys.Contains("showNotifications"))
				{
					MainWindow.sShowNotifications = Convert.ToBoolean(requestData.Data["showNotifications"], CultureInfo.InvariantCulture);
				}
				if (requestData.Data.AllKeys.Contains("all"))
				{
					using (Dictionary<string, MainWindow>.Enumerator enumerator = BlueStacksUIUtils.DictWindows.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, MainWindow> keyValuePair = enumerator.Current;
							keyValuePair.Value.ShowWindow(false);
						}
						goto IL_0123;
					}
				}
				if (requestData.Data.AllKeys.Contains("vmname"))
				{
					text = requestData.Data["vmname"];
				}
				bool flag = requestData.Data["hidden"] != null && Convert.ToBoolean(requestData.Data["hidden"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(text))
				{
					if (!flag)
					{
						BlueStacksUIUtils.DictWindows[text].ShowWindow(false);
					}
				}
				else
				{
					RegistryManager.ClearRegistryMangerInstance();
					BlueStacksUIUtils.RunInstance(text, flag);
				}
				IL_0123:
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server ShowWindowHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x00082A94 File Offset: 0x00080C94
		public static void ShowWindowAndAppHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						HTTPHandler.ShowWindowHandler(req, res);
						HTTPHandler.ShowAppHandler(req, res);
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server ShowWindowAndAppHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x00082B48 File Offset: 0x00080D48
		public static void IsVisibleHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].IsVisible)
					{
						HTTPHandler.WriteSuccessJsonArray(res);
					}
					else
					{
						HTTPHandler.WriteErrorJsonArray("unused", res);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server IsVisibleHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x00082BD0 File Offset: 0x00080DD0
		public static void AppUninstalledHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string package = requestData.Data["package"];
				string text2 = requestData.Data["name"];
				Logger.Info("package: " + package);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.AppUninstalled(package);
					}), new object[0]);
				}
				NotificationManager.Instance.RemoveNotificationItem(text2, package);
				Publisher.PublishMessage(BrowserControlTags.appUninstalled, requestData.RequestVmName, new JObject(new JProperty("PackageName", package)));
				ClientStats.SendClientStatsAsync("uninstall", "success", "app_install", package, "", "");
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server AppUninstalledHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x00082D74 File Offset: 0x00080F74
		public static void AppInstalledHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Info("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string package = requestData.Data["package"];
				bool isUpdate = false;
				if (requestData.Data.AllKeys.Contains("isUpdate"))
				{
					isUpdate = string.Equals(requestData.Data["isUpdate"], "true", StringComparison.InvariantCultureIgnoreCase);
				}
				Logger.Info("package: " + package + ", isUpdate: " + isUpdate.ToString());
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.AppInstalled(package, isUpdate);
					}), new object[0]);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server AppInstalledHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
			}
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x00082EFC File Offset: 0x000810FC
		public static void ShowHomeTabHandler(HttpListenerRequest req, HttpListenerResponse _)
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
				{
					Logger.Info("Switching to Welcome tab");
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.GoToTab("Home", true, false);
				}), new object[0]);
			}
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x00082F64 File Offset: 0x00081164
		public static void ShowWebPageHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string title = requestData.Data["title"].ToString(CultureInfo.InvariantCulture);
				string webUrl = requestData.Data["url"].ToString(CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						HTTPHandler.ShowWindowHandler(req, res);
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.AddWebTab(webUrl, title, "cef_tab", true, "", false);
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server ShowWebPageHandler : " + ex.ToString());
			}
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x00083054 File Offset: 0x00081254
		public static void ForceQuitHandler(HttpListenerRequest req, HttpListenerResponse _)
		{
			Logger.Info("Quiting BlueStacksUI");
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				bool flag = false;
				try
				{
					flag = Convert.ToBoolean(requestData.Data["softclose"], CultureInfo.InvariantCulture);
				}
				catch
				{
				}
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					if (flag)
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].CloseWindow();
					}
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						App.ExitApplication();
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ForceQuit... Err : " + ex.ToString());
			}
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x00083134 File Offset: 0x00081334
		public static void OpenGoogleHandler(HttpListenerRequest req, HttpListenerResponse _)
		{
			RequestData requestData = HTTPUtils.ParseRequest(req);
			string tabName = "tab_" + (new Random().Next(100) + 1).ToString();
			if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
			{
				BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.AddWebTab("http://www.google.com", tabName, "cef_tab", true, "", false);
				}), new object[0]);
			}
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x000831C4 File Offset: 0x000813C4
		private static void WriteSuccessJsonWithVmName(string vmName, HttpListenerResponse res)
		{
			HTTPUtils.Write(new JArray
			{
				new JObject
				{
					new JProperty("success", true),
					new JProperty("vmname", vmName)
				}
			}.ToString(Formatting.None, new JsonConverter[0]), res);
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x0008321C File Offset: 0x0008141C
		public static void UnsupportedCPUError(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string reason = requestData.Data["PlusFailureReason"];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						string localizedString = LocaleStrings.GetLocalizedString("STRING_INCOMPATIBLE_FRONTEND_QUIT_CAPTION", "");
						if (global::System.Windows.Forms.MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_INCOMPATIBLE_FRONTEND_QUIT", ""), localizedString, MessageBoxButtons.OK) == DialogResult.OK)
						{
							Logger.Info("Quit BlueStacksUI End with reason {0}", new object[] { reason });
							HTTPHandler.WriteSuccessJsonArray(res);
							BlueStacksUIUtils.DictWindows[requestData.RequestVmName].ForceCloseWindow(false);
						}
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in QuitBlueStacksUI: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x000832E4 File Offset: 0x000814E4
		public static void UpdateUserInfoHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string text = requestData.Data["result"].Trim();
				if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mPostOtsWelcomeWindow != null)
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mPostOtsWelcomeWindow.ChangeBasedonTokenReceived(text);
				}
				if (text.Equals("true", StringComparison.InvariantCultureIgnoreCase) && BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium);
					}), new object[0]);
					PromotionManager.CheckIsUserPremium();
					Action<bool> appRecommendationHandler = PromotionObject.AppRecommendationHandler;
					if (appRecommendationHandler != null)
					{
						appRecommendationHandler(false);
					}
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mLaunchStartupTabWhenTokenReceived && PromotionObject.Instance.StartupTab.Count > 0)
						{
							BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Utils.HandleGenericActionFromDictionary(PromotionObject.Instance.StartupTab, "startup_action", "");
						}
					}), new object[0]);
					Publisher.PublishMessage(BrowserControlTags.userInfoUpdated, requestData.RequestVmName, null);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in UpdateUserInfoHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0008345C File Offset: 0x0008165C
		internal static void AppInstallStarted(HttpListenerRequest req, HttpListenerResponse _)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Info("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string apkPath = requestData.Data["filePath"];
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					string package = string.Empty;
					string appName = string.Empty;
					DownloadInstallApk downloader = new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestData.RequestVmName]);
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
						appName = apkInfo.AppName;
						package = apkInfo.PackageName;
					}
					HTTPHandler.dictFileNamesPackageName[apkPath] = package;
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.AddAppIcon(package, appName, string.Empty, downloader);
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallStart(package, apkPath);
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetUserInfo: " + ex.ToString());
			}
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x0008363C File Offset: 0x0008183C
		public static void AppInstallFailed(HttpListenerRequest req, HttpListenerResponse _)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Debug("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string apkPath = requestData.Data["filePath"];
				int errorCode = Convert.ToInt32(requestData.Data["errorCode"], CultureInfo.InvariantCulture);
				string vmName = requestData.RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						try
						{
							BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.ApkInstallFailed(HTTPHandler.dictFileNamesPackageName[apkPath]);
							HTTPHandler.ShowErrorPromptIfNeeded(vmName, errorCode);
						}
						catch (Exception ex2)
						{
							Logger.Error("error in install failed http call: {0}", new object[] { ex2 });
						}
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in AppInstallFailed. Err : " + ex.ToString());
			}
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x00083748 File Offset: 0x00081948
		private static void ShowErrorPromptIfNeeded(string vmName, int errorCode)
		{
			string text = string.Empty;
			if (errorCode == 10)
			{
				text = LocaleStrings.GetLocalizedString("STRING_INVALID_APK_BLACKLISTED_ERROR", "");
			}
			else
			{
				text = LocaleStrings.GetLocalizedString("STRING_INVALID_APK_BLACKLISTED_ERROR", "");
			}
			if (!string.IsNullOrEmpty(text))
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_INSTALLATION_ERROR", "");
				customMessageWindow.BodyTextBlock.Text = text;
				customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), null, null, false, null);
				customMessageWindow.Owner = BlueStacksUIUtils.DictWindows[vmName];
				BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay(null);
				customMessageWindow.ShowDialog();
				BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
			}
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x00083808 File Offset: 0x00081A08
		public static void GooglePlayAppInstall(HttpListenerRequest req, HttpListenerResponse _)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				foreach (string text in requestData.Data.AllKeys)
				{
					Logger.Info("Key: {0}, Value: {1}", new object[]
					{
						text,
						requestData.Data[text]
					});
				}
				string packageName = requestData.Data["packageName"];
				string appName = requestData.Data["appName"];
				string isAdditionalFile = requestData.Data["isAdditionalFile"];
				string status = requestData.Data["status"];
				if (!string.IsNullOrEmpty(status) && BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						AppIconModel appIcon = BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
						if (appIcon == null || !appIcon.mIsAppInstalled)
						{
							if (status.Equals("STARTED", StringComparison.InvariantCultureIgnoreCase))
							{
								BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.AddAppIcon(packageName, appName, string.Empty, null);
								BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallStart(packageName, string.Empty);
							}
							if (status.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase) && isAdditionalFile.Equals("false", StringComparison.OrdinalIgnoreCase))
							{
								BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallCompleted(packageName);
								return;
							}
							if (status.Equals("CANCELED", StringComparison.InvariantCultureIgnoreCase))
							{
								BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.RemoveAppIcon(packageName, null);
							}
						}
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GooglePlayAppInstall: " + ex.ToString());
			}
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x0008396C File Offset: 0x00081B6C
		internal static void ChangeTextOTSHandler(HttpListenerRequest req, HttpListenerResponse _)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIBinding.Bind(BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendOTSControl.mBaseControl.mTitleLabel, "STRING_WELCOME_TO_BLUESTACKS");
						string text = "string set after change text OTS ..";
						object content = BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendOTSControl.mBaseControl.mTitleLabel.Content;
						Logger.Info(text + ((content != null) ? content.ToString() : null));
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("error in change ots text." + ex.ToString());
			}
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x00083A00 File Offset: 0x00081C00
		internal static void ShootingModeChanged(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendHandler.IsShootingModeActivated = Convert.ToBoolean(requestData.Data["IsShootingModeActivated"], CultureInfo.InvariantCulture);
						if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendHandler.IsShootingModeActivated)
						{
							BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenSidebarPopup.IsOpen = false;
							BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenTopbarPopup.IsOpen = false;
							return;
						}
						BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Shooting Mode Changed: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x00083AA0 File Offset: 0x00081CA0
		internal static void ChangeOrientaionHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string text = requestData.Data["package"].ToString(CultureInfo.InvariantCulture);
				bool flag = Convert.ToBoolean(requestData.Data["is_portrait"], CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Frontend_OrientationChanged(text, flag);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ChangeOrientaionHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x00083B4C File Offset: 0x00081D4C
		internal static void ShowGrmAndLaunchAppHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string packageName = requestData.Data["package"].ToString(CultureInfo.InvariantCulture);
				string vmName = requestData.RequestVmName;
				BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
				{
					if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) != null)
					{
						BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.OpenApp(packageName, true);
					}
				}), new object[0]);
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ShowGrmAndLaunchAppHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x00083BF8 File Offset: 0x00081DF8
		internal static void UpdateSizeOfOverlay(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke(new Action(delegate
					{
						try
						{
							IntPtr intPtr = new IntPtr(Convert.ToInt32(requestData.Data["handle"], CultureInfo.InvariantCulture));
							BlueStacksUIUtils.DictWindows[requestData.RequestVmName].StaticComponents.mLastMappableWindowHandle = intPtr;
							if (KMManager.dictOverlayWindow.ContainsKey(BlueStacksUIUtils.DictWindows[requestData.RequestVmName]))
							{
								KMManager.dictOverlayWindow[BlueStacksUIUtils.DictWindows[requestData.RequestVmName]].UpdateSize();
							}
						}
						catch (Exception ex2)
						{
							Logger.Error("Exception in UpdateSizeOfOverlay: " + ex2.ToString());
							HTTPHandler.WriteErrorJsonArray(ex2.Message, res);
						}
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in UpdateSizeOfOverlay: " + ex.ToString());
				HTTPHandler.WriteErrorJsonArray(ex.Message, res);
			}
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x00083CA4 File Offset: 0x00081EA4
		internal static void BootFailedPopupHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Utils.SendGuestBootFailureStats("com exception");
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in BootFailedPopupHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x00083D20 File Offset: 0x00081F20
		internal static void DragDropInstallHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				string text = requestData.Data["filePath"].ToString(CultureInfo.InvariantCulture);
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestVmName]).InstallApk(text, true);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in DragDropInstallHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x00083DB8 File Offset: 0x00081FB8
		internal static void DeviceProvisionedHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				Logger.Info("Device provisioned client");
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.IsOneTimeSetupCompleted = true;
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in DeviceProvisionedHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x00083E40 File Offset: 0x00082040
		internal static void GoogleSigninHandler(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string requestVmName = requestData.RequestVmName;
				string text = requestData.Data["email"].ToString(CultureInfo.InvariantCulture).Trim();
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
				{
					RegistryManager.Instance.Guest[requestData.RequestVmName].IsGoogleSigninDone = true;
					Stats.SendUnifiedInstallStatsAsync("google_login_completed", text);
					BlueStacksUIUtils.DictWindows[requestData.RequestVmName].PostGoogleSigninCompleteTask();
					Publisher.PublishMessage(BrowserControlTags.googleSigninComplete, requestData.RequestVmName, null);
				}
				HTTPHandler.WriteSuccessJsonArray(res);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GoogleSigninHandler: " + ex.ToString());
				HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
			}
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x00083F0C File Offset: 0x0008210C
		internal static void SetDMMKeymapping(HttpListenerRequest req, HttpListenerResponse _)
		{
			Logger.Info("Got SetKeymapping {0} request from {1}", new object[]
			{
				req.HttpMethod,
				req.RemoteEndPoint.ToString()
			});
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				string package = requestData.Data["package"].ToString(CultureInfo.InvariantCulture);
				bool isKeymapEnabled = Convert.ToBoolean(requestData.Data["enablekeymap"], CultureInfo.InvariantCulture);
				Logger.Info("package : " + package + " enablekeymap : " + isKeymapEnabled.ToString());
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
				{
					int retries = 3;
					Action <>9__0;
					while (retries > 0)
					{
						Dispatcher dispatcher = BlueStacksUIUtils.DictWindows[vmName].Dispatcher;
						Action action;
						if ((action = <>9__0) == null)
						{
							action = (<>9__0 = delegate
							{
								if (BlueStacksUIUtils.DictWindows[vmName].Visibility == Visibility.Visible && BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.mDictTabs.ContainsKey(package))
								{
									retries = 0;
									BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.mDictTabs[package].IsDMMKeymapEnabled = isKeymapEnabled;
								}
							});
						}
						dispatcher.Invoke(action, new object[0]);
						if (retries > 0)
						{
							int retries2 = retries;
							retries = retries2 - 1;
							Thread.Sleep(1000);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Server SetKeymapping: " + ex.ToString());
			}
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x00084080 File Offset: 0x00082280
		internal static void ReloadShortcuts(HttpListenerRequest req, HttpListenerResponse _)
		{
			try
			{
				string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					BlueStacksUIUtils.DictWindows[requestVmName].Dispatcher.Invoke(new Action(delegate
					{
						CommonHandlers.ReloadShortcutsForAllInstances();
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ReloadShortcuts: " + ex.ToString());
			}
		}

		// Token: 0x060015A3 RID: 5539 RVA: 0x0008410C File Offset: 0x0008230C
		internal static void ReloadPromotions(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
				if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
				{
					PromotionManager.ReloadPromotionsAsync();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in ReloadPromotions: " + ex.ToString());
			}
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x00084164 File Offset: 0x00082364
		internal static void HandleOverlayControlsVisibility(HttpListenerRequest req, HttpListenerResponse res)
		{
			try
			{
				RequestData requestData = HTTPUtils.ParseRequest(req);
				string vmName = requestData.RequestVmName;
				BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
				{
					if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && RegistryManager.Instance.IsGameTvEnabled)
					{
						string text = requestData.Data["data"];
						KMManager.HandleCallbackControl(BlueStacksUIUtils.DictWindows[vmName], text);
					}
					if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && (BlueStacksUIUtils.DictWindows[vmName].IsActive || (KMManager.sGuidanceWindow != null && KMManager.sGuidanceWindow.ParentWindow == BlueStacksUIUtils.DictWindows[vmName] && KMManager.sGuidanceWindow.IsActive) || (KMManager.CanvasWindow != null && KMManager.CanvasWindow.ParentWindow == BlueStacksUIUtils.DictWindows[vmName] && KMManager.CanvasWindow.SidebarWindow != null && KMManager.CanvasWindow.SidebarWindow.IsActive)))
					{
						string text2 = requestData.Data["data"];
						KMManager.ShowDynamicOverlay(BlueStacksUIUtils.DictWindows[vmName], true, false, text2);
					}
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleOverlayControlsVisibility: " + ex.ToString());
			}
		}

		// Token: 0x04000D11 RID: 3345
		private static object sLockObject = new object();

		// Token: 0x04000D12 RID: 3346
		internal static string lastPackage = string.Empty;

		// Token: 0x04000D13 RID: 3347
		private static Dictionary<string, string> dictFileNamesPackageName = new Dictionary<string, string>();

		// Token: 0x04000D14 RID: 3348
		private static bool mSendGamepadStats = false;

		// Token: 0x04000D15 RID: 3349
		private static object syncRoot = new object();

		// Token: 0x04000D16 RID: 3350
		private static string mPreviousActivityReported = "";
	}
}
