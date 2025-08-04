using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x020001B5 RID: 437
	public class CustomToggleButton : UserControl, IComponentConnector
	{
		// Token: 0x170002F3 RID: 755
		// (set) Token: 0x06001149 RID: 4425 RVA: 0x0000C50D File Offset: 0x0000A70D
		public bool HideIcon
		{
			set
			{
				if (value)
				{
					this.mIconColDef.Width = new GridLength(0.0);
				}
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600114A RID: 4426 RVA: 0x0000C52B File Offset: 0x0000A72B
		// (set) Token: 0x0600114B RID: 4427 RVA: 0x0000C538 File Offset: 0x0000A738
		public string AppLabel
		{
			get
			{
				return this.mAppLabel.Text;
			}
			set
			{
				BlueStacksUIBinding.Bind(this.mAppLabel, value, "");
			}
		}

		// Token: 0x170002F5 RID: 757
		// (set) Token: 0x0600114C RID: 4428 RVA: 0x0000C54B File Offset: 0x0000A74B
		public Orientation CheckBoxOrientation
		{
			set
			{
				this.mMuteCheckBox.Orientation = value;
				this.mAutoHideCheckBox.Orientation = value;
				Grid.SetRow(this.mAppLabel, 1);
				Grid.SetRowSpan(this.mAppLabel, 1);
			}
		}

		// Token: 0x170002F6 RID: 758
		// (set) Token: 0x0600114D RID: 4429 RVA: 0x0000C57D File Offset: 0x0000A77D
		public bool IsThreeStateCheckBox
		{
			set
			{
				this.mMuteCheckBox.IsThreeState = value;
				this.mAutoHideCheckBox.IsThreeState = value;
			}
		}

		// Token: 0x170002F7 RID: 759
		// (set) Token: 0x0600114E RID: 4430 RVA: 0x0000C597 File Offset: 0x0000A797
		public Visibility CheckBoxLabelVisibility
		{
			set
			{
				this.mMuteCheckBox.LabelVisibility = value;
				this.mAutoHideCheckBox.LabelVisibility = value;
			}
		}

		// Token: 0x170002F8 RID: 760
		// (set) Token: 0x0600114F RID: 4431 RVA: 0x0000C5B1 File Offset: 0x0000A7B1
		public BitmapImage Image
		{
			set
			{
				this.mAppImage.Source = value;
				if (value != null)
				{
					this.mAppImage.ImageName = string.Empty;
				}
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06001150 RID: 4432 RVA: 0x0000C5D2 File Offset: 0x0000A7D2
		public bool? IsMuted
		{
			get
			{
				return this.mMuteCheckBox.IsChecked;
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06001151 RID: 4433 RVA: 0x0000C5DF File Offset: 0x0000A7DF
		public bool? IsAutoHide
		{
			get
			{
				return this.mAutoHideCheckBox.IsChecked;
			}
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x0000C5EC File Offset: 0x0000A7EC
		public void HideAppLable()
		{
			this.mAppLableColDef.Width = new GridLength(0.0);
			this.mIconColDef.Width = new GridLength(0.0);
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x0000C620 File Offset: 0x0000A820
		public void HideShowButton()
		{
			this.mShowColDef.Width = new GridLength(0.0);
			this.mIconColDef.Width = new GridLength(0.0);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x0000C654 File Offset: 0x0000A854
		public CustomToggleButton()
		{
			this.InitializeComponent();
			this.SetProperties();
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x0000C668 File Offset: 0x0000A868
		private void SetProperties()
		{
			BlueStacksUIBinding.Bind(this.mMuteCheckBox, "STRING_SHOW");
			BlueStacksUIBinding.Bind(this.mAutoHideCheckBox, "STRING_AUTO_HIDE");
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x0006C0FC File Offset: 0x0006A2FC
		private void MuteButton_Checked(object sender, RoutedEventArgs e)
		{
			if (this.mAutoHideCheckBox.IsChecked != null && !this.mAutoHideCheckBox.IsChecked.Value && base.IsLoaded)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, this.AppLabel);
			}
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x0000C68A File Offset: 0x0000A88A
		private void MuteButton_Unchecked(object sender, RoutedEventArgs e)
		{
			this.mAutoHideCheckBox.IsChecked = new bool?(false);
			if (base.IsLoaded)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.AppLabel);
			}
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x0000C6B6 File Offset: 0x0000A8B6
		private void MuteCheckBox_Indeterminate(object sender, RoutedEventArgs e)
		{
			if (base.IsLoaded)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.AppLabel);
			}
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0000C6D1 File Offset: 0x0000A8D1
		private void AutoHideButton_Checked(object sender, RoutedEventArgs e)
		{
			this.mMuteCheckBox.IsChecked = new bool?(true);
			if (base.IsLoaded)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.AutoHide, this.AppLabel);
			}
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x0000C6FD File Offset: 0x0000A8FD
		private void AutoHideButton_Unchecked(object sender, RoutedEventArgs e)
		{
			if (base.IsLoaded)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, this.AppLabel);
			}
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0000C6B6 File Offset: 0x0000A8B6
		private void AutoHideCheckBox_Indeterminate(object sender, RoutedEventArgs e)
		{
			if (base.IsLoaded)
			{
				NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.AppLabel);
			}
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0006C14C File Offset: 0x0006A34C
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (NotificationManager.Instance.IsNotificationMutedForKey(this.AppLabel, "Android") == MuteState.AutoHide)
			{
				this.mAutoHideCheckBox.IsChecked = new bool?(true);
				return;
			}
			if (NotificationManager.Instance.IsNotificationMutedForKey(this.AppLabel, "Android") == MuteState.NotMuted)
			{
				this.mMuteCheckBox.IsChecked = new bool?(true);
				return;
			}
			this.mMuteCheckBox.IsChecked = new bool?(false);
			this.mAutoHideCheckBox.IsChecked = new bool?(false);
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x0006C1D0 File Offset: 0x0006A3D0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/customtogglebutton.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0006C200 File Offset: 0x0006A400
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
				((CustomToggleButton)target).Loaded += this.UserControl_Loaded;
				return;
			case 2:
				this.mIconColDef = (ColumnDefinition)target;
				return;
			case 3:
				this.mAppLableColDef = (ColumnDefinition)target;
				return;
			case 4:
				this.mShowColDef = (ColumnDefinition)target;
				return;
			case 5:
				this.mAppImage = (CustomPictureBox)target;
				return;
			case 6:
				this.mAppLabel = (TextBlock)target;
				return;
			case 7:
				this.mMuteCheckBox = (CustomCheckbox)target;
				this.mMuteCheckBox.Checked += this.MuteButton_Checked;
				this.mMuteCheckBox.Unchecked += this.MuteButton_Unchecked;
				this.mMuteCheckBox.Indeterminate += this.MuteCheckBox_Indeterminate;
				return;
			case 8:
				this.mAutoHideCheckBox = (CustomCheckbox)target;
				this.mAutoHideCheckBox.Checked += this.AutoHideButton_Checked;
				this.mAutoHideCheckBox.Unchecked += this.AutoHideButton_Unchecked;
				this.mAutoHideCheckBox.Indeterminate += this.AutoHideCheckBox_Indeterminate;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000B2E RID: 2862
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mIconColDef;

		// Token: 0x04000B2F RID: 2863
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mAppLableColDef;

		// Token: 0x04000B30 RID: 2864
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ColumnDefinition mShowColDef;

		// Token: 0x04000B31 RID: 2865
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mAppImage;

		// Token: 0x04000B32 RID: 2866
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mAppLabel;

		// Token: 0x04000B33 RID: 2867
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mMuteCheckBox;

		// Token: 0x04000B34 RID: 2868
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mAutoHideCheckBox;

		// Token: 0x04000B35 RID: 2869
		private bool _contentLoaded;
	}
}
