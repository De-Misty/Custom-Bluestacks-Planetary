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
	// Token: 0x02000074 RID: 116
	public class MOBAOtherSettingsMoreInfoPopup : CustomPopUp, IComponentConnector
	{
		// Token: 0x060005B6 RID: 1462 RVA: 0x00005D1A File Offset: 0x00003F1A
		public MOBAOtherSettingsMoreInfoPopup(CanvasElement canvasElement)
		{
			this.mCanvasElement = canvasElement;
			this.InitializeComponent();
			CanvasElement canvasElement2 = this.mCanvasElement;
			base.PlacementTarget = ((canvasElement2 != null) ? canvasElement2.MOBASkillSettingsPopup.mOtherSettingsHelpIcon : null);
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x00021304 File Offset: 0x0001F504
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

		// Token: 0x060005B8 RID: 1464 RVA: 0x00021384 File Offset: 0x0001F584
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/mobaothersettingsmoreinfopopup.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x000213B4 File Offset: 0x0001F5B4
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
				this.mMaskBorder5 = (Border)target;
				return;
			case 2:
				this.mSettingsHyperLink = (Hyperlink)target;
				this.mSettingsHyperLink.RequestNavigate += this.Hyperlink_RequestNavigate;
				return;
			case 3:
				this.LeftArrowPath = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040002FC RID: 764
		private CanvasElement mCanvasElement;

		// Token: 0x040002FD RID: 765
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder5;

		// Token: 0x040002FE RID: 766
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Hyperlink mSettingsHyperLink;

		// Token: 0x040002FF RID: 767
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path LeftArrowPath;

		// Token: 0x04000300 RID: 768
		private bool _contentLoaded;
	}
}
