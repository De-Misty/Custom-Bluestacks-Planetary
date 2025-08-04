using System;
using System.Windows;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000127 RID: 295
	public class DisplaySettingsControl : DisplaySettingsBase
	{
		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00009768 File Offset: 0x00007968
		// (set) Token: 0x06000BE7 RID: 3047 RVA: 0x00009770 File Offset: 0x00007970
		public MainWindow ParentWindow { get; private set; }

		// Token: 0x06000BE8 RID: 3048 RVA: 0x00009779 File Offset: 0x00007979
		public DisplaySettingsControl(MainWindow window)
			: base(window, (window != null) ? window.mVmName : null, "")
		{
			this.ParentWindow = window;
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x00042ACC File Offset: 0x00040CCC
		protected override void Save(object param)
		{
			if (FeatureManager.Instance.IsCustomUIForDMM)
			{
				base.SaveDisplaySetting();
				BlueStacksUIUtils.CloseContainerWindow(this);
				this.ParentWindow.Close();
				return;
			}
			if (base.IsDirty())
			{
				CustomMessageWindow customMessageWindow = new CustomMessageWindow();
				customMessageWindow.Owner = this.ParentWindow;
				customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
				customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
				customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_NOW", delegate(object o, EventArgs e)
				{
					base.SaveDisplaySetting();
					if (BlueStacksUIUtils.DictWindows.Count == 1)
					{
						App.defaultResolution = new Fraction((long)RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestWidth, (long)RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestHeight);
						PromotionManager.ReloadPromotionsAsync();
					}
					BlueStacksUIUtils.CloseContainerWindow(this);
					BlueStacksUIUtils.RestartInstance(base.VmName);
				}, null, false, null);
				customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", delegate(object o, EventArgs e)
				{
					base.DiscardCurrentChangingModel();
				}, null, false, null);
				customMessageWindow.ShowDialog();
			}
		}
	}
}
