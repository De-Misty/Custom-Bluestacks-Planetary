using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200002A RID: 42
	public class AppIconModel : INotifyPropertyChanged
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600029F RID: 671 RVA: 0x00013BE0 File Offset: 0x00011DE0
		// (remove) Token: 0x060002A0 RID: 672 RVA: 0x00013C18 File Offset: 0x00011E18
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x060002A1 RID: 673 RVA: 0x00003B29 File Offset: 0x00001D29
		protected void OnPropertyChanged(string property)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(property));
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x00003B42 File Offset: 0x00001D42
		// (set) Token: 0x060002A3 RID: 675 RVA: 0x00003B4A File Offset: 0x00001D4A
		public string PackageName { get; set; } = string.Empty;

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x00003B53 File Offset: 0x00001D53
		// (set) Token: 0x060002A5 RID: 677 RVA: 0x00003B5B File Offset: 0x00001D5B
		public string ActivityName { get; set; } = string.Empty;

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x00003B64 File Offset: 0x00001D64
		// (set) Token: 0x060002A7 RID: 679 RVA: 0x00003B6C File Offset: 0x00001D6C
		public Visibility AppIconVisibility
		{
			get
			{
				return this.mAppIconVisibility;
			}
			set
			{
				this.mAppIconVisibility = value;
				this.OnPropertyChanged("AppIconVisibility");
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x00003B80 File Offset: 0x00001D80
		// (set) Token: 0x060002A9 RID: 681 RVA: 0x00003B88 File Offset: 0x00001D88
		public string ImageName
		{
			get
			{
				return this.mImageName;
			}
			set
			{
				this.mImageName = value;
				this.OnPropertyChanged("ImageName");
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060002AA RID: 682 RVA: 0x00003B9C File Offset: 0x00001D9C
		// (set) Token: 0x060002AB RID: 683 RVA: 0x00003BA4 File Offset: 0x00001DA4
		public string AppName
		{
			get
			{
				return this.mAppName;
			}
			set
			{
				this.mAppName = value;
				this.OnPropertyChanged("AppName");
				this.AppNameTooltip = value;
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060002AC RID: 684 RVA: 0x00003BBF File Offset: 0x00001DBF
		// (set) Token: 0x060002AD RID: 685 RVA: 0x00003BC7 File Offset: 0x00001DC7
		public string AppNameTooltip
		{
			get
			{
				return this.mAppNameTooltip;
			}
			set
			{
				this.mAppNameTooltip = value;
				this.OnPropertyChanged("AppNameTooltip");
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00003BDB File Offset: 0x00001DDB
		// (set) Token: 0x060002AF RID: 687 RVA: 0x00003BE3 File Offset: 0x00001DE3
		public TextTrimming AppNameTextTrimming
		{
			get
			{
				return this.mAppNameTextTrimming;
			}
			set
			{
				this.mAppNameTextTrimming = value;
				this.OnPropertyChanged("AppNameTextTrimming");
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x00003BF7 File Offset: 0x00001DF7
		// (set) Token: 0x060002B1 RID: 689 RVA: 0x00003BFF File Offset: 0x00001DFF
		public TextWrapping AppNameTextWrapping
		{
			get
			{
				return this.mAppNameTextWrapping;
			}
			set
			{
				this.mAppNameTextWrapping = value;
				this.OnPropertyChanged("AppNameTextWrapping");
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x00003C13 File Offset: 0x00001E13
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x00003C1B File Offset: 0x00001E1B
		public bool IsGamepadCompatible
		{
			get
			{
				return this.mIsGamepadCompatible;
			}
			set
			{
				this.mIsGamepadCompatible = value;
				this.OnPropertyChanged("IsGamepadCompatible");
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x00003C2F File Offset: 0x00001E2F
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x00003C37 File Offset: 0x00001E37
		public bool IsGamepadConnected
		{
			get
			{
				return this.mIsGamepadConnected;
			}
			set
			{
				this.mIsGamepadConnected = value;
				this.OnPropertyChanged("IsGamepadConnected");
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x00003C4B File Offset: 0x00001E4B
		// (set) Token: 0x060002B7 RID: 695 RVA: 0x00003C53 File Offset: 0x00001E53
		public bool IsRedDotVisible
		{
			get
			{
				return this.mIsRedDotVisible;
			}
			set
			{
				this.mIsRedDotVisible = value;
				this.OnPropertyChanged("IsRedDotVisible");
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x00003C67 File Offset: 0x00001E67
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x00003C6F File Offset: 0x00001E6F
		public string ApkUrl { get; set; } = string.Empty;

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x060002BA RID: 698 RVA: 0x00003C78 File Offset: 0x00001E78
		// (set) Token: 0x060002BB RID: 699 RVA: 0x00003C80 File Offset: 0x00001E80
		public bool IsGifIcon { get; set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060002BC RID: 700 RVA: 0x00003C89 File Offset: 0x00001E89
		// (set) Token: 0x060002BD RID: 701 RVA: 0x00003C91 File Offset: 0x00001E91
		public bool IsAppSuggestionActive { get; set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00003C9A File Offset: 0x00001E9A
		// (set) Token: 0x060002BF RID: 703 RVA: 0x00003CA2 File Offset: 0x00001EA2
		public bool mIsAppRemovable { get; set; } = true;

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x00003CAB File Offset: 0x00001EAB
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x00003CB3 File Offset: 0x00001EB3
		public bool IsGl3App { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x00003CBC File Offset: 0x00001EBC
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x00003CC4 File Offset: 0x00001EC4
		public AppIncompatType AppIncompatType
		{
			get
			{
				return this.mAppIncompatType;
			}
			set
			{
				this.mAppIncompatType = value;
				this.OnPropertyChanged("AppIncompatType");
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x00003CD8 File Offset: 0x00001ED8
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x00003CE0 File Offset: 0x00001EE0
		public bool IsDownloading { get; set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x00003CE9 File Offset: 0x00001EE9
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x00003CF1 File Offset: 0x00001EF1
		public double DownloadPercentage { get; set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00003CFA File Offset: 0x00001EFA
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x00003D02 File Offset: 0x00001F02
		public bool IsInstalling { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060002CA RID: 714 RVA: 0x00003D0B File Offset: 0x00001F0B
		// (set) Token: 0x060002CB RID: 715 RVA: 0x00003D13 File Offset: 0x00001F13
		public bool IsDownLoadingFailed { get; set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060002CC RID: 716 RVA: 0x00003D1C File Offset: 0x00001F1C
		// (set) Token: 0x060002CD RID: 717 RVA: 0x00003D24 File Offset: 0x00001F24
		public bool IsInstallingFailed { get; set; }

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00003D2D File Offset: 0x00001F2D
		// (set) Token: 0x060002CF RID: 719 RVA: 0x00003D35 File Offset: 0x00001F35
		public bool IsInstalledApp { get; set; } = true;

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00003D3E File Offset: 0x00001F3E
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x00003D46 File Offset: 0x00001F46
		public int MyAppPriority { get; set; } = 999;

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x00003D4F File Offset: 0x00001F4F
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x00003D57 File Offset: 0x00001F57
		public bool IsRerollIcon { get; set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x00003D60 File Offset: 0x00001F60
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x00003D68 File Offset: 0x00001F68
		public string ApkFilePath { get; set; } = string.Empty;

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x00003D71 File Offset: 0x00001F71
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x00003D79 File Offset: 0x00001F79
		public bool mIsAppInstalled { get; set; } = true;

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x00003D82 File Offset: 0x00001F82
		// (set) Token: 0x060002D9 RID: 729 RVA: 0x00003D8A File Offset: 0x00001F8A
		public AppIconLocation IconLocation { get; set; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060002DA RID: 730 RVA: 0x00003D93 File Offset: 0x00001F93
		// (set) Token: 0x060002DB RID: 731 RVA: 0x00003D9B File Offset: 0x00001F9B
		public double IconHeight { get; set; } = 60.0;

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060002DC RID: 732 RVA: 0x00003DA4 File Offset: 0x00001FA4
		// (set) Token: 0x060002DD RID: 733 RVA: 0x00003DAC File Offset: 0x00001FAC
		public double IconWidth { get; set; } = 60.0;

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060002DE RID: 734 RVA: 0x00003DB5 File Offset: 0x00001FB5
		// (set) Token: 0x060002DF RID: 735 RVA: 0x00003DBD File Offset: 0x00001FBD
		public string ApplyImageBorder
		{
			get
			{
				return this.mApplyImageBorder;
			}
			set
			{
				this.mApplyImageBorder = value;
				this.OnPropertyChanged("ApplyImageBorder");
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x00003DD1 File Offset: 0x00001FD1
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x00003DD9 File Offset: 0x00001FD9
		public string mPromotionId { get; private set; }

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060002E2 RID: 738 RVA: 0x00013C50 File Offset: 0x00011E50
		// (remove) Token: 0x060002E3 RID: 739 RVA: 0x00013C88 File Offset: 0x00011E88
		public event Action<AppIconDownloadingPhases> mAppDownloadingEvent;

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x00003DE2 File Offset: 0x00001FE2
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x00003DEA File Offset: 0x00001FEA
		public AppSuggestionPromotion AppSuggestionInfo { get; private set; }

		// Token: 0x060002E6 RID: 742 RVA: 0x00013CC0 File Offset: 0x00011EC0
		private void Init(string package, string appName)
		{
			if (AppHandler.ListIgnoredApps.Contains(package, StringComparer.InvariantCultureIgnoreCase) || string.Equals(this.PackageName, "macro_recorder", StringComparison.InvariantCulture))
			{
				this.AppIconVisibility = Visibility.Collapsed;
			}
			this.PackageName = package;
			this.AppName = appName;
			if (RegistryManager.Instance.IsShowIconBorder)
			{
				this.ApplyBorder("appFrameIcon");
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00003DF3 File Offset: 0x00001FF3
		internal void InitRerollIcon(string package, string appname)
		{
			this.Init(package, appname);
			this.IsRerollIcon = true;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00003E04 File Offset: 0x00002004
		internal void Init(string package, string appName, string apkUrl)
		{
			this.Init(package, appName);
			this.ApkUrl = apkUrl;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00013D20 File Offset: 0x00011F20
		internal void Init(AppInfo item)
		{
			this.mAppInfoItem = item;
			this.Init(item.Package, item.Name);
			this.LoadDownloadAppIcon();
			this.ActivityName = item.Activity;
			if (item.Gl3Required)
			{
				this.IsGl3App = true;
			}
			this.IsGamepadCompatible = item.IsGamepadCompatible;
			if (this.IsGamepadCompatible)
			{
				this.AppNameTextWrapping = TextWrapping.NoWrap;
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00013D84 File Offset: 0x00011F84
		internal void Init(AppSuggestionPromotion appSuggestionInfo)
		{
			this.AppSuggestionInfo = appSuggestionInfo;
			this.AppName = appSuggestionInfo.AppName;
			this.ActivityName = appSuggestionInfo.AppActivity;
			this.IsAppSuggestionActive = true;
			this.AppNameTooltip = (string.IsNullOrEmpty(this.AppSuggestionInfo.ToolTip) ? this.AppName : null);
			this.AppNameTextWrapping = TextWrapping.Wrap;
			this.AppNameTextTrimming = TextTrimming.CharacterEllipsis;
			if (this.AppSuggestionInfo.ExtraPayload.ContainsKey("click_generic_action") && (EnumHelper.Parse<GenericAction>(this.AppSuggestionInfo.ExtraPayload["click_generic_action"], GenericAction.None) & (GenericAction)448) != (GenericAction)0)
			{
				this.mIsAppRemovable = false;
				this.AppNameTextWrapping = TextWrapping.NoWrap;
				this.AppNameTextTrimming = TextTrimming.None;
			}
			this.mIsAppInstalled = false;
			this.mPromotionId = this.AppSuggestionInfo.AppIconId;
			if (this.AppSuggestionInfo.AppIcon.EndsWith(".gif", StringComparison.InvariantCulture))
			{
				this.IsGifIcon = true;
			}
			this.ImageName = this.AppSuggestionInfo.AppIconPath;
			if (this.AppSuggestionInfo.IsIconBorder)
			{
				this.ApplyBorder(Path.Combine(RegistryStrings.PromotionDirectory, string.Format(CultureInfo.InvariantCulture, "{0}{1}.png", new object[]
				{
					this.AppSuggestionInfo.IconBorderId,
					"app_suggestion_icon_border"
				})));
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00013ECC File Offset: 0x000120CC
		private void LoadDownloadAppIcon()
		{
			if (!string.IsNullOrEmpty(this.PackageName) && !this.IsAppSuggestionActive)
			{
				string text = Regex.Replace(this.PackageName + ".png", "[\\x22\\\\\\/:*?|<>]", " ");
				string text2 = Path.Combine(RegistryStrings.GadgetDir, text);
				if (!AppHandler.ListIgnoredApps.Contains(this.PackageName, StringComparer.InvariantCultureIgnoreCase))
				{
					AppInfo appInfo = this.mAppInfoItem;
					if (!string.IsNullOrEmpty((appInfo != null) ? appInfo.Img : null) && !File.Exists(text2) && File.Exists(Path.Combine(RegistryStrings.GadgetDir, this.mAppInfoItem.Img)))
					{
						File.Copy(Path.Combine(RegistryStrings.GadgetDir, this.mAppInfoItem.Img), text2, false);
					}
				}
				if (File.Exists(text2))
				{
					this.ImageName = text2;
				}
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00003E15 File Offset: 0x00002015
		internal void AddRedDot()
		{
			this.IsRedDotVisible = true;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00013FA0 File Offset: 0x000121A0
		internal void AddToDock(double height = 50.0, double width = 50.0)
		{
			this.IconLocation = AppIconLocation.Dock;
			this.IconHeight = height;
			this.IconWidth = width;
			this.mIsAppRemovable = false;
			if (PromotionObject.Instance.DockOrder.ContainsKey(this.PackageName) && this.MyAppPriority != PromotionObject.Instance.DockOrder[this.PackageName])
			{
				this.MyAppPriority = PromotionObject.Instance.DockOrder[this.PackageName];
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x00014018 File Offset: 0x00012218
		internal void AddToMoreAppsDock(double height = 55.0, double width = 55.0)
		{
			this.IconLocation = AppIconLocation.Moreapps;
			this.IconHeight = height;
			this.IconWidth = width;
			this.mIsAppRemovable = false;
			if (PromotionObject.Instance.MoreAppsDockOrder.ContainsKey(this.PackageName) && this.MyAppPriority != PromotionObject.Instance.MoreAppsDockOrder[this.PackageName])
			{
				this.MyAppPriority = PromotionObject.Instance.MoreAppsDockOrder[this.PackageName];
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00014090 File Offset: 0x00012290
		internal void AddToInstallDrawer()
		{
			if (string.Compare(this.PackageName, "com.android.vending", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.MyAppPriority = 1;
			}
			if (PromotionObject.Instance.MyAppsOrder.ContainsKey(this.PackageName) && this.MyAppPriority != PromotionObject.Instance.MyAppsOrder[this.PackageName])
			{
				this.MyAppPriority = PromotionObject.Instance.MyAppsOrder[this.PackageName];
			}
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00003E1E File Offset: 0x0000201E
		internal void AddPromotionBorderInstalledApp(AppSuggestionPromotion appSuggestionInfo)
		{
			this.AppSuggestionInfo = appSuggestionInfo;
			if (this.AppSuggestionInfo.IsIconBorder)
			{
				this.ApplyBorder(this.AppSuggestionInfo.IconBorderId + "app_suggestion_icon_border");
			}
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00003E4F File Offset: 0x0000204F
		internal void RemovePromotionBorderInstalledApp()
		{
			this.IsAppSuggestionActive = false;
			this.ApplyBorder("");
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00003E63 File Offset: 0x00002063
		private void ApplyBorder(string path)
		{
			if (this.mPromotionId == null)
			{
				this.ApplyImageBorder = path;
			}
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00003E74 File Offset: 0x00002074
		internal void DownloadStarted()
		{
			this.mIsAppInstalled = false;
			this.IsDownLoadingFailed = false;
			this.IsDownloading = true;
			Action<AppIconDownloadingPhases> action = this.mAppDownloadingEvent;
			if (action == null)
			{
				return;
			}
			action(AppIconDownloadingPhases.DownloadStarted);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00003E9C File Offset: 0x0000209C
		internal void UpdateAppDownloadProgress(int percent)
		{
			this.DownloadPercentage = (double)percent;
			Action<AppIconDownloadingPhases> action = this.mAppDownloadingEvent;
			if (action == null)
			{
				return;
			}
			action(AppIconDownloadingPhases.Downloading);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00003EB7 File Offset: 0x000020B7
		internal void DownloadFailed()
		{
			this.IsDownLoadingFailed = true;
			Action<AppIconDownloadingPhases> action = this.mAppDownloadingEvent;
			if (action == null)
			{
				return;
			}
			action(AppIconDownloadingPhases.DownloadFailed);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00003ED1 File Offset: 0x000020D1
		internal void DownloadCompleted(string filePath)
		{
			this.IsDownloading = false;
			this.IsInstalling = true;
			this.ApkFilePath = filePath;
			Action<AppIconDownloadingPhases> action = this.mAppDownloadingEvent;
			if (action == null)
			{
				return;
			}
			action(AppIconDownloadingPhases.DownloadCompleted);
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00003EF9 File Offset: 0x000020F9
		internal void ApkInstallStart(string filePath)
		{
			this.mIsAppInstalled = false;
			this.IsInstalling = true;
			this.IsInstallingFailed = false;
			this.ApkFilePath = filePath;
			Action<AppIconDownloadingPhases> action = this.mAppDownloadingEvent;
			if (action == null)
			{
				return;
			}
			action(AppIconDownloadingPhases.InstallStarted);
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00003F28 File Offset: 0x00002128
		internal void ApkInstallFailed()
		{
			if (!this.mIsAppInstalled)
			{
				this.IsInstallingFailed = true;
			}
			Action<AppIconDownloadingPhases> action = this.mAppDownloadingEvent;
			if (action == null)
			{
				return;
			}
			action(AppIconDownloadingPhases.InstallFailed);
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00003F4A File Offset: 0x0000214A
		internal void ApkInstallCompleted()
		{
			this.mIsAppInstalled = true;
			this.IsInstalling = false;
			Action<AppIconDownloadingPhases> action = this.mAppDownloadingEvent;
			if (action == null)
			{
				return;
			}
			action(AppIconDownloadingPhases.InstallCompleted);
		}

		// Token: 0x0400014E RID: 334
		private AppInfo mAppInfoItem;

		// Token: 0x04000151 RID: 337
		private Visibility mAppIconVisibility;

		// Token: 0x04000152 RID: 338
		private string mImageName = string.Empty;

		// Token: 0x04000153 RID: 339
		private string mAppName = string.Empty;

		// Token: 0x04000154 RID: 340
		private string mAppNameTooltip;

		// Token: 0x04000155 RID: 341
		private TextTrimming mAppNameTextTrimming;

		// Token: 0x04000156 RID: 342
		private TextWrapping mAppNameTextWrapping = TextWrapping.NoWrap;

		// Token: 0x04000157 RID: 343
		private bool mIsGamepadCompatible;

		// Token: 0x04000158 RID: 344
		private bool mIsGamepadConnected;

		// Token: 0x04000159 RID: 345
		private bool mIsRedDotVisible;

		// Token: 0x0400015F RID: 351
		private AppIncompatType mAppIncompatType;

		// Token: 0x0400016D RID: 365
		private string mApplyImageBorder = string.Empty;
	}
}
