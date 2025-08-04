using System;
using System.Collections.Generic;
using System.Globalization;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000095 RID: 149
	internal sealed class CloudNotificationManager
	{
		// Token: 0x06000667 RID: 1639 RVA: 0x00003957 File Offset: 0x00001B57
		private CloudNotificationManager()
		{
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x000248E4 File Offset: 0x00022AE4
		public static CloudNotificationManager Instance
		{
			get
			{
				if (CloudNotificationManager.sInstance == null)
				{
					object obj = CloudNotificationManager.syncRoot;
					lock (obj)
					{
						if (CloudNotificationManager.sInstance == null)
						{
							CloudNotificationManager.sInstance = new CloudNotificationManager();
						}
					}
				}
				return CloudNotificationManager.sInstance;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x0002493C File Offset: 0x00022B3C
		private static SerialWorkQueue WorkQueue
		{
			get
			{
				if (CloudNotificationManager.mWorkQueue == null)
				{
					object obj = CloudNotificationManager.syncRoot;
					lock (obj)
					{
						if (CloudNotificationManager.mWorkQueue == null)
						{
							CloudNotificationManager.mWorkQueue = new SerialWorkQueue("androidCloudNotifications");
							if (BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mGuestBootCompleted)
							{
								CloudNotificationManager.mWorkQueue.Start();
							}
						}
					}
				}
				return CloudNotificationManager.mWorkQueue;
			}
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x000249B0 File Offset: 0x00022BB0
		internal void HandleCloudNotification(string jsonReceived, string vmName)
		{
			try
			{
				Logger.Info("CloudFireBaseNotification response received: " + jsonReceived + " from vm: " + vmName);
				JObject jobject = JObject.Parse(jsonReceived);
				if (jobject["bluestacks_notification"] != null && jobject["bluestacks_notification"].ToObject<JObject>()["tag"] != null && !JsonExtensions.IsNullOrEmptyBrackets(jobject["bluestacks_notification"].GetValue("tag")))
				{
					CloudNotificationManager.HandleTagsInfo(jobject, jsonReceived);
				}
				if (jobject["bluestacks_notification"] != null && jobject["bluestacks_notification"].ToObject<JObject>()["type"] != null)
				{
					string text = jobject["bluestacks_notification"]["type"].ToString().ToLower(CultureInfo.InvariantCulture);
					if (text != null)
					{
						if (text == "genericnotification")
						{
							CloudNotificationManager.HandleGenericNotification(jobject, vmName);
							goto IL_0111;
						}
						if (text == "genericreddotnotification")
						{
							CloudNotificationManager.HandleGenericRedDotNotification(jobject, vmName);
							goto IL_0111;
						}
						if (text == "callmethod")
						{
							CloudNotificationManager.HandleCallMethod(jobject, vmName);
							goto IL_0111;
						}
					}
					Logger.Warning("No notification type found in HandleCloudNotification. json: " + jsonReceived);
				}
				IL_0111:;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleCloudNotification. json: " + jsonReceived + " Error: " + ex.ToString());
			}
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x00024B0C File Offset: 0x00022D0C
		private static void HandleTagsInfo(JObject json, string jsonReceived)
		{
			try
			{
				foreach (string text in json["bluestacks_notification"]["tag"].ToObject<List<string>>())
				{
					if (BrowserControl.mFirebaseTagsSubscribed.Contains(text))
					{
						CloudNotificationManager.SendNotifJsonToHtmlTag(text, jsonReceived);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleTagsInfo: " + ex.ToString());
			}
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x00024BA8 File Offset: 0x00022DA8
		private static void SendNotifJsonToHtmlTag(string _, string data)
		{
			try
			{
				object[] array = new object[] { "" };
				if (!string.IsNullOrEmpty(data))
				{
					array[0] = data;
				}
				foreach (BrowserControl browserControl in BrowserControl.sAllBrowserControls)
				{
					if (browserControl != null && browserControl.CefBrowser != null)
					{
						browserControl.CefBrowser.CallJs(browserControl.mFirebaseCallbackMethod, array);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in sending json to appcenter:" + ex.ToString());
			}
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00024C54 File Offset: 0x00022E54
		internal static SerializableDictionary<string, string> HandleExtraPayload(JObject json, NotificationPayloadType payloadType)
		{
			return json.ToObject<SerializableDictionary<string, string>>();
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x00024C70 File Offset: 0x00022E70
		internal static void HandleGenericRedDotNotification(JObject resJson, string vmName)
		{
			JObject jobject = JObject.Parse(resJson["bluestacks_notification"]["payload"]["GenericRedDotNotificationItem"].ToString());
			if (!JsonExtensions.IsNullOrEmptyBrackets(jobject["myapps_cross_promotion"].ToString()))
			{
				PromotionManager.AddNewMyAppsCrossPromotion(jobject);
				PromotionObject.Save();
				string appPackage = jobject["myapps_cross_promotion"]["app_pkg"].ToString();
				BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
				{
					BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.AddIconWithRedDot(appPackage);
				}), new object[0]);
			}
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00024D24 File Offset: 0x00022F24
		internal static void HandleGenericNotification(JObject resJson, string vmName)
		{
			GenericNotificationItem genericItem = new GenericNotificationItem();
			try
			{
				JObject jobject = JObject.Parse(resJson["bluestacks_notification"]["payload"]["GenericNotificationItem"].ToString());
				jobject.AssignIfContains("id", delegate(string x)
				{
					genericItem.Id = x;
				});
				jobject.AssignIfContains("priority", delegate(string x)
				{
					genericItem.Priority = EnumHelper.Parse<NotificationPriority>(x, NotificationPriority.Normal);
				});
				jobject.AssignIfContains("title", delegate(string x)
				{
					genericItem.Title = x;
				});
				jobject.AssignIfContains("message", delegate(string x)
				{
					genericItem.Message = x;
				});
				jobject.AssignIfContains("showribbon", delegate(bool x)
				{
					genericItem.ShowRibbon = x;
				});
				jobject.AssignIfContains("menuimagename", delegate(string x)
				{
					genericItem.NotificationMenuImageName = x;
				});
				jobject.AssignIfContains("menuimageurl", delegate(string x)
				{
					genericItem.NotificationMenuImageUrl = x;
				});
				jobject.AssignIfContains("isread", delegate(bool x)
				{
					genericItem.IsRead = x;
				});
				jobject.AssignIfContains("isdeleted", delegate(bool x)
				{
					genericItem.IsDeleted = x;
				});
				jobject.AssignIfContains("deferred", delegate(bool x)
				{
					genericItem.IsDeferred = x;
				});
				jobject.AssignIfContains("creationtime", delegate(string x)
				{
					genericItem.CreationTime = DateTime.ParseExact(x, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
				});
				if (!string.IsNullOrEmpty(genericItem.NotificationMenuImageName) && !string.IsNullOrEmpty(genericItem.NotificationMenuImageUrl))
				{
					genericItem.NotificationMenuImageName = Utils.TinyDownloader(genericItem.NotificationMenuImageUrl, genericItem.NotificationMenuImageName, RegistryStrings.PromotionDirectory, false);
				}
				if (jobject["ExtraPayload"] != null && !JsonExtensions.IsNullOrEmptyBrackets(jobject.GetValue("ExtraPayload", StringComparison.InvariantCulture).ToString()))
				{
					jobject["ExtraPayload"].AssignIfContains("payloadtype", delegate(string x)
					{
						genericItem.PayloadType = EnumHelper.Parse<NotificationPayloadType>(x, NotificationPayloadType.Generic);
					});
					SerializableDictionary<string, string> extraPayload = genericItem.ExtraPayload;
					if (extraPayload != null)
					{
						extraPayload.ClearAddRange(CloudNotificationManager.HandleExtraPayload(jobject.GetValue("ExtraPayload", StringComparison.InvariantCulture).ToObject<JObject>(), genericItem.PayloadType));
					}
				}
				ClientStats.SendMiscellaneousStatsAsync("notification_received", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, genericItem.Id, genericItem.Title, genericItem.ExtraPayload.ContainsKey("campaign_id") ? genericItem.ExtraPayload["campaign_id"] : "", null, null, null);
				genericItem.IsReceivedStatSent = true;
				if (jobject["conditions"] != null && !JsonExtensions.IsNullOrEmptyBrackets(jobject.GetValue("conditions", StringComparison.InvariantCulture).ToString()))
				{
					jobject["conditions"].AssignIfContains("app_pkg_on_top", delegate(string x)
					{
						genericItem.DeferredApp = x;
					});
					jobject["conditions"].AssignIfContains("app_usage_seconds", delegate(long x)
					{
						genericItem.DeferredAppUsage = x;
					});
				}
				if (genericItem.ShowRibbon && resJson["bluestacks_notification"]["payload"].ToObject<JObject>()["RibbonDesign"] != null && !JsonExtensions.IsNullOrEmptyBrackets(resJson["bluestacks_notification"]["payload"].GetValue("RibbonDesign")))
				{
					genericItem.NotificationDesignItem = new GenericNotificationDesignItem();
					JObject jobject2 = JObject.Parse(resJson["bluestacks_notification"]["payload"]["RibbonDesign"].ToString());
					jobject2.AssignIfContains("titleforegroundcolor", delegate(string x)
					{
						genericItem.NotificationDesignItem.TitleForeGroundColor = x;
					});
					jobject2.AssignIfContains("messageforegroundcolor", delegate(string x)
					{
						genericItem.NotificationDesignItem.MessageForeGroundColor = x;
					});
					jobject2.AssignIfContains("bordercolor", delegate(string x)
					{
						genericItem.NotificationDesignItem.BorderColor = x;
					});
					jobject2.AssignIfContains("ribboncolor", delegate(string x)
					{
						genericItem.NotificationDesignItem.Ribboncolor = x;
					});
					jobject2.AssignIfContains("auto_hide_timer", delegate(double x)
					{
						genericItem.NotificationDesignItem.AutoHideTime = x;
					});
					jobject2.AssignIfContains("hoverbordercolor", delegate(string x)
					{
						genericItem.NotificationDesignItem.HoverBorderColor = x;
					});
					jobject2.AssignIfContains("hoverribboncolor", delegate(string x)
					{
						genericItem.NotificationDesignItem.HoverRibboncolor = x;
					});
					jobject2.AssignIfContains("leftgifname", delegate(string x)
					{
						genericItem.NotificationDesignItem.LeftGifName = x;
					});
					jobject2.AssignIfContains("leftgifurl", delegate(string x)
					{
						genericItem.NotificationDesignItem.LeftGifUrl = x;
					});
					if (!string.IsNullOrEmpty(genericItem.NotificationDesignItem.LeftGifName) && !string.IsNullOrEmpty(genericItem.NotificationDesignItem.LeftGifUrl))
					{
						Utils.TinyDownloader(genericItem.NotificationDesignItem.LeftGifUrl, genericItem.NotificationDesignItem.LeftGifName, RegistryStrings.PromotionDirectory, false);
					}
					if (jobject2["background_gradient"] != null)
					{
						foreach (JObject jobject3 in JArray.Parse(jobject2["background_gradient"].ToString()).ToObject<List<JObject>>())
						{
							genericItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>(jobject3["color"].ToString(), jobject3["offset"].ToObject<double>()));
						}
					}
					if (jobject2["hover_background_gradient"] != null)
					{
						foreach (JObject jobject4 in JArray.Parse(jobject2["hover_background_gradient"].ToString()).ToObject<List<JObject>>())
						{
							genericItem.NotificationDesignItem.HoverBackGroundGradient.Add(new SerializableKeyValuePair<string, double>(jobject4["color"].ToString(), jobject4["offset"].ToObject<double>()));
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while parsing generic notification. Not showing notification and not adding in notification menu." + ex.ToString());
				return;
			}
			try
			{
				if (string.IsNullOrEmpty(genericItem.Title) && string.IsNullOrEmpty(genericItem.Message))
				{
					genericItem.IsDeleted = true;
				}
				if (!genericItem.IsDeferred)
				{
					GenericNotificationManager.AddNewNotification(genericItem, false);
				}
				if (genericItem.ShowRibbon && resJson["bluestacks_notification"]["payload"].ToObject<JObject>()["RibbonDesign"] != null && !JsonExtensions.IsNullOrEmptyBrackets(resJson["bluestacks_notification"]["payload"].GetValue("RibbonDesign")))
				{
					if (!genericItem.IsDeferred)
					{
						BlueStacksUIUtils.DictWindows[vmName].HandleGenericNotificationPopup(genericItem);
					}
					else
					{
						CloudNotificationManager.HandleDeferredNotification(genericItem);
					}
				}
				BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception when handling notification json. Id " + genericItem.Id + " Error: " + ex2.ToString());
			}
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0000633B File Offset: 0x0000453B
		private static void HandleDeferredNotification(GenericNotificationItem genericItem)
		{
			PromotionManager.sDeferredNotificationsList.Add(genericItem, AppUsageTimer.GetTotalTimeForPackageAfterReset(genericItem.DeferredApp.ToLower(CultureInfo.InvariantCulture)));
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x000254A8 File Offset: 0x000236A8
		internal static void HandleCallMethod(JObject resJson, string vmName)
		{
			CloudNotificationManager.<>c__DisplayClass15_0 CS$<>8__locals1 = new CloudNotificationManager.<>c__DisplayClass15_0();
			CS$<>8__locals1.vmName = vmName;
			CS$<>8__locals1.resJson = resJson;
			string text = "";
			JObject.Parse(CS$<>8__locals1.resJson["bluestacks_notification"]["payload"].ToString()).AssignStringIfContains("methodName", ref text);
			string text2 = text.ToLower(CultureInfo.InvariantCulture);
			if (text2 != null)
			{
				if (text2 == "appusagestats")
				{
					CloudNotificationManager.HandleUsageNotification(CS$<>8__locals1.resJson, CS$<>8__locals1.vmName);
					return;
				}
				if (text2 == "updatepromotions")
				{
					PromotionManager.ReloadPromotionsAsync();
					return;
				}
				if (text2 == "updatebstconfig")
				{
					CloudNotificationManager.UpdateBstConfig();
					return;
				}
				if (text2 == "openquitpopup")
				{
					CloudNotificationManager.OpenQuitPopup(CS$<>8__locals1.resJson, CS$<>8__locals1.vmName);
					return;
				}
				if (text2 == "updategrm")
				{
					GrmManager.UpdateGrmAsync(CS$<>8__locals1.resJson["bluestacks_notification"]["payload"]["app_pkg_list"].ToIenumerableString());
					return;
				}
				if (text2 == "calendarentry")
				{
					try
					{
						CS$<>8__locals1.androidPayload = (JObject)CS$<>8__locals1.resJson["bluestacks_notification"]["payload"]["androidPayload"];
						ClientStats.SendCalendarStats("calendar_" + CS$<>8__locals1.resJson["bluestacks_notification"]["payload"]["methodType"].ToString() + "_firebase", CS$<>8__locals1.androidPayload.ContainsKey("startDate") ? CS$<>8__locals1.androidPayload["startDate"].ToString() : "", CS$<>8__locals1.androidPayload.ContainsKey("endDate") ? CS$<>8__locals1.androidPayload["endDate"].ToString() : "", CS$<>8__locals1.androidPayload["location"].ToString(), "", "");
						string text3 = CS$<>8__locals1.resJson["bluestacks_notification"]["payload"]["methodType"].ToString();
						if (text3 != null)
						{
							string text4;
							if (!(text3 == "add"))
							{
								if (!(text3 == "update"))
								{
									if (!(text3 == "delete"))
									{
										goto IL_026C;
									}
									text4 = "deletecalendarevent";
								}
								else
								{
									text4 = "updatecalendarevent";
								}
							}
							else
							{
								text4 = "addcalendarevent";
							}
							string text5 = text4;
							CS$<>8__locals1.endpoint = text5;
							JObject jobject = new JObject(new JProperty("event", CS$<>8__locals1.androidPayload));
							CloudNotificationManager.<>c__DisplayClass15_0 CS$<>8__locals2 = CS$<>8__locals1;
							Dictionary<string, string> dictionary = new Dictionary<string, string>();
							dictionary["event"] = jobject.ToString();
							CS$<>8__locals2.data = dictionary;
							CloudNotificationManager.WorkQueue.Enqueue(delegate
							{
								try
								{
									string text7 = HTTPUtils.SendRequestToGuest(CS$<>8__locals1.endpoint, CS$<>8__locals1.data, CS$<>8__locals1.vmName, 0, null, false, 1, 0, "bgp64");
									Logger.Info("Response for calendarEntry " + text7);
									JObject jobject2 = JObject.Parse(text7);
									ClientStats.SendCalendarStats("calendar_" + CS$<>8__locals1.resJson["bluestacks_notification"]["payload"]["methodType"].ToString() + "_android", CS$<>8__locals1.androidPayload.ContainsKey("startDate") ? CS$<>8__locals1.androidPayload["startDate"].ToString() : "", CS$<>8__locals1.androidPayload.ContainsKey("endDate") ? CS$<>8__locals1.androidPayload["endDate"].ToString() : "", CS$<>8__locals1.androidPayload["location"].ToString(), string.Equals(jobject2["result"].ToString(), "ok", StringComparison.InvariantCultureIgnoreCase).ToString(CultureInfo.InvariantCulture), jobject2.ContainsKey("rowsDeleted") ? jobject2["rowsDeleted"].ToString() : (jobject2.ContainsKey("rowsUpdated") ? jobject2["rowsUpdated"].ToString() : ""));
								}
								catch (Exception ex2)
								{
									Logger.Warning(string.Format("Guest not booted, error in sending Calendar entry event: {0}", ex2));
								}
							});
							return;
						}
						IL_026C:
						throw new Exception("could not identify the methodType ");
					}
					catch (Exception ex)
					{
						Logger.Warning(string.Format("Error in sending Calendar entry event data to android.. Json:{0} error:  {1}", CS$<>8__locals1.resJson, ex));
						return;
					}
				}
			}
			string text6 = "No method type found in HandleCallMethod json: ";
			JObject resJson2 = CS$<>8__locals1.resJson;
			Logger.Error(text6 + ((resJson2 != null) ? resJson2.ToString() : null));
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0000635D File Offset: 0x0000455D
		internal static void UpdateBstConfig()
		{
			RegistryManager.Instance.UpdateBstConfig = true;
			FeatureManager.Init(true);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x000257DC File Offset: 0x000239DC
		internal static void OpenQuitPopup(JObject resJson, string vmName)
		{
			string text = "";
			string text2 = "";
			string text3 = "";
			string text4 = "";
			JObject jobject = JObject.Parse(resJson["bluestacks_notification"]["payload"].ToString());
			jobject.AssignStringIfContains("url", ref text);
			if (!string.IsNullOrEmpty(text))
			{
				jobject.AssignStringIfContains("app_pkg", ref text2);
				jobject.AssignStringIfContains("force_reload", ref text3);
				jobject.AssignStringIfContains("show_on_quit", ref text4);
				bool flag2;
				bool flag = bool.TryParse(text3, out flag2) && flag2;
				bool flag4;
				bool flag3 = bool.TryParse(text4, out flag4) && flag4;
				text = WebHelper.GetUrlWithParams(text);
				BlueStacksUIUtils.DictWindows[vmName].IsQuitPopupNotficationReceived = true;
				if (BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl == null)
				{
					BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl = new QuitPopupBrowserControl(BlueStacksUIUtils.DictWindows[vmName]);
				}
				BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl.SetQuitPopParams(text, text2, flag, flag3);
				BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl.LoadBrowser();
				return;
			}
			Logger.Info("Quit Popup notification received without url");
			BlueStacksUIUtils.DictWindows[vmName].IsQuitPopupNotficationReceived = false;
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x00025918 File Offset: 0x00023B18
		internal static void HandleUsageNotification(JObject resJson, string vmName)
		{
			try
			{
				string text = "";
				string jsonobjectString = JSONUtils.GetJSONObjectString(AppUsageTimer.GetRealtimeDictionary());
				JObject.Parse(resJson["bluestacks_notification"]["payload"].ToString()).AssignStringIfContains("handler", ref text);
				string text2 = WebHelper.GetServerHost() + "/v2/" + text;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["oem"] = "bgp64";
				dictionary["client_ver"] = RegistryManager.Instance.ClientVersion;
				dictionary["engine_ver"] = RegistryManager.Instance.Version;
				dictionary["guid"] = RegistryManager.Instance.UserGuid;
				dictionary["locale"] = RegistryManager.Instance.UserSelectedLocale;
				dictionary["partner"] = RegistryManager.Instance.Partner;
				dictionary["campaignMD5"] = RegistryManager.Instance.CampaignMD5;
				Dictionary<string, string> dictionary2 = dictionary;
				if (!string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail))
				{
					dictionary2["email"] = RegistryManager.Instance.RegisteredEmail;
				}
				dictionary2["usage_data"] = jsonobjectString;
				if (!dictionary2.ContainsKey("current_app"))
				{
					dictionary2.Add("current_app", BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.SelectedTab.PackageName);
				}
				else
				{
					dictionary2["current_app"] = BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.SelectedTab.PackageName;
				}
				string text3 = BstHttpClient.Post(text2, dictionary2, null, false, string.Empty, 0, 1, 0, false, "bgp64");
				Logger.Info("real time app usage response:" + text3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in handling usage notification" + ex.ToString());
			}
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x00006370 File Offset: 0x00004570
		internal static void PostBootCompleted()
		{
			if (CloudNotificationManager.mWorkQueue != null)
			{
				CloudNotificationManager.mWorkQueue.Start();
			}
		}

		// Token: 0x04000356 RID: 854
		private static volatile CloudNotificationManager sInstance;

		// Token: 0x04000357 RID: 855
		private static object syncRoot = new object();

		// Token: 0x04000358 RID: 856
		private static SerialWorkQueue mWorkQueue = null;
	}
}
