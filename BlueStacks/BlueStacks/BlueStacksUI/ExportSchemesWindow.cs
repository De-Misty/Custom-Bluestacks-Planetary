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
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200015E RID: 350
	public class ExportSchemesWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x06000E86 RID: 3718 RVA: 0x0000AD32 File Offset: 0x00008F32
		public ExportSchemesWindow(KeymapCanvasWindow window, MainWindow mainWindow)
		{
			this.InitializeComponent();
			this.CanvasWindow = window;
			this.ParentWindow = mainWindow;
			this.mSchemesStackPanel = this.mSchemesListScrollbar.Content as StackPanel;
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x0000AD6F File Offset: 0x00008F6F
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.CloseWindow();
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x0000AD77 File Offset: 0x00008F77
		private void CloseWindow()
		{
			base.Close();
			this.CanvasWindow.SidebarWindow.mExportSchemesWindow = null;
			this.CanvasWindow.SidebarWindow.mOverlayGrid.Visibility = Visibility.Hidden;
			this.CanvasWindow.SidebarWindow.Focus();
		}

		// Token: 0x06000E89 RID: 3721 RVA: 0x0005C22C File Offset: 0x0005A42C
		internal void Init()
		{
			try
			{
				this.mNumberOfSchemesSelectedForExport = 0;
				this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn).ToList<IMControlScheme>().ForEach(delegate(IMControlScheme scheme)
				{
					this.<Init>g__AddSchemeToExportCheckbox|8_4(scheme);
				});
				this.ParentWindow.OriginalLoadedConfig.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn).ToList<IMControlScheme>().ForEach(delegate(IMControlScheme scheme)
				{
					if (this.dict.Keys.Contains(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
					{
						scheme.Name += " (Edited)";
						scheme.Name = KMManager.GetUniqueName(scheme.Name, this.ParentWindow.OriginalLoadedConfig.ControlSchemesDict.Keys);
					}
					this.<Init>g__AddSchemeToExportCheckbox|8_4(scheme);
				});
			}
			catch (Exception ex)
			{
				Logger.Error("Error in export window init err: " + ex.ToString());
			}
		}

		// Token: 0x06000E8A RID: 3722 RVA: 0x0005C300 File Offset: 0x0005A500
		private void Box_Unchecked(object sender, RoutedEventArgs e)
		{
			this.mNumberOfSchemesSelectedForExport--;
			if (this.mNumberOfSchemesSelectedForExport == this.mSchemesStackPanel.Children.Count - 1)
			{
				this.mSelectAllBtn.IsChecked = new bool?(false);
			}
			if (this.mNumberOfSchemesSelectedForExport == 0)
			{
				this.mExportBtn.IsEnabled = false;
			}
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x0005C35C File Offset: 0x0005A55C
		private void Box_Checked(object sender, RoutedEventArgs e)
		{
			this.mNumberOfSchemesSelectedForExport++;
			if (this.mNumberOfSchemesSelectedForExport == this.mSchemesStackPanel.Children.Count)
			{
				this.mSelectAllBtn.IsChecked = new bool?(true);
			}
			if (this.mNumberOfSchemesSelectedForExport == 1)
			{
				this.mExportBtn.IsEnabled = true;
			}
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x0005C3B8 File Offset: 0x0005A5B8
		private void ExportBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int num = 0;
				List<IMControlScheme> list = new List<IMControlScheme>();
				foreach (object obj in this.mSchemesStackPanel.Children)
				{
					bool? isChecked = (obj as CustomCheckbox).IsChecked;
					bool flag = true;
					if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
					{
						list.Add(this.dict.ElementAt(num).Value);
					}
					num++;
				}
				if (list.Count != 0)
				{
					using (SaveFileDialog saveFileDialog = new SaveFileDialog
					{
						AddExtension = true,
						DefaultExt = ".cfg",
						Filter = "Cfg files(*.cfg) | *.cfg",
						FileName = this.ParentWindow.StaticComponents.mSelectedTabButton.AppName
					})
					{
						if (saveFileDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
						{
							using (BackgroundWorker backgroundWorker = new BackgroundWorker())
							{
								backgroundWorker.DoWork += this.BgExport_DoWork;
								backgroundWorker.RunWorkerCompleted += this.BgExport_RunWorkerCompleted;
								this.ShowLoadingGrid(true);
								backgroundWorker.RunWorkerAsync(new List<object> { saveFileDialog.FileName, list });
								goto IL_013D;
							}
						}
						this.ToggleCheckBoxForExport();
						IL_013D:
						goto IL_0175;
					}
				}
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_SELECTED", ""), 1.3, false);
				IL_0175:;
			}
			catch (Exception)
			{
				Logger.Error("Error while exporting script. err:" + e.ToString());
			}
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x0000ADB7 File Offset: 0x00008FB7
		private void BgExport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.ShowLoadingGrid(false);
			this.ToggleCheckBoxForExport();
			this.CloseWindow();
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x0005C5B8 File Offset: 0x0005A7B8
		private void ToggleCheckBoxForExport()
		{
			foreach (object obj in this.mSchemesStackPanel.Children)
			{
				(obj as CustomCheckbox).IsChecked = new bool?(false);
			}
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x0005C61C File Offset: 0x0005A81C
		private void BgExport_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				List<object> list = e.Argument as List<object>;
				string text = list[0] as string;
				List<IMControlScheme> list2 = list[1] as List<IMControlScheme>;
				IMConfig imconfig = new IMConfig();
				imconfig.Strings = this.ParentWindow.OriginalLoadedConfig.Strings.DeepCopy<Dictionary<string, Dictionary<string, string>>>();
				imconfig.ControlSchemes = list2;
				JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
				serializerSettings.Formatting = Formatting.Indented;
				string text2 = JsonConvert.SerializeObject(imconfig, serializerSettings);
				File.WriteAllText(text, text2);
				this.ParentWindow.mCommonHandler.AddToastPopup(this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_CONTROLS_EXPORTED", ""), 1.3, false);
			}
			catch (Exception)
			{
				Logger.Error("Error in creating exported file " + e.ToString());
			}
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x0005C6F0 File Offset: 0x0005A8F0
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

		// Token: 0x06000E91 RID: 3729 RVA: 0x0005C730 File Offset: 0x0005A930
		private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
		{
			if (this.mSelectAllBtn.IsChecked.Value)
			{
				using (IEnumerator enumerator = this.mSchemesStackPanel.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						(obj as CustomCheckbox).IsChecked = new bool?(true);
					}
					return;
				}
			}
			foreach (object obj2 in this.mSchemesStackPanel.Children)
			{
				(obj2 as CustomCheckbox).IsChecked = new bool?(false);
			}
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x0005C7F8 File Offset: 0x0005A9F8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/exportschemeswindow.xaml", UriKind.Relative);
			global::System.Windows.Application.LoadComponent(this, uri);
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x0005C828 File Offset: 0x0005AA28
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
				this.mSchemesListScrollbar = (ScrollViewer)target;
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

		// Token: 0x06000E97 RID: 3735 RVA: 0x0005C960 File Offset: 0x0005AB60
		[CompilerGenerated]
		private void <Init>g__AddSchemeToExportCheckbox|8_4(IMControlScheme scheme)
		{
			this.dict.Add(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim(), scheme);
			CustomCheckbox customCheckbox = new CustomCheckbox
			{
				Content = scheme.Name,
				TextFontSize = 14.0,
				ImageMargin = new Thickness(2.0),
				Margin = new Thickness(0.0, 1.0, 0.0, 1.0),
				MaxHeight = 20.0
			};
			customCheckbox.Checked += this.Box_Checked;
			customCheckbox.Unchecked += this.Box_Unchecked;
			this.mSchemesStackPanel.Children.Add(customCheckbox);
		}

		// Token: 0x0400093E RID: 2366
		private KeymapCanvasWindow CanvasWindow;

		// Token: 0x0400093F RID: 2367
		private MainWindow ParentWindow;

		// Token: 0x04000940 RID: 2368
		internal StackPanel mSchemesStackPanel;

		// Token: 0x04000941 RID: 2369
		internal int mNumberOfSchemesSelectedForExport;

		// Token: 0x04000942 RID: 2370
		private Dictionary<string, IMControlScheme> dict = new Dictionary<string, IMControlScheme>();

		// Token: 0x04000943 RID: 2371
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000944 RID: 2372
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mSchemesListScrollbar;

		// Token: 0x04000945 RID: 2373
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mSelectAllBtn;

		// Token: 0x04000946 RID: 2374
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mExportBtn;

		// Token: 0x04000947 RID: 2375
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal BlueStacks.BlueStacksUI.ProgressBar mLoadingGrid;

		// Token: 0x04000948 RID: 2376
		private bool _contentLoaded;
	}
}
