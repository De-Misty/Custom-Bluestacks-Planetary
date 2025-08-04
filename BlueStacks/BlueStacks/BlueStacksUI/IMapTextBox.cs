using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000068 RID: 104
	public class IMapTextBox : XTextBox, IComponentConnector
	{
		// Token: 0x06000511 RID: 1297 RVA: 0x000055D7 File Offset: 0x000037D7
		public IMapTextBox()
		{
			this.InitializeComponent();
			InputMethod.SetIsInputMethodEnabled(this, false);
			base.ClearValue(IMapTextBox.IMActionItemsProperty);
			base.Loaded += this.IMapTextBox_Loaded;
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x00005614 File Offset: 0x00003814
		// (set) Token: 0x06000513 RID: 1299 RVA: 0x00005626 File Offset: 0x00003826
		public bool IsKeyBoardInFocus
		{
			get
			{
				return (bool)base.GetValue(IMapTextBox.IsKeyBoardInFocusProperty);
			}
			set
			{
				base.SetValue(IMapTextBox.IsKeyBoardInFocusProperty, value);
			}
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0001F0B8 File Offset: 0x0001D2B8
		private static void OnKeyBoardInFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			IMapTextBox mapTextBox = sender as IMapTextBox;
			bool flag;
			if (mapTextBox != null && bool.TryParse(args.NewValue.ToString(), out flag))
			{
				KMManager.CurrentIMapTextBox = (flag ? mapTextBox : null);
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x00005639 File Offset: 0x00003839
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x0000564B File Offset: 0x0000384B
		public Type PropertyType
		{
			get
			{
				return (Type)base.GetValue(IMapTextBox.PropertyTypeProperty);
			}
			set
			{
				base.SetValue(IMapTextBox.PropertyTypeProperty, value);
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x00005659 File Offset: 0x00003859
		// (set) Token: 0x06000518 RID: 1304 RVA: 0x0000566B File Offset: 0x0000386B
		public KeyActionType ActionType
		{
			get
			{
				return (KeyActionType)base.GetValue(IMapTextBox.ActionTypeProperty);
			}
			set
			{
				base.SetValue(IMapTextBox.ActionTypeProperty, value);
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x0000567E File Offset: 0x0000387E
		// (set) Token: 0x0600051A RID: 1306 RVA: 0x00005690 File Offset: 0x00003890
		public ObservableCollection<IMActionItem> IMActionItems
		{
			get
			{
				return (ObservableCollection<IMActionItem>)base.GetValue(IMapTextBox.IMActionItemsProperty);
			}
			set
			{
				if (value == null)
				{
					base.ClearValue(IMapTextBox.IMActionItemsProperty);
					return;
				}
				base.SetValue(IMapTextBox.IMActionItemsProperty, value);
			}
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001F0F0 File Offset: 0x0001D2F0
		private void IMapTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (base.TextBlock != null)
			{
				base.TextBlock.TextTrimming = TextTrimming.None;
				base.TextBlock.TextWrapping = TextWrapping.Wrap;
			}
			if (!string.IsNullOrEmpty(base.Tag.ToString()))
			{
				string[] array = base.Tag.ToString().Split(new char[] { '+' });
				if (array.Length != 0)
				{
					if (this.IMActionItems[0].ActionItem.Contains("_alt1", StringComparison.InvariantCultureIgnoreCase) || this.IMActionItems[0].ActionItem.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
					{
						base.Text = string.Join(" + ", (from x in array.ToList<string>()
							select LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(KMManager.CheckForGamepadSuffix(x.Trim())), "")).ToArray<string>());
						return;
					}
					base.Text = string.Join(" + ", (from x in array.ToList<string>()
						select LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(x.Trim()), "")).ToArray<string>());
				}
			}
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001F210 File Offset: 0x0001D410
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			KMManager.CurrentIMapTextBox = this;
			KMManager.pressedGamepadKeyList.Clear();
			KMManager.CallGamepadHandler(BlueStacksUIUtils.LastActivatedWindow, "true");
			base.TextChanged -= this.IMapTextBox_TextChanged;
			base.TextChanged += this.IMapTextBox_TextChanged;
			base.PreviewMouseWheel -= this.IMapTextBox_PreviewMouseWheel;
			base.PreviewMouseWheel += this.IMapTextBox_PreviewMouseWheel;
			this.SetCaretIndex();
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001F294 File Offset: 0x0001D494
		private void IMapTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs args)
		{
			if (args != null && args.Delta != 0 && this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
			{
				foreach (IMActionItem imactionItem in this.IMActionItems)
				{
					if (imactionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
					{
						base.Tag = ((args.Delta < 0) ? "MouseWheelDown" : "MouseWheelUp");
						this.SetValueHandling(imactionItem);
						BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(base.Tag.ToString()));
					}
					if (this.PropertyType.Equals(typeof(bool)))
					{
						bool flag = !Convert.ToBoolean(imactionItem.IMAction[imactionItem.ActionItem], CultureInfo.InvariantCulture);
						base.Tag = flag;
						IMapTextBox.Setvalue(imactionItem, flag.ToString(CultureInfo.InvariantCulture));
						string imapLocaleStringsConstant = Constants.ImapLocaleStringsConstant;
						object tag = base.Tag;
						BlueStacksUIBinding.Bind(this, imapLocaleStringsConstant + ((tag != null) ? tag.ToString() : null));
						args.Handled = true;
					}
				}
				args.Handled = true;
			}
			this.SetCaretIndex();
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001F3EC File Offset: 0x0001D5EC
		private void IMapTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
			{
				foreach (IMActionItem imactionItem in this.IMActionItems)
				{
					this.SetValueHandling(imactionItem);
				}
				KMManager.CheckAndCreateNewScheme();
			}
			this.SetCaretIndex();
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001F45C File Offset: 0x0001D65C
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.TextChanged -= this.IMapTextBox_TextChanged;
			base.PreviewMouseWheel -= this.IMapTextBox_PreviewMouseWheel;
			KMManager.CurrentIMapTextBox = null;
			base.InputTextValidity = TextValidityOptions.Success;
			ToolTip toolTip = base.ToolTip as ToolTip;
			if (toolTip != null)
			{
				toolTip.IsOpen = false;
			}
			KMManager.CurrentIMapTextBox = null;
			base.OnLostFocus(e);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001F4C0 File Offset: 0x0001D6C0
		protected override void OnPreviewKeyDown(KeyEventArgs args)
		{
			if (args != null && args.Key != Key.Escape)
			{
				if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
				{
					foreach (IMActionItem imactionItem in this.IMActionItems)
					{
						if (imactionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
						{
							if (imactionItem.IMAction.Type == KeyActionType.Tap || imactionItem.IMAction.Type == KeyActionType.TapRepeat || imactionItem.IMAction.Type == KeyActionType.Script)
							{
								if (args.Key == Key.Back || args.SystemKey == Key.Back)
								{
									base.Tag = string.Empty;
									string imapLocaleStringsConstant = Constants.ImapLocaleStringsConstant;
									object tag = base.Tag;
									BlueStacksUIBinding.Bind(this, imapLocaleStringsConstant + ((tag != null) ? tag.ToString() : null));
								}
								else if (IMAPKeys.mDictKeys.ContainsKey(args.SystemKey) || IMAPKeys.mDictKeys.ContainsKey(args.Key))
								{
									if (args.SystemKey == Key.LeftAlt || args.SystemKey == Key.RightAlt || args.SystemKey == Key.F10)
									{
										this.mKeyList.AddIfNotContain(args.SystemKey);
									}
									else if (args.KeyboardDevice.Modifiers != ModifierKeys.None)
									{
										if (args.KeyboardDevice.Modifiers == ModifierKeys.Alt)
										{
											this.mKeyList.AddIfNotContain(args.SystemKey);
										}
										else if (args.KeyboardDevice.Modifiers == (ModifierKeys.Alt | ModifierKeys.Shift))
										{
											this.mKeyList.AddIfNotContain(args.SystemKey);
										}
										else
										{
											this.mKeyList.AddIfNotContain(args.Key);
										}
									}
									else
									{
										this.mKeyList.AddIfNotContain(args.Key);
									}
								}
							}
							else
							{
								if (args.Key == Key.System && IMAPKeys.mDictKeys.ContainsKey(args.SystemKey))
								{
									base.Tag = IMAPKeys.GetStringForFile(args.SystemKey);
									BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(args.SystemKey));
								}
								else if (IMAPKeys.mDictKeys.ContainsKey(args.Key))
								{
									base.Tag = IMAPKeys.GetStringForFile(args.Key);
									BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(args.Key));
								}
								else if (args.Key == Key.Back)
								{
									base.Tag = string.Empty;
									BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + string.Empty);
								}
								args.Handled = true;
							}
						}
						if (string.Equals(imactionItem.ActionItem, "GamepadStick", StringComparison.InvariantCulture))
						{
							if (args.Key == Key.Back || args.Key == Key.Delete)
							{
								base.Tag = string.Empty;
								BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + string.Empty);
							}
							args.Handled = true;
						}
						if (this.PropertyType.Equals(typeof(bool)))
						{
							bool flag = !Convert.ToBoolean(imactionItem.IMAction[imactionItem.ActionItem], CultureInfo.InvariantCulture);
							base.Tag = flag;
							IMapTextBox.Setvalue(imactionItem, flag.ToString(CultureInfo.InvariantCulture));
							string imapLocaleStringsConstant2 = Constants.ImapLocaleStringsConstant;
							object tag2 = base.Tag;
							BlueStacksUIBinding.Bind(this, imapLocaleStringsConstant2 + ((tag2 != null) ? tag2.ToString() : null));
							if (imactionItem.IMAction.Type == KeyActionType.EdgeScroll && imactionItem.ActionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
							{
								KMManager.AssignEdgeScrollMode(flag.ToString(CultureInfo.InvariantCulture), this);
							}
							args.Handled = true;
						}
					}
				}
				base.Focus();
				args.Handled = true;
			}
			if (this.PropertyType.Equals(typeof(bool)))
			{
				KMManager.CheckAndCreateNewScheme();
			}
			this.SetCaretIndex();
			base.OnPreviewKeyDown(args);
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001F8A0 File Offset: 0x0001DAA0
		protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
		{
			if (args != null)
			{
				if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
				{
					foreach (IMActionItem imactionItem in this.IMActionItems)
					{
						if (imactionItem.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
						{
							if (args.MiddleButton == MouseButtonState.Pressed)
							{
								args.Handled = true;
								base.Tag = "MouseMButton";
								BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + "MouseMButton");
							}
							else if (args.RightButton == MouseButtonState.Pressed)
							{
								args.Handled = true;
								base.Tag = "MouseRButton";
								BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + "MouseRButton");
							}
							else if (args.XButton1 == MouseButtonState.Pressed)
							{
								args.Handled = true;
								base.Tag = "MouseXButton1";
								BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + "MouseXButton1");
							}
							else if (args.XButton2 == MouseButtonState.Pressed)
							{
								args.Handled = true;
								base.Tag = "MouseXButton2";
								BlueStacksUIBinding.Bind(this, Constants.ImapLocaleStringsConstant + "MouseXButton2");
							}
						}
						if (this.PropertyType.Equals(typeof(bool)))
						{
							bool flag = !Convert.ToBoolean(imactionItem.IMAction[imactionItem.ActionItem], CultureInfo.InvariantCulture);
							base.Tag = flag;
							IMapTextBox.Setvalue(imactionItem, flag.ToString(CultureInfo.InvariantCulture));
							string imapLocaleStringsConstant = Constants.ImapLocaleStringsConstant;
							object tag = base.Tag;
							BlueStacksUIBinding.Bind(this, imapLocaleStringsConstant + ((tag != null) ? tag.ToString() : null));
							if (imactionItem.IMAction.Type == KeyActionType.EdgeScroll && imactionItem.ActionItem.Equals("EdgeScrollEnabled", StringComparison.InvariantCultureIgnoreCase))
							{
								KMManager.AssignEdgeScrollMode(flag.ToString(CultureInfo.InvariantCulture), this);
							}
						}
					}
				}
				if (args.LeftButton == MouseButtonState.Pressed && base.IsKeyboardFocusWithin)
				{
					args.Handled = true;
				}
				base.Focus();
				args.Handled = true;
			}
			if (this.PropertyType.Equals(typeof(bool)))
			{
				KMManager.CheckAndCreateNewScheme();
			}
			this.SetCaretIndex();
			base.OnPreviewMouseDown(args);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001FAF4 File Offset: 0x0001DCF4
		protected override void OnKeyUp(KeyEventArgs args)
		{
			if (args != null)
			{
				if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
				{
					foreach (IMActionItem imactionItem in this.IMActionItems)
					{
						if (imactionItem.IMAction.Type == KeyActionType.Tap || imactionItem.IMAction.Type == KeyActionType.TapRepeat || imactionItem.IMAction.Type == KeyActionType.Script)
						{
							if (this.mKeyList.Count >= 2)
							{
								string text = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(this.mKeyList.Count - 1));
								string text2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(this.mKeyList.Count - 2)) + " + " + IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(this.mKeyList.Count - 1));
								base.Tag = text2;
								base.Text = text;
								this.SetValueHandling(imactionItem);
							}
							else if (this.mKeyList.Count == 1)
							{
								string text = IMAPKeys.GetStringForUI(this.mKeyList.ElementAt(0));
								string text2 = IMAPKeys.GetStringForFile(this.mKeyList.ElementAt(0));
								base.Tag = text2;
								base.Text = text;
								this.SetValueHandling(imactionItem);
							}
							this.mKeyList.Clear();
						}
					}
				}
				args.Handled = true;
			}
			if (this.PropertyType.Equals(typeof(bool)))
			{
				KMManager.CheckAndCreateNewScheme();
			}
			this.SetCaretIndex();
			base.OnKeyUp(args);
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001FCD0 File Offset: 0x0001DED0
		private void SetValueHandling(IMActionItem item)
		{
			string text = item.IMAction[item.ActionItem].ToString();
			if (base.IsLoaded)
			{
				KMManager.CallGamepadHandler(BlueStacksUIUtils.LastActivatedWindow, "true");
			}
			if (this.PropertyType.Equals(typeof(double)))
			{
				double num;
				if (double.TryParse(base.Text, out num))
				{
					text = base.Text;
				}
				else if (!string.IsNullOrEmpty(base.Text))
				{
					base.Text = text;
				}
			}
			else if (this.PropertyType.Equals(typeof(int)))
			{
				int num2;
				if (int.TryParse(base.Text, out num2))
				{
					text = base.Text;
				}
				else if (!string.IsNullOrEmpty(base.Text))
				{
					base.Text = text;
				}
			}
			else if (this.PropertyType.Equals(typeof(bool)))
			{
				text = base.Tag.ToString();
			}
			else
			{
				text = base.Tag.ToString();
			}
			IMapTextBox.Setvalue(item, text);
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001FDD0 File Offset: 0x0001DFD0
		internal static void Setvalue(IMActionItem item, string value)
		{
			if (!string.Equals(item.IMAction[item.ActionItem].ToString(), value, StringComparison.InvariantCulture))
			{
				item.IMAction[item.ActionItem] = value;
			}
			Logger.Debug("GUIDANCE: " + item.IMAction.Type.ToString());
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x000056AD File Offset: 0x000038AD
		private void SetCaretIndex()
		{
			if (!string.IsNullOrEmpty(base.Text))
			{
				base.CaretIndex = base.Text.Length;
			}
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001FE38 File Offset: 0x0001E038
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/guidancemodels/imaptextbox.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x000056CD File Offset: 0x000038CD
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				this.mTextBox = (IMapTextBox)target;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x040002AA RID: 682
		private List<Key> mKeyList = new List<Key>();

		// Token: 0x040002AB RID: 683
		public static readonly DependencyProperty IsKeyBoardInFocusProperty = DependencyProperty.Register("IsKeyBoardInFocus", typeof(bool), typeof(IMapTextBox), new PropertyMetadata(false, new PropertyChangedCallback(IMapTextBox.OnKeyBoardInFocusChanged)));

		// Token: 0x040002AC RID: 684
		public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register("PropertyType", typeof(Type), typeof(IMapTextBox), new PropertyMetadata());

		// Token: 0x040002AD RID: 685
		public static readonly DependencyProperty ActionTypeProperty = DependencyProperty.Register("ActionType", typeof(KeyActionType), typeof(IMapTextBox), new PropertyMetadata());

		// Token: 0x040002AE RID: 686
		public static readonly DependencyProperty IMActionItemsProperty = DependencyProperty.Register("IMActionItems", typeof(ObservableCollection<IMActionItem>), typeof(IMapTextBox), new PropertyMetadata());

		// Token: 0x040002AF RID: 687
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal IMapTextBox mTextBox;

		// Token: 0x040002B0 RID: 688
		private bool _contentLoaded;
	}
}
