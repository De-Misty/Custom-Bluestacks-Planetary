using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001D0 RID: 464
	internal class ClientStats
	{
		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06001283 RID: 4739 RVA: 0x00071B34 File Offset: 0x0006FD34
		internal static Dictionary<string, string> GetCommonData
		{
			get
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"guid",
						RegistryManager.Instance.UserGuid
					},
					{
						"engine_guid",
						RegistryManager.Instance.UserGuid
					},
					{
						"engine_ver",
						RegistryManager.Instance.Version
					},
					{
						"client_ver",
						RegistryManager.Instance.ClientVersion
					},
					{
						"oem",
						Oem.Instance.OEM
					},
					{
						"campaign_md5",
						RegistryManager.Instance.CampaignMD5
					},
					{
						"partner",
						RegistryManager.Instance.Partner
					},
					{
						"lang",
						RegistryManager.Instance.UserSelectedLocale
					},
					{
						"email",
						RegistryManager.Instance.RegisteredEmail
					},
					{
						"engine_mode",
						RegistryManager.Instance.DeviceCaps
					}
				};
				string campaignJson = RegistryManager.Instance.CampaignJson;
				if (!string.IsNullOrEmpty(campaignJson))
				{
					try
					{
						JObject jobject = JObject.Parse(campaignJson);
						dictionary.Add("campaign_name", jobject["campaign_name"].ToString());
						goto IL_0132;
					}
					catch
					{
						dictionary.Add("campaign_name", "");
						goto IL_0132;
					}
				}
				dictionary.Add("campaign_name", "");
				IL_0132:
				if (!string.IsNullOrEmpty(RegistryManager.Instance.ClientLaunchParams))
				{
					JObject jobject2 = JObject.Parse(RegistryManager.Instance.ClientLaunchParams);
					if (jobject2["campaign_id"] != null)
					{
						dictionary.Add("externalsource_campaignid", jobject2["campaign_id"].ToString());
					}
					if (jobject2["source_version"] != null)
					{
						dictionary.Add("externalsource_version", jobject2["source_version"].ToString());
					}
				}
				return dictionary;
			}
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00071CF8 File Offset: 0x0006FEF8
		internal static void SendClientStatsAsync(string op, string status, string uri, string package = "", string errorCode = "", string vmName = "")
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					ClientStats.SendStatsSync(op, status, uri, package, errorCode, vmName);
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to send stats for uri : " + uri + ". Reason : " + ex.ToString());
				}
			});
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x00071D48 File Offset: 0x0006FF48
		internal static void SendFrontendClickStats(string eventType, string keyword, string app_loc, string app_pkg, string is_installed, string app_position, string app_rank, string apps_recommendation_obj)
		{
			Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
			getCommonData.Add("event", eventType);
			getCommonData.Add("keyword", keyword);
			getCommonData.Add("app_loc", app_loc);
			getCommonData.Add("app_pkg", app_pkg);
			getCommonData.Add("is_installed", is_installed);
			getCommonData.Add("app_position", app_position);
			getCommonData.Add("app_rank", app_rank);
			getCommonData.Add("apps_recommendation_obj", apps_recommendation_obj);
			ClientStats.SendStatsAsync(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", new object[]
			{
				RegistryManager.Instance.Host,
				"frontend_click_stats"
			}), getCommonData, null);
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x00071DF0 File Offset: 0x0006FFF0
		internal static void SendCalendarStats(string eventType, string calendarstartdate, string calendarenddate, string calendarlink, string success = "", string rowsaffected = "")
		{
			Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
			getCommonData.Add("event_type", eventType);
			getCommonData.Add("calendar_start_date", calendarstartdate);
			getCommonData.Add("calendar_end_date", calendarenddate);
			getCommonData.Add("calendar_link", calendarlink);
			getCommonData.Add("success", success);
			getCommonData.Add("rows_affected", rowsaffected);
			ClientStats.SendStatsAsync(RegistryManager.Instance.Host + "/bs4/stats/calendar_stats", getCommonData, null);
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x00071E68 File Offset: 0x00070068
		internal static void SendStatsSync(string op, string status, string uri, string package, string errorCode = "", string vmname = "")
		{
			Dictionary<string, string> data = ClientStats.GetCommonData;
			data.Add("op", op);
			data.Add("status", status);
			string text;
			if (uri != "engine_activity")
			{
				text = "4.220.0.4001";
			}
			else
			{
				text = RegistryManager.Instance.Version;
			}
			data.Add("version", text);
			if (uri == "emulator_activity")
			{
				Dictionary<string, string> dictionary = BlueStacksUIUtils.GetResolutionData();
				try
				{
					dictionary.ToList<KeyValuePair<string, string>>().ForEach(delegate(KeyValuePair<string, string> kvp)
					{
						data[kvp.Key] = kvp.Value;
					});
				}
				catch (Exception ex)
				{
					Logger.Error("Merge dictionary failed. Ex : " + ex.ToString());
				}
				try
				{
					dictionary = BlueStacksUIUtils.GetEngineSettingsData(vmname);
					dictionary.ToList<KeyValuePair<string, string>>().ForEach(delegate(KeyValuePair<string, string> kvp)
					{
						data[kvp.Key] = kvp.Value;
					});
				}
				catch (Exception ex2)
				{
					Logger.Error("Merge dictionary failed. Ex : " + ex2.ToString());
				}
			}
			if (!string.IsNullOrEmpty(errorCode))
			{
				data.Add("error_code", errorCode);
			}
			if (!string.IsNullOrEmpty(package))
			{
				data.Add("app_pkg", package);
			}
			ClientStats.SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", new object[]
			{
				string.IsNullOrEmpty(ClientStats.sDevUrl) ? RegistryManager.Instance.Host : ClientStats.sDevUrl,
				uri
			}), data, null, vmname);
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x0000D46D File Offset: 0x0000B66D
		internal static void SendGPlayClickStats(Dictionary<string, string> clientData)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
					if (clientData != null)
					{
						foreach (KeyValuePair<string, string> keyValuePair in clientData)
						{
							getCommonData.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					ClientStats.SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/gplay_click_stats", new object[] { string.IsNullOrEmpty(ClientStats.sDevUrl) ? RegistryManager.Instance.Host : ClientStats.sDevUrl }), getCommonData, null, "");
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to send gplay stats... Err : " + ex.ToString());
				}
			});
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x00071FF0 File Offset: 0x000701F0
		internal static void SendMiscellaneousStatsAsync(string tag, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6 = null, string arg7 = null, string arg8 = null)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Logger.Info("Sending miscellaneous Stats for tag : " + tag);
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "tag", tag },
						{ "arg1", arg1 },
						{ "arg2", arg2 },
						{ "arg3", arg3 },
						{ "arg4", arg4 },
						{ "arg5", arg5 },
						{ "arg6", arg6 },
						{ "arg7", arg7 },
						{ "arg8", arg8 }
					};
					ClientStats.SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
					{
						RegistryManager.Instance.Host,
						"/stats/miscellaneousstats"
					}), dictionary, null, "");
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
				}
			});
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x0000D48C File Offset: 0x0000B68C
		internal static void SendKeyMappingUIStatsAsync(string eventtype, string packageName, string extraInfo = "")
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Logger.Info("Sending KeyMappingUI Stats");
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{
							"guid",
							RegistryManager.Instance.UserGuid
						},
						{
							"prod_ver",
							RegistryManager.Instance.ClientVersion
						},
						{
							"oem",
							RegistryManager.Instance.Oem
						},
						{ "app_pkg", packageName },
						{ "event_type", eventtype },
						{
							"email",
							RegistryManager.Instance.RegisteredEmail
						},
						{ "extra_info", extraInfo },
						{
							"locale",
							RegistryManager.Instance.UserSelectedLocale
						}
					};
					ClientStats.SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
					{
						RegistryManager.Instance.Host,
						"/stats/keymappinguistats"
					}), dictionary, null, "");
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
				}
			});
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x00072058 File Offset: 0x00070258
		internal static void SendLocalQuitPopupStatsAsync(string tag, string eventType)
		{
			Logger.Debug("Sending LocalQuitPopupStats for {0}", new object[] { eventType });
			string userGuid = RegistryManager.Instance.UserGuid;
			string clientVersion = RegistryManager.Instance.ClientVersion;
			string campaignMD = RegistryManager.Instance.CampaignMD5;
			ClientStats.SendMiscellaneousStatsAsync(tag, eventType, userGuid, clientVersion, campaignMD, "", null, null, null);
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x0000D4B9 File Offset: 0x0000B6B9
		internal static void SendBluestacksUpdaterUIStatsAsync(string eventName, string comment = "")
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Logger.Info("Sending Bluestacks Updater UI Stats");
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "event", eventName },
						{
							"install_id",
							RegistryManager.Instance.InstallID
						},
						{
							"engine_version",
							RegistryManager.Instance.Version
						},
						{
							"client_version",
							RegistryManager.Instance.ClientVersion
						},
						{
							"os",
							Profile.OS
						}
					};
					string text = InstallerArchitectures.AMD64;
					if (!SystemUtils.IsOs64Bit())
					{
						text = InstallerArchitectures.X86;
					}
					dictionary.Add("installer_arch", text);
					dictionary.Add("guid", RegistryManager.Instance.UserGuid);
					dictionary.Add("oem", Oem.Instance.OEM);
					dictionary.Add("campaign_hash", RegistryManager.Instance.CampaignMD5);
					dictionary.Add("campaign_name", RegistryManager.Instance.CampaignName);
					dictionary.Add("locale", RegistryManager.Instance.UserSelectedLocale);
					dictionary.Add("comment", comment);
					dictionary.Add("installation_type", RegistryManager.Instance.InstallationType.ToString());
					dictionary.Add("gaming_pkg_name", RegistryManager.Instance.InstallerPkgName);
					ClientStats.SendStats(string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
					{
						RegistryManager.Instance.Host,
						"/bs3/stats/unified_install_stats"
					}), dictionary, null, "");
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
				}
			});
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x000720AC File Offset: 0x000702AC
		internal static void SendPopupBrowserStatsInMiscASync(string eventType, string url)
		{
			ClientStats.SendMiscellaneousStatsAsync("PopupBrowser", RegistryManager.Instance.UserGuid, eventType, url, RegistryManager.Instance.RegisteredEmail, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, null, null);
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0000D4DF File Offset: 0x0000B6DF
		internal static void SendGeneralStats(string op, Dictionary<string, string> sourceData)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
					getCommonData.Add("op", op);
					if (sourceData != null)
					{
						foreach (KeyValuePair<string, string> keyValuePair in sourceData)
						{
							getCommonData.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					getCommonData.Add("os_ver", string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[]
					{
						Environment.OSVersion.Version.Major,
						Environment.OSVersion.Version.Minor
					}));
					ClientStats.SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/general_json", new object[] { string.IsNullOrEmpty(ClientStats.sDevUrl) ? RegistryManager.Instance.Host : ClientStats.sDevUrl }), getCommonData, null, "");
				}
				catch (Exception ex)
				{
					Logger.Info("Failed to send general stat for op : " + op + "...Err : " + ex.ToString());
				}
			});
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x0000D505 File Offset: 0x0000B705
		internal static void SendStatsAsync(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				try
				{
					ClientStats.SendStats(url, data, headers, "");
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to send stats for uri : " + url + ". Reason : " + ex.ToString());
				}
			});
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x0000D532 File Offset: 0x0000B732
		internal static void SendPromotionAppClickStatsAsync(Dictionary<string, string> appData, string uri)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
				foreach (KeyValuePair<string, string> keyValuePair in appData)
				{
					getCommonData.Add(keyValuePair.Key, keyValuePair.Value);
				}
				ClientStats.SendStats(string.Format(CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", new object[]
				{
					string.IsNullOrEmpty(ClientStats.sDevUrl) ? RegistryManager.Instance.Host : ClientStats.sDevUrl,
					uri
				}), getCommonData, null, "");
			});
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x000720F0 File Offset: 0x000702F0
		internal static void SendStats(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null, string vmname = "")
		{
			try
			{
				BstHttpClient.Post(url, data, headers, false, vmname, 0, 1, 0, false, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Info("Failed to send stats for : " + url + ". Reason : " + ex.ToString());
			}
		}

		// Token: 0x04000BF5 RID: 3061
		private static string sDevUrl = RegistryManager.Instance.BGPDevUrl;
	}
}
