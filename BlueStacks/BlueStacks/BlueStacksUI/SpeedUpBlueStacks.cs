using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x0200012E RID: 302
	public class SpeedUpBlueStacks : UserControl, IComponentConnector
	{
		// Token: 0x06000C1C RID: 3100 RVA: 0x00009AA9 File Offset: 0x00007CA9
		public SpeedUpBlueStacks()
		{
			this.InitializeComponent();
			this.SetUrl();
			this.SetContent();
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x000435CC File Offset: 0x000417CC
		private void SetUrl()
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.mEnableVt.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
				this.mUpgradeComputer.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
				this.mConfigureAntivirus.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
				this.mDiasbleHyperV.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
				this.mPowerPlan.mHyperLink.NavigateUri = new Uri("http://help.dmm.com/-/detail/=/qid=45997/");
				return;
			}
			string text = WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=";
			this.mEnableVt.mHyperLink.NavigateUri = new Uri(text + "enable_virtualization");
			this.mUpgradeComputer.mHyperLink.NavigateUri = new Uri(text + "bs3_nougat_min_requirements");
			this.mConfigureAntivirus.mHyperLink.NavigateUri = new Uri(text + "disable_antivirus");
			this.mDiasbleHyperV.mHyperLink.NavigateUri = new Uri(text + "disable_hypervisor");
			this.mPowerPlan.mHyperLink.NavigateUri = new Uri(text + "change_powerplan");
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x00043740 File Offset: 0x00041940
		private void SetContent()
		{
			BlueStacksUIBinding.Bind(this.mEnableVt.mTitleText, "STRING_ENABLE_VIRT", "");
			BlueStacksUIBinding.Bind(this.mEnableVt.mBodyText, "STRING_ENABLE_VIRT_BODY", "");
			this.mEnableVt.mHyperLink.Inlines.Clear();
			this.mEnableVt.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_ENABLE_VIRT_HYPERLINK", ""));
			this.mEnableVt.mImage.ImageName = "virtualization";
			BlueStacksUIBinding.Bind(this.mDiasbleHyperV.mTitleText, "STRING_DISABLE_HYPERV", "");
			BlueStacksUIBinding.Bind(this.mDiasbleHyperV.mBodyText, "STRING_DISABLE_HYPERV_BODY", "");
			this.mDiasbleHyperV.mHyperLink.Inlines.Clear();
			this.mDiasbleHyperV.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_DISABLE_HYPERV_HYPERLINK", ""));
			this.mDiasbleHyperV.mImage.ImageName = "hypervisor";
			BlueStacksUIBinding.Bind(this.mConfigureAntivirus.mTitleText, "STRING_CONFIGURE_ANTIVIRUS", "");
			BlueStacksUIBinding.Bind(this.mConfigureAntivirus.mBodyText, "STRING_CONFIGURE_ANTIVIRUS_BODY", "");
			this.mConfigureAntivirus.mHyperLink.Inlines.Clear();
			this.mConfigureAntivirus.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_CONFIGURE_ANTIVIRUS_HYPERLINK", ""));
			this.mConfigureAntivirus.mImage.ImageName = "antivirus";
			BlueStacksUIBinding.Bind(this.mPowerPlan.mTitleText, "STRING_POWER_PLAN", "");
			BlueStacksUIBinding.Bind(this.mPowerPlan.mBodyText, "STRING_POWER_PLAN_BODY", "");
			this.mPowerPlan.mHyperLink.Inlines.Clear();
			this.mPowerPlan.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_POWER_PLAN_HYPERLINK", ""));
			this.mPowerPlan.mImage.ImageName = "powerplan";
			BlueStacksUIBinding.Bind(this.mUpgradeComputer.mTitleText, "STRING_UPGRADE_SYSTEM", "");
			BlueStacksUIBinding.Bind(this.mUpgradeComputer.mBodyText, "STRING_UPGRADE_SYSTEM_BODY", "");
			this.mUpgradeComputer.mHyperLink.Inlines.Clear();
			this.mUpgradeComputer.mHyperLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_UPGRADE_SYSTEM_HYPERLINK", ""));
			this.mUpgradeComputer.mImage.ImageName = "upgrade";
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x00009AC3 File Offset: 0x00007CC3
		private void CloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Logger.Info("Clicked close button speedUpBluestacks");
			BlueStacksUIUtils.CloseContainerWindow(this);
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x000439D8 File Offset: 0x00041BD8
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/controls/speedupbluestacks.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00043A08 File Offset: 0x00041C08
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
				this.CloseBtn = (CustomPictureBox)target;
				this.CloseBtn.PreviewMouseLeftButtonUp += this.CloseBtn_PreviewMouseLeftButtonUp;
				return;
			case 2:
				this.mEnableVt = (SpeedUpBluestacksUserControl)target;
				return;
			case 3:
				this.mConfigureAntivirus = (SpeedUpBluestacksUserControl)target;
				return;
			case 4:
				this.mDiasbleHyperV = (SpeedUpBluestacksUserControl)target;
				return;
			case 5:
				this.mPowerPlan = (SpeedUpBluestacksUserControl)target;
				return;
			case 6:
				this.mUpgradeComputer = (SpeedUpBluestacksUserControl)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000767 RID: 1895
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox CloseBtn;

		// Token: 0x04000768 RID: 1896
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SpeedUpBluestacksUserControl mEnableVt;

		// Token: 0x04000769 RID: 1897
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SpeedUpBluestacksUserControl mConfigureAntivirus;

		// Token: 0x0400076A RID: 1898
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SpeedUpBluestacksUserControl mDiasbleHyperV;

		// Token: 0x0400076B RID: 1899
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SpeedUpBluestacksUserControl mPowerPlan;

		// Token: 0x0400076C RID: 1900
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal SpeedUpBluestacksUserControl mUpgradeComputer;

		// Token: 0x0400076D RID: 1901
		private bool _contentLoaded;
	}
}
