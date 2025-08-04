using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000045 RID: 69
	public class TopbarOptions : UserControl, IComponentConnector
	{
		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0000489A File Offset: 0x00002A9A
		public MainWindow ParentWindow
		{
			get
			{
				if (this.mMainWindow == null)
				{
					this.mMainWindow = Window.GetWindow(this) as MainWindow;
				}
				return this.mMainWindow;
			}
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x000048BB File Offset: 0x00002ABB
		public TopbarOptions()
		{
			this.InitializeComponent();
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x000048D4 File Offset: 0x00002AD4
		private void Topbar_Loaded(object sender, RoutedEventArgs e)
		{
			if (!this.mIsLoadedOnce)
			{
				this.mIsLoadedOnce = true;
				this.BindEvents();
			}
			this.SetLabel();
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0001A854 File Offset: 0x00018A54
		public void SetLabel()
		{
			this.mFullScreenTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_FULL_SCREEN", "") + " (" + this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false) + ")";
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x000048F1 File Offset: 0x00002AF1
		internal void BindEvents()
		{
			this.ParentWindow.FullScreenChanged += this.ParentWindow_FullScreenChangedEvent;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0001A8A0 File Offset: 0x00018AA0
		private void ParentWindow_FullScreenChangedEvent(object sender, MainWindowEventArgs.FullScreenChangedEventArgs args)
		{
			object obj = this.mSyncRoot;
			lock (obj)
			{
				this.mIsInFullscreenMode = args.IsFullscreen;
				if (!this.mIsInFullscreenMode)
				{
					this.ParentWindow.mFullscreenTopbarPopupButton.IsOpen = false;
					this.ParentWindow.mFullscreenTopbarPopup.IsOpen = false;
				}
				else
				{
					ColumnDefinition gameGuideColumn = this.GameGuideColumn;
					GridLength gridLength;
					if (this.ParentWindow.SelectedConfig != null && this.ParentWindow.SelectedConfig.SelectedControlScheme != null && this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls != null)
					{
						if (this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any((IMAction action) => action.Guidance.Any<KeyValuePair<string, string>>()))
						{
							gridLength = new GridLength(1.0, GridUnitType.Star);
							goto IL_00DF;
						}
					}
					gridLength = new GridLength(0.0, GridUnitType.Star);
					IL_00DF:
					gameGuideColumn.Width = gridLength;
				}
			}
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0000490A File Offset: 0x00002B0A
		internal void HideTopBarInFullscreen()
		{
			this.ParentWindow.mFullscreenTopbarPopupButton.IsOpen = false;
			this.ParentWindow.mFullscreenTopbarPopup.IsOpen = false;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00004786 File Offset: 0x00002986
		internal void ToggleTopbarButtonVisibilityInFullscreen(bool isVisible)
		{
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00004786 File Offset: 0x00002986
		internal void ToggleTopbarVisibilityInFullscreen(bool isVisible)
		{
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0001A9AC File Offset: 0x00018BAC
		private void Label_MouseEnter(object sender, MouseEventArgs e)
		{
			Label label = sender as Label;
			if (label != null)
			{
				SolidColorBrush solidColorBrush = base.TryFindResource("LabelMouseHoverBackground") as SolidColorBrush;
				if (solidColorBrush != null)
				{
					label.Background = solidColorBrush;
				}
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0001A9E0 File Offset: 0x00018BE0
		private void Label_MouseLeave(object sender, MouseEventArgs e)
		{
			Label label = sender as Label;
			if (label != null)
			{
				SolidColorBrush solidColorBrush = base.TryFindResource("LabelBackground") as SolidColorBrush;
				if (solidColorBrush != null)
				{
					label.Background = solidColorBrush;
				}
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0000492E File Offset: 0x00002B2E
		private void FullScreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.ParentWindow.mStreamingModeEnabled)
			{
				this.ParentWindow.mCommonHandler.FullScreenButtonHandler("topbar", "MouseClick");
			}
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0001AA14 File Offset: 0x00018C14
		private void GameGuide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad"))
			{
				KMManager.HandleInputMapperWindow(this.ParentWindow, "gamepad");
			}
			string text = "topbar";
			string userGuid = RegistryManager.Instance.UserGuid;
			string text2 = "gameGuide";
			string text3 = "MouseClick";
			string clientVersion = RegistryManager.Instance.ClientVersion;
			string version = RegistryManager.Instance.Version;
			string oem = RegistryManager.Instance.Oem;
			AppTabButton selectedTab = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
			ClientStats.SendMiscellaneousStatsAsync(text, userGuid, text2, text3, clientVersion, version, oem, (selectedTab != null) ? selectedTab.PackageName : null, null);
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001AAA8 File Offset: 0x00018CA8
		private void Setting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			string text = string.Empty;
			if (this.ParentWindow.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName))
			{
				text = "STRING_GAME_SETTINGS";
			}
			ClientStats.SendMiscellaneousStatsAsync("topbar", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
			this.ParentWindow.mCommonHandler.LaunchSettingsWindow(text);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0001AB48 File Offset: 0x00018D48
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/topbaroptions.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0001AB78 File Offset: 0x00018D78
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
				((TopbarOptions)target).Loaded += this.Topbar_Loaded;
				return;
			case 2:
				this.TopMenu = (Grid)target;
				return;
			case 3:
				this.GameGuideColumn = (ColumnDefinition)target;
				return;
			case 4:
				((Label)target).MouseEnter += this.Label_MouseEnter;
				((Label)target).MouseLeave += this.Label_MouseLeave;
				((Label)target).MouseLeftButtonDown += this.FullScreen_MouseLeftButtonDown;
				return;
			case 5:
				this.mFullScreenTextBlock = (TextBlock)target;
				return;
			case 6:
				((Label)target).MouseEnter += this.Label_MouseEnter;
				((Label)target).MouseLeave += this.Label_MouseLeave;
				((Label)target).MouseLeftButtonDown += this.GameGuide_MouseLeftButtonDown;
				return;
			case 7:
				((Label)target).MouseEnter += this.Label_MouseEnter;
				((Label)target).MouseLeave += this.Label_MouseLeave;
				((Label)target).MouseLeftButtonDown += this.Setting_MouseLeftButtonDown;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400020F RID: 527
		private bool mIsLoadedOnce;

		// Token: 0x04000210 RID: 528
		private bool mIsInFullscreenMode;

		// Token: 0x04000211 RID: 529
		private readonly object mSyncRoot = new object();

		// Token: 0x04000212 RID: 530
		private MainWindow mMainWindow;

		// Token: 0x04000213 RID: 531
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid TopMenu;

		// Token: 0x04000214 RID: 532
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition GameGuideColumn;

		// Token: 0x04000215 RID: 533
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mFullScreenTextBlock;

		// Token: 0x04000216 RID: 534
		private bool _contentLoaded;
	}
}
