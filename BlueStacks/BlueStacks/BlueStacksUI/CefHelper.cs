using System;
using System.IO;
using System.Windows;
using BlueStacks.Common;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000264 RID: 612
	internal class CefHelper : CefApp
	{
		// Token: 0x06001632 RID: 5682 RVA: 0x0000EEB9 File Offset: 0x0000D0B9
		public CefHelper()
		{
			if (RegistryManager.Instance.CefDevEnv == 0)
			{
				this.mDevToolEnable = false;
			}
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0000EED4 File Offset: 0x0000D0D4
		protected override CefRenderProcessHandler GetRenderProcessHandler()
		{
			return new RenderProcessHandler();
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x000853C4 File Offset: 0x000835C4
		protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
		{
			if (string.IsNullOrEmpty(processType))
			{
				commandLine.AppendSwitch("disable-gpu");
				commandLine.AppendSwitch("disable-gpu-compositing");
				commandLine.AppendSwitch("disable-smooth-scrolling");
				commandLine.AppendSwitch("--enable-system-flash");
				commandLine.AppendSwitch("ppapi-flash-path", Path.Combine(RegistryManager.Instance.CefDataPath, "pepflashplayer.dll"));
				commandLine.AppendSwitch("plugin-policy", "allow");
				commandLine.AppendSwitch("enable-media-stream", "1");
				if (this.mDevToolEnable)
				{
					commandLine.AppendSwitch("enable-begin-frame-scheduling");
				}
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06001635 RID: 5685 RVA: 0x0000EEDB File Offset: 0x0000D0DB
		// (set) Token: 0x06001636 RID: 5686 RVA: 0x0000EEE2 File Offset: 0x0000D0E2
		internal static bool CefInited { get; set; }

		// Token: 0x06001637 RID: 5687 RVA: 0x00085458 File Offset: 0x00083658
		internal static bool InitCef(string[] args, string mBSTProcessIdentifier)
		{
			try
			{
				Logger.Info("Install Boot: CefRuntime.Load");
				CefRuntime.Load(RegistryManager.Instance.CefDataPath);
			}
			catch (DllNotFoundException ex)
			{
				Logger.Info("Install Boot: DllNotFoundException");
				MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			catch (CefRuntimeException ex2)
			{
				Logger.Info("Install Boot: CefRuntimeException");
				MessageBox.Show(ex2.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			catch (Exception ex3)
			{
				Logger.Info("Install Boot: ex");
				MessageBox.Show(ex3.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			CefMainArgs cefMainArgs = new CefMainArgs(args);
			CefHelper cefHelper = new CefHelper();
			CefRuntime.EnableHighDpiSupport();
			if (CefRuntime.ExecuteProcess(cefMainArgs, cefHelper, IntPtr.Zero) != -1)
			{
				return false;
			}
			string text = "Mozilla/5.0(Windows NT 6.2; Win64; x64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36" + mBSTProcessIdentifier;
			if (!SystemUtils.IsOs64Bit())
			{
				text = "Mozilla/5.0(Windows NT 6.2; WOW64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36" + mBSTProcessIdentifier;
			}
			CefSettings cefSettings = new CefSettings
			{
				SingleProcess = false,
				WindowlessRenderingEnabled = true,
				MultiThreadedMessageLoop = true,
				LogSeverity = CefLogSeverity.Verbose,
				BackgroundColor = new CefColor(byte.MaxValue, 39, 41, 65),
				CachePath = Path.Combine(RegistryManager.Instance.CefDataPath, "Cache"),
				PersistSessionCookies = true,
				UserAgent = text,
				Locale = RegistryManager.Instance.UserSelectedLocale
			};
			if (RegistryManager.Instance.CefDebugPort != 0)
			{
				cefSettings.RemoteDebuggingPort = RegistryManager.Instance.CefDebugPort;
			}
			try
			{
				CefRuntime.Initialize(cefMainArgs, cefSettings, cefHelper, IntPtr.Zero);
				Logger.Info("Install Boot: cef Initialized");
			}
			catch (CefRuntimeException ex4)
			{
				MessageBox.Show(ex4.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Hand);
				return false;
			}
			CefHelper.CefInited = true;
			Logger.Info("Install Boot: cef Initialize completed");
			return true;
		}

		// Token: 0x04000D95 RID: 3477
		private bool mDevToolEnable;
	}
}
