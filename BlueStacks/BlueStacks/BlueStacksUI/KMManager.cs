using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200014B RID: 331
	internal static class KMManager
	{
		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000DA3 RID: 3491 RVA: 0x0000A717 File Offset: 0x00008917
		public static bool IsDragging
		{
			get
			{
				return KMManager.sDragCanvasElement != null;
			}
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x000508E0 File Offset: 0x0004EAE0
		internal static void GetCurrentParserVersion(MainWindow window)
		{
			try
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						JObject jobject = JObject.Parse(window.mFrontendHandler.SendFrontendRequest("getkeymappingparserversion", null));
						if (jobject["success"].ToObject<bool>())
						{
							KMManager.ParserVersion = jobject["parserversion"].ToString();
						}
					}
					catch (Exception ex2)
					{
						Logger.Error("Failed to get/parse result for getkeymappingparserversion");
						Logger.Error(ex2.ToString());
					}
				});
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in KMManager init: " + ex.ToString());
			}
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x00050938 File Offset: 0x0004EB38
		internal static void CheckAndCreateNewScheme()
		{
			if (BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
			{
				bool isBookMarked = BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
				IMControlScheme imcontrolScheme = BlueStacksUIUtils.LastActivatedWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn && string.Equals(scheme.Name, BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)).FirstOrDefault<IMControlScheme>();
				if (imcontrolScheme != null)
				{
					KMManager.AddNewControlSchemeAndSelectImap(BlueStacksUIUtils.LastActivatedWindow, imcontrolScheme);
					BlueStacksUIUtils.LastActivatedWindow.SelectedConfig.SelectedControlScheme.IsBookMarked = isBookMarked;
				}
			}
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x000509CC File Offset: 0x0004EBCC
		internal static void UpdateUIForGamepadEvent(string text, bool isDown)
		{
			if (text.Contains("GamepadStart", StringComparison.InvariantCultureIgnoreCase) || text.Contains("GamepadBack", StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			string text2 = string.Empty;
			string text3 = ".";
			if (text.Contains(text3))
			{
				text2 = text.Substring(text.IndexOf(text3, StringComparison.InvariantCultureIgnoreCase));
				text = text.Substring(0, text.IndexOf(text3, StringComparison.InvariantCultureIgnoreCase));
			}
			if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.IsVisible && KMManager.sGamepadDualTextbox != null)
			{
				if (string.Equals(KMManager.sGamepadDualTextbox.ActionItemProperty, "GamepadStick", StringComparison.InvariantCultureIgnoreCase))
				{
					text = KMManager.CheckForAnalogEvent(text);
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (KMManager.sGamepadDualTextbox.LstActionItem[0].Type != KeyActionType.Tap && KMManager.sGamepadDualTextbox.LstActionItem[0].Type != KeyActionType.TapRepeat && KMManager.sGamepadDualTextbox.LstActionItem[0].Type != KeyActionType.Script)
					{
						KMManager.sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + text, "");
						KMManager.sGamepadDualTextbox.Setvalue(text + text2);
						KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = KMManager.sGamepadDualTextbox.mKeyTextBox.Text;
						return;
					}
					KMManager.CheckItemToAddInList(text, isDown);
					if (KMManager.pressedGamepadKeyList.Count > 2)
					{
						KMManager.pressedGamepadKeyList.Clear();
						KMManager.sGamepadDualTextbox.mKeyTextBox.Text = string.Empty;
						KMManager.sGamepadDualTextbox.Setvalue(string.Empty);
						KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = string.Empty;
						return;
					}
					if (KMManager.pressedGamepadKeyList.Count == 2)
					{
						string text4 = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(0)) + " + " + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(1));
						KMManager.sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(0)), "") + " + " + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(1)), "");
						KMManager.sGamepadDualTextbox.Setvalue(text4 + text2);
						KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = KMManager.sGamepadDualTextbox.mKeyTextBox.Text;
						KMManager.pressedGamepadKeyList.Clear();
						return;
					}
					if (KMManager.pressedGamepadKeyList.Count == 1)
					{
						string stringForUI = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(0));
						KMManager.sGamepadDualTextbox.mKeyTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + stringForUI, "");
						KMManager.sGamepadDualTextbox.Setvalue(stringForUI + text2);
						KMManager.sGamepadDualTextbox.mKeyTextBox.ToolTip = KMManager.sGamepadDualTextbox.mKeyTextBox.Text;
						return;
					}
				}
			}
			else if (KMManager.sGuidanceWindow != null && KMManager.sGuidanceWindow.IsVisible && KMManager.CurrentIMapTextBox != null && KMManager.CurrentIMapTextBox.IMActionItems != null && KMManager.CurrentIMapTextBox.IMActionItems.Any<IMActionItem>())
			{
				KMManager.CheckAndCreateNewScheme();
				GuidanceWindow.sIsDirty = true;
				if (KMManager.CurrentIMapTextBox.IMActionItems != null)
				{
					if (KMManager.CurrentIMapTextBox.IMActionItems.Any((IMActionItem item) => string.Equals(item.ActionItem, "GamepadStick", StringComparison.InvariantCultureIgnoreCase)))
					{
						text = KMManager.CheckForAnalogEvent(text);
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (KMManager.CurrentIMapTextBox.ActionType == KeyActionType.Tap || KMManager.CurrentIMapTextBox.ActionType == KeyActionType.TapRepeat || KMManager.CurrentIMapTextBox.ActionType == KeyActionType.Script)
					{
						KMManager.CheckItemToAddInList(text, isDown);
						if (KMManager.pressedGamepadKeyList.Count > 2)
						{
							KMManager.pressedGamepadKeyList.Clear();
							KMManager.CurrentIMapTextBox.Tag = string.Empty;
							KMManager.CurrentIMapTextBox.Text = string.Empty;
							using (IEnumerator<IMActionItem> enumerator = KMManager.CurrentIMapTextBox.IMActionItems.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									IMActionItem imactionItem = enumerator.Current;
									IMapTextBox.Setvalue(imactionItem, string.Empty);
								}
								return;
							}
						}
						if (KMManager.pressedGamepadKeyList.Count == 2)
						{
							string text5 = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(0)) + " + " + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(1));
							KMManager.CurrentIMapTextBox.Tag = text5 + text2;
							KMManager.CurrentIMapTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(0)), "") + " + " + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(1)), "");
							foreach (IMActionItem imactionItem2 in KMManager.CurrentIMapTextBox.IMActionItems)
							{
								IMapTextBox.Setvalue(imactionItem2, KMManager.CurrentIMapTextBox.Tag.ToString());
							}
							KMManager.pressedGamepadKeyList.Clear();
							return;
						}
						if (KMManager.pressedGamepadKeyList.Count != 1)
						{
							return;
						}
						string stringForUI2 = IMAPKeys.GetStringForUI(KMManager.pressedGamepadKeyList.Keys.ElementAt(0));
						KMManager.CurrentIMapTextBox.Tag = stringForUI2 + text2;
						KMManager.CurrentIMapTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + stringForUI2, "");
						using (IEnumerator<IMActionItem> enumerator = KMManager.CurrentIMapTextBox.IMActionItems.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								IMActionItem imactionItem3 = enumerator.Current;
								IMapTextBox.Setvalue(imactionItem3, KMManager.CurrentIMapTextBox.Tag.ToString());
							}
							return;
						}
					}
					KMManager.CurrentIMapTextBox.Tag = text + text2;
					KMManager.CurrentIMapTextBox.Text = LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text), "");
					foreach (IMActionItem imactionItem4 in KMManager.CurrentIMapTextBox.IMActionItems)
					{
						IMapTextBox.Setvalue(imactionItem4, KMManager.CurrentIMapTextBox.Tag.ToString());
					}
				}
			}
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x00051078 File Offset: 0x0004F278
		private static void CheckItemToAddInList(string text, bool isDown)
		{
			if (KMManager.pressedGamepadKeyList.ContainsKey(text) && KMManager.pressedGamepadKeyList[text] && !isDown)
			{
				KMManager.pressedGamepadKeyList.Remove(text);
			}
			if (!KMManager.pressedGamepadKeyList.ContainsKey(text) && isDown)
			{
				KMManager.pressedGamepadKeyList.Add(text, isDown);
			}
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x000510CC File Offset: 0x0004F2CC
		private static string CheckForAnalogEvent(string text)
		{
			string text2 = string.Empty;
			if (string.Equals(text, "GamepadLStickUp", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickDown", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickLeft", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadLStickRight", StringComparison.InvariantCultureIgnoreCase))
			{
				text2 = "LeftStick";
			}
			else if (string.Equals(text, "GamepadRStickUp", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickDown", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickLeft", StringComparison.InvariantCultureIgnoreCase) || string.Equals(text, "GamepadRStickRight", StringComparison.InvariantCultureIgnoreCase))
			{
				text2 = "RightStick";
			}
			return text2;
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x0000A721 File Offset: 0x00008921
		internal static bool KeyMappingFilesAvailable(string packageName)
		{
			return !string.IsNullOrEmpty(Utils.GetInputmapperFile(packageName));
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x00051160 File Offset: 0x0004F360
		internal static bool IsSelectedSchemeSmart(MainWindow mainWindow)
		{
			if (mainWindow == null)
			{
				return false;
			}
			IMConfig selectedConfig = mainWindow.SelectedConfig;
			int? num;
			if (selectedConfig == null)
			{
				num = null;
			}
			else
			{
				IMControlScheme selectedControlScheme = selectedConfig.SelectedControlScheme;
				if (selectedControlScheme == null)
				{
					num = null;
				}
				else
				{
					List<JObject> images = selectedControlScheme.Images;
					num = ((images != null) ? new int?(images.Count) : null);
				}
			}
			int? num2 = num;
			int num3 = 0;
			return (num2.GetValueOrDefault() > num3) & (num2 != null);
		}

		// Token: 0x06000DAB RID: 3499 RVA: 0x000511D0 File Offset: 0x0004F3D0
		internal static bool IsShowShootingModeTooltip(MainWindow mainWindow)
		{
			bool flag = false;
			foreach (IMAction imaction in mainWindow.SelectedConfig.SelectedControlScheme.GameControls)
			{
				Pan pan = imaction as Pan;
				if (pan != null)
				{
					if ((pan.Tweaks & 32) != 0)
					{
						return false;
					}
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06000DAC RID: 3500 RVA: 0x00051244 File Offset: 0x0004F444
		internal static void HandleInputMapperWindow(MainWindow mainWindow, string isSelectedTab = "")
		{
			if (FeatureManager.Instance.IsCustomUIForNCSoft && mainWindow.mDimOverlay != null && mainWindow.mDimOverlay.Control != null && mainWindow.mDimOverlay.Control.GetType() == mainWindow.ScreenLockInstance.GetType() && mainWindow.ScreenLockInstance.IsVisible)
			{
				return;
			}
			if (mainWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
			{
				return;
			}
			KeymapCanvasWindow canvasWindow = KMManager.CanvasWindow;
			if (((canvasWindow != null) ? canvasWindow.SidebarWindow : null) != null)
			{
				return;
			}
			if (mainWindow.SelectedConfig != null && mainWindow.SelectedConfig.SelectedControlScheme != null)
			{
				if (mainWindow.SelectedConfig.ControlSchemes.Any((IMControlScheme scheme) => !string.IsNullOrEmpty(scheme.Name)))
				{
					mainWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = true;
					mainWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide_active");
					KMManager.sGuidanceWindow = new GuidanceWindow(mainWindow);
					KMManager.sGuidanceWindow.GuidanceWindowTabSelected(isSelectedTab);
					KMManager.sGuidanceWindow.Show();
					mainWindow.Focus();
					return;
				}
			}
			if (string.Equals(isSelectedTab, "gamepad", StringComparison.InvariantCultureIgnoreCase))
			{
				KMManager.ShowAdvancedSettings(mainWindow);
			}
		}

		// Token: 0x06000DAD RID: 3501 RVA: 0x00051374 File Offset: 0x0004F574
		internal static void ResizeMainWindow(MainWindow window)
		{
			Screen screen = Screen.FromHandle(new WindowInteropHelper(window).Handle);
			double sScalingFactor = MainWindow.sScalingFactor;
			Rectangle rectangle = new Rectangle((int)((double)screen.WorkingArea.X / sScalingFactor), (int)((double)screen.WorkingArea.Y / sScalingFactor), (int)((double)screen.WorkingArea.Width / sScalingFactor), (int)((double)screen.WorkingArea.Height / sScalingFactor));
			if (window.Top + window.ActualHeight > (double)rectangle.Height)
			{
				window.Top = ((double)rectangle.Height - window.ActualHeight) / 2.0;
			}
			if (window.Left < 0.0 || window.Left + window.ActualWidth > (double)rectangle.Width)
			{
				window.Left = ((double)rectangle.Width - window.ActualWidth) / 2.0;
			}
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x00051464 File Offset: 0x0004F664
		internal static void ShowAdvancedSettings(MainWindow mainWindow)
		{
			if (mainWindow.WindowState != WindowState.Normal)
			{
				mainWindow.RestoreWindows(false);
				KeymapCanvasWindow.sWasMaximized = true;
			}
			KMManager.CloseWindows();
			if (KMManager.sGuidanceWindow == null)
			{
				KMManager.CanvasWindow = new KeymapCanvasWindow(mainWindow)
				{
					Owner = mainWindow
				};
				KMManager.CanvasWindow.InitLayout();
				KMManager.ShowOverlayWindow(mainWindow, false, false);
				KMManager.CanvasWindow.ShowDialog();
				if (RegistryManager.Instance.ShowKeyControlsOverlay)
				{
					KMManager.ShowOverlayWindow(mainWindow, true, false);
				}
			}
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x0000A731 File Offset: 0x00008931
		internal static void ShowDynamicOverlay(MainWindow mainWindow, bool isShow, bool isReload = false, string data = "")
		{
			DynamicOverlayConfigControls.Init(data);
			KMManager.ShowOverlayWindow(mainWindow, isShow, isReload);
		}

		// Token: 0x06000DB0 RID: 3504 RVA: 0x000514D8 File Offset: 0x0004F6D8
		internal static void HandleCallbackControl(MainWindow mainWindow, string data = "")
		{
			DynamicOverlayConfigControls.Init(data);
			if (!KMManager.sIsInScriptEditingMode)
			{
				KeymapCanvasWindow canvasWindow = KMManager.CanvasWindow;
				if (((canvasWindow != null) ? canvasWindow.SidebarWindow : null) != null)
				{
					return;
				}
			}
			if (KMManager.listCanvasElement != null && DynamicOverlayConfigControls.Instance.GameControls != null && KMManager.listCanvasElement.Count == DynamicOverlayConfigControls.Instance.GameControls.Count)
			{
				try
				{
					for (int i = 0; i < DynamicOverlayConfigControls.Instance.GameControls.Count; i++)
					{
						DynamicOverlayConfig dynamicOverlayConfig = DynamicOverlayConfigControls.Instance.GameControls[i];
						CanvasElement canvasElement = KMManager.listCanvasElement[i][0];
						if (canvasElement != null && canvasElement.ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
						{
							Logger.Info("Callback: IsEnabled1 : " + dynamicOverlayConfig.Enabled);
							IMAction imaction = canvasElement.ListActionItem.First<IMAction>();
							KMManager.HandleCallbackPrimitive(mainWindow, dynamicOverlayConfig, (imaction as Callback).Action, (imaction as Callback).Id);
						}
					}
				}
				catch (Exception ex)
				{
					string text = "ERROR : GameControl not found in canvas elements. ";
					Exception ex2 = ex;
					Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
		}

		// Token: 0x06000DB1 RID: 3505 RVA: 0x00051608 File Offset: 0x0004F808
		internal static void ShowOverlayWindow(MainWindow mainWindow, bool isShow, bool isreload = false)
		{
			if (mainWindow == null)
			{
				return;
			}
			if (isShow && mainWindow.IsVisible)
			{
				if (!KMManager.dictOverlayWindow.ContainsKey(mainWindow) && mainWindow != null)
				{
					TopBar mTopBar = mainWindow.mTopBar;
					bool flag;
					if (mTopBar == null)
					{
						flag = null != null;
					}
					else
					{
						AppTabButtons mAppTabButtons = mTopBar.mAppTabButtons;
						if (mAppTabButtons == null)
						{
							flag = null != null;
						}
						else
						{
							AppTabButton selectedTab = mAppTabButtons.SelectedTab;
							flag = ((selectedTab != null) ? selectedTab.PackageName : null) != null;
						}
					}
					if (flag && mainWindow != null)
					{
						TopBar mTopBar2 = mainWindow.mTopBar;
						TabType? tabType;
						if (mTopBar2 == null)
						{
							tabType = null;
						}
						else
						{
							AppTabButtons mAppTabButtons2 = mTopBar2.mAppTabButtons;
							tabType = ((mAppTabButtons2 != null) ? new TabType?(mAppTabButtons2.SelectedTab.mTabType) : null);
						}
						TabType? tabType2 = tabType;
						TabType tabType3 = TabType.AppTab;
						if ((tabType2.GetValueOrDefault() == tabType3) & (tabType2 != null))
						{
							if (FeatureManager.Instance.IsCustomUIForNCSoft && mainWindow.mDimOverlay != null && mainWindow.mDimOverlay.Control != null && mainWindow.mDimOverlay.Control.GetType() == mainWindow.ScreenLockInstance.GetType() && mainWindow.ScreenLockInstance.IsVisible)
							{
								return;
							}
							KeymapCanvasWindow keymapCanvasWindow = new KeymapCanvasWindow(mainWindow);
							KMManager.dictOverlayWindow[mainWindow] = keymapCanvasWindow;
							keymapCanvasWindow.IsInOverlayMode = true;
							keymapCanvasWindow.Owner = mainWindow;
							keymapCanvasWindow.InitLayout();
							if (mainWindow.mFrontendHandler.mFrontendHandle == IntPtr.Zero)
							{
								mainWindow.mFrontendHandler.ReparentingCompletedAction = new Action<MainWindow>(KMManager.ShowOverlayWindowAfterReparenting);
								goto IL_0190;
							}
							KMManager.ShowOverlayWindowAfterReparenting(mainWindow);
							goto IL_0190;
						}
					}
				}
				if (mainWindow != null && KMManager.dictOverlayWindow.ContainsKey(mainWindow))
				{
					if (isreload)
					{
						KMManager.dictOverlayWindow[mainWindow].ReloadCanvasWindow();
					}
					KMManager.dictOverlayWindow[mainWindow].UpdateSize();
				}
				IL_0190:
				if (KMManager.dictOverlayWindow.ContainsKey(mainWindow))
				{
					KeymapCanvasWindow keymapCanvasWindow2 = KMManager.dictOverlayWindow[mainWindow];
					if (keymapCanvasWindow2 != null)
					{
						keymapCanvasWindow2.ShowOnboardingOverlayControl(0.0, 0.0, false);
					}
				}
			}
			else if (KMManager.dictOverlayWindow.ContainsKey(mainWindow) && !KMManager.dictOverlayWindow[mainWindow].mIsClosing)
			{
				KMManager.dictOverlayWindow[mainWindow].Close();
			}
			KMManager.ToggleOverlayVisibility(mainWindow);
		}

		// Token: 0x06000DB2 RID: 3506 RVA: 0x00051814 File Offset: 0x0004FA14
		private static void ToggleOverlayVisibility(MainWindow mainWindow)
		{
			KeymapCanvasWindow canvasWindow = KMManager.CanvasWindow;
			if (((canvasWindow != null) ? canvasWindow.SidebarWindow : null) != null && !KMManager.sIsInScriptEditingMode)
			{
				return;
			}
			if (KMManager.listCanvasElement != null && DynamicOverlayConfigControls.Instance.GameControls != null && (KMManager.listCanvasElement.Count == DynamicOverlayConfigControls.Instance.GameControls.Count || KMManager.sIsInScriptEditingMode))
			{
				try
				{
					int i = 0;
					while (i < DynamicOverlayConfigControls.Instance.GameControls.Count)
					{
						if (!mainWindow.SelectedConfig.SelectedControlScheme.GameControls[i].IsVisibleInOverlay)
						{
							goto IL_071F;
						}
						DynamicOverlayConfig dynamicOverlayConfig = DynamicOverlayConfigControls.Instance.GameControls[i];
						if (dynamicOverlayConfig.Enabled.Equals("true", StringComparison.InvariantCultureIgnoreCase))
						{
							using (List<CanvasElement>.Enumerator enumerator = KMManager.listCanvasElement[i].GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									CanvasElement canvasElement = enumerator.Current;
									IMAction imaction = canvasElement.ListActionItem.First<IMAction>();
									KeyActionType type = imaction.Type;
									if (type <= KeyActionType.Pan)
									{
										if (type != KeyActionType.Dpad)
										{
											if (type == KeyActionType.Pan)
											{
												if (((Pan)imaction).IsLookAroundEnabled || ((Pan)imaction).IsShootOnClickEnabled)
												{
													canvasElement.Visibility = Visibility.Hidden;
													continue;
												}
											}
										}
										else
										{
											Dpad dpad = imaction as Dpad;
											if (dpad != null && dpad.IsMOBADpadEnabled)
											{
												Logger.Info(string.Concat(new string[]
												{
													"Position: ",
													dynamicOverlayConfig.Type,
													" ",
													dynamicOverlayConfig.X.ToString(),
													" ",
													dynamicOverlayConfig.Y.ToString()
												}));
												double num = (string.IsNullOrEmpty(dpad.mMOBADpad.XOverlayOffset) ? 0.0 : Convert.ToDouble(dpad.mMOBADpad.XOverlayOffset, CultureInfo.InvariantCulture));
												double num2 = (string.IsNullOrEmpty(dpad.mMOBADpad.YOverlayOffset) ? 0.0 : Convert.ToDouble(dpad.mMOBADpad.YOverlayOffset, CultureInfo.InvariantCulture));
												canvasElement.SetElementLayout(true, dynamicOverlayConfig.X + num, dynamicOverlayConfig.Y + num2);
												canvasElement.mXPosition = dynamicOverlayConfig.X + num;
												canvasElement.mYPosition = dynamicOverlayConfig.Y + num2;
												canvasElement.Visibility = Visibility.Visible;
												continue;
											}
										}
									}
									else
									{
										if (type == KeyActionType.Callback)
										{
											Logger.Info("Callback: IsEnabled2 : " + dynamicOverlayConfig.Enabled);
											KMManager.HandleCallbackPrimitive(mainWindow, dynamicOverlayConfig, (imaction as Callback).Action, (imaction as Callback).Id);
											continue;
										}
										switch (type)
										{
										case KeyActionType.LookAround:
											Logger.Info(string.Concat(new string[]
											{
												"Position: ",
												dynamicOverlayConfig.Type,
												" ",
												dynamicOverlayConfig.LookAroundX.ToString(),
												" ",
												dynamicOverlayConfig.LookAroundY.ToString()
											}));
											if (dynamicOverlayConfig.LookAroundShowOnOverlay)
											{
												LookAround lookAround = imaction as LookAround;
												if (lookAround != null)
												{
													double num3 = (string.IsNullOrEmpty(lookAround.LookAroundXOverlayOffset) ? 0.0 : Convert.ToDouble(lookAround.LookAroundXOverlayOffset, CultureInfo.InvariantCulture));
													double num4 = (string.IsNullOrEmpty(lookAround.LookAroundXOverlayOffset) ? 0.0 : Convert.ToDouble(lookAround.LookAroundXOverlayOffset, CultureInfo.InvariantCulture));
													canvasElement.SetElementLayout(true, dynamicOverlayConfig.LookAroundX + num3, dynamicOverlayConfig.LookAroundY + num4);
													canvasElement.mXPosition = dynamicOverlayConfig.LookAroundX + num3;
													canvasElement.mYPosition = dynamicOverlayConfig.LookAroundY + num4;
													canvasElement.Visibility = Visibility.Visible;
													continue;
												}
											}
											canvasElement.Visibility = Visibility.Hidden;
											continue;
										case KeyActionType.PanShoot:
											Logger.Info(string.Concat(new string[]
											{
												"Position: ",
												dynamicOverlayConfig.Type,
												" ",
												dynamicOverlayConfig.LButtonX.ToString(),
												" ",
												dynamicOverlayConfig.LButtonY.ToString()
											}));
											if (dynamicOverlayConfig.LButtonShowOnOverlay)
											{
												PanShoot panShoot = imaction as PanShoot;
												if (panShoot != null)
												{
													double num5 = (string.IsNullOrEmpty(panShoot.LButtonXOverlayOffset) ? 0.0 : Convert.ToDouble(panShoot.LButtonXOverlayOffset, CultureInfo.InvariantCulture));
													double num6 = (string.IsNullOrEmpty(panShoot.LButtonYOverlayOffset) ? 0.0 : Convert.ToDouble(panShoot.LButtonYOverlayOffset, CultureInfo.InvariantCulture));
													canvasElement.SetElementLayout(true, dynamicOverlayConfig.LButtonX + num5, dynamicOverlayConfig.LButtonY + num6);
													canvasElement.mXPosition = dynamicOverlayConfig.LButtonX + num5;
													canvasElement.mYPosition = dynamicOverlayConfig.LButtonY + num6;
													canvasElement.Visibility = Visibility.Visible;
													continue;
												}
											}
											canvasElement.Visibility = Visibility.Hidden;
											continue;
										case KeyActionType.MOBASkillCancel:
											Logger.Info(string.Concat(new string[]
											{
												"Position: ",
												dynamicOverlayConfig.Type,
												" ",
												dynamicOverlayConfig.CancelX.ToString(),
												" ",
												dynamicOverlayConfig.CancelY.ToString()
											}));
											if (dynamicOverlayConfig.CancelShowOnOverlay)
											{
												MOBASkillCancel mobaskillCancel = imaction as MOBASkillCancel;
												if (mobaskillCancel != null)
												{
													double num7 = (string.IsNullOrEmpty(mobaskillCancel.MOBASkillCancelOffsetX) ? 0.0 : Convert.ToDouble(mobaskillCancel.MOBASkillCancelOffsetX, CultureInfo.InvariantCulture));
													double num8 = (string.IsNullOrEmpty(mobaskillCancel.MOBASkillCancelOffsetX) ? 0.0 : Convert.ToDouble(mobaskillCancel.MOBASkillCancelOffsetX, CultureInfo.InvariantCulture));
													canvasElement.SetElementLayout(true, dynamicOverlayConfig.CancelX + num7, dynamicOverlayConfig.CancelY + num8);
													canvasElement.mXPosition = dynamicOverlayConfig.CancelX + num7;
													canvasElement.mYPosition = dynamicOverlayConfig.CancelY + num8;
													canvasElement.Visibility = Visibility.Visible;
													continue;
												}
											}
											canvasElement.Visibility = Visibility.Hidden;
											continue;
										}
									}
									Logger.Info(string.Concat(new string[]
									{
										"Position: ",
										dynamicOverlayConfig.Type,
										" ",
										dynamicOverlayConfig.X.ToString(),
										" ",
										dynamicOverlayConfig.Y.ToString()
									}));
									IMAction imaction2 = imaction;
									double num9 = (string.IsNullOrEmpty(imaction2.XOverlayOffset) ? 0.0 : Convert.ToDouble(imaction2.XOverlayOffset, CultureInfo.InvariantCulture));
									double num10 = (string.IsNullOrEmpty(imaction2.YOverlayOffset) ? 0.0 : Convert.ToDouble(imaction2.YOverlayOffset, CultureInfo.InvariantCulture));
									canvasElement.SetElementLayout(true, dynamicOverlayConfig.X + num9, dynamicOverlayConfig.Y + num10);
									canvasElement.mXPosition = dynamicOverlayConfig.X + num9;
									canvasElement.mYPosition = dynamicOverlayConfig.Y + num10;
									canvasElement.Visibility = Visibility.Visible;
								}
								goto IL_07BD;
							}
							goto IL_071F;
						}
						KMManager.listCanvasElement[i].ForEach(delegate(CanvasElement element)
						{
							element.Visibility = Visibility.Hidden;
						});
						IL_07BD:
						i++;
						continue;
						IL_071F:
						DynamicOverlayConfig dynamicOverlayConfig2 = DynamicOverlayConfigControls.Instance.GameControls[i];
						CanvasElement canvasElement2 = KMManager.listCanvasElement[i][0];
						if (canvasElement2 != null && canvasElement2.ListActionItem.First<IMAction>().Type == KeyActionType.Callback)
						{
							Logger.Info("Callback: IsEnabled3 : " + dynamicOverlayConfig2.Enabled);
							IMAction imaction3 = canvasElement2.ListActionItem.First<IMAction>();
							canvasElement2.mXPosition = dynamicOverlayConfig2.X;
							canvasElement2.mYPosition = dynamicOverlayConfig2.Y;
							KMManager.HandleCallbackPrimitive(mainWindow, dynamicOverlayConfig2, (imaction3 as Callback).Action, (imaction3 as Callback).Id);
							goto IL_07BD;
						}
						goto IL_07BD;
					}
				}
				catch (Exception ex)
				{
					string text = "ERROR : GameControl not found in canvas elements. ";
					Exception ex2 = ex;
					Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
		}

		// Token: 0x06000DB3 RID: 3507 RVA: 0x00052050 File Offset: 0x00050250
		private static void HandleCallbackPrimitive(MainWindow mainWindow, DynamicOverlayConfig item, string action, string id)
		{
			if (action != null)
			{
				if (action == "Api")
				{
					mainWindow.mCallbackEnabled = item.Enabled.ToString(CultureInfo.InvariantCulture);
					Logger.Info("Callback: HandleCallbackPrimitive(): " + mainWindow.mCallbackEnabled);
					return;
				}
				if (!(action == "Onboarding"))
				{
					return;
				}
				if (item.Enabled.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!KMManager.mIsEnabledStateChanged && id.Equals("Step2", StringComparison.InvariantCultureIgnoreCase))
					{
						KMManager.mOnboardingCounter++;
						KMManager.mIsEnabledStateChanged = true;
					}
					KeymapCanvasWindow keymapCanvasWindow = KMManager.dictOverlayWindow[mainWindow];
					if (keymapCanvasWindow == null)
					{
						return;
					}
					keymapCanvasWindow.ShowOnboardingOverlayControl(item.X, item.Y, true);
					return;
				}
				else if (!id.Equals("Step1", StringComparison.InvariantCultureIgnoreCase) && KMManager.mOnboardingCounter > 1)
				{
					KMManager.mIsEnabledStateChanged = false;
					KeymapCanvasWindow keymapCanvasWindow2 = KMManager.dictOverlayWindow[mainWindow];
					if (keymapCanvasWindow2 == null)
					{
						return;
					}
					keymapCanvasWindow2.ShowOnboardingOverlayControl(item.X, item.Y, false);
				}
			}
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x00052148 File Offset: 0x00050348
		private static void ShowOverlayWindowAfterReparenting(MainWindow window)
		{
			window.Dispatcher.Invoke(new Action(delegate
			{
				if (window != null)
				{
					window.mFrontendHandler.ShowGLWindow();
					if (KMManager.dictOverlayWindow.ContainsKey(window))
					{
						KeymapCanvasWindow keymapCanvasWindow = KMManager.dictOverlayWindow[window];
						if (keymapCanvasWindow != null && !keymapCanvasWindow.mIsClosing)
						{
							keymapCanvasWindow.mCanvas.Opacity = RegistryManager.Instance.TranslucentControlsTransparency;
							if (window.StaticComponents.mLastMappableWindowHandle != IntPtr.Zero)
							{
								keymapCanvasWindow.Show();
								if (KMManager.sIsInScriptEditingMode && KMManager.CanvasWindow.SidebarWindow != null)
								{
									KMManager.CanvasWindow.SidebarWindow.Owner = keymapCanvasWindow;
									KMManager.CanvasWindow.SidebarWindow.Activate();
								}
							}
						}
					}
				}
			}), new object[0]);
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x0000A741 File Offset: 0x00008941
		internal static void ChangeTransparency(MainWindow window, double value)
		{
			if (window != null && KMManager.dictOverlayWindow.ContainsKey(window))
			{
				KMManager.dictOverlayWindow[window].mCanvas.Opacity = value;
			}
			RegistryManager.Instance.TranslucentControlsTransparency = value;
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x00052188 File Offset: 0x00050388
		internal static void CloseWindows()
		{
			if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed)
			{
				try
				{
					KMManager.sGuidanceWindow.Close();
				}
				catch (Exception ex)
				{
					Logger.Error("exception closing GameControlWindow " + ex.ToString());
				}
			}
			if (KMManager.CanvasWindow != null && !KMManager.CanvasWindow.mIsClosing)
			{
				try
				{
					KMManager.CanvasWindow.SidebarWindow.Close();
				}
				catch (Exception ex2)
				{
					Logger.Error("exception closing GameControlWindow " + ex2.ToString());
				}
			}
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x00052224 File Offset: 0x00050424
		internal static void LoadIMActions(MainWindow mainWindow, string packageName)
		{
			Logger.Debug("Extralog: LoadImAction called. vmName:" + mainWindow.mVmName + "..." + Environment.StackTrace);
			KMManager.sPackageName = packageName;
			string inputmapperFile = Utils.GetInputmapperFile(KMManager.sPackageName);
			try
			{
				KMManager.ClearConfig(mainWindow);
				if (File.Exists(inputmapperFile))
				{
					mainWindow.SelectedConfig = KMManager.GetDeserializedIMConfigObject(inputmapperFile, true);
					if (mainWindow.SelectedConfig.ControlSchemes.Any<IMControlScheme>())
					{
						foreach (IMControlScheme imcontrolScheme in mainWindow.SelectedConfig.ControlSchemes)
						{
							if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(imcontrolScheme.Name))
							{
								if (mainWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name].BuiltIn)
								{
									mainWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name] = imcontrolScheme;
								}
							}
							else
							{
								mainWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name] = imcontrolScheme;
							}
						}
						IMControlScheme imcontrolScheme2 = mainWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => scheme.Selected).FirstOrDefault<IMControlScheme>();
						mainWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme2 ?? mainWindow.SelectedConfig.ControlSchemes[0];
					}
					mainWindow.OriginalLoadedConfig = mainWindow.SelectedConfig.DeepCopy();
				}
				KMManager.CheckForShootingModeTooltip(mainWindow);
				if (!AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName].ContainsKey(packageName))
				{
					AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName] = new AppSettings();
				}
				if (!AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName].IsDefaultSchemeRecorded)
				{
					string text = "DefaultScheme";
					string userGuid = RegistryManager.Instance.UserGuid;
					IMConfig selectedConfig = mainWindow.SelectedConfig;
					string text2;
					if (selectedConfig == null)
					{
						text2 = null;
					}
					else
					{
						IMControlScheme selectedControlScheme = selectedConfig.SelectedControlScheme;
						text2 = ((selectedControlScheme != null) ? selectedControlScheme.Name : null);
					}
					ClientStats.SendMiscellaneousStatsAsync(text, userGuid, packageName, text2, null, null, null, null, null);
					AppConfigurationManager.Instance.VmAppConfig[mainWindow.mVmName][packageName].IsDefaultSchemeRecorded = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error parsing file " + inputmapperFile + ex.ToString());
			}
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x000524A4 File Offset: 0x000506A4
		internal static void SendSchemeChangedStats(MainWindow window, string source = "")
		{
			ClientStats.SendMiscellaneousStatsAsync("SchemeChanged", RegistryManager.Instance.UserGuid, KMManager.sPackageName, window.SelectedConfig.SelectedControlScheme.Name, source, null, null, null, null);
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x000524E0 File Offset: 0x000506E0
		internal static void PopulateMOBADpadAsChildOfDpad(IMConfig imConfigObj)
		{
			foreach (IMControlScheme imcontrolScheme in imConfigObj.ControlSchemes)
			{
				List<Dpad> list = new List<Dpad>();
				List<MOBADpad> list2 = new List<MOBADpad>();
				foreach (IMAction imaction in imcontrolScheme.GameControls)
				{
					if (imaction.Type == KeyActionType.Dpad)
					{
						Dpad dpad = imaction as Dpad;
						list.Add(dpad);
					}
					else if (imaction.Type == KeyActionType.MOBADpad)
					{
						MOBADpad mobadpad = imaction as MOBADpad;
						list2.Add(mobadpad);
					}
				}
				foreach (MOBADpad mobadpad2 in list2)
				{
					foreach (Dpad dpad2 in list)
					{
						if (mobadpad2.X.Equals(dpad2.X) && mobadpad2.Y.Equals(dpad2.Y))
						{
							dpad2.mMOBADpad = mobadpad2;
							mobadpad2.mDpad = dpad2;
							mobadpad2.ParentAction = dpad2;
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x000526A8 File Offset: 0x000508A8
		internal static MOBADpad GetMOBADPad(MainWindow mainWindow)
		{
			foreach (IMAction imaction in mainWindow.SelectedConfig.SelectedControlScheme.GameControls)
			{
				if (imaction.Type == KeyActionType.MOBADpad)
				{
					MOBADpad mobadpad = imaction as MOBADpad;
					if (mobadpad.OriginX != -1.0 && mobadpad.OriginY != -1.0)
					{
						return mobadpad;
					}
				}
			}
			return null;
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x00052738 File Offset: 0x00050938
		internal static IMConfig GetDeserializedIMConfigObject(string fileName, bool isFileNameUsed = true)
		{
			IMConfig imconfig;
			if (!isFileNameUsed)
			{
				imconfig = JsonConvert.DeserializeObject<IMConfig>(fileName, Utils.GetSerializerSettings());
			}
			else
			{
				bool flag = false;
				string text = "";
				using (Mutex mutex = new Mutex(false, "BlueStacks_CfgAccess"))
				{
					if (mutex.WaitOne())
					{
						try
						{
							text = File.ReadAllText(fileName);
							flag = true;
						}
						catch (Exception ex)
						{
							Logger.Error(string.Format("Failed to read cfg file... filepath: {0} Err : {1}", fileName, ex));
						}
						finally
						{
							mutex.ReleaseMutex();
						}
					}
				}
				if (!flag)
				{
					throw new Exception("Could not read file " + fileName);
				}
				imconfig = JsonConvert.DeserializeObject<IMConfig>(text, Utils.GetSerializerSettings());
			}
			KMManager.PopulateMOBADpadAsChildOfDpad(imconfig);
			return imconfig;
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x0000A774 File Offset: 0x00008974
		internal static IMControlScheme GetNewControlSchemes(string name)
		{
			return new IMControlScheme
			{
				Name = name
			};
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x000527FC File Offset: 0x000509FC
		internal static ComboBoxSchemeControl GetComboBoxSchemeControlFromName(string schemeName)
		{
			foreach (object obj in KMManager.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
			{
				ComboBoxSchemeControl comboBoxSchemeControl = (ComboBoxSchemeControl)obj;
				if (comboBoxSchemeControl.mSchemeName.Text == schemeName)
				{
					return comboBoxSchemeControl;
				}
			}
			return null;
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x0005287C File Offset: 0x00050A7C
		internal static void AddNewControlSchemeAndSelect(MainWindow mainWindow, IMControlScheme toCopyFromScheme = null, bool isCopyOrNew = false)
		{
			bool flag = false;
			if (toCopyFromScheme != null && toCopyFromScheme.Selected)
			{
				IMConfig selectedConfig = mainWindow.SelectedConfig;
				if (string.Equals((selectedConfig != null) ? selectedConfig.SelectedControlScheme.Name : null, toCopyFromScheme.Name, StringComparison.InvariantCulture))
				{
					flag = true;
				}
			}
			IMControlScheme imcontrolScheme;
			if (toCopyFromScheme != null)
			{
				imcontrolScheme = toCopyFromScheme.DeepCopy();
				if (flag)
				{
					List<IMAction> gameControls = toCopyFromScheme.GameControls;
					toCopyFromScheme.SetGameControls(imcontrolScheme.GameControls);
					imcontrolScheme.SetGameControls(gameControls);
				}
			}
			else
			{
				imcontrolScheme = new IMControlScheme();
			}
			imcontrolScheme.Name = KMManager.GetNewSchemeName(mainWindow, toCopyFromScheme, isCopyOrNew);
			imcontrolScheme.Selected = true;
			imcontrolScheme.BuiltIn = false;
			mainWindow.SelectedConfig.ControlSchemes.Add(imcontrolScheme);
			bool flag2 = false;
			if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(imcontrolScheme.Name))
			{
				if (mainWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name].BuiltIn)
				{
					flag2 = mainWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name].IsBookMarked;
					mainWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name] = imcontrolScheme;
				}
			}
			else
			{
				mainWindow.SelectedConfig.ControlSchemesDict.Add(imcontrolScheme.Name, imcontrolScheme);
			}
			imcontrolScheme.IsBookMarked = flag2;
			if (isCopyOrNew && KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
			{
				KMManager.CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = imcontrolScheme.Name;
				ComboBoxSchemeControl comboBoxSchemeControl = new ComboBoxSchemeControl(KMManager.CanvasWindow, mainWindow);
				comboBoxSchemeControl.mSchemeName.Text = LocaleStrings.GetLocalizedString(imcontrolScheme.Name, "");
				comboBoxSchemeControl.IsEnabled = true;
				BlueStacksUIBinding.BindColor(comboBoxSchemeControl, global::System.Windows.Controls.Control.BackgroundProperty, "AdvancedGameControlButtonGridBackground");
				KMManager.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children.Add(comboBoxSchemeControl);
			}
			if (mainWindow.SelectedConfig.SelectedControlScheme != null)
			{
				if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
				{
					ComboBoxSchemeControl comboBoxSchemeControlFromName = KMManager.GetComboBoxSchemeControlFromName(mainWindow.SelectedConfig.SelectedControlScheme.Name);
					if (comboBoxSchemeControlFromName != null)
					{
						BlueStacksUIBinding.BindColor(comboBoxSchemeControlFromName, global::System.Windows.Controls.Control.BackgroundProperty, "ComboBoxBackgroundColor");
					}
				}
				mainWindow.SelectedConfig.SelectedControlScheme.Selected = false;
			}
			mainWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme;
			KeymapCanvasWindow.sIsDirty = true;
			if (!flag && KMManager.CanvasWindow != null)
			{
				KMManager.CanvasWindow.Init();
			}
			if (isCopyOrNew && KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
			{
				KMManager.CanvasWindow.SidebarWindow.FillProfileCombo();
			}
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x00052AEC File Offset: 0x00050CEC
		private static void AddNewControlSchemeAndSelectImap(MainWindow mainWindow, IMControlScheme builtInScheme)
		{
			IMConfig selectedConfig = mainWindow.SelectedConfig;
			IMControlScheme imcontrolScheme = ((selectedConfig != null) ? selectedConfig.SelectedControlScheme : null);
			int? num;
			if (mainWindow == null)
			{
				num = null;
			}
			else
			{
				IMConfig selectedConfig2 = mainWindow.SelectedConfig;
				num = ((selectedConfig2 != null) ? new int?(selectedConfig2.ControlSchemes.IndexOf(imcontrolScheme)) : null);
			}
			int? num2 = num;
			if (num2 != null)
			{
				int? num3 = num2;
				int num4 = -1;
				if ((num3.GetValueOrDefault() > num4) & (num3 != null))
				{
					mainWindow.SelectedConfig.ControlSchemes[num2.Value] = builtInScheme.DeepCopy();
					mainWindow.SelectedConfig.ControlSchemes[num2.Value].IsBookMarked = false;
				}
			}
			imcontrolScheme.BuiltIn = false;
			foreach (IMControlScheme imcontrolScheme2 in mainWindow.SelectedConfig.ControlSchemes)
			{
				imcontrolScheme2.Selected = false;
			}
			mainWindow.SelectedConfig.ControlSchemes.Add(imcontrolScheme);
			if (mainWindow.SelectedConfig.ControlSchemesDict.ContainsKey(imcontrolScheme.Name))
			{
				mainWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name] = imcontrolScheme;
			}
			mainWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme;
			imcontrolScheme.Selected = true;
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x00052C40 File Offset: 0x00050E40
		internal static string GetNewSchemeName(MainWindow mainWindow, IMControlScheme builtInScheme, bool isCopyOrNew)
		{
			string text;
			if (builtInScheme == null)
			{
				text = "Custom";
			}
			else
			{
				text = builtInScheme.Name;
				if (builtInScheme.BuiltIn && !isCopyOrNew)
				{
					return text;
				}
			}
			List<string> list = new List<string>();
			foreach (IMControlScheme imcontrolScheme in mainWindow.SelectedConfig.ControlSchemes)
			{
				list.Add(imcontrolScheme.Name);
			}
			return KMManager.GetUniqueName(text, list);
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x00052CCC File Offset: 0x00050ECC
		internal static string GetUniqueName(string baseName, IEnumerable<string> nameCollection)
		{
			int length = baseName.Length;
			int num = 0;
			bool flag = false;
			foreach (string text in nameCollection)
			{
				if (string.Compare(baseName, 0, text, 0, length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					if (text.Length > length + 3 && text[length] == ' ' && text[length + 1] == '(' && text[text.Length - 1] == ')')
					{
						int num2;
						try
						{
							num2 = int.Parse(text.Substring(length + 2, text.Length - length - 3), CultureInfo.InvariantCulture);
						}
						catch (Exception)
						{
							continue;
						}
						if (num2 > num)
						{
							num = num2;
						}
					}
				}
			}
			if (!flag)
			{
				return baseName;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
			{
				baseName,
				num + 1
			});
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x00052DCC File Offset: 0x00050FCC
		internal static bool IsValidCfg(string fileName)
		{
			bool flag;
			try
			{
				if (JsonConvert.DeserializeObject(File.ReadAllText(fileName)) == null)
				{
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception)
			{
				Logger.Error("invalid cfg file: {0}", new object[] { fileName });
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x00052E18 File Offset: 0x00051018
		private static void CheckForShootingModeTooltip(MainWindow window)
		{
			try
			{
				foreach (IMAction imaction in window.SelectedConfig.SelectedControlScheme.GameControls)
				{
					if (imaction.Type == KeyActionType.Pan)
					{
						KMManager.sShootingModeKey = ((Pan)imaction).KeyStartStop.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing shooting mode tooltip: " + ex.ToString());
			}
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x0000A782 File Offset: 0x00008982
		internal static void ClearConfig(MainWindow mainWindow)
		{
			MOBADpad.sListMOBADpad.Clear();
			Dpad.sListDpad.Clear();
			mainWindow.SelectedConfig = null;
			mainWindow.OriginalLoadedConfig = null;
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x00052EB8 File Offset: 0x000510B8
		internal static void GetCanvasElement(MainWindow mainWindow, IMAction action, Canvas canvas, bool addToCanvas = true)
		{
			KMManager.sDragCanvasElement = new CanvasElement(KMManager.CanvasWindow, mainWindow);
			KMManager.sDragCanvasElement.AddAction(action);
			KMManager.sDragCanvasElement.Opacity = 0.1;
			if (addToCanvas)
			{
				canvas.Children.Add(KMManager.sDragCanvasElement);
			}
			if (action.Type == KeyActionType.Swipe)
			{
				KMManager.AssignSwapValues(action);
				List<Direction> list = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList<Direction>();
				list.Remove(action.Direction);
				foreach (Direction direction in list)
				{
					IMAction imaction = action.DeepCopy<IMAction>();
					imaction.Direction = direction;
					imaction.RadiusProperty = action.RadiusProperty;
					KMManager.AssignSwapValues(imaction);
					KMManager.sDragCanvasElement.AddAction(imaction);
				}
				action.RadiusProperty = action.RadiusProperty;
			}
		}

		// Token: 0x06000DC6 RID: 3526 RVA: 0x00052FB0 File Offset: 0x000511B0
		private static void AssignSwapValues(IMAction action)
		{
			if (action.Direction == Direction.Up)
			{
				(action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Up);
				return;
			}
			if (action.Direction == Direction.Down)
			{
				(action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Down);
				return;
			}
			if (action.Direction == Direction.Left)
			{
				(action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Left);
				return;
			}
			(action as Swipe).Key = IMAPKeys.GetStringForUI(Key.Right);
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x00053024 File Offset: 0x00051224
		internal static List<IMAction> ClearElement()
		{
			List<IMAction> list = null;
			if (KMManager.sDragCanvasElement != null)
			{
				list = KMManager.sDragCanvasElement.ListActionItem;
				global::System.Windows.Controls.Panel panel = KMManager.sDragCanvasElement.Parent as global::System.Windows.Controls.Panel;
				if (panel != null)
				{
					panel.Children.Remove(KMManager.sDragCanvasElement);
				}
				KMManager.sDragCanvasElement = null;
			}
			return list;
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x00053070 File Offset: 0x00051270
		internal static void RepositionCanvasElement()
		{
			if (KMManager.sDragCanvasElement != null)
			{
				global::System.Windows.Point position = Mouse.GetPosition(KMManager.sDragCanvasElement.Parent as IInputElement);
				Canvas.SetTop(KMManager.sDragCanvasElement, position.Y - KMManager.sDragCanvasElement.ActualHeight / 2.0);
				Canvas.SetLeft(KMManager.sDragCanvasElement, position.X - KMManager.sDragCanvasElement.ActualWidth / 2.0);
			}
		}

		// Token: 0x06000DC9 RID: 3529 RVA: 0x000530E8 File Offset: 0x000512E8
		internal static void SaveIMActions(MainWindow mainWindow, bool isSavedFromGameControlWindow, bool isdDeleteIfEmpty = false)
		{
			Logger.Debug(string.Format("ExtraLog:Calling SaveIMActions, VmName:{0}, Scheme:{1}, SchemeCount:{2}", mainWindow.mVmName, mainWindow.SelectedConfig.SelectedControlScheme.Name, mainWindow.SelectedConfig.ControlSchemes.Count));
			if (!KeymapCanvasWindow.sIsDirty && !GuidanceWindow.sIsDirty && !isdDeleteIfEmpty)
			{
				Logger.Info("No changes were made in config file. Not saving");
				return;
			}
			KMManager.sPackageName = mainWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
			KeymapCanvasWindow.sIsDirty = false;
			GuidanceWindow.sIsDirty = false;
			KMManager.sGamepadDualTextbox = null;
			string inputmapperUserFilePath = Utils.GetInputmapperUserFilePath(KMManager.sPackageName);
			KMManager.CheckForShootingModeTooltip(mainWindow);
			try
			{
				string directoryName = Path.GetDirectoryName(inputmapperUserFilePath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				KMManager.CleanupGuidanceAccordingToSchemes(mainWindow.SelectedConfig.ControlSchemes, mainWindow.SelectedConfig.Strings);
				KMManager.SaveAndUpdateKeymapUI(mainWindow, isSavedFromGameControlWindow, inputmapperUserFilePath);
			}
			catch (Exception ex)
			{
				Logger.Error("Error saving file  for " + inputmapperUserFilePath + Environment.NewLine + ex.ToString());
			}
		}

		// Token: 0x06000DCA RID: 3530 RVA: 0x000531F4 File Offset: 0x000513F4
		private static void SaveAndUpdateKeymapUI(MainWindow mainWindow, bool isSavedFromGameControlWindow, string path)
		{
			Logger.Debug(string.Format("ExtraLog:Calling SaveAndUpdateKeymapUI, VmName:{0}, Scheme:{1}, SchemeCount:{2}", mainWindow.mVmName, mainWindow.SelectedConfig.SelectedControlScheme.Name, mainWindow.SelectedConfig.ControlSchemes.Count));
			try
			{
				mainWindow.SelectedConfig.MetaData.ParserVersion = KMManager.ParserVersion;
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = Formatting.Indented;
				string text = JsonConvert.SerializeObject(mainWindow.SelectedConfig, serializerSettings);
				bool callUpdateGrm = false;
				if (!File.Exists(path))
				{
					callUpdateGrm = true;
				}
				using (Mutex mutex = new Mutex(false, "BlueStacks_CfgAccess"))
				{
					if (mutex.WaitOne())
					{
						try
						{
							Logger.Debug(string.Format("ExtraLog:Calling WriteAllText, VmName:{0}, Scheme:{1}, SchemeCount:{2}", mainWindow.mVmName, mainWindow.SelectedConfig.SelectedControlScheme.Name, mainWindow.SelectedConfig.ControlSchemes.Count));
							File.WriteAllText(path, text);
						}
						catch (Exception ex)
						{
							Logger.Error(string.Format("Failed to write cfg path: {0} Err : {1}", path, ex));
						}
						finally
						{
							mutex.ReleaseMutex();
						}
					}
				}
				Logger.Debug(string.Format("ExtraLog:Updating Original Config, VmName:{0}, Scheme:{1}, SchemeCount:{2}", mainWindow.mVmName, mainWindow.SelectedConfig.SelectedControlScheme.Name, mainWindow.SelectedConfig.ControlSchemes.Count));
				mainWindow.OriginalLoadedConfig = mainWindow.SelectedConfig.DeepCopy();
				bool isEnabled = false;
				if (mainWindow.OriginalLoadedConfig.ControlSchemes != null && mainWindow.OriginalLoadedConfig.ControlSchemes.Count > 0)
				{
					isEnabled = true;
				}
				else
				{
					isEnabled = false;
				}
				mainWindow.Dispatcher.Invoke(new Action(delegate
				{
					if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null)
					{
						KMManager.CanvasWindow.SidebarWindow.mExport.IsEnabled = isEnabled;
					}
					mainWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(isEnabled);
					ClientStats.SendKeyMappingUIStatsAsync("cfg_saved", KMManager.sPackageName, isSavedFromGameControlWindow ? "edit_keys" : "advanced");
					if (callUpdateGrm)
					{
						GrmHandler.RequirementConfigUpdated(mainWindow.mVmName);
					}
					BlueStacksUIUtils.RefreshKeyMap(KMManager.sPackageName);
					if (KMManager.CanvasWindow != null && !KMManager.CanvasWindow.IsInOverlayMode)
					{
						KMManager.CanvasWindow.Init();
					}
					if (KMManager.dictGamepadEligibility.ContainsKey(KMManager.sPackageName))
					{
						KMManager.dictGamepadEligibility.Remove(KMManager.sPackageName);
					}
				}), new object[0]);
			}
			catch (Exception ex2)
			{
				Logger.Error("Exception in SaveAndUpdateKeymapUI.." + ex2.ToString());
			}
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x00053488 File Offset: 0x00051688
		internal static void SaveConfigToFile(string path, IMConfig config)
		{
			JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
			serializerSettings.Formatting = Formatting.Indented;
			string text = JsonConvert.SerializeObject(config, serializerSettings);
			File.WriteAllText(path, text);
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x000534B4 File Offset: 0x000516B4
		internal static bool CheckIfKeymappingWindowVisible(bool checkForGuidanceWindow = false)
		{
			bool isVisible = false;
			bool guidanceWindowVisible = false;
			try
			{
				BlueStacksUIUtils.LastActivatedWindow.Dispatcher.Invoke(new Action(delegate
				{
					if (KMManager.sGuidanceWindow != null && (KMManager.sGuidanceWindow.IsActive || KMManager.sGuidanceWindow.IsVisible))
					{
						guidanceWindowVisible = true;
					}
					if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.IsActive)
					{
						isVisible = true;
					}
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Info("Exception in checkifkeymappingwindowvisible: " + ex.ToString());
			}
			if (checkForGuidanceWindow)
			{
				return guidanceWindowVisible;
			}
			return isVisible;
		}

		// Token: 0x06000DCD RID: 3533 RVA: 0x00053534 File Offset: 0x00051734
		internal static void CallGamepadHandler(MainWindow mainWindow, string isEnable = "true")
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "enable", isEnable } };
			mainWindow.mFrontendHandler.SendFrontendRequestAsync("toggleGamepadButton", dictionary);
		}

		// Token: 0x06000DCE RID: 3534 RVA: 0x00053564 File Offset: 0x00051764
		internal static string GetStringsToShowInUI(string text)
		{
			string[] array = text.ToString(CultureInfo.InvariantCulture).Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
			string text2 = string.Empty;
			if (array.Length == 2)
			{
				string text3 = IMAPKeys.GetStringForUI(array[0].Trim());
				string stringForUI = IMAPKeys.GetStringForUI(array[1].Trim());
				text2 = Constants.ImapLocaleStringsConstant + text3 + " + " + stringForUI;
			}
			else if (array.Length == 1)
			{
				string text3 = IMAPKeys.GetStringForUI(array[0].Trim());
				text2 = Constants.ImapLocaleStringsConstant + text3;
			}
			return text2;
		}

		// Token: 0x06000DCF RID: 3535 RVA: 0x000535EC File Offset: 0x000517EC
		internal static Dictionary<string, Dictionary<string, string>> CleanupGuidanceAccordingToSchemes(List<IMControlScheme> schemes, Dictionary<string, Dictionary<string, string>> locales)
		{
			HashSet<string> guidanceInUse = new HashSet<string>();
			foreach (IMControlScheme imcontrolScheme in schemes)
			{
				foreach (IMAction imaction in imcontrolScheme.GameControls)
				{
					guidanceInUse.UnionWith(imaction.Guidance.Values);
					guidanceInUse.Add(imaction.GuidanceCategory);
				}
			}
			Func<KeyValuePair<string, string>, bool> <>9__0;
			foreach (string text in locales.Keys)
			{
				IEnumerable<KeyValuePair<string, string>> enumerable = locales[text];
				Func<KeyValuePair<string, string>, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (KeyValuePair<string, string> kv) => !guidanceInUse.Contains(kv.Key));
				}
				foreach (KeyValuePair<string, string> keyValuePair in enumerable.Where(func).ToList<KeyValuePair<string, string>>())
				{
					locales[text].Remove(keyValuePair.Key);
				}
			}
			return locales;
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x00053768 File Offset: 0x00051968
		public static string GetPackageFromCfgFile(string cfgFileName)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(cfgFileName))
			{
				text = Path.GetFileNameWithoutExtension(cfgFileName);
			}
			return text;
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x0005378C File Offset: 0x0005198C
		public static void MergeConfig(string pdPath)
		{
			Logger.Info("In MergeConfig");
			try
			{
				string text = Path.Combine(pdPath, "Engine\\UserData\\InputMapper\\UserFiles");
				string[] files = Directory.GetFiles(text);
				for (int i = 0; i < files.Length; i++)
				{
					FileInfo fileInfo = new FileInfo(files[i]);
					string text2 = Path.Combine(pdPath, "Engine\\UserData\\InputMapper");
					string text3 = Path.Combine(text2, fileInfo.Name);
					string text4 = Path.Combine(text, fileInfo.Name);
					IMConfig deserializedIMConfigObject = KMManager.GetDeserializedIMConfigObject(text4, true);
					if (deserializedIMConfigObject.ControlSchemes.Count == 1)
					{
						deserializedIMConfigObject.ControlSchemes[0].Selected = true;
					}
					if (!File.Exists(text3))
					{
						KMManager.SaveConfigToFile(Path.Combine(text2, "UserFiles\\" + fileInfo.Name), deserializedIMConfigObject);
					}
					else
					{
						KMManager.ControlSchemesHandling(KMManager.GetPackageFromCfgFile(fileInfo.Name), text4, text3);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in merging cfg. err: " + ex.ToString());
			}
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x00053894 File Offset: 0x00051A94
		internal static void MergeConflictingGuidanceStrings(IMConfig newConfig, List<IMControlScheme> toCopyFromSchemes, Dictionary<string, Dictionary<string, string>> stringsToImport)
		{
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (string text in stringsToImport.Keys)
			{
				hashSet2.UnionWith(stringsToImport[text].Keys);
				if (newConfig.Strings.Keys.Contains(text))
				{
					hashSet2.UnionWith(newConfig.Strings[text].Keys);
					foreach (string text2 in stringsToImport[text].Keys)
					{
						if (newConfig.Strings[text].Keys.Contains(text2) && stringsToImport[text][text2] != newConfig.Strings[text][text2])
						{
							hashSet.Add(text2);
						}
					}
				}
			}
			foreach (string text3 in hashSet)
			{
				string uniqueName = KMManager.GetUniqueName(text3, hashSet2);
				foreach (IMControlScheme imcontrolScheme in toCopyFromSchemes)
				{
					foreach (IMAction imaction in imcontrolScheme.GameControls)
					{
						if (imaction.GuidanceCategory == text3)
						{
							imaction.GuidanceCategory = uniqueName;
						}
						foreach (string text4 in imaction.Guidance.Keys)
						{
							if (imaction.Guidance[text4] == text3)
							{
								imaction.Guidance[text4] = uniqueName;
								break;
							}
						}
					}
				}
				foreach (Dictionary<string, string> dictionary in stringsToImport.Values)
				{
					if (dictionary.ContainsKey(text3))
					{
						dictionary[uniqueName] = dictionary[text3];
						dictionary.Remove(text3);
					}
				}
			}
			foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in stringsToImport)
			{
				if (newConfig.Strings.ContainsKey(keyValuePair.Key))
				{
					using (Dictionary<string, string>.Enumerator enumerator8 = keyValuePair.Value.GetEnumerator())
					{
						while (enumerator8.MoveNext())
						{
							KeyValuePair<string, string> keyValuePair2 = enumerator8.Current;
							newConfig.Strings[keyValuePair.Key][keyValuePair2.Key] = keyValuePair2.Value;
						}
						continue;
					}
				}
				newConfig.Strings[keyValuePair.Key] = keyValuePair.Value;
			}
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x00053CA8 File Offset: 0x00051EA8
		public static void ControlSchemesHandlingWhileCfgUpdateFromCloud(string package)
		{
			string text = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
			string text2 = string.Format(CultureInfo.InvariantCulture, Path.Combine(text, package) + ".cfg", new object[0]);
			string text3 = string.Format(CultureInfo.InvariantCulture, Path.Combine(RegistryStrings.InputMapperFolder, package) + ".cfg", new object[0]);
			KMManager.ControlSchemesHandlingFromCloud(package, text2, text3);
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x00053D18 File Offset: 0x00051F18
		private static void ControlSchemesHandling(string package, string userFilesCfgPath, string inputMapperCfgPath)
		{
			try
			{
				if (File.Exists(userFilesCfgPath))
				{
					IMConfig deserializedIMConfigObject = KMManager.GetDeserializedIMConfigObject(inputMapperCfgPath, true);
					IMConfig deserializedIMConfigObject2 = KMManager.GetDeserializedIMConfigObject(userFilesCfgPath, true);
					KMManager.MergeConflictingGuidanceStrings(deserializedIMConfigObject, deserializedIMConfigObject2.ControlSchemes, deserializedIMConfigObject2.Strings);
					deserializedIMConfigObject2.Strings = deserializedIMConfigObject.Strings;
					List<IMControlScheme> list = new List<IMControlScheme>();
					foreach (IMControlScheme imcontrolScheme in deserializedIMConfigObject.ControlSchemes)
					{
						if (imcontrolScheme.BuiltIn)
						{
							list.Add(imcontrolScheme);
						}
					}
					IMControlScheme imcontrolScheme2;
					bool flag = KMManager.IsBuiltInSchemeSelected(deserializedIMConfigObject2, out imcontrolScheme2);
					List<IMControlScheme> list2 = new List<IMControlScheme>();
					foreach (IMControlScheme imcontrolScheme3 in deserializedIMConfigObject2.ControlSchemes)
					{
						if (imcontrolScheme3.BuiltIn)
						{
							list2.Add(imcontrolScheme3);
						}
					}
					foreach (IMControlScheme imcontrolScheme4 in list2)
					{
						deserializedIMConfigObject2.ControlSchemes.Remove(imcontrolScheme4);
					}
					List<string> list3 = list.Select((IMControlScheme scheme) => scheme.Name).ToList<string>();
					List<string> list4 = deserializedIMConfigObject2.ControlSchemes.Select((IMControlScheme scheme) => scheme.Name).ToList<string>();
					list4.AddRange(list3);
					string text = " (Custom)";
					foreach (IMControlScheme imcontrolScheme5 in deserializedIMConfigObject2.ControlSchemes)
					{
						if (list3.Contains(imcontrolScheme5.Name))
						{
							imcontrolScheme5.Name = KMManager.GetUniqueName(imcontrolScheme5.Name + text, list4);
							list4.Add(imcontrolScheme5.Name);
						}
					}
					foreach (IMControlScheme imcontrolScheme6 in list)
					{
						deserializedIMConfigObject2.ControlSchemes.Add(imcontrolScheme6);
					}
					if (!flag)
					{
						using (List<IMControlScheme>.Enumerator enumerator = deserializedIMConfigObject2.ControlSchemes.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								IMControlScheme imcontrolScheme7 = enumerator.Current;
								if (imcontrolScheme7.BuiltIn)
								{
									if (!Opt.Instance.isUpgradeFromImap13 || !string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase))
									{
										imcontrolScheme7.Selected = false;
									}
								}
								else if (Opt.Instance.isUpgradeFromImap13 && string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase))
								{
									imcontrolScheme7.Selected = false;
								}
							}
							goto IL_0345;
						}
					}
					if (imcontrolScheme2 != null)
					{
						IMControlScheme imcontrolScheme8 = null;
						bool flag2 = false;
						foreach (IMControlScheme imcontrolScheme9 in deserializedIMConfigObject2.ControlSchemes)
						{
							if (imcontrolScheme9.BuiltIn)
							{
								if (imcontrolScheme9.Name == imcontrolScheme2.Name)
								{
									imcontrolScheme9.Selected = true;
									flag2 = true;
								}
								else if (imcontrolScheme9.Selected)
								{
									imcontrolScheme8 = imcontrolScheme9;
								}
							}
						}
						if (imcontrolScheme8 != null && flag2)
						{
							imcontrolScheme8.Selected = false;
						}
					}
					IL_0345:
					if (string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase) || PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Any((string codPckg) => string.Equals(codPckg, package, StringComparison.InvariantCultureIgnoreCase)))
					{
						Dictionary<string, IMControlScheme> dictionary = new Dictionary<string, IMControlScheme>();
						foreach (IMControlScheme imcontrolScheme10 in list2)
						{
							if (imcontrolScheme10.Images != null && imcontrolScheme10.Images.Count > 0)
							{
								string text2 = JsonConvert.SerializeObject(imcontrolScheme10.Images, Utils.GetSerializerSettings());
								dictionary.Add(text2, imcontrolScheme10);
							}
						}
						foreach (IMControlScheme imcontrolScheme11 in deserializedIMConfigObject2.ControlSchemes)
						{
							if (imcontrolScheme11.Images != null && !imcontrolScheme11.BuiltIn && imcontrolScheme11.Images.Count > 0)
							{
								string images = JsonConvert.SerializeObject(imcontrolScheme11.Images, Utils.GetSerializerSettings());
								IEnumerable<KeyValuePair<string, IMControlScheme>> prevSchemesMatchingImages = dictionary.Where((KeyValuePair<string, IMControlScheme> kvp) => string.Compare(kvp.Key, images, StringComparison.InvariantCultureIgnoreCase) == 0);
								if (prevSchemesMatchingImages.Any<KeyValuePair<string, IMControlScheme>>())
								{
									IEnumerable<IMControlScheme> enumerable = list.Where((IMControlScheme newScheme) => string.Compare(newScheme.Name, prevSchemesMatchingImages.First<KeyValuePair<string, IMControlScheme>>().Value.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
									if (enumerable.Any<IMControlScheme>() && enumerable.First<IMControlScheme>().Images != null && enumerable.First<IMControlScheme>().Images.Any<JObject>())
									{
										imcontrolScheme11.SetImages(enumerable.First<IMControlScheme>().Images);
									}
								}
							}
						}
					}
					KMManager.SaveConfigToFile(userFilesCfgPath, deserializedIMConfigObject2);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in updating control schemes err: " + ex.ToString());
			}
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x00054314 File Offset: 0x00052514
		internal static void SelectSchemeIfPresent(MainWindow window, string schemeNameToSelect, string statSource, bool forceSave)
		{
			IEnumerable<IMControlScheme> enumerable = window.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeNameToSelect, StringComparison.InvariantCulture));
			bool flag = true;
			if (enumerable.Any<IMControlScheme>())
			{
				IMControlScheme imcontrolScheme;
				if (enumerable.Count<IMControlScheme>() == 1)
				{
					imcontrolScheme = enumerable.FirstOrDefault<IMControlScheme>();
				}
				else
				{
					imcontrolScheme = enumerable.Where((IMControlScheme scheme) => !scheme.BuiltIn).FirstOrDefault<IMControlScheme>();
				}
				if (imcontrolScheme == null || (imcontrolScheme.Name == window.SelectedConfig.SelectedControlScheme.Name && !forceSave))
				{
					flag = false;
				}
				else
				{
					window.SelectedConfig.SelectedControlScheme.Selected = false;
					imcontrolScheme.Selected = true;
					window.SelectedConfig.SelectedControlScheme = imcontrolScheme;
				}
			}
			if (flag)
			{
				KeymapCanvasWindow.sIsDirty = true;
				KMManager.SaveIMActions(window, false, false);
				if (KMManager.dictOverlayWindow.ContainsKey(window) && KMManager.dictOverlayWindow[window] != null && RegistryManager.Instance.ShowKeyControlsOverlay)
				{
					KMManager.ShowOverlayWindow(window, true, true);
				}
				BlueStacksUIUtils.RefreshKeyMap(KMManager.sPackageName);
				KMManager.SendSchemeChangedStats(window, statSource);
			}
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x00054434 File Offset: 0x00052634
		private static bool IsBuiltInSchemeSelected(IMConfig prevConfig, out IMControlScheme selectedScheme)
		{
			selectedScheme = null;
			foreach (IMControlScheme imcontrolScheme in prevConfig.ControlSchemes)
			{
				if (imcontrolScheme.Selected && imcontrolScheme.BuiltIn)
				{
					selectedScheme = imcontrolScheme;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x000544A0 File Offset: 0x000526A0
		private static void ControlSchemesHandlingFromCloud(string package, string userFilesCfgPath, string inputMapperCfgPath)
		{
			try
			{
				if (File.Exists(userFilesCfgPath))
				{
					IMConfig deserializedIMConfigObject = KMManager.GetDeserializedIMConfigObject(inputMapperCfgPath, true);
					IMConfig deserializedIMConfigObject2 = KMManager.GetDeserializedIMConfigObject(userFilesCfgPath, true);
					KMManager.MergeConflictingGuidanceStrings(deserializedIMConfigObject, deserializedIMConfigObject2.ControlSchemes, deserializedIMConfigObject2.Strings);
					IEnumerable<IMControlScheme> enumerable = deserializedIMConfigObject2.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn);
					if (enumerable.Any<IMControlScheme>())
					{
						foreach (IMControlScheme imcontrolScheme in enumerable)
						{
							deserializedIMConfigObject.ControlSchemes.Add(imcontrolScheme);
						}
					}
					string selectedSchemeName = string.Empty;
					IMControlScheme imcontrolScheme2 = deserializedIMConfigObject2.ControlSchemes.Where((IMControlScheme scheme) => scheme.Selected).FirstOrDefault<IMControlScheme>();
					if (imcontrolScheme2 != null)
					{
						selectedSchemeName = imcontrolScheme2.Name;
					}
					deserializedIMConfigObject.ControlSchemes.ForEach(delegate(IMControlScheme scheme)
					{
						scheme.Selected = false;
					});
					if (imcontrolScheme2 != null)
					{
						List<IMControlScheme> list = deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, selectedSchemeName, StringComparison.InvariantCultureIgnoreCase)).ToList<IMControlScheme>();
						if (list.Count == 1)
						{
							list[0].Selected = true;
						}
						else if (list.Count == 2)
						{
							IMControlScheme imcontrolScheme3 = list.Where((IMControlScheme scheme) => !scheme.BuiltIn).FirstOrDefault<IMControlScheme>();
							if (imcontrolScheme3 != null)
							{
								imcontrolScheme3.Selected = true;
							}
						}
					}
					IEnumerable<IMControlScheme> enumerable2 = deserializedIMConfigObject2.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn);
					if (enumerable2.Any<IMControlScheme>())
					{
						using (IEnumerator<IMControlScheme> enumerator = enumerable2.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								IMControlScheme userScheme = enumerator.Current;
								IMControlScheme imcontrolScheme4 = deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn && string.Equals(scheme.Name, userScheme.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault<IMControlScheme>();
								if (imcontrolScheme4 != null)
								{
									imcontrolScheme4.IsBookMarked = userScheme.IsBookMarked;
								}
							}
						}
						if (string.Equals(package, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase) || PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Any((string codPckg) => string.Equals(codPckg, package, StringComparison.InvariantCultureIgnoreCase)))
						{
							Dictionary<string, IMControlScheme> dictionary = new Dictionary<string, IMControlScheme>();
							foreach (IMControlScheme imcontrolScheme5 in enumerable2)
							{
								if (imcontrolScheme5.Images != null && imcontrolScheme5.Images.Count > 0)
								{
									string text = JsonConvert.SerializeObject(imcontrolScheme5.Images, Utils.GetSerializerSettings());
									dictionary.Add(text, imcontrolScheme5);
								}
							}
							foreach (IMControlScheme imcontrolScheme6 in deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn))
							{
								if (imcontrolScheme6.Images != null && imcontrolScheme6.Images.Count > 0)
								{
									string text2 = JsonConvert.SerializeObject(imcontrolScheme6.Images, Utils.GetSerializerSettings());
									if (dictionary.ContainsKey(text2))
									{
										IMControlScheme userSchemeMatchingBuiltInImage = dictionary[text2];
										IMControlScheme imcontrolScheme7 = deserializedIMConfigObject.ControlSchemes.Where((IMControlScheme cloudScheme) => cloudScheme.BuiltIn && string.Equals(cloudScheme.Name, userSchemeMatchingBuiltInImage.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault<IMControlScheme>();
										if (imcontrolScheme7 != null && imcontrolScheme7.Images != null && imcontrolScheme7.Images.Any<JObject>())
										{
											imcontrolScheme6.SetImages(imcontrolScheme7.Images);
										}
									}
								}
							}
						}
					}
					KMManager.SaveConfigToFile(userFilesCfgPath, deserializedIMConfigObject);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in updating control schemes err: " + ex.ToString());
			}
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x0005491C File Offset: 0x00052B1C
		internal static string CheckForGamepadSuffix(string text)
		{
			if (text.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase) || text.Contains("LeftStick", StringComparison.InvariantCultureIgnoreCase) || text.Contains("RightStick", StringComparison.InvariantCultureIgnoreCase))
			{
				string text2 = ".";
				if (text.Contains(text2))
				{
					text = text.Substring(0, text.IndexOf(text2, StringComparison.InvariantCultureIgnoreCase));
				}
			}
			return text;
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x00054974 File Offset: 0x00052B74
		internal static string GetKeyUIValue(string text)
		{
			return string.Join(" + ", (from singleItem in text.Split(new char[] { '+' }).ToList<string>()
				select LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.CheckForGamepadSuffix(singleItem.Trim())), "")).ToArray<string>());
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x000549CC File Offset: 0x00052BCC
		internal static void ShowShootingModeTooltip(MainWindow mainWindow, string package)
		{
			if (!KMManager.CheckIfKeymappingWindowVisible(false) && KMManager.KeyMappingFilesAvailable(package) && !mainWindow.mTopBar.mAppTabButtons.SelectedTab.mIsShootingModeToastDisplayed && !KMManager.IsSelectedSchemeSmart(mainWindow) && KMManager.IsShowShootingModeTooltip(mainWindow))
			{
				string[] array = LocaleStrings.GetLocalizedString("STRING_PRESS_TO_AIM_AND_SHOOT", "").Split(new char[] { '{', '}' });
				mainWindow.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen = true;
				mainWindow.ToggleFullScreenToastVisibility(true, array[0], IMAPKeys.GetStringForUI(KMManager.sShootingModeKey), array[2]);
				mainWindow.mTopBar.mAppTabButtons.SelectedTab.mIsShootingModeToastDisplayed = true;
				mainWindow.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen = false;
			}
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x00054A9C File Offset: 0x00052C9C
		internal static void AssignEdgeScrollMode(string keyValue, global::System.Windows.Controls.TextBox keyTextBox)
		{
			string text = (Convert.ToBoolean(keyValue, CultureInfo.InvariantCulture) ? "ON" : "OFF");
			BlueStacksUIBinding.Bind(keyTextBox, Constants.ImapLocaleStringsConstant + text);
		}

		// Token: 0x040008B1 RID: 2225
		internal static KeymapCanvasWindow CanvasWindow = null;

		// Token: 0x040008B2 RID: 2226
		internal static GuidanceWindow sGuidanceWindow = null;

		// Token: 0x040008B3 RID: 2227
		internal static string mComboEvents = string.Empty;

		// Token: 0x040008B4 RID: 2228
		internal static bool sIsCancelComboClicked = false;

		// Token: 0x040008B5 RID: 2229
		internal static bool sIsSaveComboClicked = false;

		// Token: 0x040008B6 RID: 2230
		internal static bool sIsComboRecordingOn = false;

		// Token: 0x040008B7 RID: 2231
		internal static DualTextBlockControl sGamepadDualTextbox = null;

		// Token: 0x040008B8 RID: 2232
		internal static IMapTextBox CurrentIMapTextBox = null;

		// Token: 0x040008B9 RID: 2233
		internal static Dictionary<string, bool> dictGamepadEligibility = new Dictionary<string, bool>();

		// Token: 0x040008BA RID: 2234
		internal static string sShootingModeKey = "F1";

		// Token: 0x040008BB RID: 2235
		public static Dictionary<string, bool> pressedGamepadKeyList = new Dictionary<string, bool>();

		// Token: 0x040008BC RID: 2236
		public static string sGameControlsEnabledDisabledArray = string.Empty;

		// Token: 0x040008BD RID: 2237
		public static string sOldGameControlsEnabledDisabledArray = string.Empty;

		// Token: 0x040008BE RID: 2238
		public static List<List<CanvasElement>> listCanvasElement = new List<List<CanvasElement>>();

		// Token: 0x040008BF RID: 2239
		internal static bool sIsInScriptEditingMode = false;

		// Token: 0x040008C0 RID: 2240
		internal static Dictionary<MainWindow, KeymapCanvasWindow> dictOverlayWindow = new Dictionary<MainWindow, KeymapCanvasWindow>();

		// Token: 0x040008C1 RID: 2241
		internal static CanvasElement sDragCanvasElement;

		// Token: 0x040008C2 RID: 2242
		public static string ParserVersion = "17";

		// Token: 0x040008C3 RID: 2243
		internal static string sPackageName = string.Empty;

		// Token: 0x040008C4 RID: 2244
		internal static GuidanceVideoType sVideoMode = GuidanceVideoType.Default;

		// Token: 0x040008C5 RID: 2245
		internal static bool sIsDeveloperModeOn = false;

		// Token: 0x040008C6 RID: 2246
		internal static int mOnboardingCounter = 1;

		// Token: 0x040008C7 RID: 2247
		internal static bool mIsEnabledStateChanged = false;

		// Token: 0x040008C8 RID: 2248
		internal static List<OnBoardingPopupWindow> onBoardingPopupWindows = new List<OnBoardingPopupWindow>();
	}
}
