using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000D0 RID: 208
	public class ShortcutKeysControl : UserControl, IComponentConnector
	{
		// Token: 0x0600087D RID: 2173 RVA: 0x0002FED0 File Offset: 0x0002E0D0
		public ShortcutKeysControl(MainWindow window, SettingsWindow settingsWindow)
		{
			this.InitializeComponent();
			base.Visibility = Visibility.Hidden;
			this.ParentWindow = window;
			this.ParentSettingsWindow = settingsWindow;
			this.mShortcutKeyPanel = this.mShortcutKeyScrollBar.Content as StackPanel;
			if (this.ParentWindow != null)
			{
				if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
				{
					this.AddShortcutKeyElements();
				}
				if (!string.IsNullOrEmpty(RegistryManager.Instance.UserDefinedShortcuts))
				{
					this.mRevertBtn.IsEnabled = true;
				}
				this.ParentWindow.mCommonHandler.ShortcutKeysChangedEvent += this.ShortcutKeysChangedEvent;
				this.ParentWindow.mCommonHandler.ShortcutKeysRefreshEvent += this.ShortcutKeysRefreshEvent;
			}
			this.mShortcutKeyScrollBar.ScrollChanged += BluestacksUIColor.ScrollBarScrollChanged;
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x0002FFAC File Offset: 0x0002E1AC
		private void ShortcutKeysRefreshEvent()
		{
			IEnumerable<ShortcutKeyControlElement> enumerable = from ele in (from ele in this.mShortcutUIElements.SelectMany((KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>> x) => x.Value.Item2)
					group ele by ele.mShortcutKeyTextBox.Text into grp
					where grp.Count<ShortcutKeyControlElement>() == 1
					select grp).SelectMany((IGrouping<string, ShortcutKeyControlElement> grp) => grp)
				where !string.IsNullOrEmpty(ele.mShortcutKeyTextBox.Text)
				select ele;
			int num = 0;
			foreach (ShortcutKeyControlElement shortcutKeyControlElement in enumerable)
			{
				if (!shortcutKeyControlElement.mIsShortcutSameAsMacroShortcut)
				{
					shortcutKeyControlElement.mKeyInfoPopup.IsOpen = false;
					shortcutKeyControlElement.mShortcutKeyTextBox.InputTextValidity = TextValidityOptions.Success;
					num++;
				}
			}
			if (num == (from ele in this.mShortcutUIElements.SelectMany((KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>> x) => x.Value.Item2)
				where !string.IsNullOrEmpty(ele.mShortcutKeyTextBox.Text)
				select ele).Count<ShortcutKeyControlElement>())
			{
				this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(true);
				return;
			}
			this.ParentWindow.mCommonHandler.OnShortcutKeysChanged(false);
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x000077C5 File Offset: 0x000059C5
		private void ShortcutKeysChangedEvent(bool isEnabled)
		{
			this.mSaveBtn.IsEnabled = isEnabled;
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x00030148 File Offset: 0x0002E348
		private void AddShortcutKeyElements()
		{
			try
			{
				new List<ShortcutKeys>();
				this.mShortcutKeyPanel.Children.Clear();
				this.mShortcutUIElements.Clear();
				foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
				{
					this.CreateShortcutCategory(shortcutKeys.ShortcutCategory);
					this.AddElement(shortcutKeys);
				}
				foreach (KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>> keyValuePair in this.mShortcutUIElements)
				{
					this.mShortcutKeyPanel.Children.Add(keyValuePair.Value.Item1);
					foreach (FrameworkElement frameworkElement in keyValuePair.Value.Item2)
					{
						(keyValuePair.Value.Item1.Content as StackPanel).Children.Add(frameworkElement);
					}
				}
				this.mShortcutUIElements.First<KeyValuePair<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>>().Value.Item1.Margin = new Thickness(0.0);
			}
			catch (Exception ex)
			{
				Logger.Error("Error in adding shortcut elements: " + ex.ToString());
			}
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x0003031C File Offset: 0x0002E51C
		private void AddElement(ShortcutKeys ele)
		{
			ShortcutKeyControlElement shortcutKeyControlElement = new ShortcutKeyControlElement(this.ParentWindow, this.ParentSettingsWindow);
			BlueStacksUIBinding.Bind(shortcutKeyControlElement.mShortcutNameTextBlock, ele.ShortcutName, "");
			string[] array = ele.ShortcutKey.Split(new char[] { '+', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string text = string.Empty;
			foreach (string text2 in array)
			{
				text = text + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(text2), "") + " + ";
			}
			this.mShortcutUIElements[ele.ShortcutCategory].Item2.Add(shortcutKeyControlElement);
			if (!string.IsNullOrEmpty(text))
			{
				shortcutKeyControlElement.mShortcutKeyTextBox.Text = text.Substring(0, text.Length - 3);
			}
			shortcutKeyControlElement.mUserDefinedConfigList = new List<ShortcutKeys> { ele };
			if (ele.ReadOnlyTextbox)
			{
				shortcutKeyControlElement.mShortcutKeyTextBox.IsEnabled = false;
			}
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x00030414 File Offset: 0x0002E614
		private void CreateShortcutCategory(string categoryName)
		{
			if (!this.mShortcutUIElements.ContainsKey(categoryName))
			{
				string localizedString = LocaleStrings.GetLocalizedString(categoryName, "");
				GroupBox groupBox = new GroupBox
				{
					Content = new StackPanel(),
					Header = localizedString,
					Tag = categoryName,
					Margin = new Thickness(0.0, 20.0, 0.0, 0.0),
					FontSize = 16.0
				};
				BlueStacksUIBinding.BindColor(groupBox, Control.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
				groupBox.BorderThickness = new Thickness(0.0);
				BlueStacksUIBinding.BindColor(new TextBlock
				{
					Text = localizedString,
					Tag = categoryName,
					FontStretch = FontStretches.ExtraExpanded,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(0.0, 0.0, 0.0, 10.0),
					TextWrapping = TextWrapping.WrapWithOverflow
				}, TextBlock.ForegroundProperty, "SettingsWindowTabMenuItemLegendForeground");
				this.mShortcutUIElements.Add(categoryName, new BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>(groupBox, new List<ShortcutKeyControlElement>()));
			}
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00030540 File Offset: 0x0002E740
		private void SaveBtnClick(object sender, RoutedEventArgs e)
		{
			this.ParentWindow.mCommonHandler.SaveAndReloadShortcuts();
			this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
			this.ParentSettingsWindow.mIsShortcutEdited = false;
			this.mSaveBtn.IsEnabled = false;
			this.mRevertBtn.IsEnabled = true;
			if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut != null)
			{
				this.RefreshShortcutConfigForUI();
			}
			TopbarOptions mTopbarOptions = this.ParentWindow.mTopbarOptions;
			if (mTopbarOptions != null)
			{
				mTopbarOptions.SetLabel();
			}
			ClientStats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Shortcut-Settings", "", null, this.ParentWindow.mVmName, null, null);
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x00030600 File Offset: 0x0002E800
		private void RefreshShortcutConfigForUI()
		{
			foreach (ShortcutKeys shortcutKeys in this.ParentWindow.mCommonHandler.mShortcutsConfigInstance.Shortcut)
			{
				foreach (ShortcutKeyControlElement shortcutKeyControlElement in this.mShortcutUIElements[shortcutKeys.ShortcutCategory].Item2)
				{
					ShortcutKeyControlElement shortcutKeyControlElement2 = shortcutKeyControlElement as ShortcutKeyControlElement;
					if (shortcutKeyControlElement2 != null && string.Equals(shortcutKeyControlElement2.mShortcutNameTextBlock.Text, LocaleStrings.GetLocalizedString(shortcutKeys.ShortcutName, ""), StringComparison.InvariantCulture))
					{
						shortcutKeyControlElement2.mUserDefinedConfigList = new List<ShortcutKeys> { shortcutKeys };
					}
				}
			}
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x000306EC File Offset: 0x0002E8EC
		private void RevertBtnClick(object sender, RoutedEventArgs e)
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTORE_DEFAULTS", "");
			customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTORE_SHORTCUTS", "");
			customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_RESTORE_BUTTON", ""), delegate(object o, EventArgs evt)
			{
				this.RestoreDefaultShortcuts();
				this.mRevertBtn.IsEnabled = false;
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CANCEL", ""), delegate(object o, EventArgs evt)
			{
			}, null, false, null);
			customMessageWindow.CloseButtonHandle(null, null);
			customMessageWindow.Owner = this.ParentWindow;
			customMessageWindow.ShowDialog();
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x000307AC File Offset: 0x0002E9AC
		private void RestoreDefaultShortcuts()
		{
			RegistryManager.Instance.UserDefinedShortcuts = string.Empty;
			this.ParentSettingsWindow.mIsShortcutEdited = false;
			CommonHandlers.ReloadShortcutsForAllInstances();
			if (this.ParentWindow.mCommonHandler.mShortcutsConfigInstance != null)
			{
				this.AddShortcutKeyElements();
			}
			this.mSaveBtn.IsEnabled = false;
			TopbarOptions mTopbarOptions = this.ParentWindow.mTopbarOptions;
			if (mTopbarOptions != null)
			{
				mTopbarOptions.SetLabel();
			}
			Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_restore_default", null, null, null, null, null, "Android", 0);
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x00030844 File Offset: 0x0002EA44
		private void AddToastPopup(string message)
		{
			try
			{
				if (this.mToastPopup == null)
				{
					this.mToastPopup = new CustomToastPopupControl(this);
				}
				this.mToastPopup.Init(this.ParentWindow, message, null, null, HorizontalAlignment.Center, VerticalAlignment.Bottom, null, 12, null, null, false);
				this.mToastPopup.ShowPopup(1.3);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in showing toast popup: " + ex.ToString());
			}
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x000308D0 File Offset: 0x0002EAD0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/shortcutkeyscontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x00030900 File Offset: 0x0002EB00
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
				this.mShortcutKeyScrollBar = (ScrollViewer)target;
				return;
			case 2:
				this.mRevertBtn = (CustomButton)target;
				this.mRevertBtn.Click += this.RevertBtnClick;
				return;
			case 3:
				this.mSaveBtn = (CustomButton)target;
				this.mSaveBtn.Click += this.SaveBtnClick;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040004C9 RID: 1225
		private MainWindow ParentWindow;

		// Token: 0x040004CA RID: 1226
		private SettingsWindow ParentSettingsWindow;

		// Token: 0x040004CB RID: 1227
		private StackPanel mShortcutKeyPanel;

		// Token: 0x040004CC RID: 1228
		private CustomToastPopupControl mToastPopup;

		// Token: 0x040004CD RID: 1229
		private Dictionary<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>> mShortcutUIElements = new Dictionary<string, BlueStacks.Common.Tuple<GroupBox, List<ShortcutKeyControlElement>>>();

		// Token: 0x040004CE RID: 1230
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mShortcutKeyScrollBar;

		// Token: 0x040004CF RID: 1231
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mRevertBtn;

		// Token: 0x040004D0 RID: 1232
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mSaveBtn;

		// Token: 0x040004D1 RID: 1233
		private bool _contentLoaded;
	}
}
