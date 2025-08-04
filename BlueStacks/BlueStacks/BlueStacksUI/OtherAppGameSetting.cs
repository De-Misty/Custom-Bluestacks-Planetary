using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000050 RID: 80
	public class OtherAppGameSetting : ViewModelBase
	{
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x00004BC6 File Offset: 0x00002DC6
		public string AppName { get; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00004BCE File Offset: 0x00002DCE
		public string PackageName { get; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x00004BD6 File Offset: 0x00002DD6
		public MainWindow ParentWindow { get; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00004BDE File Offset: 0x00002DDE
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x00004BE6 File Offset: 0x00002DE6
		public string MouseSenstivity
		{
			get
			{
				return this.mMouseSenstivity;
			}
			set
			{
				base.SetProperty<string>(ref this.mMouseSenstivity, value, null);
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00004BF7 File Offset: 0x00002DF7
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x00004BFF File Offset: 0x00002DFF
		public Type SensitivityPropertyType
		{
			get
			{
				return this.mPropertyType;
			}
			set
			{
				base.SetProperty<Type>(ref this.mPropertyType, value, null);
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00004C10 File Offset: 0x00002E10
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x00004C18 File Offset: 0x00002E18
		public ObservableCollection<IMActionItem> SensitivityIMActionItems
		{
			get
			{
				return this.mIMActionItems;
			}
			set
			{
				base.SetProperty<ObservableCollection<IMActionItem>>(ref this.mIMActionItems, value, null);
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0001B834 File Offset: 0x00019A34
		private void InitMouseSenstivity()
		{
			IMAction imaction = this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Where((IMAction item) => item.GetType() == typeof(Pan)).FirstOrDefault<IMAction>();
			if (imaction != null)
			{
				Pan pan = imaction as Pan;
				if (pan != null && (pan.Tweaks & 32) == 0)
				{
					this.mShowSensitivity = Visibility.Visible;
					this.SensitivityPropertyType = IMAction.DictPropertyInfo[imaction.Type]["Sensitivity"].PropertyType;
					this.MouseSenstivity = imaction["Sensitivity"].ToString();
					this.SensitivityIMActionItems.Add(new IMActionItem
					{
						ActionItem = "Sensitivity",
						IMAction = imaction
					});
					return;
				}
			}
			this.mShowSensitivity = Visibility.Collapsed;
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x00004C29 File Offset: 0x00002E29
		// (set) Token: 0x06000446 RID: 1094 RVA: 0x00004C31 File Offset: 0x00002E31
		public Visibility ShowSensitivity
		{
			get
			{
				return this.mShowSensitivity;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.mShowSensitivity, value, null);
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000447 RID: 1095 RVA: 0x00004C42 File Offset: 0x00002E42
		// (set) Token: 0x06000448 RID: 1096 RVA: 0x00004C4A File Offset: 0x00002E4A
		public bool PlayInLandscapeMode
		{
			get
			{
				return this.mPlayInLandscapeMode;
			}
			set
			{
				base.SetProperty<bool>(ref this.mPlayInLandscapeMode, value, null);
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x00004C5B File Offset: 0x00002E5B
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x00004C63 File Offset: 0x00002E63
		public Visibility PlayInLandscapeModeVisibility
		{
			get
			{
				return this.mPlayInLandscapeModeVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.mPlayInLandscapeModeVisibility, value, null);
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x00004C74 File Offset: 0x00002E74
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x00004C7C File Offset: 0x00002E7C
		public bool PlayInPortraitMode
		{
			get
			{
				return this.mPlayInPortraitMode;
			}
			set
			{
				base.SetProperty<bool>(ref this.mPlayInPortraitMode, value, null);
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600044D RID: 1101 RVA: 0x00004C8D File Offset: 0x00002E8D
		// (set) Token: 0x0600044E RID: 1102 RVA: 0x00004C95 File Offset: 0x00002E95
		public Visibility PlayInPortraitModeVisibility
		{
			get
			{
				return this.mPlayInPortraitModeVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.mPlayInPortraitModeVisibility, value, null);
			}
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00004CA6 File Offset: 0x00002EA6
		public OtherAppGameSetting(MainWindow parentWindow, string appName, string packageName)
		{
			this.AppName = appName;
			this.PackageName = packageName;
			this.ParentWindow = parentWindow;
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0001B904 File Offset: 0x00019B04
		public virtual void Init()
		{
			string cloudOrientationForPackage = GuidanceCloudInfoManager.GetCloudOrientationForPackage(this.PackageName);
			this.PlayInLandscapeModeVisibility = Visibility.Collapsed;
			this.PlayInPortraitModeVisibility = Visibility.Collapsed;
			if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
			{
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName] = new AppSettings();
			}
			string text = cloudOrientationForPackage.ToLowerInvariant();
			if (text != null)
			{
				if (!(text == "landscape"))
				{
					if (text == "portrait")
					{
						this.PlayInPortraitModeVisibility = Visibility.Visible;
						this.PlayInPortraitMode = AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedPortraitEnabled;
					}
				}
				else
				{
					this.PlayInLandscapeModeVisibility = Visibility.Visible;
					this.PlayInLandscapeMode = AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedLandscapeEnabled;
				}
			}
			this.InitMouseSenstivity();
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00004CE3 File Offset: 0x00002EE3
		public virtual void LockOriginal()
		{
			this.mOldMouseSenstivity = this.MouseSenstivity;
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00004CF1 File Offset: 0x00002EF1
		public virtual bool HasChanged()
		{
			return this.HasLandscapeModeChanged() || this.HasPortraitModeChanged() || this.MouseSenstivity != this.mOldMouseSenstivity;
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001BA1C File Offset: 0x00019C1C
		public bool HasLandscapeModeChanged()
		{
			return this.PlayInLandscapeModeVisibility == Visibility.Visible && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName) && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedLandscapeEnabled != this.PlayInLandscapeMode;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001BA90 File Offset: 0x00019C90
		public bool HasPortraitModeChanged()
		{
			return this.PlayInPortraitModeVisibility == Visibility.Visible && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName) && AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedPortraitEnabled != this.PlayInPortraitMode;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00004D16 File Offset: 0x00002F16
		private bool HasMouseSenstivityChanged()
		{
			return this.mOldMouseSenstivity != this.MouseSenstivity;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001BB04 File Offset: 0x00019D04
		public virtual bool Save(bool restartReq)
		{
			if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
			{
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName] = new AppSettings();
			}
			if (this.HasLandscapeModeChanged())
			{
				Utils.SetCustomAppSize(this.ParentWindow.mVmName, this.PackageName, this.PlayInLandscapeMode ? ScreenMode.full : ScreenMode.original);
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedLandscapeEnabled = this.PlayInLandscapeMode;
				KMManager.SelectSchemeIfPresent(this.ParentWindow, this.PlayInLandscapeMode ? "Landscape" : "Portrait", "gamesettings", false);
				ClientStats.SendMiscellaneousStatsAsync("client_game_settings", RegistryManager.Instance.UserGuid, "landscapeMode", RegistryManager.Instance.ClientVersion, this.PackageName, this.PlayInLandscapeMode ? ScreenMode.full.ToString() : ScreenMode.original.ToString(), RegistryManager.Instance.Oem, null, null);
				restartReq = true;
			}
			if (this.HasPortraitModeChanged())
			{
				Utils.SetCustomAppSize(this.ParentWindow.mVmName, this.PackageName, this.PlayInPortraitMode ? ScreenMode.small : ScreenMode.original);
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsForcedPortraitEnabled = this.PlayInPortraitMode;
				KMManager.SelectSchemeIfPresent(this.ParentWindow, this.PlayInPortraitMode ? "Portrait" : "Landscape", "gamesettings", false);
				ClientStats.SendMiscellaneousStatsAsync("client_game_settings", RegistryManager.Instance.UserGuid, "portraitMode", RegistryManager.Instance.ClientVersion, this.PackageName, this.PlayInPortraitMode ? ScreenMode.small.ToString() : ScreenMode.original.ToString(), RegistryManager.Instance.Oem, null, null);
				restartReq = true;
			}
			if (this.HasMouseSenstivityChanged())
			{
				KeymapCanvasWindow.sIsDirty = true;
				KMManager.SaveIMActions(this.ParentWindow, true, false);
				if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed && KMManager.sGuidanceWindow.IsVisible)
				{
					KMManager.sGuidanceWindow.InitUI();
				}
				ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "mouseSenstivityChanged", string.Empty, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.PackageName, null);
				this.InitMouseSenstivity();
			}
			return restartReq;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0001BDBC File Offset: 0x00019FBC
		public void Reset()
		{
			string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
			IEnumerable<IMControlScheme> enumerable = this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme_) => string.Equals(scheme_.Name, schemeName, StringComparison.InvariantCultureIgnoreCase));
			if (enumerable.Any<IMControlScheme>())
			{
				IMControlScheme imcontrolScheme;
				if (enumerable.Count<IMControlScheme>() != 1)
				{
					imcontrolScheme = enumerable.Where((IMControlScheme scheme_) => !scheme_.BuiltIn).First<IMControlScheme>();
				}
				else
				{
					imcontrolScheme = enumerable.First<IMControlScheme>();
				}
				IMControlScheme imcontrolScheme2 = imcontrolScheme;
				if (imcontrolScheme2.BuiltIn)
				{
					this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
					IMControlScheme imcontrolScheme3 = this.ParentWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture)).FirstOrDefault<IMControlScheme>();
					if (imcontrolScheme3 != null)
					{
						imcontrolScheme3.Selected = true;
						this.ParentWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme3;
						this.ParentWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme3.Name] = imcontrolScheme3;
						return;
					}
				}
				else
				{
					this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
					this.ParentWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme2.DeepCopy();
					this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeName] = this.ParentWindow.SelectedConfig.SelectedControlScheme;
					this.ParentWindow.SelectedConfig.ControlSchemes.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme);
				}
			}
		}

		// Token: 0x0400025C RID: 604
		private string mMouseSenstivity;

		// Token: 0x0400025D RID: 605
		private Type mPropertyType;

		// Token: 0x0400025E RID: 606
		private ObservableCollection<IMActionItem> mIMActionItems = new ObservableCollection<IMActionItem>();

		// Token: 0x0400025F RID: 607
		private string mOldMouseSenstivity;

		// Token: 0x04000260 RID: 608
		private Visibility mShowSensitivity = Visibility.Collapsed;

		// Token: 0x04000261 RID: 609
		private bool mPlayInLandscapeMode;

		// Token: 0x04000262 RID: 610
		private Visibility mPlayInLandscapeModeVisibility = Visibility.Collapsed;

		// Token: 0x04000263 RID: 611
		private bool mPlayInPortraitMode;

		// Token: 0x04000264 RID: 612
		private Visibility mPlayInPortraitModeVisibility = Visibility.Collapsed;
	}
}
