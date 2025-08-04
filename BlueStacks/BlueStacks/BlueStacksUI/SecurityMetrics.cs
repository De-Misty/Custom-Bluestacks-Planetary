using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000A5 RID: 165
	internal class SecurityMetrics : IDisposable
	{
		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x000066D2 File Offset: 0x000048D2
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x000066D9 File Offset: 0x000048D9
		public static SerializableDictionary<string, SecurityMetrics> SecurityMetricsInstanceList { get; set; } = new SerializableDictionary<string, SecurityMetrics>();

		// Token: 0x060006C4 RID: 1732 RVA: 0x00026600 File Offset: 0x00024800
		public SecurityMetrics(string vmName)
		{
			this.mVmName = vmName;
			this.mTimer = new global::System.Timers.Timer
			{
				Interval = 86400000.0
			};
			this.mTimer.Elapsed += this.OnTimedEvent;
			this.mTimer.AutoReset = true;
			this.mTimer.Enabled = true;
			new Thread(delegate
			{
				this.CheckMd5HashOfRootVdi();
				this.CheckAppPlayerRootInfoFromAndroidBstk();
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x000066E1 File Offset: 0x000048E1
		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			this.SendSecurityBreachesStatsToCloud(false);
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x000066EA File Offset: 0x000048EA
		internal static void Init(string vmName)
		{
			if (!SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(vmName))
			{
				SecurityMetrics.SecurityMetricsInstanceList.Add(vmName, new SecurityMetrics(vmName));
			}
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0000670A File Offset: 0x0000490A
		internal void SendSecurityBreachesStatsToCloud(bool isOnClose = false)
		{
			new Thread(delegate
			{
				try
				{
					this.AddBlacklistedRunningApplicationsToSecurityBreaches();
					if (this.mSecurityBreachesList.Count > 0)
					{
						string urlWithParams = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
						{
							RegistryManager.Instance.Host,
							"/bs4/security_metrics"
						}));
						Dictionary<string, string> dictionary = new Dictionary<string, string> { 
						{
							"security_metric_data",
							this.GetSecurityMetricsData()
						} };
						BstHttpClient.Post(urlWithParams, dictionary, null, false, this.mVmName, 10000, 1, 0, false, "bgp64");
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception while sending security stats to cloud : {0}", new object[] { ex.ToString() });
				}
				if (isOnClose)
				{
					SecurityMetrics.SecurityMetricsInstanceList.Remove(this.mVmName);
				}
			}).Start();
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0002668C File Offset: 0x0002488C
		private string GetSecurityMetricsData()
		{
			string text = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter)
				{
					Formatting = Formatting.Indented
				})
				{
					jsonWriter.WriteStartObject();
					foreach (SecurityBreach securityBreach in this.mSecurityBreachesList.Keys)
					{
						if (securityBreach != SecurityBreach.SCRIPT_TOOLS)
						{
							if (securityBreach - SecurityBreach.DEVICE_PROBED <= 3)
							{
								jsonWriter.WritePropertyName(securityBreach.ToString().ToLower(CultureInfo.InvariantCulture));
								jsonWriter.WriteValue(this.mSecurityBreachesList[securityBreach]);
							}
						}
						else
						{
							jsonWriter.WritePropertyName(securityBreach.ToString().ToLower(CultureInfo.InvariantCulture));
							jsonWriter.WriteStartObject();
							jsonWriter.WritePropertyName("running_blacklist_programs");
							jsonWriter.WriteValue(this.mSecurityBreachesList[securityBreach]);
							jsonWriter.WriteEndObject();
						}
					}
					jsonWriter.WriteEndObject();
					text = stringBuilder.ToString();
					Logger.Debug("security data " + text);
				}
			}
			return text;
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x000267DC File Offset: 0x000249DC
		private void AddBlacklistedRunningApplicationsToSecurityBreaches()
		{
			List<string> blackListedApplicationsList = PromotionObject.Instance.BlackListedApplicationsList;
			List<string> list = new List<string>();
			foreach (string text in blackListedApplicationsList)
			{
				if (ProcessUtils.FindProcessByName(text))
				{
					list.Add(text);
				}
			}
			if (list.Count > 0)
			{
				string text2 = JsonConvert.SerializeObject(list);
				this.AddSecurityBreach(SecurityBreach.SCRIPT_TOOLS, text2);
			}
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0002685C File Offset: 0x00024A5C
		internal void AddSecurityBreach(SecurityBreach breach, string data)
		{
			try
			{
				if (!this.mSecurityBreachesList.ContainsKey(breach))
				{
					this.mSecurityBreachesList.Add(breach, data);
					Logger.Info("Security breach added for: {0}", new object[] { breach });
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in adding security breach: {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x000268CC File Offset: 0x00024ACC
		internal void CheckMd5HashOfRootVdi()
		{
			try
			{
				string blockDevice0Path = RegistryManager.Instance.Guest["Android"].BlockDevice0Path;
				string rootVdiMd5Hash = RegistryManager.Instance.RootVdiMd5Hash;
				if (string.IsNullOrEmpty(rootVdiMd5Hash))
				{
					Utils.CreateMD5HashOfRootVdi();
				}
				else
				{
					string md5HashFromFile = Utils.GetMD5HashFromFile(blockDevice0Path);
					if (!string.IsNullOrEmpty(md5HashFromFile) && !string.Equals(md5HashFromFile, rootVdiMd5Hash, StringComparison.OrdinalIgnoreCase))
					{
						this.AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in checking md5 hash of root vdi: {0}", new object[] { ex });
			}
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x0002695C File Offset: 0x00024B5C
		private void CheckAppPlayerRootInfoFromAndroidBstk()
		{
			try
			{
				JArray jarray = JArray.Parse(HTTPUtils.SendRequestToEngine("isAppPlayerRooted", null, this.mVmName, 0, null, false, 1, 0, ""));
				if ((bool)jarray[0]["success"] && (bool)jarray[0]["isRooted"])
				{
					this.AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in checking root info from engine: {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x00006734 File Offset: 0x00004934
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.mTimer != null)
				{
					this.mTimer.Elapsed -= this.OnTimedEvent;
					this.mTimer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x000269F4 File Offset: 0x00024BF4
		~SecurityMetrics()
		{
			this.Dispose(false);
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00006771 File Offset: 0x00004971
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000393 RID: 915
		private Dictionary<SecurityBreach, string> mSecurityBreachesList = new Dictionary<SecurityBreach, string>();

		// Token: 0x04000394 RID: 916
		private string mVmName;

		// Token: 0x04000395 RID: 917
		private global::System.Timers.Timer mTimer;

		// Token: 0x04000396 RID: 918
		private bool disposedValue;
	}
}
