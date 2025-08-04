using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200027D RID: 637
	internal class WindowWndProcHandler
	{
		// Token: 0x06001743 RID: 5955 RVA: 0x0008A138 File Offset: 0x00088338
		internal WindowWndProcHandler(MainWindow window)
		{
			this.mWindowInstance = window;
			MainWindow mainWindow = this.mWindowInstance;
			mainWindow.ResizeBegin = (EventHandler)Delegate.Combine(mainWindow.ResizeBegin, new EventHandler(this.mWindowInstance.MainWindow_ResizeBegin));
			MainWindow mainWindow2 = this.mWindowInstance;
			mainWindow2.ResizeEnd = (EventHandler)Delegate.Combine(mainWindow2.ResizeEnd, new EventHandler(this.mWindowInstance.MainWindow_ResizeEnd));
			this.mWindowInstance.SourceInitialized += this.Instance_SourceInitialized;
			WindowWndProcHandler.SetMenuDropDownAlignment();
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0000FB13 File Offset: 0x0000DD13
		private void Instance_SourceInitialized(object sender, EventArgs e)
		{
			this._hwndSource = (HwndSource)PresentationSource.FromVisual(this.mWindowInstance);
			this._hwndSource.AddHook(new HwndSourceHook(this.WndProc));
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x0008A1D4 File Offset: 0x000883D4
		internal void AddRawInputHandler()
		{
			try
			{
				if (PromotionObject.Instance != null && PromotionObject.Instance.IsSecurityMetricsEnable)
				{
					WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this.mWindowInstance);
					this.mRawInput = new RawInputClass(windowInteropHelper.Handle);
					Logger.Info("Adding raw input handle");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while adding raw input handle: {0}", new object[] { ex.ToString() });
			}
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0008A24C File Offset: 0x0008844C
		internal void ResizeRectangle_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.IsResizingEnabled)
			{
				string name = (sender as global::System.Windows.Shapes.Rectangle).Name;
				if (name != null)
				{
					uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num <= 1319594794U)
					{
						if (num <= 591972422U)
						{
							if (num != 306900080U)
							{
								if (num != 591972422U)
								{
									return;
								}
								if (!(name == "topRight"))
								{
									return;
								}
								this.mWindowInstance.Cursor = Cursors.SizeNESW;
								return;
							}
							else
							{
								if (!(name == "left"))
								{
									return;
								}
								this.mWindowInstance.Cursor = Cursors.SizeWE;
								return;
							}
						}
						else if (num != 873742264U)
						{
							if (num != 1319594794U)
							{
								return;
							}
							if (!(name == "bottom"))
							{
								return;
							}
							this.mWindowInstance.Cursor = Cursors.SizeNS;
							return;
						}
						else
						{
							if (!(name == "bottomRight"))
							{
								return;
							}
							this.mWindowInstance.Cursor = Cursors.SizeNWSE;
						}
					}
					else if (num <= 2059707271U)
					{
						if (num != 2028154341U)
						{
							if (num != 2059707271U)
							{
								return;
							}
							if (!(name == "bottomLeft"))
							{
								return;
							}
							this.mWindowInstance.Cursor = Cursors.SizeNESW;
							return;
						}
						else
						{
							if (!(name == "right"))
							{
								return;
							}
							this.mWindowInstance.Cursor = Cursors.SizeWE;
							return;
						}
					}
					else if (num != 2387400333U)
					{
						if (num == 2802900028U)
						{
							if (!(name == "top"))
							{
								return;
							}
							this.mWindowInstance.Cursor = Cursors.SizeNS;
							return;
						}
					}
					else
					{
						if (!(name == "topLeft"))
						{
							return;
						}
						this.mWindowInstance.Cursor = Cursors.SizeNWSE;
						return;
					}
				}
			}
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x0008A3D8 File Offset: 0x000885D8
		internal void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsResizingEnabled)
			{
				e.Handled = true;
				string name = (sender as global::System.Windows.Shapes.Rectangle).Name;
				if (name != null)
				{
					uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num <= 1319594794U)
					{
						if (num <= 591972422U)
						{
							if (num != 306900080U)
							{
								if (num == 591972422U)
								{
									if (name == "topRight")
									{
										this.mWindowInstance.Cursor = Cursors.SizeNESW;
										this.ResizeWindow(WindowWndProcHandler.ResizeDirection.TopRight);
										return;
									}
								}
							}
							else if (name == "left")
							{
								this.mWindowInstance.Cursor = Cursors.SizeWE;
								this.mAdjustingWidth = true;
								this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Left);
								return;
							}
						}
						else if (num != 873742264U)
						{
							if (num == 1319594794U)
							{
								if (name == "bottom")
								{
									this.mWindowInstance.Cursor = Cursors.SizeNS;
									this.mAdjustingWidth = false;
									this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Bottom);
									return;
								}
							}
						}
						else if (name == "bottomRight")
						{
							this.mWindowInstance.Cursor = Cursors.SizeNWSE;
							this.ResizeWindow(WindowWndProcHandler.ResizeDirection.BottomRight);
							return;
						}
					}
					else if (num <= 2059707271U)
					{
						if (num != 2028154341U)
						{
							if (num == 2059707271U)
							{
								if (name == "bottomLeft")
								{
									this.mWindowInstance.Cursor = Cursors.SizeNESW;
									this.ResizeWindow(WindowWndProcHandler.ResizeDirection.BottomLeft);
									return;
								}
							}
						}
						else if (name == "right")
						{
							this.mWindowInstance.Cursor = Cursors.SizeWE;
							this.mAdjustingWidth = true;
							this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Right);
							return;
						}
					}
					else if (num != 2387400333U)
					{
						if (num == 2802900028U)
						{
							if (name == "top")
							{
								this.mWindowInstance.Cursor = Cursors.SizeNS;
								this.mAdjustingWidth = false;
								this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Top);
								return;
							}
						}
					}
					else if (name == "topLeft")
					{
						this.mWindowInstance.Cursor = Cursors.SizeNWSE;
						this.ResizeWindow(WindowWndProcHandler.ResizeDirection.TopLeft);
						return;
					}
				}
				e.Handled = false;
			}
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0008A614 File Offset: 0x00088814
		internal void ResizeWindow(WindowWndProcHandler.ResizeDirection direction)
		{
			this.mWindowInstance.ResizeBegin(this.mWindowInstance, new EventArgs());
			NativeMethods.SendMessage(this._hwndSource.Handle, 274U, (IntPtr)((int)(61440 + direction)), IntPtr.Zero);
			this.mWindowInstance.ResizeEnd(this.mWindowInstance, new EventArgs());
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0008A680 File Offset: 0x00088880
		internal global::System.Drawing.Point GetMousePosition()
		{
			NativeMethods.Win32Point win32Point = default(NativeMethods.Win32Point);
			NativeMethods.GetCursorPos(ref win32Point);
			return new global::System.Drawing.Point(win32Point.X, win32Point.Y);
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x0008A6B0 File Offset: 0x000888B0
		internal IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (WindowWndProcHandler.isLogWndProc)
			{
				Logger.Info(string.Concat(new string[]
				{
					"WndProcMessage: ",
					msg.ToString(),
					"~~",
					wParam.ToString(),
					"~~",
					lParam.ToString(),
					"~~"
				}));
			}
			WindowWndProcHandler.WM wm = (WindowWndProcHandler.WM)msg;
			if (wm <= WindowWndProcHandler.WM.WINDOWPOSCHANGING)
			{
				if (wm <= WindowWndProcHandler.WM.SYSCOLORCHANGE)
				{
					if (wm == WindowWndProcHandler.WM.SETFOCUS)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							this.mWindowInstance.Dispatcher.Invoke(new Action(delegate
							{
								try
								{
									bool flag2 = true;
									foreach (object obj in this.mWindowInstance.OwnedWindows)
									{
										Window window = (Window)obj;
										CustomWindow customWindow = window as CustomWindow;
										if (customWindow != null)
										{
											if (!customWindow.IsShowGLWindow && !KMManager.sIsInScriptEditingMode)
											{
												flag2 = false;
												Logger.Debug("OnFocusChanged window IsShowGLWindow false: " + customWindow.Name);
											}
										}
										else
										{
											Logger.Debug("OnFocusChanged Non Custom window found! " + window.Name);
										}
									}
									if (flag2 && !this.mWindowInstance.mIsFocusComeFromImap)
									{
										this.mWindowInstance.mFrontendHandler.ShowGLWindow();
									}
									this.mWindowInstance.mIsFocusComeFromImap = false;
								}
								catch
								{
								}
							}), new object[0]);
						});
						goto IL_033F;
					}
					if (wm != WindowWndProcHandler.WM.SYSCOLORCHANGE)
					{
						goto IL_033F;
					}
				}
				else if (wm != WindowWndProcHandler.WM.WININICHANGE)
				{
					if (wm == WindowWndProcHandler.WM.GETMINMAXINFO)
					{
						goto IL_0149;
					}
					if (wm != WindowWndProcHandler.WM.WINDOWPOSCHANGING)
					{
						goto IL_033F;
					}
					WindowWndProcHandler.WINDOWPOS windowpos = (WindowWndProcHandler.WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WindowWndProcHandler.WINDOWPOS));
					if ((windowpos.flags & 2) != 0)
					{
						return IntPtr.Zero;
					}
					if ((Window)HwndSource.FromHwnd(hwnd).RootVisual == null)
					{
						return IntPtr.Zero;
					}
					if (this.mWindowInstance.WindowState != WindowState.Normal)
					{
						return IntPtr.Zero;
					}
					bool flag = true;
					if (this.mWindowInstance.MinWidthScaled > windowpos.cx)
					{
						windowpos.cx = this.mWindowInstance.MinWidthScaled;
						windowpos.cy = (int)this.mWindowInstance.GetHeightFromWidth((double)windowpos.cx, true, false);
						flag = false;
					}
					else if (this.mWindowInstance.MinHeightScaled > windowpos.cy)
					{
						windowpos.cy = this.mWindowInstance.MinHeightScaled;
						windowpos.cx = (int)this.mWindowInstance.GetWidthFromHeight((double)windowpos.cy, true, false);
						flag = false;
					}
					if (windowpos.cx > this.mWindowInstance.MaxWidthScaled || windowpos.cy > this.mWindowInstance.MaxHeightScaled)
					{
						windowpos.cx = this.mWindowInstance.MaxWidthScaled;
						windowpos.cy = this.mWindowInstance.MaxHeightScaled;
						flag = false;
					}
					if (flag)
					{
						if (this.mAdjustingWidth)
						{
							windowpos.cy = (int)this.mWindowInstance.GetHeightFromWidth((double)windowpos.cx, true, false);
						}
						else
						{
							windowpos.cx = (int)this.mWindowInstance.GetWidthFromHeight((double)windowpos.cy, true, false);
						}
					}
					Marshal.StructureToPtr(windowpos, lParam, true);
					handled = true;
					goto IL_033F;
				}
			}
			else if (wm <= WindowWndProcHandler.WM.SYSCOMMAND)
			{
				if (wm != WindowWndProcHandler.WM.DISPLAYCHANGE)
				{
					if (wm != WindowWndProcHandler.WM.INPUT)
					{
						if (wm != WindowWndProcHandler.WM.SYSCOMMAND)
						{
							goto IL_033F;
						}
						if (wParam == (IntPtr)61696)
						{
							handled = true;
							goto IL_033F;
						}
						goto IL_033F;
					}
					else
					{
						int num = -1;
						if (this.mRawInput != null)
						{
							num = RawInputClass.GetDeviceID(lParam);
						}
						if (num == 0 && SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.mWindowInstance.mVmName))
						{
							SecurityMetrics.SecurityMetricsInstanceList[this.mWindowInstance.mVmName].AddSecurityBreach(SecurityBreach.SYNTHETIC_INPUT, string.Empty);
							goto IL_033F;
						}
						goto IL_033F;
					}
				}
			}
			else
			{
				if (wm == WindowWndProcHandler.WM.ENTERMENULOOP)
				{
					handled = true;
					goto IL_033F;
				}
				if (wm != WindowWndProcHandler.WM.DEVICECHANGE && wm != WindowWndProcHandler.WM.THEMECHANGED)
				{
					goto IL_033F;
				}
			}
			using (new Timer(delegate(object x)
			{
				WindowWndProcHandler.SetMenuDropDownAlignment();
			}, null, TimeSpan.FromMilliseconds(2.0), TimeSpan.FromMilliseconds(-1.0)))
			{
				goto IL_033F;
			}
			IL_0149:
			this.WmGetMinMaxInfo(hwnd, lParam);
			handled = true;
			IL_033F:
			return IntPtr.Zero;
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0008AA14 File Offset: 0x00088C14
		private static void SetMenuDropDownAlignment()
		{
			try
			{
				if (SystemParameters.MenuDropAlignment)
				{
					typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
					bool menuDropAlignment = SystemParameters.MenuDropAlignment;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("error setting _menuDropAlignment" + ex.ToString());
			}
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0008AA7C File Offset: 0x00088C7C
		private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
		{
			WindowWndProcHandler.MINMAXINFO minmaxinfo = (WindowWndProcHandler.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(WindowWndProcHandler.MINMAXINFO));
			IntPtr intPtr = NativeMethods.MonitorFromWindow(hwnd, 1);
			if (intPtr != IntPtr.Zero)
			{
				WindowWndProcHandler.MONITORINFOEX monitorinfoex = new WindowWndProcHandler.MONITORINFOEX
				{
					cbSize = Marshal.SizeOf(typeof(MONITORINFO))
				};
				NativeMethods.GetMonitorInfo(intPtr, monitorinfoex);
				IntereopRect rcWork = monitorinfoex.rcWork;
				IntereopRect rcMonitor = monitorinfoex.rcMonitor;
				WindowWndProcHandler.TaskbarLocation taskbarPosition = WindowWndProcHandler.GetTaskbarPosition();
				if (!this.mWindowInstance.mIsFullScreen)
				{
					minmaxinfo.ptMaxPosition.X = Math.Abs(rcWork.Left - rcMonitor.Left);
					minmaxinfo.ptMaxPosition.Y = Math.Abs(rcWork.Top - rcMonitor.Top);
					minmaxinfo.ptMaxSize.X = Math.Abs(rcWork.Width);
					minmaxinfo.ptMaxSize.Y = Math.Abs(rcWork.Height);
					if (rcWork == rcMonitor)
					{
						switch (taskbarPosition)
						{
						case WindowWndProcHandler.TaskbarLocation.Left:
							minmaxinfo.ptMaxPosition.X = minmaxinfo.ptMaxPosition.X + 2;
							break;
						case WindowWndProcHandler.TaskbarLocation.Top:
							minmaxinfo.ptMaxPosition.Y = minmaxinfo.ptMaxPosition.Y + 2;
							break;
						case WindowWndProcHandler.TaskbarLocation.Right:
							minmaxinfo.ptMaxSize.X = minmaxinfo.ptMaxSize.X - 2;
							break;
						case WindowWndProcHandler.TaskbarLocation.Bottom:
							minmaxinfo.ptMaxSize.Y = minmaxinfo.ptMaxSize.Y - 2;
							break;
						}
					}
				}
				else
				{
					minmaxinfo.ptMaxPosition.X = 0;
					minmaxinfo.ptMaxPosition.Y = 0;
					minmaxinfo.ptMaxSize.X = Math.Abs(rcMonitor.Width);
					minmaxinfo.ptMaxSize.Y = Math.Abs(rcMonitor.Height);
				}
				minmaxinfo.ptMaxTrackSize.X = minmaxinfo.ptMaxSize.X;
				minmaxinfo.ptMaxTrackSize.Y = minmaxinfo.ptMaxSize.Y;
			}
			Marshal.StructureToPtr(minmaxinfo, lParam, true);
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x0008AC70 File Offset: 0x00088E70
		internal static IntereopRect GetFullscreenMonitorSize(IntPtr hwnd, bool isWorkAreaRequired = false)
		{
			IntPtr intPtr = NativeMethods.MonitorFromWindow(hwnd, 1);
			if (!(intPtr != IntPtr.Zero))
			{
				return default(IntereopRect);
			}
			WindowWndProcHandler.MONITORINFOEX monitorinfoex = new WindowWndProcHandler.MONITORINFOEX();
			NativeMethods.GetMonitorInfo(intPtr, monitorinfoex);
			if (isWorkAreaRequired)
			{
				return monitorinfoex.rcWork;
			}
			return monitorinfoex.rcMonitor;
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x0008ACBC File Offset: 0x00088EBC
		private static WindowWndProcHandler.TaskbarLocation GetTaskbarPosition()
		{
			WindowWndProcHandler.TaskbarLocation taskbarLocation = WindowWndProcHandler.TaskbarLocation.None;
			WindowWndProcHandler.APPBARDATA appbardata = default(WindowWndProcHandler.APPBARDATA);
			appbardata.cbSize = Marshal.SizeOf(appbardata);
			if (NativeMethods.SHAppBarMessage(5, ref appbardata) == IntPtr.Zero)
			{
				return taskbarLocation;
			}
			if (appbardata.rc.Left == appbardata.rc.Top)
			{
				if (appbardata.rc.Right < appbardata.rc.Bottom)
				{
					taskbarLocation = WindowWndProcHandler.TaskbarLocation.Left;
				}
				if (appbardata.rc.Right > appbardata.rc.Bottom)
				{
					taskbarLocation = WindowWndProcHandler.TaskbarLocation.Top;
				}
			}
			if (appbardata.rc.Left > appbardata.rc.Top)
			{
				taskbarLocation = WindowWndProcHandler.TaskbarLocation.Right;
			}
			if (appbardata.rc.Left < appbardata.rc.Top)
			{
				taskbarLocation = WindowWndProcHandler.TaskbarLocation.Bottom;
			}
			return taskbarLocation;
		}

		// Token: 0x04000E52 RID: 3666
		private const int ABM_GETTASKBARPOS = 5;

		// Token: 0x04000E53 RID: 3667
		internal bool IsResizingEnabled = true;

		// Token: 0x04000E54 RID: 3668
		internal bool IsMinMaxEnabled = true;

		// Token: 0x04000E55 RID: 3669
		internal bool mAdjustingWidth;

		// Token: 0x04000E56 RID: 3670
		private MainWindow mWindowInstance;

		// Token: 0x04000E57 RID: 3671
		private RawInputClass mRawInput;

		// Token: 0x04000E58 RID: 3672
		private HwndSource _hwndSource;

		// Token: 0x04000E59 RID: 3673
		internal static bool isLogWndProc;

		// Token: 0x04000E5A RID: 3674
		private const int MONITOR_DEFAULTTOPRIMARY = 1;

		// Token: 0x0200027E RID: 638
		internal enum ResizeDirection
		{
			// Token: 0x04000E5C RID: 3676
			Left = 1,
			// Token: 0x04000E5D RID: 3677
			Right,
			// Token: 0x04000E5E RID: 3678
			Top,
			// Token: 0x04000E5F RID: 3679
			TopLeft,
			// Token: 0x04000E60 RID: 3680
			TopRight,
			// Token: 0x04000E61 RID: 3681
			Bottom,
			// Token: 0x04000E62 RID: 3682
			BottomLeft,
			// Token: 0x04000E63 RID: 3683
			BottomRight
		}

		// Token: 0x0200027F RID: 639
		private struct WINDOWPOS
		{
			// Token: 0x04000E64 RID: 3684
			public IntPtr hwnd;

			// Token: 0x04000E65 RID: 3685
			public IntPtr hwndInsertAfter;

			// Token: 0x04000E66 RID: 3686
			public int x;

			// Token: 0x04000E67 RID: 3687
			public int y;

			// Token: 0x04000E68 RID: 3688
			public int cx;

			// Token: 0x04000E69 RID: 3689
			public int cy;

			// Token: 0x04000E6A RID: 3690
			public int flags;
		}

		// Token: 0x02000280 RID: 640
		private enum SWP
		{
			// Token: 0x04000E6C RID: 3692
			NOMOVE = 2
		}

		// Token: 0x02000281 RID: 641
		private enum WM
		{
			// Token: 0x04000E6E RID: 3694
			SYSCOMMAND = 274,
			// Token: 0x04000E6F RID: 3695
			ENTERMENULOOP = 529,
			// Token: 0x04000E70 RID: 3696
			WINDOWPOSCHANGING = 70,
			// Token: 0x04000E71 RID: 3697
			NCCALCSIZE = 131,
			// Token: 0x04000E72 RID: 3698
			EXITSIZEMOVE = 562,
			// Token: 0x04000E73 RID: 3699
			GETMINMAXINFO = 36,
			// Token: 0x04000E74 RID: 3700
			WININICHANGE = 26,
			// Token: 0x04000E75 RID: 3701
			DEVICECHANGE = 537,
			// Token: 0x04000E76 RID: 3702
			DISPLAYCHANGE = 126,
			// Token: 0x04000E77 RID: 3703
			THEMECHANGED = 794,
			// Token: 0x04000E78 RID: 3704
			SYSCOLORCHANGE = 21,
			// Token: 0x04000E79 RID: 3705
			INPUT = 255,
			// Token: 0x04000E7A RID: 3706
			SETFOCUS = 7,
			// Token: 0x04000E7B RID: 3707
			ACTIVATE = 6
		}

		// Token: 0x02000282 RID: 642
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
		public class MONITORINFOEX
		{
			// Token: 0x04000E7C RID: 3708
			public int cbSize = Marshal.SizeOf(typeof(WindowWndProcHandler.MONITORINFOEX));

			// Token: 0x04000E7D RID: 3709
			public IntereopRect rcMonitor;

			// Token: 0x04000E7E RID: 3710
			public IntereopRect rcWork;

			// Token: 0x04000E7F RID: 3711
			public int dwFlags;

			// Token: 0x04000E80 RID: 3712
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szDevice = new char[32];
		}

		// Token: 0x02000283 RID: 643
		internal struct MINMAXINFO
		{
			// Token: 0x04000E81 RID: 3713
			public WindowWndProcHandler.POINT ptReserved;

			// Token: 0x04000E82 RID: 3714
			public WindowWndProcHandler.POINT ptMaxSize;

			// Token: 0x04000E83 RID: 3715
			public WindowWndProcHandler.POINT ptMaxPosition;

			// Token: 0x04000E84 RID: 3716
			public WindowWndProcHandler.POINT ptMinTrackSize;

			// Token: 0x04000E85 RID: 3717
			public WindowWndProcHandler.POINT ptMaxTrackSize;
		}

		// Token: 0x02000284 RID: 644
		public struct POINT
		{
			// Token: 0x06001753 RID: 5971 RVA: 0x0000FB91 File Offset: 0x0000DD91
			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			// Token: 0x06001754 RID: 5972 RVA: 0x0000FBA1 File Offset: 0x0000DDA1
			public POINT(global::System.Drawing.Point pt)
			{
				this = new WindowWndProcHandler.POINT(pt.X, pt.Y);
			}

			// Token: 0x06001755 RID: 5973 RVA: 0x0000FBB7 File Offset: 0x0000DDB7
			public static implicit operator global::System.Drawing.Point(WindowWndProcHandler.POINT p)
			{
				return new global::System.Drawing.Point(p.X, p.Y);
			}

			// Token: 0x06001756 RID: 5974 RVA: 0x0000FBCA File Offset: 0x0000DDCA
			public static implicit operator WindowWndProcHandler.POINT(global::System.Drawing.Point p)
			{
				return new WindowWndProcHandler.POINT(p.X, p.Y);
			}

			// Token: 0x04000E86 RID: 3718
			public int X;

			// Token: 0x04000E87 RID: 3719
			public int Y;
		}

		// Token: 0x02000285 RID: 645
		internal struct APPBARDATA
		{
			// Token: 0x04000E88 RID: 3720
			public int cbSize;

			// Token: 0x04000E89 RID: 3721
			public IntPtr hWnd;

			// Token: 0x04000E8A RID: 3722
			public int uCallbackMessage;

			// Token: 0x04000E8B RID: 3723
			public int uEdge;

			// Token: 0x04000E8C RID: 3724
			public RECT rc;

			// Token: 0x04000E8D RID: 3725
			public IntPtr lParam;
		}

		// Token: 0x02000286 RID: 646
		private enum TaskbarLocation
		{
			// Token: 0x04000E8F RID: 3727
			None,
			// Token: 0x04000E90 RID: 3728
			Left,
			// Token: 0x04000E91 RID: 3729
			Top,
			// Token: 0x04000E92 RID: 3730
			Right,
			// Token: 0x04000E93 RID: 3731
			Bottom
		}
	}
}
