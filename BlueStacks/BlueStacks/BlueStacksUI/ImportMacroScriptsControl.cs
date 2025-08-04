using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020000B7 RID: 183
	public class ImportMacroScriptsControl : UserControl, IComponentConnector
	{
		// Token: 0x06000769 RID: 1897 RVA: 0x00006D04 File Offset: 0x00004F04
		public ImportMacroScriptsControl(ImportMacroWindow importMacroWindow, MainWindow window)
		{
			this.InitializeComponent();
			this.mImportMacroWindow = importMacroWindow;
			this.ParentWindow = window;
			ImportMacroScriptsControl.mIdCount++;
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00006D2C File Offset: 0x00004F2C
		private void Box_Checked(object sender, RoutedEventArgs e)
		{
			this.mImportMacroWindow.Box_Checked(sender, e);
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x00006D3B File Offset: 0x00004F3B
		private void Box_Unchecked(object sender, RoutedEventArgs e)
		{
			this.mImportMacroWindow.Box_Unchecked(sender, e);
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x00006D4A File Offset: 0x00004F4A
		private void ImportName_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.mImportMacroWindow.TextChanged(sender, e);
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x000291D8 File Offset: 0x000273D8
		internal void Init(string macroName, bool isSingleRecording)
		{
			this.mRenameBtn.ApplyTemplate();
			this.mReplaceExistingBtn.ApplyTemplate();
			this.mRenameBtn.RadioBtnImage.Width = 14.0;
			this.mRenameBtn.RadioBtnImage.Height = 14.0;
			this.mRenameBtn.GroupName = string.Format("MacroConflictAction_{0}{1}", macroName, ImportMacroScriptsControl.mIdCount);
			this.mReplaceExistingBtn.RadioBtnImage.Width = 14.0;
			this.mReplaceExistingBtn.RadioBtnImage.Height = 14.0;
			this.mReplaceExistingBtn.GroupName = string.Format("MacroConflictAction_{0}{1}", macroName, ImportMacroScriptsControl.mIdCount);
			this.mReplaceExistingBtn.IsChecked = new bool?(true);
			this.mReplaceExistingBtn.Checked += this.ConflictingMacroHandlingRadioBtn_Checked;
			this.mContent.Content = macroName;
			if (isSingleRecording)
			{
				this.mContent.Visibility = Visibility.Collapsed;
				this.mBlock.Margin = new Thickness(0.0);
				this.mMainGrid.Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
				this.mSingleMacroRecordTextblock.Visibility = Visibility.Visible;
				this.mSingleMacroRecordTextblock.Text = macroName;
				this.mWarningMsg.Margin = new Thickness(0.0, 1.0, 0.0, 1.0);
				return;
			}
			this.mMainGrid.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x000293B8 File Offset: 0x000275B8
		private void ConflictingMacroHandlingRadioBtn_Checked(object sender, RoutedEventArgs e)
		{
			CustomRadioButton customRadioButton = sender as CustomRadioButton;
			if (customRadioButton != null && this.mImportName != null)
			{
				if (customRadioButton == this.mRenameBtn)
				{
					this.mImportName.Visibility = Visibility.Visible;
					return;
				}
				this.mImportName.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x000293FC File Offset: 0x000275FC
		internal bool IsScriptInRenameMode()
		{
			return this.mRenameBtn.IsChecked != null && this.mRenameBtn.IsChecked.Value;
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x00029438 File Offset: 0x00027638
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/importmacroscriptscontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x00029468 File Offset: 0x00027668
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
				this.mMainGrid = (Grid)target;
				return;
			case 2:
				this.mContent = (CustomCheckbox)target;
				this.mContent.Checked += this.Box_Checked;
				this.mContent.Unchecked += this.Box_Unchecked;
				return;
			case 3:
				this.mSingleMacroRecordTextblock = (TextBlock)target;
				return;
			case 4:
				this.mBlock = (Grid)target;
				return;
			case 5:
				this.mMacroImportedAsTextBlock = (TextBlock)target;
				return;
			case 6:
				this.mConflictingMacroOptionsPanel = (StackPanel)target;
				return;
			case 7:
				this.mReplaceExistingBtn = (CustomRadioButton)target;
				return;
			case 8:
				this.mRenameBtn = (CustomRadioButton)target;
				this.mRenameBtn.Checked += this.ConflictingMacroHandlingRadioBtn_Checked;
				return;
			case 9:
				this.mImportName = (CustomTextBox)target;
				this.mImportName.TextChanged += this.ImportName_TextChanged;
				return;
			case 10:
				this.mWarningMsg = (TextBlock)target;
				return;
			case 11:
				this.mDependentScriptsMsg = (TextBlock)target;
				return;
			case 12:
				this.mDependentScriptsPanel = (StackPanel)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040003F9 RID: 1017
		internal ImportMacroWindow mImportMacroWindow;

		// Token: 0x040003FA RID: 1018
		internal MainWindow ParentWindow;

		// Token: 0x040003FB RID: 1019
		private static int mIdCount;

		// Token: 0x040003FC RID: 1020
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mMainGrid;

		// Token: 0x040003FD RID: 1021
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mContent;

		// Token: 0x040003FE RID: 1022
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mSingleMacroRecordTextblock;

		// Token: 0x040003FF RID: 1023
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mBlock;

		// Token: 0x04000400 RID: 1024
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mMacroImportedAsTextBlock;

		// Token: 0x04000401 RID: 1025
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mConflictingMacroOptionsPanel;

		// Token: 0x04000402 RID: 1026
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mReplaceExistingBtn;

		// Token: 0x04000403 RID: 1027
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mRenameBtn;

		// Token: 0x04000404 RID: 1028
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mImportName;

		// Token: 0x04000405 RID: 1029
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mWarningMsg;

		// Token: 0x04000406 RID: 1030
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mDependentScriptsMsg;

		// Token: 0x04000407 RID: 1031
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel mDependentScriptsPanel;

		// Token: 0x04000408 RID: 1032
		private bool _contentLoaded;
	}
}
