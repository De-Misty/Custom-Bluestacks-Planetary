using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000C6 RID: 198
	public class MacroSettingsWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x060007EF RID: 2031 RVA: 0x0002C420 File Offset: 0x0002A620
		internal MacroSettingsWindow(MainWindow window, MacroRecording record, MacroRecorderWindow singleMacroControl)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			base.Owner = this.ParentWindow;
			this.mMacroRecorderWindow = singleMacroControl;
			this.mRecording = record;
			InputMethod.SetIsInputMethodEnabled(this.mLoopCountTextBox, false);
			InputMethod.SetIsInputMethodEnabled(this.mLoopHours, false);
			InputMethod.SetIsInputMethodEnabled(this.mLoopMinutes, false);
			InputMethod.SetIsInputMethodEnabled(this.mLoopSeconds, false);
			InputMethod.SetIsInputMethodEnabled(this.mLoopIntervalMinsTextBox, false);
			InputMethod.SetIsInputMethodEnabled(this.mRestartPlayerIntervalTextBox, false);
			InputMethod.SetIsInputMethodEnabled(this.mLoopIntervalSecondsTextBox, false);
			this.InitSettings();
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0002C4B4 File Offset: 0x0002A6B4
		private void InitSettings()
		{
			this.mSettingsHeaderText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.mRecording.Name,
				LocaleStrings.GetLocalizedString("STRING_SETTINGS", "").ToLower(CultureInfo.InvariantCulture)
			});
			this.mLoopCountTextBox.Text = this.mRecording.LoopNumber.ToString(CultureInfo.InvariantCulture);
			this.mLoopHours.Text = (this.mRecording.LoopTime / 3600).ToString(CultureInfo.InvariantCulture);
			this.mLoopMinutes.Text = (this.mRecording.LoopTime / 60 % 60).ToString(CultureInfo.InvariantCulture);
			this.mLoopSeconds.Text = (this.mRecording.LoopTime % 60).ToString(CultureInfo.InvariantCulture);
			this.mLoopIntervalMinsTextBox.Text = (this.mRecording.LoopInterval / 60).ToString(CultureInfo.InvariantCulture);
			this.mLoopIntervalSecondsTextBox.Text = (this.mRecording.LoopInterval % 60).ToString(CultureInfo.InvariantCulture);
			this.mRestartPlayerCheckBox.IsChecked = new bool?(this.mRecording.RestartPlayer);
			this.mPlayOnStartCheckBox.IsChecked = new bool?(this.mRecording.PlayOnStart);
			this.mDonotShowWindowOnFinishCheckBox.IsChecked = new bool?(this.mRecording.DonotShowWindowOnFinish);
			this.mRestartPlayerIntervalTextBox.Text = this.mRecording.RestartPlayerAfterMinutes.ToString(CultureInfo.InvariantCulture);
			this.mAccelerationCombobox.Items.Clear();
			for (int i = 0; i <= 8; i++)
			{
				ComboBoxItem comboBoxItem = new ComboBoxItem();
				comboBoxItem.Content = ((double)(i + 2) * 0.5).ToString(CultureInfo.InvariantCulture) + "x";
				this.mAccelerationCombobox.Items.Add(comboBoxItem);
			}
			if (this.mRecording.Acceleration == 0.0)
			{
				this.mAccelerationCombobox.SelectedIndex = 0;
			}
			else
			{
				this.mAccelerationCombobox.SelectedIndex = (int)(this.mRecording.Acceleration / 0.5 - 2.0);
			}
			this.mRestartTextBlock.ToolTip = string.Format(CultureInfo.InvariantCulture, string.Concat(new string[]
			{
				LocaleStrings.GetLocalizedString("STRING_AFTER", ""),
				" ",
				this.mRestartPlayerIntervalTextBox.Text,
				" ",
				LocaleStrings.GetLocalizedString("STRING_RESTART_PLAYER_AFTER", "")
			}), new object[0]);
			this.SelectRepeatExecutionSetting();
			this.mLoopCountTextBox.TextChanged += this.LoopCountTextBox_TextChanged;
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0002C79C File Offset: 0x0002A99C
		private void SelectRepeatExecutionSetting()
		{
			switch (this.mRecording.LoopType)
			{
			case OperationsLoopType.TillLoopNumber:
				this.mRepeatActionTimePanelGrid.IsEnabled = false;
				this.mRepeatActionInSession.IsChecked = new bool?(true);
				return;
			case OperationsLoopType.TillTime:
				this.mRepeatActionInSessionGrid.IsEnabled = false;
				this.mRepeatActionTime.IsChecked = new bool?(true);
				return;
			case OperationsLoopType.UntilStopped:
				this.mRepeatActionTimePanelGrid.IsEnabled = false;
				this.mRepeatActionInSessionGrid.IsEnabled = false;
				this.mRepeatSessionInfinite.IsChecked = new bool?(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x00007197 File Offset: 0x00005397
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.IsMacroSettingsChanged())
			{
				this.GetUnsavedChangesWindow().ShowDialog();
			}
			this.CloseWindow();
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0002C830 File Offset: 0x0002AA30
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.mLoopHours.Text))
			{
				this.mLoopHours.Text = "0";
			}
			if (string.IsNullOrEmpty(this.mLoopMinutes.Text))
			{
				this.mLoopMinutes.Text = "0";
			}
			if (string.IsNullOrEmpty(this.mLoopSeconds.Text))
			{
				this.mLoopSeconds.Text = "0";
			}
			if (string.IsNullOrEmpty(this.mLoopCountTextBox.Text))
			{
				this.mLoopCountTextBox.Text = "0";
			}
			if (string.IsNullOrEmpty(this.mLoopIntervalMinsTextBox.Text))
			{
				this.mLoopIntervalMinsTextBox.Text = "0";
			}
			if (string.IsNullOrEmpty(this.mLoopIntervalSecondsTextBox.Text))
			{
				this.mLoopIntervalSecondsTextBox.Text = "0";
			}
			if (string.IsNullOrEmpty(this.mRestartPlayerIntervalTextBox.Text))
			{
				this.mRestartPlayerIntervalTextBox.Text = "0";
			}
			bool flag = this.IsMacroSettingsChanged();
			if (!string.IsNullOrEmpty(this.mRestartPlayerIntervalTextBox.Text) && int.Parse(this.mRestartPlayerIntervalTextBox.Text, CultureInfo.InvariantCulture) > 0)
			{
				if (!flag)
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_NO_CHANGES_SAVE", ""), 4.0, true);
					return;
				}
				this.SaveScriptSettings();
				if (sender != null)
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""), 4.0, true);
					return;
				}
			}
			else
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this, LocaleStrings.GetLocalizedString("STRING_MACRO_RESTART_INTERVAL_NULL", ""), 4.0, true);
				this.mRestartPlayerIntervalTextBox.Text = this.mRecording.RestartPlayerAfterMinutes.ToString(CultureInfo.InvariantCulture);
			}
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0002CA10 File Offset: 0x0002AC10
		private CustomMessageWindow GetUnsavedChangesWindow()
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_MACRO_TOOL", "");
			customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNSAVED_CHANGES_CLOSE_WINDOW", "");
			customMessageWindow.IsWindowClosable = false;
			customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_SAVE_CHANGES", ""), delegate(object s, EventArgs e)
			{
				this.SaveAndCloseWindow();
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), null, null, false, null);
			customMessageWindow.Owner = this.ParentWindow;
			return customMessageWindow;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x000071B3 File Offset: 0x000053B3
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x000071BC File Offset: 0x000053BC
		private void CloseWindow()
		{
			base.Close();
			this.mMacroRecorderWindow.mOverlayGrid.Visibility = Visibility.Hidden;
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x000071D5 File Offset: 0x000053D5
		private void SaveAndCloseWindow()
		{
			this.SaveButton_Click(null, null);
			this.ParentWindow.mCommonHandler.AddToastPopup(this.mMacroRecorderWindow, LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""), 4.0, true);
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x00007197 File Offset: 0x00005397
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.IsMacroSettingsChanged())
			{
				this.GetUnsavedChangesWindow().ShowDialog();
			}
			this.CloseWindow();
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0002CAA8 File Offset: 0x0002ACA8
		private void SaveScriptSettings()
		{
			try
			{
				this.mRecording.LoopType = this.FindLoopType();
				if ((double)this.mAccelerationCombobox.SelectedIndex < 0.0)
				{
					this.mRecording.Acceleration = 1.0;
				}
				else
				{
					this.mRecording.Acceleration = (double)(this.mAccelerationCombobox.SelectedIndex + 2) * 0.5;
				}
				bool? isChecked = this.mPlayOnStartCheckBox.IsChecked;
				bool flag = true;
				if ((isChecked.GetValueOrDefault() == flag) & (isChecked != null))
				{
					if (this.ParentWindow.mAutoRunMacro != null)
					{
						this.ParentWindow.mAutoRunMacro.PlayOnStart = false;
						CommonHandlers.SaveMacroJson(this.ParentWindow.mAutoRunMacro, this.ParentWindow.mAutoRunMacro.Name + ".json");
					}
					foreach (object obj in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
					{
						SingleMacroControl singleMacroControl = (SingleMacroControl)obj;
						if (this.ParentWindow.mAutoRunMacro != null && singleMacroControl.mScriptName.Text == this.ParentWindow.mAutoRunMacro.Name)
						{
							singleMacroControl.mAutorunImage.Visibility = Visibility.Hidden;
						}
						if (singleMacroControl.mScriptName.Text == this.mRecording.Name)
						{
							singleMacroControl.mAutorunImage.Visibility = Visibility.Visible;
						}
					}
					this.ParentWindow.mAutoRunMacro = this.mRecording;
				}
				this.mRecording.LoopTime = Convert.ToInt32(this.mLoopHours.Text, CultureInfo.InvariantCulture) * 3600 + Convert.ToInt32(this.mLoopMinutes.Text, CultureInfo.InvariantCulture) * 60 + Convert.ToInt32(this.mLoopSeconds.Text, CultureInfo.InvariantCulture);
				if (this.mLoopCountTextBox.InputTextValidity == TextValidityOptions.Success)
				{
					this.mRecording.LoopNumber = Convert.ToInt32(this.mLoopCountTextBox.Text, CultureInfo.InvariantCulture);
				}
				this.mRecording.LoopInterval = Convert.ToInt32(this.mLoopIntervalMinsTextBox.Text, CultureInfo.InvariantCulture) * 60 + Convert.ToInt32(this.mLoopIntervalSecondsTextBox.Text, CultureInfo.InvariantCulture);
				this.mRecording.PlayOnStart = Convert.ToBoolean(this.mPlayOnStartCheckBox.IsChecked, CultureInfo.InvariantCulture);
				this.mRecording.DonotShowWindowOnFinish = Convert.ToBoolean(this.mDonotShowWindowOnFinishCheckBox.IsChecked, CultureInfo.InvariantCulture);
				this.mRecording.RestartPlayer = Convert.ToBoolean(this.mRestartPlayerCheckBox.IsChecked, CultureInfo.InvariantCulture);
				this.mRecording.RestartPlayerAfterMinutes = Convert.ToInt32(this.mRestartPlayerIntervalTextBox.Text, CultureInfo.InvariantCulture);
				if (this.mRecording.RecordingType == RecordingTypes.SingleRecording)
				{
					CommonHandlers.SaveMacroJson(this.mRecording, this.mRecording.Name + ".json");
				}
				CommonHandlers.RefreshAllMacroRecorderWindow();
				CommonHandlers.OnMacroSettingChanged(this.mRecording);
				this.InitSettings();
			}
			catch (Exception ex)
			{
				Logger.Error("Error in saving macro settings: " + ex.ToString());
			}
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0002CE20 File Offset: 0x0002B020
		private OperationsLoopType FindLoopType()
		{
			if (this.mRepeatActionInSession.IsChecked.Value)
			{
				return OperationsLoopType.TillLoopNumber;
			}
			if (this.mRepeatActionTime.IsChecked.Value)
			{
				return OperationsLoopType.TillTime;
			}
			return OperationsLoopType.UntilStopped;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0002CE5C File Offset: 0x0002B05C
		private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
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

		// Token: 0x060007FC RID: 2044 RVA: 0x0000720E File Offset: 0x0000540E
		private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !this.IsTextAllowed(e.Text);
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00006F01 File Offset: 0x00005101
		private bool IsTextAllowed(string text)
		{
			return new Regex("^[0-9]+$").IsMatch(text) && text.IndexOf(' ') == -1;
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x0002CEA0 File Offset: 0x0002B0A0
		private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				string text = (string)e.DataObject.GetData(typeof(string));
				if (!this.IsTextAllowed(text))
				{
					e.CancelCommand();
					return;
				}
			}
			else
			{
				e.CancelCommand();
			}
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00006EEE File Offset: 0x000050EE
		private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x0002CEF8 File Offset: 0x0002B0F8
		private bool IsMacroSettingsChanged()
		{
			if (this.mLoopHours.Text != (this.mRecording.LoopTime / 3600).ToString(CultureInfo.InvariantCulture))
			{
				return true;
			}
			if (this.mLoopMinutes.Text != (this.mRecording.LoopTime / 60 % 60).ToString(CultureInfo.InvariantCulture))
			{
				return true;
			}
			if (this.mLoopSeconds.Text != (this.mRecording.LoopTime % 60).ToString(CultureInfo.InvariantCulture))
			{
				return true;
			}
			if (this.mLoopCountTextBox.Text != this.mRecording.LoopNumber.ToString(CultureInfo.InvariantCulture))
			{
				return true;
			}
			if (this.mLoopIntervalMinsTextBox.Text != (this.mRecording.LoopInterval / 60).ToString(CultureInfo.InvariantCulture))
			{
				return true;
			}
			if (this.mLoopIntervalSecondsTextBox.Text != (this.mRecording.LoopInterval % 60).ToString(CultureInfo.InvariantCulture))
			{
				return true;
			}
			bool? flag = this.mRestartPlayerCheckBox.IsChecked;
			bool flag2 = this.mRecording.RestartPlayer;
			if (!((flag.GetValueOrDefault() == flag2) & (flag != null)))
			{
				return true;
			}
			flag = this.mPlayOnStartCheckBox.IsChecked;
			flag2 = this.mRecording.PlayOnStart;
			if (!((flag.GetValueOrDefault() == flag2) & (flag != null)))
			{
				return true;
			}
			flag = this.mDonotShowWindowOnFinishCheckBox.IsChecked;
			flag2 = this.mRecording.DonotShowWindowOnFinish;
			return !((flag.GetValueOrDefault() == flag2) & (flag != null)) || this.mRestartPlayerIntervalTextBox.Text != this.mRecording.RestartPlayerAfterMinutes.ToString(CultureInfo.InvariantCulture) || this.FindLoopType() != this.mRecording.LoopType || (this.mAccelerationCombobox.SelectedIndex != 0 && this.mRecording.Acceleration == 0.0) || this.mAccelerationCombobox.SelectedIndex != (int)(this.mRecording.Acceleration / 0.5 - 2.0);
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x0002D144 File Offset: 0x0002B344
		private void RestartTextBlock_ToolTipOpening(object sender, ToolTipEventArgs e)
		{
			this.mRestartTextBlock.ToolTip = string.Format(CultureInfo.InvariantCulture, string.Concat(new string[]
			{
				LocaleStrings.GetLocalizedString("STRING_AFTER", ""),
				" ",
				this.mRestartPlayerIntervalTextBox.Text,
				" ",
				LocaleStrings.GetLocalizedString("STRING_RESTART_PLAYER_AFTER", "")
			}), new object[0]);
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x0002D1BC File Offset: 0x0002B3BC
		private void RepeatAction_Checked(object sender, RoutedEventArgs e)
		{
			switch (this.FindLoopType())
			{
			case OperationsLoopType.TillLoopNumber:
				this.mRepeatActionTimePanelGrid.IsEnabled = false;
				this.mRepeatActionInSessionGrid.IsEnabled = true;
				this.mRepeatActionInSession.IsChecked = new bool?(true);
				return;
			case OperationsLoopType.TillTime:
				this.mRepeatActionInSessionGrid.IsEnabled = false;
				this.mRepeatActionTimePanelGrid.IsEnabled = true;
				this.mRepeatActionTime.IsChecked = new bool?(true);
				return;
			case OperationsLoopType.UntilStopped:
				this.mRepeatActionTimePanelGrid.IsEnabled = false;
				this.mRepeatActionInSessionGrid.IsEnabled = false;
				this.mRepeatSessionInfinite.IsChecked = new bool?(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x0002D260 File Offset: 0x0002B460
		private void LoopCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CustomTextBox customTextBox = sender as CustomTextBox;
			customTextBox.InputTextValidity = ((string.IsNullOrEmpty(customTextBox.Text) || Convert.ToInt32(this.mLoopCountTextBox.Text, CultureInfo.InvariantCulture) == 0) ? TextValidityOptions.Error : TextValidityOptions.Success);
			this.mErrorNamePopup.IsOpen = customTextBox.InputTextValidity == TextValidityOptions.Error;
			this.mSaveButton.IsEnabled = customTextBox.InputTextValidity == TextValidityOptions.Success;
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00007225 File Offset: 0x00005425
		private void LoopCountTextBox_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.mLoopCountTextBox.InputTextValidity == TextValidityOptions.Error)
			{
				this.mErrorNamePopup.IsOpen = true;
				this.mErrorNamePopup.StaysOpen = true;
				return;
			}
			this.mErrorNamePopup.IsOpen = false;
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x0000725A File Offset: 0x0000545A
		private void LoopCountTextBox_MouseLeave(object sender, MouseEventArgs e)
		{
			this.mErrorNamePopup.IsOpen = false;
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0002D2CC File Offset: 0x0002B4CC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/macrosettingswindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0002D2FC File Offset: 0x0002B4FC
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
				this.mSettingsHeaderText = (TextBlock)target;
				this.mSettingsHeaderText.MouseDown += this.TopBar_MouseDown;
				return;
			case 3:
				((CustomPictureBox)target).MouseLeftButtonUp += this.Close_MouseLeftButtonUp;
				return;
			case 4:
				this.mRepeactActionPanel = (StackPanel)target;
				return;
			case 5:
				this.mRepeatActionInSession = (CustomRadioButton)target;
				this.mRepeatActionInSession.Checked += this.RepeatAction_Checked;
				return;
			case 6:
				this.mRepeatActionInSessionGrid = (Grid)target;
				return;
			case 7:
				this.mLoopCountTextBox = (CustomTextBox)target;
				this.mLoopCountTextBox.PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				this.mLoopCountTextBox.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				this.mLoopCountTextBox.PreviewKeyDown += this.NumericTextBox_KeyDown;
				this.mLoopCountTextBox.MouseEnter += this.LoopCountTextBox_MouseEnter;
				this.mLoopCountTextBox.MouseLeave += this.LoopCountTextBox_MouseLeave;
				return;
			case 8:
				this.mErrorNamePopup = (CustomPopUp)target;
				return;
			case 9:
				this.mMaskBorder1 = (Border)target;
				return;
			case 10:
				this.mErrorText = (TextBlock)target;
				return;
			case 11:
				this.mDownArrow = (Path)target;
				return;
			case 12:
				this.mRepeatActionTimePanel = (StackPanel)target;
				return;
			case 13:
				this.mRepeatActionTime = (CustomRadioButton)target;
				this.mRepeatActionTime.Checked += this.RepeatAction_Checked;
				return;
			case 14:
				this.mRepeatActionTimePanelGrid = (Grid)target;
				return;
			case 15:
				this.mLoopHours = (CustomTextBox)target;
				this.mLoopHours.PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				this.mLoopHours.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				this.mLoopHours.PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 16:
				this.mLoopMinutes = (CustomTextBox)target;
				this.mLoopMinutes.PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				this.mLoopMinutes.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				this.mLoopMinutes.PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 17:
				this.mLoopSeconds = (CustomTextBox)target;
				this.mLoopSeconds.PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				this.mLoopSeconds.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				this.mLoopSeconds.PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 18:
				this.mRepeatSessionInfinitePanel = (StackPanel)target;
				return;
			case 19:
				this.mRepeatSessionInfinite = (CustomRadioButton)target;
				this.mRepeatSessionInfinite.Checked += this.RepeatAction_Checked;
				return;
			case 20:
				this.mLoopIntervalMinsTextBox = (CustomTextBox)target;
				this.mLoopIntervalMinsTextBox.PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				this.mLoopIntervalMinsTextBox.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				this.mLoopIntervalMinsTextBox.PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 21:
				this.mLoopIntervalSecondsTextBox = (CustomTextBox)target;
				this.mLoopIntervalSecondsTextBox.PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				this.mLoopIntervalSecondsTextBox.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				this.mLoopIntervalSecondsTextBox.PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 22:
				this.mAccelerationCombobox = (CustomComboBox)target;
				return;
			case 23:
				this.mPlayOnStartCheckBox = (CustomCheckbox)target;
				return;
			case 24:
				this.mRestartPlayerCheckBox = (CustomCheckbox)target;
				return;
			case 25:
				this.mRestartPlayerIntervalTextBox = (CustomTextBox)target;
				this.mRestartPlayerIntervalTextBox.PreviewTextInput += this.NumericTextBox_PreviewTextInput;
				this.mRestartPlayerIntervalTextBox.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(this.NumericTextBox_Pasting));
				this.mRestartPlayerIntervalTextBox.PreviewKeyDown += this.NumericTextBox_KeyDown;
				return;
			case 26:
				this.mRestartTextBlock = (TextBlock)target;
				this.mRestartTextBlock.ToolTipOpening += this.RestartTextBlock_ToolTipOpening;
				return;
			case 27:
				this.mDonotShowWindowOnFinishCheckBox = (CustomCheckbox)target;
				return;
			case 28:
				this.mSaveButton = (CustomButton)target;
				this.mSaveButton.Click += this.SaveButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400044C RID: 1100
		private MainWindow ParentWindow;

		// Token: 0x0400044D RID: 1101
		private MacroRecorderWindow mMacroRecorderWindow;

		// Token: 0x0400044E RID: 1102
		private MacroRecording mRecording;

		// Token: 0x0400044F RID: 1103
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000450 RID: 1104
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSettingsHeaderText;

		// Token: 0x04000451 RID: 1105
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mRepeactActionPanel;

		// Token: 0x04000452 RID: 1106
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mRepeatActionInSession;

		// Token: 0x04000453 RID: 1107
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mRepeatActionInSessionGrid;

		// Token: 0x04000454 RID: 1108
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mLoopCountTextBox;

		// Token: 0x04000455 RID: 1109
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mErrorNamePopup;

		// Token: 0x04000456 RID: 1110
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder1;

		// Token: 0x04000457 RID: 1111
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mErrorText;

		// Token: 0x04000458 RID: 1112
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path mDownArrow;

		// Token: 0x04000459 RID: 1113
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mRepeatActionTimePanel;

		// Token: 0x0400045A RID: 1114
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mRepeatActionTime;

		// Token: 0x0400045B RID: 1115
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mRepeatActionTimePanelGrid;

		// Token: 0x0400045C RID: 1116
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mLoopHours;

		// Token: 0x0400045D RID: 1117
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mLoopMinutes;

		// Token: 0x0400045E RID: 1118
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mLoopSeconds;

		// Token: 0x0400045F RID: 1119
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mRepeatSessionInfinitePanel;

		// Token: 0x04000460 RID: 1120
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mRepeatSessionInfinite;

		// Token: 0x04000461 RID: 1121
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mLoopIntervalMinsTextBox;

		// Token: 0x04000462 RID: 1122
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mLoopIntervalSecondsTextBox;

		// Token: 0x04000463 RID: 1123
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomComboBox mAccelerationCombobox;

		// Token: 0x04000464 RID: 1124
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mPlayOnStartCheckBox;

		// Token: 0x04000465 RID: 1125
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mRestartPlayerCheckBox;

		// Token: 0x04000466 RID: 1126
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mRestartPlayerIntervalTextBox;

		// Token: 0x04000467 RID: 1127
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mRestartTextBlock;

		// Token: 0x04000468 RID: 1128
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mDonotShowWindowOnFinishCheckBox;

		// Token: 0x04000469 RID: 1129
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mSaveButton;

		// Token: 0x0400046A RID: 1130
		private bool _contentLoaded;
	}
}
