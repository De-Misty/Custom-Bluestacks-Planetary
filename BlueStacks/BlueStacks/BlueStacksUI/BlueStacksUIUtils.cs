using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000180 RID: 384
	internal class BlueStacksUIUtils : IDisposable
	{
		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000F5C RID: 3932 RVA: 0x0000B379 File Offset: 0x00009579
		// (set) Token: 0x06000F5D RID: 3933 RVA: 0x00061684 File Offset: 0x0005F884
		public int CurrentVolumeLevel
		{
			get
			{
				return this.mCurrentVolumeLevel;
			}
			private set
			{
				if (value <= 0)
				{
					this.mCurrentVolumeLevel = 0;
				}
				else if (value >= 100)
				{
					this.mCurrentVolumeLevel = 100;
				}
				else
				{
					this.mCurrentVolumeLevel = value;
				}
				this.ParentWindow.EngineInstanceRegistry.Volume = this.mCurrentVolumeLevel;
				this.ParentWindow.mCommonHandler.OnVolumeChanged(this.mCurrentVolumeLevel);
			}
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x0000B381 File Offset: 0x00009581
		internal static bool IsAlphabet(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x000616E0 File Offset: 0x0005F8E0
		internal BlueStacksUIUtils(MainWindow window)
		{
			this.ParentWindow = window;
			this.mCurrentVolumeLevel = this.ParentWindow.EngineInstanceRegistry.Volume;
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x0006172C File Offset: 0x0005F92C
		internal static void CloseContainerWindow(FrameworkElement control)
		{
			FrameworkElement frameworkElement = control;
			while (frameworkElement != null && !(frameworkElement is ContainerWindow))
			{
				frameworkElement = frameworkElement.Parent as FrameworkElement;
			}
			if (frameworkElement != null)
			{
				(frameworkElement as ContainerWindow).Close();
			}
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x00061764 File Offset: 0x0005F964
		internal static void RefreshKeyMap(string packageName)
		{
			using (Dictionary<string, MainWindow>.Enumerator enumerator = BlueStacksUIUtils.DictWindows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, MainWindow> item = enumerator.Current;
					try
					{
						if (string.Equals(item.Value.mTopBar.mAppTabButtons.SelectedTab.PackageName, packageName, StringComparison.InvariantCulture))
						{
							item.Value.mFrontendHandler.RefreshKeyMap(packageName);
							if (RegistryManager.Instance.ShowKeyControlsOverlay)
							{
								KMManager.LoadIMActions(item.Value, packageName);
								if (KMManager.CanvasWindow == null || !KMManager.CanvasWindow.IsVisible || KMManager.CanvasWindow.Owner.GetHashCode() != item.Value.GetHashCode())
								{
									Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate
									{
										if (KMManager.dictOverlayWindow.ContainsKey(item.Value) && BlueStacksUIUtils.LastActivatedWindow != item.Value)
										{
											KMManager.dictOverlayWindow[item.Value].Init();
										}
									}), new object[0]);
								}
							}
						}
					}
					catch (Exception ex)
					{
						Logger.Error(string.Concat(new string[]
						{
							ex.ToString(),
							Environment.NewLine,
							"Exception refreshing mapping of package : ",
							packageName,
							" for instance : ",
							item.Value.mVmName
						}));
					}
				}
			}
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0000B39E File Offset: 0x0000959E
		public static bool IsModal(Window window)
		{
			return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x000618E0 File Offset: 0x0005FAE0
		private static void CloseWindows(Window win)
		{
			for (int i = win.OwnedWindows.Count - 1; i >= 0; i--)
			{
				BlueStacksUIUtils.CloseWindows(win.OwnedWindows[i]);
				if (win.OwnedWindows[i] != null && win.OwnedWindows[i].IsLoaded && win.OwnedWindows[i].Visibility == Visibility.Visible)
				{
					if (BlueStacksUIUtils.IsModal(win.OwnedWindows[i]))
					{
						win.OwnedWindows[i].Close();
					}
					else
					{
						win.OwnedWindows[i].Visibility = Visibility.Hidden;
					}
				}
			}
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x00061988 File Offset: 0x0005FB88
		internal static void HideUnhideBlueStacks(bool isHide)
		{
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
			{
				if (!mainWindow.mIsMinimizedThroughCloseButton)
				{
					BlueStacksUIUtils.HideUnhideParentWindow(isHide, mainWindow);
				}
			}
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x000619E8 File Offset: 0x0005FBE8
		internal static void HideUnhideParentWindow(bool isHide, MainWindow window)
		{
			if (isHide)
			{
				window.Dispatcher.Invoke(new Action(delegate
				{
					BlueStacksUIUtils.CloseWindows(window);
					window.WindowState = WindowState.Minimized;
					window.Hide();
					window.ShowInTaskbar = false;
				}), new object[0]);
				return;
			}
			window.ShowInTaskbar = true;
			window.ShowActivated = true;
			window.Show();
			if (window.mIsFullScreen)
			{
				window.WindowState = WindowState.Maximized;
			}
			else
			{
				window.WindowState = WindowState.Normal;
			}
			if (!window.Topmost)
			{
				window.Topmost = true;
				Action <>9__2;
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					Dispatcher dispatcher = window.Dispatcher;
					Action action;
					if ((action = <>9__2) == null)
					{
						action = (<>9__2 = delegate
						{
							window.Topmost = false;
						});
					}
					dispatcher.Invoke(action, new object[0]);
				});
			}
			if (RegistryManager.Instance.ShowKeyControlsOverlay)
			{
				KMManager.ShowOverlayWindow(window, true, false);
			}
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x00061AB8 File Offset: 0x0005FCB8
		public static void SetWindowTaskbarIcon(MainWindow window)
		{
			try
			{
				using (Bitmap bitmap = new Bitmap(RegistryStrings.ProductIconCompletePath))
				{
					new Uri(RegistryStrings.ProductIconCompletePath);
					if (GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && (string.Equals(x.VmName, window.mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification)).Count > 0 && window.IsInNotificationMode)
					{
						if (window.DummyWindow == null)
						{
							window.DummyWindow = new DummyTaskbarWindow(window)
							{
								Icon = new BitmapImage(new Uri(Path.Combine(RegistryManager.Instance.ClientInstallDir, Path.Combine("Assets", "ProductLogo.ico")))),
								Title = Strings.ProductDisplayName,
								TaskbarThumbnailPath = Path.Combine(CustomPictureBox.AssetsDir, "PreviewThumbnail.png"),
								WindowState = WindowState.Minimized
							};
							window.DummyWindow.StateChanged -= BlueStacksUIUtils.DummyWindow_StateChanged;
							window.DummyWindow.StateChanged += BlueStacksUIUtils.DummyWindow_StateChanged;
							window.DummyWindow.Show();
						}
						BlueStacksUIUtils.AddIconOverlay(window.DummyWindow, bitmap, window.mVmName);
					}
				}
			}
			catch (Exception ex)
			{
				string text = "Exception while setting taskbar icon ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x00061C54 File Offset: 0x0005FE54
		private static void DummyWindow_StateChanged(object sender, EventArgs e)
		{
			DummyTaskbarWindow dummyTaskbarWindow = sender as DummyTaskbarWindow;
			if (dummyTaskbarWindow == null || dummyTaskbarWindow.WindowState != WindowState.Minimized)
			{
				BlueStacksUIUtils.HideUnhideParentWindow(false, (sender as DummyTaskbarWindow).ParentWindow);
				Stats.SendCommonClientStatsAsync("notification_mode", "taskbar_bluestacksicon_clicked", "Android", "", "", "");
				DummyTaskbarWindow dummyTaskbarWindow2 = sender as DummyTaskbarWindow;
				if (dummyTaskbarWindow2 == null)
				{
					return;
				}
				dummyTaskbarWindow2.Close();
			}
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x00061CC0 File Offset: 0x0005FEC0
		public static void AddIconOverlay(Window window, Bitmap originalIcon, string vmName)
		{
			try
			{
				SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((GenericNotificationItem x) => !x.IsDeleted && !x.IsRead && (string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification));
				string text = notificationItems.Count.ToString(CultureInfo.InvariantCulture);
				if (notificationItems.Count > 99)
				{
					text = "99+";
				}
				using (Bitmap bitmap = new Bitmap(256, 256))
				{
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						int num = (text.Length + 1) * 45 + 10;
						int num2 = 256 - num + 5;
						if (num < 120)
						{
							num2 = 206 - num / 2;
							num = 120;
						}
						Rectangle rectangle = new Rectangle(256 - num, 111, num, 120);
						int num3 = 120;
						global::System.Drawing.Size size = new global::System.Drawing.Size(num3, num3);
						Rectangle rectangle2 = new Rectangle(rectangle.Location, size);
						using (GraphicsPath graphicsPath = new GraphicsPath())
						{
							graphicsPath.AddArc(rectangle2, 180f, 90f);
							rectangle2.X = rectangle.Right - num3;
							graphicsPath.AddArc(rectangle2, 270f, 90f);
							rectangle2.Y = rectangle.Bottom - num3;
							graphicsPath.AddArc(rectangle2, 0f, 90f);
							rectangle2.X = rectangle.Left;
							graphicsPath.AddArc(rectangle2, 90f, 90f);
							graphicsPath.CloseFigure();
							graphics.FillPath(global::System.Drawing.Brushes.OrangeRed, graphicsPath);
							global::System.Drawing.Image image = bitmap;
							using (Bitmap bitmap2 = new Bitmap(256, 256))
							{
								Graphics graphics2 = Graphics.FromImage(bitmap2);
								Rectangle rectangle3 = new Rectangle(0, 0, 256, 256);
								graphics2.DrawImage(originalIcon, rectangle3);
								graphics2.DrawImage(image, new global::System.Drawing.Point(0, 0));
								using (Font font = new Font("Arial", (float)(70.0 / MainWindow.sScalingFactor), global::System.Drawing.FontStyle.Regular))
								{
									graphics2.DrawString(text, font, global::System.Drawing.Brushes.White, (float)num2, 117f);
									graphics2.Save();
									window.Icon = Imaging.CreateBitmapSourceFromHIcon(bitmap2.GetHicon(), Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
									using (Bitmap bitmap3 = new Bitmap(256, 256))
									{
										using (Graphics graphics3 = Graphics.FromImage(bitmap3))
										{
											graphics3.SmoothingMode = SmoothingMode.AntiAlias;
											Rectangle rectangle4 = new Rectangle(0, 0, 256, 256);
											int num4 = 256;
											global::System.Drawing.Size size2 = new global::System.Drawing.Size(num4, num4);
											Rectangle rectangle5 = new Rectangle(rectangle4.Location, size2);
											using (GraphicsPath graphicsPath2 = new GraphicsPath())
											{
												graphicsPath2.AddArc(rectangle5, 180f, 90f);
												rectangle5.X = rectangle4.Right - num4;
												graphicsPath2.AddArc(rectangle5, 270f, 90f);
												rectangle5.Y = rectangle4.Bottom - num4;
												graphicsPath2.AddArc(rectangle5, 0f, 90f);
												rectangle5.X = rectangle4.Left;
												graphicsPath2.AddArc(rectangle5, 90f, 90f);
												graphicsPath2.CloseFigure();
												graphics3.FillPath(global::System.Drawing.Brushes.OrangeRed, graphicsPath2);
												int num5 = 175 - (text.Length - 1) * 35;
												int num6 = 10 + (text.Length - 1) * 22;
												int num7 = -5;
												if (text.Length == 1)
												{
													num7 = 35;
												}
												if (num5 > 150)
												{
													num5 = 150;
													num6 += 14;
												}
												using (Font font2 = new Font("Arial", (float)((double)num5 / MainWindow.sScalingFactor), global::System.Drawing.FontStyle.Regular))
												{
													graphics3.DrawString(text, font2, global::System.Drawing.Brushes.White, (float)num7, (float)num6);
													TaskbarManager.Instance.SetOverlayIcon(window, Icon.FromHandle(bitmap3.GetHicon()), text);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				string text2 = "error";
				Exception ex2 = ex;
				Logger.Info(text2 + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x000621E0 File Offset: 0x000603E0
		internal void MuteApplication(bool allInstances)
		{
			NativeMethods.waveOutSetVolume(IntPtr.Zero, 0U);
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["allInstances"] = allInstances.ToString(CultureInfo.InvariantCulture);
				dictionary["explicit"] = "true";
				Dictionary<string, string> dictionary2 = dictionary;
				HTTPUtils.SendRequestToEngine("mute", dictionary2, this.ParentWindow.mVmName, 0, null, false, 1, 0, "");
				this.ParentWindow.mCommonHandler.OnVolumeMuted(true);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
			}
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x00062284 File Offset: 0x00060484
		internal void UnmuteApplication(bool allInstances)
		{
			if (!FeatureManager.Instance.IsCustomUIForDMM)
			{
				if (allInstances && this.ParentWindow.EngineInstanceRegistry.IsMuted)
				{
					this.ParentWindow.mSidebar.UpdateMuteAllInstancesCheckbox();
					return;
				}
				if (!allInstances && RegistryManager.Instance.AreAllInstancesMuted)
				{
					RegistryManager.Instance.AreAllInstancesMuted = false;
					foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
					{
						mainWindow.mSidebar.UpdateMuteAllInstancesCheckbox();
					}
				}
			}
			NativeMethods.waveOutSetVolume(IntPtr.Zero, uint.MaxValue);
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["allInstances"] = allInstances.ToString(CultureInfo.InvariantCulture);
				Dictionary<string, string> dictionary2 = dictionary;
				HTTPUtils.SendRequestToEngine("unmute", dictionary2, this.ParentWindow.mVmName, 0, null, false, 1, 0, "");
				this.ParentWindow.mCommonHandler.OnVolumeMuted(false);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
			}
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x0000B3C1 File Offset: 0x000095C1
		internal void SetCurrentVolumeForDMM(int previousVolume, int newVolume)
		{
			new Thread(delegate
			{
				try
				{
					this.ParentWindow.Utils.SetVolumeInFrontendAsync(newVolume);
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						this.ParentWindow.mDmmBottomBar.CurrentVolume = previousVolume;
					}), new object[0]);
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to set volume... Err : " + ex.ToString());
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						this.ParentWindow.mDmmBottomBar.CurrentVolume = previousVolume;
					}), new object[0]);
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0000B3F9 File Offset: 0x000095F9
		internal void SetVolumeLevelFromAndroid(int volume)
		{
			this.CurrentVolumeLevel = volume;
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0000B402 File Offset: 0x00009602
		internal void SetVolumeInFrontendAsync(int newVolume)
		{
			int currentVolumeLevel = this.CurrentVolumeLevel;
			new Thread(delegate
			{
				try
				{
					if (this.ParentWindow.mGuestBootCompleted)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string> { 
						{
							"vol",
							newVolume.ToString(CultureInfo.InvariantCulture)
						} };
						if (Convert.ToBoolean(JArray.Parse(HTTPUtils.SendRequestToEngine("setVolume", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, ""))[0]["success"], CultureInfo.InvariantCulture))
						{
							this.CurrentVolumeLevel = newVolume;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to set volume. Ex: " + ex.ToString());
				}
				if (this.CurrentVolumeLevel == 0)
				{
					this.MuteApplication(false);
				}
				if ((this.ParentWindow.EngineInstanceRegistry.IsMuted || RegistryManager.Instance.AreAllInstancesMuted) && this.CurrentVolumeLevel != 0)
				{
					this.UnmuteApplication(false);
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0000B43A File Offset: 0x0000963A
		internal void GetCurrentVolumeAtBootAsyncAndSetMuteInstancesState()
		{
			new Thread(delegate
			{
				try
				{
					int num = 1000;
					int volume = this.mCurrentVolumeLevel;
					int i = 60;
					while (i > 0)
					{
						i--;
						try
						{
							JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getVolume", null, this.ParentWindow.mVmName, 0, null, true, 1, 0, "bgp64"));
							if (!(jobject["result"].ToString() != "ok"))
							{
								volume = Convert.ToInt32(jobject["volume"].ToString(), CultureInfo.InvariantCulture);
								break;
							}
							Thread.Sleep(num);
						}
						catch (Exception ex)
						{
							Logger.Warning("Failed to get volume from guest: {0}", new object[] { ex.Message });
							Thread.Sleep(num);
						}
					}
					this.CurrentVolumeLevel = volume;
					if (FeatureManager.Instance.IsCustomUIForDMM)
					{
						this.ParentWindow.mDmmBottomBar.Dispatcher.Invoke(new Action(delegate
						{
							this.ParentWindow.mDmmBottomBar.CurrentVolume = volume;
						}), new object[0]);
					}
					if (RegistryManager.Instance.AreAllInstancesMuted)
					{
						this.MuteApplication(true);
					}
				}
				catch (Exception ex2)
				{
					Logger.Error("Failed to get volume: " + ex2.ToString());
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0000B459 File Offset: 0x00009659
		internal static void RestartInstance(string vmName)
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				BlueStacksUIUtils.DictWindows[vmName].RestartInstanceAndPerform(null);
			}
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x000623AC File Offset: 0x000605AC
		internal static void SwitchAndRestartInstanceInAgl(string vmName)
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 1;
				Utils.UpdateValueInBootParams("GlMode", "2", vmName, true, "bgp64");
				BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlMode = 2;
				BlueStacksUIUtils.RestartInstance(vmName);
			}
		}

		// Token: 0x06000F71 RID: 3953 RVA: 0x00062410 File Offset: 0x00060610
		internal static void SwitchAndRestartInstanceInOglAfterRunningGlCheck(string vmName, Action openApp)
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				if (BlueStacksUIUtils.isOglSupported == null)
				{
					BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.ProgressText = "STRING_RUNNING_CHECKS";
					BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.Visibility = Visibility.Visible;
					using (BackgroundWorker backgroundWorker = new BackgroundWorker())
					{
						backgroundWorker.DoWork += BlueStacksUIUtils.Bgw_DoWork;
						backgroundWorker.RunWorkerCompleted += BlueStacksUIUtils.Bgw_RunWorkerCompleted;
						backgroundWorker.RunWorkerAsync(new object[] { vmName, openApp });
						return;
					}
				}
				BlueStacksUIUtils.Bgw_RunWorkerCompleted(new object(), new RunWorkerCompletedEventArgs(new object[] { vmName, openApp }, null, false));
			}
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x000624E4 File Offset: 0x000606E4
		private static void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				return;
			}
			string text = ((object[])e.Result)[0].ToString();
			Action openApp = (Action)((object[])e.Result)[1];
			BlueStacksUIUtils.DictWindows[text].mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
			BlueStacksUIUtils.DictWindows[text].mExitProgressGrid.Visibility = Visibility.Hidden;
			bool? flag = BlueStacksUIUtils.isOglSupported;
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				BlueStacksUIUtils.DictWindows[text].EngineInstanceRegistry.GlRenderMode = 1;
				Utils.UpdateValueInBootParams("GlMode", "1", text, true, "bgp64");
				BlueStacksUIUtils.DictWindows[text].EngineInstanceRegistry.GlMode = 1;
				BlueStacksUIUtils.RestartInstance(text);
				return;
			}
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_OPENGL_NOT_SUPPORTED", "");
			customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_OPENGL_NOTSUPPORTED_BODY", "");
			customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_CONTINUE", ""), delegate(object o, EventArgs args)
			{
				openApp();
			}, null, false, null);
			customMessageWindow.Owner = BlueStacksUIUtils.DictWindows[text];
			BlueStacksUIUtils.DictWindows[text].ShowDimOverlay(null);
			customMessageWindow.ShowDialog();
			BlueStacksUIUtils.DictWindows[text].HideDimOverlay();
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x00062658 File Offset: 0x00060858
		private static void Bgw_DoWork(object sender, DoWorkEventArgs e)
		{
			int num;
			string text;
			string text2;
			string text3;
			BlueStacksUIUtils.isOglSupported = new bool?(Utils.CheckOpenGlSupport(out num, out text, out text2, out text3, RegistryStrings.InstallDir));
			e.Result = e.Argument;
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x00062690 File Offset: 0x00060890
		internal static void SwitchAndRestartInstanceInDx(string vmName)
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 4;
				Utils.UpdateValueInBootParams("GlMode", "1", vmName, true, "bgp64");
				BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlMode = 1;
				BlueStacksUIUtils.RestartInstance(vmName);
			}
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x000626F4 File Offset: 0x000608F4
		internal static void SwitchAndRestartInstanceInAdx(string vmName)
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
			{
				BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 4;
				Utils.UpdateValueInBootParams("GlMode", "2", vmName, true, "bgp64");
				BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlMode = 2;
				BlueStacksUIUtils.RestartInstance(vmName);
			}
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x00062758 File Offset: 0x00060958
		internal void RunAppOrCreateTabButton(string packageName)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (this.ParentWindow.mTopBar.mAppTabButtons.mHomeAppTabButton.IsSelected)
				{
					this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false);
					return;
				}
				AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
				if (appIcon != null)
				{
					this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, false, false, false);
				}
			}), new object[0]);
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x0006279C File Offset: 0x0006099C
		internal void ResetPendingUIOperations()
		{
			try
			{
				if (this.ParentWindow.mGuestBootCompleted)
				{
					this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = string.Empty;
					AppHandler.EventOnAppDisplayed = null;
					if (FeatureManager.Instance.IsCustomUIForDMM)
					{
						this.ParentWindow.mDmmBottomBar.ShowKeyMapPopup(false);
					}
					else
					{
						this.ParentWindow.mSidebar.ShowKeyMapPopup(false);
						this.ParentWindow.mSidebar.ShowOverlayTooltip(false, false);
					}
					this.ParentWindow.mWelcomeTab.mFrontendPopupControl.HideWindow();
					this.ParentWindow.StaticComponents.ShowUninstallButtons(false);
					this.ParentWindow.ClosePopUps();
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Error in ResetPendingUIOperations " + ex.ToString());
			}
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x0000B479 File Offset: 0x00009679
		internal static void AddBootEventHandler(string vmName, EventHandler bootedEvennt)
		{
			if (BlueStacksUIUtils.BootEventsForMIManager.ContainsKey(vmName))
			{
				BlueStacksUIUtils.BootEventsForMIManager[vmName].Add(bootedEvennt);
				return;
			}
			BlueStacksUIUtils.BootEventsForMIManager.Add(vmName, new List<EventHandler> { bootedEvennt });
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x00062874 File Offset: 0x00060A74
		internal static void InvokeMIManagerEvents(string VmName)
		{
			if (BlueStacksUIUtils.BootEventsForMIManager.ContainsKey(VmName))
			{
				foreach (EventHandler eventHandler in BlueStacksUIUtils.BootEventsForMIManager[VmName])
				{
					eventHandler(null, null);
				}
			}
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x000628D8 File Offset: 0x00060AD8
		internal void ShakeWindow()
		{
			if (this.ParentWindow.WindowState == WindowState.Maximized)
			{
				this.ParentWindow.StoryBoard.Begin();
				return;
			}
			int i = 10;
			int num = 5;
			int num2 = 0;
			int num3 = 0;
			while (i > 0)
			{
				if (num3 == 0)
				{
					num2 = num;
				}
				else if (num3 == 1)
				{
					num2 = num * -1;
				}
				else if (num3 == 2)
				{
					num2 = num * -1;
				}
				else if (num3 == 3)
				{
					num2 = num;
				}
				num3++;
				if (num3 == 4)
				{
					num3 = 0;
					i--;
				}
				this.ParentWindow.Left += (double)num2;
				Thread.Sleep(30);
			}
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x00062960 File Offset: 0x00060B60
		internal static void RunInstance(string vmName, bool hiddenMode = false)
		{
			if (!BlueStacksUIUtils.lstCreatingWindows.Contains(vmName))
			{
				if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && !hiddenMode)
				{
					BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke(new Action(delegate
					{
						BlueStacksUIUtils.DictWindows[vmName].ShowWindow(false);
					}), new object[0]);
					return;
				}
				global::System.Windows.Application.Current.Dispatcher.Invoke(new Action(delegate
				{
					BlueStacksUIUtils.lstCreatingWindows.Add(vmName);
					FrontendHandler frontendHandler = new FrontendHandler(vmName);
					MainWindow mainWindow = new MainWindow(vmName, frontendHandler);
					BlueStacksUIUtils.DictWindows[vmName] = mainWindow;
					BlueStacksUIUtils.lstCreatingWindows.Remove(vmName);
					if (!hiddenMode)
					{
						mainWindow.ShowWindow(true);
					}
				}), new object[0]);
			}
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x0000B4B1 File Offset: 0x000096B1
		internal void CheckGuestFailedAsync()
		{
			this.sBootCheckTimer.Elapsed += this.SBootCheckTimer_Elapsed;
			this.sBootCheckTimer.Enabled = true;
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x00062A00 File Offset: 0x00060C00
		internal static void HideAllBlueStacks()
		{
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
			{
				BlueStacksUIUtils.CloseWindows(mainWindow);
				mainWindow.ShowInTaskbar = false;
				mainWindow.Hide();
			}
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0000B4D6 File Offset: 0x000096D6
		private void SBootCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			(sender as global::System.Timers.Timer).Enabled = false;
			if (!this.ParentWindow.mGuestBootCompleted)
			{
				this.SendGuestBootFailureStats("boot timeout exception");
			}
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x00062A68 File Offset: 0x00060C68
		public static string GetFinalRedirectedUrl(string url)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			httpWebRequest.AllowAutoRedirect = true;
			string text = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + text;
			httpWebRequest.Headers.Add("x_oem", RegistryManager.Instance.Oem);
			httpWebRequest.Headers.Set("x_email", RegistryManager.Instance.RegisteredEmail);
			httpWebRequest.Headers.Add("x_guid", RegistryManager.Instance.UserGuid);
			httpWebRequest.Headers.Add("x_prod_ver", RegistryManager.Instance.Version);
			httpWebRequest.Headers.Add("x_home_app_ver", RegistryManager.Instance.ClientVersion);
			string text3;
			try
			{
				string text2 = null;
				using (WebResponse response = httpWebRequest.GetResponse())
				{
					text2 = response.ResponseUri.ToString();
				}
				text3 = text2;
			}
			catch (Exception ex)
			{
				Logger.Error("Error in getting redirected url" + ex.ToString());
				text3 = null;
			}
			return text3;
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x00062B9C File Offset: 0x00060D9C
		internal void SendGuestBootFailureStats(string errorString)
		{
			if (RegistryManager.Instance.IsEngineUpgraded == 1 && RegistryManager.Instance.IsClientFirstLaunch == 1)
			{
				ClientStats.SendClientStatsAsync("update_init", "fail", "engine_activity", "", errorString, "");
			}
			else if (RegistryManager.Instance.IsClientFirstLaunch == 1)
			{
				ClientStats.SendClientStatsAsync("first_init", "fail", "engine_activity", "", errorString, "");
			}
			else
			{
				ClientStats.SendClientStatsAsync("init", "fail", "engine_activity", "", errorString, "");
			}
			this.ParentWindow.HandleRestartPopup();
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x00062C3C File Offset: 0x00060E3C
		internal static bool CheckForMacrAvailable(string packageName)
		{
			string text = Path.Combine(Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper"), packageName + "_macro.cfg");
			string text2 = Path.Combine(Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper\\UserFiles"), packageName + "_macro.cfg");
			return File.Exists(text) || File.Exists(text2);
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x00062CA4 File Offset: 0x00060EA4
		internal static string GetVideoTutorialUrl(string packageName, string videoMode, string selectedSchemeName)
		{
			string text = WebHelper.GetServerHost();
			text = text.Substring(0, text.Length - 4);
			string text2;
			if (GuidanceVideoType.SchemeSpecific.ToString().ToLower(CultureInfo.InvariantCulture).Equals(videoMode, StringComparison.InvariantCulture))
			{
				text2 = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&mode={3}&scheme={4}", new object[] { text, "videoTutorial", packageName, videoMode, selectedSchemeName }));
			}
			else
			{
				text2 = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&mode={3}", new object[] { text, "videoTutorial", packageName, videoMode }));
			}
			if (!RegistryManager.Instance.IgnoreAutoPlayPackageList.Contains(packageName))
			{
				text2 = string.Format(CultureInfo.InvariantCulture, "{0}&autoplay=1", new object[] { text2 });
				List<string> ignoreAutoPlayPackageList = RegistryManager.Instance.IgnoreAutoPlayPackageList;
				ignoreAutoPlayPackageList.Add(packageName);
				RegistryManager.Instance.IgnoreAutoPlayPackageList = ignoreAutoPlayPackageList;
			}
			else
			{
				text2 = string.Format(CultureInfo.InvariantCulture, "{0}&autoplay=0", new object[] { text2 });
			}
			if (!string.IsNullOrEmpty(RegistryManager.Instance.Partner))
			{
				text2 += string.Format(CultureInfo.InvariantCulture, "&partner={0}", new object[] { RegistryManager.Instance.Partner });
			}
			return text2;
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0000B4FC File Offset: 0x000096FC
		internal static string GetOnboardingUrl(string packageName, string source)
		{
			return WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&source={3}", new object[]
			{
				RegistryManager.Instance.Host,
				"bs3/page/onboarding-tutorial",
				packageName,
				source
			}));
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x00062DF0 File Offset: 0x00060FF0
		internal void SetKeyMapping(string packageName, string source)
		{
			string text = Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper\\UserFiles");
			string text2 = Path.Combine(text, string.Format(CultureInfo.InvariantCulture, "{0}.cfg", new object[] { packageName }));
			string text3 = Path.Combine(text, string.Format(CultureInfo.InvariantCulture, "{0}_{1}.cfg", new object[] { packageName, source }));
			try
			{
				File.Copy(text3, text2, true);
			}
			catch (Exception ex)
			{
				Logger.Error("Faield to copy cfgs... Err : " + ex.ToString());
				return;
			}
			this.ParentWindow.mFrontendHandler.RefreshKeyMap(packageName);
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x00062E9C File Offset: 0x0006109C
		internal static void OpenUrl(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch (Win32Exception)
			{
				try
				{
					Process.Start("IExplore.exe", url);
				}
				catch (Exception ex)
				{
					Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex.ToString());
				}
			}
			catch (Exception ex2)
			{
				Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex2.ToString());
			}
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x00062F28 File Offset: 0x00061128
		internal bool IsSufficientRAMAvailable()
		{
			Logger.Info("Checking for physical memory...");
			long num = long.Parse(SystemUtils.GetSysInfo("Select TotalPhysicalMemory from Win32_ComputerSystem"), CultureInfo.InvariantCulture);
			long num2 = 1073741824L;
			return num >= num2;
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x00062F60 File Offset: 0x00061160
		public void SendMessageToAndroidForAffiliate(string pkgName, string source)
		{
			try
			{
				Logger.Info("Sending message to Android for affiliate");
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "action", "com.bluestacks.home.AFFILIATE_HANDLER_HTML" } };
				JObject jobject = new JObject
				{
					{ "success", true },
					{ "app_pkg", pkgName },
					{ "WINDOWS_SOURCE", source }
				};
				dictionary.Add("extras", jobject.ToString(Formatting.None, new JsonConverter[0]));
				HTTPUtils.SendRequestToGuest("customStartService", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't send message to Adnroid: " + ex.ToString());
			}
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x0006302C File Offset: 0x0006122C
		public void AppendUrlWithCommonParamsAndOpenTab(string url, string title, string imagePath, string tabKey = "")
		{
			try
			{
				url = WebHelper.GetUrlWithParams(url);
				if (new Uri(url).Host.Contains("bluestacks", StringComparison.InvariantCultureIgnoreCase))
				{
					string registeredEmail = RegistryManager.Instance.RegisteredEmail;
					string token = RegistryManager.Instance.Token;
					if (string.IsNullOrEmpty(registeredEmail))
					{
						Logger.Warning("User email not found. Not opening webpage.");
					}
					if (string.IsNullOrEmpty(token))
					{
						Logger.Warning("User token not found. Not opening webpage.");
					}
				}
				this.ParentWindow.Dispatcher.Invoke(new Action(delegate
				{
					this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(url, title, imagePath, true, tabKey, false);
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception when parsing uri for opening in webtab " + ex.ToString());
			}
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x00063118 File Offset: 0x00061318
		public void ApplyTheme(string themeName)
		{
			BlueStacksUIColorManager.ReloadAppliedTheme(themeName);
			Publisher.PublishMessage(BrowserControlTags.themeChange, this.ParentWindow.mVmName, new JObject(new JProperty("Theme", themeName)));
			BlueStacksUIUtils.RefreshAppCenterUrl();
			BlueStacksUIUtils.RefreshHtmlSidePanelUrl();
			this.ParentWindow.mCommonHandler.SetCustomCursorForApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
			ClientStats.SendMiscellaneousStatsAsync("SkinChangedStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.ClientThemeName, null, null, null, null);
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x000631B4 File Offset: 0x000613B4
		public void RestoreWallpaperImageForAllVms()
		{
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
			{
				mainWindow.mWelcomeTab.mHomeAppManager.RestoreWallpaper();
			}
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x00063218 File Offset: 0x00061418
		public void ChooseWallpaper()
		{
			try
			{
				using (OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Title = LocaleStrings.GetLocalizedString("STRING_CHANGE_WALLPAPER", ""),
					RestoreDirectory = true,
					DefaultExt = ".jpg",
					Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
				})
				{
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						Bitmap bitmap = new Bitmap(openFileDialog.FileName);
						bitmap.Save(HomeAppManager.BackgroundImagePath);
						bitmap.Dispose();
						foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
						{
							mainWindow.mWelcomeTab.mHomeAppManager.ApplyWallpaper();
						}
						ClientStats.SendMiscellaneousStatsAsync("WallPaperStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Premium", "Changed_Wallpaper", null, null, null, null);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in changing wallpaper:" + ex.ToString());
				global::System.Windows.MessageBox.Show("Cannot change wallpaper.Please try again.", "Error");
			}
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x00063354 File Offset: 0x00061554
		internal static string GetAppCenterUrl(string tabId)
		{
			string text = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/appcenter-v2");
			text += "&theme=";
			text += RegistryManager.ClientThemeName;
			text += "&naked=1";
			if (!string.IsNullOrEmpty(tabId))
			{
				text += "&tabid=";
				text += tabId;
			}
			return text;
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0000B535 File Offset: 0x00009735
		internal static string GetHtmlSidePanelUrl()
		{
			return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/myapps-sidepanel") + "&theme=" + RegistryManager.ClientThemeName;
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x000633B8 File Offset: 0x000615B8
		internal string GetHtmlHomeUrl()
		{
			string text = string.Concat(new string[]
			{
				WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/bgp-home-html"),
				"&theme=",
				RegistryManager.ClientThemeName,
				"&vmId=",
				Utils.GetVmIdFromVmName(this.ParentWindow.mVmName),
				"&vmName=",
				this.ParentWindow.mVmName,
				"&firstLaunchedVmName=",
				Strings.CurrentDefaultVmName,
				"&oem=",
				RegistryManager.Instance.Oem
			});
			if (!string.IsNullOrEmpty(Opt.Instance.Json))
			{
				JObject jobject = JObject.Parse(Opt.Instance.Json);
				if (jobject["fle_pkg"] != null)
				{
					text = text + "&flePackageName=" + jobject["fle_pkg"].ToString().Trim();
				}
				if (jobject["campaign_id"] != null)
				{
					text = text + "&campaignId=" + jobject["campaign_id"].ToString().Trim();
				}
				if (jobject["source"] != null)
				{
					text = text + "&source=" + jobject["source"].ToString().Trim();
				}
			}
			return text;
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x00063500 File Offset: 0x00061700
		internal void RefreshHtmlHomeUrl()
		{
			try
			{
				this.ParentWindow.mWelcomeTab.ReInitHtmlHome();
			}
			catch (Exception ex)
			{
				Logger.Error("Error while refreshing side html panel for vmname: {0} and exception is: {1}", new object[]
				{
					this.ParentWindow.mVmName,
					ex
				});
			}
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x0000B55F File Offset: 0x0000975F
		internal static string GetGiftTabUrl()
		{
			return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/gift") + "&theme=" + RegistryManager.ClientThemeName;
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0000B589 File Offset: 0x00009789
		internal static string GetPikaWorldUrl()
		{
			return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/pikaworld") + "&naked=1";
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x00063554 File Offset: 0x00061754
		internal void SwitchProfile(string vmName, string pcode)
		{
			try
			{
				Dictionary<string, string> data = new Dictionary<string, string>();
				JObject jobject = new JObject
				{
					{ "pcode", pcode },
					{ "createcustomprofile", "false" }
				};
				data.Add("data", jobject.ToString(Formatting.None, new JsonConverter[0]));
				BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.ProgressText = "STRING_SWITCHING_PROFILE";
				BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.Visibility = Visibility.Visible;
				Action <>9__1;
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					JObject jobject2 = new JObject();
					jobject2["pcode"] = Utils.GetValueInBootParams("pcode", this.ParentWindow.mVmName, "", "bgp64");
					JObject jobject3 = jobject2;
					string text = HTTPUtils.SendRequestToGuest("changeDeviceProfile", data, vmName, 0, null, false, 3, 60000, "bgp64");
					Logger.Info("Response for ChangeDeviceProfile: " + text);
					JObject jobject4 = JObject.Parse(text);
					Dispatcher dispatcher = this.ParentWindow.Dispatcher;
					Action action;
					if ((action = <>9__1) == null)
					{
						action = (<>9__1 = delegate
						{
							this.ParentWindow.mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
							this.ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
						});
					}
					dispatcher.Invoke(action, new object[0]);
					JObject jobject5 = new JObject();
					jobject5["pcode"] = pcode;
					JObject jobject6 = jobject5;
					if (jobject4["result"].ToString() == "ok")
					{
						Logger.Info("Successfully updated Device Profile.");
						Utils.UpdateValueInBootParams("pcode", pcode, this.ParentWindow.mVmName, false, "bgp64");
						this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow, LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_UPDATED", ""), 1.3, false);
						ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "success", JsonConvert.SerializeObject(jobject6), JsonConvert.SerializeObject(jobject3), RegistryManager.Instance.Version, "GRM", null);
						return;
					}
					Logger.Warning("DeviceProfile Update failed in android");
					this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow, LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_FAILED", ""), 1.3, false);
					ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "failed", JsonConvert.SerializeObject(jobject6), JsonConvert.SerializeObject(jobject3), RegistryManager.Instance.Version, "GRM", null);
				});
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SwitchProfileAndRestart: {0}", new object[] { ex });
			}
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x0000B5A9 File Offset: 0x000097A9
		internal static string GetHelpCenterUrl()
		{
			return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/feedback");
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x00063668 File Offset: 0x00061868
		internal static void RefreshHtmlSidePanelUrl()
		{
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
			{
				try
				{
					mainWindow.mWelcomeTab.mHomeAppManager.ReinitHtmlSidePanel();
				}
				catch (Exception ex)
				{
					Logger.Error("Error while refreshing side html panel for vmname: {0} and exception is: {1}", new object[] { mainWindow.mVmName, ex });
				}
			}
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x000636F8 File Offset: 0x000618F8
		internal static void RefreshAppCenterUrl()
		{
			if (BlueStacksUIUtils.DictWindows.ContainsKey(Strings.CurrentDefaultVmName))
			{
				MainWindow mainWindow = BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName];
				AppTabButton tab = mainWindow.mTopBar.mAppTabButtons.GetTab("appcenter");
				if (tab != null && tab.GetBrowserControl() != null)
				{
					tab.GetBrowserControl().NavigateTo(BlueStacksUIUtils.GetAppCenterUrl(""));
				}
				AppTabButton tab2 = mainWindow.mTopBar.mAppTabButtons.GetTab("gift");
				if (tab2 != null && tab2.GetBrowserControl() != null)
				{
					tab2.GetBrowserControl().NavigateTo(BlueStacksUIUtils.GetGiftTabUrl());
				}
			}
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0000B5BF File Offset: 0x000097BF
		internal static string GetMacroCommunityUrl(string currentAppPackage)
		{
			return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/macro-share") + "&pkg=" + currentAppPackage;
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0006378C File Offset: 0x0006198C
		internal bool IsRequiredFreeRAMAvailable()
		{
			try
			{
				ulong num = 1048576UL;
				ulong availablePhysicalMemory = new ComputerInfo().AvailablePhysicalMemory;
				int num2 = this.ParentWindow.EngineInstanceRegistry.Memory + 100;
				if (num2 > 2148)
				{
					num2 = 2148;
				}
				ulong num3 = (ulong)((long)num2 * (long)num);
				if (availablePhysicalMemory < num3)
				{
					Logger.Warning("Available physical memory is less than required. {0} < {1}", new object[]
					{
						availablePhysicalMemory / num,
						num2
					});
					return false;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("An error occurred while finding free RAM");
				Logger.Error(ex.ToString());
			}
			return true;
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0006382C File Offset: 0x00061A2C
		public bool CheckQuitPopupLocal()
		{
			try
			{
				if (!this.ParentWindow.mGuestBootCompleted)
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						QuitPopupControl quitPopupControl = new QuitPopupControl(this.ParentWindow);
						string text = "exit_popup_boot";
						quitPopupControl.CurrentPopupTag = text;
						BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_TROUBLE_STARTING_BLUESTACKS", "");
						BlueStacksUIBinding.Bind(quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
						quitPopupControl.AddQuitActionItem(QuitActionItem.StuckAtBoot);
						quitPopupControl.AddQuitActionItem(QuitActionItem.SlowPerformance);
						quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
						quitPopupControl.CloseBlueStacksButton.PreviewMouseUp += new MouseButtonEventHandler(this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
						this.ParentWindow.HideDimOverlay();
						this.ParentWindow.ShowDimOverlay(quitPopupControl);
						ClientStats.SendLocalQuitPopupStatsAsync(text, "popup_shown");
					}), new object[0]);
					return true;
				}
				if (!RegistryManager.Instance.Guest[this.ParentWindow.mVmName].IsGoogleSigninDone && string.Equals(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName, "com.android.vending", StringComparison.InvariantCultureIgnoreCase))
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						QuitPopupControl quitPopupControl2 = new QuitPopupControl(this.ParentWindow);
						string text2 = "exit_popup_ots";
						quitPopupControl2.CurrentPopupTag = text2;
						BlueStacksUIBinding.Bind(quitPopupControl2.TitleTextBlock, "STRING_YOU_ARE_ONE_STEP_AWAY", "");
						BlueStacksUIBinding.Bind(quitPopupControl2.mCloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
						quitPopupControl2.AddQuitActionItem(QuitActionItem.WhyGoogleAccount);
						quitPopupControl2.AddQuitActionItem(QuitActionItem.TroubleSigningIn);
						quitPopupControl2.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
						quitPopupControl2.CloseBlueStacksButton.PreviewMouseUp += new MouseButtonEventHandler(this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
						this.ParentWindow.HideDimOverlay();
						this.ParentWindow.ShowDimOverlay(quitPopupControl2);
						ClientStats.SendLocalQuitPopupStatsAsync(text2, "popup_shown");
					}), new object[0]);
					return true;
				}
				if (this.ParentWindow.mVmName == "Android" && RegistryManager.Instance.FirstAppLaunchState != AppLaunchState.Launched)
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						QuitPopupControl quitPopupControl3 = new QuitPopupControl(this.ParentWindow);
						string text3 = "exit_popup_no_app";
						quitPopupControl3.CurrentPopupTag = text3;
						BlueStacksUIBinding.Bind(quitPopupControl3.TitleTextBlock, "STRING_HAVING_TROUBLE_STARTING_GAME", "");
						BlueStacksUIBinding.Bind(quitPopupControl3.ReturnBlueStacksButton, "STRING_RETURN_BLUESTACKS");
						BlueStacksUIBinding.Bind(quitPopupControl3.CloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
						quitPopupControl3.AddQuitActionItem(QuitActionItem.UnsureWhereStart);
						quitPopupControl3.AddQuitActionItem(QuitActionItem.IssueInstallingGame);
						quitPopupControl3.AddQuitActionItem(QuitActionItem.FacingOtherTroubles);
						quitPopupControl3.CloseBlueStacksButton.PreviewMouseUp += new MouseButtonEventHandler(this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
						this.ParentWindow.HideDimOverlay();
						this.ParentWindow.ShowDimOverlay(quitPopupControl3);
						ClientStats.SendLocalQuitPopupStatsAsync(text3, "popup_shown");
					}), new object[0]);
					return true;
				}
				string package;
				if (!RegistryManager.Instance.IsNotificationModeAlwaysOn && this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose && this.ParentWindow.EngineInstanceRegistry.NotificationModePopupShownCount < RegistryManager.Instance.NotificationModeCounter && this.CheckIfNotificationModePopupToBeShown(this.ParentWindow, out package) && string.Compare("Android", this.ParentWindow.mVmName, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						this.ParentWindow.HideDimOverlay();
						NotificationModeExitPopup notificationModeExitPopup = new NotificationModeExitPopup(this.ParentWindow, package);
						new ContainerWindow(this.ParentWindow, notificationModeExitPopup, notificationModeExitPopup.Width, notificationModeExitPopup.Height, false, true, false, 12.0, (SolidColorBrush)new BrushConverter().ConvertFrom("#4CFFFFFF"));
					}), new object[0]);
					return true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to show local quit popup. " + ex.ToString());
			}
			return false;
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x00063A04 File Offset: 0x00061C04
		private bool CheckIfNotificationModePopupToBeShown(MainWindow window, out string packageName)
		{
			packageName = window.StaticComponents.mSelectedTabButton.PackageName;
			new JsonParser(window.mVmName);
			if (window.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab)
			{
				bool flag = true;
				bool flag2 = true;
				bool? flag4;
				foreach (string text in window.mTopBar.mAppTabButtons.mDictTabs.Keys)
				{
					NotificationModeInfo gameNotificationAppPackages = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages;
					bool? flag3;
					if (gameNotificationAppPackages == null)
					{
						flag3 = null;
					}
					else
					{
						AppPackageListObject notificationModeAppPackages = gameNotificationAppPackages.NotificationModeAppPackages;
						flag3 = ((notificationModeAppPackages != null) ? new bool?(notificationModeAppPackages.IsPackageAvailable(text)) : null);
					}
					flag4 = flag3;
					bool value = flag4.Value;
					flag = flag && value;
					flag2 = flag2 && !value;
				}
				if (flag)
				{
					return true;
				}
				if (flag2)
				{
					return false;
				}
				NotificationModeInfo gameNotificationAppPackages2 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages;
				bool? flag5;
				if (gameNotificationAppPackages2 == null)
				{
					flag5 = null;
				}
				else
				{
					AppPackageListObject notificationModeAppPackages2 = gameNotificationAppPackages2.NotificationModeAppPackages;
					flag5 = ((notificationModeAppPackages2 != null) ? new bool?(notificationModeAppPackages2.IsPackageAvailable(packageName)) : null);
				}
				flag4 = flag5;
				return flag4.Value;
			}
			else
			{
				bool flag6 = true;
				List<string> list = Utils.GetInstalledPackagesFromAppsJSon(this.ParentWindow.mVmName).Split(new char[] { ',' }).ToList<string>();
				foreach (string text2 in list)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						NotificationModeInfo gameNotificationAppPackages3 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages;
						bool? flag7;
						if (gameNotificationAppPackages3 == null)
						{
							flag7 = null;
						}
						else
						{
							AppPackageListObject notificationModeAppPackages3 = gameNotificationAppPackages3.NotificationModeAppPackages;
							flag7 = ((notificationModeAppPackages3 != null) ? new bool?(notificationModeAppPackages3.IsPackageAvailable(text2)) : null);
						}
						bool? flag4 = flag7;
						bool value2 = flag4.Value;
						flag6 = flag6 && !value2;
					}
				}
				if (flag6)
				{
					return false;
				}
				packageName = window.EngineInstanceRegistry.LastNotificationEnabledAppLaunched;
				if (string.IsNullOrEmpty(packageName) || !list.Contains(packageName))
				{
					foreach (string text3 in list)
					{
						NotificationModeInfo gameNotificationAppPackages4 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages;
						bool? flag8;
						if (gameNotificationAppPackages4 == null)
						{
							flag8 = null;
						}
						else
						{
							AppPackageListObject notificationModeAppPackages4 = gameNotificationAppPackages4.NotificationModeAppPackages;
							flag8 = ((notificationModeAppPackages4 != null) ? new bool?(notificationModeAppPackages4.IsPackageAvailable(text3)) : null);
						}
						bool? flag4 = flag8;
						if (flag4.Value)
						{
							packageName = text3;
							break;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x00063CC4 File Offset: 0x00061EC4
		public void OpenPikaAccountPage()
		{
			if (this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted && !string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail))
			{
				string text = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/bluestacks_account?extra=section:pika");
				text += "&email=";
				text += RegistryManager.Instance.RegisteredEmail;
				text += "&token=";
				text += RegistryManager.Instance.Token;
				this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(text, "STRING_ACCOUNT", "account_tab", true, "account_tab", true);
			}
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x00063D70 File Offset: 0x00061F70
		internal void HandleApplicationBrowserClick(string clickActionValue, string title, string key, bool paramsOnlyActionValue = false, string customImageName = "")
		{
			title = title.Trim();
			string text = "cef_tab";
			if (key != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(key);
				if (num > 2737665056U)
				{
					if (num <= 3655924613U)
					{
						if (num != 2938085890U)
						{
							if (num != 3655924613U)
							{
								goto IL_021D;
							}
							if (!(key == "appcenter"))
							{
								goto IL_021D;
							}
							goto IL_0138;
						}
						else if (!(key == "pikaworld"))
						{
							goto IL_021D;
						}
					}
					else if (num != 3727264231U)
					{
						if (num != 3975455884U)
						{
							goto IL_021D;
						}
						if (!(key == "MAPS_TEXT"))
						{
							goto IL_021D;
						}
					}
					else
					{
						if (!(key == "GIFT_TEXT"))
						{
							goto IL_021D;
						}
						goto IL_0177;
					}
					clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetPikaWorldUrl(), clickActionValue, paramsOnlyActionValue);
					if (string.IsNullOrEmpty(title))
					{
						title = LocaleStrings.GetLocalizedString("STRING_MAPS", "");
					}
					key = "pikaworld";
					text = "pikaworld";
					goto IL_021D;
				}
				if (num <= 2322801903U)
				{
					if (num != 255579182U)
					{
						if (num != 2322801903U)
						{
							goto IL_021D;
						}
						if (!(key == "gift"))
						{
							goto IL_021D;
						}
						goto IL_0177;
					}
					else if (!(key == "APP_CENTER_TEXT"))
					{
						goto IL_021D;
					}
				}
				else if (num != 2333368623U)
				{
					if (num != 2737665056U)
					{
						goto IL_021D;
					}
					if (!(key == "FEEDBACK_TEXT"))
					{
						goto IL_021D;
					}
					clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetHelpCenterUrl(), clickActionValue, paramsOnlyActionValue);
					Process.Start(clickActionValue);
					return;
				}
				else
				{
					if (!(key == "preregistration"))
					{
						goto IL_021D;
					}
					if (string.IsNullOrEmpty(title))
					{
						title = LocaleStrings.GetLocalizedString("STRING_PREREGISTER", "");
					}
					text = "preregistration";
					goto IL_021D;
				}
				IL_0138:
				clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetAppCenterUrl(""), clickActionValue, paramsOnlyActionValue);
				if (string.IsNullOrEmpty(title))
				{
					title = LocaleStrings.GetLocalizedString("STRING_APP_CENTER", "");
				}
				key = "appcenter";
				text = "appcenter";
				goto IL_021D;
				IL_0177:
				clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetGiftTabUrl(), clickActionValue, paramsOnlyActionValue);
				if (string.IsNullOrEmpty(title))
				{
					title = LocaleStrings.GetLocalizedString("STRING_GIFT", "");
				}
				key = "gift";
				text = "gift";
			}
			IL_021D:
			if (!string.IsNullOrEmpty(customImageName))
			{
				text = customImageName;
			}
			this.ParentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(clickActionValue, title, text, key);
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x00063FBC File Offset: 0x000621BC
		private static string GetImagePath(Dictionary<string, string> payload, string customImageName = "")
		{
			if (!string.IsNullOrEmpty(customImageName))
			{
				return customImageName;
			}
			if (payload.ContainsKey("icon_path"))
			{
				return payload["icon_path"];
			}
			if (payload.ContainsKey("click_action_app_icon_id") && File.Exists(Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + payload["click_action_app_icon_id"])))
			{
				return Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + payload["click_action_app_icon_id"]);
			}
			return "";
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x00064044 File Offset: 0x00062244
		public void HandleGenericActionFromDictionary(Dictionary<string, string> payload, string source, string customImageName = "")
		{
			try
			{
				if (payload.ContainsKey("click_generic_action"))
				{
					GenericAction genericAction = EnumHelper.Parse<GenericAction>(payload["click_generic_action"], GenericAction.None);
					if (genericAction <= GenericAction.SettingsMenu)
					{
						if (genericAction <= GenericAction.UserBrowser)
						{
							switch (genericAction)
							{
							case GenericAction.InstallPlay:
								if (!this.ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
								{
									this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
								}
								this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(payload["click_action_packagename"], PlayStoreAction.OpenApp);
								goto IL_04FF;
							case GenericAction.InstallCDN:
								if (!this.ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
								{
									this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
								}
								this.ParentWindow.mAppInstaller.DownloadAndInstallApp(string.Empty, payload["click_action_title"], payload["click_action_value"], payload["click_action_packagename"], false, true, "");
								goto IL_04FF;
							case (GenericAction)3:
								break;
							case GenericAction.ApplicationBrowser:
								this.HandleApplicationBrowserClick(payload["click_action_value"], payload["click_action_title"], payload.ContainsKey("click_action_key") ? payload["click_action_key"] : "", false, BlueStacksUIUtils.GetImagePath(payload, customImageName));
								goto IL_04FF;
							default:
								if (genericAction == GenericAction.UserBrowser)
								{
									BlueStacksUIUtils.OpenUrl(payload["click_action_value"]);
									goto IL_04FF;
								}
								break;
							}
						}
						else if (genericAction != GenericAction.HomeAppTab)
						{
							if (genericAction == GenericAction.SettingsMenu)
							{
								this.ParentWindow.Dispatcher.Invoke(new Action(delegate
								{
									MainWindow.OpenSettingsWindow(this.ParentWindow, payload["click_action_value"]);
								}), new object[0]);
								goto IL_04FF;
							}
						}
						else
						{
							this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
							if (string.Compare(payload["click_action_value"], "my_app_text", StringComparison.OrdinalIgnoreCase) == 0)
							{
								goto IL_04FF;
							}
							if (payload.ContainsKey("query_params") && !string.IsNullOrEmpty(payload["query_params"].Trim()))
							{
								this.HandleApplicationBrowserClick(payload["query_params"], "", payload["click_action_value"], true, customImageName);
								goto IL_04FF;
							}
							this.HandleApplicationBrowserClick("", "", payload["click_action_value"], true, customImageName);
							goto IL_04FF;
						}
					}
					else if (genericAction <= GenericAction.OpenSystemApp)
					{
						if (genericAction != GenericAction.KeyBasedPopup)
						{
							if (genericAction == GenericAction.OpenSystemApp)
							{
								this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(payload["click_action_title"], payload["click_action_packagename"], payload["click_action_app_activity"], BlueStacksUIUtils.GetImagePath(payload, customImageName), false, true, false);
								this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = payload["click_action_packagename"];
								this.ParentWindow.mAppHandler.SendRunAppRequestAsync(payload["click_action_packagename"], payload["click_action_app_activity"], false);
								goto IL_04FF;
							}
						}
						else
						{
							string text = payload["click_action_key"].Trim().ToLower(CultureInfo.InvariantCulture);
							if (text == null)
							{
								goto IL_04FF;
							}
							if (text == "instance_manager")
							{
								BlueStacksUIUtils.LaunchMultiInstanceManager();
								goto IL_04FF;
							}
							if (!(text == "macro_recorder"))
							{
								goto IL_04FF;
							}
							this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
							goto IL_04FF;
						}
					}
					else
					{
						if (genericAction == GenericAction.PopupBrowser)
						{
							this.ParentWindow.mCommonHandler.OpenBrowserInPopup(payload);
							goto IL_04FF;
						}
						if (genericAction == GenericAction.QuickLaunch)
						{
							BlueStacksUIUtils.LaunchAllInstancesAndArrange();
							goto IL_04FF;
						}
						if (genericAction == GenericAction.InstallPlayPopup)
						{
							if (!this.ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
							{
								this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
							}
							this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(payload["click_action_packagename"], payload["click_action_title"], PlayStoreAction.OpenApp, true);
							goto IL_04FF;
						}
					}
					Logger.Warning("Unknown case {0}", new object[] { payload["click_generic_action"] });
				}
				IL_04FF:;
			}
			catch (Exception ex)
			{
				Logger.Error(string.Concat(new string[]
				{
					"Exception on handling click event for payload ",
					payload.ToDebugString<string, string>(),
					Environment.NewLine,
					"Exception: ",
					ex.ToString()
				}));
			}
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x000645B0 File Offset: 0x000627B0
		internal bool CheckQuitPopupFromCloud(string appPackage = "")
		{
			bool flag;
			try
			{
				Logger.Info("IsQuitPopupNotificationReceived status: " + this.ParentWindow.IsQuitPopupNotficationReceived.ToString());
				if (!RegistryManager.Instance.ShowGamingSummary || !this.ParentWindow.IsQuitPopupNotficationReceived || (string.IsNullOrEmpty(appPackage) && !this.ParentWindow.mQuitPopupBrowserControl.ShowOnQuit) || (!string.Equals(this.ParentWindow.mQuitPopupBrowserControl.PackageName, appPackage, StringComparison.InvariantCulture) && !string.Equals(this.ParentWindow.mQuitPopupBrowserControl.PackageName, "*", StringComparison.InvariantCulture) && !string.IsNullOrEmpty(appPackage)))
				{
					flag = false;
				}
				else
				{
					this.ParentWindow.IsQuitPopupNotficationReceived = false;
					if (this.ParentWindow.mQuitPopupBrowserControl.IsForceReload)
					{
						string text = this.ParentWindow.mQuitPopupBrowserControl.QuitPopupUrl;
						string text2 = JsonConvert.SerializeObject(AppUsageTimer.GetRealtimeDictionary()[this.ParentWindow.mVmName], Formatting.None);
						string text3 = "usage_data=" + text2;
						text = HTTPUtils.MergeQueryParams(text, text3, true);
						text = WebHelper.GetUrlWithParams(text);
						this.ParentWindow.mQuitPopupBrowserControl.RefreshBrowserUrl(text);
					}
					if (!string.IsNullOrEmpty(this.ParentWindow.mQuitPopupBrowserControl.QuitPopupUrl))
					{
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							this.ParentWindow.HideDimOverlay();
							this.ParentWindow.mQuitPopupBrowserControl.Init(appPackage);
						}), new object[0]);
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to show quit popup. " + ex.ToString());
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0000B5E0 File Offset: 0x000097E0
		internal static void LaunchMultiInstanceManager()
		{
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				ProcessUtils.GetProcessObject(Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"), null, false).Start();
			}
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0006476C File Offset: 0x0006296C
		internal static void RemoveChildFromParent(UIElement child)
		{
			DependencyObject parent = VisualTreeHelper.GetParent(child);
			global::System.Windows.Controls.Panel panel = parent as global::System.Windows.Controls.Panel;
			if (panel != null)
			{
				panel.Children.Remove(child);
			}
			ContentControl contentControl = parent as ContentControl;
			if (contentControl != null)
			{
				contentControl.Content = null;
			}
			Decorator decorator = parent as Decorator;
			if (decorator != null)
			{
				decorator.Child = null;
			}
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x000647B8 File Offset: 0x000629B8
		public static void UpdateLocale(string locale, string vmToIgnore = "")
		{
			new List<string>();
			using (List<string>.Enumerator enumerator = RegistryManager.Instance.VmList.ToList<string>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string vmName = enumerator.Current;
					try
					{
						if (!RegistryManager.Instance.Guest.ContainsKey(vmName))
						{
							InstanceRegistry instanceRegistry = new InstanceRegistry(vmName, "bgp64");
							RegistryManager.Instance.Guest.Add(vmName, instanceRegistry);
						}
						if (RegistryManager.Instance.UserSelectedLocale != RegistryManager.Instance.Guest[vmName].Locale)
						{
							RegistryManager.Instance.Guest[vmName].Locale = locale;
							Utils.UpdateValueInBootParams("LANG", locale, vmName, false, "bgp64");
							if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && string.Compare(vmName, vmToIgnore.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
							{
								string cmd = string.Format(CultureInfo.InvariantCulture, "setlocale {0}", new object[] { locale });
								new Thread(delegate
								{
									if (VmCmdHandler.RunCommand(cmd, vmName) == null)
									{
										Logger.Error("Set locale did not work for vm " + vmName);
									}
								})
								{
									IsBackground = true
								}.Start();
							}
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to change locale for vm : " + vmName);
						Logger.Error(ex.ToString());
					}
				}
			}
			HTTPUtils.SendRequestToAgentAsync("reinitlocalization", null, "Android", 0, null, false, 1, 0, "bgp64");
			LocaleStrings.InitLocalization(null, "Android", false);
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x00064994 File Offset: 0x00062B94
		internal static bool SendBluestacksLoginRequest(string vmName)
		{
			bool flag = false;
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "action", "com.bluestacks.account.RETRY_BLUESTACKS_LOGIN" } };
				JObject jobject = new JObject { { "windows", "true" } };
				dictionary.Add("extras", jobject.ToString(Formatting.None, new JsonConverter[0]));
				Logger.Info("Sending bluestacks login request");
				HTTPUtils.SendRequestToGuest("customStartService".ToLower(CultureInfo.InvariantCulture), dictionary, vmName, 500, null, false, 1, 0, "bgp64");
				flag = true;
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't send request to guest for login. Ex: {0}", new object[] { ex.Message });
			}
			return flag;
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x0000B60A File Offset: 0x0000980A
		internal static bool CheckIfMacroScriptBookmarked(string fileName)
		{
			return RegistryManager.Instance.BookmarkedScriptList.Contains(fileName);
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x0000B621 File Offset: 0x00009821
		internal static string GetMacroPlaybackEventName(string vmname)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[] { "MacroPlayBack", vmname });
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x00064A4C File Offset: 0x00062C4C
		internal void HandleLaunchPlay(string package)
		{
			object obj = BlueStacksUIUtils.mLaunchPlaySyncObj;
			lock (obj)
			{
				int i = 180000;
				while (i > 0)
				{
					i--;
					if (this.ParentWindow.mEnableLaunchPlayForNCSoft || (!FeatureManager.Instance.IsCustomUIForNCSoft && this.ParentWindow.mGuestBootCompleted))
					{
						break;
					}
					Thread.Sleep(1000);
				}
				if (i > 0)
				{
					HTTPUtils.SendRequestToGuest(string.Format(CultureInfo.InvariantCulture, "launchplay?pkgname={0}", new object[] { package }), null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
				}
			}
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x00064AF8 File Offset: 0x00062CF8
		internal void VolumeDownHandler()
		{
			if (this.CurrentVolumeLevel == 0)
			{
				return;
			}
			int num = this.CurrentVolumeLevel - 7;
			if (num <= 0)
			{
				num = 0;
			}
			this.SetVolumeInFrontendAsync(num);
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x00064B24 File Offset: 0x00062D24
		internal void VolumeUpHandler()
		{
			if (this.CurrentVolumeLevel >= 100)
			{
				return;
			}
			int num = this.CurrentVolumeLevel + 7;
			if (num >= 100)
			{
				num = 100;
			}
			this.SetVolumeInFrontendAsync(num);
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0000B644 File Offset: 0x00009844
		internal void ToggleTopBarSidebarEnabled(bool isEnabled)
		{
			this.ParentWindow.TopBar.IsEnabled = isEnabled;
			this.ParentWindow.mSidebar.IsEnabled = isEnabled;
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x00064B54 File Offset: 0x00062D54
		internal static void SendGamepadStatusToBrowsers(bool status)
		{
			try
			{
				object[] array = new object[] { "" };
				array[0] = status.ToString(CultureInfo.InvariantCulture);
				foreach (BrowserControl browserControl in BrowserControl.sAllBrowserControls)
				{
					try
					{
						if (browserControl != null && browserControl.CefBrowser != null)
						{
							browserControl.CefBrowser.CallJs("toggleGamePadSupport", array);
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Exception in sending gamepad status to browser:" + browserControl.mUrl + Environment.NewLine + ex.ToString());
					}
				}
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception in sending gamepad status to browser:" + ex2.ToString());
			}
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x00064C34 File Offset: 0x00062E34
		internal static double GetDefaultHeight()
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				return SystemParameters.MaximizedPrimaryScreenHeight * 0.6 + 94.0;
			}
			return SystemParameters.MaximizedPrimaryScreenHeight * 0.75 + 94.0;
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0000B668 File Offset: 0x00009868
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.sBootCheckTimer != null)
				{
					this.sBootCheckTimer.Elapsed -= this.SBootCheckTimer_Elapsed;
					this.sBootCheckTimer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x00064C80 File Offset: 0x00062E80
		~BlueStacksUIUtils()
		{
			this.Dispose(false);
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0000B6A5 File Offset: 0x000098A5
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x00064CB0 File Offset: 0x00062EB0
		internal static Dictionary<string, string> GetEngineSettingsData(string vmName)
		{
			return new Dictionary<string, string>
			{
				{
					"cpu",
					RegistryManager.Instance.Guest[vmName].VCPUs.ToString(CultureInfo.InvariantCulture)
				},
				{
					"ram",
					RegistryManager.Instance.Guest[vmName].Memory.ToString(CultureInfo.InvariantCulture)
				},
				{
					"glMode",
					RegistryManager.Instance.Guest[vmName].GlMode.ToString(CultureInfo.InvariantCulture)
				},
				{
					"glRenderMode",
					RegistryManager.Instance.Guest[vmName].GlRenderMode.ToString(CultureInfo.InvariantCulture)
				},
				{
					"gpu",
					RegistryManager.Instance.AvailableGPUDetails
				}
			};
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x00064D8C File Offset: 0x00062F8C
		internal static Dictionary<string, string> GetResolutionData()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int guestWidth = RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestWidth;
			int guestHeight = RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestHeight;
			string text = Convert.ToString(guestWidth, CultureInfo.InvariantCulture) + "x" + Convert.ToString(guestHeight, CultureInfo.InvariantCulture);
			dictionary.Add("resolution", text);
			double num = (double)guestWidth / (double)guestHeight;
			string text2 = "landscape";
			if (num < 1.0)
			{
				text2 = "portrait";
			}
			dictionary.Add("resolution_type", text2);
			return dictionary;
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x00064E28 File Offset: 0x00063028
		internal void DownloadAndUpdateMacro(string macroData)
		{
			try
			{
				JObject jobject = JObject.Parse(macroData);
				string text = jobject["macro_name"].ToString();
				string url = jobject["download_link"].ToString();
				string userName = jobject["nickname"].ToString();
				string authorPageUrl = jobject["author_url"].ToString();
				string macroId = jobject["macro_id"].ToString();
				string macroPageUrl = jobject["macro_url"].ToString();
				string invalidCharsFreeName;
				if (text.GetValidFileName(out invalidCharsFreeName))
				{
					string text2 = string.Format(CultureInfo.InvariantCulture, "{0}.json", new object[] { invalidCharsFreeName });
					string filePath = Path.Combine(Path.GetTempPath(), text2);
					new Thread(delegate
					{
						using (WebClient webClient = new WebClient())
						{
							try
							{
								webClient.DownloadFile(url, filePath);
							}
							catch (Exception ex2)
							{
								Logger.Error("Failed to download macro at path : " + filePath + ". Ex : " + ex2.ToString());
							}
							finally
							{
								if (webClient != null)
								{
									webClient.Dispose();
								}
							}
							if (File.Exists(filePath))
							{
								this.ParentWindow.Dispatcher.Invoke(new Action(delegate
								{
									try
									{
										MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(filePath), Utils.GetSerializerSettings());
										int num;
										try
										{
											if (!string.IsNullOrEmpty(userName))
											{
												macroRecording.User = userName;
											}
											Uri uri;
											if (!string.IsNullOrEmpty(authorPageUrl) && Uri.TryCreate(authorPageUrl, UriKind.RelativeOrAbsolute, out uri))
											{
												macroRecording.AuthorPageUrl = uri;
											}
											if (!string.IsNullOrEmpty(macroId))
											{
												macroRecording.MacroId = macroId;
											}
											Uri uri2;
											if (!string.IsNullOrEmpty(macroPageUrl) && Uri.TryCreate(macroPageUrl, UriKind.RelativeOrAbsolute, out uri2))
											{
												macroRecording.MacroPageUrl = uri2;
											}
											macroRecording.Name = invalidCharsFreeName;
											if (string.IsNullOrEmpty(macroRecording.TimeCreated))
											{
												macroRecording.TimeCreated = DateTime.Now.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
											}
											bool flag = false;
											this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Clear();
											this.ParentWindow.MacroRecorderWindow.mImportMultiMacroAsUnified = new bool?(true);
											num = this.ParentWindow.MacroRecorderWindow.ImportMacroRecordings(new List<MacroRecording> { macroRecording }, ref flag);
											if (flag)
											{
												num = 3;
											}
										}
										catch (Exception ex3)
										{
											Logger.Error("Failed to import macro recording.");
											Logger.Error(ex3.ToString());
											num = 2;
										}
										if (num == 0)
										{
											foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
											{
												if (keyValuePair.Value.MacroRecorderWindow != null)
												{
													keyValuePair.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
													keyValuePair.Value.MacroRecorderWindow.Init();
												}
											}
										}
										this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
										this.ParentWindow.MacroRecorderWindow.ValidateReturnCode(num);
									}
									catch (Exception ex4)
									{
										Logger.Error("Failed to deserialize downloaded macro.");
										Logger.Error(ex4.ToString());
									}
								}), new object[0]);
								try
								{
									File.Delete(filePath);
								}
								catch
								{
								}
							}
						}
					})
					{
						IsBackground = true
					}.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format("Invalid data in DowloadMacro api : {0}", ex));
			}
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x00064F48 File Offset: 0x00063148
		internal static List<string> GetMacroList()
		{
			List<string> list = new List<string>();
			try
			{
				MacroGraph.ReCreateMacroGraphInstance();
				foreach (BiDirectionalVertex<MacroRecording> biDirectionalVertex in MacroGraph.Instance.Vertices)
				{
					MacroRecording macroRecording = (MacroRecording)biDirectionalVertex;
					if (!string.IsNullOrEmpty(macroRecording.Name) && string.IsNullOrEmpty(macroRecording.MacroId))
					{
						list.Add(macroRecording.Name);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Debug("Failed to get macro list. Ex : " + ex.ToString());
			}
			return list;
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x00064FF0 File Offset: 0x000631F0
		internal static string GetBase64MacroData(string macroName)
		{
			string text = string.Empty;
			try
			{
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				string text2 = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroName.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
				MacroRecording macroRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
					where string.Equals(macroName, macro.Name, StringComparison.InvariantCultureIgnoreCase)
					select macro).FirstOrDefault<MacroRecording>();
				if (macroRecording.RecordingType == RecordingTypes.SingleRecording)
				{
					Logger.Info("Uploading single recording macro");
					text = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(macroRecording, serializerSettings)));
				}
				else
				{
					List<string> list = new List<string>();
					foreach (BiDirectionalVertex<MacroRecording> biDirectionalVertex in MacroGraph.Instance.GetAllChilds(macroRecording))
					{
						MacroRecording macroRecording2 = (MacroRecording)biDirectionalVertex;
						list.Add(File.ReadAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroRecording2.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json")));
					}
					MacroRecording macroRecording3 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(text2), serializerSettings);
					macroRecording3.SourceRecordings = list;
					text = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(macroRecording3, serializerSettings)));
					Logger.Info("Uploading merged macro");
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Coulnd't upload macro recording {0}, Ex: {1}", new object[] { macroName, ex });
			}
			return text;
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x0000B6B4 File Offset: 0x000098B4
		private static void LaunchAllInstancesAndArrange()
		{
			ThreadPool.QueueUserWorkItem(delegate(object job)
			{
				RegistryManager.ClearRegistryMangerInstance();
				foreach (string text in RegistryManager.Instance.VmList)
				{
					if (!BlueStacksUIUtils.DictWindows.ContainsKey(text))
					{
						BlueStacksUIUtils.RunInstance(text, false);
						int num = RegistryManager.Instance.BatchInstanceStartInterval;
						if (num <= 0)
						{
							num = 2;
						}
						Thread.Sleep(num * 1000);
					}
				}
			});
		}

		// Token: 0x04000A15 RID: 2581
		private MainWindow ParentWindow;

		// Token: 0x04000A16 RID: 2582
		private int mCurrentVolumeLevel = 33;

		// Token: 0x04000A17 RID: 2583
		internal static object mLaunchPlaySyncObj = new object();

		// Token: 0x04000A18 RID: 2584
		internal static string sLoggedInImageName = "loggedin";

		// Token: 0x04000A19 RID: 2585
		internal static string sPremiumUserImageName = "premiumuser";

		// Token: 0x04000A1A RID: 2586
		internal static string sUserAccountPackageName = "com.uncube.account";

		// Token: 0x04000A1B RID: 2587
		internal static string sUserAccountActivityName = "com.bluestacks.account.activities.AccountActivity_";

		// Token: 0x04000A1C RID: 2588
		internal static string sAndroidSettingsPackageName = "com.android.settings";

		// Token: 0x04000A1D RID: 2589
		internal static string sAndroidAccountSettingsActivityName = "com.android.settings.BstAccountsSettings";

		// Token: 0x04000A1E RID: 2590
		internal static bool sStopStatSendingThread = false;

		// Token: 0x04000A1F RID: 2591
		internal global::System.Timers.Timer sBootCheckTimer = new global::System.Timers.Timer(360000.0);

		// Token: 0x04000A20 RID: 2592
		internal static List<string> lstCreatingWindows = new List<string>();

		// Token: 0x04000A21 RID: 2593
		internal static Dictionary<string, MainWindow> DictWindows = new Dictionary<string, MainWindow>();

		// Token: 0x04000A22 RID: 2594
		internal static bool sIsSynchronizationActive = false;

		// Token: 0x04000A23 RID: 2595
		internal static List<string> sSelectedInstancesForSync = new List<string>();

		// Token: 0x04000A24 RID: 2596
		public static MainWindow LastActivatedWindow;

		// Token: 0x04000A25 RID: 2597
		public static MainWindow ActivatedWindow;

		// Token: 0x04000A26 RID: 2598
		public static Dictionary<string, List<EventHandler>> BootEventsForMIManager = new Dictionary<string, List<EventHandler>>();

		// Token: 0x04000A27 RID: 2599
		public static List<string> sSyncInvolvedInstances = new List<string>();

		// Token: 0x04000A28 RID: 2600
		private static bool? isOglSupported = null;

		// Token: 0x04000A29 RID: 2601
		private bool disposedValue;
	}
}
