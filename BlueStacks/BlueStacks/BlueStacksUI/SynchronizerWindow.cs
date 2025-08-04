using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200012F RID: 303
	public class SynchronizerWindow : CustomWindow, IComponentConnector
	{
		// Token: 0x06000C23 RID: 3107 RVA: 0x00043AA4 File Offset: 0x00041CA4
		public SynchronizerWindow(MainWindow parent)
		{
			this.ParentWindow = parent;
			base.Owner = parent;
			base.IsShowGLWindow = true;
			this.InitializeComponent();
			string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=";
			this.mHyperLink.NavigateUri = new Uri(text + "operation_synchronization");
			this.mHyperLink.Inlines.Clear();
			this.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_SYNC_HELP", ""));
			BlueStacksUIBinding.Instance.PropertyChanged += this.Binding_PropertyChanged;
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.mSyncHelp.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x00043B80 File Offset: 0x00041D80
		private void Binding_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "LocaleModel")
			{
				this.mHyperLink.Inlines.Clear();
				this.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_SYNC_HELP", ""));
			}
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x00043BD0 File Offset: 0x00041DD0
		internal void Init()
		{
			this.mIsActiveWindowPresent = false;
			this.mActiveWindowsPanel.Children.Clear();
			foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
			{
				if (keyValuePair.Key != this.ParentWindow.mVmName && (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(keyValuePair.Key) || this.ParentWindow.mSelectedInstancesForSync.Contains(keyValuePair.Key)))
				{
					CustomCheckbox customCheckbox = new CustomCheckbox
					{
						Content = SynchronizerWindow.GetInstanceGameOrDisplayName(keyValuePair.Key),
						Tag = keyValuePair.Key
					};
					if (customCheckbox.Image != null)
					{
						customCheckbox.Image.Height = 16.0;
						customCheckbox.Image.Width = 16.0;
					}
					customCheckbox.Height = 25.0;
					customCheckbox.FontSize = 16.0;
					customCheckbox.Margin = new Thickness(12.0, 8.0, 0.0, 0.0);
					if (this.ParentWindow.mSelectedInstancesForSync.Contains(customCheckbox.Tag.ToString()))
					{
						customCheckbox.IsChecked = new bool?(true);
					}
					else
					{
						customCheckbox.IsChecked = new bool?(false);
					}
					customCheckbox.MouseEnter += this.InstanceCheckbox_MouseEnter;
					customCheckbox.MouseLeave += this.InstanceCheckbox_MouseLeave;
					customCheckbox.Checked += this.InstanceCheckbox_Checked;
					customCheckbox.Unchecked += this.InstanceCheckbox_Unchecked;
					this.mActiveWindowsPanel.Children.Add(customCheckbox);
					this.mIsActiveWindowPresent = true;
					this.mActiveWindowsListScrollbar.Visibility = Visibility.Visible;
				}
			}
			if (this.mIsActiveWindowPresent)
			{
				this.mLaunchInstanceManagerBtn.Visibility = Visibility.Collapsed;
				this.mNoActiveWindowsGrid.Visibility = Visibility.Collapsed;
				this.mStartSyncBtn.Visibility = Visibility.Visible;
				if (this.ParentWindow.mIsSynchronisationActive)
				{
					this.mStartSyncBtn.IsEnabled = false;
				}
				else
				{
					this.ToggleStartSyncButton();
				}
				this.ToggleSelectAllCheckboxSelection();
				return;
			}
			if (FeatureManager.Instance.IsCustomUIForNCSoft)
			{
				this.Close_MouseLeftButtonUp(null, null);
				return;
			}
			this.mActiveWindowsListScrollbar.Visibility = Visibility.Collapsed;
			this.mNoActiveWindowsGrid.Visibility = Visibility.Visible;
			this.mStartSyncBtn.Visibility = Visibility.Collapsed;
			this.mLaunchInstanceManagerBtn.Visibility = Visibility.Visible;
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x00043E70 File Offset: 0x00042070
		private void InstanceCheckbox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.mStopEventFromPropagatingFurther)
			{
				return;
			}
			this.mStopEventFromPropagatingFurther = true;
			CustomCheckbox customCheckbox = sender as CustomCheckbox;
			customCheckbox.IsChecked = new bool?(false);
			this.ParentWindow.mSelectedInstancesForSync.Remove(customCheckbox.Tag.ToString());
			this.ToggleSelectAllCheckboxSelection();
			if (this.ParentWindow.mIsSynchronisationActive)
			{
				HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", null, customCheckbox.Tag.ToString(), 0, null, false, 1, 0);
				BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.HideSyncPanel();
				if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox.Tag.ToString()))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Remove(customCheckbox.Tag.ToString());
				}
				if (this.ParentWindow.mSelectedInstancesForSync.Count == 0)
				{
					this.ParentWindow.mIsSynchronisationActive = false;
					this.ParentWindow.mIsSyncMaster = false;
					if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
					{
						BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.ParentWindow.mVmName);
					}
					this.ParentWindow._TopBar.HideSyncPanel();
					this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
				}
				this.UpdateOtherSyncWindows();
			}
			if (!this.ParentWindow.mIsSynchronisationActive)
			{
				this.ToggleStartSyncButton();
			}
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x00043FDC File Offset: 0x000421DC
		private void InstanceCheckbox_Checked(object sender, RoutedEventArgs e)
		{
			if (this.mStopEventFromPropagatingFurther)
			{
				return;
			}
			this.mStopEventFromPropagatingFurther = true;
			CustomCheckbox customCheckbox = sender as CustomCheckbox;
			customCheckbox.IsChecked = new bool?(true);
			this.ParentWindow.mSelectedInstancesForSync.Add(customCheckbox.Tag.ToString());
			this.ToggleSelectAllCheckboxSelection();
			if (this.ParentWindow.mIsSynchronisationActive)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string> { 
				{
					"instance",
					this.ParentWindow.mVmName
				} };
				HTTPUtils.SendRequestToEngineAsync("startSyncConsumer", dictionary, BlueStacksUIUtils.DictWindows[(sender as CustomCheckbox).Tag.ToString()].mVmName, 0, null, false, 1, 0);
				BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.ShowSyncPanel(false);
				if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox.Tag.ToString()))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Add(customCheckbox.Tag.ToString());
				}
				this.UpdateOtherSyncWindows();
			}
			else
			{
				this.ToggleStartSyncButton();
			}
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x000440EC File Offset: 0x000422EC
		private void mSelectAll_Checked(object sender, RoutedEventArgs e)
		{
			if (this.mStopEventFromPropagatingFurther)
			{
				return;
			}
			this.mStopEventFromPropagatingFurther = true;
			foreach (object obj in this.mActiveWindowsPanel.Children)
			{
				CustomCheckbox customCheckbox = (CustomCheckbox)obj;
				customCheckbox.IsChecked = new bool?(true);
				if (!this.ParentWindow.mSelectedInstancesForSync.Contains(customCheckbox.Tag.ToString()))
				{
					this.ParentWindow.mSelectedInstancesForSync.Add(customCheckbox.Tag.ToString());
					if (this.ParentWindow.mIsSynchronisationActive)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string> { 
						{
							"instance",
							this.ParentWindow.mVmName
						} };
						HTTPUtils.SendRequestToEngineAsync("startSyncConsumer", dictionary, customCheckbox.Tag.ToString(), 0, null, false, 1, 0);
						BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.ShowSyncPanel(false);
						if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox.Tag.ToString()))
						{
							BlueStacksUIUtils.sSyncInvolvedInstances.Add(customCheckbox.Tag.ToString());
						}
						this.UpdateOtherSyncWindows();
					}
				}
			}
			this.ToggleStartSyncButton();
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x00044248 File Offset: 0x00042448
		private void mSelectAll_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.mStopEventFromPropagatingFurther)
			{
				return;
			}
			this.mStopEventFromPropagatingFurther = true;
			foreach (object obj in this.mActiveWindowsPanel.Children)
			{
				CustomCheckbox customCheckbox = (CustomCheckbox)obj;
				customCheckbox.IsChecked = new bool?(false);
				if (this.ParentWindow.mSelectedInstancesForSync.Contains(customCheckbox.Tag.ToString()))
				{
					this.ParentWindow.mSelectedInstancesForSync.Remove(customCheckbox.Tag.ToString());
					if (this.ParentWindow.mIsSynchronisationActive)
					{
						HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", null, customCheckbox.Tag.ToString(), 0, null, false, 1, 0);
						BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.HideSyncPanel();
						if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox.Tag.ToString()))
						{
							BlueStacksUIUtils.sSyncInvolvedInstances.Remove(customCheckbox.Tag.ToString());
						}
					}
				}
			}
			if (this.ParentWindow.mIsSynchronisationActive)
			{
				this.ParentWindow.mIsSynchronisationActive = false;
				this.ParentWindow.mIsSyncMaster = false;
				if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.ParentWindow.mVmName);
				}
				this.ParentWindow._TopBar.HideSyncPanel();
				this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
				this.UpdateOtherSyncWindows();
			}
			this.ToggleStartSyncButton();
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x00009AD5 File Offset: 0x00007CD5
		private void InstanceCheckbox_MouseLeave(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as CustomCheckbox, Control.BackgroundProperty, "SettingsWindowBackground");
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x00009AEC File Offset: 0x00007CEC
		private void InstanceCheckbox_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(sender as CustomCheckbox, Control.BackgroundProperty, "GameControlNavigationBackgroundColor");
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x0002CE5C File Offset: 0x0002B05C
		private void Topbar_MouseDown(object sender, MouseButtonEventArgs e)
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

		// Token: 0x06000C2D RID: 3117 RVA: 0x00044400 File Offset: 0x00042600
		private void mStartSyncBtn_Click(object sender, RoutedEventArgs e)
		{
			this.mStartSyncBtn.IsEnabled = false;
			this.ParentWindow._TopBar.ShowSyncPanel(true);
			this.ParentWindow.mIsSyncMaster = true;
			if (!RegistryManager.Instance.IsSynchronizerUsedStatSent)
			{
				ClientStats.SendMiscellaneousStatsAsync("MultipleInstancesSynced", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null, null, null);
				RegistryManager.Instance.IsSynchronizerUsedStatSent = true;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IEnumerable<CustomCheckbox> enumerable = this.mActiveWindowsPanel.Children.OfType<CustomCheckbox>().Where(delegate(CustomCheckbox _)
			{
				bool? isChecked = _.IsChecked;
				bool flag = true;
				return (isChecked.GetValueOrDefault() == flag) & (isChecked != null);
			});
			if (enumerable.Any<CustomCheckbox>())
			{
				this.ParentWindow.mIsSynchronisationActive = true;
				dictionary.Add("instances", string.Join(",", enumerable.Select((CustomCheckbox _) => _.Tag.ToString()).ToArray<string>()));
				this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startOperationsSync", dictionary);
				enumerable.ToList<CustomCheckbox>().ForEach(delegate(CustomCheckbox customCheckbox)
				{
					BlueStacksUIUtils.DictWindows[customCheckbox.Tag.ToString()]._TopBar.ShowSyncPanel(false);
				});
			}
			foreach (CustomCheckbox customCheckbox2 in enumerable.ToList<CustomCheckbox>())
			{
				if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(customCheckbox2.Tag.ToString()))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Add(customCheckbox2.Tag.ToString());
				}
			}
			if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
			{
				BlueStacksUIUtils.sSyncInvolvedInstances.Add(this.ParentWindow.mVmName);
			}
			this.UpdateOtherSyncWindows();
			this.Close_MouseLeftButtonUp(null, null);
			if (RegistryManager.Instance.IsShowToastNotification)
			{
				this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STARTED", ""));
			}
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x00009B03 File Offset: 0x00007D03
		private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Hide();
			this.ShowWithParentWindow = false;
			if (this.ParentWindow != null)
			{
				this.ParentWindow.Focus();
			}
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x000434B8 File Offset: 0x000416B8
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			try
			{
				Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
				BlueStacksUIUtils.OpenUrl(e.Uri.AbsoluteUri);
				e.Handled = true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in opening url" + ex.ToString());
			}
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x00044620 File Offset: 0x00042820
		private void mLaunchInstanceManagerBtn_Click(object sender, RoutedEventArgs e)
		{
			BlueStacksUIUtils.LaunchMultiInstanceManager();
			ClientStats.SendMiscellaneousStatsAsync("syncWindow", RegistryManager.Instance.UserGuid, "MultiInstance", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, null, null);
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00009B26 File Offset: 0x00007D26
		private void ToggleStartSyncButton()
		{
			if (this.ParentWindow.mSelectedInstancesForSync.Count > 0)
			{
				this.mStartSyncBtn.IsEnabled = true;
				return;
			}
			this.mStartSyncBtn.IsEnabled = false;
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x00044670 File Offset: 0x00042870
		private void ToggleSelectAllCheckboxSelection()
		{
			this.mStopEventFromPropagatingFurther = true;
			if (this.ParentWindow.mSelectedInstancesForSync.Count == this.mActiveWindowsPanel.Children.Count)
			{
				this.mSelectAllCheckbox.IsChecked = new bool?(true);
			}
			else
			{
				this.mSelectAllCheckbox.IsChecked = new bool?(false);
			}
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x000446D4 File Offset: 0x000428D4
		private void SynchronizerWindow_Activated(object sender, EventArgs e)
		{
			if (this.mActiveWindowsPanel.Children.Count == 0)
			{
				if (FeatureManager.Instance.IsCustomUIForNCSoft)
				{
					this.Close_MouseLeftButtonUp(null, null);
				}
				else
				{
					this.mIsActiveWindowPresent = false;
					this.mActiveWindowsListScrollbar.Visibility = Visibility.Collapsed;
					this.mStartSyncBtn.Visibility = Visibility.Collapsed;
					this.mNoActiveWindowsGrid.Visibility = Visibility.Visible;
					this.mLaunchInstanceManagerBtn.Visibility = Visibility.Visible;
					this.mNoActiveWindowsGrid.Height = double.NaN;
					base.SizeToContent = SizeToContent.WidthAndHeight;
				}
			}
			base.Left = this.ParentWindow.Left + (this.ParentWindow.Width - base.Width) / 2.0;
			base.Top = this.ParentWindow.Top + (this.ParentWindow.Height - base.Height) / 2.0;
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000447B8 File Offset: 0x000429B8
		internal void PauseAllSyncOperations()
		{
			if (this.mStopEventFromPropagatingFurther)
			{
				return;
			}
			this.mStopEventFromPropagatingFurther = true;
			foreach (string text in this.ParentWindow.mSelectedInstancesForSync)
			{
				BlueStacksUIUtils.DictWindows[text]._TopBar.HideSyncPanel();
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "pause", "true" } };
			HTTPUtils.SendRequestToEngineAsync("playPauseSync", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0);
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x00044868 File Offset: 0x00042A68
		internal void StopAllSyncOperations()
		{
			if (this.mStopEventFromPropagatingFurther)
			{
				return;
			}
			this.mStopEventFromPropagatingFurther = true;
			this.ParentWindow.mIsSynchronisationActive = false;
			this.ParentWindow.mIsSyncMaster = false;
			foreach (string text in this.ParentWindow.mSelectedInstancesForSync)
			{
				BlueStacksUIUtils.DictWindows[text]._TopBar.HideSyncPanel();
				if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(text))
				{
					BlueStacksUIUtils.sSyncInvolvedInstances.Remove(text);
				}
			}
			if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName))
			{
				BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.ParentWindow.mVmName);
			}
			this.UpdateOtherSyncWindows();
			this.ParentWindow.mSelectedInstancesForSync.Clear();
			this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
			this.Init();
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0004497C File Offset: 0x00042B7C
		internal void PlayAllSyncOperations()
		{
			if (this.mStopEventFromPropagatingFurther)
			{
				return;
			}
			this.mStopEventFromPropagatingFurther = true;
			foreach (string text in this.ParentWindow.mSelectedInstancesForSync)
			{
				BlueStacksUIUtils.DictWindows[text]._TopBar.ShowSyncPanel(false);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "pause", "false" } };
			HTTPUtils.SendRequestToEngineAsync("playPauseSync", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0);
			this.mStopEventFromPropagatingFurther = false;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x00044A2C File Offset: 0x00042C2C
		private void UpdateOtherSyncWindows()
		{
			try
			{
				base.Dispatcher.Invoke(new Action(delegate
				{
					foreach (KeyValuePair<string, MainWindow> keyValuePair in BlueStacksUIUtils.DictWindows)
					{
						if (keyValuePair.Key != this.ParentWindow.mVmName && keyValuePair.Value.mSynchronizerWindow != null && keyValuePair.Value.mSynchronizerWindow.IsVisible)
						{
							keyValuePair.Value.mSynchronizerWindow.Init();
						}
					}
				}), new object[0]);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in updating instances for sync operation: " + ex.ToString());
			}
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x00044A84 File Offset: 0x00042C84
		private static string GetInstanceGameOrDisplayName(string vmName)
		{
			string appName = BlueStacksUIUtils.DictWindows[vmName]._TopBar.AppName;
			string characterName = BlueStacksUIUtils.DictWindows[vmName]._TopBar.CharacterName;
			string text;
			if (!string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(characterName))
			{
				text = appName + " " + characterName;
			}
			else
			{
				text = Utils.GetDisplayName(vmName, "bgp64");
			}
			return text;
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x00044AEC File Offset: 0x00042CEC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/synchronizerwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x00044B1C File Offset: 0x00042D1C
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
				((SynchronizerWindow)target).Activated += this.SynchronizerWindow_Activated;
				return;
			case 2:
				this.mMaskBorder = (Border)target;
				return;
			case 3:
				this.mTopGrid = (Grid)target;
				this.mTopGrid.MouseDown += this.Topbar_MouseDown;
				return;
			case 4:
				((CustomPictureBox)target).MouseLeftButtonUp += this.Close_MouseLeftButtonUp;
				return;
			case 5:
				this.mLineSeperator = (Border)target;
				return;
			case 6:
				this.mNoActiveWindowsGrid = (Grid)target;
				return;
			case 7:
				this.mActiveWindowsListScrollbar = (ScrollViewer)target;
				return;
			case 8:
				this.mSelectAllCheckbox = (CustomCheckbox)target;
				this.mSelectAllCheckbox.Checked += this.mSelectAll_Checked;
				this.mSelectAllCheckbox.Unchecked += this.mSelectAll_Unchecked;
				return;
			case 9:
				this.mActiveWindowsPanel = (StackPanel)target;
				return;
			case 10:
				this.mBottomGrid = (Grid)target;
				return;
			case 11:
				this.mLineSeperator1 = (Border)target;
				return;
			case 12:
				this.mStartSyncBtn = (CustomButton)target;
				this.mStartSyncBtn.Click += this.mStartSyncBtn_Click;
				return;
			case 13:
				this.mLaunchInstanceManagerBtn = (CustomButton)target;
				this.mLaunchInstanceManagerBtn.Click += this.mLaunchInstanceManagerBtn_Click;
				return;
			case 14:
				this.mSyncHelp = (TextBlock)target;
				return;
			case 15:
				this.mHyperLink = (Hyperlink)target;
				this.mHyperLink.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400076E RID: 1902
		private MainWindow ParentWindow;

		// Token: 0x0400076F RID: 1903
		private bool mIsActiveWindowPresent;

		// Token: 0x04000770 RID: 1904
		private bool mStopEventFromPropagatingFurther;

		// Token: 0x04000771 RID: 1905
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder;

		// Token: 0x04000772 RID: 1906
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mTopGrid;

		// Token: 0x04000773 RID: 1907
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mLineSeperator;

		// Token: 0x04000774 RID: 1908
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mNoActiveWindowsGrid;

		// Token: 0x04000775 RID: 1909
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mActiveWindowsListScrollbar;

		// Token: 0x04000776 RID: 1910
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mSelectAllCheckbox;

		// Token: 0x04000777 RID: 1911
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mActiveWindowsPanel;

		// Token: 0x04000778 RID: 1912
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBottomGrid;

		// Token: 0x04000779 RID: 1913
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mLineSeperator1;

		// Token: 0x0400077A RID: 1914
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mStartSyncBtn;

		// Token: 0x0400077B RID: 1915
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mLaunchInstanceManagerBtn;

		// Token: 0x0400077C RID: 1916
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSyncHelp;

		// Token: 0x0400077D RID: 1917
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mHyperLink;

		// Token: 0x0400077E RID: 1918
		private bool _contentLoaded;
	}
}
