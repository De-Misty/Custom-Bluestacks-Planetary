using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000114 RID: 276
	public class SettingsWindow : SettingsWindowBase
	{
		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000B91 RID: 2961 RVA: 0x0000951A File Offset: 0x0000771A
		public MainWindow ParentWindow { get; }

		// Token: 0x06000B92 RID: 2962 RVA: 0x000404FC File Offset: 0x0003E6FC
		public SettingsWindow(MainWindow window, string startUpTab)
		{
			do
			{
				this.mDuplicateShortcutsList = new List<string>();
				this.mSettingsButtons = new Dictionary<string, CustomSettingsButton>();
				base..ctor();
				SettingsWindow <>4__this = this;
				this.ParentWindow = window;
				base.SettingsControlNameList.Add("STRING_DISPLAY_SETTINGS");
				base.SettingsControlNameList.Add("STRING_ENGINE_SETTING");
			}
			while (window == null);
			if (CS$<>8__locals1.window.mGuestBootCompleted)
			{
			}
			if (CS$<>8__locals1.window.mCommonHandler.mShortcutsConfigInstance != null)
			{
			}
			Logger.Warning("Not showing shortcuts settings as the config instance is null");
			base.SettingsControlNameList.Add("STRING_ABOUT_SETTING");
			this.UpdateSettingsListAndStartTabForCustomOEMs();
			base.Loaded += delegate(object sender, RoutedEventArgs e)
			{
				CS$<>8__locals1.<>4__this.SettingsWindow_Loaded(CS$<>8__locals1.window);
			};
			if (!string.IsNullOrEmpty(startUpTab))
			{
			}
			this.CreateAllButtons(base.StartUpTab);
			this.ChangeSettingsTab(CS$<>8__locals1.window, base.StartUpTab);
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x000405E4 File Offset: 0x0003E7E4
		public void ChangeSettingsTab(MainWindow window, string tab)
		{
			UserControl userControl = this.GetUserControl(tab, window);
			if (userControl == null)
			{
				userControl = this.GetUserControl("STRING_DISPLAY_SETTINGS", window);
			}
			base.AddControlInGridAndDict(tab, userControl);
			base.BringToFront(userControl);
			if (!this.mSettingsButtons[tab].IsSelected)
			{
				this.mSettingsButtons[tab].IsSelected = true;
				this.mSettingsButtons[tab].IsEnabled = true;
			}
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x00040650 File Offset: 0x0003E850
		public void UpdateSettingsListAndStartTabForCustomOEMs()
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				base.SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_SCREENSHOT" };
				return;
			}
			if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
			{
				base.SettingsControlNameList = new List<string> { "STRING_ABOUT_SETTING" };
				base.StartUpTab = "STRING_ABOUT_SETTING";
				return;
			}
			if (string.Equals(Oem.Instance.OEM, "yoozoo", StringComparison.InvariantCulture))
			{
				base.SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_PREFERENCES" };
				return;
			}
			if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
			{
				base.SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_ABOUT_SETTING" };
				return;
			}
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				base.SettingsControlNameList = new List<string> { "STRING_DISPLAY_SETTINGS", "STRING_ENGINE_SETTING", "STRING_PREFERENCES", "STRING_SHORTCUT_KEY_SETTINGS", "STRING_USER_DATA_SETTINGS" };
			}
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x00040790 File Offset: 0x0003E990
		private UserControl GetUserControl(string controlName, MainWindow window)
		{
			if (controlName != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(controlName);
				if (num <= 2851124811U)
				{
					if (num <= 1158533478U)
					{
						if (num != 453138840U)
						{
							if (num == 1158533478U)
							{
								if (controlName == "STRING_NOTIFICATION")
								{
									return new NotificationsSettings(window);
								}
							}
						}
						else if (controlName == "STRING_ENGINE_SETTING")
						{
							return SettingsWindow.GetEngineView(window);
						}
					}
					else if (num != 2256989329U)
					{
						if (num != 2395752955U)
						{
							if (num == 2851124811U)
							{
								if (controlName == "STRING_DISPLAY_SETTINGS")
								{
									return new DisplaySettingsControl(window);
								}
							}
						}
						else if (controlName == "STRING_ADVANCED")
						{
							return new DeviceProfileControl(window);
						}
					}
					else if (controlName == "STRING_SCREENSHOT")
					{
						return new DMMScreenshotSettingControl(window);
					}
				}
				else if (num <= 3066349429U)
				{
					if (num != 2925263117U)
					{
						if (num == 3066349429U)
						{
							if (controlName == "STRING_GAME_SETTINGS")
							{
								return SettingsWindow.GetGameSettingView(window);
							}
						}
					}
					else if (controlName == "STRING_USER_DATA_SETTINGS")
					{
						return new BackupRestoreSettingsControl(window);
					}
				}
				else if (num != 3082043033U)
				{
					if (num != 3350936637U)
					{
						if (num == 3467783225U)
						{
							if (controlName == "STRING_SHORTCUT_KEY_SETTINGS")
							{
								return new ShortcutKeysControl(window, this);
							}
						}
					}
					else if (controlName == "STRING_PREFERENCES")
					{
						return new PreferencesSettingsControl(window);
					}
				}
				else if (controlName == "STRING_ABOUT_SETTING")
				{
					return new AboutSettingsControl(window, this);
				}
			}
			return null;
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x00040954 File Offset: 0x0003EB54
		private static GameSettingView GetGameSettingView(MainWindow window)
		{
			GameSettingViewModel gameSettingViewModel = new GameSettingViewModel(window);
			GameSettingView gameSettingView = new GameSettingView
			{
				Visibility = Visibility.Collapsed,
				DataContext = gameSettingViewModel
			};
			gameSettingViewModel.View = gameSettingView;
			return gameSettingView;
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x00040984 File Offset: 0x0003EB84
		private static EngineSettingBase GetEngineView(MainWindow window)
		{
			EngineSettingBase engineSettingBase = new EngineSettingBase
			{
				Visibility = Visibility.Collapsed
			};
			EngineSettingViewModel engineSettingViewModel = new EngineSettingViewModel(window, window.mVmName, engineSettingBase);
			engineSettingBase.DataContext = engineSettingViewModel;
			return engineSettingBase;
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x000409B4 File Offset: 0x0003EBB4
		private void SettingsWindow_Loaded(MainWindow window)
		{
			Window.GetWindow(this).Closing += this.SettingWindow_Closing;
			new Thread(delegate
			{
				Thread.Sleep(500);
				using (List<string>.Enumerator enumerator = this.SettingsControlNameList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string settingName = enumerator.Current;
						if (!string.Equals(settingName, this.StartUpTab, StringComparison.InvariantCulture))
						{
							this.Dispatcher.Invoke(new Action(delegate
							{
								UserControl userControl = this.GetUserControl(settingName, window);
								if (userControl != null)
								{
									this.AddControlInGridAndDict(settingName, userControl);
									foreach (object obj in this.SettingsWindowStackPanel.Children)
									{
										CustomSettingsButton customSettingsButton = (CustomSettingsButton)obj;
										if (customSettingsButton.Name == settingName)
										{
											customSettingsButton.IsEnabled = true;
										}
									}
								}
							}), new object[0]);
						}
					}
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x00040A08 File Offset: 0x0003EC08
		private void SettingWindow_Closing(object sender, CancelEventArgs e)
		{
			try
			{
				MainWindow.CloseSettingsWindow(null);
				if (this.mIsShortcutEdited && this.mIsShortcutSaveBtnEnabled)
				{
					CommonHandlers.ReloadShortcutsForAllInstances();
				}
			}
			catch (Exception ex)
			{
				string text = "Exception in SettingsWindowClosing. Exception: ";
				Exception ex2 = ex;
				Logger.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x00040A64 File Offset: 0x0003EC64
		private void CreateAllButtons(string mstartUpTab)
		{
			foreach (string text in base.SettingsControlNameList)
			{
				CustomSettingsButton customSettingsButton = new CustomSettingsButton
				{
					Name = text,
					Group = "Settings"
				};
				this.mSettingsButtons.Add(text, customSettingsButton);
				TextBlock textBlock = new TextBlock
				{
					FontSize = 15.0,
					TextWrapping = TextWrapping.Wrap
				};
				BlueStacksUIBinding.Bind(textBlock, text, "");
				customSettingsButton.Content = textBlock;
				customSettingsButton.MinHeight = 40.0;
				customSettingsButton.FontWeight = FontWeights.Normal;
				customSettingsButton.IsTabStop = false;
				customSettingsButton.FocusVisualStyle = null;
				customSettingsButton.IsEnabled = false;
				customSettingsButton.PreviewMouseDown += this.ValidateAndSwitchTab;
				base.SettingsWindowStackPanel.Children.Add(customSettingsButton);
				if (mstartUpTab == text)
				{
					customSettingsButton.IsEnabled = true;
					customSettingsButton.IsSelected = true;
				}
			}
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x00040B74 File Offset: 0x0003ED74
		private void ValidateAndSwitchTab(object sender, MouseButtonEventArgs args)
		{
			SettingsWindow.<>c__DisplayClass18_0 CS$<>8__locals1 = new SettingsWindow.<>c__DisplayClass18_0();
			CS$<>8__locals1.args = args;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.sender = sender;
			CustomSettingsButton customSettingsButton = CS$<>8__locals1.sender as CustomSettingsButton;
			if (base.SettingsWindowControlsDict[customSettingsButton.Name].GetType() == base.visibleControl.GetType())
			{
				return;
			}
			EngineSettingBase engineSettingBase = base.visibleControl as EngineSettingBase;
			if (engineSettingBase != null)
			{
				EngineSettingBaseViewModel engineSettingBaseViewModel = engineSettingBase.DataContext as EngineSettingBaseViewModel;
				if (engineSettingBaseViewModel != null)
				{
					if (engineSettingBaseViewModel.Status == Status.Progress)
					{
						Logger.Info("Compatibility check is running");
						return;
					}
					if (engineSettingBaseViewModel.IsDirty())
					{
						CustomMessageWindow customMessageWindow = new CustomMessageWindow();
						customMessageWindow.Owner = engineSettingBaseViewModel.Owner;
						customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
						BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
						BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
						customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", delegate(object o, EventArgs e)
						{
							CS$<>8__locals1.args.Handled = true;
						}, null, false, null);
						customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs e)
						{
							CS$<>8__locals1.<>4__this.SettingsBtn_Click(CS$<>8__locals1.sender, null);
						}, null, false, null);
						customMessageWindow.ShowDialog();
						return;
					}
					base.SettingsBtn_Click(CS$<>8__locals1.sender, null);
					return;
				}
			}
			SettingsWindow.<>c__DisplayClass18_1 CS$<>8__locals2 = new SettingsWindow.<>c__DisplayClass18_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			UserControl userControl = base.visibleControl;
			CS$<>8__locals2.displaySetting = userControl as DisplaySettingsControl;
			if (CS$<>8__locals2.displaySetting != null && CS$<>8__locals2.displaySetting.IsDirty())
			{
				CustomMessageWindow customMessageWindow2 = new CustomMessageWindow();
				customMessageWindow2.Owner = CS$<>8__locals2.displaySetting.ParentWindow;
				customMessageWindow2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				BlueStacksUIBinding.Bind(customMessageWindow2.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
				BlueStacksUIBinding.Bind(customMessageWindow2.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
				customMessageWindow2.AddButton(ButtonColors.Blue, "STRING_NO", delegate(object o, EventArgs e)
				{
					CS$<>8__locals2.CS$<>8__locals1.args.Handled = true;
				}, null, false, null);
				customMessageWindow2.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs e)
				{
					CS$<>8__locals2.displaySetting.DiscardCurrentChangingModel();
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.SettingsBtn_Click(CS$<>8__locals2.CS$<>8__locals1.sender, null);
				}, null, false, null);
				customMessageWindow2.ShowDialog();
				return;
			}
			SettingsWindow.<>c__DisplayClass18_2 CS$<>8__locals3 = new SettingsWindow.<>c__DisplayClass18_2();
			CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
			GameSettingView gameSettingView = base.visibleControl as GameSettingView;
			if (gameSettingView != null)
			{
				object dataContext = gameSettingView.DataContext;
				CS$<>8__locals3.gameSettingViewModel = dataContext as GameSettingViewModel;
				if (CS$<>8__locals3.gameSettingViewModel != null && CS$<>8__locals3.gameSettingViewModel.IsDirty())
				{
					CustomMessageWindow customMessageWindow3 = new CustomMessageWindow();
					customMessageWindow3.Owner = this.ParentWindow;
					customMessageWindow3.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					BlueStacksUIBinding.Bind(customMessageWindow3.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
					BlueStacksUIBinding.Bind(customMessageWindow3.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
					customMessageWindow3.AddButton(ButtonColors.Blue, "STRING_NO", delegate(object o, EventArgs e)
					{
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.args.Handled = true;
					}, null, false, null);
					customMessageWindow3.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs e)
					{
						CS$<>8__locals3.gameSettingViewModel.Reset();
						CS$<>8__locals3.gameSettingViewModel.Init();
						CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.SettingsBtn_Click(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sender, null);
					}, null, false, null);
					customMessageWindow3.ShowDialog();
					return;
				}
			}
			userControl = base.visibleControl;
			DeviceProfileControl deviceSetting = userControl as DeviceProfileControl;
			if (deviceSetting != null && deviceSetting.IsDirty())
			{
				CustomMessageWindow customMessageWindow4 = new CustomMessageWindow();
				customMessageWindow4.Owner = this.ParentWindow;
				customMessageWindow4.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				BlueStacksUIBinding.Bind(customMessageWindow4.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
				BlueStacksUIBinding.Bind(customMessageWindow4.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
				customMessageWindow4.AddButton(ButtonColors.Blue, "STRING_NO", delegate(object o, EventArgs e)
				{
					CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.args.Handled = true;
				}, null, false, null);
				customMessageWindow4.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs e)
				{
					deviceSetting.Init();
					CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.SettingsBtn_Click(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sender, null);
				}, null, false, null);
				customMessageWindow4.ShowDialog();
				return;
			}
			base.SettingsBtn_Click(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.sender, null);
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x00009522 File Offset: 0x00007722
		protected override void SetPopupOffset()
		{
			new Thread(delegate
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					if (this.ParentWindow.mTopBar.mSnailMode == PerformanceState.VtxDisabled && !base.IsVtxLearned && base.CheckWidth())
					{
						base.EnableVTPopup.HorizontalOffset = base.SettingsWindowStackPanel.ActualWidth;
						base.EnableVTPopup.Width = base.SettingsWindowGrid.ActualWidth;
						base.EnableVTPopup.IsOpen = true;
						base.EnableVTPopup.StaysOpen = true;
					}
				}), DispatcherPriority.Render, new object[0]);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x00040F3C File Offset: 0x0003F13C
		public override void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked settings menu close button");
			bool staysOpen = false;
			bool hasChanges = false;
			UserControl visibleControl = base.visibleControl;
			EngineSettingBase engineSettingBase = visibleControl as EngineSettingBase;
			if (engineSettingBase == null)
			{
				DisplaySettingsControl displaySettingsControl = visibleControl as DisplaySettingsControl;
				if (displaySettingsControl == null)
				{
					GameSettingView gameSettingView = visibleControl as GameSettingView;
					if (gameSettingView == null)
					{
						DeviceProfileControl deviceProfileControl = visibleControl as DeviceProfileControl;
						if (deviceProfileControl != null)
						{
							hasChanges = deviceProfileControl.IsDirty();
						}
					}
					else
					{
						GameSettingViewModel gameSettingViewModel = gameSettingView.DataContext as GameSettingViewModel;
						hasChanges = gameSettingViewModel.IsDirty();
					}
				}
				else
				{
					hasChanges = displaySettingsControl.IsDirty();
				}
			}
			else
			{
				EngineSettingViewModel engineSettingViewModel = engineSettingBase.DataContext as EngineSettingViewModel;
				if (engineSettingViewModel.Status == Status.Progress)
				{
					Logger.Info("Compatibility check is running");
					return;
				}
				hasChanges = engineSettingViewModel.IsDirty();
			}
			if (hasChanges)
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CLOSE_MESSAGE", ""), new object[] { "bluestacks" }), "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", delegate(object o, EventArgs evt)
				{
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs evt)
				{
					GameSettingViewModel gameSettingViewModel2 = this.visibleControl.DataContext as GameSettingViewModel;
					if (gameSettingViewModel2 != null)
					{
						gameSettingViewModel2.Reset();
					}
					hasChanges = false;
				}, null, false, null);
				customMessageWindow.Owner = this.ParentWindow;
				customMessageWindow.ShowDialog();
			}
			if (hasChanges)
			{
				return;
			}
			GrmHandler.RequirementConfigUpdated(this.ParentWindow.mVmName);
			if (this.mIsShortcutEdited && this.mIsShortcutSaveBtnEnabled)
			{
				CustomMessageWindow customMessageWindow2 = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow2.TitleTextBlock, "STRING_SAVE_CHANGES_QUESTION", "");
				BlueStacksUIBinding.Bind(customMessageWindow2.BodyTextBlock, "STRING_UNSAVED_CHANGES", "");
				customMessageWindow2.AddButton(ButtonColors.Blue, "STRING_SAVE_CHANGES", delegate(object o, EventArgs evt)
				{
					this.ParentWindow.mCommonHandler.SaveAndReloadShortcuts();
					this.mIsShortcutEdited = false;
				}, null, false, null);
				customMessageWindow2.AddButton(ButtonColors.White, "STRING_DISCARD", delegate(object o, EventArgs evt)
				{
					CommonHandlers.ReloadShortcutsForAllInstances();
				}, null, false, null);
				customMessageWindow2.Owner = this.ParentWindow;
				customMessageWindow2.CloseButton.PreviewMouseLeftButtonUp += delegate(object o, MouseButtonEventArgs evt)
				{
					staysOpen = true;
				};
				customMessageWindow2.ShowDialog();
			}
			else if (this.mDuplicateShortcutsList.Count > 0)
			{
				CommonHandlers.ReloadShortcutsForAllInstances();
			}
			if (staysOpen)
			{
				return;
			}
			BlueStacksUIUtils.CloseContainerWindow(this);
		}

		// Token: 0x040006FA RID: 1786
		internal CustomSettingsButton updateButton;

		// Token: 0x040006FB RID: 1787
		internal CustomSettingsButton gameSettingsButton;

		// Token: 0x040006FC RID: 1788
		internal bool mIsShortcutEdited;

		// Token: 0x040006FD RID: 1789
		internal bool mIsShortcutSaveBtnEnabled;

		// Token: 0x040006FE RID: 1790
		internal List<string> mDuplicateShortcutsList;

		// Token: 0x040006FF RID: 1791
		internal Dictionary<string, CustomSettingsButton> mSettingsButtons;
	}
}
