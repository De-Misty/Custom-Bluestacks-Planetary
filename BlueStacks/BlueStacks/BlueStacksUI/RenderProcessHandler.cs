using System;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200022A RID: 554
	internal sealed class RenderProcessHandler : CefRenderProcessHandler
	{
		// Token: 0x060014AF RID: 5295 RVA: 0x0007CAA8 File Offset: 0x0007ACA8
		protected override void OnWebKitInitialized()
		{
			string text = "var gmApi = function(jsonArg) {\r\n                    native function MyNativeFunction(jsonArg);\r\n                    return MyNativeFunction(jsonArg);\r\n                };";
			CefRuntime.RegisterExtension("MessageEvent", text, this.myCefV8Handler);
			base.OnWebKitInitialized();
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x0000E59F File Offset: 0x0000C79F
		protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
		{
			this.myCefV8Handler.OnProcessMessageReceived(message);
			return base.OnProcessMessageReceived(browser, sourceProcess, message);
		}

		// Token: 0x04000D01 RID: 3329
		private MyCustomCefV8Handler myCefV8Handler = new MyCustomCefV8Handler();
	}
}
