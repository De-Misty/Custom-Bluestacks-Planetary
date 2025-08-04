using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000C7 RID: 199
	public class BlueStacksAdvancedExit : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000809 RID: 2057 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x0600080A RID: 2058 RVA: 0x00004786 File Offset: 0x00002986
		bool IDimOverlayControl.IsCloseOnOverLayClick
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600080B RID: 2059 RVA: 0x00007270 File Offset: 0x00005470
		// (set) Token: 0x0600080C RID: 2060 RVA: 0x00007278 File Offset: 0x00005478
		public bool ShowControlInSeparateWindow { get; set; } = true;

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x0600080D RID: 2061 RVA: 0x00007281 File Offset: 0x00005481
		// (set) Token: 0x0600080E RID: 2062 RVA: 0x00007289 File Offset: 0x00005489
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x0600080F RID: 2063 RVA: 0x00007292 File Offset: 0x00005492
		bool IDimOverlayControl.Close()
		{
			this.Close();
			return true;
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x000047D5 File Offset: 0x000029D5
		bool IDimOverlayControl.Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0000729C File Offset: 0x0000549C
		public BlueStacksAdvancedExit(MainWindow window)
		{
			this.ParentWindow = window;
			this.InitializeComponent();
			this.AddOptions();
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x000072CE File Offset: 0x000054CE
		private void AddOptions()
		{
			this.GenerateOptions("STRING_QUIT_BLUESTACKS", LocaleStringsConstants.ExitOptions);
			this.AddLineSeperator();
			this.GenerateOptions("STRING_RESTART", LocaleStringsConstants.RestartOptions);
			this.AddLineSeperator();
			this.GenerateCheckBox();
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0002D7C4 File Offset: 0x0002B9C4
		private void AddLineSeperator()
		{
			Border border = new Border
			{
				Opacity = 0.5,
				Height = 1.0,
				Margin = new Thickness(0.0, 10.0, 0.0, 0.0)
			};
			BlueStacksUIBinding.BindColor(border, Border.BackgroundProperty, "SettingsWindowTabMenuItemForeground");
			this.mOptionsStackPanel.Children.Add(border);
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0002D848 File Offset: 0x0002BA48
		private void GenerateCheckBox()
		{
			CustomCheckbox customCheckbox = new CustomCheckbox();
			BlueStacksUIBinding.Bind(customCheckbox, "STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04");
			if (customCheckbox.Image != null)
			{
				customCheckbox.Image.Height = 14.0;
				customCheckbox.Image.Width = 14.0;
			}
			customCheckbox.Height = 20.0;
			customCheckbox.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
			customCheckbox.IsChecked = new bool?(false);
			customCheckbox.Checked += this.DontShowAgainCB_Checked;
			customCheckbox.Unchecked += this.DontShowAgainCB_Unchecked;
			this.mOptionsStackPanel.Children.Add(customCheckbox);
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x00007302 File Offset: 0x00005502
		private void DontShowAgainCB_Checked(object sender, RoutedEventArgs e)
		{
			RegistryManager.Instance.IsQuitOptionSaved = true;
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0000730F File Offset: 0x0000550F
		private void DontShowAgainCB_Unchecked(object sender, RoutedEventArgs e)
		{
			RegistryManager.Instance.IsQuitOptionSaved = false;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0002D918 File Offset: 0x0002BB18
		private void GenerateOptions(string title, string[] childrenKeys)
		{
			TextBlock textBlock = new TextBlock();
			BlueStacksUIBinding.Bind(textBlock, title, "");
			textBlock.Padding = new Thickness(0.0);
			textBlock.FontSize = 16.0;
			textBlock.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
			BlueStacksUIBinding.BindColor(textBlock, Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
			textBlock.FontWeight = FontWeights.Normal;
			textBlock.HorizontalAlignment = HorizontalAlignment.Left;
			textBlock.VerticalAlignment = VerticalAlignment.Center;
			this.mOptionsStackPanel.Children.Add(textBlock);
			foreach (string text in childrenKeys)
			{
				CustomRadioButton customRadioButton = new CustomRadioButton();
				customRadioButton.Checked += this.Btn_Checked;
				customRadioButton.HorizontalAlignment = HorizontalAlignment.Left;
				BlueStacksUIBinding.Bind(customRadioButton, text);
				customRadioButton.Tag = text;
				customRadioButton.Margin = new Thickness(0.0, 10.0, 0.0, 5.0);
				this.mOptionsStackPanel.Children.Add(customRadioButton);
				if (text == this.mCurrentGlobalDefault)
				{
					customRadioButton.IsChecked = new bool?(true);
				}
			}
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0000731C File Offset: 0x0000551C
		private void Btn_Checked(object sender, RoutedEventArgs e)
		{
			RegistryManager.Instance.QuitDefaultOption = (sender as CustomRadioButton).Tag.ToString();
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000819 RID: 2073 RVA: 0x00007338 File Offset: 0x00005538
		public CustomButton YesButton
		{
			get
			{
				return this.mYesButton;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x00007340 File Offset: 0x00005540
		public CustomButton NoButton
		{
			get
			{
				return this.mNoButton;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x0600081B RID: 2075 RVA: 0x00007348 File Offset: 0x00005548
		public CustomPictureBox CrossButton
		{
			get
			{
				return this.mCrossButtonPictureBox;
			}
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x0002DA70 File Offset: 0x0002BC70
		internal bool Close()
		{
			try
			{
				BlueStacksUIUtils.CloseContainerWindow(this);
				this.ParentWindow.HideDimOverlay();
				base.Visibility = Visibility.Hidden;
				return true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception while trying to close the advanced exit from dimoverlay " + ex.ToString());
			}
			return false;
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x00007350 File Offset: 0x00005550
		private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00007350 File Offset: 0x00005550
		private void MYesButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x00007350 File Offset: 0x00005550
		private void MNoButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0002DAC4 File Offset: 0x0002BCC4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/bluestacksadvancedexit.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0002DAF4 File Offset: 0x0002BCF4
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
				this.mCrossButtonPictureBox = (CustomPictureBox)target;
				this.mCrossButtonPictureBox.PreviewMouseLeftButtonUp += this.Close_PreviewMouseLeftButtonUp;
				return;
			case 2:
				this.mParentGrid = (Grid)target;
				return;
			case 3:
				this.mTitleGrid = (Grid)target;
				return;
			case 4:
				this.mTitleText = (TextBlock)target;
				return;
			case 5:
				this.mOptionsGrid = (Grid)target;
				return;
			case 6:
				this.mOptionsStackPanel = (StackPanel)target;
				return;
			case 7:
				this.mFooterGrid = (Grid)target;
				return;
			case 8:
				this.mNoButton = (CustomButton)target;
				this.mNoButton.PreviewMouseLeftButtonUp += this.MNoButton_PreviewMouseLeftButtonUp;
				return;
			case 9:
				this.mYesButton = (CustomButton)target;
				this.mYesButton.PreviewMouseLeftButtonUp += this.MYesButton_PreviewMouseLeftButtonUp;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400046D RID: 1133
		private MainWindow ParentWindow;

		// Token: 0x0400046E RID: 1134
		private string mCurrentGlobalDefault = RegistryManager.Instance.QuitDefaultOption;

		// Token: 0x0400046F RID: 1135
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCrossButtonPictureBox;

		// Token: 0x04000470 RID: 1136
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mParentGrid;

		// Token: 0x04000471 RID: 1137
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTitleGrid;

		// Token: 0x04000472 RID: 1138
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTitleText;

		// Token: 0x04000473 RID: 1139
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOptionsGrid;

		// Token: 0x04000474 RID: 1140
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mOptionsStackPanel;

		// Token: 0x04000475 RID: 1141
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mFooterGrid;

		// Token: 0x04000476 RID: 1142
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mNoButton;

		// Token: 0x04000477 RID: 1143
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mYesButton;

		// Token: 0x04000478 RID: 1144
		private bool _contentLoaded;
	}
}
