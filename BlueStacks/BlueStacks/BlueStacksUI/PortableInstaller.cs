using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000078 RID: 120
	internal class PortableInstaller
	{
		// Token: 0x060005D0 RID: 1488 RVA: 0x000218F0 File Offset: 0x0001FAF0
		internal static void CheckAndRunPortableInstaller()
		{
			try
			{
				if (Oem.Instance.IsPortableInstaller)
				{
					string text = (string)RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "Version", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
					string fullName = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.Trim(new char[] { '\\' })).FullName;
					Logger.InitLogAtPath(Path.Combine(fullName, "Logs\\PortableInstaller.log"), "PortableInstaller", true);
					if (string.IsNullOrEmpty(text) || (string)RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "InstallDir", "", RegistryKeyKind.HKEY_LOCAL_MACHINE) != Path.Combine(fullName, "BlueStacksPF") || Opt.Instance.isForceInstall)
					{
						PortableInstaller.InstallPortableBlueStacks(AppDomain.CurrentDomain.BaseDirectory);
					}
				}
			}
			catch (Exception ex)
			{
				string text2 = "Error in CheckAndRunPortableInstaller";
				Exception ex2 = ex;
				Logger.Info(text2 + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x000219E4 File Offset: 0x0001FBE4
		private static void InstallPortableBlueStacks(string cwd)
		{
			try
			{
				string fullName = Directory.GetParent(cwd.Trim(new char[] { '\\' })).FullName;
				string text = Path.Combine(fullName, "Engine");
				string text2 = Path.Combine(fullName, "BlueStacksPF");
				string text3 = Path.Combine(Path.Combine(text, "Android"), "Android.bstk");
				string text4 = Path.Combine(Path.Combine(text, "Manager"), "BstkGlobal.xml");
				if (File.Exists(text3))
				{
					File.Delete(text3);
				}
				if (File.Exists(text4))
				{
					File.Delete(text4);
				}
				CommonInstallUtils.ModifyDirectoryPermissionsForEveryone(fullName);
				if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "install.bat")))
				{
					if (PortableInstaller.RunInstallBat(text2, text) == 0)
					{
						PortableInstaller.FixRegistries(fullName, text2);
						PortableInstaller.DoComRegistration(text2);
						CommonInstallUtils.InstallVirtualBoxConfig(text, false);
						CommonInstallUtils.InstallVmConfig(text2, text);
					}
				}
				else
				{
					Logger.Error("Install.bat file missing");
				}
			}
			catch (Exception ex)
			{
				string text5 = "Exception in InstallPortableBlueStacks ";
				Exception ex2 = ex;
				Logger.Error(text5 + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x00021AF8 File Offset: 0x0001FCF8
		private static void FixRegistries(string userDefinedDir, string installDir)
		{
			string text = Path.Combine(userDefinedDir, "Engine");
			string text2 = "Android";
			int num = 46;
			int num2 = 54;
			RegistryManager.Instance.SetAccessPermissions();
			RegistryManager.Instance.UserDefinedDir = userDefinedDir.Trim(new char[] { '\\' });
			RegistryManager.Instance.DataDir = text.Trim(new char[] { '\\' }) + "\\";
			RegistryManager.Instance.LogDir = Path.Combine(userDefinedDir, "Logs").Trim(new char[] { '\\' }) + "\\";
			RegistryManager.Instance.InstallDir = installDir.Trim(new char[] { '\\' }) + "\\";
			RegistryManager.Instance.EngineDataDir = Path.Combine(userDefinedDir, "Engine");
			RegistryManager.Instance.ClientInstallDir = Path.Combine(userDefinedDir, "Client");
			RegistryManager.Instance.CefDataPath = Path.Combine(userDefinedDir, "CefData");
			RegistryManager.Instance.SetupFolder = Path.Combine(Directory.GetParent(userDefinedDir).ToString(), "BlueStacksSetup");
			RegistryManager.Instance.PartnerExePath = Path.Combine(RegistryManager.Instance.ClientInstallDir, "BlueStacks.exe");
			RegistryManager.Instance.UserGuid = Guid.NewGuid().ToString();
			Utils.UpdateValueInBootParams("GUID", RegistryManager.Instance.UserGuid, text2, true, "bgp64");
			string text3 = Path.Combine(text, text2);
			text3 += "\\";
			RegistryManager.Instance.Guest[text2].BlockDevice0Name = "sda1";
			RegistryManager.Instance.Guest[text2].BlockDevice0Path = text3 + "Root.vdi";
			RegistryManager.Instance.Guest[text2].BlockDevice1Name = "sdb1";
			RegistryManager.Instance.Guest[text2].BlockDevice1Path = text3 + "Data.vdi";
			RegistryManager.Instance.Guest[text2].BlockDevice2Name = "sdc1";
			RegistryManager.Instance.Guest[text2].BlockDevice2Path = text3 + "SDCard.vdi";
			string text4 = Path.Combine(text, "UserData\\SharedFolder\\");
			RegistryManager.Instance.Guest[text2].SharedFolder0Name = "BstSharedFolder";
			RegistryManager.Instance.Guest[text2].SharedFolder0Path = text4;
			RegistryManager.Instance.Guest[text2].SharedFolder0Writable = 1;
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			RegistryManager.Instance.Guest[text2].SharedFolder1Name = "Pictures";
			RegistryManager.Instance.Guest[text2].SharedFolder1Path = folderPath;
			RegistryManager.Instance.Guest[text2].SharedFolder1Writable = 1;
			string folderPath2 = CommonInstallUtils.GetFolderPath(num2);
			RegistryManager.Instance.Guest[text2].SharedFolder2Name = "PublicPictures";
			RegistryManager.Instance.Guest[text2].SharedFolder2Path = folderPath2;
			RegistryManager.Instance.Guest[text2].SharedFolder2Writable = 1;
			string folderPath3 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			RegistryManager.Instance.Guest[text2].SharedFolder3Name = "Documents";
			RegistryManager.Instance.Guest[text2].SharedFolder3Path = folderPath3;
			RegistryManager.Instance.Guest[text2].SharedFolder3Writable = 1;
			string folderPath4 = CommonInstallUtils.GetFolderPath(num);
			RegistryManager.Instance.Guest[text2].SharedFolder4Name = "PublicDocuments";
			RegistryManager.Instance.Guest[text2].SharedFolder4Path = folderPath4;
			RegistryManager.Instance.Guest[text2].SharedFolder4Writable = 1;
			string text5 = Path.Combine(text, "UserData\\InputMapper");
			RegistryManager.Instance.Guest[text2].SharedFolder5Name = "InputMapper";
			RegistryManager.Instance.Guest[text2].SharedFolder5Path = text5;
			RegistryManager.Instance.Guest[text2].SharedFolder5Writable = 1;
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x00021F20 File Offset: 0x00020120
		private static int RunInstallBat(string installDir, string dataDir)
		{
			Process process = new Process();
			process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
			process.StartInfo.FileName = "install.bat";
			process.StartInfo.Arguments = string.Concat(new string[] { "\"", installDir, "\" \"", dataDir, "\"" });
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			Countdown countDown = new Countdown(2);
			StringBuilder sb = new StringBuilder();
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
			{
				if (outLine.Data != null)
				{
					try
					{
						string data = outLine.Data;
						sb.AppendLine(data);
						Logger.Info(data);
						return;
					}
					catch (Exception ex)
					{
						Console.WriteLine("Exception in RunInstallBat");
						Console.WriteLine(ex.ToString());
						return;
					}
				}
				countDown.Signal();
			};
			process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
			{
				if (outLine.Data != null)
				{
					try
					{
						string data2 = outLine.Data;
						sb.AppendLine(data2);
						Logger.Info(data2);
						return;
					}
					catch (Exception ex2)
					{
						Console.WriteLine("A crash occured in RunInstallBat");
						Console.WriteLine(ex2.ToString());
						return;
					}
				}
				countDown.Signal();
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			int num = 200000;
			process.WaitForExit(num);
			Logger.Info("Exit Code for InstallBat " + process.ExitCode.ToString());
			countDown.Wait();
			return process.ExitCode;
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x00022050 File Offset: 0x00020250
		private static void DoComRegistration(string installDir)
		{
			string text = "HD-ComRegistrar.exe";
			try
			{
				Logger.Info("Starting registration of COM process with: {0}", new object[] { text });
				Process process = new Process();
				process.StartInfo.FileName = Path.Combine(installDir, text);
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.Start();
				process.WaitForExit();
				Logger.Info("ExitCode: {0}", new object[] { process.ExitCode });
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to execute process {0}. Err: {1}", new object[]
				{
					text,
					ex.ToString()
				});
			}
		}
	}
}
