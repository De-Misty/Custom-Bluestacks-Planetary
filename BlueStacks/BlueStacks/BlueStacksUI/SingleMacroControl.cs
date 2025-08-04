using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000F5 RID: 245
	public class SingleMacroControl : UserControl, IComponentConnector
	{
		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x00008983 File Offset: 0x00006B83
		// (set) Token: 0x06000A4F RID: 2639 RVA: 0x0000898B File Offset: 0x00006B8B
		public bool IsBookmarked
		{
			get
			{
				return this.mIsBookmarked;
			}
			set
			{
				this.mIsBookmarked = value;
				this.ToggleBookmarkIcon(value);
			}
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x0000899B File Offset: 0x00006B9B
		private void ToggleBookmarkIcon(bool isBookmarked)
		{
			if (isBookmarked)
			{
				this.mBookmarkImg.ImageName = "bookmarked";
				return;
			}
			this.mBookmarkImg.ImageName = "bookmark";
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x000393C4 File Offset: 0x000375C4
		internal SingleMacroControl(MainWindow parentWindow, MacroRecording record, MacroRecorderWindow recorderWindow)
		{
			this.InitializeComponent();
			this.mRecording = record;
			this.ParentWindow = parentWindow;
			this.mMacroRecorderWindow = recorderWindow;
			InputMethod.SetIsInputMethodEnabled(this.mMacroShortcutTextBox, false);
			this.mTimestamp.Text = DateTime.ParseExact(this.mRecording.TimeCreated, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal).ToString("yyyy.MM.dd HH.mm.ss", CultureInfo.InvariantCulture);
			this.mScriptName.Text = this.mRecording.Name;
			this.mMacroShortcutTextBox.Text = IMAPKeys.GetStringForUI(this.mRecording.Shortcut);
			this.mScriptName.ToolTip = this.mScriptName.Text;
			if (record.RecordingType == RecordingTypes.MultiRecording)
			{
				this.mScriptSettingsImg.Visibility = Visibility.Collapsed;
				this.mMergeScriptSettingsImg.Visibility = Visibility.Visible;
			}
			if (!string.IsNullOrEmpty(this.mRecording.Shortcut))
			{
				Key key = IMAPKeys.mDictKeys.FirstOrDefault((KeyValuePair<Key, string> x) => x.Value == this.mRecording.Shortcut).Key;
				this.mMacroShortcutTextBox.Tag = IMAPKeys.GetStringForFile(key);
				MainWindow.sMacroMapping[this.mMacroShortcutTextBox.Tag.ToString()] = this.mScriptName.Text;
			}
			else
			{
				this.mMacroShortcutTextBox.Tag = "";
			}
			this.IsBookmarked = BlueStacksUIUtils.CheckIfMacroScriptBookmarked(this.mRecording.Name);
			if (record.PlayOnStart)
			{
				this.mAutorunImage.Visibility = Visibility.Visible;
			}
			if (this.ParentWindow.mIsMacroPlaying && string.Equals(this.mRecording.Name, this.ParentWindow.mMacroPlaying, StringComparison.InvariantCulture))
			{
				this.ToggleScriptPlayPauseUi(true);
				return;
			}
			this.ToggleScriptPlayPauseUi(false);
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x000089C1 File Offset: 0x00006BC1
		public void UpdateMacroRecordingObject(MacroRecording record)
		{
			this.mRecording = record;
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x0002A6B4 File Offset: 0x000288B4
		public static bool DeleteScriptNameFromBookmarkedScriptListIfPresent(string fileName)
		{
			if (RegistryManager.Instance.BookmarkedScriptList.Contains(fileName))
			{
				List<string> list = new List<string>(RegistryManager.Instance.BookmarkedScriptList);
				list.Remove(fileName);
				RegistryManager.Instance.BookmarkedScriptList = list.ToArray();
				return true;
			}
			return false;
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x0003957C File Offset: 0x0003777C
		public bool AddScriptNameToBookmarkedScriptListIfNotPresent(string fileName)
		{
			if (!RegistryManager.Instance.BookmarkedScriptList.Contains(this.mRecording.Name))
			{
				List<string> list = new List<string>(RegistryManager.Instance.BookmarkedScriptList) { fileName };
				RegistryManager.Instance.BookmarkedScriptList = list.ToArray();
				return true;
			}
			return false;
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x000395D0 File Offset: 0x000377D0
		private void UpdateMacroDeleteWindowSettings()
		{
			this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = !this.mDeleteScriptMessageWindow.CheckBox.IsChecked.Value;
			this.mDeleteScriptMessageWindow = null;
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00039610 File Offset: 0x00037810
		private void DeleteMacroScript()
		{
			string text = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, this.mRecording.Name + ".json");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			if (this.mRecording.Shortcut != null && MainWindow.sMacroMapping.ContainsKey(this.mRecording.Shortcut))
			{
				MainWindow.sMacroMapping.Remove(this.mRecording.Shortcut);
			}
			SingleMacroControl.DeleteScriptNameFromBookmarkedScriptListIfPresent(this.mRecording.Name);
			MacroRecording macroRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
				where string.Equals(macro.Name, this.mRecording.Name, StringComparison.InvariantCultureIgnoreCase)
				select macro).FirstOrDefault<MacroRecording>();
			MacroGraph.Instance.RemoveVertex(macroRecording);
			if (this.ParentWindow.mAutoRunMacro != null && this.ParentWindow.mAutoRunMacro.Name.ToLower(CultureInfo.InvariantCulture).Trim() == this.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				this.ParentWindow.mAutoRunMacro = null;
			}
			CommonHandlers.RefreshAllMacroRecorderWindow();
			CommonHandlers.OnMacroDeleted(this.mRecording.Name);
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x000089CA File Offset: 0x00006BCA
		private void SingleMacroControl_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this.mGrid, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
			this.mEditNameImg.Visibility = Visibility.Visible;
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x000089ED File Offset: 0x00006BED
		private void SingleMacroControl_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mGrid.Background = new SolidColorBrush(Colors.Transparent);
			this.mEditNameImg.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00004786 File Offset: 0x00002986
		private void PauseScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x00039738 File Offset: 0x00037938
		private void StopScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.ToggleScriptPlayPauseUi(false);
			this.ParentWindow.mCommonHandler.StopMacroScriptHandling();
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_stop", null, this.mRecording.RecordingType.ToString(), null, null, null);
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x00008A10 File Offset: 0x00006C10
		internal void ToggleScriptPlayPauseUi(bool isScriptRunning)
		{
			if (isScriptRunning)
			{
				this.mScriptPlayPanel.Visibility = Visibility.Collapsed;
				this.mScriptRunningPanel.Visibility = Visibility.Visible;
				return;
			}
			this.mScriptPlayPanel.Visibility = Visibility.Visible;
			this.mScriptRunningPanel.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0003979C File Offset: 0x0003799C
		private void ScriptSettingsImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mMacroRecorderWindow.mOverlayGrid.Visibility = Visibility.Visible;
			MacroRecording macroRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
				where macro.Equals(this.mRecording)
				select macro).FirstOrDefault<MacroRecording>();
			MacroRecording macroRecording2 = this.mRecording;
			if (macroRecording2 != null && macroRecording2.RecordingType == RecordingTypes.MultiRecording)
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_edit", null, null, null, null, null);
				if (this.mMacroRecorderWindow.mMergeMacroWindow == null)
				{
					this.mMacroRecorderWindow.mMergeMacroWindow = new MergeMacroWindow(this.mMacroRecorderWindow, this.ParentWindow)
					{
						Owner = this.ParentWindow
					};
				}
				this.mMacroRecorderWindow.mMergeMacroWindow.Init(macroRecording, this);
				this.mMacroRecorderWindow.mMergeMacroWindow.Show();
				return;
			}
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_settings", null, this.mRecording.RecordingType.ToString(), null, null, null);
			if (this.mMacroSettingsWindow == null || this.mMacroSettingsWindow.IsClosed)
			{
				this.mMacroSettingsWindow = new MacroSettingsWindow(this.ParentWindow, macroRecording, this.mMacroRecorderWindow);
			}
			this.mMacroSettingsWindow.ShowDialog();
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x000398F4 File Offset: 0x00037AF4
		private void BookMarkScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.IsBookmarked)
			{
				this.IsBookmarked = false;
				SingleMacroControl.DeleteScriptNameFromBookmarkedScriptListIfPresent(this.mRecording.Name);
				this.ParentWindow.mCommonHandler.OnMacroBookmarkChanged(this.mRecording.Name, false);
			}
			else if (RegistryManager.Instance.BookmarkedScriptList.Length < 5)
			{
				this.IsBookmarked = true;
				this.AddScriptNameToBookmarkedScriptListIfNotPresent(this.mRecording.Name);
				this.ParentWindow.mCommonHandler.OnMacroBookmarkChanged(this.mRecording.Name, true);
			}
			else
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this.mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_BOOKMARK_MACRO_WARNING", ""), 4.0, true);
			}
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_bookmark", null, this.mRecording.RecordingType.ToString(), null, null, null);
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x000399F4 File Offset: 0x00037BF4
		private void DeleteScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			MacroRecording currentRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
				where string.Equals(macro.Name, this.mRecording.Name, StringComparison.InvariantCultureIgnoreCase)
				select macro).FirstOrDefault<MacroRecording>();
			if (currentRecording == null)
			{
				return;
			}
			if (currentRecording.Parents.Count > 0)
			{
				this.mDeleteScriptMessageWindow = new CustomMessageWindow();
				this.mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_DEPENDENT_MACRO", "");
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(LocaleStrings.GetLocalizedString("STRING_MACRO_IN_USE_BY_OTHER_MACROS", ""));
				stringBuilder.Append(" ");
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_MACRO_LOSE_CONFIGURABILITY", ""), new object[] { this.mRecording.Name }));
				this.mDeleteScriptMessageWindow.BodyTextBlock.Text = stringBuilder.ToString();
				this.mDeleteScriptMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_DELETE_ANYWAY", ""), delegate(object o, EventArgs evt)
				{
					int i;
					int j;
					for (i = currentRecording.Parents.Count - 1; i >= 0; i = j - 1)
					{
						MacroRecording macroRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
							where macro.Equals(currentRecording.Parents[i])
							select macro).FirstOrDefault<MacroRecording>();
						this.mMacroRecorderWindow.FlattenRecording(currentRecording.Parents[i] as MacroRecording, false);
						CommonHandlers.SaveMacroJson(currentRecording.Parents[i] as MacroRecording, (currentRecording.Parents[i] as MacroRecording).Name + ".json");
						foreach (object obj in this.mMacroRecorderWindow.mScriptsStackPanel.Children)
						{
							SingleMacroControl singleMacroControl = (SingleMacroControl)obj;
							if (singleMacroControl.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == macroRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
							{
								singleMacroControl.mScriptSettingsImg.ImageName = "macro_settings";
							}
						}
						MacroGraph.Instance.DeLinkMacroChild(currentRecording.Parents[i] as MacroRecording);
						j = i;
					}
					this.DeleteMacroScript();
					CommonHandlers.RefreshAllMacroRecorderWindow();
				}, null, false, null);
				this.mDeleteScriptMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_DONT_DELETE", ""), delegate(object o, EventArgs evt)
				{
				}, null, false, null);
				this.mDeleteScriptMessageWindow.CloseButtonHandle(delegate(object o, EventArgs evt)
				{
				}, null);
				this.mDeleteScriptMessageWindow.Owner = this.ParentWindow;
				this.mDeleteScriptMessageWindow.ShowDialog();
				return;
			}
			if (!this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup)
			{
				this.DeleteMacroScript();
				return;
			}
			this.mDeleteScriptMessageWindow = new CustomMessageWindow();
			this.mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_MACRO", "");
			this.mDeleteScriptMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCRIPT", "");
			this.mDeleteScriptMessageWindow.CheckBox.Content = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
			this.mDeleteScriptMessageWindow.CheckBox.Visibility = Visibility.Visible;
			this.mDeleteScriptMessageWindow.CheckBox.IsChecked = new bool?(false);
			this.mDeleteScriptMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_DELETE", ""), new EventHandler(this.FlattenTargetMacrosAndDeleteSourceMacro), null, false, null);
			this.mDeleteScriptMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), delegate(object o, EventArgs evt)
			{
				this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup = !this.mDeleteScriptMessageWindow.CheckBox.IsChecked.Value;
			}, null, false, null);
			this.mDeleteScriptMessageWindow.CloseButtonHandle(delegate(object o, EventArgs evt)
			{
			}, null);
			this.mDeleteScriptMessageWindow.Owner = this.ParentWindow;
			this.mDeleteScriptMessageWindow.ShowDialog();
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x00008A46 File Offset: 0x00006C46
		private void FlattenTargetMacrosAndDeleteSourceMacro(object sender, EventArgs e)
		{
			this.UpdateMacroDeleteWindowSettings();
			this.DeleteMacroScript();
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x00039CE8 File Offset: 0x00037EE8
		private void PlayScriptImg_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.ParentWindow.mIsMacroPlaying)
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this.mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_STOP_THE_SCRIPT", ""), 4.0, true);
				return;
			}
			if (MacroGraph.CheckIfDependentMacrosAreAvailable(this.mRecording))
			{
				this.ToggleScriptPlayPauseUi(true);
				this.ParentWindow.mCommonHandler.PlayMacroScript(this.mRecording);
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_play", "macro_popup", this.mRecording.RecordingType.ToString(), string.IsNullOrEmpty(this.mRecording.MacroId) ? "local" : "community", null, null);
				this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
				return;
			}
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.Owner = this.mMacroRecorderWindow;
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_ERROR_IN_MERGE_MACRO", "");
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", delegate(object o, EventArgs evt)
			{
			}, null, false, null);
			customMessageWindow.ShowDialog();
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x00039E30 File Offset: 0x00038030
		private void EditMacroName_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (string.Equals(this.mEditNameImg.ImageName, "edit_icon", StringComparison.InvariantCulture))
			{
				e.Handled = true;
				this.mScriptName.IsEnabled = true;
				this.mEditNameImg.ImageName = "macro_name_save";
				this.mScriptName.Width = this.mScriptNameGrid.ActualWidth - this.mEditNameImg.ActualWidth - this.mTimestamp.ActualWidth - this.mPrefix.ActualWidth - 30.0;
				this.mLastScriptName = this.mScriptName.Text;
				this.mScriptName.Focusable = true;
				this.mScriptName.IsReadOnly = false;
				this.mScriptName.Focus();
				BlueStacksUIBinding.Bind(this.mEditNameImg, "STRING_SAVE");
				this.mScriptName.CaretIndex = this.mScriptName.Text.Length;
				return;
			}
			this.PerformSaveMacroNameOperations();
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x00039F28 File Offset: 0x00038128
		private void PerformSaveMacroNameOperations()
		{
			this.mScriptName.IsEnabled = false;
			this.mScriptName.Focusable = false;
			this.mScriptName.IsReadOnly = true;
			this.mScriptName.BorderThickness = new Thickness(0.0);
			this.mEditNameImg.ImageName = "edit_icon";
			BlueStacksUIBinding.Bind(this.mEditNameImg, "STRING_RENAME");
			this.SaveMacroName();
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x00039F98 File Offset: 0x00038198
		private void SaveMacroName()
		{
			if (string.IsNullOrEmpty(this.mScriptName.Text.Trim()))
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", ""), 4.0, true);
				this.mScriptName.Text = this.mLastScriptName;
				return;
			}
			if ((from MacroRecording macro in MacroGraph.Instance.Vertices
				select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(this.mScriptName.Text.ToLower(CultureInfo.InvariantCulture).Trim()) && !(this.mScriptName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == this.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_NOT_SAVED_MESSAGE", ""), 4.0, true);
				this.mScriptName.Text = this.mLastScriptName;
				return;
			}
			if (this.mScriptName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[]
				{
					LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
					Environment.NewLine,
					"\\ / : * ? \" < > |"
				});
				this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow.MacroRecorderWindow, text, 4.0, true);
				this.mScriptName.Text = this.mLastScriptName;
				return;
			}
			if (Constants.ReservedFileNamesList.Contains(this.mScriptName.Text.Trim().ToLower(CultureInfo.InvariantCulture)))
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", ""), 4.0, true);
				this.mScriptName.Text = this.mLastScriptName;
				return;
			}
			this.SaveScriptSettings();
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x0003A1DC File Offset: 0x000383DC
		private void SaveScriptSettings()
		{
			try
			{
				if (!string.Equals(this.mRecording.Shortcut, this.mMacroShortcutTextBox.Tag.ToString(), StringComparison.InvariantCulture) || !string.Equals(this.mRecording.Name.Trim(), this.mScriptName.Text.Trim(), StringComparison.InvariantCultureIgnoreCase))
				{
					JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
					serializerSettings.Formatting = Formatting.Indented;
					string text = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, this.mRecording.Name + ".json");
					if (this.mRecording.Shortcut != this.mMacroShortcutTextBox.Tag.ToString())
					{
						if (!string.IsNullOrEmpty(this.mRecording.Shortcut) && MainWindow.sMacroMapping.ContainsKey(this.mRecording.Shortcut))
						{
							MainWindow.sMacroMapping.Remove(this.mRecording.Shortcut);
						}
						if (this.mMacroShortcutTextBox.Tag != null && !string.IsNullOrEmpty(this.mMacroShortcutTextBox.Tag.ToString()))
						{
							MainWindow.sMacroMapping[this.mMacroShortcutTextBox.Tag.ToString()] = this.mScriptName.Text;
						}
						if (this.mRecording.Shortcut != null && this.mMacroShortcutTextBox.Tag != null && !string.Equals(this.mRecording.Shortcut, this.mMacroShortcutTextBox.Tag.ToString(), StringComparison.InvariantCulture))
						{
							ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_window_shortcutkey", null, this.mRecording.RecordingType.ToString(), null, null, null);
						}
						if (this.mMacroShortcutTextBox.Tag != null)
						{
							this.mRecording.Shortcut = this.mMacroShortcutTextBox.Tag.ToString();
						}
						if (this.mRecording.PlayOnStart)
						{
							this.ParentWindow.mAutoRunMacro = this.mRecording;
						}
						string text2 = JsonConvert.SerializeObject(this.mRecording, serializerSettings);
						File.WriteAllText(text, text2);
					}
					if (!string.Equals(this.mRecording.Name.Trim(), this.mScriptName.Text.Trim(), StringComparison.InvariantCultureIgnoreCase))
					{
						string oldMacroName = this.mRecording.Name;
						MacroRecording macroRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
							where string.Equals(macro.Name, this.mRecording.Name, StringComparison.InvariantCultureIgnoreCase)
							select macro).FirstOrDefault<MacroRecording>();
						macroRecording.Name = this.mScriptName.Text.ToLower(CultureInfo.InvariantCulture).Trim();
						this.mRecording.Name = this.mScriptName.Text.Trim();
						if (this.mRecording.PlayOnStart)
						{
							this.ParentWindow.mAutoRunMacro = this.mRecording;
						}
						string text3 = JsonConvert.SerializeObject(this.mRecording, serializerSettings);
						File.WriteAllText(text, text3);
						string text4 = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, this.mScriptName.Text.Trim() + ".json");
						File.Move(text, text4);
						Func<string, string> <>9__1;
						foreach (MacroRecording macroRecording2 in macroRecording.Parents.Cast<MacroRecording>())
						{
							foreach (MergedMacroConfiguration mergedMacroConfiguration in macroRecording2.MergedMacroConfigurations)
							{
								List<string> list = new List<string>();
								IEnumerable<string> macrosToRun = mergedMacroConfiguration.MacrosToRun;
								Func<string, string> func;
								if ((func = <>9__1) == null)
								{
									func = (<>9__1 = delegate(string macroToRun)
									{
										if (!string.Equals(oldMacroName, macroToRun, StringComparison.CurrentCultureIgnoreCase))
										{
											return macroToRun;
										}
										return macroToRun.Replace(macroToRun, this.mRecording.Name);
									});
								}
								foreach (string text5 in macrosToRun.Select(func))
								{
									list.Add(text5);
								}
								mergedMacroConfiguration.MacrosToRun.Clear();
								foreach (string text6 in list)
								{
									mergedMacroConfiguration.MacrosToRun.Add(text6);
								}
							}
							CommonHandlers.SaveMacroJson(macroRecording2, CommonHandlers.GetCompleteMacroRecordingPath(macroRecording2.Name));
							CommonHandlers.OnMacroSettingChanged(macroRecording2);
						}
						if (this.IsBookmarked)
						{
							SingleMacroControl.DeleteScriptNameFromBookmarkedScriptListIfPresent(oldMacroName);
							this.AddScriptNameToBookmarkedScriptListIfNotPresent(this.mRecording.Name);
						}
					}
					CommonHandlers.OnMacroSettingChanged(this.mRecording);
					CommonHandlers.RefreshAllMacroRecorderWindow();
					CommonHandlers.ReloadMacroShortcutsForAllInstances();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in saving macro settings: " + ex.ToString());
			}
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x00008A54 File Offset: 0x00006C54
		private void NoSelection_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.mScriptName.SelectionLength = 0;
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x0003A708 File Offset: 0x00038908
		private bool Valid(Key key)
		{
			if (key == Key.Escape || key == Key.LeftAlt || key == Key.RightAlt || key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.RightShift || key == Key.LeftShift || key == Key.Capital || key == Key.Return || key == Key.Space || key == Key.Delete)
			{
				return false;
			}
			if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
			{
				string text = string.Format(CultureInfo.InvariantCulture, "{0} + {1} + {2}", new object[]
				{
					IMAPKeys.GetStringForFile(Key.LeftCtrl),
					IMAPKeys.GetStringForFile(Key.LeftAlt),
					IMAPKeys.GetStringForFile(key)
				});
				using (List<ShortcutKeys>.Enumerator enumerator = this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (string.Equals(enumerator.Current.ShortcutKey, text, StringComparison.InvariantCulture))
						{
							this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_ALREADY_IN_USE_MESSAGE", ""), 4.0, true);
							return false;
						}
					}
				}
			}
			if (!MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForFile(key)))
			{
				return true;
			}
			if (MainWindow.sMacroMapping[IMAPKeys.GetStringForFile(key)] != this.mRecording.Name)
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this.ParentWindow.MacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_ALREADY_IN_USE_MESSAGE", ""), 4.0, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x0003A89C File Offset: 0x00038A9C
		private void MacroShortcutPreviewKeyDown(object sender, KeyEventArgs e)
		{
			this.mMacroShortcutTextBox.Text = "";
			this.mMacroShortcutTextBox.Tag = "";
			Key key = ((e.Key == Key.System) ? e.SystemKey : e.Key);
			if (IMAPKeys.mDictKeys.ContainsKey(key) && this.Valid(key))
			{
				this.mMacroShortcutTextBox.Text = IMAPKeys.GetStringForUI(key);
				this.mMacroShortcutTextBox.Tag = IMAPKeys.GetStringForFile(key);
			}
			else
			{
				this.mMacroShortcutTextBox.Text = "";
				this.mMacroShortcutTextBox.Tag = "";
			}
			e.Handled = true;
			this.SaveScriptSettings();
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00008A62 File Offset: 0x00006C62
		private void ScriptName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.PerformSaveMacroNameOperations();
			}
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00008A73 File Offset: 0x00006C73
		private void ScriptName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			this.PerformSaveMacroNameOperations();
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x00008A73 File Offset: 0x00006C73
		private void ScriptName_LostFocus(object sender, RoutedEventArgs e)
		{
			this.PerformSaveMacroNameOperations();
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x00008A7B File Offset: 0x00006C7B
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			this.mMacroRecorderWindow.OpenCommunityAndCloseMacroWindow(this.mRecording.AuthorPageUrl.ToString());
			e.Handled = true;
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x0003A94C File Offset: 0x00038B4C
		private void UserNameHyperlink_Loaded(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.mRecording.User))
			{
				this.mUserNameHyperlink.Inlines.Clear();
				this.mUserNameHyperlink.Inlines.Add(this.mRecording.User);
				if (this.mRecording.AuthorPageUrl != null && !string.IsNullOrEmpty(this.mRecording.AuthorPageUrl.ToString()))
				{
					this.mUserNameHyperlink.NavigateUri = this.mRecording.AuthorPageUrl;
				}
				this.mScriptName.FontSize = 13.0;
				this.mUserNameTextblock.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00008A9F File Offset: 0x00006C9F
		private void CommunityMacroPage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.mMacroRecorderWindow.OpenCommunityAndCloseMacroWindow(this.mRecording.MacroPageUrl.ToString());
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x00008ABC File Offset: 0x00006CBC
		private void ScriptControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.mRecording.User))
			{
				this.mCommunityMacroImage.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x0003A9FC File Offset: 0x00038BFC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/singlemacrocontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0003AA2C File Offset: 0x00038C2C
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
				((SingleMacroControl)target).MouseEnter += this.SingleMacroControl_MouseEnter;
				((SingleMacroControl)target).MouseLeave += this.SingleMacroControl_MouseLeave;
				((SingleMacroControl)target).Loaded += this.ScriptControl_Loaded;
				return;
			case 2:
				this.mGrid = (Grid)target;
				return;
			case 3:
				this.mBookmarkImg = (CustomPictureBox)target;
				this.mBookmarkImg.PreviewMouseLeftButtonUp += this.BookMarkScriptImg_PreviewMouseLeftButtonUp;
				return;
			case 4:
				this.mScriptNameGrid = (Grid)target;
				return;
			case 5:
				this.mScriptName = (CustomTextBox)target;
				this.mScriptName.PreviewLostKeyboardFocus += this.ScriptName_LostKeyboardFocus;
				this.mScriptName.LostFocus += this.ScriptName_LostFocus;
				this.mScriptName.MouseLeftButtonUp += this.NoSelection_MouseUp;
				this.mScriptName.KeyDown += this.ScriptName_KeyDown;
				return;
			case 6:
				this.mUserNameTextblock = (TextBlock)target;
				return;
			case 7:
				this.mUserNameHyperlink = (Hyperlink)target;
				this.mUserNameHyperlink.RequestNavigate += this.Hyperlink_RequestNavigate;
				this.mUserNameHyperlink.Loaded += this.UserNameHyperlink_Loaded;
				return;
			case 8:
				this.mEditNameImg = (CustomPictureBox)target;
				this.mEditNameImg.MouseLeftButtonDown += this.EditMacroName_MouseDown;
				return;
			case 9:
				this.mTimestamp = (TextBlock)target;
				return;
			case 10:
				this.mPrefix = (TextBlock)target;
				return;
			case 11:
				this.mMacroShortcutTextBox = (CustomTextBox)target;
				this.mMacroShortcutTextBox.PreviewKeyDown += this.MacroShortcutPreviewKeyDown;
				return;
			case 12:
				this.mScriptPlayPanel = (StackPanel)target;
				return;
			case 13:
				this.mAutorunImage = (CustomPictureBox)target;
				this.mAutorunImage.PreviewMouseLeftButtonUp += this.BookMarkScriptImg_PreviewMouseLeftButtonUp;
				return;
			case 14:
				this.mCommunityMacroImage = (CustomPictureBox)target;
				this.mCommunityMacroImage.PreviewMouseLeftButtonUp += this.CommunityMacroPage_PreviewMouseLeftButtonUp;
				return;
			case 15:
				this.mPlayScriptImg = (CustomPictureBox)target;
				this.mPlayScriptImg.PreviewMouseLeftButtonUp += this.PlayScriptImg_PreviewMouseLeftButtonUp;
				return;
			case 16:
				this.mScriptSettingsImg = (CustomPictureBox)target;
				this.mScriptSettingsImg.PreviewMouseLeftButtonUp += this.ScriptSettingsImg_PreviewMouseLeftButtonUp;
				return;
			case 17:
				this.mMergeScriptSettingsImg = (CustomPictureBox)target;
				this.mMergeScriptSettingsImg.PreviewMouseLeftButtonUp += this.ScriptSettingsImg_PreviewMouseLeftButtonUp;
				return;
			case 18:
				this.mDeleteScriptImg = (CustomPictureBox)target;
				this.mDeleteScriptImg.PreviewMouseLeftButtonUp += this.DeleteScriptImg_PreviewMouseLeftButtonUp;
				return;
			case 19:
				this.mScriptRunningPanel = (StackPanel)target;
				return;
			case 20:
				this.mStopScriptImg = (CustomPictureBox)target;
				this.mStopScriptImg.PreviewMouseLeftButtonUp += this.StopScriptImg_PreviewMouseLeftButtonUp;
				return;
			case 21:
				this.mRunning = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000618 RID: 1560
		private MainWindow ParentWindow;

		// Token: 0x04000619 RID: 1561
		internal MacroRecorderWindow mMacroRecorderWindow;

		// Token: 0x0400061A RID: 1562
		internal MacroRecording mRecording;

		// Token: 0x0400061B RID: 1563
		internal MacroSettingsWindow mMacroSettingsWindow;

		// Token: 0x0400061C RID: 1564
		private CustomMessageWindow mDeleteScriptMessageWindow;

		// Token: 0x0400061D RID: 1565
		private bool mIsBookmarked;

		// Token: 0x0400061E RID: 1566
		private string mLastScriptName;

		// Token: 0x0400061F RID: 1567
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x04000620 RID: 1568
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mBookmarkImg;

		// Token: 0x04000621 RID: 1569
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mScriptNameGrid;

		// Token: 0x04000622 RID: 1570
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mScriptName;

		// Token: 0x04000623 RID: 1571
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mUserNameTextblock;

		// Token: 0x04000624 RID: 1572
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mUserNameHyperlink;

		// Token: 0x04000625 RID: 1573
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mEditNameImg;

		// Token: 0x04000626 RID: 1574
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTimestamp;

		// Token: 0x04000627 RID: 1575
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mPrefix;

		// Token: 0x04000628 RID: 1576
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mMacroShortcutTextBox;

		// Token: 0x04000629 RID: 1577
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mScriptPlayPanel;

		// Token: 0x0400062A RID: 1578
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mAutorunImage;

		// Token: 0x0400062B RID: 1579
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCommunityMacroImage;

		// Token: 0x0400062C RID: 1580
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mPlayScriptImg;

		// Token: 0x0400062D RID: 1581
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mScriptSettingsImg;

		// Token: 0x0400062E RID: 1582
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mMergeScriptSettingsImg;

		// Token: 0x0400062F RID: 1583
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mDeleteScriptImg;

		// Token: 0x04000630 RID: 1584
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mScriptRunningPanel;

		// Token: 0x04000631 RID: 1585
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mStopScriptImg;

		// Token: 0x04000632 RID: 1586
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mRunning;

		// Token: 0x04000633 RID: 1587
		private bool _contentLoaded;
	}
}
