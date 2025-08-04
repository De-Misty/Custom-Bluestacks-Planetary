using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001BB RID: 443
	public sealed class Opt : GetOpt
	{
		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06001192 RID: 4498 RVA: 0x0000C866 File Offset: 0x0000AA66
		// (set) Token: 0x06001193 RID: 4499 RVA: 0x0000C86E File Offset: 0x0000AA6E
		public string vmname { get; set; } = "Android";

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06001194 RID: 4500 RVA: 0x0000C877 File Offset: 0x0000AA77
		// (set) Token: 0x06001195 RID: 4501 RVA: 0x0000C87F File Offset: 0x0000AA7F
		public bool h { get; set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06001196 RID: 4502 RVA: 0x0000C888 File Offset: 0x0000AA88
		// (set) Token: 0x06001197 RID: 4503 RVA: 0x0000C890 File Offset: 0x0000AA90
		public bool mergeCfg { get; set; }

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06001198 RID: 4504 RVA: 0x0000C899 File Offset: 0x0000AA99
		// (set) Token: 0x06001199 RID: 4505 RVA: 0x0000C8A1 File Offset: 0x0000AAA1
		public bool isForceInstall { get; set; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x0600119A RID: 4506 RVA: 0x0000C8AA File Offset: 0x0000AAAA
		// (set) Token: 0x0600119B RID: 4507 RVA: 0x0000C8B2 File Offset: 0x0000AAB2
		public string newPDPath { get; set; } = string.Empty;

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x0600119C RID: 4508 RVA: 0x0000C8BB File Offset: 0x0000AABB
		// (set) Token: 0x0600119D RID: 4509 RVA: 0x0000C8C3 File Offset: 0x0000AAC3
		public bool isUpgradeFromImap13 { get; set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x0600119E RID: 4510 RVA: 0x0000C8CC File Offset: 0x0000AACC
		// (set) Token: 0x0600119F RID: 4511 RVA: 0x0000C8D4 File Offset: 0x0000AAD4
		public bool force { get; set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060011A0 RID: 4512 RVA: 0x0000C8DD File Offset: 0x0000AADD
		// (set) Token: 0x060011A1 RID: 4513 RVA: 0x0000C8E5 File Offset: 0x0000AAE5
		public bool launchedFromSysTray { get; set; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060011A2 RID: 4514 RVA: 0x0000C8EE File Offset: 0x0000AAEE
		// (set) Token: 0x060011A3 RID: 4515 RVA: 0x0000C8F6 File Offset: 0x0000AAF6
		public string Json
		{
			get
			{
				return this.json;
			}
			set
			{
				this.json = Uri.UnescapeDataString(value);
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060011A4 RID: 4516 RVA: 0x0000C904 File Offset: 0x0000AB04
		// (set) Token: 0x060011A5 RID: 4517 RVA: 0x0000C90C File Offset: 0x0000AB0C
		public bool hiddenBootMode { get; set; }

		// Token: 0x060011A6 RID: 4518 RVA: 0x0000C915 File Offset: 0x0000AB15
		private Opt()
		{
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060011A7 RID: 4519 RVA: 0x0006E81C File Offset: 0x0006CA1C
		public static Opt Instance
		{
			get
			{
				if (Opt.instance == null)
				{
					object obj = Opt.syncRoot;
					lock (obj)
					{
						if (Opt.instance == null)
						{
							Opt.instance = new Opt();
						}
					}
				}
				return Opt.instance;
			}
		}

		// Token: 0x04000B86 RID: 2950
		private static volatile Opt instance;

		// Token: 0x04000B87 RID: 2951
		private static object syncRoot = new object();

		// Token: 0x04000B90 RID: 2960
		private string json = "";
	}
}
