using System;
using System.Runtime.InteropServices;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000093 RID: 147
	internal static class NativeMethods
	{
		// Token: 0x06000656 RID: 1622
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		// Token: 0x06000657 RID: 1623
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		// Token: 0x06000658 RID: 1624
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int idHook, GlobalKeyBoardMouseHooks.LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		// Token: 0x06000659 RID: 1625
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

		// Token: 0x0600065A RID: 1626
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		// Token: 0x0600065B RID: 1627
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x0600065C RID: 1628
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int idHook, GlobalKeyBoardMouseHooks.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		// Token: 0x0600065D RID: 1629
		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

		// Token: 0x0600065E RID: 1630
		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool RegisterRawInputDevices(RawInputClass.RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

		// Token: 0x0600065F RID: 1631
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref NativeMethods.Win32Point pt);

		// Token: 0x06000660 RID: 1632
		[DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

		// Token: 0x06000661 RID: 1633
		[DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

		// Token: 0x06000662 RID: 1634
		[DllImport("urlmon.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern uint FindMimeFromData(uint pBC, [MarshalAs(UnmanagedType.LPStr)] string pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer, uint cbSize, [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed, uint dwMimeFlags, out uint ppwzMimeOut, uint dwReserverd);

		// Token: 0x06000663 RID: 1635
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x06000664 RID: 1636
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

		// Token: 0x06000665 RID: 1637
		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool GetMonitorInfo(IntPtr hmonitor, [In] [Out] WindowWndProcHandler.MONITORINFOEX info);

		// Token: 0x06000666 RID: 1638
		[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SHAppBarMessage(int msg, ref WindowWndProcHandler.APPBARDATA data);

		// Token: 0x02000094 RID: 148
		internal struct Win32Point
		{
			// Token: 0x04000354 RID: 852
			public int X;

			// Token: 0x04000355 RID: 853
			public int Y;
		}
	}
}
