using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000272 RID: 626
	public partial class HomeAppTabButton : Button
	{
		// Token: 0x1700034F RID: 847
		// (get) Token: 0x060016EC RID: 5868 RVA: 0x0000F6B8 File Offset: 0x0000D8B8
		// (set) Token: 0x060016ED RID: 5869 RVA: 0x0000F6C0 File Offset: 0x0000D8C0
		public string Key { get; set; } = string.Empty;

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x060016EE RID: 5870 RVA: 0x0000F6C9 File Offset: 0x0000D8C9
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

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060016EF RID: 5871 RVA: 0x0000F6EA File Offset: 0x0000D8EA
		// (set) Token: 0x060016F0 RID: 5872 RVA: 0x0000F6F7 File Offset: 0x0000D8F7
		public string Text
		{
			get
			{
				return this.mTabHeader.Text;
			}
			set
			{
				this.Key = value + "-Normal";
				this.ImageBox.ImageName = this.Key;
				BlueStacksUIBinding.Bind(this.mTabHeader, value, "");
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060016F1 RID: 5873 RVA: 0x0000F72C File Offset: 0x0000D92C
		// (set) Token: 0x060016F2 RID: 5874 RVA: 0x0000F734 File Offset: 0x0000D934
		public int Column { get; internal set; }

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060016F3 RID: 5875 RVA: 0x0000F73D File Offset: 0x0000D93D
		// (set) Token: 0x060016F4 RID: 5876 RVA: 0x0000F745 File Offset: 0x0000D945
		public BrowserControl AssociatedUserControl
		{
			get
			{
				return this.mAssociatedUserControl;
			}
			set
			{
				this.mAssociatedUserControl = value;
				if (this.mAssociatedUserControl != null)
				{
					this.mAssociatedUserControl.RenderTransform = this.mTranslateTransform;
				}
			}
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x00089128 File Offset: 0x00087328
		private void AssociatedGrid_LayoutUpdated(object sender, EventArgs e)
		{
			try
			{
				if (!this.IsSelected && Math.Abs(Math.Abs(this.mTranslateTransform.X) - this.mAssociatedUserControl.ActualWidth) <= 10.0)
				{
					this.mAssociatedUserControl.Visibility = Visibility.Hidden;
					this.mAssociatedUserControl.LayoutUpdated -= this.AssociatedGrid_LayoutUpdated;
				}
			}
			catch (Exception ex)
			{
				Logger.Info("Error updating " + ex.ToString());
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060016F6 RID: 5878 RVA: 0x0000F767 File Offset: 0x0000D967
		// (set) Token: 0x060016F7 RID: 5879 RVA: 0x000891B8 File Offset: 0x000873B8
		public bool IsSelected
		{
			get
			{
				return this.mIsSelected;
			}
			set
			{
				if (this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton != this || !value)
				{
					this.mIsSelected = value;
					if (this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton != null)
					{
						HomeAppTabButton mSelectedHomeAppTabButton = this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton;
						if (this.Column > mSelectedHomeAppTabButton.Column)
						{
							HomeAppTabButton.mIsSwipeDirectonLeft = true;
						}
						else
						{
							HomeAppTabButton.mIsSwipeDirectonLeft = false;
						}
						this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton = null;
						if (this.mAssociatedUserControl == mSelectedHomeAppTabButton.mAssociatedUserControl)
						{
							mSelectedHomeAppTabButton.IsAnimationIgnored = true;
							this.IsAnimationIgnored = true;
						}
						mSelectedHomeAppTabButton.IsSelected = false;
					}
					if (this.mIsSelected)
					{
						BlueStacksUIBinding.BindColor(this.mTabHeader, TextBlock.ForegroundProperty, "SelectedHomeAppTabForegroundColor");
						BlueStacksUIBinding.BindColor(this.mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundColor");
						this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton = this;
						this.ParentWindow.Utils.ResetPendingUIOperations();
						this.mGridHighlighterBox.Visibility = Visibility.Visible;
						this.AnimateAssociatedGrid(true);
						this.ImageBox.ImageName = this.Key.Replace("Normal", "Active");
						if (this.mAssociatedUserControl.CefBrowser != null)
						{
							this.ParentWindow.mWelcomeTab.mHomeAppManager.SetSearchTextBoxFocus(this.animationTime + 100);
							MiscUtils.SetFocusAsync(this.mAssociatedUserControl.CefBrowser, 0);
							return;
						}
					}
					else
					{
						BlueStacksUIBinding.BindColor(this.mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundColor");
						this.mGridHighlighterBox.Visibility = Visibility.Hidden;
						this.AnimateAssociatedGrid(false);
						this.ImageBox.ImageName = this.Key.Replace("Active", "Normal");
					}
				}
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060016F8 RID: 5880 RVA: 0x0000F76F File Offset: 0x0000D96F
		// (set) Token: 0x060016F9 RID: 5881 RVA: 0x0000F777 File Offset: 0x0000D977
		public bool IsAnimationIgnored { get; set; }

		// Token: 0x060016FA RID: 5882 RVA: 0x00089368 File Offset: 0x00087568
		private void AnimateAssociatedGrid(bool show)
		{
			if (this.IsAnimationIgnored)
			{
				this.IsAnimationIgnored = false;
				return;
			}
			this.mAssociatedUserControl.LayoutUpdated += this.AssociatedGrid_LayoutUpdated;
			DoubleAnimation doubleAnimation;
			if (show)
			{
				this.mAssociatedUserControl.Visibility = Visibility.Visible;
				if (HomeAppTabButton.mIsSwipeDirectonLeft)
				{
					doubleAnimation = new DoubleAnimation(this.mAssociatedUserControl.ActualWidth, 0.0, TimeSpan.FromMilliseconds((double)this.animationTime));
				}
				else
				{
					doubleAnimation = new DoubleAnimation(-1.0 * this.mAssociatedUserControl.ActualWidth, 0.0, TimeSpan.FromMilliseconds((double)this.animationTime));
				}
			}
			else if (HomeAppTabButton.mIsSwipeDirectonLeft)
			{
				doubleAnimation = new DoubleAnimation(0.0, -1.0 * this.mAssociatedUserControl.ActualWidth, TimeSpan.FromMilliseconds((double)this.animationTime));
			}
			else
			{
				doubleAnimation = new DoubleAnimation(0.0, this.mAssociatedUserControl.ActualWidth, TimeSpan.FromMilliseconds((double)this.animationTime));
			}
			this.mTranslateTransform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x0000F780 File Offset: 0x0000D980
		public HomeAppTabButton()
		{
			this.InitializeComponent();
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x0000F7AF File Offset: 0x0000D9AF
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.IsSelected = true;
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x0000F7B8 File Offset: 0x0000D9B8
		private void Button_MouseEnter(object sender, MouseEventArgs e)
		{
			if (!this.IsSelected)
			{
				BlueStacksUIBinding.BindColor(this.mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundHoverColor");
				BlueStacksUIBinding.BindColor(this.mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundHoverColor");
			}
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x0000F7EC File Offset: 0x0000D9EC
		private void Button_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.IsSelected)
			{
				BlueStacksUIBinding.BindColor(this.mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundColor");
				BlueStacksUIBinding.BindColor(this.mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundColor");
			}
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x0000F820 File Offset: 0x0000DA20
		private void Button_Loaded(object sender, RoutedEventArgs e)
		{
			this.SetMaxWidth(0.0);
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x00089494 File Offset: 0x00087694
		internal void SetMaxWidth(double extraWidth = 0.0)
		{
			double num = 0.0;
			Typeface typeface = new Typeface(this.mTabHeader.FontFamily, this.mTabHeader.FontStyle, this.mTabHeader.FontWeight, this.mTabHeader.FontStretch);
			FormattedText formattedText = new FormattedText(this.mTabHeader.Text, Thread.CurrentThread.CurrentCulture, this.mTabHeader.FlowDirection, typeface, this.mTabHeader.FontSize, this.mTabHeader.Foreground);
			num += this.mAppTabNotificationCountBorder.ActualWidth;
			num += formattedText.WidthIncludingTrailingWhitespace;
			num += this.tabGrid.ActualHeight;
			num += 30.0;
			if (extraWidth == 1.7976931348623157E+308)
			{
				if (this.tabGrid.ColumnDefinitions[0].Width.IsStar)
				{
					num += 50.0;
				}
			}
			else
			{
				num += extraWidth;
			}
			int num2 = Grid.GetColumn(this.mTabHeader);
			if ((extraWidth > 0.0 && extraWidth < 1.7976931348623157E+308) || (this.tabGrid.ColumnDefinitions[0].Width.IsStar && extraWidth == 1.7976931348623157E+308))
			{
				this.tabGrid.ColumnDefinitions[num2].Width = new GridLength(1.0, GridUnitType.Auto);
				this.tabGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
				this.tabGrid.ColumnDefinitions[5].Width = new GridLength(1.0, GridUnitType.Star);
			}
			else
			{
				this.tabGrid.ColumnDefinitions[num2].Width = new GridLength(1.0, GridUnitType.Star);
				this.tabGrid.ColumnDefinitions[0].Width = new GridLength(0.0, GridUnitType.Pixel);
				this.tabGrid.ColumnDefinitions[5].Width = new GridLength(0.0, GridUnitType.Pixel);
			}
			num2 = Grid.GetColumn(this);
			((Grid)base.Parent).ColumnDefinitions[num2].MaxWidth = num;
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x0000F820 File Offset: 0x0000DA20
		private void mAppTabNotificationCountBorder_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.SetMaxWidth(0.0);
		}

		// Token: 0x04000E1C RID: 3612
		private int animationTime = 150;

		// Token: 0x04000E1D RID: 3613
		private MainWindow mMainWindow;

		// Token: 0x04000E1F RID: 3615
		private static bool mIsSwipeDirectonLeft = true;

		// Token: 0x04000E20 RID: 3616
		private TranslateTransform mTranslateTransform = new TranslateTransform();

		// Token: 0x04000E21 RID: 3617
		private BrowserControl mAssociatedUserControl;

		// Token: 0x04000E22 RID: 3618
		private bool mIsSelected;
	}
}
