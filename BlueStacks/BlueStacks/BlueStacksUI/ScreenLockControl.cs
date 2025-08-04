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
	// Token: 0x020000D2 RID: 210
	public class ScreenLockControl : UserControl, IDimOverlayControl, IComponentConnector
	{
		// Token: 0x06000895 RID: 2197 RVA: 0x00007831 File Offset: 0x00005A31
		public ScreenLockControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000896 RID: 2198 RVA: 0x00004783 File Offset: 0x00002983
		// (set) Token: 0x06000897 RID: 2199 RVA: 0x00004786 File Offset: 0x00002986
		bool IDimOverlayControl.IsCloseOnOverLayClick
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x0000783F File Offset: 0x00005A3F
		// (set) Token: 0x06000899 RID: 2201 RVA: 0x00007847 File Offset: 0x00005A47
		public bool ShowControlInSeparateWindow { get; set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x0600089A RID: 2202 RVA: 0x00007850 File Offset: 0x00005A50
		// (set) Token: 0x0600089B RID: 2203 RVA: 0x00007858 File Offset: 0x00005A58
		public bool ShowTransparentWindow { get; set; }

		// Token: 0x0600089C RID: 2204 RVA: 0x00007861 File Offset: 0x00005A61
		public bool Close()
		{
			base.Visibility = Visibility.Hidden;
			return true;
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x000047D5 File Offset: 0x000029D5
		public bool Show()
		{
			base.Visibility = Visibility.Visible;
			return true;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00030980 File Offset: 0x0002EB80
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/screenlockcontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0000786B File Offset: 0x00005A6B
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				this.mScreenLockImage = (CustomPictureBox)target;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			this.mScreenLockText = (TextBlock)target;
		}

		// Token: 0x040004DD RID: 1245
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mScreenLockImage;

		// Token: 0x040004DE RID: 1246
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mScreenLockText;

		// Token: 0x040004DF RID: 1247
		private bool _contentLoaded;
	}
}
