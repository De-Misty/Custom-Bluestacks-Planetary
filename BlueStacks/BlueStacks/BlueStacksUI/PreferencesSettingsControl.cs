using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001B7 RID: 439
	public class PreferencesSettingsControl : UserControl, IComponentConnector
	{
		// Token: 0x06001174 RID: 4468 RVA: 0x0006CC74 File Offset: 0x0006AE74
		public PreferencesSettingsControl(MainWindow window)
		{
			this.InitializeComponent();
			this.mChangePrefGrid.Visibility = Visibility.Visible;
			this.mChangeLocaleGrid.Visibility = Visibility.Collapsed;
			this.ParentWindow = window;
			base.Visibility = Visibility.Hidden;
			this.InitSettings();
			this.AddLanguages();
			this.AddQuitOptions();
			if (!FeatureManager.Instance.IsShowLanguagePreference)
			{
				this.mLanguageSettingsGrid.Visibility = Visibility.Collapsed;
				this.mLanguagePreferencePaddingGrid.Visibility = Visibility.Collapsed;
			}
			if (!FeatureManager.Instance.IsShowDesktopShortcutPreference)
			{
				this.mAddDesktopShortcuts.Visibility = Visibility.Collapsed;
			}
			if (!FeatureManager.Instance.IsShowGamingSummaryPreference)
			{
				this.mShowGamingSummary.Visibility = Visibility.Collapsed;
			}
			if (!FeatureManager.Instance.IsShowPerformancePreference)
			{
				this.mPerformancePreference.Visibility = Visibility.Collapsed;
				this.mPerformancePreferencePaddingGrid.Visibility = Visibility.Collapsed;
			}
			if (!FeatureManager.Instance.IsShowDiscordPreference)
			{
				this.mDiscordCheckBox.Visibility = Visibility.Collapsed;
			}
			if (!FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mQuitOptionsGrid.Visibility = Visibility.Collapsed;
			}
			this.mScrollBar.ScrollChanged += BluestacksUIColor.ScrollBarScrollChanged;
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x0006CD90 File Offset: 0x0006AF90
		private void AddQuitOptions()
		{
			ComboBoxItem comboBoxItem = null;
			foreach (string text in LocaleStringsConstants.ExitOptions)
			{
				ComboBoxItem comboBoxItem2 = new ComboBoxItem
				{
					Content = LocaleStrings.GetLocalizedString(text, ""),
					Tag = text
				};
				this.mQuitOptionsComboBox.Items.Add(comboBoxItem2);
				if (text == RegistryManager.Instance.QuitDefaultOption)
				{
					comboBoxItem = comboBoxItem2;
				}
			}
			foreach (string text2 in LocaleStringsConstants.RestartOptions)
			{
				ComboBoxItem comboBoxItem3 = new ComboBoxItem
				{
					Content = LocaleStrings.GetLocalizedString(text2, "")
				};
				this.mQuitOptionsComboBox.Items.Add(comboBoxItem3);
				comboBoxItem3.Tag = text2;
				if (text2 == RegistryManager.Instance.QuitDefaultOption)
				{
					comboBoxItem = comboBoxItem3;
				}
			}
			if (comboBoxItem == null)
			{
				this.mQuitOptionsComboBox.SelectedIndex = 0;
				return;
			}
			this.mQuitOptionsComboBox.SelectedItem = comboBoxItem;
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x0006CE80 File Offset: 0x0006B080
		private void AddLanguages()
		{
			foreach (string text in Globalization.sSupportedLocales.Keys)
			{
				ComboBoxItem comboBoxItem = new ComboBoxItem
				{
					Content = Globalization.sSupportedLocales[text].ToString(CultureInfo.InvariantCulture)
				};
				this.dictComboBoxItems.Add(comboBoxItem.Content.ToString(), comboBoxItem);
				this.mLanguageCombobox.Items.Add(comboBoxItem);
			}
			this.SelectDefaultValue();
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x0006CF20 File Offset: 0x0006B120
		private void SelectDefaultValue()
		{
			this.mLanguageCombobox.SelectionChanged -= this.mLanguageCombobox_SelectionChanged;
			string text = RegistryManager.Instance.UserSelectedLocale;
			if (string.IsNullOrEmpty(text))
			{
				text = LocaleStrings.GetLocaleName("Android", false);
				RegistryManager.Instance.UserSelectedLocale = text;
			}
			else if (!Globalization.sSupportedLocales.ContainsKey(text))
			{
				string locale = text;
				text = "en-US";
				string text2 = Globalization.sSupportedLocales.Keys.FirstOrDefault((string x) => x.StartsWith(locale.Substring(0, 2), StringComparison.InvariantCulture));
				if (!string.IsNullOrEmpty(text2))
				{
					text = text2;
				}
			}
			this.mLanguageCombobox.SelectedItem = this.dictComboBoxItems[Globalization.sSupportedLocales[text].ToString(CultureInfo.InvariantCulture)];
			this.mLanguageCombobox.SelectionChanged += this.mLanguageCombobox_SelectionChanged;
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x0006CFF8 File Offset: 0x0006B1F8
		private void InitSettings()
		{
			if (!this.ParentWindow.IsDefaultVM)
			{
				this.mAddDesktopShortcuts.Visibility = Visibility.Collapsed;
			}
			if (RegistryManager.Instance.AddDesktopShortcuts)
			{
				this.mAddDesktopShortcuts.IsChecked = new bool?(true);
			}
			if (RegistryManager.Instance.SwitchToAndroidHome)
			{
				this.mSwitchToHome.IsChecked = new bool?(true);
			}
			else
			{
				this.mSwitchToHome.IsChecked = new bool?(false);
			}
			if (RegistryManager.Instance.SwitchKillWebTab)
			{
				this.mSwitchKillWebTab.IsChecked = new bool?(true);
			}
			else
			{
				this.mSwitchKillWebTab.IsChecked = new bool?(false);
			}
			this.mEnableMemoryTrim.IsChecked = new bool?(RegistryManager.Instance.EnableMemoryTrim);
			if (RegistryManager.Instance.ShowGamingSummary)
			{
				this.mShowGamingSummary.IsChecked = new bool?(true);
			}
			else
			{
				this.mShowGamingSummary.IsChecked = new bool?(false);
			}
			if (FeatureManager.Instance.IsMacroRecorderEnabled)
			{
				this.mShowMacroDeleteWarning.IsChecked = new bool?(this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup);
			}
			else
			{
				this.mShowMacroDeleteWarning.Visibility = Visibility.Collapsed;
			}
			this.mShowSchemeDeleteWarning.IsChecked = new bool?(this.ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup);
			if (PromotionObject.Instance != null && !string.IsNullOrEmpty(PromotionObject.Instance.DiscordClientID) && this.ParentWindow.IsDefaultVM)
			{
				this.mDiscordCheckBox.Visibility = Visibility.Visible;
				if (RegistryManager.Instance.DiscordEnabled)
				{
					this.mDiscordCheckBox.IsChecked = new bool?(true);
				}
				else
				{
					this.mDiscordCheckBox.IsChecked = new bool?(false);
				}
			}
			else
			{
				this.mDiscordCheckBox.Visibility = Visibility.Collapsed;
			}
			this.mEnableGamePadCheckbox.IsChecked = new bool?(RegistryManager.Instance.GamepadDetectionEnabled);
			bool? isChecked = this.mEnableGamePadCheckbox.IsChecked;
			bool flag = true;
			if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
			{
				this.mEnableNativeGamepad.IsEnabled = true;
			}
			else
			{
				this.mEnableNativeGamepad.IsEnabled = false;
			}
			this.InitNativeGamepadSettings(false);
			if (FeatureManager.Instance.AllowADBSettingToggle)
			{
				try
				{
					if (this.ParentWindow.mGuestBootCompleted)
					{
						this.CheckIfAdbIsEnabled();
					}
					else
					{
						this.mEnableAdbCheckBox.Visibility = Visibility.Collapsed;
						this.mEnableAdbWarning.Visibility = Visibility.Collapsed;
					}
					goto IL_0289;
				}
				catch (Exception ex)
				{
					Logger.Error("Exception when initialising adb checkbox " + ex.ToString());
					this.mEnableAdbCheckBox.Visibility = Visibility.Collapsed;
					this.mEnableAdbWarning.Visibility = Visibility.Collapsed;
					goto IL_0289;
				}
			}
			this.mEnableAdbCheckBox.Visibility = Visibility.Collapsed;
			this.mEnableAdbWarning.Visibility = Visibility.Collapsed;
			IL_0289:
			if (FeatureManager.Instance.IsShowAndroidInputDebugSetting)
			{
				try
				{
					if (this.ParentWindow.mGuestBootCompleted)
					{
						this.CheckIfAndroidTouchPointsEnabled();
					}
					else
					{
						this.mInputGrid.Visibility = Visibility.Collapsed;
					}
					goto IL_02E8;
				}
				catch (Exception ex2)
				{
					Logger.Error("Exception when initialising android input debugging checkbox " + ex2.ToString());
					this.mInputGrid.Visibility = Visibility.Collapsed;
					goto IL_02E8;
				}
			}
			this.mInputGrid.Visibility = Visibility.Collapsed;
			IL_02E8:
			if (StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath))
			{
				this.mScreenShotPathLable.Text = RegistryManager.Instance.ScreenShotsPath;
			}
			else
			{
				try
				{
					string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Strings.ProductTopBarDisplayName);
					Logger.Info("Path to save screenshots and recording is ", new object[] { text });
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					RegistryManager.Instance.ScreenShotsPath = text;
					this.mScreenShotPathLable.Text = text;
				}
				catch (Exception ex3)
				{
					Logger.Error("Exception while creating picutres directory: " + ex3.ToString());
				}
			}
			if (!RegistryManager.Instance.Guest[this.ParentWindow.mVmName].IsGoogleSigninDone)
			{
				this.mLanguageSettingsGrid.Visibility = Visibility.Collapsed;
				this.mLanguagePreferencePaddingGrid.Visibility = Visibility.Collapsed;
			}
			this.mShowOnExitCheckbox.IsChecked = new bool?(!RegistryManager.Instance.IsQuitOptionSaved);
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x0006D404 File Offset: 0x0006B604
		private void InitNativeGamepadSettings(bool isUpdate = false)
		{
			string text = "false";
			switch (this.ParentWindow.EngineInstanceRegistry.NativeGamepadState)
			{
			case NativeGamepadState.ForceOn:
				this.mForcedOnMode.IsChecked = new bool?(true);
				text = "true";
				this.mNativeGamepadInfo.Visibility = Visibility.Visible;
				break;
			case NativeGamepadState.ForceOff:
				this.mForcedOffMode.IsChecked = new bool?(true);
				text = "false";
				this.mNativeGamepadInfo.Visibility = Visibility.Collapsed;
				break;
			case NativeGamepadState.Auto:
				this.mAutoMode.IsChecked = new bool?(true);
				if (this.ParentWindow.mGuestBootCompleted)
				{
					text = this.ParentWindow.mCommonHandler.CheckNativeGamepadState(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName).ToString(CultureInfo.InvariantCulture);
				}
				this.mNativeGamepadInfo.Visibility = Visibility.Collapsed;
				break;
			}
			if (isUpdate)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { { "isEnabled", text } };
				this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", dictionary);
			}
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x0006D51C File Offset: 0x0006B71C
		private void CheckIfAdbIsEnabled()
		{
			string text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_ADB_CONNECTED_PORT_0", ""), new object[] { this.ParentWindow.EngineInstanceRegistry.BstAdbPort });
			this.mEnableAdbWarning.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				LocaleStrings.GetLocalizedString("STRING_ENABLE_ADB_WARNING", ""),
				text
			});
			JObject jobject = JsonConvert.DeserializeObject(HTTPUtils.SendRequestToGuest("checkADBStatus", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64"), Utils.GetSerializerSettings()) as JObject;
			if (string.Compare("ok", jobject["result"].Value<string>(), StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.mEnableAdbCheckBox.IsChecked = new bool?(true);
			}
			else
			{
				this.mEnableAdbCheckBox.IsChecked = new bool?(false);
			}
			this.mEnableAdbCheckBox.Visibility = Visibility.Visible;
			this.mEnableAdbWarning.Visibility = Visibility.Visible;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x0006D620 File Offset: 0x0006B820
		private void CheckIfAndroidTouchPointsEnabled()
		{
			JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("checkTouchPointState", null, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64"));
			string text = jobject["touchPoint"].ToString().Trim();
			string text2 = jobject["pointerLocation"].ToString().Trim();
			this.mEnableTouchPointsCheckBox.IsChecked = new bool?(string.Equals("enabled", text, StringComparison.InvariantCultureIgnoreCase));
			this.mEnableTouchCoordinatesCheckbox.IsChecked = new bool?(string.Equals("enabled", text2, StringComparison.InvariantCultureIgnoreCase));
			this.mInputGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x0006D6C4 File Offset: 0x0006B8C4
		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			this.mChangeLocaleGrid.Visibility = Visibility.Collapsed;
			this.mChangePrefGrid.Visibility = Visibility.Visible;
			CustomCheckbox customCheckbox = sender as CustomCheckbox;
			if (customCheckbox == this.mAddDesktopShortcuts)
			{
				bool flag = true;
				bool? flag2 = this.mAddDesktopShortcuts.IsChecked;
				if ((flag == flag2.GetValueOrDefault()) & (flag2 != null))
				{
					RegistryManager.Instance.AddDesktopShortcuts = true;
					return;
				}
				RegistryManager.Instance.AddDesktopShortcuts = false;
				return;
			}
			else
			{
				bool? flag2;
				bool flag3;
				if (customCheckbox != this.mShowGamingSummary)
				{
					if (customCheckbox == this.mDiscordCheckBox)
					{
						flag2 = this.mDiscordCheckBox.IsChecked;
						flag3 = true;
						if (!((flag2.GetValueOrDefault() == flag3) & (flag2 != null)))
						{
							RegistryManager.Instance.DiscordEnabled = false;
							if (this.ParentWindow.mDiscordhandler != null)
							{
								this.ParentWindow.mDiscordhandler.ToggleDiscordState(false);
							}
							this.ParentWindow.mDiscordhandler = null;
							return;
						}
						RegistryManager.Instance.DiscordEnabled = true;
						if (this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted && this.ParentWindow.mGuestBootCompleted)
						{
							if (this.ParentWindow.mDiscordhandler == null)
							{
								this.ParentWindow.InitDiscord();
								return;
							}
							this.ParentWindow.mDiscordhandler.ToggleDiscordState(true);
							return;
						}
					}
					else if (customCheckbox == this.mEnableAdbCheckBox)
					{
						flag2 = this.mEnableAdbCheckBox.IsChecked;
						flag3 = true;
						HTTPUtils.SendRequestToGuestAsync(((flag2.GetValueOrDefault() == flag3) & (flag2 != null)) ? "connectHost?d=permanent" : "disconnectHost?d=permanent", null, this.ParentWindow.mVmName, 0, null, false, 1, 0);
						flag2 = this.mEnableAdbCheckBox.IsChecked;
						flag3 = true;
						if (((flag2.GetValueOrDefault() == flag3) & (flag2 != null)) && SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.ParentWindow.mVmName))
						{
							SecurityMetrics.SecurityMetricsInstanceList[this.ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_PROBED, "");
							return;
						}
					}
					else
					{
						if (customCheckbox == this.mEnableTouchPointsCheckBox)
						{
							flag2 = this.mEnableTouchPointsCheckBox.IsChecked;
							flag3 = true;
							string text = (((flag2.GetValueOrDefault() == flag3) & (flag2 != null)) ? "enable" : "disable");
							flag2 = this.mEnableTouchCoordinatesCheckbox.IsChecked;
							flag3 = true;
							string text2 = (((flag2.GetValueOrDefault() == flag3) & (flag2 != null)) ? "enable" : "disable");
							string text3 = "{";
							text3 += string.Format(CultureInfo.InvariantCulture, "\"touchPoint\":\"{0}\",", new object[] { text });
							text3 += string.Format(CultureInfo.InvariantCulture, "\"pointerLocation\":\"{0}\"", new object[] { text2 });
							text3 += "}";
							Dictionary<string, string> dictionary = new Dictionary<string, string> { { "data", text3 } };
							HTTPUtils.SendRequestToGuest("showTouchPoints", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
							return;
						}
						if (customCheckbox == this.mEnableTouchCoordinatesCheckbox)
						{
							flag2 = this.mEnableTouchPointsCheckBox.IsChecked;
							flag3 = true;
							string text4 = (((flag2.GetValueOrDefault() == flag3) & (flag2 != null)) ? "enable" : "disable");
							flag2 = this.mEnableTouchCoordinatesCheckbox.IsChecked;
							flag3 = true;
							string text5 = (((flag2.GetValueOrDefault() == flag3) & (flag2 != null)) ? "enable" : "disable");
							string text6 = "{";
							text6 += string.Format(CultureInfo.InvariantCulture, "\"touchPoint\":\"{0}\",", new object[] { text4 });
							text6 += string.Format(CultureInfo.InvariantCulture, "\"pointerLocation\":\"{0}\"", new object[] { text5 });
							text6 += "}";
							Dictionary<string, string> dictionary2 = new Dictionary<string, string> { { "data", text6 } };
							HTTPUtils.SendRequestToGuest("showTouchPoints", dictionary2, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64");
							return;
						}
						if (customCheckbox == this.mShowMacroDeleteWarning)
						{
							this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = this.mShowMacroDeleteWarning.IsChecked.Value;
							return;
						}
						if (customCheckbox == this.mShowSchemeDeleteWarning)
						{
							this.ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup = this.mShowSchemeDeleteWarning.IsChecked.Value;
							return;
						}
						if (customCheckbox == this.mEnableGamePadCheckbox)
						{
							flag2 = this.mEnableGamePadCheckbox.IsChecked;
							flag3 = true;
							if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
							{
								RegistryManager.Instance.GamepadDetectionEnabled = true;
								this.mEnableNativeGamepad.IsEnabled = true;
								this.InitNativeGamepadSettings(true);
							}
							else
							{
								RegistryManager.Instance.GamepadDetectionEnabled = false;
								Dictionary<string, string> dictionary3 = new Dictionary<string, string> { 
								{
									"isEnabled",
									RegistryManager.Instance.GamepadDetectionEnabled.ToString(CultureInfo.InvariantCulture)
								} };
								this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", dictionary3);
								this.mEnableNativeGamepad.IsEnabled = false;
							}
							Dictionary<string, string> dictionary4 = new Dictionary<string, string> { 
							{
								"enable",
								RegistryManager.Instance.GamepadDetectionEnabled.ToString(CultureInfo.InvariantCulture)
							} };
							this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableGamepad", dictionary4);
						}
					}
					return;
				}
				ClientStats.SendMiscellaneousStatsAsync("gamingSummaryCheckboxClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "checked" + this.mShowGamingSummary.IsChecked.ToString(), null, null, null, null, null);
				flag2 = this.mShowGamingSummary.IsChecked;
				flag3 = true;
				if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
				{
					RegistryManager.Instance.ShowGamingSummary = true;
					return;
				}
				RegistryManager.Instance.ShowGamingSummary = false;
				return;
			}
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x0006DC68 File Offset: 0x0006BE68
		private void mLanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				string selectedLocale = (this.mLanguageCombobox.SelectedItem as ComboBoxItem).Content.ToString();
				if (selectedLocale != null)
				{
					this.mChangePrefGrid.Visibility = Visibility.Visible;
					string key = Globalization.sSupportedLocales.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == selectedLocale).Key;
					if (!string.Equals(RegistryManager.Instance.UserSelectedLocale, key, StringComparison.InvariantCultureIgnoreCase))
					{
						RegistryManager.Instance.UserSelectedLocale = key;
						BlueStacksUIUtils.UpdateLocale(key, "");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in set locale" + ex.ToString());
			}
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x0006DD24 File Offset: 0x0006BF24
		private void mSwitchToHome_Click(object sender, RoutedEventArgs e)
		{
			bool? isChecked = this.mSwitchToHome.IsChecked;
			bool flag = true;
			if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
			{
				RegistryManager.Instance.SwitchToAndroidHome = true;
				return;
			}
			RegistryManager.Instance.SwitchToAndroidHome = false;
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x0006DD6C File Offset: 0x0006BF6C
		private void SwitchKillWebTab_Click(object sender, RoutedEventArgs e)
		{
			bool? isChecked = this.mSwitchKillWebTab.IsChecked;
			bool flag = true;
			if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
			{
				RegistryManager.Instance.SwitchKillWebTab = true;
				return;
			}
			RegistryManager.Instance.SwitchKillWebTab = false;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x0006DDB4 File Offset: 0x0006BFB4
		private void mChangePathBtn_Click(object sender, RoutedEventArgs e)
		{
			string text = this.mScreenShotPathLable.Text;
			this.ParentWindow.mCommonHandler.ShowFolderBrowserDialog(text);
			this.mScreenShotPathLable.Text = RegistryManager.Instance.ScreenShotsPath;
			ClientStats.SendMiscellaneousStatsAsync("MediaFilesPathSet", RegistryManager.Instance.UserGuid, "PathChangeFromPreferences", text, RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null);
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x0000C75F File Offset: 0x0000A95F
		private void MQuitOptionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RegistryManager.Instance.QuitDefaultOption = (this.mQuitOptionsComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x0006DE38 File Offset: 0x0006C038
		private void MShowOnExitCheckbox_Click(object sender, RoutedEventArgs e)
		{
			CustomCheckbox customCheckbox = sender as CustomCheckbox;
			RegistryManager.Instance.IsQuitOptionSaved = !customCheckbox.IsChecked.Value;
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x0000C785 File Offset: 0x0000A985
		private void HelpIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=native_gamepad_help");
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x0006DE68 File Offset: 0x0006C068
		private void NativeGamepadMode_Click(object sender, RoutedEventArgs e)
		{
			string text = string.Empty;
			string name = (sender as CustomRadioButton).Name;
			if (name != null)
			{
				if (!(name == "mForcedOnMode"))
				{
					if (!(name == "mForcedOffMode"))
					{
						if (name == "mAutoMode")
						{
							if (this.ParentWindow.mGuestBootCompleted)
							{
								text = this.ParentWindow.mCommonHandler.CheckNativeGamepadState(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName).ToString(CultureInfo.InvariantCulture);
							}
							this.ParentWindow.EngineInstanceRegistry.NativeGamepadState = NativeGamepadState.Auto;
							this.mNativeGamepadInfo.Visibility = Visibility.Collapsed;
							ClientStats.SendMiscellaneousStatsAsync("GamepadModeChanged", RegistryManager.Instance.UserGuid, "Auto", "SettingsWindow", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
						}
					}
					else
					{
						text = "false";
						this.ParentWindow.EngineInstanceRegistry.NativeGamepadState = NativeGamepadState.ForceOff;
						this.mNativeGamepadInfo.Visibility = Visibility.Collapsed;
						ClientStats.SendMiscellaneousStatsAsync("GamepadModeChanged", RegistryManager.Instance.UserGuid, "ForcedOff", "SettingsWindow", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
					}
				}
				else
				{
					text = RegistryManager.Instance.GamepadDetectionEnabled.ToString(CultureInfo.InvariantCulture);
					this.ParentWindow.EngineInstanceRegistry.NativeGamepadState = NativeGamepadState.ForceOn;
					this.mNativeGamepadInfo.Visibility = Visibility.Visible;
					ClientStats.SendMiscellaneousStatsAsync("GamepadModeChanged", RegistryManager.Instance.UserGuid, "ForcedOn", "SettingsWindow", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "isEnabled", text } };
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", dictionary);
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x0006E068 File Offset: 0x0006C268
		private void EnableMemoryTrim_Click(object sender, RoutedEventArgs e)
		{
			RegistryManager instance = RegistryManager.Instance;
			bool? flag = this.mEnableMemoryTrim.IsChecked;
			bool flag2 = true;
			instance.EnableMemoryTrim = (flag.GetValueOrDefault() == flag2) & (flag != null);
			flag = this.mEnableMemoryTrim.IsChecked;
			flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				foreach (string text in BlueStacksUIUtils.DictWindows.Keys.ToList<string>())
				{
					HTTPUtils.SendRequestToEngineAsync("enableMemoryTrim", null, text, 0, null, false, 1, 0);
				}
			}
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x0006E11C File Offset: 0x0006C31C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/preferencessettingscontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x0006E14C File Offset: 0x0006C34C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.mScrollBar = (ScrollViewer)target;
				return;
			case 2:
				this.mMainGrid = (Grid)target;
				return;
			case 3:
				this.mLanguageSettingsGrid = (Grid)target;
				return;
			case 4:
				this.mLanguageCombobox = (CustomComboBox)target;
				this.mLanguageCombobox.SelectionChanged += this.mLanguageCombobox_SelectionChanged;
				return;
			case 5:
				this.mLanguagePreferencePaddingGrid = (Grid)target;
				return;
			case 6:
				this.mPerformancePreference = (Grid)target;
				return;
			case 7:
				this.mPerformanceSettingsLabel = (Label)target;
				return;
			case 8:
				this.mSwitchToHome = (CustomCheckbox)target;
				this.mSwitchToHome.Click += this.mSwitchToHome_Click;
				return;
			case 9:
				this.mSwitchKillWebTab = (CustomCheckbox)target;
				this.mSwitchKillWebTab.Click += this.SwitchKillWebTab_Click;
				return;
			case 10:
				this.mEnableMemoryTrim = (CustomCheckbox)target;
				this.mEnableMemoryTrim.Click += this.EnableMemoryTrim_Click;
				return;
			case 11:
				this.mEnableMemoryTrimWarning = (TextBlock)target;
				return;
			case 12:
				this.mGameControlPreferencePaddingGrid = (Grid)target;
				return;
			case 13:
				this.mGameControlsSettingsGrid = (Grid)target;
				return;
			case 14:
				this.mGameControlSettingsLabel = (Label)target;
				return;
			case 15:
				this.mGameControlsStackPanel = (StackPanel)target;
				return;
			case 16:
				this.mEnableGamePadCheckbox = (CustomCheckbox)target;
				this.mEnableGamePadCheckbox.Click += this.CheckBox_Click;
				return;
			case 17:
				this.mHelpIcon = (CustomPictureBox)target;
				this.mHelpIcon.PreviewMouseLeftButtonUp += this.HelpIconPreviewMouseLeftButtonUp;
				return;
			case 18:
				this.mEnableNativeGamepad = (Grid)target;
				return;
			case 19:
				this.mForcedOnMode = (CustomRadioButton)target;
				this.mForcedOnMode.Click += this.NativeGamepadMode_Click;
				return;
			case 20:
				this.mForcedOffMode = (CustomRadioButton)target;
				this.mForcedOffMode.Click += this.NativeGamepadMode_Click;
				return;
			case 21:
				this.mAutoMode = (CustomRadioButton)target;
				this.mAutoMode.Click += this.NativeGamepadMode_Click;
				return;
			case 22:
				this.mNativeGamepadInfo = (Grid)target;
				return;
			case 23:
				this.mShowSchemeDeleteWarning = (CustomCheckbox)target;
				this.mShowSchemeDeleteWarning.Click += this.CheckBox_Click;
				return;
			case 24:
				this.mPerformancePreferencePaddingGrid = (Grid)target;
				return;
			case 25:
				this.mPlatformStackPanel = (StackPanel)target;
				return;
			case 26:
				this.mAddDesktopShortcuts = (CustomCheckbox)target;
				this.mAddDesktopShortcuts.Click += this.CheckBox_Click;
				return;
			case 27:
				this.mShowGamingSummary = (CustomCheckbox)target;
				this.mShowGamingSummary.Click += this.CheckBox_Click;
				return;
			case 28:
				this.mShowMacroDeleteWarning = (CustomCheckbox)target;
				this.mShowMacroDeleteWarning.Click += this.CheckBox_Click;
				return;
			case 29:
				this.mDiscordCheckBox = (CustomCheckbox)target;
				this.mDiscordCheckBox.Click += this.CheckBox_Click;
				return;
			case 30:
				this.mEnableAdbCheckBox = (CustomCheckbox)target;
				this.mEnableAdbCheckBox.Click += this.CheckBox_Click;
				return;
			case 31:
				this.mEnableAdbWarning = (TextBlock)target;
				return;
			case 32:
				this.mInputGrid = (Grid)target;
				return;
			case 33:
				this.mEnableTouchPointsCheckBox = (CustomCheckbox)target;
				this.mEnableTouchPointsCheckBox.Click += this.CheckBox_Click;
				return;
			case 34:
				this.mEnableTouchCoordinatesCheckbox = (CustomCheckbox)target;
				this.mEnableTouchCoordinatesCheckbox.Click += this.CheckBox_Click;
				return;
			case 35:
				this.mEnableDevSettingsWarning = (TextBlock)target;
				return;
			case 36:
				this.mQuitOptionsGrid = (Grid)target;
				return;
			case 37:
				this.mQuitOptionsComboBox = (CustomComboBox)target;
				this.mQuitOptionsComboBox.SelectionChanged += this.MQuitOptionsComboBox_SelectionChanged;
				return;
			case 38:
				this.mShowOnExitCheckbox = (CustomCheckbox)target;
				this.mShowOnExitCheckbox.Click += this.MShowOnExitCheckbox_Click;
				return;
			case 39:
				this.mScreenshotGrid = (Grid)target;
				return;
			case 40:
				this.mScreenShotPathLable = (TextBlock)target;
				return;
			case 41:
				this.mChangePathBtn = (CustomButton)target;
				this.mChangePathBtn.Click += this.mChangePathBtn_Click;
				return;
			case 42:
				this.mChangeLocaleGrid = (Grid)target;
				return;
			case 43:
				this.mInfoIconLocale = (CustomPictureBox)target;
				return;
			case 44:
				this.mChangePrefGrid = (Grid)target;
				return;
			case 45:
				this.mInfoIcon = (CustomPictureBox)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000B4C RID: 2892
		private Dictionary<string, ComboBoxItem> dictComboBoxItems = new Dictionary<string, ComboBoxItem>();

		// Token: 0x04000B4D RID: 2893
		private MainWindow ParentWindow;

		// Token: 0x04000B4E RID: 2894
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mScrollBar;

		// Token: 0x04000B4F RID: 2895
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMainGrid;

		// Token: 0x04000B50 RID: 2896
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mLanguageSettingsGrid;

		// Token: 0x04000B51 RID: 2897
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomComboBox mLanguageCombobox;

		// Token: 0x04000B52 RID: 2898
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mLanguagePreferencePaddingGrid;

		// Token: 0x04000B53 RID: 2899
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mPerformancePreference;

		// Token: 0x04000B54 RID: 2900
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mPerformanceSettingsLabel;

		// Token: 0x04000B55 RID: 2901
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mSwitchToHome;

		// Token: 0x04000B56 RID: 2902
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mSwitchKillWebTab;

		// Token: 0x04000B57 RID: 2903
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mEnableMemoryTrim;

		// Token: 0x04000B58 RID: 2904
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mEnableMemoryTrimWarning;

		// Token: 0x04000B59 RID: 2905
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGameControlPreferencePaddingGrid;

		// Token: 0x04000B5A RID: 2906
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGameControlsSettingsGrid;

		// Token: 0x04000B5B RID: 2907
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Label mGameControlSettingsLabel;

		// Token: 0x04000B5C RID: 2908
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mGameControlsStackPanel;

		// Token: 0x04000B5D RID: 2909
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mEnableGamePadCheckbox;

		// Token: 0x04000B5E RID: 2910
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mHelpIcon;

		// Token: 0x04000B5F RID: 2911
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mEnableNativeGamepad;

		// Token: 0x04000B60 RID: 2912
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mForcedOnMode;

		// Token: 0x04000B61 RID: 2913
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mForcedOffMode;

		// Token: 0x04000B62 RID: 2914
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mAutoMode;

		// Token: 0x04000B63 RID: 2915
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mNativeGamepadInfo;

		// Token: 0x04000B64 RID: 2916
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mShowSchemeDeleteWarning;

		// Token: 0x04000B65 RID: 2917
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mPerformancePreferencePaddingGrid;

		// Token: 0x04000B66 RID: 2918
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mPlatformStackPanel;

		// Token: 0x04000B67 RID: 2919
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mAddDesktopShortcuts;

		// Token: 0x04000B68 RID: 2920
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mShowGamingSummary;

		// Token: 0x04000B69 RID: 2921
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mShowMacroDeleteWarning;

		// Token: 0x04000B6A RID: 2922
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mDiscordCheckBox;

		// Token: 0x04000B6B RID: 2923
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mEnableAdbCheckBox;

		// Token: 0x04000B6C RID: 2924
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mEnableAdbWarning;

		// Token: 0x04000B6D RID: 2925
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mInputGrid;

		// Token: 0x04000B6E RID: 2926
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mEnableTouchPointsCheckBox;

		// Token: 0x04000B6F RID: 2927
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mEnableTouchCoordinatesCheckbox;

		// Token: 0x04000B70 RID: 2928
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mEnableDevSettingsWarning;

		// Token: 0x04000B71 RID: 2929
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mQuitOptionsGrid;

		// Token: 0x04000B72 RID: 2930
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomComboBox mQuitOptionsComboBox;

		// Token: 0x04000B73 RID: 2931
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mShowOnExitCheckbox;

		// Token: 0x04000B74 RID: 2932
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mScreenshotGrid;

		// Token: 0x04000B75 RID: 2933
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mScreenShotPathLable;

		// Token: 0x04000B76 RID: 2934
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mChangePathBtn;

		// Token: 0x04000B77 RID: 2935
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mChangeLocaleGrid;

		// Token: 0x04000B78 RID: 2936
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mInfoIconLocale;

		// Token: 0x04000B79 RID: 2937
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mChangePrefGrid;

		// Token: 0x04000B7A RID: 2938
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mInfoIcon;

		// Token: 0x04000B7B RID: 2939
		private bool _contentLoaded;
	}
}
