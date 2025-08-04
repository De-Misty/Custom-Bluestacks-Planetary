using System;
using System.CodeDom.Compiler;
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

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200015B RID: 347
	public class ComboBoxSchemeControl : UserControl, IComponentConnector
	{
		// Token: 0x06000E5B RID: 3675 RVA: 0x0000AC66 File Offset: 0x00008E66
		public ComboBoxSchemeControl(KeymapCanvasWindow window, MainWindow mainWindow)
		{
			this.CanvasWindow = window;
			this.ParentWindow = mainWindow;
			this.InitializeComponent();
		}

		// Token: 0x06000E5C RID: 3676 RVA: 0x00059D64 File Offset: 0x00057F64
		private void Bookmark_img_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.mSchemeName.IsReadOnly)
			{
				this.HandleNameEdit(this);
			}
			if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(this.mSchemeName.Text))
			{
				IMControlScheme imcontrolScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text];
				if (imcontrolScheme.IsBookMarked)
				{
					imcontrolScheme.IsBookMarked = false;
					this.mBookmarkImg.ImageName = "bookmark";
				}
				else
				{
					List<IMControlScheme> controlSchemes = this.ParentWindow.SelectedConfig.ControlSchemes;
					bool flag;
					if (controlSchemes == null)
					{
						flag = false;
					}
					else
					{
						flag = controlSchemes.Count((IMControlScheme scheme) => scheme.IsBookMarked) < 5;
					}
					if (flag)
					{
						imcontrolScheme.IsBookMarked = true;
						this.mBookmarkImg.ImageName = "bookmarked";
					}
					else
					{
						this.CanvasWindow.SidebarWindow.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_BOOKMARK_SCHEMES_WARNING", ""));
					}
				}
				this.CanvasWindow.SidebarWindow.FillProfileCombo();
				KeymapCanvasWindow.sIsDirty = true;
			}
			e.Handled = true;
		}

		// Token: 0x06000E5D RID: 3677 RVA: 0x00059E84 File Offset: 0x00058084
		private void EditImg_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.mEditImg.Visibility = Visibility.Collapsed;
			this.mSaveImg.Visibility = Visibility.Visible;
			this.mOldSchemeName = this.mSchemeName.Text;
			this.mSchemeName.Focusable = true;
			this.mSchemeName.IsReadOnly = false;
			this.mSchemeName.CaretIndex = this.mSchemeName.Text.Length;
			this.mSchemeName.Focus();
			e.Handled = true;
		}

		// Token: 0x06000E5E RID: 3678 RVA: 0x0000AC82 File Offset: 0x00008E82
		private void SaveImg_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.HandleNameEdit(this);
			e.Handled = true;
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x00059F00 File Offset: 0x00058100
		private bool EditedNameIsAllowed(string text, ComboBoxSchemeControl toBeRenamedControl)
		{
			if (string.IsNullOrEmpty(text.Trim()))
			{
				this.ParentWindow.mCommonHandler.AddToastPopup(this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), 1.3, false);
				return false;
			}
			foreach (object obj in this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
			{
				ComboBoxSchemeControl comboBoxSchemeControl = (ComboBoxSchemeControl)obj;
				if (comboBoxSchemeControl.mSchemeName.Text.ToLower(CultureInfo.InvariantCulture).Trim() == text.ToLower(CultureInfo.InvariantCulture).Trim() && comboBoxSchemeControl != toBeRenamedControl)
				{
					this.ParentWindow.mCommonHandler.AddToastPopup(this.CanvasWindow.SidebarWindow, LocaleStrings.GetLocalizedString("STRING_INVALID_SCHEME_NAME", ""), 1.3, false);
					return false;
				}
				if (comboBoxSchemeControl.mSchemeName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
				{
					string text2 = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[]
					{
						LocaleStrings.GetLocalizedString("STRING_SCHEME_INVALID_CHARACTERS", ""),
						Environment.NewLine,
						"\\ / : * ? \" < > |"
					});
					this.ParentWindow.mCommonHandler.AddToastPopup(this.CanvasWindow.SidebarWindow, text2, 3.0, false);
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x0005A0B0 File Offset: 0x000582B0
		private void CopyImg_MouseDown(object sender, MouseButtonEventArgs e)
		{
			bool flag = false;
			foreach (object obj in this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
			{
				ComboBoxSchemeControl comboBoxSchemeControl = (ComboBoxSchemeControl)obj;
				if (!comboBoxSchemeControl.mSchemeName.IsReadOnly)
				{
					this.HandleNameEdit(comboBoxSchemeControl);
					flag = true;
					e.Handled = true;
					break;
				}
			}
			if (!flag)
			{
				KMManager.AddNewControlSchemeAndSelect(this.ParentWindow, this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text], true);
			}
			e.Handled = true;
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x0005A170 File Offset: 0x00058370
		private void DeleteImg_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.mSchemeName.IsReadOnly)
			{
				this.HandleNameEdit(this);
			}
			if (!this.ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup)
			{
				this.DeleteControlScheme();
				e.Handled = true;
				return;
			}
			this.mDeleteScriptMessageWindow = new CustomMessageWindow();
			this.mDeleteScriptMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCHEME", "");
			this.mDeleteScriptMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_SCHEME_CONFIRMATION", "");
			this.mDeleteScriptMessageWindow.CheckBox.Content = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_GOOGLE_APP_POPUP_STRING_04", "");
			this.mDeleteScriptMessageWindow.CheckBox.Visibility = Visibility.Visible;
			this.mDeleteScriptMessageWindow.CheckBox.IsChecked = new bool?(false);
			this.mDeleteScriptMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_DELETE", ""), new EventHandler(this.UpdateSettingsAndDeleteScheme), null, false, null);
			this.mDeleteScriptMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), delegate(object o, EventArgs evt)
			{
				KeymapCanvasWindow.sIsDirty = false;
				GuidanceWindow.sIsDirty = false;
			}, null, false, null);
			this.mDeleteScriptMessageWindow.CloseButtonHandle(delegate(object o, EventArgs evt)
			{
			}, null);
			this.mDeleteScriptMessageWindow.Owner = this.CanvasWindow;
			this.mDeleteScriptMessageWindow.ShowDialog();
			e.Handled = true;
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x0005A2F8 File Offset: 0x000584F8
		private void UpdateSettingsAndDeleteScheme(object sender, EventArgs e)
		{
			this.ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup = !this.mDeleteScriptMessageWindow.CheckBox.IsChecked.Value;
			this.mDeleteScriptMessageWindow = null;
			this.DeleteControlScheme();
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x0005A340 File Offset: 0x00058540
		private void DeleteControlScheme()
		{
			if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(this.mSchemeName.Text) && !this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].BuiltIn)
			{
				if (this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].Selected)
				{
					this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].Selected = false;
					if (this.ParentWindow.SelectedConfig.ControlSchemes.Count > 1)
					{
						if (this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem == (this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children[0] as ComboBoxSchemeControl).mSchemeName.Text.ToString(CultureInfo.InvariantCulture))
						{
							this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = (this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children[1] as ComboBoxSchemeControl).mSchemeName.Text.ToString(CultureInfo.InvariantCulture);
						}
						else
						{
							this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = (this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children[0] as ComboBoxSchemeControl).mSchemeName.Text.ToString(CultureInfo.InvariantCulture);
						}
						this.ParentWindow.SelectedConfig.ControlSchemesDict[this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem].Selected = true;
						this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName.Text = this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem;
						this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem];
						this.CanvasWindow.SidebarWindow.ProfileChanged();
					}
					else
					{
						this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem = null;
						BlueStacksUIBinding.Bind(this.CanvasWindow.SidebarWindow.mSchemeComboBox.mName, "Custom", "");
					}
				}
				this.ParentWindow.SelectedConfig.ControlSchemes.Remove(this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text]);
				this.ParentWindow.SelectedConfig.ControlSchemesDict.Remove(this.mSchemeName.Text);
				ComboBoxSchemeControl comboBoxSchemeControlFromName = KMManager.GetComboBoxSchemeControlFromName(this.mSchemeName.Text);
				if (comboBoxSchemeControlFromName != null)
				{
					this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children.Remove(comboBoxSchemeControlFromName);
				}
				KeymapCanvasWindow.sIsDirty = true;
				this.CanvasWindow.SidebarWindow.FillProfileCombo();
				if (this.ParentWindow.SelectedConfig.ControlSchemes.Count == 0)
				{
					this.CanvasWindow.ClearWindow();
				}
			}
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x0005A6A4 File Offset: 0x000588A4
		private void ComboBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
		{
			bool flag = false;
			foreach (object obj in this.CanvasWindow.SidebarWindow.mSchemeComboBox.Items.Children)
			{
				ComboBoxSchemeControl comboBoxSchemeControl = (ComboBoxSchemeControl)obj;
				if (!comboBoxSchemeControl.mSchemeName.IsReadOnly)
				{
					this.HandleNameEdit(comboBoxSchemeControl);
					flag = true;
					e.Handled = true;
					break;
				}
			}
			if (!flag)
			{
				if (this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem == this.mSchemeName.Text)
				{
					this.CanvasWindow.SidebarWindow.mSchemeComboBox.mItems.IsOpen = false;
					return;
				}
				if (this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem != null)
				{
					this.ParentWindow.SelectedConfig.ControlSchemesDict[this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem].Selected = false;
				}
				this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text].Selected = true;
				this.ParentWindow.SelectedConfig.ControlSchemesDict[this.ParentWindow.SelectedConfig.SelectedControlScheme.Name].Selected = false;
				this.ParentWindow.SelectedConfig.SelectedControlScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[this.mSchemeName.Text];
				this.CanvasWindow.SidebarWindow.FillProfileCombo();
				this.CanvasWindow.SidebarWindow.ProfileChanged();
				this.CanvasWindow.SidebarWindow.mSchemeComboBox.mItems.IsOpen = false;
				KeymapCanvasWindow.sIsDirty = true;
				KMManager.SendSchemeChangedStats(this.ParentWindow, "controls_editor");
			}
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x00006F4D File Offset: 0x0000514D
		private void ComboBoxItem_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x0005A894 File Offset: 0x00058A94
		private void ComboBoxItem_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.mSchemeName.Text != this.CanvasWindow.SidebarWindow.mSchemeComboBox.SelectedItem)
			{
				BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ComboBoxBackgroundColor");
				return;
			}
			BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x0000AC92 File Offset: 0x00008E92
		private void ComboBoxItem_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.mSchemeName.Focusable)
			{
				this.HandleNameEdit(this);
			}
			e.Handled = true;
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x0005A8EC File Offset: 0x00058AEC
		private void HandleNameEdit(ComboBoxSchemeControl control)
		{
			control.mEditImg.Visibility = Visibility.Visible;
			control.mSaveImg.Visibility = Visibility.Collapsed;
			if (this.EditedNameIsAllowed(control.mSchemeName.Text, control))
			{
				if (this.ParentWindow.SelectedConfig.ControlSchemesDict.ContainsKey(control.mOldSchemeName))
				{
					IMControlScheme imcontrolScheme = this.ParentWindow.SelectedConfig.ControlSchemesDict[control.mOldSchemeName];
					imcontrolScheme.Name = control.mSchemeName.Text.Trim();
					this.ParentWindow.SelectedConfig.ControlSchemesDict.Remove(control.mOldSchemeName);
					this.ParentWindow.SelectedConfig.ControlSchemesDict.Add(imcontrolScheme.Name, imcontrolScheme);
					this.CanvasWindow.SidebarWindow.FillProfileCombo();
					KeymapCanvasWindow.sIsDirty = true;
				}
			}
			else
			{
				control.mSchemeName.Text = control.mOldSchemeName;
			}
			control.mSchemeName.Focusable = false;
			control.mSchemeName.IsReadOnly = true;
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x0000ACAF File Offset: 0x00008EAF
		private void MSchemeName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.HandleNameEdit(this);
			}
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x0005A9F4 File Offset: 0x00058BF4
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/comboboxschemecontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x0005AA24 File Offset: 0x00058C24
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
				((ComboBoxSchemeControl)target).MouseDown += this.ComboBoxItem_MouseDown;
				((ComboBoxSchemeControl)target).MouseEnter += this.ComboBoxItem_MouseEnter;
				((ComboBoxSchemeControl)target).MouseLeave += this.ComboBoxItem_MouseLeave;
				((ComboBoxSchemeControl)target).LostFocus += this.ComboBoxItem_LostFocus;
				return;
			case 2:
				this.mSchemeControl = (Grid)target;
				return;
			case 3:
				this.mBookmarkImg = (CustomPictureBox)target;
				this.mBookmarkImg.MouseDown += this.Bookmark_img_MouseDown;
				return;
			case 4:
				this.mSchemeName = (CustomTextBox)target;
				this.mSchemeName.KeyUp += this.MSchemeName_KeyUp;
				return;
			case 5:
				this.mEditImg = (CustomPictureBox)target;
				this.mEditImg.MouseDown += this.EditImg_MouseDown;
				return;
			case 6:
				this.mSaveImg = (CustomPictureBox)target;
				this.mSaveImg.MouseDown += this.SaveImg_MouseDown;
				return;
			case 7:
				this.mCopyImg = (CustomPictureBox)target;
				this.mCopyImg.MouseDown += this.CopyImg_MouseDown;
				return;
			case 8:
				this.mDeleteImg = (CustomPictureBox)target;
				this.mDeleteImg.MouseDown += this.DeleteImg_MouseDown;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000922 RID: 2338
		private KeymapCanvasWindow CanvasWindow;

		// Token: 0x04000923 RID: 2339
		private MainWindow ParentWindow;

		// Token: 0x04000924 RID: 2340
		private CustomMessageWindow mDeleteScriptMessageWindow;

		// Token: 0x04000925 RID: 2341
		internal string mOldSchemeName;

		// Token: 0x04000926 RID: 2342
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mSchemeControl;

		// Token: 0x04000927 RID: 2343
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mBookmarkImg;

		// Token: 0x04000928 RID: 2344
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mSchemeName;

		// Token: 0x04000929 RID: 2345
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mEditImg;

		// Token: 0x0400092A RID: 2346
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mSaveImg;

		// Token: 0x0400092B RID: 2347
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCopyImg;

		// Token: 0x0400092C RID: 2348
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mDeleteImg;

		// Token: 0x0400092D RID: 2349
		private bool _contentLoaded;
	}
}
