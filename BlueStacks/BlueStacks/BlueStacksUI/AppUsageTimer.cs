using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000AC RID: 172
	public static class AppUsageTimer
	{
		// Token: 0x060006FD RID: 1789 RVA: 0x000068FB File Offset: 0x00004AFB
		internal static void StartTimer(string vmName, string packageName)
		{
			AppUsageTimer.StopTimer();
			AppUsageTimer.sLastAppPackage = packageName;
			AppUsageTimer.sLastVMName = vmName;
			AppUsageTimer.sStopwatch.Reset();
			AppUsageTimer.sStopwatch.Start();
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x000270FC File Offset: 0x000252FC
		internal static void StopTimer()
		{
			if (AppUsageTimer.sStopwatch.IsRunning && !string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage))
			{
				AppUsageTimer.sStopwatch.Stop();
				long num = (long)AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
				if (AppUsageTimer.sDictAppUsageInfo.ContainsKey(AppUsageTimer.sLastVMName))
				{
					Dictionary<string, long> dictionary;
					if (AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName].ContainsKey(AppUsageTimer.sLastAppPackage))
					{
						dictionary = AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName];
						string text = AppUsageTimer.sLastAppPackage;
						dictionary[text] += num;
					}
					else
					{
						AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, num);
					}
					dictionary = AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName];
					dictionary["TotalUsage"] = dictionary["TotalUsage"] + num;
				}
				else
				{
					AppUsageTimer.sDictAppUsageInfo.Add(AppUsageTimer.sLastVMName, new Dictionary<string, long> { { "TotalUsage", num } });
					AppUsageTimer.sDictAppUsageInfo[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, num);
				}
				AppUsageTimer.sLastAppPackage = string.Empty;
			}
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x00027224 File Offset: 0x00025424
		internal static Dictionary<string, Dictionary<string, long>> GetRealtimeDictionary()
		{
			if (AppUsageTimer.sStopwatch.IsRunning && !string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage))
			{
				Dictionary<string, Dictionary<string, long>> dictionary = new Dictionary<string, Dictionary<string, long>>();
				foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in AppUsageTimer.sDictAppUsageInfo)
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value.ToDictionary((KeyValuePair<string, long> _) => _.Key, (KeyValuePair<string, long> _) => _.Value));
				}
				long num = (long)AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
				if (dictionary.ContainsKey(AppUsageTimer.sLastVMName))
				{
					Dictionary<string, long> dictionary2;
					if (dictionary[AppUsageTimer.sLastVMName].ContainsKey(AppUsageTimer.sLastAppPackage))
					{
						dictionary2 = dictionary[AppUsageTimer.sLastVMName];
						string text = AppUsageTimer.sLastAppPackage;
						dictionary2[text] += num;
					}
					else
					{
						dictionary[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, num);
					}
					dictionary2 = dictionary[AppUsageTimer.sLastVMName];
					dictionary2["TotalUsage"] = dictionary2["TotalUsage"] + num;
				}
				else
				{
					dictionary.Add(AppUsageTimer.sLastVMName, new Dictionary<string, long> { { "TotalUsage", num } });
					dictionary[AppUsageTimer.sLastVMName].Add(AppUsageTimer.sLastAppPackage, num);
				}
				return dictionary;
			}
			return AppUsageTimer.sDictAppUsageInfo;
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x000273C8 File Offset: 0x000255C8
		internal static long GetTotalTimeForPackageAcrossInstances(string packageName)
		{
			long num = 0L;
			try
			{
				Func<KeyValuePair<string, long>, bool> <>9__0;
				foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in AppUsageTimer.sDictAppUsageInfo)
				{
					IEnumerable<KeyValuePair<string, long>> value = keyValuePair.Value;
					Func<KeyValuePair<string, long>, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (KeyValuePair<string, long> _) => string.Equals(_.Key, packageName, StringComparison.OrdinalIgnoreCase));
					}
					IEnumerable<KeyValuePair<string, long>> enumerable = value.Where(func);
					if (enumerable.Any<KeyValuePair<string, long>>())
					{
						num += enumerable.First<KeyValuePair<string, long>>().Value;
					}
				}
				if (!string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage) && string.Compare(AppUsageTimer.sLastAppPackage, packageName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					num += (long)AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
				}
				Logger.Debug("Total time for package " + packageName + " " + num.ToString());
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetTotalTimeForPackageAcrossInstances. Err : " + ex.ToString());
			}
			return num;
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x000274F0 File Offset: 0x000256F0
		internal static long GetTotalTimeForAllPackages()
		{
			long num = 0L;
			try
			{
				foreach (KeyValuePair<string, Dictionary<string, long>> keyValuePair in AppUsageTimer.sDictAppUsageInfo)
				{
					long num2 = 0L;
					IEnumerable<KeyValuePair<string, long>> enumerable = keyValuePair.Value.Where((KeyValuePair<string, long> _) => string.Compare(_.Key, "Home", StringComparison.OrdinalIgnoreCase) == 0);
					if (enumerable.Any<KeyValuePair<string, long>>())
					{
						num2 += enumerable.First<KeyValuePair<string, long>>().Value;
					}
					enumerable = keyValuePair.Value.Where((KeyValuePair<string, long> _) => string.Compare(_.Key, "TotalUsage", StringComparison.OrdinalIgnoreCase) == 0);
					if (enumerable.Any<KeyValuePair<string, long>>())
					{
						num += enumerable.First<KeyValuePair<string, long>>().Value;
						num -= num2;
					}
				}
				if (!string.IsNullOrEmpty(AppUsageTimer.sLastAppPackage) && !string.Equals(AppUsageTimer.sLastAppPackage, "Home", StringComparison.InvariantCulture))
				{
					num += (long)AppUsageTimer.sStopwatch.Elapsed.TotalSeconds;
				}
				Logger.Debug("Total time for all packages " + num.ToString());
				if (num < 0L)
				{
					return 0L;
				}
				return num;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetTotalTimeForAllPackages " + ex.ToString());
			}
			return 0L;
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0002767C File Offset: 0x0002587C
		internal static long GetTotalTimeForPackageAfterReset(string packageName)
		{
			try
			{
				long totalTimeForPackageAcrossInstances = AppUsageTimer.GetTotalTimeForPackageAcrossInstances(packageName);
				if (totalTimeForPackageAcrossInstances < 0L)
				{
					return 0L;
				}
				return totalTimeForPackageAcrossInstances;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in GetTotalTimeForPackageAfterReset. Err : " + ex.ToString());
			}
			return 0L;
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x00006922 File Offset: 0x00004B22
		internal static void AddPackageForReset(string package, long time)
		{
			AppUsageTimer.sResetQuestDict[package] = time;
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x00006930 File Offset: 0x00004B30
		internal static void SessionEventHandler()
		{
			SystemEvents.SessionSwitch += AppUsageTimer.sessionSwitchHandler;
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0000693C File Offset: 0x00004B3C
		private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason == SessionSwitchReason.SessionLock)
			{
				AppUsageTimer.StopTimer();
				return;
			}
			if (e.Reason == SessionSwitchReason.SessionUnlock)
			{
				AppUsageTimer.StartTimerAfterResume();
			}
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x0000695B File Offset: 0x00004B5B
		internal static void DetachSessionEventHandler()
		{
			SystemEvents.SessionSwitch -= AppUsageTimer.sessionSwitchHandler;
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x000276CC File Offset: 0x000258CC
		private static void StartTimerAfterResume()
		{
			try
			{
				if (BlueStacksUIUtils.DictWindows.ContainsKey(AppUsageTimer.sLastVMName))
				{
					MainWindow mainWindow = BlueStacksUIUtils.DictWindows[AppUsageTimer.sLastVMName];
					if (mainWindow != null && mainWindow.mTopBar.mAppTabButtons.SelectedTab != null)
					{
						AppUsageTimer.StartTimer(AppUsageTimer.sLastVMName, mainWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in starting timer after sleep. Err : " + ex.ToString());
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00006967 File Offset: 0x00004B67
		internal static void SaveData()
		{
			AppUsageTimer.StopTimer();
			RegistryManager.Instance.AInfo = AppUsageTimer.EncryptString(JsonConvert.SerializeObject(AppUsageTimer.sDictAppUsageInfo));
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00027758 File Offset: 0x00025958
		internal static string EncryptString(string encryptString)
		{
			string userGuid = RegistryManager.Instance.UserGuid;
			byte[] array = Encoding.Unicode.GetBytes(encryptString);
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(userGuid, AppUsageTimer.bytes);
			string text;
			using (Aes aes = Aes.Create())
			{
				aes.Key = rfc2898DeriveBytes.GetBytes(32);
				aes.IV = rfc2898DeriveBytes.GetBytes(16);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cryptoStream.Write(array, 0, array.Length);
						cryptoStream.Close();
						encryptString = Convert.ToBase64String(memoryStream.ToArray());
						text = encryptString;
					}
				}
			}
			return text;
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x0002782C File Offset: 0x00025A2C
		public static string DecryptString(string decryptString)
		{
			string userGuid = RegistryManager.Instance.UserGuid;
			decryptString = ((decryptString != null) ? decryptString.Replace(" ", "+") : null);
			byte[] array = Convert.FromBase64String(decryptString);
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(userGuid, AppUsageTimer.bytes);
			string text;
			using (Aes aes = Aes.Create())
			{
				aes.Key = rfc2898DeriveBytes.GetBytes(32);
				aes.IV = rfc2898DeriveBytes.GetBytes(16);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cryptoStream.Write(array, 0, array.Length);
						cryptoStream.Close();
						decryptString = Encoding.Unicode.GetString(memoryStream.ToArray());
						text = decryptString;
					}
				}
			}
			return text;
		}

		// Token: 0x040003B1 RID: 945
		internal static Dictionary<string, Dictionary<string, long>> sDictAppUsageInfo = new Dictionary<string, Dictionary<string, long>>();

		// Token: 0x040003B2 RID: 946
		private static Dictionary<string, long> sResetQuestDict = new Dictionary<string, long>();

		// Token: 0x040003B3 RID: 947
		private static Stopwatch sStopwatch = new Stopwatch();

		// Token: 0x040003B4 RID: 948
		private static string sLastAppPackage = null;

		// Token: 0x040003B5 RID: 949
		private static string sLastVMName = null;

		// Token: 0x040003B6 RID: 950
		private static SessionSwitchEventHandler sessionSwitchHandler = new SessionSwitchEventHandler(AppUsageTimer.SystemEvents_SessionSwitch);

		// Token: 0x040003B7 RID: 951
		private static readonly byte[] bytes = new byte[]
		{
			73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
			100, 101, 118
		};
	}
}
