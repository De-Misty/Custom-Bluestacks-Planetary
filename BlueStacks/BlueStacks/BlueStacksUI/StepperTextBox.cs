using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200006A RID: 106
	public class StepperTextBox : XTextBox, IComponentConnector, IStyleConnector
	{
		// Token: 0x0600052D RID: 1325 RVA: 0x00005730 File Offset: 0x00003930
		public StepperTextBox()
		{
			this.InitializeComponent();
			base.ClearValue(StepperTextBox.IMActionItemsProperty);
			InputMethod.SetIsInputMethodEnabled(this, false);
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x00005760 File Offset: 0x00003960
		// (set) Token: 0x0600052F RID: 1327 RVA: 0x00005772 File Offset: 0x00003972
		public Type PropertyType
		{
			get
			{
				return (Type)base.GetValue(StepperTextBox.PropertyTypeProperty);
			}
			set
			{
				base.SetValue(StepperTextBox.PropertyTypeProperty, value);
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x00005780 File Offset: 0x00003980
		// (set) Token: 0x06000531 RID: 1329 RVA: 0x00005788 File Offset: 0x00003988
		public double MinValue { get; set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000532 RID: 1330 RVA: 0x00005791 File Offset: 0x00003991
		// (set) Token: 0x06000533 RID: 1331 RVA: 0x00005799 File Offset: 0x00003999
		public double MaxValue { get; set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x000057A2 File Offset: 0x000039A2
		// (set) Token: 0x06000535 RID: 1333 RVA: 0x000057B4 File Offset: 0x000039B4
		public ObservableCollection<IMActionItem> IMActionItems
		{
			get
			{
				return (ObservableCollection<IMActionItem>)base.GetValue(StepperTextBox.IMActionItemsProperty);
			}
			set
			{
				if (value == null)
				{
					base.ClearValue(StepperTextBox.IMActionItemsProperty);
					return;
				}
				base.SetValue(StepperTextBox.IMActionItemsProperty, value);
			}
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x0001FF28 File Offset: 0x0001E128
		protected override void OnPreviewTextInput(TextCompositionEventArgs args)
		{
			if (args != null)
			{
				string text;
				if (base.SelectionLength > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(base.Text);
					stringBuilder.Remove(base.SelectionStart, base.SelectionLength);
					stringBuilder.Insert(base.SelectionStart, args.Text);
					text = stringBuilder.ToString();
				}
				else
				{
					text = base.Text.Insert(base.SelectionStart, args.Text);
				}
				if (this.PropertyType == typeof(int))
				{
					int num;
					args.Handled = int.TryParse(text, out num);
				}
				else if (this.PropertyType == typeof(double))
				{
					double num2;
					if (double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num2))
					{
						args.Handled = !this.decimalRegex.IsMatch(text) || this.MinValue > num2 || num2 > this.MaxValue;
					}
					else
					{
						if (string.Equals(text, ".", StringComparison.InvariantCultureIgnoreCase))
						{
							base.Text = "0.";
							KMManager.CheckAndCreateNewScheme();
							if (this.IMActionItems != null && this.IMActionItems.Any<IMActionItem>())
							{
								foreach (IMActionItem imactionItem in this.IMActionItems)
								{
									this.SetValueHandling(imactionItem);
								}
							}
							base.CaretIndex = base.Text.Length;
						}
						args.Handled = true;
					}
				}
			}
			base.OnPreviewTextInput(args);
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x000057D1 File Offset: 0x000039D1
		private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste;
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x000200A8 File Offset: 0x0001E2A8
		private void OnIncrease(object sender, RoutedEventArgs e)
		{
			double num;
			int num2;
			if (this.PropertyType == typeof(double) && double.TryParse(base.Text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num))
			{
				if (this.CanIncrease(num, 0.05))
				{
					num += 0.05;
					base.Text = num.ToString(CultureInfo.InvariantCulture);
					KMManager.CheckAndCreateNewScheme();
				}
			}
			else if (this.PropertyType == typeof(int) && int.TryParse(base.Text, out num2) && this.CanIncrease((double)num2, 1.0))
			{
				num2++;
				base.Text = num2.ToString(CultureInfo.InvariantCulture);
				KMManager.CheckAndCreateNewScheme();
			}
			foreach (IMActionItem imactionItem in this.IMActionItems)
			{
				this.SetValueHandling(imactionItem);
			}
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00005803 File Offset: 0x00003A03
		private bool CanIncrease(double doubleVal, double val)
		{
			return doubleVal + val <= this.MaxValue;
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x00005813 File Offset: 0x00003A13
		private bool CanDecrease(double doubleVal, double val)
		{
			return doubleVal - val >= this.MinValue;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x000201A8 File Offset: 0x0001E3A8
		private void OnDecrease(object sender, RoutedEventArgs e)
		{
			double num;
			int num2;
			if (this.PropertyType == typeof(double) && double.TryParse(base.Text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num) && num > 0.0)
			{
				if (this.CanDecrease(num, 0.05))
				{
					num -= 0.05;
					base.Text = num.ToString(CultureInfo.InvariantCulture);
					KMManager.CheckAndCreateNewScheme();
				}
			}
			else if (this.PropertyType == typeof(int) && int.TryParse(base.Text, out num2) && num2 > 0 && this.CanDecrease((double)num2, 1.0))
			{
				num2--;
				base.Text = num2.ToString(CultureInfo.InvariantCulture);
				KMManager.CheckAndCreateNewScheme();
			}
			foreach (IMActionItem imactionItem in this.IMActionItems)
			{
				this.SetValueHandling(imactionItem);
			}
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x00005823 File Offset: 0x00003A23
		protected override void OnPreviewKeyDown(KeyEventArgs args)
		{
			if (args != null)
			{
				if (args.Key == Key.Space)
				{
					args.Handled = true;
				}
				else
				{
					if (args.Key == Key.Escape)
					{
						return;
					}
					base.Focus();
				}
			}
			base.OnPreviewKeyDown(args);
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x00005854 File Offset: 0x00003A54
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.TextChanged -= this.IMapTextBox_TextChanged;
			base.TextChanged += this.IMapTextBox_TextChanged;
			base.CaretIndex = base.Text.Length;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x000202B8 File Offset: 0x0001E4B8
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
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0000588B File Offset: 0x00003A8B
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.TextChanged -= this.IMapTextBox_TextChanged;
			base.OnLostFocus(e);
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00020320 File Offset: 0x0001E520
		private void SetValueHandling(IMActionItem item)
		{
			string text = item.IMAction[item.ActionItem].ToString();
			if (this.PropertyType.Equals(typeof(double)))
			{
				double num;
				if (double.TryParse(base.Text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out num))
				{
					text = base.Text;
				}
				else if (!string.IsNullOrEmpty(base.Text))
				{
					base.Text = text;
				}
			}
			else if (this.PropertyType.Equals(typeof(decimal)))
			{
				decimal num2;
				if (decimal.TryParse(base.Text, out num2))
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
				int num3;
				if (int.TryParse(base.Text, out num3))
				{
					text = base.Text;
				}
				else if (!string.IsNullOrEmpty(base.Text))
				{
					base.Text = text;
				}
			}
			this.Setvalue(item, text);
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0002042C File Offset: 0x0001E62C
		internal void Setvalue(IMActionItem item, string value)
		{
			if (!string.Equals(item.IMAction[item.ActionItem].ToString(), value, StringComparison.InvariantCulture))
			{
				item.IMAction[item.ActionItem] = value;
			}
			if (item.ActionItem.StartsWith("Key", StringComparison.InvariantCulture))
			{
				base.Text = base.Text.ToUpper(CultureInfo.InvariantCulture);
			}
			if (item.ActionItem.Contains("Gamepad", StringComparison.InvariantCultureIgnoreCase))
			{
				base.Text = base.Text.ToUpper(CultureInfo.InvariantCulture);
			}
			Logger.Debug("GUIDANCE: " + item.IMAction.Type.ToString());
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x000058A6 File Offset: 0x00003AA6
		private void Path_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Path, Shape.FillProperty, "SettingsWindowTabMenuItemLegendForeground");
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x000058BD File Offset: 0x00003ABD
		private void Path_MouseLeave(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as Path, Shape.FillProperty, "SettingsWindowForegroundDimColor");
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x000204E4 File Offset: 0x0001E6E4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/guidancemodels/steppertextbox.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x000058D4 File Offset: 0x00003AD4
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
				((StepperTextBox)target).AddHandler(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(this.OnPreviewExecuted));
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00020514 File Offset: 0x0001E714
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 2:
				((RepeatButton)target).Click += this.OnIncrease;
				return;
			case 3:
				((Path)target).MouseEnter += this.Path_MouseEnter;
				((Path)target).MouseLeave += this.Path_MouseLeave;
				return;
			case 4:
				((RepeatButton)target).Click += this.OnDecrease;
				return;
			case 5:
				((Path)target).MouseEnter += this.Path_MouseEnter;
				((Path)target).MouseLeave += this.Path_MouseLeave;
				return;
			default:
				return;
			}
		}

		// Token: 0x040002B4 RID: 692
		private Regex decimalRegex = new Regex("^[0-9]*(\\.)?[0-9]*$");

		// Token: 0x040002B7 RID: 695
		public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register("PropertyType", typeof(Type), typeof(StepperTextBox), new PropertyMetadata());

		// Token: 0x040002B8 RID: 696
		public static readonly DependencyProperty IMActionItemsProperty = DependencyProperty.Register("IMActionItems", typeof(ObservableCollection<IMActionItem>), typeof(StepperTextBox), new PropertyMetadata());

		// Token: 0x040002B9 RID: 697
		private bool _contentLoaded;
	}
}
