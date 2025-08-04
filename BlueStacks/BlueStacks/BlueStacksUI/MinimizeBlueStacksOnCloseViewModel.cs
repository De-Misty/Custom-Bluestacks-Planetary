using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BlueStacks.Common;
using GalaSoft.MvvmLight.Command;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200006D RID: 109
	public class MinimizeBlueStacksOnCloseViewModel : INotifyPropertyChanged
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000552 RID: 1362 RVA: 0x00020628 File Offset: 0x0001E828
		// (remove) Token: 0x06000553 RID: 1363 RVA: 0x00020660 File Offset: 0x0001E860
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x00005970 File Offset: 0x00003B70
		// (set) Token: 0x06000555 RID: 1365 RVA: 0x00005978 File Offset: 0x00003B78
		public MainWindow ParentWindow { get; set; }

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x00005981 File Offset: 0x00003B81
		// (set) Token: 0x06000557 RID: 1367 RVA: 0x00005989 File Offset: 0x00003B89
		public bool IsDoNotShowAgainChkBoxChecked
		{
			get
			{
				return this.mIsDoNotShowAgainChkBoxChecked;
			}
			set
			{
				this.mIsDoNotShowAgainChkBoxChecked = value;
				this.NotifyPropertyChanged("IsDoNotShowAgainChkBoxChecked");
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x0000599D File Offset: 0x00003B9D
		// (set) Token: 0x06000559 RID: 1369 RVA: 0x000059A5 File Offset: 0x00003BA5
		public bool IsQuitBluestacksChecked
		{
			get
			{
				return this.mIsQuitBluestacksChecked;
			}
			set
			{
				this.mIsQuitBluestacksChecked = value;
				this.NotifyPropertyChanged("IsQuitBluestacksChecked");
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x000059B9 File Offset: 0x00003BB9
		// (set) Token: 0x0600055B RID: 1371 RVA: 0x000059C1 File Offset: 0x00003BC1
		public bool IsMinimizeBlueStacksRadioBtnChecked
		{
			get
			{
				return this.mIsMinimizeBlueStacksRadioBtnChecked;
			}
			set
			{
				this.mIsMinimizeBlueStacksRadioBtnChecked = value;
				this.NotifyPropertyChanged("IsMinimizeBlueStacksRadioBtnChecked");
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x000059D5 File Offset: 0x00003BD5
		public static Dictionary<string, string> LocaleModel
		{
			get
			{
				return BlueStacksUIBinding.Instance.LocaleModel;
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x000059E1 File Offset: 0x00003BE1
		public static Dictionary<string, Brush> ColorModel
		{
			get
			{
				return BlueStacksUIBinding.Instance.ColorModel;
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x000059ED File Offset: 0x00003BED
		// (set) Token: 0x0600055F RID: 1375 RVA: 0x000059F5 File Offset: 0x00003BF5
		public ICommand CloseControlCommand { get; set; }

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x000059FE File Offset: 0x00003BFE
		// (set) Token: 0x06000561 RID: 1377 RVA: 0x00005A06 File Offset: 0x00003C06
		public ICommand MinimizeCommand { get; set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x00005A0F File Offset: 0x00003C0F
		// (set) Token: 0x06000563 RID: 1379 RVA: 0x00005A17 File Offset: 0x00003C17
		public ICommand QuitCommand { get; set; }

		// Token: 0x06000564 RID: 1380 RVA: 0x00005A20 File Offset: 0x00003C20
		private void NotifyPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x00005A3C File Offset: 0x00003C3C
		public MinimizeBlueStacksOnCloseViewModel(MainWindow window)
		{
			this.ParentWindow = window;
			this.Init();
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00020698 File Offset: 0x0001E898
		private void Init()
		{
			this.CloseControlCommand = new RelayCommand<UserControl>(new Action<UserControl>(this.Close), false);
			this.MinimizeCommand = new RelayCommand<UserControl>(new Action<UserControl>(this.MinimizeBluestacksHandler), false);
			this.QuitCommand = new RelayCommand(new Action(this.QuitBlueStacks), false);
			if (this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup)
			{
				this.IsMinimizeBlueStacksRadioBtnChecked = true;
				return;
			}
			this.IsQuitBluestacksChecked = true;
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x00005A58 File Offset: 0x00003C58
		private void DoNotShowAgainPromptHandler()
		{
			if (this.IsDoNotShowAgainChkBoxChecked)
			{
				this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = false;
			}
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x00020710 File Offset: 0x0001E910
		private void QuitBlueStacks()
		{
			Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "BlueStacks_exit_popup", this.ParentWindow.mVmName, "", "", "");
			this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = false;
			this.DoNotShowAgainPromptHandler();
			this.ParentWindow.CloseWindowHandler(false);
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x0002076C File Offset: 0x0001E96C
		private void MinimizeBluestacksHandler(UserControl control)
		{
			Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "BlueStacks_minimize_popup", this.ParentWindow.mVmName, "", "", "");
			this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
			this.DoNotShowAgainPromptHandler();
			this.Close(control);
			this.ParentWindow.MinimizeWindowHandler();
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x000207CC File Offset: 0x0001E9CC
		private void Close(UserControl control)
		{
			try
			{
				BlueStacksUIUtils.CloseContainerWindow(control);
				this.ParentWindow.HideDimOverlay();
				control.Visibility = Visibility.Hidden;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to close CloseBluestacksControl from dimoverlay " + ex.ToString());
			}
		}

		// Token: 0x040002C0 RID: 704
		private bool mIsDoNotShowAgainChkBoxChecked;

		// Token: 0x040002C1 RID: 705
		private bool mIsQuitBluestacksChecked;

		// Token: 0x040002C2 RID: 706
		private bool mIsMinimizeBlueStacksRadioBtnChecked = true;
	}
}
