using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000B3 RID: 179
	public class ExportMacroWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x0600074D RID: 1869 RVA: 0x00006C23 File Offset: 0x00004E23
		public ExportMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
		{
			this.InitializeComponent();
			this.mOperationWindow = window;
			this.ParentWindow = mainWindow;
			this.mScriptsStackPanel = this.mScriptsListScrollbar.Content as StackPanel;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00006C60 File Offset: 0x00004E60
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.CloseWindow();
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00006C68 File Offset: 0x00004E68
		private void CloseWindow()
		{
			base.Close();
			this.mOperationWindow.mExportMacroWindow = null;
			this.mOperationWindow.mOverlayGrid.Visibility = Visibility.Hidden;
			this.mOperationWindow.Focus();
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x000285E0 File Offset: 0x000267E0
		internal void Init()
		{
			try
			{
				foreach (BiDirectionalVertex<MacroRecording> biDirectionalVertex in MacroGraph.Instance.Vertices)
				{
					MacroRecording macroRecording = (MacroRecording)biDirectionalVertex;
					this.ParentWindow.mIsScriptsPresent = true;
					if (!this.mNameRecordingDict.ContainsKey(macroRecording.Name.ToLower(CultureInfo.InvariantCulture)))
					{
						this.mNameRecordingDict.Add(macroRecording.Name.ToLower(CultureInfo.InvariantCulture), macroRecording);
						CustomCheckbox customCheckbox = new CustomCheckbox
						{
							Content = macroRecording.Name,
							TextFontSize = 12.0,
							Margin = new Thickness(0.0, 6.0, 0.0, 6.0)
						};
						customCheckbox.Checked += this.Box_Checked;
						customCheckbox.Unchecked += this.Box_Unchecked;
						customCheckbox.ImageMargin = new Thickness(2.0);
						customCheckbox.MaxHeight = 20.0;
						this.mScriptsStackPanel.Children.Add(customCheckbox);
					}
				}
				this.mNumberOfFilesSelectedForExport = 0;
			}
			catch (Exception ex)
			{
				Logger.Error("Error in export window init err: " + ex.ToString());
			}
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x0002876C File Offset: 0x0002696C
		private void Box_Unchecked(object sender, RoutedEventArgs e)
		{
			this.mNumberOfFilesSelectedForExport--;
			if (this.mNumberOfFilesSelectedForExport == 0)
			{
				this.mExportBtn.IsEnabled = false;
			}
			if (this.mNumberOfFilesSelectedForExport == this.mScriptsStackPanel.Children.Count - 1)
			{
				this.mSelectAllBtn.IsChecked = new bool?(false);
			}
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x000287C8 File Offset: 0x000269C8
		private void Box_Checked(object sender, RoutedEventArgs e)
		{
			this.mNumberOfFilesSelectedForExport++;
			if (this.mNumberOfFilesSelectedForExport == 1)
			{
				this.mExportBtn.IsEnabled = true;
			}
			if (this.mNumberOfFilesSelectedForExport == this.mScriptsStackPanel.Children.Count)
			{
				this.mSelectAllBtn.IsChecked = new bool?(true);
			}
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00028824 File Offset: 0x00026A24
		private void ExportBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int num = 0;
				List<MacroRecording> list = new List<MacroRecording>();
				foreach (object obj in this.mScriptsStackPanel.Children)
				{
					bool? isChecked = (obj as CustomCheckbox).IsChecked;
					bool flag = true;
					if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
					{
						list.Add(this.mNameRecordingDict.ElementAt(num).Value);
					}
					num++;
				}
				if (list.Count != 0)
				{
					using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
					{
						if (folderBrowserDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
						{
							using (BackgroundWorker backgroundWorker = new BackgroundWorker())
							{
								backgroundWorker.DoWork += this.BgExport_DoWork;
								backgroundWorker.RunWorkerCompleted += this.BgExport_RunWorkerCompleted;
								this.ShowLoadingGrid(true);
								backgroundWorker.RunWorkerAsync(new List<object> { folderBrowserDialog.SelectedPath, list });
								goto IL_0105;
							}
						}
						this.ToggleCheckBoxForExport();
						IL_0105:
						goto IL_013D;
					}
				}
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_SELECTED", ""), 4.0, true);
				IL_013D:;
			}
			catch (Exception)
			{
				Logger.Error("Error while exporting script. err:" + e.ToString());
			}
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00006C99 File Offset: 0x00004E99
		private void BgExport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.ShowLoadingGrid(false);
			this.ToggleCheckBoxForExport();
			this.CloseWindow();
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x000289EC File Offset: 0x00026BEC
		private void ToggleCheckBoxForExport()
		{
			foreach (object obj in this.mScriptsStackPanel.Children)
			{
				(obj as CustomCheckbox).IsChecked = new bool?(false);
			}
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00028A50 File Offset: 0x00026C50
		private void BgExport_DoWork(object sender, DoWorkEventArgs e)
		{
			List<object> list = e.Argument as List<object>;
			string text = list[0] as string;
			foreach (MacroRecording macroRecording in (list[1] as List<MacroRecording>))
			{
				string name = macroRecording.Name;
				string text2 = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, name.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
				string text3 = Path.Combine(text, macroRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
				if (macroRecording.RecordingType == RecordingTypes.SingleRecording)
				{
					File.Copy(text2, text3, true);
				}
				else
				{
					try
					{
						Logger.Info("Saving multi-macro");
						List<string> list2 = new List<string>();
						foreach (BiDirectionalVertex<MacroRecording> biDirectionalVertex in MacroGraph.Instance.GetAllChilds(macroRecording))
						{
							MacroRecording macroRecording2 = (MacroRecording)biDirectionalVertex;
							list2.Add(File.ReadAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroRecording2.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json")));
						}
						MacroRecording macroRecording3 = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(text2), Utils.GetSerializerSettings());
						macroRecording3.SourceRecordings = list2;
						JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
						serializerSettings.Formatting = Formatting.Indented;
						string text4 = JsonConvert.SerializeObject(macroRecording3, serializerSettings);
						File.WriteAllText(text3, text4);
					}
					catch (Exception ex)
					{
						Logger.Error("Coulnd't take backup of script {0}, Ex: {1}", new object[] { name, ex });
					}
				}
			}
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00028C44 File Offset: 0x00026E44
		private void ShowLoadingGrid(bool isShow)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (isShow)
				{
					this.mLoadingGrid.Visibility = Visibility.Visible;
					return;
				}
				this.mLoadingGrid.Visibility = Visibility.Hidden;
			}), new object[0]);
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x00028C84 File Offset: 0x00026E84
		private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
		{
			if (this.mSelectAllBtn.IsChecked.Value)
			{
				using (IEnumerator enumerator = this.mScriptsStackPanel.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						(obj as CustomCheckbox).IsChecked = new bool?(true);
					}
					return;
				}
			}
			foreach (object obj2 in this.mScriptsStackPanel.Children)
			{
				(obj2 as CustomCheckbox).IsChecked = new bool?(false);
			}
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00028D4C File Offset: 0x00026F4C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/exportmacrowindow.xaml", UriKind.Relative);
			global::System.Windows.Application.LoadComponent(this, uri);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00028D7C File Offset: 0x00026F7C
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
				this.mMaskBorder = (Border)target;
				return;
			case 2:
				((CustomPictureBox)target).MouseLeftButtonUp += this.Close_MouseLeftButtonUp;
				return;
			case 3:
				this.mScriptsListScrollbar = (ScrollViewer)target;
				return;
			case 4:
				this.mSelectAllBtn = (CustomCheckbox)target;
				this.mSelectAllBtn.Click += this.SelectAllBtn_Click;
				return;
			case 5:
				this.mExportBtn = (CustomButton)target;
				this.mExportBtn.Click += this.ExportBtn_Click;
				return;
			case 6:
				this.mLoadingGrid = (BlueStacks.BlueStacksUI.ProgressBar)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040003E0 RID: 992
		private MacroRecorderWindow mOperationWindow;

		// Token: 0x040003E1 RID: 993
		private MainWindow ParentWindow;

		// Token: 0x040003E2 RID: 994
		internal StackPanel mScriptsStackPanel;

		// Token: 0x040003E3 RID: 995
		internal int mNumberOfFilesSelectedForExport;

		// Token: 0x040003E4 RID: 996
		private Dictionary<string, MacroRecording> mNameRecordingDict = new Dictionary<string, MacroRecording>();

		// Token: 0x040003E5 RID: 997
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x040003E6 RID: 998
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mScriptsListScrollbar;

		// Token: 0x040003E7 RID: 999
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mSelectAllBtn;

		// Token: 0x040003E8 RID: 1000
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mExportBtn;

		// Token: 0x040003E9 RID: 1001
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal BlueStacks.BlueStacksUI.ProgressBar mLoadingGrid;

		// Token: 0x040003EA RID: 1002
		private bool _contentLoaded;
	}
}
