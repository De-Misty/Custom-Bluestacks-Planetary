using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using BlueStacks.Common;
using Microsoft.Win32;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001CD RID: 461
	public static class MiscUtils
	{
		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06001276 RID: 4726 RVA: 0x0000D409 File Offset: 0x0000B609
		private static SerialWorkQueue FocusWorker
		{
			get
			{
				if (MiscUtils.sFocusWorker == null)
				{
					MiscUtils.sFocusWorker = new SerialWorkQueue();
					MiscUtils.sFocusWorker.Start();
				}
				return MiscUtils.sFocusWorker;
			}
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x000715A8 File Offset: 0x0006F7A8
		public static void SetFocusAsync(UIElement control, int delay = 0)
		{
			MiscUtils.FocusWorker.Enqueue(delegate
			{
				try
				{
					int i = 0;
					if (delay > 0)
					{
						Thread.Sleep(delay);
					}
					while (10 > i)
					{
						control.Dispatcher.Invoke(new Action(delegate
						{
							if (control.Focus())
							{
								i = 11;
							}
						}), new object[0]);
						int j = i;
						i = j + 1;
						Thread.Sleep(10);
					}
				}
				catch (Exception ex)
				{
					Logger.Info("Error setting focus on control" + ex.ToString());
				}
			});
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x000715E0 File Offset: 0x0006F7E0
		public static void GetWindowWidthAndHeight(out int width, out int height)
		{
			int width2 = Screen.PrimaryScreen.Bounds.Width;
			int height2 = Screen.PrimaryScreen.Bounds.Height;
			if (width2 > 2560 && height2 > 1440)
			{
				width = 2560;
				height = 1440;
				return;
			}
			if (width2 > 1920 && height2 > 1080)
			{
				width = 1920;
				height = 1080;
				return;
			}
			if (width2 > 1600 && height2 > 900)
			{
				width = 1600;
				height = 900;
				return;
			}
			if (width2 > 1280 && height2 > 720)
			{
				width = 1280;
				height = 720;
				return;
			}
			width = 960;
			height = 540;
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x000716A0 File Offset: 0x0006F8A0
		private static bool IsParametersValid(Window window)
		{
			try
			{
				if (window.Left < 0.0 || window.Left > SystemParameters.VirtualScreenWidth || window.Top < 0.0 || window.Top > SystemParameters.VirtualScreenHeight)
				{
					return false;
				}
				if (SystemParameters.VirtualScreenWidth - window.Left < window.Width / 10.0 || SystemParameters.VirtualScreenHeight - window.Top < window.Height / 10.0)
				{
					return false;
				}
				double screenWidth = (double)RegistryManager.Instance.ScreenWidth;
				int screenHeight = RegistryManager.Instance.ScreenHeight;
				if (Math.Abs(screenWidth - SystemParameters.VirtualScreenWidth) > 100.0 || Math.Abs((double)screenHeight - SystemParameters.VirtualScreenHeight) > 100.0)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Exception calculating size" + ex.ToString());
				return false;
			}
			return true;
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x000717A8 File Offset: 0x0006F9A8
		private static void SaveControlSize(double width, double height, string prefix)
		{
			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
			registryKey.SetValue(prefix + "Width", width, RegistryValueKind.DWord);
			registryKey.SetValue(prefix + "Height", height, RegistryValueKind.DWord);
			registryKey.Close();
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x00071800 File Offset: 0x0006FA00
		public static void SetWindowSizeAndLocation(Window window, string prefix, bool isGMWindow = false)
		{
			if (window != null)
			{
				try
				{
					double num = 1.7777777777777777;
					bool flag = true;
					RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
					if ((int)registryKey.GetValue(prefix + "Width", -2147483648) != -2147483648)
					{
						try
						{
							window.Width = (double)((int)registryKey.GetValue(prefix + "Width"));
							window.Height = (double)((int)registryKey.GetValue(prefix + "Height"));
							RegistryKey registryKey2 = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.ClientBaseKeyPath);
							window.Left = (double)((int)registryKey2.GetValue(prefix + "Left"));
							window.Top = (double)((int)registryKey2.GetValue(prefix + "Top"));
							flag = false;
							if (!MiscUtils.IsParametersValid(window))
							{
								flag = true;
							}
						}
						catch (Exception ex)
						{
							Logger.Info("Exception in geting value from reg" + ex.ToString());
							flag = true;
						}
					}
					if (flag)
					{
						double num2;
						double num3;
						double num4;
						WpfUtils.GetDefaultSize(out num2, out num3, out num4, num, isGMWindow);
						double num5 = num4 + num2;
						double num6 = (double)((int)(SystemParameters.PrimaryScreenHeight - num3) / 2);
						if (isGMWindow)
						{
							window.Left = num4;
							window.Top = num6;
							window.Height = num3;
							window.Width = num2;
							MiscUtils.SaveControlSize(num2, num3, "DefaultGM");
						}
						else
						{
							window.Left = num5;
							window.Top = num6;
							window.Height = num3;
							window.Width = (window.Height - 33.0) / 27.0 * 16.0;
							if (window.Left + window.Width > SystemParameters.PrimaryScreenWidth)
							{
								window.Left = SystemParameters.PrimaryScreenWidth - window.Width - 20.0;
							}
						}
					}
				}
				catch (Exception ex2)
				{
					Logger.Info("Exception getting size" + ex2.ToString());
				}
			}
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x00071A30 File Offset: 0x0006FC30
		public static int Extract7Zip(string zipFilePath, string extractDirectory)
		{
			string text = Path.Combine(RegistryStrings.InstallDir, "7zr.exe");
			if (!Directory.Exists(extractDirectory))
			{
				Directory.CreateDirectory(extractDirectory);
			}
			string text2 = string.Format(CultureInfo.InvariantCulture, "x \"{0}\" -o\"{1}\" -aoa", new object[] { zipFilePath, extractDirectory });
			return RunCommand.RunCmd(text, text2, false, true, false, 0).ExitCode;
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x0000D42B File Offset: 0x0000B62B
		public static void GetStreamWidthAndHeight(int sWidth, int sHeight, out int width, out int height)
		{
			height = Utils.GetInt(RegistryManager.Instance.FrontendHeight, sHeight);
			width = Utils.GetInt(RegistryManager.Instance.FrontendWidth, sWidth);
		}

		// Token: 0x04000BEF RID: 3055
		private const int TextBoxFoxusAttemts = 10;

		// Token: 0x04000BF0 RID: 3056
		private static SerialWorkQueue sFocusWorker;
	}
}
