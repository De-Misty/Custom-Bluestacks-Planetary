using System;
using System.Collections.Generic;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001C9 RID: 457
	public static class HttpHandlerSetup
	{
		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06001238 RID: 4664 RVA: 0x0000D091 File Offset: 0x0000B291
		// (set) Token: 0x06001239 RID: 4665 RVA: 0x0000D098 File Offset: 0x0000B298
		public static HTTPServer Server { get; set; }

		// Token: 0x0600123A RID: 4666 RVA: 0x00070814 File Offset: 0x0006EA14
		public static void InitHTTPServer(Dictionary<string, HTTPServer.RequestHandler> routes)
		{
			int num = 2871;
			int num2 = num + 10;
			HttpHandlerSetup.Server = HTTPUtils.SetupServer(num, num2, routes, string.Empty);
			RegistryManager.Instance.PartnerServerPort = HttpHandlerSetup.Server.Port;
			HttpHandlerSetup.Server.Run();
		}
	}
}
