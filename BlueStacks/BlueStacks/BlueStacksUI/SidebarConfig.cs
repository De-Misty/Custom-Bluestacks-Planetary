using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000A7 RID: 167
	internal class SidebarConfig
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x0000679A File Offset: 0x0000499A
		public List<List<string>> GroupElements { get; } = new List<List<string>>();

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x00026B08 File Offset: 0x00024D08
		// (set) Token: 0x060006D6 RID: 1750 RVA: 0x000067A2 File Offset: 0x000049A2
		public static SidebarConfig Instance
		{
			get
			{
				if (SidebarConfig.sInstance == null)
				{
					object obj = SidebarConfig.syncRoot;
					lock (obj)
					{
						if (SidebarConfig.sInstance == null)
						{
							SidebarConfig.sInstance = new SidebarConfig();
							SidebarConfig.sInstance.Init(SidebarConfig.sFilePath);
						}
					}
				}
				return SidebarConfig.sInstance;
			}
			set
			{
				SidebarConfig.sInstance = value;
			}
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00026B70 File Offset: 0x00024D70
		private void Init(string filePath)
		{
			SidebarConfig.InitFile(filePath);
			JObject jobject = JObject.Parse(File.ReadAllText(filePath));
			int num = 0;
			foreach (JProperty jproperty in from x in jobject.Properties()
				orderby x.Name
				select x)
			{
				List<string> list = new List<string>();
				foreach (JProperty jproperty2 in from x in jproperty.Value.ToObject<JObject>().Properties()
					orderby x.Name
					select x)
				{
					list.Add(jproperty2.Value.ToString());
				}
				this.GroupElements.Add(list);
				num++;
			}
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x000067AC File Offset: 0x000049AC
		private static void InitFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				SidebarConfig.InitNewFile(filePath);
			}
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x000067BC File Offset: 0x000049BC
		public static void InitNewFile(string filePath)
		{
			File.Copy(Path.Combine(RegistryStrings.GadgetDir, "sidebar_config.json"), filePath);
		}

		// Token: 0x04000399 RID: 921
		private static volatile SidebarConfig sInstance;

		// Token: 0x0400039A RID: 922
		private static object syncRoot = new object();

		// Token: 0x0400039B RID: 923
		public static string sFilePath = Path.Combine(RegistryStrings.GadgetDir, string.Format(CultureInfo.InvariantCulture, "SidebarConfig_{0}.json", new object[] { "Android" }));
	}
}
