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
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001B3 RID: 435
	public class BackupRestoreSettingsControl : UserControl, IComponentConnector
	{
		// Token: 0x0600113A RID: 4410 RVA: 0x0006BA98 File Offset: 0x00069C98
		public BackupRestoreSettingsControl(MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			if (this.ParentWindow != null && !this.ParentWindow.IsDefaultVM)
			{
				this.mBackupRestoreGrid.Visibility = Visibility.Collapsed;
				this.mLineSeperator.Visibility = Visibility.Collapsed;
			}
			base.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x0006BAEC File Offset: 0x00069CEC
		private void RestoreBtn_Click(object sender, RoutedEventArgs e)
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.ImageName = "backup_restore_popup_window";
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RESTORE_BACKUP", "");
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_MAKE_SURE_LATEST_WARNING", "");
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTORE_BUTTON", delegate(object sender1, EventArgs e1)
			{
				this.LaunchDataManager("restore");
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
			customMessageWindow.Owner = this.ParentWindow;
			customMessageWindow.ShowDialog();
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x0006BB70 File Offset: 0x00069D70
		private void BackupBtn_Click(object sender, RoutedEventArgs e)
		{
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.ImageName = "backup_restore_popup_window";
			BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_BACKUP_WARNING", "");
			BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_BLUESTACKS_BACKUP_PROMPT", "");
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_BACKUP", delegate(object sender1, EventArgs e1)
			{
				this.LaunchDataManager("backup");
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", null, null, false, null);
			customMessageWindow.Owner = this.ParentWindow;
			customMessageWindow.ShowDialog();
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x0006BBF4 File Offset: 0x00069DF4
		private void DiskCleanupBtn_Click(object sender, RoutedEventArgs e)
		{
			EventHandler <>9__1;
			this.ParentWindow.Dispatcher.Invoke(new Action(delegate
			{
				if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_DiskCompactor_Lockbgp64"))
				{
					CustomMessageWindow customMessageWindow = new CustomMessageWindow();
					customMessageWindow.ImageName = "disk_cleanup_popup_window";
					customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_HEADING", "");
					customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_MESSAGE", "");
					customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", null, null, false, null);
					customMessageWindow.CloseButtonHandle(null, null);
					customMessageWindow.Owner = this.ParentWindow;
					customMessageWindow.ShowDialog();
					return;
				}
				CustomMessageWindow customMessageWindow2 = new CustomMessageWindow();
				customMessageWindow2.ImageName = "disk_cleanup_popup_window";
				customMessageWindow2.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP", "");
				customMessageWindow2.BodyTextBlockTitle.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MESSAGE", "");
				customMessageWindow2.BodyTextBlockTitle.Visibility = Visibility.Visible;
				customMessageWindow2.BodyTextBlockTitle.FontWeight = FontWeights.Regular;
				customMessageWindow2.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CONTINUE_CONFIRMATION", "");
				customMessageWindow2.AddButton(ButtonColors.White, "STRING_CLOSE", null, null, false, null);
				ButtonColors buttonColors = ButtonColors.Blue;
				string text = "STRING_CONTINUE";
				EventHandler eventHandler;
				if ((eventHandler = <>9__1) == null)
				{
					eventHandler = (<>9__1 = delegate(object sender1, EventArgs e1)
					{
						this.LaunchDiskCompaction(sender, null);
					});
				}
				customMessageWindow2.AddButton(buttonColors, text, eventHandler, null, false, null);
				customMessageWindow2.CloseButtonHandle(null, null);
				customMessageWindow2.Owner = this.ParentWindow;
				customMessageWindow2.ShowDialog();
			}), new object[0]);
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0006BC38 File Offset: 0x00069E38
		private void LaunchDataManager(string argument)
		{
			foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
			{
				MainWindow.sIsClosingForBackupRestore = true;
				if (argument == "backup")
				{
					mainWindow.CloseAllWindowAndPerform(new EventHandler(this.Closing_WindowHandlerForBackup));
				}
				else if (argument == "restore")
				{
					mainWindow.CloseAllWindowAndPerform(new EventHandler(this.Closing_WindowHandlerForRestore));
				}
			}
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x0006BCD4 File Offset: 0x00069ED4
		private void LaunchDiskCompaction(object sender, MouseButtonEventArgs e)
		{
			try
			{
				this.ParentWindow.mFrontendHandler.IsRestartFrontendWhenClosed = false;
				BlueStacksUIUtils.HideUnhideBlueStacks(true);
				using (Process process = new Process())
				{
					process.StartInfo.FileName = global::System.IO.Path.Combine(RegistryStrings.InstallDir, "DiskCompactionTool.exe");
					process.StartInfo.Arguments = string.Format(CultureInfo.InvariantCulture, "-vmname:{0} -relaunch", new object[] { this.ParentWindow.mVmName });
					process.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in starting disk compaction" + ex.ToString());
			}
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x0006BD90 File Offset: 0x00069F90
		internal void Closing_WindowHandlerForBackup(object sender, EventArgs e)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "relaunch", "true" },
					{ "sendResponseImmediately", "true" }
				};
				HTTPUtils.SendRequestToAgent("backup", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64", true);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in closing window handler for backup" + ex.ToString());
			}
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x0006BE10 File Offset: 0x0006A010
		internal void Closing_WindowHandlerForRestore(object sender, EventArgs e)
		{
			try
			{
				Utils.KillCurrentOemProcessByName("HD-MultiInstanceManager", null);
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "relaunch", "true" },
					{ "sendResponseImmediately", "true" }
				};
				HTTPUtils.SendRequestToAgent("restore", dictionary, this.ParentWindow.mVmName, 0, null, false, 1, 0, "bgp64", true);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in closing window handler for restore" + ex.ToString());
			}
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x0006BE9C File Offset: 0x0006A09C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/backuprestoresettingscontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x0006BECC File Offset: 0x0006A0CC
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
				this.mDiskCleanupGrid = (Grid)target;
				return;
			case 2:
				this.mDiskCleanupBtn = (CustomButton)target;
				this.mDiskCleanupBtn.Click += this.DiskCleanupBtn_Click;
				return;
			case 3:
				this.mLineSeperator = (Line)target;
				return;
			case 4:
				this.mBackupRestoreGrid = (Grid)target;
				return;
			case 5:
				this.mRestoreBtn = (CustomButton)target;
				this.mRestoreBtn.Click += this.RestoreBtn_Click;
				return;
			case 6:
				this.mBackupBtn = (CustomButton)target;
				this.mBackupBtn.Click += this.BackupBtn_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000B23 RID: 2851
		private MainWindow ParentWindow;

		// Token: 0x04000B24 RID: 2852
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mDiskCleanupGrid;

		// Token: 0x04000B25 RID: 2853
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mDiskCleanupBtn;

		// Token: 0x04000B26 RID: 2854
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Line mLineSeperator;

		// Token: 0x04000B27 RID: 2855
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBackupRestoreGrid;

		// Token: 0x04000B28 RID: 2856
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mRestoreBtn;

		// Token: 0x04000B29 RID: 2857
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mBackupBtn;

		// Token: 0x04000B2A RID: 2858
		private bool _contentLoaded;
	}
}
