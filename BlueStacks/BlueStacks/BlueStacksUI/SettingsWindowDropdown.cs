using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000D3 RID: 211
	public class SettingsWindowDropdown : UserControl, IComponentConnector
	{
		// Token: 0x060008A0 RID: 2208 RVA: 0x00007898 File Offset: 0x00005A98
		public SettingsWindowDropdown()
		{
			this.InitializeComponent();
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x000078A6 File Offset: 0x00005AA6
		internal void Init(MainWindow window)
		{
			this.ParentWindow = window;
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x000309B0 File Offset: 0x0002EBB0
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Grid, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
			foreach (object obj in (sender as Grid).Children)
			{
				UIElement uielement = (UIElement)obj;
				if (uielement is CustomPictureBox)
				{
					(uielement as CustomPictureBox).ImageName = (uielement as CustomPictureBox).ImageName + "_hover";
				}
				if (uielement is TextBlock)
				{
					BlueStacksUIBinding.BindColor(uielement as TextBlock, Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
				}
			}
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00030A64 File Offset: 0x0002EC64
		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			(sender as Grid).Background = Brushes.Transparent;
			foreach (object obj in (sender as Grid).Children)
			{
				UIElement uielement = (UIElement)obj;
				if (uielement is CustomPictureBox)
				{
					(uielement as CustomPictureBox).ImageName = (uielement as CustomPictureBox).ImageName.Replace("_hover", "");
				}
				if (uielement is TextBlock)
				{
					BlueStacksUIBinding.BindColor(uielement as TextBlock, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
				}
			}
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x000078AF File Offset: 0x00005AAF
		private void SettingsButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
			this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x00030B18 File Offset: 0x0002ED18
		private void FullscreenButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
			if (!this.ParentWindow.mResizeHandler.IsMinMaxEnabled)
			{
				return;
			}
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (FeatureManager.Instance.IsCustomUIForDMM || this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
				{
					if (this.ParentWindow.mIsFullScreen)
					{
						this.ParentWindow.RestoreWindows(false);
						return;
					}
					this.ParentWindow.FullScreenWindow();
				}
			}), new object[0]);
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x000078DC File Offset: 0x00005ADC
		private void SortingButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
			CommonHandlers.ArrangeWindow();
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00030B74 File Offset: 0x0002ED74
		private void AccountButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
			if (this.ParentWindow.mGuestBootCompleted)
			{
				this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab("STRING_ACCOUNT", BlueStacksUIUtils.sAndroidSettingsPackageName, BlueStacksUIUtils.sAndroidAccountSettingsActivityName, "account_tab", true, true, false);
			}
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00030BD0 File Offset: 0x0002EDD0
		private void PinOnTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox.ImageName.Contains("_off"))
			{
				customPictureBox.ImageName = "toggle_on";
				this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
				this.ParentWindow.Topmost = true;
				return;
			}
			customPictureBox.ImageName = "toggle_off";
			this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
			this.ParentWindow.Topmost = false;
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x00030C48 File Offset: 0x0002EE48
		internal void LateInit()
		{
			if (BlueStacksUIUtils.DictWindows.Keys.Count == 1)
			{
				this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp -= this.SyncOperationsButton_PreviewMouseLeftButtonUp;
				this.mSyncOperationsButtonGrid.MouseEnter -= this.Grid_MouseEnter;
				this.mSyncOperationsButtonGrid.Opacity = 0.5;
				this.mSortingGrid.PreviewMouseLeftButtonUp -= this.SortingButton_MouseLeftButtonUp;
				this.mSortingGrid.MouseEnter -= this.Grid_MouseEnter;
				this.mSortingGrid.Opacity = 0.5;
			}
			else
			{
				this.mSortingGrid.PreviewMouseLeftButtonUp -= this.SortingButton_MouseLeftButtonUp;
				this.mSortingGrid.PreviewMouseLeftButtonUp += this.SortingButton_MouseLeftButtonUp;
				this.mSortingGrid.MouseEnter -= this.Grid_MouseEnter;
				this.mSortingGrid.MouseEnter += this.Grid_MouseEnter;
				this.mSortingGrid.Opacity = 1.0;
				if ((BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName) && this.ParentWindow.mIsSyncMaster) || (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName) && BlueStacksUIUtils.DictWindows.Keys.Count - BlueStacksUIUtils.sSyncInvolvedInstances.Count > 1))
				{
					this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp -= this.SyncOperationsButton_PreviewMouseLeftButtonUp;
					this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp += this.SyncOperationsButton_PreviewMouseLeftButtonUp;
					this.mSyncOperationsButtonGrid.MouseEnter -= this.Grid_MouseEnter;
					this.mSyncOperationsButtonGrid.MouseEnter += this.Grid_MouseEnter;
					this.mSyncOperationsButtonGrid.Opacity = 1.0;
				}
				else
				{
					this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp -= this.SyncOperationsButton_PreviewMouseLeftButtonUp;
					this.mSyncOperationsButtonGrid.MouseEnter -= this.Grid_MouseEnter;
					this.mSyncOperationsButtonGrid.Opacity = 0.5;
				}
			}
			if (this.ParentWindow.EngineInstanceRegistry.IsClientOnTop)
			{
				this.mPinOnTopToggleButton.ImageName = "toggle_on";
				return;
			}
			this.mPinOnTopToggleButton.ImageName = "toggle_off";
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x000078F9 File Offset: 0x00005AF9
		private void SyncOperationsButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
			this.ParentWindow.ShowSynchronizerWindow();
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x00030EA8 File Offset: 0x0002F0A8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/ncsettingsdropdown.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x00030ED8 File Offset: 0x0002F0D8
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
				this.mPinOnTopButtonGrid = (Grid)target;
				this.mPinOnTopButtonGrid.MouseEnter += this.Grid_MouseEnter;
				this.mPinOnTopButtonGrid.MouseLeave += this.Grid_MouseLeave;
				return;
			case 2:
				this.mPinOnTopButtonImage = (CustomPictureBox)target;
				return;
			case 3:
				this.mPinOnTopButtonText = (TextBlock)target;
				return;
			case 4:
				this.mPinOnTopToggleButton = (CustomPictureBox)target;
				this.mPinOnTopToggleButton.PreviewMouseLeftButtonUp += this.PinOnTop_MouseLeftButtonUp;
				return;
			case 5:
				this.mFullScreenButtonGrid = (Grid)target;
				this.mFullScreenButtonGrid.MouseEnter += this.Grid_MouseEnter;
				this.mFullScreenButtonGrid.MouseLeave += this.Grid_MouseLeave;
				this.mFullScreenButtonGrid.PreviewMouseLeftButtonUp += this.FullscreenButton_MouseLeftButtonUp;
				return;
			case 6:
				this.mFullScreenImage = (CustomPictureBox)target;
				return;
			case 7:
				this.mFullScreenButtonText = (TextBlock)target;
				return;
			case 8:
				this.mSyncOperationsButtonGrid = (Grid)target;
				this.mSyncOperationsButtonGrid.MouseLeave += this.Grid_MouseLeave;
				this.mSyncOperationsButtonGrid.MouseEnter += this.Grid_MouseEnter;
				this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp += this.SyncOperationsButton_PreviewMouseLeftButtonUp;
				return;
			case 9:
				this.mSyncOperationsButtonImage = (CustomPictureBox)target;
				return;
			case 10:
				this.mSyncOperationsButtonText = (TextBlock)target;
				return;
			case 11:
				this.mSortingGrid = (Grid)target;
				this.mSortingGrid.MouseEnter += this.Grid_MouseEnter;
				this.mSortingGrid.MouseLeave += this.Grid_MouseLeave;
				this.mSortingGrid.PreviewMouseLeftButtonUp += this.SortingButton_MouseLeftButtonUp;
				return;
			case 12:
				this.mSortingButtonImage = (CustomPictureBox)target;
				return;
			case 13:
				this.mSortingButtonText = (TextBlock)target;
				return;
			case 14:
				this.mAccountGrid = (Grid)target;
				this.mAccountGrid.MouseEnter += this.Grid_MouseEnter;
				this.mAccountGrid.MouseLeave += this.Grid_MouseLeave;
				this.mAccountGrid.PreviewMouseLeftButtonUp += this.AccountButton_MouseLeftButtonUp;
				return;
			case 15:
				this.mAccountButtonImage = (CustomPictureBox)target;
				return;
			case 16:
				this.mAccountButtonText = (TextBlock)target;
				return;
			case 17:
				this.mSettingsButtonGrid = (Grid)target;
				this.mSettingsButtonGrid.MouseEnter += this.Grid_MouseEnter;
				this.mSettingsButtonGrid.MouseLeave += this.Grid_MouseLeave;
				this.mSettingsButtonGrid.PreviewMouseLeftButtonUp += this.SettingsButton_MouseLeftButtonUp;
				return;
			case 18:
				this.mSettingsButtonImage = (CustomPictureBox)target;
				return;
			case 19:
				this.mSettingsButtonText = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040004E0 RID: 1248
		private MainWindow ParentWindow;

		// Token: 0x040004E1 RID: 1249
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mPinOnTopButtonGrid;

		// Token: 0x040004E2 RID: 1250
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPinOnTopButtonImage;

		// Token: 0x040004E3 RID: 1251
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mPinOnTopButtonText;

		// Token: 0x040004E4 RID: 1252
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPinOnTopToggleButton;

		// Token: 0x040004E5 RID: 1253
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mFullScreenButtonGrid;

		// Token: 0x040004E6 RID: 1254
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mFullScreenImage;

		// Token: 0x040004E7 RID: 1255
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mFullScreenButtonText;

		// Token: 0x040004E8 RID: 1256
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSyncOperationsButtonGrid;

		// Token: 0x040004E9 RID: 1257
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSyncOperationsButtonImage;

		// Token: 0x040004EA RID: 1258
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSyncOperationsButtonText;

		// Token: 0x040004EB RID: 1259
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSortingGrid;

		// Token: 0x040004EC RID: 1260
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSortingButtonImage;

		// Token: 0x040004ED RID: 1261
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSortingButtonText;

		// Token: 0x040004EE RID: 1262
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mAccountGrid;

		// Token: 0x040004EF RID: 1263
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mAccountButtonImage;

		// Token: 0x040004F0 RID: 1264
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mAccountButtonText;

		// Token: 0x040004F1 RID: 1265
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSettingsButtonGrid;

		// Token: 0x040004F2 RID: 1266
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSettingsButtonImage;

		// Token: 0x040004F3 RID: 1267
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSettingsButtonText;

		// Token: 0x040004F4 RID: 1268
		private bool _contentLoaded;
	}
}
