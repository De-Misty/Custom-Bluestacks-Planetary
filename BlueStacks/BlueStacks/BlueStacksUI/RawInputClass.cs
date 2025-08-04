using System;
using System.Runtime.InteropServices;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200016A RID: 362
	internal class RawInputClass
	{
		// Token: 0x06000ED7 RID: 3799 RVA: 0x0005DF78 File Offset: 0x0005C178
		internal static int GetDeviceID(IntPtr lParam)
		{
			try
			{
				uint num = 0U;
				NativeMethods.GetRawInputData(lParam, 268435459U, IntPtr.Zero, ref num, (uint)Marshal.SizeOf(typeof(RawInputClass.RawInputHeader)));
				IntPtr intPtr = Marshal.AllocHGlobal((int)num);
				NativeMethods.GetRawInputData(lParam, 268435459U, intPtr, ref num, (uint)Marshal.SizeOf(typeof(RawInputClass.RawInputHeader)));
				RawInputClass.RawInput rawInput = (RawInputClass.RawInput)Marshal.PtrToStructure(intPtr, typeof(RawInputClass.RawInput));
				Marshal.FreeHGlobal(intPtr);
				if (rawInput.Data.Mouse.ButtonFlags == RawMouseButtons.LeftDown || rawInput.Data.Mouse.ButtonFlags == RawMouseButtons.RightDown)
				{
					return (int)rawInput.Header.Device;
				}
				return -1;
			}
			catch (Exception ex)
			{
				Logger.Info("Exception in raw input constructor : {0}", new object[] { ex.ToString() });
			}
			return -1;
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0005E058 File Offset: 0x0005C258
		public RawInputClass(IntPtr hwnd)
		{
			try
			{
				RawInputClass.RAWINPUTDEVICE[] array = new RawInputClass.RAWINPUTDEVICE[3];
				array[0].usUsagePage = 1;
				array[0].usUsage = 2;
				array[0].dwFlags = 256;
				array[0].hwndTarget = hwnd;
				array[1].usUsagePage = 1;
				array[1].usUsage = 5;
				array[1].dwFlags = 256;
				array[1].hwndTarget = hwnd;
				array[2].usUsagePage = 1;
				array[2].usUsage = 4;
				array[2].dwFlags = 256;
				array[2].hwndTarget = hwnd;
				if (!NativeMethods.RegisterRawInputDevices(array, (uint)array.Length, (uint)Marshal.SizeOf(array[0])))
				{
					Logger.Info("Failed to register raw input device(s).");
				}
				else
				{
					Logger.Info("Successfully registered raw input device(s).");
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Exception in raw input constructor : {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x04000994 RID: 2452
		private const int RID_INPUT = 268435459;

		// Token: 0x04000995 RID: 2453
		private const int RIDEV_INPUTSINK = 256;

		// Token: 0x0200016B RID: 363
		internal struct RAWINPUTDEVICE
		{
			// Token: 0x04000996 RID: 2454
			[MarshalAs(UnmanagedType.U2)]
			public ushort usUsagePage;

			// Token: 0x04000997 RID: 2455
			[MarshalAs(UnmanagedType.U2)]
			public ushort usUsage;

			// Token: 0x04000998 RID: 2456
			[MarshalAs(UnmanagedType.U4)]
			public int dwFlags;

			// Token: 0x04000999 RID: 2457
			public IntPtr hwndTarget;
		}

		// Token: 0x0200016C RID: 364
		internal struct RAWHID
		{
			// Token: 0x0400099A RID: 2458
			[MarshalAs(UnmanagedType.U4)]
			public int dwSizHid;

			// Token: 0x0400099B RID: 2459
			[MarshalAs(UnmanagedType.U4)]
			public int dwCount;
		}

		// Token: 0x0200016D RID: 365
		[StructLayout(LayoutKind.Explicit)]
		public struct RawMouse
		{
			// Token: 0x0400099C RID: 2460
			[FieldOffset(0)]
			public RawMouseFlags Flags;

			// Token: 0x0400099D RID: 2461
			[FieldOffset(4)]
			public RawMouseButtons ButtonFlags;

			// Token: 0x0400099E RID: 2462
			[FieldOffset(6)]
			public ushort ButtonData;

			// Token: 0x0400099F RID: 2463
			[FieldOffset(8)]
			public uint RawButtons;

			// Token: 0x040009A0 RID: 2464
			[FieldOffset(12)]
			public int LastX;

			// Token: 0x040009A1 RID: 2465
			[FieldOffset(16)]
			public int LastY;

			// Token: 0x040009A2 RID: 2466
			[FieldOffset(20)]
			public uint ExtraInformation;
		}

		// Token: 0x0200016E RID: 366
		internal struct RAWKEYBOARD
		{
			// Token: 0x040009A3 RID: 2467
			[MarshalAs(UnmanagedType.U2)]
			public ushort MakeCode;

			// Token: 0x040009A4 RID: 2468
			[MarshalAs(UnmanagedType.U2)]
			public ushort Flags;

			// Token: 0x040009A5 RID: 2469
			[MarshalAs(UnmanagedType.U2)]
			public ushort Reserved;

			// Token: 0x040009A6 RID: 2470
			[MarshalAs(UnmanagedType.U2)]
			public ushort VKey;

			// Token: 0x040009A7 RID: 2471
			[MarshalAs(UnmanagedType.U4)]
			public uint Message;

			// Token: 0x040009A8 RID: 2472
			[MarshalAs(UnmanagedType.U4)]
			public uint ExtraInformation;
		}

		// Token: 0x0200016F RID: 367
		public enum RawInputType
		{
			// Token: 0x040009AA RID: 2474
			Mouse,
			// Token: 0x040009AB RID: 2475
			Keyboard,
			// Token: 0x040009AC RID: 2476
			HID
		}

		// Token: 0x02000170 RID: 368
		public struct RawInput
		{
			// Token: 0x06000ED9 RID: 3801 RVA: 0x0000B065 File Offset: 0x00009265
			public RawInput(RawInputClass.RawInputHeader _header, RawInputClass.RawInput.Union _data)
			{
				this.Header = _header;
				this.Data = _data;
			}

			// Token: 0x040009AD RID: 2477
			public RawInputClass.RawInputHeader Header;

			// Token: 0x040009AE RID: 2478
			public RawInputClass.RawInput.Union Data;

			// Token: 0x02000171 RID: 369
			[StructLayout(LayoutKind.Explicit)]
			public struct Union
			{
				// Token: 0x040009AF RID: 2479
				[FieldOffset(0)]
				public RawInputClass.RawMouse Mouse;

				// Token: 0x040009B0 RID: 2480
				[FieldOffset(0)]
				public RawInputClass.RAWKEYBOARD Keyboard;

				// Token: 0x040009B1 RID: 2481
				[FieldOffset(0)]
				public RawInputClass.RAWHID HID;
			}
		}

		// Token: 0x02000172 RID: 370
		internal struct RawInputHeader
		{
			// Token: 0x040009B2 RID: 2482
			public RawInputClass.RawInputType Type;

			// Token: 0x040009B3 RID: 2483
			public int Size;

			// Token: 0x040009B4 RID: 2484
			public IntPtr Device;

			// Token: 0x040009B5 RID: 2485
			public IntPtr wParam;
		}
	}
}
