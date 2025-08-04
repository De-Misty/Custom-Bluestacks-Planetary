using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000039 RID: 57
	[Serializable]
	public class AppPackageListObject
	{
		// Token: 0x1700016C RID: 364
		// (get) Token: 0x0600039F RID: 927 RVA: 0x00004606 File Offset: 0x00002806
		// (set) Token: 0x060003A0 RID: 928 RVA: 0x0000460E File Offset: 0x0000280E
		[JsonProperty(PropertyName = "app_pkg_list", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public List<string> CloudPackageList { get; set; } = new List<string>();

		// Token: 0x060003A1 RID: 929 RVA: 0x00004617 File Offset: 0x00002817
		public AppPackageListObject(List<string> packageList)
		{
			this.CloudPackageList = packageList;
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x000196C4 File Offset: 0x000178C4
		public bool IsPackageAvailable(string appPackage)
		{
			foreach (string text in this.CloudPackageList)
			{
				string text2 = text;
				if (text.EndsWith("*", StringComparison.InvariantCulture))
				{
					text2 = text.TrimEnd(new char[] { '*' });
				}
				if (text2.StartsWith("~", StringComparison.InvariantCulture))
				{
					if (appPackage.StartsWith(text2.Substring(1), StringComparison.InvariantCulture))
					{
						return false;
					}
				}
				else if (appPackage.StartsWith(text2, StringComparison.InvariantCulture))
				{
					return true;
				}
			}
			return false;
		}
	}
}
