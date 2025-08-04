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
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000CF RID: 207
	public class ShortcutKeyControlElement : UserControl, IComponentConnector
	{
		// Token: 0x1700022C RID: 556
		// (get) Token: 0x0600086E RID: 2158 RVA: 0x00007752 File Offset: 0x00005952
		// (set) Token: 0x0600086F RID: 2159 RVA: 0x0000775A File Offset: 0x0000595A
		internal List<ShortcutKeys> mUserDefinedConfigList { get; set; }

		// Token: 0x06000870 RID: 2160 RVA: 0x0002F39C File Offset: 0x0002D59C
		public ShortcutKeyControlElement(MainWindow window, SettingsWindow settingsWindow)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			this.ParentSettingsWindow = settingsWindow;
			InputMethod.SetIsInputMethodEnabled(this.mShortcutKeyTextBox, false);
			MainWindow parentWindow = this.ParentWindow;
			foreach (string text in (parentWindow != null) ? parentWindow.mCommonHandler.mShortcutsConfigInstance.DefaultModifier.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null)
			{
				Key key = (Key)Enum.Parse(typeof(Key), text);
				this.mDefaultModifierForUI = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(key) + " + ";
				this.mDefaultModifierForFile = this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(key) + " + ";
			}
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x00007763 File Offset: 0x00005963
		private static bool IsValid(Key key)
		{
			return key != Key.LeftAlt && key != Key.RightAlt && key != Key.LeftShift && key != Key.RightShift && key != Key.LeftCtrl && key != Key.RightCtrl && key != Key.None && key != Key.System;
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x00007791 File Offset: 0x00005991
		private void ShortcutKeyTextBoxKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Snapshot || e.SystemKey == Key.Snapshot)
			{
				this.HandleShortcutKeyDown(e);
			}
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x000077AE File Offset: 0x000059AE
		private void ShortcutKeyTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			this.HandleShortcutKeyDown(e);
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x0002F474 File Offset: 0x0002D674
		private void HandleShortcutKeyDown(KeyEventArgs e)
		{
			Logger.Debug("SHORTCUT: PrintKey............" + e.Key.ToString());
			Logger.Debug("SHORTCUT: PrintSystemKey............" + e.SystemKey.ToString());
			if (((IMAPKeys.mDictKeys.ContainsKey(e.Key) || IMAPKeys.mDictKeys.ContainsKey(e.SystemKey)) && (ShortcutKeyControlElement.IsValid(e.Key) || ShortcutKeyControlElement.IsValid(e.SystemKey))) || e.Key == Key.Back || e.Key == Key.Delete)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				this.mShortcutKeyTextBox.Tag = string.Empty;
				if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
				{
					if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
					{
						text = IMAPKeys.GetStringForUI(Key.LeftCtrl) + " + ";
						this.mShortcutKeyTextBox.Tag = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
					}
					if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
					{
						text2 = IMAPKeys.GetStringForUI(Key.LeftAlt) + " + ";
						CustomTextBox customTextBox = this.mShortcutKeyTextBox;
						object tag = customTextBox.Tag;
						customTextBox.Tag = ((tag != null) ? tag.ToString() : null) + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
					}
					if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
					{
						text3 = IMAPKeys.GetStringForUI(Key.LeftShift) + " + ";
						CustomTextBox customTextBox2 = this.mShortcutKeyTextBox;
						object tag2 = customTextBox2.Tag;
						customTextBox2.Tag = ((tag2 != null) ? tag2.ToString() : null) + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
					}
					if ((string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2)) || e.SystemKey == Key.F10)
					{
						this.mShortcutKeyTextBox.Text = text + text2 + text3 + IMAPKeys.GetStringForUI(e.SystemKey);
						CustomTextBox customTextBox3 = this.mShortcutKeyTextBox;
						object tag3 = customTextBox3.Tag;
						customTextBox3.Tag = ((tag3 != null) ? tag3.ToString() : null) + IMAPKeys.GetStringForFile(e.SystemKey);
					}
					else
					{
						this.mShortcutKeyTextBox.Text = text + text2 + text3 + IMAPKeys.GetStringForUI(e.Key);
						CustomTextBox customTextBox4 = this.mShortcutKeyTextBox;
						object tag4 = customTextBox4.Tag;
						customTextBox4.Tag = ((tag4 != null) ? tag4.ToString() : null) + IMAPKeys.GetStringForFile(e.Key);
					}
				}
				else if (e.Key == Key.Back || e.Key == Key.Delete)
				{
					this.mShortcutKeyTextBox.Text = string.Empty;
					this.mShortcutKeyTextBox.Tag = string.Empty;
					if (this.ParentSettingsWindow.mDuplicateShortcutsList.Contains(this.mShortcutNameTextBlock.Text))
					{
						this.ParentSettingsWindow.mDuplicateShortcutsList.Remove(this.mShortcutNameTextBlock.Text);
					}
					this.SetSaveButtonState(this.ParentSettingsWindow.mIsShortcutEdited);
				}
				else if (e.Key == Key.Escape)
				{
					if (string.Equals(this.mDefaultModifierForFile, "Shift + ", StringComparison.InvariantCulture))
					{
						this.mShortcutKeyTextBox.Text = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.Key);
						this.mShortcutKeyTextBox.Tag = this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.Key);
					}
				}
				else if ((e.Key == Key.D0 || e.SystemKey == Key.D0) && string.Equals(this.mDefaultModifierForUI, "Ctrl + Shift + ", StringComparison.InvariantCulture))
				{
					this.mShortcutKeyTextBox.Text = string.Empty;
					this.mShortcutKeyTextBox.Tag = string.Empty;
					this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_WINDOW_ACTION_ERROR", ""));
				}
				else if (e.Key == Key.System)
				{
					this.mShortcutKeyTextBox.Text = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.SystemKey);
					this.mShortcutKeyTextBox.Tag = this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.SystemKey);
				}
				else
				{
					this.mShortcutKeyTextBox.Text = this.mDefaultModifierForUI + IMAPKeys.GetStringForUI(e.Key);
					this.mShortcutKeyTextBox.Tag = this.mDefaultModifierForFile + IMAPKeys.GetStringForFile(e.Key);
				}
				e.Handled = true;
				this.mShortcutKeyTextBox.CaretIndex = this.mShortcutKeyTextBox.Text.Length;
				this.mIsShortcutSameAsMacroShortcut = false;
				if ((MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForUI(e.Key)) || MainWindow.sMacroMapping.ContainsKey(IMAPKeys.GetStringForUI(e.SystemKey))) && (string.Equals(this.mShortcutKeyTextBox.Text, text + text2 + IMAPKeys.GetStringForUI(e.Key), StringComparison.InvariantCulture) || string.Equals(this.mShortcutKeyTextBox.Text, text + text2 + IMAPKeys.GetStringForUI(e.SystemKey), StringComparison.InvariantCulture)))
				{
					this.mIsShortcutSameAsMacroShortcut = true;
				}
				if (string.Equals(this.mShortcutKeyTextBox.Text, "Alt + F4", StringComparison.InvariantCulture))
				{
					this.mShortcutKeyTextBox.Text = string.Empty;
					this.mShortcutKeyTextBox.Tag = string.Empty;
					this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_WINDOW_ACTION_ERROR", ""));
				}
				foreach (ShortcutKeys shortcutKeys in this.mUserDefinedConfigList)
				{
					this.mShortcutKeyTextBox.InputTextValidity = TextValidityOptions.Success;
					this.mKeyInfoPopup.IsOpen = false;
					this.ParentSettingsWindow.mIsShortcutEdited = true;
					this.CheckIfShortcutAlreadyUsed();
					this.ParentWindow.mCommonHandler.OnShortcutKeysRefresh();
					if (string.Equals(LocaleStrings.GetLocalizedString(shortcutKeys.ShortcutName, ""), this.mShortcutNameTextBlock.Text, StringComparison.InvariantCulture) && !string.Equals(shortcutKeys.ShortcutKey, this.mShortcutKeyTextBox.Text, StringComparison.InvariantCulture))
					{
						shortcutKeys.ShortcutKey = this.mShortcutKeyTextBox.Tag.ToString();
						Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_edit", this.mShortcutNameTextBlock.Text, null, null, null, null, "Android", 0);
					}
				}
			}
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x0002FACC File Offset: 0x0002DCCC
		private void AddToastPopup(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this.ParentSettingsWindow);
				}
				this.mToastPopup.Init(this.ParentSettingsWindow, message, null, null, HorizontalAlignment.Center, VerticalAlignment.Top, null, 12, null, null);
				this.mToastPopup.Margin = new Thickness(20.0, 30.0, 0.0, 0.0);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x0002FB90 File Offset: 0x0002DD90
		private void CheckIfShortcutAlreadyUsed()
		{
			this.mErrorMessageShown = false;
			foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
			{
				if ((!string.IsNullOrEmpty(shortcutKeys.ShortcutKey) && string.Equals(shortcutKeys.ShortcutKey, this.mShortcutKeyTextBox.Tag.ToString(), StringComparison.InvariantCulture) && !string.Equals(LocaleStrings.GetLocalizedString(shortcutKeys.ShortcutName, ""), this.mShortcutNameTextBlock.Text, StringComparison.InvariantCulture)) || this.mIsShortcutSameAsMacroShortcut)
				{
					this.mKeyInfoPopup.PlacementTarget = this.mShortcutKeyTextBox;
					this.mShortcutKeyTextBox.InputTextValidity = TextValidityOptions.Error;
					this.mKeyInfoPopup.IsOpen = true;
					this.mErrorMessageShown = true;
					if (!this.ParentSettingsWindow.mDuplicateShortcutsList.Contains(this.mShortcutNameTextBlock.Text))
					{
						this.ParentSettingsWindow.mDuplicateShortcutsList.Add(this.mShortcutNameTextBlock.Text);
					}
				}
			}
			if (!this.mErrorMessageShown && this.ParentSettingsWindow.mDuplicateShortcutsList.Contains(this.mShortcutNameTextBlock.Text))
			{
				this.ParentSettingsWindow.mDuplicateShortcutsList.Remove(this.mShortcutNameTextBlock.Text);
			}
			this.SetSaveButtonState(this.ParentSettingsWindow.mIsShortcutEdited);
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x0002FD08 File Offset: 0x0002DF08
		private void SetSaveButtonState(bool isEdited)
		{
			if (this.ParentSettingsWindow.mDuplicateShortcutsList.Count == 0 && isEdited)
			{
				this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(true);
				this.ParentSettingsWindow.mIsShortcutSaveBtnEnabled = true;
				return;
			}
			this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(false);
			this.ParentSettingsWindow.mIsShortcutSaveBtnEnabled = false;
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x0002FD68 File Offset: 0x0002DF68
		private void ShortcutKeyTextBoxMouseEnter(object sender, MouseEventArgs e)
		{
			if (this.mShortcutKeyTextBox.InputTextValidity == TextValidityOptions.Error)
			{
				this.mKeyInfoPopup.PlacementTarget = this.mShortcutKeyTextBox;
				this.mKeyInfoPopup.IsOpen = true;
				this.mKeyInfoPopup.StaysOpen = true;
				return;
			}
			this.mKeyInfoPopup.IsOpen = false;
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x000077B7 File Offset: 0x000059B7
		private void ShortcutKeyTextBoxMouseLeave(object sender, MouseEventArgs e)
		{
			this.mKeyInfoPopup.IsOpen = false;
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x0002FDBC File Offset: 0x0002DFBC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/shortcutkeycontrolelement.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x0002FDEC File Offset: 0x0002DFEC
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
				this.mShortcutNameTextBlock = (TextBlock)target;
				return;
			case 2:
				this.mShortcutKeyTextBox = (CustomTextBox)target;
				this.mShortcutKeyTextBox.PreviewKeyDown += this.ShortcutKeyTextBoxKeyDown;
				this.mShortcutKeyTextBox.MouseEnter += this.ShortcutKeyTextBoxMouseEnter;
				this.mShortcutKeyTextBox.MouseLeave += this.ShortcutKeyTextBoxMouseLeave;
				this.mShortcutKeyTextBox.PreviewKeyUp += this.ShortcutKeyTextBoxKeyUp;
				return;
			case 3:
				this.mKeyInfoPopup = (CustomPopUp)target;
				return;
			case 4:
				this.mMaskBorder = (Border)target;
				return;
			case 5:
				this.mKeyInfoText = (TextBlock)target;
				return;
			case 6:
				this.mDownArrow = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040004BB RID: 1211
		internal MainWindow ParentWindow;

		// Token: 0x040004BC RID: 1212
		internal SettingsWindow ParentSettingsWindow;

		// Token: 0x040004BD RID: 1213
		internal string mDefaultModifierForUI = string.Empty;

		// Token: 0x040004BE RID: 1214
		internal string mDefaultModifierForFile = string.Empty;

		// Token: 0x040004BF RID: 1215
		private bool mErrorMessageShown;

		// Token: 0x040004C0 RID: 1216
		internal bool mIsShortcutSameAsMacroShortcut;

		// Token: 0x040004C1 RID: 1217
		private CustomToastPopupControl mToastPopup;

		// Token: 0x040004C2 RID: 1218
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mShortcutNameTextBlock;

		// Token: 0x040004C3 RID: 1219
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mShortcutKeyTextBox;

		// Token: 0x040004C4 RID: 1220
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mKeyInfoPopup;

		// Token: 0x040004C5 RID: 1221
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x040004C6 RID: 1222
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mKeyInfoText;

		// Token: 0x040004C7 RID: 1223
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path mDownArrow;

		// Token: 0x040004C8 RID: 1224
		private bool _contentLoaded;
	}
}
