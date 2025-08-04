using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BlueStacks.Common;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001BC RID: 444
	public class FrontendHandler
	{
		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060011A9 RID: 4521 RVA: 0x0006E874 File Offset: 0x0006CA74
		// (remove) Token: 0x060011AA RID: 4522 RVA: 0x0006E8AC File Offset: 0x0006CAAC
		internal event EventHandler mEventOnFrontendClosed;

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060011AB RID: 4523 RVA: 0x0000C94A File Offset: 0x0000AB4A
		// (set) Token: 0x060011AC RID: 4524 RVA: 0x0000C952 File Offset: 0x0000AB52
		internal bool IsRestartFrontendWhenClosed { get; set; }

		// Token: 0x060011AD RID: 4525 RVA: 0x0006E8E4 File Offset: 0x0006CAE4
		public FrontendHandler(string vmName)
		{
			this.mVmName = vmName;
			this.mWindowTitle = Oem.Instance.CommonAppTitleText + vmName;
			this.StartFrontend();
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x0006E938 File Offset: 0x0006CB38
		internal void FrontendHandler_ShowLowRAMMessage()
		{
			MainWindow parentWindow = this.ParentWindow;
			if (((parentWindow != null) ? new bool?(parentWindow.IsLoaded) : null).GetValueOrDefault())
			{
				CustomMessageWindow cmw = new CustomMessageWindow();
				cmw.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_PERF_WARNING", "");
				cmw.AddWarning(LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TITLE", ""), "message_error");
				cmw.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TEXT1", "") + Environment.NewLine + LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TEXT2", "");
				cmw.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_CONTINUE_ANYWAY", ""), delegate(object o, EventArgs args)
				{
					cmw.Close();
				}, null, false, null);
				cmw.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CLOSE_BLUESTACKS", ""), delegate(object o, EventArgs args)
				{
					this.ParentWindow.Close();
				}, null, false, null);
				this.ParentWindow.ShowDimOverlay(null);
				cmw.Owner = this.ParentWindow.mDimOverlay;
				cmw.ShowDialog();
				this.ParentWindow.HideDimOverlay();
			}
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x0000C95B File Offset: 0x0000AB5B
		internal void StartFrontend()
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				Logger.Info("BOOT_STAGE: Starting player");
				if (ProcessUtils.IsLockInUse(Strings.GetPlayerLockName(this.mVmName, "bgp64")))
				{
					this.KillFrontend(true);
				}
				this.mEventOnFrontendClosed = null;
				this.mIsSufficientRAMAvailable = true;
				this.IsRestartFrontendWhenClosed = true;
				this.mFrontendStartTime = DateTime.Now;
				int num = BluestacksProcessHelper.StartFrontend(this.mVmName);
				if (this.ParentWindow == null)
				{
					this.WaitForParentWindowInit();
				}
				if (this.ParentWindow != null)
				{
					if (num == -5)
					{
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							Logger.Error("Hyper v enabled on this machine");
							CustomMessageWindow customMessageWindow = new CustomMessageWindow();
							BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RESTART_UTILITY_CANNOT_START", "");
							customMessageWindow.AddWarning(LocaleStrings.GetLocalizedString("STRING_HYPERV_ENABLED_WARNING", ""), "message_error");
							BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_HYPERV_ENABLED_MESSAGE", "");
							customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHECK_FAQ", delegate(object sender1, EventArgs e1)
							{
								BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
								{
									WebHelper.GetServerHost(),
									"help_articles"
								})) + "&article=disable_hypervisor");
							}, null, false, null);
							customMessageWindow.ShowDialog();
							App.ExitApplication();
						}), new object[0]);
						return;
					}
					if (num == -2)
					{
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							Logger.Error("Android File Integrity check failed");
							CustomMessageWindow customMessageWindow2 = new CustomMessageWindow();
							BlueStacksUIBinding.Bind(customMessageWindow2.TitleTextBlock, "STRING_CORRUPT_INSTALLATION", "");
							BlueStacksUIBinding.Bind(customMessageWindow2.BodyTextBlock, "STRING_CORRUPT_INSTALLATION_MESSAGE", "");
							customMessageWindow2.AddButton(ButtonColors.Blue, "STRING_EXIT", null, null, false, null);
							customMessageWindow2.ShowDialog();
							App.ExitApplication();
						}), new object[0]);
						return;
					}
					if (num == -7)
					{
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							Logger.Error("VBox couldn't detect driver");
							CustomMessageWindow customMessageWindow3 = new CustomMessageWindow();
							BlueStacksUIBinding.Bind(customMessageWindow3.TitleTextBlock, "STRING_ENGINE_FAIL_HEADER", "");
							BlueStacksUIBinding.Bind(customMessageWindow3.BodyTextBlock, "STRING_COULDNT_BOOT_TRY_RESTART", "");
							customMessageWindow3.AddButton(ButtonColors.Blue, "STRING_RESTART_PC", new EventHandler(this.RestartPCEvent), null, false, null);
							customMessageWindow3.AddButton(ButtonColors.White, "STRING_EXIT", null, null, false, null);
							customMessageWindow3.ShowDialog();
							App.ExitApplication();
						}), new object[0]);
						return;
					}
					if (num == -6)
					{
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							Logger.Error("Unable to initialise audio on this machine");
							CustomMessageWindow customMessageWindow4 = new CustomMessageWindow();
							customMessageWindow4.ImageName = "sound_error";
							BlueStacksUIBinding.Bind(customMessageWindow4.TitleTextBlock, "STRING_AUDIO_SERVICE_FAILURE", "");
							BlueStacksUIBinding.Bind(customMessageWindow4.BodyTextBlockTitle, "STRING_AUDIO_SERVICE_FAILUE_FIX", "");
							customMessageWindow4.BodyTextBlockTitle.Visibility = Visibility.Visible;
							BlueStacksUIBinding.Bind(customMessageWindow4.BodyTextBlock, "STRING_AUDIO_SERVICE_FAILURE_ALTERNATE_FIX", "");
							customMessageWindow4.AddButton(ButtonColors.Blue, "STRING_READ_MORE", delegate(object sender1, EventArgs e1)
							{
								BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
								{
									WebHelper.GetServerHost(),
									"help_articles"
								})) + "&article=audio_service_issue");
							}, "external_link", true, null);
							customMessageWindow4.ShowDialog();
							App.ExitApplication();
						}), new object[0]);
						return;
					}
					if (num == -10)
					{
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							string url = null;
							url = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
							{
								WebHelper.GetServerHost(),
								"help_articles"
							}));
							url = string.Format(CultureInfo.InvariantCulture, "{0}&article={1}", new object[] { url, "enable_virtualization" });
							string text = "STRING_VTX_DISABLED_ENABLEIT_BODY";
							CustomMessageWindow customMessageWindow5 = new CustomMessageWindow();
							BlueStacksUIBinding.Bind(customMessageWindow5.TitleTextBlock, "STRING_RESTART_UTILITY_CANNOT_START", "");
							customMessageWindow5.AddAboveBodyWarning(LocaleStrings.GetLocalizedString("STRING_VTX_DISABLED_WARNING", ""));
							customMessageWindow5.AboveBodyWarningTextBlock.Visibility = Visibility.Visible;
							customMessageWindow5.MessageIcon.VerticalAlignment = VerticalAlignment.Center;
							BlueStacksUIBinding.Bind(customMessageWindow5.BodyTextBlock, text, "");
							customMessageWindow5.AddButton(ButtonColors.Blue, "STRING_CHECK_FAQ", delegate(object sender1, EventArgs e1)
							{
								BlueStacksUIUtils.OpenUrl(url);
							}, null, false, null);
							customMessageWindow5.AddButton(ButtonColors.White, "STRING_EXIT", null, null, false, null);
							customMessageWindow5.ShowDialog();
							App.ExitApplication();
						}), new object[0]);
						return;
					}
					if (this.IsRestartFrontendWhenClosed)
					{
						this.ParentWindow.Dispatcher.Invoke(new Action(delegate
						{
							if (this.frontendRestartAttempts < 2)
							{
								this.frontendRestartAttempts++;
								this.ParentWindow.RestartFrontend();
							}
						}), new object[0]);
						return;
					}
				}
				else
				{
					Logger.Error("parent window is null for vmName: {0} and frontend Exit code: {1}", new object[] { this.mVmName, num });
				}
			});
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0006EA88 File Offset: 0x0006CC88
		private void WaitForParentWindowInit()
		{
			Logger.Info("In method WaitForParentWindowInit for vmName: " + this.mVmName);
			int i = 20;
			while (i > 0)
			{
				i--;
				try
				{
					if (this.ParentWindow != null && BlueStacksUIUtils.DictWindows.ContainsKey(this.mVmName))
					{
						Logger.Info("parent window init for vmName: " + this.mVmName);
						return;
					}
					Thread.Sleep(200);
				}
				catch (Exception ex)
				{
					Logger.Error("Exception in wait for mainwindow init: " + ex.ToString());
					Thread.Sleep(200);
				}
			}
			Logger.Error("Parent window not init after {0} retries", new object[] { i });
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0000C96F File Offset: 0x0000AB6F
		private void RestartPCEvent(object sender, EventArgs e)
		{
			Process.Start("shutdown.exe", "/r /t 0");
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x0000C981 File Offset: 0x0000AB81
		internal void KillFrontendAsync()
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				this.KillFrontend(true);
			});
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x0006EB40 File Offset: 0x0006CD40
		internal void KillFrontend(bool isWaitForPlayerClosing = false)
		{
			try
			{
				this.IsRestartFrontendWhenClosed = false;
				Utils.StopFrontend(this.mVmName, isWaitForPlayerClosing);
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in killing frontend: " + ex.ToString());
			}
			finally
			{
				EventHandler eventHandler = this.mEventOnFrontendClosed;
				if (eventHandler != null)
				{
					eventHandler(this.mVmName, null);
				}
			}
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x0006EBB0 File Offset: 0x0006CDB0
		internal void EnableKeyMapping(bool isEnabled)
		{
			try
			{
				this.SendFrontendRequestAsync("setKeymappingState", new Dictionary<string, string> { 
				{
					"keymapping",
					isEnabled.ToString(CultureInfo.InvariantCulture)
				} });
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send EnableKeyMapping to frontend... Err : " + ex.ToString());
			}
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x0006EC10 File Offset: 0x0006CE10
		internal void GetScreenShot(string filePath)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "path", filePath },
				{
					"showSavedInfo",
					true.ToString(CultureInfo.InvariantCulture)
				}
			};
			this.SendFrontendRequestAsync("getScreenshot", dictionary);
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x0006EC54 File Offset: 0x0006CE54
		internal void FrontendVisibleChanged(bool value)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { 
				{
					"new_value",
					Convert.ToString(value, CultureInfo.InvariantCulture)
				} };
				if (RegistryManager.Instance.AreAllInstancesMuted || this.ParentWindow.EngineInstanceRegistry.IsMuted)
				{
					dictionary.Add("is_mute", Convert.ToString(true, CultureInfo.InvariantCulture));
				}
				else
				{
					dictionary.Add("is_mute", Convert.ToString(false, CultureInfo.InvariantCulture));
				}
				this.SendFrontendRequestAsync("frontendVisibleChanged", dictionary);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send refresh keymap to frontend... Err : " + ex.ToString());
			}
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x0006ED00 File Offset: 0x0006CF00
		internal void RefreshKeyMap(string packageName)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "package", packageName } };
				this.SendFrontendRequestAsync("refreshKeymap", dictionary);
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send refresh keymap to frontend... Err : " + ex.ToString());
			}
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x0006ED58 File Offset: 0x0006CF58
		internal void DeactivateFrontend()
		{
			try
			{
				if (this.mFrontendHandle != IntPtr.Zero)
				{
					Logger.Debug("KMP deactivateFrontend");
					this.SendFrontendRequestAsync("deactivateFrontend", null);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Failed to send deactivate to frontend.. Err : " + ex.ToString());
			}
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x0006EDB8 File Offset: 0x0006CFB8
		internal void ToggleStreamingMode(bool state)
		{
			this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
			Action <>9__2;
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				Logger.Info("Streaming mode toggle called with state.." + state.ToString());
				this.ParentWindow.mStreamingModeEnabled = state;
				string text = this.ParentWindow.Handle.ToString();
				if (state)
				{
					text = "0";
				}
				Rectangle windowRectangle = this.GetWindowRectangle();
				if (windowRectangle.Width == 0 && windowRectangle.Height == 0)
				{
					windowRectangle.Width = (int)this.ParentWindow.Width;
					windowRectangle.Height = (int)this.ParentWindow.Height;
				}
				Dictionary<string, string> dict = new Dictionary<string, string>
				{
					{ "ParentHandle", text },
					{
						"X",
						windowRectangle.X.ToString(CultureInfo.InvariantCulture)
					},
					{
						"Y",
						windowRectangle.Y.ToString(CultureInfo.InvariantCulture)
					},
					{
						"Width",
						windowRectangle.Width.ToString(CultureInfo.InvariantCulture)
					},
					{
						"Height",
						windowRectangle.Height.ToString(CultureInfo.InvariantCulture)
					}
				};
				ThreadPool.QueueUserWorkItem(delegate(object obj1)
				{
					try
					{
						JObject jobject = JObject.Parse(JArray.Parse(this.SendFrontendRequest("setparent", dict))[0].ToString());
						if (jobject["success"].ToObject<bool>())
						{
							this.mFrontendHandle = new IntPtr(jobject["frontendhandle"].ToObject<int>());
							Dispatcher dispatcher = this.ParentWindow.Dispatcher;
							Action action;
							if ((action = <>9__2) == null)
							{
								action = (<>9__2 = delegate
								{
									this.ShowGLWindow();
								});
							}
							dispatcher.Invoke(action, new object[0]);
						}
					}
					catch (Exception ex)
					{
						Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
					}
				});
			}), new object[0]);
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x0006EE14 File Offset: 0x0006D014
		internal void ShowGLWindow()
		{
			if (this.CanfrontendBeResizedAndFocused())
			{
				this.ResizeWindow();
				return;
			}
			if (this.ParentWindow.mFrontendGrid.IsVisible)
			{
				if (!this.ParentWindow.Handle.ToString().Equals("0", StringComparison.OrdinalIgnoreCase))
				{
					this.sIsfrontendAlreadyVisible = true;
					if (this.mFrontendHandle == IntPtr.Zero)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							try
							{
								this.ParentWindow.Dispatcher.Invoke(new Action(delegate
								{
									Rectangle windowRectangle = this.GetWindowRectangle();
									Dictionary<string, string> dict = new Dictionary<string, string>
									{
										{
											"ParentHandle",
											(!this.ParentWindow.mStreamingModeEnabled) ? this.ParentWindow.Handle.ToString() : "0"
										},
										{
											"X",
											windowRectangle.X.ToString(CultureInfo.InvariantCulture)
										},
										{
											"Y",
											windowRectangle.Y.ToString(CultureInfo.InvariantCulture)
										},
										{
											"Width",
											windowRectangle.Width.ToString(CultureInfo.InvariantCulture)
										},
										{
											"Height",
											windowRectangle.Height.ToString(CultureInfo.InvariantCulture)
										}
									};
									if (windowRectangle.Width == 0 || windowRectangle.Height == 0)
									{
										this.sIsfrontendAlreadyVisible = false;
										return;
									}
									ThreadPool.QueueUserWorkItem(delegate(object obj1)
									{
										try
										{
											object obj2 = this.mLockObject;
											lock (obj2)
											{
												if (this.mFrontendHandle == IntPtr.Zero)
												{
													JObject jobject = JObject.Parse(JArray.Parse(this.SendFrontendRequest("setParent", dict))[0].ToString());
													if (jobject["success"].ToObject<bool>())
													{
														this.mFrontendHandle = new IntPtr(jobject["frontendhandle"].ToObject<int>());
													}
													Action<MainWindow> reparentingCompletedAction = this.ReparentingCompletedAction;
													if (reparentingCompletedAction != null)
													{
														reparentingCompletedAction(this.ParentWindow);
													}
													Logger.Debug("Set parent call completed. handle: " + this.mFrontendHandle.ToString());
												}
											}
										}
										catch (Exception ex2)
										{
											Logger.Error("Failed to send Show event to engine... err : " + ex2.ToString());
										}
									});
								}), new object[0]);
							}
							catch (Exception ex)
							{
								Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
							}
						});
						return;
					}
					this.ResizeWindow();
					return;
				}
			}
			else
			{
				this.sIsfrontendAlreadyVisible = false;
				if (this.mFrontendHandle != IntPtr.Zero)
				{
					InteropWindow.ShowWindow(this.mFrontendHandle, 0);
					if (KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow) && this.ParentWindow.WindowState != WindowState.Maximized)
					{
						KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
					}
				}
			}
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x0000C995 File Offset: 0x0000AB95
		private bool CanfrontendBeResizedAndFocused()
		{
			return (this.ParentWindow.mDimOverlay == null || !this.ParentWindow.mDimOverlay.IsWindowVisible) && this.ParentWindow.mFrontendGrid.IsVisible && this.sIsfrontendAlreadyVisible;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x0006EEE8 File Offset: 0x0006D0E8
		internal void ResizeWindow()
		{
			Rectangle windowRectangle = this.GetWindowRectangle();
			if (this.ParentWindow.mStreamingModeEnabled)
			{
				InteropWindow.ShowWindow(this.mFrontendHandle, 5);
			}
			else
			{
				InteropWindow.SetWindowPos(this.mFrontendHandle, (IntPtr)0, windowRectangle.X, windowRectangle.Y, windowRectangle.Width, windowRectangle.Height, 16448U);
			}
			if (KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow))
			{
				if (this.ParentWindow.StaticComponents.mLastMappableWindowHandle == IntPtr.Zero)
				{
					this.ParentWindow.StaticComponents.mLastMappableWindowHandle = this.mFrontendHandle;
				}
				KMManager.dictOverlayWindow[this.ParentWindow].UpdateSize();
			}
			this.FocusFrontend();
			RegistryManager.Instance.FrontendHeight = windowRectangle.Height;
			RegistryManager.Instance.FrontendWidth = windowRectangle.Width;
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x0006EFCC File Offset: 0x0006D1CC
		internal Rectangle GetWindowRectangle()
		{
			Grid mFrontendGrid = this.ParentWindow.mFrontendGrid;
			global::System.Windows.Point point = mFrontendGrid.TranslatePoint(new global::System.Windows.Point(0.0, 0.0), this.ParentWindow);
			global::System.Drawing.Point point2 = new global::System.Drawing.Point((int)(MainWindow.sScalingFactor * point.X), (int)(MainWindow.sScalingFactor * point.Y));
			global::System.Drawing.Size size = new global::System.Drawing.Size((int)(mFrontendGrid.ActualWidth * MainWindow.sScalingFactor), (int)(mFrontendGrid.ActualHeight * MainWindow.sScalingFactor));
			return new Rectangle(point2, size);
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x0006F054 File Offset: 0x0006D254
		internal void ChangeFrontendToPortraitMode()
		{
			Rectangle windowRectangle = this.GetWindowRectangle();
			InteropWindow.SetWindowPos(this.mFrontendHandle, (IntPtr)0, windowRectangle.X, windowRectangle.Y, windowRectangle.Width, windowRectangle.Height, 16448U);
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x0006F09C File Offset: 0x0006D29C
		internal void FocusFrontend()
		{
			if (this.CanfrontendBeResizedAndFocused() && !this.ParentWindow.mStreamingModeEnabled && !this.ParentWindow.mIsFocusComeFromImap && this.ParentWindow.IsActive)
			{
				InteropWindow.SetFocus(this.mFrontendHandle);
				Logger.Debug("KMP REFRESH Frontend...." + Environment.StackTrace);
				this.SendFrontendRequestAsync("refreshWindow", null);
				return;
			}
			Logger.Debug("KMP CanfrontendBeResizedAndFocused false " + this.ParentWindow.mFrontendGrid.IsVisible.ToString() + this.sIsfrontendAlreadyVisible.ToString());
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x0000C9D2 File Offset: 0x0000ABD2
		internal void SendFrontendRequestAsync(string path, Dictionary<string, string> data = null)
		{
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				this.SendFrontendRequest(path, data);
			});
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x0006F138 File Offset: 0x0006D338
		internal string SendFrontendRequest(string path, Dictionary<string, string> data = null)
		{
			string text = string.Empty;
			try
			{
				text = HTTPUtils.SendRequestToEngine(path, data, this.ParentWindow.mVmName, 0, null, false, 1, 0, "");
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SendFrontendRequest: " + ex.ToString());
			}
			return text;
		}

		// Token: 0x060011C2 RID: 4546 RVA: 0x0006F194 File Offset: 0x0006D394
		internal static void UpdateBootTimeInregistry(DateTime time)
		{
			try
			{
				int num = (int)(DateTime.Now - time).TotalSeconds * 1000;
				int noOfBootCompleted = RegistryManager.Instance.NoOfBootCompleted;
				RegistryManager.Instance.LastBootTime = num;
				RegistryManager.Instance.NoOfBootCompleted = noOfBootCompleted + 1;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in UpdateBootTimeInregistry: " + ex.ToString());
			}
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x0006F20C File Offset: 0x0006D40C
		internal void UpdateOverlaySizeStatus()
		{
			this.SendFrontendRequestAsync("sendGlWindowSize", new Dictionary<string, string> { 
			{
				"updateSize",
				(this.ParentWindow.WindowState == WindowState.Maximized).ToString(CultureInfo.InvariantCulture)
			} });
		}

		// Token: 0x04000B92 RID: 2962
		private int frontendRestartAttempts;

		// Token: 0x04000B93 RID: 2963
		internal MainWindow ParentWindow;

		// Token: 0x04000B94 RID: 2964
		private string mVmName;

		// Token: 0x04000B95 RID: 2965
		internal string mWindowTitle;

		// Token: 0x04000B96 RID: 2966
		private bool sIsfrontendAlreadyVisible;

		// Token: 0x04000B97 RID: 2967
		internal IntPtr mFrontendHandle;

		// Token: 0x04000B98 RID: 2968
		internal DateTime mFrontendStartTime = DateTime.Now;

		// Token: 0x04000B9A RID: 2970
		internal bool mIsSufficientRAMAvailable = true;

		// Token: 0x04000B9B RID: 2971
		internal bool IsShootingModeActivated;

		// Token: 0x04000B9C RID: 2972
		private object mLockObject = new object();

		// Token: 0x04000B9D RID: 2973
		internal Action<MainWindow> ReparentingCompletedAction;
	}
}
