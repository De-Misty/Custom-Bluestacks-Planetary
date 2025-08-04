using System;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000053 RID: 83
	public class FreeFireGameSettingViewModel : OtherAppGameSetting
	{
		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600045F RID: 1119 RVA: 0x00004D7C File Offset: 0x00002F7C
		// (set) Token: 0x06000460 RID: 1120 RVA: 0x00004D84 File Offset: 0x00002F84
		public bool OptimizeInGameSetting
		{
			get
			{
				return this.mOptimizeInGameSetting;
			}
			set
			{
				base.SetProperty<bool>(ref this.mOptimizeInGameSetting, value, null);
			}
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00004D95 File Offset: 0x00002F95
		public FreeFireGameSettingViewModel(MainWindow parentWindow, string appName, string packageName)
			: base(parentWindow, appName, packageName)
		{
			this.mParentWindow = parentWindow;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00004DA7 File Offset: 0x00002FA7
		public override void Init()
		{
			base.Init();
			this.OptimizeInGameSetting = this.mParentWindow.EngineInstanceRegistry.IsFreeFireInGameSettingsCustomized;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00004DC5 File Offset: 0x00002FC5
		public override void LockOriginal()
		{
			base.LockOriginal();
			this.mOldOptimizeInGameSetting = this.OptimizeInGameSetting;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00004DD9 File Offset: 0x00002FD9
		public override bool HasChanged()
		{
			return base.HasChanged() || this.OptimizeInGameSetting != this.mOldOptimizeInGameSetting;
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001BF74 File Offset: 0x0001A174
		public override bool Save(bool restartReq)
		{
			restartReq = base.Save(restartReq);
			if (this.OptimizeInGameSetting != this.mOldOptimizeInGameSetting)
			{
				this.mParentWindow.EngineInstanceRegistry.IsFreeFireInGameSettingsCustomized = this.OptimizeInGameSetting;
				GameSettingViewModel.SendGameSettingsEnabledToGuest(this.mParentWindow, this.OptimizeInGameSetting);
				GameSettingViewModel.SendGameSettingsStat(this.OptimizeInGameSetting ? "freefire_optimizegame_enabled" : "freefire_optimizegame_disabled");
			}
			return restartReq;
		}

		// Token: 0x04000269 RID: 617
		private readonly MainWindow mParentWindow;

		// Token: 0x0400026A RID: 618
		private bool mOldOptimizeInGameSetting;

		// Token: 0x0400026B RID: 619
		private bool mOptimizeInGameSetting;
	}
}
