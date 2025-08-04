using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000273 RID: 627
	internal class GlobalKeyBoardMouseHooks
	{
		// Token: 0x06001705 RID: 5893 RVA: 0x00089820 File Offset: 0x00087A20
		private static IntPtr SetHook(GlobalKeyBoardMouseHooks.LowLevelMouseProc proc)
		{
			IntPtr intPtr;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				using (ProcessModule mainModule = currentProcess.MainModule)
				{
					intPtr = NativeMethods.SetWindowsHookEx(14, proc, NativeMethods.GetModuleHandle(mainModule.ModuleName), 0U);
				}
			}
			return intPtr;
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x00089884 File Offset: 0x00087A84
		private static IntPtr SetHook(GlobalKeyBoardMouseHooks.LowLevelKeyboardProc proc)
		{
			IntPtr intPtr;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				using (ProcessModule mainModule = currentProcess.MainModule)
				{
					intPtr = NativeMethods.SetWindowsHookEx(13, proc, NativeMethods.GetModuleHandle(mainModule.ModuleName), 0U);
				}
			}
			return intPtr;
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x000898E8 File Offset: 0x00087AE8
		internal static void SetMouseMoveHook()
		{
			try
			{
				if (GlobalKeyBoardMouseHooks.mMouseHookId == IntPtr.Zero)
				{
					GlobalKeyBoardMouseHooks.mMouseHookId = GlobalKeyBoardMouseHooks.SetHook(GlobalKeyBoardMouseHooks.mMouseProc);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception setting global mouse hook" + ex.ToString());
			}
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x00089940 File Offset: 0x00087B40
		internal static void SetBossKeyHook()
		{
			try
			{
				GlobalKeyBoardMouseHooks.SetKey(RegistryManager.Instance.BossKey);
				if (GlobalKeyBoardMouseHooks.mKeyboardHookID == IntPtr.Zero)
				{
					GlobalKeyBoardMouseHooks.mKeyboardHookID = GlobalKeyBoardMouseHooks.SetHook(GlobalKeyBoardMouseHooks.mKeyboardProc);
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception setting global hook" + ex.ToString());
			}
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x000899A8 File Offset: 0x00087BA8
		internal static void SetKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				Logger.Warning("Cannot set an empty key");
				return;
			}
			string[] array = key.Split(new char[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string bossKey = array[array.Length - 1];
			GlobalKeyBoardMouseHooks.sKey = IMAPKeys.mDictKeys.First((KeyValuePair<Key, string> x) => x.Value == bossKey).Key.ToString();
			GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey = (GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey = (GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey = false));
			foreach (string text in array)
			{
				if (string.Equals(text, "Ctrl", StringComparison.InvariantCulture))
				{
					GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey = true;
				}
				else if (string.Equals(text, "Alt", StringComparison.InvariantCulture))
				{
					GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey = true;
				}
				else if (string.Equals(text, "Shift", StringComparison.InvariantCulture))
				{
					GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey = true;
				}
			}
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x0000F839 File Offset: 0x0000DA39
		internal static void UnsetKey()
		{
			GlobalKeyBoardMouseHooks.sKey = string.Empty;
			GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey = false;
			GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey = false;
			GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey = false;
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x0000F857 File Offset: 0x0000DA57
		internal static void UnHookGlobalHooks()
		{
			NativeMethods.UnhookWindowsHookEx(GlobalKeyBoardMouseHooks.mKeyboardHookID);
			GlobalKeyBoardMouseHooks.mKeyboardHookID = IntPtr.Zero;
			GlobalKeyBoardMouseHooks.UnhookGlobalMouseHooks();
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0000F873 File Offset: 0x0000DA73
		internal static void UnhookGlobalMouseHooks()
		{
			if (GlobalKeyBoardMouseHooks.mMouseHookId != IntPtr.Zero)
			{
				NativeMethods.UnhookWindowsHookEx(GlobalKeyBoardMouseHooks.mMouseHookId);
				GlobalKeyBoardMouseHooks.mMouseHookId = IntPtr.Zero;
			}
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x00089A98 File Offset: 0x00087C98
		private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			try
			{
				if (GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging)
				{
					Logger.Info(string.Concat(new string[]
					{
						"Keyboard hook ..",
						nCode.ToString(),
						"..",
						wParam.ToString(),
						"..",
						lParam.ToString()
					}));
				}
				MainWindow window = BlueStacksUIUtils.ActivatedWindow;
				if (nCode >= 0 && (wParam == (IntPtr)256 || wParam == (IntPtr)260 || wParam == (IntPtr)257))
				{
					int num = Marshal.ReadInt32(lParam);
					Logger.Debug("Keyboard hook .." + num.ToString() + ".." + GlobalKeyBoardMouseHooks.sKey);
					if (wParam == (IntPtr)256 || wParam == (IntPtr)260)
					{
						if (!string.IsNullOrEmpty(GlobalKeyBoardMouseHooks.sKey) && num == (int)((Keys)Enum.Parse(typeof(Keys), GlobalKeyBoardMouseHooks.sKey, false)) && GlobalKeyBoardMouseHooks.sIsControlUsedInBossKey == (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && GlobalKeyBoardMouseHooks.sIsAltUsedInBossKey == (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) && GlobalKeyBoardMouseHooks.sIsShiftUsedInBossKey == (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
						{
							ThreadPool.QueueUserWorkItem(delegate(object obj)
							{
								if (BlueStacksUIUtils.DictWindows.Values.Count > 0)
								{
									MainWindow mainWindow = BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0];
									mainWindow.Dispatcher.Invoke(new Action(delegate
									{
										try
										{
											if (!mainWindow.OwnedWindows.OfType<OnBoardingPopupWindow>().Any<OnBoardingPopupWindow>() && !mainWindow.OwnedWindows.OfType<GameOnboardingControl>().Any<GameOnboardingControl>())
											{
												GlobalKeyBoardMouseHooks.mIsHidden = !GlobalKeyBoardMouseHooks.mIsHidden;
												BlueStacksUIUtils.HideUnhideBlueStacks(GlobalKeyBoardMouseHooks.mIsHidden);
											}
										}
										catch
										{
										}
									}), new object[0]);
								}
							});
							return (IntPtr)1;
						}
						if (window != null && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
						{
							if (num >= 96 && num <= 105)
							{
								num -= 48;
							}
							Key key = KeyInterop.KeyFromVirtualKey(num);
							string vkString = IMAPKeys.GetStringForFile(key);
							if (MainWindow.sMacroMapping.Keys.Contains(vkString))
							{
								Action <>9__3;
								ThreadPool.QueueUserWorkItem(delegate(object obj)
								{
									try
									{
										Dispatcher dispatcher = window.Dispatcher;
										Action action;
										if ((action = <>9__3) == null)
										{
											action = (<>9__3 = delegate
											{
												if (window.mSidebar.GetElementFromTag("sidebar_macro") != null && window.mSidebar.GetElementFromTag("sidebar_macro").Visibility == Visibility.Visible && window.mSidebar.GetElementFromTag("sidebar_macro").IsEnabled)
												{
													if (window.mIsMacroRecorderActive)
													{
														window.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""), "", "", false);
														return;
													}
													if (window.mIsMacroPlaying)
													{
														CustomMessageWindow customMessageWindow = new CustomMessageWindow();
														BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CANNOT_RUN_MACRO", "");
														BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_STOP_MACRO_SCRIPT", "");
														customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
														customMessageWindow.Owner = window;
														customMessageWindow.ShowDialog();
														return;
													}
													try
													{
														string text = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, MainWindow.sMacroMapping[vkString] + ".json");
														if (File.Exists(text))
														{
															MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(text), Utils.GetSerializerSettings());
															macroRecording.Name = MainWindow.sMacroMapping[vkString];
															window.mCommonHandler.FullMacroScriptPlayHandler(macroRecording);
															ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "shortcut_keys", macroRecording.RecordingType.ToString(), string.IsNullOrEmpty(macroRecording.MacroId) ? "local" : "community", null, null);
														}
														return;
													}
													catch (Exception ex)
													{
														Logger.Error("Exception in macro play with shortcut: " + ex.ToString());
														return;
													}
												}
												Logger.Info("Macro not enabled for the current package: " + window.StaticComponents.mSelectedTabButton.PackageName);
											});
										}
										dispatcher.Invoke(action, new object[0]);
									}
									catch
									{
									}
								});
							}
						}
					}
				}
			}
			catch
			{
			}
			return NativeMethods.CallNextHookEx(GlobalKeyBoardMouseHooks.mKeyboardHookID, nCode, wParam, lParam);
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x0000F89B File Offset: 0x0000DA9B
		private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			return NativeMethods.CallNextHookEx(GlobalKeyBoardMouseHooks.mMouseHookId, nCode, wParam, lParam);
		}

		// Token: 0x04000E2C RID: 3628
		private static bool mIsHidden = false;

		// Token: 0x04000E2D RID: 3629
		private const int WH_KEYBOARD_LL = 13;

		// Token: 0x04000E2E RID: 3630
		private const int WM_KEYDOWN = 256;

		// Token: 0x04000E2F RID: 3631
		private const int WM_KEYUP = 257;

		// Token: 0x04000E30 RID: 3632
		private const int WM_SYSKEYDOWN = 260;

		// Token: 0x04000E31 RID: 3633
		private static readonly GlobalKeyBoardMouseHooks.LowLevelKeyboardProc mKeyboardProc = new GlobalKeyBoardMouseHooks.LowLevelKeyboardProc(GlobalKeyBoardMouseHooks.KeyboardHookCallback);

		// Token: 0x04000E32 RID: 3634
		private static IntPtr mKeyboardHookID = IntPtr.Zero;

		// Token: 0x04000E33 RID: 3635
		private static string sKey = null;

		// Token: 0x04000E34 RID: 3636
		private static bool sIsControlUsedInBossKey = false;

		// Token: 0x04000E35 RID: 3637
		private static bool sIsAltUsedInBossKey = false;

		// Token: 0x04000E36 RID: 3638
		private static bool sIsShiftUsedInBossKey = false;

		// Token: 0x04000E37 RID: 3639
		internal static bool sIsEnableKeyboardHookLogging = false;

		// Token: 0x04000E38 RID: 3640
		private static GlobalKeyBoardMouseHooks.LowLevelMouseProc mMouseProc = new GlobalKeyBoardMouseHooks.LowLevelMouseProc(GlobalKeyBoardMouseHooks.MouseHookCallback);

		// Token: 0x04000E39 RID: 3641
		private static IntPtr mMouseHookId = IntPtr.Zero;

		// Token: 0x04000E3A RID: 3642
		private const int WH_MOUSE_LL = 14;

		// Token: 0x02000274 RID: 628
		// (Invoke) Token: 0x06001712 RID: 5906
		internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

		// Token: 0x02000275 RID: 629
		// (Invoke) Token: 0x06001716 RID: 5910
		internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		// Token: 0x02000276 RID: 630
		private enum MouseMessages
		{
			// Token: 0x04000E3C RID: 3644
			WM_LBUTTONDOWN = 513,
			// Token: 0x04000E3D RID: 3645
			WM_LBUTTONUP,
			// Token: 0x04000E3E RID: 3646
			WM_MOUSEMOVE = 512,
			// Token: 0x04000E3F RID: 3647
			WM_MOUSEWHEEL = 522,
			// Token: 0x04000E40 RID: 3648
			WM_RBUTTONDOWN = 516,
			// Token: 0x04000E41 RID: 3649
			WM_RBUTTONUP
		}

		// Token: 0x02000277 RID: 631
		private struct MSLLHOOKSTRUCT
		{
			// Token: 0x04000E42 RID: 3650
			public POINT pt;

			// Token: 0x04000E43 RID: 3651
			public uint mouseData;

			// Token: 0x04000E44 RID: 3652
			public uint flags;

			// Token: 0x04000E45 RID: 3653
			public uint time;

			// Token: 0x04000E46 RID: 3654
			public IntPtr dwExtraInfo;
		}
	}
}
