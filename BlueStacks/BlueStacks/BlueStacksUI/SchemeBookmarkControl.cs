using System;
using System.CodeDom.Compiler;
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
	// Token: 0x020000C0 RID: 192
	public class SchemeBookmarkControl : UserControl, IComponentConnector
	{
		// Token: 0x060007B6 RID: 1974 RVA: 0x0002B8A4 File Offset: 0x00029AA4
		public SchemeBookmarkControl(IMControlScheme scheme, MainWindow window)
		{
			this.InitializeComponent();
			this.ParentWindow = window;
			this.mSchemeName.Text = ((scheme != null) ? scheme.Name : null);
			if (scheme.Selected)
			{
				this.mCheckbox.ImageName = "radio_selected";
				BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
			}
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x00006F4D File Offset: 0x0000514D
		private void UserControl_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x0002B904 File Offset: 0x00029B04
		private void UserControl_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.ParentWindow.SelectedConfig.SelectedControlScheme != null && this.mSchemeName.Text != this.ParentWindow.SelectedConfig.SelectedControlScheme.Name)
			{
				BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ComboBoxBackgroundColor");
				return;
			}
			BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "ContextMenuItemBackgroundSelectedColor");
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0002B96C File Offset: 0x00029B6C
		private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.mCheckbox.ImageName == "radio_unselected")
			{
				foreach (object obj in this.ParentWindow.mSidebar.mBookmarkedSchemesStackPanel.Children)
				{
					SchemeBookmarkControl schemeBookmarkControl = (SchemeBookmarkControl)obj;
					schemeBookmarkControl.mCheckbox.ImageName = "radio_unselected";
					BlueStacksUIBinding.BindColor(schemeBookmarkControl, Control.BackgroundProperty, "ComboBoxBackgroundColor");
				}
				this.mCheckbox.ImageName = "radio_selected";
				BlueStacksUIBinding.BindColor(this, Control.BackgroundProperty, "SelectedTabBackgroundColor");
				KMManager.SelectSchemeIfPresent(this.ParentWindow, this.mSchemeName.Text, "bookmark", true);
			}
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0002BA44 File Offset: 0x00029C44
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/schemebookmarkcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0002BA74 File Offset: 0x00029C74
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
				((SchemeBookmarkControl)target).MouseEnter += this.UserControl_MouseEnter;
				((SchemeBookmarkControl)target).MouseLeave += this.UserControl_MouseLeave;
				((SchemeBookmarkControl)target).MouseDown += this.UserControl_PreviewMouseDown;
				return;
			case 2:
				this.mCheckbox = (CustomPictureBox)target;
				return;
			case 3:
				this.mSchemeName = (CustomTextBox)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000427 RID: 1063
		private MainWindow ParentWindow;

		// Token: 0x04000428 RID: 1064
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mCheckbox;

		// Token: 0x04000429 RID: 1065
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomTextBox mSchemeName;

		// Token: 0x0400042A RID: 1066
		private bool _contentLoaded;
	}
}
