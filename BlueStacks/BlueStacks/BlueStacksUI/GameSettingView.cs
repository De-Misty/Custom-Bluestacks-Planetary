using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000047 RID: 71
	public class GameSettingView : UserControl, IComponentConnector
	{
		// Token: 0x060003FF RID: 1023 RVA: 0x00004963 File Offset: 0x00002B63
		public GameSettingView()
		{
			this.InitializeComponent();
			this.mKnowMoreLink.Inlines.Clear();
			this.mKnowMoreLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_KNOW_MORE", ""));
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x000049A0 File Offset: 0x00002BA0
		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			BluestacksUIColor.ScrollBarScrollChanged(sender, e);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0001ACC8 File Offset: 0x00018EC8
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			try
			{
				Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
				Utils.OpenUrl(e.Uri.AbsoluteUri);
				e.Handled = true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in opening url" + ex.ToString());
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0001AD30 File Offset: 0x00018F30
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/settingswindows/gamesettingview.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0001AD60 File Offset: 0x00018F60
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
				((ScrollViewer)target).ScrollChanged += this.ScrollViewer_ScrollChanged;
				return;
			case 2:
				this.mGuideBtn = (CustomButton)target;
				return;
			case 3:
				this.mKnowMoreLink = (Hyperlink)target;
				this.mKnowMoreLink.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000219 RID: 537
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomButton mGuideBtn;

		// Token: 0x0400021A RID: 538
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mKnowMoreLink;

		// Token: 0x0400021B RID: 539
		private bool _contentLoaded;
	}
}
