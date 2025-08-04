using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.BlueStacksUI.Custom;
using BlueStacks.Common;
using Microsoft.Win32;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020002D2 RID: 722
	public class FpsBind
	{
		// Token: 0x06001ABB RID: 6843
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		// Token: 0x06001ABC RID: 6844
		[DllImport("user32.dll")]
		private static extern IntPtr SetFocus(IntPtr hWnd);

		// Token: 0x06001ABD RID: 6845
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern short GetAsyncKeyState(Keys vKey);

		// Token: 0x06001ABE RID: 6846 RVA: 0x0009DA7C File Offset: 0x0009BC7C
		public static void SetSynchronizationContextAsync()
		{
			try
			{
				Logger.Info("Starting FpsBind key monitoring");
				for (;;)
				{
					if (FpsBind.IsKeyCombinationPressed(FpsBind.bindKeyF2, FpsBind.modifierF2) && !FpsBind.isKeyPressed)
					{
						FpsBind.isKeyPressed = true;
						Logger.Info(string.Format("F2 key ({0}) pressed, setting FPS to {1}", FpsBind.GetKeyDisplayText(FpsBind.modifierF2, FpsBind.bindKeyF2), FpsBind.isF2LowFps ? FpsBind.fpsValue2 : FpsBind.fpsValue1));
						int num = (FpsBind.isF2LowFps ? FpsBind.fpsValue2 : FpsBind.fpsValue1);
						FpsBind.SetFpsValue(num, string.Format("FPS: {0}", num));
						FpsBind.isF2LowFps = !FpsBind.isF2LowFps;
						Thread.Sleep(500);
						FpsBind.isAlertShown = false;
					}
					else if (FpsBind.IsKeyCombinationPressed(FpsBind.bindKeyF3, FpsBind.modifierF3) && !FpsBind.isKeyPressed)
					{
						FpsBind.isKeyPressed = true;
						Logger.Info("F3 key (" + FpsBind.GetKeyDisplayText(FpsBind.modifierF3, FpsBind.bindKeyF3) + ") pressed, starting FPS unlock sequence");
						FpsBind.SetFpsValueWithDelay();
						Thread.Sleep(500);
						FpsBind.isAlertShown = false;
					}
					else if (!FpsBind.IsKeyCombinationPressed(FpsBind.bindKeyF2, FpsBind.modifierF2) && !FpsBind.IsKeyCombinationPressed(FpsBind.bindKeyF3, FpsBind.modifierF3))
					{
						FpsBind.isKeyPressed = false;
					}
					Thread.Sleep(50);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in FpsBind: " + ex.ToString());
				File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR: {1}\n{2}\n\n", DateTime.Now, ex.Message, ex.StackTrace));
			}
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x0009DC24 File Offset: 0x0009BE24
		private static void SetFpsValueWithDelay()
		{
			try
			{
				FpsBind.SetFpsValue(1, "Unlocking FPS...");
				Logger.Info("Waiting 3 seconds before setting FPS to 999");
				Thread.Sleep(3000);
				FpsBind.SetFpsValue(999, "FPS Unlocked");
			}
			catch (Exception ex)
			{
				Logger.Error("Error in SetFpsValueWithDelay: " + ex.Message);
				File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR in SetFpsValueWithDelay: {1}\n{2}\n\n", DateTime.Now, ex.Message, ex.StackTrace));
			}
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x0009DCB4 File Offset: 0x0009BEB4
		private static void SetFpsValue(int fps, string notificationMessage)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BlueStacks_bgp64\\Guests"))
				{
					if (registryKey != null)
					{
						ThreadStart <>9__0;
						foreach (string text in registryKey.GetSubKeyNames())
						{
							if (text.StartsWith("Android", StringComparison.OrdinalIgnoreCase))
							{
								string text2 = "SOFTWARE\\BlueStacks_bgp64\\Guests\\" + text + "\\Config";
								using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(text2, true))
								{
									if (registryKey2 != null)
									{
										registryKey2.SetValue("FPS", fps, RegistryValueKind.DWord);
									}
								}
								string text3 = "SOFTWARE\\BlueStacks_bgp64\\Guests\\" + text;
								using (RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey(text3, true))
								{
									if (registryKey3 != null)
									{
										string text4 = registryKey3.GetValue("BootParameters") as string;
										if (!string.IsNullOrEmpty(text4))
										{
											string text5 = Regex.Replace(text4, "fps=\\d+", string.Format("fps={0}", fps));
											registryKey3.SetValue("BootParameters", text5, RegistryValueKind.String);
										}
									}
								}
								Utils.SendChangeFPSToInstanceASync(text, fps);
								if (!FpsBind.isAlertShown)
								{
									Logger.Info("Attempting to show FPS notification with message: {0}", new object[] { notificationMessage });
									ThreadStart threadStart;
									if ((threadStart = <>9__0) == null)
									{
										threadStart = (<>9__0 = delegate
										{
											try
											{
												FpsNotificationForm fpsNotificationForm = new FpsNotificationForm(notificationMessage);
												fpsNotificationForm.FormClosed += delegate(object s, FormClosedEventArgs e)
												{
													FpsBind.isAlertShown = false;
													Logger.Info("FPS notification closed");
												};
												Logger.Info("FPS notification created, starting Application.Run");
												Application.Run(fpsNotificationForm);
												Logger.Info("FPS notification shown and closed");
												Thread.Sleep(200);
												MainWindow mainWindow = BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName];
												if (mainWindow != null)
												{
													FpsBind.SetForegroundWindow(mainWindow.Handle);
													FpsBind.SetFocus(mainWindow.Handle);
													Logger.Info("Focus returned to main window");
												}
												else
												{
													Logger.Warning("Main window not found for focus return");
												}
											}
											catch (Exception ex2)
											{
												Logger.Error("Error showing FPS notification: " + ex2.ToString());
												File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR showing notification: {1}\n{2}\n\n", DateTime.Now, ex2.Message, ex2.StackTrace));
											}
										});
									}
									Thread thread = new Thread(threadStart);
									thread.SetApartmentState(ApartmentState.STA);
									thread.IsBackground = true;
									thread.Start();
									FpsBind.isAlertShown = true;
								}
								else
								{
									Logger.Warning("Notification skipped because isAlertShown is true");
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error setting FPS: " + ex.Message);
				File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR: {1}\n{2}\n\n", DateTime.Now, ex.Message, ex.StackTrace));
			}
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x0009DF00 File Offset: 0x0009C100
		private static string GetKeyDisplayText(Keys modifier, Keys key)
		{
			string text = "";
			if ((modifier & Keys.Control) != Keys.None)
			{
				text += "Ctrl+";
			}
			if ((modifier & Keys.Shift) != Keys.None)
			{
				text += "Shift+";
			}
			if ((modifier & Keys.Alt) != Keys.None)
			{
				text += "Alt+";
			}
			return text + key.ToString();
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x0009DF68 File Offset: 0x0009C168
		private static bool IsKeyCombinationPressed(Keys key, Keys modifier)
		{
			bool flag = ((int)FpsBind.GetAsyncKeyState(key) & 32768) != 0;
			bool flag2 = (modifier & Keys.Control) == Keys.None || (Control.ModifierKeys & Keys.Control) > Keys.None;
			bool flag3 = (modifier & Keys.Shift) == Keys.None || (Control.ModifierKeys & Keys.Shift) > Keys.None;
			bool flag4 = (modifier & Keys.Alt) == Keys.None || (Control.ModifierKeys & Keys.Alt) > Keys.None;
			return flag && flag2 && flag3 && flag4;
		}

		// Token: 0x06001AC5 RID: 6853 RVA: 0x0009DFDC File Offset: 0x0009C1DC
		public static void OnBlueStacksClosing()
		{
			try
			{
				Logger.Info("BlueStacks is closing, locking FPS to 30 and showing notification");
				FpsBind.isAlertShown = false;
				Thread thread = new Thread(delegate
				{
					try
					{
						FpsBind.SetFpsValue(30, "Domination is closed");
						Logger.Info("Notification thread for OnBlueStacksClosing completed");
					}
					catch (Exception ex2)
					{
						Logger.Error("Error in OnBlueStacksClosing notification thread: " + ex2.ToString());
						File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR in OnBlueStacksClosing notification: {1}\n{2}\n\n", DateTime.Now, ex2.Message, ex2.StackTrace));
					}
				});
				thread.SetApartmentState(ApartmentState.STA);
				thread.IsBackground = true;
				thread.Start();
				thread.Join(2000);
			}
			catch (Exception ex)
			{
				Logger.Error("Error in OnBlueStacksClosing: " + ex.Message);
				File.AppendAllText("fps_manager.log", string.Format("[{0}] ERROR in OnBlueStacksClosing: {1}\n{2}\n\n", DateTime.Now, ex.Message, ex.StackTrace));
			}
		}

		// Token: 0x040010BA RID: 4282
		private static bool isKeyPressed;

		// Token: 0x040010BB RID: 4283
		private static bool isAlertShown;

		// Token: 0x040010BC RID: 4284
		private static bool isF2LowFps;

		// Token: 0x040010BD RID: 4285
		private static Keys bindKeyF2 = Keys.F2;

		// Token: 0x040010BE RID: 4286
		private static Keys bindKeyF3 = Keys.F3;

		// Token: 0x040010BF RID: 4287
		private static int fpsValue1 = 30;

		// Token: 0x040010C0 RID: 4288
		private static int fpsValue2 = 999;

		// Token: 0x040010C1 RID: 4289
		private static Keys modifierF2 = Keys.None;

		// Token: 0x040010C2 RID: 4290
		private static Keys modifierF3 = Keys.None;
	}
}
