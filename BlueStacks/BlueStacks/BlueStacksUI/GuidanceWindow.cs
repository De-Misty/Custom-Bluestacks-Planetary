using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000141 RID: 321
	public class GuidanceWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000CEB RID: 3307 RVA: 0x0000A1A0 File Offset: 0x000083A0
		// (set) Token: 0x06000CEC RID: 3308 RVA: 0x0000A1A8 File Offset: 0x000083A8
		private bool IsVideoTutorialAvailable { get; set; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000CED RID: 3309 RVA: 0x0000A1B1 File Offset: 0x000083B1
		// (set) Token: 0x06000CEE RID: 3310 RVA: 0x00048530 File Offset: 0x00046730
		public bool IsViewState
		{
			get
			{
				return this.isViewState;
			}
			set
			{
				this.isViewState = value;
				this.mSchemesComboBox.IsEnabled = this.isViewState;
				if (this.isViewState)
				{
					this.mEditKeysGrid.Visibility = Visibility.Collapsed;
					this.mControlsTab.Visibility = Visibility.Visible;
					this.mSchemeTextBlock.Visibility = Visibility.Visible;
					this.mSchemesComboBox.Visibility = Visibility.Visible;
					return;
				}
				this.mEditKeysGrid.Visibility = Visibility.Visible;
				this.mControlsTab.Visibility = Visibility.Collapsed;
				this.mSchemeTextBlock.Visibility = Visibility.Collapsed;
				this.mSchemesComboBox.Visibility = Visibility.Collapsed;
				this.mVideoBorder.Visibility = Visibility.Collapsed;
				this.mReadArticlePanel.Visibility = Visibility.Collapsed;
				this.mHowToPlayGrid.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000CEF RID: 3311 RVA: 0x0000A1B9 File Offset: 0x000083B9
		// (set) Token: 0x06000CF0 RID: 3312 RVA: 0x0000A1C0 File Offset: 0x000083C0
		public static bool sIsDirty
		{
			get
			{
				return GuidanceWindow.IsDirty;
			}
			set
			{
				GuidanceWindow.IsDirty = value;
			}
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x000485E4 File Offset: 0x000467E4
		internal GuidanceWindow(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			base.Owner = window;
			base.IsShowGLWindow = true;
			this.mIsOnboardingPopupToBeShownOnGuidanceClose = false;
			this.ShowWithParentWindow = true;
			if (window.WindowState != WindowState.Normal)
			{
				window.RestoreWindows(false);
			}
			KMManager.CloseWindows();
			this.ResizeGuidanceWindow();
			this.mGuidanceHasBeenMoved = false;
			this.ResetGuidanceTab();
			this.Init();
			GuidanceWindow.HideOnNextLaunch(false);
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0000A1C8 File Offset: 0x000083C8
		internal void Init()
		{
			this.FillProfileComboBox();
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x000486B0 File Offset: 0x000468B0
		internal void InitUI()
		{
			GuidanceWindow.IsDirty = false;
			this.mGuidanceData.Clear();
			foreach (IMAction imaction in this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls)
			{
				if (imaction.Type == KeyActionType.Pan)
				{
					this.lstPanTags.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name);
				}
				else if (imaction.Type == KeyActionType.MOBADpad)
				{
					this.lstMOBATags.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name);
				}
				string text = string.Empty;
				if (this.ParentWindow.SelectedConfig.SelectedControlScheme.IsCategoryVisible)
				{
					text = (string.Equals(imaction.GuidanceCategory.Trim(), "MISC", StringComparison.InvariantCulture) ? LocaleStrings.GetLocalizedString("STRING_" + imaction.GuidanceCategory.Trim(), "") : this.ParentWindow.SelectedConfig.GetUIString(imaction.GuidanceCategory.Trim()));
				}
				Dictionary<string, string> dictionary = imaction.Guidance.DeepCopy<Dictionary<string, string>>();
				if (imaction.Type == KeyActionType.Dpad)
				{
					dictionary.Clear();
					foreach (object obj in Enum.GetValues(typeof(DPadControls)))
					{
						DPadControls dpadControls = (DPadControls)obj;
						if (imaction.Guidance.ContainsKey(dpadControls.ToString()))
						{
							dictionary.Add(dpadControls.ToString(), imaction.Guidance[dpadControls.ToString()]);
						}
					}
				}
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					string text2 = string.Empty;
					MOBASkill mobaskill = imaction as MOBASkill;
					if (mobaskill != null && keyValuePair.Key.Contains("KeyActivate"))
					{
						text2 = GuidanceWindow.AppendMOBASkillModeInGuidance(mobaskill);
					}
					bool flag = keyValuePair.Key.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase);
					if (!flag && IMAction.DictPropertyInfo[imaction.Type].ContainsKey(keyValuePair.Key))
					{
						this.mGuidanceData.AddGuidance(false, text, this.ParentWindow.SelectedConfig.GetUIString(imaction.Guidance[keyValuePair.Key]) + text2, imaction[keyValuePair.Key].ToString(), keyValuePair.Key, imaction);
					}
					string text3 = (flag ? keyValuePair.Key : (keyValuePair.Key + "_alt1"));
					if (IMAction.DictPropertyInfo[imaction.Type].ContainsKey(text3))
					{
						this.mGuidanceData.AddGuidance(true, text, this.ParentWindow.SelectedConfig.GetUIString(imaction.Guidance[keyValuePair.Key]) + text2, imaction[text3].ToString(), text3, imaction);
					}
				}
			}
			this.mGamepadIcon.Visibility = ((this.mGuidanceData.GamepadViewGuidance.Count > 0) ? Visibility.Visible : Visibility.Collapsed);
			this.mGuidanceData.SaveOriginalData();
			if (this.mGuidanceData.GamepadViewGuidance.Count > 0 || this.mGuidanceData.KeymapViewGuidance.Count > 0)
			{
				this.mKeyboardIcon.Visibility = Visibility.Visible;
				this.separator.Visibility = Visibility.Visible;
				this.noGameGuidePanel.Visibility = Visibility.Collapsed;
				this.AddVideoElementInUI();
			}
			else
			{
				this.mKeyboardIcon.Visibility = Visibility.Collapsed;
				this.separator.Visibility = Visibility.Collapsed;
				this.noGameGuidePanel.Visibility = Visibility.Visible;
			}
			this.ShowGuidance();
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x00048ADC File Offset: 0x00046CDC
		private static string AppendMOBASkillModeInGuidance(MOBASkill mobaSkill)
		{
			string text = string.Empty;
			if (!mobaSkill.AdvancedMode && !mobaSkill.AutocastEnabled)
			{
				text = string.Format(CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_MANUAL_MODE", "") + ")", new object[0]);
			}
			else if (mobaSkill.AdvancedMode && !mobaSkill.AutocastEnabled)
			{
				text = string.Format(CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_AUTOCAST", "") + ")", new object[0]);
			}
			else if (mobaSkill.AdvancedMode && mobaSkill.AutocastEnabled)
			{
				text = string.Format(CultureInfo.InvariantCulture, " (" + LocaleStrings.GetLocalizedString("STRING_QUICK_CAST", "") + ")", new object[0]);
			}
			return text;
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x00048BB4 File Offset: 0x00046DB4
		private void ResetGuidanceTab()
		{
			this.mIsGamePadTabSelected = false;
			this.mGamepadIconImage.ImageName = "guidance_gamepad";
			BlueStacksUIBinding.BindColor(this.mGamepadIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
			this.mKeyboardIconImage.ImageName = "guidance_controls_hover";
			BlueStacksUIBinding.BindColor(this.mKeyboardIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x00046D4C File Offset: 0x00044F4C
		private int CompareSchemesAlphabetically(IMControlScheme x, IMControlScheme y)
		{
			string text = x.Name.ToLower(CultureInfo.InvariantCulture).Trim();
			string text2 = y.Name.ToLower(CultureInfo.InvariantCulture).Trim();
			if (text.Contains(text2))
			{
				return 1;
			}
			if (text2.Contains(text))
			{
				return -1;
			}
			if (string.CompareOrdinal(text, text2) < 0)
			{
				return -1;
			}
			return 1;
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x00048C14 File Offset: 0x00046E14
		internal void OrderingControlSchemes()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.ParentWindow.SelectedConfig.ControlSchemes.Sort(new Comparison<IMControlScheme>(this.CompareSchemesAlphabetically));
			foreach (IMControlScheme imcontrolScheme in new List<IMControlScheme>(this.ParentWindow.SelectedConfig.ControlSchemes))
			{
				if (imcontrolScheme.BuiltIn)
				{
					if (imcontrolScheme.IsBookMarked)
					{
						this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imcontrolScheme);
						this.ParentWindow.SelectedConfig.ControlSchemes.Insert(num3, imcontrolScheme);
						num3++;
						num2++;
						num++;
					}
					else
					{
						this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imcontrolScheme);
						this.ParentWindow.SelectedConfig.ControlSchemes.Insert(num2, imcontrolScheme);
						num2++;
						num++;
					}
				}
				else if (imcontrolScheme.IsBookMarked)
				{
					this.ParentWindow.SelectedConfig.ControlSchemes.Remove(imcontrolScheme);
					this.ParentWindow.SelectedConfig.ControlSchemes.Insert(num, imcontrolScheme);
					num++;
				}
			}
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x00048D68 File Offset: 0x00046F68
		internal void FillProfileComboBox()
		{
			this.OrderingControlSchemes();
			this.mSchemesComboBox.Items.Clear();
			if (this.ParentWindow.SelectedConfig.ControlSchemes != null && this.ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
			{
				bool flag = false;
				foreach (IMControlScheme imcontrolScheme in this.ParentWindow.SelectedConfig.ControlSchemesDict.Values)
				{
					ComboBoxItem comboBoxItem = new ComboBoxItem
					{
						Content = imcontrolScheme.Name
					};
					if (string.Equals(imcontrolScheme.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture))
					{
						comboBoxItem.IsSelected = true;
						flag = true;
					}
					comboBoxItem.ToolTip = comboBoxItem.Content;
					this.mSchemesComboBox.Items.Add(comboBoxItem);
				}
				if (!flag)
				{
					((ComboBoxItem)this.mSchemesComboBox.Items[0]).IsSelected = true;
				}
				this.mSchemePanel.Visibility = ((this.ParentWindow.SelectedConfig.ControlSchemesDict.Count == 1) ? Visibility.Collapsed : Visibility.Visible);
				this.InitUI();
			}
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x00048EB4 File Offset: 0x000470B4
		private void AddVideoElementInUI()
		{
			foreach (AppInfo appInfo in new JsonParser(Strings.CurrentDefaultVmName).GetAppList().ToList<AppInfo>())
			{
				if (string.Equals(appInfo.Package, KMManager.sPackageName, StringComparison.InvariantCulture))
				{
					this.mIsGuidanceVideoPresent = appInfo.VideoPresent;
				}
			}
			this.UpdateVideoElement(this.mIsGamePadTabSelected);
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x00048F3C File Offset: 0x0004713C
		private void UpdateVideoElement(bool isGamepadTabSelected = false)
		{
			if (isGamepadTabSelected)
			{
				KMManager.sVideoMode = GuidanceVideoType.Gamepad;
			}
			else if (this.lstMOBATags.Contains(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name) || this.lstMOBATags.Contains("GlobalValidTag"))
			{
				KMManager.sVideoMode = GuidanceVideoType.Moba;
			}
			else if (this.lstPanTags.Contains(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name) || this.lstPanTags.Contains("GlobalValidTag"))
			{
				KMManager.sVideoMode = GuidanceVideoType.Pan;
			}
			else
			{
				KMManager.sVideoMode = GuidanceVideoType.Default;
			}
			this.UpdateTutorialGrid();
			this.UpdateReadArticleGrid();
			this.GuidanceVisualInfoVisibility();
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00048FE4 File Offset: 0x000471E4
		private void UpdateTutorialGrid()
		{
			string text = "";
			try
			{
				Dictionary<string, CustomThumbnail> customThumbnails = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.CustomThumbnails;
				Dictionary<GuidanceVideoType, VideoThumbnailInfo> defaultThumbnails = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.DefaultThumbnails;
				if (customThumbnails.ContainsKey(KMManager.sPackageName))
				{
					if (KMManager.sVideoMode == GuidanceVideoType.Gamepad && customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()] != null)
					{
						text = ((VideoThumbnailInfo)customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()]).ImagePath;
					}
					else if (customThumbnails[KMManager.sPackageName][GuidanceVideoType.SchemeSpecific.ToString()] != null && ((Dictionary<string, VideoThumbnailInfo>)customThumbnails[KMManager.sPackageName][GuidanceVideoType.SchemeSpecific.ToString()]).ContainsKey(this.ParentWindow.SelectedConfig.SelectedControlScheme.Name))
					{
						string name = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
						text = ((Dictionary<string, VideoThumbnailInfo>)customThumbnails[KMManager.sPackageName][GuidanceVideoType.SchemeSpecific.ToString()])[name].ImagePath;
						KMManager.sVideoMode = GuidanceVideoType.SchemeSpecific;
					}
					else if (customThumbnails[KMManager.sPackageName][GuidanceVideoType.Special.ToString()] != null)
					{
						text = ((VideoThumbnailInfo)customThumbnails[KMManager.sPackageName][GuidanceVideoType.Special.ToString()]).ImagePath;
						KMManager.sVideoMode = GuidanceVideoType.Special;
					}
					else if (customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()] != null)
					{
						text = ((VideoThumbnailInfo)customThumbnails[KMManager.sPackageName][KMManager.sVideoMode.ToString()]).ImagePath;
					}
				}
				else if (defaultThumbnails.ContainsKey(KMManager.sVideoMode))
				{
					text = defaultThumbnails[KMManager.sVideoMode].ImagePath;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in evaluating tutorial grid : " + ex.ToString());
			}
			this.mVideoThumbnail.ImageName = text;
			this.IsVideoTutorialAvailable = !string.IsNullOrEmpty(text);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x0004924C File Offset: 0x0004744C
		private void UpdateReadArticleGrid()
		{
			this.mHelpArticleUrl = null;
			try
			{
				Dictionary<string, HelpArticle> helpArticles = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.HelpArticles;
				if (helpArticles.ContainsKey(KMManager.sPackageName) && helpArticles[KMManager.sPackageName][KMManager.sVideoMode.ToString()] != null)
				{
					string name = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
					Dictionary<string, HelpArticleInfo> dictionary = (Dictionary<string, HelpArticleInfo>)helpArticles[KMManager.sPackageName][GuidanceVideoType.SchemeSpecific.ToString()];
					if (KMManager.sVideoMode == GuidanceVideoType.SchemeSpecific && dictionary.ContainsKey(name))
					{
						this.mHelpArticleUrl = dictionary[name].HelpArticleUrl;
					}
					else if (KMManager.sVideoMode != GuidanceVideoType.SchemeSpecific)
					{
						this.mHelpArticleUrl = ((HelpArticleInfo)helpArticles[KMManager.sPackageName][KMManager.sVideoMode.ToString()]).HelpArticleUrl;
					}
				}
				else if (helpArticles.ContainsKey("default") && helpArticles["default"][KMManager.sVideoMode.ToString()] != null)
				{
					this.mHelpArticleUrl = ((HelpArticleInfo)helpArticles["default"][KMManager.sVideoMode.ToString()]).HelpArticleUrl;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in evaluating read article : " + ex.ToString());
			}
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x00009FEE File Offset: 0x000081EE
		private void GuidanceWindow_Loaded(object sender, RoutedEventArgs e)
		{
			base.Activate();
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x000493DC File Offset: 0x000475DC
		internal void ResizeGuidanceWindow()
		{
			bool flag = false;
			IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.ParentWindow.Handle, true);
			double num = this.ParentWindow.Width * MainWindow.sScalingFactor;
			double num2 = this.ParentWindow.Height * MainWindow.sScalingFactor;
			if (num + (double)this.mSidebarWidth * MainWindow.sScalingFactor + this.ParentWindow.mSidebar.Width * MainWindow.sScalingFactor > (double)fullscreenMonitorSize.Width)
			{
				num = (double)fullscreenMonitorSize.Width - (double)this.mSidebarWidth * MainWindow.sScalingFactor - this.ParentWindow.mSidebar.Width * MainWindow.sScalingFactor;
				num2 = this.ParentWindow.GetHeightFromWidth(num, true, false);
				flag = true;
			}
			if (num2 > (double)fullscreenMonitorSize.Height)
			{
				num2 = (double)fullscreenMonitorSize.Height;
				num = this.ParentWindow.GetWidthFromHeight(num2, true, false);
				flag = true;
			}
			double num3;
			if (this.ParentWindow.Top * MainWindow.sScalingFactor + num2 > (double)(fullscreenMonitorSize.Height + fullscreenMonitorSize.Y))
			{
				num3 = (double)(fullscreenMonitorSize.Y + fullscreenMonitorSize.Height) - num2;
				flag = true;
			}
			else
			{
				num3 = this.ParentWindow.Top * MainWindow.sScalingFactor;
			}
			double num4;
			if (this.ParentWindow.Left * MainWindow.sScalingFactor + num + ((double)this.mSidebarWidth + this.ParentWindow.mSidebar.Width) * MainWindow.sScalingFactor > (double)(fullscreenMonitorSize.Width + fullscreenMonitorSize.X))
			{
				num4 = (double)(fullscreenMonitorSize.X + fullscreenMonitorSize.Width) - num - ((double)this.mSidebarWidth + this.ParentWindow.mSidebar.Width) * MainWindow.sScalingFactor;
				flag = true;
			}
			else
			{
				num4 = this.ParentWindow.Left * MainWindow.sScalingFactor;
			}
			if (flag)
			{
				this.ParentWindow.ChangeHeightWidthTopLeft(num, num2, num3, num4);
			}
			base.Left = ((this.mGuidanceWindowLeft == -1.0) ? (this.ParentWindow.Left + this.ParentWindow.ActualWidth) : this.mGuidanceWindowLeft);
			base.Top = ((this.mGuidanceWindowTop == -1.0) ? this.ParentWindow.Top : this.mGuidanceWindowTop);
			base.Height = this.ParentWindow.ActualHeight;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x0000A1D0 File Offset: 0x000083D0
		internal void DimOverLayVisibility(Visibility visible)
		{
			this.mOverlayGrid.Visibility = visible;
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0004961C File Offset: 0x0004781C
		private void GuidanceWindow_Closing(object sender, CancelEventArgs e)
		{
			if (!this.IsViewState && (GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged(this.mGuidanceData)))
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
				customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CANCEL_CONFIG_CHANGES", "");
				customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_DISCARD", ""), delegate(object o, EventArgs e)
				{
					KMManager.LoadIMActions(this.ParentWindow, KMManager.sPackageName);
					GuidanceWindow.sIsDirty = false;
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), delegate(object o, EventArgs evt)
				{
					e.Cancel = true;
				}, null, false, null);
				customMessageWindow.CloseButtonHandle(delegate(object o, EventArgs evt)
				{
					e.Cancel = true;
				}, null);
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
			}
			this.mGuidanceWindowLeft = base.Left;
			this.mGuidanceWindowTop = base.Top;
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x00049744 File Offset: 0x00047944
		private void GuidanceWindow_Closed(object sender, EventArgs e)
		{
			if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (AppSettings appSettings) => appSettings.IsCloseGuidanceOnboardingCompleted) && this.mIsOnboardingPopupToBeShownOnGuidanceClose)
			{
				Sidebar mSidebar = this.ParentWindow.mSidebar;
				if (mSidebar != null)
				{
					mSidebar.ShowViewGuidancePopup();
				}
				AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName].IsCloseGuidanceOnboardingCompleted = true;
			}
			KMManager.sGuidanceWindow = null;
			this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
			this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mIsAnyOperationPendingForTab = false;
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x00049830 File Offset: 0x00047A30
		private void ProfileComboBox_ProfileChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!this.mSchemesComboBox.IsDropDownOpen)
			{
				return;
			}
			this.mSchemesComboBox.IsDropDownOpen = false;
			if (this.mSchemesComboBox.SelectedItem != null)
			{
				string text = ((ComboBoxItem)this.mSchemesComboBox.SelectedItem).Content.ToString();
				if (this.SelectControlScheme(text))
				{
					this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_USING_SCHEME", "") + " : " + text);
					KMManager.SendSchemeChangedStats(this.ParentWindow, "game_guide");
					KMManager.ShowShootingModeTooltip(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
				}
			}
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x000498E0 File Offset: 0x00047AE0
		internal bool SelectControlScheme(string schemeSelected)
		{
			if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(schemeSelected))
			{
				if (!this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeSelected].Selected)
				{
					this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = false;
					this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeSelected];
					this.ParentWindow.SelectedConfig.SelectedControlScheme.Selected = true;
					if (!GuidanceWindow.sIsDirty)
					{
						GuidanceWindow.sIsDirty = true;
						this.SaveGuidanceChanges();
					}
					else
					{
						GuidanceWindow.sIsDirty = false;
					}
					this.mSchemesComboBox.SelectedValue = schemeSelected;
					return true;
				}
				this.InitUI();
			}
			return false;
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x000499A8 File Offset: 0x00047BA8
		private void AddToastPopup(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this);
				}
				this.mToastPopup.Init(this, message, null, null, HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 12, null, null, false);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x00047088 File Offset: 0x00045288
		private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				base.DragMove();
			}
			catch
			{
			}
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void CustomPictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x00049A30 File Offset: 0x00047C30
		private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mIsOnboardingPopupToBeShownOnGuidanceClose = true;
			this.ShowWithParentWindow = false;
			base.Close();
			GuidanceWindow.HideOnNextLaunch(true);
			this.ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
			if (this.ParentWindow != null)
			{
				this.ParentWindow.Focus();
			}
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0000A1DE File Offset: 0x000083DE
		internal void RestartConfirmationAcceptedHandler(object sender, EventArgs e)
		{
			Logger.Info("Restarting Game Tab.");
			new Thread(delegate
			{
				this.ParentWindow.mTopBar.mAppTabButtons.RestartTab(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName);
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x00049A84 File Offset: 0x00047C84
		public static void HideOnNextLaunch(bool updatedFlag)
		{
			List<string> list = new List<string>(RegistryManager.Instance.DisabledGuidancePackages);
			if (updatedFlag)
			{
				list.AddIfNotContain(KMManager.sPackageName);
			}
			else if (list.Contains(KMManager.sPackageName))
			{
				list.Remove(KMManager.sPackageName);
			}
			RegistryManager.Instance.DisabledGuidancePackages = list.ToArray();
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x00049ADC File Offset: 0x00047CDC
		internal OnBoardingPopupWindow GuidanceSchemeOnboardingBlurb()
		{
			OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
			onBoardingPopupWindow.Owner = this.ParentWindow;
			onBoardingPopupWindow.Title = "SelectedGameSchemeBlurb";
			onBoardingPopupWindow.PlacementTarget = this.mSchemesComboBox;
			onBoardingPopupWindow.LeftMargin = 50;
			onBoardingPopupWindow.TopMargin = 4;
			onBoardingPopupWindow.Startevent += delegate
			{
				this.mSchemesComboBox.Highlight = true;
			};
			onBoardingPopupWindow.Endevent += delegate
			{
				this.mSchemesComboBox.Highlight = false;
			};
			onBoardingPopupWindow.IsBlurbRelatedToGuidance = true;
			onBoardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_SELECTED_MODE", "");
			onBoardingPopupWindow.BodyContent = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SELECTED_MODE_MESSAGE", ""), new object[] { this.ParentWindow.SelectedConfig.SelectedControlScheme.Name });
			onBoardingPopupWindow.Left = this.mSchemesComboBox.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin;
			onBoardingPopupWindow.Top = this.mSchemesComboBox.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin;
			return onBoardingPopupWindow;
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x00049C3C File Offset: 0x00047E3C
		internal OnBoardingPopupWindow GuidanceOnboardingBlurb()
		{
			if (this.mGuidanceKeysGrid.ActualHeight < 1.0)
			{
				return null;
			}
			OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
			onBoardingPopupWindow.Owner = this.ParentWindow;
			onBoardingPopupWindow.PlacementTarget = this.mGuidanceKeysGrid;
			onBoardingPopupWindow.Title = "GameControlBlurb";
			onBoardingPopupWindow.LeftMargin = 320;
			onBoardingPopupWindow.TopMargin = (230 - (int)this.mGuidanceKeysGrid.ActualHeight) / 2;
			onBoardingPopupWindow.Startevent += delegate
			{
				this.mGuidanceKeyBorder.BorderThickness = new Thickness(2.0);
			};
			onBoardingPopupWindow.Endevent += delegate
			{
				this.mGuidanceKeyBorder.BorderThickness = new Thickness(0.0);
			};
			onBoardingPopupWindow.IsBlurbRelatedToGuidance = true;
			onBoardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_GAME_CONTROLS_HEADER", "");
			onBoardingPopupWindow.BodyContent = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_GAME_CONTROLS_MESSAGE", ""), new object[0]);
			onBoardingPopupWindow.PopArrowAlignment = PopupArrowAlignment.Right;
			onBoardingPopupWindow.SetValue(Window.LeftProperty, this.mGuidanceKeysGrid.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin);
			onBoardingPopupWindow.SetValue(Window.TopProperty, this.mGuidanceKeysGrid.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin);
			return onBoardingPopupWindow;
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x00049DCC File Offset: 0x00047FCC
		internal OnBoardingPopupWindow GuidanceVideoOnboardingBlurb()
		{
			if (this.mVideoBorder.Visibility != Visibility.Visible && this.mQuickLearnBorder.Visibility != Visibility.Visible)
			{
				return null;
			}
			OnBoardingPopupWindow onBoardingPopupWindow = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
			onBoardingPopupWindow.Owner = this.ParentWindow;
			onBoardingPopupWindow.Title = "GuidanceVideoBlurb";
			onBoardingPopupWindow.IsBlurbRelatedToGuidance = true;
			onBoardingPopupWindow.HeaderContent = LocaleStrings.GetLocalizedString("STRING_VIDEO_TUTORIAL_BLURB_HEADER", "");
			onBoardingPopupWindow.BodyContent = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_VIDEO_TUTORIAL_BLURB_MESSAGE", ""), new object[0]);
			onBoardingPopupWindow.PopArrowAlignment = PopupArrowAlignment.Right;
			onBoardingPopupWindow.LeftMargin = 320;
			if (this.mVideoBorder.Visibility == Visibility.Visible)
			{
				onBoardingPopupWindow.PlacementTarget = this.mVideoBorder;
				onBoardingPopupWindow.TopMargin = ((int)this.mVideoBorder.ActualHeight - 80) / 2;
				onBoardingPopupWindow.Startevent += delegate
				{
					this.mVideoBorder.BorderThickness = new Thickness(2.0);
				};
				onBoardingPopupWindow.Endevent += delegate
				{
					this.mVideoBorder.BorderThickness = new Thickness(0.0);
				};
				onBoardingPopupWindow.SetValue(Window.LeftProperty, this.mVideoBorder.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin);
				onBoardingPopupWindow.SetValue(Window.TopProperty, this.mVideoBorder.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin);
			}
			else
			{
				onBoardingPopupWindow.PlacementTarget = this.mQuickLearnBorder;
				onBoardingPopupWindow.TopMargin = ((int)this.mQuickLearnBorder.ActualHeight + 160) / 2;
				onBoardingPopupWindow.Startevent += delegate
				{
					this.mQuickLearnBorder.BorderThickness = new Thickness(2.0);
				};
				onBoardingPopupWindow.Endevent += delegate
				{
					this.mQuickLearnBorder.BorderThickness = new Thickness(0.0);
				};
				onBoardingPopupWindow.SetValue(Window.LeftProperty, this.mQuickLearnBorder.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.LeftMargin);
				onBoardingPopupWindow.SetValue(Window.TopProperty, this.mQuickLearnBorder.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double)onBoardingPopupWindow.TopMargin);
			}
			return onBoardingPopupWindow;
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x0000A207 File Offset: 0x00008407
		private void ControlsTabMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mControlsGrid.Visibility = Visibility.Visible;
			BlueStacksUIBinding.BindColor(this.mControlsTabTextBlock, TextBlock.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x0000A22A File Offset: 0x0000842A
		private void SettingsTabMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mControlsGrid.Visibility = Visibility.Collapsed;
			BlueStacksUIBinding.BindColor(this.mControlsTabTextBlock, TextBlock.ForegroundProperty, "SettingsWindowForegroundDimColor");
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x0004A04C File Offset: 0x0004824C
		private void CustomPictureBox_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("guidance-howtoplay", "watchvideo", this.ParentWindow.mVmName, KMManager.sPackageName, "", "");
			using (GuidanceVideoWindow guidanceVideoWindow = new GuidanceVideoWindow(this.ParentWindow))
			{
				guidanceVideoWindow.Owner = this.ParentWindow;
				guidanceVideoWindow.Width = Math.Max(this.ParentWindow.ActualWidth * 0.7, 700.0);
				guidanceVideoWindow.Height = Math.Max(this.ParentWindow.ActualHeight * 0.7, 450.0);
				guidanceVideoWindow.Loaded += this.Window_Loaded;
				guidanceVideoWindow.ShowDialog();
			}
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x0004A124 File Offset: 0x00048324
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.mGuidanceHasBeenMoved)
				{
					CustomWindow customWindow = sender as CustomWindow;
					customWindow.Left = this.ParentWindow.Left + (this.ParentWindow.Width + KMManager.sGuidanceWindow.ActualWidth - customWindow.ActualWidth) / 2.0;
					customWindow.Top = this.ParentWindow.Top + (this.ParentWindow.Height - customWindow.ActualHeight) / 2.0;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in setting position guidance video window: " + ex.ToString());
			}
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x0004A1DC File Offset: 0x000483DC
		private void GamepadIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mKeyboardIconImage.ImageName = "guidance_controls";
			BlueStacksUIBinding.BindColor(this.mKeyboardIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
			this.mGamepadIconImage.ImageName = "guidance_gamepad_hover";
			BlueStacksUIBinding.BindColor(this.mGamepadIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
			this.mIsGamePadTabSelected = true;
			this.ShowGuidance();
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x0004A240 File Offset: 0x00048440
		private void KeyboardIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mGamepadIconImage.ImageName = "guidance_gamepad";
			BlueStacksUIBinding.BindColor(this.mGamepadIconSeparator, Panel.BackgroundProperty, "HorizontalSeparator");
			this.mKeyboardIconImage.ImageName = "guidance_controls_hover";
			BlueStacksUIBinding.BindColor(this.mKeyboardIconSeparator, Panel.BackgroundProperty, "SettingsWindowTabMenuItemUnderline");
			this.mIsGamePadTabSelected = false;
			this.ShowGuidance();
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x0004A2A4 File Offset: 0x000484A4
		private void ReadMoreLinkMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("guidance-howtoplay", "readarticle", this.ParentWindow.mVmName, KMManager.sPackageName, "", "");
			if (this.mHelpArticleUrl != null)
			{
				Utils.OpenUrl(this.mHelpArticleUrl);
			}
			e.Handled = true;
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x0004A2F4 File Offset: 0x000484F4
		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (this.mHeaderGrid.IsMouseOver && !e.OriginalSource.GetType().Equals(typeof(TextBlock)) && !this.mControlsTab.IsMouseOver)
				{
					base.DragMove();
					this.mGuidanceHasBeenMoved = true;
					base.ResizeMode = ResizeMode.CanResizeWithGrip;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0004A360 File Offset: 0x00048560
		internal void GuidanceWindowTabSelected(string mSelectedTab)
		{
			mSelectedTab = (string.IsNullOrEmpty(mSelectedTab) ? ((this.mGuidanceData.GamepadViewGuidance.Count > 0 && this.ParentWindow.IsGamepadConnected) ? "gamepad" : "default") : mSelectedTab);
			if (mSelectedTab == "gamepad")
			{
				if (this.mGamepadIcon.Visibility == Visibility.Visible)
				{
					this.GamepadIconPreviewMouseLeftButtonUp(null, null);
					return;
				}
			}
			else if (this.mKeyboardIcon.Visibility == Visibility.Visible)
			{
				this.KeyboardIconPreviewMouseLeftButtonUp(null, null);
			}
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0004A3E0 File Offset: 0x000485E0
		private void GuidanceWindow_KeyDown(object sender, KeyEventArgs e)
		{
			string text = string.Empty;
			if (e.Key != Key.None)
			{
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
				{
					text = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
				}
				if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
				{
					text = text + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
				}
				if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				{
					text = text + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
				}
				text += IMAPKeys.GetStringForFile(e.Key);
			}
			Logger.Debug("SHORTCUT: KeyPressed.." + text);
			if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
			{
				foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
				{
					if (string.Equals(shortcutKeys.ShortcutKey, text, StringComparison.InvariantCulture))
					{
						if (string.Equals(shortcutKeys.ShortcutName, "STRING_TOGGLE_KEYMAP_WINDOW", StringComparison.InvariantCulture))
						{
							this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("default");
						}
						else if (string.Equals(shortcutKeys.ShortcutName, "STRING_GAMEPAD_CONTROLS", StringComparison.InvariantCulture))
						{
							this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad");
						}
					}
				}
			}
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0004A554 File Offset: 0x00048754
		internal void UpdateSize()
		{
			if (!this.mGuidanceHasBeenMoved)
			{
				base.Left = ((this.mGuidanceWindowLeft == -1.0) ? (this.ParentWindow.Left + this.ParentWindow.ActualWidth) : this.mGuidanceWindowLeft);
				base.Top = ((this.mGuidanceWindowTop == -1.0) ? this.ParentWindow.Top : this.mGuidanceWindowTop);
				base.Height = this.ParentWindow.ActualHeight;
			}
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0000A24D File Offset: 0x0000844D
		private void CustomWindow_StateChanged(object sender, EventArgs e)
		{
			base.WindowState = WindowState.Normal;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0004A5DC File Offset: 0x000487DC
		private void GuidanceWindow_Activated(object sender, EventArgs e)
		{
			try
			{
				if (RegistryManager.Instance.ShowKeyControlsOverlay && KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow))
				{
					KeymapCanvasWindow keymapCanvasWindow = KMManager.dictOverlayWindow[this.ParentWindow];
					if (keymapCanvasWindow != null)
					{
						keymapCanvasWindow.Show();
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format("Exception in GuidanceWindow_Activated {0}", ex));
			}
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0004A648 File Offset: 0x00048848
		private void GuidanceWindow_Deactivated(object sender, EventArgs e)
		{
			try
			{
				if (!this.ParentWindow.IsActive && KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow))
				{
					KMManager.dictOverlayWindow[this.ParentWindow].Hide();
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format("Exception in GuidanceWindow_Deactivated {0}", ex));
			}
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x0004A6B0 File Offset: 0x000488B0
		private void GuidanceKeyTextChanged(object sender, TextChangedEventArgs e)
		{
			IEnumerable<GuidanceCategoryEditModel> enumerable = this.mGuidanceData.KeymapEditGuidance.Union(this.mGuidanceData.GamepadEditGuidance);
			enumerable.SelectMany((GuidanceCategoryEditModel cgem) => cgem.GuidanceEditModels).OfType<GuidanceEditTextModel>().ToList<GuidanceEditTextModel>()
				.ForEach(delegate(GuidanceEditTextModel gem)
				{
					gem.TextValidityOption = TextValidityOptions.Success;
				});
			IMapTextBox mapTextBox = sender as IMapTextBox;
			if (mapTextBox != null)
			{
				object dataContext = mapTextBox.DataContext;
				GuidanceEditTextModel guidanceEditTextModel = dataContext as GuidanceEditTextModel;
				if (guidanceEditTextModel != null && !string.Equals(guidanceEditTextModel.OriginalGuidanceKey, guidanceEditTextModel.GuidanceKey, StringComparison.OrdinalIgnoreCase))
				{
					GuidanceWindow.sIsDirty = true;
					ToolTip toolTip = mapTextBox.ToolTip as ToolTip;
					if (toolTip != null)
					{
						toolTip.PlacementTarget = mapTextBox;
					}
					if ((from gem in enumerable.SelectMany((GuidanceCategoryEditModel cgem) => cgem.GuidanceEditModels).OfType<GuidanceEditTextModel>()
						where !string.IsNullOrEmpty(gem.GuidanceKey) && gem.PropertyType == typeof(string) && string.Equals(guidanceEditTextModel.GuidanceKey, gem.GuidanceKey, StringComparison.OrdinalIgnoreCase)
						select gem).Count<GuidanceEditTextModel>() > 1)
					{
						mapTextBox.InputTextValidity = TextValidityOptions.Warning;
						if (toolTip != null)
						{
							toolTip.IsOpen = true;
						}
					}
				}
			}
			this.mSaveBtn.IsEnabled = GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged(this.mGuidanceData);
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0004A818 File Offset: 0x00048A18
		private void StepperTextChanged(object sender, TextChangedEventArgs e)
		{
			StepperTextBox stepperTextBox = sender as StepperTextBox;
			if (stepperTextBox != null)
			{
				GuidanceEditDecimalModel guidanceEditDecimalModel = stepperTextBox.DataContext as GuidanceEditDecimalModel;
				if (guidanceEditDecimalModel != null && !string.Equals(guidanceEditDecimalModel.OriginalGuidanceKey, guidanceEditDecimalModel.GuidanceKey, StringComparison.OrdinalIgnoreCase))
				{
					GuidanceWindow.sIsDirty = true;
				}
			}
			this.mSaveBtn.IsEnabled = GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged(this.mGuidanceData);
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0004A880 File Offset: 0x00048A80
		private void EditBtn_Click(object sender, RoutedEventArgs e)
		{
			this.ShowEditGuidance();
			this.DataModificationTracker.Lock(this.mGuidanceData.DeepCopy<GuidanceData>(), new List<string> { "KeymapViewGuidance", "GamepadViewGuidance", "Item", "TextValidityOption", "GuidanceText", "OriginalGuidanceKey", "IMActionItems", "PropertyType", "ActionType" }, true);
			ClientStats.SendKeyMappingUIStatsAsync("guide_edit", KMManager.sPackageName, "");
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x0000A256 File Offset: 0x00008456
		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			this.SaveGuidanceChanges();
			this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
			ClientStats.SendKeyMappingUIStatsAsync("guide_save", KMManager.sPackageName, "");
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x0004A928 File Offset: 0x00048B28
		private void SaveGuidanceChanges()
		{
			Logger.Debug(string.Format("ExtraLog: SaveGuidanceChanges, VmName:{0}, Scheme:{1}, SchemeCount:{2}", this.ParentWindow.mVmName, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, this.ParentWindow.SelectedConfig.ControlSchemes.Count));
			bool flag = false;
			if (this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Count != this.ParentWindow.SelectedConfig.ControlSchemes.Count)
			{
				flag = true;
			}
			if (GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged(this.mGuidanceData))
			{
				GuidanceWindow.sIsDirty = true;
				KMManager.SaveIMActions(this.ParentWindow, true, false);
			}
			if (flag)
			{
				this.FillProfileComboBox();
			}
			else
			{
				this.InitUI();
			}
			this.ShowViewGuidance();
			if (KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow) && KMManager.dictOverlayWindow[this.ParentWindow] != null)
			{
				KMManager.dictOverlayWindow[this.ParentWindow].Init();
				if (RegistryManager.Instance.ShowKeyControlsOverlay)
				{
					KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
				}
			}
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0004AA44 File Offset: 0x00048C44
		private void DiscardBtn_Click(object sender, RoutedEventArgs e)
		{
			if (GuidanceWindow.sIsDirty || this.DataModificationTracker.HasChanged(this.mGuidanceData))
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISCARD_CHANGES", "");
				customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISCARD_GUIDANCE_CHANGES", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_DISCARD", delegate(object o, EventArgs e)
				{
					string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
					IEnumerable<IMControlScheme> enumerable = this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme_) => string.Equals(scheme_.Name, schemeName, StringComparison.InvariantCultureIgnoreCase));
					if (enumerable.Any<IMControlScheme>())
					{
						this.mGuidanceData.Reset();
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
								GuidanceWindow.sIsDirty = true;
							}
						}
						else
						{
							this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
							this.ParentWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme2.DeepCopy();
							this.ParentWindow.SelectedConfig.ControlSchemesDict[schemeName] = this.ParentWindow.SelectedConfig.SelectedControlScheme;
							this.ParentWindow.SelectedConfig.ControlSchemes.Add(this.ParentWindow.SelectedConfig.SelectedControlScheme);
							this.InitUI();
						}
						this.ShowViewGuidance();
					}
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", delegate(object o, EventArgs e)
				{
				}, null, false, null);
				this.ParentWindow.ShowDimOverlay(null);
				customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
				customMessageWindow.ShowDialog();
				this.ParentWindow.HideDimOverlay();
				return;
			}
			this.ShowViewGuidance();
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0000A287 File Offset: 0x00008487
		private void ShowGuidance()
		{
			if (this.IsViewState)
			{
				this.ShowViewGuidance();
				return;
			}
			this.ShowEditGuidance();
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0004AB2C File Offset: 0x00048D2C
		private void ShowViewGuidance()
		{
			this.IsViewState = true;
			this.mViewDock.Visibility = Visibility.Visible;
			this.mEditDock.Visibility = Visibility.Collapsed;
			if (this.mGuidanceData.GamepadViewGuidance.Count == 0)
			{
				this.mIsGamePadTabSelected = false;
			}
			ObservableCollection<GuidanceCategoryViewModel> observableCollection = (this.mIsGamePadTabSelected ? this.mGuidanceData.GamepadViewGuidance : this.mGuidanceData.KeymapViewGuidance);
			if (observableCollection != null)
			{
				if (observableCollection.Count == 1)
				{
					this.mGuidanceListBox.DataContext = observableCollection[0].GuidanceViewModels;
					this.mGuidanceListBox.AlternationCount = 2;
				}
				else
				{
					this.mGuidanceListBox.DataContext = observableCollection;
					this.mGuidanceListBox.AlternationCount = 0;
				}
			}
			UIElement uielement = this.mEditBtn;
			Visibility visibility;
			if (this.ParentWindow.SelectedConfig.SelectedControlScheme != null && this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any<IMAction>())
			{
				if (this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.SelectMany((IMAction action) => action.Guidance).Any<KeyValuePair<string, string>>())
				{
					visibility = Visibility.Visible;
					goto IL_011A;
				}
			}
			visibility = Visibility.Collapsed;
			IL_011A:
			uielement.Visibility = visibility;
			this.mRevertBtn.Visibility = ((this.ParentWindow.SelectedConfig.ControlSchemes.Count((IMControlScheme x) => string.Equals(x.Name, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, StringComparison.InvariantCulture)) == 2) ? Visibility.Visible : Visibility.Collapsed);
			this.UpdateVideoElement(this.mIsGamePadTabSelected);
			GuidanceWindow.sIsDirty = false;
			this.mSaveBtn.IsEnabled = false;
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0004ACAC File Offset: 0x00048EAC
		private void ShowEditGuidance()
		{
			this.IsViewState = false;
			this.mViewDock.Visibility = Visibility.Collapsed;
			this.mEditDock.Visibility = Visibility.Visible;
			ObservableCollection<GuidanceCategoryEditModel> observableCollection = (this.mIsGamePadTabSelected ? this.mGuidanceData.GamepadEditGuidance : this.mGuidanceData.KeymapEditGuidance);
			if (observableCollection != null)
			{
				if (observableCollection.Count == 1)
				{
					this.mGuidanceListBox.DataContext = observableCollection[0].GuidanceEditModels;
					this.mGuidanceListBox.AlternationCount = 2;
					return;
				}
				this.mGuidanceListBox.DataContext = observableCollection;
				this.mGuidanceListBox.AlternationCount = 0;
			}
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x0004AD44 File Offset: 0x00048F44
		private void RevertBtn_Click(object sender, RoutedEventArgs e)
		{
			if (this.ParentWindow.SelectedConfig.SelectedControlScheme.BuiltIn)
			{
				return;
			}
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_TO_DEFAULT", "");
			customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESET_SCHEME_CHANGES", "");
			customMessageWindow.AddButton(ButtonColors.Red, "STRING_RESET", delegate(object o, EventArgs e)
			{
				Logger.Debug(string.Format("ExtraLog: Revert Clicked, VmName:{0}, Scheme:{1}, SchemeCount:{2}", this.ParentWindow.mVmName, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, this.ParentWindow.SelectedConfig.ControlSchemes.Count));
				string schemeName = this.ParentWindow.SelectedConfig.SelectedControlScheme.Name;
				bool isBookMarked = this.ParentWindow.SelectedConfig.SelectedControlScheme.IsBookMarked;
				this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.SelectedControlScheme);
				IMControlScheme imcontrolScheme = this.ParentWindow.SelectedConfig.ControlSchemes.Where((IMControlScheme scheme) => string.Equals(scheme.Name, schemeName, StringComparison.InvariantCulture) && scheme.BuiltIn).FirstOrDefault<IMControlScheme>();
				if (imcontrolScheme != null)
				{
					Logger.Debug(string.Format("ExtraLog: Updating scheme dictionary, VmName:{0}, Scheme:{1}, SchemeCount:{2}", this.ParentWindow.mVmName, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, this.ParentWindow.SelectedConfig.ControlSchemes.Count));
					imcontrolScheme.Selected = true;
					imcontrolScheme.IsBookMarked = isBookMarked;
					this.ParentWindow.SelectedConfig.SelectedControlScheme = imcontrolScheme;
					this.ParentWindow.SelectedConfig.ControlSchemesDict[imcontrolScheme.Name] = imcontrolScheme;
					GuidanceWindow.sIsDirty = true;
					this.SaveGuidanceChanges();
					ClientStats.SendKeyMappingUIStatsAsync("guide_reset", KMManager.sPackageName, "");
				}
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", delegate(object o, EventArgs e)
			{
			}, null, false, null);
			this.ParentWindow.ShowDimOverlay(null);
			customMessageWindow.Owner = this.ParentWindow.mDimOverlay;
			customMessageWindow.ShowDialog();
			this.ParentWindow.HideDimOverlay();
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x0004AE20 File Offset: 0x00049020
		public void Highlight()
		{
			SolidColorBrush solidColorBrush = BlueStacksUIBinding.Instance.ColorModel["BlueMouseDownBorderBackground"] as SolidColorBrush;
			if (solidColorBrush != null)
			{
				Border border = new Border
				{
					BorderThickness = new Thickness(base.ActualWidth / 2.0, base.ActualHeight / 2.0, base.ActualWidth / 2.0, base.ActualHeight / 2.0),
					BorderBrush = new RadialGradientBrush(new GradientStopCollection
					{
						new GradientStop
						{
							Color = Colors.Transparent,
							Offset = 0.0
						},
						new GradientStop
						{
							Offset = 1.0,
							Color = Color.FromArgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B)
						}
					})
					{
						RadiusX = 1.0,
						RadiusY = 1.0,
						Opacity = 0.5
					}
				};
				this.mGuidanceMainGrid.Children.Add(border);
				Action <>9__1;
				new Thread(delegate
				{
					Thread.Sleep(500);
					Dispatcher dispatcher = this.Dispatcher;
					Action action;
					if ((action = <>9__1) == null)
					{
						action = (<>9__1 = delegate
						{
							this.mGuidanceMainGrid.Children.Remove(border);
						});
					}
					dispatcher.BeginInvoke(action, new object[0]);
				}).Start();
			}
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x0000A29E File Offset: 0x0000849E
		private void GuidanceWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (base.Visibility == Visibility.Visible)
			{
				this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide_active");
				return;
			}
			this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x0004AF9C File Offset: 0x0004919C
		private void QuickLearnBorder_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("guidance-howtoplay", "quicklearn", this.ParentWindow.mVmName, KMManager.sPackageName, "", "");
			PostBootCloudInfo mPostBootCloudInfo = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
			bool? flag;
			if (mPostBootCloudInfo == null)
			{
				flag = null;
			}
			else
			{
				AppPackageListObject onBoardingAppPackages = mPostBootCloudInfo.OnBoardingInfo.OnBoardingAppPackages;
				flag = ((onBoardingAppPackages != null) ? new bool?(onBoardingAppPackages.IsPackageAvailable(KMManager.sPackageName)) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				this.ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl = new GameOnboardingControl(this.ParentWindow, KMManager.sPackageName, "guidancewindow");
				GuidanceWindow sGuidanceWindow = KMManager.sGuidanceWindow;
				if (sGuidanceWindow != null)
				{
					sGuidanceWindow.DimOverLayVisibility(Visibility.Visible);
				}
				this.ParentWindow.ShowDimOverlay(this.ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl);
			}
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x0000A2DD File Offset: 0x000084DD
		private void HowToPlay_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.mHowToPlayCollapseExpand.ImageName = (string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase) ? "outline_settings_expand" : "outline_settings_collapse");
			this.GuidanceVisualInfoVisibility();
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x0004B078 File Offset: 0x00049278
		private void GuidanceVisualInfoVisibility()
		{
			if (string.Equals(KMManager.sPackageName, "com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase))
			{
				this.mHowToPlayGrid.Visibility = Visibility.Visible;
				UIElement uielement = this.mQuickLearnBorder;
				PostBootCloudInfo mPostBootCloudInfo = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
				bool? flag;
				if (mPostBootCloudInfo == null)
				{
					flag = null;
				}
				else
				{
					AppPackageListObject onBoardingAppPackages = mPostBootCloudInfo.OnBoardingInfo.OnBoardingAppPackages;
					flag = ((onBoardingAppPackages != null) ? new bool?(onBoardingAppPackages.IsPackageAvailable(KMManager.sPackageName)) : null);
				}
				bool? flag2 = flag;
				uielement.Visibility = ((flag2.GetValueOrDefault() && string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase)) ? Visibility.Visible : Visibility.Collapsed);
				this.mVideoTutorialBorder.Visibility = ((this.IsVideoTutorialAvailable && string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase)) ? Visibility.Visible : Visibility.Collapsed);
				this.mReadArticleBorder.Visibility = ((!string.IsNullOrEmpty(this.mHelpArticleUrl) && string.Equals(this.mHowToPlayCollapseExpand.ImageName, "outline_settings_collapse", StringComparison.InvariantCultureIgnoreCase)) ? Visibility.Visible : Visibility.Collapsed);
				return;
			}
			this.mVideoBorder.Visibility = (this.IsVideoTutorialAvailable ? Visibility.Visible : Visibility.Collapsed);
			this.mReadArticlePanel.Visibility = ((!string.IsNullOrEmpty(this.mHelpArticleUrl)) ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x0004B1B0 File Offset: 0x000493B0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/guidancewindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x0004B1E0 File Offset: 0x000493E0
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
				((GuidanceWindow)target).StateChanged += this.CustomWindow_StateChanged;
				((GuidanceWindow)target).Closing += this.GuidanceWindow_Closing;
				((GuidanceWindow)target).Closed += this.GuidanceWindow_Closed;
				((GuidanceWindow)target).Loaded += this.GuidanceWindow_Loaded;
				((GuidanceWindow)target).KeyDown += this.GuidanceWindow_KeyDown;
				((GuidanceWindow)target).IsVisibleChanged += this.GuidanceWindow_IsVisibleChanged;
				((GuidanceWindow)target).Activated += this.GuidanceWindow_Activated;
				((GuidanceWindow)target).Deactivated += this.GuidanceWindow_Deactivated;
				return;
			case 2:
				this.mGuidanceMainGrid = (Grid)target;
				return;
			case 3:
				this.mGameControlBorder = (Border)target;
				return;
			case 4:
				this.mHeaderGrid = (DockPanel)target;
				this.mHeaderGrid.MouseLeftButtonDown += this.Grid_MouseLeftButtonDown;
				return;
			case 5:
				this.mControlsTab = (Grid)target;
				this.mControlsTab.MouseLeftButtonUp += this.ControlsTabMouseLeftButtonUp;
				return;
			case 6:
				this.mControlsTabTextBlock = (TextBlock)target;
				return;
			case 7:
				this.mEditKeysGrid = (Grid)target;
				return;
			case 8:
				this.mEditKeysGridTextBlock = (TextBlock)target;
				return;
			case 9:
				this.mCloseSideBarWindow = (CustomPictureBox)target;
				this.mCloseSideBarWindow.MouseDown += this.CustomPictureBox_MouseDown;
				this.mCloseSideBarWindow.MouseLeftButtonUp += this.CloseButton_MouseLeftButtonUp;
				return;
			case 10:
				this.mControlsGrid = (Grid)target;
				return;
			case 11:
				this.mSchemePanel = (StackPanel)target;
				return;
			case 12:
				this.mSchemeTextBlock = (TextBlock)target;
				return;
			case 13:
				this.mSchemesComboBox = (CustomComboBox)target;
				this.mSchemesComboBox.SelectionChanged += this.ProfileComboBox_ProfileChanged;
				return;
			case 14:
				this.mVideoBorder = (Border)target;
				return;
			case 15:
				((Grid)target).MouseUp += this.CustomPictureBox_MouseUp;
				return;
			case 16:
				this.mVideoThumbnail = (CustomPictureBox)target;
				return;
			case 17:
				this.mHowToPlayGrid = (Border)target;
				return;
			case 18:
				((Grid)target).MouseUp += this.HowToPlay_MouseUp;
				return;
			case 19:
				this.mHowToPlayCollapseExpand = (CustomPictureBox)target;
				return;
			case 20:
				this.mQuickLearnBorder = (Border)target;
				this.mQuickLearnBorder.MouseUp += this.QuickLearnBorder_MouseUp;
				return;
			case 21:
				this.mVideoTutorialBorder = (Border)target;
				this.mVideoTutorialBorder.MouseUp += this.CustomPictureBox_MouseUp;
				return;
			case 22:
				this.mReadArticleBorder = (Border)target;
				this.mReadArticleBorder.MouseUp += this.ReadMoreLinkMouseLeftButtonUp;
				return;
			case 23:
				this.mKeysIconGrid = (DockPanel)target;
				return;
			case 24:
				this.mKeyboardIcon = (Grid)target;
				return;
			case 25:
				this.mKeyboardIconImage = (CustomPictureBox)target;
				this.mKeyboardIconImage.PreviewMouseLeftButtonUp += this.KeyboardIconPreviewMouseLeftButtonUp;
				return;
			case 26:
				this.mKeyboardIconSeparator = (Grid)target;
				return;
			case 27:
				this.mGamepadIcon = (Grid)target;
				return;
			case 28:
				this.mGamepadIconImage = (CustomPictureBox)target;
				this.mGamepadIconImage.PreviewMouseLeftButtonUp += this.GamepadIconPreviewMouseLeftButtonUp;
				return;
			case 29:
				this.mGamepadIconSeparator = (Grid)target;
				return;
			case 30:
				this.mReadArticlePanel = (StackPanel)target;
				return;
			case 31:
				((TextBlock)target).MouseLeftButtonDown += this.ReadMoreLinkMouseLeftButtonUp;
				return;
			case 32:
				((CustomPictureBox)target).MouseLeftButtonDown += this.ReadMoreLinkMouseLeftButtonUp;
				return;
			case 33:
				this.separator = (Grid)target;
				return;
			case 34:
				this.mGuidanceKeyBorder = (Border)target;
				return;
			case 35:
				this.mGuidanceKeysGrid = (Grid)target;
				return;
			case 36:
				this.mGuidanceListBox = (ListBox)target;
				return;
			case 37:
				this.noGameGuidePanel = (StackPanel)target;
				return;
			case 38:
				this.mViewDock = (DockPanel)target;
				return;
			case 39:
				this.mEditBtn = (CustomButton)target;
				this.mEditBtn.Click += this.EditBtn_Click;
				return;
			case 40:
				this.mRevertBtn = (CustomButton)target;
				this.mRevertBtn.Click += this.RevertBtn_Click;
				return;
			case 41:
				this.mEditDock = (Grid)target;
				return;
			case 42:
				this.mDiscardBtn = (CustomButton)target;
				this.mDiscardBtn.Click += this.DiscardBtn_Click;
				return;
			case 43:
				this.mSaveBtn = (CustomButton)target;
				this.mSaveBtn.Click += this.SaveBtn_Click;
				return;
			case 44:
				this.mOverlayGrid = (Grid)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040007F4 RID: 2036
		internal MainWindow ParentWindow;

		// Token: 0x040007F5 RID: 2037
		private bool mIsGuidanceVideoPresent;

		// Token: 0x040007F6 RID: 2038
		private CustomToastPopupControl mToastPopup;

		// Token: 0x040007F7 RID: 2039
		private List<string> lstPanTags = new List<string>();

		// Token: 0x040007F8 RID: 2040
		private List<string> lstMOBATags = new List<string>();

		// Token: 0x040007F9 RID: 2041
		internal bool mIsGamePadTabSelected;

		// Token: 0x040007FA RID: 2042
		internal bool mIsOnboardingPopupToBeShownOnGuidanceClose;

		// Token: 0x040007FB RID: 2043
		private int mSidebarWidth = 220;

		// Token: 0x040007FC RID: 2044
		internal double mGuidanceWindowLeft = -1.0;

		// Token: 0x040007FD RID: 2045
		internal double mGuidanceWindowTop = -1.0;

		// Token: 0x040007FE RID: 2046
		internal bool mGuidanceHasBeenMoved;

		// Token: 0x040007FF RID: 2047
		internal static bool IsDirty;

		// Token: 0x04000800 RID: 2048
		private GuidanceData mGuidanceData = new GuidanceData();

		// Token: 0x04000801 RID: 2049
		private string mHelpArticleUrl;

		// Token: 0x04000803 RID: 2051
		private bool isViewState = true;

		// Token: 0x04000804 RID: 2052
		private readonly DataModificationTracker DataModificationTracker = new DataModificationTracker();

		// Token: 0x04000805 RID: 2053
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGuidanceMainGrid;

		// Token: 0x04000806 RID: 2054
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mGameControlBorder;

		// Token: 0x04000807 RID: 2055
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal DockPanel mHeaderGrid;

		// Token: 0x04000808 RID: 2056
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mControlsTab;

		// Token: 0x04000809 RID: 2057
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mControlsTabTextBlock;

		// Token: 0x0400080A RID: 2058
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mEditKeysGrid;

		// Token: 0x0400080B RID: 2059
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mEditKeysGridTextBlock;

		// Token: 0x0400080C RID: 2060
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCloseSideBarWindow;

		// Token: 0x0400080D RID: 2061
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mControlsGrid;

		// Token: 0x0400080E RID: 2062
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mSchemePanel;

		// Token: 0x0400080F RID: 2063
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSchemeTextBlock;

		// Token: 0x04000810 RID: 2064
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomComboBox mSchemesComboBox;

		// Token: 0x04000811 RID: 2065
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mVideoBorder;

		// Token: 0x04000812 RID: 2066
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mVideoThumbnail;

		// Token: 0x04000813 RID: 2067
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mHowToPlayGrid;

		// Token: 0x04000814 RID: 2068
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mHowToPlayCollapseExpand;

		// Token: 0x04000815 RID: 2069
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mQuickLearnBorder;

		// Token: 0x04000816 RID: 2070
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mVideoTutorialBorder;

		// Token: 0x04000817 RID: 2071
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mReadArticleBorder;

		// Token: 0x04000818 RID: 2072
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal DockPanel mKeysIconGrid;

		// Token: 0x04000819 RID: 2073
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mKeyboardIcon;

		// Token: 0x0400081A RID: 2074
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mKeyboardIconImage;

		// Token: 0x0400081B RID: 2075
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mKeyboardIconSeparator;

		// Token: 0x0400081C RID: 2076
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGamepadIcon;

		// Token: 0x0400081D RID: 2077
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mGamepadIconImage;

		// Token: 0x0400081E RID: 2078
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGamepadIconSeparator;

		// Token: 0x0400081F RID: 2079
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mReadArticlePanel;

		// Token: 0x04000820 RID: 2080
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid separator;

		// Token: 0x04000821 RID: 2081
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mGuidanceKeyBorder;

		// Token: 0x04000822 RID: 2082
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGuidanceKeysGrid;

		// Token: 0x04000823 RID: 2083
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ListBox mGuidanceListBox;

		// Token: 0x04000824 RID: 2084
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel noGameGuidePanel;

		// Token: 0x04000825 RID: 2085
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal DockPanel mViewDock;

		// Token: 0x04000826 RID: 2086
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mEditBtn;

		// Token: 0x04000827 RID: 2087
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mRevertBtn;

		// Token: 0x04000828 RID: 2088
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mEditDock;

		// Token: 0x04000829 RID: 2089
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mDiscardBtn;

		// Token: 0x0400082A RID: 2090
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mSaveBtn;

		// Token: 0x0400082B RID: 2091
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOverlayGrid;

		// Token: 0x0400082C RID: 2092
		private bool _contentLoaded;
	}
}
