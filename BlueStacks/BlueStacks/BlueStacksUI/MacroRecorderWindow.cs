using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000FA RID: 250
	public class MacroRecorderWindow : CustomWindow, IDisposable, IComponentConnector
	{
		// Token: 0x06000A84 RID: 2692 RVA: 0x0003AF64 File Offset: 0x00039164
		public MacroRecorderWindow(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			base.Owner = this.ParentWindow;
			base.IsShowGLWindow = true;
			this.mScriptsStackPanel = this.mScriptsListScrollbar.Content as StackPanel;
			base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			if (window != null)
			{
				window.mCommonHandler.MacroSettingChangedEvent += this.ParentWindow_MacroSettingChangedEvent;
			}
			this.Init();
			if (this.ParentWindow != null)
			{
				if (FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptPlayEvent -= this.ParentWindow_ScriptPlayEvent;
					this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptStopEvent -= this.MacroPlayControl_ScriptStopEvent;
					this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptPlayEvent += this.ParentWindow_ScriptPlayEvent;
					this.ParentWindow.mNCTopBar.mMacroPlayControl.ScriptStopEvent += this.MacroPlayControl_ScriptStopEvent;
					return;
				}
				this.ParentWindow.mTopBar.mMacroPlayControl.ScriptPlayEvent -= this.ParentWindow_ScriptPlayEvent;
				this.ParentWindow.mTopBar.mMacroPlayControl.ScriptStopEvent -= this.MacroPlayControl_ScriptStopEvent;
				this.ParentWindow.mTopBar.mMacroPlayControl.ScriptPlayEvent += this.ParentWindow_ScriptPlayEvent;
				this.ParentWindow.mTopBar.mMacroPlayControl.ScriptStopEvent += this.MacroPlayControl_ScriptStopEvent;
			}
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x0003B110 File Offset: 0x00039310
		public void ShowAtCenter()
		{
			base.Show();
			RECT rect;
			NativeMethods.GetWindowRect((base.Owner as MainWindow).Handle, out rect);
			RECT rect2;
			NativeMethods.GetWindowRect(new WindowInteropHelper(this).Handle, out rect2);
			RECT rect3 = new RECT
			{
				Left = (rect.Right - rect.Left - rect2.Right + rect2.Left) / 2 + rect.Left,
				Top = (rect.Bottom - rect.Top - rect2.Bottom + rect2.Top) / 2 + rect.Top
			};
			rect3.Right = rect3.Left + rect2.Right - rect2.Left;
			rect3.Bottom = rect3.Top + rect2.Bottom - rect2.Top;
			WindowPlacement.SetPlacement(new WindowInteropHelper(this).Handle, rect3);
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0003B208 File Offset: 0x00039408
		private void ParentWindow_MacroSettingChangedEvent(MacroRecording record)
		{
			if (record.PlayOnStart)
			{
				foreach (object obj in this.mScriptsStackPanel.Children)
				{
					SingleMacroControl singleMacroControl = (SingleMacroControl)obj;
					if (singleMacroControl.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == record.Name.ToLower(CultureInfo.InvariantCulture).Trim())
					{
						this.ChangeAutorunImageVisibility(singleMacroControl.mAutorunImage, Visibility.Visible);
					}
					else
					{
						this.ChangeAutorunImageVisibility(singleMacroControl.mAutorunImage, Visibility.Hidden);
						if (singleMacroControl.mRecording.PlayOnStart)
						{
							singleMacroControl.mRecording.PlayOnStart = false;
							if (singleMacroControl.mMacroSettingsWindow != null)
							{
								singleMacroControl.mMacroSettingsWindow.mPlayOnStartCheckBox.IsChecked = new bool?(false);
							}
							JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
							serializerSettings.Formatting = Formatting.Indented;
							string text = JsonConvert.SerializeObject(singleMacroControl.mRecording, serializerSettings);
							File.WriteAllText(CommonHandlers.GetCompleteMacroRecordingPath(singleMacroControl.mRecording.Name), text);
						}
					}
				}
			}
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0003B32C File Offset: 0x0003952C
		private void ChangeAutorunImageVisibility(CustomPictureBox cpb, Visibility visibility)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				cpb.Visibility = visibility;
			}), new object[0]);
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x0003B36C File Offset: 0x0003956C
		private void MacroPlayControl_ScriptStopEvent(string tag)
		{
			SingleMacroControl controlFromTag = this.GetControlFromTag(tag);
			if (controlFromTag != null)
			{
				controlFromTag.ToggleScriptPlayPauseUi(false);
			}
			if ((controlFromTag == null || !controlFromTag.mRecording.DonotShowWindowOnFinish) && !this.ParentWindow.IsClosed)
			{
				this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
			}
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x0003B3B8 File Offset: 0x000395B8
		private void ParentWindow_ScriptPlayEvent(string tag)
		{
			SingleMacroControl controlFromTag = this.GetControlFromTag(tag);
			if (controlFromTag != null)
			{
				controlFromTag.ToggleScriptPlayPauseUi(true);
			}
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x0003B3D8 File Offset: 0x000395D8
		public void Init()
		{
			this.ParentWindow.mIsScriptsPresent = false;
			this.mAlternateBackgroundColor = false;
			this.AddScriptsToStackPanel();
			if (!this.ParentWindow.mIsScriptsPresent)
			{
				this.mNoScriptsGrid.Visibility = Visibility.Visible;
				this.mExport.IsEnabled = false;
				this.mExport.Opacity = 0.4;
			}
			else
			{
				this.mNoScriptsGrid.Visibility = Visibility.Collapsed;
				this.mExport.IsEnabled = true;
				this.mExport.Opacity = 1.0;
			}
			if (this.ParentWindow.mIsMacroRecorderActive)
			{
				this.mStartMacroRecordingBtn.Visibility = Visibility.Collapsed;
				this.mStopMacroRecordingBtn.Visibility = Visibility.Visible;
			}
			else
			{
				this.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
				this.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
			}
			this.ToggleUI(this.ParentWindow.mIsMacroRecorderActive);
			if (this.ParentWindow.mIsMacroPlaying)
			{
				this.mStartMacroRecordingBtn.Visibility = Visibility.Hidden;
				this.mStopMacroRecordingBtn.Visibility = Visibility.Hidden;
			}
			this.ShowLoadingGrid(false);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x0003B4E4 File Offset: 0x000396E4
		private void AddScriptsToStackPanel()
		{
			foreach (MacroRecording macroRecording in from MacroRecording macro in MacroGraph.Instance.Vertices
				orderby DateTime.ParseExact(macro.TimeCreated, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
				select macro)
			{
				if (macroRecording != null && !string.IsNullOrEmpty(macroRecording.Name) && !string.IsNullOrEmpty(macroRecording.TimeCreated))
				{
					if (macroRecording.Events == null)
					{
						ObservableCollection<MergedMacroConfiguration> mergedMacroConfigurations = macroRecording.MergedMacroConfigurations;
						if (mergedMacroConfigurations == null || mergedMacroConfigurations.Count <= 0)
						{
							continue;
						}
					}
					this.ParentWindow.mIsScriptsPresent = true;
					SingleMacroControl singleMacroControl = new SingleMacroControl(this.ParentWindow, macroRecording, this)
					{
						Tag = macroRecording.Name
					};
					if (this.ParentWindow.mIsMacroPlaying && !string.Equals(this.ParentWindow.mMacroPlaying, macroRecording.Name, StringComparison.InvariantCulture))
					{
						CommonHandlers.DisableScriptControl(singleMacroControl);
					}
					else if (this.ParentWindow.mIsMacroPlaying)
					{
						singleMacroControl.mEditNameImg.IsEnabled = false;
					}
					else if (this.ParentWindow.mIsMacroRecorderActive)
					{
						CommonHandlers.DisableScriptControl(singleMacroControl);
					}
					if (macroRecording.PlayOnStart)
					{
						singleMacroControl.mAutorunImage.Visibility = Visibility.Visible;
					}
					if (this.mAlternateBackgroundColor)
					{
						BlueStacksUIBinding.BindColor(singleMacroControl, global::System.Windows.Controls.Control.BackgroundProperty, "DarkBandingColor");
					}
					else
					{
						BlueStacksUIBinding.BindColor(singleMacroControl, global::System.Windows.Controls.Control.BackgroundProperty, "LightBandingColor");
					}
					this.mAlternateBackgroundColor = !this.mAlternateBackgroundColor;
					this.mScriptsStackPanel.Children.Add(singleMacroControl);
				}
			}
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x0003B694 File Offset: 0x00039894
		private SingleMacroControl GetControlFromTag(string tag)
		{
			foreach (object obj in this.mScriptsStackPanel.Children)
			{
				SingleMacroControl singleMacroControl = (SingleMacroControl)obj;
				if ((string)singleMacroControl.Tag == tag)
				{
					return singleMacroControl;
				}
			}
			return null;
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0003B708 File Offset: 0x00039908
		private void OpenScriptFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
			{
				using (Process process = new Process())
				{
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.FileName = RegistryStrings.MacroRecordingsFolderPath;
					process.Start();
				}
			}
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x00008B93 File Offset: 0x00006D93
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x0003B768 File Offset: 0x00039968
		private void mStartMacroRecordingBtn_Click(object sender, RoutedEventArgs e)
		{
			this.ParentWindow.mCommonHandler.StartMacroRecording();
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "new_macro_record", null, RecordingTypes.SingleRecording.ToString(), null, null, null);
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0003B7BC File Offset: 0x000399BC
		private void mStopMacroRecordingBtn_Click(object sender, RoutedEventArgs e)
		{
			this.ParentWindow.mCommonHandler.StopMacroRecording();
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_stop", null, RecordingTypes.SingleRecording.ToString(), null, null, null);
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x00008BA5 File Offset: 0x00006DA5
		internal void PerformStopMacroAfterSave()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.mTopBar.HideRecordingIcons();
				this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
				this.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
				this.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
				this.ParentWindow.mIsMacroRecorderActive = false;
			}), new object[0]);
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x0003B810 File Offset: 0x00039A10
		internal void SaveOperation(string events)
		{
			try
			{
				if (!string.Equals(events, "[]", StringComparison.InvariantCulture))
				{
					string macroRecordingsFolderPath = RegistryStrings.MacroRecordingsFolderPath;
					MacroRecording macroRecording = new MacroRecording();
					string text = DateTime.Now.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
					macroRecording.TimeCreated = text;
					macroRecording.Name = CommonHandlers.GetMacroName("Macro");
					macroRecording.Events = JsonConvert.DeserializeObject<List<MacroEvents>>(events, Utils.GetSerializerSettings());
					this.SaveMacroRecord(macroRecording);
				}
				else
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_OPERATION_MESSAGE", ""), 4.0, true);
				}
				this.PerformStopMacroAfterSave();
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in SaveOperations. Exception: " + ex.ToString());
			}
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x0003B8DC File Offset: 0x00039ADC
		internal void SaveMacroRecord(MacroRecording record)
		{
			CommonHandlers.SaveMacroJson(record, record.Name + ".json");
			MacroGraph.Instance.AddVertex(record);
			MacroGraph.LinkMacroChilds(record);
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.mIsMacroRecorderActive = false;
				foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
				{
					if (keyValuePair.Value.MacroRecorderWindow != null)
					{
						new SingleMacroControl(keyValuePair.Value, record, this).Tag = record.Name;
						keyValuePair.Value.MacroRecorderWindow.mNoScriptsGrid.Visibility = Visibility.Collapsed;
						this.mExport.IsEnabled = true;
						this.mExport.Opacity = 1.0;
						if (!keyValuePair.Value.mIsScriptsPresent)
						{
							keyValuePair.Value.mIsScriptsPresent = true;
						}
						keyValuePair.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
						keyValuePair.Value.MacroRecorderWindow.Init();
						keyValuePair.Value.MacroRecorderWindow.mScriptsListScrollbar.ScrollToEnd();
						int num = keyValuePair.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Count - 1;
						SingleMacroControl singleMacroControl = keyValuePair.Value.MacroRecorderWindow.mScriptsStackPanel.Children[num] as SingleMacroControl;
						BlueStacksUIBinding.BindColor(singleMacroControl.mGrid, global::System.Windows.Controls.Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
						BlueStacksUIBinding.BindColor(singleMacroControl.mScriptName, TextBlock.ForegroundProperty, "WhiteMouseOutBorderBackground");
						BlueStacksUIBinding.BindColor(singleMacroControl.mMacroShortcutTextBox, TextBlock.ForegroundProperty, "DualTextBlockForeground");
					}
				}
			}), new object[0]);
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x0002CE5C File Offset: 0x0002B05C
		private void Topbar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!e.OriginalSource.GetType().Equals(typeof(CustomPictureBox)))
			{
				try
				{
					base.DragMove();
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x0003B958 File Offset: 0x00039B58
		internal void ToggleUI(bool isRecording)
		{
			if (isRecording)
			{
				this.mStopMacroRecordingBtn.Visibility = Visibility.Visible;
				this.mStartMacroRecordingBtn.Visibility = Visibility.Collapsed;
				this.mNoScriptsGrid.Visibility = Visibility.Collapsed;
				this.mScriptsGrid.Visibility = Visibility.Visible;
				return;
			}
			this.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
			this.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
			if (this.ParentWindow.mIsScriptsPresent)
			{
				this.mNoScriptsGrid.Visibility = Visibility.Collapsed;
				this.mScriptsGrid.Visibility = Visibility.Visible;
				return;
			}
			this.mNoScriptsGrid.Visibility = Visibility.Visible;
			this.mScriptsGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x0003B9F0 File Offset: 0x00039BF0
		private void ShowLoadingGrid(bool isShow)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				if (isShow)
				{
					this.mLoadingGrid.Visibility = Visibility.Visible;
					return;
				}
				this.mLoadingGrid.Visibility = Visibility.Collapsed;
			}), new object[0]);
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x0003BA30 File Offset: 0x00039C30
		private void ExportBtn_Click(object sender, MouseButtonEventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_export", null, null, null, null, null);
			if (this.ParentWindow.mIsScriptsPresent)
			{
				this.mOverlayGrid.Visibility = Visibility.Visible;
				if (this.mExportMacroWindow == null)
				{
					this.mExportMacroWindow = new ExportMacroWindow(this, this.ParentWindow)
					{
						Owner = this
					};
					this.mExportMacroWindow.Init();
					this.mExportMacroWindow.ShowDialog();
					return;
				}
			}
			else
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_AVAILABLE", ""), 4.0, true);
			}
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x0003BAE4 File Offset: 0x00039CE4
		private void MergeMacroBtn_Click(object sender, MouseButtonEventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_icon", null, null, null, null, null);
			if (this.ParentWindow.mIsScriptsPresent)
			{
				this.mOverlayGrid.Visibility = Visibility.Visible;
				if (this.mMergeMacroWindow == null)
				{
					this.mMergeMacroWindow = new MergeMacroWindow(this, this.ParentWindow)
					{
						Owner = this.ParentWindow
					};
					this.mMergeMacroWindow.Init(null, null);
					this.mMergeMacroWindow.Show();
					return;
				}
			}
			else
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_MACRO_AVAILABLE", ""), 4.0, true);
			}
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x0003BB9C File Offset: 0x00039D9C
		private void ImportBtn_Click(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (this.ParentWindow.mIsMacroPlaying)
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_IMPORT_MACRO_WARNING", "");
					customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_WARNING", "");
					customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_STOP_IMPORT", ""), delegate(object o, EventArgs e)
					{
						this.ParentWindow.mTopBar.mMacroPlayControl.StopMacro();
						this.ImportMacro();
					}, null, false, null);
					customMessageWindow.Owner = this;
					customMessageWindow.ShowDialog();
				}
				else
				{
					this.ImportMacro();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Importing file. err: " + ex.ToString());
				this.ShowLoadingGrid(false);
			}
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x0003BC5C File Offset: 0x00039E5C
		private void ImportMacro()
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_import", null, null, null, null, null);
			using (OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = true,
				Filter = "Json files (*.json)|*.json"
			})
			{
				if (openFileDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK && openFileDialog.FileNames.Length != 0)
				{
					if (string.Equals(Path.GetDirectoryName(openFileDialog.FileNames[0]), RegistryStrings.MacroRecordingsFolderPath, StringComparison.InvariantCultureIgnoreCase))
					{
						CustomMessageWindow customMessageWindow = new CustomMessageWindow();
						customMessageWindow.Owner = this;
						BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SAME_MACRO_EXISTS", "");
						customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", delegate(object o, EventArgs evt)
						{
						}, null, false, null);
						customMessageWindow.ShowDialog();
					}
					else
					{
						this.RunImportMacroScriptBacgroundWorker(openFileDialog.FileNames.ToList<string>());
					}
				}
			}
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x0003BD60 File Offset: 0x00039F60
		internal void RunImportMacroScriptBacgroundWorker(List<string> fileNames)
		{
			using (BackgroundWorker backgroundWorker = new BackgroundWorker())
			{
				backgroundWorker.DoWork += this.BgImport_DoWork;
				backgroundWorker.RunWorkerCompleted += this.BgImport_RunWorkerCompleted;
				this.ShowLoadingGrid(true);
				backgroundWorker.RunWorkerAsync(fileNames);
			}
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x0003BDC4 File Offset: 0x00039FC4
		private void BgImport_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				e.Result = this.CopyMacroScriptIfFileFormatSupported(e.Argument as List<string>);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in Importing file. err: " + ex.ToString());
				e.Result = true;
			}
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00008BC5 File Offset: 0x00006DC5
		private void BgImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.ValidateReturnCode((int)e.Result);
			if ((int)e.Result == 0)
			{
				CommonHandlers.RefreshAllMacroWindowWithScroll();
			}
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x0003BE24 File Offset: 0x0003A024
		internal void ValidateReturnCode(int retCode)
		{
			this.ShowLoadingGrid(false);
			if (retCode == 2)
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_FILE_FORMAT_NOT_SUPPORTED", ""), 4.0, true);
				return;
			}
			if (retCode == 3)
			{
				this.ShowMacroImportWizard();
				if (this.mNewlyAddedMacrosList.Count > 0)
				{
					CommonHandlers.RefreshAllMacroWindowWithScroll();
					this.ShowMacroImportSuccessPopup();
					return;
				}
			}
			else
			{
				if (retCode == 1)
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_IMPORTING_CANCELLED", ""), 4.0, true);
					return;
				}
				if (retCode == 0)
				{
					if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
					{
						Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
					}
					this.ShowMacroImportSuccessPopup();
				}
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0003BEDC File Offset: 0x0003A0DC
		internal int CopyMacroScriptIfFileFormatSupported(List<string> selectedFileNames)
		{
			int num2;
			try
			{
				this.mRenamingMacrosList.Clear();
				bool flag = false;
				List<string> list = new List<string>();
				List<MacroRecording> list2 = new List<MacroRecording>();
				foreach (string text in selectedFileNames)
				{
					MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(File.ReadAllText(text), Utils.GetSerializerSettings());
					if (macroRecording == null || string.IsNullOrEmpty(macroRecording.Name) || string.IsNullOrEmpty(macroRecording.TimeCreated) || (macroRecording.RecordingType == RecordingTypes.MultiRecording && (macroRecording.SourceRecordings == null || !macroRecording.SourceRecordings.Any<string>())))
					{
						list.Add(Path.GetFileNameWithoutExtension(text));
					}
					else
					{
						list2.Add(macroRecording);
					}
				}
				if (list2.Any((MacroRecording x) => x.RecordingType == RecordingTypes.MultiRecording))
				{
					this.AskUserHowToImportMultiMacro();
					if (this.mImportMultiMacroAsUnified == null)
					{
						return 1;
					}
				}
				int num = this.ImportMacroRecordings(list2, ref flag);
				if (num != 0)
				{
					num2 = num;
				}
				else
				{
					if (list.Count > 0)
					{
						string text2 = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_INVALID_FILES_LIST", ""), new object[] { string.Join(", ", list.ToArray()) });
						this.ParentWindow.mCommonHandler.AddToastPopup(this, text2, 4.0, true);
						if (list2.Count <= 0)
						{
							return 4;
						}
					}
					if (flag)
					{
						num2 = 3;
					}
					else
					{
						num2 = 0;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Wrong file format wont import. err:" + ex.ToString());
				num2 = 2;
			}
			return num2;
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x0003C0B8 File Offset: 0x0003A2B8
		internal int ImportMacroRecordings(List<MacroRecording> recordingsToImport, ref bool isShowRenameWizard)
		{
			try
			{
				this.mNewlyAddedMacrosList.Clear();
				foreach (MacroRecording macroRecording in recordingsToImport)
				{
					macroRecording.Shortcut = string.Empty;
					macroRecording.PlayOnStart = false;
					if (MacroRecorderWindow.CheckIfDuplicateMacroInImport(macroRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
					{
						isShowRenameWizard = true;
						this.mRenamingMacrosList.Add(macroRecording);
					}
					if (macroRecording.RecordingType == RecordingTypes.MultiRecording)
					{
						bool flag = false;
						bool? flag2 = this.mImportMultiMacroAsUnified;
						if (((flag == flag2.GetValueOrDefault()) & (flag2 != null)) && !this.mRenamingMacrosList.Contains(macroRecording))
						{
							new List<string>();
							if (MacroRecorderWindow.CheckIfDuplicateMacroInImport(macroRecording.Name, macroRecording.MergedMacroConfigurations.SelectMany((MergedMacroConfiguration macro) => macro.MacrosToRun)))
							{
								isShowRenameWizard = true;
								this.mRenamingMacrosList.AddIfNotContain(macroRecording);
							}
						}
					}
					if (!this.mRenamingMacrosList.Contains(macroRecording))
					{
						macroRecording.Name = macroRecording.Name.Trim();
						if (macroRecording.RecordingType == RecordingTypes.SingleRecording)
						{
							MacroGraph.Instance.AddVertex(macroRecording);
							this.mNewlyAddedMacrosList.Add(macroRecording);
							CommonHandlers.SaveMacroJson(macroRecording, macroRecording.Name + ".json");
						}
						else
						{
							this.ImportMultiMacro(macroRecording, this.mImportMultiMacroAsUnified.Value, this.mNewlyAddedMacrosList, null);
						}
					}
				}
				foreach (MacroRecording macroRecording2 in this.mNewlyAddedMacrosList)
				{
					MacroGraph.LinkMacroChilds(macroRecording2);
				}
			}
			catch
			{
				throw;
			}
			return 0;
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00008BEA File Offset: 0x00006DEA
		private void AskUserHowToImportMultiMacro()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_IMPORT_DEPENDENT_MACRO", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_IMPORT_ALL_MACROS", delegate(object o, EventArgs evt)
				{
					this.mImportMultiMacroAsUnified = new bool?(false);
					ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_all", null, null, null, null, null);
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_IMPORT_UNIFIED", delegate(object o, EventArgs evt)
				{
					this.mImportMultiMacroAsUnified = new bool?(true);
					ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_unify", null, null, null, null, null);
				}, null, false, null);
				BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_IMPORT_DEPENDENT_MACRO_UNIFIED", "");
				customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
				BlueStacksUIBinding.Bind(customMessageWindow.BodyWarningTextBlock, "STRING_UNIFIYING_LOSE_CONFIGURE", "");
				customMessageWindow.CloseButtonHandle(delegate(object o, EventArgs evt)
				{
					ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_import_cancel", null, null, null, null, null);
					this.mImportMultiMacroAsUnified = null;
				}, null);
				customMessageWindow.Owner = this;
				customMessageWindow.ShowDialog();
			}), new object[0]);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0003C2B0 File Offset: 0x0003A4B0
		private List<MacroEvents> GetFlattenedEventsFromMultiRecording(MacroRecording srcRecording, long initialTime, out long elapsedTime, bool isExternalMacro = false)
		{
			List<MacroEvents> list = new List<MacroEvents>();
			elapsedTime = initialTime;
			List<MacroRecording> list2 = MacroGraph.Instance.Vertices.Cast<MacroRecording>().ToList<MacroRecording>();
			if (isExternalMacro)
			{
				list2.Clear();
				foreach (string text in srcRecording.SourceRecordings)
				{
					MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(text, Utils.GetSerializerSettings());
					list2.Add(macroRecording);
				}
			}
			try
			{
				foreach (MergedMacroConfiguration mergedMacroConfiguration in srcRecording.MergedMacroConfigurations)
				{
					for (int i = 0; i < mergedMacroConfiguration.LoopCount; i++)
					{
						using (IEnumerator<string> enumerator3 = mergedMacroConfiguration.MacrosToRun.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								string gMacro = enumerator3.Current;
								MacroRecording macroRecording2 = list2.Where((MacroRecording macro) => string.Equals(gMacro, macro.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault<MacroRecording>();
								if (macroRecording2.RecordingType == RecordingTypes.SingleRecording)
								{
									list.AddRange(MacroRecorderWindow.GetRecordingEventsFromSourceRecording(macroRecording2, mergedMacroConfiguration.Acceleration, elapsedTime, ref elapsedTime));
								}
								else
								{
									list.AddRange(this.GetFlattenedEventsFromMultiRecording(macroRecording2, elapsedTime, out elapsedTime, false));
								}
								elapsedTime += (long)(mergedMacroConfiguration.LoopInterval * 1000);
							}
						}
						elapsedTime += (long)(mergedMacroConfiguration.DelayNextScript * 1000);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Couldn't get flattened events. Ex: {0}", new object[] { ex });
			}
			return list;
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0003C4A8 File Offset: 0x0003A6A8
		internal void FlattenRecording(MacroRecording srcRecording, bool isExternalMacro = false)
		{
			Logger.Info("Will attempt to flatten {0}", new object[] { srcRecording.Name });
			long num;
			srcRecording.Events = this.GetFlattenedEventsFromMultiRecording(srcRecording, 0L, out num, isExternalMacro);
			srcRecording.SourceRecordings = null;
			srcRecording.MergedMacroConfigurations = null;
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x0003C4F0 File Offset: 0x0003A6F0
		private static MacroRecording GetFixedMultiMacroSourceRecording(MacroRecording record, string baseChildRecordingName)
		{
			try
			{
				MacroRecording macroRecording = record.DeepCopy<MacroRecording>();
				macroRecording.Name = MacroRecorderWindow.GetDependentRecordingName(baseChildRecordingName, macroRecording.Name);
				macroRecording.MergedMacroConfigurations.Clear();
				foreach (MergedMacroConfiguration mergedMacroConfiguration in record.MergedMacroConfigurations)
				{
					MergedMacroConfiguration mergedMacroConfiguration2 = mergedMacroConfiguration.DeepCopy<MergedMacroConfiguration>();
					mergedMacroConfiguration2.MacrosToRun.Clear();
					foreach (string text in mergedMacroConfiguration.MacrosToRun)
					{
						mergedMacroConfiguration2.MacrosToRun.Add(MacroRecorderWindow.GetDependentRecordingName(baseChildRecordingName, text));
					}
					macroRecording.MergedMacroConfigurations.Add(mergedMacroConfiguration2);
				}
				record = macroRecording;
				record.SourceRecordings = null;
			}
			catch (Exception ex)
			{
				Logger.Error("Some error occured while fixing dependent source multi macro: Ex: {0}", new object[] { ex });
			}
			return record;
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x0003C5F4 File Offset: 0x0003A7F4
		internal void ImportMultiMacro(MacroRecording record, bool flatten, List<MacroRecording> newlyAddedMacro, Dictionary<string, string> dependentMacroNewNamesDict = null)
		{
			if (flatten)
			{
				this.FlattenRecording(record, true);
				if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(record.Name))
				{
					record.Name = dependentMacroNewNamesDict[record.Name];
				}
			}
			else
			{
				try
				{
					MacroRecording macroRecording = record.DeepCopy<MacroRecording>();
					string text = record.Name;
					if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(record.Name))
					{
						text = dependentMacroNewNamesDict[record.Name];
						macroRecording.Name = text;
					}
					foreach (string text2 in record.SourceRecordings)
					{
						MacroRecording macroRecording2 = JsonConvert.DeserializeObject<MacroRecording>(text2, Utils.GetSerializerSettings());
						if (macroRecording2.RecordingType == RecordingTypes.MultiRecording)
						{
							macroRecording2 = MacroRecorderWindow.GetFixedMultiMacroSourceRecording(macroRecording2, text);
						}
						else
						{
							string name = macroRecording2.Name;
							string text3 = MacroRecorderWindow.GetDependentRecordingName(text, name);
							if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(name))
							{
								text3 = dependentMacroNewNamesDict[name];
							}
							macroRecording2.Name = text3;
						}
						MacroGraph.Instance.AddVertex(macroRecording2);
						newlyAddedMacro.Add(macroRecording2);
						CommonHandlers.SaveMacroJson(macroRecording2, macroRecording2.Name + ".json");
					}
					macroRecording.MergedMacroConfigurations.Clear();
					foreach (MergedMacroConfiguration mergedMacroConfiguration in record.MergedMacroConfigurations)
					{
						MergedMacroConfiguration mergedMacroConfiguration2 = mergedMacroConfiguration.DeepCopy<MergedMacroConfiguration>();
						mergedMacroConfiguration2.MacrosToRun.Clear();
						new ObservableCollection<string>();
						foreach (string text4 in mergedMacroConfiguration.MacrosToRun)
						{
							string text5 = MacroRecorderWindow.GetDependentRecordingName(text, text4);
							if (dependentMacroNewNamesDict != null && dependentMacroNewNamesDict.ContainsKey(text4))
							{
								text5 = dependentMacroNewNamesDict[text4];
							}
							mergedMacroConfiguration2.MacrosToRun.Add(text5);
						}
						macroRecording.MergedMacroConfigurations.Add(mergedMacroConfiguration2);
					}
					record = macroRecording;
				}
				catch (Exception ex)
				{
					Logger.Error("Some error occured: Ex: {0}", new object[] { ex });
				}
				record.SourceRecordings = null;
			}
			MacroGraph.Instance.AddVertex(record);
			newlyAddedMacro.Add(record);
			CommonHandlers.SaveMacroJson(record, record.Name + ".json");
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0003C898 File Offset: 0x0003AA98
		private static List<MacroEvents> GetRecordingEventsFromSourceRecording(MacroRecording srcRecording, double acceleration, long initialTime, ref long elapsedTime)
		{
			if (srcRecording == null)
			{
				throw new Exception("Source recording now found in multiMacro");
			}
			List<MacroEvents> list = new List<MacroEvents>();
			foreach (MacroEvents macroEvents in srcRecording.Events)
			{
				MacroEvents macroEvents2 = macroEvents.DeepCopy<MacroEvents>();
				macroEvents2.Timestamp = (long)Math.Floor((double)macroEvents.Timestamp / acceleration);
				macroEvents2.Timestamp += initialTime;
				elapsedTime = macroEvents2.Timestamp;
				list.Add(macroEvents2);
			}
			return list;
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x0003C934 File Offset: 0x0003AB34
		private static bool CheckIfDuplicateMacroInImport(string origMacroName, IEnumerable<string> lsMacros)
		{
			foreach (string text in lsMacros)
			{
				string dependentRecordingName = MacroRecorderWindow.GetDependentRecordingName(text, origMacroName);
				if ((from MacroRecording macro in MacroGraph.Instance.Vertices
					select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(dependentRecordingName.ToLower(CultureInfo.InvariantCulture)))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x00008C0A File Offset: 0x00006E0A
		internal static string GetDependentRecordingName(string originalMacroName, string dependentMacroName)
		{
			return originalMacroName + "-" + dependentMacroName;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x0003C9C8 File Offset: 0x0003ABC8
		private static bool CheckIfDuplicateMacroInImport(string macroName)
		{
			return (from MacroRecording macro in MacroGraph.Instance.Vertices
				select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(macroName.ToLower(CultureInfo.InvariantCulture));
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00008C18 File Offset: 0x00006E18
		private void ShowMacroImportWizard()
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mOverlayGrid.Visibility = Visibility.Visible;
				if (this.mImportMacroWindow == null)
				{
					this.mImportMacroWindow = new ImportMacroWindow(this, this.ParentWindow)
					{
						Owner = this
					};
					this.mImportMacroWindow.Init();
					this.mImportMacroWindow.ShowDialog();
				}
			}), new object[0]);
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x00008C38 File Offset: 0x00006E38
		private void MImportBtn_Click(object sender, RoutedEventArgs e)
		{
			CommonHandlers.RefreshAllMacroWindowWithScroll();
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x0003CA20 File Offset: 0x0003AC20
		private void mBGMacroPlaybackWorker_DoWork(BackgroundWorker bg, MacroRecording record)
		{
			if (File.Exists(CommonHandlers.GetCompleteMacroRecordingPath(record.Name)) && !this.ParentWindow.mIsMacroPlaying)
			{
				Logger.Debug("Macro Playback started");
				this.ParentWindow.mIsMacroPlaying = true;
				this.ParentWindow.mFrontendHandler.SendFrontendRequest("initMacroPlayback", new Dictionary<string, string> { 
				{
					"scriptFilePath",
					CommonHandlers.GetCompleteMacroRecordingPath(record.Name)
				} });
				switch (record.LoopType)
				{
				case OperationsLoopType.TillLoopNumber:
					this.HandleMacroPlaybackTillLoopNumber(bg, record);
					return;
				case OperationsLoopType.TillTime:
					this.HandleMacroPlaybackTillTime(bg, record);
					return;
				case OperationsLoopType.UntilStopped:
					this.HandleMacroPlaybackUntillStopped(bg, record);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x0003CACC File Offset: 0x0003ACCC
		internal void RunMacroOperation(MacroRecording record)
		{
			BackgroundWorker bg = new BackgroundWorker
			{
				WorkerSupportsCancellation = true
			};
			bg.DoWork += delegate(object obj, DoWorkEventArgs e)
			{
				this.mBGMacroPlaybackWorker_DoWork(bg, record);
			};
			this.mBGMacroPlaybackWorker = bg;
			bg.RunWorkerAsync();
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x0003CB30 File Offset: 0x0003AD30
		private void HandleMacroPlaybackUntillStopped(BackgroundWorker bg, MacroRecording record)
		{
			try
			{
				EventWaitHandle eventWaitHandle = null;
				string macroPlaybackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(this.ParentWindow.mVmName);
				this.ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
				int num = 1;
				this.UpdateMacroPlayBackUI(num, record);
				while (this.ParentWindow.mIsMacroPlaying && !bg.CancellationPending)
				{
					this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit", null);
					if (eventWaitHandle == null)
					{
						eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, macroPlaybackEventName);
					}
					this.UpdateMacroPlayBackUI(num, record);
					num++;
					eventWaitHandle.WaitOne();
					Thread.Sleep(record.LoopInterval * 1000);
				}
				eventWaitHandle.Close();
			}
			catch (Exception ex)
			{
				Logger.Error("Error in macroplaybackuntil stopped. err:" + ex.ToString());
			}
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x0003CBF8 File Offset: 0x0003ADF8
		private void HandleMacroPlaybackTillTime(BackgroundWorker bg, MacroRecording record)
		{
			try
			{
				if (record.LoopTime > 0)
				{
					EventWaitHandle eventWaitHandle = null;
					string macroPlaybackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(this.ParentWindow.mVmName);
					this.mMacroLoopTimer = new global::System.Timers.Timer((double)record.LoopTime)
					{
						Interval = (double)(record.LoopTime * 1000)
					};
					this.mMacroLoopTimer.Elapsed += delegate(object sender, ElapsedEventArgs e)
					{
						this.MacroLoopTimer_Elapsed(record.Name);
					};
					DateTime now = DateTime.Now;
					TimeSpan timeSpan = DateTime.Now - now;
					this.ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
					this.mMacroLoopTimer.Enabled = true;
					int num = 1;
					this.UpdateMacroPlayBackUI(num, record);
					while (timeSpan.TotalSeconds < (double)record.LoopTime && this.ParentWindow.mIsMacroPlaying && !bg.CancellationPending)
					{
						this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit", null);
						if (eventWaitHandle == null)
						{
							eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, macroPlaybackEventName);
						}
						eventWaitHandle.WaitOne();
						Thread.Sleep(record.LoopInterval * 1000);
						timeSpan = DateTime.Now - now;
					}
					eventWaitHandle.Close();
				}
				else
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_TIMER_SET", ""), 4.0, true);
					Logger.Debug("Macro timer set to ZERO");
					this.SendStopMacroEventsAndUpdateUi(record.Name);
				}
				if (this.mMacroLoopTimer != null)
				{
					this.mMacroLoopTimer.Enabled = false;
					this.mMacroLoopTimer = null;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in MacroPlaybacktillTime. err:" + ex.ToString());
			}
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00008C3F File Offset: 0x00006E3F
		private void MacroLoopTimer_Elapsed(string fileName)
		{
			Logger.Debug("Macro timer finished");
			this.SendStopMacroEventsAndUpdateUi(fileName);
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x0003CDE4 File Offset: 0x0003AFE4
		private void SendStopMacroEventsAndUpdateUi(string fileName)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				this.ParentWindow.mCommonHandler.StopMacroScriptHandling();
				if (FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					this.ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptStopEvent(fileName);
					return;
				}
				this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptStopEvent(fileName);
			}), new object[0]);
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x0003CE28 File Offset: 0x0003B028
		private void HandleMacroPlaybackTillLoopNumber(BackgroundWorker bg, MacroRecording record)
		{
			try
			{
				string macroPlaybackEventName = BlueStacksUIUtils.GetMacroPlaybackEventName(this.ParentWindow.mVmName);
				if (record.LoopNumber >= 1)
				{
					this.ParentWindow.mCommonHandler.InitUiOnMacroPlayback(record);
				}
				else
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_LOOP_ITERATION_SET", ""), 4.0, true);
					Logger.Debug("Macro loop iterations set to ZERO");
				}
				EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, macroPlaybackEventName);
				int num = 1;
				while (num <= record.LoopNumber && this.ParentWindow.mIsMacroPlaying && !bg.CancellationPending)
				{
					this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("runMacroUnit", null);
					this.UpdateMacroPlayBackUI(num, record);
					eventWaitHandle.WaitOne();
					if (num != record.LoopNumber)
					{
						Thread.Sleep(record.LoopInterval * 1000);
					}
					num++;
				}
				eventWaitHandle.Close();
				if (!bg.CancellationPending && this.ParentWindow.mIsMacroPlaying)
				{
					this.ParentWindow.Dispatcher.Invoke(new Action(delegate
					{
						this.ParentWindow.mCommonHandler.StopMacroPlaybackOperation();
						if (FeatureManager.Instance.IsCustomUIForNCSoft)
						{
							this.ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptStopEvent(record.Name);
							return;
						}
						this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptStopEvent(record.Name);
					}), new object[0]);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception err: " + ex.ToString());
			}
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0003CFAC File Offset: 0x0003B1AC
		private void UpdateMacroPlayBackUI(int i, MacroRecording record)
		{
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (record.LoopType == OperationsLoopType.TillLoopNumber)
				{
					if (FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						this.ParentWindow.mNCTopBar.mMacroPlayControl.mTimerDisplay.Visibility = Visibility.Collapsed;
						this.ParentWindow.mNCTopBar.mMacroPlayControl.mRunningIterations.Visibility = Visibility.Visible;
						this.ParentWindow.mNCTopBar.mMacroPlayControl.IncreaseIteration(i);
						return;
					}
					this.ParentWindow.mTopBar.mMacroPlayControl.mTimerDisplay.Visibility = Visibility.Collapsed;
					this.ParentWindow.mTopBar.mMacroPlayControl.mRunningIterations.Visibility = Visibility.Visible;
					this.ParentWindow.mTopBar.mMacroPlayControl.IncreaseIteration(i);
					return;
				}
				else if (record.LoopType == OperationsLoopType.TillTime)
				{
					if (FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						this.ParentWindow.mNCTopBar.mMacroPlayControl.UpdateUiForIterationTillTime();
						return;
					}
					this.ParentWindow.mTopBar.mMacroPlayControl.UpdateUiForIterationTillTime();
					return;
				}
				else
				{
					if (FeatureManager.Instance.IsCustomUIForNCSoft)
					{
						this.ParentWindow.mNCTopBar.mMacroPlayControl.UpdateUiMacroPlaybackForInfiniteTime(i);
						return;
					}
					this.ParentWindow.mTopBar.mMacroPlayControl.UpdateUiMacroPlaybackForInfiniteTime(i);
					return;
				}
			}), new object[0]);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0003CFF8 File Offset: 0x0003B1F8
		private void OpenFolder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
				{
					Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
				}
				using (Process process = new Process())
				{
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.FileName = RegistryStrings.MacroRecordingsFolderPath;
					process.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error in Open folder err: " + ex.ToString());
			}
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00008C52 File Offset: 0x00006E52
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				BackgroundWorker backgroundWorker = this.mBGMacroPlaybackWorker;
				if (backgroundWorker != null)
				{
					backgroundWorker.Dispose();
				}
				global::System.Timers.Timer timer = this.mMacroLoopTimer;
				if (timer != null)
				{
					timer.Dispose();
				}
				this.disposedValue = true;
			}
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0003D088 File Offset: 0x0003B288
		~MacroRecorderWindow()
		{
			this.Dispose(false);
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x00008C87 File Offset: 0x00006E87
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0003D0B8 File Offset: 0x0003B2B8
		private void OpenCommunityBtn_Click(object sender, RoutedEventArgs e)
		{
			if (string.Equals((sender as CustomButton).Name, "mOpenCommunityBtn", StringComparison.InvariantCultureIgnoreCase))
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_community_opened", "Open_Community_Button", null, null, null, null);
			}
			else
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_community_opened", "Get_Macro_Button", null, null, null, null);
			}
			AppTabButton selectedTab = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
			this.OpenCommunityAndCloseMacroWindow(BlueStacksUIUtils.GetMacroCommunityUrl((selectedTab != null) ? selectedTab.PackageName : null));
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0003D164 File Offset: 0x0003B364
		internal void OpenCommunityAndCloseMacroWindow(string url)
		{
			AppTabButtons mAppTabButtons = this.ParentWindow.mTopBar.mAppTabButtons;
			string text = url;
			if (url == null)
			{
				AppTabButton selectedTab = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab;
				text = BlueStacksUIUtils.GetMacroCommunityUrl((selectedTab != null) ? selectedTab.PackageName : null);
			}
			mAppTabButtons.AddWebTab(text, "STRING_MACRO_COMMUNITY", "community_big", true, "STRING_MACRO_COMMUNITY", false);
			this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
			this.ParentWindow.Focus();
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x00008C96 File Offset: 0x00006E96
		internal void ShowMacroImportSuccessPopup()
		{
			this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_MACRO_IMPORT_SUCCESS", ""), 2.0, true);
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x00008CC2 File Offset: 0x00006EC2
		private void CustomWindow_Closing(object sender, CancelEventArgs e)
		{
			base.Visibility = Visibility.Hidden;
			e.Cancel = true;
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x0003D1E0 File Offset: 0x0003B3E0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrorecorderwindow.xaml", UriKind.Relative);
			global::System.Windows.Application.LoadComponent(this, uri);
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0003D210 File Offset: 0x0003B410
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
				((MacroRecorderWindow)target).Closing += this.CustomWindow_Closing;
				return;
			case 2:
				this.mOperationRecorderBorder = (Border)target;
				return;
			case 3:
				this.mMaskBorder = (Border)target;
				return;
			case 4:
				((Grid)target).MouseDown += this.Topbar_MouseDown;
				return;
			case 5:
				((CustomPictureBox)target).MouseLeftButtonUp += this.Close_MouseLeftButtonUp;
				return;
			case 6:
				this.mMerge = (CustomPictureBox)target;
				this.mMerge.MouseLeftButtonUp += this.MergeMacroBtn_Click;
				return;
			case 7:
				this.mImport = (CustomPictureBox)target;
				this.mImport.MouseLeftButtonUp += this.ImportBtn_Click;
				return;
			case 8:
				this.mExport = (CustomPictureBox)target;
				this.mExport.MouseLeftButtonUp += this.ExportBtn_Click;
				return;
			case 9:
				this.mOpenFolder = (CustomPictureBox)target;
				this.mOpenFolder.MouseLeftButtonUp += this.OpenFolder_MouseLeftButtonUp;
				return;
			case 10:
				this.mStartMacroRecordingBtn = (CustomButton)target;
				this.mStartMacroRecordingBtn.Click += this.mStartMacroRecordingBtn_Click;
				return;
			case 11:
				this.mStopMacroRecordingBtn = (CustomButton)target;
				this.mStopMacroRecordingBtn.Click += this.mStopMacroRecordingBtn_Click;
				return;
			case 12:
				this.mGetMacroBtn = (CustomButton)target;
				this.mGetMacroBtn.Click += this.OpenCommunityBtn_Click;
				return;
			case 13:
				this.mNoScriptsGrid = (Border)target;
				return;
			case 14:
				this.mScriptsGrid = (Grid)target;
				return;
			case 15:
				this.mScriptsListScrollbar = (ScrollViewer)target;
				return;
			case 16:
				this.mOpenCommunityBtn = (CustomButton)target;
				this.mOpenCommunityBtn.Click += this.OpenCommunityBtn_Click;
				return;
			case 17:
				this.mLoadingGrid = (BlueStacks.BlueStacksUI.ProgressBar)target;
				return;
			case 18:
				this.mOverlayGrid = (Grid)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000641 RID: 1601
		private MainWindow ParentWindow;

		// Token: 0x04000642 RID: 1602
		internal string mMacroOnRestart;

		// Token: 0x04000643 RID: 1603
		internal StackPanel mScriptsStackPanel;

		// Token: 0x04000644 RID: 1604
		private global::System.Timers.Timer mMacroLoopTimer;

		// Token: 0x04000645 RID: 1605
		internal ExportMacroWindow mExportMacroWindow;

		// Token: 0x04000646 RID: 1606
		internal MergeMacroWindow mMergeMacroWindow;

		// Token: 0x04000647 RID: 1607
		internal ImportMacroWindow mImportMacroWindow;

		// Token: 0x04000648 RID: 1608
		internal List<MacroRecording> mRenamingMacrosList = new List<MacroRecording>();

		// Token: 0x04000649 RID: 1609
		internal List<MacroRecording> mNewlyAddedMacrosList = new List<MacroRecording>();

		// Token: 0x0400064A RID: 1610
		internal bool? mImportMultiMacroAsUnified;

		// Token: 0x0400064B RID: 1611
		private bool mAlternateBackgroundColor;

		// Token: 0x0400064C RID: 1612
		internal BackgroundWorker mBGMacroPlaybackWorker;

		// Token: 0x0400064D RID: 1613
		private bool disposedValue;

		// Token: 0x0400064E RID: 1614
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mOperationRecorderBorder;

		// Token: 0x0400064F RID: 1615
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000650 RID: 1616
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMerge;

		// Token: 0x04000651 RID: 1617
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mImport;

		// Token: 0x04000652 RID: 1618
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mExport;

		// Token: 0x04000653 RID: 1619
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mOpenFolder;

		// Token: 0x04000654 RID: 1620
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mStartMacroRecordingBtn;

		// Token: 0x04000655 RID: 1621
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mStopMacroRecordingBtn;

		// Token: 0x04000656 RID: 1622
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mGetMacroBtn;

		// Token: 0x04000657 RID: 1623
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mNoScriptsGrid;

		// Token: 0x04000658 RID: 1624
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mScriptsGrid;

		// Token: 0x04000659 RID: 1625
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mScriptsListScrollbar;

		// Token: 0x0400065A RID: 1626
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mOpenCommunityBtn;

		// Token: 0x0400065B RID: 1627
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal BlueStacks.BlueStacksUI.ProgressBar mLoadingGrid;

		// Token: 0x0400065C RID: 1628
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOverlayGrid;

		// Token: 0x0400065D RID: 1629
		private bool _contentLoaded;
	}
}
