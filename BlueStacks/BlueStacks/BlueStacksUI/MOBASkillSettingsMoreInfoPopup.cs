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
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000075 RID: 117
	public class MOBASkillSettingsMoreInfoPopup : CustomPopUp, IComponentConnector
	{
		// Token: 0x060005BB RID: 1467 RVA: 0x00005D4C File Offset: 0x00003F4C
		public MOBASkillSettingsMoreInfoPopup(CanvasElement canvasElement)
		{
			this.mCanvasElement = canvasElement;
			this.InitializeComponent();
			CanvasElement canvasElement2 = this.mCanvasElement;
			base.PlacementTarget = ((canvasElement2 != null) ? canvasElement2.MOBASkillSettingsPopup.mHelpIcon : null);
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x0002141C File Offset: 0x0001F61C
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			try
			{
				Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
				this.mCanvasElement.SendMOBAStats("read_more_clicked", "");
				BlueStacksUIUtils.OpenUrl(e.Uri.AbsoluteUri);
				e.Handled = true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in opening url" + ex.ToString());
			}
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x0002149C File Offset: 0x0001F69C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/mobaskillsettingsmoreinfopopup.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x000214CC File Offset: 0x0001F6CC
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
				this.mMOBASkillSettingsMoreInfoPopup = (MOBASkillSettingsMoreInfoPopup)target;
				return;
			case 2:
				this.mMaskBorder4 = (Border)target;
				return;
			case 3:
				this.mHyperLink = (Hyperlink)target;
				this.mHyperLink.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			case 4:
				this.LeftArrow = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000301 RID: 769
		private CanvasElement mCanvasElement;

		// Token: 0x04000302 RID: 770
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal MOBASkillSettingsMoreInfoPopup mMOBASkillSettingsMoreInfoPopup;

		// Token: 0x04000303 RID: 771
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder4;

		// Token: 0x04000304 RID: 772
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mHyperLink;

		// Token: 0x04000305 RID: 773
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path LeftArrow;

		// Token: 0x04000306 RID: 774
		private bool _contentLoaded;
	}
}
