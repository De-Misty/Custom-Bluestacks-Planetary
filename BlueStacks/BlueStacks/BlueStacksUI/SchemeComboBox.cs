using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000167 RID: 359
	public class SchemeComboBox : UserControl, IComponentConnector
	{
		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000EC8 RID: 3784 RVA: 0x0000AF91 File Offset: 0x00009191
		// (set) Token: 0x06000EC9 RID: 3785 RVA: 0x0000AF99 File Offset: 0x00009199
		public string mSelectedItem { get; set; }

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000ECA RID: 3786 RVA: 0x0000AFA2 File Offset: 0x000091A2
		// (set) Token: 0x06000ECB RID: 3787 RVA: 0x0000AFAA File Offset: 0x000091AA
		public string SelectedItem
		{
			get
			{
				return this.mSelectedItem;
			}
			set
			{
				this.mSelectedItem = value;
			}
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0000AFB3 File Offset: 0x000091B3
		public SchemeComboBox()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0000AFC1 File Offset: 0x000091C1
		private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
			{
				return;
			}
			e.Handled = true;
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void ComboBoxItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x0000AFDD File Offset: 0x000091DD
		private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
		{
			((ComboBoxSchemeControl)sender).mBookmarkImg.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0000AFF0 File Offset: 0x000091F0
		private void NewProfile_MouseDown(object sender, MouseButtonEventArgs e)
		{
			KMManager.AddNewControlSchemeAndSelect(BlueStacksUIUtils.LastActivatedWindow, null, true);
			KMManager.CanvasWindow.ClearWindow();
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x0000B008 File Offset: 0x00009208
		private void NewProfile_MouseEnter(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this.NewProfile, Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x0000B01F File Offset: 0x0000921F
		private void NewProfile_MouseLeave(object sender, MouseEventArgs e)
		{
			BlueStacksUIBinding.BindColor(this.NewProfile, Panel.BackgroundProperty, "ComboBoxBackgroundColor");
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x0005DE48 File Offset: 0x0005C048
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/schemecombobox.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0005DE78 File Offset: 0x0005C078
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
				this._this = (SchemeComboBox)target;
				return;
			case 2:
				this.mGrid = (Grid)target;
				return;
			case 3:
				this.TogglePopupButton = (ToggleButton)target;
				return;
			case 4:
				this.mName = (TextBlock)target;
				return;
			case 5:
				this.Arrow = (Path)target;
				return;
			case 6:
				this.mItems = (CustomPopUp)target;
				return;
			case 7:
				this.mSchemesListScrollbar = (ScrollViewer)target;
				return;
			case 8:
				this.Items = (StackPanel)target;
				return;
			case 9:
				this.NewProfile = (Grid)target;
				this.NewProfile.MouseDown += this.NewProfile_MouseDown;
				this.NewProfile.MouseEnter += this.NewProfile_MouseEnter;
				this.NewProfile.MouseLeave += this.NewProfile_MouseLeave;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000977 RID: 2423
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(ComboBoxSchemeControl), new UIPropertyMetadata(string.Empty));

		// Token: 0x04000978 RID: 2424
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SchemeComboBox _this;

		// Token: 0x04000979 RID: 2425
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mGrid;

		// Token: 0x0400097A RID: 2426
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ToggleButton TogglePopupButton;

		// Token: 0x0400097B RID: 2427
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mName;

		// Token: 0x0400097C RID: 2428
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path Arrow;

		// Token: 0x0400097D RID: 2429
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPopUp mItems;

		// Token: 0x0400097E RID: 2430
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ScrollViewer mSchemesListScrollbar;

		// Token: 0x0400097F RID: 2431
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal StackPanel Items;

		// Token: 0x04000980 RID: 2432
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid NewProfile;

		// Token: 0x04000981 RID: 2433
		private bool _contentLoaded;
	}
}
