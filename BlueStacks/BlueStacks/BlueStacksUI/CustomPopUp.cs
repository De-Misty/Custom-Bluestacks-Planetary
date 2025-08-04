using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000B1 RID: 177
	public class CustomPopUp : Popup
	{
		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x00006AC3 File Offset: 0x00004CC3
		// (set) Token: 0x0600072A RID: 1834 RVA: 0x00006ACB File Offset: 0x00004CCB
		public bool IsFocusOnMouseClick { get; set; }

		// Token: 0x0600072B RID: 1835 RVA: 0x00006AD4 File Offset: 0x00004CD4
		private void CustomPopUp_Initialized(object sender, EventArgs e)
		{
			RenderHelper.ChangeRenderModeToSoftware(sender);
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600072C RID: 1836 RVA: 0x00006ADC File Offset: 0x00004CDC
		// (set) Token: 0x0600072D RID: 1837 RVA: 0x00006AEE File Offset: 0x00004CEE
		public bool IsTopmost
		{
			get
			{
				return (bool)base.GetValue(CustomPopUp.IsTopmostProperty);
			}
			set
			{
				base.SetValue(CustomPopUp.IsTopmostProperty, value);
			}
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x00027DE4 File Offset: 0x00025FE4
		public CustomPopUp()
		{
			base.Loaded += this.OnPopupLoaded;
			base.Unloaded += this.OnPopupUnloaded;
			base.Opened += this.CustomPopUp_Initialized;
			base.PreviewMouseDown += this.CustomPopUp_PreviewMouseDown;
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x00027E40 File Offset: 0x00026040
		private void CustomPopUp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsFocusOnMouseClick)
			{
				try
				{
					HwndSource hwndSource = PresentationSource.FromVisual(base.Child) as HwndSource;
					if (hwndSource != null)
					{
						InteropWindow.SetForegroundWindow(hwndSource.Handle);
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in setting popup as foreground window: {0}", new object[] { ex });
				}
			}
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x00027EA0 File Offset: 0x000260A0
		private void OnPopupLoaded(object sender, RoutedEventArgs e)
		{
			if (this.mAlreadyLoaded)
			{
				return;
			}
			this.mAlreadyLoaded = true;
			UIElement child = base.Child;
			if (child != null)
			{
				child.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnChildPreviewMouseLeftButtonDown), true);
			}
			this.mParentWindow = Window.GetWindow(this);
			if (this.mParentWindow == null)
			{
				return;
			}
			this.mParentWindow.Activated += this.OnParentWindowActivated;
			this.mParentWindow.Deactivated += this.OnParentWindowDeactivated;
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x00006B01 File Offset: 0x00004D01
		private void OnPopupUnloaded(object sender, RoutedEventArgs e)
		{
			if (this.mParentWindow == null)
			{
				return;
			}
			this.mParentWindow.Activated -= this.OnParentWindowActivated;
			this.mParentWindow.Deactivated -= this.OnParentWindowDeactivated;
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x00006B3A File Offset: 0x00004D3A
		private void OnParentWindowActivated(object sender, EventArgs e)
		{
			this.SetTopmostState(true);
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00006B43 File Offset: 0x00004D43
		private void OnParentWindowDeactivated(object sender, EventArgs e)
		{
			if (!this.IsTopmost)
			{
				this.SetTopmostState(this.IsTopmost);
			}
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00027F24 File Offset: 0x00026124
		private void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Logger.Debug("Child Mouse Left Button Down");
			this.SetTopmostState(true);
			if (this.mParentWindow != null && !this.mParentWindow.IsActive && !this.IsTopmost)
			{
				this.mParentWindow.Activate();
				Logger.Debug("Activating Parent from child Left Button Down");
			}
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00006B59 File Offset: 0x00004D59
		private static void OnIsTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs _)
		{
			CustomPopUp customPopUp = (CustomPopUp)obj;
			customPopUp.SetTopmostState(customPopUp.IsTopmost);
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x00006B6C File Offset: 0x00004D6C
		protected override void OnOpened(EventArgs e)
		{
			this.mParentWindow = Window.GetWindow(this);
			this.SetTopmostState(this.IsTopmost);
			base.OnOpened(e);
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x00027F78 File Offset: 0x00026178
		private void SetTopmostState(bool isTop)
		{
			try
			{
				if (this.mParentWindow != null && !isTop && InteropWindow.GetTopmostOwnerWindow(this.mParentWindow).Topmost)
				{
					isTop = true;
				}
				if (this.mAppliedTopMost != null)
				{
					bool? flag = this.mAppliedTopMost;
					bool flag2 = isTop;
					if ((flag.GetValueOrDefault() == flag2) & (flag != null))
					{
						return;
					}
				}
				if (base.Child != null)
				{
					HwndSource hwndSource = PresentationSource.FromVisual(base.Child) as HwndSource;
					if (hwndSource != null)
					{
						IntPtr handle = hwndSource.Handle;
						RECT rect;
						if (NativeMethods.GetWindowRect(handle, out rect))
						{
							Logger.Debug("setting z-order " + isTop.ToString());
							if (isTop)
							{
								NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_TOPMOST, rect.Left, rect.Top, (int)base.Width, (int)base.Height, 1563U);
							}
							else
							{
								NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_BOTTOM, rect.Left, rect.Top, (int)base.Width, (int)base.Height, 1563U);
								NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_TOP, rect.Left, rect.Top, (int)base.Width, (int)base.Height, 1563U);
								NativeMethods.SetWindowPos(handle, CustomPopUp.HWND_NOTOPMOST, rect.Left, rect.Top, (int)base.Width, (int)base.Height, 1563U);
							}
							this.mAppliedTopMost = new bool?(isTop);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in set topmost state in custom popup: {0}", new object[] { ex });
			}
		}

		// Token: 0x040003CC RID: 972
		public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register("IsTopmost", typeof(bool), typeof(CustomPopUp), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(CustomPopUp.OnIsTopmostChanged)));

		// Token: 0x040003CD RID: 973
		private bool? mAppliedTopMost;

		// Token: 0x040003CE RID: 974
		private bool mAlreadyLoaded;

		// Token: 0x040003CF RID: 975
		private Window mParentWindow;

		// Token: 0x040003D0 RID: 976
		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

		// Token: 0x040003D1 RID: 977
		private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

		// Token: 0x040003D2 RID: 978
		private static readonly IntPtr HWND_TOP = new IntPtr(0);

		// Token: 0x040003D3 RID: 979
		private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

		// Token: 0x040003D4 RID: 980
		private const uint SWP_NOSIZE = 1U;

		// Token: 0x040003D5 RID: 981
		private const uint SWP_NOMOVE = 2U;

		// Token: 0x040003D6 RID: 982
		private const uint SWP_NOREDRAW = 8U;

		// Token: 0x040003D7 RID: 983
		private const uint SWP_NOACTIVATE = 16U;

		// Token: 0x040003D8 RID: 984
		private const uint SWP_NOOWNERZORDER = 512U;

		// Token: 0x040003D9 RID: 985
		private const uint SWP_NOSENDCHANGING = 1024U;

		// Token: 0x040003DA RID: 986
		private const uint TOPMOST_FLAGS = 1563U;
	}
}
