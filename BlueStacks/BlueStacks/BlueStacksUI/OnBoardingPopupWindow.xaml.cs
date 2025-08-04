using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000072 RID: 114
	public partial class OnBoardingPopupWindow : CustomWindow, INotifyPropertyChanged
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000590 RID: 1424 RVA: 0x00020E0C File Offset: 0x0001F00C
		// (remove) Token: 0x06000591 RID: 1425 RVA: 0x00020E44 File Offset: 0x0001F044
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000592 RID: 1426 RVA: 0x00005B8B File Offset: 0x00003D8B
		protected void OnPropertyChanged(string property)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(property));
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000593 RID: 1427 RVA: 0x00020E7C File Offset: 0x0001F07C
		// (remove) Token: 0x06000594 RID: 1428 RVA: 0x00020EB4 File Offset: 0x0001F0B4
		public event Action Startevent;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000595 RID: 1429 RVA: 0x00020EEC File Offset: 0x0001F0EC
		// (remove) Token: 0x06000596 RID: 1430 RVA: 0x00020F24 File Offset: 0x0001F124
		public event Action Endevent;

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000597 RID: 1431 RVA: 0x00005BA4 File Offset: 0x00003DA4
		// (set) Token: 0x06000598 RID: 1432 RVA: 0x00005BAC File Offset: 0x00003DAC
		public string HeaderContent
		{
			get
			{
				return this.mHeaderContent;
			}
			set
			{
				if (this.mHeaderContent != value)
				{
					this.mHeaderContent = value;
					this.OnPropertyChanged("HeaderContent");
				}
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x00005BCE File Offset: 0x00003DCE
		// (set) Token: 0x0600059A RID: 1434 RVA: 0x00005BD6 File Offset: 0x00003DD6
		public string BodyContent
		{
			get
			{
				return this.mBodyContent;
			}
			set
			{
				if (this.mBodyContent != value)
				{
					this.mBodyContent = value;
					this.OnPropertyChanged("BodyContent");
				}
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x00005BF8 File Offset: 0x00003DF8
		// (set) Token: 0x0600059C RID: 1436 RVA: 0x00005C00 File Offset: 0x00003E00
		public bool IsLastPopup
		{
			get
			{
				return this.mIsLastPopup;
			}
			set
			{
				if (this.mIsLastPopup != value)
				{
					this.mIsLastPopup = value;
					this.OnPropertyChanged("IsLastPopup");
				}
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x00005C1D File Offset: 0x00003E1D
		// (set) Token: 0x0600059E RID: 1438 RVA: 0x00005C25 File Offset: 0x00003E25
		public PopupArrowAlignment PopArrowAlignment
		{
			get
			{
				return this.mPopArrowAlignment;
			}
			set
			{
				if (this.mPopArrowAlignment != value)
				{
					this.mPopArrowAlignment = value;
					this.OnPropertyChanged("PopArrowAlignment");
				}
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x00005C42 File Offset: 0x00003E42
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x00005C4A File Offset: 0x00003E4A
		public UIElement PlacementTarget { get; set; }

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x00005C53 File Offset: 0x00003E53
		// (set) Token: 0x060005A2 RID: 1442 RVA: 0x00005C5B File Offset: 0x00003E5B
		public int LeftMargin { get; set; }

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00005C64 File Offset: 0x00003E64
		// (set) Token: 0x060005A4 RID: 1444 RVA: 0x00005C6C File Offset: 0x00003E6C
		public int TopMargin { get; set; }

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00005C75 File Offset: 0x00003E75
		// (set) Token: 0x060005A6 RID: 1446 RVA: 0x00005C7D File Offset: 0x00003E7D
		public bool IsBlurbRelatedToGuidance { get; set; }

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x00005C86 File Offset: 0x00003E86
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x00005C8E File Offset: 0x00003E8E
		public string PackageName { get; set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x00005C97 File Offset: 0x00003E97
		// (set) Token: 0x060005AA RID: 1450 RVA: 0x00005C9F File Offset: 0x00003E9F
		public MainWindow ParentWindow { get; set; }

		// Token: 0x060005AB RID: 1451 RVA: 0x00020F5C File Offset: 0x0001F15C
		public OnBoardingPopupWindow(MainWindow mainWindow, string packageName)
		{
			this.PackageName = packageName;
			this.ParentWindow = mainWindow;
			this.InitializeComponent();
			if (this.ParentWindow != null)
			{
				this.ParentWindow.SizeChanged -= new SizeChangedEventHandler(this.Owner_SizeChanged);
				this.ParentWindow.LocationChanged -= this.Owner_SizeChanged;
				this.ParentWindow.StateChanged -= this.Owner_SizeChanged;
				this.ParentWindow.SizeChanged += new SizeChangedEventHandler(this.Owner_SizeChanged);
				this.ParentWindow.LocationChanged += this.Owner_SizeChanged;
				this.ParentWindow.StateChanged += this.Owner_SizeChanged;
			}
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x00005CA8 File Offset: 0x00003EA8
		private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Action startevent = this.Startevent;
			if (startevent == null)
			{
				return;
			}
			startevent();
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00021020 File Offset: 0x0001F220
		private void Owner_SizeChanged(object sender, EventArgs e)
		{
			if (this.PlacementTarget != null && this.PlacementTarget.IsVisible)
			{
				base.SetValue(Window.LeftProperty, this.PlacementTarget.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double)this.LeftMargin);
				base.SetValue(Window.TopProperty, this.PlacementTarget.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double)this.TopMargin);
			}
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00005CBA File Offset: 0x00003EBA
		public void CloseWindow()
		{
			Action endevent = this.Endevent;
			if (endevent != null)
			{
				endevent();
			}
			base.Close();
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x00005CD3 File Offset: 0x00003ED3
		private void OnBoardingPopupNext_Click(object sender, RoutedEventArgs e)
		{
			Stats.SendCommonClientStatsAsync("general-onboarding", "okay_clicked", this.ParentWindow.mVmName, this.PackageName, base.Title, "");
			this.CloseWindow();
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x000210D8 File Offset: 0x0001F2D8
		private void CustomWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.System && e.SystemKey == Key.F4)
			{
				e.Handled = true;
				return;
			}
			string keyPressed = string.Empty;
			if (e.Key == Key.System)
			{
				keyPressed = MainWindow.GetShortcutKey(e.SystemKey);
			}
			else
			{
				keyPressed = MainWindow.GetShortcutKey(e.Key);
			}
			MainWindow mainWindow = (MainWindow)base.Owner;
			if (string.Equals(keyPressed, mainWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false), StringComparison.InvariantCulture) && base.Title == "FullScreenBlurb")
			{
				ShortcutKeys shortcutKeys = mainWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut.Where((ShortcutKeys e) => e.ShortcutKey.Equals(keyPressed, StringComparison.InvariantCulture)).First<ShortcutKeys>();
				ClientHotKeys clientHotKeys = (ClientHotKeys)Enum.Parse(typeof(ClientHotKeys), shortcutKeys.ShortcutName);
				mainWindow.HandleClientHotKey(clientHotKeys);
				this.CloseWindow();
			}
		}

		// Token: 0x040002E9 RID: 745
		private string mHeaderContent;

		// Token: 0x040002EA RID: 746
		private string mBodyContent;

		// Token: 0x040002EB RID: 747
		private bool mIsLastPopup;

		// Token: 0x040002EC RID: 748
		private PopupArrowAlignment mPopArrowAlignment = PopupArrowAlignment.Top;
	}
}
