using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200012D RID: 301
	public class SpeedUpBluestacksUserControl : UserControl, IComponentConnector
	{
		// Token: 0x06000C18 RID: 3096 RVA: 0x00009A70 File Offset: 0x00007C70
		public SpeedUpBluestacksUserControl()
		{
			this.InitializeComponent();
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.mHyperLink.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF328CF2");
			}
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x000434B8 File Offset: 0x000416B8
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

		// Token: 0x06000C1A RID: 3098 RVA: 0x00043520 File Offset: 0x00041720
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/speedupbluestacksusercontrol.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x00043550 File Offset: 0x00041750
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
				this.mTitleText = (TextBlock)target;
				return;
			case 2:
				this.mBodyText = (TextBlock)target;
				return;
			case 3:
				this.mImage = (CustomPictureBox)target;
				return;
			case 4:
				this.mHyperLink = (Hyperlink)target;
				this.mHyperLink.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000762 RID: 1890
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mTitleText;

		// Token: 0x04000763 RID: 1891
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal TextBlock mBodyText;

		// Token: 0x04000764 RID: 1892
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mImage;

		// Token: 0x04000765 RID: 1893
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mHyperLink;

		// Token: 0x04000766 RID: 1894
		private bool _contentLoaded;
	}
}
