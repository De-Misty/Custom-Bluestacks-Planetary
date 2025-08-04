using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI.Controls
{
	// Token: 0x020002BB RID: 699
	public partial class ChangeThemeWindow : UserControl
	{
		// Token: 0x060019B7 RID: 6583 RVA: 0x00011512 File Offset: 0x0000F712
		public ChangeThemeWindow(MainWindow parentWindow)
		{
			this.InitializeComponent();
			this.ParentWindow = parentWindow;
			this.ThemesDrawer = this.mThemesDrawerScrollBar.Content as WrapPanel;
			this.AddSkinImages();
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x00099080 File Offset: 0x00097280
		public void AddSkinImages()
		{
			try
			{
				this.ThemesDrawer.Children.Clear();
				foreach (string text in Directory.GetDirectories(RegistryManager.Instance.ClientInstallDir))
				{
					if (File.Exists(Path.Combine(text, "ThemeThumbnail.png")))
					{
						string themeName = BlueStacksUIColorManager.GetThemeName(text);
						SkinSelectorControl skinSelectorControl = new SkinSelectorControl
						{
							Visibility = Visibility.Visible,
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Top
						};
						skinSelectorControl.mThemeImage.Visibility = Visibility.Visible;
						skinSelectorControl.mThemeName.Visibility = Visibility.Visible;
						skinSelectorControl.mThemeImage.IsFullImagePath = true;
						skinSelectorControl.mThemeImage.ImageName = Path.Combine(text, "ThemeThumbnail.png");
						skinSelectorControl.mThemeCheckButton.Height = 30.0;
						skinSelectorControl.mThemeName.ToolTip = themeName;
						skinSelectorControl.mThemeName.Width = double.NaN;
						skinSelectorControl.mThemeCheckButton.Width = double.NaN;
						skinSelectorControl.mThemeName.Text = themeName;
						BlueStacksUIBinding.BindColor(skinSelectorControl.mThemeName, TextBlock.ForegroundProperty, "ContextMenuItemForegroundColor");
						skinSelectorControl.mThemeCheckButton.Tag = Path.GetFileName(text);
						skinSelectorControl.mThemeCheckButton.Click += this.ThemeApplyButton_Click;
						if (string.Compare(RegistryManager.ClientThemeName, Path.GetFileName(text), StringComparison.OrdinalIgnoreCase) == 0)
						{
							skinSelectorControl.mThemeAppliedText.Text = LocaleStrings.GetLocalizedString("STRING_APPLIED", "");
							skinSelectorControl.mThemeAppliedText.Visibility = Visibility.Visible;
							skinSelectorControl.mThemeAppliedText.Margin = new Thickness(0.0, 3.0, 4.0, 0.0);
						}
						else
						{
							skinSelectorControl.mThemeCheckButton.ButtonColor = ButtonColors.Blue;
							skinSelectorControl.mThemeCheckButton.IsEnabled = true;
							skinSelectorControl.mThemeCheckButton.Content = LocaleStrings.GetLocalizedString("STRING_APPLY", "");
							skinSelectorControl.mThemeCheckButton.Visibility = Visibility.Visible;
						}
						this.ThemesDrawer.Children.Add(skinSelectorControl);
						this.mThemesDrawerScrollBar.Visibility = Visibility.Visible;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in populating themes in skin widget " + ex.ToString());
			}
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x000992E0 File Offset: 0x000974E0
		private void ThemeApplyButton_Click(object sender, RoutedEventArgs e)
		{
			Logger.Info("Clicked theme apply button");
			string text = (sender as CustomButton).Tag.ToString();
			this.ParentWindow.Utils.ApplyTheme(text);
			this.AddSkinImages();
			this.ParentWindow.Utils.RestoreWallpaperImageForAllVms();
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x00008DB9 File Offset: 0x00006FB9
		private void mCrossButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			BlueStacksUIUtils.CloseContainerWindow(this);
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void mCrossButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x0400103B RID: 4155
		private WrapPanel ThemesDrawer;

		// Token: 0x0400103C RID: 4156
		private MainWindow ParentWindow;
	}
}
