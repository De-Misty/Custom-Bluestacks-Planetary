using System;
using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
	// Token: 0x02000054 RID: 84
	public class CallOfDutyGameSettingViewModel : OtherAppGameSetting
	{
		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00004DF6 File Offset: 0x00002FF6
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x00004DFE File Offset: 0x00002FFE
		public InGameResolution InGameResolution
		{
			get
			{
				return this.mInGameResolution;
			}
			set
			{
				base.SetProperty<InGameResolution>(ref this.mInGameResolution, value, null);
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00004E0F File Offset: 0x0000300F
		// (set) Token: 0x06000469 RID: 1129 RVA: 0x00004E17 File Offset: 0x00003017
		public GraphicsQuality GraphicsQuality
		{
			get
			{
				return this.mGraphicsQuality;
			}
			set
			{
				base.SetProperty<GraphicsQuality>(ref this.mGraphicsQuality, value, null);
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00004E28 File Offset: 0x00003028
		public CallOfDutyGameSettingViewModel(MainWindow parentWindow, string appName, string packageName)
			: base(parentWindow, appName, packageName)
		{
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001BFDC File Offset: 0x0001A1DC
		public override void Init()
		{
			base.Init();
			if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD) || string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD, "720", StringComparison.InvariantCultureIgnoreCase))
			{
				this.InGameResolution = InGameResolution.HD_720p;
			}
			else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD, "1080", StringComparison.InvariantCultureIgnoreCase))
			{
				this.InGameResolution = InGameResolution.FHD_1080p;
			}
			else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD, "1440", StringComparison.InvariantCultureIgnoreCase))
			{
				this.InGameResolution = InGameResolution.QHD_1440p;
			}
			else if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD, "2160", StringComparison.InvariantCultureIgnoreCase))
			{
				this.InGameResolution = InGameResolution.UHD_2160p;
			}
			if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD) || string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD, "-1", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.Auto;
				return;
			}
			if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD, "0", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.Smooth;
				return;
			}
			if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD, "1", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.Balanced;
				return;
			}
			if (string.Equals(RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD, "2", StringComparison.InvariantCultureIgnoreCase))
			{
				this.GraphicsQuality = GraphicsQuality.HD;
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00004E33 File Offset: 0x00003033
		public override void LockOriginal()
		{
			base.LockOriginal();
			this.mOldInGameResolution = this.InGameResolution;
			this.mOldGraphicsQuality = this.GraphicsQuality;
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00004E53 File Offset: 0x00003053
		public override bool HasChanged()
		{
			return base.HasChanged() || this.InGameResolution != this.mOldInGameResolution || this.GraphicsQuality != this.mOldGraphicsQuality;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001C1E0 File Offset: 0x0001A3E0
		public override bool Save(bool restartReq)
		{
			restartReq = base.Save(restartReq);
			if (this.InGameResolution != this.mOldInGameResolution)
			{
				restartReq = true;
				switch (this.InGameResolution)
				{
				case InGameResolution.HD_720p:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD = "720";
					GameSettingViewModel.SendGameSettingsStat("cod_res_720");
					break;
				case InGameResolution.FHD_1080p:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD = "1080";
					GameSettingViewModel.SendGameSettingsStat("cod_res_1080");
					break;
				case InGameResolution.QHD_1440p:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD = "1440";
					GameSettingViewModel.SendGameSettingsStat("cod_res_1440");
					break;
				case InGameResolution.UHD_2160p:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].GamingResolutionCOD = "2160";
					GameSettingViewModel.SendGameSettingsStat("cod_res_2160");
					break;
				}
			}
			if (this.GraphicsQuality != this.mOldGraphicsQuality)
			{
				restartReq = true;
				switch (this.GraphicsQuality)
				{
				case GraphicsQuality.Auto:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD = "-1";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_auto");
					break;
				case GraphicsQuality.Smooth:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD = "0";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_smooth");
					break;
				case GraphicsQuality.Balanced:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD = "1";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_balanced");
					break;
				case GraphicsQuality.HD:
					RegistryManager.Instance.Guest[base.ParentWindow.mVmName].DisplayQualityCOD = "2";
					GameSettingViewModel.SendGameSettingsStat("pubg_gfx_hd");
					break;
				}
			}
			return restartReq;
		}

		// Token: 0x0400026C RID: 620
		private InGameResolution mOldInGameResolution;

		// Token: 0x0400026D RID: 621
		private GraphicsQuality mOldGraphicsQuality;

		// Token: 0x0400026E RID: 622
		private InGameResolution mInGameResolution;

		// Token: 0x0400026F RID: 623
		private GraphicsQuality mGraphicsQuality;
	}
}
