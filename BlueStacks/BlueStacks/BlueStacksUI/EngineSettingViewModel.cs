using System;
using System.Globalization;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000044 RID: 68
	public class EngineSettingViewModel : EngineSettingBaseViewModel
	{
		// Token: 0x060003E7 RID: 999 RVA: 0x00004860 File Offset: 0x00002A60
		public EngineSettingViewModel(MainWindow owner, string vmName, EngineSettingBase engineSettingBase)
			: base(owner, vmName, engineSettingBase, false, "")
		{
			this.ParentWindow = owner;
			this._VmName = vmName;
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0001A6EC File Offset: 0x000188EC
		protected override void Save(object param)
		{
			if (base.Status == Status.Progress)
			{
				Logger.Info("Compatibility check is running");
				return;
			}
			if (!base.IsRestartRequired())
			{
				base.SaveEngineSettings("");
				base.AddToastPopupUserControl(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
				return;
			}
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				this.RestartInstanceHandler();
				this.ParentWindow.Close();
				return;
			}
			CustomMessageWindow customMessageWindow = new CustomMessageWindow();
			customMessageWindow.Owner = base.Owner;
			customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
			customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
			customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_NOW", delegate(object o, EventArgs e)
			{
				this.RestartInstanceHandler();
				BlueStacksUIUtils.RestartInstance(this._VmName);
			}, null, false, null);
			customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs e)
			{
				base.Init();
			}, null, false, null);
			customMessageWindow.ShowDialog();
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0001A7E0 File Offset: 0x000189E0
		private void RestartInstanceHandler()
		{
			string text = "";
			if (base.EngineData.ABISetting != base.ABISetting)
			{
				text = VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
				{
					"switchAbi",
					base.ABISetting.GetDescription()
				}), this._VmName);
			}
			base.SaveEngineSettings(text);
			BlueStacksUIUtils.CloseContainerWindow(base.ParentView);
		}

		// Token: 0x0400020D RID: 525
		private string _VmName;

		// Token: 0x0400020E RID: 526
		private MainWindow ParentWindow;
	}
}
