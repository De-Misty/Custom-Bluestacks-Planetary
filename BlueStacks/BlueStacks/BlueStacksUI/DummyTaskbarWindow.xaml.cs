using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using BlueStacks.Common;
using Vanara.PInvoke;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000057 RID: 87
	public partial class DummyTaskbarWindow : CustomWindow
	{
		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000482 RID: 1154 RVA: 0x00004F2E File Offset: 0x0000312E
		// (set) Token: 0x06000483 RID: 1155 RVA: 0x00004F36 File Offset: 0x00003136
		public string TaskbarThumbnailPath { get; set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x00004F3F File Offset: 0x0000313F
		// (set) Token: 0x06000485 RID: 1157 RVA: 0x00004F47 File Offset: 0x00003147
		public MainWindow ParentWindow { get; set; }

		// Token: 0x06000486 RID: 1158 RVA: 0x00004F50 File Offset: 0x00003150
		public DummyTaskbarWindow(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001D024 File Offset: 0x0001B224
		private void DummyTaskbarWindow_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				DummyTaskbarWindow.sThisHandle = new WindowInteropHelper(this).Handle;
				int num = Marshal.SizeOf(1);
				IntPtr intPtr = Marshal.AllocHGlobal(num);
				Marshal.WriteInt32(intPtr, 0, 1);
				HWND hwnd = new HWND(DummyTaskbarWindow.sThisHandle);
				DwmApi.DwmSetWindowAttribute(hwnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_FORCE_ICONIC_REPRESENTATION, intPtr, num);
				DwmApi.DwmSetWindowAttribute(hwnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_HAS_ICONIC_BITMAP, intPtr, num);
				Marshal.FreeHGlobal(intPtr);
				HwndSource.FromHwnd(DummyTaskbarWindow.sThisHandle).AddHook(new HwndSourceHook(this.WndProc));
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in setting window porperties for taskbar thumbnail : " + ex.ToString());
			}
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0001D0C8 File Offset: 0x0001B2C8
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			try
			{
				if (msg == 803)
				{
					object obj = DummyTaskbarWindow.sync;
					lock (obj)
					{
						int num = (lParam.ToInt32() >> 16) & 65535;
						int num2 = lParam.ToInt32() & 65535;
						BitmapImage bitmapImage = new BitmapImage();
						bitmapImage.BeginInit();
						bitmapImage.UriSource = new Uri(this.TaskbarThumbnailPath);
						bitmapImage.DecodePixelWidth = num;
						bitmapImage.DecodePixelHeight = num2;
						bitmapImage.EndInit();
						Bitmap bitmap = ImageUtils.BitmapImage2Bitmap(bitmapImage);
						DwmApi.DwmSetIconicThumbnail(new HWND(DummyTaskbarWindow.sThisHandle), bitmap.GetHbitmap(), DwmApi.DWM_SETICONICPREVIEW_Flags.DWM_SIT_NONE);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in setting taskbar thumbnail : " + ex.ToString());
			}
			return IntPtr.Zero;
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00004F65 File Offset: 0x00003165
		private void DummyTaskbarWindow_Closing(object sender, CancelEventArgs e)
		{
			this.ParentWindow.DummyWindow = null;
		}

		// Token: 0x04000278 RID: 632
		private const int WM_DWMSENDICONICTHUMBNAIL = 803;

		// Token: 0x04000279 RID: 633
		private static readonly object sync = new object();

		// Token: 0x0400027A RID: 634
		private static IntPtr sThisHandle = IntPtr.Zero;
	}
}
