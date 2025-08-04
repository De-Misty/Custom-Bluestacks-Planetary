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
using Newtonsoft.Json.Linq;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000162 RID: 354
	public class ImportSchemesWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x06000EA8 RID: 3752 RVA: 0x0000AE90 File Offset: 0x00009090
		public ImportSchemesWindow(KeymapCanvasWindow window, MainWindow mainWindow)
		{
			this.InitializeComponent();
			this.CanvasWindow = window;
			this.ParentWindow = mainWindow;
			this.mSchemesStackPanel = this.mSchemesListScrollbar.Content as StackPanel;
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x0000AECD File Offset: 0x000090CD
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.CloseWindow();
		}

		// Token: 0x06000EAA RID: 3754 RVA: 0x0000AED5 File Offset: 0x000090D5
		private void CloseWindow()
		{
			base.Close();
			this.CanvasWindow.SidebarWindow.mImportSchemesWindow = null;
			this.CanvasWindow.SidebarWindow.mOverlayGrid.Visibility = Visibility.Hidden;
			this.CanvasWindow.SidebarWindow.Focus();
		}

		// Token: 0x06000EAB RID: 3755 RVA: 0x0005CD08 File Offset: 0x0005AF08
		internal void Init(string fileName)
		{
			try
			{
				List<string> schemeNames = new List<string>();
				foreach (IMControlScheme imcontrolScheme in this.ParentWindow.SelectedConfig.ControlSchemes)
				{
					schemeNames.Add(imcontrolScheme.Name);
				}
				this.mSchemesStackPanel.Children.Clear();
				JObject jobject = JObject.Parse(File.ReadAllText(fileName));
				int? num = ConfigConverter.GetConfigVersion(jobject);
				int num2 = 14;
				IMConfig imconfig;
				if ((num.GetValueOrDefault() < num2) & (num != null))
				{
					object obj = ConfigConverter.Convert(jobject, "14", false, true);
					JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
					serializerSettings.Formatting = Formatting.Indented;
					imconfig = KMManager.GetDeserializedIMConfigObject(JsonConvert.SerializeObject(obj, serializerSettings), false);
				}
				else
				{
					num = ConfigConverter.GetConfigVersion(jobject);
					num2 = 16;
					if (((num.GetValueOrDefault() < num2) & (num != null)) && (string.Equals(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName, "com.dts.freefireth", StringComparison.InvariantCultureIgnoreCase) || PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName)))
					{
						JObject jobject2 = jobject;
						foreach (JToken jtoken in ((IEnumerable<JToken>)jobject["ControlSchemes"]))
						{
							JObject jobject3 = (JObject)jtoken;
							jobject3["Images"] = ConfigConverter.ConvertImagesArrayForPV16(jobject3);
						}
						jobject2["MetaData"]["Comment"] = string.Format(CultureInfo.InvariantCulture, "Generated automatically from ver {0}", new object[] { (int)jobject2["MetaData"]["ParserVersion"] });
						jobject2["MetaData"]["ParserVersion"] = 16;
						JsonSerializerSettings serializerSettings2 = Utils.GetSerializerSettings();
						serializerSettings2.Formatting = Formatting.Indented;
						imconfig = KMManager.GetDeserializedIMConfigObject(JsonConvert.SerializeObject(jobject2, serializerSettings2), false);
					}
					else
					{
						imconfig = KMManager.GetDeserializedIMConfigObject(fileName, true);
					}
				}
				this.mStringsToImport = imconfig.Strings;
				this.mNumberOfSchemesSelectedForImport = 0;
				imconfig.ControlSchemes.Where((IMControlScheme scheme) => scheme.BuiltIn).ToList<IMControlScheme>().ForEach(delegate(IMControlScheme scheme)
				{
					base.<Init>g__AddSchemeToImportCheckbox|4(scheme);
				});
				imconfig.ControlSchemes.Where((IMControlScheme scheme) => !scheme.BuiltIn).ToList<IMControlScheme>().ForEach(delegate(IMControlScheme scheme)
				{
					if (this.dict.Keys.Contains(scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim()))
					{
						scheme.Name += " (Edited)";
						scheme.Name = KMManager.GetUniqueName(scheme.Name, schemeNames);
					}
					base.<Init>g__AddSchemeToImportCheckbox|4(scheme);
				});
			}
			catch (Exception ex)
			{
				Logger.Error("Error in import window init err: " + ex.ToString());
			}
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x0005D034 File Offset: 0x0005B234
		internal void Box_Unchecked(object sender, RoutedEventArgs e)
		{
			this.mNumberOfSchemesSelectedForImport--;
			if (this.mNumberOfSchemesSelectedForImport == this.mSchemesStackPanel.Children.Count - 1)
			{
				this.mSelectAllBtn.IsChecked = new bool?(false);
			}
			if (this.mNumberOfSchemesSelectedForImport == 0)
			{
				this.mImportBtn.IsEnabled = false;
			}
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0005D090 File Offset: 0x0005B290
		internal void Box_Checked(object sender, RoutedEventArgs e)
		{
			this.mNumberOfSchemesSelectedForImport++;
			if (this.mNumberOfSchemesSelectedForImport == this.mSchemesStackPanel.Children.Count)
			{
				this.mSelectAllBtn.IsChecked = new bool?(true);
			}
			if (this.mNumberOfSchemesSelectedForImport == 1)
			{
				this.mImportBtn.IsEnabled = true;
			}
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x0005D0EC File Offset: 0x0005B2EC
		private bool EditedNameIsAllowed(string text, ImportSchemesWindowControl item)
		{
			if (string.IsNullOrEmpty(text.Trim()))
			{
				BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), "");
				return false;
			}
			if (text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				BlueStacksUIBinding.Bind(item.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), "");
				return false;
			}
			using (List<IMControlScheme>.Enumerator enumerator = this.ParentWindow.SelectedConfig.ControlSchemes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Name.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
					{
						return false;
					}
				}
			}
			foreach (object obj in this.mSchemesStackPanel.Children)
			{
				ImportSchemesWindowControl importSchemesWindowControl = (ImportSchemesWindowControl)obj;
				bool? flag = importSchemesWindowControl.mContent.IsChecked;
				bool flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && importSchemesWindowControl.mBlock.Visibility == Visibility.Visible && importSchemesWindowControl.mImportName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim() && importSchemesWindowControl.mContent.Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture) != item.mContent.Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture))
				{
					return false;
				}
				flag = importSchemesWindowControl.mContent.IsChecked;
				flag2 = true;
				if (((flag.GetValueOrDefault() == flag2) & (flag != null)) && importSchemesWindowControl.mContent.Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim())
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x0005D350 File Offset: 0x0005B550
		private void ImportBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int num = 0;
				bool flag = true;
				List<IMControlScheme> list = new List<IMControlScheme>();
				foreach (object obj in this.mSchemesStackPanel.Children)
				{
					ImportSchemesWindowControl importSchemesWindowControl = (ImportSchemesWindowControl)obj;
					bool? isChecked = importSchemesWindowControl.mContent.IsChecked;
					bool flag2 = true;
					if ((isChecked.GetValueOrDefault() == flag2) & (isChecked != null))
					{
						list.Add(this.dict.ElementAt(num).Value);
						if (this.ParentWindow.SelectedConfig.ControlSchemesDict.Keys.Select((string key) => key.ToLower(CultureInfo.InvariantCulture).Trim()).Contains(importSchemesWindowControl.mContent.Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim()))
						{
							if (!this.EditedNameIsAllowed(importSchemesWindowControl.mImportName.Text, importSchemesWindowControl))
							{
								importSchemesWindowControl.mImportName.InputTextValidity = TextValidityOptions.Error;
								if (!string.IsNullOrEmpty(importSchemesWindowControl.mImportName.Text) && importSchemesWindowControl.mImportName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
								{
									BlueStacksUIBinding.Bind(importSchemesWindowControl.mWarningMsg, LocaleStrings.GetLocalizedString("STRING_DUPLICATE_SCHEME_NAME_WARNING", ""), "");
								}
								importSchemesWindowControl.mWarningMsg.Visibility = Visibility.Visible;
								flag = false;
							}
							else
							{
								importSchemesWindowControl.mImportName.InputTextValidity = TextValidityOptions.Success;
								importSchemesWindowControl.mWarningMsg.Visibility = Visibility.Collapsed;
							}
						}
					}
					num++;
				}
				if (list.Count == 0)
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_SCHEME_SELECTED", ""), 1.3, false);
				}
				else if (flag)
				{
					foreach (IMControlScheme imcontrolScheme in list)
					{
						ImportSchemesWindowControl controlFromScheme = this.GetControlFromScheme(imcontrolScheme);
						if (this.ParentWindow.SelectedConfig.ControlSchemesDict.Keys.Select((string key) => key.ToLower(CultureInfo.InvariantCulture)).Contains(controlFromScheme.mContent.Content.ToString().ToLower(CultureInfo.InvariantCulture).Trim()))
						{
							imcontrolScheme.Name = controlFromScheme.mImportName.Text.Trim();
						}
					}
					this.mStringsToImport = KMManager.CleanupGuidanceAccordingToSchemes(list, this.mStringsToImport);
					this.ImportSchemes(list, this.mStringsToImport);
					KeymapCanvasWindow.sIsDirty = true;
					KMManager.SaveIMActions(this.ParentWindow, false, false);
					this.CanvasWindow.SidebarWindow.FillProfileCombo();
					this.CanvasWindow.SidebarWindow.ProfileChanged();
					KMManager.SendSchemeChangedStats(this.ParentWindow, "import_scheme");
					this.ParentWindow.mCommonHandler.AddToastPopup(this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_CONTROLS_IMPORTED", ""), 1.3, false);
					this.CloseWindow();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while importing script. err:" + ex.ToString());
			}
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x0005D6EC File Offset: 0x0005B8EC
		private ImportSchemesWindowControl GetControlFromScheme(IMControlScheme scheme)
		{
			foreach (object obj in this.mSchemesStackPanel.Children)
			{
				ImportSchemesWindowControl importSchemesWindowControl = (ImportSchemesWindowControl)obj;
				if (importSchemesWindowControl.mContent.Content.ToString().Trim().ToLower(CultureInfo.InvariantCulture) == scheme.Name.ToLower(CultureInfo.InvariantCulture).Trim())
				{
					return importSchemesWindowControl;
				}
			}
			return null;
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x0005D788 File Offset: 0x0005B988
		private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
		{
			if (this.mSelectAllBtn.IsChecked.Value)
			{
				using (IEnumerator enumerator = this.mSchemesStackPanel.Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						((ImportSchemesWindowControl)obj).mContent.IsChecked = new bool?(true);
					}
					return;
				}
			}
			foreach (object obj2 in this.mSchemesStackPanel.Children)
			{
				((ImportSchemesWindowControl)obj2).mContent.IsChecked = new bool?(false);
			}
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0005D858 File Offset: 0x0005BA58
		internal void ImportSchemes(List<IMControlScheme> toCopyFromSchemes, Dictionary<string, Dictionary<string, string>> stringsToImport)
		{
			bool flag = false;
			bool flag2 = false;
			KMManager.MergeConflictingGuidanceStrings(this.ParentWindow.SelectedConfig, toCopyFromSchemes, stringsToImport);
			if (this.ParentWindow.SelectedConfig.ControlSchemes.Count > 0)
			{
				flag = true;
			}
			foreach (IMControlScheme imcontrolScheme in toCopyFromSchemes)
			{
				IMControlScheme imcontrolScheme2 = imcontrolScheme.DeepCopy();
				if (flag)
				{
					imcontrolScheme2.Selected = false;
				}
				imcontrolScheme2.BuiltIn = false;
				imcontrolScheme2.IsBookMarked = false;
				this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = imcontrolScheme2.Name;
				this.ParentWindow.SelectedConfig.ControlSchemes.Add(imcontrolScheme2);
				this.ParentWindow.SelectedConfig.ControlSchemesDict.Add(imcontrolScheme2.Name, imcontrolScheme2);
				ComboBoxSchemeControl comboBoxSchemeControl = new ComboBoxSchemeControl(this.CanvasWindow, this.ParentWindow);
				comboBoxSchemeControl.mSchemeName.Text = LocaleStrings.GetLocalizedString(imcontrolScheme2.Name, "");
				comboBoxSchemeControl.IsEnabled = true;
				BlueStacksUIBinding.BindColor(comboBoxSchemeControl, Control.BackgroundProperty, "ComboBoxBackgroundColor");
				this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children.Add(comboBoxSchemeControl);
			}
			if (!flag)
			{
				using (List<IMControlScheme>.Enumerator enumerator = this.ParentWindow.SelectedConfig.ControlSchemes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Selected)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					this.ParentWindow.SelectedConfig.ControlSchemes[0].Selected = true;
				}
			}
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0005DA20 File Offset: 0x0005BC20
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/importschemeswindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x0005DA50 File Offset: 0x0005BC50
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
				this.mImportBtn = (CustomButton)target;
				this.mImportBtn.Click += this.ImportBtn_Click;
				return;
			case 6:
				this.mLoadingGrid = (ProgressBar)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000956 RID: 2390
		private KeymapCanvasWindow CanvasWindow;

		// Token: 0x04000957 RID: 2391
		private MainWindow ParentWindow;

		// Token: 0x04000958 RID: 2392
		internal StackPanel mSchemesStackPanel;

		// Token: 0x04000959 RID: 2393
		internal int mNumberOfSchemesSelectedForImport;

		// Token: 0x0400095A RID: 2394
		private Dictionary<string, IMControlScheme> dict = new Dictionary<string, IMControlScheme>();

		// Token: 0x0400095B RID: 2395
		private Dictionary<string, Dictionary<string, string>> mStringsToImport;

		// Token: 0x0400095C RID: 2396
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x0400095D RID: 2397
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mSchemesListScrollbar;

		// Token: 0x0400095E RID: 2398
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mSelectAllBtn;

		// Token: 0x0400095F RID: 2399
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mImportBtn;

		// Token: 0x04000960 RID: 2400
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ProgressBar mLoadingGrid;

		// Token: 0x04000961 RID: 2401
		private bool _contentLoaded;
	}
}
