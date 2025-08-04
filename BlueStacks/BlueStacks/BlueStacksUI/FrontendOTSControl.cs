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
	// Token: 0x02000265 RID: 613
	public class FrontendOTSControl : UserControl, IComponentConnector
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06001638 RID: 5688 RVA: 0x0000EEEA File Offset: 0x0000D0EA
		public MainWindow ParentWindow
		{
			get
			{
				if (this.mMainWindow == null)
				{
					this.mMainWindow = Window.GetWindow(this) as MainWindow;
				}
				return this.mMainWindow;
			}
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x0000EF0B File Offset: 0x0000D10B
		public FrontendOTSControl()
		{
			this.InitializeComponent();
			this.OneTimeSetupCompletedEventHandle = (EventHandler<EventArgs>)Delegate.Combine(this.OneTimeSetupCompletedEventHandle, new EventHandler<EventArgs>(this.OneTimeSetup_Completed));
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x00085638 File Offset: 0x00083838
		private void UserControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
		{
			if (base.Visibility == Visibility.Visible)
			{
				BlueStacksUIBinding.Bind(this.mBaseControl.mTitleLabel, "STRING_GOOGLE_LOGIN_MESSAGE");
				this.mBaseControl.Init(this, this.ParentWindow.mFrontendGrid, true, true);
				this.mBaseControl.ShowContent();
				this.ParentWindow.mAppHandler.EventOnOneTimeSetupCompleted = this.OneTimeSetupCompletedEventHandle;
			}
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x0000EF3B File Offset: 0x0000D13B
		private void OneTimeSetup_Completed(object sender, EventArgs e)
		{
			base.Dispatcher.Invoke(new Action(delegate
			{
				this.mBaseControl.HideWindow();
			}), new object[0]);
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x0008569C File Offset: 0x0008389C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/frontendotscontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x0000EF5B File Offset: 0x0000D15B
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
				((FrontendOTSControl)target).IsVisibleChanged += this.UserControl_IsVisibleChanged;
				return;
			}
			if (connectionId != 2)
			{
				this._contentLoaded = true;
				return;
			}
			this.mBaseControl = (DimControlWithProgresBar)target;
		}

		// Token: 0x04000D97 RID: 3479
		private MainWindow mMainWindow;

		// Token: 0x04000D98 RID: 3480
		private EventHandler<EventArgs> OneTimeSetupCompletedEventHandle;

		// Token: 0x04000D99 RID: 3481
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal DimControlWithProgresBar mBaseControl;

		// Token: 0x04000D9A RID: 3482
		private bool _contentLoaded;
	}
}
