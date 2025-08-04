using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000CA RID: 202
	public class MergeMacroWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x1700022B RID: 555
		// (get) Token: 0x0600084D RID: 2125 RVA: 0x00007605 File Offset: 0x00005805
		// (set) Token: 0x0600084E RID: 2126 RVA: 0x0000760D File Offset: 0x0000580D
		internal MacroRecording MergedMacroRecording { get; set; }

		// Token: 0x0600084F RID: 2127 RVA: 0x00007616 File Offset: 0x00005816
		public MergeMacroWindow(MacroRecorderWindow window, MainWindow mainWindow)
		{
			this.InitializeComponent();
			this.mMacroRecorderWindow = window;
			this.ParentWindow = mainWindow;
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x0002E5BC File Offset: 0x0002C7BC
		internal void Init(MacroRecording mergedMacro = null, SingleMacroControl singleMacroControl = null)
		{
			try
			{
				this.mMacroNameStackPanel.Visibility = ((mergedMacro == null) ? Visibility.Visible : Visibility.Collapsed);
				this.mOriginalMacroRecording = mergedMacro;
				int num = 0;
				using (List<MacroRecording>.Enumerator enumerator = (from MacroRecording o in MacroGraph.Instance.Vertices
					orderby DateTime.ParseExact(o.TimeCreated, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
					select o).ToList<MacroRecording>().GetEnumerator())
				{
					Func<BiDirectionalVertex<MacroRecording>, bool> <>9__1;
					while (enumerator.MoveNext())
					{
						MacroRecording record = enumerator.Current;
						this.ParentWindow.mIsScriptsPresent = true;
						if (!record.Equals(mergedMacro))
						{
							if (mergedMacro != null)
							{
								BiDirectionalGraph<MacroRecording> instance = MacroGraph.Instance;
								IEnumerable<BiDirectionalVertex<MacroRecording>> vertices = MacroGraph.Instance.Vertices;
								Func<BiDirectionalVertex<MacroRecording>, bool> func;
								if ((func = <>9__1) == null)
								{
									func = (<>9__1 = (BiDirectionalVertex<MacroRecording> macro) => macro.Equals(mergedMacro));
								}
								if (instance.DoesParentExist(vertices.Where(func).FirstOrDefault<BiDirectionalVertex<MacroRecording>>(), MacroGraph.Instance.Vertices.Where((BiDirectionalVertex<MacroRecording> macro) => macro.Equals(record)).FirstOrDefault<BiDirectionalVertex<MacroRecording>>()))
								{
									continue;
								}
							}
							MacroToAdd macroToAdd = new MacroToAdd(this, record.Name);
							if (num % 2 == 0)
							{
								BlueStacksUIBinding.BindColor(macroToAdd, Control.BackgroundProperty, "DarkBandingColor");
							}
							else
							{
								BlueStacksUIBinding.BindColor(macroToAdd, Control.BackgroundProperty, "LightBandingColor");
							}
							this.mCurrentMacroScripts.Children.Add(macroToAdd);
							num++;
						}
					}
				}
				if (singleMacroControl != null)
				{
					this.mSingleMacroControl = singleMacroControl;
				}
				if (mergedMacro == null)
				{
					string text = DateTime.Now.ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
					this.MergedMacroRecording = new MacroRecording
					{
						Name = CommonHandlers.GetMacroName("Macro"),
						TimeCreated = text,
						MergedMacroConfigurations = new ObservableCollection<MergedMacroConfiguration>()
					};
					this.mUnifyButton.Visibility = Visibility.Collapsed;
					BlueStacksUIBinding.Bind(this.mMergeButton, "STRING_MERGE");
					BlueStacksUIBinding.Bind(this.mMergeMacroWindowHeading, "STRING_MERGE_MACROS", "");
				}
				else
				{
					this.MergedMacroRecording = mergedMacro.DeepCopy<MacroRecording>();
					BlueStacksUIBinding.Bind(this.mMergeButton, "STRING_UPDATE_SETTING");
					BlueStacksUIBinding.Bind(this.mMergeMacroWindowHeading, "STRING_EDIT_MERGED_MACRO", "");
					this.mUnifyButton.Visibility = Visibility.Visible;
				}
				this.MacroName.Text = this.MergedMacroRecording.Name;
				this.mMacroDragControl.Init();
				this.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged -= this.Items_CollectionChanged;
				this.MergedMacroRecording.MergedMacroConfigurations.CollectionChanged += this.Items_CollectionChanged;
				this.Items_CollectionChanged(null, null);
				this.DataModificationTracker.Lock(this.mOriginalMacroRecording, new List<string> { "IsGroupButtonVisible", "IsUnGroupButtonVisible", "IsSettingsVisible", "IsFirstListBoxItem", "IsLastListBoxItem", "Parents", "Childs", "IsVisited" }, true);
				this.CheckIfCanSave();
			}
			catch (Exception ex)
			{
				Logger.Error("Error in export window init err: " + ex.ToString());
			}
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0002E940 File Offset: 0x0002CB40
		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args != null)
			{
				if (args.OldItems != null)
				{
					foreach (object obj in args.OldItems)
					{
						((INotifyPropertyChanged)obj).PropertyChanged -= this.Item_PropertyChanged;
					}
				}
				if (args.NewItems == null)
				{
					goto IL_00E1;
				}
				using (IEnumerator enumerator = args.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj2 = enumerator.Current;
						((INotifyPropertyChanged)obj2).PropertyChanged += this.Item_PropertyChanged;
					}
					goto IL_00E1;
				}
			}
			foreach (MergedMacroConfiguration mergedMacroConfiguration in this.MergedMacroRecording.MergedMacroConfigurations)
			{
				mergedMacroConfiguration.PropertyChanged += this.Item_PropertyChanged;
			}
			IL_00E1:
			foreach (MergedMacroConfiguration mergedMacroConfiguration2 in this.MergedMacroRecording.MergedMacroConfigurations)
			{
				mergedMacroConfiguration2.IsGroupButtonVisible = true;
				mergedMacroConfiguration2.IsFirstListBoxItem = false;
				mergedMacroConfiguration2.IsLastListBoxItem = false;
				mergedMacroConfiguration2.IsUnGroupButtonVisible = mergedMacroConfiguration2.MacrosToRun.Count > 1;
			}
			if (this.MergedMacroRecording.MergedMacroConfigurations.Count > 0)
			{
				this.MergedMacroRecording.MergedMacroConfigurations[0].IsGroupButtonVisible = false;
				this.MergedMacroRecording.MergedMacroConfigurations[0].IsFirstListBoxItem = true;
				this.MergedMacroRecording.MergedMacroConfigurations[this.MergedMacroRecording.MergedMacroConfigurations.Count - 1].IsLastListBoxItem = true;
				this.MergedMacroRecording.MergedMacroConfigurations[this.MergedMacroRecording.MergedMacroConfigurations.Count - 1].DelayNextScript = 0;
				this.mMergedMacrosHeader.Visibility = Visibility.Visible;
				this.mHelpCenterImage.Visibility = Visibility.Visible;
				this.mMergedMacrosFooter.IsEnabled = true;
			}
			else
			{
				this.mMergedMacrosHeader.Visibility = Visibility.Collapsed;
				this.mHelpCenterImage.Visibility = Visibility.Collapsed;
				this.mMergedMacrosFooter.IsEnabled = false;
			}
			this.CheckIfCanSave();
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x0000763D File Offset: 0x0000583D
		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			this.CheckIfCanSave();
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0002EB9C File Offset: 0x0002CD9C
		private void CheckIfCanSave()
		{
			bool flag = this.MergedMacroRecording.MergedMacroConfigurations.Count > 0 && (this.MergedMacroRecording.MergedMacroConfigurations.Count > 1 || this.MergedMacroRecording.MergedMacroConfigurations[0].MacrosToRun.Count > 1);
			UIElement uielement = this.mMergeButton;
			bool flag2;
			if ((this.mMacroNameStackPanel.Visibility == Visibility.Collapsed || this.MacroName.InputTextValidity == TextValidityOptions.Success) && flag)
			{
				if (this.MergedMacroRecording.MergedMacroConfigurations.All((MergedMacroConfiguration macro) => macro.LoopCount > 0))
				{
					flag2 = this.DataModificationTracker.HasChanged(this.MergedMacroRecording);
					goto IL_00B9;
				}
			}
			flag2 = false;
			IL_00B9:
			uielement.IsEnabled = flag2;
			this.mUnifyButton.IsEnabled = flag;
			this.mMacroSettings.IsEnabled = flag;
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x0002EC80 File Offset: 0x0002CE80
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_close", null, null, null, null, null);
			this.CloseWindow();
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x00007645 File Offset: 0x00005845
		private void CloseWindow()
		{
			base.Close();
			this.mMacroRecorderWindow.mMergeMacroWindow = null;
			this.mMacroRecorderWindow.mOverlayGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x0002ECBC File Offset: 0x0002CEBC
		private void MergeButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.mOriginalMacroRecording == null)
			{
				this.mOriginalMacroRecording = new MacroRecording();
			}
			this.mOriginalMacroRecording.CopyFrom(this.MergedMacroRecording);
			this.mMacroRecorderWindow.SaveMacroRecord(this.mOriginalMacroRecording);
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_success", null, null, null, null, null);
			this.CloseWindow();
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x0002ED2C File Offset: 0x0002CF2C
		private void MacroName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.mMacroNameStackPanel.Visibility == Visibility.Visible)
			{
				if (string.IsNullOrEmpty(this.MacroName.Text.Trim()))
				{
					this.mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_NULL_MESSAGE", "");
					this.MacroName.InputTextValidity = TextValidityOptions.Error;
				}
				else if (this.MacroName.Text.Trim().IndexOfAny(global::System.IO.Path.GetInvalidFileNameChars()) >= 0)
				{
					this.mErrorText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[]
					{
						LocaleStrings.GetLocalizedString("STRING_MACRO_NAME_ERROR", ""),
						Environment.NewLine,
						"\\ / : * ? \" < > |"
					});
					this.MacroName.InputTextValidity = TextValidityOptions.Error;
				}
				else if (Constants.ReservedFileNamesList.Contains(this.MacroName.Text.Trim().ToLower(CultureInfo.InvariantCulture)))
				{
					this.mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_FILE_NAME_ERROR", "");
					this.MacroName.InputTextValidity = TextValidityOptions.Error;
				}
				else if (MacroGraph.Instance.Vertices.Cast<MacroRecording>().Any((MacroRecording macro) => string.Equals(macro.Name, this.MacroName.Text.Trim(), StringComparison.InvariantCultureIgnoreCase)))
				{
					this.mErrorText.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_NOT_SAVED_MESSAGE", "");
					this.MacroName.InputTextValidity = TextValidityOptions.Error;
				}
				else
				{
					this.MacroName.InputTextValidity = TextValidityOptions.Success;
				}
				this.mErrorNamePopup.IsOpen = this.MacroName.InputTextValidity == TextValidityOptions.Error;
				this.MergedMacroRecording.Name = this.MacroName.Text;
				this.CheckIfCanSave();
			}
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x0002EED4 File Offset: 0x0002D0D4
		private void MacroSettings_Click(object sender, RoutedEventArgs e)
		{
			ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_macro_settings", null, null, null, null, null);
			if (this.mMacroSettingsWindow == null || this.mMacroSettingsWindow.IsClosed)
			{
				this.mMacroSettingsWindow = new MacroSettingsWindow(this.ParentWindow, this.MergedMacroRecording, this.mMacroRecorderWindow);
				this.mMacroSettingsWindow.Closed += delegate(object o, EventArgs e)
				{
					this.CheckIfCanSave();
				};
			}
			this.mMacroSettingsWindow.ShowDialog();
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x0002EF60 File Offset: 0x0002D160
		private void UnifyButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.mOriginalMacroRecording == null)
			{
				this.mOriginalMacroRecording = new MacroRecording();
			}
			this.mOriginalMacroRecording.CopyFrom(this.MergedMacroRecording);
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.TitleTextBlock.Text = string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_UNIFY_0", ""), new object[] { this.mOriginalMacroRecording.Name });
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_UNIFIYING_LOSE_CONFIGURE", "");
			bool closeWindow = false;
			customMessageWindow.AddButton(ButtonColors.Blue, string.Format(CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_CONTINUE", ""), new object[] { "" }).Trim(), delegate(object o, EventArgs evt)
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify", null, null, null, null, null);
				this.mMacroRecorderWindow.FlattenRecording(this.mOriginalMacroRecording, false);
				CommonHandlers.SaveMacroJson(this.mOriginalMacroRecording, this.mOriginalMacroRecording.Name + ".json");
				CommonHandlers.RefreshAllMacroRecorderWindow();
				closeWindow = true;
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", delegate(object o, EventArgs evt)
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify_cancel", null, null, null, null, null);
			}, null, false, null);
			customMessageWindow.CloseButtonHandle(delegate(object o, EventArgs e)
			{
				ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "merge_unify_cancel", null, null, null, null, null);
			}, null);
			customMessageWindow.Owner = this;
			customMessageWindow.ShowDialog();
			if (closeWindow)
			{
				this.CloseWindow();
			}
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x0000766A File Offset: 0x0000586A
		private void MacroName_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.MacroName.InputTextValidity == TextValidityOptions.Error)
			{
				this.mErrorNamePopup.IsOpen = true;
				this.mErrorNamePopup.StaysOpen = true;
				return;
			}
			this.mErrorNamePopup.IsOpen = false;
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x0000769F File Offset: 0x0000589F
		private void MacroName_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mErrorNamePopup.IsOpen = false;
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x000076AD File Offset: 0x000058AD
		private void mHelpCenterImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=MergeMacro_Help");
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x0002F0AC File Offset: 0x0002D2AC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/mergemacrowindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x0002F0DC File Offset: 0x0002D2DC
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
				this.mMergeMacroWindowHeading = (TextBlock)target;
				return;
			case 3:
				this.mUnifyButton = (CustomButton)target;
				this.mUnifyButton.Click += this.UnifyButton_Click;
				return;
			case 4:
				((CustomPictureBox)target).MouseLeftButtonUp += this.Close_MouseLeftButtonUp;
				return;
			case 5:
				this.mCurrentMacroScripts = (StackPanel)target;
				return;
			case 6:
				this.mLineSeperator = (Line)target;
				return;
			case 7:
				this.mMergedMacrosHeader = (TextBlock)target;
				return;
			case 8:
				this.mHelpCenterImage = (CustomPictureBox)target;
				this.mHelpCenterImage.MouseDown += this.mHelpCenterImage_MouseDown;
				return;
			case 9:
				this.mMacroDragControl = (MacroAddedDragControl)target;
				return;
			case 10:
				this.mMergedMacrosFooter = (StackPanel)target;
				return;
			case 11:
				this.mMacroNameStackPanel = (StackPanel)target;
				return;
			case 12:
				this.MacroName = (CustomTextBox)target;
				this.MacroName.MouseEnter += this.MacroName_MouseEnter;
				this.MacroName.MouseLeave += this.MacroName_MouseLeave;
				this.MacroName.TextChanged += this.MacroName_TextChanged;
				return;
			case 13:
				this.mMacroSettings = (CustomButton)target;
				this.mMacroSettings.Click += this.MacroSettings_Click;
				return;
			case 14:
				this.mMergeButton = (CustomButton)target;
				this.mMergeButton.Click += this.MergeButton_Click;
				return;
			case 15:
				this.mErrorNamePopup = (CustomPopUp)target;
				return;
			case 16:
				this.mMaskBorder1 = (Border)target;
				return;
			case 17:
				this.mErrorText = (TextBlock)target;
				return;
			case 18:
				this.mDownArrow = (global::System.Windows.Shapes.Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000496 RID: 1174
		private readonly DataModificationTracker DataModificationTracker = new DataModificationTracker();

		// Token: 0x04000497 RID: 1175
		private MacroRecorderWindow mMacroRecorderWindow;

		// Token: 0x04000498 RID: 1176
		private MainWindow ParentWindow;

		// Token: 0x04000499 RID: 1177
		private MacroRecording mOriginalMacroRecording;

		// Token: 0x0400049B RID: 1179
		internal int mAddedMacroTag;

		// Token: 0x0400049C RID: 1180
		private MacroSettingsWindow mMacroSettingsWindow;

		// Token: 0x0400049D RID: 1181
		private SingleMacroControl mSingleMacroControl;

		// Token: 0x0400049E RID: 1182
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x0400049F RID: 1183
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMergeMacroWindowHeading;

		// Token: 0x040004A0 RID: 1184
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mUnifyButton;

		// Token: 0x040004A1 RID: 1185
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mCurrentMacroScripts;

		// Token: 0x040004A2 RID: 1186
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Line mLineSeperator;

		// Token: 0x040004A3 RID: 1187
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMergedMacrosHeader;

		// Token: 0x040004A4 RID: 1188
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mHelpCenterImage;

		// Token: 0x040004A5 RID: 1189
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal MacroAddedDragControl mMacroDragControl;

		// Token: 0x040004A6 RID: 1190
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mMergedMacrosFooter;

		// Token: 0x040004A7 RID: 1191
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mMacroNameStackPanel;

		// Token: 0x040004A8 RID: 1192
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox MacroName;

		// Token: 0x040004A9 RID: 1193
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mMacroSettings;

		// Token: 0x040004AA RID: 1194
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mMergeButton;

		// Token: 0x040004AB RID: 1195
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mErrorNamePopup;

		// Token: 0x040004AC RID: 1196
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder1;

		// Token: 0x040004AD RID: 1197
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mErrorText;

		// Token: 0x040004AE RID: 1198
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal global::System.Windows.Shapes.Path mDownArrow;

		// Token: 0x040004AF RID: 1199
		private bool _contentLoaded;
	}
}
