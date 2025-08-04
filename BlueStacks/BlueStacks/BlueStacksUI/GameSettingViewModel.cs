using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200004D RID: 77
	public class GameSettingViewModel : ViewModelBase
	{
		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x000049A9 File Offset: 0x00002BA9
		// (set) Token: 0x06000407 RID: 1031 RVA: 0x000049B1 File Offset: 0x00002BB1
		public GameSettingView View { get; set; }

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x000049BA File Offset: 0x00002BBA
		// (set) Token: 0x06000409 RID: 1033 RVA: 0x000049C2 File Offset: 0x00002BC2
		public CursorMode CursorMode
		{
			get
			{
				return this.mCursorMode;
			}
			set
			{
				base.SetProperty<CursorMode>(ref this.mCursorMode, value, null);
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x000049D3 File Offset: 0x00002BD3
		// (set) Token: 0x0600040B RID: 1035 RVA: 0x000049DB File Offset: 0x00002BDB
		public string ImageName
		{
			get
			{
				return this.mImageName;
			}
			set
			{
				base.SetProperty<string>(ref this.mImageName, value, null);
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x000049EC File Offset: 0x00002BEC
		// (set) Token: 0x0600040D RID: 1037 RVA: 0x000049F4 File Offset: 0x00002BF4
		public string AppName
		{
			get
			{
				return this.mAppName;
			}
			set
			{
				base.SetProperty<string>(ref this.mAppName, value, null);
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x00004A05 File Offset: 0x00002C05
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x00004A0D File Offset: 0x00002C0D
		public string PackageName
		{
			get
			{
				return this.mPackageName;
			}
			set
			{
				base.SetProperty<string>(ref this.mPackageName, value, null);
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x00004A1E File Offset: 0x00002C1E
		// (set) Token: 0x06000411 RID: 1041 RVA: 0x00004A26 File Offset: 0x00002C26
		public CurrentGame CurrentGame
		{
			get
			{
				return this.mCurrentGame;
			}
			set
			{
				base.SetProperty<CurrentGame>(ref this.mCurrentGame, value, null);
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00004A37 File Offset: 0x00002C37
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x00004A3F File Offset: 0x00002C3F
		public Uri LearnMoreUri
		{
			get
			{
				return this.mLearnMoreUri;
			}
			set
			{
				base.SetProperty<Uri>(ref this.mLearnMoreUri, value, null);
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x00004A50 File Offset: 0x00002C50
		// (set) Token: 0x06000415 RID: 1045 RVA: 0x00004A58 File Offset: 0x00002C58
		public Visibility LearnMoreVisibility
		{
			get
			{
				return this.mLearnMoreVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.mLearnMoreVisibility, value, null);
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x00004A69 File Offset: 0x00002C69
		// (set) Token: 0x06000417 RID: 1047 RVA: 0x00004A71 File Offset: 0x00002C71
		public OtherAppGameSetting OtherAppGameSetting
		{
			get
			{
				return this.mOtherAppGameSetting;
			}
			set
			{
				base.SetProperty<OtherAppGameSetting>(ref this.mOtherAppGameSetting, value, null);
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x00004A82 File Offset: 0x00002C82
		// (set) Token: 0x06000419 RID: 1049 RVA: 0x00004A8A File Offset: 0x00002C8A
		public FreeFireGameSettingViewModel FreeFireGameSettingViewModel
		{
			get
			{
				return this.mFreeFireGameSettingViewModel;
			}
			set
			{
				base.SetProperty<FreeFireGameSettingViewModel>(ref this.mFreeFireGameSettingViewModel, value, null);
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x00004A9B File Offset: 0x00002C9B
		// (set) Token: 0x0600041B RID: 1051 RVA: 0x00004AA3 File Offset: 0x00002CA3
		public PubgGameSettingViewModel PubgGameSettingViewModel
		{
			get
			{
				return this.mPubgGameSettingViewModel;
			}
			set
			{
				base.SetProperty<PubgGameSettingViewModel>(ref this.mPubgGameSettingViewModel, value, null);
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x00004AB4 File Offset: 0x00002CB4
		// (set) Token: 0x0600041D RID: 1053 RVA: 0x00004ABC File Offset: 0x00002CBC
		public CallOfDutyGameSettingViewModel CallOfDutyGameSettingViewModel
		{
			get
			{
				return this.mCallOfDutyGameSettingViewModel;
			}
			set
			{
				base.SetProperty<CallOfDutyGameSettingViewModel>(ref this.mCallOfDutyGameSettingViewModel, value, null);
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x0600041E RID: 1054 RVA: 0x00004ACD File Offset: 0x00002CCD
		// (set) Token: 0x0600041F RID: 1055 RVA: 0x00004AD5 File Offset: 0x00002CD5
		public Visibility ShowGuideVisibility
		{
			get
			{
				return this.mShowGuideVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.mShowGuideVisibility, value, null);
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x00004AE6 File Offset: 0x00002CE6
		// (set) Token: 0x06000421 RID: 1057 RVA: 0x00004AEE File Offset: 0x00002CEE
		public string CustomCursorImageName
		{
			get
			{
				return this.mCustomCursorImageName;
			}
			set
			{
				base.SetProperty<string>(ref this.mCustomCursorImageName, value, null);
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x00004AFF File Offset: 0x00002CFF
		// (set) Token: 0x06000423 RID: 1059 RVA: 0x00004B07 File Offset: 0x00002D07
		public ICommand SaveCommand { get; set; }

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x00004B10 File Offset: 0x00002D10
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x00004B18 File Offset: 0x00002D18
		public ICommand OpenGameGuideCommand { get; set; }

		// Token: 0x06000426 RID: 1062 RVA: 0x0001ADD4 File Offset: 0x00018FD4
		public GameSettingViewModel(MainWindow owner)
		{
			this.mParentWindow = owner;
			if (((owner != null) ? owner.StaticComponents.mSelectedTabButton : null) != null)
			{
				this.ImageName = ((owner != null) ? owner.StaticComponents.mSelectedTabButton.mAppTabIcon.ImageName : null);
				this.AppName = ((owner != null) ? owner.StaticComponents.mSelectedTabButton.AppLabel : null);
				this.PackageName = ((owner != null) ? owner.StaticComponents.mSelectedTabButton.PackageName : null);
				this.OpenGameGuideCommand = new RelayCommand2(new Action<object>(this.OpenGameGuide));
				if (string.Equals(this.PackageName, "com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase))
				{
					this.CustomCursorImageName = "yellow_cursor_brawl";
				}
				else
				{
					this.CustomCursorImageName = "yellow_cursor";
				}
				if (owner != null && owner.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab)
				{
					if ("com.dts.freefireth".Contains(this.PackageName))
					{
						this.CurrentGame = CurrentGame.FreeFire;
					}
					else if (PackageActivityNames.ThirdParty.AllPUBGPackageNames.Contains(this.PackageName))
					{
						this.CurrentGame = CurrentGame.PubG;
					}
					else if (PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(this.PackageName))
					{
						this.CurrentGame = CurrentGame.CallOfDuty;
					}
					else
					{
						this.CurrentGame = CurrentGame.OtherApp;
					}
				}
				else
				{
					this.CurrentGame = CurrentGame.None;
				}
				this.OtherAppGameSetting = new OtherAppGameSetting(this.mParentWindow, this.AppName, this.PackageName);
				this.FreeFireGameSettingViewModel = new FreeFireGameSettingViewModel(this.mParentWindow, this.AppName, this.PackageName);
				this.PubgGameSettingViewModel = new PubgGameSettingViewModel(this.mParentWindow, this.AppName, this.PackageName);
				this.CallOfDutyGameSettingViewModel = new CallOfDutyGameSettingViewModel(this.mParentWindow, this.AppName, this.PackageName);
			}
			this.SaveCommand = new RelayCommand2(new Func<object, bool>(this.CanSave), new Action<object>(this.SaveGameSettings));
			this.Init();
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0001AFBC File Offset: 0x000191BC
		public void Init()
		{
			this.CursorMode = (RegistryManager.Instance.CustomCursorEnabled ? CursorMode.Custom : CursorMode.Normal);
			this.mPreviousCursorMode = this.CursorMode;
			if (this.mParentWindow.StaticComponents.mSelectedTabButton != null)
			{
				Visibility visibility;
				if (this.mParentWindow.SelectedConfig.SelectedControlScheme != null && this.mParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any<IMAction>())
				{
					if (this.mParentWindow.SelectedConfig.SelectedControlScheme.GameControls.SelectMany((IMAction action) => action.Guidance).Any<KeyValuePair<string, string>>())
					{
						visibility = Visibility.Visible;
						goto IL_00AA;
					}
				}
				visibility = Visibility.Collapsed;
				IL_00AA:
				this.ShowGuideVisibility = visibility;
				switch (this.CurrentGame)
				{
				case CurrentGame.PubG:
					this.LearnMoreVisibility = Visibility.Visible;
					this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_pubg");
					this.PubgGameSettingViewModel.Init();
					this.PubgGameSettingViewModel.LockOriginal();
					this.OtherAppGameSetting = this.PubgGameSettingViewModel;
					return;
				case CurrentGame.CallOfDuty:
					this.LearnMoreVisibility = Visibility.Visible;
					this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_callofduty");
					this.CallOfDutyGameSettingViewModel.Init();
					this.CallOfDutyGameSettingViewModel.LockOriginal();
					this.OtherAppGameSetting = this.CallOfDutyGameSettingViewModel;
					return;
				case CurrentGame.FreeFire:
					this.LearnMoreVisibility = Visibility.Visible;
					this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_freefire");
					this.FreeFireGameSettingViewModel.Init();
					this.FreeFireGameSettingViewModel.LockOriginal();
					this.OtherAppGameSetting = this.FreeFireGameSettingViewModel;
					return;
				case CurrentGame.OtherApp:
					this.OtherAppGameSetting.Init();
					this.OtherAppGameSetting.LockOriginal();
					if (this.OtherAppGameSetting.PlayInLandscapeModeVisibility == Visibility.Visible || this.OtherAppGameSetting.PlayInPortraitModeVisibility == Visibility.Visible)
					{
						this.LearnMoreVisibility = Visibility.Visible;
						this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_sevendeadly");
						return;
					}
					if (this.ShowGuideVisibility == Visibility.Collapsed)
					{
						this.CurrentGame = CurrentGame.None;
						this.LearnMoreVisibility = Visibility.Collapsed;
						return;
					}
					break;
				default:
					this.LearnMoreVisibility = Visibility.Collapsed;
					break;
				}
			}
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x0001B1DC File Offset: 0x000193DC
		private void OpenGameGuide(object obj)
		{
			if (KMManager.sGuidanceWindow != null && KMManager.sGuidanceWindow.Visibility == Visibility.Visible)
			{
				KMManager.sGuidanceWindow.Highlight();
				return;
			}
			if (this.IsDirty())
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.Owner = this.mParentWindow;
				customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_CLOSE_MESSAGE", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", delegate(object o, EventArgs e)
				{
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs e)
				{
					this.OpenGameGuide();
				}, null, false, null);
				customMessageWindow.ShowDialog();
				return;
			}
			this.OpenGameGuide();
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0001B2AC File Offset: 0x000194AC
		private void OpenGameGuide()
		{
			BlueStacksUIUtils.CloseContainerWindow(MainWindow.SettingsWindow);
			if (!this.mParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad"))
			{
				KMManager.HandleInputMapperWindow(this.mParentWindow, "gamepad");
			}
			ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "gameGuide", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.PackageName, null);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00004B21 File Offset: 0x00002D21
		private bool CanSave(object obj)
		{
			return this.IsDirty();
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x0001B330 File Offset: 0x00019530
		public bool IsDirty()
		{
			bool flag = false;
			switch (this.CurrentGame)
			{
			case CurrentGame.PubG:
				flag = this.PubgGameSettingViewModel.HasChanged();
				break;
			case CurrentGame.CallOfDuty:
				flag = this.CallOfDutyGameSettingViewModel.HasChanged();
				break;
			case CurrentGame.FreeFire:
				flag = this.FreeFireGameSettingViewModel.HasChanged();
				break;
			case CurrentGame.OtherApp:
				flag = this.OtherAppGameSetting.HasChanged();
				break;
			}
			return flag || this.mPreviousCursorMode != this.CursorMode;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0001B3AC File Offset: 0x000195AC
		private void SaveGameSettings(object obj)
		{
			bool flag = false;
			bool flag2 = false;
			switch (this.CurrentGame)
			{
			case CurrentGame.PubG:
				flag = this.PubgGameSettingViewModel.Save(flag);
				this.PubgGameSettingViewModel.LockOriginal();
				flag2 = true;
				break;
			case CurrentGame.CallOfDuty:
				flag = this.CallOfDutyGameSettingViewModel.Save(flag);
				this.CallOfDutyGameSettingViewModel.LockOriginal();
				flag2 = true;
				break;
			case CurrentGame.FreeFire:
				flag = this.FreeFireGameSettingViewModel.Save(flag);
				this.FreeFireGameSettingViewModel.LockOriginal();
				flag2 = true;
				break;
			case CurrentGame.OtherApp:
				flag = this.OtherAppGameSetting.Save(flag);
				this.OtherAppGameSetting.LockOriginal();
				flag2 = true;
				break;
			}
			if (flag2)
			{
				ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, this.AppName, "Save", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
			if (this.mPreviousCursorMode != this.CursorMode)
			{
				RegistryManager.Instance.CustomCursorEnabled = this.CursorMode == CursorMode.Custom;
				foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
				{
					mainWindow.mCommonHandler.SetCustomCursorForApp(this.PackageName);
				}
				this.mPreviousCursorMode = this.CursorMode;
				ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "Is Custom Cursor", (this.CursorMode == CursorMode.Custom).ToString(CultureInfo.InvariantCulture), RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			}
			this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
			if (flag)
			{
				GameSettingViewModel.RestartApp(this.mParentWindow, this.AppName);
			}
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0001B58C File Offset: 0x0001978C
		public static void SendGameSettingsEnabledToGuest(MainWindow parentWindow, bool enabled)
		{
			string text = "{";
			text += string.Format(CultureInfo.InvariantCulture, "\"package_name\":\"{0}\",", new object[] { "com.dts.freefireth" });
			text += string.Format(CultureInfo.InvariantCulture, "\"game_settings_enabled\":\"{0}\"", new object[] { enabled.ToString(CultureInfo.InvariantCulture) });
			text += "}";
			VmCmdHandler.RunCommandAsync(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { "gameSettingsEnabled", text }), null, null, (parentWindow != null) ? parentWindow.mVmName : null);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001B630 File Offset: 0x00019830
		public static void SendGameSettingsStat(string statsTag)
		{
			ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, statsTag, string.Empty, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x0001B678 File Offset: 0x00019878
		protected void AddToastPopup(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this.View);
				}
				this.mToastPopup.Init(this.View, message, null, null, HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 12, null, null);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x0001B708 File Offset: 0x00019908
		public static void RestartApp(MainWindow parentWindow, string appName)
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow
			{
				Owner = parentWindow
			};
			string text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RESTART", ""), new object[] { appName });
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, text, "");
			string text2 = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CHANGED_RESTART_APP_MESSAGE", ""), new object[] { appName });
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, text2, "");
			ThreadStart <>9__1;
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_NOW", delegate(object o, EventArgs e)
			{
				if (MainWindow.SettingsWindow.ParentWindow == parentWindow)
				{
					BlueStacksUIUtils.CloseContainerWindow(MainWindow.SettingsWindow);
				}
				ThreadStart threadStart;
				if ((threadStart = <>9__1) == null)
				{
					threadStart = (<>9__1 = delegate
					{
						parentWindow.mTopBar.mAppTabButtons.RestartTab(parentWindow.StaticComponents.mSelectedTabButton.PackageName);
					});
				}
				Thread thread = new Thread(threadStart);
				thread.IsBackground = true;
				Logger.Info("Restarting Game Tab.");
				thread.Start();
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
			customMessageWindow.ShowDialog();
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00004B29 File Offset: 0x00002D29
		public void Reset()
		{
			if (this.OtherAppGameSetting.ShowSensitivity == Visibility.Visible)
			{
				this.OtherAppGameSetting.Reset();
			}
		}

		// Token: 0x04000240 RID: 576
		private static readonly string sKnowMoreBaseUrl = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
		{
			WebHelper.GetServerHost(),
			"help_articles"
		})) + "&article=";

		// Token: 0x04000242 RID: 578
		private readonly MainWindow mParentWindow;

		// Token: 0x04000243 RID: 579
		private CursorMode mPreviousCursorMode;

		// Token: 0x04000244 RID: 580
		private CursorMode mCursorMode;

		// Token: 0x04000245 RID: 581
		private string mImageName;

		// Token: 0x04000246 RID: 582
		private string mAppName;

		// Token: 0x04000247 RID: 583
		private string mPackageName;

		// Token: 0x04000248 RID: 584
		private CurrentGame mCurrentGame;

		// Token: 0x04000249 RID: 585
		private Uri mLearnMoreUri;

		// Token: 0x0400024A RID: 586
		private Visibility mLearnMoreVisibility = Visibility.Collapsed;

		// Token: 0x0400024B RID: 587
		private OtherAppGameSetting mOtherAppGameSetting;

		// Token: 0x0400024C RID: 588
		private FreeFireGameSettingViewModel mFreeFireGameSettingViewModel;

		// Token: 0x0400024D RID: 589
		private PubgGameSettingViewModel mPubgGameSettingViewModel;

		// Token: 0x0400024E RID: 590
		private CallOfDutyGameSettingViewModel mCallOfDutyGameSettingViewModel;

		// Token: 0x0400024F RID: 591
		private Visibility mShowGuideVisibility;

		// Token: 0x04000250 RID: 592
		private string mCustomCursorImageName;

		// Token: 0x04000253 RID: 595
		private CustomToastPopupControl mToastPopup;
	}
}
