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
	// Token: 0x02000262 RID: 610
	public class ProgressBar : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06001618 RID: 5656 RVA: 0x0000ED5B File Offset: 0x0000CF5B
		// (set) Token: 0x06001619 RID: 5657 RVA: 0x0000ED68 File Offset: 0x0000CF68
		public string ProgressText
		{
			get
			{
				return this.mLabel.Text;
			}
			set
			{
				BlueStacksUIBinding.Bind(this.mLabel, value, "");
				if (string.IsNullOrEmpty(this.mLabel.Text))
				{
					this.mLabel.Visibility = Visibility.Collapsed;
				}
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x0600161A RID: 5658 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x0600161B RID: 5659 RVA: 0x00004786 File Offset: 0x00002986
		public bool IsCloseOnOverLayClick
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x0600161C RID: 5660 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x0600161D RID: 5661 RVA: 0x00004786 File Offset: 0x00002986
		public bool ShowControlInSeparateWindow
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600161E RID: 5662 RVA: 0x0000ED99 File Offset: 0x0000CF99
		// (set) Token: 0x0600161F RID: 5663 RVA: 0x0000EDA1 File Offset: 0x0000CFA1
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x06001620 RID: 5664 RVA: 0x0000EDAA File Offset: 0x0000CFAA
		public ProgressBar()
		{
			this.InitializeComponent();
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x00007861 File Offset: 0x00005A61
		public bool Close()
		{
			base.Visibility = Visibility.Hidden;
			return true;
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x000047D5 File Offset: 0x000029D5
		public bool Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x00085198 File Offset: 0x00083398
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/progressbar.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x000851C8 File Offset: 0x000833C8
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
				this.mProgressBar = (ProgressBar)target;
				return;
			case 2:
				this.mLoadingImage = (CustomPictureBox)target;
				return;
			case 3:
				this.mLabel = (TextBlock)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000D89 RID: 3465
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal ProgressBar mProgressBar;

		// Token: 0x04000D8A RID: 3466
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mLoadingImage;

		// Token: 0x04000D8B RID: 3467
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mLabel;

		// Token: 0x04000D8C RID: 3468
		private bool _contentLoaded;
	}
}
