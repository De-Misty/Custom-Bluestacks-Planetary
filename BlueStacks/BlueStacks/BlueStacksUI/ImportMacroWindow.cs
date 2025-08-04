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
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;
using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000B8 RID: 184
	public class ImportMacroWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x06000773 RID: 1907 RVA: 0x000295B4 File Offset: 0x000277B4
		public ImportMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
		{
			this.InitializeComponent();
			this.mOperationWindow = window;
			this.ParentWindow = mainWindow;
			this.mScriptsStackPanel = this.mScriptsListScrollbar.Content as StackPanel;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00029608 File Offset: 0x00027808
		internal void TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.mInited)
			{
				ImportMacroScriptsControl scriptControlFromMacroItemGrandchild = this.GetScriptControlFromMacroItemGrandchild((sender as FrameworkElement).Parent);
				string text = (sender as CustomTextBox).Text;
				foreach (object obj in scriptControlFromMacroItemGrandchild.mDependentScriptsPanel.Children)
				{
					CustomTextBox customTextBox = ((UIElement)obj) as CustomTextBox;
					customTextBox.Text = MacroRecorderWindow.GetDependentRecordingName(text, this.mDependentRecordingDict[customTextBox].Name);
				}
			}
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00006D59 File Offset: 0x00004F59
		private void ImportMacroWindow_Closing(object sender, CancelEventArgs e)
		{
			this.CloseWindow();
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00006D61 File Offset: 0x00004F61
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x00006D69 File Offset: 0x00004F69
		private void CloseWindow()
		{
			this.mOperationWindow.mImportMacroWindow = null;
			this.mOperationWindow.mOverlayGrid.Visibility = Visibility.Hidden;
			this.mOperationWindow.Focus();
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x000296A8 File Offset: 0x000278A8
		private ImportMacroScriptsControl AddRecordingToStackPanelAndDict(MacroRecording record, bool isSingleRecording, out string suggestedName)
		{
			ImportMacroScriptsControl importMacroScriptsControl = new ImportMacroScriptsControl(this, this.ParentWindow);
			importMacroScriptsControl.Init(record.Name, isSingleRecording);
			suggestedName = ((!(from MacroRecording macro in MacroGraph.Instance.Vertices
				select macro.Name.ToLower(CultureInfo.InvariantCulture)).Contains(record.Name.ToLower(CultureInfo.InvariantCulture).Trim())) ? record.Name : CommonHandlers.GetMacroName(record.Name));
			importMacroScriptsControl.mImportName.Text = this.ValidateSuggestedName(suggestedName);
			this.mScriptsStackPanel.Children.Add(importMacroScriptsControl);
			this.mBoxToRecordingDict[importMacroScriptsControl] = record;
			return importMacroScriptsControl;
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00029768 File Offset: 0x00027968
		internal void Init()
		{
			bool flag = this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Count == 1;
			try
			{
				this.mInited = false;
				this.mScriptsStackPanel.Children.Clear();
				foreach (MacroRecording macroRecording in this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList)
				{
					string text;
					ImportMacroScriptsControl importMacroScriptsControl = this.AddRecordingToStackPanelAndDict(macroRecording, flag, out text);
					if (macroRecording.RecordingType == RecordingTypes.MultiRecording)
					{
						bool flag2 = false;
						bool? mImportMultiMacroAsUnified = this.mOperationWindow.mImportMultiMacroAsUnified;
						if ((flag2 == mImportMultiMacroAsUnified.GetValueOrDefault()) & (mImportMultiMacroAsUnified != null))
						{
							importMacroScriptsControl.mDependentScriptsMsg.Visibility = Visibility.Visible;
							importMacroScriptsControl.mDependentScriptsPanel.Visibility = Visibility.Visible;
							importMacroScriptsControl.mDependentScriptsPanel.Children.Clear();
							foreach (string text2 in macroRecording.SourceRecordings)
							{
								MacroRecording macroRecording2 = JsonConvert.DeserializeObject<MacroRecording>(text2, Utils.GetSerializerSettings());
								string dependentRecordingName = MacroRecorderWindow.GetDependentRecordingName(text, macroRecording2.Name);
								string text3 = ((!(from MacroRecording macro in MacroGraph.Instance.Vertices
									select macro.Name).Contains(dependentRecordingName.ToLower(CultureInfo.InvariantCulture).Trim())) ? dependentRecordingName : CommonHandlers.GetMacroName(dependentRecordingName));
								CustomTextBox customTextBox = new CustomTextBox
								{
									Height = 24.0,
									HorizontalAlignment = HorizontalAlignment.Left,
									Margin = new Thickness(0.0, 5.0, 0.0, 0.0),
									Text = this.ValidateSuggestedName(text3),
									Visibility = Visibility.Visible,
									IsEnabled = false
								};
								importMacroScriptsControl.mDependentScriptsPanel.Children.Add(customTextBox);
								this.mDependentRecordingDict[customTextBox] = macroRecording2;
							}
						}
					}
				}
				this.mNumberOfFilesSelectedForImport = 0;
			}
			catch (Exception ex)
			{
				Logger.Error("Error in import window init err: " + ex.ToString());
			}
			this.mInited = true;
			if (flag)
			{
				this.mSelectAllBtn.Visibility = Visibility.Hidden;
			}
			this.mSelectAllBtn.IsChecked = new bool?(true);
			this.SelectAllBtn_Click(null, null);
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x00029A18 File Offset: 0x00027C18
		private string ValidateSuggestedName(string suggestedName)
		{
			if (this.mBoxToRecordingDict.Keys.Any((ImportMacroScriptsControl box) => string.Equals(box.mImportName.Text.Trim(), suggestedName, StringComparison.InvariantCultureIgnoreCase)))
			{
				int num = suggestedName.LastIndexOf('(') + 1;
				int num2 = suggestedName.LastIndexOf(')');
				int num3;
				if (int.TryParse(suggestedName.Substring(num, num2 - num), out num3))
				{
					suggestedName = suggestedName.Remove(num, num2 - num).Insert(num, (num3 + 1).ToString(CultureInfo.InvariantCulture));
					return this.ValidateSuggestedName(suggestedName);
				}
				Logger.Error("Error in ValidateSuggestedName: Could not get integer part in suggested name '{0}'", new object[] { suggestedName });
			}
			return suggestedName;
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00029AE4 File Offset: 0x00027CE4
		private bool CheckIfEditedMacroNameIsAllowed(string text, ImportMacroScriptsControl item)
		{
			if (string.IsNullOrEmpty(text.Trim()))
			{
				BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", ""), "");
				return false;
			}
			using (IEnumerator<BiDirectionalVertex<MacroRecording>> enumerator = MacroGraph.Instance.Vertices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((MacroRecording)enumerator.Current).Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
					{
						return false;
					}
				}
			}
			foreach (object obj in this.mScriptsStackPanel.Children)
			{
				ImportMacroScriptsControl importMacroScriptsControl = (ImportMacroScriptsControl)obj;
				if (item != importMacroScriptsControl)
				{
					bool? isChecked = importMacroScriptsControl.mContent.IsChecked;
					bool flag = true;
					if (((isChecked.GetValueOrDefault() == flag) & (isChecked != null)) && importMacroScriptsControl.IsScriptInRenameMode() && importMacroScriptsControl.mImportName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x00006D94 File Offset: 0x00004F94
		private bool IsMacroItemDependentOfParent(ImportMacroScriptsControl item, string name)
		{
			return item.Tag != null && item.Tag.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x00029C44 File Offset: 0x00027E44
		private ImportMacroScriptsControl GetScriptControlFromMacroItemGrandchild(object grandchild)
		{
			while (grandchild != null)
			{
				DependencyObject parent = (grandchild as FrameworkElement).Parent;
				if (parent != null && parent is ImportMacroScriptsControl)
				{
					return parent as ImportMacroScriptsControl;
				}
				grandchild = parent;
			}
			return null;
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x00029C78 File Offset: 0x00027E78
		internal void Box_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!this.mIsInDependentFileFindingMode)
			{
				this.mIsInDependentFileFindingMode = true;
				this.mNumberOfFilesSelectedForImport--;
				if (this.mNumberOfFilesSelectedForImport == 0)
				{
					this.mImportBtn.IsEnabled = false;
				}
				if (this.mNumberOfFilesSelectedForImport < this.mScriptsStackPanel.Children.Count)
				{
					this.mSelectAllBtn.IsChecked = new bool?(false);
				}
				this.mIsInDependentFileFindingMode = false;
			}
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x00029CE8 File Offset: 0x00027EE8
		internal void Box_Checked(object sender, RoutedEventArgs e)
		{
			if (!this.mIsInDependentFileFindingMode)
			{
				this.mIsInDependentFileFindingMode = true;
				this.mNumberOfFilesSelectedForImport++;
				if (this.mNumberOfFilesSelectedForImport > 0)
				{
					this.mImportBtn.IsEnabled = true;
				}
				if (this.mNumberOfFilesSelectedForImport == this.mScriptsStackPanel.Children.Count)
				{
					this.mSelectAllBtn.IsChecked = new bool?(true);
				}
				this.mIsInDependentFileFindingMode = false;
			}
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x00029D58 File Offset: 0x00027F58
		private static List<MacroEvents> GetRecordingEventsFromSourceRecording(MacroRecording srcRecording, double acceleration, long initialTime, ref long elapsedTime)
		{
			if (srcRecording == null)
			{
				throw new Exception("Source recording now found in multiMacro");
			}
			List<MacroEvents> list = new List<MacroEvents>();
			foreach (MacroEvents macroEvents in srcRecording.Events)
			{
				MacroEvents macroEvents2 = macroEvents;
				macroEvents2.Timestamp = (long)Math.Floor((double)macroEvents.Timestamp / acceleration);
				macroEvents2.Timestamp += initialTime;
				elapsedTime = macroEvents2.Timestamp;
			}
			return list;
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x00029DE8 File Offset: 0x00027FE8
		private ImportMacroScriptsControl GetMacroItemFromTag(string tag)
		{
			foreach (object obj in this.mScriptsStackPanel.Children)
			{
				ImportMacroScriptsControl importMacroScriptsControl = (ImportMacroScriptsControl)obj;
				if (this.mBoxToRecordingDict[importMacroScriptsControl].Name == tag)
				{
					return importMacroScriptsControl;
				}
			}
			return null;
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00029E60 File Offset: 0x00028060
		private void ImportBtn_Click(object sender, RoutedEventArgs e)
		{
			int num = 0;
			bool flag = false;
			bool flag2 = true;
			List<MacroRecording> list = new List<MacroRecording>();
			foreach (object obj in this.mScriptsStackPanel.Children)
			{
				ImportMacroScriptsControl importMacroScriptsControl = (ImportMacroScriptsControl)obj;
				bool? isChecked = importMacroScriptsControl.mContent.IsChecked;
				bool flag3 = true;
				if ((isChecked.GetValueOrDefault() == flag3) & (isChecked != null))
				{
					if (importMacroScriptsControl.mImportName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
					{
						string text = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[]
						{
							LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
							Environment.NewLine,
							"\\ / : * ? \" < > |"
						});
						BlueStacksUIBinding.Bind(importMacroScriptsControl.mWarningMsg, text, "");
						importMacroScriptsControl.mImportName.InputTextValidity = TextValidityOptions.Error;
						if (importMacroScriptsControl.mImportName.IsEnabled)
						{
							importMacroScriptsControl.mWarningMsg.Visibility = Visibility.Visible;
						}
						flag2 = false;
					}
					else if (Constants.ReservedFileNamesList.Contains(importMacroScriptsControl.mImportName.Text.Trim().ToLower(CultureInfo.InvariantCulture)))
					{
						BlueStacksUIBinding.Bind(importMacroScriptsControl.mWarningMsg, "STRING_MACRO_FILE_NAME_ERROR", "");
						importMacroScriptsControl.mImportName.InputTextValidity = TextValidityOptions.Error;
						if (importMacroScriptsControl.mImportName.IsEnabled)
						{
							importMacroScriptsControl.mWarningMsg.Visibility = Visibility.Visible;
						}
						flag2 = false;
					}
					else if (!this.CheckIfEditedMacroNameIsAllowed(importMacroScriptsControl.mImportName.Text, importMacroScriptsControl) && importMacroScriptsControl.IsScriptInRenameMode())
					{
						if (!string.IsNullOrEmpty(importMacroScriptsControl.mImportName.Text.Trim()))
						{
							BlueStacksUIBinding.Bind(importMacroScriptsControl.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", ""), "");
						}
						importMacroScriptsControl.mImportName.InputTextValidity = TextValidityOptions.Error;
						if (importMacroScriptsControl.mImportName.IsEnabled)
						{
							importMacroScriptsControl.mWarningMsg.Visibility = Visibility.Visible;
						}
						flag2 = false;
					}
					else if (importMacroScriptsControl.mDependentScriptsPanel.Visibility == Visibility.Visible && importMacroScriptsControl.mDependentScriptsPanel.Children.Count > 0)
					{
						string text2 = this.CheckIfDependentScriptsHaveInvalidName(importMacroScriptsControl);
						if (text2 != "TEXT_VALID")
						{
							BlueStacksUIBinding.Bind(importMacroScriptsControl.mWarningMsg, text2, "");
							importMacroScriptsControl.mImportName.InputTextValidity = TextValidityOptions.Error;
							flag2 = false;
						}
						else
						{
							importMacroScriptsControl.mImportName.InputTextValidity = TextValidityOptions.Success;
							importMacroScriptsControl.mWarningMsg.Visibility = Visibility.Collapsed;
						}
					}
					else
					{
						importMacroScriptsControl.mImportName.InputTextValidity = TextValidityOptions.Success;
						importMacroScriptsControl.mWarningMsg.Visibility = Visibility.Collapsed;
					}
					flag = true;
				}
				num++;
			}
			if (!flag)
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_IMPORT_MACRO_SELECTED", ""), 4.0, true);
				return;
			}
			if (flag2)
			{
				if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
				{
					Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
				}
				foreach (object obj2 in this.mScriptsStackPanel.Children)
				{
					ImportMacroScriptsControl importMacroScriptsControl2 = (ImportMacroScriptsControl)obj2;
					if (importMacroScriptsControl2.mContent.IsChecked.GetValueOrDefault())
					{
						MacroRecording macroRecording = this.mBoxToRecordingDict[importMacroScriptsControl2];
						string newScript = ((importMacroScriptsControl2.mReplaceExistingBtn.IsChecked != null && importMacroScriptsControl2.mReplaceExistingBtn.IsChecked.Value) ? importMacroScriptsControl2.mContent.Content.ToString() : importMacroScriptsControl2.mImportName.Text.Trim());
						MacroRecording existingMacro = (from MacroRecording m in MacroGraph.Instance.Vertices
							where string.Equals(m.Name, newScript, StringComparison.InvariantCultureIgnoreCase)
							select m).FirstOrDefault<MacroRecording>();
						if (existingMacro != null)
						{
							if (existingMacro.Parents.Count > 0)
							{
								int index2;
								int index;
								for (index = existingMacro.Parents.Count - 1; index >= 0; index = index2 - 1)
								{
									MacroRecording macroRecording2 = (from MacroRecording macro in MacroGraph.Instance.Vertices
										where macro.Equals(existingMacro.Parents[index])
										select macro).FirstOrDefault<MacroRecording>();
									this.mOperationWindow.FlattenRecording(existingMacro.Parents[index] as MacroRecording, false);
									CommonHandlers.SaveMacroJson(existingMacro.Parents[index] as MacroRecording, (existingMacro.Parents[index] as MacroRecording).Name + ".json");
									foreach (object obj3 in this.mOperationWindow.mScriptsStackPanel.Children)
									{
										SingleMacroControl singleMacroControl = (SingleMacroControl)obj3;
										if (singleMacroControl.mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() == macroRecording2.Name.ToLower(CultureInfo.InvariantCulture).Trim())
										{
											singleMacroControl.mScriptSettingsImg.ImageName = "macro_settings";
										}
									}
									MacroGraph.Instance.DeLinkMacroChild(existingMacro.Parents[index] as MacroRecording);
									index2 = index;
								}
							}
							this.DeleteMacroScript(existingMacro);
						}
						macroRecording.Name = newScript;
						if (macroRecording.RecordingType == RecordingTypes.MultiRecording)
						{
							this.mOperationWindow.ImportMultiMacro(macroRecording, this.mOperationWindow.mImportMultiMacroAsUnified.Value, list, this.GetDictionaryOfNewNamesForDependentRecordings(macroRecording.Name));
						}
						else
						{
							CommonHandlers.SaveMacroJson(macroRecording, macroRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json");
							MacroGraph.Instance.AddVertex(macroRecording);
							list.Add(macroRecording);
						}
					}
				}
				foreach (MacroRecording macroRecording3 in list)
				{
					MacroGraph.LinkMacroChilds(macroRecording3);
				}
				this.mOperationWindow.mNewlyAddedMacrosList.AddRange(list);
				this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Clear();
				base.Close();
			}
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0002A584 File Offset: 0x00028784
		private void DeleteMacroScript(MacroRecording mRecording)
		{
			string text = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, mRecording.Name + ".json");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			if (mRecording.Shortcut != null && MainWindow.sMacroMapping.ContainsKey(mRecording.Shortcut))
			{
				MainWindow.sMacroMapping.Remove(mRecording.Shortcut);
			}
			ImportMacroWindow.DeleteScriptNameFromBookmarkedScriptListIfPresent(mRecording.Name);
			MacroRecording macroRecording = (from MacroRecording macro in MacroGraph.Instance.Vertices
				where string.Equals(macro.Name, mRecording.Name, StringComparison.InvariantCultureIgnoreCase)
				select macro).FirstOrDefault<MacroRecording>();
			MacroGraph.Instance.RemoveVertex(macroRecording);
			if (this.ParentWindow.mAutoRunMacro != null && this.ParentWindow.mAutoRunMacro.Name.ToLower(CultureInfo.InvariantCulture).Trim() == mRecording.Name.ToLower(CultureInfo.InvariantCulture).Trim())
			{
				this.ParentWindow.mAutoRunMacro = null;
			}
			CommonHandlers.OnMacroDeleted(mRecording.Name);
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0002A6B4 File Offset: 0x000288B4
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

		// Token: 0x06000785 RID: 1925 RVA: 0x0002A700 File Offset: 0x00028900
		private string CheckIfDependentScriptsHaveInvalidName(ImportMacroScriptsControl scriptControl)
		{
			string text = "TEXT_VALID";
			foreach (object obj in scriptControl.mDependentScriptsPanel.Children)
			{
				CustomTextBox customTextBox = ((UIElement)obj) as CustomTextBox;
				string text2 = customTextBox.Text;
				if (text2.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
				{
					customTextBox.InputTextValidity = TextValidityOptions.Error;
					text = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[]
					{
						LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
						Environment.NewLine,
						"\\ / : * ? \" < > |"
					});
				}
				else if (Constants.ReservedFileNamesList.Contains(text2.Trim().ToLower(CultureInfo.InvariantCulture)))
				{
					customTextBox.InputTextValidity = TextValidityOptions.Error;
					text = LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", "");
				}
				else if (scriptControl.IsScriptInRenameMode())
				{
					using (IEnumerator<BiDirectionalVertex<MacroRecording>> enumerator2 = MacroGraph.Instance.Vertices.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (((MacroRecording)enumerator2.Current).Name.ToLower(CultureInfo.InvariantCulture).Trim() == text2.ToLower(CultureInfo.InvariantCulture).Trim())
							{
								customTextBox.InputTextValidity = TextValidityOptions.Error;
								return LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
							}
						}
					}
					foreach (object obj2 in this.mScriptsStackPanel.Children)
					{
						ImportMacroScriptsControl importMacroScriptsControl = (ImportMacroScriptsControl)obj2;
						if (importMacroScriptsControl != scriptControl && scriptControl.IsScriptInRenameMode())
						{
							bool? isChecked = importMacroScriptsControl.mContent.IsChecked;
							bool flag = true;
							if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
							{
								if (importMacroScriptsControl.mImportName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text2.ToLower(CultureInfo.InvariantCulture).Trim())
								{
									customTextBox.InputTextValidity = TextValidityOptions.Error;
									text = LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
								}
								else
								{
									using (IEnumerator enumerator4 = importMacroScriptsControl.mDependentScriptsPanel.Children.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											if ((((UIElement)enumerator4.Current) as CustomTextBox).Text.ToLower(CultureInfo.InvariantCulture).Trim() == text2.ToLower(CultureInfo.InvariantCulture).Trim())
											{
												customTextBox.InputTextValidity = TextValidityOptions.Error;
												text = LocaleStrings.GetLocalizedString("STRING_DUPLICATE_MACRO_NAME_WARNING", "");
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return text;
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x0002AA3C File Offset: 0x00028C3C
		private Dictionary<string, string> GetDictionaryOfNewNamesForDependentRecordings(string parentMacroName)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in this.mScriptsStackPanel.Children)
			{
				ImportMacroScriptsControl importMacroScriptsControl = (ImportMacroScriptsControl)obj;
				if ((importMacroScriptsControl.Tag != null && importMacroScriptsControl.Tag.ToString().Equals(parentMacroName, StringComparison.InvariantCultureIgnoreCase)) || (importMacroScriptsControl.mContent.Content.ToString().Equals(parentMacroName, StringComparison.InvariantCultureIgnoreCase) && !importMacroScriptsControl.mReplaceExistingBtn.IsChecked.GetValueOrDefault()))
				{
					dictionary.Add(importMacroScriptsControl.mContent.Content.ToString(), importMacroScriptsControl.mImportName.Text);
				}
			}
			return dictionary;
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x0002AB08 File Offset: 0x00028D08
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

		// Token: 0x06000788 RID: 1928 RVA: 0x0002AB48 File Offset: 0x00028D48
		private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
		{
			if (this.mSelectAllBtn.IsChecked.Value)
			{
				using (IEnumerator enumerator = this.mScriptsStackPanel.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						(obj as ImportMacroScriptsControl).mContent.IsChecked = new bool?(true);
					}
					return;
				}
			}
			foreach (object obj2 in this.mScriptsStackPanel.Children)
			{
				(obj2 as ImportMacroScriptsControl).mContent.IsChecked = new bool?(false);
			}
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0002AC18 File Offset: 0x00028E18
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/importmacrowindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0002AC48 File Offset: 0x00028E48
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
				((ImportMacroWindow)target).Closing += this.ImportMacroWindow_Closing;
				return;
			case 2:
				this.mMaskBorder = (Border)target;
				return;
			case 3:
				((CustomPictureBox)target).MouseLeftButtonUp += this.Close_MouseLeftButtonUp;
				return;
			case 4:
				this.mScriptsListScrollbar = (ScrollViewer)target;
				return;
			case 5:
				this.mSelectAllBtn = (CustomCheckbox)target;
				this.mSelectAllBtn.Click += this.SelectAllBtn_Click;
				return;
			case 6:
				this.mImportBtn = (CustomButton)target;
				this.mImportBtn.Click += this.ImportBtn_Click;
				return;
			case 7:
				this.mLoadingGrid = (ProgressBar)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000409 RID: 1033
		private MacroRecorderWindow mOperationWindow;

		// Token: 0x0400040A RID: 1034
		private MainWindow ParentWindow;

		// Token: 0x0400040B RID: 1035
		internal StackPanel mScriptsStackPanel;

		// Token: 0x0400040C RID: 1036
		internal int mNumberOfFilesSelectedForImport;

		// Token: 0x0400040D RID: 1037
		private Dictionary<ImportMacroScriptsControl, MacroRecording> mBoxToRecordingDict = new Dictionary<ImportMacroScriptsControl, MacroRecording>();

		// Token: 0x0400040E RID: 1038
		private Dictionary<CustomTextBox, MacroRecording> mDependentRecordingDict = new Dictionary<CustomTextBox, MacroRecording>();

		// Token: 0x0400040F RID: 1039
		private bool mInited;

		// Token: 0x04000410 RID: 1040
		internal bool mIsInDependentFileFindingMode;

		// Token: 0x04000411 RID: 1041
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000412 RID: 1042
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mScriptsListScrollbar;

		// Token: 0x04000413 RID: 1043
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mSelectAllBtn;

		// Token: 0x04000414 RID: 1044
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mImportBtn;

		// Token: 0x04000415 RID: 1045
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ProgressBar mLoadingGrid;

		// Token: 0x04000416 RID: 1046
		private bool _contentLoaded;
	}
}
