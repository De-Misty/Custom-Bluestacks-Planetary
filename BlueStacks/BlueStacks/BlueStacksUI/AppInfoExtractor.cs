using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001DE RID: 478
	internal class AppInfoExtractor
	{
		// Token: 0x060012D6 RID: 4822 RVA: 0x00072B30 File Offset: 0x00070D30
		internal static AppInfoExtractor GetApkInfo(string apkFile)
		{
			AppInfoExtractor appInfoExtractor = new AppInfoExtractor();
			try
			{
				string text = string.Empty;
				using (Process process = new Process())
				{
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
					process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "hd-aapt.exe");
					process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "dump badging \"{0}\"", new object[] { apkFile });
					process.Start();
					text = process.StandardOutput.ReadToEnd();
					process.WaitForExit();
				}
				Match match = new Regex("package:\\sname='(.+?)'").Match(text);
				appInfoExtractor.PackageName = match.Groups[1].Value;
				if (!string.IsNullOrEmpty(appInfoExtractor.PackageName))
				{
					match = new Regex("application:\\slabel='(.+)'\\sicon='(.+?)'").Match(text);
					appInfoExtractor.AppName = match.Groups[1].Value;
					appInfoExtractor.AppName = Regex.Replace(appInfoExtractor.AppName, "[\\x22\\\\\\/:*?|<>]", "");
					match.Groups[2].Value.Replace("/", "\\");
					match = new Regex("launchable\\sactivity\\sname='(.+?)'").Match(text);
					appInfoExtractor.ActivityName = match.Groups[1].Value;
				}
			}
			catch
			{
				Logger.Error("Error getting file info");
			}
			return appInfoExtractor;
		}

		// Token: 0x04000C28 RID: 3112
		internal string PackageName;

		// Token: 0x04000C29 RID: 3113
		internal string AppName;

		// Token: 0x04000C2A RID: 3114
		internal string ActivityName;
	}
}
