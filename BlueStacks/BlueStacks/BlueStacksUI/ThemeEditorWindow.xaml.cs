using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BlueStacks.Common;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000175 RID: 373
	public partial class ThemeEditorWindow : CustomWindow
	{
		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0000B0FC File Offset: 0x000092FC
		// (set) Token: 0x06000EF1 RID: 3825 RVA: 0x0000B114 File Offset: 0x00009314
		public static ThemeEditorWindow Instance
		{
			get
			{
				if (ThemeEditorWindow.mInstance == null)
				{
					ThemeEditorWindow.mInstance = new ThemeEditorWindow();
				}
				return ThemeEditorWindow.mInstance;
			}
			set
			{
				ThemeEditorWindow.mInstance = value;
			}
		}

		// Token: 0x06000EF2 RID: 3826 RVA: 0x0005E738 File Offset: 0x0005C938
		public ThemeEditorWindow()
		{
			this.InitializeComponent();
			base.Closing += this.ThemeEditorWindow_Closing;
			base.Activated += this.ThemeEditorWindow_Activated;
			this.sliderX.Value = BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusX;
			this.sliderY.Value = BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusY;
			this.TabTransFormLandscape.IsChecked = new bool?(true);
			this.ListView2.ItemsSource = BlueStacksUIBinding.Instance.ImageModel.Keys.ToList<string>();
			using (DataTable dataTable = new DataTable())
			{
				dataTable.Columns.Add(new DataColumn("Category", typeof(string)));
				dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
				dataTable.Columns.Add(new DataColumn("Brush", typeof(Brush)));
				foreach (KeyValuePair<string, Brush> keyValuePair in BlueStacksUIColorManager.AppliedTheme.DictBrush)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Name"] = keyValuePair.Key;
					dataRow["Brush"] = keyValuePair.Value;
					if (BlueStacksUIColorManager.AppliedTheme.DictCategory.ContainsKey(keyValuePair.Key))
					{
						dataRow["Category"] = BlueStacksUIColorManager.AppliedTheme.DictCategory[keyValuePair.Key];
					}
					dataTable.Rows.Add(dataRow);
				}
				DataView defaultView = dataTable.DefaultView;
				defaultView.Sort = "Category asc";
				DataTable dataTable2 = defaultView.ToTable();
				this.dataGrid.ColumnsToBeGrouped.Add(0);
				this.dataGrid.DataSource = dataTable2;
				this.dataGrid.CellClick += this.DataGrid_CellClick;
				this.dataGrid.CellValueChanged += this.DataGrid_CellValueChanged;
				this.dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
				this.dataGrid.Columns["Brush"].Visible = false;
			}
			using (DataTable dataTable3 = new DataTable())
			{
				dataTable3.Columns.Add(new DataColumn("Name", typeof(string)));
				dataTable3.Columns.Add(new DataColumn("CornerRadius", typeof(CornerRadius)));
				foreach (KeyValuePair<string, CornerRadius> keyValuePair2 in BlueStacksUIColorManager.AppliedTheme.DictCornerRadius)
				{
					DataRow dataRow2 = dataTable3.NewRow();
					dataRow2["Name"] = keyValuePair2.Key;
					dataRow2["CornerRadius"] = keyValuePair2.Value;
					dataTable3.Rows.Add(dataRow2);
				}
				this.dataGrid1.DataSource = dataTable3;
				this.dataGrid1.CellClick += this.DataGrid1_CellClick;
				this.dataGrid1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			}
			this.ignore = false;
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x0005EB18 File Offset: 0x0005CD18
		private void DataGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			this.ignore = true;
			try
			{
				this.topleftCornerRadius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].TopLeft;
				this.toprightcornerradius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].TopRight;
				this.bottomleftCornerRadius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].BottomLeft;
				this.bottomrightcornerradius.Value = BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.Rows[e.RowIndex].Cells["Name"].Value.ToString()].BottomRight;
			}
			catch (Exception ex)
			{
				Console.WriteLine("exception:" + ex.ToString());
			}
			this.ignore = false;
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x0005ECB0 File Offset: 0x0005CEB0
		private void DataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (this.dataGrid.Columns[e.ColumnIndex].Name == "Category")
			{
				BlueStacksUIColorManager.AppliedTheme.DictCategory[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] = this.dataGrid.Rows[e.RowIndex].Cells["Category"].Value.ToString();
				this.dataGrid.Sort(this.dataGrid.Columns[e.ColumnIndex], ListSortDirection.Ascending);
			}
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x0005ED78 File Offset: 0x0005CF78
		private void DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			this.ignore = true;
			try
			{
				this.sliderA.Value = (double)(BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.A;
				this.sliderR.Value = (double)(BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.R;
				this.sliderG.Value = (double)(BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.G;
				this.sliderB.Value = (double)(BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.Rows[e.RowIndex].Cells["Name"].Value.ToString()] as SolidColorBrush).Color.B;
				this.textBox.Text = this.dataGrid.Rows[e.RowIndex].Cells["Brush"].Value.ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine("exception:" + ex.ToString());
			}
			this.ignore = false;
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x0005EF74 File Offset: 0x0005D174
		private void ThemeEditorWindow_Activated(object sender, EventArgs e)
		{
			if (this.isCreateDraftDirectory)
			{
				this.ListView2.ItemsSource = BlueStacksUIBinding.Instance.ImageModel.Keys.ToList<string>();
				this.isCreateDraftDirectory = false;
				ThemeEditorWindow.CopyEverything(CustomPictureBox.AssetsDir, this.DraftDirectory);
				File.Delete(Path.Combine(this.DraftDirectory, "ThemeThumbnail.png"));
			}
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x0000B11C File Offset: 0x0000931C
		private void ThemeEditorWindow_Closing(object sender, CancelEventArgs e)
		{
			this.isCreateDraftDirectory = true;
			e.Cancel = true;
			base.Hide();
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x0005EFD4 File Offset: 0x0005D1D4
		private void Color_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				byte b = (byte)this.sliderA.Value;
				byte b2 = (byte)this.sliderR.Value;
				byte b3 = (byte)this.sliderG.Value;
				byte b4 = (byte)this.sliderB.Value;
				Color color = Color.FromArgb(b, b2, b3, b4);
				Brush brush = new SolidColorBrush(color);
				this.gridColor.Background = brush;
				if (!this.ignore)
				{
					this.textBox.Text = color.ToString(CultureInfo.InvariantCulture);
					BlueStacksUIColorManager.AppliedTheme.DictBrush[this.dataGrid.CurrentRow.Cells["Name"].Value.ToString()] = new SolidColorBrush(new ColorUtils(color).WPFColor);
					if (this.dataGrid.CurrentRow.Cells["Category"].Value.ToString().Equals("*MainColors*", StringComparison.OrdinalIgnoreCase))
					{
						BlueStacksUIColorManager.AppliedTheme.CalculateAndNotify(true);
					}
					else
					{
						BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0005F100 File Offset: 0x0005D300
		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (!this.ignore)
				{
					SolidColorBrush solidColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.textBox.Text));
					this.sliderA.Value = (double)solidColorBrush.Color.A;
					this.sliderR.Value = (double)solidColorBrush.Color.R;
					this.sliderG.Value = (double)solidColorBrush.Color.G;
					this.sliderB.Value = (double)solidColorBrush.Color.B;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x0005F1B0 File Offset: 0x0005D3B0
		private void Curve_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!this.ignore)
			{
				BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusX = this.sliderX.Value;
				BlueStacksUIColorManager.AppliedTheme.AppIconRectangleGeometry.RadiusY = this.sliderY.Value;
				BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
			}
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0005F204 File Offset: 0x0005D404
		private void Load_Click(object sender, RoutedEventArgs e)
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = RegistryManager.Instance.ClientInstallDir
			})
			{
				if (folderBrowserDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
				{
					string selectedPath = folderBrowserDialog.SelectedPath;
					if (File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "ThemeFile")))
					{
						string fileName = Path.GetFileName(folderBrowserDialog.SelectedPath);
						if (!folderBrowserDialog.SelectedPath.Contains(RegistryManager.Instance.ClientInstallDir))
						{
							string text = Path.Combine(RegistryManager.Instance.ClientInstallDir, fileName);
							if (Directory.Exists(text))
							{
								global::System.Windows.MessageBox.Show("Theme with this name already exists. Please rename the folder an try again");
							}
							else
							{
								Directory.CreateDirectory(text);
								ThemeEditorWindow.CopyEverything(folderBrowserDialog.SelectedPath, text);
							}
						}
						BlueStacksUIColorManager.ReloadAppliedTheme(fileName);
					}
					else
					{
						global::System.Windows.MessageBox.Show("Please select theme folder");
					}
				}
			}
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0005F2EC File Offset: 0x0005D4EC
		private void Save_Click(object sender, RoutedEventArgs e)
		{
			string text = Interaction.InputBox("Theme name", "BlueStacks Theme Editor Tool", string.Format(CultureInfo.CurrentCulture, "{0:F}", new object[] { DateTime.Now }), 0, 0);
			if (Directory.Exists(Path.Combine(RegistryManager.Instance.ClientInstallDir, text)))
			{
				global::System.Windows.MessageBox.Show("Already Exists. Please retry");
				return;
			}
			Directory.CreateDirectory(Path.Combine(RegistryManager.Instance.ClientInstallDir, text));
			ThemeEditorWindow.CopyEverything(this.DraftDirectory, Path.Combine(RegistryManager.Instance.ClientInstallDir, text));
			RegistryManager.Instance.SetClientThemeNameInRegistry(text);
			Window w = BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0];
			w.Dispatcher.Invoke(new Action(delegate
			{
				RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)w.ActualWidth, (int)w.ActualHeight, 0.0, 0.0, PixelFormats.Pbgra32);
				renderTargetBitmap.Render(BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0]);
				PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
				pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
				using (Stream stream = File.Create(Path.Combine(CustomPictureBox.AssetsDir, "ThemeThumbnail.png")))
				{
					pngBitmapEncoder.Save(stream);
				}
			}), new object[0]);
			BlueStacksUIColorManager.AppliedTheme.Save(BlueStacksUIColorManager.GetThemeFilePath(RegistryManager.ClientThemeName));
			CustomPictureBox.UpdateImagesFromNewDirectory("");
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x0005F3F0 File Offset: 0x0005D5F0
		private static void CopyEverything(string SourcePath, string DestinationPath)
		{
			string[] array = Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories);
			for (int i = 0; i < array.Length; i++)
			{
				Directory.CreateDirectory(array[i].Replace(SourcePath, DestinationPath));
			}
			foreach (string text in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
			{
				File.Copy(text, text.Replace(SourcePath, DestinationPath), true);
			}
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0005F454 File Offset: 0x0005D654
		private void tabangle_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!this.ignore)
			{
				if (this.SearchTextBoxCurvature.IsChecked.Value)
				{
					BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm = new SkewTransform(this.tabangleX.Value, this.tabangleY.Value);
					BlueStacksUIColorManager.AppliedTheme.TextBoxAntiTransForm = new SkewTransform(this.tabangleX.Value * -1.0, this.tabangleY.Value * -1.0);
					BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
					return;
				}
				if (this.TabTransFormLandscape.IsChecked.Value)
				{
					BlueStacksUIColorManager.AppliedTheme.TabTransform = new SkewTransform(this.tabangleX.Value, this.tabangleY.Value);
					BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
					return;
				}
				BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait = new SkewTransform(this.tabangleX.Value, this.tabangleY.Value);
				BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
			}
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0005F560 File Offset: 0x0005D760
		private void TabTransFormCheckedPortrait(object sender, RoutedEventArgs e)
		{
			this.ignore = true;
			this.tabangleX.Value = BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX;
			this.tabangleY.Value = BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY;
			this.ignore = false;
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0005F5B0 File Offset: 0x0005D7B0
		private void SearchTextBoxCurvatureChecked(object sender, RoutedEventArgs e)
		{
			this.ignore = true;
			this.tabangleX.Value = BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm.AngleX;
			this.tabangleY.Value = BlueStacksUIColorManager.AppliedTheme.TextBoxTransForm.AngleY;
			this.ignore = false;
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0005F600 File Offset: 0x0005D800
		private void TabTransFormCheckedLandscape(object sender, RoutedEventArgs e)
		{
			this.ignore = true;
			this.tabangleX.Value = BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX;
			this.tabangleY.Value = BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY;
			this.ignore = false;
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x0005F650 File Offset: 0x0005D850
		private void cornerRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!this.ignore)
			{
				BlueStacksUIColorManager.AppliedTheme.DictCornerRadius[this.dataGrid1.CurrentRow.Cells["Name"].Value.ToString()] = new CornerRadius(this.topleftCornerRadius.Value, this.toprightcornerradius.Value, this.bottomrightcornerradius.Value, this.bottomleftCornerRadius.Value);
				try
				{
					this.dataGrid1[1, this.dataGrid1.CurrentRow.Index].Value = new CornerRadius(this.topleftCornerRadius.Value, this.toprightcornerradius.Value, this.bottomrightcornerradius.Value, this.bottomleftCornerRadius.Value);
				}
				catch (Exception)
				{
				}
				BlueStacksUIColorManager.AppliedTheme.NotifyUIElements();
			}
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0005F744 File Offset: 0x0005D944
		private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.ListView2.SelectedItem != null)
			{
				this.selectedItem = this.ListView2.SelectedItem.ToString();
				CustomPictureBox.SetBitmapImage(this.pictureBox, this.ListView2.SelectedItem.ToString(), false);
			}
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0005F790 File Offset: 0x0005D990
		private void pictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.selectedItem))
			{
				Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
				bool? flag = openFileDialog.ShowDialog();
				if (flag != null && flag.Value)
				{
					string fileName = openFileDialog.FileName;
					string text = Path.Combine(this.DraftDirectory, this.selectedItem.ToString(CultureInfo.InvariantCulture));
					if (!File.Exists(text))
					{
						text += ".png";
					}
					File.Copy(fileName, text, true);
					CustomPictureBox.UpdateImagesFromNewDirectory(this.DraftDirectory);
				}
			}
		}

		// Token: 0x040009BE RID: 2494
		internal const string THUMBNAIL_ICON = "ThemeThumbnail.png";

		// Token: 0x040009BF RID: 2495
		private string selectedItem = string.Empty;

		// Token: 0x040009C0 RID: 2496
		private bool isCreateDraftDirectory = true;

		// Token: 0x040009C1 RID: 2497
		private bool ignore = true;

		// Token: 0x040009C2 RID: 2498
		private static ThemeEditorWindow mInstance;

		// Token: 0x040009C3 RID: 2499
		private const string DraftFolderName = "Drafts";

		// Token: 0x040009C4 RID: 2500
		private string DraftDirectory = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Drafts");
	}
}
