using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using BlueStacks.Common;
using BlueStacks.Common.Grm;
using BlueStacks.Common.Grm.Evaluators;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000080 RID: 128
	internal class GrmHandler
	{
		// Token: 0x060005E9 RID: 1513 RVA: 0x00022240 File Offset: 0x00020440
		internal static void RequirementConfigUpdated(string vmName = "Android")
		{
			if (AppRequirementsParser.Instance.Requirements == null)
			{
				return;
			}
			foreach (AppInfo appInfo in new JsonParser(vmName).GetAppList().ToList<AppInfo>())
			{
				GrmHandler.RefreshGrmIndication(appInfo.Package, vmName);
			}
			GrmHandler.SendUpdateGrmPackagesToAndroid(vmName);
			GrmHandler.SendUpdateGrmPackagesToBrowser(vmName);
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x000222BC File Offset: 0x000204BC
		internal static void SendUpdateGrmPackagesToAndroid(string vmName)
		{
			try
			{
				if (GrmHandler.sDictAppRuleSet.ContainsKey(vmName) && GrmHandler.sDictAppRuleSet[vmName].Count != 0 && Utils.IsGuestBooted(vmName, "bgp64"))
				{
					JObject jobject = new JObject { 
					{
						"GrmPackageList",
						JArray.FromObject(GrmHandler.sDictAppRuleSet[vmName].Keys)
					} };
					Dictionary<string, string> dictionary = new Dictionary<string, string> { 
					{
						"data",
						jobject.ToString(Formatting.None, new JsonConverter[0])
					} };
					HTTPUtils.SendRequestToGuestAsync("grmPackages", dictionary, vmName, 0, null, false, 1, 0);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SendUpdateGrmPackagesToAndroid: " + ex.ToString());
			}
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00022378 File Offset: 0x00020578
		internal static void SendUpdateGrmPackagesToBrowser(string vmName)
		{
			try
			{
				if (GrmHandler.sDictAppRuleSet.ContainsKey(vmName))
				{
					JObject jobject = new JObject();
					foreach (KeyValuePair<string, GrmRuleSet> keyValuePair in GrmHandler.sDictAppRuleSet[vmName])
					{
						jobject.Add(new JProperty(keyValuePair.Key, keyValuePair.Value.MessageWindow.MessageType));
					}
					Publisher.PublishMessage(BrowserControlTags.grmAppListUpdate, vmName, new JObject(new JProperty("GrmPackageData", jobject)));
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SendUpdateGrmPackagesToBrowser: " + ex.ToString());
			}
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00022444 File Offset: 0x00020644
		internal static void RefreshGrmIndication(string package, string vmName = "Android")
		{
			try
			{
				List<AppRequirement> requirements = AppRequirementsParser.Instance.Requirements;
				if (requirements != null)
				{
					if (!GrmHandler.sDictAppRuleSet.ContainsKey(vmName))
					{
						GrmHandler.sDictAppRuleSet[vmName] = new Dictionary<string, GrmRuleSet>();
					}
					AppIconModel appIcon = BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(package);
					if (appIcon.AppIncompatType != AppIncompatType.None && !requirements.Any((AppRequirement _) => string.Equals(_.PackageName, package, StringComparison.OrdinalIgnoreCase)))
					{
						GrmHandler.RemoveAppCompatError(appIcon, BlueStacksUIUtils.DictWindows[vmName]);
					}
					AppRequirement appRequirement = requirements.Where((AppRequirement _) => string.Compare(_.PackageName, package, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault<AppRequirement>();
					if (appRequirement == null)
					{
						appRequirement = requirements.Where((AppRequirement _) => _.PackageName.EndsWith("*", StringComparison.InvariantCulture) && package.StartsWith(_.PackageName.Trim(new char[] { '*' }), StringComparison.InvariantCulture)).FirstOrDefault<AppRequirement>();
					}
					if (appRequirement != null)
					{
						GrmRuleSet grmRuleSet = appRequirement.EvaluateRequirement(package, vmName);
						if (grmRuleSet != null)
						{
							GrmHandler.AddGRMIndicationForIncompatibleApp(appIcon, BlueStacksUIUtils.DictWindows[vmName], grmRuleSet);
						}
						else
						{
							GrmHandler.RemoveAppCompatError(appIcon, BlueStacksUIUtils.DictWindows[vmName]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				string text = "Exception in RefreshGrmIndication. Exception: ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00022580 File Offset: 0x00020780
		internal static void HandleCompatibility(string package, string vmName)
		{
			try
			{
				GrmHandler.AppCompatErrorWindow = new CustomMessageWindow();
				GrmHandler.AppCompatErrorWindow.TitleTextBlock.Text = BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(package).AppName;
				if (!string.IsNullOrEmpty(AppRequirementsParser.Instance.GetLocalizedString(GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.HeaderStringKey)))
				{
					GrmHandler.AppCompatErrorWindow.BodyTextBlockTitle.Text = AppRequirementsParser.Instance.GetLocalizedString(GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.HeaderStringKey);
					GrmHandler.AppCompatErrorWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
				}
				GrmHandler.AppCompatErrorWindow.BodyTextBlock.Text = AppRequirementsParser.Instance.GetLocalizedString(GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.MessageStringKey);
				if (GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.MessageType == MessageType.Info.ToString())
				{
					GrmHandler.AppCompatErrorWindow.MessageIcon.ImageName = "message_info";
				}
				else if (GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.MessageType == MessageType.Error.ToString())
				{
					GrmHandler.AppCompatErrorWindow.MessageIcon.ImageName = "message_error";
				}
				GrmHandler.AppCompatErrorWindow.MessageIcon.Visibility = Visibility.Visible;
				if (GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.DontShowOption)
				{
					GrmHandler.AppCompatErrorWindow.CheckBox.Content = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
					GrmHandler.AppCompatErrorWindow.CheckBox.Visibility = Visibility.Visible;
				}
				using (List<GrmMessageButton>.Enumerator enumerator = GrmHandler.sDictAppRuleSet[vmName][package].MessageWindow.Buttons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GrmMessageButton button = enumerator.Current;
						ButtonColors buttonColors = EnumHelper.Parse<ButtonColors>(button.ButtonColor, ButtonColors.Blue);
						GrmHandler.AppCompatErrorWindow.AddButton(buttonColors, AppRequirementsParser.Instance.GetLocalizedString(button.ButtonStringKey), delegate(object o, EventArgs e)
						{
							GrmHandler.PerformGrmActions(button.Actions, package, BlueStacksUIUtils.DictWindows[vmName]);
						}, null, false, null);
					}
				}
				GrmHandler.AppCompatErrorWindow.Owner = BlueStacksUIUtils.DictWindows[vmName];
				BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay(null);
				GrmHandler.AppCompatErrorWindow.ShowDialog();
				BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while showing appcompat message to user. Exception: " + ex.ToString());
				BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.OpenApp(package, false);
			}
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00022914 File Offset: 0x00020B14
		private static void PerformGrmActions(List<GrmAction> actions, string package, MainWindow ParentWindow)
		{
			using (BackgroundWorker backgroundWorker = new BackgroundWorker())
			{
				backgroundWorker.DoWork += delegate(object obj, DoWorkEventArgs e)
				{
					GrmHandler.PerformGrmActionsWorker_DoWork(e, actions, package, ParentWindow);
				};
				backgroundWorker.RunWorkerCompleted += delegate(object obj, RunWorkerCompletedEventArgs e)
				{
					GrmHandler.PerformGrmActionsWorker_RunWorkerCompleted(e, ParentWindow);
				};
				backgroundWorker.RunWorkerAsync();
			}
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x00022988 File Offset: 0x00020B88
		private static void PerformGrmActionsWorker_DoWork(DoWorkEventArgs e, List<GrmAction> actions, string package, MainWindow ParentWindow)
		{
			try
			{
				ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					ParentWindow.mFrontendGrid.Visibility = Visibility.Hidden;
					ParentWindow.mExitProgressGrid.ProgressText = LocaleStrings.GetLocalizedString("STRING_PERFORMING_ACTIONS", "");
					ParentWindow.mExitProgressGrid.Visibility = Visibility.Visible;
				}), new object[0]);
				try
				{
					ClientStats.SendMiscellaneousStatsAsync("grm_action_clicked", RegistryManager.Instance.UserGuid, string.Join(",", actions.Select((GrmAction _) => _.ActionType.ToString(CultureInfo.InvariantCulture)).ToArray<string>()), RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp64", package, null, null);
				}
				catch (Exception ex)
				{
					string text = "Exception while sending misc stat for grm. ";
					Exception ex2 = ex;
					Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				e.Result = false;
				Action <>9__4;
				foreach (GrmAction grmAction in actions)
				{
					GrmActionType grmActionType = EnumHelper.Parse<GrmActionType>(grmAction.ActionType, GrmActionType.NoAction);
					bool? flag = new bool?(false);
					switch (grmActionType)
					{
					case GrmActionType.UpdateRam:
					{
						int num = int.Parse(grmAction.ActionDictionary["actionValue"], CultureInfo.InvariantCulture);
						int num2 = 4096;
						int num3;
						if (int.TryParse(PhysicalRamEvaluator.RAM, out num3))
						{
							num2 = (int)((double)num3 * 0.5);
						}
						if (RegistryManager.Instance.CurrentEngine == EngineState.raw.ToString() && num2 >= 3072)
						{
							num2 = 3072;
						}
						if (num2 < num)
						{
							num = num2;
						}
						RegistryManager.Instance.Guest[ParentWindow.mVmName].Memory = num;
						flag = new bool?(true);
						break;
					}
					case GrmActionType.UserBrowser:
						ParentWindow.Utils.HandleGenericActionFromDictionary(grmAction.ActionDictionary, "grm", "");
						flag = new bool?(false);
						break;
					case GrmActionType.DownloadFileAndExecute:
					{
						Random random = new Random();
						string text2 = grmAction.ActionDictionary["fileName"];
						text2 += " ";
						string text3 = text2.Substring(0, text2.IndexOf(' '));
						string text4 = text2.Substring(text2.IndexOf(' ') + 1);
						text3 = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[]
						{
							random.Next(),
							text3
						});
						text3 = Path.Combine(RegistryStrings.PromotionDirectory, text3);
						try
						{
							using (WebClient webClient = new WebClient())
							{
								webClient.DownloadFile(grmAction.ActionDictionary["url"], text3);
							}
							Thread.Sleep(2000);
							using (Process process = new Process())
							{
								process.StartInfo.UseShellExecute = true;
								process.StartInfo.CreateNoWindow = true;
								if ((text3.ToUpperInvariant().EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase) || text3.ToUpperInvariant().EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)) && !BlueStacksUtils.IsSignedByBlueStacks(text3))
								{
									Logger.Info("Not executing unsigned binary " + text3);
									GrmHandler.GrmExceptionMessageBox(ParentWindow);
									return;
								}
								if (text3.ToUpperInvariant().EndsWith(".msi", StringComparison.InvariantCultureIgnoreCase))
								{
									process.StartInfo.FileName = "msiexec";
									text4 = string.Format(CultureInfo.InvariantCulture, "/i {0} {1}", new object[] { text3, text4 });
									process.StartInfo.Arguments = text4;
								}
								else
								{
									process.StartInfo.FileName = text3;
									process.StartInfo.Arguments = text4;
								}
								Logger.Info("Starting process: {0} {1}", new object[]
								{
									process.StartInfo.FileName,
									text4
								});
								process.Start();
							}
						}
						catch (Exception ex3)
						{
							GrmHandler.GrmExceptionMessageBox(ParentWindow);
							string text5 = "Failed to download and execute. err: ";
							Exception ex4 = ex3;
							Logger.Error(text5 + ((ex4 != null) ? ex4.ToString() : null));
						}
						flag = new bool?(false);
						break;
					}
					case GrmActionType.NoAction:
						flag = new bool?(false);
						break;
					case GrmActionType.ContinueAnyway:
					{
						Dispatcher dispatcher = ParentWindow.Dispatcher;
						Action action;
						if ((action = <>9__4) == null)
						{
							action = (<>9__4 = delegate
							{
								ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
								ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
								if (GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].MessageWindow.DontShowOption)
								{
									GrmHandler.DonotShowCheckboxHandling(ParentWindow, package);
									GrmHandler.RefreshGrmIndication(package, ParentWindow.mVmName);
									GrmHandler.SendUpdateGrmPackagesToAndroid(ParentWindow.mVmName);
									GrmHandler.SendUpdateGrmPackagesToBrowser(ParentWindow.mVmName);
								}
								ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(package, false);
							});
						}
						dispatcher.Invoke(action, new object[0]);
						flag = null;
						break;
					}
					case GrmActionType.GlMode:
					{
						string text6 = grmAction.ActionDictionary["actionValue"];
						int glRenderMode = RegistryManager.Instance.Guest[ParentWindow.mVmName].GlRenderMode;
						int glMode = RegistryManager.Instance.Guest[ParentWindow.mVmName].GlMode;
						GlMode glMode2 = GlMode.PGA_GL;
						if (glRenderMode == 1 && glMode == 1)
						{
							glMode2 = GlMode.PGA_GL;
						}
						else if (glRenderMode == 1 && glMode == 2)
						{
							glMode2 = GlMode.AGA_GL;
						}
						else if (glMode == 1)
						{
							glMode2 = GlMode.PGA_DX;
						}
						else if (glMode == 2)
						{
							glMode2 = GlMode.AGA_DX;
						}
						List<string> list = (from _ in text6.Split(new char[] { ',' })
							select _.Trim()).ToList<string>();
						if (list.Contains(glMode2.ToString(), StringComparer.InvariantCultureIgnoreCase))
						{
							flag = new bool?(false);
						}
						else
						{
							text6 = list.RandomElement<string>();
							string text7 = "";
							int num4;
							if (string.Compare(text6.Split(new char[] { '_' })[1].Trim(), "GL", StringComparison.OrdinalIgnoreCase) == 0)
							{
								num4 = 1;
								text7 += "1";
							}
							else
							{
								num4 = 4;
								text7 += "4";
							}
							int num5;
							if (string.Compare(text6.Split(new char[] { '_' })[0].Trim(), "PGA", StringComparison.OrdinalIgnoreCase) == 0)
							{
								num5 = 1;
								text7 += " 1";
							}
							else
							{
								num5 = 2;
								text7 += " 2";
							}
							if (RunCommand.RunCmd(Path.Combine(RegistryStrings.InstallDir, "HD-GlCheck"), text7, true, true, false, 0).ExitCode == 0)
							{
								RegistryManager.Instance.Guest[ParentWindow.mVmName].GlRenderMode = num4;
								Utils.UpdateValueInBootParams("GlMode", num5.ToString(CultureInfo.InvariantCulture), ParentWindow.mVmName, true, "bgp64");
								RegistryManager.Instance.Guest[ParentWindow.mVmName].GlMode = num5;
							}
							else
							{
								GrmHandler.GrmExceptionMessageBox(ParentWindow);
								Logger.Info("GL check execution for the required combination failed.");
							}
							flag = new bool?(true);
						}
						break;
					}
					case GrmActionType.DeviceProfile:
					{
						string text8 = grmAction.ActionDictionary["pCode"];
						string text9 = string.Empty;
						if (string.Compare(text8, "custom", StringComparison.OrdinalIgnoreCase) == 0)
						{
							text9 = "{";
							text9 += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[] { "true" });
							text9 += string.Format(CultureInfo.InvariantCulture, "\"model\":\"{0}\",", new object[] { grmAction.ActionDictionary["model"] });
							text9 += string.Format(CultureInfo.InvariantCulture, "\"brand\":\"{0}\",", new object[] { grmAction.ActionDictionary["brand"] });
							text9 += string.Format(CultureInfo.InvariantCulture, "\"manufacturer\":\"{0}\"", new object[] { grmAction.ActionDictionary["manufacturer"] });
							text9 += "}";
						}
						else
						{
							List<string> list2 = (from _ in text8.Split(new char[] { ',' })
								select _.Trim()).ToList<string>();
							string valueInBootParams = Utils.GetValueInBootParams("pcode", ParentWindow.mVmName, "", "bgp64");
							if (list2.Contains(valueInBootParams))
							{
								break;
							}
							text8 = list2.RandomElement<string>();
							text9 = "{";
							text9 += string.Format(CultureInfo.InvariantCulture, "\"createcustomprofile\":\"{0}\",", new object[] { "false" });
							text9 += string.Format(CultureInfo.InvariantCulture, "\"pcode\":\"{0}\"", new object[] { text8 });
							text9 += "}";
						}
						if (string.Equals(VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { "changeDeviceProfile", text9 }), ParentWindow.mVmName), "ok", StringComparison.InvariantCulture))
						{
							Utils.UpdateValueInBootParams("pcode", text8, ParentWindow.mVmName, false, "bgp64");
							if (PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(package))
							{
								HTTPUtils.SendRequestToAgentAsync("clearAppData", new Dictionary<string, string> { { "package", package } }, ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
							}
						}
						else
						{
							GrmHandler.GrmExceptionMessageBox(ParentWindow);
							Logger.Error("Setting device profile for the required combination failed.");
						}
						flag = new bool?(false);
						break;
					}
					case GrmActionType.BootParam:
					{
						string text10 = grmAction.ActionDictionary["param"].Trim();
						string text11 = grmAction.ActionDictionary["actionValue"].Trim();
						Utils.UpdateValueInBootParams(text10, text11, ParentWindow.mVmName, true, "bgp64");
						flag = new bool?(true);
						break;
					}
					case GrmActionType.DPI:
					{
						string text12 = grmAction.ActionDictionary["actionValue"];
						Utils.SetDPIInBootParameters(RegistryManager.Instance.Guest[ParentWindow.mVmName].BootParameters, text12, ParentWindow.mVmName, "bgp64");
						flag = new bool?(true);
						break;
					}
					case GrmActionType.CpuCores:
					{
						int num6 = int.Parse(grmAction.ActionDictionary["actionValue"], CultureInfo.InvariantCulture);
						if (RegistryManager.Instance.CurrentEngine != EngineState.raw.ToString())
						{
							RegistryManager.Instance.Guest[ParentWindow.mVmName].VCPUs = ((num6 > 8) ? 8 : num6);
						}
						flag = new bool?(true);
						break;
					}
					case GrmActionType.Resolution:
					{
						string text13 = grmAction.ActionDictionary["actionValue"];
						List<string> list3 = (from _ in text13.Split(new char[] { ',' })
							select _.Replace(" ", string.Empty)).ToList<string>();
						int guestWidth = RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth;
						int guestHeight = RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight;
						string text14 = guestWidth.ToString(CultureInfo.InvariantCulture) + "x" + guestHeight.ToString(CultureInfo.InvariantCulture);
						if (!list3.Contains(text14, StringComparer.InvariantCultureIgnoreCase))
						{
							text13 = list3.RandomElement<string>();
							RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestWidth = int.Parse(text13.Split(new char[] { 'x' })[0].Trim(), CultureInfo.InvariantCulture);
							RegistryManager.Instance.Guest[ParentWindow.mVmName].GuestHeight = int.Parse(text13.Split(new char[] { 'x' })[1].Trim(), CultureInfo.InvariantCulture);
							flag = new bool?(true);
						}
						break;
					}
					case GrmActionType.RestartBluestacks:
						flag = new bool?(true);
						break;
					case GrmActionType.RestartMachine:
						Process.Start("shutdown.exe", "-r -t 0");
						break;
					case GrmActionType.Fps:
					{
						int num7 = int.Parse(grmAction.ActionDictionary["actionValue"], CultureInfo.InvariantCulture);
						RegistryManager.Instance.Guest[ParentWindow.mVmName].FPS = num7;
						Utils.UpdateValueInBootParams("fps", num7.ToString(CultureInfo.InvariantCulture), ParentWindow.mVmName, true, "bgp64");
						Utils.SendChangeFPSToInstanceASync(ParentWindow.mVmName, num7);
						flag = new bool?(false);
						break;
					}
					case GrmActionType.EditRegistry:
					{
						string text15 = grmAction.ActionDictionary["location"];
						if (string.Compare(text15, "registryManager", StringComparison.OrdinalIgnoreCase) == 0)
						{
							PropertyInfo property = typeof(RegistryManager).GetProperty(grmAction.ActionDictionary["propertyName"]);
							object obj = Convert.ChangeType(grmAction.ActionDictionary["propertyValue"], Type.GetTypeCode(property.PropertyType), CultureInfo.InvariantCulture);
							property.SetValue(RegistryManager.Instance, obj, null);
						}
						else if (string.Compare(text15, "instanceManager", StringComparison.OrdinalIgnoreCase) == 0)
						{
							PropertyInfo property2 = typeof(InstanceRegistry).GetProperty(grmAction.ActionDictionary["propertyName"]);
							object obj2 = Convert.ChangeType(grmAction.ActionDictionary["propertyValue"], Type.GetTypeCode(property2.PropertyType), CultureInfo.InvariantCulture);
							property2.SetValue(RegistryManager.Instance.Guest[ParentWindow.mVmName], obj2, null);
						}
						else
						{
							string text16 = string.Format(CultureInfo.InvariantCulture, "Software\\BlueStacks{0}\\{1}", new object[]
							{
								Strings.GetOemTag(),
								grmAction.ActionDictionary["propertyPath"].Replace("vmName", ParentWindow.mVmName)
							});
							object obj3 = null;
							RegistryValueKind registryValueKind = EnumHelper.Parse<RegistryValueKind>(grmAction.ActionDictionary["propertyRegistryKind"], RegistryValueKind.String);
							if (registryValueKind != RegistryValueKind.String)
							{
								if (registryValueKind == RegistryValueKind.DWord)
								{
									obj3 = int.Parse(grmAction.ActionDictionary["propertyValue"], CultureInfo.InvariantCulture);
								}
							}
							else
							{
								obj3 = grmAction.ActionDictionary["propertyValue"];
							}
							RegistryUtils.SetRegistryValue(text16, grmAction.ActionDictionary["propertyName"], obj3, registryValueKind, RegistryKeyKind.HKEY_LOCAL_MACHINE);
						}
						flag = new bool?(false);
						break;
					}
					case GrmActionType.ClearAppData:
						HTTPUtils.SendRequestToAgentAsync("clearAppData", new Dictionary<string, string> { { "package", package } }, ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
						break;
					}
					bool? flag2 = flag;
					bool flag3 = true;
					if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
					{
						e.Result = true;
					}
					else if (flag == null)
					{
						e.Result = null;
					}
				}
				Thread.Sleep(1000);
			}
			catch (Exception ex5)
			{
				Logger.Error("Exception in performing grm actions, ex: " + ex5.ToString());
				ClientStats.SendMiscellaneousStatsAsync("grm_action_error", RegistryManager.Instance.UserGuid, GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].RuleId, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "bgp64", package, ex5.Message, null);
				GrmHandler.GrmExceptionMessageBox(ParentWindow);
				e.Result = null;
			}
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00023990 File Offset: 0x00021B90
		private static void PerformGrmActionsWorker_RunWorkerCompleted(RunWorkerCompletedEventArgs e, MainWindow ParentWindow)
		{
			if (e.Result == null)
			{
				return;
			}
			if (!(bool)e.Result)
			{
				ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
					ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
				}), new object[0]);
				GrmHandler.RequirementConfigUpdated(ParentWindow.mVmName);
				return;
			}
			BlueStacksUIUtils.RestartInstance(ParentWindow.mVmName);
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00023A04 File Offset: 0x00021C04
		private static void GrmExceptionMessageBox(MainWindow ParentWindow)
		{
			ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
				ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ERROR", "");
				customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_GRM_EXCEPTION_MESSAGE", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
				customMessageWindow.Owner = ParentWindow;
				ParentWindow.ShowDimOverlay(null);
				customMessageWindow.ShowDialog();
				ParentWindow.HideDimOverlay();
			}), new object[0]);
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00023A44 File Offset: 0x00021C44
		private static void DonotShowCheckboxHandling(MainWindow ParentWindow, string package)
		{
			bool? isChecked = GrmHandler.AppCompatErrorWindow.CheckBox.IsChecked;
			bool flag = true;
			if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
			{
				List<string> list = RegistryManager.Instance.Guest[ParentWindow.mVmName].GrmDonotShowRuleList.ToList<string>();
				if (!list.Contains(GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].RuleId))
				{
					list.Add(GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][package].RuleId);
				}
				RegistryManager.Instance.Guest[ParentWindow.mVmName].GrmDonotShowRuleList = list.ToArray();
			}
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00023B00 File Offset: 0x00021D00
		private static void AddGRMIndicationForIncompatibleApp(AppIconModel appIcon, MainWindow ParentWindow, GrmRuleSet passedRuleSet)
		{
			ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				switch (EnumHelper.Parse<MessageType>(passedRuleSet.MessageWindow.MessageType, MessageType.None))
				{
				case MessageType.None:
					appIcon.AppIncompatType = AppIncompatType.Error;
					GrmHandler.sDictAppRuleSet[ParentWindow.mVmName].Remove(appIcon.PackageName);
					return;
				case MessageType.Info:
					appIcon.AppIncompatType = AppIncompatType.Info;
					GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][appIcon.PackageName] = passedRuleSet;
					return;
				case MessageType.Error:
					appIcon.AppIncompatType = AppIncompatType.Error;
					GrmHandler.sDictAppRuleSet[ParentWindow.mVmName][appIcon.PackageName] = passedRuleSet;
					return;
				default:
					return;
				}
			}), new object[0]);
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x00023B4C File Offset: 0x00021D4C
		private static void RemoveAppCompatError(AppIconModel appIcon, MainWindow ParentWindow)
		{
			ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				appIcon.AppIncompatType = AppIncompatType.None;
				GrmHandler.sDictAppRuleSet[ParentWindow.mVmName].Remove(appIcon.PackageName);
			}), new object[0]);
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x00005F0C File Offset: 0x0000410C
		internal static void RemovePackageFromGrmList(string packageName, string vmName)
		{
			if (GrmHandler.sDictAppRuleSet.ContainsKey(vmName))
			{
				GrmHandler.sDictAppRuleSet[vmName].Remove(packageName);
			}
		}

		// Token: 0x0400031F RID: 799
		private static Dictionary<string, Dictionary<string, GrmRuleSet>> sDictAppRuleSet = new Dictionary<string, Dictionary<string, GrmRuleSet>>();

		// Token: 0x04000320 RID: 800
		private static CustomMessageWindow AppCompatErrorWindow = null;
	}
}
