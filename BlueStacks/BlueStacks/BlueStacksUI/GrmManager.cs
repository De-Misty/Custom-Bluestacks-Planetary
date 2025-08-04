using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200008B RID: 139
	internal class GrmManager
	{
		// Token: 0x06000613 RID: 1555 RVA: 0x0000608D File Offset: 0x0000428D
		internal static void UpdateGrmAsync(IEnumerable<string> listOfPackages = null)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				if (AppRequirementsParser.Instance.Requirements == null)
				{
					AppRequirementsParser.Instance.PopulateRequirementsFromFile();
				}
				GrmManager.GetGrmFromCloud(listOfPackages);
			});
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00023E20 File Offset: 0x00022020
		private static void GetGrmFromCloud(IEnumerable<string> listOfPackages = null)
		{
			try
			{
				if (listOfPackages != null && listOfPackages.Any<string>())
				{
					List<string> list = new List<string>();
					foreach (string text in RegistryManager.Instance.VmList)
					{
						list = list.Union(JsonParser.GetInstalledAppsList(text)).ToList<string>();
					}
					if (!listOfPackages.Intersect(list).Any<string>())
					{
						return;
					}
				}
				JObject jobject = JObject.Parse(HTTPUtils.SendRequestToCloud("grm/files", null, "Android", 0, null, false, 1, 0, false));
				if ((int)jobject["code"] == 200 && jobject["data"].Value<bool>("success"))
				{
					string text2 = jobject["data"]["files"].Value<string>("translations_file");
					string text3 = BstHttpClient.Get(jobject["data"]["files"].Value<string>("config_file"), null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp64");
					string text4 = BstHttpClient.Get(text2, null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp64");
					AppRequirementsParser.Instance.UpdateOverwriteRequirements(text3, text4);
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Error Getting Grm json " + ex.ToString());
			}
		}
	}
}
