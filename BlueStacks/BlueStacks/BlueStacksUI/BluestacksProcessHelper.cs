using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001DF RID: 479
	internal class BluestacksProcessHelper
	{
		// Token: 0x060012D8 RID: 4824 RVA: 0x00072CF0 File Offset: 0x00070EF0
		internal static int StartFrontend(string vmName)
		{
			try
			{
				string installDir = RegistryStrings.InstallDir;
				string text = Path.Combine(installDir, "HD-Player.exe");
				string text2 = (FeatureManager.Instance.IsUseWpfTextbox ? " -w" : "");
				string text3 = " -h";
				if (RegistryManager.Instance.DevEnv == 1)
				{
					text3 = "";
				}
				Process process = new Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.FileName = text;
				process.StartInfo.Arguments = vmName + text3 + text2;
				process.StartInfo.WorkingDirectory = installDir;
				Logger.Info("Starting Frontend for vm: {0} with args: {1}", new object[]
				{
					vmName,
					process.StartInfo.Arguments
				});
				process.Start();
				process.WaitForExit();
				return process.ExitCode;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in starting frontend. Err : " + ex.ToString());
			}
			return 0;
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x00072DFC File Offset: 0x00070FFC
		public static void RunUpdateInstaller(string filePath, string arg, bool isAdmin = false)
		{
			Logger.Info("RunUpdateInstaller start");
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = filePath;
					process.StartInfo.Arguments = arg;
					if (isAdmin)
					{
						process.StartInfo.Verb = "runas";
					}
					process.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in running update installer " + ex.ToString());
			}
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x00072E90 File Offset: 0x00071090
		public static Process StartBluestacks(string vmName)
		{
			string text = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = text;
			process.StartInfo.Arguments = "-vmname:" + vmName + " -h";
			Logger.Info("Sending RunApp for vm calling {0}", new object[] { vmName });
			Logger.Info("Utils: Starting hidden Frontend");
			process.Start();
			return process;
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x00072F18 File Offset: 0x00071118
		public static int RunApkInstaller(string apkPath, bool isSilentInstall, string vmName)
		{
			Logger.Info("Installing apk :{0} vmname: {1} ", new object[] { apkPath, vmName });
			if (vmName == null)
			{
				vmName = "Android";
			}
			int num = -1;
			try
			{
				string installDir = RegistryStrings.InstallDir;
				ProcessStartInfo processStartInfo = new ProcessStartInfo
				{
					WorkingDirectory = installDir
				};
				if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
				{
					processStartInfo.FileName = Path.Combine(installDir, "HD-XapkHandler.exe");
					if (isSilentInstall)
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-xapk \"{0}\" -s -vmname {1}", new object[] { apkPath, vmName });
					}
					else
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-xapk \"{0}\" -vmname {1}", new object[] { apkPath, vmName });
					}
				}
				else
				{
					processStartInfo.FileName = Path.Combine(installDir, "HD-ApkHandler.exe");
					if (isSilentInstall)
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-apk \"{0}\" -s -vmname {1}", new object[] { apkPath, vmName });
					}
					else
					{
						processStartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-apk \"{0}\" -vmname {1}", new object[] { apkPath, vmName });
					}
				}
				processStartInfo.UseShellExecute = false;
				processStartInfo.CreateNoWindow = true;
				Logger.Info("Console: installer path {0}", new object[] { processStartInfo.FileName });
				Process process = Process.Start(processStartInfo);
				process.WaitForExit();
				num = process.ExitCode;
				Logger.Info("Console: apk installer exit code: {0}", new object[] { process.ExitCode });
			}
			catch (Exception ex)
			{
				Logger.Info("Error Installing Apk : " + ex.ToString());
			}
			return num;
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x000730C0 File Offset: 0x000712C0
		internal static bool TakeLock(string lockBane)
		{
			Mutex mutex;
			return ProcessUtils.CheckAlreadyRunningAndTakeLock(lockBane, out mutex);
		}
	}
}
