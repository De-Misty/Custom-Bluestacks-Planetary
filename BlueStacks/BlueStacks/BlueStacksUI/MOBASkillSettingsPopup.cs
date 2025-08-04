using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000076 RID: 118
	public class MOBASkillSettingsPopup : CustomPopUp, IComponentConnector
	{
		// Token: 0x060005C0 RID: 1472 RVA: 0x00005D7E File Offset: 0x00003F7E
		public MOBASkillSettingsPopup(CanvasElement canvasElement)
		{
			this.mCanvasElement = canvasElement;
			this.InitializeComponent();
			CanvasElement canvasElement2 = this.mCanvasElement;
			base.PlacementTarget = ((canvasElement2 != null) ? canvasElement2.mSkillImage : null);
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00021548 File Offset: 0x0001F748
		private void MobaSkillsRadioButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
			this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
			CustomRadioButton customRadioButton = sender as CustomRadioButton;
			customRadioButton.IsChecked = new bool?(true);
			KeymapCanvasWindow.sIsDirty = true;
			string name = customRadioButton.Name;
			string text;
			if (name != null)
			{
				if (name == "mManualSkill")
				{
					this.mCanvasElement.AssignMobaSkill(false, false);
					text = "ManualSkill";
					goto IL_00C4;
				}
				if (name == "mAutoSkill")
				{
					this.mCanvasElement.AssignMobaSkill(true, false);
					text = "AutoSkill";
					goto IL_00C4;
				}
				if (name == "mQuickSkill")
				{
					this.mCanvasElement.AssignMobaSkill(true, true);
					text = "QuickSkill";
					goto IL_00C4;
				}
			}
			this.mCanvasElement.AssignMobaSkill(true, true);
			text = "QuickSkill";
			IL_00C4:
			this.mCanvasElement.SendMOBAStats("moba_skill_changed", text);
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00005DAB File Offset: 0x00003FAB
		private void MOBASkillSettingsPopup_Opened(object sender, EventArgs e)
		{
			this.mCanvasElement.mSkillImage.IsEnabled = false;
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00005DBE File Offset: 0x00003FBE
		private void MOBASkillSettingsPopup_Closed(object sender, EventArgs e)
		{
			this.mCanvasElement.mSkillImage.IsEnabled = true;
			this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
			this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00005DF3 File Offset: 0x00003FF3
		private void HelpIcon_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
			this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = true;
			this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.StaysOpen = true;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00005E28 File Offset: 0x00004028
		private void OtherSettingsHelpIcon_MouseEnter(object sender, MouseEventArgs e)
		{
			this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
			this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = true;
			this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.StaysOpen = true;
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00005E5D File Offset: 0x0000405D
		private void StopMovementCheckbox_Checked(object sender, RoutedEventArgs e)
		{
			this.SetStopMobaDpadValue(true);
			this.mCanvasElement.SendMOBAStats("stop_moba_dpad_checked", "");
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x0002162C File Offset: 0x0001F82C
		private void SetStopMobaDpadValue(bool isChecked)
		{
			this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
			this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
			KMManager.CheckAndCreateNewScheme();
			this.mStopMovementCheckbox.IsChecked = new bool?(isChecked);
			if (this.mCanvasElement.ListActionItem.First<IMAction>().Type == KeyActionType.MOBASkill)
			{
				((MOBASkill)this.mCanvasElement.ListActionItem.First<IMAction>()).StopMOBADpad = isChecked;
			}
			KeymapCanvasWindow.sIsDirty = true;
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x00005E7B File Offset: 0x0000407B
		private void StopMovementCheckbox_Unchecked(object sender, RoutedEventArgs e)
		{
			this.SetStopMobaDpadValue(false);
			this.mCanvasElement.SendMOBAStats("stop_moba_dpad_unchecked", "");
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x000216AC File Offset: 0x0001F8AC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Bluestacks;component/keymap/uielement/mobaskillsettingspopup.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00004028 File Offset: 0x00002228
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x000216DC File Offset: 0x0001F8DC
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
				this.mMOBASkillSettingsPopup = (MOBASkillSettingsPopup)target;
				return;
			case 2:
				this.mBorder = (Border)target;
				return;
			case 3:
				this.mMaskBorder3 = (Border)target;
				return;
			case 4:
				this.mHeaderGrid = (Grid)target;
				return;
			case 5:
				this.mHelpIcon = (CustomPictureBox)target;
				this.mHelpIcon.MouseEnter += this.HelpIcon_MouseEnter;
				return;
			case 6:
				this.mQuickSkill = (CustomRadioButton)target;
				this.mQuickSkill.PreviewMouseLeftButtonDown += this.MobaSkillsRadioButton_PreviewMouseLeftButtonDown;
				return;
			case 7:
				this.mAutoSkill = (CustomRadioButton)target;
				this.mAutoSkill.PreviewMouseLeftButtonDown += this.MobaSkillsRadioButton_PreviewMouseLeftButtonDown;
				return;
			case 8:
				this.mManualSkill = (CustomRadioButton)target;
				this.mManualSkill.PreviewMouseLeftButtonDown += this.MobaSkillsRadioButton_PreviewMouseLeftButtonDown;
				return;
			case 9:
				this.mOtherSettingsGrid = (Grid)target;
				return;
			case 10:
				this.mOtherSettingsHelpIcon = (CustomPictureBox)target;
				this.mOtherSettingsHelpIcon.MouseEnter += this.OtherSettingsHelpIcon_MouseEnter;
				return;
			case 11:
				this.mStopMovementCheckbox = (CustomCheckbox)target;
				this.mStopMovementCheckbox.Checked += this.StopMovementCheckbox_Checked;
				this.mStopMovementCheckbox.Unchecked += this.StopMovementCheckbox_Unchecked;
				return;
			case 12:
				this.DownArrow = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000307 RID: 775
		private CanvasElement mCanvasElement;

		// Token: 0x04000308 RID: 776
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal MOBASkillSettingsPopup mMOBASkillSettingsPopup;

		// Token: 0x04000309 RID: 777
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mBorder;

		// Token: 0x0400030A RID: 778
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Border mMaskBorder3;

		// Token: 0x0400030B RID: 779
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mHeaderGrid;

		// Token: 0x0400030C RID: 780
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mHelpIcon;

		// Token: 0x0400030D RID: 781
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mQuickSkill;

		// Token: 0x0400030E RID: 782
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mAutoSkill;

		// Token: 0x0400030F RID: 783
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomRadioButton mManualSkill;

		// Token: 0x04000310 RID: 784
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Grid mOtherSettingsGrid;

		// Token: 0x04000311 RID: 785
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomPictureBox mOtherSettingsHelpIcon;

		// Token: 0x04000312 RID: 786
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal CustomCheckbox mStopMovementCheckbox;

		// Token: 0x04000313 RID: 787
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		internal Path DownArrow;

		// Token: 0x04000314 RID: 788
		private bool _contentLoaded;
	}
}
