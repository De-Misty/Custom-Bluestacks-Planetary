using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using BlueStacks.Common;
using Microsoft.Win32;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000291 RID: 657
	public partial class App : global::System.Windows.Application
	{
		// Token: 0x17000368 RID: 872
		// (get) Token: 0x060017F2 RID: 6130 RVA: 0x00010270 File Offset: 0x0000E470
		// (set) Token: 0x060017F3 RID: 6131 RVA: 0x00010277 File Offset: 0x0000E477
		public static Mutex BlueStacksUILock
		{
			get
			{
				return App.mBluestacksUILock;
			}
			set
			{
				App.mBluestacksUILock = value;
			}
		}

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x0001027F File Offset: 0x0000E47F
		// (set) Token: 0x060017F5 RID: 6133 RVA: 0x00010286 File Offset: 0x0000E486
		internal static bool IsApplicationActive { get; set; }

		// Token: 0x060017F7 RID: 6135 RVA: 0x0008E7C4 File Offset: 0x0008C9C4
		private static void HandleDisplaySettingsChanged(object sender, EventArgs e)
		{
			try
			{
				foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
				{
					if (mainWindow != null && !mainWindow.mClosed)
					{
						mainWindow.HandleDisplaySettingsChanged();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
			}
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x0008E850 File Offset: 0x0008CA50
		private static void ParseWebMagnetArgs(ref string[] args)
		{
			if (args.Length != 0 && args[0].StartsWith("bluestacksgp:", StringComparison.InvariantCultureIgnoreCase))
			{
				Logger.Info("Handling web uri: " + args[0]);
				string[] array = args[0].Split(new char[] { ':' }, 2);
				string[] array2 = new string[args.Length + 1];
				string[] array3 = Uri.UnescapeDataString(array[1]).TrimStart(new char[0]).Split(new char[] { ' ' }, 2);
				if (array3.Length > 1)
				{
					Array.Copy(array3, 0, array2, 0, 2);
					Array.Copy(args, 1, array2, 2, args.Length - 1);
					args = array2;
					return;
				}
				args[0] = array3[0];
			}
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x0008E8FC File Offset: 0x0008CAFC
		private static void InitExceptionAndLogging()
		{
			Logger.InitLog("BlueStacksUI", "BlueStacksUI", true);
			global::System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			ThreadExceptionEventHandler threadExceptionEventHandler;
			if ((threadExceptionEventHandler = App.O.Application_ThreadException) == null)
			{
				threadExceptionEventHandler = (App.O.Application_ThreadException = new ThreadExceptionEventHandler(App.Application_ThreadException));
			}
			global::System.Windows.Forms.Application.ThreadException += threadExceptionEventHandler;
			AppDomain currentDomain = AppDomain.CurrentDomain;
			UnhandledExceptionEventHandler unhandledExceptionEventHandler;
			if ((unhandledExceptionEventHandler = App.O.CurrentDomain_UnhandledException) == null)
			{
				unhandledExceptionEventHandler = (App.O.CurrentDomain_UnhandledException = new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException));
			}
			currentDomain.UnhandledException += unhandledExceptionEventHandler;
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x0001028E File Offset: 0x0000E48E
		private static void Application_Startup(object sender, StartupEventArgs e)
		{
			Logger.Info("In Application_Startup");
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(App.ValidateRemoteCertificate));
			ServicePointManager.DefaultConnectionLimit = 1000;
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x00005AAF File Offset: 0x00003CAF
		private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return true;
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x0008E968 File Offset: 0x0008CB68
		private static void CheckIfAlreadyRunning()
		{
			try
			{
				if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_DiskCompactor_Lockbgp64"))
				{
					Logger.Info("Disk compaction is running in background");
					using (List<string>.Enumerator enumerator = GetProcessExecutionPath.GetApplicationPath(Process.GetProcessesByName("DiskCompactionTool")).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Equals(Path.Combine(RegistryStrings.InstallDir, "DiskCompactionTool.exe"), StringComparison.InvariantCultureIgnoreCase))
							{
								CustomMessageWindow customMessageWindow = new CustomMessageWindow();
								customMessageWindow.ImageName = "ProductLogo";
								customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_HEADING", "");
								customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_MESSAGE", "");
								customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
								customMessageWindow.CloseButtonHandle(null, null);
								customMessageWindow.ShowDialog();
								Logger.Info("Disk compaction running for this instance. Exiting this instance");
								App.ExitApplication();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to check if disk compaction is running: " + ex.Message);
			}
			string text;
			if (!Opt.Instance.force && ProcessUtils.IsAnyInstallerProcesRunning(out text) && !string.IsNullOrEmpty(text))
			{
				Logger.Info(text + " process is running. Exiting BlueStacks");
				App.ExitApplication();
			}
			ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_BlueStacksUI_Lockbgp64", out App.mBluestacksUILock);
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x0008EACC File Offset: 0x0008CCCC
		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			if (App.CheckForIgnoredExceptions(e.Exception.ToString()))
			{
				Logger.Error("Unhandled Thread Exception:");
				Logger.Error(e.Exception.ToString());
				if (!FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					global::System.Windows.Forms.MessageBox.Show("BlueStacks App Player.\nError: " + e.Exception.ToString());
				}
				App.ExitApplication();
			}
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x0008EB34 File Offset: 0x0008CD34
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (App.CheckForIgnoredExceptions(((Exception)e.ExceptionObject).ToString()))
			{
				Logger.Error("Unhandled Application Exception.");
				Logger.Error("Err: " + e.ExceptionObject.ToString());
				if (!FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					global::System.Windows.Forms.MessageBox.Show("BlueStacks App Player.\nError: " + ((Exception)e.ExceptionObject).ToString());
				}
				App.ExitApplication();
			}
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x000102C4 File Offset: 0x0000E4C4
		private static bool CheckForIgnoredExceptions(string s)
		{
			if (s.Contains("GetFocusedElementFromWinEvent"))
			{
				Logger.Warning("Ignoring Unhandled Application Exception: " + s);
				return false;
			}
			return true;
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x0008EBB0 File Offset: 0x0008CDB0
		internal static void ExitApplication()
		{
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
			{
				if (mainWindow != null && !mainWindow.mClosed)
				{
					mainWindow.ForceCloseWindow(false);
				}
			}
			App.UnwindEvents();
			App.ReleaseLock();
			Process.GetCurrentProcess().Kill();
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x0008EC2C File Offset: 0x0008CE2C
		internal static void UnwindEvents()
		{
			try
			{
				EventHandler eventHandler;
				if ((eventHandler = App.O.HandleDisplaySettingsChanged) == null)
				{
					eventHandler = (App.O.HandleDisplaySettingsChanged = new EventHandler(App.HandleDisplaySettingsChanged));
				}
				SystemEvents.DisplaySettingsChanged -= eventHandler;
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't unwind events properly; " + ex.ToString());
			}
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x0008EC88 File Offset: 0x0008CE88
		internal static void ReleaseLock()
		{
			try
			{
				BluestacksProcessHelper.TakeLock("Global\\BlueStacks_BlueStacksUI_Closing_Lockbgp64");
				if (App.BlueStacksUILock != null)
				{
					App.BlueStacksUILock.Close();
					App.BlueStacksUILock = null;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Ignoring Exception while releasing lock. Err : " + ex.ToString());
			}
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x0008ECE4 File Offset: 0x0008CEE4
		private void Application_Activated(object sender, EventArgs e)
		{
			App.IsApplicationActive = true;
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
			{
				mainWindow.SendTempGamepadState(true);
			}
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x0008ED44 File Offset: 0x0008CF44
		private void Application_Deactivated(object sender, EventArgs e)
		{
			App.IsApplicationActive = false;
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
			{
				if (mainWindow.mStreamingModeEnabled)
				{
					mainWindow.SendTempGamepadState(true);
				}
				else
				{
					mainWindow.SendTempGamepadState(false);
				}
			}
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x000102EF File Offset: 0x0000E4EF
		public App()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x0008EE0C File Offset: 0x0008D00C
		private static void InitializeFpsBind()
		{
			try
			{
				App.fpsBindThread = new Thread(delegate
				{
					try
					{
						Thread.Sleep(5000);
						FpsBind.SetSynchronizationContextAsync();
					}
					catch (Exception ex2)
					{
						File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR: {1}\n{2}\n\n", DateTime.Now, ex2.Message, ex2.StackTrace));
					}
				})
				{
					IsBackground = true,
					Name = "BlueStacks FPS Manager",
					Priority = ThreadPriority.BelowNormal
				};
				App.fpsBindThread.Start();
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to initialize FpsBind: " + ex.Message);
			}
		}

		// Token: 0x04000F25 RID: 3877
		private static Mutex mBluestacksUILock;

		// Token: 0x04000F27 RID: 3879
		internal static Fraction defaultResolution;

		// Token: 0x04000F29 RID: 3881
		private static Thread fpsBindThread;

		// Token: 0x02000293 RID: 659
		[CompilerGenerated]
		private static class O
		{
			// Token: 0x04000F2C RID: 3884
			public static StartupEventHandler Application_Startup;

			// Token: 0x04000F2D RID: 3885
			public static EventHandler HandleDisplaySettingsChanged;

			// Token: 0x04000F2E RID: 3886
			public static ThreadExceptionEventHandler Application_ThreadException;

			// Token: 0x04000F2F RID: 3887
			public static UnhandledExceptionEventHandler CurrentDomain_UnhandledException;
		}
	}
}
