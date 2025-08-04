using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200015D RID: 349
	public class DualTextBlockControl : UserControl, IComponentConnector
	{
		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0000ACE3 File Offset: 0x00008EE3
		internal List<IMAction> LstActionItem
		{
			get
			{
				return this.lstActionItem;
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000E72 RID: 3698 RVA: 0x0000ACEB File Offset: 0x00008EEB
		// (set) Token: 0x06000E73 RID: 3699 RVA: 0x0005ABA8 File Offset: 0x00058DA8
		internal string ActionItemProperty
		{
			get
			{
				return this.mActionItemProperty;
			}
			set
			{
				this.mActionItemProperty = value;
				if (value == "Tags" || value == "EnableCondition" || value == "StartCondition" || value == "Note")
				{
					this.mValueColumn.Width = new GridLength(1.0, GridUnitType.Star);
					this.mKeyTextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
					this.mKeyPropertyName.Visibility = Visibility.Collapsed;
					this.mKeyTextBox.MaxWidth = double.PositiveInfinity;
					this.mKeyPropertyNameTextBox.Visibility = Visibility.Collapsed;
				}
				if (value == "DpadTitle")
				{
					this.mKeyTextBox.IsEnabled = false;
				}
			}
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x0005AC5C File Offset: 0x00058E5C
		public DualTextBlockControl(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			InputMethod.SetIsInputMethodEnabled(this.mKeyTextBox, false);
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x0000ACF3 File Offset: 0x00008EF3
		private void KeyPropertyNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			KeymapCanvasWindow.sIsDirty = true;
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x0005ACB0 File Offset: 0x00058EB0
		private void KeyTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			KMManager.CheckAndCreateNewScheme();
			KeymapCanvasWindow.sIsDirty = true;
			if (e.Key == Key.Escape)
			{
				return;
			}
			if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
			{
				if (this.lstActionItem[0].Type == KeyActionType.Tap || this.lstActionItem[0].Type == KeyActionType.TapRepeat || this.lstActionItem[0].Type == KeyActionType.Script)
				{
					if (e.Key == Key.Back || e.SystemKey == Key.Back)
					{
						this.mKeyTextBox.Tag = string.Empty;
						TextBox textBox = this.mKeyTextBox;
						string imapLocaleStringsConstant = Constants.ImapLocaleStringsConstant;
						object tag = this.mKeyTextBox.Tag;
						BlueStacksUIBinding.Bind(textBox, imapLocaleStringsConstant + ((tag != null) ? tag.ToString() : null));
					}
					else if (IMAPKeys.mDictKeys.ContainsKey(e.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(e.Key))
					{
						if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.F10)
						{
							this.mKeyList.AddIfNotContain(e.SystemKey);
						}
						else if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
						{
							if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
							{
								this.mKeyList.AddIfNotContain(e.SystemKey);
							}
							else if (e.KeyboardDevice.Modifiers == (ModifierKeys.Alt | ModifierKeys.Shift))
							{
								this.mKeyList.AddIfNotContain(e.SystemKey);
							}
							else
							{
								this.mKeyList.AddIfNotContain(e.Key);
							}
						}
						else
						{
							this.mKeyList.AddIfNotContain(e.Key);
						}
					}
				}
				else
				{
					if (e.Key == Key.System && IMAPKeys.mDictKeys.ContainsKey(e.SystemKey))
					{
						this.mKeyTextBox.Tag = IMAPKeys.GetStringForFile(e.SystemKey);
						BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.SystemKey));
					}
					else if (IMAPKeys.mDictKeys.ContainsKey(e.Key))
					{
						this.mKeyTextBox.Tag = IMAPKeys.GetStringForFile(e.Key);
						BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(e.Key));
					}
					else if (e.Key == Key.Back)
					{
						this.mKeyTextBox.Tag = string.Empty;
						BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + string.Empty);
					}
					e.Handled = true;
				}
			}
			if (this.PropertyType.Equals(typeof(bool)))
			{
				this.mKeyTextBox.Tag = !Convert.ToBoolean(this.lstActionItem.First<IMAction>()[this.ActionItemProperty], CultureInfo.InvariantCulture);
				TextBox textBox2 = this.mKeyTextBox;
				string imapLocaleStringsConstant2 = Constants.ImapLocaleStringsConstant;
				object tag2 = this.mKeyTextBox.Tag;
				BlueStacksUIBinding.Bind(textBox2, imapLocaleStringsConstant2 + ((tag2 != null) ? tag2.ToString() : null));
				if (this.lstActionItem.First<IMAction>().Type == KeyActionType.TapRepeat && KMManager.CanvasWindow.mCanvasElement != null)
				{
					KMManager.CanvasWindow.mCanvasElement.SetToggleModeValues(this.lstActionItem.First<IMAction>());
				}
				if (this.lstActionItem.First<IMAction>().Type == KeyActionType.EdgeScroll && this.ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
				{
					KMManager.AssignEdgeScrollMode(this.mKeyTextBox.Tag.ToString(), this.mKeyTextBox);
				}
				e.Handled = true;
			}
			if (this.PropertyType.Equals(typeof(int)) && this.lstActionItem.First<IMAction>().Type == KeyActionType.FreeLook && KMManager.CanvasWindow.mCanvasElement != null)
			{
				KMManager.CanvasWindow.mCanvasElement.SetToggleModeValues(this.lstActionItem.First<IMAction>());
			}
			if (string.Equals(this.ActionItemProperty, "GamepadStick", StringComparison.InvariantCultureIgnoreCase) && (e.Key == Key.Back || e.SystemKey == Key.Back))
			{
				this.mKeyTextBox.Tag = string.Empty;
				TextBox textBox3 = this.mKeyTextBox;
				string imapLocaleStringsConstant3 = Constants.ImapLocaleStringsConstant;
				object tag3 = this.mKeyTextBox.Tag;
				BlueStacksUIBinding.Bind(textBox3, imapLocaleStringsConstant3 + ((tag3 != null) ? tag3.ToString() : null));
			}
			if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase) && (this.lstActionItem[0].Type == KeyActionType.Tap || this.lstActionItem[0].Type == KeyActionType.TapRepeat || this.lstActionItem[0].Type == KeyActionType.Script) && e.Key == Key.Tab)
			{
				e.Handled = true;
			}
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x0005B144 File Offset: 0x00059344
		private void KeyTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (this.lstActionItem[0].Type == KeyActionType.Tap || this.lstActionItem[0].Type == KeyActionType.TapRepeat || this.lstActionItem[0].Type == KeyActionType.Script)
			{
				if (this.mKeyList.Count >= 2)
				{
					string text = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(this.mKeyList.Count - 1));
					string text2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(this.mKeyList.Count - 1));
					this.mKeyTextBox.Text = text;
					this.mKeyTextBox.Tag = text2;
					this.SetValueHandling();
				}
				else if (this.mKeyList.Count == 1)
				{
					string text = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(0));
					string text2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(0));
					this.mKeyTextBox.Text = text;
					this.mKeyTextBox.Tag = text2;
					this.SetValueHandling();
				}
				if (!this.ActionItemProperty.Equals("EnableCondition", StringComparison.InvariantCultureIgnoreCase) && !this.ActionItemProperty.Equals("StartCondition", StringComparison.InvariantCultureIgnoreCase) && !this.ActionItemProperty.Equals("Note", StringComparison.InvariantCultureIgnoreCase))
				{
					this.mKeyTextBox.CaretIndex = this.mKeyTextBox.Text.Length;
				}
				this.mKeyList.Clear();
			}
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x0005B2FC File Offset: 0x000594FC
		private void KeyTextBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
			{
				if (e.MiddleButton == MouseButtonState.Pressed)
				{
					e.Handled = true;
					this.mKeyTextBox.Tag = "MouseMButton";
					BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseMButton");
				}
				else if (e.RightButton == MouseButtonState.Pressed)
				{
					e.Handled = true;
					this.mKeyTextBox.Tag = "MouseRButton";
					BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseRButton");
				}
				else if (e.XButton1 == MouseButtonState.Pressed)
				{
					e.Handled = true;
					this.mKeyTextBox.Tag = "MouseXButton1";
					BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseXButton1");
				}
				else if (e.XButton2 == MouseButtonState.Pressed)
				{
					e.Handled = true;
					this.mKeyTextBox.Tag = "MouseXButton2";
					BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseXButton2");
				}
			}
			if (this.PropertyType.Equals(typeof(bool)))
			{
				this.mKeyTextBox.Tag = !Convert.ToBoolean(this.lstActionItem.First<IMAction>()[this.ActionItemProperty], CultureInfo.InvariantCulture);
				TextBox textBox = this.mKeyTextBox;
				string imapLocaleStringsConstant = Constants.ImapLocaleStringsConstant;
				object tag = this.mKeyTextBox.Tag;
				BlueStacksUIBinding.Bind(textBox, imapLocaleStringsConstant + ((tag != null) ? tag.ToString() : null));
				if (this.lstActionItem.First<IMAction>().Type == KeyActionType.EdgeScroll && this.ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
				{
					KMManager.AssignEdgeScrollMode(this.mKeyTextBox.Tag.ToString(), this.mKeyTextBox);
				}
			}
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x0005B4CC File Offset: 0x000596CC
		internal bool AddActionItem(IMAction action)
		{
			this.PropertyType = IMAction.DictPropertyInfo[action.Type][this.ActionItemProperty].PropertyType;
			string text;
			if ((!this.PropertyType.Equals(typeof(string)) && !string.Equals(this.ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(this.ActionItemProperty, "EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(this.ActionItemProperty, "GamepadSensitivity", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(this.ActionItemProperty, "MouseAcceleration", StringComparison.InvariantCultureIgnoreCase)) || (action.Type == KeyActionType.State && (string.Equals(this.ActionItemProperty, "Name", StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.ActionItemProperty, "Model", StringComparison.InvariantCultureIgnoreCase))))
			{
				this.mKeyPropertyNameTextBox.IsEnabled = false;
			}
			else
			{
				this.mKeyPropertyNameTextBox.IsEnabled = true;
				int num = this.mActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
				text = this.mActionItemProperty;
				if (num > 0)
				{
					text = this.mActionItemProperty.Substring(0, num);
				}
				this.AssignGuidanceText(action, text);
			}
			if ((action.Type == KeyActionType.Zoom || action.Type == KeyActionType.MouseZoom) && (string.Equals(this.ActionItemProperty, "Speed", StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.ActionItemProperty, "Acceleration", StringComparison.InvariantCultureIgnoreCase) || string.Equals(this.ActionItemProperty, "Amplitude", StringComparison.InvariantCultureIgnoreCase)))
			{
				this.mKeyPropertyNameTextBox.IsEnabled = true;
				this.AssignGuidanceText(action, this.mActionItemProperty);
			}
			this.lstActionItem.Add(action);
			string text2 = action[this.ActionItemProperty].ToString();
			text = this.mActionItemProperty;
			if (this.mActionItemProperty.EndsWith("_alt1", StringComparison.InvariantCulture))
			{
				int num2 = this.mActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
				if (num2 > 0)
				{
					text = this.mActionItemProperty.Substring(0, num2);
				}
			}
			if (this.IsAddDirectionAttribute)
			{
				BlueStacksUIBinding.Bind(this.mKeyPropertyName, string.Concat(new string[]
				{
					Constants.ImapLocaleStringsConstant,
					action.Type.ToString(),
					"_",
					text,
					action.Direction.ToString()
				}));
			}
			else
			{
				BlueStacksUIBinding.Bind(this.mKeyPropertyName, Constants.ImapLocaleStringsConstant + action.Type.ToString() + "_" + text);
			}
			this.mKeyTextBox.Tag = action[this.ActionItemProperty];
			if (this.ActionItemProperty.StartsWith("Key", StringComparison.CurrentCultureIgnoreCase))
			{
				BlueStacksUIBinding.Bind(this.mKeyTextBox, KMManager.GetStringsToShowInUI(text2));
			}
			else
			{
				this.mKeyTextBox.Text = text2;
			}
			if (this.lstActionItem.First<IMAction>().Type == KeyActionType.EdgeScroll && this.ActionItemProperty.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
			{
				KMManager.AssignEdgeScrollMode(text2, this.mKeyTextBox);
			}
			if (text2.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase) || this.ActionItemProperty.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
			{
				BlueStacksUIBinding.Bind(this.mKeyTextBox, KMManager.GetKeyUIValue(text2));
				this.mKeyTextBox.ToolTip = this.mKeyTextBox.Text;
				return true;
			}
			return false;
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x0005B7F8 File Offset: 0x000599F8
		private void AssignGuidanceText(IMAction action, string origKey)
		{
			if (action.Guidance.ContainsKey(origKey) && !string.IsNullOrEmpty(action.Guidance[origKey]))
			{
				this.mKeyPropertyNameTextBox.Text = this.ParentWindow.SelectedConfig.GetUIString(action.Guidance[origKey]);
				return;
			}
			if (action.Guidance.ContainsKey(this.mActionItemProperty) && !string.IsNullOrEmpty(action.Guidance[this.mActionItemProperty]))
			{
				this.mKeyPropertyNameTextBox.Text = this.ParentWindow.SelectedConfig.GetUIString(action.Guidance[this.mActionItemProperty]);
				if (!action.Guidance.ContainsKey(origKey) && !string.IsNullOrEmpty(this.mKeyPropertyNameTextBox.Text.Trim()))
				{
					action.Guidance.Add(origKey, this.mKeyPropertyNameTextBox.Text.ToString(CultureInfo.InvariantCulture));
					return;
				}
			}
			else
			{
				BlueStacksUIBinding.Bind(this.mKeyPropertyNameTextBox, "STRING_ENTER_GUIDANCE_TEXT");
				this.mKeyPropertyNameTextBox.FontStyle = FontStyles.Italic;
				this.mKeyPropertyNameTextBox.FontWeight = FontWeights.ExtraLight;
				BlueStacksUIBinding.BindColor(this.mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockLightForegroundColor");
			}
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x0000AD00 File Offset: 0x00008F00
		private void KeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.SetValueHandling();
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x0005B934 File Offset: 0x00059B34
		private void SetValueHandling()
		{
			string text = this.lstActionItem[0][this.ActionItemProperty].ToString();
			if (this.PropertyType.Equals(typeof(double)))
			{
				double num;
				if (double.TryParse(text, out num))
				{
					text = num.ToString(CultureInfo.InvariantCulture);
				}
				double num2;
				if (double.TryParse(this.mKeyTextBox.Text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num2))
				{
					if (!string.Equals(this.ActionItemProperty, "Sensitivity", StringComparison.InvariantCultureIgnoreCase))
					{
						text = this.mKeyTextBox.Text;
					}
					else if (this.decimalRegex.IsMatch(this.mKeyTextBox.Text) && 0.0 <= num2 && num2 <= 10.0)
					{
						text = num2.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						this.mKeyTextBox.Text = text;
					}
				}
				else if (string.Equals(this.mKeyTextBox.Text, ".", StringComparison.InvariantCultureIgnoreCase))
				{
					this.mKeyTextBox.Text = "0.";
					text = "0";
					this.mKeyTextBox.CaretIndex = this.mKeyTextBox.Text.Length;
				}
				else if (string.IsNullOrEmpty(this.mKeyTextBox.Text))
				{
					this.mKeyTextBox.Text = "0";
					text = "0";
					this.mKeyTextBox.CaretIndex = this.mKeyTextBox.Text.Length;
				}
				else if (!string.IsNullOrEmpty(this.mKeyTextBox.Text))
				{
					this.mKeyTextBox.Text = text.ToString(CultureInfo.InvariantCulture);
				}
			}
			else if (this.PropertyType.Equals(typeof(int)))
			{
				int num3;
				if (int.TryParse(this.mKeyTextBox.Text, out num3))
				{
					text = this.mKeyTextBox.Text;
				}
				else if (!string.IsNullOrEmpty(this.mKeyTextBox.Text))
				{
					this.mKeyTextBox.Text = text;
				}
			}
			else if (this.PropertyType.Equals(typeof(bool)))
			{
				text = this.mKeyTextBox.Tag.ToString();
			}
			else if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase) || this.ActionItemProperty.StartsWith("Gamepad", StringComparison.InvariantCultureIgnoreCase))
			{
				text = this.mKeyTextBox.Tag.ToString();
			}
			else
			{
				text = this.mKeyTextBox.Text;
			}
			this.Setvalue(text);
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x0005BBBC File Offset: 0x00059DBC
		internal void Setvalue(string value)
		{
			foreach (IMAction imaction in this.lstActionItem)
			{
				if (imaction[this.ActionItemProperty].ToString().Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
				{
					this.mKeyTextBox.ToolTip = this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
				}
				if (!string.Equals(imaction[this.ActionItemProperty].ToString(), value, StringComparison.InvariantCultureIgnoreCase))
				{
					imaction[this.ActionItemProperty] = value;
					KeymapCanvasWindow.sIsDirty = true;
				}
			}
			if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
			{
				this.mKeyTextBox.Text = this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
			}
			if (this.ActionItemProperty.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
			{
				this.mKeyTextBox.Text = this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
				this.mKeyTextBox.ToolTip = this.mKeyTextBox.Text.ToUpper(CultureInfo.InvariantCulture);
			}
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x0005BCFC File Offset: 0x00059EFC
		private void KeyPropertyNameTextBox_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (!this.mKeyPropertyNameTextBox.IsVisible)
			{
				string text = this.ActionItemProperty;
				if (this.ActionItemProperty.EndsWith("_alt1", StringComparison.InvariantCulture))
				{
					int num = this.ActionItemProperty.IndexOf("_alt1", StringComparison.InvariantCulture);
					if (num > 0)
					{
						text = this.ActionItemProperty.Substring(0, num);
					}
				}
				if (string.Equals(LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", ""), this.mKeyPropertyNameTextBox.Text, StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(this.mKeyPropertyNameTextBox.Text.Trim()))
				{
					using (List<IMAction>.Enumerator enumerator = this.lstActionItem.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IMAction imaction = enumerator.Current;
							imaction.Guidance.Remove(text);
						}
						return;
					}
				}
				KeymapCanvasWindow.sIsDirty = true;
				foreach (IMAction imaction2 in this.lstActionItem)
				{
					imaction2.Guidance[text] = this.mKeyPropertyNameTextBox.Text;
				}
				this.ParentWindow.SelectedConfig.AddString(this.mKeyPropertyNameTextBox.Text);
			}
		}

		// Token: 0x06000E7F RID: 3711 RVA: 0x0005BE4C File Offset: 0x0005A04C
		private void KeyTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			double num;
			if (this.PropertyType.Equals(typeof(double)) && !double.TryParse(this.mKeyTextBox.Text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num))
			{
				this.Setvalue("0");
				this.mKeyTextBox.Text = "0";
			}
			int num2;
			if (this.PropertyType.Equals(typeof(int)) && !int.TryParse(this.mKeyTextBox.Text, out num2))
			{
				this.Setvalue("0");
				this.mKeyTextBox.Text = "0";
			}
		}

		// Token: 0x06000E80 RID: 3712 RVA: 0x0005BEF0 File Offset: 0x0005A0F0
		private void KeyPropertyNameTextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			if (string.Equals(LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", ""), this.mKeyPropertyNameTextBox.Text, StringComparison.InvariantCulture))
			{
				this.mKeyPropertyNameTextBox.Text = "";
				this.mKeyPropertyNameTextBox.FontStyle = FontStyles.Normal;
				this.mKeyPropertyNameTextBox.FontWeight = FontWeights.Normal;
				BlueStacksUIBinding.BindColor(this.mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockForeground");
			}
		}

		// Token: 0x06000E81 RID: 3713 RVA: 0x0005BF64 File Offset: 0x0005A164
		private void KeyPropertyNameTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.mKeyPropertyNameTextBox.Text))
			{
				this.mKeyPropertyNameTextBox.Text = LocaleStrings.GetLocalizedString("STRING_ENTER_GUIDANCE_TEXT", "");
				this.mKeyPropertyNameTextBox.FontStyle = FontStyles.Italic;
				this.mKeyPropertyNameTextBox.FontWeight = FontWeights.ExtraLight;
				BlueStacksUIBinding.BindColor(this.mKeyPropertyNameTextBox, Control.ForegroundProperty, "DualTextBlockLightForegroundColor");
			}
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x0000AD08 File Offset: 0x00008F08
		private void mKeyTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (base.IsLoaded)
			{
				KMManager.sGamepadDualTextbox = this;
				KMManager.pressedGamepadKeyList.Clear();
				KMManager.CallGamepadHandler(this.ParentWindow, "true");
			}
		}

		// Token: 0x06000E83 RID: 3715 RVA: 0x0005BFD4 File Offset: 0x0005A1D4
		private void KeyTextBoxPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (this.ActionItemProperty.StartsWith("Key", StringComparison.InvariantCultureIgnoreCase))
			{
				if (e.Delta > 0)
				{
					e.Handled = true;
					this.mKeyTextBox.Tag = "MouseWheelUp";
					BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseWheelUp");
					return;
				}
				if (e.Delta < 0)
				{
					e.Handled = true;
					this.mKeyTextBox.Tag = "MouseWheelDown";
					BlueStacksUIBinding.Bind(this.mKeyTextBox, Constants.ImapLocaleStringsConstant + "MouseWheelDown");
				}
			}
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x0005C06C File Offset: 0x0005A26C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/dualtextblockcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000E85 RID: 3717 RVA: 0x0005C09C File Offset: 0x0005A29C
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
				this.mValueColumn = (ColumnDefinition)target;
				return;
			case 2:
				this.mKeyPropertyName = (TextBox)target;
				this.mKeyPropertyName.TextChanged += this.KeyPropertyNameTextBox_TextChanged;
				this.mKeyPropertyName.IsVisibleChanged += this.KeyPropertyNameTextBox_IsVisibleChanged;
				return;
			case 3:
				this.mKeyTextBox = (TextBox)target;
				this.mKeyTextBox.PreviewMouseDown += this.KeyTextBox_MouseDown;
				this.mKeyTextBox.PreviewMouseLeftButtonDown += this.mKeyTextBox_PreviewMouseLeftButtonDown;
				this.mKeyTextBox.TextChanged += this.KeyTextBox_TextChanged;
				this.mKeyTextBox.PreviewKeyDown += this.KeyTextBox_KeyDown;
				this.mKeyTextBox.KeyUp += this.KeyTextBox_KeyUp;
				this.mKeyTextBox.LostFocus += this.KeyTextBox_LostFocus;
				this.mKeyTextBox.PreviewMouseWheel += this.KeyTextBoxPreviewMouseWheel;
				return;
			case 4:
				this.mKeyPropertyNameTextBox = (TextBox)target;
				this.mKeyPropertyNameTextBox.GotFocus += this.KeyPropertyNameTextBox_GotFocus;
				this.mKeyPropertyNameTextBox.LostFocus += this.KeyPropertyNameTextBox_LostFocus;
				this.mKeyPropertyNameTextBox.TextChanged += this.KeyPropertyNameTextBox_TextChanged;
				this.mKeyPropertyNameTextBox.IsVisibleChanged += this.KeyPropertyNameTextBox_IsVisibleChanged;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000932 RID: 2354
		private Regex decimalRegex = new Regex("^[0-9]*(\\.)?[0-9]*$");

		// Token: 0x04000933 RID: 2355
		private List<IMAction> lstActionItem = new List<IMAction>();

		// Token: 0x04000934 RID: 2356
		private MainWindow ParentWindow;

		// Token: 0x04000935 RID: 2357
		private List<Key> mKeyList = new List<Key>();

		// Token: 0x04000936 RID: 2358
		private string mActionItemProperty;

		// Token: 0x04000937 RID: 2359
		private Type PropertyType;

		// Token: 0x04000938 RID: 2360
		internal bool IsAddDirectionAttribute;

		// Token: 0x04000939 RID: 2361
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mValueColumn;

		// Token: 0x0400093A RID: 2362
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBox mKeyPropertyName;

		// Token: 0x0400093B RID: 2363
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBox mKeyTextBox;

		// Token: 0x0400093C RID: 2364
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBox mKeyPropertyNameTextBox;

		// Token: 0x0400093D RID: 2365
		private bool _contentLoaded;
	}
}
