using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000AF RID: 175
	public class AutoCompleteComboBox : UserControl, IComponentConnector
	{
		// Token: 0x06000714 RID: 1812 RVA: 0x00027978 File Offset: 0x00025B78
		public AutoCompleteComboBox()
		{
			this.InitializeComponent();
			this.mAutoComboBox.IsDropDownOpen = false;
			this.mAutoComboBox.Loaded += delegate
			{
				TextBox textBox = this.mAutoComboBox.Template.FindName("PART_EditableTextBox", this.mAutoComboBox) as TextBox;
				if (textBox != null)
				{
					textBox.TextChanged += this.EditTextBox_TextChanged;
				}
			};
			this.mAutoComboBox.DropDownOpened += this.MAutoComboBox_DropDownOpened;
			EventManager.RegisterClassHandler(typeof(TextBox), UIElement.KeyUpEvent, new RoutedEventHandler(this.DeselectText));
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x000279F8 File Offset: 0x00025BF8
		private void DeselectText(object sender, RoutedEventArgs e)
		{
			TextBox textBox = e.OriginalSource as TextBox;
			if (textBox != null && textBox.Text.Length < 2)
			{
				textBox.SelectionLength = 0;
				textBox.SelectionStart = 1;
			}
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00027A30 File Offset: 0x00025C30
		private void EditTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.Down))
			{
				e.Handled = true;
				return;
			}
			TextBox textBox = sender as TextBox;
			this.mAutoComboBox_TextChanged(textBox.Text);
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x000069E8 File Offset: 0x00004BE8
		private void MAutoComboBox_DropDownOpened(object sender, EventArgs e)
		{
			this.mAutoComboBox.SelectedItem = null;
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x00027A64 File Offset: 0x00025C64
		public void AddItems(string key)
		{
			ComboBoxItem comboBoxItem = new ComboBoxItem
			{
				Content = key
			};
			this.mAutoComboBox.Items.Add(comboBoxItem);
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x000069F6 File Offset: 0x00004BF6
		public void AddSuggestions(List<string> listOfSuggestions)
		{
			this.mListData.Clear();
			this.mListData = listOfSuggestions;
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x00027A90 File Offset: 0x00025C90
		private void mAutoComboBox_TextChanged(string msg)
		{
			bool flag = false;
			if (string.IsNullOrEmpty(msg))
			{
				this.mAutoComboBox.IsDropDownOpen = false;
			}
			this.mAutoComboBox.Items.Clear();
			foreach (string text in this.mListData)
			{
				if (text.StartsWith(msg, StringComparison.InvariantCultureIgnoreCase))
				{
					this.AddItems(text);
					flag = true;
				}
			}
			if (flag)
			{
				this.mAutoComboBox.IsDropDownOpen = true;
				return;
			}
			this.mAutoComboBox.IsDropDownOpen = false;
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00027B34 File Offset: 0x00025D34
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/autocompletecombobox.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00006A0A File Offset: 0x00004C0A
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
				this.mAutoComboBox = (CustomComboBox)target;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x040003BF RID: 959
		private List<string> mListData = new List<string>();

		// Token: 0x040003C0 RID: 960
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomComboBox mAutoComboBox;

		// Token: 0x040003C1 RID: 961
		private bool _contentLoaded;
	}
}
